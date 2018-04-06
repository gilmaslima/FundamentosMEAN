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
using Microsoft.SharePoint.Utilities;
using Redecard.PN.FMS.Sharepoint.Exceptions;
using System.Collections.Generic;

namespace Redecard.PN.FMS.Sharepoint.WebParts.TransacoesSuspeitasEstabelecimentoAgrupadasPorCartao
{
    /// <summary>
    /// Publica o serviço 'Transacões Suspeitas de Estabelecimento agrupadas por Cartao' para chamada aos serviços do webservice referentes ao FMS - PN.
    /// </summary>
    public partial class TransacoesSuspeitasEstabelecimentoAgrupadasPorCartaoUserControl : RelatorioGridBaseUserControl<CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao>, IPossuiExportacao
    {
        private long NumeroEstabelecimento
        {
            get
            {
                return (long)ViewState["NumeroEstabelecimento"];
            }
            set
            {
                ViewState["NumeroEstabelecimento"] = value;
            }
        }



        /// <summary>
        /// Evento que irá ocorrer ao carregar a página.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            ObterDadosRequest();

            if (Request.QueryString["release"] != null)
            {
                DesbloquearAnaliseEstabelecimento();
                return;
            }

            RegistrarJSAbandono();

            if (!IsPostBack)
            {
                RespostaTransacoesEstabelecimentoPorCartao cartoesEstabelecimento;
                
                if (Session["FMS_cartoesEstabelecimento"] != null)
                {   
                    cartoesEstabelecimento = (RespostaTransacoesEstabelecimentoPorCartao)Session["FMS_cartoesEstabelecimento"];
                    this.Session.Remove("FMS_cartoesEstabelecimento");
                    CarregarGrid(cartoesEstabelecimento);

                    PaginacaoControl.PaginaAtual = 1;
                    PaginacaoControl.QuantidadeTotalRegistros = cartoesEstabelecimento.QuantidadeRegistros;

                    if (base.PaginacaoControl != null)
                    {
                        int totalRegistrosDe = base.PaginacaoControl.PaginaAtual * base.PaginacaoControl.RegistrosPorPagina;
                        int totalRegistrosAte = Convert.ToInt32(cartoesEstabelecimento.QuantidadeRegistros);

                        if (totalRegistrosDe > totalRegistrosAte)
                        {
                            totalRegistrosDe = totalRegistrosAte;
                        }
                    }
                }
                else
                {
                    base.MontaGridInicial();
                }
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

            AgrupamentoTransacoesEstabelecimentoCartao transacao = (AgrupamentoTransacoesEstabelecimentoCartao)e.Row.DataItem;

            //foreach (TableCell c in e.Row.Cells)
            //{
            //    LinkButton linkButton = new LinkButton();
            //    linkButton.Click += new EventHandler(linkButton_Click);
            //    linkButton.CommandArgument = transacao.NumeroCartao;
            //    linkButton.Text = c.Text;
            //    c.Controls.Add(linkButton);
            //}

            e.Row.Cells[e.Row.Cells.Count - 1].CssClass += " last";
        }
        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão de link.
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

            Redecard.PN.Comum.SharePointUlsLog.LogMensagem(string.Format("[Transações suspeitas agrupadas por estabelecimento / cartão] - numeroCartao [{0}]", numeroCartao));

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
                    NumeroEstabelecimento = NumeroEstabelecimento
                };

                using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
                {
                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Transações suspeitas agrupadas por estabelecimento / cartão] - início da execução do serviço 'PesquisarTransacoesPorNumeroCartao'.");

