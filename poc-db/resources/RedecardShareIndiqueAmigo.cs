using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.Portal.Aberto.WebParts.RedecardShareIndiqueAmigo
{
    [ToolboxItemAttribute(false)]
    public class RedecardShareIndiqueAmigo : WebPart
    {

        #region Enumeração____________________

        #endregion

        #region Constantes____________________

        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/RedecardShareIndiqueAmigo/RedecardShareIndiqueAmigoUserControl.ascx";

        #endregion

        #region Propriedades__________________

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
