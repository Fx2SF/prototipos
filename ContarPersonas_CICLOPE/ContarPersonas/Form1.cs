using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ContarPersonas
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (dialogSelectFile.ShowDialog() == DialogResult.OK)
            {
                lblSelectedFile.Text = dialogSelectFile.FileName;
                nombreBasePredeterminado();
                TimeSpan duration = new Extractor().GetDuration(dialogSelectFile.FileName);
                pickerFin.Value = DateTime.Today + duration;
                btSelecBase.Enabled = true;
            }
        }

        private void Terminar()
        {
            MessageBox.Show(this, "Terminó");
            btnStart.Enabled = true;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            string filename = dialogSelectFile.FileName;
            string reportName = dialogSelectBase.FileName;
            bool resizeThumb = cbImagenes.SelectedIndex == 1;
            
            TimeSpan  start = pickerInicio.Value.TimeOfDay;
            TimeSpan end = pickerFin.Value.TimeOfDay;
            int seconds = (int)numSeconds.Value;
            progressBar.Value = 0;
            btnStart.Enabled = false;


            Action<double> callback = progress =>
            {
                // usa Invoke para correr en el thread de UI
                progressBar.Invoke(new Action(() =>
                {
                    int newVal = (int) (progress * 100);
                    progressBar.Value = Math.Max(progressBar.Value, newVal);
                } ));
            };
            try
            {
                if (rbGoogle.Checked)
                {
                    ContarPersonasGoogle contar = new ContarPersonasGoogle();
                    contar.ResizeThumbnails = resizeThumb;
                    await contar.ProcessVideo(filename, reportName, start, end, seconds, checkEq.Checked, callback, ckAumentar.Checked);
                }
                else if (rbMicro.Checked)
                {
                    ContarPersonasMS contar = new ContarPersonasMS();
                    contar.ResizeThumbnails = resizeThumb;
                    await contar.ProcessVideoTime(filename, reportName, start, end, seconds,checkEq.Checked,callback, ckAumentar.Checked);
                }
                else
                {
                    double[] valores_scale = new double[] { 1.07, 1.12, 1.25, 1.4 };
                    double cascadeScale = valores_scale[comboScaleFactor.SelectedIndex];

                    ContarPersonasOpenCV contar = new ContarPersonasOpenCV();
                    contar.ResizeThumbnails = resizeThumb;
                    await contar.ProcessVideoTime(filename, reportName, start, end, seconds, callback, (int)updownSensitivity.Value, comboCascade.SelectedItem.ToString(),checkEq.Checked, cascadeScale);
                }
                Terminar();
              
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("Ex: " + ex.ToString());
                MessageBox.Show("Error: " + ex.ToString());
            }
            

        }

        private void EnablePanelContents(Panel panel, bool enabled)
        {
            foreach (Control ctrl in panel.Controls)
            {
                ctrl.Enabled = enabled;
            }
        }




        private void nombreBasePredeterminado()
        {
            string fileName = dialogSelectFile.FileName;
            string ext = Path.GetFileNameWithoutExtension(fileName);
            int idx = fileName.LastIndexOf(fileName);
            //string without_ext2 = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
            string withoutExt = idx > -1 ? fileName.Substring(0,idx) : fileName;
            dialogSelectBase.FileName = withoutExt;
            lblBase.Text = withoutExt;

        }

        private void cargarClaves()
        {
            txtClaveMS.Text = Properties.Settings.Default.MS_Face_API_key;
            txtArchivoJSON.Text = Properties.Settings.Default.Key_Google;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pickerInicio.Value = DateTime.Today;
            pickerFin.Value = DateTime.Today;
            var cascade = Directory.GetFiles(Application.StartupPath, "haar*.xml", SearchOption.TopDirectoryOnly).Select(p => Path.GetFileName(p));
            comboCascade.Items.Add("Face+Body");
            comboCascade.Items.AddRange(cascade.ToArray<object>());
            comboCascade.SelectedIndex = 0;
            comboScaleFactor.SelectedIndex = 0;
            cbImagenes.SelectedIndex = 0;
            lblBase.Text = "";
            lblSelectedFile.Text = "";
            cargarClaves();
            Opciones();
        }

        private void dialogSelectBase_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btSelecBase_Click(object sender, EventArgs e)
        {
            if (dialogSelectBase.ShowDialog() == DialogResult.OK){
                lblBase.Text = dialogSelectBase.FileName;
                btnStart.Enabled = true;

            }
        }

        private void btCambiarJSON_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON|*.json";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.Key_Google = dialog.FileName;
                Properties.Settings.Default.Save();
                txtArchivoJSON.Text = Properties.Settings.Default.Key_Google;

            }
        }

        private void btCambiarClave_Click(object sender, EventArgs e)
        {
            if (txtClaveMS.Text.Trim().Length > 0)
            {
                Properties.Settings.Default.MS_Free = rbFree.Checked;
                Properties.Settings.Default.MS_Face_API_key = txtClaveMS.Text.Trim();
                Properties.Settings.Default.Save();
            }
           
        }

        private void Opciones()
        {
            ckAumentar.Enabled = !rbOpen.Checked;
            comboCascade.Enabled = rbOpen.Checked;
            comboScaleFactor.Enabled = rbOpen.Checked;
            updownSensitivity.Enabled = rbOpen.Checked;
        }

        private void rbOpen_CheckedChanged(object sender, EventArgs e)
        {
            Opciones();
        }

        private void rbGoogle_CheckedChanged(object sender, EventArgs e)
        {
            Opciones();
        }

        private void rbMicro_CheckedChanged(object sender, EventArgs e)
        {
            Opciones();
        }

        private void tabEjecutar_Click(object sender, EventArgs e)
        {

        }
    }
}
