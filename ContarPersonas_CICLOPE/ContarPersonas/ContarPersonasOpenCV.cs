using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Google.Apis.Vision.v1.Data;
using Color = System.Drawing.Color;

namespace ContarPersonas
{
    class ContarPersonasOpenCV
    {
        private Capture _capture = null;


        private int sumFaces = 0;
        private int processedFrames = 0;
        private double resizeFactor = 1.0; // cuanto se agranda la imagen
        private double scaleFactor = 1.07; // cuanto se agranda el haarcascade en cada paso
        private bool eq = true;

        public bool ResizeThumbnails { get; set; } = false;




        private Rectangle MergeRec(Rectangle r1, Rectangle r2)
        {
            int min_X = Math.Min(r1.Left, r2.Left);
            int max_X = Math.Max(r1.Right, r2.Right);
            int min_Y = Math.Min(r1.Top, r2.Top);
            int max_Y = Math.Max(r1.Bottom, r2.Bottom);
            Rectangle merged = new Rectangle(min_X, min_Y, max_X - min_X, max_Y - min_Y);
            return merged;
        }


        private List<Rectangle> MergeRectangles(IEnumerable<Rectangle> input,double minArea)
        {
            var list = new List<Rectangle>(input);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Rectangle r1 = list[i];
                for (int j = list.Count - 1; j >= 0; j--)
                {
                    Rectangle r2 = list[j];
                    if (i != j && r1.IntersectsWith(r2))
                    {
                        Rectangle intersection = Rectangle.Intersect(r1, r2);
                        double areaInter = intersection.Height * intersection.Width;

                        double areaR2_R1 = areaInter / (double)(r1.Height * r1.Width);
                        double areaR1_R2 = areaInter / (double)(r2.Height * r2.Width);
                        if (areaR1_R2 >= minArea || areaR2_R1 >= minArea)
                        {

                            // elimino los originales
                            // es probable que el algoritmo no sea estable ante cambios en el orden
                            list[j] = Rectangle.Union(r1,r2);
                            list.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            return list;
        }

        private List<Rectangle> MergeUpper(List<Rectangle> bodyDetections, List<Rectangle> faceDetections,double heightTolerance,double minArea)
        {
            var bodiesToRemove = new HashSet<Rectangle>();
            var facesToRemove = new HashSet<Rectangle>();
            var mergedRectangles = new List<Rectangle>();
            foreach (var r1 in bodyDetections)
            {
                var recAumentado = new Rectangle(r1.X, (int) Math.Max(r1.Y - r1.Height * heightTolerance, 0),
                    r1.Width, (int) (r1.Height * (1 + heightTolerance)));
                foreach (var r2 in faceDetections)
                {
                    if (!recAumentado.IntersectsWith(r2)) continue;
                    // si el centro de la detección de cara está debajo del 75% de la detección del cuerpo, seguramente no correspondan
                    if ((r2.Top + 0.5 * r2.Height) > (r1.Top + r1.Height * 0.75) ) continue;     
                    Rectangle intersection = Rectangle.Intersect(recAumentado, r2);

                    double areaInter = intersection.Height * intersection.Width;
                    double areaR2_in_R1 = areaInter / (double)(recAumentado.Height * recAumentado.Width);
                    double areaR1_in_R2 = areaInter / (double)(r2.Height * r2.Width);
                    double areaR2_divR1 = (r2.Width * r2.Height) / (double) (r1.Width * r1.Height);
                    if (areaR2_divR1 >= 0.1 || r1.Contains(r2))
                    {
                        int intersectionWidth = Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left);
                        double proportion = intersectionWidth / (double)r2.Width;
                        if (areaR1_in_R2 >= minArea || areaR2_in_R1 >= minArea)
                        {
                            mergedRectangles.Add(Rectangle.Union(r1, r2));
                            bodiesToRemove.Add(r1);
                            facesToRemove.Add(r2);
                        }
                    }

                    
                }
            }
            bodyDetections.RemoveAll(r => bodiesToRemove.Contains(r));
            faceDetections.RemoveAll(r => facesToRemove.Contains(r));
            return mergedRectangles;
        }

        private enum CascadeType { Simple, DualFace, TripleFace,FaceBody, HOG };

        public async Task ProcessVideoTime(string videoFileName, string reportName, TimeSpan start, TimeSpan end,
            int seconds, Action<double> progressCallback, int precision,
            string cascadeFile,bool eq, double cascadeScale = 1.07)
        {
            Stopwatch swProcess = Stopwatch.StartNew();
            this.scaleFactor = cascadeScale;
            this.eq = eq;
            var framesToProcess = Math.Floor((end - start).TotalSeconds / seconds) + 1;
            string directory = Path.Combine(Path.GetDirectoryName(reportName),
                Path.GetFileNameWithoutExtension(reportName) + "_img");
            Directory.CreateDirectory(directory);
            string baseName = Path.Combine(directory, "frame");
            Extractor extractor = new Extractor();
            extractor.Iniciar(videoFileName);

            ReporteHelper reporte = new ReporteHelper();
            reporte.Iniciar(reportName);
            reporte.IniciarTabla("Imagen", "Id Frame", "Timestamp", "Cantidad Personas");

            int id = 0;
            CascadeType type = CascadeType.Simple;
            if(cascadeFile.Equals("Face+Body")) type = CascadeType.FaceBody;
            else if (cascadeFile.Equals("Profile+Alt2")) type = CascadeType.DualFace;
            else if (cascadeFile.Equals("Profile+Alt2+Def")) type = CascadeType.TripleFace;
            else if (cascadeFile.Equals("HOG")) type = CascadeType.HOG;
            CascadeClassifier cascade = null;
            try
            {
                _capture = new Capture(videoFileName);
                if (type == CascadeType.Simple)
                {
                    cascade = new CascadeClassifier(Application.StartupPath + "/" + cascadeFile);
                }

                for (TimeSpan pos = start; pos <= end; pos = pos.Add(TimeSpan.FromSeconds(seconds)))
                {
                    await Task.Run(() =>
                    {
                        Stopwatch time = Stopwatch.StartNew();
                        id++;
                        _capture.SetCaptureProperty(CapProp.PosMsec, pos.TotalMilliseconds);
                        using (var image = _capture.QueryFrame().Clone().ToImage<Bgr, Byte>())
                        {
                            Debug.WriteLine("query " + time.ElapsedMilliseconds + " ms");

                            var jpgFilename = String.Format("{0}_{1:D4}s.jpg", baseName, (int) pos.TotalSeconds);

                            var totalFaces = 0;
                            if (type == CascadeType.FaceBody) totalFaces = RecognizeFacesComplex(image,  precision);
                            else if (type == CascadeType.DualFace) totalFaces = RecognizeFacesDuo(image,  precision);
                            else if (type == CascadeType.TripleFace) totalFaces = RecognizeFacesTriple(image, precision);
                            else if (type == CascadeType.HOG) totalFaces = RecognizeFacesHOG(image,precision);
                            else totalFaces = RecognizeFaces(precision, image, cascade);

                            string timestamp = pos.ToString("h\\:mm\\:ss");

                            reporte.AgregarImagen(jpgFilename,timestamp);
                            reporte.AgregarCeldas(id, pos, totalFaces);
                            reporte.FinalizarFila();
                            if (this.ResizeThumbnails)
                            {
                                using (var resized = image.Resize(0.5, Inter.Cubic))
                                {
                                    Util.GuardarJPGToDisk(resized.Bitmap, jpgFilename, 75);
                                }  
                            }
                            else
                            {
                                Util.GuardarJPGToDisk(image.Bitmap, jpgFilename, 60);
                            }

                            processedFrames++;
                            sumFaces += totalFaces;
                        }

                        Debug.WriteLine("took " + time.ElapsedMilliseconds + " ms");
                    });
                    progressCallback(id / framesToProcess);
                }
                reporte.FinalizarTabla();
                reporte.Resumen(this.sumFaces, this.processedFrames);
            }
            finally
            {
                reporte.Finalizar();
                cascade?.Dispose();
                _capture.Stop();
                _capture.Dispose();
                Debug.WriteLine("Termino");
            }
            Debug.WriteLine("El procesamiento tomo " + swProcess.ElapsedMilliseconds / 1000.0 + " s");
        }


        private Rectangle Scale(Rectangle r, double factor)
        {
            return new Rectangle(
                x: (int)Math.Round(r.X * factor),
                y: (int)Math.Round(r.Y * factor),
                width: (int)Math.Round(r.Width * factor),
                height: (int)Math.Round(r.Height * factor)
            );
        }

        private void ScaleBack(IList<Rectangle> list)
        {
            var factor = 1 / this.resizeFactor;
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = Scale(list[i],factor);
            }
        }

     

        private Image<Gray, byte> ConvertirGris(Image<Bgr, byte> image)
        {
            return (this.resizeFactor == 1.0) ? image.Convert<Gray, Byte>() : image.Convert<Gray, Byte>().Resize(this.resizeFactor, Inter.Cubic) ;
        }

        private int RecognizeFacesHOG(Image<Bgr, byte> image,int precision)
        {
            // nota: no se aplico escala
            HOGDescriptor hog = new HOGDescriptor();
            hog.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());
            var detections = hog.DetectMultiScale(image.Mat,finalThreshold:precision);
            foreach (var face in detections)
            {
                var rec = face.Rect;
                image.Draw(face.Rect, new Bgr(Color.Red), 1);
                image.Draw(String.Format("{0:n3}",face.Score), new Point(rec.Left, Math.Max(rec.Top - 5, 0)), FontFace.HersheyComplex, 1.0,
    new Bgr(Color.Red));
            }
            return detections.Length;

        }

