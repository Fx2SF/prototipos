using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using OpenTK.Graphics.OpenGL;

namespace ContarPersonas
{
    class FrameInfo
    {
        public int Id { get; set; }
        public TimeSpan TimeStamp { get; set; } = new TimeSpan();
        public String FileName { get; set; }
    }

    class ContarPersonasMS
    {
        private ReporteHelper reporte = null;
        private ConcurrentDictionary<int, TimeSpan> reintentos = new ConcurrentDictionary<int, TimeSpan>();
        private ConcurrentDictionary<int, string> textos = new ConcurrentDictionary<int, string>();

        private int sumFaces = 0;
        private int processedFrames = 0;
        private double scaleFactor ;
        public bool ResizeThumbnails { get; set; }= false;


        public Task<bool> Send(Stream src, FrameInfo info, Action<Task<Face[]>, FrameInfo> handler)
        {
            MemoryStream copy = new MemoryStream();
            // hago una copia para poder ejecutar este metodo en fire and forget
            // sino tira ObjectDisposedException
            src.CopyTo(copy);
            copy.Seek(0, SeekOrigin.Begin);
            Debug.WriteLine("Largo: " + copy.Length);
            string key = ContarPersonas.Properties.Settings.Default.MS_Face_API_key;
            FaceServiceClient client = new FaceServiceClient(key);
            var atributes = new List<FaceAttributeType>() {FaceAttributeType.Gender};
            Task<Face[]> task = client.DetectAsync(copy, true, false, atributes);
            return task.ContinueWith(t =>
            {
                if (t.Status != TaskStatus.RanToCompletion)
                {
                    MostrarErrores(t, info);
                }
                else
                {
                    handler(t, info);
                }
                return t.Status == TaskStatus.RanToCompletion;
            });
        }

        private void MostrarErrores(Task<Face[]> task, FrameInfo info)
        {
            Debug.WriteLine("Id: " + info.Id);
            Debug.WriteLine(task.Status.ToString());
            if (task.Exception != null)
            {
                foreach (var e in task.Exception.InnerExceptions)
                {
                    var faceEx = e as FaceAPIException;
                    if (faceEx != null)
                    {
                        Debug.WriteLine("Error code {0} - {1}", faceEx.ErrorCode, faceEx.ErrorMessage);
                    }
                    else
                    {
                        Debug.Write(e.ToString());
                    }
                }
            }
        }

        private void escribir()
        {
            SortedDictionary<int, string> sorted = new SortedDictionary<int, string>(textos);
            foreach (var celda in sorted.Values)
            {
                reporte.AgregarLinea(celda);
            }
            reporte.FinalizarTabla();
            reporte.Resumen(this.sumFaces, this.processedFrames);
            reporte.Finalizar();
            Debug.WriteLine("Termino!");
        }


