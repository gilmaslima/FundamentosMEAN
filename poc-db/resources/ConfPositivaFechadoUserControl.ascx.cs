using Redecard.PN.Comum;
using System;
using System.ComponentModel;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.ConfPositivaFechado
{
    /// <summary>
    /// Confirmacao Positiva User Control
    /// </summary>
    [ToolboxItemAttribute(false)]
    public partial class ConfPositivaFechadoUserControl : UserControlBase
    {
        /// <summary>
        /// Sobrescreve o Onload do Base para nao validar as permissoes
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnLoad(e);
        }

        /// <summary>
        /// Objeto de confirmação positiva
        /// </summary>
        private ConfirmacaoPositivaNovoAcesso ConfirmacaoPositiva
        {
            get
            {
                return (ConfirmacaoPositivaNovoAcesso)this.cpConfPositiva;
            }
        }

        /// <summary>
        /// Pagina que chamou.
        /// </summary>
        public String PreviousPage
        {
            get
            {
                if(ViewState["PreviousPageUrl"] != null)
                    return ViewState["PreviousPageUrl"].ToString();
                return string.Empty;
            }
            set
            {
                ViewState["PreviousPageUrl"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (HttpContext.Current.Session["PreviousPageUrl"] != null)
                {
                    PreviousPage = Session["PreviousPageUrl"].ToString();
                }
                else
                    PreviousPage = Util.BuscarUrlRedirecionamento("/sites/fechado/paginas/pn_home.aspx", Microsoft.SharePoint.Administration.SPUrlZone.Internet);
            }
        }

        /// <summary>
        /// Processo de liberação de Acesso Completo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Continuar(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Continuando Processo de liberação de Alteração de Domicílio bancário após recuperação de usuário e senha"))
            {
                try
                {
                    Boolean ok = this.ConfirmacaoPositiva.ConfirmarConfirmacaoPositiva();
                    if (ok)
                        Response.Redirect(PreviousPage);
                    else
                        return;
                }
                catch (ThreadAbortException ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }
        }
    }
}
