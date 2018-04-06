using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;

namespace Redecard.PN.DataCash.masterpage
{
    public partial class DataCash : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (Request.QueryString["dados"] != null)
                    {
                        QueryStringSegura qs = new QueryStringSegura(Request.QueryString["dados"]);
                        if(!String.IsNullOrEmpty(qs["url"]))
                            Session["urlParent"] = qs["url"];
                    }
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro durante leitura de QueryString", ex);
                    throw ex;
                }
            }

            if(String.IsNullOrEmpty(urlParent.Value))
                urlParent.Value = Session["urlParent"] as String;
        }
    }
}