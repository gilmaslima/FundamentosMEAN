using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Globalization;

namespace Redecard.PN.DadosCadastrais.ISRobo
{
    /// <summary>
    /// Monitoração de Status do Serviço
    /// </summary>
    public class Monitoracao
    {
        /// <summary>
        /// Ambiente de execução
        /// </summary>
        Ambiente ambiente = new Ambiente();

        public enum TipoStatus { 
            InicioServico,
            ParadaServico,
            ParadaProcessamento
        }

        /// <summary>
        /// Envia a notificação de status
        /// </summary>
        /// <param name="status">Status do serviço ou thread</param>
        /// <param name="mensagem">Mensagem de notificação</param>
        /// <returns></returns>
        public Boolean NotificarStatus(TipoStatus status, String mensagem)
        {
            Boolean emailEnviado = false;

            try
            {
                Boolean enviarMonitoracao = Convert.ToBoolean(ConfigurationManager.AppSettings["EnviarMonitoracao"]);
                
                String conteudoEmail = "";
                
                if (enviarMonitoracao)
                {
                    Util.AddLog(String.Format("{0} - Abrindo arquivo de email", DateTime.Now.ToString()));

                    // Lê arquivo com o conteúdo do e-mail
                    //StreamReader srEmail = File.OpenText(Path.Combine(Environment.CurrentDirectory, @"HTML\" + tipoEmail.ToString() + ".htm"));
                    String modeloEmail = String.Format("{0}\\HTML\\{1}.htm", Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Monitoracao");

                    Util.AddLog(String.Format("- Template de e-mail utilizado: {0};", modeloEmail));

                    StreamReader srEmail = File.OpenText(modeloEmail);
                    conteudoEmail = srEmail.ReadToEnd();
                    srEmail.Close();

                    conteudoEmail = conteudoEmail.Replace("##DATA##", 
                                                          DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("pt-BR")));

                    mensagem = String.Format("{0}<br><br>[{1}]", 
                                             mensagem, 
                                             String.Concat("Server: ", Environment.MachineName));

                    conteudoEmail = conteudoEmail.Replace("##MSGEMAIL##", mensagem);

                    EnviarEmail email = new EnviarEmail();
                    String emailTo = ConfigurationManager.AppSettings[String.Format("{0}_{1}", "EmailMonitoracao", ambiente.Sigla)];

                    email.To = emailTo;

                    email.Assunto = "Monitoria ISRobô - Portal Use Rede";

                    if (ConfigurationManager.AppSettings["EmailFrom_" + ambiente.Sigla] != null)
                    {
                        email.From = ConfigurationManager.AppSettings[String.Format("EmailFrom_{0}", ambiente.Sigla)];
                    }
                    else
                    {
                        email.From = "teste@teste.com.br";
                    }

                    switch (status)
                    {
                        case TipoStatus.InicioServico:
                            email.Prioridade = MailPriority.Normal;
                            break;
                        case TipoStatus.ParadaProcessamento:
                        case TipoStatus.ParadaServico:
                            email.Prioridade = MailPriority.High;
                            break;
                        default:
                            email.Prioridade = MailPriority.Normal;
                            break;
                    }
                                        
                    email.Texto = conteudoEmail;

                    Util.AddLog(String.Format("{0} - Enviando email para: {1}", DateTime.Now.ToString(), email.To));

                    emailEnviado = email.Enviar();

                }
            }
            catch (ArgumentNullException ex)
            {
                Util.AddLog(String.Format("Erro ao enviar e-mail de monitoria.\n{0}", ex.Message));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Util.AddLog(String.Format("Erro ao enviar e-mail de monitoria.\n{0}", ex.Message));
            }
            catch (ObjectDisposedException ex)
            {
                Util.AddLog(String.Format("Erro ao enviar e-mail de monitoria.\n{0}", ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                Util.AddLog(String.Format("Erro ao enviar e-mail de monitoria.\n{0}", ex.Message));
            }
            catch (SmtpException ex)
            {
                Util.AddLog(String.Format("Erro ao enviar e-mail de monitoria.\n{0}", ex.Message));
            }
            catch (Exception ex)
            {
                Util.AddLog(String.Format("Erro ao enviar e-mail de monitoria.\n{0}", ex.Message));
            }

            return emailEnviado;
        }
    }
}
