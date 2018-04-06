using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Request.SharePoint.Business;
using Redecard.PN.Request.SharePoint.Model;
using Redecard.PN.Request.SharePoint.XBChargebackServico;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Request.SharePoint.Layouts.Request
{
    /// <summary>
    /// Mecanismo de impressão dos comprovantes de vendas (históricos e pendentes)
    /// </summary>
    public partial class ComprovantesImpressao : ApplicationPageBaseAnonima
    {
        /// <summary>
        /// Ids para as pesquisas armazenadas em cache o lado servidor
        /// </summary>
        public List<ComprovanteRequestIdPesquisa> ListPesquisaId
        {
            get
            {
                if (Session["ListPesquisaIdImpressao"] != null)
                    return (List<ComprovanteRequestIdPesquisa>)Session["ListPesquisaIdImpressao"];

                return new List<ComprovanteRequestIdPesquisa>();
            }
        }

        /// <summary>
        /// Evento de carregamento da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ComprovanteServiceRequest filter = this.GetFilterData();

            String msgLog = String.Format(
                "RequestReport - {0} ({1}-{2})",
                filter.TipoVenda.ToString(),
                filter.RegistroInicial,
                filter.QuantidadeRegistros);

            using (Logger Log = Logger.IniciarLog(msgLog))
            {
                try
                {
                    var response = ComprovantesService.Consultar(filter, this.ListPesquisaId);

                    // se não retornou registros, exibe mensagem adequada
                    Boolean exibirComprovantes =
                        response.Comprovantes.Count > 0 &&
                        response.QuantidadeTotalRegistrosEmCache > 0;

                    if (exibirComprovantes)
                    {
                        rptComprovantes.DataSource = response.Comprovantes;
                        rptComprovantes.DataBind();
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

        /// <summary>
        /// Handler que exibe efetivamente as informações nas linhas da tabela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptComprovantes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    // Recupera objetos da linha
                    var ltlNumProcesso = (Literal)e.Item.FindControl("ltlNumProcesso");
                    var ltlNumResumo = (Literal)e.Item.FindControl("ltlNumResumo");
                    var ltlCentralizadora = (Literal)e.Item.FindControl("ltlCentralizadora");
                    var ltlNumCartao = (Literal)e.Item.FindControl("ltlNumCartao");
                    var ltlMotivo = (Literal)e.Item.FindControl("ltlMotivo");
                    var ltlDataVenda = (Literal)e.Item.FindControl("ltlDataVenda");
                    var ltlValorVenda = (Literal)e.Item.FindControl("ltlValorVenda");
                    var ltlEnvioNotificacao = (Literal)e.Item.FindControl("ltlEnvioNotificacao");
                    var ltlDocEnviado = (Literal)e.Item.FindControl("ltlDocEnviado");
                    var ltlQualidadeDoc = (Literal)e.Item.FindControl("ltlQualidadeDoc");
                    var ltlPrazo = (Literal)e.Item.FindControl("ltlPrazo");

                    ComprovanteModel model = e.Item.DataItem as ComprovanteModel;

                    // numero do processo
                    ltlNumProcesso.Text = model.Processo.ToString();

                    // resumo de vendas
                    ltlNumResumo.Text = model.ResumoVenda.ToString();

                    // centralizadora
                    if (model.Status == StatusComprovante.Pendente && model.TipoVenda == TipoVendaComprovante.Credito)
                    {
                        ltlCentralizadora.Text = String.Format(
                            "{0}/{1}",
                            model.Centralizadora.ToString(),
                            model.PontoVenda.ToString());
                    }
                    else
                    {
                        ltlCentralizadora.Text = model.Centralizadora.ToString();
                    }

                    // número do cartão
                    if (model.Status == StatusComprovante.Pendente)
                    {
                        if (model.TipoVenda == TipoVendaComprovante.Credito)
                        {
                            ltlNumCartao.Text = model.FlagNSUCartao == 'C'
                                    ? (model.NumeroCartao)
                                    : (model.NumeroCartao.ToDecimal() > 0 ? model.NumeroCartao : "-");
                        }
                        else
                        {
                            ltlNumCartao.Text = model.NumeroReferencia;
                        }
                    }
                    else
                    {
                        ltlNumCartao.Text = model.NumeroCartao;
                    }

                    // data da venda
                    ltlDataVenda.Text = model.DataVenda.ToString("dd/MM/yy");

                    // valor da venda
                    ltlValorVenda.Text = model.ValorVenda.ToString("N2");

                    // envio de notificação
                    String canalEnvio = String.Empty;
                    if (model.Status == StatusComprovante.Pendente)
                    {
                        canalEnvio = model.CanalEnvio.HasValue
                            ? ComprovantesService.ObtemDescricaoCanalEnvio((Int32?)model.CanalEnvio, model.DescricaoCanalEnvio)
                            : String.Empty;
                    }
                    else
                    {
                        canalEnvio = ComprovantesService.ObtemDescricaoCanalEnvio((Int32?)model.CanalEnvio, null);
                    }

                    String dataEnvio =
                        model.DataEnvio.HasValue && model.DataEnvio.Value > DateTime.MinValue
                        ? (model.DataEnvio.Value.ToString("dd/MM/yy"))
                        : String.Empty;
                    ltlEnvioNotificacao.Text = String.Format("{0} {1}", canalEnvio, dataEnvio);

                    // documento enviado
                    ltlDocEnviado.Text = model.SolicitacaoAtendida ? "sim" : "não";

                    // qualidade do documento
                    ltlQualidadeDoc.Text = model.QualidadeRecebimentoDocumentos;

                    // botão de informação
                    ltlMotivo.Text = model.Motivo;
                }
            }
            catch (FormatException ex)
            {
                Logger.GravarErro("Erro de FormatException durante DataBind de dados de Histórico de Comprovantes", ex);
                SharePointUlsLog.LogErro(ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.GravarErro("Erro de ArgumentOutOfRangeException durante DataBind de dados de Histórico de Comprovantes", ex);
                SharePointUlsLog.LogErro(ex);
            }
            catch (NullReferenceException ex)
            {
                Logger.GravarErro("Erro de NulReferenceException durante DataBind de dados de Histórico de Comprovantes", ex);
                SharePointUlsLog.LogErro(ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro genérico durante DataBind de dados de Histórico de Comprovantes", ex);
                SharePointUlsLog.LogErro(ex);
            }
        }

        /// <summary>
        /// Obtém os dados passados na URL
        /// </summary>
        /// <returns></returns>
        private ComprovanteServiceRequest GetFilterData()
        {
            ComprovanteServiceRequest data = new ComprovanteServiceRequest();
            QueryStringSegura qs = new QueryStringSegura(Request.QueryString["dados"]);

            data.DataInicio = qs["DataInicio"].ToDateTimeNull() ?? DateTime.Now.AddDays(-60);
            data.DataFim = qs["DataFim"].ToDateTimeNull() ?? DateTime.Now;
            data.CodProcesso = qs["CodProcesso"].ToDecimalNull();

            if (qs["TipoVenda"] != null)
            {
                var tipoVenda = TipoVendaComprovante.Nenhum;
                Enum.TryParse(qs["TipoVenda"], out tipoVenda);
                data.TipoVenda = tipoVenda;
            }

            if (qs["Status"] != null)
            {
                var status = StatusComprovante.Todos;
                Enum.TryParse(qs["Status"], out status);
                data.Status = status;
            }

            data.SessaoUsuario = Sessao.Obtem();
            data.QuantidadeRegistros = 0;
            data.Parametros = new String[] { "REQUESTHISTORICO" };
            data.RegistroInicial = 0;

            return data;
        }

        /// <summary>
        /// Definições pré-renderização
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            Boolean excel = false;
            Boolean.TryParse(Request.QueryString["excel"], out excel);

            if (excel)
            {
                var sessao = Sessao.Obtem();
                pnlHeaderImpressao.Visible = true;

                ltlDataConsulta.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                ltlUsuario.Text = sessao.LoginUsuario;
                ltlNomeEstb.Text = sessao.NomeEntidade;
                ltlNumeroPv.Text = sessao.CodigoEntidade.ToString();

                Response.Clear();
                Response.ClearHeaders();
                Response.AddHeader("Content-Disposition", String.Format("attachment;filename=Comprovantes_{0}.xls", DateTime.Now.ToString("yyyyMMdd_hhmmss")));
                Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.ContentType = "application/ms-excel";
                Response.Buffer = true;
            }
        }
    }
}
