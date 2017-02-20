using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace ContarPersonas
{
    class ReporteHelper
    {


        private HtmlTextWriter writer = null;
        private StreamWriter sw = null;
        private SortedDictionary<int, string> textos = new SortedDictionary<int, string>();
        private StringBuilder sb = new StringBuilder();
        private bool _nuevaFila = true;

        public void Iniciar(String reportName)
        {
            sw = new StreamWriter(reportName);
            writer = new HtmlTextWriter(sw);
        }

        public ReporteHelper()
        {
            
        }

        public ReporteHelper(String reportName)
        {
            Iniciar(reportName);
        }

        public void AgregarLinea(string linea)
        {
            writer.WriteLine(linea);
        }

        public void IniciarTabla(params string[] headers)
        {
            writer.WriteLine("<style>\ntable td, table th { border: 1px solid black; }");
            writer.WriteLine("img {width: 320px}\n</style>");
            writer.WriteLine("<h1>Reporte</h1>");
            writer.WriteLine("<table>");
            writer.WriteLine("<tr>");
            foreach (var field in headers)
            {
                writer.WriteLine("<th>{0}</th>", field);
            }
            writer.WriteLine("</tr>");
        }

        private void CrearNuevaFila()
        {
            if (_nuevaFila)
            {
                sb.AppendLine("<tr>");
            }
            _nuevaFila = false;
        }

        public void AgregarImagen(string jpgFilename,string timestamp)
        {
            CrearNuevaFila();
            string directory = Path.GetFileName(Path.GetDirectoryName(jpgFilename));
            sb.AppendLine(String.Format(
                "<td><a href='{0}' target='_blank'><img src='{0}' alt={1}></a></td>",
                directory + "\\" + Path.GetFileName(jpgFilename),
                "frame " + timestamp));
        }

        public void AgregarCeldas(params object[] data)
        {
            foreach (object x in data)
            {
                sb.AppendLine(String.Format("<td>{0}</td>", x));
            }
        }

        /// <summary>
        /// Escribe en disco la fila
        /// </summary>
        public void FinalizarFila()
        {
            writer.WriteLine(sb.ToString());
            writer.WriteLine("</tr>");
            sb.Clear();
            _nuevaFila = true;
        }


        public string ObtenerFila()
        {
            sb.AppendLine("</tr>");
            _nuevaFila = true;
            var ret =  sb.ToString();
            sb.Clear();
            return ret;
        }

        public void FinalizarTabla()
        {
            Debug.WriteLine("FinalizarTabla");
            writer.WriteLine("</table>");
            writer.WriteBreak();
        }

        public void Resumen(int totalDetecciones, int frames)
        {
            writer.Flush();
            writer.WriteLine("<h3>Total detecciones: {0}<br />", totalDetecciones.ToString());
            writer.WriteLine(String.Format("<h3>Promedio caras por frame: {0:0.0}<br />", totalDetecciones / (double)frames));
        }

        public void ResumenFinalizar(int totalDetecciones, int frames)
        {
            this.Resumen(totalDetecciones,frames);
            this.Finalizar();
        }


        public void Finalizar()
        {
            writer.Close();
            sw.Close();
        }
    }
}
