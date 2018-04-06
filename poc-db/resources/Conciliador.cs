using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebPartPages;
using System.Web;
using System.Linq;
using System.Web.SessionState;

namespace Rede.PN.Conciliador.SharePoint {
    /// <summary>
    /// 
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class Conciliador : Microsoft.SharePoint.WebPartPages.WebPart {
        /// <summary>
        /// 
        /// </summary>
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Rede.PN.Conciliador/Conciliador/ConciliadorUserControl.ascx";
        /// <summary>
        /// 
        /// </summary>
        protected override void CreateChildControls() {
            Control control = null;
            control = Page.LoadControl(ascxPath);
            Controls.Add(control);
        }
    }
}