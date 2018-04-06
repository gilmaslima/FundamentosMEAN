using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Collections.Generic;
using Redecard.PN.Outro.Core.Web.Controles.Portal;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.ContrateAqui
{
    public partial class ContrateAquiUserControl : UserControlBase
    {
        /// <summary>
        /// Código de versão do Regime de Franquia Atual
        /// </summary>
        private Int32 CodigoVersao
        {
            get
            {
                if (ViewState["CodigoVersao"] != null)
                    return (Int32)ViewState["CodigoVersao"].ToString().ToInt32Null(1);
                else
                    return 1;
            }
            set
            {
                ViewState["CodigoVersao"] = value;
            }
        }
        
        /// <summary>
        /// Código do regime atual
        /// </summary>
        private Int32 CodigoRegimeAtual
        {
            get
            {
                if (ViewState["CodigoRegimeAtual"] != null)
                    return (Int32)ViewState["CodigoRegimeAtual"].ToString().ToInt32Null(1);
                else
                    return 1;
            }
            set
            {
                ViewState["CodigoRegimeAtual"] = value;
            }
        }

        /// <summary>
        /// Identifica se o botão deve possuir label Alterar
        /// </summary>
        private Boolean BotaoAlterar
        {
            get
            {
                if (ViewState["BotaoAlterar"] == null)
                    ViewState["BotaoAlterar"] = false;

                return (Boolean)ViewState["BotaoAlterar"];
            }
            set
            {
                ViewState["BotaoAlterar"] = value;
            }
        }

        /// <summary>
        /// Inicialização da Página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CarregarRegimes())
                {
                    rptFranquias.Visible = !(pnlSemFranquias.Visible = true);
                    mnuAcoes.Visible = false;
                }
                else
                    mnuAcoes.Visible = true;
            }

            if (this.BotaoAlterar)
                btnContratar.Text = "alterar";
        }

        /// <summary>
        /// Preencher as informações de Franquia no Repeater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptFranquias_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Bind das informações de franquia"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        RadioButton rdTopicoRegime = (RadioButton)e.Item.FindControl("rdTopicoRegime");
                        HiddenField hdnCodigoRegime = (HiddenField)e.Item.FindControl("hdnCodigoRegime");
                        Label lblQtdMinimaConsulta = (Label)e.Item.FindControl("lblQtdMinimaConsulta");
                        Label lblValorFranquia = (Label)e.Item.FindControl("lblValorFranquia");
                        Label lblValorConsulta = (Label)e.Item.FindControl("lblValorConsulta");

                        Repeater rptListaRegime = (Repeater)e.Item.FindControl("rptListaRegime");
                        QuadroAviso qdAvisoSemRegimes = (QuadroAviso)e.Item.FindControl("qdAvisoSemRegimes");

                        ZPOutrosServicos.RegimeFranquia regime = (ZPOutrosServicos.RegimeFranquia)e.Item.DataItem;
                        if (regime != null)
                        {
                            if (rdTopicoRegime != null)
                            {
                                rdTopicoRegime.Text = String.Format(rdTopicoRegime.Text, regime.CodigoRegime);
                                rdTopicoRegime.Checked = (CodigoRegimeAtual == regime.CodigoRegime);

                                if (rdTopicoRegime.Checked)
                                    this.BotaoAlterar = true;
                            }
                            if (hdnCodigoRegime != null)
                                hdnCodigoRegime.Value = regime.CodigoRegime.ToString();

                            if (lblQtdMinimaConsulta != null)
                                lblQtdMinimaConsulta.Text = regime.QuantidadeConsulta.ToString();

                            if (lblValorFranquia != null)
                                lblValorFranquia.Text = String.Format("{0:N2}", regime.ValorFranquia);

                            if (lblValorConsulta != null)
                                lblValorConsulta.Text = String.Format("{0:N2}", regime.ValorConsulta);

                            if (rptListaRegime != null)
                            {
                                if (regime.FaixasConsultaFranquia != null)
                                {
                                    if (regime.FaixasConsultaFranquia.Length > 0)
                                    {
                                        rptListaRegime.DataSource = regime.FaixasConsultaFranquia;
                                        rptListaRegime.DataBind();
                                        rptListaRegime.Visible = !(qdAvisoSemRegimes.Visible = false);
                                    }
                                    else
                                    {
                                        rptListaRegime.Visible = !(qdAvisoSemRegimes.Visible = true);
                                        qdAvisoSemRegimes.TipoQuadro = TipoQuadroAviso.Aviso;
                                    }
                                }
                                else
                                {
                                    rptListaRegime.Visible = !(qdAvisoSemRegimes.Visible = true);
                                    qdAvisoSemRegimes.TipoQuadro = TipoQuadroAviso.Aviso;
                                }
                            }
                            else
                                base.ExibirPainelExcecao("SharePoint.ContrateAqui", 1442);
                        }
                        else
                            base.ExibirPainelExcecao("SharePoint.ContrateAqui", 1442);
                    }
                }
                catch (FaultException<ZPOutrosServicos.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Preenche as informações das Faixas de Regime da Franquia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptListaRegime_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Bind das Faixas de Regime da franquia"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        ZPOutrosServicos.FaixaConsultaFranquia faixaConsultas = (ZPOutrosServicos.FaixaConsultaFranquia)e.Item.DataItem;

                        Literal ltrInicioFaixa = (Literal)e.Item.FindControl("ltrInicioFaixa");
                        Literal ltrFinalFaixa = (Literal)e.Item.FindControl("ltrFinalFaixa");
                        Literal ltrValorConsulta = (Literal)e.Item.FindControl("ltrValorConsulta");

                        if (faixaConsultas != null)
                        {
                            if (ltrInicioFaixa != null)
                                ltrInicioFaixa.Text = faixaConsultas.FaixaInicial.ToString();

                            if (ltrFinalFaixa != null)
                                ltrFinalFaixa.Text = faixaConsultas.FaixaFinal.ToString();

                            if (ltrValorConsulta != null)
                                ltrValorConsulta.Text = String.Format("{0:C2}", faixaConsultas.ValorConsulta);

                        }
                    }
                }
                catch (FaultException<ZPOutrosServicos.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibi painel com mensagem
        /// </summary>
        private void ExibirPainelConfirmacao()
        {
            //pnlConsultaPlano
            Panel[] paineis = new Panel[1] { pnlContrateAqui };

            String urlQuadro = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_ConsultaCheques.aspx");

            base.ExibirPainelConfirmacaoAcao("Aviso", "não há franquias disponíveis", urlQuadro, paineis);
        }

        /// <summary>
        /// Carrega as informações dos Regimes através do Serviço do HIS
        /// </summary>
        private Boolean CarregarRegimes()
        {
            using (Logger Log = Logger.IniciarLog("Carregando regimes"))
            {
                //var regimeFranquiaCliente = new RegimeFranquiaServico.RegimeFranquiaServicoClient();
                //var restricao = regimeFranquiaCliente.ConsultarRestricaoRegime(SessaoAtual.CodigoEntidade, SessaoAtual.GrupoEntidade);
                try
                {
                    //Código de Serviço fixo em 1
                    Int16 codigoServico = 1;

                    //Erros codigoRetorno:
                    //  '00' - 0: RETORNO OK
                    //  'E1' - 1: ERRO DE PARAMETROS
                    //  'E2' - 2: ERRO DE ARQUIVOS
                    Int16 codigoRetorno = 0;

                    ZPOutrosServicos.RegimeFranquia[] regimes;
                    using (var servicoCliente = new ZPOutrosServicos.HISServicoZP_OutrosServicosClient())
                    {

                        codigoRetorno = servicoCliente.ListarRegime(out regimes, codigoServico);

                        if (codigoRetorno != 0)
                        {
                            this.ExibirPainelConfirmacao();
                            base.ExibirPainelExcecao("ZPOutrosServicos.ListarRegime", codigoRetorno);
                            return false;
                        }
                        else
                        {
                            if (SessaoAtual != null)
                            {
                                using (var regimeServico = new RegimeFranquiaServico.RegimeFranquiaServicoClient())
                                {
                                    int codRetorno;
                                    CodigoRegimeAtual = regimeServico.ConsultarCodigoRegime(out codRetorno, SessaoAtual.CodigoEntidade);

                                    var regimeAtual = regimeServico.ConsultarRestricaoRegime(SessaoAtual.CodigoEntidade, SessaoAtual.GrupoEntidade);

                                    CodigoVersao = 1;

                                    if (regimeAtual != null)
                                        CodigoVersao = regimeAtual.CodigoVersao;

                                    PreencherParametrosSemAceite();
                                    PreencherParametrosComAceite();

                                    if (regimes.Length > 0)
                                    {
                                        rptFranquias.DataSource = regimes;
                                        rptFranquias.DataBind();
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (FaultException<ZPOutrosServicos.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                return false;
            }
        }

        /// <summary>
        /// Botão que dispara a atualização de serviço
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAtualizarFranquia_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Atualizando franquia"))
            {
                try
                {
                    Int32 codigoRegime = 0;

                    if (rptFranquias.Items.Count > 0)
                    {
                        if (!object.ReferenceEquals(hdnCodigoRegimeSelecionado, null))
                        {
                            codigoRegime = (Int32)hdnCodigoRegimeSelecionado.Value.ToString().ToInt32Null(0);

                            if (AtualizarFranquia(CodigoVersao, codigoRegime))
                            {
                                PreencherParametrosComprovante();
                            }
                        }
                    }
                }
                catch (FaultException<RegimeFranquiaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Atualiza o Regime de Franquia da Entidade
        /// </summary>
        /// <returns>True - Atualizou sem erros.
        /// False - Atualizou com erros.</returns>
        private Boolean AtualizarFranquia(Int32 codigoVersao, Int32 codigoRegime)
        {
            using (Logger Log = Logger.IniciarLog("Atualizando franquia"))
            {
                try
                {
                    Int32 codigoCelula = 354;
                    Int32 codigoCanal = 15;
                    Int32 codigoRetorno = 0;

                    using (var regimeServico = new RegimeFranquiaServico.RegimeFranquiaServicoClient())
                    {
                        regimeServico.IncluirAceite(codigoVersao, SessaoAtual.GrupoEntidade, SessaoAtual.CodigoEntidade, SessaoAtual.LoginUsuario);

                        codigoRetorno = regimeServico.AtualizarFranquia(SessaoAtual.CodigoEntidade, codigoRegime, codigoCelula, codigoCanal, SessaoAtual.LoginUsuario);
                        if (codigoRetorno != 0)
                        {
                            this.ExibirPainelConfirmacao();
                            base.ExibirPainelExcecao("RegimeFranquiaServico.AtualizarFranquia", codigoRetorno);
                            return false;
                        }
                    }

                    //Registro no histórico/log de atividades
                    Historico.RealizacaoServico(SessaoAtual, "Contratação de Consulta de Cheque");

                    return true;
                }
                catch (FaultException<RegimeFranquiaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }

        /// <summary>
        /// Carrega URL do popup de Aceite do Contrato com Parâmetro com Query String Popup
        /// </summary>
        public void PreencherParametrosSemAceite()
        {
            String urlParametros = "";
            try
            {
                QueryStringSegura queryString = new QueryStringSegura();
                queryString["Aceite"] = "0";
                queryString["CodigoVersao"] = CodigoVersao.ToString();

                urlParametros = String.Format("version={0}&dados={1}",
                        System.Guid.NewGuid().ToString("N"), queryString.ToString());

                hdnParametrosSemAceite.Value = urlParametros;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Preenchimento dos parâmetros sem aceite", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                hdnParametrosSemAceite.Value = "";
            }
        }

        /// <summary>
        /// Carrega URL do popup de Aceite do Contrato com Parâmetro com Query String Popup
        /// </summary>
        public void PreencherParametrosComAceite()
        {
            String urlParametros = "";
            try
            {
                QueryStringSegura queryString = new QueryStringSegura();
                queryString["Aceite"] = "1";
                queryString["CodigoVersao"] = CodigoVersao.ToString();

                urlParametros = String.Format("version={0}&dados={1}",
                        System.Guid.NewGuid().ToString("N"), queryString.ToString());

                hdnParametrosComAceite.Value = urlParametros;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Preenchimento dos parâmetros com aceite", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                hdnParametrosComAceite.Value = "";
            }
        }

        /// <summary>
        /// Retorna URL do popup de Comprovante Serasa
        /// </summary>
        /// <returns>Parâmetros Popup de comprovante</returns>
        public void PreencherParametrosComprovante()
        {
            String urlParametros = "";
            try
            {
                Int32 codigoRegime = 0;

                if (rptFranquias.Items.Count > 0)
                {
                    if (!object.ReferenceEquals(hdnCodigoRegimeSelecionado, null))
                    {
                        codigoRegime = (Int32)hdnCodigoRegimeSelecionado.Value.ToString().ToInt32Null(0);
                    }
                }

                QueryStringSegura queryString = new QueryStringSegura();
                queryString["CodigoRegime"] = codigoRegime.ToString();

                urlParametros = String.Format("version={0}&dados={1}",
                        System.Guid.NewGuid().ToString("N"), queryString.ToString());

                String urlComprovante = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_SerasaComprovante.aspx?dados={0}", queryString.ToString());
                Response.Redirect(urlComprovante);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante preenchimento dos parâmetros do comprovante", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }
    }
}
