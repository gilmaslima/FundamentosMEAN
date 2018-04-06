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

namespace Redecard.PN.FMS.Sharepoint.WebParts.ConsultaTransacoesPorPeriodoUsuario
{
    /// <summary>
    /// Este componente publica a classe ConsultaTransacoesPorPeriodoUsuario, estendidada de WebPart, que expõe métodos para manipular as consultas de transações por período por usuário.
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class ConsultaTransacoesPorPeriodoUsuario : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.FMS.Sharepoint.WebParts/ConsultaTransacoesPorPeriodoUsuario/ConsultaTransacoesPorPeriodoUsuarioUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
