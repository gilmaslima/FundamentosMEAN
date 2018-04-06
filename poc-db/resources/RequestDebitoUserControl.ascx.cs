using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Redecard.PN.Comum;
using System.ServiceModel;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Web.UI.HtmlControls;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using PortalControl = Redecard.PN.Request.Core.Web.Controles.Portal;

namespace Redecard.PN.Request.SharePoint.WebParts.RequestDebito
{
    public partial class RequestDebitoUserControl : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Atribui o botão default da página
            this.Page.Form.DefaultButton = this.btnBuscar.UniqueID;

            if (!Page.IsPostBack)
            {
                lblMensagem.Visible = false;
                if (Request.QueryString["dados"] != null)
                {
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                    this.CarregarDados(queryString);
                }
                else
                {
                    this.CarregarDados(null);
                }
            }
            else { pnlErro.Visible = false; }
        }

        private void CarregarDados(QueryStringSegura qs)
        {
            if (qs == null)
                this.paginacao.Carregar();
            else
            {
                String tipoVenda = qs["TipoVenda"].Trim();
                Decimal numeroProcesso = qs["NumProcesso"].ToDecimalNull(0).Value;

                //Carrega os campos de filtro com os dados da queryString
                ddlTipoVenda.SelectedValue = tipoVenda;

                //Atualiza o repeater
                this.paginacao.Carregar(tipoVenda, numeroProcesso);

                this.ExibirAvisoDebito(true, false);
            }
        }

        private void ExibirAvisoDebito(Boolean exibirAvisosDebito, Boolean exibirSemAvisosDebito)
        {
            rptDebitos.Visible = exibirAvisosDebito;
            pnlSemAvisos.Visible = exibirSemAvisosDebito;
            (qSemAvisos as PortalControl.QuadroAviso).TipoQuadro = PortalControl.TipoQuadroAviso.Aviso;

            if (exibirSemAvisosDebito)
            {
                (qSemAvisos as PortalControl.QuadroAviso).Titulo = "Aviso";
                (qSemAvisos as PortalControl.QuadroAviso).Mensagem = "Não foram encontrados Avisos de Débito.";
            }
        }

        /// <summary>Chama o servico e carrega os Debitos pendentes</summary>        
        /// <param name="parametros">Possui objeto QueryString quando a solicitação vem da tela de Histórico</param>        
        protected IEnumerable<Object> Paginacao_ObterDados(Guid IdPesquisa, Int32 registroInicial, Int32 quantidadeRegistros, Int32 quantidadeRegistrosBuffer, out Int32 quantidadeTotalRegistrosEmCache, params Object[] parametros)
        {
            String msgLog = String.Format("Avisos de Débito - {0} ({1}-{2})", ddlTipoVenda.SelectedValue, registroInicial, registroInicial + quantidadeRegistros);

            using (Logger Log = Logger.IniciarLog(msgLog))
            {
                //Objetos de retorno
                quantidadeTotalRegistrosEmCache = 0;
                IEnumerable<Object> listaRetorno = new List<Object>();

                try
                {
                    Int32 codigoRetorno = 0;
                    Int32 codEstabelecimento = base.SessaoAtual.CodigoEntidade;
                    String sistemaOrigem = SessaoAtual.UsuarioAtendimento ? "IZ" : "IS";
                    String tipoVenda = string.Empty;
                    Decimal codProcesso = 0;
                    string programa = string.Empty;

                    if (parametros != null && parametros.Length > 1)
                    {
                        tipoVenda = parametros[0].ToString();
                        codProcesso = parametros[1].ToString().ToDecimal();
                        programa = "REQUESTHISTORICO";//parametros usado quando TipoVenda=Crédito
                    }
                    else
                    {
                        tipoVenda = ddlTipoVenda.SelectedValue.ToUpper();
                        codProcesso = 0; //se não vier por query string, é zerado
                        programa = "DEBITO";//parametros usado quando TipoVenda=Crédito
                    }

                    #region Crédito
                    if (tipoVenda.Equals("CREDITO", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Log.GravarMensagem("Instanciando serviço de Avisos de Débito - Crédito");

                        //variáveis de servico
                        using (XBChargebackServico.HISServicoXBChargebackClient client = new XBChargebackServico.HISServicoXBChargebackClient())
                        {
                            Log.GravarLog(EventoLog.ChamadaServico, new {
                                IdPesquisa, registroInicial, quantidadeRegistros, quantidadeRegistrosBuffer,
                                codEstabelecimento, codProcesso, programa, sistemaOrigem });

                            //recuperando os requests pendentes
                            XBChargebackServico.AvisoDebito[] avisos = client.ConsultarDebitoPendente(
                                out quantidadeTotalRegistrosEmCache,
                                out codigoRetorno,
                                IdPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeRegistrosBuffer,
                                codEstabelecimento,
                                programa,
                                sistemaOrigem);

                            Log.GravarLog(EventoLog.RetornoServico, new {
                                quantidadeTotalRegistrosEmCache, codigoRetorno, avisos });

                            //caso o código de retorno seja != 0 ocorreu um erro.
                            //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                            if (codigoRetorno > 0 && codigoRetorno != 10 && codigoRetorno != 53)
                                base.ExibirPainelExcecao("XBChargebackServico.ConsultarDebitoPendente", codigoRetorno);
                            else
                                listaRetorno = avisos;
                        }

                        //Se não retornou registros, exibe mensagem adequada
                        Boolean exibirAvisos = listaRetorno != null && listaRetorno.Count() > 0 && quantidadeTotalRegistrosEmCache > 0;
                        this.ExibirAvisoDebito(exibirAvisos, !exibirAvisos);
                        cvValidaData.Visible = false;
                    }
                    #endregion
                    #region Débito
                    else if (tipoVenda.Equals("DEBITO", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //parâmetros                        
                        DateTime dataInicio = new DateTime(1980, 1, 1);
                        DateTime dataFim = new DateTime(2999, 12, 31);
                        string codigoTransacao = "XB96";

                        if (codProcesso == 0)
                        {
                            DateTime.TryParseExact(txtDtIniDatePicker.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataInicio);
                            DateTime.TryParseExact(txtDtFimDatePicker.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataFim);
                        }

                        Log.GravarMensagem("Instanciando serviço de Avisos de Débito - Débito");

                        //variáveis de servico
                        using (XBChargebackServico.HISServicoXBChargebackClient client = new XBChargebackServico.HISServicoXBChargebackClient())
                        {
                            Log.GravarLog(EventoLog.ChamadaServico, new {
                                IdPesquisa, registroInicial, quantidadeRegistros, quantidadeRegistrosBuffer, codProcesso,
                                codEstabelecimento, dataInicio, dataFim, sistemaOrigem, codigoTransacao });                            

                            XBChargebackServico.AvisoDebito[] avisos = client.ConsultarAvisosDebito(
                                out quantidadeTotalRegistrosEmCache,
                                out codigoRetorno,
                                IdPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeRegistrosBuffer,
                                codProcesso,
                                codEstabelecimento,
                                dataInicio,
                                dataFim,
                                sistemaOrigem,
                                codigoTransacao);

                            Log.GravarLog(EventoLog.RetornoServico, new { quantidadeTotalRegistrosEmCache, codigoRetorno, avisos });

                            //caso o código de retorno seja != 0 ocorreu um erro.
                            //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                            if (codigoRetorno > 0 && codigoRetorno != 10 && codigoRetorno != 53)
                                base.ExibirPainelExcecao("XBChargebackServico.ConsultarAvisosDebito", codigoRetorno);
                            else
                                listaRetorno = avisos;
                        }

                        //Se não retornou registros, exibe mensagem adequada
                        Boolean exibirAvisos = listaRetorno != null && listaRetorno.Count() > 0 && quantidadeTotalRegistrosEmCache > 0;
                        this.ExibirAvisoDebito(exibirAvisos, !exibirAvisos);
                        cvValidaData.Visible = false;
                    }
                    #endregion
                    #region Selecione
                    else if (tipoVenda.Equals("SELECIONE", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Log.GravarMensagem("Nenhum tipo de venda selecionado");
                        this.ExibirAvisoDebito(false, false);
                        cvValidaData.Visible = false;
                    }
                    #endregion
                }
                catch (FaultException<XBChargebackServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirAvisoDebito(false, false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirAvisoDebito(false, false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                return listaRetorno;
            }
        }

        #region Negócio e Carga de Dados
        /// <summary>
        /// Exibe a mensagem de erro
        /// </summary>
        private void SetarAviso(String aviso)
        {
            lblMensagem.Text = aviso;
            lblMensagem.Visible = true;
            pnlErro.Visible = true;            
        }

        #endregion


        #region Handlers de eventos

        /// <summary>
        /// Apresenta as informações no asp:repeater.
        /// </summary>
        protected void rptDebito_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Header)
                {
                    HtmlTableCell td = (HtmlTableCell)e.Item.FindControl("tdHead");
                    HtmlTableCell tdPenultimoHead = (HtmlTableCell)e.Item.FindControl("tdPenultimoHead");
                    if (td != null && tdPenultimoHead != null)
                    {
                        if (ddlTipoVenda.SelectedValue.Equals("Debito"))
                            td.Visible = false;
                        else
                            tdPenultimoHead.Attributes.Remove("class");
                    }
                }
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    // Recupera objetos da linha
                    var lblNumProcesso = (Label)e.Item.FindControl("lblNumProcesso");
                    var lblNumResumo = (Label)e.Item.FindControl("lblNumResumo");
                    var btnRequest = (HyperLink)e.Item.FindControl("btnRequest");
                    var lblNumCartao = (Label)e.Item.FindControl("lblNumCartao");
                    var lblCentralizadora = (Label)e.Item.FindControl("lblCentralizadora");
                    var lblDataVenda = (Label)e.Item.FindControl("lblDataVenda");
                    var lblValorVenda = (Label)e.Item.FindControl("lblValorVenda");
                    var lblDataCancel = (Label)e.Item.FindControl("lblDataCancel");
                    var lblValorCancel = (Label)e.Item.FindControl("lblValorCancel");
                    var btnMotivo = (PortalControl.BotaoInformacao)e.Item.FindControl("btnMotivo");
                    var btnParc = (HyperLink)e.Item.FindControl("btnParc");
                    XBChargebackServico.HISServicoXBChargebackClient client = new XBChargebackServico.HISServicoXBChargebackClient();

                    Int64 codigoOcorrencia = 0;
                    Int32 codigoRetorno = 0;
                    string motivo = string.Empty;
                    //converte a linha num objeto request
                    if (ddlTipoVenda.SelectedValue == "Credito")
                    {
                        XBChargebackServico.AvisoDebito request = (XBChargebackServico.AvisoDebito)e.Item.DataItem;

                        //setando os valores corretos nas labels das linhas
                        lblNumProcesso.Text = request.Processo.ToString();
                        lblNumResumo.Text = request.ResumoVenda.ToString();
                        lblNumCartao.Text = request.NumeroCartao;
                        lblCentralizadora.Text = request.Centralizadora.ToString();
                        lblDataVenda.Text = request.DataVenda.ToString("dd/MM/yy");
                        lblValorVenda.Text = request.ValorVenda.ToString("N2");
                        lblDataCancel.Text = request.DataCancelamento.ToString("dd/MM/yy");
                        lblValorCancel.Text = request.ValorLiquidoCancelamento.ToString("N2");

                        // Carrega o motivo do débito
                        if (request.CodigoMotivoDebito > 0)
                        {
                            motivo = client.ConsultarMotivoDebito(out codigoRetorno, request.CodigoMotivoDebito, "IS", "XB86");
                            btnMotivo.Mensagem = motivo;
                            btnMotivo.Titulo = "MOTIVO DO PROCESSO";
                        }
                        btnMotivo.Visible = request.CodigoMotivoDebito > 0;//Esconde o botão de motivo caso o código do motivo <=0
                        btnParc.Visible = request.IndicadorParcela ?? false; // Esconde o botão de parcelas caso o indicador de parcela false
                        btnRequest.Visible = request.IndicadorRequest;//Esconde o botão caso o indicador de request seja falso

                        HtmlTableCell tdPenultimoItem = (HtmlTableCell)e.Item.FindControl("tdPenultimoItem");
                        tdPenultimoItem.Attributes.Remove("class");
                    }
                    else
                    {
                        XBChargebackServico.HISServicoXBChargebackClient clienteDebito = new XBChargebackServico.HISServicoXBChargebackClient();
                        XBChargebackServico.AvisoDebito request = (XBChargebackServico.AvisoDebito)e.Item.DataItem;

                        //setando os valores corretos nas labels das linhas
                        lblNumProcesso.Text = request.Processo.ToString();
                        lblNumResumo.Text = request.ResumoVenda.ToString();
                        lblNumCartao.Text = request.TipoCartao;
                        lblCentralizadora.Text = request.Centralizadora.ToString();
                        lblDataVenda.Text = request.DataVenda.ToString("dd/MM/yy");
                        lblValorVenda.Text = request.ValorVenda.ToString("N2");
                        lblDataCancel.Text = request.DataCancelamento.ToString("dd/MM/yy");
                        lblValorCancel.Text = request.ValorLiquidoCancelamento.ToString("N2");
                        if (request.CodigoMotivoDebito > 0)
                        {
                            // Carrega o motivo do débito
                            motivo = clienteDebito.ConsultarMotivoDebito(out codigoRetorno, request.CodigoMotivoDebito, "IS", "XB86");
                            btnMotivo.Attributes.Add("mensagem", HttpUtility.HtmlEncode(motivo));
                            btnMotivo.Attributes.Add("titulo", "MOTIVO DO PROCESSO");
                        }
                        btnMotivo.Visible = request.CodigoMotivoDebito > 0;//Esconde o botão de motivo caso o código do motivo <=0
                        btnParc.Visible = request.IndicadorParcela ?? false; // Esconde o botão de parcelas caso o indicador de parcela false
                        btnRequest.Visible = request.IndicadorRequest;//Esconde o botão caso o indicador de request seja falso
                        HtmlTableCell tdItem = (HtmlTableCell)e.Item.FindControl("tdItem");
                        if (tdItem != null)
                            tdItem.Visible = false;
                    }

                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString["NumProcesso"] = lblNumProcesso.Text;
                    queryString["PV"] = lblCentralizadora.Text;
                    queryString["TipoVenda"] = ddlTipoVenda.SelectedValue;

                    btnRequest.NavigateUrl = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_HistoricoComprovantes.aspx?dados={0}", queryString.ToString());
                    btnParc.NavigateUrl = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_ResumoVendas.aspx?dados={0}", queryString.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante DataBind de dados de Avisos de Débito", ex);
                ExibirAvisoDebito(false, false);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }       
        }
        #endregion

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            switch (ddlTipoVenda.SelectedValue.ToUpper())
            {
                case "SELECIONE": //não exibe nada
                    rptDebitos.Visible = false;
                    pnlSemAvisos.Visible = false;
                    cvValidaData.Visible = false;
                    break;
                case "DEBITO": //exibe painel de filtro por data e botão buscar
                    rptDebitos.Visible = false;
                    pnlSemAvisos.Visible = false;
                    cvValidaData.Visible= true;

                    //Valida os campos digitados
                    cvValidaData.Validate();

                    if (cvValidaData.IsValid)
                    {
                        //Carrega os dados da pesquisa
                        CarregarDados(null);
                    }
                    break;
                case "CREDITO": //exibe automaticamente o repeater com os dados
                    rptDebitos.Visible = true;
                    cvValidaData.Visible = false;
                    this.ExibirAvisoDebito(false, false);
                    this.CarregarDados(null);
                    break;
            }
        }
        /// <summary>Valida os filtros informados. </summary>
        /// <returns>Mensagem de validação. Retorna String.Empty caso validação esteja OK.</returns>
        private String ValidarFiltros()
        {
            //Verifica se o período informado está dentro dos 60 dias
            DateTime dtInicio = DateTime.MinValue;
            DateTime dtFim = DateTime.MinValue;
            DateTime dtNow = DateTime.Now;
            TimeSpan dtDiff = default(TimeSpan);


            if (!DateTime.TryParse(txtDtIniDatePicker.Text, out dtInicio))
            {
                return "Data inicial do período não foi informada ou inválida.";
            }
            if (!DateTime.TryParse(txtDtFimDatePicker.Text, out dtFim))
            {
                return "Data final do período não foi informada ou inválida.";
            }

            //A data final não pode ser maior que hoje
            dtDiff = dtNow.Subtract(dtFim);
            if (dtDiff.TotalDays < 0)
                return "O período contém data futura!";

            //Calcula a diferença de dias entre as datas informadas, limitando em 60 dias
            dtDiff = dtFim.Subtract(dtInicio);
            if (dtDiff.TotalDays > 60)
                return "A consulta não deve ultrapassar 60 dias!";

            //A data final não pode ser menor que a data inicial
            dtDiff = dtFim.Subtract(dtInicio);
            if (dtDiff.TotalDays < 0)
                return "Período inválido! A data final não pode ser menor que a data inicial.";
            else return string.Empty;
        }

        protected void cvValidaData_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string mensagem = ValidarFiltros();
            if (!string.IsNullOrEmpty(mensagem))
            {
                cvValidaData.Text = mensagem;
                cvValidaData.Visible = true;
                args.IsValid = false;
            }
            else
            {
                cvValidaData.Visible = false;
                args.IsValid = true;
            }
        }
    }
}
