using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Faixada para comunicação de Serviços HIS
    /// </summary>
    [ServiceContract]
    public interface IHISServicoZP_OutrosServicos
    {
        /// <summary>
        /// Lista os regimes no MainFrame acordo com o código de Regime
        /// </summary>
        /// <param name="regimes">Lista de regimes disponíveis</param>
        /// <param name="codigoServico">Código de Serviço</param>
        /// <returns>Código de retorno
        /// Erros codigoRetorno: 0 - RETORNO OK. 1 - ERRO DE PARAMETROS. 2 - ERRO DE ARQUIVOS
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int16 ListarRegime(out RegimeFranquia[] regimesFranquia, Int16 codigoServico);

        /// <summary>
        /// Consulta informações do Regime
        /// </summary>
        /// <param name="codigoRetorno">Código de mensagem de erro</param>
        /// <param name="numeroPV">Número do Estabelecimento</param>
        /// <returns>Regime de franquia com:
        /// Código de Serviço = 1</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        RegimeFranquia ConsultarRegime(out Int16 codigoRetorno, int numeroPV);
    }
}
