using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Redecard.PN.Extrato.SharePoint.Modelo
{
    public class FormlessPage : Page
    {
        public override void VerifyRenderingInServerForm(Control control) { }

        public override bool EnableEventValidation
        {
            get { return false; }
            set { }
        }
    }
}
