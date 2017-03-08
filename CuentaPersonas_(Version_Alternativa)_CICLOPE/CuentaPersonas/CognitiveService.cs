using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using System.IO;
using System;
using System.Drawing;
using Microsoft.ProjectOxford.Vision.Contract;
using Rectangle = System.Drawing.Rectangle;


namespace CuentaPersonas
{
    public class CognitiveService
    {

        // creo un cliente de la API (usando una API Key)
        private VisionServiceClient visionClient = null;
        public bool MantenerImagenes { get; set; } = false;

        public CognitiveService()
        {

            string api = "912104c5a65c444089b5ed0e6d358dff";//"8b2d73635e3849efac561f4e8c8290be";
            visionClient = new VisionServiceClient(api);

        }

        // método para llamar a la API
        private async Task<AnalysisResult> GetImageAnalysis(string date)
        {
            // le digo que me interesa
            VisualFeature[] features = { VisualFeature.Faces };
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            using (Stream imageFileStream = File.OpenRead(directory + date + ".jpg"))
            {
                AnalysisResult analysisResult = await visionClient.AnalyzeImageAsync(imageFileStream, features);
                return analysisResult;
            }
        }

        
        //método para tomar los resultados e imprimirlos en consola
        public string LogAnalysisResult(AnalysisResult result)
        {
            if (result == null)
            {
                Console.WriteLine("Analisis vacio.");
            }

            string res = "";
            if (result.Faces != null)
            {
                foreach (var face in result.Faces)
                {
                   res = face.Gender + " " + face.Age;
                   break;
                }
            }

            return res;
        }


        public async Task<AnalysisResult> DoWork(string date,Bitmap bmp,double scale)
        {

            AnalysisResult analisis = await GetImageAnalysis(date);
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            if (bmp != null)
            {
                DrawFaceResult(bmp, analisis, scale);
            }
            
            if (!MantenerImagenes)
            {
                File.Delete(directory + date + ".jpg");
            }

            return analisis;

        }
        private Rectangle Scale(FaceRectangle r, double factor)
        {
            return new Rectangle(
                x: (int)Math.Round(r.Left * factor),
                y: (int)Math.Round(r.Top * factor),
                width: (int)Math.Round(r.Width * factor),
                height: (int)Math.Round(r.Height * factor)
            );
        }

        public void DrawFaceResult(Bitmap bmp, AnalysisResult result,double scale)
        {
            // Create pen.
            Pen pen = new Pen(System.Drawing.Color.Red, 1);
            
            if (result.Faces != null)
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    foreach (var face in result.Faces)
                    {
                        Rectangle rect = Scale(face.FaceRectangle, 1 / scale);
                        g.DrawRectangle(pen, rect);
                        var text = face.Gender + " " + face.Age;
                        Font drawFont = new System.Drawing.Font("Arial", 10);
                        SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Red);
                        FontFamily fontFamily = drawFont.FontFamily;
                        FontStyle fontStyle = drawFont.Style;
                        float x = rect.Left;
                        float y = rect.Top - 5;
                        y -= fontFamily.GetCellAscent(fontStyle) * drawFont.Size / fontFamily.GetEmHeight(fontStyle);
                        y = Math.Max(y, 0);
                        
                        g.DrawString(text, drawFont, drawBrush,x,y);
                        drawFont.Dispose();
                        drawBrush.Dispose();
                    }
                }

            }
        }
    }
}
