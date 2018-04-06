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

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.DadosIniciaisMob
{
    [ToolboxItemAttribute(false)]
    public class DadosIniciaisMob : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile/DadosIniciaisMob/DadosIniciaisMobUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);

            if (control != null)
                ((DadosIniciaisMobUserControl)control).WebPartDadosIniciais = this;

            Controls.Add(control);
        }


        /// <summary>
        /// Atributo costumizado para verificar se a criação de acesso será para recuperar Usuário Legado
        /// </summary>
        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Acesso Usuário Legado"),
        WebDescription("Indicador se a criação de acesso será para recuperar Usuário Legado. Caso não seja seguirá o processo de Criação de Novo Acesso")]
        public Boolean AcessoUsuarioLegado { get; set; }
    }
}
