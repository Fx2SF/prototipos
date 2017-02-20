
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;


namespace ContarPersonas
{
    public  class Util
    {
        public static void ConvertBmpIntoJPG(Bitmap bmp, Stream outStream, int quality)
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            // solo el nivel de calidad está soportado, salida progresiva no está soportada (si se especifica es ignorado)
            myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
            bmp.Save(outStream, jpgEncoder, myEncoderParameters);
            outStream.Seek(0, SeekOrigin.Begin);
        }

        public static void GuardarJPGToDisk(Bitmap bmp, string filename, int quality, double scale)
        {
            using (Image<Bgr,byte> image = new Image<Bgr, byte>(bmp))
            using (var resized = image.Resize(scale, Inter.Cubic))
            {
                using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    // solo el nivel de calidad está soportado, salida progresiva no está soportada (si se especifica es ignorado)
                    myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
                    resized.Bitmap.Save(fs, jpgEncoder, myEncoderParameters);
                }
            }
        }

        public static void GuardarStreamToDisk(Stream stream, string filename)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fs);
                stream.Seek(0, SeekOrigin.Begin);
            }
        }

        public static void GuardarJPGToDisk(Bitmap bmp, string filename, int quality)
        {
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                // solo el nivel de calidad está soportado, salida progresiva no está soportada (si se especifica es ignorado)
                myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
                bmp.Save(fs, jpgEncoder, myEncoderParameters);
            }
        }


        private static  ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

    }
}
