using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace Redecard.Portal.Aberto.WebParts.ExibicaoDeSons
{
    [ToolboxItemAttribute(false)]
    public class ExibicaoDeSons : System.Web.UI.WebControls.WebParts.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/ExibicaoDeSons/ExibicaoDeSonsUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        private bool exibirInfoSom = true;

        /// <summary>
        /// Mostrar informações (Título, Data e Descrição) da foto selecionada 
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Mostrar informações do Som")]
        [Description("Define se exibe ou não as informações de Som como Título, Data e Descrição")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Mostrar informações do Som")]
        public bool ExibirInfoSom
        {
            get { return this.exibirInfoSom; }
            set { this.exibirInfoSom = value; }
        }
    }
}