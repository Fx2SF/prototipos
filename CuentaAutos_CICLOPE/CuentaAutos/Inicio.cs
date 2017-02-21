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

        private MCvScalar SCALAR_BLACK = new MCvScalar(0.0, 0.0, 0.0);
        private MCvScalar SCALAR_WHITE = new MCvScalar(255.0, 255.0, 255.0);
        private MCvScalar SCALAR_BLUE = new MCvScalar(255.0, 0.0, 0.0);
        private MCvScalar SCALAR_GREEN = new MCvScalar(0.0, 255.0, 0.0);
        private MCvScalar SCALAR_RED = new MCvScalar(0.0, 0.0, 255.0);

        private int _cont;
        List<uint> _idBlobs;

        private int _areaMin;
        private int _areaMax;

        private float _track1;
        private uint _track2;
        private uint _track3;

        int _direction;

        Mat structuringElement;
        int cantErode;

        Mat imgThresh;
        Mat forgroundMask;
        Point p0;

        int o1;
        int o2;
        int o3;

        bool log;

        double fps;

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
                    fps = _cameraCapture.GetCaptureProperty(CapProp.Fps);

                    _areaMin = int.Parse(textBoxA1min.Text);
                    _areaMax = int.Parse(textBoxA1max.Text);

                    _track1 = float.Parse(textBoxTrackerDist.Text);
                    _track2 = uint.Parse(textBoxTrackerInac.Text);
                    _track3 = uint.Parse(textBoxTrackerActive.Text);

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
                    }else
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
                    else if (shape.CompareTo("Cross") == 0) {

                        structuringElement = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(size, size), p0);

                    }
                    //------------
                    o1 = 0;
                    o2 = 0;
                    o3 = 0;

                    //------------
                    if (checkBoxO1.Checked == true)
                    {

                        labelO1.Text = textBoxO1Name.Text + ": 0";

                    }
                    else {

                        labelO1.Text = "";
                    }
                    if (checkBoxO2.Checked == true)
                    {

                        labelO2.Text = textBoxO2Name.Text + ": 0";

                    }
                    else {

                        labelO2.Text = "";

                    }
                    if (checkBoxO3.Checked == true)
                    {

                        labelO3.Text = textBoxO3Name.Text + ": 0"; ;

                    }
                    else {

                        labelO3.Text = "";
                    }

                    log = checkBoxLog.Checked;
                    richTextBoxLog.Clear();

                    Application.Idle += ProcessFrame;
                }
                else {

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
                {   _cameraCapture.Dispose();
                    _inProgress = false;
                    imageBox1.Image = null;
                    imageBox2.Image = null;
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
                    LineSegment2D line = new LineSegment2D(_p1, _p2);

                    // -----------
                    

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

                    Point _pt3 = new Point(cx,cy);
                    Point _pt4 = new Point(dx,dy);
                    LineSegment2D l1 = new LineSegment2D(_pt4, _pt3);
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

                    Point _pt5 = new Point(cx2, cy2);
                    Point _pt6 = new Point(dx2, dy2);
                    LineSegment2D l2 = new LineSegment2D(_pt5, _pt6);

                    // ------------
                    Mat smoothedFrame = frame.Clone();
                    CvInvoke.GaussianBlur(smoothedFrame, imgThresh, new Size(5, 5), 0);
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
                    _tracker.Update(blobs, _track1 * scale, _track2, _track3);//5,5

                    foreach (var pair in _tracker)
                    {
                        CvTrack b = pair.Value;

                        CvInvoke.Rectangle(frame, b.BoundingBox, SCALAR_RED, 2);
                        CvInvoke.PutText(frame, b.Id.ToString(), new Point((int)Math.Round(b.Centroid.X), (int)Math.Round(b.Centroid.Y)), FontFace.HersheyPlain, 1.0, new MCvScalar(255.0, 255.0, 255.0));

                        Point p3 = new Point((int)Math.Round(b.Centroid.X), (int)Math.Round(b.Centroid.Y));
                        CvInvoke.Circle(frame, p3, 3, SCALAR_GREEN, 3);

                        if (_direction == 1)
                        {
                            //Console.WriteLine(line.Side(p3) +" "+l1.Side(p3)+" "+l2.Side(p3));

                            if (line.Side(p3) == 1 && !_idBlobs.Contains(b.Id) && l1.Side(p3) == -1 && l2.Side(p3) == -1)
                            {

                                //lo agrego al array
                                _idBlobs.Add(b.Id);

                            }

                            if (line.Side(p3) == -1 && _idBlobs.Contains(b.Id) && l1.Side(p3) == -1 && l2.Side(p3) == -1)
                            {
                                _idBlobs.RemoveAll(item => item == b.Id);
                                _cont++;

                                //check areas
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
                        }
                        else if (_direction == 2)
                        {

                            if (line.Side(p3) == 1 && !_idBlobs.Contains(b.Id) && l1.Side(p3) == -1 && l2.Side(p3) == 1)
                            {

                                _idBlobs.Add(b.Id);

                            }

                            if (line.Side(p3) == -1 && _idBlobs.Contains(b.Id) && l1.Side(p3) == -1 && l2.Side(p3) == 1)
                            {
                                _idBlobs.RemoveAll(item => item == b.Id);
                                _cont++;

                                //check areas
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

                        }
                        else if (_direction == 3)
                        {

                            if (line.Side(p3) == -1 && !_idBlobs.Contains(b.Id) && l1.Side(p3) == -1 && l2.Side(p3) == -1)
                            {
                                _idBlobs.Add(b.Id);

                            }

                            if (line.Side(p3) == 1 && _idBlobs.Contains(b.Id) && l1.Side(p3) == -1 && l2.Side(p3) == -1)
                            {
                                _idBlobs.RemoveAll(item => item == b.Id);
                                _cont++;

                                //check areas
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

                        }
                        else {// _direction == 4

                            if (line.Side(p3) == -1 && !_idBlobs.Contains(b.Id) && l1.Side(p3) == -1 && l2.Side(p3) == 1)
                            {

                                _idBlobs.Add(b.Id);

                            }

                            if (line.Side(p3) == 1 && _idBlobs.Contains(b.Id) && l1.Side(p3) == -1 && l2.Side(p3) == 1)
                            {
                                _idBlobs.RemoveAll(item => item == b.Id);
                                _cont++;

                                //check areas
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

                        }

                    }

                    CvInvoke.PutText(frame, _cont.ToString(), new Point(frame.Width / 2, frame.Height / 2), FontFace.HersheyDuplex, 4.0, SCALAR_GREEN);
                    CvInvoke.Line(frame, _p1, _p2, SCALAR_BLUE, 2);

                    CvInvoke.Line(frame, _pt4, _pt3, SCALAR_WHITE, 2);
                    CvInvoke.Line(frame, _pt5, _pt6, SCALAR_WHITE, 2);

                  
                    imageBox1.Image = frame;
                    imageBox2.Image = forgroundMask;

                    Thread.Sleep(1000 / (int)fps);

                }
            }
        }

        // cargar
        private void button4_Click(object sender, EventArgs e)
        {

            string option = comboBox1.Text;
            if (option.CompareTo("CarsDrivingUnderBridge") == 0)
            {

                textX1.Text = "0,10";
                textY1.Text = "0,65";
                textX2.Text = "0,95";
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
                textBoxMOG2thres.Text = "45";
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

                textX1.Text = "0,50";
                textY1.Text = "0,45";
                textX2.Text = "0,50";
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


    }
}
