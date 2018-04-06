using System;
using System.Web.UI;

namespace Redecard.Portal.Aberto.WebParts.NossasBandeiras {
    public partial class NossasBandeirasUserControl : UserControl {

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private NossasBandeiras WebPart {
            get {
                return this.Parent as NossasBandeiras;
            }
        }

        /// <summary>
        /// Carregamento da web part, setar as informações de título e link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {
            NossasBandeiras bandeirasWp = this.WebPart;
            if (!object.ReferenceEquals(bandeirasWp, null)) {
                this.ltTitle.Text = bandeirasWp.WpTitle;
                this.ltLink.Text = bandeirasWp.Link;
                this.ltLinkDescription.Text = bandeirasWp.LinkDescription;
            }
        }
    }
}