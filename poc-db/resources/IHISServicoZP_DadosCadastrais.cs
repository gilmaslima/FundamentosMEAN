using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IHISServicoZP_DadosCadastrais" in both code and config file together.
    [ServiceContract]
    public interface IHISServicoZP_DadosCadastrais
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.TerminalBancario> ObterTecnologia(Int32 codigoEntidade, Int32 dataPesquisa, out Int16 codigoRetorno);
    }
}
