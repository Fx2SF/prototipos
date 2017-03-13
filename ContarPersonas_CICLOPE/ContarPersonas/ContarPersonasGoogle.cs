using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;


namespace ContarPersonas
{
    class ContarPersonasGoogle
    {
        private int sumFaces = 0;
        private int processedFrames = 0;

        private ReporteHelper reporte = null;

        private double scaleFactor;
        public bool ResizeThumbnails { get; set; } = false;

        public static GoogleCredential CreateCredentials()
        {
            string json = Properties.Settings.Default.Key_Google;

            string path = json;
            GoogleCredential credential;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var c = GoogleCredential.FromStream(stream);
                credential = c.CreateScoped(VisionService.Scope.CloudPlatform);
            }

            return credential;
        }

        public static VisionService CreateService(string applicationName,
            IConfigurableHttpClientInitializer credentials)
        {
            var service = new VisionService(new BaseClientService.Initializer()
            {
                ApplicationName = applicationName,
                HttpClientInitializer = credentials
            });

            return service;
        }

        private static AnnotateImageRequest CreateAnnotationImageRequest(byte[] bytes, string[] featureTypes)
        {
            /*if (!File.Exists(path))
            {
                throw new FileNotFoundException("Archivo no encontrado.", path);
            }*/

            var request = new AnnotateImageRequest();
            request.Image = new Google.Apis.Vision.v1.Data.Image();

            //var bytes = File.ReadAllBytes(path);
            request.Image.Content = Convert.ToBase64String(bytes);

            request.Features = new List<Feature>();

            foreach (var featureType in featureTypes)
            {
                request.Features.Add(new Feature() {Type = featureType});
            }
            return request;
        }

        public static async Task<AnnotateImageResponse> AnnotateAsync(VisionService service, MemoryStream stream,
            params string[] features)
        {
            byte[] bytes = stream.ToArray();
            var request = new BatchAnnotateImagesRequest();
            request.Requests = new List<AnnotateImageRequest>();
            request.Requests.Add(CreateAnnotationImageRequest(bytes, features));

            var result = await service.Images.Annotate(request).ExecuteAsync();

            if (result?.Responses?.Count > 0)
            {
                return result.Responses[0];
            }
            return null;
        }


        public async Task ProcessVideo(string videoFileName, string reportName, TimeSpan start, TimeSpan end,
            int seconds, bool eq, Action<double> progressCallback, bool resize)
        {
            this.scaleFactor = resize ? 2.0 : 1.0;
            //crear servicio
            var credentails = CreateCredentials();
            var service = CreateService("CICLOPE", credentails);
            string[] features = new string[] {"FACE_DETECTION"};

            string directory = Path.Combine(Path.GetDirectoryName(reportName),
                Path.GetFileNameWithoutExtension(reportName) + "_img");
            Directory.CreateDirectory(directory);
            string baseName = Path.Combine(directory, "frame");

            var framesToProcess = Math.Floor((end - start).TotalSeconds / seconds) + 1;

            Extractor extractor = new Extractor();
            extractor.Iniciar(videoFileName);

            int delay_ms = (int) (600 * (this.scaleFactor * this.scaleFactor));

            reporte = new ReporteHelper(reportName);
            reporte.IniciarTabla("Imagen", "Id Frame", "Timestamp", "Cantidad Personas");

            int id = 0;
            var allTasks = new List<Task<AnnotateImageResponse>>();
            var allInfo = new List<FrameInfo>();
            for (TimeSpan pos = start; pos <= end; pos = pos.Add(TimeSpan.FromSeconds(seconds)))
            {
                Stopwatch sw = Stopwatch.StartNew();
                id++;
                var tmpFilename = baseName + "_" + (int) pos.TotalSeconds + "s.jpg";

                MemoryStream mstream = new MemoryStream();
                //MemoryStream thumbStream = mstream;
                using (Mat mat = extractor.GetMat((long) pos.TotalMilliseconds))
                {
                    if (true)
                    {
                        using (var img = mat.ToImage<Bgr, byte>())
                        {
                            using (var sentImg = img.Resize(scaleFactor, Inter.Cubic))
                            {
                                if (eq)
                                {
                                    sentImg._EqualizeHist();
                                }
                                Util.ConvertBmpIntoJPG(sentImg.Bitmap, mstream, this.scaleFactor > 1 ? 80 : 85);
                                //Util.GuardarStreamToDisk(mstream, tmpFilename + "_eq.jpg");
                                Util.GuardarJPGToDisk(mat.Bitmap, tmpFilename, 60);
                            }
                        }
                    }
                }

                FrameInfo info = new FrameInfo() {Id = id, TimeStamp = pos, FileName = tmpFilename};
                Debug.WriteLine("pre - " + sw.ElapsedMilliseconds + " ms");
                Stopwatch swTask = Stopwatch.StartNew();
                var task = AnnotateAsync(service, mstream, features);
                var t2 = task.ContinueWith(t =>
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        progressCallback(info.Id / framesToProcess);
                    }
                    Debug.WriteLine(info.Id + " - " + t.Status + " took: " + swTask.ElapsedMilliseconds);
                    swTask.Stop();
                    if (t.Exception != null)
                    {
                        foreach (var e in t.Exception.InnerExceptions)
                        {
                            Debug.WriteLine(e.Message);
                            if (e.InnerException != null) Debug.WriteLine(e.InnerException.Message);
                        }
                    }
                });

