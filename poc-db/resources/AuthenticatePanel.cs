using System.Web.UI.WebControls;

namespace Redecard.Portal.Aberto.WebControls {

    /// <summary>
    /// Controle customizado para inclusão de painel na página, este só é exibido
    /// se o usuário estiver autenticado
    /// </summary>
    public class AuthenticatePanel : Panel {

        /// <summary>
        /// Verifica se o painel é exibido na interface do usuário
        /// </summary>
        public override bool Visible {
            get {
                if (base.Visible && this.Page.User.Identity.IsAuthenticated && Page.User.Identity.Name.Contains("\\")) {
                    return true;
                }
                return false;
            }
            set {
                base.Visible = value;
            }
        }
    }
}