        void ProcesarFaces(Task<Face[]> task, FrameInfo info)
        {
            if (task.Status == TaskStatus.RanToCompletion)
            {
                Debug.WriteLine("Id: " + info.Id);
                Face[] result = task.Result;
                int totalFaces = result.Length;
                int females = result.Count(f => f.FaceAttributes.Gender.Equals("female"));
                int males = result.Count(f => f.FaceAttributes.Gender.Equals("male"));
                int otros = totalFaces - (males + females);
                string timestamp = info.TimeStamp.ToString("h\\:mm\\:ss");

                string fileName = info.FileName;
                foreach (var face in result)
                {
                    Debug.WriteLine("[{0}] {1} {2},{3}", info.Id, face.FaceAttributes.Gender, face.FaceRectangle.Top,
                        face.FaceRectangle.Left);
                }

                StringBuilder sb = new StringBuilder();
                processedFrames++;
                sumFaces += totalFaces;


                try
                {
                    using (Image<Bgr, Byte> img = new Image<Bgr, Byte>(fileName))
                    {
                        foreach (var face in result)
                        {
                            string gender = face.FaceAttributes.Gender;
                            gender = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gender);
                            FaceRectangle frec = face.FaceRectangle;
                            Rectangle rec = new Rectangle((int) (frec.Left / scaleFactor),
                                (int) (frec.Top / scaleFactor),
                                (int) (frec.Width / scaleFactor),
                                (int) (frec.Height / scaleFactor));
                            var red = new Bgr(0, 0, 255);
                            img.Draw(rec, red, 2);
                            img.Draw(gender, new Point(rec.Left, Math.Max(rec.Top - 5, 0)), FontFace.HersheyComplex, 1.25,
                                red);
                        }
                        using (Bitmap bmp = img.ToBitmap())
                        {
                            if (this.ResizeThumbnails)
                            {
                                Util.GuardarJPGToDisk(bmp, fileName, 75, 0.5);
                            }
                            else
                            {
                                Util.GuardarJPGToDisk(bmp, fileName, 60, 0.5);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                ReporteHelper rep = new ReporteHelper();
                rep.AgregarImagen(fileName, timestamp);
                rep.AgregarCeldas(info.Id, info.TimeStamp, totalFaces, males, females, otros);
                textos.TryAdd(info.Id, rep.ObtenerFila());
            }
        }


        public async Task ProcessVideoTime(string videoFileName, string reportName, TimeSpan start, TimeSpan end,
            int seconds, bool eq, Action<double> progressCallback, bool resize)
        {
            this.scaleFactor = resize ? 2.0 : 1.0;
            var framesToProcess = Math.Floor((end - start).TotalSeconds / seconds) + 1;
            int quotaTime = Properties.Settings.Default.MS_Free ? 3000 : 100;
            string directory = Path.Combine(Path.GetDirectoryName(reportName),
                Path.GetFileNameWithoutExtension(reportName) + "_img");
            Directory.CreateDirectory(directory);
            string baseName = Path.Combine(directory, "frame");
            int delay_ms = (int) (300 * (this.scaleFactor * this.scaleFactor));

            reporte = new ReporteHelper(reportName);
            reporte.IniciarTabla("Imagen", "Id Frame", "Timestamp", "Cantidad Personas", "Hombres", "Mujeres", "Otros");


            Extractor extractor = new Extractor();
            extractor.Iniciar(videoFileName);
            int id = 0;
            Stopwatch time = Stopwatch.StartNew();
            List<Task> allTasks = new List<Task>();
            for (TimeSpan pos = start; pos <= end; pos = pos.Add(TimeSpan.FromSeconds(seconds)))
            {
                id++;
                var currentPos = pos;
                var currentId = id; // para reintentar
                Task t = ProcessPosition(id, pos, baseName, extractor, eq).ContinueWith(sucess =>
                {
                    if (sucess.Result) progressCallback(processedFrames / framesToProcess);
                    else reintentos.TryAdd(currentId, currentPos);
                });
                allTasks.Add(t);
                await Task.Delay(Math.Max(delay_ms, quotaTime));
            }
            await Task.WhenAll(allTasks);
            // termino de ejecutarse las sin errores
            foreach (var key in reintentos.Keys)
            {
                // intento de vuelta, 1 por 1
                await ProcessPosition(key, reintentos[key], baseName, extractor, eq);
                progressCallback(processedFrames / framesToProcess);
            }
            escribir();
            if (processedFrames != (int) framesToProcess) progressCallback(1);
        }

        private Task<bool> ProcessPosition(int id, TimeSpan pos, string baseName, Extractor extractor, bool eq)
        {
            MemoryStream mstream = new MemoryStream();
            MemoryStream thumbStream = mstream;
            var tmpFilename = String.Format("{0}_{1:D4}s.jpg", baseName, (int) pos.TotalSeconds);
            using (Mat mat = extractor.GetMat((long) pos.TotalMilliseconds))
            {
                using (var img = mat.ToImage<Bgr, byte>())
                {
                    //img._GammaCorrect(gamma);
                    using (var sentImg = img.Resize(scaleFactor, Inter.Cubic))
                    {
                        if (eq)
                        {
                            sentImg._EqualizeHist();
                        }
                        Util.ConvertBmpIntoJPG(sentImg.Bitmap, mstream, 80);
                        //Util.GuardarStreamToDisk(mstream, tmpFilename + "_eq.jpg");
                        thumbStream = new MemoryStream();
                        Util.ConvertBmpIntoJPG(mat.Bitmap, thumbStream, 85);
                    }
                    //var sentImg2 = sentImg.Clone();
                    //var lumaChannel = sentImg[1];
                    //lumaChannel._EqualizeHist();
                    //sentImg[1] = lumaChannel;
                    //sentImg._EqualizeHist();
                    //var sentImg2 = sentImg.Convert<Bgr, byte>();
                    //CvInvoke.CLAHE(sentImg, 2.0, new Size(8, 8), sentImg2);
                }
            }
            //extractor.FrameIntoJPGStreamMs((long)pos.TotalMilliseconds, mstream);

            FrameInfo info = new FrameInfo() {Id = id, TimeStamp = pos, FileName = tmpFilename};
            var send = Send(mstream, info, ProcesarFaces); // empieza envio
            Util.GuardarStreamToDisk(thumbStream, tmpFilename);
            return send;
        }
    }
}