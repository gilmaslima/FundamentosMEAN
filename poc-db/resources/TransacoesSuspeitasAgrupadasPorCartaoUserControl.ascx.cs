/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 28/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;
using Redecard.PN.Comum;
using Redecard.PN.FMS.Sharepoint.Interfaces;
using Redecard.PN.FMS.Sharepoint.Helpers;
using System.Collections.Generic;
using Redecard.PN.FMS.Sharepoint.Exceptions;
using Microsoft.SharePoint.Utilities;

namespace Redecard.PN.FMS.Sharepoint.WebParts.TransacoesSuspeitasAgrupadasPorCartao
{
    /// <summary>
    /// Publica o serviço 'Transações Suspeitas Agrupadas por Cartão' para chamada aos serviços do webservice referentes ao FMS - PN.
    /// </summary>
    public partial class TransacoesSuspeitasAgrupadasPorCartaoUserControl : RelatorioGridBaseUserControl<CriterioOrdemTransacoesAgrupadasPorCartao>, IPossuiExportacao
    {
        #region Eventos
        /// <summary>
        /// Evento que irá ocorrer ao carregar a página.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                base.MontaGridInicial();
            }
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

            AgrupamentoTransacoesCartao transacao = e.Row.DataItem as AgrupamentoTransacoesCartao;

            e.Row.Cells[8].Text = base.ObterDescricaoPorIndicadorTipoCartao(transacao.TipoCartao);

