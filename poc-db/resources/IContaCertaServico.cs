using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IContaCertaServico" in both code and config file together.
    [ServiceContract]
    public interface IContaCertaServico
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.FilialTerminais> VerificaTerminalContaCerta(List<Servicos.FilialTerminais> filiais);
    }
}
