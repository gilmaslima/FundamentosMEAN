using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.WebParts.ConsultarVendas.ConsultarVendasListagem;
using System;
using System.Linq;
using System.Web.UI;
using System.Collections.Generic;

namespace Redecard.PN.Extrato.SharePoint.WebParts.ConsultarVendas.ConsultarVendasFiltro
{
    /// <summary>
    /// Controle de filtro da tela de consulta de vendas
    /// </summary>
    public partial class ConsultarVendasFiltroUserControl : BaseUserControl
    {
        #region Controles filhos

        /// <summary>
        /// Controle ConsultaPV
        /// </summary>
        public ConsultaPv ConsultaPv
        {
            get
            {
                return (ConsultaPv)this.consultaPv;
            }
        }

        /// <summary>
        /// Controle de listagem/resultado
        /// </summary>
        public ConsultarVendasListagemUserControl ResultadoControl
        {
            get
            {
                return (ConsultarVendasListagemUserControl)this.resultadoControl;
            }
        }

        #endregion

        /// <summary>
        /// Sobrescrita do método OnLoad do BaseUserControl
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // impede a validação da permissão no controle
            this.ValidarPermissao = false;

            base.OnLoad(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // tratamento para a página se estiver em modo edição
            if (SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                return;

            //se leu algum parâmetro da query string e carregou no filtro
            Boolean leuQueryString = false;

            //se não é postback, faz leitura da querystring
            if (!IsPostBack)
            {
                leuQueryString = VerificarQueryString();
            }

            using (var log = Logger.IniciarLog("ConsultarVendasFiltroUserControl - Page Load"))
            {
                try
                {
                    ConsultarVendasListagemUserControl.TipoBusca tipoBusca = 
                        (ConsultarVendasListagemUserControl.TipoBusca)this.ddlTipoBusca.SelectedValue.ToInt32Null(0);

                    #region Visibilidade dos campos de data em função do tipo de busca

                    // busca por resumo de vendas pede somente uma data (single)
                    if (tipoBusca == ConsultarVendasListagemUserControl.TipoBusca.ResumoVendas)
                    {
                        this.pnlDataRange.Style["display"] = "none";
                        this.pnlDataSingle.Style["display"] = "";
                    }
                    else
                    {
                        this.pnlDataSingle.Style["display"] = "none";
                        this.pnlDataRange.Style["display"] = "";
                    }

                    // busca por TID não precisa do campo de data
                    if (tipoBusca == ConsultarVendasListagemUserControl.TipoBusca.TID)
                    {
                        this.pnlDataRange.Style["visibility"] = "hidden";
                        this.pnlDataSingle.Style["visibility"] = "hidden";
                    }
                    else
                    {
                        this.pnlDataRange.Style["visibility"] = "visible";
                        this.pnlDataSingle.Style["visibility"] = "visible";
                    }

                    #endregion

                    if (!IsPostBack && !leuQueryString)
                    {
                        this.ResultadoControl.Visible = false;
                        this.ConsultaPv.DropDownList.SelectedValue = "0";
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// verifica a querystring 
        /// </summary>
        /// <return>Se leu algum parâmetro da QueryString
        /// </return>
        private Boolean VerificarQueryString()
        {
            //leitura dos parâmetros via query string
            Int32? tipoEstabelecimento = Request["tipoEstabelecimento"].ToInt32();
            Int32? tipoBusca = Request["tipoBusca"].ToInt32Null();
            Int64? numero = Request["numero"].ToInt64Null();
            DateTime? dataInicial = Request["de"].ToDateTimeNull("dd/MM/yyyy");
            DateTime? dataFinal = Request["ate"].ToDateTimeNull("dd/MM/yyyy");
            Int32? tipoVenda = Request["tipoVenda"].ToInt32Null();
            Int32? modoSelecao = Request["modoSelecao"].ToInt32Null();
            Boolean pesquisar = Request.QueryString.AllKeys.Contains("pesquisar");

            //preenchimento de controles caso parâmetros foram passados e sejam válidos
            if (tipoEstabelecimento.HasValue)
            {
                consultaPv.TipoAssociacao = (ConsultaPvTipoAssociacao)tipoEstabelecimento.Value;
                if (consultaPv.TipoAssociacao == ConsultaPvTipoAssociacao.Proprio)
                {
                    consultaPv.PVsSelecionados = new List<Int32>() { SessaoAtual.CodigoEntidade };
                }
            }
            if (modoSelecao.HasValue)
                consultaPv.ModoSelecao = (ConsultaPvModo)modoSelecao.Value;
            if(tipoBusca.HasValue)
                ddlTipoBusca.SelectedValue = tipoBusca.Value.ToString();
            if(numero.HasValue)
                txtNumero.Text = numero.Value.ToString();
            if(dataInicial.HasValue)
                txtDataInicial.Text = dataInicial.Value.ToString("dd/MM/yyyy");
            if(dataFinal.HasValue)
                txtDataFinal.Text = dataFinal.Value.ToString("dd/MM/yyyy");
            if(tipoVenda.HasValue)
                ddlTipoVenda.SelectedValue = tipoVenda.Value.ToString();

            //executa pesquisa se solicitado via query string
            if (pesquisar)
                Pesquisar();

            //retorna booleano indicando se leu algum valor da query string
            return 
                tipoEstabelecimento.HasValue ||
                tipoBusca.HasValue ||
                numero.HasValue ||
                dataInicial.HasValue ||
                dataFinal.HasValue ||
                tipoVenda.HasValue ||
                modoSelecao.HasValue || 
                pesquisar;
        }

        /// <summary>
        /// Clique no botão Pesquisar
        /// </summary>
        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("ConsultarVendasFiltroUserControl - btnPesquisar_Click"))
            {
                try
                {
                    Pesquisar();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// executa pesquisa, validando campos
        /// </summary>
        protected void Pesquisar()
        {
            if (!this.ValidarCampos())
                return;

            // tipo de busca
            ConsultarVendasListagemUserControl.TipoBusca tipoBusca =
                (ConsultarVendasListagemUserControl.TipoBusca)this.ddlTipoBusca.SelectedValue.ToInt32Null(0).Value;

            // tipo de venda
            ConsultarVendasListagemUserControl.TipoVenda tipoVenda =
                (ConsultarVendasListagemUserControl.TipoVenda)this.ddlTipoVenda.SelectedValue.ToInt32Null(0).Value;

            // valor correspondente ao tipo de busca
            String numero = this.txtNumero.Text;

            // datas inicial e final
            DateTime? dataInicial = this.txtDataInicial.Text.ToDateTimeNull();
            DateTime? dataFinal = this.txtDataFinal.Text.ToDateTimeNull();

            // data de busca específica para resumo de vendas
            if (tipoBusca == ConsultarVendasListagemUserControl.TipoBusca.ResumoVendas || !dataInicial.HasValue)
                dataInicial = this.txtDataBusca.Text.ToDateTimeNull();

            // pv selecionado
            Int32 pvSelecionado = 0;
            if (this.ConsultaPv.PVsSelecionados != null && this.ConsultaPv.PVsSelecionados.Count > 0)
                pvSelecionado = this.ConsultaPv.PVsSelecionados[0];

            // faz a busca com os dados informados
            this.ResultadoControl.ConsultarVendas(tipoBusca, tipoVenda, numero, dataInicial, dataFinal, pvSelecionado);
            this.ResultadoControl.Visible = true;
        }

        /// <summary>
        /// Método para validar os campos em server side
        /// </summary>
        /// <returns></returns>
        private bool ValidarCampos()
        {
            this.lblMensagem.Text = string.Empty;

            // valida tipo de busca
            if (this.ddlTipoBusca.SelectedValue.ToInt32Null(0) == 0)
            {
                this.lblMensagem.Text = this.ddlTipoBusca.Attributes["data-msg-erro"];
                return false;
            }

            // valida número segundo o tipo de busca
            if (string.IsNullOrEmpty(this.txtNumero.Text))
            {
                this.lblMensagem.Text = this.txtNumero.Attributes["data-msg-erro"];
                return false;
            }

            // valida o tipo de venda
            if (this.ddlTipoVenda.SelectedValue.ToInt32Null(0) == 0)
            {
                this.lblMensagem.Text = this.ddlTipoVenda.Attributes["data-msg-erro"];
                return false;
            }

            // valida as datas informadas
            ConsultarVendasListagemUserControl.TipoBusca tipoBusca = 
                (ConsultarVendasListagemUserControl.TipoBusca)this.ddlTipoBusca.SelectedValue.ToInt32Null(0);
            if (tipoBusca != ConsultarVendasListagemUserControl.TipoBusca.TID)
            {
                if (tipoBusca == ConsultarVendasListagemUserControl.TipoBusca.ResumoVendas)
                {
                    if (string.IsNullOrEmpty(this.txtDataBusca.Text))
                    {
                        this.lblMensagem.Text = this.txtDataBusca.Attributes["data-msg-erro"];
                        return false;
                    }
                }
                else if (string.IsNullOrEmpty(this.txtDataInicial.Text))
                {
                    this.lblMensagem.Text = this.txtDataInicial.Attributes["data-msg-erro"];
                    return false;
                }
                else if (string.IsNullOrEmpty(this.txtDataFinal.Text))
                {
                    this.lblMensagem.Text = this.txtDataFinal.Attributes["data-msg-erro"];
                    return false;
                }
            }

            return true;
        }
    }
}