                allTasks.Add(task);
                allInfo.Add(info);

                await Task.Delay(delay_ms); // para evitar sobrecargarse


                Debug.WriteLine(sw.ElapsedMilliseconds + " ms");
            }
            await Task.WhenAll(allTasks);
            for (int i = 0; i < allTasks.Count; i++)
            {
                Debug.WriteLine("Id: " + allInfo[i].Id);
                if (allTasks[i].Status == TaskStatus.RanToCompletion)
                {
                    ProcessResult(allTasks[i].Result, allInfo[i]);
                }
                else
                {
                    Debug.WriteLine("Status: " + allTasks[i].Status);
                }
            }
            reporte.FinalizarTabla();
            reporte.ResumenFinalizar(this.sumFaces, this.processedFrames);

            Debug.WriteLine("Termino");
        }


        public void ProcessResult(AnnotateImageResponse response, FrameInfo info)
        {
            string timestamp = info.TimeStamp.ToString("h\\:mm\\:ss");
            Debug.WriteLine(timestamp);
            if (response.FaceAnnotations == null && response.Error?.Message != null)
            {
                Debug.WriteLine("[{0}] Error: {1}", timestamp, response.Error);
                return;
            }

            int totalFaces = response.FaceAnnotations?.Count ?? 0;

            reporte.AgregarImagen(info.FileName, timestamp);
            reporte.AgregarCeldas(info.Id, info.TimeStamp, totalFaces);
            reporte.FinalizarFila();

            processedFrames++;
            sumFaces += totalFaces;
            if (totalFaces > 0)
            {
                try
                {
                    foreach (var face in response.FaceAnnotations)
                    {
                        var vertices = face.BoundingPoly.Vertices;
                        Debug.WriteLine("[{0}] {1} ,{2}", info.Id, vertices[0].X, vertices[0].Y);
                    }
                    using (Image<Bgr, Byte> img = new Image<Bgr, Byte>(info.FileName))
                    {
                        using (Bitmap bmp = img.ToBitmap())
                        {
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                foreach (var face in response.FaceAnnotations)
                                {
                                    var vertices = face.BoundingPoly.Vertices.Select(
                                        (vertex) =>
                                            new Point((int) ((vertex.X ?? 0) / scaleFactor),
                                                (int) ((vertex.Y ?? 0) / scaleFactor))).ToArray();
                                    Pen pen = new Pen(System.Drawing.Color.Red, 2);
                                    pen.Width = 2;
                                    g.DrawPolygon(pen, vertices);
                                }
                                g.Dispose();
                            }
                            if (this.ResizeThumbnails)
                            {
                                Util.GuardarJPGToDisk(bmp, info.FileName, 75, 0.5);
                            }
                            else
                            {
                                Util.GuardarJPGToDisk(bmp, info.FileName, 60);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}