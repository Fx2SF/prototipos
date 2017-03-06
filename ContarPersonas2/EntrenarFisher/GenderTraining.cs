using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;

namespace EntrenarFisher
{
    class GenderTraining
    {
        public static Task Train(string csvFaces, string modelFile,string configFile, Size normSize = default(Size))
        {
            return Task.Run(() =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                List<Image<Gray, byte>> listImages;
                List<int> labels;

                ReadTrainingCsv(Path.GetDirectoryName(csvFaces), csvFaces, out listImages, out labels);
                if (listImages.Count < 2)
                {
                    throw new ArgumentException("La lista debe contener al menos 2 imagenes");
                }
                var clases = labels.Distinct().ToList();
                if (!clases.Contains(0) | !clases.Contains(1))
                {
                    throw new ArgumentException("Se necesitan clases 0 y 1");
                }
                // normaliza el tamaño a normSize (por ej. 140x200)
                if (normSize != default(Size))
                {
                    for (int i = 0; i < listImages.Count; i++)
                    {
                        var img = listImages[i];
                        var centerX = img.Width / 2;
                        var centerY = img.Height / 2;
                        int targetWidth = normSize.Width;
                        int targetHeight = normSize.Height;
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
                    }
                }
             
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
                Size size = listImages[0].Mat.Size;
                SaveConfigFile(configFile,size);

                FisherFaceRecognizer fisher = new FisherFaceRecognizer();
                fisher.Train(listImages.ToArray(), labels.ToArray());
                fisher.Save(modelFile);

                Debug.WriteLine("Training took {0}s:", (int)Math.Round(sw.ElapsedMilliseconds / 1000.0));
            });

        }

        private static void SaveConfigFile(string filename,Size size)
        {
            using (var w = new StreamWriter(filename))
            {
                w.WriteLine("Size;{0};{1};  // ancho, alto", size.Width,size.Height);
                w.WriteLine("Bias;0;0; // clase y offset del sesgo");
                w.WriteLine("Uncertain;0;0; // para masculino");
                w.WriteLine("Uncertain;1;0; // para femenino");
                w.WriteLine("MarginX;0; // margen ancho cara ");
                w.WriteLine("MarginY;0; // margen alto cara");
            }
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

                        if (File.Exists(imagePath))
                        {
                            Debug.WriteLine(imagePath);
                            Image<Gray, byte> img = new Image<Gray, byte>(imagePath);
                            imageList.Add(img);
                            int label;
                            Int32.TryParse(values[1], out label);
                            labels.Add(label);
                        }
                        else
                        {
                            Debug.WriteLine("NO EXISTE: " + imagePath);
                        }
                    }

                }
            }
        }
    }
}
