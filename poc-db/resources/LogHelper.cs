using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using El = Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Redecard.PN.FMS.Comum
{
    
    /// <summary>
    /// Classe que efetua a gravacao de mensagens de log.
    /// </summary>
    public class LogHelper
    {
        private const string _separadorCampo = "|";
        private const string _separadorCampoClasse = ";";
        private const string _quebraLinha = "\n";
        private const string _qualificadorTexto = @"""";
        private const string _regexIdentificaCartao16 = @"(?<inicio>^*)\b*(?<bin>\d{6})(?:\d{6})(?<final>\d{4})\b";
        private const string _regexIdentificaCartao19 = @"(?<inicio>^*)\b*(?<bin>\d{6})(?:\d{9})(?<final>\d{4})\b";
        private const string _regexMascaraCartao16 = @"$1$2XXXXXX$3";
        private const string _regexMascaraCartao19 = @"$1$2XXXXXXXXX$3";

        /// <summary>
        /// Método que efetua a gravação no log, fazendo a interface com a Enterprise Library.
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="categoria"></param>
        private static void GravarLog(string mensagem, string categoria)
        {
            string mensagemLog;

            mensagemLog = Regex.Replace(mensagem, _regexIdentificaCartao19, _regexMascaraCartao19, RegexOptions.IgnoreCase);
            mensagemLog = Regex.Replace(mensagem, _regexIdentificaCartao16, _regexMascaraCartao16, RegexOptions.IgnoreCase);

            El.Logger.Write(mensagemLog, categoria);
        }

        /// <summary>
        /// Efetua gravação do log
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="categoria"></param>
        /// <param name="valores"></param>
        private static void GravarLog(string mensagem, string categoria, IDictionary<string, object> valores)
        {
            string mensagemLog;

            mensagemLog = Regex.Replace(mensagem, _regexIdentificaCartao19, _regexMascaraCartao19, RegexOptions.IgnoreCase);
            mensagemLog = Regex.Replace(mensagem, _regexIdentificaCartao16, _regexMascaraCartao16, RegexOptions.IgnoreCase);

            El.Logger.Write(mensagemLog, categoria, valores);
        }


        /// <summary>
        /// Efetua a gravação do log de erro
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="exception"></param>
        public static void GravarErrorLog(Exception exception)
        {
            GravarLog(exception.Message + _separadorCampo + "StackTrace: " + exception.StackTrace, "Exception");
        }
        /// <summary>
        /// Efetua a gravação do log de erro
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void GravarErrorLog(string message, Exception exception)
        {
            GravarLog(message + _separadorCampo + exception.Message + _separadorCampo + " StackTrace: " + exception.StackTrace, "Exception");
        }

        /// <summary>
        /// Efetua a gravação do log de trace
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="message"></param>
        public static void GravarTraceLog(string message)
        {
            GravarLog(message, "Trace");
        }

        /// <summary>
        /// Efetua a gravação do log de trace
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="objeto"></param>
        public static void GravarTraceLog(object objetoSerializavel)
        {
            GravarTraceLog(SerializadorHelper.SerializarDados(objetoSerializavel));
        }
        /// <summary>
        /// Efetua a gravação do log de integração.
        /// </summary>
        /// <param name="objetoSerializavel"></param>
        public static void GravarLogIntegracao(object objetoSerializavel)
        {
            GravarLog(SerializadorHelper.SerializarDados(objetoSerializavel), "Integracao");
        }
        /// <summary>
        /// Efetua a gravação do log de integração.
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="objetoSerializavel"></param>
        public static void GravarLogIntegracao(string mensagem, object objetoSerializavel)
        {
            GravarLog(mensagem + _separadorCampo + SerializadorHelper.SerializarDados(objetoSerializavel), "Integracao");
        }
        /// <summary>
        /// Efetua a gravação do log de integração.
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="valoresASerializar"></param>
        public static void GravarLogIntegracao(string mensagem, IList<KeyValuePair<string, string>> valoresASerializar)
        {
            StringBuilder valoresSerializados = new StringBuilder();

            foreach (KeyValuePair<string, string> valor in valoresASerializar)
            {
                valoresSerializados.AppendFormat("{0}={1}{2}", valor.Key, valor.Value, _separadorCampoClasse);
            }

            GravarLog(mensagem + _separadorCampo + valoresSerializados, "Integracao");

        }
        /// <summary>
        /// Efetua a gravação do log de integração.
        /// </summary>
        /// <param name="mensagem"></param>
        public static void GravarLogIntegracao(string mensagem)
        {
            GravarLog(mensagem, "Integracao");
        }

    }
}

