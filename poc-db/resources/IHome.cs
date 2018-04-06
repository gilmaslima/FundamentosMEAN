using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    [ServiceContract]
    public interface IHome
    {
        #region WACA1107 - Home - Últimas Vendas.
        /// <summary>
        /// WACA1107 - Home - Últimas Vendas.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ConsultarTransacoesCreditoDebitoRetorno> ConsultarTransacoesCreditoDebito(out StatusRetorno statusRetorno, ConsultarTransacoesCreditoDebitoEnvio envio);
        #endregion

        #region WACA1108 - Home - Lançamentos futuros.
        /// <summary>
        /// WACA1108 - Home - Lançamentos futuros.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ConsultarCreditoDebitoRetorno> ConsultarCreditoDebito(out StatusRetorno statusRetorno, ConsultarCreditoDebitoEnvio envio);
        #endregion
    }
}
