using Redecard.PN.Comum;
using Redecard.PN.OutrosServicos.SharePoint.ZPContaCertaServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.OutrosServicos.SharePoint.ControlTemplates.OutrosServicos.Mdr
{
    public partial class GridDetalheOferta : UserControlBase
    {
        #region [ Atributos do controle ]

        /// <summary>
        /// Tipo do grid de detalhe
        /// </summary>
        /// <remarks>
        /// Tipos: <br />
        ///     - Detalhe
        ///     - Historico
        /// </remarks>
        public String TipoGrid { get; set; }

        #endregion

        #region [ Eventos do controle ]

        /// <summary>
        /// Evento de carregamento do controle/página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CarregarDados();
        }

        /// <summary>
        /// Databound para o repeater com os dados das ofertas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void repDetalheOferta_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DetalheOferta item = e.Item.DataItem as DetalheOferta;

                if (item != null)
                {
                    Literal ltrDataContratacao = e.Item.FindControl("ltrDataContratacao") as Literal;
                    Literal ltrDataSolicitacaoCancelamento = e.Item.FindControl("ltrDataSolicitacaoCancelamento") as Literal;
                    Literal ltrDataCancelamento = e.Item.FindControl("ltrDataCancelamento") as Literal;
                    Literal ltrPeriodoVigencia = e.Item.FindControl("ltrPeriodoVigencia") as Literal;
                    Literal ltrCodigoOferta = e.Item.FindControl("ltrCodigoOferta") as Literal;
                    Literal ltrStatusOferta = e.Item.FindControl("ltrStatusOferta") as Literal;
                    Literal ltrCanalContratacao = e.Item.FindControl("ltrCanalContratacao") as Literal;
                    Literal ltrCnpj = e.Item.FindControl("ltrCnpj") as Literal;
                    Literal ltrQuantidadeTerminais = e.Item.FindControl("ltrQuantidadeTerminais") as Literal;
                    Literal ltrTipoTecnologia = e.Item.FindControl("ltrTipoTecnologia") as Literal;
                    Literal ltrAluguelPrimeiro = e.Item.FindControl("ltrAluguelPrimeiro") as Literal;
                    Literal ltrAluguelDemais = e.Item.FindControl("ltrAluguelDemais") as Literal;

                    if (item.DataContratacao.HasValue)
                    {
                        ltrDataContratacao.Text = String.Format(
                            "{0} <span>{1} {2}</span>",
                            item.DataContratacao.Value.ToString("dd"),
                            item.DataContratacao.Value.ToString("MMM"),
                            item.DataContratacao.Value.ToString("yyyy"));
                    }
                    else
                    {
                        ltrDataContratacao.Text = "-";
                    }

                    ltrDataSolicitacaoCancelamento.Text = 
                        item.DataSolicitacaoCancelamento.HasValue &&
                        item.DataSolicitacaoCancelamento.Value != DateTime.MinValue ? 
                        item.DataSolicitacaoCancelamento.Value.ToShortDateString() : 
                        "-";

                    ltrDataCancelamento.Text = 
                        item.DataCancelamento.HasValue && 
                        item.DataCancelamento.Value != DateTime.MinValue ? 
                        item.DataCancelamento.Value.ToShortDateString() : 
                        "-";
                    
                    ltrPeriodoVigencia.Text = String.Format(
                        "{0} à {1}", 
                        item.PeriodoInicialVigencia.ToShortDateString(), 
                        (item.PeriodoFinalVigencia.Year != 9999 ? item.PeriodoFinalVigencia.ToShortDateString() : "Indeterminada"));

                    ltrCodigoOferta.Text = item.CodigoOferta.ToString();
                    ltrStatusOferta.Text = ObterDescricaoStatusOferta(item.Status);
                    ltrCanalContratacao.Text = item.DescricaoCanal;
                    ltrCnpj.Text = ((Int64)item.NumeroCnpj).FormatToCnpj();
                    ltrQuantidadeTerminais.Text = item.QuantidadeTerminais.ToString();
                    ltrTipoTecnologia.Text = item.TipoTecnologia;
                    ltrAluguelPrimeiro.Text = item.ValorAluguelPrimeiroTerminal.ToString("n");
                    ltrAluguelDemais.Text = item.ValorAluguelDemaisTerminais.ToString("n");
                }
            }
        }

        #endregion

        #region [ Métodos de consulta/carregamento dos dados ]

        /// <summary>
        /// Carrega os dados do grid
        /// </summary>
        private void CarregarDados()
        {
            if (!String.IsNullOrWhiteSpace(Request.QueryString["q"]))
            {
                Int16 codigoRetorno = default(Int16);

                if (String.Compare(TipoGrid, "Detalhe", true) == 0)
                {
                    var detalheOferta = ObterOferta(out codigoRetorno);
                    if (codigoRetorno <= 1)
                    {
                        BindDetalheOferta(detalheOferta);
                        phDetalhe.Visible = true;
                    }
                    else
                        qdAvisoDetalhe.Visible = true;
                }
                else if (String.Compare(TipoGrid, "Historico", true) == 0)
                {
                    var historicoOferta = ObterHistorico(out codigoRetorno);
                    if (codigoRetorno <= 1)
                    {
                        repDetalheOferta.DataSource = historicoOferta;
                        repDetalheOferta.DataBind();
                        repDetalheOferta.Visible = true;
                    }
                    else
                        qdAvisoDetalhe.Visible = true;
                }
                else
                    base.ExibirPainelExcecao("Grid não reconhecido para este controle.", CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Obtém os dados da oferta junto ao WCF
        /// </summary>
        /// <param name="codigoRetorno">OUT: código de retorno</param>
        /// <returns>Modelo retornado pelo serviço</returns>
        private DetalheOferta ObterOferta(out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Consulta o Detalhe da Oferta"))
            {
                DetalheOferta detalhe = new DetalheOferta();
                codigoRetorno = default(Int16);

                if (Sessao.Contem())
                {
                    Int32 numeroPv = SessaoAtual.CodigoEntidade;

                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["q"]);
                    Int16 codSitContrato = default(Int16);
                    DateTime dataFimVigencia = default(DateTime);
                    Int16.TryParse(queryString["codSitContrato"], out codSitContrato);
                    DateTime.TryParse(queryString["dataFimVigencia"], out dataFimVigencia);

                    try
                    {
                        using (var client = new ContextoWCF<ZPContaCertaServico.HISServicoZPContaCertaClient>())
                            detalhe = client.Cliente.ConsultarContratoVigencia(numeroPv, codSitContrato, dataFimVigencia, out codigoRetorno);
                    }
                    catch (FaultException<OfertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (FaultException<ZPContaCertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
                return detalhe;
            }
        }

        /// <summary>
        /// Obtém as ofertas históricas
        /// </summary>
        /// <param name="codigoRetorno">OUT: codigo de retorno do serviço</param>
        /// <returns>Lista com as ofertas</returns>
        private List<DetalheOferta> ObterHistorico(out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Consulta o histórico da oferta"))
            {
                List<DetalheOferta> historico = new List<DetalheOferta>();
                codigoRetorno = default(Int16);

                if (Sessao.Contem())
                {
                    Int32 numeroPv = SessaoAtual.CodigoEntidade;

                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["q"]);
                    Int16 codSitContrato = default(Int16);
                    Int16.TryParse(queryString["codSitContrato"], out codSitContrato);

                    try
                    {
                        using (var client = new ContextoWCF<ZPContaCertaServico.HISServicoZPContaCertaClient>())
                        {
                            historico = client.Cliente.ConsultarContratoHistorico(numeroPv, codSitContrato, out codigoRetorno);
                            if (historico != null && historico.Count > 0)
                                historico = historico.OrderByDescending(x => x.DataContratacao).ToList();
                        }
                    }
                    catch (FaultException<OfertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (FaultException<ZPContaCertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }

                return historico;
            }
        }

        #endregion

        #region [ Métodos auxiliares ]

        /// <summary>
        /// Apresenta os dados da oferta
        /// </summary>
        /// <param name="oferta">Modelo para representação dos dados</param>
        private void BindDetalheOferta(DetalheOferta oferta)
        {
            ltrValorAluguelPrimeiroTerminal.Text = oferta.ValorAluguelPrimeiroTerminal.ToString("n");
            ltrValorAluguelDemaisTerminais.Text = oferta.ValorAluguelDemaisTerminais.ToString("n");
            ltrQuantidadeTerminais.Text = oferta.QuantidadeTerminais.ToString();
            ltrDataContratacao.Text = oferta.DataContratacao.HasValue ? oferta.DataContratacao.Value.ToString("dd/MM/yyyy") : "";

            ltrPeriodoVigencia.Text = String.Format(
                "{0} a {1}",
                oferta.PeriodoInicialVigencia.ToShortDateString(),
                (oferta.PeriodoFinalVigencia.Year != 9999 ? oferta.PeriodoFinalVigencia.ToShortDateString() : "indeterminada"));

            ltrTipoTecnologia.Text = oferta.TipoTecnologia;
            ltrDescricaoCanal.Text = oferta.DescricaoCanal;
            ltrCodigoOferta.Text = oferta.CodigoOferta.ToString();
            ltrNumeroCnpj.Text = String.Format(@"{0:00\.000\.000\/0000\-00}", oferta.NumeroCnpj);
        }

        /// <summary>
        /// Traduz o status do serviço para uma descrição amigável
        /// </summary>
        /// <param name="status">Status original</param>
        /// <returns>Descrição amigável</returns>
        public String ObterDescricaoStatusOferta(StatusOferta status)
        {
            switch (status)
            {
                case StatusOferta.Contratado:
                    return "Ativa";
                case StatusOferta.Pendente:
                    return "Pendente";
                default:
                    return "Cancelada";
            }
        }
        
        #endregion
    }
}
