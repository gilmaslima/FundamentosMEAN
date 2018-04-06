using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net.Mail;
using System.Configuration;

namespace Redecard.PN.DadosCadastrais.ISRobo
{
    public class EnviarEmail
    {
        private String cc = null;
        private String to = null;
        private String from = null;
        private String assunto = null;
        private String texto = null;
        private IList carbonCopy = new ArrayList();
        private String amb = null;
        private MailPriority prioridade = MailPriority.Normal;

        /// <summary>
        /// 
        /// </summary>
        public EnviarEmail()
        {
            Ambiente _amb = new Ambiente();
            amb = _amb.Sigla;
        }

        /// <summary>
        /// 
        /// </summary>
        public String CC
        {
            get { return cc; }
            set { cc = value.Trim(); }
        }

        /// <summary>
        /// Endereço de e-mail Destinatário
        /// </summary>
        public String To
        {
            get { return to; }
            set { to = value.Trim(); }
        }

        /// <summary>
        /// Endereço de e-mail Remetente
        /// </summary>
        public String From
        {
            get { return from; }
            set { from = value.Trim(); }
        }

        /// <summary>
        /// Assunto do E-mail
        /// </summary>
        public String Assunto
        {
            get { return assunto; }
            set { assunto = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Texto
        {
            get { return texto; }
            set { texto = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        public void AdiconarCC(String email)
        {
            carbonCopy.Add(email);
        }

        /// <summary>
        /// 
        /// </summary>
        public MailPriority Prioridade
        {
            get { return prioridade; }
            set { prioridade = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean Enviar()
        {
            try
            {
                Util.AddLog(String.Format("{0} - Início enviando email.\nDe: {1}\nPara: {2}\nAssunto: {3}\nBody: {4}", 
                                            DateTime.Now.ToString(), From, To, Assunto, Texto));
                
                MailMessage mail = new MailMessage(From, To, Assunto, Texto);

                mail.Subject = Assunto;

                mail.Body = Texto;

                if (CC != null)
                    mail.CC.Add(CC);

                mail.IsBodyHtml = true;

                for (int i = 0; i < carbonCopy.Count; i++)
                    mail.CC.Add(carbonCopy[i].ToString());

                if ((amb == "P") || (amb == "S"))
                {
                    SmtpClient emailClient =
                        new SmtpClient(ConfigurationManager.AppSettings["SMTPServer_" + amb]);

                    emailClient.Send(mail);

                    Util.AddLog(String.Format("{0} - Email enviado com sucesso!", DateTime.Now.ToString()));
                }
                return true;
            }
            catch (Exception ex)
            {
                Util.AddLog(String.Format("{0}\n\n{1}", ex.Message, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
                if (ex.InnerException != null)
                    Util.AddLog(String.Format("{0}\n\n{1}", ex.InnerException.Message, ex.InnerException.StackTrace, System.Diagnostics.EventLogEntryType.Error));
                return false;
            }
        }

    }
}
