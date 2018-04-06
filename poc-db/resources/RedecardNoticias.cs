using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.Portal.Aberto.WebParts.RedecardNoticias
{
    [ToolboxItemAttribute(false)]
    public class RedecardNoticias : WebPart
    {

        #region Enumeração____________________

        public enum Visao : int
        {
            Resumo = 0,
            Detalhe = 1,
            Todos = 2
        }

        #endregion

        #region Propriedades__________________

        /// <summary>
        /// Quantidade de itens
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("Quantidade de itens")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue(4)]
        public int quantidadeItens
        {
            get;
            set;
        }

        /// <summary>
        /// URL de listagem de todos itens
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("URL de listagem de todos itens")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public string urlLista
        {
            get;
            set;
        }

        [WebBrowsable(true)]
        [WebDisplayName("Tipo de visão")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue(Visao.Resumo)]
        public Visao tipoVisao
        {
            get;
            set;
        }

        #endregion

        #region Constantes____________________

        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/RedecardNoticias/RedecardNoticiasUserControl.ascx";

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
