using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Rede.PN.Eadquirencia.Sharepoint.Helper;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.RedirecionarEadquirencia
{
    [ToolboxItemAttribute(false)]
    public class RedirecionarEadquirencia : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public RedirecionarEadquirencia()
        {
        }

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Rede.PN.Eadquirencia.Sharepoint/RedirecionarEadquirencia/RedirecionarEadquirenciaUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        /// <summary>
        /// Recupera o valor atribuído na WebPart
        /// </summary>
        /// <summary>
        [WebBrowsable(true)]
        [WebDisplayName("URL do Eadquirencia - Front EC")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue("")]
        public String URLEadquirencia
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        String largura = "100%";

        /// <summary>
        /// Largura do Iframe
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("Largura do Iframe")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue("100%")]
        public string Largura
        {
            get
            {
                return largura;
            }
            set
            {
                largura = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        String altura = "200px";

        /// <summary>
        /// Altura do Iframe
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("Altura do Iframe")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue("200px")]
        public string Altura
        {
            get
            {
                return altura;
            }
            set
            {
                altura = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        String scroll = "auto";

        /// <summary>
        /// Scroll do Iframe
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("Scroll do Iframe")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue("auto")]
        public String Scroll
        {
            get
            {
                return scroll;
            }
            set
            {
                scroll = value;
            }
        }

        /// <summary>
        /// Tipo de Web Part
        /// </summary>
        TipoRedirecionamento _tipoRedirecionamento = TipoRedirecionamento.Default;
         [WebBrowsable(true)]
         [Personalizable(PersonalizationScope.Shared)]
         [Microsoft.SharePoint.WebPartPages.SPWebCategoryName("Configuração Redirecionamento")]
         [Microsoft.SharePoint.WebPartPages.FriendlyName("Tipo Configuração Redirecionamento")]
        public TipoRedirecionamento ConfiguracaoRedirecionamento
         {
             get
             {
                 return _tipoRedirecionamento;
             }
             set
             {
                 _tipoRedirecionamento = value;
             }
         }
    }
}
