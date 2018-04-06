#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [24/05/2012] – [André Garcia] – [Criação]
- [26/11/2015] – [Rodrigo Rodrigues] – Migração do método ConsultarSQL() para novo projeto (Redecard.PN.Sustentacao.Servicos)
*/
#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Data;
using Redecard.PN.Sustentacao.Modelo;

namespace Redecard.PN.Sustentacao.Servicos
{

    /// <summary>
    /// Métodos de usuários que possuem acesso ao banco de dados
    /// </summary>
    /// <remarks>
    /// Métodos de usuários que possuem acesso ao banco de dados
    /// </remarks>
    [ServiceContract]
    public interface ISustentacaoAdministracaoServico 
    {
        /// <summary>
        /// Executa o script no banco de dados informado.
        /// </summary>
        /// <remarks>
        /// Executa o script no banco de dados informado.
        /// </remarks>
        /// <param name="bancoDados">Nome da connection string</param>
        /// <param name="script">Sql que será executado no banco.</param>
        /// <returns>Instância do objeto DataTable</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        DataTable[] ConsultarSql(String bancoDados, String script);
       

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        DataTable[] ConsultarQuerySQLServer(String script, String usuarioLogado, String infoOperacao, String nomeBanco);

        /// <summary>
        /// Método que retorna registros da tabela TBPN003 e TBPN002 
        /// </summary>
        /// <param name="email">Parâmetro de pesquisa</param>
        /// <returns>Lista unificada de usuários e entidades</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<UsuarioPV> ConsultarUsuariosPorEmail(String email, Int32 pv, Int32 grupo);

        /// <summary>
        /// Método que retorna registros da tabela TBPN026
        /// </summary>
        /// <param name="dataInclusao"></param>
        /// <param name="codigoPv"></param>
        /// <param name="codigoPvAcesso"></param>
        /// <param name="email"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<RetornoCancelamento> ConsultarRetornoCancelamento(DateTime dataInclusao, Int32? codigoPv, Int32? codigoPvAcesso, String email, String ip);

        /// <summary>
        /// Método utilizado para alterar status e quantidade de tentativas de senhas na tabela TBPN003
        /// </summary>
        /// <param name="codUsrId">Identificacao do usuario na tabela TBPN003</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 DesbloquearUsuario(Int32 codUsrId, Int32 codEntidade, String usuarioLogado, String nomEmlUsr);

        /// <summary>
        /// Método utilizado para alterar status e quantidade de tentativas de senhas na tabela TBPN003
        /// </summary>
        /// <param name="codUsrId">Identificacao do usuario na tabela TBPN003</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirUsuario(Int32 codUsrId, String usuarioLogado, String nomEmlUsr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codUsrId"></param>
        /// <param name="codEtd"></param>
        /// <param name="usuarioLogado"></param>
        /// <param name="nomEmlUsr"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirRelEtd(Int32 codUsrId, Int32 codEtd, Int32 grupo, String usuarioLogado, String nomEmlUsr);

        /// <summary>
        /// Expurgo da tabela de log
        /// </summary>
        /// <remarks>
        /// Expurgo da tabela de log
        /// </remarks>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void Expurgo();

        /// <summary>
        ///  Lista todas o nome de todas as connection strings configuradas no web.config
        /// </summary>
        /// <returns>Lista de connection strings</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<String> ListarConnectionStrings();

        /// <summary>
        /// Lista todos os endpoints dos web configs dentro de Redecard.PN
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<String> ListarServicosPN();

        /// <summary>
        /// Healthcheck dos servicos do PN
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servico> HealthCheckServicos(string servidor, string ambiente);

        /// <summary>
        /// Retorna as definições de classe e config para a criação do client proxy, baseado no endpoint informado.
        /// </summary>
        /// <param name="endPointAddress">URL do serviço que se deseja chamar.</param>
        /// <returns>Instância do objeto ProxyInfo.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ProxyInfo GetProxy(String endPointAddress);
    }
}