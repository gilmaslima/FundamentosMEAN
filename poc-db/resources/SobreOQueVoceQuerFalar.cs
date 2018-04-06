using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Redecard.Portal.Helper.Paginacao;

namespace Redecard.Portal.Aberto.WebParts.SobreOQueVoceQuerFalar
{
    [ToolboxItemAttribute(false)]
    public class SobreOQueVoceQuerFalar : System.Web.UI.WebControls.WebParts.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/SobreOQueVoceQuerFalar/SobreOQueVoceQuerFalarUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        private int quantidadePerguntasAMostrar = 5;

        /// <summary>
        /// Quantidade de perguntas a serem mostradas
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Quantidade de perguntas a mostrar")]
        [Description("Define a quantidade de perguntas que serão mostradas")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Quantidade de perguntas a mostrar")]
        public int QuantidadePerguntasAMostrar
        {
            get { return this.quantidadePerguntasAMostrar; }
            set { this.quantidadePerguntasAMostrar = value; }
        }
    }
}