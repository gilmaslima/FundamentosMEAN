#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [15/05/2012] – [André Rentes] – [Criação]
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint.Administration;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe auxiliar para Log de Erros e Mensagens.
    /// </summary>
    public class SharePointUlsLog : SPDiagnosticsServiceBase
    {
        /// <summary>
        /// Constante de log de informação
        /// </summary>
        /// <value>Redecard PN Info</value>
        private const String REDECARD_INFO = "Redecard PN Info";
        
        /// <summary>
        /// Constante de log de erro
        /// </summary>
        /// <value>Redecard PN Error</value>
        private const String REDECARD_ERROR = "Redecard PN Error";
        
        /// <summary>
        /// Constante do nome da área do log
        /// </summary>
        /// <value>Redecard PN</value>
        private static String REDECARD_AREA = "Redecard PN";

        /// <summary>
        /// Utiliza o construtor base para passar o nome do Log no padrão Redecard
        /// </summary>
        private SharePointUlsLog() : base("Serviço de Log Redecard PN", SPFarm.Local) { }

        /// <summary>
        /// Criação das área e categorias do log
        /// </summary>
        /// <returns>Áreas e categorias do log</returns>
        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>{
                new SPDiagnosticsArea(REDECARD_AREA, new List<SPDiagnosticsCategory>{
                  new SPDiagnosticsCategory(REDECARD_INFO, TraceSeverity.Verbose, EventSeverity.Information),
                  new SPDiagnosticsCategory(REDECARD_ERROR, TraceSeverity.Unexpected, EventSeverity.Warning),
                })
              };

            return areas;
        }

        /// <summary>
        /// Membro da classe atual
        /// </summary>
        private static SharePointUlsLog _atual;

        /// <summary>
        /// Propriedade da classe atual estática
        /// </summary>
        private static SharePointUlsLog Atual
        {
            get
            {
                if (_atual == null)
                    _atual = new SharePointUlsLog();
                return _atual;
            }
        }

        /// <summary>
        /// Método para realizar o log de mensagens
        /// </summary>
        /// <param name="mensagem">Mensagem informativa</param>
        public static void LogMensagem(String mensagem)
        {
            SPDiagnosticsCategory categoria =
            SharePointUlsLog.Atual.Areas[REDECARD_AREA].Categories[REDECARD_INFO];
            SharePointUlsLog.Atual.WriteTrace(0, categoria, TraceSeverity.Verbose, mensagem);
        }

        /// <summary>
        /// Método para realizar o log de erro
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        public static void LogErro(String mensagem)
        {
            SPDiagnosticsCategory categoria =
            SharePointUlsLog.Atual.Areas[REDECARD_AREA].Categories[REDECARD_ERROR];
            SharePointUlsLog.Atual.WriteTrace(0, categoria, TraceSeverity.Unexpected, mensagem);
        }

        /// <summary>
        /// Método para realizar o log de erro
        /// </summary>
        /// <param name="mensagem">Exceção padrão</param>
        public static void LogErro(Exception ex)
        {
            SPDiagnosticsCategory categoria =
            SharePointUlsLog.Atual.Areas[REDECARD_AREA].Categories[REDECARD_ERROR];
            
            SharePointUlsLog.Atual.WriteTrace(0, categoria, TraceSeverity.Unexpected, ex.Source);
            SharePointUlsLog.Atual.WriteTrace(0, categoria, TraceSeverity.Unexpected, ex.Message);
            SharePointUlsLog.Atual.WriteTrace(0, categoria, TraceSeverity.Unexpected, ex.StackTrace);
        }

        /// <summary>
        /// Método para realizar o log de erro
        /// </summary>
        /// <param name="mensagem">Exceção PortalRedecardException</param>
        public static void LogErro(PortalRedecardException ex)
        {
            SPDiagnosticsCategory categoria =
            SharePointUlsLog.Atual.Areas[REDECARD_AREA].Categories[REDECARD_ERROR];

            SharePointUlsLog.Atual.WriteTrace(0, categoria, TraceSeverity.Unexpected, ex.GetBaseException().Source);
            SharePointUlsLog.Atual.WriteTrace(0, categoria, TraceSeverity.Unexpected, ex.GetBaseException().Message);
            SharePointUlsLog.Atual.WriteTrace(0, categoria, TraceSeverity.Unexpected, ex.GetBaseException().StackTrace);
        }
    }
}
