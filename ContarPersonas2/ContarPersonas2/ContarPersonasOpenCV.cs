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
        private double resizeFactor = 0.75; // cuanto se agranda o reduce la imagen
        private double scaleFactor = 1.12; // cuanto se agranda el haarcascade en cada paso
        private bool eq = false;



        public double ResizeFactorBodies { get; set; }= 0.5;
        public double ResizeFactorFaces { get; set; } = 1.0;

 
        public bool ShowROI { get; set; } = true;
        // si mostrar en color distinto las detecciones de frente 
        public bool ColorHaarCascade = false;

        // cauntos frames el lugar de una deteccion es candidato a otra si se usa ROI
        private int ttl = 8;
        // TTL por rectangulo para candidato ROI 
        private Dictionary<Rectangle, int> _detections = new Dictionary<Rectangle, int>();

        // cuanto tiempo mantener una deteccion 
        public int DetectionKeepTime = 1;
        // TTL por rectangulo para mantener deteccion 
        private Dictionary<Rectangle, int> _keep = new Dictionary<Rectangle, int>();




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
            // establece el patron de personas por defecto
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


        public IList<Rectangle> RecognizeHOG(Image<Bgr, byte> image,int precision)
        {
            this.resizeFactor = this.ResizeFactorBodies;
            var roiExpansion = 25;
            Rectangle[] bodies = new Rectangle[0];
            using (var greyFrame = ConvertirGris(image))
            {
                if (this.eq)
                {
                    greyFrame._EqualizeHist();
                }
                try
                {
                    bodies = hog.DetectMultiScale(greyFrame,finalThreshold: precision).Select(d => d.Rect).ToArray();
                    ScaleBack(bodies);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            var allBodies = UpdateTTLDict(_keep, bodies);
            foreach (Rectangle face in allBodies)
            {
                image.Draw(face, new Bgr(Color.Red), 2);
            }

            return allBodies;

        }

        private List<Rectangle> UpdateTTLDict(Dictionary<Rectangle,int> dict, IList<Rectangle> newRecs)
        {
            // eliminar rectangulos con TTL = 0
            var toRemove = new HashSet<Rectangle>(dict.Where(a => a.Value <= 1).Select(b => b.Key));
            foreach (var old in dict.Keys)
            {
                foreach (var nuevo in newRecs)
                {
                    if (nuevo.IntersectsWith(old))
                    {
                        toRemove.Add(old);
                    }
                }
            }
            var tmp = dict.Where(f => !toRemove.Contains(f.Key)).ToList();
            dict.Clear();
            tmp.ForEach(keyVal => dict.Add(keyVal.Key, keyVal.Value - 1)); // Quita 1 al TTL
            foreach (var rec in newRecs)
            {
                dict.Add(rec, ttl);
            }
            return new List<Rectangle>(dict.Keys);
        }

        public IList<Rectangle> RecognizeUpper(Image<Bgr, byte> image, int precision, IList<Rectangle> ROIs = null)
        {
            this.resizeFactor = this.ResizeFactorBodies;
            var roiExpansion = 50;
            using (var greyFrame = ConvertirGris(image))
            {
                if (this.eq)
                {
                    greyFrame._EqualizeHist();
                }
                if (ROIs == null)
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
                }
                //Size maxFaceSize = new Size((int) (150 * this.resizeFactor), (int) (150 * this.resizeFactor));
                // Size maxBodySize = new Size((int)(160 * this.resizeFactor), (int)(240 * this.resizeFactor));

                List<Rectangle> allBodies = new List<Rectangle>();
                var scaledRois = ROIs.Select(Scaled).ToArray();
                var frameRec = new Rectangle(0, 0, greyFrame.Width, greyFrame.Height);
                foreach (var roi in scaledRois)
                {
                    roi.Intersect(frameRec);
                    greyFrame.ROI = roi;


                    try
                    {
                        var bodies = haarUpper.DetectMultiScale(greyFrame, this.scaleFactor, precision);

                        Offset(bodies, roi);
                        ScaleBack(bodies);
                        allBodies.AddRange(bodies);
                        Debug.WriteLine(" Rect: [{0},{1},{2},{3}]", roi.Left, roi.Top, roi.Width, roi.Height);
                    }
                    catch (Exception ex)
                    {

                        
                    }
                }
                UpdateTTLDict(_detections, allBodies);
                allBodies = UpdateTTLDict(_keep, allBodies);

                foreach (Rectangle face in allBodies)
                {
                    image.Draw(face, new Bgr(Color.Red), 2);
                }

                if (ShowROI)
                {
                    foreach (Rectangle face in ROIs)
                    {
                        image.Draw(face, new Bgr(Color.BlueViolet), 1);
                    }
                }


                return allBodies;
            }
        }

        public IList<Rectangle> RecognizeFacesDuo(Image<Bgr, byte> image, int precision, IList<Rectangle> ROIs = null)
        {
            this.resizeFactor = ResizeFactorFaces;
            var roiExpansion = 25;
            var maxFace = (int)(image.Height * 0.25 * this.resizeFactor);
            Size maxFaceSize = new Size(maxFace, maxFace);
            using (var greyFrame = ConvertirGris(image))
            {
                if (true)
                {
                    greyFrame._EqualizeHist();
                }
                if (ROIs == null )
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
                }                 
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

                UpdateTTLDict(_detections, mergedFaces);

                mergedFaces = UpdateTTLDict(_keep, mergedFaces);

                foreach (Rectangle face in mergedFaces)
                {
                    // no dibujar si la cara esta exactamente en alt2 o profile y ColorHaarCascade es verdadero
                    if (this.ColorHaarCascade && (allFacesAlt2.Contains(face) || allFacesProfile.Contains(face))) continue;;
                    
                    image.Draw(face, new Bgr(Color.DarkOrange), 2);
                }
                if (this.ColorHaarCascade)
                {
                    foreach (Rectangle face in allFacesAlt2)
                    {
                        image.Draw(face, new Bgr(Color.Red), 1);
                    }
                    foreach (Rectangle face in allFacesProfile)
                    {
                        image.Draw(face, new Bgr(Color.Yellow), 1);
                    }
                }

                if (ShowROI)
                {
                    foreach (Rectangle face in ROIs)
                    {
                        image.Draw(face, new Bgr(Color.BlueViolet), 1);
                    }
                }
 
                return mergedFaces;
            }
        }

 
    }
}