        private int RecognizeFacesTriple(Image<Bgr, byte> image,  int precision)
        {
            using (var greyFrame = ConvertirGris(image))
            {
                if (this.eq)
                {
                    greyFrame._EqualizeHist();
                }



                Size maxFaceSize = new Size((int)(100 * this.resizeFactor), (int)(100 * this.resizeFactor));
                Size maxBodySize = new Size((int)(160 * this.resizeFactor), (int)(240 * this.resizeFactor));

                // haarcascade ya esta paralelizado, no tiene sentido correrlo en paralelo
                var haarAlt2 = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_frontalface_alt2.xml");
                Rectangle[] facesAlt2 = haarAlt2.DetectMultiScale(greyFrame, this.scaleFactor, precision);
                ScaleBack(facesAlt2);
                haarAlt2.Dispose();
                
                var haarDef =
                    new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_frontalface_default.xml");
                Rectangle[] facesDef = haarDef.DetectMultiScale(greyFrame, this.scaleFactor, precision);
                ScaleBack(facesDef);
                haarDef.Dispose();


                var haarProfile = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_profileface.xml");
                Rectangle[] facesProfile = haarProfile.DetectMultiScale(greyFrame, this.scaleFactor, precision);
                ScaleBack(facesProfile);
                haarProfile.Dispose();



                var mergedFaces = MergeRectangles(facesProfile.Concat(facesAlt2), 0.4);
                mergedFaces = MergeRectangles(mergedFaces, 0.4);



                //Util.GuardarJPGToDisk(greyFrame.Bitmap, filename + "_grey.jpg", 70);

                foreach (Rectangle face in mergedFaces)
                {
                    if (!facesAlt2.Contains(face) && !facesProfile.Contains(face))
                    {
                        //image.Draw(face, new Bgr(Color.DarkOrange), 2);
                    }
                }

                foreach (Rectangle face in facesAlt2)
                {
                    image.Draw(face, new Bgr(Color.Red), 1);
                }
                foreach (Rectangle face in facesProfile)
                {
                    image.Draw(face, new Bgr(Color.Yellow), 1);
                }
                foreach (Rectangle face in facesDef)
                {
                    image.Draw(face, new Bgr(Color.Aqua), 1);
                }


                return mergedFaces.Count;
            }
        }

