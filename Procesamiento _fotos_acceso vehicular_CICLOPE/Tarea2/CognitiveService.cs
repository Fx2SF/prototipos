using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace Tarea2
{
    public class CognitiveService
    {

        // creo un cliente de la API (usando una API Key)
        private VisionServiceClient visionClient = null;

        string[] colors = new string[] { "amaranth", "amber", "amethyst", "apricot", "aquamarine", "azure",
            "beige", "black", "blue", "blush", "bronze", "brown", "burgundy" , "byzantium", "carmine", "cerise", "cerulean",
            "champagne", "chocolate", "coffee", "copper", "coral", "crimson", "cyan","emerald", "erin", "gold", "gray", "green",
            "harlequin", "indigo", "ivory", "jade","lavender", "lemon", "lilac", "lime", "magenta", "maroon", "mauve",
            "ocher", "olive", "orange","orchid", "peach", "pear", "periwinkle", "pink",
            "plum", "puce", "purple", "raspberry", "red", "rose", "ruby", "salmon",
            "sangria", "sapphire", "scarlet", "silver", "tan", "taupe", "teal", "turquoise",
            "violet", "viridian", "white", "yellow"};

        string[] types = new string[] {"car", "truck", "van", "bus" };
        string[] state = new string[] { "outdoor", "indoor"};

        public CognitiveService() {

            string api = Properties.Settings.Default.api_ms;

            this.visionClient = new VisionServiceClient(api);

        }

        // método para llamar a la API
        private async Task<AnalysisResult> GetImageAnalysis(string imageFilePath)
        {
            // le digo que me interesa
            VisualFeature[] features = { VisualFeature.Tags, VisualFeature.Categories, VisualFeature.Description, VisualFeature.Color};

            using (Stream imageFileStream = File.OpenRead(imageFilePath))
            {
                AnalysisResult analysisResult = await this.visionClient.AnalyzeImageAsync(imageFileStream, features);
                return analysisResult;
            }
        }

        private async Task<OcrResults> UploadAndRecognizeImage(string imageFilePath)
        {

            using (Stream imageFileStream = File.OpenRead(imageFilePath))
            {
                // OCR de la imagen
                OcrResults ocrResult = await this.visionClient.RecognizeTextAsync(imageFileStream, "es");
                return ocrResult;
            }

        }

        //método para tomar los resultados e imprimirlos en consola
        private void LogAnalysisResult(AnalysisResult result, Worksheet ws, int i)
        {
            if (result == null)
            {
                System.Diagnostics.Debug.WriteLine("Analisis vacio.");

            }

            // metadata
            if (result.Metadata != null)
            {
                //System.Diagnostics.Debug.WriteLine("Image Format : " + result.Metadata.Format);
                //System.Diagnostics.Debug.WriteLine("Image Dimensions : " + result.Metadata.Width + " x " + result.Metadata.Height);

                ws.Cells[i, 6] = result.Metadata.Width + " x " + result.Metadata.Height;

            }

            // categorias
            if (result.Categories != null && result.Categories.Length > 0)
            {
                foreach (var category in result.Categories)
                {
                    System.Diagnostics.Debug.WriteLine("Name : " + category.Name + "; Score : " + category.Score);
                }
            }

            if (result.Color != null)
            {

                if (result.Color.IsBWImg)
                {
                    ws.Cells[i, 5] = "B&N";
                }
                else {

                    ws.Cells[i, 5] = "Color";
                }
/*
                System.Diagnostics.Debug.WriteLine("AccentColor : " + result.Color.AccentColor);
                System.Diagnostics.Debug.WriteLine("Dominant Color Background : " + result.Color.DominantColorBackground);
                System.Diagnostics.Debug.WriteLine("Dominant Color Foreground : " + result.Color.DominantColorForeground);

                if (result.Color.DominantColors != null && result.Color.DominantColors.Length > 0)
                {
                    string colors = "Dominant Colors : ";
                    foreach (var color in result.Color.DominantColors)
                    {
                        colors += color + " ";
                    }
                    System.Diagnostics.Debug.WriteLine(colors);
                }
  */
            }

            if (result.Description != null)
            {
                //System.Diagnostics.Debug.WriteLine("Description : ");
                foreach (var caption in result.Description.Captions)
                {
                    //System.Diagnostics.Debug.WriteLine("   Caption : " + caption.Text + "; Confidence : " + caption.Confidence);
                    ws.Cells[i, 4] = caption.Text;
                    break;

                }


                string tags = "   Tags : ";
                foreach (var tag in result.Description.Tags)
                {
                    tags += tag + ", ";
                }
                System.Diagnostics.Debug.WriteLine(tags);

                string color = "";
                foreach (var tag in result.Description.Tags)
                {
                    if (colors.Contains(tag)) {
                        color = tag;
                        break;
                    }
                }
                ws.Cells[i, 3] = color;

                string tipo = "";
                foreach (var tag in result.Description.Tags)
                {
                    if (types.Contains(tag))
                    {
                        tipo = tag;
                        break;
                    }
                }
                ws.Cells[i, 2] = tipo;

                string estado = "";
                foreach (var tag in result.Description.Tags)
                {
                    if (state.Contains(tag))
                    {
                        estado = tag;
                        break;
                    }
                }
                ws.Cells[i, 8] = estado;

            }

            if (result.Tags != null)
            {
                System.Diagnostics.Debug.WriteLine("Tags : ");
                foreach (var tag in result.Tags)
                {
                    System.Diagnostics.Debug.WriteLine("   Name : " + tag.Name + "; Confidence : " + tag.Confidence + "; Hint : " + tag.Hint);
                }
            }           

        }

        private void LogOCRResult(OcrResults results, Worksheet ws, int i)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (results == null){
                System.Diagnostics.Debug.WriteLine("OCR vacio.");
            }

            if (results != null && results.Regions != null)
            {
                stringBuilder.AppendLine();
                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Append(word.Text);
                            stringBuilder.Append(" ");
                        }

                        stringBuilder.AppendLine();
                    }

                    stringBuilder.AppendLine();
                }
            }

            ws.Cells[i, 9] = stringBuilder.ToString();

            //System.Diagnostics.Debug.WriteLine(stringBuilder.ToString());

        }

        public async Task DoWork(string path, Workbook wb)
        {

            var ext = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".gif" };
            var dir = path;
            var files =
                Directory
                    .GetFiles(dir, "*.*", SearchOption.AllDirectories)
                    .Where(f => ext.Contains(Path.GetExtension(f)))
                    .Select(f => new FileInfo(f))
                    .ToArray();

            // Excel
            Worksheet ws = (Worksheet)wb.Worksheets.Add();
            ws.Name = "MS Cognitive Service";
            
            ws.Cells[1, 1] = "Imagen";
            ws.Cells[1, 2] = "Tipo";
            ws.Cells[1, 3] = "Color";
            ws.Cells[1, 4] = "Otros Datos";
            ws.Cells[1, 5] = "Tipo Imagen";
            ws.Cells[1, 6] = "Tamaño Imagen";
            ws.Cells[1, 7] = "Fecha Archivo";
            ws.Cells[1, 8] = "Afuera/Adentro";
            ws.Cells[1, 9] = "Texto(OCR)";
            ws.Cells[1, 1].EntireRow.Font.Bold = true;

            int i = 2;
            foreach (var file in files)
            {

                ws.Cells[i, 1] = file.Name.Substring(0, file.Name.LastIndexOf("."));
                ws.Cells[i, 7] = file.LastWriteTime.ToShortDateString();

                AnalysisResult analisis = await GetImageAnalysis(file.FullName);
                OcrResults ocr = await UploadAndRecognizeImage(file.FullName);
                LogAnalysisResult(analisis, ws, i);
                LogOCRResult(ocr, ws, i);

                i++;
            }

            ws.Columns.AutoFit();

        }
    }

}
