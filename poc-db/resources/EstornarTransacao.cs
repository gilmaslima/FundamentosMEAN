using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.EstornarTransacao
{
    [ToolboxItemAttribute(false)]
    public class EstornarTransacao : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public EstornarTransacao()
        {
        }

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Rede.PN.Eadquirencia.Sharepoint/EstornarTransacao/EstornarTransacaoUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(ascxPath);
            Controls.Add(control);
        }
    }
}
