using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Vision.v1;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Vision.v1.Data;
using Microsoft.Office.Interop.Excel;


namespace Tarea2
{
    public class CloudVision
    {

        public CloudVision() { }

        public static GoogleCredential CreateCredentials()
        {

            string json = Properties.Settings.Default.json_google;

            string path = json;
            GoogleCredential credential;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var c = GoogleCredential.FromStream(stream);
                credential = c.CreateScoped(VisionService.Scope.CloudPlatform);
            }

            return credential;
        }

        public static VisionService CreateService(string applicationName,
            IConfigurableHttpClientInitializer credentials)
        {
            var service = new VisionService(new BaseClientService.Initializer()
            {
                ApplicationName = applicationName,
                HttpClientInitializer = credentials
            });

            return service;
        }

        private static AnnotateImageRequest CreateAnnotationImageRequest(string path, string[] featureTypes)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Archivo no encontrado.", path);
            }

            var request = new AnnotateImageRequest();
            request.Image = new Google.Apis.Vision.v1.Data.Image();

            var bytes = File.ReadAllBytes(path);
            request.Image.Content = Convert.ToBase64String(bytes);

            request.Features = new List<Feature>();

            foreach (var featureType in featureTypes)
            {
                request.Features.Add(new Feature() { Type = featureType });
            }

            return request;
        }

        public static async Task<AnnotateImageResponse> AnnotateAsync(VisionService service, FileInfo file,
    params string[] features)
        {
            var request = new BatchAnnotateImagesRequest();
            request.Requests = new List<AnnotateImageRequest>();
            request.Requests.Add(CreateAnnotationImageRequest(file.FullName, features));

            var result = await service.Images.Annotate(request).ExecuteAsync();

            if (result?.Responses?.Count > 0)
            {
                return result.Responses[0];
            }

            return null;
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

            //crear servicio
            var credentails = CreateCredentials();
            var service = CreateService("CICLOPE", credentails);

            string[] features = new string[] { "LABEL_DETECTION", "TEXT_DETECTION", "LANDMARK_DETECTION", "LOGO_DETECTION", "IMAGE_PROPERTIES" };
            string[] types = new string[] { "car", "truck", "van", "bus","vehicle","transport"};
            string[] brands = new string[] { "seat", "renault", "peugeot", "dacia","citroen","opel",
            "alfa romeo", "skoda", "chevrolet", "porsche", "honda", "subaru", "mazda", "mitsubishi",
            "lexus", "toyota", "bmw","volkswagen","suzuki","mercedes","mercedes-benz","saab","audi","kia",
            "land rover","dodge","chrysler","ford","hummer","hyundai","infiniti","jaguar","jeep",
            "nissan","volvo","daewoo","fiat","rover" };

            // Excel
            Worksheet ws = (Worksheet)wb.Worksheets.Add();//[1];
            ws.Name = "Google Cloud Vision";

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
            //procesar cada archivo
            foreach (var file in files)
            {
                string f = file.FullName;
                Console.WriteLine("Reading " + f + ":");

                using (var img = System.Drawing.Image.FromFile(file.FullName))
                {
                    var height = img.Height;
                    var width = img.Width;

                    ws.Cells[i, 6] = width + " x " + height;
                }

                ws.Cells[i, 1] = file.Name.Substring(0, file.Name.LastIndexOf("."));
                ws.Cells[i, 7] = file.LastWriteTime.ToShortDateString();
                ws.Cells[i, 8] = "N/A";

                var task = await AnnotateAsync(service, file, features);

                var result = task.LabelAnnotations;
                var keywords = result?.Select(s => s.Description).ToArray();

                //var words = String.Join(", ", keywords);
               //System.Diagnostics.Debug.WriteLine(words);

                string tipo = "N/A";
                foreach (var key in keywords)
                {
                    if (types.Contains(key))
                    {
                        tipo = key;
                        break;
                    }
                }
                ws.Cells[i, 2] = tipo;

                //marcas
                string marca = "N/A";
                foreach (var key in keywords)
                {
                    if (brands.Contains(key))
                    {
                        marca = key;
                        break;
                    }
                }
                ws.Cells[i, 4] = marca;


                var text = task.TextAnnotations;
                var keywords2 = text?.Select(s => s.Description).ToArray();

                if (keywords2 != null && keywords2[0] != null) {

                    ws.Cells[i, 9] = keywords2[0];

                }
               // var words2 = String.Join(", ", keywords2);
               // System.Diagnostics.Debug.WriteLine(words2);

 /*               
                var landmark = task.LandmarkAnnotations;
                var keywords3 = landmark?.Select(s => s.Description).ToArray();
                if (keywords3 != null)
                {
                    var words3 = String.Join(", ", keywords3);
                    System.Diagnostics.Debug.WriteLine(words3);
                }
*/
                var logo = task.LogoAnnotations;
                var keywords4 = logo?.Select(s => s.Description).ToArray();
                if (keywords4 != null)
                {
                    var words4 = String.Join(", ", keywords4);
                    ws.Cells[i, 4] = ws.Cells[i, 4] +" Logos: " +words4;
                }
                var props = task.ImagePropertiesAnnotation;

                ColorInfo c = props?.DominantColors.Colors.FirstOrDefault();

                float? r = c.Color.Red;
                float? g = c.Color.Green;
                float? b = c.Color.Blue;

                System.Drawing.Color color = System.Drawing.Color.FromArgb((int)r, (int)g, (int)b);

                if (ColorWithinRangeR(color))
                {
                    ws.Cells[i, 3] = "red" + " (" + color.R+" "+color.G+" "+color.B+")";

                }
                else if (ColorWithinRangeG(color))
                {

                    ws.Cells[i, 3] = "green" + " (" + color.R + " " + color.G + " " + color.B + ")";

                }
                else if (ColorWithinRangeB(color))
                {
                    ws.Cells[i, 3] = "blue" + " (" + color.R + " " + color.G + " " + color.B + ")";
                }
                else if (ColorWithinRangeBL(color))
                {
                    ws.Cells[i, 3] = "black" + " (" + color.R + " " + color.G + " " + color.B + ")";

                }
                else {

                    ws.Cells[i, 3] = "white" + " (" + color.R + " " + color.G + " " + color.B + ")";
                }
                //Console.WriteLine(r.ToString()+" "+g.ToString()+" "+b.ToString());

                // escala de grises
                var keywords5 = props?.DominantColors.Colors.Select(s => s.Color.ToString()).ToArray();

                bool byn = false;
                foreach (ColorInfo col in props?.DominantColors.Colors) {

                    if ((col.Color.Red == col.Color.Green) &&
                         (col.Color.Red == col.Color.Blue) &&
                         (col.Color.Green == col.Color.Blue))
                    {

                        byn = true;
                    }
                    else {

                        byn = false;
                    }
                }
                if (byn)
                {

                    ws.Cells[i, 5] = "B&N";
                }
                else {

                    ws.Cells[i, 5] = "Color";
                }


                i++;
            }

            ws.Columns.AutoFit();

        }

        // red
        private readonly System.Drawing.Color r_from = System.Drawing.Color.FromArgb(50, 20, 20);
        private readonly System.Drawing.Color r_to = System.Drawing.Color.FromArgb(255, 105, 97);
        bool ColorWithinRangeR(System.Drawing.Color c)
        {
            return
               (r_from.R <= c.R && c.R <= r_to.R) &&
               (r_from.G <= c.G && c.G <= r_to.G) &&
               (r_from.B <= c.B && c.B <= r_to.B);
        }

        // blue
        private readonly System.Drawing.Color b_from = System.Drawing.Color.FromArgb(0, 35, 102);
        private readonly System.Drawing.Color b_to = System.Drawing.Color.FromArgb(204, 204, 255);
        bool ColorWithinRangeB(System.Drawing.Color c)
        {
            return
               (b_from.R <= c.R && c.R <= b_to.R) &&
               (b_from.G <= c.G && c.G <= b_to.G) &&
               (b_from.B <= c.B && c.B <= b_to.B);
        }

        // green
        private readonly System.Drawing.Color g_from = System.Drawing.Color.FromArgb(85, 107, 47);
        private readonly System.Drawing.Color g_to = System.Drawing.Color.FromArgb(178, 236, 93);
        bool ColorWithinRangeG(System.Drawing.Color c)
        {
            return
               (g_from.R <= c.R && c.R <= g_to.R) &&
               (g_from.G <= c.G && c.G <= g_to.G) &&
               (g_from.B <= c.B && c.B <= g_to.B);
        }

        // black
        private readonly System.Drawing.Color bl_from = System.Drawing.Color.FromArgb(0, 0, 0);
        private readonly System.Drawing.Color bl_to = System.Drawing.Color.FromArgb(85, 85, 85);
        bool ColorWithinRangeBL(System.Drawing.Color c)
        {
            return
               (bl_from.R <= c.R && c.R <= bl_to.R) &&
               (bl_from.G <= c.G && c.G <= bl_to.G) &&
               (bl_from.B <= c.B && c.B <= bl_to.B);
        }


    }
}
