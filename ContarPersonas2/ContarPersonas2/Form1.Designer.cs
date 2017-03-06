namespace ContarPersonas2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageBox = new Emgu.CV.UI.ImageBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbWeb = new System.Windows.Forms.RadioButton();
            this.btArchivo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFuente = new System.Windows.Forms.TextBox();
            this.btConectar = new System.Windows.Forms.Button();
            this.rbArchivo = new System.Windows.Forms.RadioButton();
            this.rbIP = new System.Windows.Forms.RadioButton();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            this.dialogVideoFile = new System.Windows.Forms.OpenFileDialog();
            this.lblFps = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabVideo = new System.Windows.Forms.TabPage();
            this.lblPos = new System.Windows.Forms.Label();
            this.btSeek = new System.Windows.Forms.Button();
            this.pickerInicio = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.btPause = new System.Windows.Forms.Button();
            this.ckUseGender = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtCantidadFemale = new System.Windows.Forms.TextBox();
            this.txtCantMale = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.gridMujeres = new System.Windows.Forms.DataGridView();
            this.dataGridViewImageColumn2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.gridHombres = new System.Windows.Forms.DataGridView();
            this.colHombres = new System.Windows.Forms.DataGridViewImageColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckSkin = new System.Windows.Forms.CheckBox();
            this.ckMotion = new System.Windows.Forms.CheckBox();
            this.tabHSV = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.imageBoxHue = new Emgu.CV.UI.ImageBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.imageBoxSat = new Emgu.CV.UI.ImageBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblHSV = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.imageBoxValue = new Emgu.CV.UI.ImageBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.imageBoxMask = new Emgu.CV.UI.ImageBox();
            this.tabConfig = new System.Windows.Forms.TabPage();
            this.ckShowCenters = new System.Windows.Forms.CheckBox();
            this.ckShowROI = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboxGeneroModelo = new System.Windows.Forms.ComboBox();
            this.lblGeneroHistoria = new System.Windows.Forms.Label();
            this.trackGeneroHistoria = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabVideo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMujeres)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridHombres)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabHSV.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxHue)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxSat)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxValue)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxMask)).BeginInit();
            this.tabConfig.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackGeneroHistoria)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBox.Location = new System.Drawing.Point(8, 156);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(720, 360);
            this.imageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox.TabIndex = 2;
            this.imageBox.TabStop = false;
            this.imageBox.Resize += new System.EventHandler(this.imageBox_Resize);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.rbWeb);
            this.panel1.Controls.Add(this.btArchivo);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtFuente);
            this.panel1.Controls.Add(this.btConectar);
            this.panel1.Controls.Add(this.rbArchivo);
            this.panel1.Controls.Add(this.rbIP);
            this.panel1.Location = new System.Drawing.Point(8, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(789, 91);
            this.panel1.TabIndex = 3;
            // 
            // rbWeb
            // 
            this.rbWeb.AutoSize = true;
            this.rbWeb.Location = new System.Drawing.Point(280, 7);
            this.rbWeb.Name = "rbWeb";
            this.rbWeb.Size = new System.Drawing.Size(116, 17);
            this.rbWeb.TabIndex = 6;
            this.rbWeb.Text = "Cámara Web Local";
            this.rbWeb.UseVisualStyleBackColor = true;
            // 
            // btArchivo
            // 
            this.btArchivo.Location = new System.Drawing.Point(119, 4);
            this.btArchivo.Name = "btArchivo";
            this.btArchivo.Size = new System.Drawing.Size(35, 23);
            this.btArchivo.TabIndex = 5;
            this.btArchivo.Text = "...";
            this.btArchivo.UseVisualStyleBackColor = true;
            this.btArchivo.Click += new System.EventHandler(this.btArchivo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Fuente:";
            // 
            // txtFuente
            // 
            this.txtFuente.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFuente.Location = new System.Drawing.Point(15, 55);
            this.txtFuente.Name = "txtFuente";
            this.txtFuente.Size = new System.Drawing.Size(690, 20);
            this.txtFuente.TabIndex = 3;
            this.txtFuente.Text = "rtsp://{user}:{pass}@fx2.no-ip.org:554";
            // 
            // btConectar
            // 
            this.btConectar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btConectar.Location = new System.Drawing.Point(711, 52);
            this.btConectar.Name = "btConectar";
            this.btConectar.Size = new System.Drawing.Size(75, 23);
            this.btConectar.TabIndex = 2;
            this.btConectar.Text = "Conectar";
            this.btConectar.UseVisualStyleBackColor = true;
            this.btConectar.Click += new System.EventHandler(this.btConectar_Click);
            // 
            // rbArchivo
            // 
            this.rbArchivo.AutoSize = true;
            this.rbArchivo.Location = new System.Drawing.Point(61, 7);
            this.rbArchivo.Name = "rbArchivo";
            this.rbArchivo.Size = new System.Drawing.Size(61, 17);
            this.rbArchivo.TabIndex = 1;
            this.rbArchivo.Text = "Archivo";
            this.rbArchivo.UseVisualStyleBackColor = true;
            // 
            // rbIP
            // 
            this.rbIP.AutoSize = true;
            this.rbIP.Checked = true;
            this.rbIP.Location = new System.Drawing.Point(178, 7);
            this.rbIP.Name = "rbIP";
            this.rbIP.Size = new System.Drawing.Size(74, 17);
            this.rbIP.TabIndex = 0;
            this.rbIP.TabStop = true;
            this.rbIP.Text = "Cámara IP";
            this.rbIP.UseVisualStyleBackColor = true;
            // 
            // txtCantidad
            // 
            this.txtCantidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCantidad.Location = new System.Drawing.Point(823, 103);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.ReadOnly = true;
            this.txtCantidad.Size = new System.Drawing.Size(100, 20);
            this.txtCantidad.TabIndex = 4;
            // 
            // dialogVideoFile
            // 
            this.dialogVideoFile.Filter = "\"Archivos de Video|*.mp4;*.avi;*.mpg;*.mov;*.mkv;\"";
            // 
            // lblFps
            // 
            this.lblFps.AutoSize = true;
            this.lblFps.Location = new System.Drawing.Point(584, 16);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(27, 13);
            this.lblFps.TabIndex = 5;
            this.lblFps.Text = "[fps]";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabVideo);
            this.tabControl1.Controls.Add(this.tabHSV);
            this.tabControl1.Controls.Add(this.tabConfig);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1035, 627);
            this.tabControl1.TabIndex = 6;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabVideo
            // 
            this.tabVideo.Controls.Add(this.lblPos);
            this.tabVideo.Controls.Add(this.btSeek);
            this.tabVideo.Controls.Add(this.pickerInicio);
            this.tabVideo.Controls.Add(this.label11);
            this.tabVideo.Controls.Add(this.btPause);
            this.tabVideo.Controls.Add(this.ckUseGender);
            this.tabVideo.Controls.Add(this.label10);
            this.tabVideo.Controls.Add(this.txtCantidadFemale);
            this.tabVideo.Controls.Add(this.txtCantMale);
            this.tabVideo.Controls.Add(this.label9);
            this.tabVideo.Controls.Add(this.label8);
            this.tabVideo.Controls.Add(this.gridMujeres);
            this.tabVideo.Controls.Add(this.gridHombres);
            this.tabVideo.Controls.Add(this.txtCantidad);
            this.tabVideo.Controls.Add(this.lblFps);
            this.tabVideo.Controls.Add(this.panel1);
            this.tabVideo.Controls.Add(this.imageBox);
            this.tabVideo.Controls.Add(this.groupBox1);
            this.tabVideo.Location = new System.Drawing.Point(4, 22);
            this.tabVideo.Name = "tabVideo";
            this.tabVideo.Padding = new System.Windows.Forms.Padding(3);
            this.tabVideo.Size = new System.Drawing.Size(1027, 601);
            this.tabVideo.TabIndex = 0;
            this.tabVideo.Text = "Video";
            this.tabVideo.UseVisualStyleBackColor = true;
            // 
            // lblPos
            // 
            this.lblPos.AutoSize = true;
            this.lblPos.Location = new System.Drawing.Point(336, 111);
            this.lblPos.Name = "lblPos";
            this.lblPos.Size = new System.Drawing.Size(28, 13);
            this.lblPos.TabIndex = 21;
            this.lblPos.Text = "Pos:";
            // 
            // btSeek
            // 
            this.btSeek.Location = new System.Drawing.Point(239, 106);
            this.btSeek.Name = "btSeek";
            this.btSeek.Size = new System.Drawing.Size(75, 23);
            this.btSeek.TabIndex = 20;
            this.btSeek.Text = "Ir";
            this.btSeek.UseVisualStyleBackColor = true;
            this.btSeek.Click += new System.EventHandler(this.btSeek_Click);
            // 
            // pickerInicio
            // 
            this.pickerInicio.CustomFormat = "H:mm:ss";
            this.pickerInicio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pickerInicio.Location = new System.Drawing.Point(136, 109);
            this.pickerInicio.Name = "pickerInicio";
            this.pickerInicio.ShowUpDown = true;
            this.pickerInicio.Size = new System.Drawing.Size(82, 20);
            this.pickerInicio.TabIndex = 19;
            this.pickerInicio.Value = new System.DateTime(2017, 2, 12, 0, 0, 0, 0);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(95, 111);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 13);
            this.label11.TabIndex = 18;
            this.label11.Text = "Seek:";
            // 
            // btPause
            // 
            this.btPause.Location = new System.Drawing.Point(8, 106);
            this.btPause.Name = "btPause";
            this.btPause.Size = new System.Drawing.Size(75, 23);
            this.btPause.TabIndex = 16;
            this.btPause.Text = "Pausar";
            this.btPause.UseVisualStyleBackColor = true;
            this.btPause.Click += new System.EventHandler(this.btPause_Click);
            // 
            // ckUseGender
            // 
            this.ckUseGender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckUseGender.AutoSize = true;
            this.ckUseGender.Location = new System.Drawing.Point(751, 129);
            this.ckUseGender.Name = "ckUseGender";
            this.ckUseGender.Size = new System.Drawing.Size(257, 17);
            this.ckUseGender.TabIndex = 15;
            this.ckUseGender.Text = "Detección de Genero por OpenCV (experimental)";
            this.ckUseGender.UseVisualStyleBackColor = true;
            this.ckUseGender.CheckedChanged += new System.EventHandler(this.ckUseGender_CheckedChanged);
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(925, 156);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Mujeres";
            // 
            // txtCantidadFemale
            // 
            this.txtCantidadFemale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCantidadFemale.Location = new System.Drawing.Point(892, 180);
            this.txtCantidadFemale.Name = "txtCantidadFemale";
            this.txtCantidadFemale.ReadOnly = true;
            this.txtCantidadFemale.Size = new System.Drawing.Size(129, 20);
            this.txtCantidadFemale.TabIndex = 13;
            // 
            // txtCantMale
            // 
            this.txtCantMale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCantMale.Location = new System.Drawing.Point(751, 180);
            this.txtCantMale.Name = "txtCantMale";
            this.txtCantMale.ReadOnly = true;
            this.txtCantMale.Size = new System.Drawing.Size(135, 20);
            this.txtCantMale.TabIndex = 12;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(779, 156);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Hombres";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(767, 106);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Total";
            // 
            // gridMujeres
            // 
            this.gridMujeres.AllowUserToAddRows = false;
            this.gridMujeres.AllowUserToDeleteRows = false;
            this.gridMujeres.AllowUserToResizeColumns = false;
            this.gridMujeres.AllowUserToResizeRows = false;
            this.gridMujeres.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridMujeres.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.gridMujeres.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridMujeres.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMujeres.ColumnHeadersVisible = false;
            this.gridMujeres.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewImageColumn2});
            this.gridMujeres.Location = new System.Drawing.Point(892, 206);
            this.gridMujeres.Name = "gridMujeres";
            this.gridMujeres.RowHeadersVisible = false;
            this.gridMujeres.Size = new System.Drawing.Size(129, 387);
            this.gridMujeres.TabIndex = 9;
            // 
            // dataGridViewImageColumn2
            // 
            this.dataGridViewImageColumn2.HeaderText = "Mujeres";
            this.dataGridViewImageColumn2.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Stretch;
            this.dataGridViewImageColumn2.Name = "dataGridViewImageColumn2";
            // 
            // gridHombres
            // 
            this.gridHombres.AllowUserToAddRows = false;
            this.gridHombres.AllowUserToDeleteRows = false;
            this.gridHombres.AllowUserToResizeColumns = false;
            this.gridHombres.AllowUserToResizeRows = false;
            this.gridHombres.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridHombres.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.gridHombres.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridHombres.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridHombres.ColumnHeadersVisible = false;
            this.gridHombres.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHombres});
            this.gridHombres.Location = new System.Drawing.Point(751, 206);
            this.gridHombres.Name = "gridHombres";
            this.gridHombres.RowHeadersVisible = false;
            this.gridHombres.Size = new System.Drawing.Size(135, 387);
            this.gridHombres.TabIndex = 8;
            this.gridHombres.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCaras_CellContentClick);
            // 
            // colHombres
            // 
            this.colHombres.HeaderText = "Hombres";
            this.colHombres.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Stretch;
            this.colHombres.Name = "colHombres";
            this.colHombres.Width = 128;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ckSkin);
            this.groupBox1.Controls.Add(this.ckMotion);
            this.groupBox1.Location = new System.Drawing.Point(823, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(163, 75);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Aceleración";
            // 
            // ckSkin
            // 
            this.ckSkin.AutoSize = true;
            this.ckSkin.Location = new System.Drawing.Point(8, 47);
            this.ckSkin.Name = "ckSkin";
            this.ckSkin.Size = new System.Drawing.Size(85, 17);
            this.ckSkin.TabIndex = 7;
            this.ckSkin.Text = "Color de Piel";
            this.ckSkin.UseVisualStyleBackColor = true;
            this.ckSkin.CheckedChanged += new System.EventHandler(this.ckSkin_CheckedChanged);
            // 
            // ckMotion
            // 
            this.ckMotion.AutoSize = true;
            this.ckMotion.Location = new System.Drawing.Point(8, 19);
            this.ckMotion.Name = "ckMotion";
            this.ckMotion.Size = new System.Drawing.Size(80, 17);
            this.ckMotion.TabIndex = 6;
            this.ckMotion.Text = "Movimiento";
            this.ckMotion.UseVisualStyleBackColor = true;
            this.ckMotion.CheckedChanged += new System.EventHandler(this.ckMotion_CheckedChanged);
            // 
            // tabHSV
            // 
            this.tabHSV.Controls.Add(this.label2);
            this.tabHSV.Controls.Add(this.tableLayoutPanel1);
            this.tabHSV.Location = new System.Drawing.Point(4, 22);
            this.tabHSV.Name = "tabHSV";
            this.tabHSV.Padding = new System.Windows.Forms.Padding(3);
            this.tabHSV.Size = new System.Drawing.Size(1027, 601);
            this.tabHSV.TabIndex = 1;
            this.tabHSV.Text = "Piel";
            this.tabHSV.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(185, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Mask";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1021, 595);
            this.tableLayoutPanel1.TabIndex = 10;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.imageBoxHue);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(513, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(505, 291);
            this.panel3.TabIndex = 1;
            // 
            // imageBoxHue
            // 
            this.imageBoxHue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBoxHue.Location = new System.Drawing.Point(30, 30);
            this.imageBoxHue.Name = "imageBoxHue";
            this.imageBoxHue.Size = new System.Drawing.Size(449, 240);
            this.imageBoxHue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBoxHue.TabIndex = 3;
            this.imageBoxHue.TabStop = false;
            this.imageBoxHue.Click += new System.EventHandler(this.imageBoxHue_Click_1);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(197, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Hue";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.imageBoxSat);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 300);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(504, 292);
            this.panel4.TabIndex = 2;
            // 
            // imageBoxSat
            // 
            this.imageBoxSat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBoxSat.Location = new System.Drawing.Point(18, 36);
            this.imageBoxSat.Name = "imageBoxSat";
            this.imageBoxSat.Size = new System.Drawing.Size(448, 240);
            this.imageBoxSat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBoxSat.TabIndex = 4;
            this.imageBoxSat.TabStop = false;
            this.imageBoxSat.Click += new System.EventHandler(this.imageBoxSat_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(164, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Sat";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.lblHSV);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.imageBoxValue);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(513, 300);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(505, 292);
            this.panel5.TabIndex = 3;
            // 
            // lblHSV
            // 
            this.lblHSV.AutoSize = true;
            this.lblHSV.ForeColor = System.Drawing.Color.Red;
            this.lblHSV.Location = new System.Drawing.Point(3, 10);
            this.lblHSV.Name = "lblHSV";
            this.lblHSV.Size = new System.Drawing.Size(29, 13);
            this.lblHSV.TabIndex = 8;
            this.lblHSV.Text = "HSV";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(197, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Value";
            // 
            // imageBoxValue
            // 
            this.imageBoxValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBoxValue.Location = new System.Drawing.Point(30, 38);
            this.imageBoxValue.Name = "imageBoxValue";
            this.imageBoxValue.Size = new System.Drawing.Size(449, 240);
            this.imageBoxValue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBoxValue.TabIndex = 5;
            this.imageBoxValue.TabStop = false;
            this.imageBoxValue.Click += new System.EventHandler(this.imageBoxValue_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.imageBoxMask);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(504, 291);
            this.panel2.TabIndex = 0;
            // 
            // imageBoxMask
            // 
            this.imageBoxMask.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBoxMask.Location = new System.Drawing.Point(27, 17);
            this.imageBoxMask.Name = "imageBoxMask";
            this.imageBoxMask.Size = new System.Drawing.Size(448, 253);
            this.imageBoxMask.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBoxMask.TabIndex = 2;
            this.imageBoxMask.TabStop = false;
            // 
            // tabConfig
            // 
            this.tabConfig.Controls.Add(this.ckShowCenters);
            this.tabConfig.Controls.Add(this.ckShowROI);
            this.tabConfig.Controls.Add(this.groupBox2);
            this.tabConfig.Location = new System.Drawing.Point(4, 22);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Size = new System.Drawing.Size(1027, 601);
            this.tabConfig.TabIndex = 2;
            this.tabConfig.Text = "Configuración";
            this.tabConfig.UseVisualStyleBackColor = true;
            // 
            // ckShowCenters
            // 
            this.ckShowCenters.AutoSize = true;
            this.ckShowCenters.Location = new System.Drawing.Point(22, 49);
            this.ckShowCenters.Name = "ckShowCenters";
            this.ckShowCenters.Size = new System.Drawing.Size(151, 17);
            this.ckShowCenters.TabIndex = 5;
            this.ckShowCenters.Text = "Mostrar centros de objetos";
            this.ckShowCenters.UseVisualStyleBackColor = true;
            this.ckShowCenters.CheckedChanged += new System.EventHandler(this.ckShowCenters_CheckedChanged);
            // 
            // ckShowROI
            // 
            this.ckShowROI.AutoSize = true;
            this.ckShowROI.Checked = true;
            this.ckShowROI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckShowROI.Location = new System.Drawing.Point(22, 17);
            this.ckShowROI.Name = "ckShowROI";
            this.ckShowROI.Size = new System.Drawing.Size(187, 17);
            this.ckShowROI.TabIndex = 4;
            this.ckShowROI.Text = "Mostrar Regiones de Interés (ROI)";
            this.ckShowROI.UseVisualStyleBackColor = true;
            this.ckShowROI.CheckedChanged += new System.EventHandler(this.ckShowROI_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cboxGeneroModelo);
            this.groupBox2.Controls.Add(this.lblGeneroHistoria);
            this.groupBox2.Controls.Add(this.trackGeneroHistoria);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(22, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(515, 139);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Detección de Genero OpenCV";
            // 
            // cboxGeneroModelo
            // 
            this.cboxGeneroModelo.FormattingEnabled = true;
            this.cboxGeneroModelo.Location = new System.Drawing.Point(189, 35);
            this.cboxGeneroModelo.Name = "cboxGeneroModelo";
            this.cboxGeneroModelo.Size = new System.Drawing.Size(186, 21);
            this.cboxGeneroModelo.TabIndex = 4;
            this.cboxGeneroModelo.SelectedIndexChanged += new System.EventHandler(this.cboxGeneroModelo_SelectedIndexChanged);
            // 
            // lblGeneroHistoria
            // 
            this.lblGeneroHistoria.AutoSize = true;
            this.lblGeneroHistoria.Location = new System.Drawing.Point(479, 75);
            this.lblGeneroHistoria.Name = "lblGeneroHistoria";
            this.lblGeneroHistoria.Size = new System.Drawing.Size(13, 13);
            this.lblGeneroHistoria.TabIndex = 3;
            this.lblGeneroHistoria.Text = "0";
            // 
            // trackGeneroHistoria
            // 
            this.trackGeneroHistoria.LargeChange = 10;
            this.trackGeneroHistoria.Location = new System.Drawing.Point(107, 63);
            this.trackGeneroHistoria.Maximum = 100;
            this.trackGeneroHistoria.Name = "trackGeneroHistoria";
            this.trackGeneroHistoria.Size = new System.Drawing.Size(366, 45);
            this.trackGeneroHistoria.SmallChange = 5;
            this.trackGeneroHistoria.TabIndex = 2;
            this.trackGeneroHistoria.TickFrequency = 5;
            this.trackGeneroHistoria.Value = 75;
            this.trackGeneroHistoria.ValueChanged += new System.EventHandler(this.trackGeneroHistoria_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(161, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Modelo de detección de genero:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Usar historia:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1035, 627);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeBegin += new System.EventHandler(this.Form1_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabVideo.ResumeLayout(false);
            this.tabVideo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMujeres)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridHombres)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabHSV.ResumeLayout(false);
            this.tabHSV.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxHue)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxSat)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxValue)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxMask)).EndInit();
            this.tabConfig.ResumeLayout(false);
            this.tabConfig.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackGeneroHistoria)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btArchivo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFuente;
        private System.Windows.Forms.Button btConectar;
        private System.Windows.Forms.RadioButton rbArchivo;
        private System.Windows.Forms.RadioButton rbIP;
        private System.Windows.Forms.RadioButton rbWeb;
        private System.Windows.Forms.TextBox txtCantidad;
        private System.Windows.Forms.OpenFileDialog dialogVideoFile;
        private System.Windows.Forms.Label lblFps;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabHSV;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private Emgu.CV.UI.ImageBox imageBoxValue;
        private Emgu.CV.UI.ImageBox imageBoxSat;
        private Emgu.CV.UI.ImageBox imageBoxHue;
        private Emgu.CV.UI.ImageBox imageBoxMask;
        private System.Windows.Forms.TabPage tabVideo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lblHSV;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckSkin;
        private System.Windows.Forms.CheckBox ckMotion;
        private System.Windows.Forms.TabPage tabConfig;
        private System.Windows.Forms.DataGridView gridHombres;
        private System.Windows.Forms.DataGridView gridMujeres;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn2;
        private System.Windows.Forms.DataGridViewImageColumn colHombres;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboxGeneroModelo;
        private System.Windows.Forms.Label lblGeneroHistoria;
        private System.Windows.Forms.TrackBar trackGeneroHistoria;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btPause;
        private System.Windows.Forms.CheckBox ckUseGender;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtCantidadFemale;
        private System.Windows.Forms.TextBox txtCantMale;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btSeek;
        private System.Windows.Forms.DateTimePicker pickerInicio;
        private System.Windows.Forms.Label lblPos;
        private System.Windows.Forms.CheckBox ckShowCenters;
        private System.Windows.Forms.CheckBox ckShowROI;
    }
}

