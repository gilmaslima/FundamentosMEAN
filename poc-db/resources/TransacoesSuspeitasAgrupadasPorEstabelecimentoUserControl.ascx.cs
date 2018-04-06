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
using Microsoft.SharePoint.Utilities;
using System.ServiceModel;

namespace Redecard.PN.FMS.Sharepoint.WebParts.TransacoesSuspeitasAgrupadasPorEstabelecimento
{
    /// <summary>
    /// Publica o serviço 'Transacões Suspeitas Agrupadas por Estabelecimento' para chamada aos serviços do webservice referentes ao FMS - PN.
    /// </summary>
    public partial class TransacoesSuspeitasAgrupadasPorEstabelecimentoUserControl : RelatorioGridBaseUserControl<CriterioOrdemTransacoesAgrupadasPorEstabelecimento>,IPossuiExportacao
    {
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

            AgrupamentoTransacaoEstabelecimento transacao = e.Row.DataItem as AgrupamentoTransacaoEstabelecimento;

            e.Row.Cells[8].Text = base.ObterDescricaoPorIndicadorTipoCartao(transacao.TipoCartao);

            e.Row.Cells[e.Row.Cells.Count - 1].CssClass += " last";
        }

        protected void LinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                long numeroEstabelecimento;

                string commandArgument = ((LinkButton)sender).CommandArgument;

                if (!long.TryParse(commandArgument, out numeroEstabelecimento))
                {
                    base.ExibirMensagem("Número do estabelecimento inválido.");
                    return;
                }


                Redecard.PN.Comum.SharePointUlsLog.LogMensagem(string.Format("[Análise de transações suspeitas do estabelecimento] - numeroEstabelecimento [{0}]", numeroEstabelecimento));

                using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
                {

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas do estabelecimento] - início da execução do serviço 'BloquearEstabelecimento'.");

