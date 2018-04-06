using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.Emissores.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IHISServicoWF_Emissores" in both code and config file together.
    [ServiceContract]
    public interface IHisServicoWfEmissores
    {
        /// <summary>
        /// Efetua a Solicitação de Tecnologia
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="entradaEmissao"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        bool EfetuarSolicitacao(int numEmissor, DadosEmissao entradaEmissao, out Int32 codigoRetorno, out String mensagemRetorno);

    }
}
