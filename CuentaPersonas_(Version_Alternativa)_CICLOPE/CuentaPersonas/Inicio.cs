﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using Emgu.CV.BgSegm;

using System.Runtime.InteropServices;
using System.Threading;


namespace CuentaPersonas
{
    public partial class Inicio : Form
    {

        private static VideoCapture _cameraCapture;

        private static BackgroundSubtractor _fgDetector;
        private static CvBlobDetector _blobDetector;
        private static CvTracks _tracker;
        private bool _inProgress;

        private Size tam5 = new Size(5, 5);

        private MCvScalar SCALAR_BLACK = new MCvScalar(0.0, 0.0, 0.0);
        private MCvScalar SCALAR_WHITE = new MCvScalar(255.0, 255.0, 255.0);
        private MCvScalar SCALAR_BLUE = new MCvScalar(255.0, 0.0, 0.0);
        private MCvScalar SCALAR_GREEN = new MCvScalar(0.0, 255.0, 0.0);
        private MCvScalar SCALAR_RED = new MCvScalar(0.0, 0.0, 255.0);
        private MCvScalar SCALAR_ORANGE = new MCvScalar(0.0, 165.0, 255.0);
        private MCvScalar SCALAR_CYAN = new MCvScalar(255.0, 255.0, 0.0);
        private MCvScalar SCALAR_AMBER = new MCvScalar(0.0, 191.0, 255.0);

        private int _contA;
        private int _contB;
        private List<uint> _idBlobsA;
        private List<uint> _idBlobsB;
        private int _direction;

        private int _areaMin;
        private int _areaMax;

        private float _track1;
        private uint _track2;
        private uint _track3;

        private Mat structuringElementErode;
        private int cantErode;

        private Mat structuringElementDilate;
        private int cantDilate;

        private Mat imgThresh;
        private Mat forgroundMask;

        private Point _p0;
        private Point _p1;
        private Point _p2;
        private Point _pt3;
        private Point _pt4;
        private Point _pt5;
        private Point _pt6;

        private LineSegment2D l2;
        private LineSegment2D l1;
        private LineSegment2D line;

        private bool parar;
        private bool api;
        CognitiveService cog;

        public Inicio()
        {
            InitializeComponent();

            string[] tipo = new string[] { "Horizontal", "Vertical" };
            comboBox1.Items.AddRange(tipo);

            string[] shapes = new string[] { "Rectangle", "Ellipse", "Cross" };
            comboBoxShapeE.Items.AddRange(shapes);
            comboBoxShapeE.Text = "Ellipse";
            textBoxErodeSize.Text = "15";
            textBoxErodeCant.Text = "3";
            comboBoxShapeD.Items.AddRange(shapes);
            comboBoxShapeD.Text = "Ellipse";
            textBoxDilateSize.Text = "12";
            textBoxDilateCant.Text = "3";

            textBoxTrackerInac.Text = "5";
            textBoxTrackerActive.Text = "5";
            textBoxTrackerDist.Text = "0,01";

            forgroundMask = new Mat();
            imgThresh = new Mat();
            _p0 = new Point(-1, -1);

            radioButtonDir1.Checked = true;
            radioButtonDir2.Checked = false;

            textBoxA1min.Text = "6000";
            textBoxA1max.Text = "90000";

            radioButtonMOG.Checked = true;

            textX1.Text = "0,35";
            textY1.Text = "0,55";
            textX2.Text = "0,65";
            textY2.Text = "0,55";

            comboBox1.Text = "Vertical";

            parar = false;
            checkBox1.Checked = false;
            richTextBox1.Enabled = false;
            cog = new CognitiveService();

            textBox1.Text = "";

            // ---------------------

            imageListA.ImageSize = new Size(120, 120);
            imageListA.ColorDepth = ColorDepth.Depth32Bit;
            listViewA.View = View.LargeIcon;
            ListViewItem_SetSpacing(listViewA, 120 + 16, 120 + 16);

            // ---------------------

            imageListB.ImageSize = new Size(120, 120);
            imageListB.ColorDepth = ColorDepth.Depth32Bit;
            listViewB.View = View.LargeIcon;
            ListViewItem_SetSpacing(listViewB, 120 + 16, 120 + 16);

            // ---------------------------------------

        }

