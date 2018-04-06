/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Data;
using System.ServiceModel.Web;
using Redecard.PN.Servicos.Modelos;

namespace Redecard.PN.Servicos
{
    /// <summary>
    /// Interface para o Serviço para o Histórico/Log de Atividades
    /// </summary>
    [ServiceContract]
    public interface IHistoricoAtividadeServico
    {
        /// <summary>
        /// Consulta os tipos de atividade existentes
        /// </summary>
        /// <param name="exibir">Opcional: se deve filtrar os tipos de atividade visíveis para consulta</param>
        /// <returns>Lista de tipos de atividade</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(UriTemplate = "ConsultarTiposAtividades",
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<TipoAtividade> ConsultarTiposAtividades(Boolean? exibir);

        /// <summary>
        /// Consulta o Histórico de Atividades
        /// </summary>
        /// <param name="codigoHistorico">Opcional: código do histórico de atividade</param>
        /// <param name="codigoEntidade">Opcional: código da entidade</param>
        /// <param name="codigoTipoAtividade">Opcional: código do tipo de atividade</param>
        /// <param name="dataInicio">Opcional: data de início</param>
        /// <param name="dataFim">Opcional: data de término</param>
        /// <param name="nomeUsuario">Opcional: nome do usuário responsável</param>
        /// <param name="emailUsuario">Opcional: email do usuário responsável</param>
        /// <param name="funcionalOperador">Opcional: Funcional do operador responsável</param>
        /// <param name="tipoAtividadeVisible">Opcional: se deve ser do tipo de atividade visível</param>
        /// <returns>Histórico de Atividade encontrado, dado os filtros</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(UriTemplate = "ConsultarHistorico",
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<HistoricoAtividade> ConsultarHistorico(
            Int64? codigoHistorico,
            Int32? codigoEntidade,
            Int64? codigoTipoAtividade,
            DateTime? dataInicio,
            DateTime? dataFim,
            String nomeUsuario,
            String emailUsuario,
            String funcionalOperador,
            Boolean? tipoAtividadeVisible);

        /// <summary>
        /// Grava um registro no histórico de atividades
        /// </summary>
        /// <param name="historico">Dados do registro do histórico de atividade</param>
        /// <returns>Código do registro do histórico de atividade criado</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(Method = "POST", 
                   BodyStyle = WebMessageBodyStyle.WrappedResponse,
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   UriTemplate = "GravarHistorico")]
        Int64 GravarHistorico(HistoricoAtividade historico);

        /// <summary>
        /// Consulta os tipos de relatórios existentes
        /// </summary>
        /// <param name="ativo">Opcional: filtro de relatório pela flag "ativo"</param>
        /// <returns>Dicionário contendo os relatórios disponíveis</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(UriTemplate = "ConsultarTiposRelatorios",
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        Dictionary<Int32, String> ConsultarTiposRelatorios(Boolean? ativo);

        /// <summary>
        /// Consulta de Relatório do Histórico de Atividades
        /// </summary>
        /// <param name="codigoTipoRelatorio">Código do tipo de relatório</param>
        /// <param name="data">Data</param>
        /// <param name="codigoEntidade">Código do estabelecimento</param>
        /// <returns>Relatório</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(UriTemplate = "ConsultarRelatorio",
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        DataSet ConsultarRelatorio(Int32 codigoTipoRelatorio, DateTime? data, Int32? codigoEntidade);

#region [WebOperations]
        /// <summary>
        /// Consulta o histórico de Contratações do 
        /// </summary>
        /// <param name="inicio">Data/Hora de início</param>
        /// <param name="fim">Data/Hora de fim</param>
        /// <returns>Lista de Históricos</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "ConsultarHistoricoContratacaoCancelamento",
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ListaHistoricoConciliadorResponse<HistoricoConciliador> ConsultarHistoricoContratacaoCancelamento(String inicio, String fim);

        /// <summary>
        /// Consulta o histórico de envio de arquivos
        /// </summary>
        /// <param name="inicio">Data/Hora de início</param>
        /// <param name="fim">Data/Hora de fim</param>
        /// <returns>Lista de Históricos</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "ConsultarHistoricoEnvioArquivos",
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ListaHistoricoConciliadorResponse<HistoricoEnvioArquivoConciliador> ConsultarHistoricoEnvioArquivos(String inicio, String fim);

#endregion
    }
}