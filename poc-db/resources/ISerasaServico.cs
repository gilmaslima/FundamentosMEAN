using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.Credenciamento.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISerasaServico" in both code and config file together.
    [ServiceContract]
    public interface ISerasaServico
    {
        [OperationContract]
        [ServiceKnownType(typeof(Socio))]
        [ServiceKnownType(typeof(CodigoCNAE))]
        [FaultContract(typeof(GeneralFault))]
        PJ ConsultaSerasaPJ(String cnpj);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        PF ConsultaSerasaPF(String cpf);
    }
}
