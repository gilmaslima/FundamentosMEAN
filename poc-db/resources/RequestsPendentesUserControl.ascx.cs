using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.Request.SharePoint.Model;
using Redecard.PN.Request.SharePoint.XBChargebackServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Controles = Redecard.PN.Request.Core.Web.Controles.Portal;

namespace Redecard.PN.Request.SharePoint.WebParts.RequestsPendentes
{
    public partial class RequestsPendentesUserControl : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request["dados"] != null) //Integração com a Home EMPI/IBBA
                {
                    var qs = new QueryStringSegura(Request["dados"]);
                    String tipoVenda = qs["TipoVenda"]; //"Credito" ou "Debito"

                    ListItem item = ddlTipoVenda.Items.FindByValue(tipoVenda);
                    if (item != null)
                        ddlTipoVenda.SelectedIndex = ddlTipoVenda.Items.IndexOf(item);
                }

                this.paginacao.Carregar();
                //this.paginacaoOld.Carregar();
                //this.lblMensagem.Visible = false;
            }
            //else { pnlErro.Visible = false; }
        }

        #region Negócio e Carga de Dados
        /// <summary>
        /// Exibe a mensagem de erro
        /// </summary>
        //private void SetarAviso(String aviso)
        //{
        //    lblMensagem.Text = aviso;
        //    lblMensagem.Visible = true;
        //    pnlErro.Visible = true;
        //    pnlConteudo.Visible = false;
        //}

        /// <summary>
        /// Exibe o repeater com os comprovantes, ou exibe mensagem de ausência de dados
        /// </summary>        
        private void ExibirComprovantes(Boolean exibirRepeater, Boolean exibirPainelSemComprovantes)
        {
            rptPendentes.Visible = exibirRepeater;
            pnlSemComprovantes.Visible = exibirPainelSemComprovantes;
            qdAvisoSemComprovantes.TipoQuadro = Controles.TipoQuadroAviso.Aviso;

            if (exibirPainelSemComprovantes)
                qdAvisoSemComprovantes.Mensagem = "Não foram encontrados comprovantes pendentes.";

            //trBotaoVoltarRight.Visible = exibirRepeater;
            //trBotaoVoltarLeft.Visible = !exibirRepeater;
        }

        /// <summary>
        /// Carrega os Requests Pendentes
        /// </summary>    
        protected IEnumerable<Object> Paginacao_ObterDados(Guid IdPesquisa, int registroInicial, int quantidadeRegistros, int quantidadeRegistroBuffer, out int quantidadeTotalRegistrosEmCache, params object[] parametros)
        {
            String msgLog = String.Format("Comprovantes Pendentes - {0} ({1}-{2})", ddlTipoVenda.SelectedValue, registroInicial, registroInicial + quantidadeRegistros);

            using (Logger Log = Logger.IniciarLog(msgLog))
            {
                //Objetos de retorno
                IEnumerable<Object> retorno = new List<Object>();
                quantidadeTotalRegistrosEmCache = 0;

                try
                {
                    Int32 codEstabelecimento = base.SessaoAtual.CodigoEntidade;
                    String sistemaOrigem = SessaoAtual.UsuarioAtendimento ? "IZ" : "IS";
                    Decimal? processo = Regex.Replace(txtNumeroProcesso.Text, "[^0-9]", "").ToDecimalNull(null);

                    #region Crédito
                    if (ddlTipoVenda.SelectedValue == "Credito")
                    {
                        Decimal codProcesso = 0;
                        String programa = "REQUESTPENDENTE";
                        Int32 codigoRetorno = 0;

                        Log.GravarMensagem("Instanciando serviço HISServicoXB - Crédito");

                        //variáveis de servico
                        using (HISServicoXBChargebackClient client = new HISServicoXBChargebackClient())
                        {
                            Log.GravarLog(EventoLog.ChamadaServico, new
                            {
                                TipoVenda = ddlTipoVenda.SelectedValue,
                                IdPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeRegistroBuffer,
                                codEstabelecimento,
                                codProcesso,
                                programa,
                                sistemaOrigem,
                                processo
                            });

                            //recuperando os requests pendentes
                            XBChargebackServico.Comprovante[] requests = client.ConsultarRequestPendente(
                                out quantidadeTotalRegistrosEmCache,
                                out codigoRetorno,
                                IdPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeRegistroBuffer,
                                codEstabelecimento,
                                programa,
                                sistemaOrigem,
                                processo);

                            Log.GravarLog(EventoLog.RetornoServico, new
                            {
                                quantidadeTotalRegistrosEmCache,
                                codigoRetorno,
                                requests
                            });

                            //caso o código de retorno seja != 0 ocorreu um erro.
                            //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                            if (codigoRetorno > 0 && codigoRetorno != 10 && codigoRetorno != 53)
                                base.ExibirPainelExcecao("XBChargebackServico.ConsultarRequestPendente", codigoRetorno);
                            else
                                retorno = requests;
                        }

                        //Se não retornou registros, exibe mensagem adequada
                        Boolean exibirComprovantes = retorno != null && retorno.Count() > 0 && quantidadeTotalRegistrosEmCache > 0;
                        ExibirComprovantes(exibirComprovantes, !exibirComprovantes);
                    }
                    #endregion
                    #region Débito
                    else if (ddlTipoVenda.SelectedValue == "Debito")
                    {
                        //parametros 
                        Int16 codigoRetorno = 0;
                        DateTime dataInicio = new DateTime(1980, 1, 1, 0, 0, 0);
                        DateTime dataFim = new DateTime(2999, 12, 1);
                        String transacao = "XB94";

                        Log.GravarMensagem("Instanciando serviço HISServicoXB - Débito");

                        //variáveis de servico
                        using (HISServicoXBChargebackClient client = new HISServicoXBChargebackClient())
                        {
                            Log.GravarLog(EventoLog.ChamadaServico, new
                            {
                                TipoVenda = ddlTipoVenda.SelectedValue,
                                IdPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeRegistroBuffer,
                                codEstabelecimento,
                                dataInicio,
                                dataFim,
                                sistemaOrigem,
                                transacao,
                                processo
                            });

                            //recuperando os requests pendentes
                            XBChargebackServico.Comprovante[] requests = client.ConsultaSolicitacaoPendente(
                                out quantidadeTotalRegistrosEmCache,
                                out codigoRetorno,
                                IdPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeRegistroBuffer,
                                codEstabelecimento,
                                dataInicio,
                                dataFim,
                                sistemaOrigem,
                                transacao,
                                processo);

                            List<DocMotivoChargeback> docsMotivoChargeback = this.ListDocsMotivoChargeback();
                            if (docsMotivoChargeback != null && docsMotivoChargeback.Count > 0)
                            {
                                foreach (var comprovante in requests)
                                {
                                    var item = docsMotivoChargeback.FirstOrDefault(x => x.CodMotivo == comprovante.CodigoMotivoProcesso);
                                    if (item != null)
                                    {
                                        comprovante.Motivo = String.Format("{0}{1}{1}{2}", comprovante.Motivo, Environment.NewLine, item.Documentos);
                                    }
                                }
                            }

                            Log.GravarLog(EventoLog.RetornoServico, new
                            {
                                quantidadeTotalRegistrosEmCache,
                                codigoRetorno,
                                requests
                            });

                            //caso o código de retorno seja != 0 ocorreu um erro.
                            //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                            if (codigoRetorno > 0 && codigoRetorno != 10 && codigoRetorno != 53)
                                base.ExibirPainelExcecao("XBChargebackServico.ConsultarDebitoPendente", codigoRetorno);
                            else
                                retorno = requests;
                        }

                        //Se não retornou registros, exibe mensagem adequada
                        Boolean exibirComprovantes = retorno != null && retorno.Count() > 0 && quantidadeTotalRegistrosEmCache > 0;
                        ExibirComprovantes(exibirComprovantes, !exibirComprovantes);
                    }
                    #endregion
                    else if (ddlTipoVenda.SelectedValue == "Selecione")
                    {
                        Log.GravarMensagem("Nenhum tipo de venda selecionado");
                        ExibirComprovantes(false, false);
                    }
                }
                catch (FaultException<XBChargebackServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    ExibirComprovantes(false, false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    ExibirComprovantes(false, false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                return retorno;
            }
        }

        #endregion


        #region Handlers de eventos


        /// <summary>
        /// Handler do DropDown que seleciona entre Crédito / Débito
        /// </summary>
        protected void ddlTipoVenda_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.paginacao.Carregar();
            //this.paginacaoOld.Carregar();
        }

        /// <summary>
        /// Apresenta as informações no asp:repeater.
        /// </summary>
        protected void rptPendentes_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    // Recupera objetos da linha
                    var lblProcesso = (Label)e.Item.FindControl("lblProcesso");
                    var lblResumoVendas = (Label)e.Item.FindControl("lblResumoVendas");
                    var lblCentralizadora = (Label)e.Item.FindControl("lblCentralizadora");
                    var lblNSU = (Label)e.Item.FindControl("lblNSU");
                    //var btnMotivo = (ImageButton)e.Item.FindControl("btnMotivo");
                    var btnInformacao = (Controles.BotaoInformacao)e.Item.FindControl("btnInformacao");
                    var lblData = (Label)e.Item.FindControl("lblData");
                    var lblValorVenda = (Label)e.Item.FindControl("lblValorVenda");
                    var lblEnvioNotificacao = (Label)e.Item.FindControl("lblEnvioNotificacao");
                    var lblDocEnviado = (Label)e.Item.FindControl("lblDocEnviado");
                    var lblQualidade = (Label)e.Item.FindControl("lblQualidade");
                    var lblPrazo = (Label)e.Item.FindControl("lblPrazo");
                    var btnEnviar = (HyperLink)e.Item.FindControl("btnEnviar");

                    QueryStringSegura queryString = new QueryStringSegura();

                    //Crédito
                    if (ddlTipoVenda.SelectedValue == "Credito")
                    {
                        //converte a linha num objeto request
                        XBChargebackServico.Comprovante request = (XBChargebackServico.Comprovante)e.Item.DataItem;

                        //setando os valores corretos nas labels das linhas
                        lblProcesso.Text = request.Processo.ToString();
                        lblResumoVendas.Text = request.ResumoVenda.ToString();
                        lblCentralizadora.Text = request.Centralizadora.ToString() + "<br/>" + request.PontoVenda.ToString();
                        lblNSU.Text = request.FlagNSUCartao == 'C'
                            ? (request.NumeroCartao)
                            : (request.NumeroCartao.ToDecimal() > 0 ? request.NumeroCartao : "-");
                        lblData.Text = request.DataVenda.ToString("dd/MM/yy");
                        lblValorVenda.Text = request.ValorVenda.ToString("N2"); ;

                        lblDocEnviado.Text = request.SolicitacaoAtendida ? "sim" : "não";
                        //lblDocEnviado.ForeColor = request.SolicitacaoAtendida ? Color.Green : Color.Red;

                        if (request.CanalEnvio.HasValue)
                            lblEnvioNotificacao.Text = ObtemDescricaoCanalEnvio((Int32?)request.CanalEnvio, request.DescricaoCanalEnvio);

                        if (request.DataEnvio.HasValue)
                            lblEnvioNotificacao.Text += String.Format(" {0}", request.DataEnvio.Value.ToString("dd/MM/yy"));

                        lblQualidade.Text = request.QualidadeRecebimentoDocumentos;
                        if (request.DataLimiteEnvioDocumentos.HasValue)
                            lblPrazo.Text = request.DataLimiteEnvioDocumentos.Value.ToString("dd/MM/yy");
                        else
                            lblPrazo.Text = string.Empty;

                        //btnMotivo.Attributes.Add("mensagem", HttpUtility.HtmlEncode(request.Motivo));
                        //btnMotivo.Attributes.Add("titulo", "MOTIVO DO PROCESSO");
                        btnInformacao.Mensagem = request.Motivo;
                        btnInformacao.Titulo = "motivo do processo";

                        queryString["flgNSU"] = request.FlagNSUCartao == 'N' ? "N" : "C";
                        queryString["Referencia"] = request.NumeroReferencia;
                        queryString["NumProcesso"] = request.Processo.ToString();
                        queryString["Estabelecimento"] = request.Centralizadora.ToString();
                        queryString["ResumoVendas"] = request.ResumoVenda.ToString();
                        queryString["NumCartao"] = lblNSU.Text;
                        queryString["DataVenda"] = lblData.Text;
                        queryString["ValorVenda"] = lblValorVenda.Text;
                    }
                    //Débito
                    else
                    {
                        //Conversao da linha em objeto request
                        XBChargebackServico.Comprovante request = (XBChargebackServico.Comprovante)e.Item.DataItem;

                        //setando os valores corretos nas labels das linhas
                        lblProcesso.Text = request.Processo.ToString();
                        lblResumoVendas.Text = request.ResumoVenda.ToString();
                        lblCentralizadora.Text = request.Centralizadora.ToString();
                        // Sempre retorna o número de referência, pois a chamada ao programa XSB419 não possui flagNSUCartao.
                        lblNSU.Text = request.NumeroReferencia;
                        lblData.Text = request.DataVenda.ToString("dd/MM/yy");
                        lblValorVenda.Text = request.ValorVenda.ToString("N2");

                        String canalEnvio = ObtemDescricaoCanalEnvio((Int32?)request.CanalEnvio, request.DescricaoCanalEnvio);
                        String dataEnvio = request.DataEnvio.HasValue && request.DataEnvio.Value > DateTime.MinValue ? request.DataEnvio.Value.ToString("dd/MM/yy") : "";
                        lblEnvioNotificacao.Text = String.Format("{0} {1}", canalEnvio, dataEnvio);
                        lblDocEnviado.Text = request.SolicitacaoAtendida ? "sim" : "não";
                        //lblDocEnviado.ForeColor = request.SolicitacaoAtendida ? Color.Green : Color.Red;

                        lblQualidade.Text = request.QualidadeRecebimentoDocumentos;
                        if (request.DataLimiteEnvioDocumentos.HasValue)
                            lblPrazo.Text = request.DataLimiteEnvioDocumentos.Value.ToString("dd/MM/yy");
                        else
                            lblPrazo.Text = string.Empty;

                        //btnMotivo.Attributes.Add("mensagem", HttpUtility.HtmlEncode(request.Motivo));
                        //btnMotivo.Attributes.Add("titulo", "MOTIVO DO PROCESSO");
                        btnInformacao.Mensagem = HttpUtility.HtmlEncode(request.Motivo).Replace(Environment.NewLine, "<br />");
                        btnInformacao.Titulo = "motivo do processo";
                        btnInformacao.Attributes["data-cod-mot"] = request.CodigoMotivoProcesso.ToString();

                        queryString["flgNSU"] = request.FlagNSUCartao == 'N' ? "N" : "C";
                        queryString["Referencia"] = request.NumeroReferencia;
                        queryString["NumProcesso"] = request.Processo.ToString();
                        queryString["Estabelecimento"] = request.Centralizadora.ToString();
                        queryString["ResumoVendas"] = request.ResumoVenda.ToString();
                        queryString["NumCartao"] = lblNSU.Text;
                        queryString["DataVenda"] = lblData.Text;
                        queryString["ValorVenda"] = lblValorVenda.Text;
                    }

                    queryString["TipoVenda"] = ddlTipoVenda.SelectedValue;

                    //ajusta a querystring e o link do botão enviar                
                    btnEnviar.NavigateUrl = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_EnvioComprovantes.aspx?dados={0}", queryString.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante DataBind de dados de Comprovantes Pendentes", ex);
                ExibirComprovantes(false, false);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        private String ObtemDescricaoCanalEnvio(Int32? canalEnvio, String descricaoPadrao)
        {
            switch (canalEnvio)
            {
                case 1: return "correio";
                case 2: return "fax automático";
                case 3: return "não envia";
                case 4: return "internet";
                case 5: return "e-mail";
                case 6: return "arquivo e transmissão de request de chargeback";
                case 7: return "extrato eletrônico";
                default: return descricaoPadrao.EmptyToNull() ?? "";
            }
        }

        #endregion

        /// <summary>Handler do botão voltar, redireciona para a tela de Comprovantes Pendentes</summary>        
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect(base.web.ServerRelativeUrl + "/Paginas/pn_ComprovacaoVendas.aspx");
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            this.paginacao.Carregar();
            //this.paginacaoOld.Carregar();
        }

        /// <sumarry>
        /// Lista de documentos por motivo de chargeback
        /// </summary>
        private List<DocMotivoChargeback> ListDocsMotivoChargeback()
        {
            // verifica se a lista já foi consultada
            if (Session["ListDocsMotivoChargeback"] == null)
            {
                try
                {
                    SPList lista = null;
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                        using (SPWeb spWeb = spSite.RootWeb)
                            lista = spWeb.Lists.TryGetList("Dicionario");
                    });

                    if (lista == null)
                    {
                        Logger.GravarErro("Lista 'Dicionario' encontrada");
                        SharePointUlsLog.LogErro("Lista 'Dicionario' encontrada");
                        return null;
                    }

                    SPQuery query = new SPQuery
                    {
                        Query = @"
<Where>
    <Eq>
        <FieldRef Name=""Categoria"" />
        <Value Type=""Text"">CHARGEBACK_DOC_MOT</Value>
    </Eq>
</Where>"
                    };

                    SPListItemCollection spList = lista.GetItems(query);
                    if (spList.Count == 0)
                    {
                        String logErro = String.Format("Nenhum registro retornado na busca; query executada: {0}", query.Query);
                        Logger.GravarErro(logErro);
                        SharePointUlsLog.LogErro(logErro);
                        return null;
                    }

                    List<DocMotivoChargeback> listDocs = new List<DocMotivoChargeback>();
                    foreach (var item in spList.Cast<SPListItem>())
                    {
                        String chave = Convert.ToString(item["Chave"]);
                        String valor = Convert.ToString(item["Valor"]);

                        DocMotivoChargeback docItem = new DocMotivoChargeback
                        {
                            CodMotivo = chave.ToInt16Null(0).Value,
                            Documentos = valor.ToString()
                        };

                        // verifica se o código foi convertido corretamente
                        if (docItem.CodMotivo == 0)
                        {
                            String logErro = String.Format("Erro ao converter o item: {0}; descrição: {1}", chave, valor);
                            Logger.GravarErro(logErro);
                            SharePointUlsLog.LogErro(logErro);
                            continue;
                        }

                        listDocs.Add(docItem);
                    }

                    Session["ListDocsMotivoChargeback"] = listDocs;
                }
                catch (WebException ex)
                {
                    Logger.GravarErro("Erro ao consultar a API de dicionário", ex);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro ao consultar a API de dicionário", ex);
                    SharePointUlsLog.LogErro(ex);
                }
            }

            return (List<DocMotivoChargeback>)Session["ListDocsMotivoChargeback"];
        }
    }
}
