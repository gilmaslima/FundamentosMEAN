using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    [ServiceContract]
    public interface IResumoVendas
    {
        #region WACA617 - Resumo de vendas - Cartões de débito.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ConsultarPreDatadosRetorno> ConsultarPreDatados(out StatusRetorno statusRetorno, ConsultarPreDatadosEnvio envio);
        #endregion

        #region WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ConsultarVencimentosResumoVendaRetorno> ConsultarVencimentosResumoVenda(out StatusRetorno statusRetorno, ConsultarVencimentosResumoVendaEnvio envio);
        #endregion

        #region WACA799 - Resumo de vendas - CDC.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarTransacaoDebitoRetorno ConsultarTransacaoDebito(out StatusRetorno statusRetorno, ConsultarTransacaoDebitoEnvio envio);
        #endregion

        #region WACA700/WACA701 - Resumo de vendas - Cartões de crédito.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ConsultarDetalhesResumoDeVendaRetorno> ConsultarDetalhesResumoDeVenda(out StatusRetorno statusRetorno, ConsultarDetalhesResumoDeVendaEnvio envio);
        #endregion

        #region WACA703 - Resumo de vendas - Cartões de crédito - Vencimentos.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ConsultarDisponibilidadeVencimentosODCRetorno> ConsultarDisponibilidadeVencimentosODC(out StatusRetorno statusRetorno, ConsultarDisponibilidadeVencimentosODCEnvio envio);
        #endregion

        #region WACA705 - Resumo de vendas - Cartões de crédito - CV's rejeitados.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ConsultarRejeitadosRetorno> ConsultarRejeitados(out StatusRetorno statusRetorno, ConsultarRejeitadosEnvio envio);
        #endregion
    }
}
