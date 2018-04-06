using Newtonsoft.Json.Linq;
using System;
using System.Web.Http;

namespace Rede.PN.HealthCheck.Controllers
{
    public class StatusSistemaController : ApiController
    {
        // GET api/statussistema/idSistema
        public JObject GetStatus(String idSistema)
        {
            JObject retorno = new JObject();

            try
            {
                if (idSistema.Equals("GE", StringComparison.InvariantCultureIgnoreCase))
                {
                    retorno.Add("statusGE", new Models.SistemaParceiroBL().VerificarStatusGE());
                }
                else if (idSistema.Equals("TG", StringComparison.InvariantCultureIgnoreCase))
                {
                    retorno.Add("statusTG", new Models.SistemaParceiroBL().VerificarStatusTG());
                }
                else
                {
                    retorno.Add("statusGE", new Models.SistemaParceiroBL().VerificarStatusGE());

                    retorno.Add("statusTG", new Models.SistemaParceiroBL().VerificarStatusTG());
                }
            }
            catch (NullReferenceException ex)
            {
                if (retorno["statusGE"].Equals(null))
                    retorno.Add("statusGE", false);

                if (retorno["statusTG"].Equals(null))
                    retorno.Add("statusTG", false);

                retorno.Add("Error", String.Format("{0}\n{1}", ex.ToString(), ex.StackTrace));
            }
            catch (Exception ex)
            {
                if (retorno["statusGE"] == null)
                    retorno.Add("statusGE", false);

                if (retorno["statusTG"] == null)
                    retorno.Add("statusTG", false);

                retorno.Add("Error", String.Format("{0}\n{1}", ex.ToString(), ex.StackTrace));
            }

            return retorno;
        }
    }
}