        private int RecognizeFacesDuo(Image<Bgr, byte> image,  int precision)
        {
            using (var greyFrame = ConvertirGris(image))
            {
                if (this.eq)
                {
                    greyFrame._EqualizeHist();
                }



                Size maxFaceSize = new Size((int)(100 * this.resizeFactor), (int)(100 * this.resizeFactor));
                Size maxBodySize = new Size((int)(160 * this.resizeFactor), (int)(240 * this.resizeFactor));

                // haarcascade ya esta paralelizado, no tiene sentido correrlo en paralelo
                var haarAlt2 = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_frontalface_alt2.xml");
                Rectangle[] facesAlt2 = haarAlt2.DetectMultiScale(greyFrame, this.scaleFactor, precision);
                ScaleBack(facesAlt2);
                haarAlt2.Dispose();



                var haarProfile = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_profileface.xml");
                Rectangle[] facesProfile = haarProfile.DetectMultiScale(greyFrame, this.scaleFactor, precision);
                ScaleBack(facesProfile);
                haarProfile.Dispose();



                var mergedFaces = MergeRectangles(facesProfile.Concat(facesAlt2), 0.4);
                mergedFaces = MergeRectangles(mergedFaces, 0.4);



                //Util.GuardarJPGToDisk(greyFrame.Bitmap, filename + "_grey.jpg", 70);

                foreach (Rectangle face in mergedFaces)
                {
                    if (!facesAlt2.Contains(face) && !facesProfile.Contains(face))
                    {
                        image.Draw(face, new Bgr(Color.DarkOrange), 2);
                    }
                }

                foreach (Rectangle face in facesAlt2)
                {
                    image.Draw(face, new Bgr(Color.Red), 1);
                }
                foreach (Rectangle face in facesProfile)
                {
                    image.Draw(face, new Bgr(Color.Yellow), 1);
                }


                return mergedFaces.Count;
            }
        }


