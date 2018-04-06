using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Rede.PN.ApiLogin.Filters
{
    public class ValidacaoActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var estadoModelo = actionContext.ModelState;

            if (!estadoModelo.IsValid)
            {
                
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);

            }
        }
    }
}