using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRelatorioPagamentosAjustados" in both code and config file together.
    [ServiceContract]
    public interface IRelatorioPagamentosAjustados
    {
        #region WACA1106 - Relatório de pagamentos ajustados.
        /// <summary>
        /// WACA1106 - Relatório de pagamentos ajustados.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <param name="quantidadeRegistrosPorPagina"></param>
        /// <param name="guidPesquisa"></param>
        /// <param name="guidUsuarioCacheExtrato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarOrdensCreditoEnviadosAoBancoRetorno ConsultarOrdensCreditoEnviadosAoBancoPesquisa(out StatusRetorno statusRetorno, ConsultarOrdensCreditoEnviadosAoBancoEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato);

        #endregion
    }
}
