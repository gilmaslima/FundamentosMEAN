using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Net.Mail;
using System.Text;

namespace Redecard.Portal.Helper.Web.Mails
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Classe utilitária para envio de Email via SharePoint
    /// </summary>
    public class EmailUtils
    {
        /// <summary>
        /// Construtor padrão que não deixa a classe ser instanciada
        /// </summary>
        private EmailUtils() { }

        /// <summary>
        /// Envia um email utilizando os parâmetros informados
        /// </summary>
        public static bool EnviarEmail(SPWeb context, string from, string para, string enderecosCopia, string enderecosCopiaOculta, string assunto, bool emailEmHTML, string email)
        {
            if (Object.ReferenceEquals(context, null))
                context = SPContext.Current.Web;
            if (!Object.ReferenceEquals(context.CurrentUser, null))
                return SPUtility.SendEmail(context, EmailUtils.ObterCabecalhosEmail(para, enderecosCopia, enderecosCopiaOculta, assunto, emailEmHTML), email);
            else {
                MailMessage message = new MailMessage();
                message.To.Add(para);
                if (!String.IsNullOrEmpty(enderecosCopia))
                    message.CC.Add(enderecosCopia);
                if (!String.IsNullOrEmpty(enderecosCopiaOculta))
                    message.Bcc.Add(enderecosCopiaOculta);
                message.Subject = assunto;
                message.Body = email;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                message.From = new MailAddress(from);
                SmtpClient client = new SmtpClient();
                client.Send(message);
                return true;
            }
        }

        /// <summary>
        /// Prepara e retorna o cabeçalho de email para envio
        /// </summary>
        private static StringDictionary ObterCabecalhosEmail(string para, string enderecosCopia, string enderecosCopiaOculta, string assunto, bool emailEmHTML)
        {
            StringDictionary dicHeaders = new StringDictionary();
            dicHeaders.Add("to", para);
            dicHeaders.Add("subject", assunto);
            if (!string.IsNullOrEmpty(enderecosCopia))
                dicHeaders.Add("cc", enderecosCopia);
            if (!string.IsNullOrEmpty(enderecosCopiaOculta))
                dicHeaders.Add("bcc", enderecosCopiaOculta);
            if (emailEmHTML)
                dicHeaders.Add("content-type", "text-html");

            return dicHeaders;
        }

        /// <summary>
        /// Retorna um pattern para fins de validação de endereços de email
        /// </summary>
        public static string ExpressaoRegular_Email
        {
            get
            {
                return @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            }
        }

        /// <summary>
        /// Retorna se um determinado endereço de email é válido
        /// </summary>
        /// <param name="email">Endereço de email a validar</param>
        /// <returns></returns>
        public static bool EnderecoEmailValido(string email)
        {
            return Regex.IsMatch(email, EmailUtils.ExpressaoRegular_Email);
        }
    }
}