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

namespace Redecard.PN.FMS.Sharepoint.WebParts.AnalisaTransacoesSuspeitas
{
    /// <summary>
    /// Este componente publica a classe AnalisaTransacoesSuspeitas, estendidada de WebPart, que expõe métodos para manipular as análises de transação suspeita.
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class AnalisaTransacoesSuspeitas : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.FMS.Sharepoint.WebParts/AnalisaTransacoesSuspeitas/AnalisaTransacoesSuspeitasUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
            
        }

        
    }
}
