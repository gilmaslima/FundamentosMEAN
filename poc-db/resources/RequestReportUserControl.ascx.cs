using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.Request.Core.Web.Controles.Portal;
using Redecard.PN.Request.SharePoint.Business;
using Redecard.PN.Request.SharePoint.Model;
using Redecard.PN.Request.SharePoint.XBChargebackServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.Request.SharePoint.ControlTemplates.Redecard.PN.Request.SharePoint
{
    public partial class RequestReportUserControl : UserControlBase
    {
        #region [ Propriedades e Atributos ]

        /// <summary>
        /// QueryStringSegura para leitura dos dados informados na query string
        /// </summary>
        private QueryStringSegura QS
        {
            get
            {
                if (Request.QueryString["dados"] != null)
                    return new QueryStringSegura(Request.QueryString["dados"]);
                else
                    return null;
            }
        }

        /// <summary>
        /// Dados do filtro inseridos em tela
        /// </summary>
        private ComprovanteServiceRequest DadosFiltro
        {
            get
            {
                if (this.dadosFiltroTemp == null)
                {
                    this.dadosFiltroTemp = new ComprovanteServiceRequest();

                    var tipoVenda = TipoVendaComprovante.Nenhum;
                    Enum.TryParse(ddlTipoVenda.Text, out tipoVenda);
                    this.dadosFiltroTemp.TipoVenda = tipoVenda;

                    var status = StatusComprovante.Todos;
                    Enum.TryParse(ddlStatus.Text, out status);
                    this.dadosFiltroTemp.Status = status;

                    this.dadosFiltroTemp.CodProcesso = txtNumeroProcesso.Text.ToDecimalNull();

                    this.dadosFiltroTemp.DataInicio = String.IsNullOrWhiteSpace(txtDtIniDatePicker.Text)
                        ? DateTime.Now.AddDays(-60)
                        : Convert.ToDateTime(txtDtIniDatePicker.Text);

                    this.dadosFiltroTemp.DataFim = String.IsNullOrWhiteSpace(txtDtFimDatePicker.Text)
                        ? DateTime.Now
                        : Convert.ToDateTime(txtDtFimDatePicker.Text);

                }
                return this.dadosFiltroTemp;
            }
        }
        private ComprovanteServiceRequest dadosFiltroTemp;

        /// <summary>
        /// Ids para as pesquisas armazenadas em cache o lado servidor
        /// </summary>
        public List<ComprovanteRequestIdPesquisa> ListPesquisaId
        {
            get
            {
                if (ViewState["IdPesquisa"] == null)
                {
                    ViewState["IdPesquisa"] = new List<ComprovanteRequestIdPesquisa>()
                    {
                        new ComprovanteRequestIdPesquisa {
                            Status = StatusComprovante.Historico,
                            TipoVenda = TipoVendaComprovante.Credito,
                            IdPesquisa = Guid.NewGuid()
                        },
                        new ComprovanteRequestIdPesquisa {
                            Status = StatusComprovante.Historico,
                            TipoVenda = TipoVendaComprovante.Debito,
                            IdPesquisa = Guid.NewGuid()
                        },
                        new ComprovanteRequestIdPesquisa {
                            Status = StatusComprovante.Pendente,
                            TipoVenda = TipoVendaComprovante.Credito,
                            IdPesquisa = Guid.NewGuid()
                        },
                        new ComprovanteRequestIdPesquisa {
                            Status = StatusComprovante.Pendente,
                            TipoVenda = TipoVendaComprovante.Debito,
                            IdPesquisa = Guid.NewGuid()
                        }
                    };
                }

                return (List<ComprovanteRequestIdPesquisa>)ViewState["IdPesquisa"];
            }
            set
            {
                ViewState["IdPesquisa"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //Definição do botão default da página
            this.Page.Form.DefaultButton = btnBuscar.UniqueID;

            //Datas não podem ser posteriores à data atual
            if (!Page.IsPostBack)
            {
                //Veio da tela Avisos de Débito se possuir QueryString
                if (QS != null)
                {
                    //Define que a origem foi a tela Avisos de Débito                    
                    Decimal codProcesso = QS["NumProcesso"].ToDecimal();
                    String tipoVenda = QS["TipoVenda"];

                    //Atualiza os campos de filtro com os dados passados pela querystring
                    ddlTipoVenda.SelectedValue = tipoVenda;
                    txtNumeroProcesso.Text = codProcesso.ToString();

                    //Atualiza os dados do repeater, informando que a 
                    CarregarComprovantes("DEBITO");
                }
                else
                {
                    VerificarQueryString();
                }
            }

            if (this.SessaoAtual != null)
            {
                // dados do usuário no header do bloco de impressão
                this.lblHeaderImpressaoUsuario.Text = string.Concat(SessaoAtual.CodigoEntidade, " / ", SessaoAtual.LoginUsuario);
            }
        }

        #region [ Handlers de eventos ]

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
                    var btnInformacao = (BotaoInformacao)e.Item.FindControl("btnInformacao");
                    var ltlDataVenda = (Literal)e.Item.FindControl("ltlDataVenda");
                    var ltlValorVenda = (Literal)e.Item.FindControl("ltlValorVenda");
                    var ltlEnvioNotificacao = (Literal)e.Item.FindControl("ltlEnvioNotificacao");
                    var ltlDocEnviado = (Literal)e.Item.FindControl("ltlDocEnviado");
                    var ltlQualidadeDoc = (Literal)e.Item.FindControl("ltlQualidadeDoc");
                    var ltlPrazo = (Literal)e.Item.FindControl("ltlPrazo");

                    // links de ação
                    var phActionPendente = (PlaceHolder)e.Item.FindControl("phActionPendente");
                    var btnEnviar = (HyperLink)e.Item.FindControl("btnEnviar");     // pendente
                    var btnDebito = (HyperLink)e.Item.FindControl("btnDebito");     // histórico

                    ComprovanteModel model = e.Item.DataItem as ComprovanteModel;

                    // numero do processo
                    ltlNumProcesso.Text = model.Processo.ToString();

                    // resumo de vendas
                    ltlNumResumo.Text = model.ResumoVenda.ToString();

                    // centralizadora
                    if (model.Status == StatusComprovante.Pendente && model.TipoVenda == TipoVendaComprovante.Credito)
                    {
                        ltlCentralizadora.Text = String.Format(
                            "{0}<br />{1}",
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
                    btnInformacao.Mensagem = HttpUtility.HtmlEncode(model.Motivo).Replace(Environment.NewLine, "<br />");
                    
                    // link para a página pn_AvisoDebito.aspx
                    btnDebito.Visible = 
                        model.Status == StatusComprovante.Historico && 
                        (model.IndicadorDebito ?? false);
                    if (btnDebito.Visible)
                    {
                        QueryStringSegura qsAvisoDebito = new QueryStringSegura();
                        qsAvisoDebito["TipoVenda"] = ddlTipoVenda.SelectedValue;
                        qsAvisoDebito["NumProcesso"] = model.Processo.ToString();
                        btnDebito.NavigateUrl = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_AvisoDebito.aspx?dados={0}", qsAvisoDebito.ToString());
                    }

                    // ajusta a querystring e o link do botão enviar                
                    btnEnviar.Visible = 
                        model.Status == StatusComprovante.Pendente &&
                        !(SessaoAtual != null && SessaoAtual.UsuarioAtendimento);
                    if (btnEnviar.Visible)
                    {
                        QueryStringSegura qsEnviarComprovante = new QueryStringSegura();
                        qsEnviarComprovante["TipoVenda"] = ddlTipoVenda.SelectedValue;
                        qsEnviarComprovante["flgNSU"] = model.FlagNSUCartao == 'N' ? "N" : "C";
                        qsEnviarComprovante["Referencia"] = model.NumeroReferencia;
                        qsEnviarComprovante["NumProcesso"] = model.Processo.ToString();
                        qsEnviarComprovante["Estabelecimento"] = model.Centralizadora.ToString();
                        qsEnviarComprovante["ResumoVendas"] = model.ResumoVenda.ToString();
                        qsEnviarComprovante["NumCartao"] = ltlNumCartao.Text;
                        qsEnviarComprovante["DataVenda"] = ltlDataVenda.Text;
                        qsEnviarComprovante["ValorVenda"] = ltlValorVenda.Text;
                        btnEnviar.NavigateUrl = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_EnvioComprovantes.aspx?dados={0}", qsEnviarComprovante.ToString());
                    }
                }
            }
            catch (FormatException ex)
            {
                Logger.GravarErro("Erro de FormatException durante DataBind de dados de Histórico de Comprovantes", ex);
                ExibirComprovantes(false, false);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.GravarErro("Erro de ArgumentOutOfRangeException durante DataBind de dados de Histórico de Comprovantes", ex);
                ExibirComprovantes(false, false);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (NullReferenceException ex)
            {
                Logger.GravarErro("Erro de NulReferenceException durante DataBind de dados de Histórico de Comprovantes", ex);
                ExibirComprovantes(false, false);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro genérico durante DataBind de dados de Histórico de Comprovantes", ex);
                ExibirComprovantes(false, false);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Handler do botão que aciona a busca e exibe o grid de resultados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            this.ListPesquisaId = null;
            Buscar();
        }

        /// <summary>
        /// Handler do botão voltar, redireciona para a tela de Comprovantes Pendentes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect(base.web.ServerRelativeUrl + "/Paginas/pn_ComprovacaoVendas.aspx");
        }

        /// <summary>
        /// Evento para obtenção dos dados do componente de paginação
        /// </summary>
        /// <param name="idPesquisa"></param>
        /// <param name="registroInicial"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="quantidadeRegistroBuffer"></param>
        /// <param name="quantidadeTotalRegistrosEmCache"></param>
        /// <param name="parametros"></param>
        /// <returns></returns>
        protected IEnumerable<Object> Paginacao_ObterDados(
            Guid idPesquisa, 
            int registroInicial, 
            int quantidadeRegistros, 
            int quantidadeRegistroBuffer, 
            out int quantidadeTotalRegistrosEmCache, 
            params object[] parametros)
        {
            quantidadeTotalRegistrosEmCache = 0;

            // obtém os dados de filtro e complementa para requisição ao WCF
            ComprovanteServiceRequest requestData = this.DadosFiltro;
            requestData.SessaoUsuario = SessaoAtual;
            requestData.QuantidadeRegistros = quantidadeRegistros;
            requestData.QuantidadeRegistroBuffer = quantidadeRegistroBuffer;
            requestData.Parametros = parametros;
            requestData.RegistroInicial = registroInicial;

            String msgLog = String.Format(
                "RequestReport - {0} ({1}-{2})",
                ddlTipoVenda.SelectedValue,
                registroInicial,
                registroInicial + quantidadeRegistros);

            using (Logger Log = Logger.IniciarLog(msgLog))
            {                
                try
                {
                    var response = ComprovantesService.Consultar(requestData, this.ListPesquisaId);

                    List<DocMotivoChargeback> docsMotivoChargeback = this.ListDocsMotivoChargeback();
                    if (docsMotivoChargeback != null && docsMotivoChargeback.Count > 0)
                    {
                        foreach (var comprovante in response.Comprovantes)
                        {
                            var item = docsMotivoChargeback.FirstOrDefault(x => x.CodMotivo == comprovante.CodigoMotivoProcesso);
                            if (item != null)
                            {
                                comprovante.Motivo = String.Format("{0}{1}{1}{2}", comprovante.Motivo, Environment.NewLine, item.Documentos);
                            }
                        }
                    }

                    // se não retornou registros, exibe mensagem adequada
                    Boolean exibirComprovantes = 
                        response.Comprovantes.Count > 0 && 
                        response.QuantidadeTotalRegistrosEmCache > 0;
                    this.ExibirComprovantes(exibirComprovantes, !exibirComprovantes);

                    quantidadeTotalRegistrosEmCache = response.QuantidadeTotalRegistrosEmCache;

                    return response.Comprovantes;
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirComprovantes(false, false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirComprovantes(false, false);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                return null;
            }
        }

        /// <summary>
        /// Evento de click para download em excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDownloadExcel_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Histórico de Comprovantes - Gerar Excel"))
            {
                try
                {
                    String urlComprovantesImpressao = String.Format(                        
                        "{0}/_layouts/Request/ComprovantesImpressao.aspx?dados={1}&excel={2}",
                        SPContext.Current.Web.Url,
                        this.GetFilterQueryString(this.DadosFiltro),
                        true.ToString());
                    Response.Redirect(urlComprovantesImpressao, false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Consultar Cancelamento - Gerar Excel", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Consultar Cancelamento - Gerar Excel", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento de validação do campo de período
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void PeriodoServerValidate(object source, ServerValidateEventArgs args)
        {
            //verifica se o período informado está dentro dos 60 dias
            TimeSpan dtDiff = DateTime.Now.Subtract(this.DadosFiltro.DataFim.Value);

            // a data final não pode ser maior que hoje
            if (this.DadosFiltro.Status == StatusComprovante.Historico)
            {
                if (dtDiff.TotalDays < 0)
                {
                    base.ExibirPainelMensagem("o período não deve conter data futura");
                    args.IsValid = false;
                    return;
                }
            }

            // calcula a diferença de dias
            dtDiff = this.DadosFiltro.DataFim.Value.Subtract(this.DadosFiltro.DataInicio.Value);

            // a diferença não pode exceder 60 dias
            if (dtDiff.TotalDays > 60)
            {
                base.ExibirPainelMensagem("a consulta não deve ultrapassar 60 dias");
                args.IsValid = false;
                return;
            }

            // a data final não pode ser menor que a data inicial
            if (dtDiff.TotalDays < 0)
            {
                base.ExibirPainelMensagem("período inválido: a data final não pode ser menor que a data inicial");
                args.IsValid = false;
                return;
            }

            args.IsValid = true;
        }

        #endregion

        #region [ Métodos privados ]

        /// <summary>
        /// Define a visibilidade do grid e do painel de mensagem
        /// </summary>
        /// <param name="exibirGrid">Define se deve exibir o grid</param>
        /// <param name="exibirMensagem">Define se deve exibir o painel de mensagem</param>
        private void ExibirComprovantes(Boolean exibirGrid, Boolean exibirMensagem)
        {
            rptComprovantes.Visible = exibirGrid;
            pnlSemComprovante.Visible = exibirMensagem;
            qdAvisoSemComprovante.TipoQuadro = TipoQuadroAviso.Aviso;

            // exibe o menu de ações
            mnuAcoes.Visible = exibirGrid;

            if (exibirMensagem)
                qdAvisoSemComprovante.Mensagem = "não foram encontrados comprovantes.";
        }

        /// <summary>
        /// Carrega o repeater carregando (paginado) os dados do mainframe
        /// </summary>
        /// <param name="pendentesOrigemConsulta">Origem da consulta: DEBITO (se proveniente da tela Aviso de Débito) ou REQUESTHISTORICO.</param>
        private void CarregarComprovantes(String pendentesOrigemConsulta)
        {
            // guarda os dados do filtro para extração do relatório
            hdnDadosFiltro.Value = this.GetFilterQueryString(this.DadosFiltro).ToString();

            // repeater visível
            rptComprovantes.Visible = true;

            // atualiza datasource do repeater  
            paginacao.Carregar(pendentesOrigemConsulta);
        }

        /// <summary>
        /// Valida os dados inseridos
        /// </summary>
        /// <returns></returns>
        private Boolean ValidateSubmit()
        {
            // se processo não foi informado, valida os campos de data
            // OBS: a data é utilizada apenas para históricos
            if (this.DadosFiltro.CodProcesso.HasValue || this.DadosFiltro.Status == StatusComprovante.Pendente)
                crvDataInicial.Enabled = crvDataFinal.Enabled = false;

            //Valida os campos digitados
            Page.Validate();
            var pageIsValid = Page.IsValid;

            // habilita os validadores de data
            crvDataInicial.Enabled = crvDataFinal.Enabled = true;

            return pageIsValid;
        }

        /// <summary>
        /// Obtém os dados de filtro em formato QueryStringSegura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private QueryStringSegura GetFilterQueryString(ComprovanteServiceRequest request)
        {
            QueryStringSegura data = new QueryStringSegura();

            if (request.DataInicio.HasValue)
                data["DataInicio"] = request.DataInicio.Value.ToString();

            if (request.DataFim.HasValue)
                data["DataFim"] = request.DataFim.Value.ToString();

            if (request.CodProcesso.HasValue)
                data["CodProcesso"] = request.CodProcesso.Value.ToString();

            data["Status"] = request.Status.ToString();
            data["TipoVenda"] = request.TipoVenda.ToString();

            Session["ListPesquisaIdImpressao"] = this.ListPesquisaId;

            return data;
        }

        /// <summary>
        /// verificação da query string, caso existam parâmetros
        /// informados via query string aberta, que podem ser:
        /// <para>"processo" (opcional): Int32, número do processo</para>
        /// <para>"tipoVenda" (opcional): String, tipo da venda ("Credito" ou "Debito")</para>
        /// <para>"de" (opcional): String, data início do período, formato dd/MM/yyyy</para>
        /// <para>"para" (opcional): String, data fim do período, formato dd/MM/yyyy</para>
        /// <para>"pesquisar" (opcional): apenas chave, se existir, executa pesquisa além de preencher os campos</para>
        /// </summary>
        private void VerificarQueryString()
        {
            //Recupera parâmetros provenientes de query string aberta
            Decimal? numeroProcesso = Request["processo"].ToDecimalNull();
            String tipoVenda = Request["tipoVenda"]; //"Credito" ou "Debito"
            DateTime? dataInicio = Request["de"].ToDateTimeNull("dd/MM/yyyy");
            DateTime? dataFim = Request["ate"].ToDateTimeNull("dd/MM/yyyy");
            Boolean pesquisar = Request.QueryString.AllKeys.Contains("pesquisar");

            //preenche controles caso parâmetros existam e sejam válidos
            if (numeroProcesso.HasValue)
                txtNumeroProcesso.Text = numeroProcesso.ToString();
            if (tipoVenda.EmptyToNull() != null)
                ddlTipoVenda.SelectedValue = tipoVenda;
            if (dataInicio.HasValue)
                txtDtIniDatePicker.Text = dataInicio.Value.ToString("dd/MM/yyyy");
            if (dataFim.HasValue)
                txtDtFimDatePicker.Text = dataFim.Value.ToString("dd/MM/yyyy");

            // indica a busca é de comprovantes históricos
            ddlStatus.SelectedValue = "1";

            //executa pesquisa se recebido parâmetro pesquisar
            if (pesquisar)
                Buscar();
        }

        /// <summary>
        /// Efetua a busca com os dados preenchidos no filtro
        /// </summary>
        private void Buscar()
        {
            if (this.ValidateSubmit())
            {
                //Carrega os dados da pesquisa
                CarregarComprovantes("REQUESTHISTORICO");
            }
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

        #endregion
    }
}
