﻿using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.ConfirmacaoPositivaResp
{
    [ToolboxItemAttribute(false)]
    public class ConfirmacaoPositivaResp : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile/ConfirmacaoPositiva/ConfirmacaoPositivaUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            if (control != null)
            {
                ((ConfirmacaoPositivaUserControl)control).WebPartRecuperacao = this;
            }
            Controls.Add(control);
        }

        /// <summary>
        /// Atributo costumizado para verificar se a identificação será para recuperar o Usuário
        /// </summary>
        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Recuperação Usuário"),
        WebDescription("Indicador se identificação será para Recuperar Usuário. Caso não seja seguirá o processo de Recuperação de Senha")]
        public Boolean RecuperacaoUsuario { get; set; }

    }
}
