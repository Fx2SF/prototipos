using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;


namespace ContarPersonas
{
    class Extractor
    {
        Capture _capture = null;
        private long totalframes = 0;

        public void Iniciar(string videoFileName)
        {
            _capture = new Capture(videoFileName);
            totalframes = (long)_capture.GetCaptureProperty(CapProp.FrameCount);
        }

        public TimeSpan GetDuration(string videoFileName)
        {
            using (Capture capture = new Capture(videoFileName))
            {
                long frameNumbers = (long)capture.GetCaptureProperty( CapProp.FrameCount);
                double fps = capture.GetCaptureProperty(CapProp.Fps);
                double seconds = Math.Floor(frameNumbers / fps);
                return TimeSpan.FromSeconds(seconds);
            }
        }

        


        public void Finalizar()
        {
            _capture.Pause();
            _capture.Dispose();
            // pongo _capture en null para eliminar la referencia al objeto,
            // sino la otra seria no usarlo a nivel de clase
            _capture = null;
        }


        public Mat GetMat(long ms)
        {
            _capture.SetCaptureProperty(CapProp.PosMsec, ms);
            return _capture.QueryFrame().Clone();
        }

        public bool FrameIntoJPGStreamMs(long ms, Stream stream)
        {
            _capture.SetCaptureProperty(CapProp.PosMsec,ms);
            using (Mat frame = _capture.QueryFrame())
            {
                Bitmap bmp = frame.Bitmap;
                Util.ConvertBmpIntoJPG(bmp, stream, 90);
            }
            return true;
        }


        public bool FrameIntoJPGStream(int frameNumber,Stream stream)
        {
            // Seteo el número de frame a capturar
            if (frameNumber < totalframes)
            {
                _capture.SetCaptureProperty(CapProp.PosFrames, frameNumber);
                using (Mat frame = _capture.QueryFrame())
                {
                    Bitmap bmp = frame.Bitmap;
                    Util.ConvertBmpIntoJPG(bmp, stream, 90);
                }
            }
            return frameNumber < totalframes;
        }

    }
}
