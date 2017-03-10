namespace EntrenarFisher
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
            this.btnModelName = new System.Windows.Forms.Button();
            this.btTrain = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.btCSV = new System.Windows.Forms.Button();
            this.csvFaces = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbNorm = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.txtCsvFile = new System.Windows.Forms.TextBox();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.btConfig = new System.Windows.Forms.Button();
            this.txtConfig = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnModelName
            // 
            this.btnModelName.Enabled = false;
            this.btnModelName.Location = new System.Drawing.Point(12, 59);
            this.btnModelName.Name = "btnModelName";
            this.btnModelName.Size = new System.Drawing.Size(140, 23);
            this.btnModelName.TabIndex = 1;
            this.btnModelName.Text = "Elegir nombre del Modelo";
            this.btnModelName.UseVisualStyleBackColor = true;
            this.btnModelName.Click += new System.EventHandler(this.btnModelName_Click);
            // 
            // btTrain
            // 
            this.btTrain.Enabled = false;
            this.btTrain.Location = new System.Drawing.Point(12, 240);
            this.btTrain.Name = "btTrain";
            this.btTrain.Size = new System.Drawing.Size(456, 23);
            this.btTrain.TabIndex = 0;
            this.btTrain.Text = "Entrenar";
            this.btTrain.UseVisualStyleBackColor = true;
            this.btTrain.Click += new System.EventHandler(this.btTrain_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(180, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Ancho:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(306, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Alto:";
            // 
            // numWidth
            // 
            this.numWidth.Enabled = false;
            this.numWidth.Location = new System.Drawing.Point(227, 54);
            this.numWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numWidth.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(73, 20);
            this.numWidth.TabIndex = 5;
            this.numWidth.Value = new decimal(new int[] {
            140,
            0,
            0,
            0});
            // 
            // numHeight
            // 
            this.numHeight.Enabled = false;
            this.numHeight.Location = new System.Drawing.Point(340, 54);
            this.numHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numHeight.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(73, 20);
            this.numHeight.TabIndex = 6;
            this.numHeight.Value = new decimal(new int[] {
            220,
            0,
            0,
            0});
            // 
            // btCSV
            // 
            this.btCSV.Location = new System.Drawing.Point(19, 12);
            this.btCSV.Name = "btCSV";
            this.btCSV.Size = new System.Drawing.Size(124, 23);
            this.btCSV.TabIndex = 8;
            this.btCSV.Text = "Elegir CSV";
            this.btCSV.UseVisualStyleBackColor = true;
            this.btCSV.Click += new System.EventHandler(this.btCsv_Click);
            // 
            // csvFaces
            // 
            this.csvFaces.FileName = "faces.csv";
            this.csvFaces.Filter = "CSV|*.csv";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbNorm);
            this.groupBox1.Controls.Add(this.rbAll);
            this.groupBox1.Controls.Add(this.numWidth);
            this.groupBox1.Controls.Add(this.numHeight);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(430, 101);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tamaño de Imágenes";
            // 
            // rbNorm
            // 
            this.rbNorm.AutoSize = true;
            this.rbNorm.Location = new System.Drawing.Point(12, 54);
            this.rbNorm.Name = "rbNorm";
            this.rbNorm.Size = new System.Drawing.Size(165, 17);
            this.rbNorm.TabIndex = 8;
            this.rbNorm.TabStop = true;
            this.rbNorm.Text = "Recortar y normalizar tamaño:";
            this.rbNorm.UseVisualStyleBackColor = true;
            this.rbNorm.CheckedChanged += new System.EventHandler(this.rbNorm_CheckedChanged);
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Location = new System.Drawing.Point(12, 19);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(232, 17);
            this.rbAll.TabIndex = 7;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "Todas las imágenes tienen el mismo tamaño";
            this.rbAll.UseVisualStyleBackColor = true;
            this.rbAll.CheckedChanged += new System.EventHandler(this.rbAll_CheckedChanged);
            // 
            // txtCsvFile
            // 
            this.txtCsvFile.Location = new System.Drawing.Point(158, 14);
            this.txtCsvFile.Name = "txtCsvFile";
            this.txtCsvFile.ReadOnly = true;
            this.txtCsvFile.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtCsvFile.Size = new System.Drawing.Size(310, 20);
            this.txtCsvFile.TabIndex = 12;
            this.txtCsvFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtModel
            // 
            this.txtModel.Location = new System.Drawing.Point(158, 61);
            this.txtModel.Name = "txtModel";
            this.txtModel.ReadOnly = true;
            this.txtModel.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtModel.Size = new System.Drawing.Size(310, 20);
            this.txtModel.TabIndex = 13;
            this.txtModel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btConfig
            // 
            this.btConfig.Enabled = false;
            this.btConfig.Location = new System.Drawing.Point(12, 96);
            this.btConfig.Name = "btConfig";
            this.btConfig.Size = new System.Drawing.Size(140, 23);
            this.btConfig.TabIndex = 14;
            this.btConfig.Text = "Elegir archivo config";
            this.btConfig.UseVisualStyleBackColor = true;
            this.btConfig.Click += new System.EventHandler(this.btConfig_Click);
            // 
            // txtConfig
            // 
            this.txtConfig.Location = new System.Drawing.Point(158, 99);
            this.txtConfig.Name = "txtConfig";
            this.txtConfig.ReadOnly = true;
            this.txtConfig.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtConfig.Size = new System.Drawing.Size(310, 20);
            this.txtConfig.TabIndex = 15;
            this.txtConfig.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 275);
            this.Controls.Add(this.txtConfig);
            this.Controls.Add(this.btConfig);
            this.Controls.Add(this.txtModel);
            this.Controls.Add(this.txtCsvFile);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btCSV);
            this.Controls.Add(this.btnModelName);
            this.Controls.Add(this.btTrain);
            this.Name = "Form1";
            this.Text = "Entrenar";
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnModelName;
        private System.Windows.Forms.Button btTrain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.Button btCSV;
        private System.Windows.Forms.OpenFileDialog csvFaces;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbNorm;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.TextBox txtCsvFile;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Button btConfig;
        private System.Windows.Forms.TextBox txtConfig;
    }
}

