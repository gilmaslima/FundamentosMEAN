/*
© Copyright 2015 Rede S.A.
Autor : Juliano Marcon
Empresa : Rede
*/
using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.WP1
{
    [ToolboxItemAttribute(false)]
    public class WP1 : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile/WP1/WP1UserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
