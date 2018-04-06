/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 28/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.FMS.Sharepoint.WebParts.CadastroIPs
{
    /// <summary>
    /// Este componente publica a classe CadastroIPs, estendidada de WebPart, que expõe métodos para manipular os cadastros de ips autorizados.
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class CadastroIPs : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.FMS.Sharepoint.WebParts/CadastroIPs/CadastroIPsUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
