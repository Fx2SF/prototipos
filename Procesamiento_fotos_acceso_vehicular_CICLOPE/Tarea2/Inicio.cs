using System;
using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Reflection;


namespace Tarea2
{
    public partial class Inicio : Form
    {


       private string destino = "";
       private string date = "";
       public Inicio()
            {
                InitializeComponent();
                this.destino = Properties.Settings.Default.destino;
                this.date = DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss");
            }

        private void button1_Click(object sender, EventArgs e)
        {

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    this.textBox1.Text = fbd.SelectedPath.ToString();
                    this.label1.Text = files.Length.ToString() + " archivos encontrados.";
                }
            }           

        }

        private async void button2_Click(object sender, EventArgs e)
        {

            // Excel

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.Visible = true;

            if (xlApp == null)
            {
                MessageBox.Show("Excel no esta instalado");
                return;
            }
            Workbook wb = xlApp.Workbooks.Add();

            if (this.textBox1.Text.CompareTo("") == 0)
            {
                System.Windows.Forms.MessageBox.Show("Debe seleccionar una carpeta con imagenes de autos.");
            }
            else
            {

                if (checkBox1.Checked == true && checkBox2.Checked == false)
                {

                    CognitiveService cog = new CognitiveService();
                    await cog.DoWork(this.textBox1.Text, wb);

                }
                else if (checkBox1.Checked == false && checkBox2.Checked == true)
                {

                    CloudVision clo = new CloudVision();
                    await clo.DoWork(this.textBox1.Text, wb);

                }
                else if (checkBox1.Checked == true && checkBox2.Checked == true)
                {

                    CloudVision clo = new CloudVision();
                    await clo.DoWork(this.textBox1.Text, wb);

                    CognitiveService cog = new CognitiveService();
                    await cog.DoWork(this.textBox1.Text, wb);

                }
                else if (checkBox1.Checked == false && checkBox2.Checked == false)
                {

                    System.Windows.Forms.MessageBox.Show("Debe seleccionar al menos una opcion.");

                }


                wb.SaveAs(this.destino + this.date + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Missing.Value,
                    Missing.Value, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true,
                    Missing.Value, Missing.Value, Missing.Value);

                wb.Close();
                xlApp.Quit();


            }

        }
    }
}
