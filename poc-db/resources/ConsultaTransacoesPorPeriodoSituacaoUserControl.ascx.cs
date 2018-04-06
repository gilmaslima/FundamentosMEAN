using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.FMS.Sharepoint.Exceptions;
using Redecard.PN.FMS.Sharepoint.Helpers;
using Redecard.PN.FMS.Sharepoint.Interfaces;
using Redecard.PN.FMS.Sharepoint.Modelo;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;

namespace Redecard.PN.FMS.Sharepoint.WebParts.ConsultaTransacoesPorPeriodoSituacao
{
    /// <summary>
    /// Publica o serviço 'Consultar Transacõess por Periodo e Situacao' para chamada aos serviços do webservice referentes ao FMS - PN.
    /// </summary>
    public partial class ConsultaTransacoesPorPeriodoSituacaoUserControl : RelatorioGridBaseUserControl<CriterioOrdemTransacoesPorSituacaoEPeriodo>, IPossuiExportacao
    {
        #region Eventos
        /// <summary>
        /// Evento que irá ocorrer ao carregar a página.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// Evento que irá ocorrer ao gravar dados no data row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvDados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            Servico.FMS.TransacaoEmissor transacao = (Servico.FMS.TransacaoEmissor)e.Row.DataItem;

            if (!(transacao.DataAnalise.Date.CompareTo(DateTime.Today) == 0))
            {
                this.RemoverLinkButton(e.Row);
            }

            SPListaPadrao lista = base.ObterPadraoASerAplicadoPorSituacaoFraude(transacao.SituacaoTransacao);

            if (lista != null)
            {
                e.Row.Cells[0].Style.Add("font-color", lista.CorDeLetra);
                e.Row.Cells[0].Style.Add("background-color", lista.CorDeFundo);
                e.Row.Cells[0].Text = lista.Titulo;
            }

