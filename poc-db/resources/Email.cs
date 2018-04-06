/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.WebParts.AtendimentoDigital.FaleConosco
{
    /// <summary>
    /// Classe estática para centralização do envio de e-mail
    /// </summary>
    internal class Email
    {
        #region [ Propriedades ]

        /// <summary>
        /// Endereço de e-mail do remetente
        /// </summary>
        public String From { get; set; }

        /// <summary>
        /// Endereço de e-mail do destinatário
        /// </summary>
        public String To { get; set; }

        /// <summary>
        /// Endereços de e-mail de cópia
        /// </summary>
        public String Cc { get; set; }

        /// <summary>
        /// Endereços de e-mail de cópia oculta
        /// </summary>
        public String Bcc { get; set; }

        /// <summary>
        /// Assunto do e-mail
        /// </summary>
        public String Subject { get; set; }

        /// <summary>
        /// Conteúdo do E-mail
        /// </summary>
        public String BodyContent { get; set; }

        /// <summary>
        /// Flag indicando se conteúdo do e-mail é HTML
        /// </summary>
        public Boolean IsBodyHtml { get; set; }

        #endregion

        #region [ Construtores ]

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="from">Endereço de e-mail do remetente.</param>
        /// <param name="to">Endereço de e-mail do destinatário.</param>
        /// <param name="subject">Assunto do e-mail</param>
        /// <param name="bodyContent">Conteúdo do e-mail</param>
        /// <param name="isBodyHtml">Se conteúdo do e-mail é HTML</param>
        /// <param name="cc">Endereço de e-mail de cópia</param>
        /// <param name="bcc">Endereço de e-mail de cópia oculta</param>
        public Email(String from,
                     String to,
                     String subject,
                     String bodyContent,
                     Boolean isBodyHtml = true,
                     String cc = null,
                     String bcc = null)
        {
            this.From = from;
            this.To = to;
            this.Subject = subject;
            this.BodyContent = bodyContent;
            this.IsBodyHtml = isBodyHtml;
            this.Cc = cc;
            this.Bcc = bcc;
        }

        #endregion

        #region [ Métodos Públicos ]

        /// <summary>
        /// Envia o e-mail.
        /// </summary>
        /// <param name="context">SPContext</param>
        /// <returns>Se enviou o e-mail com sucesso</returns>
        public Boolean EnviarEmail(SPWeb context = null)
        {
            //Tentativa 1: utilizando o próprio SPUtility
            Boolean envioSucesso = EnviarEmailSharePoint(context);

            //Tentaitva 2: Em caso de erro, tenta enviar diretamente pelo SmtpClient
            if(!envioSucesso)
                envioSucesso = EnviarEmailSmtpClient();

            return envioSucesso;
        }

        /// <summary>
        /// Retorna se um determinado endereço de email é válido
        /// </summary>
        /// <param name="email">Endereço de email a validar</param>
        /// <returns>E-mail válido</returns>
        public static Boolean EnderecoEmailValido(String email)
        {
            return Regex.IsMatch(email, ExpressaoRegularEmail);
        }

        #endregion

        #region [ Método Privados ]

        /// <summary>
        /// Retorna o servidor SMTP a partir das configurações do SharePoint
        /// </summary>
        /// <returns>Servidor SMTP</returns>
        private String GetSmtpServer()
        {
            String smtpServer = SPAdministrationWebApplication.Local.OutboundMailServiceInstance != null ?
                                SPAdministrationWebApplication.Local.OutboundMailServiceInstance.Server.Address
                                : "";

#if !DEBUG
            //Verifica se retornou o servidor para envio de e-mail
            if (string.IsNullOrEmpty(smtpServer))
                throw new Exception("SMTP para envio de e-mail não configurado no servidor do Sharepoint.");
#else
            //smtpServer = "smptpServer";
#endif
            return smtpServer;
        }

        /// <summary>
        /// Envia e-mail através da classe SmtpClient, utilizando o servidor SMTP configurado no SharePoint
        /// </summary>
        /// <returns>Se mensagem foi enviada com sucesso</returns>
        private Boolean EnviarEmailSmtpClient()
        {
            Boolean sucesso = default(Boolean);

            try
            {
                //Cria o objeto para envio do e-mail
                String smtpServer = GetSmtpServer();
                SmtpClient smtpClient = new SmtpClient(smtpServer);

                //Cria a mensagem e adiciona o anexo
                MailMessage mensagemEmail = new MailMessage(From, To, Subject, BodyContent);
                mensagemEmail.IsBodyHtml = IsBodyHtml;

                //Envia o e-mail
                smtpClient.Send(mensagemEmail);

                sucesso = true;
            }
            catch (SmtpException ex)
            {
                Logger.GravarErro("Erro durante envio de e-mail via SmtpClient: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante envio de e-mail via SmtpClient: " + ex.Message, ex);
            }

            return sucesso;
        }

        /// <summary>
        /// Envia e-mail utilizando a classe SPUtility
        /// </summary>
        /// <param name="context">SPContext</param>
        /// <returns>Se enviou a mensagem com sucesso</returns>
        private Boolean EnviarEmailSharePoint(SPWeb context = null)
        {
            if (context == null)
                context = SPContext.Current.Web;

            if (context.CurrentUser != null)
            {
                StringDictionary headers = new StringDictionary();
                headers.Add("to", To);
                headers.Add("subject", Subject);

                if (!String.IsNullOrEmpty(Cc))
                    headers.Add("cc", Cc);

                if (!String.IsNullOrEmpty(Bcc))
                    headers.Add("bcc", Bcc);

                if (IsBodyHtml)
                    headers.Add("content-type", "text-html");

                return SPUtility.SendEmail(
                    web: context,
                    messageHeaders: headers,
                    messageBody: BodyContent);
            }
            else
                return false;
        }

        /// <summary>
        /// Regular expression para fins de validação de endereços de email
        /// </summary>
        private static String ExpressaoRegularEmail
        {
            get
            {
                return @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            }
        }

        #endregion
    }
}