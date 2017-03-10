using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;

namespace ContarPersonas2
{
    class GenderDetector : IDisposable
    {
        // http://docs.opencv.org/3.0-beta/modules/face/doc/facerec/tutorial/facerec_gender_classification.html

        private FisherFaceRecognizer _fisher;
        private Config _config;


        public GenderDetector(string modelFile,string configFile)
        {
            _fisher = new FisherFaceRecognizer();
            _fisher.Load(modelFile);
            ReadConfig(configFile);

        }



        public class GenderPrediction
        {
            // Etiqueta luego de la correccion
            public GenderLabel Gender { get; set; }

            /// <summary>
            /// Distancia desde borde entre Male y Female (corregido por el bias del modelo)
            /// Es una confidencia, a mayor distancia mayor seguridad
            /// </summary>
            public double Distance { get; set; }
            /// <summary>
            /// Genero y distancia combinados, positivo para Female y negativo para Male
            /// Para hacer cuentas
            /// </summary>
            public double GenderDistance { get; set; }

            /// <summary>
            /// Indica si la prediccion es considerada imprecisa en función a que la distancia
            /// sea menor a al valor indicado en el modelo para el label
            /// </summary>
            public bool IsUncertain { get; set; }

            /// <summary>
            /// Rectangulo usado para la prediccion
            /// </summary>
            public Rectangle Area { get; set; }

            /// <summary>
            /// Label obtenido del predictor (sin correccion por bias)
            /// </summary>
            public GenderLabel UncorrectedGender { get; set; }

            /// <summary>
            /// Label obtenido del predictor (sin correccion por bias)
            /// </summary>
            public double UncorrectedDistance { get; set; }
        }

        public enum GenderLabel
        {
            Male = 0,
            Female = 1,
        }

        public class Config
        {
            // el tamaño de las imagenes de prueba.
            // se debera convertir las imagenes a clasificar a este tamaño
            public Size Size { get; set; } 

            // el genero hacia el cual tiende a detectar de mas (0 o 1)
            public int LabelBias { get; set; }

            /*
                correccion de la distancia, si LabelBias es 0, entonces si la prediccion
                es 0 se le resta la corrección, si es 1 se le suma la correccion.
                Si la distancia despues de la correccion es negativa se pasa para la otra clase.
            */
            public double Offset { get; set; }

            //  las distancias menores a este valor (corregido por bias) para la clase indice se consideran Uncertain
            public double[] Uncertain { get;} = {0.0,0.0};
            public double MarginX { get; set; } = 0.0;
            public double MarginY { get; set; } = 0.0;

        }

        public void UpdatePrediction(GenderPrediction p, double newGenderDistance)
        {
            p.GenderDistance = newGenderDistance;
            p.Gender = newGenderDistance >= 0 ? GenderLabel.Female : GenderLabel.Male;
            p.Distance = Math.Abs(newGenderDistance);
            p.IsUncertain = p.Distance < _config.Uncertain[(int) p.Gender];
        }

        private GenderPrediction Correct(FaceRecognizer.PredictionResult prediction, Rectangle roi,
            GenderDetector.Config config)
        {
            double corrected = prediction.Distance;
            corrected += (prediction.Label != config.LabelBias) ? config.Offset : -config.Offset;
            int label = prediction.Label;
            if (corrected < 0)
            {
                label = (label == 0) ? 1 : 0;
                corrected = corrected * -1;
            }
            bool uncertain = false;
            if (corrected < config.Uncertain[label])
            {
                uncertain = true;
            }
            return new GenderPrediction()
            {
                Area = roi,
                Distance = corrected,
                Gender =  (GenderLabel)label,
                GenderDistance = label == 0 ? -corrected  : corrected,
                IsUncertain = uncertain,
                UncorrectedDistance = prediction.Distance,
                UncorrectedGender = (GenderLabel)prediction.Label
            };
        }

