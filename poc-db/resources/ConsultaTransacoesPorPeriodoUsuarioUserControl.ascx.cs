using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;
using Redecard.PN.FMS.Sharepoint.Modelo;
using Redecard.PN.FMS.Sharepoint.Interfaces;
using Redecard.PN.FMS.Sharepoint.Helpers;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.FMS.Sharepoint.Exceptions;


namespace Redecard.PN.FMS.Sharepoint.WebParts.ConsultaTransacoesPorPeriodoUsuario
{
    /// <summary>
    /// Publica o serviço 'Consultar Transacõess por Periodo e Usuário' para chamada aos serviços do webservice referentes ao FMS - PN.
    /// </summary>
    public partial class ConsultaTransacoesPorPeriodoUsuarioUserControl : RelatorioGridBaseUserControl<CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo>, IPossuiExportacao
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
                CarregarListaUsuarios();
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

                Redecard.PN.Comum.SharePointUlsLog.LogMensagem(string.Format("[Consultar transações por período/usuário] - identificadorTransacao [{0}]", identificadorTransacao));

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
                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Consultar transações por período/usuário] - início da execução do serviço 'PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada'.");

                    // invoca o serviço findAnalysedAndNotAnalysedTransactionListByAssociatedTransaction.
                    Servico.FMS.TransacoesEmissorRetornoComQuantidade retorno = client.PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada(envio);

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Consultar transações por período/usuário] - fim da execução do serviço 'PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada'.");

                    if (retorno.ListaTransacoes == null || retorno.ListaTransacoes.Length == 0)
                    {
                        base.MontaGridInicial();

                        base.ExibirMensagem("Não existem transações para esta consulta.");

                        return;
                    }

                    Session["FMS_identificadorTransacao"] = identificadorTransacao;
                    Session["FMS_transacoes"] = new List<Servico.FMS.TransacaoEmissor>(retorno.ListaTransacoes);
                }

                Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Consultar transações por período/usuário] - redirecionando o usuário para a página 'pn_AnalisaTransacoesSuspeitasPorCartao.aspx'.");

                SPUtility.Redirect("pn_AnalisaTransacoesSuspeitasPorCartao.aspx?redirectedPage=ConsultaTransacoesPorPeriodoSituacao.aspx", SPRedirectFlags.Default, Context);
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }
        #endregion

        #region Métodos
        protected override long MontaGrid(FMSClient objClient, CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            TransacoesEmissorRetornoComQuantidade retorno = BuscarTransacaoEmissor(criterioOrdem,
                                                                ordemClassificacao,
                                                                primeiroRegistro,
                                                                quantidadeRegistroPagina,
                                                                objClient);

            if (retorno.ListaTransacoes == null || retorno.ListaTransacoes.Length == 0)
            {
                base.ExibirMensagem("Não existem transações para esta consulta.");

                return 0;
            }

            grvDados.DataSource = retorno.ListaTransacoes;
            grvDados.DataBind();

            if (retorno.QuantidadeRegistros != -1)
                Session["FMS_quantidaderegistros"] = retorno.QuantidadeRegistros;

            return Convert.ToInt64(Session["FMS_quantidaderegistros"]);
        }

        private TransacoesEmissorRetornoComQuantidade BuscarTransacaoEmissor(CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina, Servico.FMS.FMSClient objClient)
        {
            IntervaloData intervaloData = ValidarData(txtPeridoEspecificoDe, txtPeridoEspecificoAte);

            PesquisarTransacoesAnalisadasPorUsuarioEPeriodoEnvio envio = new PesquisarTransacoesAnalisadasPorUsuarioEPeriodoEnvio()
            {
                DataInicial = intervaloData.dataInicial,
                DataFinal = intervaloData.dataFinal,
                PosicaoPrimeiroRegistro = primeiroRegistro,
                QuantidadeRegistros = quantidadeRegistroPagina,
                Usuario = ddlUsuario.SelectedValue,
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                UsuarioLogin = GetSessaoAtual.LoginUsuario,
                TipoCartao = base.Tipocartao,
                CriterioOrdenacao = criterioOrdem,
                Ordem = ordemClassificacao,
                RenovarContador = (primeiroRegistro == 0)
            };

            TransacoesEmissorRetornoComQuantidade retorno = objClient.PesquisarTransacoesAnalisadasPorUsuarioEPeriodo(envio);

            return retorno;
        }

        private void CarregarListaUsuarios()
        {
            try
            {
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    string[] usuarios = objClient.PesquisarUsuariosPorEmissor(GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade, GetSessaoAtual.LoginUsuario);

                    ddlUsuario.Items.Add(new ListItem("Todos", string.Empty));

                    foreach (string usuario in usuarios)
                    {
                        ddlUsuario.Items.Add(new ListItem()
                        {
                            Text = usuario,
                            Value = usuario
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        protected override Control GetControleOndeColocarPaginacao()
        {
            return divPaginacao;
        }

        protected override CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo ObterCriterioOrdemInicial()
        {
            return CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo.Data;
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

        private TransacaoEmissor[] BuscarTransacaoEmissorRelatorio(Servico.FMS.FMSClient client)
        {
            IntervaloData intervaloData = ValidarData(txtPeridoEspecificoDe, txtPeridoEspecificoAte);

            PesquisarTransacoesAnalisadasPorUsuarioEPeriodoEnvio envio = new PesquisarTransacoesAnalisadasPorUsuarioEPeriodoEnvio()
            {
                DataInicial = intervaloData.dataInicial,
                DataFinal = intervaloData.dataFinal,
                PosicaoPrimeiroRegistro = Constantes.PosicaoInicialPrimeiroRegistro,
                QuantidadeRegistros = Constantes.QtdMaximaRegistros,
                RenovarContador = true,
                Usuario = ddlUsuario.SelectedValue,
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                UsuarioLogin = GetSessaoAtual.LoginUsuario,
                TipoCartao = base.Tipocartao,
                CriterioOrdenacao = ObterCriterioOrdemInicial(),
                Ordem = this.OrdemClassificacao
            };

            TransacaoEmissor[] retorno = client.ExportarTransacoesAnalisadasPorUsuarioEPeriodo(envio);

            return retorno;
        }

        public void Exportar()
        {
            try
            {
                using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
                {
                    TransacaoEmissor[] lista = BuscarTransacaoEmissorRelatorio(client);

                    if (lista.Length == 0)
                    {
                        base.ExibirMensagem("Não foram encontrados dados para a exportação.");
                        return;
                    }

                    string[] campos = new string[] { "Tipo alarme", "Tipo resposta", "Número do cartão", "Data/hora da transação", "Valor", "Score", "MCC", "UF", "C/D", "Bandeira", "Usuário", "Data/hora da análise", "Comentário" };

                    ExportadorHelper.GerarExcel(ExportadorHelper.ObterConsultaTransacoesPorPeriodoUsuarioRelatorio(lista), Response, campos);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }
    }
}
