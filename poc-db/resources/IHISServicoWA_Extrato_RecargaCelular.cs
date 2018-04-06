/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Recarga;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Interface para acesso ao componente WA utilizado no módulo Extrato - Relatório de Recarga
    /// de Celular - Detalhes.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book BKWA2420 / Programa WA242 / TranID ISIB / Método ConsultarRecargaCelularDetalhe
    /// </remarks>    
    [ServiceContract]
    public interface IHISServicoWA_Extrato_RecargaCelular
    {
         #region [ Recarga de Celular - Detalhes - BKWA2420 / WA242 / ISIB ]

        /// <summary>
        /// Consulta utilizada no Relatório de Recarga de Celular - Detalhes,
        /// com pesquisa através do Resumo de Vendas.<br/>
        /// - Book BKWA2420 / Programa WA242 / TranID ISIB
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="rv">Número do resumo de venda</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Recarga de Celular - Detalhes.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalhePorResumo(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicial,
            Int32 pv,
            Int32 rv,
            out StatusRetorno status);

         /// <summary>
        /// Consulta utilizada no Relatório de Recarga de Celular - Detalhes,
        /// com pesquisa através do Período.<br/>
        /// - Book BKWA2420 / Programa WA242 / TranID ISIB
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Recarga de Celular - Detalhes.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalhePorPeriodo(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicial,
            DateTime dataFinal,
            Int32 pv,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada no Relatório de Recarga de Celular - Detalhes.<br/>
        /// - Book BKWA2420 / Programa WA242 / TranID ISIB
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="rv">Número do resumo de venda</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Recarga de Celular - Detalhes.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicial,
            DateTime? dataFinal,
            Int32 pv,
            Int32? rv,
            out StatusRetorno status);

        #endregion
    }
}
