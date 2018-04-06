using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.Eadquirencia.Sharepoint.Helper
{
    /// <summary>
    /// Classe para auxiliar no envio do SMS
    /// </summary>
    public class SMSHelper
    {
        /// <summary>
        /// Usuario de autentificação para envio de SMS
        /// </summary>
        public static String UsuarioSms
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["usuarioSMS"]);
            }
        }

        /// <summary>
        /// Senha de autentificação para envio de SMS
        /// </summary>
        public static String SenhaSms
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["senhaSMS"]);
            }
        }

        /// <summary>
        /// Enviar SMS
        /// </summary>
        /// <param name="mensagem">Mensagem a ser enviada.</param>
        /// <param name="numeroCelular">Número do celular</param>
        /// <returns></returns>
        public static Boolean EnviarSMS(String mensagem, String numeroCelular, out String msgErro)
        {
            Boolean retorno = false;

            using (Logger log = Logger.IniciarLog(String.Format("Enviando mensagem para o numero: {0}", numeroCelular)))
            {
                try
                {
                    String retornoSMS = SMS.EnviaSMS(UsuarioSms, SenhaSms, numeroCelular, mensagem);

                    if (String.Compare(retornoSMS, "OK", true) == 0)
                    {
                        retorno = true;
                        msgErro = String.Empty;
                    }
                    else if (String.Compare(retornoSMS, "NOK2", true) == 0)
                    {
                        retorno = false;
                        msgErro = "Sua última solicitação foi feita há menos de 20 minutos. Por favor, aguarde.";
                    }
                    else
                    {
                        retorno = false;
                        msgErro = "Ocorreu um erro ao enviar o SMS. Tente novamente.";
                    }
                }
                catch (IntranetRedecardException ex)
                {
                    log.GravarErro(ex);
                    retorno = false;
                    msgErro = "Ocorreu um erro ao enviar o SMS. Tente novamente.";
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    retorno = false;
                    msgErro = "Ocorreu um erro ao enviar o SMS. Tente novamente.";
                }
            }

            return retorno;
        }
    }
}
