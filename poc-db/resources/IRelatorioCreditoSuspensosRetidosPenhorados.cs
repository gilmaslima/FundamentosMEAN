using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRelatorioCreditoSuspensosRetidosPenhorados" in both code and config file together.
    [ServiceContract]
    public interface IRelatorioCreditoSuspensosRetidosPenhorados
    {
        #region WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos - Cartões de crédito.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarSuspensaoRetorno ConsultarSuspensaoPesquisaCredito(out StatusRetorno statusRetorno, ConsultarSuspensaoEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato);
        #endregion

        #region WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos - Cartões de débito.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarSuspensaoRetorno ConsultarSuspensaoPesquisaDebito(out StatusRetorno statusRetorno, ConsultarSuspensaoEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato);
        #endregion

        #region WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarPenhoraRetorno ConsultarPenhoraPesquisa(out StatusRetorno statusRetorno, ConsultarPenhoraEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato);
        #endregion

        #region WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarRetencaoRetorno ConsultarRetencaoPesquisa(out StatusRetorno statusRetorno, ConsultarRetencaoEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato);
        #endregion
    }
}
