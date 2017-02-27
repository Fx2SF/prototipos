using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using Emgu.CV.Cvb;
using Emgu.CV.BgSegm;
using System.Threading;

namespace CuentaAutos
{
    public partial class Inicio : Form
    {

        private static VideoCapture _cameraCapture;
        private bool _inProgress;

        private static BackgroundSubtractor _fgDetector;
        private static Emgu.CV.Cvb.CvBlobDetector _blobDetector;
        private static Emgu.CV.Cvb.CvTracks _tracker;

        private Point _p1;
        private Point _p2;
        private Point _pt3;
        private Point _pt4;
        private Point _pt5;
        private Point _pt6;

        private MCvScalar SCALAR_BLACK = new MCvScalar(0.0, 0.0, 0.0);
        private MCvScalar SCALAR_WHITE = new MCvScalar(255.0, 255.0, 255.0);
        private MCvScalar SCALAR_BLUE = new MCvScalar(255.0, 0.0, 0.0);
        private MCvScalar SCALAR_GREEN = new MCvScalar(0.0, 255.0, 0.0);
        private MCvScalar SCALAR_RED = new MCvScalar(0.0, 0.0, 255.0);
        private MCvScalar SCALAR_ORANGE = new MCvScalar(0.0, 165.0, 255.0);
        private MCvScalar SCALAR_CYAN = new MCvScalar(255.0, 255.0, 0.0);
        private MCvScalar SCALAR_AMBER = new MCvScalar(0.0, 191.0, 255.0);

        private int _cont;
        private List<uint> _idBlobs;

        private int _areaMin;
        private int _areaMax;

        private float _track1;
        private uint _track2;
        private uint _track3;

        private int _direction;

        private Mat structuringElement;
        private int cantErode;

        private Mat imgThresh;
        private Mat forgroundMask;
        private Point p0;

        private int o1;
        private int o2;
        private int o3;

        private LineSegment2D l2;
        private LineSegment2D l1;
        private LineSegment2D line;

        private Size tam5 = new Size(5, 5);

        private bool log;
        private bool sleep;

        // color
        private bool color;
        private int radio;
        private int colorMin;
        private Mat HsvFrame;

        //red-------->
        private int h2 = 160;
        private int h2m = 180;
        private int s2 = 70;
        private int s2m = 255;
        private int v2 = 42;
        private int v2m = 255;

        //black------->    
        private int bh2 = 0;
        private int bh2m = 179;
        private int bs2 = 0;
        private int bs2m = 65;
        private int bv2 = 0;
        private int bv2m = 144;

        //white--------->
        private int wh2 = 0;
        private int wh2m = 180;
        private int ws2 = 0;
        private int ws2m = 50;
        private int wv2 = 145;
        private int wv2m = 255;

        //yellow--------->
        private int yh2 = 20;
        private int yh2m = 30;
        private int ys2 = 100;
        private int ys2m = 255;
        private int yv2 = 100;
        private int yv2m = 255;

        private double fps;

        // carriles
        private bool carriles;
        private List<LineSegment2D> _listaCar;
        private List<int> _cantCar;
        private int cantCarriles;
        private double minCarriles;
        private string salida = "";
        private int tope = 0;
        private int topeCarriles = 0;

        // velocidad
        private bool velocidad;
        private Point _pV1;
        private Point _pV2;
        private Dictionary<uint, uint> _listaVelocidades;
        private LineSegment2D lineaVelocidad;
        private float velocidadKm;
        private LineSegment2D lineaVelocidadP1;
        private LineSegment2D lineaVelocidadP2;

        private Point _pt3V1;
        private Point _pt4V1;
        private Point _pt5V2;
        private Point _pt6V2;

        // frame actual
        private int index = 0;

        public Inicio()
        {
            InitializeComponent();

            string[] videos = new string[] { "CarsDrivingUnderBridge", "vid2", "RamblaYJackson", "lateral" };
            comboBox1.Items.AddRange(videos);

            string[] shapes = new string[] { "Rectangle", "Ellipse", "Cross" };
            comboBoxShape.Items.AddRange(shapes);
            comboBoxShape.Text = "Rectangle";
            textBoxErodeSize.Text = "7";
            textBoxErodeCant.Text = "3";

            textBoxTrackerInac.Text = "5";
            textBoxTrackerActive.Text = "5";
            textBoxTrackerDist.Text = "0,01";

            groupBox5.Enabled = false;
            groupBox10.Enabled = false;
            groupBox6.Enabled = false;
            groupBox13.Enabled = false;
            textBox1.Text = "";

            forgroundMask = new Mat();
            imgThresh = new Mat();
            p0 = new Point(-1, -1);

            textBoxO1Min.Enabled = false;
            textBoxO1Max.Enabled = false;
            textBoxO1Name.Enabled = false;
            textBoxO2Min.Enabled = false;
            textBoxO2Max.Enabled = false;
            textBoxO2Name.Enabled = false;
            textBoxO3Min.Enabled = false;
            textBoxO3Max.Enabled = false;
            textBoxO3Name.Enabled = false;

            richTextBoxLog.Enabled = false;
            log = false;

            color = false;
            radio = 0;
            checkBoxColor.Checked = false;
            textBoxRadio.Enabled = false;
            textBoxColorMin.Enabled = false;

            textBoxCantCarriles.Enabled = false;
            textBoxTiempoCarriles.Enabled = false;

            textBoxX1Velocidad.Enabled = false;
            textBoxY1Velocidad.Enabled = false;
            textBoxX2Velocidad.Enabled = false;
            textBoxY2Velocidad.Enabled = false;

            labelTiempoCarril.Text = "";
            labelTiempoCarrilMin.Text = "";

            labelO1.Text = "";
            labelO2.Text = "";
            labelO3.Text = "";

        }

