using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Extrato.SharePoint.WebParts.Relatorios;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Comum;
using Redecard.PN.Extrato;
using System.Web.Script.Serialization;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato;
using System.Web;

namespace Redecard.PN.Extrato.SharePoint.WebParts.RelatoriosDetalhe
{
    public partial class RelatoriosDetalheUserControl : BaseUserControl, IPostBackDataHandler
    {
        #region [ Atributos e Propriedades ]

        private static JavaScriptSerializer _jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get
            {
                if (_jsSerializer == null)
                    _jsSerializer = new JavaScriptSerializer();
                return _jsSerializer;
            }
        }

        protected DynamicControlsPlaceholder pnlRelatorio;
        protected PopupEmail popupEmail;

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

#if DEBUG   //Se em DEBUG, sempre exibe botão para envio por e-mail
            mnuAcoes.BotaoEmail = true;
#else
            // Verificar se é central de atendimento, caso positivo, exibir o link de enviar por e-mail
            if(this.SessaoAtual != null)
                mnuAcoes.BotaoEmail = this.SessaoAtual.UsuarioAtendimento;
#endif
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // colocar data e hora da consulta na tela de impressão
            DateTime data = DateTime.Now;
            hdnDataHora.Value = String.Format("Data da consulta {0} às {1}", data.ToString("dd/MM/yyyy"), data.ToString("HH:mm"));

            String target = Request["__EVENTTARGET"];           
            Buscar(target == null);
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            base.RetornarPaginaAnterior();            
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            String html = String.Empty;

            if (!object.ReferenceEquals(Dados, null))
            {
                Relatorio _relatorio = Relatorio.Obter((TipoRelatorio)Dados.IDRelatorio, (TipoVenda)Dados.IDTipoVenda);

                if (!String.IsNullOrEmpty(_relatorio.ControleDetalheRelatorio))
                {
                    try
                    {
                        String guidBuscarDados = Guid.NewGuid().ToString();
                        this.ArmazenaInformacaoTransicaoSession(guidBuscarDados, Dados);

                        // Redirecionar 
                        QueryStringSegura queryString = QS;
                        queryString.Add("MAXLINHAS", MAX_LINHAS_DOWNLOAD.ToString());
                        queryString.Add("SRC", _relatorio.ControleDetalheRelatorio);
                        queryString.Add("GUID_DADOS", guidBuscarDados);

                        Response.Redirect("/_layouts/Redecard.PN.Extrato.SharePoint/RelatorioExcel.aspx?dados=" + queryString.ToString(), true);
                    }
                    catch (Exception ex)
                    {
                        SharePointUlsLog.LogErro(ex);
                    }
                }
            }
        }
        
        private BuscarDados Dados
        {
            get
            {
                // valida se foi passado contéudo na URL
                // (prevendo edição de conteúdo)
                if (QS == null)
                    return null;

                if (ViewState["Dados"] == null)
                    ViewState["Dados"] = RetiraInformacaoTransicaoSession<BuscarDados>(QS["guidBuscarDados"]);
                return (BuscarDados)ViewState["Dados"];
            }
        }

        private void CarregarRelatorioDetalhe(Boolean pesquisar)
        {
            if (!object.ReferenceEquals(Dados, null))
            {
                Relatorio _relatorio = Relatorio.Obter((TipoRelatorio)Dados.IDRelatorio, (TipoVenda)Dados.IDTipoVenda);
                try
                {
                    if (!String.IsNullOrEmpty(_relatorio.ControleDetalheRelatorio))
                    {
                        pnlRelatorio.ASCXPath = _relatorio.ControleDetalheRelatorio;

                        if (pesquisar)
                        {
                            pnlRelatorio.Controls.Clear();
                            Control control = Page.LoadControl(_relatorio.ControleDetalheRelatorio);
                            control.ID = ((IRelatorioHandler)control).IdControl;
                            pnlRelatorio.Controls.Add(control);
                            ((IRelatorioHandler)control).Pesquisar(Dados);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

        protected void Buscar(Boolean pesquisar)
        {
            BuscarDados dados = Dados;
            if (!object.ReferenceEquals(dados, null))
            {
                Relatorio _relatorio = Relatorio.Obter((TipoRelatorio)dados.IDRelatorio, (TipoVenda)dados.IDTipoVenda);

                if (String.IsNullOrEmpty(_relatorio.NomeDetalhe))
                    ttlTitulo.Descricao = _relatorio.Nome + " - detalhe";
                else
                    ttlTitulo.Descricao = _relatorio.NomeDetalhe;
                
                CarregarRelatorioDetalhe(pesquisar);
            }
        }

        protected void popupEmail_PrepararEmail()
        {
            using (Logger Log = Logger.IniciarLog("Envio por E-mail do Relatório - Detalhe"))
            {
                try
                {
                    if (!object.ReferenceEquals(Dados, null))
                    {
                        QuantidadePost--;

                        Relatorio _relatorio = Relatorio.Obter((TipoRelatorio)Dados.IDRelatorio, (TipoVenda)Dados.IDTipoVenda);

                        if (!String.IsNullOrEmpty(_relatorio.ControleDetalheRelatorio))
                        {
                            //Renderização e geração do HTML do relatório
                            Page pageHolder = new FormlessPage() { AppRelativeTemplateSourceDirectory = HttpRuntime.AppDomainAppVirtualPath };
                                                        
                            var control = pageHolder.LoadControl(_relatorio.ControleDetalheRelatorio);
                            control.ID = "Email_RelatorioDetalhe";
                            (control as BaseUserControl)._QSDados = _QSDados;

                            String corpoEmail = ((IRelatorioHandler)control).ObterTabelaExcel(Dados, MAX_LINHAS_DOWNLOAD, true);
                            pageHolder.Controls.Add(control);
                            control.EnableViewState = false;

                            popupEmail.AssuntoEmail = String.Format("Extrato Rede - {0} ({1})",
                                _relatorio.Nome, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                            popupEmail.EnviarEmail(corpoEmail);
                        }
                    }                                                                                
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                }
            }
        }

        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            return false;
        }

        public void RaisePostDataChangedEvent() { }
    }
}
