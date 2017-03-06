using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;
using System;


namespace CuentaPersonas
{
    public class CognitiveService
    {

        // creo un cliente de la API (usando una API Key)
        private VisionServiceClient visionClient = null;

        public CognitiveService() {

            string api = "8b2d73635e3849efac561f4e8c8290be";
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
        private string LogAnalysisResult(AnalysisResult result)
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


        public async Task<string> DoWork(string date)
        {

            AnalysisResult analisis = await GetImageAnalysis(date);
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            File.Delete(directory + date + ".jpg");

            return LogAnalysisResult(analisis);

        }
    }
}
