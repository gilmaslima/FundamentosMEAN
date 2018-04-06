using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Configuration;
using System.Collections.Generic;

namespace Redecard.PN.OutrasEntidades.SharePoint.WebParts.RedecardRedirecionamentoLegado
{
    [ToolboxItemAttribute(false)]
    public class RedecardRedirecionamentoLegado : WebPart
    {

        #region Enumeração____________________

        #endregion

        #region Constantes____________________

        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.OutrasEntidades.RedirecionamentoLegado/RedecardRedirecionamentoLegadoUserControl.ascx";

        #endregion

        #region Propriedades__________________

        /// <summary>
        /// URL do Legado
        /// </summary>
        //[WebBrowsable(true)]
        //[WebDisplayName("URL do Legado")]
        //[Description()]
        //[Personalizable(PersonalizationScope.Shared)]
        //[Category("Configuracoes")]
        //[DefaultValue("")]
        //public string urlLegado {
        //    get;
        //    set;
        //}

        /// <summary>
        /// 
        /// </summary>
        string _urlLegado = string.Empty;

        /// <summary>
        /// Recupera o valor do web.config atráves do appSettings 'urlRedirecionamentoLegado'
        /// </summary>
        public string urlLegado
        {
            get
            {
                if (String.IsNullOrEmpty(_urlLegado))
                    _urlLegado = ConfigurationManager.AppSettings["urlRedirecionamentoLegado"];
                return _urlLegado;
            }
        }

        /// <summary>
        /// URL do Destino
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("URL do Destino")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configuracoes")]
        [DefaultValue("")]
        public string urlDestino
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        string _largura = "727px";

        /// <summary>
        /// Largura do Iframe
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("Largura")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configuracoes")]
        [DefaultValue("727px")]
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
        /// 
        /// </summary>
        string _altura = "200px";

        /// <summary>
        /// Altura do Iframe
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("Altura")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configuracoes")]
        [DefaultValue("200px")]
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
            // Verificar se o cookie de Confirmação Positiva existe no SharePoint, caso positivo,
            // redirecionar o usuário automaticamente para a página de Confirmação Positiva
            string sNEstabelecimento = string.Empty;
            string sNLogin = string.Empty;
            string sUrlConfirmacaoPositiva = "/novoportal/portals/servicoSharePoint/identificacaopositiva.asp?id=2&txtnu_pdv={0}&txtUsuario={1}";
            HttpCookieCollection cookies = this.Page.Request.Cookies;
            List<string> dicKeys = new List<string>();
            dicKeys.AddRange(cookies.AllKeys);
            if (dicKeys.Contains("needValidation"))
            {
                sNEstabelecimento = (dicKeys.Contains("NEstabelecimento") ? cookies["NEstabelecimento"].Value : string.Empty);
                sNLogin = (dicKeys.Contains("NLoginName") ? cookies["NLoginName"].Value : string.Empty);
                urlDestino = String.Format(sUrlConfirmacaoPositiva, sNEstabelecimento, sNLogin);
            }

            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        #endregion

    }
}