            e.Row.Cells[e.Row.Cells.Count - 1].CssClass += " last";
        }
        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão remover.
        /// </summary>
        /// <param name="row"></param>
        private void RemoverLinkButton(GridViewRow row)
        {
            for (int i = 0; i < row.Cells.Count; i++)
            {
                foreach (Control controle in row.Cells[i].Controls)
                {
                    if (controle.GetType() == typeof(LinkButton))
                    {
                        row.Cells[i].Text = (controle as LinkButton).Text;
                        row.Cells[i].Controls.Remove(controle);
                    }
                }
            }
        }
        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão buscar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            base.MontaGridInicial();
        }
        /// <summary>
        /// Evento que irá ocorrer ao clicar no link.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                string commandArgument = (sender as LinkButton).CommandArgument;

                long identificadorTransacao;

                long.TryParse(commandArgument, out identificadorTransacao);

                Redecard.PN.Comum.SharePointUlsLog.LogMensagem(string.Format("[Consultar transações por período/situação] - identificadorTransacao [{0}]", identificadorTransacao));

                Servico.FMS.PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociadaEnvio envio = new Servico.FMS.PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociadaEnvio()
                {
                    IdentificadorTransacao = identificadorTransacao,
                    NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                    GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                    UsuarioLogin = GetSessaoAtual.LoginUsuario,
                    PosicaoPrimeiroRegistro = Constantes.PosicaoInicialPrimeiroRegistro,
                    QuantidadeRegistros = Constantes.QtdMaximaRegistros,
                    RenovarContador = true,
                    TipoCartao = base.Tipocartao
                };

                using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
                {
                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Consultar transações por período/situação] - início da execução do serviço 'PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada'.");

                    // invoca o serviço findAnalysedAndNotAnalysedTransactionListByAssociatedTransaction.
                    Servico.FMS.TransacoesEmissorRetornoComQuantidade retorno = client.PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada(envio);

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Consultar transações por período/situação] - fim da execução do serviço 'PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada'.");

                    if (retorno.ListaTransacoes == null || retorno.ListaTransacoes.Length == 0)
                    {
                        base.ExibirMensagem("Não existem transações para esta consulta.");
                        return;
                    }

                    Session["FMS_identificadorTransacao"] = identificadorTransacao;
                    Session["FMS_transacoes"] = new List<Servico.FMS.TransacaoEmissor>(retorno.ListaTransacoes);
                }

                Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Consultar transações por período/situação] - redirecionando o usuário para a página 'pn_AnalisaTransacoesSuspeitasPorCartao.aspx'.");

                SPUtility.Redirect("pn_AnalisaTransacoesSuspeitasPorCartao.aspx?redirectedPage=ConsultaTransacoesPorPeriodoSituacao.aspx", SPRedirectFlags.Default, Context);
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Este método é utilizado para obter o tipo de período.
        /// </summary>
        /// <returns></returns>
        private TipoPeriodo ObterTipoPeriodo()
        {
            TipoPeriodo result = TipoPeriodo.DataTransacao;

            if (rdoDataEnvioAnalise.Checked)
            {
                result = TipoPeriodo.DataEnvioAnalise;
            }

            if (rdoDataTransacao.Checked)
            {
                result = TipoPeriodo.DataTransacao;
            }

            return result;
        }

        /// <summary>
        /// Este método é utilizado para obter a situação da análise selecionada.
        /// </summary>
        /// <returns></returns>
        private SituacaoAnalisePesquisa ObterSituacaoAnaliseSelecionada()
        {
            SituacaoAnalisePesquisa result = SituacaoAnalisePesquisa.Ambos;

            if (chkAnalisada.Checked && chkNaoAnalisada.Checked)
            {
                result = SituacaoAnalisePesquisa.Ambos;
            }
            else if (chkAnalisada.Checked)
            {
                result = SituacaoAnalisePesquisa.Analisada;
            }
            else if (chkNaoAnalisada.Checked)
            {
                result = SituacaoAnalisePesquisa.NaoAnalisada;
            }

            return result;
        }

        /// <summary>
        /// Este método é utilizado para obter o critério ordem inicial.
        /// </summary>
        /// <returns></returns>
        protected override CriterioOrdemTransacoesPorSituacaoEPeriodo ObterCriterioOrdemInicial()
        {
            return CriterioOrdemTransacoesPorSituacaoEPeriodo.Data;
        }

        /// <summary>
        /// Este método é utilizado para montar o grid.
        /// </summary>
        /// <param name="objClient"></param>
        /// <param name="criterioOrdem"></param>
        /// <param name="ordemClassificacao"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeRegistroPagina"></param>
        /// <returns></returns>
        protected override long MontaGrid(FMSClient objClient, CriterioOrdemTransacoesPorSituacaoEPeriodo criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            IntervaloData intervaloData = ValidarData(txtPeridoEspecificoDe, txtPeridoEspecificoAte);

            if (!chkAnalisada.Checked && !chkNaoAnalisada.Checked)
            {
                base.ExibirMensagem("Por favor, escolha a situação da transação.");

                return 0;
            }

            TransacoesEmissorRetornoComQuantidade retorno = BuscarTransacaoEmissor(criterioOrdem, ordemClassificacao, primeiroRegistro, quantidadeRegistroPagina, intervaloData, objClient);

            if (retorno.ListaTransacoes == null || retorno.ListaTransacoes.Length == 0)
            {
                base.ExibirMensagem("Não existem transações para esta consulta.");

                return 0;
            }

            grvDados.DataSource = retorno.ListaTransacoes;
            grvDados.DataBind();

            if (retorno.QuantidadeRegistros != -1)
                Session["FMS_quantidaderegistros"] = retorno.QuantidadeRegistros;

            return Convert.ToInt64(Session["FMS_quantidaderegistros"]) ;
        }

        /// <summary>
        /// Este método é utilizado para buscar transação por emissor.
        /// </summary>
        /// <param name="criterioOrdem"></param>
        /// <param name="ordemClassificacao"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeRegistroPagina"></param>
        /// <param name="intervaloData"></param>
        /// <param name="objClient"></param>
        /// <returns></returns>
        private TransacoesEmissorRetornoComQuantidade BuscarTransacaoEmissor(CriterioOrdemTransacoesPorSituacaoEPeriodo criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina, IntervaloData intervaloData, Servico.FMS.FMSClient objClient)
        {
            PesquisarTransacoesPorSituacaoEPeriodoEnvio envio = new PesquisarTransacoesPorSituacaoEPeriodoEnvio()
            {
                CriterioOrdem = criterioOrdem,
                DataFinal = intervaloData.dataFinal,
                DataInicial = intervaloData.dataInicial,
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                Ordem = ordemClassificacao,
                PosicaoPrimeiroRegistro = primeiroRegistro,
                QuantidadeRegistros = quantidadeRegistroPagina,
                Situacao = ObterSituacaoAnaliseSelecionada(),
                TipoCartao = base.Tipocartao,
                TipoPeriodo = ObterTipoPeriodo(),
                UsuarioLogin = GetSessaoAtual.LoginUsuario,
                RenovarContador = (primeiroRegistro == 0)
            };

            TransacoesEmissorRetornoComQuantidade retorno = objClient.PesquisarTransacoesPorSituacaoEPeriodo(envio);

            return retorno;
        }

        /// <summary>
        /// Este método é utilizado para posicionar a paginação.
        /// </summary>
        /// <returns></returns>
        private TransacaoEmissor[] BuscarTransacaoEmissorRelatorio(CriterioOrdemTransacoesPorSituacaoEPeriodo criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina, IntervaloData intervaloData, Servico.FMS.FMSClient objClient)
        {
            PesquisarTransacoesPorSituacaoEPeriodoEnvio envio = new PesquisarTransacoesPorSituacaoEPeriodoEnvio()
            {
                CriterioOrdem = criterioOrdem,
                DataFinal = intervaloData.dataFinal,
                DataInicial = intervaloData.dataInicial,
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                Ordem = ordemClassificacao,
                PosicaoPrimeiroRegistro = primeiroRegistro,
                QuantidadeRegistros = quantidadeRegistroPagina,
                Situacao = ObterSituacaoAnaliseSelecionada(),
                TipoCartao = base.Tipocartao,
                TipoPeriodo = ObterTipoPeriodo(),
                UsuarioLogin = GetSessaoAtual.LoginUsuario
            };

            TransacaoEmissor[] retorno = objClient.ExportarTransacoesPorSituacaoEPeriodo(envio);

            return retorno;
        }

        protected override Control GetControleOndeColocarPaginacao()
        {
            return divPaginacao;
        }
        #endregion

        /// <summary>
        /// Este método é utilizado para saber se os parãmetros sistema estão carregados.
        /// </summary>
        /// <returns></returns>
        protected override bool CarregarParametrosSistema()
        {
            return true;
        }

        public void Exportar()
        {
            try
            {
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    IntervaloData intervaloData = ValidarData(txtPeridoEspecificoDe, txtPeridoEspecificoAte);

                    TransacaoEmissor[] lista = BuscarTransacaoEmissorRelatorio(ObterCriterioOrdemInicial(), Servico.FMS.OrdemClassificacao.Ascendente, Constantes.PosicaoInicialPrimeiroRegistro, Constantes.QtdMaximaRegistros, intervaloData, objClient);

                    if (lista.Length == 0)
                    {
                        base.ExibirMensagem("Não foram encontrados dados para a exportação.");
                        return;
                    }

                    string[] campos = new string[] { "Tipo alarme", "Tipo resposta", "Cartão", "Data/hora da transação", "Data/hora de envio para análise", "Valor", "Score", "MCC", "UF", "C/D", "Bandeira", "Usuário", "Data/hora da análise", "Comentário" };

                    ExportadorHelper.GerarExcel(ExportadorHelper.ObterConsultaTransacoesPorPeriodoSituacaoRelatorio(lista), Response, campos);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }
    }
}
