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

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Interface de serviço para acesso ao componente XA do módulo Request.<br/>
    /// Referente aos Requests de Crédito.
    /// </summary>    
    /// <seealso cref="IHISServicoXD_Request"/>
    [ServiceContract]
    public interface IHISServicoXA_Request
    {
        /// <summary>
        /// Consulta de Comprovantes de Crédito Pendentes.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>        
        /// <seealso cref="IHISServicoXA_Request.ConsultarRequestPendente"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA790 / Programa XA790 / TranID IS68
        /// </remarks>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Lista contendo os Comprovantes de Crédito pendentes.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Comprovante> Cached_ConsultarRequestPendente(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta do Log de Recebimento de CV.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="IHISServicoXA_Request.RecebimentoCV"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA760 / Programa XA760 / TranID IS66
        /// </remarks>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
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
        List<Servicos.RecebimentoCV> Cached_RecebimentoCV(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta de avisos de débito pendentes.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="IHISServicoXA_Request.ConsultarDebitoPendente"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA770 / Programa XA770 / TranID IS67
        /// </remarks>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
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
        List<Servicos.AvisoDebito> Cached_ConsultarDebitoPendente(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta do histórico de comprovantes de crédito.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="IHISServicoXA_Request.HistoricoRequest"/>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA780 / Programa XA780 / TranID IS39
        /// </remarks>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="dataIni">Data de início do período a ser consultado</param>
        /// <param name="dataFim">Data final do período a ser consultado</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Histórico dos comprovantes</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Comprovante> Cached_HistoricoRequest(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            DateTime dataIni,
            DateTime dataFim,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta de Comprovantes de Crédito Pendentes.        
        /// </summary>        
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA790 / Programa XA790 / TranID IS68
        /// </remarks>
        /// <seealso cref="IHISServicoXA_Request.Cached_ConsultarRequestPendente"/>
        /// <param name="codEstabelecimento">Código do estabelecimento</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>                
        /// <param name="possuiMaisRegistros">Flag indicando o término de registros</param>
        /// <param name="codUltimoProcesso">Código indicando o último processo</param>
        /// <param name="qtdLinhasOcorrencia">Quantidade de linhas na ocorrência</param>
        /// <param name="qtdTotalOcorrencias">Quantidade total de ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Código de Ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Lista contendo os Comprovantes de Crédito pendentes.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Comprovante> ConsultarRequestPendente(
           Int32 codEstabelecimento,
           Decimal codProcesso,
           String programa,
           String origem,
           ref Boolean possuiMaisRegistros,
           ref Decimal codUltimoProcesso,
           ref Int16 qtdLinhasOcorrencia,
           ref Int32 qtdTotalOcorrencias,
           ref String filler,
           ref Int64 codigoOcorrencia,
           out Int32 codigoRetorno);

        /// <summary>
        /// Consulta de canal de um estabelecimento.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA380 / Programa XA380 / TranID IS63
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

        /// <summary>
        /// Consulta do histórico de comprovantes de crédito.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA780 / Programa XA780 / TranID IS39
        /// </remarks>
        /// <seealso cref="IHISServicoXA_Request.Cached_HistoricoRequest"/>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="programa">Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="dataIni">Data de início do período a ser consultado</param>
        /// <param name="dataFim">Data final do período a ser consultado</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>
        /// <param name="ultimoRegistro">Código do último processo</param>
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Código de Ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Histórico dos comprovantes</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Comprovante> HistoricoRequest(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            DateTime dataIni,
            DateTime dataFim,
            String origem,
            ref Boolean possuiMaisRegistros,
            ref Decimal ultimoRegistro,
            ref Int16 qtdOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta do Log de Recebimento de CV.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA760 / Programa XA760 / TranID IS66
        /// </remarks>
        /// <seealso cref="IHISServicoXA_Request.Cached_RecebimentoCV"/>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="codigoOcorrencia">Código de Ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Log de recebimento de CV</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.RecebimentoCV> RecebimentoCV(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            ref Boolean possuiMaisRegistros,
            ref Int16 qtdOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno);

        /// <summary>
        /// Retorna a composição do RV.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA740 / Programa XA740 / TranID IS69
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

        /// <summary>
        /// Consulta do motivo do débito.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA750 / Programa XA750 / TranID IS65
        /// </remarks>
        /// <param name="codigoMotivoDebito">Código do motivo do débito</param>
        /// <param name="codigoOcorrencia">Código de ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <param name="origem">Sistema de origem</param>
        /// <returns>Descrição do motivo do débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String MotivoDebito(
            Int32 codigoMotivoDebito,
            String origem,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno);

        /// <summary>
        /// Consulta de avisos de débito pendentes.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA770 / Programa XA770 / TranID IS67
        /// </remarks>
        /// <seealso cref="IHISServicoXA_Request.Cached_ConsultarDebitoPendente"/>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>
        /// <param name="codUltimoProcesso">Código do último processo retornado</param>
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Código de Ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Avisos de débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.AvisoDebito> ConsultarDebitoPendente(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            String origem,
            ref Boolean possuiMaisRegistros,
            ref Decimal codUltimoProcesso,
            ref Int16 qtdOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno);

        /// <summary>
        /// Atualiza o Canal de recebimento de avisos.
        /// </summary>
        /// <remarks>
        /// Realiza a consulta ao mainframe através do seguinte Book:<br/>
        /// - Book BXA390 / Programa XA390 / TranID IS64
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

        /// <summary>
        /// Consulta o total de requests pendentes de Crédito.
        /// Utilizado na HomePage Segmentada.        
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do book:
        /// - Book BKXA0791 / Programa XA791 / Transação XAHS
        /// </remarks>
        /// <param name="numeroPv">Número do Estabelecimento</param>
        /// <returns>Quantidade de requests pendentes Crédito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ConsultarTotalPendentesCredito(Int32 numeroPv);
    }
}
