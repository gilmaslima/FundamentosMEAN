using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Redecard.Portal.Helper;
using System.Collections.Generic;
using Redecard.Portal.Helper.Conversores;
using Redecard.Portal.Helper.DTO;

namespace Redecard.Portal.Aberto.WebParts.RedecardPromocoes
{
    [ToolboxItemAttribute(false)]
    public class RedecardPromocoes : Microsoft.SharePoint.WebPartPages.WebPart
    {
        #region Variaveis

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/RedecardPromocoes/RedecardPromocoesUserControl.ascx";
        private string _promocao = string.Empty;

        #endregion

        #region Propriedades

        public string Promocao
        {
            get { return this._promocao; }
            set { this._promocao = value; }
        }

        #endregion

        #region Eventos

        /// <summary>
        /// Método disparado automaticamente para renderização do controles na área customizada
        /// </summary>
        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        /// <summary>
        /// Método para carregamento dos contêineres da parte de edição da WebPart
        /// </summary>
        /// <returns></returns>
        public override ToolPart[] GetToolParts()
        {
            ToolPart[] allToolParts = new ToolPart[3];

            allToolParts[0] = new WebPartToolPart();
            allToolParts[1] = new CustomPropertyToolPart();
            allToolParts[2] = new GerenciamentoPromocoesToolPart();

            return allToolParts;
        }

        #endregion
    }
}