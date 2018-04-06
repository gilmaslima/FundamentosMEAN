using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using Redecard.PN.FMS.Sharepoint.Exceptions;
using Redecard.PN.FMS.Sharepoint.Helpers;
using Redecard.PN.FMS.Sharepoint.Interfaces;
using Redecard.PN.FMS.Sharepoint.Modelo;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;

namespace Redecard.PN.FMS.Sharepoint.WebParts.AnalisaTransacoesSuspeitas
{
    public partial class AnalisaTransacoesSuspeitasUserControl : RelatorioGridBaseUserControl<CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin>, IPossuiExportacao
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarListaTransacoesSuspeitas();
            }
        }


        protected void DescartarDadosSessao()
        {
            try
            {
                using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
                {
                    client.DescartarSessaoPesquisaTransacoesSuspeitas(GetSessaoAtual.CodigoEntidade,
                        GetSessaoAtual.GrupoEntidade,
                        GetSessaoAtual.LoginUsuario);
                }

            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        protected void grvDados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            Servico.FMS.TransacaoEmissor transacao = (Servico.FMS.TransacaoEmissor)e.Row.DataItem;

            SPListaPadrao lista = base.ObterPadraoASerAplicadoPorSituacaoFraude(transacao.SituacaoTransacao);

            if (lista != null)
            {
                e.Row.Cells[10].Style.Add("color", lista.CorDeLetra);
                e.Row.Cells[10].Style.Add("background-color", lista.CorDeFundo);

                e.Row.Cells[10].Text = lista.Titulo;
            }

            e.Row.Cells[e.Row.Cells.Count - 1].CssClass += " last";
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            decimal numeroCartao;

            if (!decimal.TryParse(txtNumeroCartao.Text, out numeroCartao))
            {
                if (string.IsNullOrEmpty(txtNumeroCartao.Text))
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

            Redecard.PN.Comum.SharePointUlsLog.LogMensagem(string.Format("[Análise de transações suspeitas] - numeroCartao [{0}]", numeroCartao));

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
                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas] - início da execução do serviço 'PesquisarTransacoesPorNumeroCartaoEEstabelecimento'.");

                    Servico.FMS.PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno retorno = client.PesquisarTransacoesPorNumeroCartaoEEstabelecimento(envio);

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas] - fim da execução do serviço 'PesquisarTransacoesPorNumeroCartaoEEstabelecimento'.");

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

        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            DescartarDadosSessao();
            CarregarListaTransacoesSuspeitas();
        }

        private void CarregarListaTransacoesSuspeitas()
        {
            base.MontaGridInicial();
        }

        protected override CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin ObterCriterioOrdemInicial()
        {
            return CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin.NumeroCartao;
        }

        protected override long MontaGrid(FMSClient objClient, CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            Servico.FMS.PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginEnvio envio = new PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginEnvio()
            {
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                UsuarioLogin = GetSessaoAtual.LoginUsuario,
                TipoCartao = base.Tipocartao,
                RenovarContador = true,
                PosicaoPrimeiroRegistro = primeiroRegistro,
                QuantidadeRegistros = quantidadeRegistroPagina,
                Criterio = criterioOrdem,
                Ordem = ordemClassificacao
            };

            Servico.FMS.PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno retorno = objClient.PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(envio);

            grvDados.DataSource = retorno.ListaTransacoesEmissor;
            grvDados.DataBind();

            if (retorno.QuantidadeHorasRecuperadas >= 0)
            {
                if (retorno.QuantidadeHorasTotalPeriodo > retorno.QuantidadeHorasRecuperadas)
                {
                    int qtdHorasTotalPeriodo = retorno.QuantidadeHorasTotalPeriodo;
                    int qtdHorasRecuperadas = retorno.QuantidadeHorasRecuperadas;

                    if (qtdHorasTotalPeriodo > 0)
                        qtdHorasTotalPeriodo = qtdHorasTotalPeriodo / 24;

                    if (qtdHorasRecuperadas > 0)
                        qtdHorasRecuperadas = qtdHorasRecuperadas / 24;

                    string msg = string.Format("Seus critérios de seleção permitiram que fossem recuperadas transações suspeitas em um período de {0} dias.\n\nA quantidade máxima, que pode ser pesquisada, é de {1} dias.", qtdHorasTotalPeriodo, qtdHorasRecuperadas);

                    base.ExibirMensagem(msg);

                    return retorno.QuantidadeTotalRegistros;
                }
            }

            return retorno.QuantidadeTotalRegistros;
        }

        protected override Control GetControleOndeColocarPaginacao()
        {
            return divPaginacao;
        }

        protected void LinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                string commandArgument = (sender as LinkButton).CommandArgument;

                long identificadorTransacao;

                long.TryParse(commandArgument, out identificadorTransacao);

                Redecard.PN.Comum.SharePointUlsLog.LogMensagem(string.Format("[Análise de transações suspeitas] - identificadorTransacao [{0}]", identificadorTransacao));

                Servico.FMS.PesquisarTransacoesPorTransacaoAssociadaEnvio envio = new Servico.FMS.PesquisarTransacoesPorTransacaoAssociadaEnvio()
                {
                    IdentificadorTransacao = identificadorTransacao,
                    NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                    GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                    UsuarioLogin = GetSessaoAtual.LoginUsuario,
                    TempoBloqueio = base.ParametrosSistema.TempoExpiracaoBloqueio,
                    TipoCartao = base.Tipocartao
                };

                using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
                {
                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas] - início da execução do serviço 'PesquisarTransacoesPorTransacaoAssociada'.");

                    Servico.FMS.TransacaoEmissor[] retorno = client.PesquisarTransacoesPorTransacaoAssociada(envio);

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas] - fim da execução do serviço 'PesquisarTransacoesPorTransacaoAssociada'.");

                    Session["FMS_transacoes"] = new List<Servico.FMS.TransacaoEmissor>(retorno);
                }

                
                QueryStringSegura query = new QueryStringSegura();
                query.Add("identificadorTransacao", identificadorTransacao.ToString());
                
                Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas] - redirecionando o usuário para a página 'pn_AnalisaTransacoesSuspeitasPorCartao.aspx'.");

                SPUtility.Redirect(String.Format("pn_AnalisaTransacoesSuspeitasPorCartao.aspx?dados={0}", query.ToString()), SPRedirectFlags.Default, Context);
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        private TransacaoEmissor[] BuscarTransacoesExportar()
        {
            PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginEnvio envio = new PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginEnvio()
            {
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                UsuarioLogin = GetSessaoAtual.LoginUsuario,
                TipoCartao = base.Tipocartao,
                RenovarContador = true,
                PosicaoPrimeiroRegistro = Constantes.PosicaoInicialPrimeiroRegistro,
                QuantidadeRegistros = Constantes.QtdMaximaRegistros,
                Criterio = ObterCriterioOrdemInicial(),
                Ordem = Servico.FMS.OrdemClassificacao.Ascendente
            };

            using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
            {
                Servico.FMS.PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno retorno = client.ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin(envio);

                return retorno.ListaTransacoesEmissor;
            }
        }

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
                    TransacaoEmissor[] lista = BuscarTransacoesExportar();

                    if (lista.Length == 0)
                    {
                        base.ExibirMensagem("Não foram encontrados dados para a exportação.");
                        return;
                    }

                    string[] campos = new string[] { "Tipo de alarme", "Cartão", "Valor", "Data/hora da transação", "Score", "MCC", "C/D", "UF", "Bandeira", "Status da transação" };

                    ExportadorHelper.GerarExcel(ExportadorHelper.ObterAnalisaTransacoesSuspeitasRelatorio(lista), Response, campos);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }


    }
}
