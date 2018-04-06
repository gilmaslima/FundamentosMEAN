/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.OutrosServicos.Servicos.Corban;

namespace Redecard.PN.OutrosServicos.Servicos
{    
    /// <summary>
    /// Serviço de acesso aos programas mainframe WA para o módulo Outros Serviços.
    /// </summary>
    [ServiceContract]
    public interface IHisServicoWaOutrosServicos
    {
        /// <summary>
        /// <para>Consulta de Totalizador de transações Corban</para>
        /// <para>Book: BKWA2650; Programa: WAC265; TRAN-ID: WAAD</para>
        /// </summary>
        /// <param name="codigoRetorno">
        /// <para>Código de retorno do programa</para>
        /// </param>
        /// <param name="quantidadeTotal">Quantidade Total de Transações</param>
        /// <param name="bandeirasTransacao">Totalizador de bandeiras</param>
        /// <param name="dataInicio">Data de início para filtragem</param>
        /// <param name="dataFinal">Data de fim para filtragem</param>
        /// <param name="tipoConta">Tipo de Contas para filtragem</param>
        /// <param name="formaPagamento">Forma de Pagamento para filtragem</param>
        /// <param name="statusCorban">Status da transação para filtragem</param>
        /// <param name="pvs">Lista de PVs para filtragem</param>
        /// <param name="codigoServico">Código do serviço</param>
        /// <returns>Totalizador Transações Corban</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TransacaoCorban ConsultarTotalizadorTransacoes(
            out Int16 codigoRetorno,
            out Int32 quantidadeTotal,
            out List<BandeiraTransacao> bandeirasTransacao,
            DateTime dataInicio,
            DateTime dataFinal,
            TipoConta tipoConta,
            FormaPagemento formaPagamento,
            StatusCorban statusCorban,
            Decimal codigoServico,
            Int32[] pvs);

        /// <summary>
        /// <para>Consulta as transações Corban</para>
        /// <para>Book: BKWA2660; Programa: WAC266; TRAN-ID: WAAE</para>
        /// </summary>
        /// <param name="codigoRetorno">
        /// <para>Código de retorno do programa</para>
        /// </param>
        /// <param name="guidPesquisa"></param>
        /// <param name="registroInicial"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="quantidadeTotalRegistros"></param>
        /// <param name="dataInicio">Data de início para filtragem</param>
        /// <param name="dataFinal">Data de fim para filtragem</param>
        /// <param name="tipoConta">Tipo de Contas para filtragem</param>
        /// <param name="formaPagamento">Forma de Pagamento para filtragem</param>
        /// <param name="statusCorban">Status da transação para filtragem</param>
        /// <param name="pvs">Lista de PVs para filtragem</param>
        /// <param name="codigoServico">Código do serviço</param>
        /// <returns>Transações Corban</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<TransacaoCorban> ConsultarTransacoes(
            out Int16 codigoRetorno,
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicio,
            DateTime dataFinal,
            TipoConta tipoConta,
            FormaPagemento formaPagamento,
            StatusCorban statusCorban,
            Decimal codigoServico,
            Int32[] pvs);
    }
}