            e.Row.Cells[e.Row.Cells.Count - 1].CssClass += " last";
        }

        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão link.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LinkButton_Click(object sender, EventArgs e)
        {
            decimal numeroCartao;

            string commandArgument = (sender as LinkButton).CommandArgument;

            if (!decimal.TryParse(commandArgument, out numeroCartao))
            {
                if (string.IsNullOrEmpty(commandArgument))
                {
                    base.ExibirMensagem("Informe o número do cartão.");
                    return;
                }
                else
                {
                    base.ExibirMensagem("Número do cartão inválido.");
                    return;
                }
            }

            Redecard.PN.Comum.SharePointUlsLog.LogMensagem(string.Format("[Transações suspeitas agrupadas por cartão] - numeroCartao [{0}]", numeroCartao));

            try
            {
                PesquisarTransacoesPorNumeroCartaoEEstabelecimentoEnvio envio = new PesquisarTransacoesPorNumeroCartaoEEstabelecimentoEnvio()
                {
                    NumeroCartao = numeroCartao.ToString(),
                    NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                    GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                    UsuarioLogin = GetSessaoAtual.LoginUsuario,
                    TempoBloqueio = base.ParametrosSistema.TempoExpiracaoBloqueio,
                    TipoCartao = base.Tipocartao,
                    NumeroEstabelecimento = 0
                    
                };

                using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
                {
                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Transações suspeitas agrupadas por cartão] - início da execução do serviço 'PesquisarTransacoesPorNumeroCartaoEEstabelecimento'.");

                    Servico.FMS.PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno retorno = client.PesquisarTransacoesPorNumeroCartaoEEstabelecimento(envio);

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Transações suspeitas agrupadas por cartão] - fim da execução do serviço 'PesquisarTransacoesPorNumeroCartaoEEstabelecimento'.");

                    ValidarRetornoPesquisaPorCartao(retorno);

                    if (retorno.ListaTransacoesEmissor == null || retorno.ListaTransacoesEmissor.Length == 0)
                    {
                        base.ExibirMensagem("Não existem transações para esta consulta.");
                        return;
                    }
                    
                    QueryStringSegura query = new QueryStringSegura();
                    query.Add("identificadorTransacao", retorno.ListaTransacoesEmissor[0].IdentificadorTransacao.ToString());
                    Session["FMS_transacoes"] = new List<Servico.FMS.TransacaoEmissor>(retorno.ListaTransacoesEmissor);

                    SPUtility.Redirect(String.Format("pn_AnalisaTransacoesSuspeitasPorCartao.aspx?dados={0}", query.ToString()), SPRedirectFlags.Default, Context);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão atualizar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                using (FMSClient client = new FMSClient())
                {
                    client.DescartarSessaoPesquisaTransacoesSuspeitasPorCartao(GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade, GetSessaoAtual.LoginUsuario);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }

            base.MontaGridInicial();
        }
        #endregion

        /// <summary>
        /// Este método é utilizado para obter o critério ordem inicial.
        /// </summary>
        /// <returns></returns>
        protected override CriterioOrdemTransacoesAgrupadasPorCartao ObterCriterioOrdemInicial()
        {
            return CriterioOrdemTransacoesAgrupadasPorCartao.Cartao;
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
        protected override long MontaGrid(FMSClient objClient, CriterioOrdemTransacoesAgrupadasPorCartao criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            TransacaoPorCartaoEnvio envio = GetObjectEnvioTransacao(criterioOrdem, ordemClassificacao, primeiroRegistro, quantidadeRegistroPagina);

            RespostaTransacoesPorCartao retorno = objClient.PesquisarTransacoesPorCartao(envio);
            
            grvDados.DataSource = retorno.ListaTransacoes;
            grvDados.DataBind();

            if (base.PaginacaoControl != null)
            {
                int totalRegistrosDe = base.PaginacaoControl.PaginaAtual * base.PaginacaoControl.RegistrosPorPagina;
                int totalRegistrosAte = Convert.ToInt32(retorno.QuantidadeRegistros);

                if (totalRegistrosDe > totalRegistrosAte)
                {
                    totalRegistrosDe = totalRegistrosAte;
                }

                lblTotalRegistrosDe.Text = totalRegistrosDe.ToString();
                lblTotalRegistrosAte.Text = totalRegistrosAte.ToString();
            }
            else
            {
                lblTotalRegistrosDe.Text = retorno.QuantidadeRegistros.ToString();
                lblTotalRegistrosAte.Text = retorno.QuantidadeRegistros.ToString();
            }

            return retorno.QuantidadeRegistros;
        }

        /// <summary>
        /// Envia dados da transação.
        /// </summary>
        /// <param name="criterioOrdem"></param>
        /// <param name="ordemClassificacao"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeRegistroPagina"></param>
        /// <returns></returns>
        private TransacaoPorCartaoEnvio GetObjectEnvioTransacao(CriterioOrdemTransacoesAgrupadasPorCartao criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            TransacaoPorCartaoEnvio envio = new TransacaoPorCartaoEnvio()
            {
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                ModoClassificacao = criterioOrdem,
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                Ordem = ordemClassificacao,
                PrimeiroRegistro = primeiroRegistro,
                QuantidadeMaximaRegistros = quantidadeRegistroPagina,
                TipoTransacao = base.Tipocartao,
                UsuarioLogin = GetSessaoAtual.LoginUsuario
            };

            return envio;
        }

        /// <summary>
        /// Este método é utilizado para posicionar a paginação.
        /// </summary>
        /// <returns></returns>
        protected override Control GetControleOndeColocarPaginacao()
        {
            return divPaginacao;
        }

        /// <summary>
        /// Este método é utilizado para saber se os parãmetros sistema estão carregados.
        /// </summary>
        /// <returns></returns>
        protected override bool CarregarParametrosSistema()
        {
            return true;
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                TransacaoPorCartaoEnvio envio = GetObjectEnvioTransacao(ObterCriterioOrdemInicial(), Servico.FMS.OrdemClassificacao.Ascendente, Constantes.PosicaoInicialPrimeiroRegistro, Constantes.QtdMaximaRegistros);

                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    RespostaTransacoesPorCartao resp = objClient.ExportarTransacoesAgrupadasPorCartao(envio);

                    AgrupamentoTransacoesCartao[] lista = resp.ListaTransacoes;

                    if (resp.ListaTransacoes.Length == 0)
                    {
                        base.ExibirMensagem("Não foram encontrados dados para a exportação.");
                        return;
                    }

                    string[] campos = new string[] { "Número do cartão", "Data/hora da transação suspeita", "Score", "Valor das transações suspeitas", "Vlr transações suspeitas APROVADAS", "Qtde transações suspeitas APROVADAS", "Vlr transações suspeitas NEGADAS", "Qtde transações suspeitas NEGADAS", "C/D" };

                    ExportadorHelper.GerarExcel(ExportadorHelper.ObterTransacoesSuspeitasAgrupadasPorCartaoRelatorio(lista), Response, campos);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        public void Exportar()
        {
            try
            {
                TransacaoPorCartaoEnvio envio = GetObjectEnvioTransacao(ObterCriterioOrdemInicial(), Servico.FMS.OrdemClassificacao.Ascendente, Constantes.PosicaoInicialPrimeiroRegistro, Constantes.QtdMaximaRegistros);

                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    RespostaTransacoesPorCartao resp = objClient.ExportarTransacoesAgrupadasPorCartao(envio);

                    AgrupamentoTransacoesCartao[] lista = resp.ListaTransacoes;

                    if (resp.ListaTransacoes.Length == 0)
                    {
                        base.ExibirMensagem("Não foram encontrados dados para a exportação.");
                        return;
                    }

                    string[] campos = new string[] { "Número do cartão", "Data/hora da transação suspeita", "Score", "Valor das transações suspeitas", "Vlr transações suspeitas APROVADAS", "Qtde transações suspeitas APROVADAS", "Vlr transações suspeitas NEGADAS", "Qtde transações suspeitas NEGADAS", "C/D" };

                    ExportadorHelper.GerarExcel(ExportadorHelper.ObterTransacoesSuspeitasAgrupadasPorCartaoRelatorio(lista), Response, campos);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

    }
}
