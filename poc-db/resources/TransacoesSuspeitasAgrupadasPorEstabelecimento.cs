﻿/*
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

namespace Redecard.PN.FMS.Sharepoint.WebParts.TransacoesSuspeitasAgrupadasPorEstabelecimento
{
    /// <summary>
    /// Este componente publica a classe TransacoesSuspeitasAgrupadasPorEstabelecimento, estendidada de WebPart, que expõe métodos para manipular as transações suspeitas agrupadas por estabelecimento.
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class TransacoesSuspeitasAgrupadasPorEstabelecimento : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.FMS.Sharepoint.WebParts/TransacoesSuspeitasAgrupadasPorEstabelecimento/TransacoesSuspeitasAgrupadasPorEstabelecimentoUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
