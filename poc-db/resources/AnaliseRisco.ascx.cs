/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoServicosServico;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.Servicos
{
    /// <summary>
    /// UserControl do Box Análise de Risco
    /// </summary>
    public partial class AnaliseRisco : BaseUserControl
    {
        #region [ Controles ]

        /// <summary>ddlTamanhoPagina control.</summary>
        protected TableSize DdlTamanhoPagina { get { return (TableSize)ddlTamanhoPagina; } }

        /// <summary>objPaginacao control.</summary>
        protected Paginacao ObjPaginacao { get { return (Paginacao)objPaginacao; } }

        #endregion

        #region [ Eventos Página ]

        /// <summary>
        /// Eventos de Página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            ObjPaginacao.RegistrosPorPagina = DdlTamanhoPagina.SelectedSize;
        }

        #endregion

        #region [ Métodos Públicos ]

        /// <summary>
        /// Prepara o box para exportação
        /// </summary>
        /// <param name="guid">Guid da pesquisa</param>
        /// <param name="dados">Parâmetros da consulta</param>
        /// <param name="tamanhoPagina">Quantidade de registros por página</param>
        public void PrepararParaExportacao(Guid guid, BuscarDados dados, Int32 tamanhoPagina)
        {
            this.GuidPesquisa(guid);
            this.GravarBuscarDados(dados);

            //Efetua pesquisa completa dos dados
            this.Consultar(guid, dados, 1, tamanhoPagina);

            //Oculta controles não necessários
            ddlTamanhoPagina.Visible = false;
        }

        /// <summary>
        /// Carrega o box com a consulta
        /// </summary>
        /// <param name="guid">Guid da pesquisa</param>
        /// <param name="dados">Parâmetros da consulta</param>
        /// <returns>Quantidade de registros encontrados</returns>
        public Int32 Carregar(Guid guid, BuscarDados dados)
        {
            this.GuidPesquisa(guid);
            this.GravarBuscarDados(dados);

            //Consulta dos dados
            return this.Consultar(guid, dados, 1, DdlTamanhoPagina.SelectedSize);
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// Evento de alteração de tamanho de página. 
        /// Recarrega o relatório com a quantidade de registros por página desejado.
        /// </summary>
        /// <param name="tamanhoPagina">Tamanho da página</param>
        protected void ddlTamanhoPagina_TamanhoPaginaChanged(Object sender, Int32 tamanhoPagina)
        {
            this.Consultar(GuidPesquisa(), ObterBuscarDados(), 1, tamanhoPagina);
        }

        /// <summary>
        /// Evento de paginação.
        /// </summary>
        /// <param name="pagina">Número da página</param>
        protected void objPaginacao_PaginacaoChanged(Int32 pagina, EventArgs e)
        {
            this.Consultar(GuidPesquisa(), ObterBuscarDados(), pagina, DdlTamanhoPagina.SelectedSize);
        }

        #endregion

        #region [ Consulta ]

        /// <summary>
        /// Consulta os registros do box.
        /// </summary>
        /// <param name="guid">Guid da pesquisa</param>
        /// <param name="dados">Parâmetros da consulta</param>
        /// <param name="pagina">Número da página</param>
        /// <param name="tamanhoPagina">Tamanho da página</param>
        /// <returns>Quantida de registros encontrados</returns>
        private Int32 Consultar(Guid guid, BuscarDados dados, Int32 pagina, Int32 tamanhoPagina)
        {
            Int32 registroInicial = (pagina - 1) * tamanhoPagina;
            var registros = default(List<WAExtratoServicosServico.AnaliseRisco>);
            var qtdTotalRegistros = default(Int32);
            var status = default(StatusRetorno);

            //Por padrão, controle fica oculto. 
            //Se consulta com sucesso e possui registros, exibe controle
            this.Visible = false;

            using (Logger log = Logger.IniciarLog("Relatório de Serviços - Análise de Risco"))
            {
                try
                {
                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_ServicosClient>())
                        registros = contexto.Cliente.ConsultarAnaliseRisco(
                            out qtdTotalRegistros, out status, guid, registroInicial, tamanhoPagina,
                            dados.DataInicial, dados.DataFinal, dados.Estabelecimentos.ToList());

                    if (RelatorioServicos.ValidaRetorno(status))
                    {
                        this.Visible = qtdTotalRegistros > 0;
                        ObjPaginacao.QuantidadeTotalRegistros = qtdTotalRegistros;
                        ObjPaginacao.PaginaAtual = pagina;
                        grvRegistros.DataSource = registros;
                        grvRegistros.DataBind();
                    }
                    else
                    {
                        this.Visible = false;
                        base.ExibirPainelExcecao(status.Fonte, status.CodigoRetorno);
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return qtdTotalRegistros;
        }

        #endregion
    }
}
