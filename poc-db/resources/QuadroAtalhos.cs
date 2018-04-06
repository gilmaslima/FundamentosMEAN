/*
(c) Copyright 2014 Rede S.A.
Autor       : Alexandre Shiroma
Empresa     : Iteris
Histórico   :
- 03/11/2014] - Criação
*/

using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.Extrato.SharePoint.WebParts.QuadroAtalhos
{
    [ToolboxItemAttribute(false)]
    public class QuadroAtalhos : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.Extrato.SharePoint.WebParts/QuadroAtalhos/QuadroAtalhosUserControl.ascx";

        /// <summary>
        /// Título do Quadro
        /// </summary>
        private String tituloQuadro = String.Empty;

        /// <summary>
        /// Título do Quadro
        /// </summary>
        [Category("Propriedade")]
        [Personalizable(PersonalizationScope.Shared)]
        [WebBrowsable(true)]
        [WebDisplayName("Título do Quadro")]
        public string TituloQuadro
        {
            get { return tituloQuadro; }
            set { tituloQuadro = value; }
        }

        /// <summary>
        /// CreateChildControls
        /// </summary>
        protected override void CreateChildControls()
        {
            QuadroAtalhosUserControl control = Page.LoadControl(ascxPath) as QuadroAtalhosUserControl;
            control.TituloQuadro = TituloQuadro;
            control.ValidarPermissao = false;
            Controls.Add(control);
        }
    }
}