                    client.BloquearEstabelecimento(GetSessaoAtual.CodigoEntidade,
                        GetSessaoAtual.GrupoEntidade,
                        GetSessaoAtual.LoginUsuario,
                        numeroEstabelecimento,
                        ParametrosSistema.TempoExpiracaoBloqueio);

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas do estabelecimento] - fim da execução do serviço 'BloquearEstabelecimento'.");

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas do estabelecimento] - início da execução do serviço 'PesquisarTransacoesEstabelecimentoAgrupadasPorCartao'.");

                    Servico.FMS.TransacaoEstabelecimentoPorCartaoEnvio envio = new TransacaoEstabelecimentoPorCartaoEnvio()
                    {
                        GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                        ModoClassificacao = CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao.Cartao,
                        NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                        Ordem = Servico.FMS.OrdemClassificacao.Ascendente,
                        PrimeiroRegistro = 0,
                        QuantidadeMaximaRegistros = PaginacaoControl.RegistrosPorPagina,
                        TipoTransacao = base.Tipocartao,
                        UsuarioLogin = GetSessaoAtual.LoginUsuario,
                        NumeroEstabelecimento = numeroEstabelecimento
                    };

                    Servico.FMS.RespostaTransacoesEstabelecimentoPorCartao retorno = client.PesquisarTransacoesEstabelecimentoAgrupadasPorCartao(envio);

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas do estabelecimento] - fim da execução do serviço 'PesquisarTransacoesEstabelecimentoAgrupadasPorCartao'.");

                    if (retorno.QuantidadeRegistros == 0)
                    {
                        ExibirPainelExcecao(FMS_FONTE, 304);
                        return;
                    }
                    
                    Session["FMS_cartoesEstabelecimento"] = retorno;
                }

                Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas] - redirecionando o usuário para a página 'pn_TransacoesEstabelecimentoAgrupadasPorCartao.aspx'.");

                QueryStringSegura query = new QueryStringSegura();
                query.Add("numeroEstabelecimento", numeroEstabelecimento.ToString());

                SPUtility.Redirect(String.Format("pn_TransacoesEstabelecimentoAgrupadasPorCartao.aspx?dados={0}", query.ToString()), SPRedirectFlags.Default, Context);
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
                    client.DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade, GetSessaoAtual.LoginUsuario);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }

            base.MontaGridInicial();
        }

        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão exportar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                TransacaoEstabelecimentoEnvio envio = ObterDadosEnvioTransacao(ObterCriterioOrdemInicial(), Servico.FMS.OrdemClassificacao.Ascendente, Constantes.PosicaoInicialPrimeiroRegistro, Constantes.QtdMaximaRegistros);

                using (FMSClient client = new FMSClient())
                {
                    client.DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade, GetSessaoAtual.LoginUsuario);

                    RespostaTransacoesEstabelecimento retorno = client.PesquisarTransacoesAgrupadasPorEstabelecimento(envio);

                    AgrupamentoTransacaoEstabelecimento[] lista = retorno.ListaTransacoes;

                    if (lista.Length == 0)
                    {
                        base.ExibirMensagem("Não foram encontrados dados para a exportação.");
                        return;
                    }

                    string[] campos = new string[] { "PV", "Nome fantasia", "Valor das transações suspeitas", "Qtde transações suspeitas", "Vlr transações suspeitas APROVADAS", "Qtde transações suspeitas APROVADAS", "Vlr transações suspeitas NEGADAS", "Qtde transações suspeitas NEGADAS", "C/D" };

                    ExportadorHelper.GerarExcel(ExportadorHelper.ObterTransacoesSuspeitasAgrupadasPorEstabelecimentoRelatorio(lista), Response, campos);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        /// <summary>
        /// Este método é utilizado para obter o critério ordem inicial.
        /// </summary>
        /// <returns></returns>
        protected override CriterioOrdemTransacoesAgrupadasPorEstabelecimento ObterCriterioOrdemInicial()
        {
            return CriterioOrdemTransacoesAgrupadasPorEstabelecimento.NumeroEstabelecimento;
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
        protected override long MontaGrid(FMSClient objClient, CriterioOrdemTransacoesAgrupadasPorEstabelecimento criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            TransacaoEstabelecimentoEnvio envio = ObterDadosEnvioTransacao(criterioOrdem, ordemClassificacao, primeiroRegistro, quantidadeRegistroPagina);

            RespostaTransacoesEstabelecimento retorno = objClient.PesquisarTransacoesAgrupadasPorEstabelecimento(envio);

            grvDados.DataSource = retorno.ListaTransacoes;
            grvDados.DataBind();

            if (base.PaginacaoControl != null)
            {
                int totalRegistrosDe = base.PaginacaoControl.PaginaAtual * base.PaginacaoControl.RegistrosPorPagina;
                int totalRegistrosAte = Convert.ToInt32(retorno.QuantidadeTransacoes);

                if (totalRegistrosDe > totalRegistrosAte)
                {
                    totalRegistrosDe = totalRegistrosAte;
                }

                lblTotalRegistrosDe.Text = totalRegistrosDe.ToString();
                lblTotalRegistrosAte.Text = totalRegistrosAte.ToString();
            }
            else
            {
                lblTotalRegistrosDe.Text = retorno.QuantidadeTransacoes.ToString();
                lblTotalRegistrosAte.Text = retorno.QuantidadeTransacoes.ToString();
            }

            return retorno.QuantidadeTransacoes;
        }

        /// <summary>
        /// Retorna o resultado do envi odos dados de transação.
        /// </summary>
        /// <param name="criterioOrdem"></param>
        /// <param name="ordemClassificacao"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeRegistroPagina"></param>
        /// <returns></returns>
        private TransacaoEstabelecimentoEnvio ObterDadosEnvioTransacao(CriterioOrdemTransacoesAgrupadasPorEstabelecimento criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            TransacaoEstabelecimentoEnvio envio = new TransacaoEstabelecimentoEnvio()
            {
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                ModoClassificacao = criterioOrdem,
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                NumeroEstabelecimento = GetSessaoAtual.CodigoEntidade,
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

        public void Exportar()
        {
            try
            {
                TransacaoEstabelecimentoEnvio envio = ObterDadosEnvioTransacao(ObterCriterioOrdemInicial(), Servico.FMS.OrdemClassificacao.Ascendente, Constantes.PosicaoInicialPrimeiroRegistro, Constantes.QtdMaximaRegistros);

                using (FMSClient client = new FMSClient())
                {
                    client.DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade, GetSessaoAtual.LoginUsuario);

                    RespostaTransacoesEstabelecimento retorno = client.PesquisarTransacoesAgrupadasPorEstabelecimento(envio);

                    AgrupamentoTransacaoEstabelecimento[] lista = retorno.ListaTransacoes;

                    if (lista.Length == 0)
                    {
                        base.ExibirMensagem("Não foram encontrados dados para a exportação.");
                        return;
                    }

                    string[] campos = new string[] { "PV", "Nome fantasia", "Valor das transações suspeitas", "Qtde transações suspeitas", "Vlr transações suspeitas APROVADAS", "Qtde transações suspeitas APROVADAS", "Vlr transações suspeitas NEGADAS", "Qtde transações suspeitas NEGADAS", "C/D" };

                    ExportadorHelper.GerarExcel(ExportadorHelper.ObterTransacoesSuspeitasAgrupadasPorEstabelecimentoRelatorio(lista), Response, campos);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

    }
}
