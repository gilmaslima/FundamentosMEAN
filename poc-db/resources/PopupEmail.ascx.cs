using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Extrato.SharePoint.Helper;
using System.Net;
using System.IO;
using System.Text;
using System.Web;
using Redecard.PN.Comum;
using Microsoft.SharePoint.Utilities;
using System.Web.UI.HtmlControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato
{
    public partial class PopupEmail : BaseUserControl
    {
        #region [ Propriedades do Controle ]

        /// <summary>Assunto do E-mail</summary>
        public String AssuntoEmail { get; set; }

        /// <summary>E-mail do remetente</summary>
        public String Remetente { get; set; }

        /// <summary>Título do popup de envio de e-mail</summary>
        public String TituloPopup { get; set; }

        /// <summary>Mensagem descritiva de envio de e-mail</summary>
        public String DescricaoPopup
        {
            get { return lblDescricao.Text; }
            set { lblDescricao.Text = value; }
        }

        /// <summary>Flag indicando se deve executar rotina para aplicar Inline Styles nos elementos</summary>
        public Boolean AplicarCSSInline { get; set; }

        public delegate void PrepararEmailHandler();
        /// <summary>Evento chamada após clique no botão de Envio de E-mail</summary>
        public event PrepararEmailHandler PrepararEmail;

        #endregion

        public void Page_Load(object sender, EventArgs e)
        {
            divPopEmail.Attributes["title"] = this.TituloPopup;
        }

        /// <summary>Função que deve ser chamada para envio do e-mail</summary>
        /// <param name="corpoEmail">Mensagem do corpo do e-mail</param>
        public void EnviarEmail(String corpoEmail)
        {
            try
            {
                //Obtém o e-mail do destinatário do textBox do controle
                String destinatario = txtEmail.Text;

                //Se algum dos campos destinatário ou mensagem forem nulos, interrompe rotina
                if (String.IsNullOrEmpty(destinatario.Trim())) return;
                if (String.IsNullOrEmpty(corpoEmail)) return;

                //Prepara o conteúdo do e-mail
                corpoEmail = String.Format(
                    "<html><body><div id=\"pageContentCentraliza\"><table class=\"tableContainer\"><tr><td valign=\"top\">" +
                    "<div class=\"res_box_conteudo\"><div>{0}</div></div></td></tr></table></div></body></html>", corpoEmail);

                //Se definido, aplica estilos inline nos elementos html
                if (AplicarCSSInline)
                {
                    //Obtém o conteúdo do CSS a ser aplicado nos elementos
                    String css = ObterCSS(Request, Page);

                    //Aplica estilos inline nos elementos
                    CSSInliner cssInliner = new CSSInliner(new CSSInlinerConfig
                    {
                        ConverterURLsRelativas = true,
                        TextoCSS = css
                    });

                    //Aplica o CSS nos elementos HTML
                    corpoEmail = cssInliner.Processar(corpoEmail);
                }

                //Envia o e-mail
                Utils.EnviarEmail(Remetente, destinatario, AssuntoEmail, corpoEmail, true);

                this.ExibirPainelMensagemUpdatePanel(
                    String.Format("E-mail enviado com sucesso para {0}.", destinatario), upnlEmail);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante envio de e-mail", ex);
                this.ExibirPainelMensagemUpdatePanel(this.RetornarMensagemErro(FONTE, CODIGO_ERRO), upnlEmail);
            }
        }

        private void ExibirPainelMensagemUpdatePanel(String mensagem, UpdatePanel updatePanel)
        {
            String titulo = this.GetGlobalResourceObject("redecard", "titMensagemErro").ToString();
            String script = String.Format("popupEmailFecharModal(); popupEmailExibirPainelMensagem('{0}', '{1}');", HttpUtility.HtmlEncode(titulo), HttpUtility.HtmlEncode(mensagem));
            ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "Key_" + this.ClientID, script, true);
        }

        private static String ObterURLAbsoluta(String relativeUrl)
        {
            if (String.IsNullOrEmpty(relativeUrl))
                return relativeUrl;

            if (HttpContext.Current == null)
                return relativeUrl;

            if (relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Insert(0, "~");
            if (!relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.Insert(0, "~/");

            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

            return String.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
        }

        protected void btnEnviarEmail_Click(object sender, EventArgs e)
        {
            try
            {
                if (PrepararEmail != null) PrepararEmail();
                txtEmail.Text = String.Empty;
            }
            catch
            {
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

#region [ CSS ]

        private static String _CSS;
        private static String ObterCSS(HttpRequest Request, Page Page)
        {
#if DEBUG
            _CSS = null;
#endif
            if (_CSS == null)
            {
                var urls = new[] { "ExtratoPorEmail.css" };

                StringBuilder css = new StringBuilder();

                foreach (String url in urls)
                {
                    String cssUrl = ObterURLAbsoluta(url);

                    try
                    {
                        //MÉTODO 1
                        //WebRequest req = WebRequest.Create(cssUrl);
                        //WebResponse result = req.GetResponse();
                        //Stream ReceiveStream = result.GetResponseStream();
                        //Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        //StreamReader sr = new StreamReader(ReceiveStream, encode);
                        //css.Append(sr.ReadToEnd());
                        //MÉTODO 2
                        //WebClient webClient = new WebClient();
                        //webClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                        //byte[] cssData = webClient.DownloadData(cssUrl);
                        //String textoCSS = new StreamReader(new MemoryStream(cssData)).ReadToEnd();
                        //css.Append(textoCSS);
                        var fPath = SPUtility.GetGenericSetupPath(@"TEMPLATE\LAYOUTS\Redecard.PN.Extrato.Sharepoint\Styles\" + url);
                        byte[] imgData = File.ReadAllBytes(fPath);
                        String textoCSS = new StreamReader(new MemoryStream(imgData)).ReadToEnd();
                        css.Append(textoCSS);
                    }
                    catch (Exception exc)
                    {
                        Logger.GravarErro("Erro durante leitura de CSS para envio de Extrato Por E-mail: " + cssUrl + ";" + url, exc);
                    }
                }
                _CSS = css.ToString();
            }
            return _CSS ?? "";
        }

#endregion
    }
}