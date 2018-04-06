using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.QuadrosAtalhos
{
    [ToolboxItemAttribute(false)]
    public class QuadrosAtalhos : WebPart
    {
        /// <summary>
        /// 
        /// </summary>
        private String _tituloQuadro = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        [Category("Propriedade")]
        [Personalizable(PersonalizationScope.Shared)]
        [WebBrowsable(true)]
        [WebDisplayName("Título do Quadro")]
        public string TituloQuadro
        {
            get
            {
                return _tituloQuadro;
            }
            set
            {
                _tituloQuadro = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/PN.DadosCadastrais.WP/QuadrosAtalhos/QuadrosAtalhosUserControl.ascx";

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateChildControls()
        {
            QuadrosAtalhosUserControl control = Page.LoadControl(_ascxPath) as QuadrosAtalhosUserControl;
            control.TituloQuadro = TituloQuadro;
            control.ValidarPermissao = false;
            Controls.Add(control);
        }
    }
}
