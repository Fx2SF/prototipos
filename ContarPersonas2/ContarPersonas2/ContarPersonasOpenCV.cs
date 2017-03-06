using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContarPersonas2;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Color = System.Drawing.Color;

namespace ContarPersonas
{
    class ContarPersonasOpenCV
    {
        private Capture _capture = null;


        private int sumFaces = 0;
        private int processedFrames = 0;
        private double resizeFactor = 1.0; // cuanto se agranda la imagen
        private double scaleFactor = 1.12; // cuanto se agranda el haarcascade en cada paso
        private bool eq = false;
        private int ttl = 8;


        //private  GenderDetector genderDetector = new GenderDetector("GeorgiaTech.fisher","GeorgiaTech.csv");
        //private GenderDetector genderDetector = new GenderDetector("Yale2.fisher", "Yale.csv");


        private Dictionary<Rectangle, int> _detections = new Dictionary<Rectangle, int>();

        public bool ResizeThumbnails { get; set; } = false;
        public bool ShowROI { get; set; } = true;




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

        private enum CascadeType { Simple, DualFace, TripleFace,FaceBody };



        private Rectangle Scale(Rectangle r,double factor)
        {
            return new Rectangle(
                x: (int) Math.Round(r.X * factor),
                y: (int) Math.Round(r.Y * factor),
                width: (int) Math.Round(r.Width * factor),
                height: (int) Math.Round(r.Height * factor)
            );
        }

        private Rectangle Scaled(Rectangle r)
        {
            return Scale(r, this.resizeFactor);
        }

        private Rectangle ScaleBack(Rectangle r)
        {
            return Scale(r, 1 / this.resizeFactor);
        }

        private void ScaleBack(IList<Rectangle> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = ScaleBack(list[i]);
            }
        }

        private Image<Gray, byte> ConvertirGris(Image<Bgr, byte> image)
        {
            return (this.resizeFactor == 1.0) ? image.Convert<Gray, Byte>() : image.Convert<Gray, Byte>().Resize(this.resizeFactor, Inter.Cubic) ;
        }

