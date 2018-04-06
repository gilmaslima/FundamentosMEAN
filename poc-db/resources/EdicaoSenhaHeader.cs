using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DataCash.SharePoint.WebParts.EdicaoSenhaHeader
{
    [ToolboxItemAttribute(false)]
    public class EdicaoSenhaHeader : WebPart, IEdicaoSenhaConnector
    {
        Control control;
        
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DataCash.SharePoint.WebParts/EdicaoSenhaHeader/EdicaoSenhaHeaderUserControl.ascx";

        protected override void CreateChildControls()
        {
            control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }
        
        [ConnectionProvider("Edição Senha Connector - Provider")]
        public IEdicaoSenhaConnector GetProvider()
        {
            return this;
        }

        public void CarregarHeader(Boolean sucessoTrocaSenha)
        {
            ((EdicaoSenhaHeaderUserControl)control).CarregarHeader(sucessoTrocaSenha);
        }
    }
}
