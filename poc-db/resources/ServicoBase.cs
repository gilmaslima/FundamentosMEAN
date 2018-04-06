using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe base de Serviços WCF
    /// </summary>
    public class ServicoBase
    {
        /// <summary>
        /// Constante com o código genérico de erro das classes de serviços WCF
        /// </summary>
        /// <value>600</value>
        public const Int32 CODIGO_ERRO = 600;

        /// <summary>
        /// Constante com o nome completo da classe
        /// </summary>
        /// <value>Redecard.PN.Serviços</value>
        public const String FONTE = "Redecard.PN.Serviços";

        /// <summary>
        /// Recupera Exceção completa do erro
        /// </summary>
        /// <param name="ex">Exceção PortalRedecardException</param>
        /// <returns>Trace do erro completo</returns>
        public String RecuperarExcecao(PortalRedecardException ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            sb.Append("Log: ");
            sb.Append(Trace.CorrelationManager.ActivityId);
            sb.Append("\n");
            sb.Append("Código: ");
            sb.Append(ex.Codigo);
            sb.Append("\n");
            sb.Append("Fonte: ");
            sb.Append(ex.Fonte);
            sb.Append("\n");
            sb.Append("Mensagem de erro Original: ");
            sb.Append(ex.Message);
            sb.Append("\n");
            sb.Append("Stack Trace: ");
            sb.Append(ex.StackTrace);
            sb.Append("\n");
            sb.Append("Stack Trace Base: ");
            sb.Append(ex.GetBaseException().StackTrace);

            return sb.ToString();
        }

        /// <summary>
        /// Recupera Exceção completa do erro
        /// </summary>
        /// <param name="ex">Exceção padrão</param>
        /// <returns>Trace do erro completo</returns>
        public String RecuperarExcecao(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            sb.Append("Log: ");
            sb.Append(Trace.CorrelationManager.ActivityId);
            sb.Append("\n");
            sb.Append("Mensagem de erro Original: ");
            sb.Append(ex.Message);
            sb.Append("\n");
            sb.Append("Stack Trace: ");
            sb.Append(ex.StackTrace);

            return sb.ToString();
        }

    }
}
