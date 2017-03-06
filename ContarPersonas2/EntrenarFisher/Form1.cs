using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntrenarFisher
{
    public partial class Form1 : Form
    {
        private string saveModelFileName = null;


        public Form1()
        {
            InitializeComponent();
        }

        private void btnModelName_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Modelo|*.fisher";
           
            if (save.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(save.FileName))
                {
                    MessageBox.Show("No está permitido sobreescribir");
                }
                else
                {
                    saveModelFileName = save.FileName;
                    var nombreConfig = saveModelFileName + ".csv";
                    txtConfig.Text = nombreConfig;
                    txtModel.Text = saveModelFileName;
                    btTrain.Enabled = true;
                }
            }
        }

        private async void btTrain_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                btTrain.Enabled = false;
                btnModelName.Enabled = false;
                btCSV.Enabled = false;
                btConfig.Enabled = false;
                Cursor = Cursors.WaitCursor; // change cursor to hourglass type
                if (rbNorm.Checked)
                {
                    Size norm = new Size((int) numWidth.Value, (int) numHeight.Value);
                    await GenderTraining.Train(csvFaces.FileName, saveModelFileName, txtConfig.Text, norm);
                }
                else
                {
                    await GenderTraining.Train(csvFaces.FileName,saveModelFileName,txtConfig.Text);
                }
                Cursor = Cursors.Default; // change cursor to normal type
                MessageBox.Show(String.Format("Termino el entrenamiento.\n Demoró {0}s.\nGuardado a: {1}",
                    sw.ElapsedMilliseconds / 1000, saveModelFileName));
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default; // change cursor to normal type
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                btCSV.Enabled = true;
            }
        }

        private void btCsv_Click(object sender, EventArgs e)
        {
            if (csvFaces.ShowDialog() == DialogResult.OK)
            {
                btnModelName.Enabled = true;
                btConfig.Enabled = true;
                txtCsvFile.Text = csvFaces.FileName;
            }
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbNorm_CheckedChanged(object sender, EventArgs e)
        {
            numHeight.Enabled = rbNorm.Checked;
            numWidth.Enabled = rbNorm.Checked;
        }

        private void btConfig_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "CSV|*.csv";
            if (save.ShowDialog() == DialogResult.OK)
            {
                txtConfig.Text = save.FileName;
            }
        }
    }
}