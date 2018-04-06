using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    [ServiceContract]
    public interface IConsultaPorTransacao
    {
        #region WACA1116 - Consultar por transação - Carta.
        /// <summary>
        /// WACA1116 - Consultar por transação - Carta.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ConsultarCartasRetorno> ConsultarCartas(out StatusRetorno statusRetorno, ConsultarCartasEnvio envio);
        #endregion
    }
}