        public void ReadConfig(string csv)
        {
            Config config = new Config();
            using (var fs = File.OpenRead(csv))
            using (var reader = new StreamReader(fs))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    var comp = StringComparison.CurrentCultureIgnoreCase;
                    if (values[0].StartsWith("Size", comp))
                    {
                        int w = Int32.Parse(values[1]);
                        int h = Int32.Parse(values[2]);
                        config.Size = new Size(w,h);
                    }
                    else if (values[0].StartsWith("Bias", comp))
                    {
                        config.LabelBias = Int32.Parse(values[1]);
                        config.Offset = Double.Parse(values[2]);
                    }
                    else if (values[0].StartsWith("Uncertain", comp))
                    {
                        int label = Int32.Parse(values[1]);
                        config.Uncertain[label] = Double.Parse(values[2]);
                    }
                    else if (values[0].StartsWith("MarginX", comp))
                    {
                        config.MarginX = Double.Parse(values[1]);
                    }
                    else if (values[0].StartsWith("MarginY", comp))
                    {
                        config.MarginY = Double.Parse(values[1]);
                    }

                }
               
            }
            _config = config;
        }

        public class GenderImage
        {
            public GenderPrediction Detection { get; set; }
            public Image<Bgr,byte> Image { get; set; }
        }

        public IList<GenderPrediction> FindGender(Image<Bgr, byte> image, IList<Rectangle> faces,bool showGender = true,bool showConfidence = true,bool drawRoI = true)
        {
            var listRes = new List<GenderPrediction>();
            using (var gray2 = image.Convert<Gray, byte>())
            {
                foreach (Rectangle face in faces)
                {
                    var gen = this.Predict(gray2, face);
                    listRes.Add(gen);
                    if (showGender)
                    {
                        string gender = "?";
                        if (!gen.IsUncertain)
                        {
                            if (gen.Gender == GenderDetector.GenderLabel.Male) gender = "M";
                            else if (gen.Gender == GenderDetector.GenderLabel.Female) gender = "F";
                            image.Draw(gender, new Point(face.Left, Math.Max(face.Top - 5, 0)), FontFace.HersheyComplex, 1.0, new Bgr(Color.Red));
                        }  
                    }
                    if (showConfidence && !gen.IsUncertain)
                    {
                        image.Draw(String.Format("{0:n0}", gen.Distance), new Point(Math.Min(face.Left + 30, image.Width), Math.Max(face.Top - 5, 0)), FontFace.HersheyComplex, 0.756, new Bgr(Color.Red));
                    }
                    if (drawRoI)
                    {
                        image.Draw(gen.Area, new Bgr(Color.Orchid), 1);
                    }
                }
            }
            return listRes;
        }

        public GenderPrediction Predict(Image<Gray,byte> img,Rectangle roi)
        {
            //double ratio = 192 / 168.0;
            roi.Inflate((int)(_config.MarginX * roi.Width / 200.0), (int)(_config.MarginY * roi.Height / 200.0));
            roi.Intersect(new Rectangle(0,0,img.Width,img.Height));
            double ratio = _config.Size.Height / (double)_config.Size.Width;
            int expectedH = (int)(roi.Width * ratio);
            roi.Y -= ((expectedH - roi.Height) / 2);
            roi.Y = Math.Max(roi.Y, 0);
            roi.Height = expectedH;
            using (var copy = img.Copy(roi))
            {
                using (var resized = copy.Resize(_config.Size.Width, _config.Size.Height, Inter.Cubic)) // 168,192
                {
                    var prediction = _fisher.Predict(resized);
                    return Correct(prediction, roi,this._config);
                }

            }
               
        }

        public static Task Train(string csvFaces,string modelFile  )
        {
            return Task.Run(() =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                List<Image<Gray, byte>> listImages;
                List<Image<Gray, byte>> resized = new List<Image<Gray, byte>>();
                List<int> labels;
                ReadTrainingCsv(Path.GetDirectoryName(csvFaces), csvFaces, out listImages, out labels);

                // normaliza el tamaño a 140x200
                /*for (int i = 0; i < listImages.Count; i++)
                {
                    var img = listImages[i];
                    var centerX = img.Width / 2;
                    var centerY = img.Height / 2;
                    int targetWidth = 140;
                    int targetHeight = 200;
                    Rectangle full = new Rectangle(0, 0, img.Width, img.Height);
                    Rectangle target = new Rectangle(centerX - targetWidth / 2, centerY - targetHeight / 2, targetWidth, targetHeight);
                    img.ROI = Rectangle.Intersect(full, target);
                    var copy = img.Copy();
                    img.Dispose();
                    if (copy.Width != targetWidth || copy.Height != targetHeight)
                    {
                        listImages[i] = copy.Resize(targetWidth, targetHeight, Inter.Cubic);
                        copy.Dispose();
                    }
                    else
                    {
                        listImages[i] = copy;
                    }
                }*/
                /*
                // para inspeccionar la normalizacion de tamaño
                for (int i = 0; i < listImages.Count; i++)
                {
                    listImages[i].Save(i + ".png");
                }*/
                if (modelFile == null)
                {
                    modelFile = Path.Combine(Path.GetDirectoryName(csvFaces), "gender_model.fisher");
                }

                FisherFaceRecognizer fisher = new FisherFaceRecognizer();
                fisher.Train(listImages.ToArray(), labels.ToArray());
                fisher.Save(modelFile);

                Debug.WriteLine("Training took {0}s:", (int)Math.Round(sw.ElapsedMilliseconds / 1000.0));
            });

        }

        private static void ReadTrainingCsv(string imageDirectory, string csvFaces, out List<Image<Gray, byte>> imageList, out List<int> labels)
        {
            imageList = new List<Image<Gray, byte>>();
            labels = new List<int>();
            using (var fs = File.OpenRead(csvFaces))
            using (var reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    if (line.Length >= 2)
                    {
                        string imageName = values[0].Replace(".\\", "");
                        string imagePath = Path.Combine(imageDirectory, imageName);
                        Debug.WriteLine(imagePath);
                        Image<Gray, byte> img = new Image<Gray, byte>(imagePath);
                        imageList.Add(img);
                        int label;
                        Int32.TryParse(values[1], out label);
                        labels.Add(label);
                    }

                }
            }
        }

        public void Dispose()
        {
            _fisher?.Dispose();
        }
    }
}
