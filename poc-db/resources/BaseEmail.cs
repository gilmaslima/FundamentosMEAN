/*
© Copyright 2014 Rede S.A.
Autor : Lucas Uehara
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Bases
{
    /// <summary>
    /// Classe abstrata que constrói a base do e-mail.
    /// </summary>
    public abstract class BaseEmail
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
        public BaseEmail(String from, String to, String subject, String bodyContent, Boolean isBodyHtml = true, String cc = null, String bcc = null)
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

        #region [ Métodos Protegido ]

        /// <summary>
        /// Envia e-mail através da classe SmtpClient, utilizando o servidor SMTP configurado no SharePoint
        /// </summary>
        /// <returns>Se mensagem foi enviada com sucesso</returns>
        protected Boolean EnviarEmailSmtpClient(Dictionary<String, String> parametros = null, String templateHtml = null, List<Repository.AtendimentoDigital> repository = null)
        {
            Boolean sucesso = default(Boolean);

            try
            {
                //Cria o objeto para envio do e-mail
                String smtpServer = GetSmtpServer();
                using(SmtpClient smtpClient = new SmtpClient(smtpServer))
                {
                    //Cria a mensagem e adiciona o anexo
                    using (var mensagemEmail = new MailMessage(From, To, Subject, BodyContent))
                    {
                        mensagemEmail.IsBodyHtml = IsBodyHtml;

                        if (parametros != null & repository != null & !String.IsNullOrEmpty(templateHtml))
                        {
                            var output = String.Empty;
                            
                            if (BuildBodyContent(templateHtml, parametros, out output))
                            {
                                //Sobrescreve a propriedade
                                BodyContent = output;
                                mensagemEmail.Body = BodyContent;

                                var alternateView = AlternateView.CreateAlternateViewFromString(BodyContent, Encoding.UTF8, MediaTypeNames.Text.Html);

                                foreach (Repository.AtendimentoDigital item in repository)
                                {
                                    var stream = new MemoryStream(item.ReadyByteFile);
                                    var inline = new LinkedResource(stream, "image/png") { ContentId = item.FileName };
                                    alternateView.LinkedResources.Add(inline);
                                }

                                mensagemEmail.AlternateViews.Add(alternateView);
                            }

                        }
                        else if (!String.IsNullOrEmpty(templateHtml) && parametros != null)
                        { 
                            var output = String.Empty;
                            if (BuildBodyContent(templateHtml, parametros, out output))
                            {
                                //Sobrescreve a propriedade
                                BodyContent = output;
                                mensagemEmail.Body = BodyContent;
                            }
                        }
                        //Envia o e-mail
                        smtpClient.Send(mensagemEmail);

                        sucesso = true;
                    }

                }
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
        /// Retorna se um determinado endereço de email é válido
        /// </summary>
        /// <param name="email">Endereço de email a validar</param>
        /// <returns>E-mail válido</returns>
        protected static Boolean EnderecoEmailValido(String email)
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
        /// Regular expression para fins de validação de endereços de email
        /// </summary>
        private static String ExpressaoRegularEmail
        {
            get
            {
                return @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            }
        }

        /// <summary>
        /// Monta corpo do e-mail
        /// </summary>        
        private static Boolean BuildBodyContent(String input, Dictionary<String, String> dicionario, out String output)
        {
            foreach (String str in dicionario.Keys)
            {
                input = input.Replace(str, dicionario[str]);
            }

            output = input;

            return (!String.IsNullOrEmpty(output));
        }
        #endregion
    }
}
