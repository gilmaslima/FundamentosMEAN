#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [08/10/2012] – [André Garcia] – [Criação]
*/
#endregion

using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using Redecard.PN.Comum;
using System.Xml.Serialization;
using System.Text;
using System.IO;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{
    /// <summary>
    /// Página de métodos simples para chamadas assíncronas
    /// </summary>
    public class Metodos : ApplicationPageBaseAnonima
    {
        /// <summary>
        /// Carregamento da página, veirifque qual o método chamado e retorna o XML resultante
        /// </summary>
        protected void Page_Load(Object sender, EventArgs e)
        {
            String metodo = Request.QueryString["metodo"];
            Response.Clear();
            switch (metodo)
            {
                case "recuperaragencia":
                    this.RecuperarAgencia();
                    break;
            }
            Response.End();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void RecuperarAgencia()
        {
            Int32 codigoRetorno = 0;
            Int32 codigoBanco = Request.QueryString["codbanco"].ToInt32();
            Int32 codigoAgencia = Request.QueryString["codagencia"].ToInt32();

            using (EntidadeServico.EntidadeServicoClient client = new EntidadeServico.EntidadeServicoClient())
            {
                var agencias = client.ConsultarAgencias(out codigoRetorno, codigoAgencia, codigoBanco);
                if (codigoRetorno == 0 && agencias.Length > 0)
                {
                    EntidadeServico.Agencia agencia = agencias[0];
                    StringBuilder sb = new StringBuilder();
                    StringWriter writer = new StringWriter(sb);

                    XmlSerializer serializer = new XmlSerializer(typeof(EntidadeServico.Agencia));
                    serializer.Serialize(writer, agencia);

                    String xml = sb.ToString();
                    xml = xml.Trim().Replace("utf-16", "utf-8");
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.ContentType = "text/xml";
                    Response.Write(xml);
                }
            }
        }
    }
}