        // Abrir
        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Seleccione un video.";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog1.FileName.ToString();
            }
        }

        // Reproducir
        private void button2_Click(object sender, EventArgs e)
        {

           try
            {
                if (textBox1.Text.ToString().CompareTo("") != 0)
                {

                    _cameraCapture = new VideoCapture(this.textBox1.Text.ToString());
                    _inProgress = true;
                    _cont = 0;
                    _idBlobs = new List<uint>();

                    //---------
                    HsvFrame = new Mat();
                    fps = _cameraCapture.GetCaptureProperty(CapProp.Fps);

                    _areaMin = int.Parse(textBoxA1min.Text);
                    _areaMax = int.Parse(textBoxA1max.Text);

                    _track1 = float.Parse(textBoxTrackerDist.Text);
                    _track2 = uint.Parse(textBoxTrackerInac.Text);
                    _track3 = uint.Parse(textBoxTrackerActive.Text);

                    labelTiempoCarril.Text = "";
                    labelTiempoCarrilMin.Text = "";

                    sleep = checkBoxSleep.Checked;

                    if (radioButtonDir1.Checked == true)
                    {

                        _direction = 1;
                    }
                    else if (radioButtonDir2.Checked == true)
                    {
                        _direction = 2;
                    }
                    else if (radioButtonDir3.Checked == true)
                    {
                        _direction = 3;
                    }
                    else
                    {
                        _direction = 4;
                    }



                    if (radioButtonMOG.Checked == true)
                    {
                        _fgDetector = new BackgroundSubtractorMOG(int.Parse(textBoxMOGhistory.Text), int.Parse(textBoxMOGnMix.Text), double.Parse(textBoxMOGRatio.Text), double.Parse(textBoxNSigma.Text));
                    }
                    else if (radioButtonMOG2.Checked == true)
                    {
                        _fgDetector = new BackgroundSubtractorMOG2(int.Parse(textBoxMOG2history.Text), int.Parse(textBoxMOG2thres.Text), checkBoxMOG2Shadow.Checked);
                    }
                    else if (radioButtonKNN.Checked == true)
                    {
                        _fgDetector = new BackgroundSubtractorKNN(int.Parse(textBoxKNNhistory.Text), double.Parse(textBoxKNNDist2.Text), checkBoxKNNShadow.Checked);
                    }
                    else if (radioButtonGMG.Checked == true)
                    {
                        _fgDetector = new BackgroundSubtractorGMG(int.Parse(textBoxGMG1.Text), double.Parse(textBoxGMG2.Text));
                    }

                    _blobDetector = new CvBlobDetector();
                    _tracker = new CvTracks();

                    float x1 = float.Parse(textX1.Text);
                    float y1 = float.Parse(textY1.Text);
                    float x2 = float.Parse(textX2.Text);
                    float y2 = float.Parse(textY2.Text);

                    _p1 = new Point((int)(_cameraCapture.Width * x1), (int)(_cameraCapture.Height * y1));
                    _p2 = new Point((int)(_cameraCapture.Width * x2), (int)(_cameraCapture.Height * y2));

                    string shape = comboBoxShape.Text;
                    int size = int.Parse(textBoxErodeSize.Text);
                    cantErode = int.Parse(textBoxErodeCant.Text);

                    if (shape.CompareTo("Rectangle") == 0)
                    {
                        structuringElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(size, size), p0);
                    }
                    else if (shape.CompareTo("Ellipse") == 0)
                    {
                        structuringElement = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(size, size), p0);
                    }
                    else if (shape.CompareTo("Cross") == 0)
                    {

                        structuringElement = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(size, size), p0);
                    }

                    o1 = 0;
                    o2 = 0;
                    o3 = 0;

                    if (checkBoxO1.Checked == true)
                    {
                        labelO1.Text = textBoxO1Name.Text + ": 0";
                    }
                    else
                    {
                        labelO1.Text = "";
                    }
                    if (checkBoxO2.Checked == true)
                    {
                        labelO2.Text = textBoxO2Name.Text + ": 0";
                    }
                    else
                    {
                        labelO2.Text = "";
                    }
                    if (checkBoxO3.Checked == true)
                    {
                        labelO3.Text = textBoxO3Name.Text + ": 0";
                    }
                    else
                    {
                        labelO3.Text = "";
                    }

                    log = checkBoxLog.Checked;
                    richTextBoxLog.Clear();

                    // --------------------->>>

                    line = new LineSegment2D(_p1, _p2);

                    double vx = _p2.X - _p1.X;
                    double vy = _p2.Y - _p1.Y;
                    double mag = Math.Sqrt(vx * vx + vy * vy);
                    vx = (int)vx / mag;
                    vy = vy / mag;

                    double temp = vx;
                    vx = vy;
                    vy = -temp;

                    int length = 10;

                    int cx = (int)(_p2.X + vx * length);
                    int cy = (int)(_p2.Y + vy * length);

                    int dx = (int)(_p2.X + vx * -length);
                    int dy = (int)(_p2.Y + vy * -length);

                    _pt3 = new Point(cx, cy);
                    _pt4 = new Point(dx, dy);
                    l1 = new LineSegment2D(_pt3, _pt4);

                    // ---------

                    double vx2 = _p2.X - _p1.X;
                    double vy2 = _p1.Y - _p2.Y;
                    double mag2 = Math.Sqrt(vx2 * vx2 + vy2 * vy2);
                    vx2 = (int)vx2 / mag2;
                    vy2 = vy2 / mag2;

                    double temp2 = vx2;
                    vx2 = vy2;
                    vy2 = -temp2;

                    int length2 = 10;

                    int cx2 = (int)(_p1.X + vx2 * length2);
                    int cy2 = (int)(_p1.Y + vy2 * length2);

                    int dx2 = (int)(_p1.X + vx2 * -length2);
                    int dy2 = (int)(_p1.Y + vy2 * -length2);

                    _pt5 = new Point(cx2, cy2);
                    _pt6 = new Point(dx2, dy2);
                    l2 = new LineSegment2D(_pt5, _pt6);

                    // color
                    if (checkBoxColor.Checked)
                    {
                        color = checkBoxColor.Checked;
                        radio = int.Parse(textBoxRadio.Text);
                        colorMin = int.Parse(textBoxColorMin.Text);

                    }

                    // colores
                    richTextBoxColores.Clear();

                    // carriles
                    if (checkBoxCarriles.Checked)
                    {
                        carriles = checkBoxCarriles.Checked;
                        cantCarriles = int.Parse(textBoxCantCarriles.Text);
                        minCarriles = double.Parse(textBoxTiempoCarriles.Text);
                        _listaCar = new List<LineSegment2D>();
                        _cantCar = new List<int>();
                    }

                    // carriles
                    if (carriles)
                    {

                        int distancia = Convert.ToInt32(Math.Sqrt((_p1.X - _p2.X) * (_p1.X - _p2.X) + ((_p1.Y - _p2.Y) * (_p1.Y - _p2.Y))));
                        int d = Convert.ToInt32(distancia / cantCarriles);

                        // -----
                        double vx2K = _p2.X - _p1.X;
                        double vy2K = _p1.Y - _p2.Y;
                        double mag2K = Math.Sqrt(vx2K * vx2K + vy2K * vy2K);
                        vx2K = (int)vx2K / mag2K;
                        vy2K = vy2K / mag2K;

                        double temp2K = vx2K;
                        vx2K = vy2K;
                        vy2K = -temp2K;

                        int length2K = 10;

                        int cx2K = (int)(_p1.X + vx2K * length2K);
                        int cy2K = (int)(_p1.Y + vy2K * length2K);

                        int dx2K = (int)(_p1.X + vx2K * -length2K);
                        int dy2K = (int)(_p1.Y + vy2K * -length2K);

                        Point _pt5C = new Point(cx2K, cy2K);
                        Point _pt6C = new Point(dx2K, dy2K);
                        LineSegment2D l2C = new LineSegment2D(_pt5C, _pt6C);
                        _listaCar.Add(l2C);
                        _cantCar.Add(0);
                        // -----

                        for (int i = 1; i <= cantCarriles; i++)
                        {

                            int Xc = Convert.ToInt32(_p1.X - ((d * i * (_p1.X - _p2.X)) / distancia));
                            int Yc = Convert.ToInt32(_p1.Y - ((d * i * (_p1.Y - _p2.Y)) / distancia));
                            Point _pc = new Point(Xc, Yc);

                            // ----------------

                            double vx1 = _pc.X - _p1.X;
                            double vy1 = _pc.Y - _p1.Y;
                            double mag1 = Math.Sqrt(vx1 * vx1 + vy1 * vy1);
                            vx1 = (int)vx1 / mag1;
                            vy1 = vy1 / mag1;

                            double temp1 = vx1;
                            vx1 = vy1;
                            vy1 = -temp1;

                            int cx1 = (int)(_pc.X + vx1 * length2);
                            int cy1 = (int)(_pc.Y + vy1 * length2);

                            int dx1 = (int)(_pc.X + vx1 * -length2);
                            int dy1 = (int)(_pc.Y + vy1 * -length2);

                            Point _pt30 = new Point(cx1, cy1);
                            Point _pt40 = new Point(dx1, dy1);
                            LineSegment2D l3C = new LineSegment2D(_pt30, _pt40);

                            _listaCar.Add(l3C);
                            _cantCar.Add(0);

                        }
                        // paso a segundos
                        int segundos = Convert.ToInt32(minCarriles * 60);
                        topeCarriles = Convert.ToInt32(segundos * fps);
                        tope = topeCarriles;
                    }
                    // velocidad
                    if (checkBoxVelocidad.Checked)
                    {
                        velocidad = true;

                        float x11 = float.Parse(textBoxX1Velocidad.Text);
                        float y11 = float.Parse(textBoxY1Velocidad.Text);
                        float x21 = float.Parse(textBoxX2Velocidad.Text);
                        float y21 = float.Parse(textBoxY2Velocidad.Text);

                        _pV1 = new Point((int)(_cameraCapture.Width * x11), (int)(_cameraCapture.Height * y11));
                        _pV2 = new Point((int)(_cameraCapture.Width * x21), (int)(_cameraCapture.Height * y21));

                        _listaVelocidades = new Dictionary<uint, uint>();

                        velocidadKm = float.Parse(textBoxVelocidadDistancia.Text);
                        richTextBoxVelocidades.Text = "";

                        lineaVelocidad = new LineSegment2D(_pV1, _pV2);

                        // 1° perpendicular
                        double vxV1 = _pV2.X - _pV1.X;
                        double vyV1 = _pV2.Y - _pV1.Y;
                        double magV1 = Math.Sqrt(vxV1 * vxV1 + vyV1 * vyV1);
                        vxV1 = (int)vxV1 / magV1;
                        vyV1 = vyV1 / magV1;

                        double tempV1 = vxV1;
                        vxV1 = vyV1;
                        vyV1 = -tempV1;

                        int lengthV1 = 10;

                        int cxV1 = (int)(_pV2.X + vxV1 * lengthV1);
                        int cyV1 = (int)(_pV2.Y + vyV1 * lengthV1);

                        int dxV1 = (int)(_pV2.X + vxV1 * -lengthV1);
                        int dyV1 = (int)(_pV2.Y + vyV1 * -lengthV1);

                        _pt3V1 = new Point(cxV1, cyV1);
                        _pt4V1 = new Point(dxV1, dyV1);
                        lineaVelocidadP1 = new LineSegment2D(_pt3V1, _pt4V1);

                        // 2° perpendicular

                        double vx2V2 = _pV2.X - _pV1.X;
                        double vy2V2 = _pV1.Y - _pV2.Y;
                        double mag2V2 = Math.Sqrt(vx2V2 * vx2V2 + vy2V2 * vy2V2);
                        vx2V2 = (int)vx2V2 / mag2V2;
                        vy2V2 = vy2V2 / mag2V2;

                        double temp2V2 = vx2V2;
                        vx2V2 = vy2V2;
                        vy2V2 = -temp2V2;

                        int length2V2 = 10;

                        int cx2V2 = (int)(_pV1.X + vx2V2 * length2V2);
                        int cy2V2 = (int)(_pV1.Y + vy2V2 * length2V2);

                        int dx2V2 = (int)(_pV1.X + vx2V2 * -length2V2);
                        int dy2V2 = (int)(_pV1.Y + vy2V2 * -length2V2);

                        _pt5V2 = new Point(cx2V2, cy2V2);
                        _pt6V2 = new Point(dx2V2, dy2V2);
                        lineaVelocidadP2 = new LineSegment2D(_pt5V2, _pt6V2);

                    }

                    Application.Idle += ProcessFrame;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Primero debe abrir un video.", "Cuidado.");
                }

            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }          
        }

        // Detener
        private void button3_Click(object sender, EventArgs e)
        {

            if (_cameraCapture != null)
            {
                if (_inProgress)
                {
                    _cameraCapture.Dispose();
                    _inProgress = false;
                    imageBox1.Image = null;
                    imageBox2.Image = null;
                    GC.Collect();
                }
            }

        }


        void ProcessFrame(object sender, EventArgs e)
        {

            if (_cameraCapture != null && _cameraCapture.Ptr != IntPtr.Zero)
            {
                Mat frame = _cameraCapture.QueryFrame();

                if (frame != null)
                {
                    index = Convert.ToInt32(_cameraCapture.GetCaptureProperty(CapProp.PosFrames));
                  
                    Mat clone = frame.Clone();

                    Mat smoothedFrame = frame.Clone();
                    CvInvoke.GaussianBlur(smoothedFrame, imgThresh, tam5, 0);
                    _fgDetector.Apply(imgThresh, forgroundMask);

                    for(int i = 0; i < cantErode; i++){
                        CvInvoke.Dilate(forgroundMask, forgroundMask, structuringElement, p0, 1, BorderType.Default, SCALAR_BLACK);
                    }
                    for (int i = 0; i < cantErode; i++){
                        CvInvoke.Erode(forgroundMask, forgroundMask, structuringElement, p0, 1, BorderType.Default, SCALAR_BLACK);
                    }

                    CvBlobs blobs = new CvBlobs();
                    _blobDetector.Detect(forgroundMask.ToImage<Gray, byte>(), blobs);
                    blobs.FilterByArea(_areaMin, _areaMax);

                    float scale = (frame.Width + frame.Width) / 2.0f;
                    _tracker.Update(blobs, _track1 * scale, _track2, _track3);

                    foreach (var pair in _tracker)
                    {
                       CvTrack b = pair.Value;

                       CvInvoke.Rectangle(frame, b.BoundingBox, SCALAR_RED, 2);
                      // CvInvoke.PutText(frame, b.Id.ToString(), new Point((int)Math.Round(b.Centroid.X), (int)Math.Round(b.Centroid.Y)), FontFace.HersheyPlain, 1.0, new MCvScalar(255.0, 255.0, 255.0));

                       Point p3 = new Point((int)Math.Round(b.Centroid.X), (int)Math.Round(b.Centroid.Y));
                       CvInvoke.Circle(frame, p3, 3, SCALAR_GREEN, 3);

                        if (_direction == 1) // Abajo --> Arriba
                        {
                             if (line.Side(p3) == 1 && !_idBlobs.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == -1)
                            {
                                _idBlobs.Add(b.Id);
                            }

                            if (line.Side(p3) == -1 && _idBlobs.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == -1)
                            {
                                _idBlobs.RemoveAll(item => item == b.Id);
                                _cont++;

                                if (color)
                                {
                                    CvInvoke.Circle(frame, p3, radio, SCALAR_ORANGE, 4);

                                    int redHSV = 0;
                                    int blackHSV = 0;
                                    int whiteHSV = 0; 
                                    int yellowHSV = 0;
                                    int x1 = 0;
                                    int y1 = 0;
                                    for (int i = 0; i < 9; i++)
                                    {

                                        if (i == 0)
                                        {
                                            x1 = p3.X + radio;
                                            y1 = p3.Y;
                                        }
                                        else if (i == 1)
                                        {
                                            x1 = p3.X - radio;
                                            y1 = p3.Y;

                                        }
                                        else if (i == 2)
                                        {
                                            x1 = p3.X;
                                            y1 = p3.Y + radio;

                                        }
                                        else if (i == 3)
                                        {
                                            x1 = p3.X;
                                            y1 = p3.Y - radio;

                                        }else if (i == 4)
                                        {
                                            // pm entre 0 y 2
                                            x1 = ((p3.X + radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y + radio)) / 2;

                                        }else if (i == 5)
                                        {
                                            // pm entre 2 y 1 
                                            x1 = ((p3.X) + (p3.X - radio)) / 2;
                                            y1 = ((p3.Y + radio) + (p3.Y)) / 2;

                                        }
                                        else if (i == 6)
                                        {
                                            // pm entre  1 y 3
                                            x1 = ((p3.X - radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y - radio)) / 2;

                                        }else if (i == 7)
                                        {
                                            // pm entre  0 y 3
                                            x1 = ((p3.X + radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y - radio)) / 2;

                                        }else if (i == 8)
                                        {
                                            // centro
                                            x1 = p3.X;
                                            y1 = p3.Y;
                                        }

                                        if (0 <= x1 && x1 <= frame.Width && 0 <= y1 && y1 <= frame.Height)
                                        {

                                            CvInvoke.Circle(frame, new Point(x1, y1), 3, SCALAR_GREEN, -1);
                                            CvInvoke.CvtColor(clone, HsvFrame, ColorConversion.Bgr2Hsv);
                                            Hsv hsvColor = HsvFrame.ToImage<Hsv, Byte>()[y1, x1];

                                            if (hsvColor.Hue >= h2 && hsvColor.Hue <= h2m && hsvColor.Satuation >= s2
                                                && hsvColor.Satuation <= s2m && hsvColor.Value >= v2 && hsvColor.Value <= v2m)
                                            {
                                                redHSV++;

                                            }
                                            else if (hsvColor.Hue >= bh2 && hsvColor.Hue <= bh2m && hsvColor.Satuation >= bs2
                                                && hsvColor.Satuation <= bs2m && hsvColor.Value >= bv2 && hsvColor.Value <= bv2m)
                                            {
                                                blackHSV++;
                                            }
                                            else if (hsvColor.Hue >= wh2 && hsvColor.Hue <= wh2m && hsvColor.Satuation >= ws2
                                                && hsvColor.Satuation <= ws2m && hsvColor.Value >= wv2 && hsvColor.Value <= wv2m)
                                            {
                                                whiteHSV++;
                                            }
                                            else if (hsvColor.Hue >= yh2 && hsvColor.Hue <= yh2m && hsvColor.Satuation >= ys2
                                                 && hsvColor.Satuation <= ys2m && hsvColor.Value >= yv2 && hsvColor.Value <= yv2m)
                                            {
                                                yellowHSV++;
                                            }
                                        }
                                    }


                                    if (redHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Rojo" + "\n";

                                    }else if (blackHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Negro" + "\n";

                                    }
                                    else if (whiteHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Blanco" + "\n";

                                    }
                                    else if (yellowHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Amarillo" + "\n";

                                    }

                                }

                                //carriles
                                if (carriles)
                                {
                                    // 2 carriles
                                    if (_listaCar.Count <= 3)
                                    {
                                        if (_listaCar[0].Side(p3) == -1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 3 carriles
                                    else if (_listaCar.Count <= 4)
                                    {
                                        if (_listaCar[0].Side(p3) == -1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {

                                            _cantCar[2]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 4 carriles
                                    else if (_listaCar.Count <= 5)
                                    {
                                        if (_listaCar[0].Side(p3) == -1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {
                                            _cantCar[2]++;
                                        }
                                        else if (_listaCar[3].Side(p3) == -1 && _listaCar[4].Side(p3) == 1)
                                        {
                                            _cantCar[3]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 5 carriles
                                    else if (_listaCar.Count <= 6)
                                    {

                                        if (_listaCar[0].Side(p3) == -1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;

                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {
                                            _cantCar[2]++;
                                        }
                                        else if (_listaCar[3].Side(p3) == -1 && _listaCar[4].Side(p3) == 1)
                                        {
                                            _cantCar[3]++;
                                        }
                                        else if (_listaCar[4].Side(p3) == -1 && _listaCar[5].Side(p3) == 1)
                                        {
                                            _cantCar[4]++;
                                        }

                                        updateInfoCarril();

                                    }
                                }

                                if (velocidad)
                                {
                                    if ( !(_listaVelocidades.ContainsKey(b.Id)) && lineaVelocidad.Side(p3) == 1 
                                        && lineaVelocidadP1.Side(p3) == 1 && lineaVelocidadP2.Side(p3) == -1)
                                    {
                                        _listaVelocidades.Add(b.Id, b.Lifetime);
                                    }
                                }

                                int area = b.BoundingBox.Width * b.BoundingBox.Height;

                                if (log)
                                {
                                    richTextBoxLog.Text = richTextBoxLog.Text + _cont.ToString() + ": " + area.ToString() + "\n";
                                }

                                if (checkBoxO1.Checked == true)
                                {
                                    int max1 = int.Parse(textBoxO1Max.Text);
                                    int min1 = int.Parse(textBoxO1Min.Text);

                                    if ((area < max1) && (area >= min1))
                                    {
                                        o1++;
                                        labelO1.Text = textBoxO1Name.Text + ": " + o1.ToString();
                                    }
                                }

                                if (checkBoxO2.Checked == true)
                                {
                                    int max2 = int.Parse(textBoxO2Max.Text);
                                    int min2 = int.Parse(textBoxO2Min.Text);

                                    if ((area < max2) && (area >= min2))
                                    {
                                        o2++;
                                        labelO2.Text = textBoxO2Name.Text + ": " + o2.ToString();
                                    }
                                }

                                if (checkBoxO3.Checked == true)
                                {
                                    int max3 = int.Parse(textBoxO3Max.Text);
                                    int min3 = int.Parse(textBoxO3Min.Text);
                                    if ((area < max3) && (area >= min3))
                                    {

                                        o3++;
                                        labelO3.Text = textBoxO3Name.Text + ": " + o3.ToString();

                                    }
                                }
                            }

                            if (velocidad)
                            {

                                if (_listaVelocidades.ContainsKey(b.Id) && lineaVelocidad.Side(p3) == -1
                                    && lineaVelocidadP1.Side(p3) == 1 && lineaVelocidadP2.Side(p3) == -1)
                                {

                                    // calculo velocidad

                                    uint vida = _listaVelocidades[b.Id];

                                    // cantidad de frames que tardo en pasar de una linea a la otra.
                                    uint diferencia = b.Lifetime - vida;

                                    double segundos = diferencia / fps;
                                    double horas = segundos / 3600;

                                    double kmh = velocidadKm / horas;
                                    richTextBoxVelocidades.Text = richTextBoxVelocidades.Text
                                        + kmh.ToString("0.#") + "\n";

                                    _listaVelocidades.Remove(b.Id);
                                }

                            }

                        }
                        else if (_direction == 2) // izquierda --> derecha
                        {

                            if (line.Side(p3) == 1 && !_idBlobs.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == 1)
                            {
                                _idBlobs.Add(b.Id);
                            }

                            if (line.Side(p3) == -1 && _idBlobs.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == 1)
                            {
                                _idBlobs.RemoveAll(item => item == b.Id);
                                _cont++;

                                if (color)
                                {
                                    CvInvoke.Circle(frame, p3, radio, SCALAR_ORANGE, 4);

                                    int redHSV = 0;
                                    int blackHSV = 0;
                                    int whiteHSV = 0;
                                    int yellowHSV = 0;
                                    int x1 = 0;
                                    int y1 = 0;
                                    for (int i = 0; i < 9; i++)
                                    {

                                        if (i == 0)
                                        {
                                            x1 = p3.X + radio;
                                            y1 = p3.Y;
                                        }
                                        else if (i == 1)
                                        {
                                            x1 = p3.X - radio;
                                            y1 = p3.Y;

                                        }
                                        else if (i == 2)
                                        {
                                            x1 = p3.X;
                                            y1 = p3.Y + radio;

                                        }
                                        else if (i == 3)
                                        {
                                            x1 = p3.X;
                                            y1 = p3.Y - radio;

                                        }
                                        else if (i == 4)
                                        {
                                            // pm entre 0 y 2
                                            x1 = ((p3.X + radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y + radio)) / 2;

                                        }
                                        else if (i == 5)
                                        {
                                            // pm entre 2 y 1 
                                            x1 = ((p3.X) + (p3.X - radio)) / 2;
                                            y1 = ((p3.Y + radio) + (p3.Y)) / 2;

                                        }
                                        else if (i == 6)
                                        {
                                            // pm entre  1 y 3
                                            x1 = ((p3.X - radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y - radio)) / 2;

                                        }
                                        else if (i == 7)
                                        {
                                            // pm entre  0 y 3
                                            x1 = ((p3.X + radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y - radio)) / 2;

                                        }
                                        else if (i == 8)
                                        {
                                            // centro
                                            x1 = p3.X;
                                            y1 = p3.Y;
                                        }

                                        if (0 <= x1 && x1 <= frame.Width && 0 <= y1 && y1 <= frame.Height)
                                        {

                                            CvInvoke.Circle(frame, new Point(x1, y1), 3, SCALAR_GREEN, -1);
                                            CvInvoke.CvtColor(clone, HsvFrame, ColorConversion.Bgr2Hsv);
                                            Hsv hsvColor = HsvFrame.ToImage<Hsv, Byte>()[y1, x1];

                                            if (hsvColor.Hue >= h2 && hsvColor.Hue <= h2m && hsvColor.Satuation >= s2
                                                && hsvColor.Satuation <= s2m && hsvColor.Value >= v2 && hsvColor.Value <= v2m)
                                            {
                                                redHSV++;

                                            }
                                            else if (hsvColor.Hue >= bh2 && hsvColor.Hue <= bh2m && hsvColor.Satuation >= bs2
                                                && hsvColor.Satuation <= bs2m && hsvColor.Value >= bv2 && hsvColor.Value <= bv2m)
                                            {
                                                blackHSV++;
                                            }
                                            else if (hsvColor.Hue >= wh2 && hsvColor.Hue <= wh2m && hsvColor.Satuation >= ws2
                                                && hsvColor.Satuation <= ws2m && hsvColor.Value >= wv2 && hsvColor.Value <= wv2m)
                                            {
                                                whiteHSV++;
                                            }
                                            else if (hsvColor.Hue >= yh2 && hsvColor.Hue <= yh2m && hsvColor.Satuation >= ys2
                                                 && hsvColor.Satuation <= ys2m && hsvColor.Value >= yv2 && hsvColor.Value <= yv2m)
                                            {
                                                yellowHSV++;
                                            }
                                        }
                                    }

                                    if (redHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Rojo" + "\n";
                                    }
                                    else if (blackHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Negro" + "\n";

                                    }
                                    else if (whiteHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Blanco" + "\n";

                                    }
                                    else if (yellowHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Amarillo" + "\n";
                                    }
                                }

                                //carriles
                                if (carriles)
                                {
                                    // 2 carriles
                                    if (_listaCar.Count <= 3)
                                    {

                                        if (_listaCar[0].Side(p3) == 1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 3 carriles
                                    else if (_listaCar.Count <= 4)
                                    {
                                        if (_listaCar[0].Side(p3) == 1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {
                                            _cantCar[2]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 4 carriles
                                    else if (_listaCar.Count <= 5)
                                    {

                                        if (_listaCar[0].Side(p3) == 1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {
                                            _cantCar[2]++;
                                        }
                                        else if (_listaCar[3].Side(p3) == -1 && _listaCar[4].Side(p3) == 1)
                                        {
                                            _cantCar[3]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 5 carriles
                                    else if (_listaCar.Count <= 6)
                                    {
                                        if (_listaCar[0].Side(p3) == 1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {
                                            _cantCar[2]++;
                                        }
                                        else if (_listaCar[3].Side(p3) == -1 && _listaCar[4].Side(p3) == 1)
                                        {
                                            _cantCar[3]++;
                                        }
                                        else if (_listaCar[4].Side(p3) == -1 && _listaCar[5].Side(p3) == 1)
                                        {
                                            _cantCar[4]++;
                                        }

                                        updateInfoCarril();

                                    }
                                }

                                if (velocidad)
                                {
                                    if (!(_listaVelocidades.ContainsKey(b.Id)) && lineaVelocidad.Side(p3) == 1
                                        && lineaVelocidadP1.Side(p3) == 1 && lineaVelocidadP2.Side(p3) == 1)
                                    {
                                        _listaVelocidades.Add(b.Id, b.Lifetime);
                                    }
                                }

                                int area = b.BoundingBox.Width * b.BoundingBox.Height;

                                if (log)
                                {
                                    richTextBoxLog.Text = richTextBoxLog.Text + _cont.ToString() + ": " + area.ToString() + "\n";
                                }

                                if (checkBoxO1.Checked == true)
                                {
                                    int max1 = int.Parse(textBoxO1Max.Text);
                                    int min1 = int.Parse(textBoxO1Min.Text);

                                    if ((area < max1) && (area >= min1))
                                    {
                                        o1++;
                                        labelO1.Text = textBoxO1Name.Text + ": " + o1.ToString();

                                    }
                                }

                                if (checkBoxO2.Checked == true)
                                {
                                    int max2 = int.Parse(textBoxO2Max.Text);
                                    int min2 = int.Parse(textBoxO2Min.Text);

                                    if ((area < max2) && (area >= min2))
                                    {
                                        o2++;
                                        labelO2.Text = textBoxO2Name.Text + ": " + o2.ToString();
                                    }
                                }

                                if (checkBoxO3.Checked == true)
                                {
                                    int max3 = int.Parse(textBoxO3Max.Text);
                                    int min3 = int.Parse(textBoxO3Min.Text);
                                    if ((area < max3) && (area >= min3))
                                    {
                                        o3++;
                                        labelO3.Text = textBoxO3Name.Text + ": " + o3.ToString();

                                    }
                                }
                            }

                            if (velocidad)
                            {
                                if (_listaVelocidades.ContainsKey(b.Id) && lineaVelocidad.Side(p3) == -1
                                    && lineaVelocidadP1.Side(p3) == 1 && lineaVelocidadP2.Side(p3) == 1)
                                {
                                    // calculo velocidad

                                    uint vida = _listaVelocidades[b.Id];

                                    // cantidad de frames que tardo en pasar de una linea a la otra.
                                    uint diferencia = b.Lifetime - vida;

                                    double segundos = diferencia / fps;
                                    double horas = segundos / 3600;

                                    double kmh = velocidadKm / horas;
                                    richTextBoxVelocidades.Text = richTextBoxVelocidades.Text
                                        + kmh.ToString("0.#") + "\n";

                                    _listaVelocidades.Remove(b.Id);
                                }
                            }
                        }
                        else if (_direction == 3) // Arriba -> Abajo
                        {

                            if (line.Side(p3) == -1 && !_idBlobs.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == -1)
                            {
                                _idBlobs.Add(b.Id);
                            }

                            if (line.Side(p3) == 1 && _idBlobs.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == -1)
                            {
                                _idBlobs.RemoveAll(item => item == b.Id);
                                _cont++;

                                if (color)
                                {
                                    CvInvoke.Circle(frame, p3, radio, SCALAR_ORANGE, 4);

                                    int redHSV = 0;
                                    int blackHSV = 0;
                                    int whiteHSV = 0;
                                    int yellowHSV = 0;
                                    int x1 = 0;
                                    int y1 = 0;
                                    for (int i = 0; i < 9; i++)
                                    {

                                        if (i == 0)
                                        {
                                            x1 = p3.X + radio;
                                            y1 = p3.Y;
                                        }
                                        else if (i == 1)
                                        {
                                            x1 = p3.X - radio;
                                            y1 = p3.Y;

                                        }
                                        else if (i == 2)
                                        {
                                            x1 = p3.X;
                                            y1 = p3.Y + radio;

                                        }
                                        else if (i == 3)
                                        {
                                            x1 = p3.X;
                                            y1 = p3.Y - radio;

                                        }
                                        else if (i == 4)
                                        {
                                            // pm entre 0 y 2
                                            x1 = ((p3.X + radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y + radio)) / 2;

                                        }
                                        else if (i == 5)
                                        {
                                            // pm entre 2 y 1 
                                            x1 = ((p3.X) + (p3.X - radio)) / 2;
                                            y1 = ((p3.Y + radio) + (p3.Y)) / 2;

                                        }
                                        else if (i == 6)
                                        {
                                            // pm entre  1 y 3
                                            x1 = ((p3.X - radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y - radio)) / 2;

                                        }
                                        else if (i == 7)
                                        {
                                            // pm entre  0 y 3
                                            x1 = ((p3.X + radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y - radio)) / 2;

                                        }
                                        else if (i == 8)
                                        {
                                            // centro
                                            x1 = p3.X;
                                            y1 = p3.Y;
                                        }

                                        if (0 <= x1 && x1 <= frame.Width && 0 <= y1 && y1 <= frame.Height)
                                        {

                                            CvInvoke.Circle(frame, new Point(x1, y1), 3, SCALAR_GREEN, -1);
                                            CvInvoke.CvtColor(clone, HsvFrame, ColorConversion.Bgr2Hsv);
                                            Hsv hsvColor = HsvFrame.ToImage<Hsv, Byte>()[y1, x1];

                                            if (hsvColor.Hue >= h2 && hsvColor.Hue <= h2m && hsvColor.Satuation >= s2
                                                && hsvColor.Satuation <= s2m && hsvColor.Value >= v2 && hsvColor.Value <= v2m)
                                            {
                                                redHSV++;

                                            }
                                            else if (hsvColor.Hue >= bh2 && hsvColor.Hue <= bh2m && hsvColor.Satuation >= bs2
                                                && hsvColor.Satuation <= bs2m && hsvColor.Value >= bv2 && hsvColor.Value <= bv2m)
                                            {
                                                blackHSV++;
                                            }
                                            else if (hsvColor.Hue >= wh2 && hsvColor.Hue <= wh2m && hsvColor.Satuation >= ws2
                                                && hsvColor.Satuation <= ws2m && hsvColor.Value >= wv2 && hsvColor.Value <= wv2m)
                                            {
                                                whiteHSV++;
                                            }
                                            else if (hsvColor.Hue >= yh2 && hsvColor.Hue <= yh2m && hsvColor.Satuation >= ys2
                                                 && hsvColor.Satuation <= ys2m && hsvColor.Value >= yv2 && hsvColor.Value <= yv2m)
                                            {
                                                yellowHSV++;
                                            }
                                        }
                                    }


                                    if (redHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Rojo" + "\n";

                                    }
                                    else if (blackHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Negro" + "\n";

                                    }
                                    else if (whiteHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Blanco" + "\n";

                                    }
                                    else if (yellowHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Amarillo" + "\n";

                                    }

                                }

                                //carriles
                                if (carriles)
                                {
                                    // 2 carriles
                                    if (_listaCar.Count <= 3)
                                    {
                                        if (_listaCar[0].Side(p3) == -1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 3 carriles
                                    else if (_listaCar.Count <= 4)
                                    {
                                        if (_listaCar[0].Side(p3) == -1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {

                                            _cantCar[2]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 4 carriles
                                    else if (_listaCar.Count <= 5)
                                    {
                                        if (_listaCar[0].Side(p3) == -1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {
                                            _cantCar[2]++;
                                        }
                                        else if (_listaCar[3].Side(p3) == -1 && _listaCar[4].Side(p3) == 1)
                                        {
                                            _cantCar[3]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 5 carriles
                                    else if (_listaCar.Count <= 6)
                                    {

                                        if (_listaCar[0].Side(p3) == -1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;

                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {
                                            _cantCar[2]++;
                                        }
                                        else if (_listaCar[3].Side(p3) == -1 && _listaCar[4].Side(p3) == 1)
                                        {
                                            _cantCar[3]++;
                                        }
                                        else if (_listaCar[4].Side(p3) == -1 && _listaCar[5].Side(p3) == 1)
                                        {
                                            _cantCar[4]++;
                                        }

                                        updateInfoCarril();

                                    }
                                }

                                if (velocidad)
                                {
                                    if (!(_listaVelocidades.ContainsKey(b.Id)) && lineaVelocidad.Side(p3) == -1
                                        && lineaVelocidadP1.Side(p3) == 1 && lineaVelocidadP2.Side(p3) == -1)
                                    {
                                        _listaVelocidades.Add(b.Id, b.Lifetime);
                                    }
                                }

                                int area = b.BoundingBox.Width * b.BoundingBox.Height;

                                if (log)
                                {
                                    richTextBoxLog.Text = richTextBoxLog.Text + _cont.ToString() + ": " + area.ToString() + "\n";
                                }

                                if (checkBoxO1.Checked == true)
                                {

                                    int max1 = int.Parse(textBoxO1Max.Text);
                                    int min1 = int.Parse(textBoxO1Min.Text);

                                    if ((area < max1) && (area >= min1))
                                    {

                                        o1++;
                                        labelO1.Text = textBoxO1Name.Text + ": " + o1.ToString();

                                    }
                                }

                                if (checkBoxO2.Checked == true)
                                {
                                    int max2 = int.Parse(textBoxO2Max.Text);
                                    int min2 = int.Parse(textBoxO2Min.Text);

                                    if ((area < max2) && (area >= min2))
                                    {

                                        o2++;
                                        labelO2.Text = textBoxO2Name.Text + ": " + o2.ToString();

                                    }
                                }

                                if (checkBoxO3.Checked == true)
                                {

                                    int max3 = int.Parse(textBoxO3Max.Text);
                                    int min3 = int.Parse(textBoxO3Min.Text);
                                    if ((area < max3) && (area >= min3))
                                    {

                                        o3++;
                                        labelO3.Text = textBoxO3Name.Text + ": " + o3.ToString();

                                    }
                                }
                            }

                            if (velocidad)
                            {

                                if (_listaVelocidades.ContainsKey(b.Id) && lineaVelocidad.Side(p3) == 1
                                    && lineaVelocidadP1.Side(p3) == 1 && lineaVelocidadP2.Side(p3) == -1)
                                {

                                    // calculo velocidad

                                    uint vida = _listaVelocidades[b.Id];

                                    // cantidad de frames que tardo en pasar de una linea a la otra.
                                    uint diferencia = b.Lifetime - vida;

                                    double segundos = diferencia / fps;
                                    double horas = segundos / 3600;

                                    double kmh = velocidadKm / horas;
                                    richTextBoxVelocidades.Text = richTextBoxVelocidades.Text
                                        + kmh.ToString("0.#") + "\n";

                                    _listaVelocidades.Remove(b.Id);
                                }
                            }
                        }
                        else {// _direction 4: Derecha -> Izquierda

                            if (line.Side(p3) == -1 && !_idBlobs.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == 1)
                            {
                                _idBlobs.Add(b.Id);
                            }

                            if (line.Side(p3) == 1 && _idBlobs.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == 1)
                            {
                                _idBlobs.RemoveAll(item => item == b.Id);
                                _cont++;

                                if (color)
                                {
                                    CvInvoke.Circle(frame, p3, radio, SCALAR_ORANGE, 4);

                                    int redHSV = 0;
                                    int blackHSV = 0;
                                    int whiteHSV = 0;
                                    int yellowHSV = 0;
                                    int x1 = 0;
                                    int y1 = 0;
                                    for (int i = 0; i < 9; i++)
                                    {

                                        if (i == 0)
                                        {
                                            x1 = p3.X + radio;
                                            y1 = p3.Y;
                                        }
                                        else if (i == 1)
                                        {
                                            x1 = p3.X - radio;
                                            y1 = p3.Y;

                                        }
                                        else if (i == 2)
                                        {
                                            x1 = p3.X;
                                            y1 = p3.Y + radio;

                                        }
                                        else if (i == 3)
                                        {
                                            x1 = p3.X;
                                            y1 = p3.Y - radio;

                                        }
                                        else if (i == 4)
                                        {
                                            // pm entre 0 y 2
                                            x1 = ((p3.X + radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y + radio)) / 2;

                                        }
                                        else if (i == 5)
                                        {
                                            // pm entre 2 y 1 
                                            x1 = ((p3.X) + (p3.X - radio)) / 2;
                                            y1 = ((p3.Y + radio) + (p3.Y)) / 2;

                                        }
                                        else if (i == 6)
                                        {
                                            // pm entre  1 y 3
                                            x1 = ((p3.X - radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y - radio)) / 2;

                                        }
                                        else if (i == 7)
                                        {
                                            // pm entre  0 y 3
                                            x1 = ((p3.X + radio) + (p3.X)) / 2;
                                            y1 = ((p3.Y) + (p3.Y - radio)) / 2;

                                        }
                                        else if (i == 8)
                                        {
                                            // centro
                                            x1 = p3.X;
                                            y1 = p3.Y;
                                        }

                                        if (0 <= x1 && x1 <= frame.Width && 0 <= y1 && y1 <= frame.Height)
                                        {

                                            CvInvoke.Circle(frame, new Point(x1, y1), 3, SCALAR_GREEN, -1);
                                            CvInvoke.CvtColor(clone, HsvFrame, ColorConversion.Bgr2Hsv);
                                            Hsv hsvColor = HsvFrame.ToImage<Hsv, Byte>()[y1, x1];

                                            if (hsvColor.Hue >= h2 && hsvColor.Hue <= h2m && hsvColor.Satuation >= s2
                                                && hsvColor.Satuation <= s2m && hsvColor.Value >= v2 && hsvColor.Value <= v2m)
                                            {
                                                redHSV++;

                                            }
                                            else if (hsvColor.Hue >= bh2 && hsvColor.Hue <= bh2m && hsvColor.Satuation >= bs2
                                                && hsvColor.Satuation <= bs2m && hsvColor.Value >= bv2 && hsvColor.Value <= bv2m)
                                            {
                                                blackHSV++;
                                            }
                                            else if (hsvColor.Hue >= wh2 && hsvColor.Hue <= wh2m && hsvColor.Satuation >= ws2
                                                && hsvColor.Satuation <= ws2m && hsvColor.Value >= wv2 && hsvColor.Value <= wv2m)
                                            {
                                                whiteHSV++;
                                            }
                                            else if (hsvColor.Hue >= yh2 && hsvColor.Hue <= yh2m && hsvColor.Satuation >= ys2
                                                 && hsvColor.Satuation <= ys2m && hsvColor.Value >= yv2 && hsvColor.Value <= yv2m)
                                            {
                                                yellowHSV++;
                                            }
                                        }
                                    }

                                    if (redHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Rojo" + "\n";
                                    }
                                    else if (blackHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Negro" + "\n";

                                    }
                                    else if (whiteHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Blanco" + "\n";

                                    }
                                    else if (yellowHSV >= colorMin)
                                    {
                                        richTextBoxColores.Text = richTextBoxColores.Text + _cont + " : " + "Amarillo" + "\n";
                                    }
                                }

                                //carriles
                                if (carriles)
                                {
                                    // 2 carriles
                                    if (_listaCar.Count <= 3)
                                    {

                                        if (_listaCar[0].Side(p3) == 1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 3 carriles
                                    else if (_listaCar.Count <= 4)
                                    {
                                        if (_listaCar[0].Side(p3) == 1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {
                                            _cantCar[2]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 4 carriles
                                    else if (_listaCar.Count <= 5)
                                    {

                                        if (_listaCar[0].Side(p3) == 1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {
                                            _cantCar[2]++;
                                        }
                                        else if (_listaCar[3].Side(p3) == -1 && _listaCar[4].Side(p3) == 1)
                                        {
                                            _cantCar[3]++;
                                        }

                                        updateInfoCarril();
                                    }
                                    // 5 carriles
                                    else if (_listaCar.Count <= 6)
                                    {
                                        if (_listaCar[0].Side(p3) == 1 && _listaCar[1].Side(p3) == 1)
                                        {
                                            _cantCar[0]++;
                                        }
                                        else if (_listaCar[1].Side(p3) == -1 && _listaCar[2].Side(p3) == 1)
                                        {
                                            _cantCar[1]++;
                                        }
                                        else if (_listaCar[2].Side(p3) == -1 && _listaCar[3].Side(p3) == 1)
                                        {
                                            _cantCar[2]++;
                                        }
                                        else if (_listaCar[3].Side(p3) == -1 && _listaCar[4].Side(p3) == 1)
                                        {
                                            _cantCar[3]++;
                                        }
                                        else if (_listaCar[4].Side(p3) == -1 && _listaCar[5].Side(p3) == 1)
                                        {
                                            _cantCar[4]++;
                                        }

                                        updateInfoCarril();

                                    }
                                }

                                if (velocidad)
                                {
                                    if (!(_listaVelocidades.ContainsKey(b.Id)) && lineaVelocidad.Side(p3) == -1
                                        && lineaVelocidadP1.Side(p3) == 1 && lineaVelocidadP2.Side(p3) == 1)
                                    {
                                        _listaVelocidades.Add(b.Id, b.Lifetime);
                                    }
                                }

                                int area = b.BoundingBox.Width * b.BoundingBox.Height;

                                if (log)
                                {
                                    richTextBoxLog.Text = richTextBoxLog.Text + _cont.ToString() + ": " + area.ToString() + "\n";
                                }

                                if (checkBoxO1.Checked == true)
                                {

                                    int max1 = int.Parse(textBoxO1Max.Text);
                                    int min1 = int.Parse(textBoxO1Min.Text);

                                    if ((area < max1) && (area >= min1))
                                    {

                                        o1++;
                                        labelO1.Text = textBoxO1Name.Text + ": " + o1.ToString();

                                    }
                                }

                                if (checkBoxO2.Checked == true)
                                {
                                    int max2 = int.Parse(textBoxO2Max.Text);
                                    int min2 = int.Parse(textBoxO2Min.Text);

                                    if ((area < max2) && (area >= min2))
                                    {

                                        o2++;
                                        labelO2.Text = textBoxO2Name.Text + ": " + o2.ToString();

                                    }
                                }

                                if (checkBoxO3.Checked == true)
                                {

                                    int max3 = int.Parse(textBoxO3Max.Text);
                                    int min3 = int.Parse(textBoxO3Min.Text);
                                    if ((area < max3) && (area >= min3))
                                    {

                                        o3++;
                                        labelO3.Text = textBoxO3Name.Text + ": " + o3.ToString();

                                    }
                                }
                            }
                            if (velocidad)
                            {
                                if (_listaVelocidades.ContainsKey(b.Id) && lineaVelocidad.Side(p3) == 1
                                    && lineaVelocidadP1.Side(p3) == 1 && lineaVelocidadP2.Side(p3) == 1)
                                {
                                    // calculo velocidad

                                    uint vida = _listaVelocidades[b.Id];

                                    // cantidad de frames que tardo en pasar de una linea a la otra.
                                    uint diferencia = b.Lifetime - vida;

                                    double segundos = diferencia / fps;
                                    double horas = segundos / 3600;

                                    double kmh = velocidadKm / horas;
                                    richTextBoxVelocidades.Text = richTextBoxVelocidades.Text
                                        + kmh.ToString("0.#") + "\n";

                                    _listaVelocidades.Remove(b.Id);
                                }
                            }
                        }
                    }

                    CvInvoke.PutText(frame, _cont.ToString(), new Point(frame.Width / 2, frame.Height / 2), FontFace.HersheyDuplex, 4.0, SCALAR_GREEN, 5);
                    CvInvoke.Line(frame, _p1, _p2, SCALAR_BLUE, 2);

                    CvInvoke.Line(frame, _pt4, _pt3, SCALAR_WHITE, 2);
                    CvInvoke.Line(frame, _pt5, _pt6, SCALAR_WHITE, 2);

                    // carriles
                    if (carriles)
                    {

                        for (int i = 0; i < _listaCar.Count ; i++)
                        {
                            LineSegment2D lx =  _listaCar[i];
                            CvInvoke.Line(frame, lx.P1, lx.P2, SCALAR_ORANGE, 3);
                        }

                        for (int i = 0; i < _listaCar.Count - 1; i++)
                        {
                            salida = salida + _cantCar[i] + "         ";
                        }
                        richTextBoxCarriles.Text = salida;
                        salida = "";
                    }

                    if (velocidad)
                    {
                        CvInvoke.Line(frame, _pV1, _pV2, SCALAR_CYAN, 2);

                        CvInvoke.Line(frame, _pt3V1, _pt4V1, SCALAR_WHITE, 2);
                        CvInvoke.Line(frame, _pt5V2, _pt6V2, SCALAR_WHITE, 2);
                    }

                    imageBox1.Image = frame;
                    imageBox2.Image = forgroundMask;


                    if (sleep)
                    {
                        Thread.Sleep(1000 / (int)fps);
                    }
                }
            }
        }

        // cargar
        private void button4_Click(object sender, EventArgs e)
        {

            string option = comboBox1.Text;
            if (option.CompareTo("CarsDrivingUnderBridge") == 0)
            {

                textX1.Text = "0,14";
                textY1.Text = "0,65";
                textX2.Text = "0,94";
                textY2.Text = "0,65";

                radioButtonDir1.Checked = true;
                textBoxA1min.Text = "600";
                textBoxA1max.Text = "50000";

                radioButtonMOG.Checked = true;
                textBoxMOGhistory.Text = "200";
                textBoxMOGnMix.Text = "5";
                textBoxMOGRatio.Text = "0,7";
                textBoxNSigma.Text = "0";

                groupBox6.Enabled = false;
                groupBox10.Enabled = false;
                groupBox13.Enabled = false;

                textBoxTrackerInac.Text = "5";
                textBoxTrackerActive.Text = "5";
                textBoxTrackerDist.Text = "0,01";

                textBoxErodeSize.Text = "7";
                textBoxErodeCant.Text = "3";
                comboBoxShape.Text = "Rectangle";

                // ----------------
                checkBoxO1.Checked = true;
                checkBoxO2.Checked = true;
                checkBoxO3.Checked = true;

                textBoxO1Min.Enabled = true;
                textBoxO1Max.Enabled = true;
                textBoxO1Name.Enabled = true;

                textBoxO2Min.Enabled = true;
                textBoxO2Max.Enabled = true;
                textBoxO2Name.Enabled = true;

                textBoxO3Min.Enabled = true;
                textBoxO3Max.Enabled = true;
                textBoxO3Name.Enabled = true;

                //--------
                textBoxO1Min.Text = "32360";
                textBoxO1Max.Text = "50000";
                textBoxO1Name.Text = "Camioneta";

                textBoxO2Min.Text = "16630";
                textBoxO2Max.Text = "32359";
                textBoxO2Name.Text = "Auto";

                textBoxO3Min.Text = "600";
                textBoxO3Max.Text = "16629";
                textBoxO3Name.Text = "Moto";

                // herramientas

                // colores

                checkBoxColor.Checked = true;
                textBoxRadio.Enabled = true;
                textBoxColorMin.Enabled = true;

                textBoxRadio.Text = "25";
                textBoxColorMin.Text = "5";

                // carriles

                checkBoxCarriles.Checked = true;
                textBoxCantCarriles.Text = "5";
                textBoxTiempoCarriles.Text = "0,05";

                // velocidad

                checkBoxVelocidad.Checked = true;
                textBoxX1Velocidad.Text = "0,20";
                textBoxX2Velocidad.Text = "0,80";
                textBoxY1Velocidad.Text = "0,32";
                textBoxY2Velocidad.Text = "0,32";
                textBoxVelocidadDistancia.Text = "0,012";

                //sleep
                checkBoxSleep.Checked = false;

            }
            else if (option.CompareTo("vid2") == 0)
            {
                textX1.Text = "0,10";
                textY1.Text = "0,75";
                textX2.Text = "0,40";
                textY2.Text = "0,75";

                textBoxA1min.Text = "1800";
                textBoxA1max.Text = "30000";

                radioButtonDir1.Checked = true;

                textBoxTrackerInac.Text = "5";
                textBoxTrackerActive.Text = "5";
                textBoxTrackerDist.Text = "0,01";

                textBoxErodeSize.Text = "10";
                textBoxErodeCant.Text = "4";
                comboBoxShape.Text = "Ellipse";

                radioButtonMOG2.Checked = true;
                textBoxMOG2history.Text = "500";
                textBoxMOG2thres.Text = "50";
                checkBoxMOG2Shadow.Checked = false;

                groupBox5.Enabled = false;
                groupBox10.Enabled = false;
                groupBox13.Enabled = false;

                // -------------
                checkBoxO1.Checked = true;

                textBoxO1Min.Enabled = true;
                textBoxO1Max.Enabled = true;
                textBoxO1Name.Enabled = true;

                textBoxO1Min.Text = "7000";
                textBoxO1Max.Text = "12000";
                textBoxO1Name.Text = "Auto";

            }
            else if (option.CompareTo("RamblaYJackson") == 0)
            {

                textX1.Text = "0";
                textY1.Text = "0,85";
                textX2.Text = "0,98";
                textY2.Text = "0,85";

                textBoxA1min.Text = "1800";
                textBoxA1max.Text = "9000";

                radioButtonDir3.Checked = true;

                textBoxTrackerInac.Text = "5";
                textBoxTrackerActive.Text = "5";
                textBoxTrackerDist.Text = "0,01";

                textBoxErodeSize.Text = "3";
                textBoxErodeCant.Text = "2";
                comboBoxShape.Text = "Ellipse";

                radioButtonMOG.Checked = true;
                textBoxMOGhistory.Text = "200";
                textBoxMOGnMix.Text = "5";
                textBoxMOGRatio.Text = "0,7";
                textBoxNSigma.Text = "0";

                groupBox6.Enabled = false;
                groupBox10.Enabled = false;
                groupBox13.Enabled = false;

                textBoxO1Min.Enabled = false;
                textBoxO1Max.Enabled = false;
                textBoxO1Name.Enabled = false;

            } else if (option.CompareTo("lateral") == 0) {

                textX1.Text = "0,30";
                textY1.Text = "0,45";
                textX2.Text = "0,30";
                textY2.Text = "0,99";

                radioButtonDir2.Checked = true;
                textBoxA1min.Text = "2000";
                textBoxA1max.Text = "20000";

                radioButtonMOG.Checked = true;
                textBoxMOGhistory.Text = "200";
                textBoxMOGnMix.Text = "5";
                textBoxMOGRatio.Text = "0,7";
                textBoxNSigma.Text = "0";

                groupBox6.Enabled = false;
                groupBox10.Enabled = false;
                groupBox13.Enabled = false;

                textBoxTrackerInac.Text = "5";
                textBoxTrackerActive.Text = "5";
                textBoxTrackerDist.Text = "0,01";

                textBoxErodeSize.Text = "7";
                textBoxErodeCant.Text = "5";
                comboBoxShape.Text = "Ellipse";

                checkBoxO1.Checked = false;
                checkBoxO2.Checked = false;
                checkBoxO3.Checked = false;

                // colores

                checkBoxColor.Checked = true;
                textBoxRadio.Enabled = true;
                textBoxColorMin.Enabled = true;

                textBoxRadio.Text = "20";
                textBoxColorMin.Text = "5";

                // carriles

                checkBoxCarriles.Checked = true;
                textBoxCantCarriles.Text = "3";
                textBoxTiempoCarriles.Text = "0,05";

                // velocidad

                checkBoxVelocidad.Checked = true;
                textBoxX1Velocidad.Text = "0,82";
                textBoxX2Velocidad.Text = "0,82";
                textBoxY1Velocidad.Text = "0,45";
                textBoxY2Velocidad.Text = "0,99";
                textBoxVelocidadDistancia.Text = "0,004";

                //sleep
                checkBoxSleep.Checked = true;

            }

        }

        private void radioButtonMOG_CheckedChanged(object sender, EventArgs e)
        {
            groupBox6.Enabled = false;
            groupBox10.Enabled = false;
            groupBox5.Enabled = true;
            groupBox13.Enabled = false;

            textBoxMOGhistory.Text = "200";
            textBoxMOGnMix.Text = "5";
            textBoxMOGRatio.Text = "0,7";
            textBoxNSigma.Text = "0";

        }

        private void radioButtonMOG2_CheckedChanged(object sender, EventArgs e)
        {

            groupBox5.Enabled = false;
            groupBox10.Enabled = false;
            groupBox6.Enabled = true;
            groupBox13.Enabled = false;

            textBoxMOG2history.Text = "500";
            textBoxMOG2thres.Text = "16";
            checkBoxMOG2Shadow.Checked = true;

        }

        private void radioButtonKNN_CheckedChanged(object sender, EventArgs e)
        {

            groupBox5.Enabled = false;
            groupBox10.Enabled = true;
            groupBox6.Enabled = false;
            groupBox13.Enabled = false;

            textBoxKNNhistory.Text = "500";
            textBoxKNNDist2.Text = "16";
            checkBoxKNNShadow.Checked = false;


        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox5.Enabled = false;
            groupBox10.Enabled = false;
            groupBox6.Enabled = false;
            groupBox13.Enabled = true;

            textBoxGMG1.Text = "30";
            textBoxGMG2.Text = "0,2";


        }

        private void checkBoxO1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxO1.Checked == false)
            {
                textBoxO1Min.Enabled = false;
                textBoxO1Max.Enabled = false;
                textBoxO1Name.Enabled = false;

            }
            else {

                textBoxO1Min.Enabled = true;
                textBoxO1Max.Enabled = true;
                textBoxO1Name.Enabled = true;

            }

        }

        private void checkBoxO2_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBoxO2.Checked == false)
            {
                textBoxO2Min.Enabled = false;
                textBoxO2Max.Enabled = false;
                textBoxO2Name.Enabled = false;

            }
            else
            {
                textBoxO2Min.Enabled = true;
                textBoxO2Max.Enabled = true;
                textBoxO2Name.Enabled = true;

            }

        }

        private void checkBoxO3_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBoxO3.Checked == false)
            {
                textBoxO3Min.Enabled = false;
                textBoxO3Max.Enabled = false;
                textBoxO3Name.Enabled = false;

            }
            else
            {
                textBoxO3Min.Enabled = true;
                textBoxO3Max.Enabled = true;
                textBoxO3Name.Enabled = true;

            }

        }

        private void checkBoxLog_CheckedChanged(object sender, EventArgs e)
        {

            if (richTextBoxLog.Enabled == false)
            {
                richTextBoxLog.Enabled = true;
            }
            else
            {
                richTextBoxLog.Enabled = false;
            }

        }

        private void checkBoxColor_CheckedChanged(object sender, EventArgs e)
        {
            if (textBoxRadio.Enabled == true)
            {
                textBoxRadio.Enabled = false;
                textBoxColorMin.Enabled = false;
            }
            else
            {
                textBoxRadio.Enabled = true;
                textBoxColorMin.Enabled = true;
            }

        }

        private void checkBoxCarriles_CheckedChanged(object sender, EventArgs e)
        {

            if (textBoxCantCarriles.Enabled == true)
            {
                textBoxCantCarriles.Enabled = false;
                textBoxTiempoCarriles.Enabled = false;
            }
            else
            {
                textBoxCantCarriles.Enabled = true;
                textBoxTiempoCarriles.Enabled = true;
            }

        }

        private void checkBoxVelocidad_CheckedChanged(object sender, EventArgs e)
        {

            if (textBoxX1Velocidad.Enabled == true)
            {
                textBoxX1Velocidad.Enabled = false;
                textBoxY1Velocidad.Enabled = false;
                textBoxX2Velocidad.Enabled = false;
                textBoxY2Velocidad.Enabled = false;
                textBoxVelocidadDistancia.Enabled = false;
            }
            else
            {
                textBoxX1Velocidad.Enabled = true;
                textBoxY1Velocidad.Enabled = true;
                textBoxX2Velocidad.Enabled = true;
                textBoxY2Velocidad.Enabled = true;
                textBoxVelocidadDistancia.Enabled = true;
            }
        }

        private void richTextBoxVelocidades_TextChanged(object sender, EventArgs e)
        {
            richTextBoxVelocidades.SelectionStart = richTextBoxVelocidades.Text.Length;
            richTextBoxVelocidades.ScrollToCaret();
        }

        private void richTextBoxColores_TextChanged(object sender, EventArgs e)
        {
            richTextBoxColores.SelectionStart = richTextBoxColores.Text.Length;
            richTextBoxColores.ScrollToCaret();
        }

        private void updateInfoCarril()
        {
            if (index >= tope)
            {
                int dato = 0;
                int indice = 0;
                for (int i = 0; i < _cantCar.Count - 1; i++)
                {

                    if (_cantCar[i] >= dato)
                    {
                        dato = _cantCar[i];
                        indice = i;
                    }
                }
                indice = indice + 1;
                labelTiempoCarril.Text = "Carril " + indice + " con mas transito.";
                tope = index + topeCarriles;

                int dato1 = int.MaxValue;
                int indice1 = 0;
                for (int i = 0; i < _cantCar.Count - 1; i++)
                {

                    if (_cantCar[i] <= dato1)
                    {
                        dato1 = _cantCar[i];
                        indice1 = i;
                    }
                }
                indice1 = indice1 + 1;
                labelTiempoCarrilMin.Text = "Carril " + indice1 + " con menos transito.";
            }
        }


    }
}

