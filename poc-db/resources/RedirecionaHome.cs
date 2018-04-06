using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Web;

namespace Redecard.PN.Comum.SharePoint.WebParts.RedirecionaHome
{
    [ToolboxItemAttribute(false)]
    public class RedirecionaHome : WebPart
    {
        /// <summary>
        /// 
        /// </summary>
        const string _camlQuery = "<Where><Eq><FieldRef Name=\"CodigoEntidade\" /><Value Type=\"Number\">{0}</Value></Eq></Where>";

        /// <summary>
        /// Obter a página inicial do grupo entidade de acordo com a lista e efetuar
        /// o redirecionamento 
        /// </summary>
        /// 
        protected override void OnLoad(EventArgs e)
        {
            // Redirecionar Home Page Vendedora por padrão
            String redirect = "/sites/fechado/Paginas/pn_Home.aspx";

            if (Sessao.Contem())
            {
                Sessao sessao = Sessao.Obtem();
                Int32 codigoEntidade = sessao.GrupoEntidade;

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    // instanciar lista
                    using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                    {
                        SPWeb web = site.RootWeb; // "/sites/fechado"
                        SPList lista = web.Lists.TryGetList("Página Inicial do Grupo Entidade");
                        if (!object.ReferenceEquals(lista, null))
                        {
                            SPQuery query = new SPQuery()
                            {
                                Query = String.Format(_camlQuery, codigoEntidade)
                            };

                            SPListItemCollection items = lista.GetItems(query);
                            if (items.Count > 0)
                            {
                                SPListItem item = items[0];
                                redirect = item["PaginaInicial"].ToString();
                            }
                        }
                    }
                });
            }

            // Efetuar redirecionamento
            HttpContext.Current.Response.Redirect(redirect, true);
        }
    }
}
