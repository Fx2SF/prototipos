using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContarPersonas;
using ContarPersonas2.Properties;
using Emgu.CV;
using Emgu.CV.BgSegm;
using Emgu.CV.Cvb;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.VideoSurveillance;

namespace ContarPersonas2
{
    public partial class Form1 : Form
    {
        private Capture _capture = null;
        ContarPersonasOpenCV _contar = new ContarPersonasOpenCV();
        
        private int _frameAmount = 0;
        private int _personas = 0;

        private static BackgroundSubtractor _fgDetector;
        private static Emgu.CV.Cvb.CvBlobDetector _blobDetector;
        private static Emgu.CV.Cvb.CvTracks _tracker;
        private static Mat _structuringElement;
        Point p0 = new Point(-1, -1);
        Point hsvSample = new Point(0,0);


        private MCvScalar SCALAR_BLACK = new MCvScalar(0.0, 0.0, 0.0);
        private MCvScalar SCALAR_WHITE = new MCvScalar(255.0, 255.0, 255.0);
        private MCvScalar SCALAR_BLUE = new MCvScalar(255.0, 0.0, 0.0);
        private MCvScalar SCALAR_GREEN = new MCvScalar(0.0, 255.0, 0.0);
        private MCvScalar SCALAR_RED = new MCvScalar(0.0, 0.0, 255.0);

        private int _cont;

        private Size _frameSize;

        private Stopwatch _timeSinceStart;

        private bool _inTabHSV = false;
        private bool _useMotion = false;
        private bool _useSkin = false;
        private bool _useGenderDetection = false;
        private bool _showObjects = false;
        private bool _showROI = true;
        private bool _showGender = true;
        private bool _showGenderConfidence = true;
        private int _modoBusqueda = 0;

        private int _generoHistoria = 75;

        private static string fisherModel = "GeorgiaTech.fisher";//"02_240.fisher";//"02_1800.fisher";
        private static string fisherConfig = "GeorgiaTech.csv";//"02_240.fisher.csv"; //"02_1800.fisher.csv";
        private GenderDetector _genderDetector = new GenderDetector(fisherModel, fisherConfig);
        // dictionary para aplicar momento en genero
        private Dictionary<uint, double> _labels = new Dictionary<uint, double>();

        int _precision = 3;




        public Form1()
        {
            InitializeComponent();
        }

        private void SetupMotionDetection()
        {
            
            _cont = 0;
            // crea MOG con valores por defecto
            _fgDetector = new BackgroundSubtractorMOG(history: 200, nMixtures: 5, backgroundRatio: 0.7, noiseSigma: 0);
            _blobDetector = new CvBlobDetector();
            _tracker = new CvTracks();
            var elipseSize = 7;

            _structuringElement = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(elipseSize, elipseSize), p0);
        }


