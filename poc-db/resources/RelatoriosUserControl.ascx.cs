/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [31/10/2012] – [André Garcia] – [Criação do Controle de Relatórios]
*/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;

namespace Redecard.PN.Extrato.SharePoint.WebParts.Relatorios
{
    public partial class RelatoriosUserControl : BaseUserControl, IPostBackDataHandler
    {
        /// <summary>
        /// Passos da Webpart
        /// </summary>
        public Relatorios WebPartRelatorios { get { return this.Parent as Relatorios; } }

        /// <summary>
        /// 
        /// </summary>
        protected PopupEmail popupEmail;

        /// <summary>
        /// 
        /// </summary>
        protected DynamicControlsPlaceholder pnlRelatorioControl;

        /// <summary>
        /// Fonte dos erros de scripts
        /// </summary>
        public String _fonteErro = "Redecard.PN.Extrato";

        /// <summary>
        /// Converte o relatório de uma tabela HTML para uma tabela do
        /// excel
        /// </summary>
        protected void DownloadExcel(object sender, EventArgs e)
        {
            String html = String.Empty;
            BuscarDados dados = ((Filtro)filtroControl).RecuperarBuscarDadosDTO();
            if (!object.ReferenceEquals(dados, null))
            {
                Relatorio _relatorio = Relatorio.Obter((TipoRelatorio)dados.IDRelatorio, (TipoVenda)dados.IDTipoVenda);

                using (Logger Log = Logger.IniciarLog("Download Excel do Relatório " + _relatorio.Nome))
                {
                    if (!String.IsNullOrEmpty(_relatorio.ControleRelatorio))
                    {
                        try
                        {
                            String guidBuscarDados = Guid.NewGuid().ToString();

                            // Redirecionar 
                            QueryStringSegura queryString = new QueryStringSegura();
                            queryString.Add("MAXLINHAS", MAX_LINHAS_DOWNLOAD.ToString());
                            queryString.Add("SRC", _relatorio.ControleRelatorio);
                            queryString.Add("GUID_DADOS", guidBuscarDados);

                            BaseUserControl.ArmazenaInformacaoTransicaoSession(guidBuscarDados, dados, Session);

                            Logger.GravarLog("Redirecionando para página de Download Excel", new { MAX_LINHAS_DOWNLOAD, SRC = _relatorio.ControleRelatorio, dados });

                            Response.Redirect("/_layouts/Redecard.PN.Extrato.SharePoint/RelatorioExcel.aspx?dados=" + queryString.ToString(), false);
                            Context.ApplicationInstance.CompleteRequest();
                        }
                        catch (Exception ex)
                        {
                            Log.GravarErro(ex);
                            SharePointUlsLog.LogErro(ex);
                        }
                    }
                    else
                    {
                        Log.GravarMensagem("Relatório indisponível");
                        // Relatório indisponível
                        this.ExibirErro(_fonteErro, 310);
                    }
                }
            }
            else
            {
                // Relatório indisponível
                this.ExibirErro(_fonteErro, 310);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
#if DEBUG
            //Se em DEBUG, sempre exibe botão para envio por e-mail
            mnuAcoes.BotaoEmail = true;
#else
            // Verificar se é central de atendimento, caso positivo, exibir o link de enviar por e-mail
            if(this.SessaoAtual != null)
                mnuAcoes.BotaoEmail = this.SessaoAtual.UsuarioAtendimento;
#endif
        }

        /// <summary>
        /// Carregamento da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.MaintainScrollPositionOnPostBack = true;
            String target = GetPostBackControlName();

            if (IsPostBack && !object.ReferenceEquals(this.SessaoAtual, null))
            {
                // colocar data e hora da consulta na tela de impressão
                DateTime data = DateTime.Now;
                hdnDataHora.Value = String.Format("Data da consulta {0} às {1}", data.ToString("dd/MM/yyyy"), data.ToString("HH:mm"));

                // Exportar para o excel
                if (target.EndsWith("linkExcel"))
                    return;

                //Busca o controle de filtro
                Filtro objFiltro = ((Filtro)filtroControl);
                BuscarDados dadosBusca = objFiltro.RecuperarBuscarDadosDTO();
                //Verifica se está válido
                if (objFiltro.ValidarFiltro())
                {
					// gerar lista de relatórios no cliente
					objFiltro.CarregarRelatorios();
					objFiltro.SetarFiltro(dadosBusca);
					
                    Buscar(dadosBusca, (target.Equals("filtroControl_btnBuscar") || target.Equals("btnBuscar")));
                    if (dadosBusca.IDRelatorio == 13)
                    {// relatório de conta corrente, esconder o filtro antigo
                        tbFiltroAntigo.Visible = false;
                        mnuAcoes.Visible = false;
                    }
                }
                else
                    pnlRelatorioControl.Controls.Clear();
            }
            else
            {
                //Recupera dados da querystring
                String dados = Request.QueryString["dados"] as String;
                if (!String.IsNullOrEmpty(dados) && !Page.IsPostBack && Sessao.Contem())
                {
                    QueryStringSegura qs = new QueryStringSegura(Request.QueryString["dados"]);
                    DateTime? dataInicial = qs["DataInicial"].ToDateTimeNull("dd/MM/yyyy");
                    DateTime? dataFinal = qs["DataFinal"].ToDateTimeNull("dd/MM/yyyy");
                    Boolean pesquisar = false;
                    Int32? tipo = qs["TipoRelatorio"].ToInt32Null();
                    Int32? tipoVenda = qs["TipoVenda"].ToInt32Null();
                    Int32 estabelecimento = qs["Estabelecimento"].ToInt32(SessaoAtual.CodigoEntidade);
                    Int32? modalidade = null;

                    foreach (String key in qs.AllKeys)
                    {
                        if (key == "Modalidade")
                            modalidade = qs["Modalidade"].ToInt32();
                    }

                    if (!IsPostBack)
                        pesquisar = String.Compare(qs["Pesquisar"], Boolean.TrueString) == 0;

                    //Mapear lançamentos de conta corrente com os relatórios de Extrato
                    if (tipo.HasValue && tipo.Value >= 20)
                    {
                        Int32? tipoExtrato = Helper.MapaRelatorios.ObterMapaTipoRelatorio(tipo.Value);
                        tipoVenda = Helper.MapaRelatorios.ObterMapaTipoVenda(tipo.Value);

                        tipo = tipoExtrato;
                        pesquisar = true;
                    }

                    //Monta objeto de filtro
                    BuscarDados filtro = new BuscarDados();
                    if (dataInicial.HasValue)
                        filtro.DataInicial = dataInicial.Value;
                    if (dataFinal.HasValue)
                        filtro.DataFinal = dataFinal.Value;
                    if (tipo.HasValue)
                        filtro.IDRelatorio = tipo.Value;
                    if (tipoVenda.HasValue)
                        filtro.IDTipoVenda = tipoVenda.Value;
                    if (modalidade.HasValue)
                        filtro.IDModalidade = modalidade.Value;
                    filtro.Estabelecimentos = new[] { estabelecimento };

                    if (tipo.HasValue && tipo.Value == 13)
                    { // relatório de conta corrente, não exibir o filtro antigo
                        tbFiltroAntigo.Visible = false;
                        mnuAcoes.Visible = false;
                    }

                    (filtroControl as Filtro).ConsultaPVs.PVsSelecionados = new List<Int32>(new[] { estabelecimento });
                    (filtroControl as Filtro).CarregarRelatorios();
                    (filtroControl as Filtro).SetarRelatorioAtual();
                    Buscar(filtro, pesquisar);
                }
                else if (this.WebPartRelatorios.HabilitarContaCorrente)
                {
                    // carregar e setar automaticamente o formulário de Conta Corrente na abertura do módulo de relatórios
                    if (base.ValidarPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=13"))
                    { // validar acesso ao relatório Conta Corrente 
                        BuscarDados filtro = new BuscarDados();
                        filtro.DataInicial = DateTime.Now.AddDays(-5);
                        filtro.DataFinal = DateTime.Now;
                        filtro.IDRelatorio = 13; // Conta Corrente
                        filtro.IDTipoVenda = 3;
                        filtro.IDModalidade = 0;
                        filtro.Estabelecimentos = new[] { this.SessaoAtual.CodigoEntidade };

                        (filtroControl as Filtro).ConsultaPVs.PVsSelecionados = new List<Int32>(new[] { this.SessaoAtual.CodigoEntidade });
                        (filtroControl as Filtro).CarregarRelatorios();
                        (filtroControl as Filtro).SetarFiltro(filtro);
                        Buscar(filtro, true);

                        // se carregar o relatório de conta corrente, não exibir o filtro antigo
                        tbFiltroAntigo.Visible = false;
                        mnuAcoes.Visible = false;
                    }
                }
            }
        }

        protected void Buscar(BuscarDados Dados, Boolean pesquisar)
        {
            Relatorio _relatorio = ObterRelatorio(Dados);

            Boolean v2 = _relatorio.Versao == 2;
            //btnVoltar.Visible = v2;
            ttlTitulo.Visible = v2;
            ttlTitulo.Descricao = _relatorio.Nome;

            CarregarRelatorio(pesquisar);
        }

        private void CarregarRelatorio(Boolean pesquisar)
        {
            Filtro objFiltro = ((Filtro)filtroControl);
            BuscarDados Dados = objFiltro.RecuperarBuscarDadosDTO();

            if (!object.ReferenceEquals(Dados, null))
            {
                Relatorio _relatorio = ObterRelatorio(Dados);

                try
                {
                    if (!String.IsNullOrEmpty(_relatorio.ControleRelatorio))
                    {
                        Logger.GravarLog(String.Format("Pesquisa Relatório ID {0} / {1}",
                            (Int32)_relatorio.TipoRelatorio, _relatorio.ControleRelatorio), new { Dados });

                        pnlRelatorioControl.ASCXPath = _relatorio.ControleRelatorio;

                        if (pesquisar)
                        {
                            pnlRelatorioControl.Controls.Clear();
                            Control control = Page.LoadControl(_relatorio.ControleRelatorio);
                            control.ID = ((IRelatorioHandler)control).IdControl;
                            pnlRelatorioControl.Controls.Add(control);

                            ((IRelatorioHandler)control).Pesquisar(Dados);
                        }
                    }
                    else
                    {
                        Logger.GravarErro("Relatório indisponível");
                        // Relatório indisponível
                        this.ExibirErro(_fonteErro, 310);
                    }
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro durante carregamento do UserControl do relatório " + _relatorio.Nome, ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(_fonteErro, 313);
                }
            }
            else
            {
                // Relatório indisponível
                this.ExibirErro(_fonteErro, 310);
            }
        }

        protected void linkExcel_Click(object sender, EventArgs e)
        {
            DownloadExcel(sender, e);
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            //Response.Redirect("pn_default.aspx");
            RetornarPaginaAnterior();
        }

        private Relatorio ObterRelatorio(BuscarDados dados)
        {
            return Relatorio.Obter((TipoRelatorio)dados.IDRelatorio, (TipoVenda)dados.IDTipoVenda);
        }

        private void ExibirErro(String fonte, Int32 codigo)
        {
            base.ExibirPainelExcecao(fonte, codigo);
        }

        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            return false;
        }

