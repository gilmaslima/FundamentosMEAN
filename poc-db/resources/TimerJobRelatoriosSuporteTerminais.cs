using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.TimerJobs
{
    public class TimerJobRelatoriosSuporteTerminais : SPJobDefinition
    {
        /// <summary>
        /// Nome do TimerJob
        /// </summary>
        public const String JobName = "Redecard PN - Dados Cadastrais - Manutenção de Relatórios de Suporte à Terminais";
                
        public TimerJobRelatoriosSuporteTerminais() : base() { }

        public TimerJobRelatoriosSuporteTerminais(SPWebApplication webApp)
            : base(JobName, webApp, null, SPJobLockType.ContentDatabase)
        {
            this.Title = JobName;
        }

        public override string  Description
        {
            get 
            {
                return String.Concat(
                    "Realiza a manutenção nos Relatórios de Suporte à Terminais ",
                    "('Conteúdos Mais Acessados', 'Retenções' e 'Troca de Terminais'), ",
                    "removendo os registros mais antigos.");
            }
        }

        protected override bool HasAdditionalUpdateAccess()
        {
            return true;
        }

        public override void Execute(Guid targetInstanceId)
        {
            using (Logger Log = Logger.IniciarLog("Limpeza dos Registros dos Relatórios de Suporte à Terminais"))
            {
                try
                {                    
                    String siteURL = String.Empty;

                    //Recupera a URL do site
                    if (!String.IsNullOrEmpty((String)this.Properties["siteURL"]))
                        siteURL = this.Properties["siteURL"].ToString();

                    //Efetura limpeza das listas dos Relatórios de Suporte à Terminais
                    if (!String.IsNullOrEmpty(siteURL))
                    {
                        using (SPSite site = new SPSite(siteURL))
                        {
                            using (SPWeb siteWeb = site.OpenWeb())
                            {
                                this.LimpezaLista(siteWeb, "Suporte à Terminais - Relatório de Conteúdos Mais Acessados");
                                this.LimpezaLista(siteWeb, "Suporte à Terminais - Relatório de Retenções");
                                this.LimpezaLista(siteWeb, "Suporte à Terminais - Relatório de Trocas de Terminal");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                }
            }
        }

        private void LimpezaLista(SPWeb spWeb, String nomeLista)
        {
            SPList spList = spWeb.Lists.TryGetList(nomeLista);
            if (spList != null)
            {
#if DEBUG
                DateTime dataCorte = DateTime.Now.Subtract(TimeSpan.FromMinutes(5));
#else
                //Registros são armazenados por 2 meses
                DateTime dataCorte = DateTime.Now.AddMonths(-2);
#endif

                SPQuery spQuery = new SPQuery();
                spQuery.Query = String.Concat(
                    "<Where>",
                        "<Leq>",
                            "<FieldRef Name=\"Created\" />",
                             "<Value Type=\"DateTime\">",
                                SPUtility.CreateISO8601DateTimeFromSystemDateTime(dataCorte),                                
                             "</Value>",                            
                        "</Leq>",
                    "</Where>");

                foreach (SPListItem spListItem in spList.GetItems(spQuery))
                    spListItem.Delete();
            }
        }
    }
}
