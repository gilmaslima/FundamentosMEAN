using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.Portal.Aberto.WebParts.RedecardRedirecionamento
{
    [ToolboxItemAttribute(false)]
    public class RedecardRedirecionamento : WebPart
    {
        #region Constantes____________________
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/RedecardRedirecionamento/RedecardRedirecionamentoUserControl.ascx";
        #endregion

        #region Propriedades__________________

        /// <summary>
        /// URL do Destino
        /// </summary>
        string _urlDestino = @"https://services.redecard.com.br/novoportal/portals/servico/Cef_Consulta.asp";

        [WebBrowsable(true)]
        [WebDisplayName("URL do Destino")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configuracoes")]
        [DefaultValue(@"https://services.redecard.com.br/novoportal/portals/servico/Cef_Consulta.asp")]
        public string urlDestino
        {
            get
            {
                return _urlDestino;
            }
            set
            {
                _urlDestino = value;
            }
        }

        /// <summary>
        /// Largura do Iframe
        /// </summary>
        string _largura = "484";
        [WebBrowsable(true)]
        [WebDisplayName("Largura")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configuracoes")]
        [DefaultValue("484")]
        public string Largura
        {
            get
            {
                return _largura;
            }
            set
            {
                _largura = value;
            }
        }

        /// <summary>
        /// Altura do Iframe
        /// </summary>
        string _altura = "350";
        [WebBrowsable(true)]
        [WebDisplayName("Altura")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configuracoes")]
        [DefaultValue("350")]
        public string Altura
        {
            get
            {
                return _altura;
            }
            set
            {
                _altura = value;
            }
        }

        #endregion

        #region Métodos_______________________

        #endregion

        #region Eventos_______________________

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        #endregion
    }
}