                    Servico.FMS.PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno retorno = client.PesquisarTransacoesPorNumeroCartaoEEstabelecimento(envio);

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Transações suspeitas agrupadas por estabelecimento / cartão] - fim da execução do serviço 'PesquisarTransacoesPorNumeroCartao'.");

                    ValidarRetornoPesquisaPorCartao(retorno);

                    if (retorno.ListaTransacoesEmissor == null || retorno.ListaTransacoesEmissor.Length == 0)
                    {
                        base.ExibirMensagem("Não existem transações para esta consulta.");
                        return;
                    }

                    Session["FMS_transacoes"] = new List<Servico.FMS.TransacaoEmissor>(retorno.ListaTransacoesEmissor);

                    QueryStringSegura query = new QueryStringSegura();
                    query.Add("identificadorTransacao", retorno.ListaTransacoesEmissor[0].IdentificadorTransacao.ToString());
                    query.Add("numeroEstabelecimento", this.NumeroEstabelecimento.ToString());
                    
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
        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    objClient.DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao(GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade, GetSessaoAtual.LoginUsuario);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }

            base.MontaGridInicial();
        }

        /// <summary>
        /// Este método é utilizado para obter o critério ordem inicial.
        /// </summary>
        /// <returns></returns>
        protected override CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao ObterCriterioOrdemInicial()
        {
            return CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao.Cartao;
        }


        /// <summary>
        /// Este método é utilizado para montar o grid.
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        protected void CarregarGrid(RespostaTransacoesEstabelecimentoPorCartao dados)
        {

            grvDados.DataSource = dados.ListaTransacoes;
            grvDados.DataBind();
            
        }

        /// <summary>
        /// Este método é utilizado para pesquisar os dados a serem exibidos no grid.
        /// </summary>
        /// <param name="objClient"></param>
        /// <param name="criterioOrdem"></param>
        /// <param name="ordemClassificacao"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeRegistroPagina"></param>
        /// <returns></returns>
        protected override long MontaGrid(FMSClient objClient, CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            Redecard.PN.Comum.SharePointUlsLog.LogMensagem("Transações suspeitas estabelecimento agrupadas por cartão. NumeroEstabelecimento: " + this.NumeroEstabelecimento);
            Redecard.PN.Comum.SharePointUlsLog.LogMensagem("Transações suspeitas estabelecimento agrupadas por cartão. primeiroRegistro: " + primeiroRegistro);
            Redecard.PN.Comum.SharePointUlsLog.LogMensagem("Transações suspeitas estabelecimento agrupadas por cartão. quantidadeRegistroPagina: " + quantidadeRegistroPagina);

            objClient.BloquearEstabelecimento(GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade, GetSessaoAtual.LoginUsuario, NumeroEstabelecimento, ParametrosSistema.TempoExpiracaoBloqueio);

            TransacaoEstabelecimentoPorCartaoEnvio envio = GetEnvioTransacao(criterioOrdem, ordemClassificacao, primeiroRegistro, quantidadeRegistroPagina);

            RespostaTransacoesEstabelecimentoPorCartao retorno = objClient.PesquisarTransacoesEstabelecimentoAgrupadasPorCartao(envio);

            CarregarGrid(retorno);

            return retorno.QuantidadeRegistros;
        }

        /// <summary>
        /// Este método é utilizado para retorno do envi ode transação.
        /// </summary>
        /// <param name="criterioOrdem"></param>
        /// <param name="ordemClassificacao"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeRegistroPagina"></param>
        /// <returns></returns>
        private TransacaoEstabelecimentoPorCartaoEnvio GetEnvioTransacao(CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            TransacaoEstabelecimentoPorCartaoEnvio envio = new TransacaoEstabelecimentoPorCartaoEnvio()
            {
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                ModoClassificacao = criterioOrdem,
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                NumeroEstabelecimento = NumeroEstabelecimento,
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
                TransacaoEstabelecimentoPorCartaoEnvio envio = GetEnvioTransacao(ObterCriterioOrdemInicial(), Servico.FMS.OrdemClassificacao.Ascendente, Constantes.PosicaoInicialPrimeiroRegistro, Constantes.QtdMaximaRegistros);

                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    RespostaTransacoesEstabelecimentoPorCartao resp = objClient.ExportarTransacoesEstabelecimentoAgrupadasPorCartao(envio);

                    AgrupamentoTransacoesEstabelecimentoCartao[] lista = resp.ListaTransacoes;

                    if (lista.Length == 0)
                    {
                        base.ExibirMensagem("Não foram encontrados dados para a exportação.");
                        return;
                    }

                    string[] campos = new string[] { "Cartão", "Valor total das transações suspeitas", "Qtde transações suspeitas", "Vlr transações APROVADAS", "Qtde transações APROVADAS", "Vlr transações NEGADAS", "Qtde transações NEGADAS" };

                    ExportadorHelper.GerarExcel(ExportadorHelper.ObterTransacoesSuspeitasEstabelecimentoAgrupadasPorCartaoRelatorio(lista), Response, campos);
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
                TransacaoEstabelecimentoPorCartaoEnvio envio = GetEnvioTransacao(ObterCriterioOrdemInicial(), Servico.FMS.OrdemClassificacao.Ascendente, Constantes.PosicaoInicialPrimeiroRegistro, Constantes.QtdMaximaRegistros);

                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    RespostaTransacoesEstabelecimentoPorCartao resp = objClient.ExportarTransacoesEstabelecimentoAgrupadasPorCartao(envio);

                    AgrupamentoTransacoesEstabelecimentoCartao[] lista = resp.ListaTransacoes;

                    if (lista.Length == 0)
                    {
                        base.ExibirMensagem("Não foram encontrados dados para a exportação.");
                        return;
                    }

                    string[] campos = new string[] { "Cartão", "Valor total das transações suspeitas", "Qtde transações suspeitas", "Vlr transações APROVADAS", "Qtde transações APROVADAS", "Vlr transações NEGADAS", "Qtde transações NEGADAS" };

                    ExportadorHelper.GerarExcel(ExportadorHelper.ObterTransacoesSuspeitasEstabelecimentoAgrupadasPorCartaoRelatorio(lista), Response, campos);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }


        private void RegistrarJSAbandono()
        {
            QueryStringSegura query = new QueryStringSegura();
            query.Add("numeroEstabelecimento", this.NumeroEstabelecimento.ToString());

            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "abandonoPagina",
                    @"<script type=""text/javascript"">$(window).unload(function(){
                    $.ajax({
                        type: 'GET',
                        url: 'pn_TransacoesEstabelecimentoAgrupadasPorCartao.aspx?release=1&dados={0}',
                        async:false,
                        data: {}
                                });
                            });</script>".Replace("{0}", query.ToString()));

            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "removerAbandonoPagina",
                        @"<script type=""text/javascript"">function removerHandlerUnload(){
                            $(window).unbind('unload');
                            return '';
                         }</script>");
        }


        private void DesbloquearAnaliseEstabelecimento()
        {
            using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
            {
                
                client.DesbloquearEstabelecimento(SessaoAtual.CodigoEntidade, SessaoAtual.GrupoEntidade, SessaoAtual.LoginUsuario, this.NumeroEstabelecimento);
            }

        }

        private void ObterDadosRequest()
        {
            string dadosQS = Request.QueryString["dados"];

            if (!string.IsNullOrEmpty(dadosQS))
            {

                QueryStringSegura query = new QueryStringSegura(dadosQS);

                this.NumeroEstabelecimento = Int64.Parse(query["numeroEstabelecimento"]);
            }
        }

    }
}
