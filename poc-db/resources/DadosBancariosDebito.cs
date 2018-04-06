﻿using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.DadosBancariosDebito
{
    [ToolboxItemAttribute(false)]
    public class DadosBancariosDebito : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const String _ascxPath = @"~/_CONTROLTEMPLATES/PN.DadosCadastrais.WP/DadosBancariosDebito/DadosBancariosDebitoUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
