using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Diagnostics;
using System.Collections.Generic;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    public partial class ConsultaULS : SustentacaoApplicationPageBase
    {
        #region [ Propriedades ]

        #endregion

        /// <summary>
        /// Page_Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            var ulsLogEntries = new SPULSRetriever(360, 100, DateTime.Now.AddDays(-1))
                .GetULSEntries(new Guid("bc54f759-ee58-4c55-9ef8-258d95d1dce6"));

            grvLog.DataSource = ulsLogEntries;
            grvLog.DataBind();


        }


    }
}
