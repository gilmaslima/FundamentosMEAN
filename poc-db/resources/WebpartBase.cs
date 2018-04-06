using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.Comum
{
    public class WebPartBase : WebPart
    {
        /// <summary>
        /// Constante com o código genérico de erro das classes de dados
        /// </summary>
        /// <value>300</value>
        public const Int32 CODIGO_ERRO = 300;

        /// <summary>
        /// Constante com o nome completo da classe
        /// </summary>
        /// <value>Redecard.PN.SharePoint</value>
        public const String FONTE = "Redecard.PN.SharePoint";

        /// <summary>
        /// Objeto SPWeb
        /// </summary>
        protected SPWeb web;

        /// <summary>
        /// 
        /// </summary>
        protected Boolean _validar = true;

        /// <summary>
        /// Objeto utilizado para verificar se a permissão será validada ou não
        /// </summary>
        public Boolean ValidarPermissao
        {
            get
            {
                return _validar;
            }
            set
            {
                _validar = value;
            }
        }

        /// <summary>
        /// Metódo auxilixar para pesquisar em toda a estrutura de Menu
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        protected Menu ObterMenuItemAtual(List<Menu> items, String caminho)
        {
            if (!object.ReferenceEquals(items, null) && items.Count > 0)
            {
                Menu itemAtual = null;
                Pagina paginaAtual = null;

                foreach (Menu item in items)
                {
                    paginaAtual = item.Paginas.FirstOrDefault(x => !String.IsNullOrEmpty(x.Url) && (caminho.Contains(x.Url) || caminho == x.Url));
                    if (!object.ReferenceEquals(paginaAtual, null))
                    {
                        itemAtual = item;
                        break;
                    }
                }

                if (!object.ReferenceEquals(itemAtual, null))
                    return itemAtual;
                else
                {
                    foreach (Menu item in items)
                    {
                        itemAtual = this.ObterMenuItemAtual(item.Items, caminho);
                        if (!object.ReferenceEquals(itemAtual, null))
                            break;
                    }
                    if (!object.ReferenceEquals(itemAtual, null))
                    {
                        return itemAtual;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Metódo auxilixar para pesquisar em toda a estrutura de Menu
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        protected Menu ObterMenuItemAtual(List<Menu> items)
        {
            String absolutePah = HttpContext.Current.Request.Url.AbsolutePath;
            return ObterMenuItemAtual(items, absolutePah);
        }

        /// <summary>
        /// Retorno o item de menu atual da página, caso não encontre nenhum item, retorno nulo.
        /// </summary>
        /// <returns></returns>
        protected Menu ObterMenuItemAtual(String caminho)
        {
            Menu itemAtual = null;
            itemAtual = this.ObterMenuItemAtual(SessaoAtual.Menu, caminho);
            return itemAtual;
        }

        /// <summary>
        /// Retorno o item de menu atual da página, caso não encontre nenhum item, retorno nulo.
        /// </summary>
        /// <returns></returns>
        protected Menu ObterMenuItemAtual()
        {
            Menu itemAtual = null;
            itemAtual = this.ObterMenuItemAtual(SessaoAtual.Menu);
            return itemAtual;
        }

        /// <summary>
        /// Flag para identificar se deve ser exibida a mensagem de acesso negado
        /// </summary>
        internal Boolean _acessoNegado = false;

        /// <summary>
        /// Identificador de subsituição do controle para um controle de Acesso Negado.
        /// </summary>
        internal Boolean AcessoNegado
        {
            get
            {
                return _acessoNegado;
            }
            set
            {
                _acessoNegado = value;
            }
        }

        /// <summary>
        /// Verificaçãoes iniciais ao carregar um UserControl
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            web = SPContext.Current.Web;
            base.OnLoad(e);

            // Verifica se a usercontrol utilizado esta na área fechada
            if (HttpContext.Current.Request.ServerVariables["URL"].Contains("sites/fechado"))
            {
                // Se não existir a sessão e o usuário logado é via FBA exibe painel de exceção
                if (this.SessaoAtual == null && !Util.UsusarioAdministrador())
                {
                    // redirecionar usuário para  a página de sessão expirada
                    this.RedirecionarSessaoExpirada();
                    //return;
                }
                // Validar página do usuário caso a sessão esteja aberta e o usuário
                // logado como FBA
                else if (this.SessaoAtual != null && Util.UsuarioLogadoFBA() && ValidarPermissao)
                {
                    if (!this.SessaoAtual.SenhaMigrada && this.SessaoAtual.PossuiKomerci
                        && !this.SessaoAtual.UsuarioAtendimento && !this.SessaoAtual.AcessoFilial)
                    {
                        this.RedirecionaAlteracaoSenha();
                    }
                    else
                    {
                        Boolean retorno = this.ValidarPagina();
                        if (!retorno)
                        {
                            this.AcessoNegado = true;
                            return;
                        }
                        else
                        {
                            //Se o PV estiver Cancelado, ele não poderá ter acesso ao subsite de Serviços
                            if (HttpContext.Current.Request.ServerVariables["URL"].Contains("sites/fechado/servicos") && this.SessaoAtual.StatusPVCancelado())
                                this.AcessoNegado = true;
                            return;
                        }
                    }
                }
                else if (this.SessaoAtual != null && Util.UsuarioLogadoFBA())
                {
                    if (!this.SessaoAtual.SenhaMigrada && this.SessaoAtual.PossuiKomerci
                            && !this.SessaoAtual.UsuarioAtendimento && !this.SessaoAtual.AcessoFilial)
                        this.RedirecionaAlteracaoSenha();
                }
                // Caso o usuário seja Windows o mesmo poderá ver a tela(SharePoint) para alteração de conteúdo
                else if (this.SessaoAtual == null && Util.UsusarioAdministrador())
                    return;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!this.AcessoNegado)
                base.RenderControl(writer);
            else
            {
                Control control = Page.LoadControl(@"~/_CONTROLTEMPLATES/Comum/QuadroAcessoNegado.ascx");
                control.RenderControl(writer);
            }
        }

        /// <summary>
        /// Redireciona o usuário para a tela de sessão expirada.
        /// </summary>
        private void RedirecionarSessaoExpirada()
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Redirect(SPContext.Current.Web.Url + "/_layouts/Redecard.Comum/SessaoExpirada.aspx");
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// Redireciona o usuário para alteração de senha
        /// </summary>
        private void RedirecionaAlteracaoSenha()
        {
            if (!HttpContext.Current.Request.ServerVariables["URL"].Contains("MeuUsuarioAlteracaoSenha.aspx") && !HttpContext.Current.Request.ServerVariables["URL"].Contains("logout.aspx"))
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Redirect(SPContext.Current.Web.Site.ServerRelativeUrl + "/minhaconta/Paginas/MeuUsuarioAlteracaoSenha.aspx");
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// Valida o acesso a página especificada no paramêtro "paginaUrl" de 
        /// acordo com a relação de páginas do usuário
        /// </summary>
        /// <param name="paginaUrl">URL da Página</param>
        /// <returns>Página válida</returns>
        public Boolean ValidarPagina(String paginaUrl)
        {
            return this.SessaoAtual.VerificarAcessoPagina(paginaUrl);
        }

        /// <summary>
        /// Valida o acesso a página atual de acordo com a relação de páginas do usuário
        /// </summary>
        /// <returns>Página válida</returns>
        public Boolean ValidarPagina()
        {
            return ValidarPagina(HttpContext.Current.Request.Url.AbsolutePath);
        }

        /// <summary>
        /// Objeto Sessão
        /// </summary>
        private Sessao _sessao = null;

        /// <summary>
        /// Recupera sessão atual caso exista
        /// </summary>
        public Sessao SessaoAtual
        {
            get
            {
                if (object.ReferenceEquals(_sessao, null))
                    _sessao = Sessao.Obtem();

                return _sessao;
            }
        }

        /// <summary>
        /// Exibe mensagem em um Alert e redireciona usuário para Home do Site
        /// </summary>
        /// <param name="mensagem">Mensagem</param>
        /// <param name="redireciona">Necessita de redirecionamento</param>
        public virtual void Alert(String mensagem, Boolean redireciona)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("alert('");
            sb.Append(mensagem);
            sb.Append("'); ");

            if (redireciona)
            {
                sb.Append("window.parent.location.href='");
                sb.Append(this.web.Url);
                sb.Append("';");
            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AlertRedirect", sb.ToString(), true);
        }

        /// <summary>
        /// Exibe mensagem em um Alert e redireciona usuário para a URL passada no parâmetro
        /// </summary>
        /// <param name="mensagem">Mensagem</param>
        /// <param name="url">URL da página que será redicionado</param>
        public virtual void Alert(String mensagem, String url)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("alert('");
            sb.Append(mensagem);
            sb.Append("'); window.parent.location.href='");
            sb.Append(url);
            sb.Append("';");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AlertRedirect", sb.ToString(), true);
        }

        /// <summary>
        /// Redireciona usuário para a URL contida na QueryString Source, caso contrário para URL inicial do site.
        /// </summary>
        public virtual void Redirect()
        {
            if (HttpContext.Current.Request.QueryString["source"] != null)
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.QueryString["source"], false);
            else
                HttpContext.Current.Response.Redirect(this.web.Url, false);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="codigo">Código do erro</param>
        public void ExibirPainelExcecao(String mensagem, String codigo)
        {
            this.GeraPainelExcecao(mensagem, codigo);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>
        /// <param name="servico">Serviço/Método onde ocorreu o erro</param>
        /// <param name="codigo">Código do erro</param>
        public void ExibirPainelExcecao(String fonte, Int32 codigo)
        {
            this.GeraPainelExcecao(fonte, codigo);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>
        /// <param name="ex">Exceção contendo a mensagem e código do erro</param>
        public void ExibirPainelExcecao(PortalRedecardException ex)
        {
            this.GeraPainelExcecao(ex.Fonte, ex.Codigo);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema com Update Panel
        /// </summary>
        /// <param name="fonte"></param>
        /// <param name="codigo"></param>
        public void ExibirPainelExcecaoAsync(String fonte, Int32 codigo, UpdatePanel panel)
        {
            this.GeraPainelExcecaoAsync(fonte, codigo, panel);
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema com Update Panel
        /// </summary>
        /// <param name="fonte"></param>
        /// <param name="codigo"></param>
        /// <param name="panel"></param>
        private void GeraPainelExcecaoAsync(String fonte, Int32 codigo, UpdatePanel panel)
        {
            using (TrataErroServico.TrataErroServicoClient trataErroServico = new TrataErroServico.TrataErroServicoClient())
            {
                var trataErro = trataErroServico.Consultar(fonte, codigo);

                if (trataErro.Codigo != 0)
                    this.GeraPainelExcecaoAsync(trataErro.Fonte, trataErro.Codigo.ToString(), panel);
                else
                    this.GeraPainelExcecaoAsync("Sistema Indisponível", "-1", panel);
            }
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema com Update Panel
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="codigo"></param>
        /// <param name="panel"></param>
        private void GeraPainelExcecaoAsync(String mensagem, String codigo, UpdatePanel panel)
        {
            this.ExibirPainelMensagemAsync(String.Format("{0} ({1})", mensagem, codigo), panel);
        }

        /// <summary>
        /// Gera painel padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem</param>
        public void ExibirPainelMensagemAsync(String mensagem, UpdatePanel panel)
        {
            String titulo = HttpContext.GetGlobalResourceObject("redecard", "titMensagemErro").ToString();
            String script = String.Format("exibirPainelMensagem('{0}', '{1}');",
                HttpUtility.HtmlEncode(titulo), HttpUtility.HtmlEncode(mensagem));
            ScriptManager.RegisterStartupScript(panel, panel.GetType(), "Key_" + panel.ClientID, script, true);
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="codigo">Código do erro</param>
        private void GeraPainelExcecao(String fonte, Int32 codigo)
        {
            using (TrataErroServico.TrataErroServicoClient trataErroServico = new TrataErroServico.TrataErroServicoClient())
            {
                var trataErro = trataErroServico.Consultar(fonte, codigo);

                if (trataErro.Codigo != 0)
                    this.GeraPainelExcecao(trataErro.Fonte, trataErro.Codigo.ToString());
                else
                    this.GeraPainelExcecao("Sistema Indisponível", "-1");
            }
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Painel de erro padrão</returns>
        public String RetornarMensagemErro(String fonte, Int32 codigo)
        {
            using (TrataErroServico.TrataErroServicoClient trataErroServico = new TrataErroServico.TrataErroServicoClient())
            {
                String _erroFormat = "{0} ({1})";
                var trataErro = trataErroServico.Consultar(fonte, codigo);

                if (trataErro.Codigo != 0)
                    return String.Format(_erroFormat, trataErro.Fonte, trataErro.Codigo);
                else
                    return String.Format(_erroFormat, "Sistema Indisponível", "-1");
            }
        }

        /// <summary>
        /// Gera painel padrão do sistema, porém, permite a padronização do HTML do painel
        /// </summary>
        /// <param name="mensagem">Mensagem</param>
        public void ExibirPainelHtml(String htmlPainel)
        {
            Literal ltPainel = new Literal() { Text = htmlPainel };

            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("<script type='text/javascript'>");
            sbScript.Append("$(document).ready(function () {");
            sbScript.Append("$('#bgProtecao').show();");
            sbScript.Append("});</script>");

            this.Controls.Add(ltPainel);
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, sbScript.ToString(), false);
        }

        /// <summary>
        /// Gera painel padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem</param>
        public void ExibirPainelMensagem(String mensagem)
        {
            String titulo = HttpContext.GetGlobalResourceObject("redecard", "titMensagemErro").ToString();
            String script = String.Format("exibirPainelMensagem('{0}', '{1}');",
                HttpUtility.HtmlEncode(titulo), HttpUtility.HtmlEncode(mensagem));
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="codigo">Código do erro</param>
        private void GeraPainelExcecao(String mensagem, String codigo)
        {
            this.ExibirPainelMensagem(String.Format("{0} ({1})", mensagem, codigo));
        }
        /// <summary>
        /// Exibe o painel de confirmação e oculta os painéis informados
        /// </summary>
        /// <param name="titulo">Título do painel</param>
        /// <param name="mensagem">Mensagem que será exibida no painel</param>
        /// <param name="urlVoltar">Url que o botão irá redirecionar ao ser acionado</param>
        /// <param name="paineis">Lista de painéis que serão ocultados</param>
        public void ExibirPainelConfirmacaoAcao(String titulo, String mensagem, String urlVoltar, Panel[] paineis)
        {
            this.ExibirPainelConfirmacaoAcao(titulo, mensagem, urlVoltar, paineis, String.Empty);
        }
        /// <summary>
        /// Exibe o painel de confirmação e oculta os painéis informados
        /// </summary>
        /// <param name="titulo">Título do painel</param>
        /// <param name="mensagem">Mensagem que será exibida no painel</param>
        /// <param name="urlVoltar">Url que o botão irá redirecionar ao ser acionado</param>
        /// <param name="paineis">Lista de painéis que serão ocultados</param>
        public void ExibirPainelConfirmacaoAcao(String titulo, String mensagem, String urlVoltar, Panel[] paineis, String classeImagem)
        {
            Control control = OcultarPanels(paineis);
            this.GerarPainelConfirmcaoAcao(titulo, mensagem, urlVoltar, control, classeImagem);
        }
        /// <summary>
        /// Oculta os painéis informados
        /// </summary>
        /// <param name="paineis">Array de painéis que serão ocultados</param>
        private Control OcultarPanels(Panel[] paineis)
        {
            Control ctl = null;
            if (paineis.Length > 0)
            {
                foreach (Panel item in paineis)
                {
                    ctl = item.Parent;
                    item.Visible = false;

                }
            }
            return ctl;
        }
        /// <summary>
        /// Gera o painel de confirmação de ação
        /// Na página que vai chamar este método deve ter uma container principal com id WpContent
        /// para que o Jquery possa inserir o painel dentro do container.
        /// </summary>
        /// <param name="titulo">Título do painel</param>
        /// <param name="mensagem">Mensagem que será exibida no painle</param>
        /// <param name="urlVoltar">Url que o botão redirecionar ao ser acionada.</param>
        private void GerarPainelConfirmcaoAcao(String titulo, String mensagem, String urlVoltar, Control controlePai)
        {
            this.GerarPainelConfirmcaoAcao(titulo, mensagem, urlVoltar, controlePai, String.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensagem"></param>
        /// <param name="urlVoltar"></param>
        /// <param name="controlePai"></param>
        private void GerarPainelConfirmcaoAcao(String titulo, String mensagem, String urlVoltar, Control controlePai, String classeImagem)
        {
            HttpContext.Current.Session["AvisoConfirmacaoAcao"] = "S";
            HttpContext.Current.Session["TituloMensagem"] = titulo;
            HttpContext.Current.Session["Mensagem"] = mensagem;
            HttpContext.Current.Session["UrlVoltar"] = urlVoltar;
            HttpContext.Current.Session["classeImagem"] = (!String.IsNullOrEmpty(classeImagem) ? classeImagem : "icone-green");

            HttpContext.Current.Response.Redirect(SPContext.Current.Site.Url + "/Paginas/Confirmacao.aspx");
        }

        /// <summary>
        /// Retorna a URL da Homepage Aberta do Portal
        /// </summary>
        /// <returns></returns>
        public String RecuperarEnderecoPortal()
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            return url;
        }

        /// <summary>
        /// Retorna a URL da Homepage Fechada do Portal
        /// </summary>
        /// <returns></returns>
        public String RecuperarEnderecoPortalFechado()
        {
            String url = String.Empty;
            url = String.Concat(Util.BuscarUrlRedirecionamento("/", SPUrlZone.Internet), "/sites/fechado");
            return url;
        }
    }
}
