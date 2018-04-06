using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Servico;
using System.Data;

namespace Redecard.PN.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILogServico" in both code and config file together.
    [ServiceContract]
    public interface ILogServico
    {
        /// <summary>
        /// Gravação de mensagem de log
        /// </summary>
        /// <param name="logEntry">Registro de log a ser armazenado em banco</param>
        [OperationContract]
        void GravarLog(LogEntry logEntry);

        /// <summary>
        /// Consulta dos eventos de uma operação de log
        /// </summary>
        /// <param name="activityID">Id da atividade/operação de log</param>
        /// <returns>Eventos associados da operação de log</returns>
        [OperationContract]
        List<LogItem> ConsultarLog(Guid activityID);

        /// <summary>
        /// Consulta de um registro de log
        /// </summary>
        /// <param name="logID">Id do registro de log</param>
        /// <returns>Eventos associados da operação de log</returns>
        [OperationContract]
        LogItem ConsultarLogRegistro(Int32 logID);

        /// <summary>
        /// Consulta das operações/atividades de log.
        /// Serão retornadas as informações básicas das operações que atendam aos critérios dos filtros.
        /// </summary>
        /// <param name="activityID">Filtro por Id da atividade/operação de log</param>
        /// <param name="assemblyID">Filtro por Id do assembly onde foi gerado algum evento da operação</param>
        /// <param name="classe">Filtro pelo nome da classe onde foi gerado algum evento da operação</param>
        /// <param name="metodo">Filtro pelo nome do método onde foi gerado algum evento da operação</param>
        /// <param name="codigoEntidade">Filtro pelo código da entidade do usuário logado durante algum evento da operação</param>
        /// <param name="grupoEntidade">Filtro pelo código do grupo da entidade do usuário logado durante algum evento da operação</param>
        /// <param name="loginUsuario">Filtro pelo login do usuário logado durante algum evento da operação</param>
        /// <param name="severityID">Filtro pelo Id da severidade de algum evento da operação</param>
        /// <param name="dataInicio">Filtro pela data inicial da operação de log</param>
        /// <param name="dataFim">Filtro pela data de término da operação de log</param>
        /// <param name="filtrarPor">Filtro genérico, que busca por campos chave dos registros de log</param>
        /// <param name="qtdRegistros">Quantidade de registros de operações para retornar na consulta</param>
        /// <param name="servidor">Filtro pelo nome do servidor/máquina onde foi gerado algum evento da operação</param>
        /// <param name="qtdTotalRegistros">Quantidade total de registros</param>
        /// <param name="registroInicial">Índice do registro inicial</param>
        /// <returns>Operações</returns>
        [OperationContract]
        List<LogOperacao> ConsultarLogOperacoes(Guid? activityID, Int32? assemblyID, String classe,
            String metodo, Int32? codigoEntidade, Int32? grupoEntidade, String loginUsuario, Int32? severityID,
            DateTime? dataInicio, DateTime? dataFim, String filtrarPor, Int32? qtdRegistros, String servidor,
            Int32? registroInicial);

        /// <summary>
        /// Consulta das operações/atividades de log.
        /// Serão retornadas as informações básicas das operações que atendam aos critérios dos filtros.
        /// </summary>
        /// <param name="activityID">Filtro por Id da atividade/operação de log</param>
        /// <param name="assemblyID">Filtro por Id do assembly onde foi gerado algum evento da operação</param>
        /// <param name="classe">Filtro pelo nome da classe onde foi gerado algum evento da operação</param>
        /// <param name="metodo">Filtro pelo nome do método onde foi gerado algum evento da operação</param>
        /// <param name="codigoEntidade">Filtro pelo código da entidade do usuário logado durante algum evento da operação</param>
        /// <param name="grupoEntidade">Filtro pelo código do grupo da entidade do usuário logado durante algum evento da operação</param>
        /// <param name="loginUsuario">Filtro pelo login do usuário logado durante algum evento da operação</param>
        /// <param name="severityID">Filtro pelo Id da severidade de algum evento da operação</param>
        /// <param name="dataInicio">Filtro pela data inicial da operação de log</param>
        /// <param name="dataFim">Filtro pela data de término da operação de log</param>
        /// <param name="filtrarPor">Filtro genérico, que busca por campos chave dos registros de log</param>
        /// <param name="qtdRegistros">Quantidade de registros de operações para retornar na consulta</param>
        /// <param name="servidor">Filtro pelo nome do servidor/máquina onde foi gerado algum evento da operação</param>
        /// <param name="registroInicial">Registro inicial</param>
        /// <returns>Operações</returns>
        [OperationContract]
        List<LogItem> ConsultarLogRegistros(Guid? activityID, Int32? assemblyID, String classe,
            String metodo, Int32? codigoEntidade, Int32? grupoEntidade, String loginUsuario, Int32? severityID,
            DateTime? dataInicio, DateTime? dataFim, String filtrarPor, Int32? qtdRegistros, String servidor,
            Int32? registroInicial);

        /// <summary>
        /// Consulta as severidades existentes no log.
        /// </summary>
        /// <returns>Dicionário contendo o ID e a descrição das severidades</returns>
        [OperationContract]
        Dictionary<Int32, String> ConsultarSeveridades();

        /// <summary>
        /// Consulta os assemblies existentes no log.
        /// </summary>
        /// <returns>Dicionário contendo o ID e a descrição do assembly</returns>
        [OperationContract]
        Dictionary<Int32, String> ConsultarAssemblies();

        /// <summary>
        /// Consulta as classes existentes no log.
        /// </summary>
        /// <returns>Lista contendo o nome das classes</returns>
        [OperationContract]
        List<String> ConsultarClasses();

        /// <summary>
        /// Consulta os métodos existentes no log.
        /// </summary>
        /// <returns>Lista contendo o nome dos métodos</returns>
        [OperationContract]
        List<String> ConsultarMetodos();

        /// <summary>
        /// Consulta os servidores existentes no log.
        /// </summary>
        /// <returns>Lista contendo o nome dos servidores</returns>
        [OperationContract]
        List<String> ConsultarServidores();

        /// <summary>
        /// Consulta os últimos estabelecimentos logados
        /// </summary>
        /// <returns>Últimos estabelecimentos logados</returns>
        [OperationContract]
        List<EstabelecimentosLogados> ConsultaEstabelecimentosLogados();

        /// <summary>
        /// Consulta os dados dos filtros
        /// </summary>
        /// <returns>Dados dos filtros</returns>
        [OperationContract]
        LogFiltros ConsultarFiltros();

        /// <summary>
        /// Consulta os assemblies existentes no log.
        /// </summary>        
        /// <param name="activityId">Activity ID</param>
        /// <param name="book">Nome do Book</param>
        /// <param name="dataFim">Data término</param>
        /// <param name="dataInicio">Data início</param>
        /// <param name="filtrarPor">Filtrar por</param>
        /// <param name="servidor">Servidor</param>
        [OperationContract]
        DataSet ConsultarExtrato(Guid? activityId, String book,
            DateTime? dataInicio, DateTime? dataFim, String servidor, String filtrarPor);

        /// <summary>
        /// Consulta os books do extrato que são tratados pelo método ConsultarExtrato
        /// </summary>      
        [OperationContract]
        Dictionary<String, String> ConsultarExtratoBooks();
    }
}
