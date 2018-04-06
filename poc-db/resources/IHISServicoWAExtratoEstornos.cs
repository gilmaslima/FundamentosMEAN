/*
© Copyright 2015 Rede S.A.
Autor : Dhouglas Lombello
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Extrato.Servicos.Modelo;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Estornos.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book BKWA2930 / Programa WAC293 / TranID WAAP / Método ConsultarTotalizadores<br/>
    /// - Book BKWA2940 / Programa WAC294 / TranID WAAQ / Método Consultar
    /// </remarks>
    [ServiceContract,
    ServiceKnownType(typeof(BasicContract)),
    ServiceKnownType(typeof(EstornoTotalizador)),
    ServiceKnownType(typeof(Estorno)),
    ServiceKnownType(typeof(EstornoD))]
    public interface IHISServicoWAExtratoEstornos
    {
        /// <summary>
        /// Consulta de registros do Relatório de Estornos.<br/>
        /// - Book BKWA2940 / Programa WAC294 / TranID WAAQ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2940 / Programa WAC294 / TranID WAAQ
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="tipoModalidade">Tipo da Modalidade da pesquisa</param>
        /// <param name="codigoTipoVenda">Código do Tipo de Venda da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void ConsultarRelatorioEstorno(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            DateTime dataInicial,
            DateTime dataFinal,
            Int16 tipoModalidade,
            Int16 codigoTipoVenda,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out EstornoTotalizador totalizador,
            out List<Estorno> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);
    }
}
