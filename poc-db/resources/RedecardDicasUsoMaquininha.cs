﻿using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.Portal.Fechado.WebParts.RedecardDicasUsoMaquininha
{
    [ToolboxItemAttribute(false)]
    public class RedecardDicasUsoMaquininha : WebPart
    {

        #region Enumeração____________________

        #endregion

        #region Constantes____________________

        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Fechado.WebParts/RedecardDicasUsoMaquininha/RedecardDicasUsoMaquininhaUserControl.ascx";

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
