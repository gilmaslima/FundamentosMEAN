using System.Web;
using System.Web.Mvc;

namespace Rede.PN.LoginPortal.CacheService
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}