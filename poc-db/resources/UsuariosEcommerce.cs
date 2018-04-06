using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DataCash.SharePoint.WebParts.UsuariosEcommerce
{
    [ToolboxItemAttribute(false)]
    public class UsuariosEcommerce : WebPart
    {
        Control control;
        IEdicaoSenhaConnector provider;

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DataCash.SharePoint.WebParts/UsuariosEcommerce/UsuariosEcommerceUserControl.ascx";

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

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //Se tem conexão com um provider do header de Edição de Senha, atualiza
            //dados do controle header
            if (provider != null)
                provider.CarregarHeader(((UsuariosEcommerceUserControl)control).SucessoTrocaSenha);
        }
        
        [ConnectionConsumer("Edição Senha Connector - Consumer")]
        public void RecieveProvider(IEdicaoSenhaConnector p)
        {
            provider = p;
        }
    }
}
