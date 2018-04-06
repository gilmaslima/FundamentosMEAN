using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRelatorioDebitosDesagendamentos" in both code and config file together.
    [ServiceContract]
    public interface IRelatorioDebitosDesagendamentos
    {
        #region WACA150 - Relatório de débitos e desagendamentos - Consolidado.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarConsolidadoDebitosEDesagendamentoRetorno ConsultarConsolidadoDebitosEDesagendamentoPesquisa(out StatusRetorno statusRetorno, ConsultarConsolidadoDebitosEDesagendamentoEnvio envio);
        #endregion

        #region WACA151 - Relatório de débitos e desagendamentos - Detalhe.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarDetalhamentoDebitosRetorno ConsultarDetalhamentoDebitosPesquisa(out StatusRetorno statusRetorno, ConsultarDetalhamentoDebitosEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato);
        #endregion

        #region WACA152 - Relatório de débitos e desagendamentos - Motivo débito.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarMotivoDebitoRetorno ConsultarMotivoDebito(out StatusRetorno statusRetorno, ConsultarMotivoDebitoEnvio envio);
        #endregion
    }
}