        public int RecognizeFacesTriple(Image<Bgr, byte> image,  int precision)
        {
            using (var greyFrame = ConvertirGris(image))
            {
                if (this.eq)
                {
                    greyFrame._EqualizeHist();
                }



                Size maxFaceSize = new Size((int)(150 * this.resizeFactor), (int)(150 * this.resizeFactor));
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

        private CascadeClassifier haarAlt2 = null;
        private CascadeClassifier haarProfile = null;
        private CascadeClassifier haarUpper = null;
        private HOGDescriptor hog = null;

        public void SetupCascades()
        {
            haarAlt2 = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_frontalface_alt2.xml");
            haarProfile = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_profileface.xml");
            haarUpper = new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_upperbody.xml");
            hog = new HOGDescriptor();
            hog.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());
        }

        public void DisposeCascades()
        {
            haarAlt2.Dispose();
            haarAlt2 = null;
            haarProfile.Dispose();
            haarProfile = null;
            haarUpper.Dispose();
            haarUpper = null;
        }

        public void Offset(IList<Rectangle> list, Rectangle roi)
        {
            for(int i = 0; i < list.Count; i++)
            {
                var r = list[i];
                r.Offset(roi.X, roi.Y);
                list[i] = r;
            }
        }


        public IList<Rectangle> RecognizeUpper(Image<Bgr, byte> image, int precision, IList<Rectangle> ROIs = null)
        {
            this.resizeFactor = 0.5;
            var roiExpansion = 25;
            using (var greyFrame = ConvertirGris(image))
            {
                if (true)
                {
                    greyFrame._EqualizeHist();
                }
                if (ROIs == null || ROIs.Count == 0)
                {
                    ROIs = new Rectangle[] {Rectangle.Empty};
                }
                else
                {
                    var expanded =
                        new List<Rectangle>(ROIs.Select(r => Rectangle.Inflate(r, roiExpansion, roiExpansion)));
                    expanded.AddRange(this._detections.Keys.Select(r => Rectangle.Inflate(r, 25, 25)));

                    for (int i = 0; i < 3; i++)
                    {
                        expanded = MergeRectangles(expanded, 0.3);
                    }
                    ROIs = expanded.ToArray();
                    /*var mergedRec = new Rectangle(ROIs[0].Location,ROIs[0].Size);
                    for (int i = 1; i < ROIs.Length; i++)
                    {
                        mergedRec = Rectangle.Union(mergedRec,ROIs[i]);
                    }
                    ROIs = new Rectangle[] { new Rectangle(0,0,100,100), };*/
                }
                Size maxFaceSize = new Size((int) (150 * this.resizeFactor), (int) (150 * this.resizeFactor));
                // Size maxBodySize = new Size((int)(160 * this.resizeFactor), (int)(240 * this.resizeFactor));
                List<Rectangle> allFacesAlt2 = new List<Rectangle>();
                List<Rectangle> allFacesProfile = new List<Rectangle>();
                var scaledRois = ROIs.Select(Scaled).ToArray();
                var frameRec = new Rectangle(0, 0, greyFrame.Width, greyFrame.Height);
                foreach (var roi in scaledRois)
                {
                    roi.Intersect(frameRec);
                    greyFrame.ROI = roi;

                    /*var facesAlt2 = haarAlt2.DetectMultiScale(greyFrame, this.scaleFactor, precision);
                    // corrigo por la region de interes
                    Offset(facesAlt2, roi);
                    ScaleBack(facesAlt2); // escalo de vuelta para compensar por el resize
                    allFacesAlt2.AddRange(facesAlt2);*/

                    var facesProfile = haarUpper.DetectMultiScale(greyFrame, this.scaleFactor, precision);
                    //var facesProfile = hog.DetectMultiScale(greyFrame).Select(d => d.Rect).ToArray();
                    Offset(facesProfile, roi);
                    ScaleBack(facesProfile);
                    allFacesProfile.AddRange(facesProfile);
                }


                //greyFrame.ROI = Rectangle.Empty;
                var mergedFaces = MergeRectangles(allFacesProfile.Concat(allFacesAlt2), 0.4);
                mergedFaces = MergeRectangles(mergedFaces, 0.4);
                // eliminar rectangulos con TTL = 0
                var toRemove = new HashSet<Rectangle>(_detections.Where(a => a.Value <= 1).Select(b => b.Key));
                foreach (var old in _detections.Keys)
                {
                    foreach (var nuevo in mergedFaces)
                    {
                        if (nuevo.IntersectsWith(old))
                        {
                            toRemove.Add(old);
                        }
                    }
                }
                var tmp = _detections.Where(f => !toRemove.Contains(f.Key)).ToList();
                _detections.Clear();
                tmp.ForEach(keyVal => _detections.Add(keyVal.Key, keyVal.Value - 1)); // Quita 1 al TTL
                mergedFaces.ForEach(rec => _detections.Add(rec, ttl));

                foreach (Rectangle face in mergedFaces)
                {
                    if (!allFacesAlt2.Contains(face) && !allFacesProfile.Contains(face))
                    {
                        image.Draw(face, new Bgr(Color.DarkOrange), 2);
                    }
                }

                foreach (Rectangle face in allFacesAlt2)
                {
                    image.Draw(face, new Bgr(Color.Red), 1);
                }
                foreach (Rectangle face in allFacesProfile)
                {
                    image.Draw(face, new Bgr(Color.Yellow), 1);
                }
                foreach (Rectangle face in ROIs)
                {
                    image.Draw(face, new Bgr(Color.BlueViolet), 1);
                }

                this.resizeFactor = 1;

                return mergedFaces;
            }
        }

        public IList<Rectangle> RecognizeFacesDuo(Image<Bgr, byte> image, int precision, IList<Rectangle> ROIs = null)
        {
            var roiExpansion = 25;
            var maxFace = (int)(image.Height * 0.25 * this.resizeFactor);
            Size maxFaceSize = new Size(maxFace, maxFace);
            using (var greyFrame = ConvertirGris(image))
            {
                if (true)
                {
                    greyFrame._EqualizeHist();
                }
                if (ROIs == null || ROIs.Count == 0)
                {
                    ROIs = new Rectangle[] {Rectangle.Empty};
                }
                else
                {
                    var expanded = new List<Rectangle>(ROIs.Select(r => Rectangle.Inflate(r, roiExpansion, roiExpansion)));
                    expanded.AddRange(this._detections.Keys.Select(r => Rectangle.Inflate(r,25,25)));

                    for (int i = 0; i < 3; i++)
                    {
                        expanded = MergeRectangles(expanded, 0.5);
                    }
                    ROIs = expanded.ToArray();
                    /*var mergedRec = new Rectangle(ROIs[0].Location,ROIs[0].Size);
                    for (int i = 1; i < ROIs.Length; i++)
                    {
                        mergedRec = Rectangle.Union(mergedRec,ROIs[i]);
                    }
                    ROIs = new Rectangle[] { new Rectangle(0,0,100,100), };*/
                }
                var total = 0;
                 
                // Size maxBodySize = new Size((int)(160 * this.resizeFactor), (int)(240 * this.resizeFactor));
                List<Rectangle> allFacesAlt2 = new List<Rectangle>();
                List<Rectangle> allFacesProfile = new List<Rectangle>();
                var scaledRois = ROIs.Select(Scaled).ToArray();
                var frameRec = new Rectangle(0,0,greyFrame.Width, greyFrame.Height);
                foreach (var roi in scaledRois)
                {
                    roi.Intersect(frameRec);
                    greyFrame.ROI = roi;

                    var facesAlt2 = haarAlt2.DetectMultiScale(greyFrame, this.scaleFactor, precision);
                    // corrigo por la region de interes
                    Offset(facesAlt2, roi);
                    ScaleBack(facesAlt2); // escalo de vuelta para compensar por el resize
                    allFacesAlt2.AddRange(facesAlt2);

                    var facesProfile = haarProfile.DetectMultiScale(greyFrame, this.scaleFactor, precision);
                    Offset(facesProfile, roi);
                    ScaleBack(facesProfile);
                    allFacesProfile.AddRange(facesProfile);
                } 
                

                //greyFrame.ROI = Rectangle.Empty;
                var mergedFaces = MergeRectangles(allFacesProfile.Concat(allFacesAlt2), 0.4);
                mergedFaces = MergeRectangles(mergedFaces, 0.4);
                // eliminar rectangulos con TTL = 0
                var toRemove = new HashSet<Rectangle>(_detections.Where(a => a.Value <= 1).Select(b => b.Key));
                foreach (var old in _detections.Keys)
                {
                    foreach (var nuevo in mergedFaces)
                    {
                        if (nuevo.IntersectsWith(old))
                        {
                            toRemove.Add(old);
                        }
                    }
                }
                var tmp = _detections.Where(f => !toRemove.Contains(f.Key)).ToList();
                _detections.Clear();
                tmp.ForEach(keyVal => _detections.Add(keyVal.Key, keyVal.Value - 1)); // Quita 1 al TTL
                mergedFaces.ForEach(rec => _detections.Add(rec, ttl));

                foreach (Rectangle face in mergedFaces)
                {
                    if (!allFacesAlt2.Contains(face) && !allFacesProfile.Contains(face))
                    {
                        image.Draw(face, new Bgr(Color.DarkOrange), 2);
                    }
                }

                foreach (Rectangle face in allFacesAlt2)
                {
                    image.Draw(face, new Bgr(Color.Red), 1);
                }
                foreach (Rectangle face in allFacesProfile)
                {
                    image.Draw(face, new Bgr(Color.Yellow), 1);
                }
                if (ShowROI)
                {
                    foreach (Rectangle face in ROIs)
                    {
                        image.Draw(face, new Bgr(Color.BlueViolet), 1);
                    }
                }

                total += mergedFaces.Count;



                return mergedFaces;
            }
        }

        




        public int RecognizeFacesComplex(Image<Bgr, byte> image, int precision )
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
                /*
                var haarDef =
                    new CascadeClassifier(Application.StartupPath + "/" + "haarcascade_frontalface_default.xml");
                Rectangle[] facesDef = haarDef.DetectMultiScale(greyFrame, scale, 5, maxSize: maxFaceSize);
                Array.ForEach(facesDef, ScaleBack);
                haarDef.Dispose();*/


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

                /*foreach (Rectangle face in facesAlt2)
                {
                    image.Draw(face, new Bgr(Color.Red), 1);
                }
                foreach (Rectangle face in facesProfile)
                {
                    image.Draw(face, new Bgr(Color.DarkOrange), 1);
                }

                foreach (Rectangle face in facesFull)
                {
                    image.Draw(face, new Bgr(Color.Lime), 1);
                }
                foreach (Rectangle face in facesUpper)
                {
                    image.Draw(face, new Bgr(Color.Cyan), 1);
                }*/

                return mergedFaces.Count + mergedBodies.Count + mergedAll.Count;
            }
        }

        public int RecognizeFaces(int precision, Image<Bgr, byte> image, CascadeClassifier cascade)
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