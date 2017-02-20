namespace ContarPersonas
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
            this.dialogSelectFile = new System.Windows.Forms.OpenFileDialog();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.lblSelectedFile = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.dialogSelectBase = new System.Windows.Forms.SaveFileDialog();
            this.label8 = new System.Windows.Forms.Label();
            this.btSelecBase = new System.Windows.Forms.Button();
            this.lblBase = new System.Windows.Forms.Label();
            this.lblDuracion = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pickerFin = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.pickerInicio = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numSeconds = new System.Windows.Forms.NumericUpDown();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.updownSensitivity = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.comboCascade = new System.Windows.Forms.ComboBox();
            this.rbMicro = new System.Windows.Forms.RadioButton();
            this.rbGoogle = new System.Windows.Forms.RadioButton();
            this.rbOpen = new System.Windows.Forms.RadioButton();
            this.checkEq = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabClaves = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtArchivoJSON = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btCambiarJSON = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbPaid = new System.Windows.Forms.RadioButton();
            this.rbFree = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtClaveMS = new System.Windows.Forms.TextBox();
            this.btCambiarClave = new System.Windows.Forms.Button();
            this.tabEjecutar = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.ckAumentar = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.comboScaleFactor = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cbImagenes = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownSensitivity)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabClaves.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabEjecutar.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // dialogSelectFile
            // 
            this.dialogSelectFile.Filter = "\"Archivos de Video|*.mp4;*.avi;*.mpg;*.mov;*.mkv;\"";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(138, 17);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(159, 23);
            this.btnSelectFile.TabIndex = 0;
            this.btnSelectFile.Text = "Seleccionar Video";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // lblSelectedFile
            // 
            this.lblSelectedFile.AutoSize = true;
            this.lblSelectedFile.Location = new System.Drawing.Point(13, 57);
            this.lblSelectedFile.Name = "lblSelectedFile";
            this.lblSelectedFile.Size = new System.Drawing.Size(55, 13);
            this.lblSelectedFile.TabIndex = 1;
            this.lblSelectedFile.Text = "[Filename]";
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(52, 541);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(323, 23);
            this.btnStart.TabIndex = 11;
            this.btnStart.Text = "Empezar";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // dialogSelectBase
            // 
            this.dialogSelectBase.Filter = "HTML|*.html";
            this.dialogSelectBase.FileOk += new System.ComponentModel.CancelEventHandler(this.dialogSelectBase_FileOk);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Nombre del reporte:";
            // 
            // btSelecBase
            // 
            this.btSelecBase.Enabled = false;
            this.btSelecBase.Location = new System.Drawing.Point(131, 19);
            this.btSelecBase.Name = "btSelecBase";
            this.btSelecBase.Size = new System.Drawing.Size(159, 23);
            this.btSelecBase.TabIndex = 21;
            this.btSelecBase.Text = "Seleccionar";
            this.btSelecBase.UseVisualStyleBackColor = true;
            this.btSelecBase.Click += new System.EventHandler(this.btSelecBase_Click);
            // 
            // lblBase
            // 
            this.lblBase.AutoSize = true;
            this.lblBase.Location = new System.Drawing.Point(6, 52);
            this.lblBase.Name = "lblBase";
            this.lblBase.Size = new System.Drawing.Size(37, 13);
            this.lblBase.TabIndex = 22;
            this.lblBase.Text = "[Base]";
            // 
            // lblDuracion
            // 
            this.lblDuracion.AutoSize = true;
            this.lblDuracion.Location = new System.Drawing.Point(77, 25);
            this.lblDuracion.Name = "lblDuracion";
            this.lblDuracion.Size = new System.Drawing.Size(76, 13);
            this.lblDuracion.TabIndex = 16;
            this.lblDuracion.Text = "                       ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(180, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "segundos";
            // 
            // pickerFin
            // 
            this.pickerFin.CustomFormat = "H:mm:ss";
            this.pickerFin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pickerFin.Location = new System.Drawing.Point(191, 59);
            this.pickerFin.Name = "pickerFin";
            this.pickerFin.ShowUpDown = true;
            this.pickerFin.Size = new System.Drawing.Size(103, 20);
            this.pickerFin.TabIndex = 13;
            this.pickerFin.Value = new System.DateTime(2017, 2, 12, 17, 19, 24, 0);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(161, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Fin:";
            // 
            // pickerInicio
            // 
            this.pickerInicio.CustomFormat = "H:mm:ss";
            this.pickerInicio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pickerInicio.Location = new System.Drawing.Point(59, 59);
            this.pickerInicio.Name = "pickerInicio";
            this.pickerInicio.ShowUpDown = true;
            this.pickerInicio.Size = new System.Drawing.Size(82, 20);
            this.pickerInicio.TabIndex = 11;
            this.pickerInicio.Value = new System.DateTime(2017, 2, 12, 17, 19, 24, 0);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Inicio:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Seleccionar cada:";
            // 
            // numSeconds
            // 
            this.numSeconds.Location = new System.Drawing.Point(112, 103);
            this.numSeconds.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numSeconds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSeconds.Name = "numSeconds";
            this.numSeconds.Size = new System.Drawing.Size(62, 20);
            this.numSeconds.TabIndex = 9;
            this.numSeconds.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(16, 588);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(413, 23);
            this.progressBar.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 446);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Precisión:";
            // 
            // updownSensitivity
            // 
            this.updownSensitivity.Location = new System.Drawing.Point(142, 444);
            this.updownSensitivity.Name = "updownSensitivity";
            this.updownSensitivity.Size = new System.Drawing.Size(120, 20);
            this.updownSensitivity.TabIndex = 25;
            this.updownSensitivity.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 478);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Archivo cascade:";
            // 
            // comboCascade
            // 
            this.comboCascade.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCascade.FormattingEnabled = true;
            this.comboCascade.Location = new System.Drawing.Point(142, 475);
            this.comboCascade.Name = "comboCascade";
            this.comboCascade.Size = new System.Drawing.Size(233, 21);
            this.comboCascade.TabIndex = 27;
            // 
            // rbMicro
            // 
            this.rbMicro.AutoSize = true;
            this.rbMicro.Location = new System.Drawing.Point(215, 373);
            this.rbMicro.Name = "rbMicro";
            this.rbMicro.Size = new System.Drawing.Size(68, 17);
            this.rbMicro.TabIndex = 29;
            this.rbMicro.Text = "Microsoft";
            this.rbMicro.UseVisualStyleBackColor = true;
            this.rbMicro.CheckedChanged += new System.EventHandler(this.rbMicro_CheckedChanged);
            // 
            // rbGoogle
            // 
            this.rbGoogle.AutoSize = true;
            this.rbGoogle.Location = new System.Drawing.Point(128, 373);
            this.rbGoogle.Name = "rbGoogle";
            this.rbGoogle.Size = new System.Drawing.Size(59, 17);
            this.rbGoogle.TabIndex = 30;
            this.rbGoogle.Text = "Google";
            this.rbGoogle.UseVisualStyleBackColor = true;
            this.rbGoogle.CheckedChanged += new System.EventHandler(this.rbGoogle_CheckedChanged);
            // 
            // rbOpen
            // 
            this.rbOpen.AutoSize = true;
            this.rbOpen.Checked = true;
            this.rbOpen.Location = new System.Drawing.Point(33, 373);
            this.rbOpen.Name = "rbOpen";
            this.rbOpen.Size = new System.Drawing.Size(65, 17);
            this.rbOpen.TabIndex = 31;
            this.rbOpen.TabStop = true;
            this.rbOpen.Text = "OpenCV";
            this.rbOpen.UseVisualStyleBackColor = true;
            this.rbOpen.CheckedChanged += new System.EventHandler(this.rbOpen_CheckedChanged);
            // 
            // checkEq
            // 
            this.checkEq.AutoSize = true;
            this.checkEq.Location = new System.Drawing.Point(266, 409);
            this.checkEq.Name = "checkEq";
            this.checkEq.Size = new System.Drawing.Size(69, 17);
            this.checkEq.TabIndex = 32;
            this.checkEq.Text = "Equalizar";
            this.checkEq.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabEjecutar);
            this.tabControl1.Controls.Add(this.tabClaves);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(445, 643);
            this.tabControl1.TabIndex = 34;
            // 
            // tabClaves
            // 
            this.tabClaves.Controls.Add(this.groupBox2);
            this.tabClaves.Controls.Add(this.groupBox1);
            this.tabClaves.Location = new System.Drawing.Point(4, 22);
            this.tabClaves.Name = "tabClaves";
            this.tabClaves.Padding = new System.Windows.Forms.Padding(3);
            this.tabClaves.Size = new System.Drawing.Size(437, 601);
            this.tabClaves.TabIndex = 1;
            this.tabClaves.Text = "Claves";
            this.tabClaves.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtArchivoJSON);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.btCambiarJSON);
            this.groupBox2.Location = new System.Drawing.Point(6, 137);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(423, 113);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Google Cloud Vision API";
            // 
            // txtArchivoJSON
            // 
            this.txtArchivoJSON.Enabled = false;
            this.txtArchivoJSON.Location = new System.Drawing.Point(19, 66);
            this.txtArchivoJSON.Name = "txtArchivoJSON";
            this.txtArchivoJSON.Size = new System.Drawing.Size(398, 20);
            this.txtArchivoJSON.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 30);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(90, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Archivo de clave:";
            // 
            // btCambiarJSON
            // 
            this.btCambiarJSON.Location = new System.Drawing.Point(148, 25);
            this.btCambiarJSON.Name = "btCambiarJSON";
            this.btCambiarJSON.Size = new System.Drawing.Size(75, 23);
            this.btCambiarJSON.TabIndex = 2;
            this.btCambiarJSON.Text = "Cambiar";
            this.btCambiarJSON.UseVisualStyleBackColor = true;
            this.btCambiarJSON.Click += new System.EventHandler(this.btCambiarJSON_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbPaid);
            this.groupBox1.Controls.Add(this.rbFree);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtClaveMS);
            this.groupBox1.Controls.Add(this.btCambiarClave);
            this.groupBox1.Location = new System.Drawing.Point(6, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(423, 97);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Microsoft Face API";
            // 
            // rbPaid
            // 
            this.rbPaid.AutoSize = true;
            this.rbPaid.Location = new System.Drawing.Point(102, 55);
            this.rbPaid.Name = "rbPaid";
            this.rbPaid.Size = new System.Drawing.Size(39, 17);
            this.rbPaid.TabIndex = 6;
            this.rbPaid.TabStop = true;
            this.rbPaid.Text = "No";
            this.rbPaid.UseVisualStyleBackColor = true;
            // 
            // rbFree
            // 
            this.rbFree.AutoSize = true;
            this.rbFree.Checked = true;
            this.rbFree.Location = new System.Drawing.Point(49, 55);
            this.rbFree.Name = "rbFree";
            this.rbFree.Size = new System.Drawing.Size(34, 17);
            this.rbFree.TabIndex = 5;
            this.rbFree.TabStop = true;
            this.rbFree.Text = "Si";
            this.rbFree.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 57);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(31, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Free:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Clave:";
            // 
            // txtClaveMS
            // 
            this.txtClaveMS.Location = new System.Drawing.Point(49, 20);
            this.txtClaveMS.Name = "txtClaveMS";
            this.txtClaveMS.Size = new System.Drawing.Size(251, 20);
            this.txtClaveMS.TabIndex = 1;
            this.txtClaveMS.UseSystemPasswordChar = true;
            // 
            // btCambiarClave
            // 
            this.btCambiarClave.Location = new System.Drawing.Point(333, 47);
            this.btCambiarClave.Name = "btCambiarClave";
            this.btCambiarClave.Size = new System.Drawing.Size(75, 23);
            this.btCambiarClave.TabIndex = 2;
            this.btCambiarClave.Text = "Guardar";
            this.btCambiarClave.UseVisualStyleBackColor = true;
            this.btCambiarClave.Click += new System.EventHandler(this.btCambiarClave_Click);
            // 
            // tabEjecutar
            // 
            this.tabEjecutar.Controls.Add(this.groupBox4);
            this.tabEjecutar.Controls.Add(this.comboScaleFactor);
            this.tabEjecutar.Controls.Add(this.label14);
            this.tabEjecutar.Controls.Add(this.label13);
            this.tabEjecutar.Controls.Add(this.ckAumentar);
            this.tabEjecutar.Controls.Add(this.label12);
            this.tabEjecutar.Controls.Add(this.checkEq);
            this.tabEjecutar.Controls.Add(this.btnSelectFile);
            this.tabEjecutar.Controls.Add(this.progressBar);
            this.tabEjecutar.Controls.Add(this.rbOpen);
            this.tabEjecutar.Controls.Add(this.label6);
            this.tabEjecutar.Controls.Add(this.lblSelectedFile);
            this.tabEjecutar.Controls.Add(this.rbGoogle);
            this.tabEjecutar.Controls.Add(this.updownSensitivity);
            this.tabEjecutar.Controls.Add(this.btnStart);
            this.tabEjecutar.Controls.Add(this.rbMicro);
            this.tabEjecutar.Controls.Add(this.label7);
            this.tabEjecutar.Controls.Add(this.comboCascade);
            this.tabEjecutar.Controls.Add(this.groupBox3);
            this.tabEjecutar.Location = new System.Drawing.Point(4, 22);
            this.tabEjecutar.Name = "tabEjecutar";
            this.tabEjecutar.Padding = new System.Windows.Forms.Padding(3);
            this.tabEjecutar.Size = new System.Drawing.Size(437, 617);
            this.tabEjecutar.TabIndex = 0;
            this.tabEjecutar.Text = "Procesar";
            this.tabEjecutar.UseVisualStyleBackColor = true;
            this.tabEjecutar.Click += new System.EventHandler(this.tabEjecutar_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(91, 13);
            this.label12.TabIndex = 33;
            this.label12.Text = "Archivo de Video:";
            // 
            // ckAumentar
            // 
            this.ckAumentar.AutoSize = true;
            this.ckAumentar.Checked = true;
            this.ckAumentar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckAumentar.Location = new System.Drawing.Point(142, 409);
            this.ckAumentar.Name = "ckAumentar";
            this.ckAumentar.Size = new System.Drawing.Size(96, 17);
            this.ckAumentar.TabIndex = 34;
            this.ckAumentar.Text = "Redimensionar";
            this.ckAumentar.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(17, 410);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(95, 13);
            this.label13.TabIndex = 35;
            this.label13.Text = "Preprocesamiento:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(14, 505);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(112, 13);
            this.label14.TabIndex = 36;
            this.label14.Text = "Cascade ScaleFactor:";
            // 
            // comboScaleFactor
            // 
            this.comboScaleFactor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboScaleFactor.FormattingEnabled = true;
            this.comboScaleFactor.Items.AddRange(new object[] {
            "1.07 (Lento - Más detecciones)",
            "1.12 (Medio)",
            "1.25 (Rápido)",
            "1.40 (Más Rápido - Menos detecciones)"});
            this.comboScaleFactor.Location = new System.Drawing.Point(142, 502);
            this.comboScaleFactor.Name = "comboScaleFactor";
            this.comboScaleFactor.Size = new System.Drawing.Size(206, 21);
            this.comboScaleFactor.TabIndex = 37;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(9, 79);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(112, 13);
            this.label15.TabIndex = 38;
            this.label15.Text = "Tamaño de imágenes:";
            // 
            // cbImagenes
            // 
            this.cbImagenes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImagenes.FormattingEnabled = true;
            this.cbImagenes.Items.AddRange(new object[] {
            "100%",
            " 50%"});
            this.cbImagenes.Location = new System.Drawing.Point(131, 76);
            this.cbImagenes.Name = "cbImagenes";
            this.cbImagenes.Size = new System.Drawing.Size(159, 21);
            this.cbImagenes.TabIndex = 39;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbImagenes);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.btSelecBase);
            this.groupBox3.Controls.Add(this.lblBase);
            this.groupBox3.Location = new System.Drawing.Point(16, 89);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(413, 124);
            this.groupBox3.TabIndex = 40;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Reporte";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblDuracion);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.numSeconds);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.pickerFin);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.pickerInicio);
            this.groupBox4.Location = new System.Drawing.Point(16, 219);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(413, 132);
            this.groupBox4.TabIndex = 41;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Selección de Frames";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Duración:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 643);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Contar Personas";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownSensitivity)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabClaves.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabEjecutar.ResumeLayout(false);
            this.tabEjecutar.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dialogSelectFile;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Label lblSelectedFile;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.SaveFileDialog dialogSelectBase;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btSelecBase;
        private System.Windows.Forms.Label lblBase;
        private System.Windows.Forms.Label lblDuracion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker pickerFin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker pickerInicio;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numSeconds;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown updownSensitivity;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboCascade;
        private System.Windows.Forms.RadioButton rbMicro;
        private System.Windows.Forms.RadioButton rbGoogle;
        private System.Windows.Forms.RadioButton rbOpen;
        private System.Windows.Forms.CheckBox checkEq;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabClaves;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtArchivoJSON;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btCambiarJSON;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtClaveMS;
        private System.Windows.Forms.Button btCambiarClave;
        private System.Windows.Forms.TabPage tabEjecutar;
        private System.Windows.Forms.RadioButton rbPaid;
        private System.Windows.Forms.RadioButton rbFree;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox ckAumentar;
        private System.Windows.Forms.ComboBox comboScaleFactor;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cbImagenes;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label4;
    }
}

