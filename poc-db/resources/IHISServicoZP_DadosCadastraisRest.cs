using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IHISServicoZP_DadosCadastrais" in both code and config file together.
    [ServiceContract]
    public interface IHISServicoZP_DadosCadastraisRest
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
           Method = "POST",
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ObterTecnologia")]
        TerminalBancarioListRest ObterTecnologia(ObterTecnologiaRequest obterTecnologia);
    }
}