        private void btConectar_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                StopCapture();
                _contar = new ContarPersonasOpenCV();
            }
            pickerInicio.Enabled = rbArchivo.Checked;
            btSeek.Enabled = rbArchivo.Checked;
            if (rbArchivo.Checked || rbIP.Checked)
            {
                _capture = new Capture(cboxFuente.Text);
                if (rbArchivo.Checked)
                {
                    try
                    {
                        DateTime end = DateTime.Today;
                        TimeSpan duration = TimeSpan.FromSeconds(_capture.GetCaptureProperty(CapProp.FrameCount) / _capture.GetCaptureProperty(CapProp.Fps));
                        end.Add(duration);
                        pickerInicio.MaxDate = end;
                    }
                    catch (Exception)
                    {
                        pickerInicio.Value = DateTime.Today;
                    }
                }

            }
            else if (rbWeb.Checked)
            {
                _capture = new Capture();
            }
            else
            {
                MessageBox.Show("Fuente no válida");
            }
            if (_capture != null)
            {
                _timeSinceStart = Stopwatch.StartNew();
                SetupMotionDetection();
                _contar.SetupCascades();
                _contar.DetectionKeepTime = trackBar2.Value;
                _contar.ColorHaarCascade = ckShowHaar.Checked;
                ConfigRes();
                _capture.ImageGrabbed += ProcessFrame;
                cboxFuente.Items.Insert(0,cboxFuente.Text);
                _capture.Start();
            }

            
        }

        private void SaveMRU()
        {
            var list = cboxFuente.Items.OfType<String>().ToList();
            if (list.Count > 10)
            {
                list = list.Take(10).ToList();
            }
            Settings.Default.MRU = String.Join(";", list);
            Settings.Default.Save();
        }

        private void LoadMRU()
        {
            try
            {
                String[] mru = Settings.Default.MRU.Split(';');
                foreach (var path in mru)
                {
                    if (path.Trim().Length > 0)
                    {
                        cboxFuente.Items.Add(path);
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private Rectangle[] SkinDetection(Mat frame, out CvBlobs blobs)
        {
            var hsvMat = new Mat();

            CvInvoke.CvtColor(frame, hsvMat, ColorConversion.Bgr2Hsv);
            var hsv = hsvMat.ToImage<Hsv, byte>();

            /* El filtro de color para la piel se define como:
                Color: Hue(0-180) entre 165 y 30
                Saturacion: Sat(0-255) entre el 23% y el 68% (http://stackoverflow.com/questions/8753833/exact-skin-color-hsv-range?rq=1)
                Valor(Brillo): Value(0-255) > 40
            */
            double scaleSatLower = 0.23;
            double scaleSatUpper = 0.68;

            Hsv lower = new Hsv(0, (int)(scaleSatLower * 255), 40);
            Hsv higher = new Hsv(30, (int)(scaleSatUpper * 255), 255);
            // necesito dividir el rango de Hue [165,30] en dos rangos [165,180] y [0,30]
            Hsv lower2 = new Hsv(165, (int)(scaleSatLower * 255), 40);
            Hsv higher2 = new Hsv(180, (int)(scaleSatUpper * 255), 255);

            var imageThresold = hsv.InRange(lower, higher) | hsv.InRange(lower2, higher2);
            var cantErode = 2;
            var cantDilate = 2;
            // aplica dilatacion y erocion para eliminar objetos muy pequeños y regularizar bordes
            CvInvoke.Dilate(imageThresold, imageThresold, _structuringElement, p0, cantDilate, BorderType.Default, SCALAR_BLACK);
            //CvInvoke.Erode(imageThresold, imageThresold, _structuringElement, p0, cantErode, BorderType.Default, SCALAR_BLACK);

            // filtra los blobs encontrados por px^2
            var _areaMin = 200;
            var _areaMax = 500000;
            blobs = new CvBlobs();
            _blobDetector.Detect(imageThresold, blobs);
            blobs.FilterByArea(_areaMin, _areaMax);


            List<Rectangle> res = new List<Rectangle>();

            foreach (var pair in blobs)
            {
                //CvTrack b = pair.Value;
                CvBlob b = pair.Value;
                res.Add(b.BoundingBox);
            }
            if (_inTabHSV)
            {
                this.Invoke(new Action(()=>
                {
                    imageBoxMask.Image = imageThresold.Mat;
                    var channels = hsv.Clone().Split();
                    imageBoxHue.Image = channels[0];
                    imageBoxSat.Image = channels[1];
                    imageBoxValue.Image = channels[2];
                    Point ps = hsvSample;
                    lblHSV.Text = String.Format("({0},{1}) => HSV: {2},{3},{4}", ps.X, ps.Y, channels[0][ps],
                        channels[1][ps], channels[2][ps]);
                }));

            }
            return res.ToArray();

        }


        private void MostrarCara(Image<Bgr, byte> img, Rectangle face, GenderDetector.GenderLabel label)
        {
            int newWidth = (int) (face.Width / (double) face.Height * 100.0);
            using (var copy1 = img.Copy(face))
            using (var copy = copy1.Resize(newWidth, 100, Inter.Linear))
            {
                this.Invoke(new Action(() =>
                {
                    if (label == GenderDetector.GenderLabel.Male)
                    {
                        gridHombres.Rows.Add(copy.ToBitmap());
                    }
                    else if (label == GenderDetector.GenderLabel.Female)
                    {
                        gridMujeres.Rows.Add(copy.ToBitmap());
                    }
                }));
            }
            
        }

        private void LimpiarCaras()
        {
            this.Invoke(new Action(() =>
            {
                gridHombres.Rows.Clear();
                gridMujeres.Rows.Clear();
                txtCantMale.Clear();
                txtCantidadFemale.Clear();
            }));
        }

        private void MostrarCaras(Image<Bgr,byte> img, IList<GenderDetector.GenderPrediction> list)
        {
            this.Invoke(new Action(() =>
            {
                gridHombres.Rows.Clear();
                gridMujeres.Rows.Clear();
                txtCantMale.Text = list.Count(p => p.Gender == GenderDetector.GenderLabel.Male).ToString();
                txtCantidadFemale.Text = list.Count(p => p.Gender == GenderDetector.GenderLabel.Female).ToString();
            }));
            foreach (var prediction in list)
            {
                MostrarCara(img, prediction.Area, prediction.Gender);
            }
        }

        private Dictionary<string, string[]> modelos = new Dictionary<string, string[]>();

        private void CargarModelosGenero()
        {
              
            using (var fs = File.OpenRead("genero.csv"))
            using (var reader = new StreamReader(fs))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!line.StartsWith("//"))
                    {
                        var values = line.Split(';');
                        var key = values[0].Trim();
                        var value = new String[] { values[1].Trim(), values[2].Trim() };
                        modelos.Add(key,value );
                        cboxGeneroModelo.Items.Add(key);
                    }

                }

            }
            if (cboxGeneroModelo.Items.Count > 0)
            {
                cboxGeneroModelo.SelectedIndex = 0;
            }
            
        }
    

        private CvTrack? BuscarMasCercano(CvTracks tracks, Rectangle rec)
        {
            CvTrack? nearest = null;
            double distance =Double.MaxValue;
            var cRec = new Point(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);
            foreach (var blob in tracks)
            {
                var bbox = blob.Value.BoundingBox;
                Point center ;
                if (bbox.IntersectsWith(rec))
                {
                    var intersection = Rectangle.Intersect(bbox, rec);
                    center = new Point(intersection.X + intersection.Width / 2,
                        intersection.Y + intersection.Height / 2);
                }
                else
                {
                    center = new Point((int) blob.Value.Centroid.X,(int) blob.Value.Centroid.Y);
                }
                var d = Math.Pow(center.X - cRec.X, 2) + Math.Pow(center.Y - cRec.Y, 2);
                if (d < distance)
                {
                    distance = d;
                    nearest = blob.Value;
                }
            }
            return nearest;
        }


        private void AplicarMomento(CvTracks tracks, IList<GenderDetector.GenderPrediction> predictions)
        {
            var newLabels = new Dictionary<uint, double>();
            // aplico momento de valor anterior utilizando el blob mas cercano
            foreach (var prediction in predictions)
            {
                double genderDistance = prediction.GenderDistance;

                var nearestTrack = BuscarMasCercano(tracks, prediction.Area);
                if (nearestTrack.HasValue)
                {
                    if (_labels.ContainsKey(nearestTrack.Value.Id))
                    {
                        var pc = _generoHistoria / 100.0;
                        genderDistance = pc * _labels[nearestTrack.Value.Id] + (1 - pc) * genderDistance;
                    }
                    _genderDetector.UpdatePrediction(prediction, genderDistance);
                    if (!newLabels.ContainsKey(nearestTrack.Value.Id))
                    {
                        newLabels.Add(nearestTrack.Value.Id, prediction.GenderDistance);
                    }

                }

            }
            // de nuevo recorro para agregar todos los blobs que intercepten al dictionary
            // si no son el mas cercano (para reducir el problema de 2 personas cruzandose
            // y tener mas opciones que el mas cercano en el momento)
            foreach (var prediction in predictions)
            {
                foreach (var blob in tracks)
                {
                    var bbox = blob.Value.BoundingBox;
                    if (!newLabels.ContainsKey(blob.Key) && bbox.IntersectsWith(prediction.Area))
                    {
                        newLabels.Add(blob.Key, prediction.GenderDistance);
                    }

                }
            }
            _labels = newLabels;
        }

        private void ProcessFrame(object sender, EventArgs eventArgs)
        {
            try
            {
                if (_capture != null && _capture.Ptr != IntPtr.Zero)
                {
                    Mat frame = new Mat();
                    _capture.Retrieve(frame, 0);
                    _frameSize = new Size(frame.Width, frame.Height);

                    CvBlobs blobsMotion = new CvBlobs();
                    using (Mat imgThresh = frame.Clone())
                    using (Mat forgroundMask = new Mat())
                    {
                        // aplica gaussian blur para reducir el ruido
                        CvInvoke.GaussianBlur(imgThresh, imgThresh, new Size(3, 3), 0);
                        // obtiene la mascada de fondo
                        if (_useMotion)
                        {
                            _fgDetector.Apply(imgThresh, forgroundMask);
                            var cantErode = 2;
                            var cantDilate = 2;
                            // aplica dilatacion y erocion para eliminar objetos muy pequeños y regularizar bordes
                            CvInvoke.Dilate(forgroundMask, forgroundMask, _structuringElement, p0, cantErode, BorderType.Default, SCALAR_BLACK);
                            CvInvoke.Erode(forgroundMask, forgroundMask, _structuringElement, p0, cantDilate, BorderType.Default, SCALAR_BLACK);

                            // filtra los blobs encontrados por px^2
                            var _areaMin = 200;
                            var _areaMax = 500000;
                            using (var greyFMask = forgroundMask.ToImage<Gray, byte>())
                            {
                                _blobDetector.Detect(greyFMask, blobsMotion);
                                blobsMotion.FilterByArea(_areaMin, _areaMax);
                            }
                            // seguimiento de objetos
                            var thDistance = 0.01; // max distance to determine when a track and a blob match
                            _tracker.Update(blobsMotion, thDistance: thDistance * frame.Width, thInactive: 5, thActive: 5);
                        }
                    }
                    // deteccion de piel
                    var skinRecs = new Rectangle[] { };
                    CvBlobs skinBlobs;
                    if (_useSkin)
                    {
                        skinRecs = SkinDetection(frame.Clone(), out skinBlobs);
                        if (!_useMotion)
                        {
                            var thDistance = 0.01; // max distance to determine when a track and a blob match
                            _tracker.Update(skinBlobs, thDistance: thDistance * frame.Width, thInactive: 5, thActive: 5);
                        }

                    }


                    Rectangle[] recsMotion = blobsMotion.Select(b => b.Value.BoundingBox).ToArray();


                    IList<Rectangle> ROI;
                    var image = frame.ToImage<Bgr, byte>();
                    if (_useMotion && _useSkin)
                    {
                        var intersected = new List<Rectangle>();
                        foreach (var r1 in recsMotion)
                        {
                            foreach (var r2 in skinRecs)
                            {
                                if (r1.IntersectsWith(r2))
                                {
                                    intersected.Add(Rectangle.Intersect(r1, r2));
                                }
                            }
                        }
                        ROI = intersected;
                    }
                    else if (_useMotion)
                    {
                        ROI = recsMotion;
                    }
                    else if (_useSkin)
                    {
                        ROI = skinRecs;
                    }
                    else
                    {
                        ROI = null;
                    }
                    // copia para sacar caras a mostrar segun genero
                    var genderCopy = image.Copy();


                    //IList<Rectangle> faceDetections = _contar.RecognizeUpper(image, precision, ROI);
                    _contar.ShowROI = _showROI;
                    IList<Rectangle> faceDetections = new List<Rectangle>();
                    switch (_modoBusqueda)
                    {
                        case 0:
                            // reconoce caras con FaceAlt2 + FaceProfile
                            faceDetections = _contar.RecognizeFacesDuo(image, _precision, ROI);
                            break;
                        case 1:
                            // reconoce la parte superior del cuerpo con UpperBody
                            faceDetections = _contar.RecognizeUpper(image, _precision,  ROI);
                            break;
                        case 2:
                            // reconoce el cuerpo de peatones con HOG Descriptor
                            faceDetections = _contar.RecognizeHOG(image,_precision);
                            break;
                        default:
                            Debug.WriteLine("Modo de búsqueda inválido!!!");
                            break;
                    }

                    _personas = faceDetections.Count;
                    if (_useGenderDetection)
                    {
                        try
                        {
                            var genderPredictions = _genderDetector.FindGender(image, faceDetections,_showGender,_showGenderConfidence);
                            if (_useSkin || _useMotion)
                            {
                                AplicarMomento(_tracker, genderPredictions);
                            }

                            MostrarCaras(genderCopy, genderPredictions);
                        }
                        catch (Exception ex)
                        {

                            Debug.WriteLine(ex );
                        }
                    }
                    genderCopy.Dispose();


                    foreach (var rec in skinRecs)
                    {
                        if (_showObjects)
                        {
                            CvInvoke.Rectangle(image.Mat, rec, SCALAR_GREEN, 1);
                        }
                    }
                    // display tracker 
                    foreach (var pair in _tracker)
                    {
                        CvTrack b = pair.Value;

                        if (_showObjects)
                        {
                            CvInvoke.PutText(image.Mat, b.Id.ToString(), new Point((int)Math.Round(b.Centroid.X), 
                                (int)Math.Round(b.Centroid.Y)), FontFace.HersheyPlain,1.0, new MCvScalar(255.0, 255.0, 255.0));

                            Point p3 = new Point((int)Math.Round(b.Centroid.X), (int)Math.Round(b.Centroid.Y));
                            CvInvoke.Circle(image.Mat, p3, 3, SCALAR_GREEN, 3);
                        }

                    }

                    DisplayImage(image.Mat, _personas);
                }
      


            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex );
            }

        }


        /// <summary>
        /// Thread safe method to display image in a picturebox  
        /// </summary>
        /// <param name="Image"></param>
        private void DisplayImage(IImage Image, int personas)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        imageBox.Image = Image;
                        txtCantidad.Text = personas.ToString();
                        _cont++;
                        if (_capture != null && _capture.Ptr != IntPtr.Zero)
                        {
                            TimeSpan pos = TimeSpan.FromMilliseconds((long) _capture.GetCaptureProperty(CapProp.PosMsec));
                            lblFps.Text = String.Format("{0}x{1} {2:0.00} fps", _frameSize.Width, _frameSize.Height,
                                1000 * _cont / (double) _timeSinceStart.ElapsedMilliseconds);
                            lblPos.Text = String.Format("Pos: {0:hh\\:mm\\:ss}", pos);
                        }

                        if (_timeSinceStart.ElapsedMilliseconds > 5000)
                        {
                            _timeSinceStart.Restart();
                            _cont = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }));
            }
            catch (ObjectDisposedException disEx)
            {
            }
        }

        private void btArchivo_Click(object sender, EventArgs e)
        {
            if (dialogVideoFile.ShowDialog() == DialogResult.OK)
            {
                /*lblSelectedFile.Text = dialogSelectFile.FileName;
                nombreBasePredeterminado();
                TimeSpan duration = new Extractor().GetDuration(dialogSelectFile.FileName);
                pickerFin.Value = DateTime.Today + duration;
                btSelecBase.Enabled = true;*/
                cboxFuente.Text = dialogVideoFile.FileName;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _inTabHSV = tabControl1.SelectedIndex == 1;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// Convert the coordinates for the image's SizeMode.
        /// </summary>
        /// https://www.codeproject.com/script/articles/view.aspx?aid=859100
        /// http://csharphelper.com/blog/2014/10/select-parts-of-a-scaled-image-picturebox-different-sizemode-values-c/</a>
        /// <param name="pic"></param>
        /// <param name="X0">out X coordinate</param>
        /// <param name="Y0">out Y coordinate</param>
        /// <param name="x">atual coordinate</param>
        /// <param name="y">atual coordinate</param>
        public static void ConvertCoordinates(ImageBox pic,
            out int X0, out int Y0, int x, int y)
        {
            int pic_hgt = pic.ClientSize.Height;
            int pic_wid = pic.ClientSize.Width;
            int img_hgt = pic.Image.Size.Height;
            int img_wid = pic.Image.Size.Width;

            X0 = x;
            Y0 = y;
            switch (pic.SizeMode)
            {
                case PictureBoxSizeMode.AutoSize:
                case PictureBoxSizeMode.Normal:
                    // These are okay. Leave them alone.
                    break;
                case PictureBoxSizeMode.CenterImage:
                    X0 = x - (pic_wid - img_wid) / 2;
                    Y0 = y - (pic_hgt - img_hgt) / 2;
                    break;
                case PictureBoxSizeMode.StretchImage:
                    X0 = (int)(img_wid * x / (float)pic_wid);
                    Y0 = (int)(img_hgt * y / (float)pic_hgt);
                    break;
                case PictureBoxSizeMode.Zoom:
                    float pic_aspect = pic_wid / (float)pic_hgt;
                    float img_aspect = img_wid / (float)img_wid;
                    if (pic_aspect > img_aspect)
                    {
                        // The PictureBox is wider/shorter than the image.
                        Y0 = (int)(img_hgt * y / (float)pic_hgt);

                        // The image fills the height of the PictureBox.
                        // Get its width.
                        float scaled_width = img_wid * pic_hgt / img_hgt;
                        float dx = (pic_wid - scaled_width) / 2;
                        X0 = (int)((x - dx) * img_hgt / (float)pic_hgt);
                    }
                    else
                    {
                        // The PictureBox is taller/thinner than the image.
                        X0 = (int)(img_wid * x / (float)pic_wid);

                        // The image fills the height of the PictureBox.
                        // Get its height.
                        float scaled_height = img_hgt * pic_wid / img_wid;
                        float dy = (pic_hgt - scaled_height) / 2;
                        Y0 = (int)((y - dy) * img_wid / pic_wid);
                    }
                    break;
            }
        }


        private void HsvOnClick(ImageBox box, Point mouseLoc)
        {
            int px = 0;
            int py = 0;
            ConvertCoordinates(box, out px, out py, mouseLoc.X, mouseLoc.Y);
            hsvSample.X = px;
            hsvSample.Y = py;

        }

        private void imageBoxHue_Click_1(object sender, EventArgs e)
        {
             MouseEventArgs mouse = e as MouseEventArgs;
            if (mouse != null)
            {
                HsvOnClick(imageBoxHue,mouse.Location);
            }

        }

        private void imageBoxValue_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;
            if (mouse != null)
            {
                HsvOnClick(imageBoxValue,mouse.Location);
            }
        }

        private void imageBoxSat_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;
            if (mouse != null)
            {
                HsvOnClick(imageBoxSat,mouse.Location);
            }
        }

        private void ckMotion_CheckedChanged(object sender, EventArgs e)
        {
            _useMotion = ckMotion.Checked;
        }

        private void ckSkin_CheckedChanged(object sender, EventArgs e)
        {
            _useSkin = ckSkin.Checked;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveMRU();
            Debug.WriteLine(e.CloseReason);
            StopCapture();
        }

        private void StopCapture()
        {
            if (_capture != null)
            {
                try
                {
                    _capture.ImageGrabbed -= ProcessFrame; // quito el handler
                    Thread.Sleep(100); // para estar seguro que haya terminado el procesamiento
                    _capture.Stop();
                    _contar.DisposeCascades();
                    _capture.Dispose();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("StopCapture Ex: " + ex);
                }
            }

        }

        private void ForceGC(bool finalizers)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void gridCaras_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
 
        }

        private void imageBox_Resize(object sender, EventArgs e)
        {
            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                _capture?.Pause();
                Thread.Sleep(200);
                _capture?.Start();
            }

        }

        private void trackGeneroHistoria_ValueChanged(object sender, EventArgs e)
        {
            _generoHistoria = trackGeneroHistoria.Value;
            lblGeneroHistoria.Text = trackGeneroHistoria.Value.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargarModelosGenero();
            comboBox1.SelectedIndex = 0;
            cboxResBodies.SelectedIndex = 2;
            cboxResFaces.SelectedIndex = 1;
            trackBar1.Value = _precision;
            LoadMRU();
        }

        private void cboxGeneroModelo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboxGeneroModelo.SelectedIndex > -1)
                {
                    var modelo = cboxGeneroModelo.SelectedItem.ToString();
                    if (modelos.ContainsKey(modelo))
                    {
                        _genderDetector = new GenderDetector(modelos[modelo][0], modelos[modelo][1]);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el modelo");
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            
        }

        private void btContinue_Click(object sender, EventArgs e)
        {
            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                _capture.Start();
            }
            
        }

        private void btPause_Click(object sender, EventArgs e)
        {
            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                if (btPause.Text == "Continuar")
                {
                    _capture.Start();
                    btPause.Text = "Pausar";
                }
                else
                {
                    _capture.Pause();
                    btPause.Text = "Continuar";
                }
            }
        }

        private void btSeek_Click(object sender, EventArgs e)
        {
            TimeSpan seekPos = pickerInicio.Value.TimeOfDay;
            _capture.SetCaptureProperty(CapProp.PosMsec,(int)seekPos.TotalMilliseconds);
        }

        private void ckUseGender_CheckedChanged(object sender, EventArgs e)
        {
            _useGenderDetection = ckUseGender.Checked;
            if (!_useGenderDetection)
            {
                LimpiarCaras();
            }
        }

        private void ckShowROI_CheckedChanged(object sender, EventArgs e)
        {
            _showROI = ckShowROI.Checked;
        }

        private void ckShowCenters_CheckedChanged(object sender, EventArgs e)
        {
            _showObjects = ckShowCenters.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                _modoBusqueda = comboBox1.SelectedIndex;
                if (_modoBusqueda == 0)
                {
                    ckUseGender.Enabled = true;
                    _useGenderDetection = ckUseGender.Checked;
                }
                else
                {
                    ckUseGender.Enabled = false;
                    _useGenderDetection = false;
                }


                if (_modoBusqueda < 2)
                {
                    ckMotion.Enabled = ckSkin.Enabled = true;
                }
                else
                {
                    ckMotion.Enabled = ckSkin.Enabled = false;
                    ckUseGender.Enabled = false;
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            lblPrecision.Text = trackBar1.Value.ToString();
            _precision = trackBar1.Value;

        }

        private double[] _resoluciones = {1.0, 0.75, 0.5,0.33,0.25};

        private void ConfigRes()
        {
            if (_contar != null && cboxResFaces.SelectedIndex > -1)
            {
                _contar.ResizeFactorFaces = _resoluciones[cboxResFaces.SelectedIndex];
                Debug.WriteLine("Nueva res caras: " + _resoluciones[cboxResFaces.SelectedIndex]);
            }
            if (_contar != null && cboxResBodies.SelectedIndex > -1)
            {
                _contar.ResizeFactorBodies = _resoluciones[cboxResBodies.SelectedIndex];
                Debug.WriteLine("Nueva res cuerpos: " + _resoluciones[cboxResBodies.SelectedIndex]);
            }
        }

        private void cboxResFaces_SelectedIndexChanged(object sender, EventArgs e)
        {

            ConfigRes();
        }

        private void cboxResBodies_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigRes();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            if (_contar != null)
            {
                _contar.DetectionKeepTime = trackBar2.Value;
            }
            lblKeep.Text = trackBar2.Value + " frames";
        }

        private void ckShowHaar_CheckedChanged(object sender, EventArgs e)
        {
            if (_contar != null)
            {
                _contar.ColorHaarCascade = ckShowHaar.Checked;
            }
        }

        private void ckShowGender_CheckedChanged(object sender, EventArgs e)
        {
            _showGender = ckShowGender.Checked;
        }

        private void ckShowGenConfidence_CheckedChanged(object sender, EventArgs e)
        {
            _showGenderConfidence = ckShowGenConfidence.Checked;
        }
    }
}