        // play
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.ToString().CompareTo("") != 0)
                {
                    _cameraCapture = new VideoCapture(this.textBox1.Text.ToString());

                    _blobDetector = new CvBlobDetector();
                    _tracker = new CvTracks();
                    _inProgress = true;
                    _idBlobsA = new List<uint>();
                    _idBlobsB = new List<uint>();
                    _contA = 0;
                    _contB = 0;

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

                    string shapeErode = comboBoxShapeE.Text;
                    int sizeErode = int.Parse(textBoxErodeSize.Text);
                    cantErode = int.Parse(textBoxErodeCant.Text);

                    if (shapeErode.CompareTo("Rectangle") == 0)
                    {
                        structuringElementErode = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(sizeErode, sizeErode), _p0);
                    }
                    else if (shapeErode.CompareTo("Ellipse") == 0)
                    {
                        structuringElementErode = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(sizeErode, sizeErode), _p0);
                    }
                    else if (shapeErode.CompareTo("Cross") == 0)
                    {
                        structuringElementErode = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(sizeErode, sizeErode), _p0);
                    }

                    string shapeDilate = comboBoxShapeD.Text;
                    int sizeDilate = int.Parse(textBoxDilateSize.Text);
                    cantDilate = int.Parse(textBoxDilateCant.Text);

                    if (shapeDilate.CompareTo("Rectangle") == 0)
                    {
                        structuringElementDilate = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(sizeDilate, sizeDilate), _p0);
                    }
                    else if (shapeDilate.CompareTo("Ellipse") == 0)
                    {
                        structuringElementDilate = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(sizeDilate, sizeDilate), _p0);
                    }
                    else if (shapeDilate.CompareTo("Cross") == 0)
                    {
                        structuringElementDilate = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(sizeDilate, sizeDilate), _p0);
                    }

                    // ----------------------------

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

                    api = checkBox1.Checked;

                    Application.Idle += ProcessFrame;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Primero debe ingresar un stream.", "Cuidado.");
                }
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        // stop
        private void button2_Click(object sender, EventArgs e)
        {
            if (_cameraCapture != null)
            {
                if (_inProgress)
                {
                    _cameraCapture.Dispose();
                    _inProgress = false;
                    imageBox1.Image = null;
                    imageBox2.Image = null;
                    //GC.Collect();
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

                    Mat clone = frame.Clone();
                    Mat smoothedFrame = frame.Clone();

                    CvInvoke.GaussianBlur(smoothedFrame, imgThresh, tam5, 0);
                    _fgDetector.Apply(imgThresh, forgroundMask);

                    for (int i = 0; i < cantErode; i++)
                    {
                        CvInvoke.Dilate(forgroundMask, forgroundMask, structuringElementErode, _p0, 1, BorderType.Default, SCALAR_BLACK);
                    }
                    for (int i = 0; i < cantDilate; i++)
                    {
                        CvInvoke.Erode(forgroundMask, forgroundMask, structuringElementDilate, _p0, 1, BorderType.Default, SCALAR_BLACK);
                    }

                    CvBlobs blobs = new CvBlobs();
                    _blobDetector.Detect(forgroundMask.ToImage<Gray, byte>(), blobs);
                    blobs.FilterByArea(_areaMin, _areaMax);

                    float scale = (frame.Width + frame.Width) / 2.0f;
                    _tracker.Update(blobs, _track1 * scale, _track2, _track3);

                    if (_tracker.Count == 0 && _idBlobsA.Count != 0)
                    {
                        _idBlobsA.Clear();
                    }
                    if (_tracker.Count == 0 && _idBlobsB.Count != 0)
                    {
                        _idBlobsB.Clear();
                    }

                    foreach (var pair in _tracker)
                    {
                        CvTrack b = pair.Value;
                        CvInvoke.Rectangle(frame, b.BoundingBox, SCALAR_RED, 2);
                        //CvInvoke.PutText(frame, b.Id.ToString(), new Point((int)Math.Round(b.Centroid.X), (int)Math.Round(b.Centroid.Y)), FontFace.HersheyPlain, 8.0, new MCvScalar(255.0, 255.0, 255.0),4);

                        Point p3 = new Point((int)Math.Round(b.Centroid.X), (int)Math.Round(b.Centroid.Y));
                        CvInvoke.Circle(frame, p3, 3, SCALAR_GREEN, 3);

                        if (!parar)
                        {
                            if (_direction == 1) // Abajo / Arriba
                            {
                                // A                          
                                if (line.Side(p3) == 1 && !_idBlobsA.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == -1)
                                {
                                    _idBlobsA.Add(b.Id);
                                }

                                if (line.Side(p3) == -1 && _idBlobsA.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == -1)
                                {
                                    _idBlobsA.RemoveAll(item => item == b.Id);
                                    _contA++;
                                    labelEntraron.Text = "Entraron " + _contA.ToString();

                                    // imagenes

                                    Mat minimat = new Mat(clone, b.BoundingBox);

                                    imageListA.Images.Add(minimat.Bitmap);
                                    listViewA.LargeImageList = imageListA;
                                    listViewA.Clear();

                                    for (int i = 0; i < imageListA.Images.Count; i++)
                                    {
                                        ListViewItem lvi = new ListViewItem();
                                        lvi.ImageIndex = i;
                                        listViewA.Items.Add(lvi);
                                    }
                                    listViewA.EnsureVisible(listViewA.Items.Count - 1);

                                }

                                // B
                                if (line.Side(p3) == -1 && !_idBlobsB.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == -1)
                                {
                                    _idBlobsB.Add(b.Id);
                                }

                                if (line.Side(p3) == 1 && _idBlobsB.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == -1)
                                {

                                    _idBlobsB.RemoveAll(item => item == b.Id);
                                    _contB++;
                                    labelSalieron.Text = "Salieron " + _contB.ToString();

                                    // imagenes

                                    Mat minimat = new Mat(clone, b.BoundingBox);

                                    imageListB.Images.Add(minimat.Bitmap);
                                    listViewB.LargeImageList = imageListB;
                                    listViewB.Clear();

                                    for (int i = 0; i < imageListB.Images.Count; i++)
                                    {
                                        ListViewItem lvi = new ListViewItem();
                                        lvi.ImageIndex = i;
                                        listViewB.Items.Add(lvi);
                                    }
                                    listViewB.EnsureVisible(listViewB.Items.Count - 1);

                                    if (api)
                                    {
                                        new Thread(() =>
                                        {
                                            CheckForIllegalCrossThreadCalls = false;

                                            Thread.CurrentThread.IsBackground = true;

                                           // Rectangle r = new Rectangle(b.BoundingBox.X - 50, b.BoundingBox.Y - 50, b.BoundingBox.Width + 100, b.BoundingBox.Height + 50);
                                            Mat minimat2 = new Mat(clone, b.BoundingBox);

                                            string directory = AppDomain.CurrentDomain.BaseDirectory;
                                            string date = DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss");
                                            minimat2.Save(directory + date +".jpg");

                                            //Image<Bgr, byte> img = minimat2.ToImage<Bgr, byte>();
                                            apiCall(_contB, date);

                                        }).Start();
                                    }
                                }

                            }
                            else if (_direction == 2) // izquierda / derecha
                            {
                                // A                          
                                if (line.Side(p3) == 1 && !_idBlobsA.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == 1)
                                {
                                    _idBlobsA.Add(b.Id);
                                }

                                if (line.Side(p3) == -1 && _idBlobsA.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == 1)
                                {
                                    _idBlobsA.RemoveAll(item => item == b.Id);
                                    _contA++;
                                    labelEntraron.Text = "Entraron " + _contA.ToString();

                                    // imagenes

                                    Mat minimat = new Mat(clone, b.BoundingBox);

                                    imageListA.Images.Add(minimat.Bitmap);
                                    listViewA.LargeImageList = imageListA;
                                    listViewA.Clear();

                                    for (int i = 0; i < imageListA.Images.Count; i++)
                                    {
                                        ListViewItem lvi = new ListViewItem();
                                        lvi.ImageIndex = i;
                                        listViewA.Items.Add(lvi);
                                    }
                                    listViewA.EnsureVisible(listViewA.Items.Count - 1);
                                }

                                // B
                                if (line.Side(p3) == -1 && !_idBlobsB.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == 1)
                                {
                                    _idBlobsB.Add(b.Id);
                                }

                                if (line.Side(p3) == 1 && _idBlobsB.Contains(b.Id) && l1.Side(p3) == 1 && l2.Side(p3) == 1)
                                {
                                    _idBlobsB.RemoveAll(item => item == b.Id);
                                    _contB++;
                                    labelSalieron.Text = "Salieron " + _contB.ToString();

                                    // imagenes

                                    Mat minimat = new Mat(clone, b.BoundingBox);

                                    imageListB.Images.Add(minimat.Bitmap);
                                    listViewB.LargeImageList = imageListB;
                                    listViewB.Clear();

                                    for (int i = 0; i < imageListB.Images.Count; i++)
                                    {
                                        ListViewItem lvi = new ListViewItem();
                                        lvi.ImageIndex = i;
                                        listViewB.Items.Add(lvi);
                                    }
                                    listViewB.EnsureVisible(listViewB.Items.Count - 1);

                                    if (api)
                                    {
                                        new Thread(() =>
                                        {
                                            CheckForIllegalCrossThreadCalls = false;
                                            Thread.CurrentThread.IsBackground = true;

                                           // Rectangle r = new Rectangle(b.BoundingBox.X - 50, b.BoundingBox.Y - 50, b.BoundingBox.Width + 100, b.BoundingBox.Height + 50);
                                            Mat minimat2 = new Mat(clone, b.BoundingBox);

                                            string directory = AppDomain.CurrentDomain.BaseDirectory;
                                            string date = DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss");
                                            minimat2.Save(directory + date + ".jpg");

                                            //Image<Bgr, byte> img = minimat2.ToImage<Bgr, byte>();
                                            apiCall(_contB, date);

                                        }).Start();
                                    }
                                }
                            }
                        }
                    }

                    CvInvoke.Line(frame, _p1, _p2, SCALAR_BLUE, 2);
                    CvInvoke.Line(frame, _pt4, _pt3, SCALAR_WHITE, 2);
                    CvInvoke.Line(frame, _pt5, _pt6, SCALAR_WHITE, 2);

                    imageBox1.Image = frame;
                    imageBox2.Image = forgroundMask;
                }
            }
        }// ProcessFrame

        private void radioButtonMOG_CheckedChanged(object sender, EventArgs e)
        {
            groupBox9.Enabled = false;
            groupBox10.Enabled = false;
            groupBox7.Enabled = true;
            groupBox8.Enabled = false;

            textBoxMOGhistory.Text = "200";
            textBoxMOGnMix.Text = "5";
            textBoxMOGRatio.Text = "0,7";
            textBoxNSigma.Text = "0";
        }

        private void radioButtonMOG2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox7.Enabled = false;
            groupBox8.Enabled = true;
            groupBox9.Enabled = false;
            groupBox10.Enabled = false;

            textBoxMOG2history.Text = "500";
            textBoxMOG2thres.Text = "16";
            checkBoxMOG2Shadow.Checked = true;
        }

        private void radioButtonKNN_CheckedChanged(object sender, EventArgs e)
        {
            groupBox7.Enabled = false;
            groupBox8.Enabled = false;
            groupBox9.Enabled = true;
            groupBox10.Enabled = false;

            textBoxKNNhistory.Text = "500";
            textBoxKNNDist2.Text = "16";
            checkBoxKNNShadow.Checked = false;
        }

        private void radioButtonGMG_CheckedChanged(object sender, EventArgs e)
        {
            groupBox7.Enabled = false;
            groupBox8.Enabled = false;
            groupBox9.Enabled = false;
            groupBox10.Enabled = true;

            textBoxGMG1.Text = "30";
            textBoxGMG2.Text = "0,2";
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public int MakeLong(short lowPart, short highPart)
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }

        public void ListViewItem_SetSpacing(ListView listview, short leftPadding, short topPadding)
        {
            const int LVM_FIRST = 0x1000;
            const int LVM_SETICONSPACING = LVM_FIRST + 53;
            SendMessage(listview.Handle, LVM_SETICONSPACING, IntPtr.Zero, (IntPtr)MakeLong(leftPadding, topPadding));
        }

        private void buttonParar_Click(object sender, EventArgs e)
        {
            parar = true;
        }

        private void buttonComenzar_Click(object sender, EventArgs e)
        {
            labelEntraron.Text = "Entraron 0";
            labelSalieron.Text = "Salieron 0";
            _contA = 0;
            _contB = 0;
            _idBlobsB.Clear();
            _idBlobsA.Clear();
            _tracker.Clear();
            listViewA.Clear();
            listViewB.Clear();
            imageListA.Images.Clear();
            imageListB.Images.Clear();
            richTextBox1.Text = "";
            parar = false;
        }

        private async void apiCall(int cont, string date)
        {

            string res = await cog.DoWork(date);
            richTextBox1.Text = richTextBox1.Text + cont.ToString() + " : " + res + "\n";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.CompareTo("Horizontal") == 0)
            {

                radioButtonDir1.Checked = false;
                radioButtonDir2.Checked = true;

                textBoxA1min.Text = "4500";
                textBoxA1max.Text = "90000";

                radioButtonMOG.Checked = true;

                textX1.Text = "0,37";
                textY1.Text = "0,02";
                textX2.Text = "0,37";
                textY2.Text = "0,98";

            }
            else if (comboBox1.Text.CompareTo("Vertical") == 0)
            {

                radioButtonDir1.Checked = true;
                radioButtonDir2.Checked = false;

                textBoxA1min.Text = "6000";
                textBoxA1max.Text = "90000";

                radioButtonMOG.Checked = true;

                textX1.Text = "0,35";
                textY1.Text = "0,55";
                textX2.Text = "0,65";
                textY2.Text = "0,55";

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                richTextBox1.Enabled = true;
            }else
            {
                richTextBox1.Enabled = false;
            }
        }
    }
}