/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Warnings.Datasource;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Warnings.Model;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Warnings.Model.Content;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Warnings
{
    /// <summary>
    /// Implementa classe WarningsAtendimentoPortal
    /// </summary>
    public class WarningsAtendimentoPortal
    {
        #region [ Propriedades ]
        /// <summary>
        /// Contexto SPWeb
        /// </summary>
        public SPWeb Web { get; set; }
        /// <summary>
        /// Sessão do usuário atual
        /// </summary>
        public Sessao Sessao { get; set; }
        /// <summary>
        /// Lista de warnings.
        /// </summary>
        public List<WarningsAtendimentoItem> ListaWarnings { get; set; }
        #endregion [ Propriedades ]

        #region [ Construtor ]
        public WarningsAtendimentoPortal(SPWeb web, Sessao sessao)
        {
            this.Web = web;
            this.Sessao = sessao;

            Initialize();
        }

        private void Initialize()
        {
            this.ListaWarnings = new WarningsAtendimentoDatasource(this.Web).GetItems().ToList<WarningsAtendimentoItem>();
        }

        #endregion [ Construtor ]

        #region [ Métodos Públicos ]
        /// <summary>
        /// Lista Warnings
        /// </summary>
        /// <returns></returns>
        public List<WarningsAtendimentoItem> ListarWarnings(DateTime dataAtual)
        {
            using (var log = Logger.IniciarLog("Warning - Handler - Obtendo warnings do pv"))
            {
                try
                {
                    //Parâmetros de consulta.
                    Int32 pv = Sessao.CodigoEntidade;
                    Char codigoSegmento = Sessao.CodigoSegmento;

                    return (from warning in ListaWarnings
                            where (warning.Segmentos.Contains(codigoSegmento.ToString()) || warning.Segmentos.Count() == 0) &&
                            (warning.NumerosPV.Contains(pv.ToString()) || warning.NumerosPV.Count() == 0) &&
                            (warning.DataInicioExibicao.Date <= DateTime.Now.Date && warning.DataFimExibicao.Date >= DateTime.Now.Date)
                            orderby warning.DataInicioExibicao
                            select warning).ToList();

                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    return new List<WarningsAtendimentoItem>();
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    return new List<WarningsAtendimentoItem>();
                }
            }

        }
        #endregion [ Métodos Públicos ]
    }
}
