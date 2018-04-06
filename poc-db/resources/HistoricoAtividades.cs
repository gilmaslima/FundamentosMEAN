using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.HistoricoAtividades
{
    [ToolboxItemAttribute(false)]
    public class HistoricoAtividades : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WebParts/HistoricoAtividades/HistoricoAtividadesUserControl.ascx";

        /// <summary>
        /// ActiveViewIndex da WebPart.
        /// </summary>
        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("ActiveViewIndex"),
        WebDescription("View que estará habilitada na página (0 - listagem, ou 1 - detalhamento de atividade)")]
        public Int32 ActiveViewIndex { get; set; }

        protected override void CreateChildControls()
        {
            var control = (HistoricoAtividadesUserControl)Page.LoadControl(_ascxPath);

            //Repassa a informação se a página é de edição ou criação de usuários
            control.ActiveViewIndex = this.ActiveViewIndex;

            Controls.Add(control);
        }
    }
}
