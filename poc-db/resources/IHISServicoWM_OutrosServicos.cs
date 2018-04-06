using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.OutrosServicos.Servicos
{
    [ServiceContract]
    public interface IHISServicoWM_OutrosServicos
    {
        /// <summary>
        /// Retorna a carta gerada no mainframe para a solicitação cadastrada
        /// </summary>
        /// <param name="numeroSolicitacao">Número de solicitação</param>
        /// <param name="codigoTipoServico">Código do tipo de Serviço</param>
        /// <param name="linhasCarta">Objeto com o conteúdo da carta em linhas</param>
        /// <param name="quantidadeLinhasCarta">Quantidade de linhas na carta</param>
        /// <returns>Código de Retorno do mainframe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int16 Manutencao(Decimal numeroSolicitacao, String codigoTipoServico, out CartaSolicitacao linhasCarta, out Int16 quantidadeLinhasCarta);
    }
}
