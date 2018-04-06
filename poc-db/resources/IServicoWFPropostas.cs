using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.OutrasEntidades.Servicos
{
    /// <summary>
    /// Interface de serviço para acesso ao Sybase WF relacionado com propostas
    /// </summary>
    [ServiceContract]
    public interface IServicoWFPropostas
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.PropostaPorCNPJCPF> ConsultaPropostaPorCNPJCPF(Char codigoTipoPessoa, Int64 numeroCnpjCpf, Int32 indicadorSequenciaProp);

    }
}
