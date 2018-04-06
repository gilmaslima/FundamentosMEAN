using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace Redecard.Portal.Aberto.WebParts.ExibicaoDeVideos
{
    [ToolboxItemAttribute(false)]
    public class ExibicaoDeVideos : System.Web.UI.WebControls.WebParts.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/ExibicaoDeVideos/ExibicaoDeVideosUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        private bool exibirInfoVideo = true;

        /// <summary>
        /// Mostrar informações (Título, Data e Descrição) da foto selecionada 
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Mostrar informações do Video")]
        [Description("Define se exibe ou não as informações de Vídeo como Título, Data e Descrição")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Mostrar informações do Video")]
        public bool ExibirInfoVideo
        {
            get { return this.exibirInfoVideo; }
            set { this.exibirInfoVideo = value; }
        }
    }
}