        public void RaisePostDataChangedEvent()
        {
        }

        private String GetPostBackControlName()
        {
            //first we will check the "__EVENTTARGET" because if post back made by       the controls
            //which used "_doPostBack" function also available in Request.Form collection.
            String ctrlname = Page.Request.Params["__EVENTTARGET"];
            if (ctrlname != null && ctrlname != String.Empty)
            {
                return ctrlname;
            }
            // if __EVENTTARGET is null, the control is a button type and we need to
            // iterate over the form collection to find it
            else
            {
                Control control = null;
                String ctrlStr = String.Empty;
                Control c = null;
                foreach (string ctl in Page.Request.Form)
                {
                    //handle ImageButton they having an additional "quasi-property" in their Id which identifies
                    //mouse x and y coordinates
                    if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                    {
                        ctrlStr = ctl.Substring(0, ctl.Length - 2);
                        c = Page.FindControl(ctrlStr);
                    }
                    else
                    {
                        c = Page.FindControl(ctl);
                    }
                    if (c is System.Web.UI.WebControls.Button ||
                             c is System.Web.UI.WebControls.ImageButton)
                    {
                        control = c;
                        break;
                    }
                }
                if (control != null)
                    return control.ID;
            }
            return String.Empty;
        }
        protected void btnSearchClient_Click(object sender, EventArgs e)
        {
            // obter valor do hdnSearchClient e preencher o objeto de busca com os dados informados
            string jsonObject = hdnSearchClient.Value;
            if (!String.IsNullOrEmpty(jsonObject))
            {
                //Modelo do objeto que deve ser informado
                /*
                {
                    "inicio": "yyyy-MM-dd",
                    "fim": "yyyy-MM-dd",
                    "tipoRelatorio": 0,
                    "tipoVenda": 0,
                    "tipoEstabelecimento": 0,
                    "estabelecimento": [ 0, 0 ]
                }
                */
                var search = SearchObject.Deserialize(jsonObject);
                BuscarDados filtro = new BuscarDados();
                filtro.DataInicial = search.inicio;
                filtro.DataFinal = search.fim;
                filtro.IDRelatorio = search.tipoRelatorio; // Conta Corrente
                filtro.IDTipoVenda = search.tipoVenda;
                filtro.IDModalidade = 0;
                filtro.Estabelecimentos = search.estabelecimento;

                (filtroControl as Filtro).CarregarRelatorios();
                (filtroControl as Filtro).SetarFiltro(filtro);
                (filtroControl as Filtro).ConsultaPVs.PVsSelecionados = search.estabelecimento.ToList();
                (filtroControl as Filtro).ConsultaPVs.TipoAssociacao = (Core.Web.Controles.Portal.ConsultaPvTipoAssociacao)search.tipoEstabelecimento;
                Buscar(filtro, true);

                //exibir filtro de pesquisa anterior
                tbFiltroAntigo.Visible = true;
                mnuAcoes.Visible = true;
            }
        }
        protected void popupEmail_PrepararEmail()
        {
            BuscarDados dados = ((Filtro)filtroControl).RecuperarBuscarDadosDTO();

            if (!object.ReferenceEquals(dados, null))
            {
                Relatorio _relatorio = ObterRelatorio(dados);

                using (Logger Log = Logger.IniciarLog("Envio por E-mail do Relatório " + _relatorio.Nome))
                {
                    if (!String.IsNullOrEmpty(_relatorio.ControleRelatorio))
                    {
                        try
                        {
                            //Renderização e geração do HTML do relatório
                            Page pageHolder = new FormlessPage() { AppRelativeTemplateSourceDirectory = HttpRuntime.AppDomainAppVirtualPath };
                            UserControl control = (UserControl)pageHolder.LoadControl(_relatorio.ControleRelatorio);
                            control.ID = "Email_" + ((IRelatorioHandler)control).IdControl;
                            pageHolder.Controls.Add(control);
                            control.EnableViewState = false;
                            String corpoEmail = ((IRelatorioHandler)control).ObterTabelaExcel(dados, MAX_LINHAS_DOWNLOAD, true);

                            popupEmail.AssuntoEmail = String.Format("Extrato Rede - Relatório de {0} ({1})",
                                _relatorio.Nome, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                            popupEmail.EnviarEmail(corpoEmail);
                        }
                        catch (Exception ex)
                        {
                            Log.GravarErro(ex);
                            SharePointUlsLog.LogErro(ex);
                        }
                    }
                    else
                    {
                        Log.GravarMensagem("Relatório indisponível");
                        // Relatório indisponível
                        this.ExibirErro(_fonteErro, 310);
                    }
                }
            }
            else
            {
                // Relatório indisponível
                this.ExibirErro(_fonteErro, 310);
            }
        }
    }
}