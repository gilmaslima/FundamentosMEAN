#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Alexandre Shiroma]
Empresa     : [Iteris]
Histórico   :
- [24/07/2012] – [Alexandre Shiroma] – [Criação]
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Interface de serviço para acesso ao componente XD do módulo Request.<br/>
    /// Referente ao Tipo de Venda Débito.
    /// </summary>
    /// <seealso cref="IHISServicoXA_Request"/>
    [ServiceContract]
    public interface IHISServicoXD_Request
    {
        /// <summary>
        /// Consulta solicitações de processos pendentes.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache,
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXD201CB / Programa XD201 / TranID XDS1
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.ConsultarDebitoPendente"/>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="dataInicio">Data inicial a ser considerada</param>
        /// <param name="dataFim">Data final a ser considerada</param>
        /// <param name="sistemaOrigem">Sistema de Origem (Exemplo: "IS", "IZ")</param>
        /// <param name="codigoTransacao">Transação (Exemplo: "XDS1")</param>
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Solicitações de processos pendentes.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Comprovante> Cached_ConsultarDebitoPendente(            
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            DateTime dataInicio,
            DateTime dataFim,
            String sistemaOrigem,
            String codigoTransacao,            
            out Int32 qtdTotalRegistrosCache,
            out Int16 codigoRetorno);

        /// <summary>
        /// Consulta histórico de solicitações.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXD203CB / Programa XD203 / TranID XDS3
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.ConsultarHistoricoSolicitacoes"/>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="numeroProcesso">Número do processo</param>
        /// <param name="numeroPV">Código do Estabelecimento / Número do PV</param>
        /// <param name="dataInicio">Data inicial do período a ser considerado na consulta</param>
        /// <param name="dataFim">Data final do período a ser considerado na consulta</param>
        /// <param name="origem">Origem (Exemplo: "IS" - Portal / "IZ" - Intranet)</param>
        /// <param name="transacao">Código da transação (Exemplo: "XDS3")</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Histórico dos comprovantes.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Comprovante> Cached_ConsultarHistoricoSolicitacoes(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Decimal numeroProcesso,
            Int32 numeroPV,
            DateTime dataInicio,
            DateTime dataFim,
            String origem,
            String transacao,
            out Int32 qtdTotalRegistrosCache,
            out Int16 codigoRetorno);

        /// <summary>
        /// Consulta de avisos de débito.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="HISServicoXD_Request.ConsultarAvisosDebito"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXD204CB / Programa XD204 / TranID XDS4
        /// </remarks>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="dataInicio">Data inicial do período a ser considerado na consulta</param>
        /// <param name="dataFim">Data final do período a ser considerado na consulta</param>
        /// <param name="origem">Sistema de Origem ("IS" - Portal / "IZ" - Intranet)</param>
        /// <param name="transacao">Código da transação (Exemplo: "XDS4")</param>
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno da consulta</param>
        /// <returns>Avisos de débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.AvisoDebito> Cached_ConsultarAvisosDebito(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Decimal codProcesso,
            Int32 codEstabelecimento,
            DateTime dataInicio,
            DateTime dataFim,
            String origem,
            String transacao,            
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta log de recebimento dos documentos.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXD202CB / Programa XD202 / TranID XDS2
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.ConsultarLogRecDoc"/>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="codEstabelecimento">Código do estabelecimento / PV</param>
        /// <param name="origem">Sistema de Origem ("IS" - Portal / "IZ" - Intranet)</param>
        /// <param name="transacao">Código da transação (Exemplo: "XDS2")</param>
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno da consulta</param>
        /// <returns>Log do recebimento de CV</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.RecebimentoCV> Cached_ConsultarLogRecDoc(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Decimal codProcesso,
            Int32 codEstabelecimento,
            String origem,
            String transacao,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta solicitações de processos pendentes.
        /// </summary>
        /// <seealso cref="HISServicoXD_Request.Cached_ConsultarDebitoPendente"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXD201CB / Programa XD201 / TranID XDS1
        /// </remarks>
        /// <param name="codEstabelecimento">Código do estabelecimento / PV</param>
        /// <param name="dataInicio">Data inicial a ser considerada</param>
        /// <param name="dataFim">Data final a ser considerada</param>
        /// <param name="sistemaOrigem">Sistema de Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XDS1")</param>
        /// <param name="indicadorPesquisa">
        /// Indicador de pesquisa:
        ///     'N' ou ' ' = Primeira chamada
        ///     'P'        = Próxima chamada
        ///     'I'        = Pesquisa intervalo</param>
        /// <param name="processosInicioFim">Marcadores dos processos retornados (início e fim), para execução de consultas complementares</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>        
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Solicitações de processos pendentes</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Comprovante> ConsultarDebitoPendente(
            Int32 codEstabelecimento,
            DateTime dataInicio,
            DateTime dataFim,
            String sistemaOrigem,
            String transacao,
            String indicadorPesquisa,
            ref Servicos.ProcessosInicioFim processosInicioFim,
            ref Boolean possuiMaisRegistros,
            ref Int16 qtdOcorrencias,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta histórico de solicitações.
        /// </summary>
        /// <seealso cref="HISServicoXD_Request.Cached_ConsultarHistoricoSolicitacoes"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXD203CB / Programa XD203 / TranID XDS3
        /// </remarks>
        /// <param name="numeroProcesso">Número do processo</param>
        /// <param name="numeroPV">Código do Estabelecimento / Número do PV</param>
        /// <param name="dataInicio">Data inicial do período a ser considerado na consulta</param>
        /// <param name="dataFim">Data final do período a ser considerado na consulta</param>
        /// <param name="origem">Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XDS3")</param>
        /// <param name="indicadorPesquisa">
        /// Indicador de pesquisa
        ///     ' ' ou 'N'    = Primeira Chamada
        ///     'P'           = Próxima Página
        ///     'I'           = Pesquisa Intervalo      
        /// </param>
        /// <param name="processosInicioFim">Marcadores dos processos retornados (início e fim), para execução de consultas complementares</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>
        /// <param name="qtdOcorrencias">Quantidade de Ocorrências</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Histórico de comprovantes</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Comprovante> ConsultarHistoricoSolicitacoes(
            Decimal numeroProcesso,
            Int32 numeroPV,
            DateTime dataInicio,
            DateTime dataFim,
            String origem,
            String transacao,
            String indicadorPesquisa,
            ref Servicos.ProcessosInicioFim processosInicioFim,
            ref Boolean possuiMaisRegistros,
            out Int16 qtdOcorrencias,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta log de recebimento dos documentos.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXD202CB / Programa XD202 / TranID XDS2
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.Cached_ConsultarLogRecDoc"/>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="codEstabelecimento">Código do estabelecimento / PV</param>
        /// <param name="origem">Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XDS2")</param>
        /// <param name="qtdOcorrencias">Quantidade de registros na área de retorno</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Log do recebimento de CV</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.RecebimentoCV> ConsultarLogRecDoc(
           Decimal codProcesso,
           Int32 codEstabelecimento,
           String origem,
           String transacao,           
           ref Int16 qtdOcorrencias,
           out Int32 codigoRetorno);

        /// <summary>
        /// Consulta do motivo do débito
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXD205CB / Programa XD205 / TranID XDS5
        /// </remarks>
        /// <param name="codMotivoDebito">Código do motivo do débito</param>
        /// <param name="origem">Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XDS5")</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Descrição do motivo do débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String ConsultarMotivoDebito(
            Int16 codMotivoDebito,
            String origem,
            String transacao,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta de avisos de débito.
        /// </summary>
        /// <seealso cref="HISServicoXD_Request.Cached_ConsultarAvisosDebito"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXD204CB / Programa XD204 / TranID XDS4
        /// </remarks>
        /// <param name="codProcesso">Código do Processo</param>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="dataIni">Data inicial do período a ser considerado na consulta</param>
        /// <param name="dataFim">Data final do período a ser considerado na consulta</param>
        /// <param name="origem">Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XDS4")</param>
        /// <param name="indicadorPesquisa">
        /// Indicador de pesquisa:
        ///     'N' ou ' ' = Primeira chamada
        ///     'P'        = Próxima chamada
        ///     'I'        = Pesquisa intervalo
        /// </param>
        /// <param name="processosInicioFim">Marcadores dos processos retornados (início e fim), para execução de consultas complementares</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Avisos de débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.AvisoDebito> ConsultarAvisosDebito(
           Decimal codProcesso,
           Int32 codEstabelecimento,
           DateTime dataIni,
           DateTime dataFim,
           String origem,
           String transacao,
           String indicadorPesquisa,
           ref Servicos.ProcessosInicioFim processosInicioFim,
           ref Boolean possuiMaisRegistros,
           out Int16 qtdOcorrencias,
           out Int32 codigoRetorno);

        /// <summary>
        /// Consulta o total de requests pendentes de Débito.
        /// Utilizado na HomePage Segmentada.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do book:
        /// - Book BKXD0791 / Programa XD791 / Transação XDHS
        /// </remarks>
        /// <param name="numeroPv">Número do Estabelecimento</param>
        /// <returns>Quantidade de requests pendentes Débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ConsultarTotalPendentesDebito(Int32 numeroPv);
    }
}