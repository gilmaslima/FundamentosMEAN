/*
© Copyright 2015 Rede S.A.
Autor : Felipe Siatiquosque
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

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.ConfirmacaoPositivaMob
{
    [ToolboxItemAttribute(false)]
    public class ConfirmacaoPositivaMob : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile/ConfirmacaoPositivaMob/ConfirmacaoPositivaMobUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