        private int RecognizeFacesComplex(Image<Bgr, byte> image,  int precision )
        {
            using (var greyFrame = ConvertirGris(image))
            {
                if (this.eq)
                {
                    greyFrame._EqualizeHist();
                }



                Size maxFaceSize = new Size((int)(100 * this.resizeFactor), (int)(100 * this.resizeFactor));
                Size maxBodySize = new Size((int)(160 * this.resizeFactor), (int)(240 * this.resizeFactor));

                // haarcascade ya esta paralelizado, no tiene sentido correrlo en paralelo
                var haarAlt2 = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_frontalface_alt2.xml");
                Rectangle[] facesAlt2 = haarAlt2.DetectMultiScale(greyFrame, this.scaleFactor, precision);
                ScaleBack(facesAlt2);
                haarAlt2.Dispose();



                var haarProfile = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_profileface.xml");
                Rectangle[] facesProfile = haarProfile.DetectMultiScale(greyFrame, this.scaleFactor, Math.Max(precision - 1,1),
                    maxSize: maxFaceSize);
                ScaleBack(facesProfile);
                haarProfile.Dispose();


                var haarUpper = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_upperbody.xml");
                Rectangle[] facesUpper = haarUpper.DetectMultiScale(greyFrame, this.scaleFactor, precision, maxSize: maxBodySize );
                ScaleBack(facesUpper);
                haarUpper.Dispose();

                var haarFull = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_fullbody.xml");
                Rectangle[] facesFull = haarFull.DetectMultiScale(greyFrame, this.scaleFactor, precision + 1, maxSize: maxBodySize);
                ScaleBack(facesFull);
                haarFull.Dispose();


                var mergedFaces = MergeRectangles(facesProfile.Concat(facesAlt2),0.4);
                mergedFaces = MergeRectangles(mergedFaces, 0.4);
                var mergedBodies = MergeRectangles(facesFull.Concat(facesUpper),0.5);
                mergedBodies = MergeRectangles(mergedBodies, 0.5);
                var mergedAll = MergeUpper(mergedBodies, mergedFaces, 0.25,0.4);


                //Util.GuardarJPGToDisk(greyFrame.Bitmap, filename + "_grey.jpg", 70);

                foreach (Rectangle face in mergedFaces)
                {
                    image.Draw(face, new Bgr(Color.DarkOrange), 2);
                }
                foreach (Rectangle face in mergedBodies)
                {
                    image.Draw(face, new Bgr(Color.Blue), 2);
                }
                foreach (Rectangle face in mergedAll)
                {
                    image.Draw(face, new Bgr(Color.Lime), 2);
                }


                return mergedFaces.Count + mergedBodies.Count + mergedAll.Count;
            }
        }

        private int RecognizeFaces(int precision, Image<Bgr, byte> image, CascadeClassifier cascade)
        {
            using (var greyFrame = ConvertirGris(image))
            {
                if (this.eq)
                {
                    greyFrame._EqualizeHist();
                }


                int minNeighboors = precision;
                // cascade ya esta paralelizado, no tiene sentido correrlo en paralelo
                Rectangle[] faces = cascade.DetectMultiScale(greyFrame, this.scaleFactor, minNeighboors);


                var red = new Bgr(0, 0, 255);
                foreach (Rectangle face in faces)
                {
                    image.Draw(face, red, 2);
                }
                return faces.Length;
            }
        }
    }
}