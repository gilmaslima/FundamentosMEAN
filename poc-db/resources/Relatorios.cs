using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.Extrato.SharePoint.WebParts.Relatorios
{
    [ToolboxItemAttribute(false)]
    public class Relatorios : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.Extrato.SharePoint.WebParts/Relatorios/RelatoriosUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Habilitar Conta Corrente"),
        WebDescription("Configuração para habilitar o relatório de Conta Corrente como principal"),
        DefaultValue(false)]
        public Boolean HabilitarContaCorrente { get; set; }
    }
}