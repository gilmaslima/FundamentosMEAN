using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.Boston.Sharepoint.WebParts
{
    public class RedirecionaDataCashBaseWebpart : WebPart
    {
        /// <summary>
        /// Recupera o valor atribuído na WebPart
        /// </summary>
        /// <summary>
        [WebBrowsable(true)]
        [WebDisplayName("URL do DataCash")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue("")]
        public String URLDataCash
        {
            get;
            set;
        }
    }
}
