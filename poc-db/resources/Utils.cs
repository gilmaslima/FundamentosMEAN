using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.SharePoint.Helper
{
    public abstract class Utils
    {
        #region [ Download de Arquivos ]

        /// <summary>
        /// Realiza o download de um conteúdo HTML em formato XLS.
        /// </summary>
        /// <param name="htmlContent">Conteúdo HTML</param>
        /// <param name="nomeArquivo">Nome do arquivo</param>
        public static void DownloadXLS(String htmlContent, String nomeArquivo)
        {
            //Ajuste nome do arquivo (remove qualquer diretório e extensão)
            nomeArquivo = Path.GetFileNameWithoutExtension(nomeArquivo) + ".xls";
            DownloadArquivo(htmlContent, nomeArquivo, "application/ms-excel");            
        }

        /// <summary>
        /// Realiza o download de um conteúdo CSV.
        /// </summary>
        /// <param name="csvContent">Conteúdo CSV</param>
        /// <param name="nomeArquivo">Nome do arquivo</param>
        public static void DownloadCSV(String csvContent, String nomeArquivo)
        {
            //Ajuste nome do arquivo (remove qualquer diretório e extensão)
            nomeArquivo = Path.GetFileNameWithoutExtension(nomeArquivo) + ".csv";
            DownloadArquivo(csvContent, nomeArquivo, "application/ms-excel");            
        }

        public static void DownloadDOC(String htmlContent, String nomeArquivo)
        {
            nomeArquivo = Path.GetFileNameWithoutExtension(nomeArquivo) + ".doc";
            htmlContent = HtmlAppRelativeUrlsToAbsoluteUrls(htmlContent);
            DownloadArquivo(htmlContent, nomeArquivo, "application/download");          
        }

        public static void DownloadHTML(String htmlContent, String nomeArquivo)
        {
            nomeArquivo = Path.GetFileNameWithoutExtension(nomeArquivo) + ".htm";
            htmlContent = HtmlAppRelativeUrlsToAbsoluteUrls(htmlContent);
            DownloadArquivo(htmlContent, nomeArquivo, "application/download");
        }

        public static void DownloadTXT(String txtContent, String nomeArquivo)
        {
            nomeArquivo = Path.GetFileNameWithoutExtension(nomeArquivo) + ".txt";
            DownloadArquivo(txtContent, nomeArquivo, "application/download");
        }

        private static void DownloadArquivo(String content, String nomeArquivo, String contentType)
        {
            //Define o Enconding para Ansi
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
            
            //Retorna
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = contentType;
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);//Define para não ter cache
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + nomeArquivo);    //Define o nome do arquivo            
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    HttpContext.Current.Response.Write(content);
                    HttpContext.Current.Response.End();
                }
            }
        }

        #endregion

        #region [ Envio de E-mail]

        /// <summary>
        /// Envia e-mail utilizando a configuração SMTP do SharePoint
        /// </summary>
        /// <param name="origem">E-mail do remetente</param>
        /// <param name="destino">E-mail do destinatário</param>
        /// <param name="assunto">Assunto do e-mail</param>
        /// <param name="corpoHtml">Mensagem do e-mail (html)</param>        
        public static void EnviarEmail(String origem, String destino, String assunto, String corpoHtml)
        {
            EnviarEmail(origem, destino, assunto, corpoHtml, false);
        }

        /// <summary>
        /// Envia e-mail utilizando a configuração SMTP do SharePoint
        /// </summary>
        /// <param name="origem">E-mail do remetente</param>
        /// <param name="destino">E-mail do destinatário</param>
        /// <param name="assunto">Assunto do e-mail</param>
        /// <param name="corpoHtml">Mensagem do e-mail (html)</param>
        /// <param name="embutirImagens">Embute ou não as imagens no próprio e-mail</param>
        public static void EnviarEmail(String origem, String destino, String assunto, String corpoHtml, Boolean embutirImagens)
        {
            //Cria o objeto para envio de e-mail (Buscando da configuração do Sharepoint)
            string smtpServer = SPAdministrationWebApplication.Local.OutboundMailServiceInstance != null ?
                                SPAdministrationWebApplication.Local.OutboundMailServiceInstance.Server.Address
                                : "";

#if !DEBUG
                //Verifica se retornou o servidor para envio de e-mail
                if (string.IsNullOrEmpty(smtpServer))
                    throw new Exception("SMTP para envio de e-mail não configurado no servidor do Sharepoint.");

#else
            //smtpServer = "smptp@iteris.com.br";
#endif

            //Cria o objeto para envio do e-mail
            SmtpClient smtpClient = new SmtpClient(smtpServer);

            //Cria a mensagem e adiciona o anexo
            MailMessage mensagemEmail = new MailMessage(origem, destino, assunto, corpoHtml);
            mensagemEmail.IsBodyHtml = true;

            //Embute as imagens no e-mail, alterando as imagens referenciadas por URLs 
            if (embutirImagens)          
                EmbutirImagens(mensagemEmail);           

            //Envia o e-mail
            smtpClient.Send(mensagemEmail);
        }

        private static void EmbutirImagens(MailMessage mail)
        {
            String htmlContent = mail.Body;
            var cids = CSSInliner.PrepararImagensEmbutidas(ref htmlContent);            
            AlternateView altView = AlternateView.CreateAlternateViewFromString(htmlContent, null, MediaTypeNames.Text.Html);

            foreach (var cid in cids)
            {                                
                try
                {                    
                    var fPath = SPUtility.GetGenericSetupPath(@"TEMPLATE\LAYOUTS\Redecard.PN.Extrato.Sharepoint\Styles\" + cid.Key); 
                    byte[] imgData = File.ReadAllBytes(fPath);
                    MemoryStream imgStream = new MemoryStream(imgData);
                    LinkedResource imgResource = new LinkedResource(imgStream, MediaTypeNames.Image.Jpeg);
                    imgResource.ContentId = cid.Key.Replace(' ', '_');
                    altView.LinkedResources.Add(imgResource);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro durante leitura de imagem para envio de e-mail: " + cid.Key + ";" + cid.Value, ex);
                }
            }
            if(altView.LinkedResources.Count != 0)
                mail.AlternateViews.Add(altView);
        }

        #endregion

        #region [ Geração/Renderização HTML ]

        public static String ObterHTMLControle(Control control)
        {            
            StringWriter stringWrite = new StringWriter();
            HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            control.RenderControl(htmlWrite);

            return stringWrite.ToString();
        }
        
        private static string HtmlAppRelativeUrlsToAbsoluteUrls(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            const string htmlPattern = "(?<attrib>\\shref|\\ssrc|\\sbackground)\\s*?=\\s*?"
                                        + "(?<delim1>[\"'\\\\]{0,2})(?!#|http|ftp|mailto|javascript)"
                                        + "/(?<url>[^\"'>\\\\]+)(?<delim2>[\"'\\\\]{0,2})";

            var htmlRegex = new Regex(htmlPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            html = htmlRegex.Replace(html,
                m => htmlRegex.Replace(m.Value, "${attrib}=${delim1}" + ToAbsoluteUrl("~/" + m.Groups["url"].Value) + "${delim2}"));

            const string cssPattern = "@import\\s+?(url)*['\"(]{1,2}"
                                        + "(?!http)\\s*/(?<url>[^\"')]+)['\")]{1,2}";

            var cssRegex = new Regex(cssPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            html = cssRegex.Replace(html, m => cssRegex.Replace(m.Value, "@import url(" + ToAbsoluteUrl("~/" + m.Groups["url"].Value) + ")"));

            return html;
        }

        private static string ToAbsoluteUrl(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return relativeUrl;

            if (HttpContext.Current == null)
                return relativeUrl;

            if (relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Insert(0, "~");
            if (!relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.Insert(0, "~/");

            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

            return string.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
        }
    
        #endregion        

        /// <summary>
        /// Method that limits the length of text to a defined length.
        /// </summary>
        /// <param name="source">The source text.</param>
        /// <param name="maxLength">The maximum limit of the string to return.</param>
        public static String LimitLength(String source, Int32 maxLength)
        {
            if (source.Length <= maxLength)
                return source;

            return source.Substring(0, maxLength);
        }

        public static Control FindControlRecursive(Control root, String id)
        {
            if (root.ID == id) return root;
            foreach (Control c in root.Controls)
            {
                Control t = FindControlRecursive(c, id);
                if (t != null) return t;
            }
            return null;
        }
    }

    public static class ExtensionMethods
    {
        public static String GetLast(this String source, Int32 tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }

        /// <summary>
        /// DateTime?.ToString
        /// </summary>
        /// <param name="date">Data</param>
        /// <param name="defaultValue">Valor padrão</param>
        /// <param name="format">Formatação</param>
        public static String ToString(this DateTime? date, String format, String defaultValue)
        {
            if (date.HasValue)
                return date.Value.ToString(format);
            else
                return defaultValue;
        }

        /// <summary>
        /// DateTime?.ToString
        /// </summary>
        /// <param name="date">Data</param>
        /// <param name="format">Formatação</param>
        public static String ToString(this DateTime? date, String format)
        {
            return date.ToString(format, DateTime.MinValue);
        }

        /// <summary>
        /// DateTime?.ToString
        /// </summary>
        /// <param name="date">Data</param>
        /// <param name="defaultValue">Valor padrão</param>
        /// <param name="format">Formatação</param>
        public static String ToString(this DateTime? date, String format, DateTime defaultValue)
        {
            if (date.HasValue)
                return date.Value.ToString(format);
            else
                return defaultValue.ToString(format);
        }
    }
}
