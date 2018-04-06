/*
© Copyright 2014 Rede S.A.
Autor : William Santos
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

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.RecNovaSenhaBasicoMob
{
    [ToolboxItemAttribute(false)]
    public class RecNovaSenhaBasicoMob : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile/RecNovaSenhaBasicoMob/RecNovaSenhaBasicoUserMobControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);

            if (control != null)
                ((RecNovaSenhaBasicoMobUserControl)control).WebPartNovaSenha = this;

            Controls.Add(control);
        }

        /// <summary>
        /// Atributo costumizado para verificar se a criação de acesso será para recuperar Usuário Legado
        /// </summary>
        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Conclusão Básica"),
        WebDescription("Indicador se a recuperação de usuário foi básico ou não.")]
        public Boolean ConclusaoBasico { get; set; }
    }
}
