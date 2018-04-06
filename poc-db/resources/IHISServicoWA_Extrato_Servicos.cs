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
using Redecard.PN.Extrato.Servicos.Servicos;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Serviços.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1210 / Programa WA1210 / TranID ISFK / Método ConsultarSerasaAvs<br/>
    /// - Book WACA1260	/ Programa WA1260 / TranID ISGH / Método ConsultarGateway<br/>
    /// - Book WACA1261	/ Programa WA1261 / TranID ISGI / Método ConsultarBoleto<br/>
    /// - Book WACA1262	/ Programa WA1262 / TranID ISGJ / Método ConsultarAnaliseRisco<br/>
    /// - Book WACA1263	/ Programa WA1263 / TranID ISGK / Método ConsultarManualReview<br/>
    /// - Book BKWA1260 / Programa WA2400 / TranID ISGH / Método ConsultarNovoPacote<br/>
    /// - Book BKWA2410 / Programa WA241  / TranID ISIA / Método ConsultarRecargaCelular
    /// </remarks>
    [ServiceContract]
    [ServiceKnownType(typeof(Serasa))]
    [ServiceKnownType(typeof(Avs))]
    public interface IHISServicoWA_Extrato_Servicos
    {
        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Gateway.<br/>
        /// - Book WACA1260 / Programa 1260 / TranID ISGH
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Gateway.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Gateway> ConsultarGateway(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,            
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,            
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - boleto.<br/>
        /// - Book WACA1261 / Programa 1261 / TranID ISGI
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Boleto.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Boleto> ConsultarBoleto(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,            
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Análise de Risco.<br/>
        /// - Book WACA1262 / Programa 1262 / TranID ISGJ
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Análise de Risco.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<AnaliseRisco> ConsultarAnaliseRisco(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,            
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Manual Review.<br/>
        /// - Book WACA1263 / Programa 1263 / TranID ISGK
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Manual Review.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ManualReview> ConsultarManualReview(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,            
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Novo Pacote.<br/>
        /// - Book BKWA1260 / Programa WA2400 / TranID ISGH
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Novo Pacote.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<NovoPacote> ConsultarNovoPacote(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,            
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Serasa.<br/>
        /// - Book WACA1210 / Programa WA1210 / TranID ISFK
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Serasa.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Serasa> ConsultarSerasa(
           Guid guidPesquisa,
           Int32 registroInicial,
           Int32 quantidadeRegistros,
           DateTime dataInicial,
           DateTime dataFinal,
           List<Int32> pvs,
           out Int32 quantidadeTotalRegistros,
           out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - AVS.<br/>
        /// - Book WACA1210 / Programa 1210 / TranID ISFK
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - AVS.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Avs> ConsultarAvs(
           Guid guidPesquisa,
           Int32 registroInicial,
           Int32 quantidadeRegistros,
           DateTime dataInicial,
           DateTime dataFinal,
           List<Int32> pvs,
           out Int32 quantidadeTotalRegistros,
           out StatusRetorno status);

         /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Recarga de Celular.<br/>
        /// - Book BKWA1260 / Programa WA241 / TranID ISIA
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Recarga de Celular.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<RecargaCelular> ConsultarRecargaCelular(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status);
    }
}
