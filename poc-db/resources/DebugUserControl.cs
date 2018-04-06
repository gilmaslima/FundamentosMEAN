using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Redecard.PN.Comum;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace Redecard.PN.FMS.Sharepoint
{
    public class DebugUserControl: Redecard.PN.Comum.UserControlBase
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
        /// Verificaçãoes iniciais ao carregar um UserControl
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            web = SPContext.Current.Web;
            base.OnLoad(e);

            // Verifica se a usercontrol utilizado esta na área fechada
            if (Request.ServerVariables["URL"].Contains("sites/fechado"))
            {
                // Se não existir a sessão e o usuário logado é via FBA exibe painel de exceção
                if (this.SessaoAtual == null && Util.UsuarioLogadoFBA())
                {
                    // redirecionar usuário para  a página de sessão expirada
                    this.RedirecionarSessaoExpirada();
                    //return;
                }
                // Validar página do usuário caso a sessão esteja aberta e o usuário
                // logado como FBA
                else if (this.SessaoAtual != null && Util.UsuarioLogadoFBA() && ValidarPermissao)
                {
                    Boolean retorno = this.ValidarPagina();
                    if (!retorno)
                    {
                        this.Visible = false;
                        return;
                    }
                }
                // Caso o usuário seja Windows o mesmo poderá ver a tela(SharePoint) para alteração de conteúdo
                else if (this.SessaoAtual == null && !Util.UsuarioLogadoFBA())
                    return;
            }
        }

        /// <summary>
        /// Redireciona o usuário para a tela de sessão expirada.
        /// </summary>
        private void RedirecionarSessaoExpirada()
        {
            Response.Clear();
            Response.Redirect(SPContext.Current.Web.Url + "/_layouts/Redecard.Comum/SessaoExpirada.aspx");
            Response.End();
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
            if (base.Request.QueryString["source"] != null)
                base.Response.Redirect(base.Request.QueryString["source"]);
            else
                base.Response.Redirect(this.web.Url);
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
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="codigo">Código do erro</param>
        private void GeraPainelExcecao(String fonte, Int32 codigo)
        {
            using (Redecard.PN.Comum.TrataErroServico.TrataErroServicoClient trataErroServico = new Redecard.PN.Comum.TrataErroServico.TrataErroServicoClient())
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
            using (Redecard.PN.Comum.TrataErroServico.TrataErroServicoClient trataErroServico = new Redecard.PN.Comum.TrataErroServico.TrataErroServicoClient())
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
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="codigo">Código do erro</param>
        private void GeraPainelExcecao(String mensagem, String codigo)
        {
            Panel painel = new Panel();
            painel.ID = "pnlErroAutenticacao" + Guid.NewGuid().ToString();

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = "pnlMensagem" + Guid.NewGuid().ToString();
            div.Attributes.Add("class", "modal painelExcecao modalDados");

            StringBuilder sb = new StringBuilder();
            sb.Append("<p class='titModal'><span>");
            sb.Append(this.GetGlobalResourceObject("redecard", "titMensagemErro").ToString());
            sb.Append("</span></p><div class='boxDados png'><a href='#' class='closeModal' onclick='$(\".painelExcecao\").hide(); $(\"#bgProtecao\").hide();'>");
            //sb.Append(this.GetGlobalResourceObject("redecard", "ok").ToString());
            sb.Append("</a><fieldset class='formIndique'><div><h3>");
            sb.Append(mensagem);
            sb.Append("&nbsp;");
            sb.Append(String.Format("({0})", codigo));
            sb.Append("</h3></div></fieldset></div>");

            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("<script type='text/javascript'>");
            sbScript.Append("$(document).ready(function () {");
            sbScript.Append("$('#bgProtecao').show();");
            sbScript.Append("});</script>");

            div.InnerHtml = sb.ToString();

            painel.Controls.Add(div);
            this.Controls.Add(painel);
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, sbScript.ToString(), false);
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
            Control control = OcultarPanels(paineis);
            this.GerarPainelConfirmcaoAcao(titulo, mensagem, urlVoltar, control);
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
            Session["AvisoConfirmacaoAcao"] = "S";
            Session["TituloMensagem"] = titulo;
            Session["Mensagem"] = mensagem;
            Session["UrlVoltar"] = urlVoltar;

            Response.Redirect(SPContext.Current.Site.Url + "/Paginas/Confirmacao.aspx");

            //StringBuilder sb = new StringBuilder();
            //sb.Append("<br /><div id='pnlConfirmacao'><center>");
            //sb.Append("<table class='confirmaAcao'>");
            //sb.Append("<thead>");
            //sb.Append("<tr>");
            //sb.Append("<td>");
            //sb.Append(titulo);
            //sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("</thead>");
            //sb.Append("<tbody>");
            //sb.Append("<tr>");
            //sb.Append("<td>");
            //sb.Append(mensagem);
            //sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("</tbody>");
            //sb.Append("</table>");
            //sb.Append("<table width='100%'>");
            //sb.Append("<tr>");
            //sb.Append("<td align='right'>");
            //sb.Append("<div>");
            //sb.Append("<span class='btnConfirmaAcao voltar'><span class='btnLeft'><span class='btnRight'>");
            //sb.Append("<a class='voltar' href='" + urlVoltar + "'>Voltar</a>");
            //sb.Append("</span></span></span>");
            //sb.Append("</div>");
            //sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("</table>");
            //sb.Append("</div></center>");

            //// JavaScript para inserir o conteúdo dentro do container com Id =WpContent
            //StringBuilder sbScript = new StringBuilder();
            //sbScript.Append("<script type='text/javascript'>");
            //sbScript.Append("$(document).ready(function () {");
            //sbScript.Append("$('#WpContent').append(\"" + sb.ToString() + "\");");
            //sbScript.Append("});</script>");

            //this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, sbScript.ToString(), false);
        }
    }
    
}
