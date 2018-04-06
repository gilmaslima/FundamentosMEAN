using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;
using Redecard.PN.Comum.SharePoint.LogServico;
using System.Text;
using System.IO;
using System.Reflection;
using System.Web;

namespace Redecard.PN.Comum.SharePoint.LAYOUTS.Redecard.Comum
{
    public partial class UsuariosLogados : ApplicationPageBaseAutenticadaWindows
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                using (LogServicoClient client = new LogServicoClient())
                {
                    List<EstabelecimentosLogados> estabelecimentosLogados = client.ConsultaEstabelecimentosLogados();

                    String csv = GetCsv(estabelecimentosLogados);
                    DownloadCSV(csv);
                }
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
            }
        }

        private void DownloadCSV(String csv)
        {
            HttpContext.Current.Response.Clear(); 
            HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename=EstabelecimentosLogados.csv"));
            HttpContext.Current.Response.ContentType = "text/csv"; 
            HttpContext.Current.Response.AddHeader("Pragma", "public"); 
            HttpContext.Current.Response.Write(csv); 
            HttpContext.Current.Response.End();
        }

        private string GetCsv<T>(List<T> lista)
        {
            StringBuilder sb = new StringBuilder();

            PropertyInfo[] propInfos = typeof(T).GetProperties();
            
            for (int i = 1; i <= propInfos.Length - 1; i++)
            {
                sb.Append(propInfos[i].Name);

                if (i < propInfos.Length - 1)
                {
                    sb.Append(",");
                }
            }

            sb.AppendLine();

            for (int i = 0; i <= lista.Count - 1; i++)
            {
                T item = lista[i];
                for (int j = 1; j <= propInfos.Length - 1; j++)
                {
                    object o = item.GetType().GetProperty(propInfos[j].Name).GetValue(item, null);
                    if (o != null)
                    {
                        string value = o.ToString();
                        
                        if (value.Contains(","))
                        {
                            value = string.Concat("\"", value, "\"");
                        }

                        if (value.Contains("\r"))
                        {
                            value = value.Replace("\r", " ");
                        }
                        if (value.Contains(";\n"))
                        {
                            value = value.Replace("\n", " ");
                        }

                        sb.Append(value);
                    }

                    if (j < propInfos.Length - 1)
                    {
                        sb.Append(",");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
