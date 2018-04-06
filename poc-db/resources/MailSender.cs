/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Redecard.PN.FMS.Comum.Log
{
    /// <summary>
    /// Este componente publica a classe MailSender, que expõe métodos para manipular o envio de e-mail.
    /// </summary>
    public class MailSender
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public MailSender()
        {
            
        }
        /// <summary>
        /// Este método é utilizado para enviar e-mail.
        /// </summary>
        /// <param name="remetente"></param>
        /// <param name="destinatarios"></param>
        /// <param name="assunto"></param>
        /// <param name="mensagem"></param>
        /// <param name="prioridade"></param>
        public void EnviarEmail(string remetente, string destinatarios,string assunto, string mensagem, MailPriority prioridade)
        {
            using (MailMessage email = new MailMessage())
            {
                email.From = new MailAddress(remetente);
                email.To.Add(destinatarios);
                email.Subject = assunto;
                email.Body = mensagem;
                email.Priority = prioridade;
                email.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                {
                    client.Send(email);
                }
            }
        }



    }
}
