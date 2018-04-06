using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using Microsoft.SharePoint;
using System.Globalization;

namespace Redecard.Portal.Aberto.WebControls {

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeInformation : WebControl {

        //<p class="hoje">
        //{0}
        //</p>

        /// <summary>
        /// 
        /// </summary>
        string sInf = "{0}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer) {
            string sMonthName = CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
            sMonthName = CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(sMonthName);
            string sDtaTimeString = sMonthName + ", " + DateTime.Now.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern);
            string sRender = String.Format(sInf, sDtaTimeString);
            writer.Write(sRender);
        }
    }
}