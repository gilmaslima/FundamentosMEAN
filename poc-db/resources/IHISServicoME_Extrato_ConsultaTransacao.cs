using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.ConsultaTransacao;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>    
    /// Serviço para acesso ao componente ME utilizado no módulo Extrato - Consulta por Transação.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book MEC084CO	/ Programa MEC084 / TranID IS89 / Método ConsultarDebito<br/>
    /// - Book MEC119CO	/ Programa MEC119 / TranID IS96 / Método ConsultarCredito<br/>
    /// - Book MEC323CO	/ Programa MEC323 / TranID IS99 / Método ConsultarCreditoTID<br/>
    /// - Book MEC324CO	/ Programa MEC324 / TranID IS88 / Método ConsultarDebitoTID<br/>
    /// </remarks>
    [ServiceContract]    
    public interface IHISServicoME_Extrato_ConsultaTransacao
    {
        /// <summary>
        /// Consulta de Transação de Débito, através do Número do Cartão ou NSU.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book: <br/>
        /// - Book MEC084CO / Programa MEC084 / TranID IS89
        /// </remarks>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="nsuAcquirer">Número do NSU</param>
        /// <param name="status">Saída: código de retorno/mensagem da consulta efetuada</param>
        /// <returns>Lista contendo as transações de débito encontradas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Debito> ConsultarDebito(
            Int32 numeroPV,
            DateTime dataInicial,
            DateTime dataFinal,
            String numeroCartao,
            Int64 nsuAcquirer,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de Transação de Crédito, através do Número do Cartão ou NSU.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book MEC119CO / Programa MEC119 / TranID IS96
        /// </remarks>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="nsu">Número do NSU</param>
        /// <param name="status">Saída: código de retorno/mensagem da consulta efetuada</param>
        /// <returns>Lista contendo as transações de crédito encontradas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Credito> ConsultarCredito(
            Int32 numeroPV,
            DateTime dataInicial,
            DateTime dataFinal,
            String numeroCartao,
            Int64 nsu,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de Transação de Crédito, através do Número de Identificação DataCash (TID).
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book MEC323CO / Programa MEC323 / TranID IS99
        /// </remarks>
        /// <param name="idDataCash">ID do DataCash</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno/mensagem da consulta efetuada</param>
        /// <returns>Transação de Crédito encontrada</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        CreditoTID ConsultarCreditoTID(            
            String idDataCash,
            Int32 numeroPV,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de Transação de Débito, através do Número de Identificação DataCash (TID).
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book MEC324CO / Programa MEC324 / TranID IS88
        /// </remarks>
        /// <param name="idDataCash">ID do DataCash</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno/mensagem da consulta efetuada</param>
        /// <returns>Transação de Débito encontrada</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        DebitoTID ConsultarDebitoTID(            
            String idDataCash,
            Int32 numeroPV,
            out StatusRetorno status);
    }
}