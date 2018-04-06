using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.Eadquirencia.Sharepoint.Helper
{
    public static class EmailHelper
    {

        /// <summary>
        /// Envia e-mail utilizando a configuração SMTP do SharePoint
        /// </summary>
        /// <param name="destinatarios">E-mails dos destinatários</param>
        /// <param name="assunto">Assunto do e-mail</param>
        /// <param name="corpoHtml">Mensagem do e-mail (html)</param>
        /// <param name="msgErro">Mensagem de erro que ocorreu</param>
        /// <returns>Retorna true se enviou com sucesso</returns>
        public static Boolean EnviarEmail(String[] destinatarios, String assunto, String corpoHtml, out String msgErro)
        {
            return EnviarEmail("rede@userede.com.br", destinatarios, assunto, corpoHtml, out msgErro);
        }

        /// <summary>
        /// Envia e-mail utilizando a configuração SMTP do SharePoint
        /// </summary>
        /// <param name="remetente">E-mail do remetente</param>
        /// <param name="destinatarios">E-mails dos destinatários</param>
        /// <param name="assunto">Assunto do e-mail</param>
        /// <param name="corpoHtml">Mensagem do e-mail (html)</param>
        /// <param name="msgErro">Mensagem de erro que ocorreu</param>
        /// <returns>Retorna true se enviou com sucesso</returns>
        public static Boolean EnviarEmail(String remetente, String[] destinatarios, String assunto, String corpoHtml, out String msgErro)
        {
            Boolean retorno = false;
            using (Logger log = Logger.IniciarLog(String.Format("Enviando e-mail para : {0}", String.Join(",", destinatarios))))
            {
                try
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
                    if (string.IsNullOrEmpty(smtpServer))
                        smtpServer = "smtp@smtp.com.br";
#endif

                    //Cria o objeto para envio do e-mail
                    SmtpClient smtpClient = new SmtpClient(smtpServer);

                    //Cria a mensagem //MailMessage mensagemEmail = new MailMessage(remetente, destino, assunto, corpoHtml);
                    var mensagemEmail = new MailMessage();
                    mensagemEmail.From = new MailAddress(remetente);
                    mensagemEmail.Sender = new MailAddress(remetente);
                    mensagemEmail.To.Add(String.Join(",", destinatarios));
                    mensagemEmail.IsBodyHtml = true;
                    mensagemEmail.Body = corpoHtml;
                    mensagemEmail.Subject = assunto;

                    //Envia o e-mail
                    smtpClient.Send(mensagemEmail);

                    retorno = true;
                    msgErro = String.Empty;
                }
                catch (IntranetRedecardException ex)
                {
                    log.GravarErro(ex);
                    retorno = false;
                    msgErro = "Ocorreu um erro ao enviar o E-mail. Tente novamente.";
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    retorno = false;
                    msgErro = "Ocorreu um erro ao enviar o E-mail. Tente novamente.";
                }
            }

            return retorno;
        }
    }
}