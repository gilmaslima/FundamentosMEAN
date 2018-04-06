#region Histórico do Arquivo
/*
(c) Copyright [2015] Rede
Autor       : [Daniel Torres]
Empresa     : [Iteris]
Histórico   :
- [18/03/2015] – [Daniel Torres] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Interface de serviço para acesso ao componente XB do módulo Request.<br/>
    /// Referente ao Tipo de Venda Débito.
    /// </summary>
    /// <seealso cref="IHISServicoXA_Request"/>
    [ServiceContract]
    public interface IHISServicoXBChargeback
    {
        //XA Idêntico e/ou XA + XD Com parâmetro novo

        /// <summary>
        /// Consulta do motivo do débito
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BKXB411 / Programa XBS411 / TranID XB86
        /// </remarks>
        /// <param name="codMotivoDebito">Código do motivo do débito</param>
        /// <param name="origem">Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XB86")</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Descrição do motivo do débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String ConsultarMotivoDebito(
            Int16 codMotivoDebito,
            String origem,
            String transacao,
            out Int32 codigoRetorno);


        //XA + XD Juntos (XA791 + XD791)
        /// <summary>
        /// Consulta o total de requests pendentes de Débito.
        /// Utilizado na HomePage Segmentada.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do book:
        /// - Book BKXB422 / Programa XBS422 / Transação XB97
        /// </remarks>
        /// <param name="numeroPv">Número do Estabelecimento</param>
        /// <param name="tipoProduto">Tipo do produto</param>
        /// <returns>Quantidade de requests pendentes Débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ConsultarTotalPendentes(Int32 numeroPv, Int16 tipoProduto);

        // XA380
        /// <summary>
        /// Consulta de canal de um estabelecimento.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BKXB412 / Programa XBS412 / TranID XB87
        /// </remarks>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="codigoOcorrencia">Código de ocorrência. Diferente de 0 indica que houve erro</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Canal do estabelecimento</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.Canal ConsultarCanal(
            Int32 codEstabelecimento,
            String origem,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno);

        //  XA390
        /// <summary>
        /// Atualiza o Canal de recebimento de avisos.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BKXB413 / Programa XBS413 / TranID XB88
        /// </remarks>
        /// <param name="codEstabelecimento">Código do estabelecimento</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="canalRecebimento">Canal de recebimento</param>
        /// <param name="msgRetorno">Mensagem de retorno</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarCanal(
            Int32 codEstabelecimento,
            CanalRecebimento canalRecebimento,
            String origem,
            out String msgRetorno);

        //  XA740
        /// <summary>
        /// Retorna a composição do RV.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BKXB414 / Programa XBS414 / TranID XB89
        /// </remarks>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="filler">Filler</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="codigoOcorrencia">Código de ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Composição RV, com as parcelas e valores</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.ComposicaoRV ComposicaoRV(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno);

        // XA760

        /// <summary>
        /// Consulta do Log de Recebimento de CV.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="IHISServicoXA_Request.RecebimentoCV"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BKXB415 / Programa XBS415 / TranID XB90
        /// </remarks>
        /// <param name="idPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Log de recebimento de CV</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.RecebimentoCV> RecebimentoCV(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);
        
        //  XD202
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
        /// <param name="idPesquisa">Identificador da pesquisa</param>
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
        List<Servicos.RecebimentoCV> ConsultarLogRecDoc(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Decimal codProcesso,
            Int32 codEstabelecimento,
            String origem,
            String transacao,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);


        //XA Com mudanças

        //BXA770
        /// <summary>
        /// Consulta de avisos de débito pendentes.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="IHISServicoXA_Request.ConsultarDebitoPendente"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BKXB416 / Programa XBS416 / TranID XB91
        /// </remarks>
        /// <param name="idPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Avisos de débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.AvisoDebito> ConsultarDebitoPendente(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            String programa,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);

        //BXA780
        /// <summary>
        /// Consulta do histórico de comprovantes de crédito.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="IHISServicoXA_Request.HistoricoRequest"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BKXB417 / Programa XBS417 / TranID XB92
        /// </remarks>
        /// <param name="idPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="dataIni">Data de início do período a ser consultado</param>
        /// <param name="dataFim">Data final do período a ser consultado</param>
        /// <param name="codProcesso">Código do processo a ser consultado</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Histórico dos comprovantes</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Comprovante> HistoricoRequest(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            String programa,
            DateTime dataIni,
            DateTime dataFim,
            Decimal codProcesso,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);

        //XA790
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Comprovante> ConsultarRequestPendente(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            String programa,
            String origem,
            Decimal? processo,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);

        //XD201
        /// <summary>
        /// Consulta solicitações de processos pendentes.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache,
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BKXB419 / Programa XBS419 / TranID XB94
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.ConsultarDebitoPendente"/>
        /// <param name="idPesquisa">Identificador da pesquisa</param>
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
        List<Servicos.Comprovante> ConsultaSolicitacaoPendente(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            DateTime dataInicio,
            DateTime dataFim,
            String sistemaOrigem,
            String codigoTransacao,
            Decimal? processo,
            out Int32 qtdTotalRegistrosCache,
            out Int16 codigoRetorno);

        //XD203

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
        /// <param name="idPesquisa">Identificador da pesquisa</param>
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
        List<Servicos.Comprovante> ConsultarHistoricoSolicitacoes(
            Guid idPesquisa,
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

        //XD204
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
        /// <param name="idPesquisa">Identificador da pesquisa</param>
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
        List<Servicos.AvisoDebito> ConsultarAvisosDebito(
            Guid idPesquisa,
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
    }
}