using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.Common;
using Redecard.PN.Servico;
using Redecard.PN.Modelo;
using AutoMapper;
using System.Threading.Tasks;
using System.Data;

namespace Redecard.PN.Servicos
{
    /// <summary>
    /// Serviço para gravação e consulta de mensagens de log
    /// </summary>
    public class LogServico : Comum.ServicoBase, ILogServico
    {
        /// <summary>
        /// Gravação de mensagem de log
        /// </summary>
        /// <param name="logEntry">Registro de log a ser armazenado em banco</param>
        public void GravarLog(LogEntry logEntry)
        {
            try
            {
                //Instanciação da classe de negócio de log
                Negocio.Log negocio = new Negocio.Log();

                //Chamando classe de negócio para gravação da mensagem de log
                negocio.GravarLog(
                    logEntry.EventId,
                    logEntry.Priority,
                    logEntry.Severity,
                    logEntry.Title,
                    logEntry.TimeStamp,
                    logEntry.MachineName,
                    logEntry.AppDomainName,
                    logEntry.ProcessId,
                    logEntry.ProcessName,
                    logEntry.ManagedThreadName,
                    logEntry.Win32ThreadId,
                    logEntry.Message,
                    logEntry.ActivityId,
                    logEntry.CodigoEntidade,
                    logEntry.GrupoEntidade,
                    logEntry.LoginUsuario,
                    logEntry.Assembly,
                    logEntry.Classe,
                    logEntry.Metodo,
                    logEntry.LinhaCodigo,
                    logEntry.ColunaCodigo,
                    logEntry.Parametros,
                    logEntry.Excecao != null ? logEntry.Excecao.Origem : null,
                    logEntry.Excecao != null ? logEntry.Excecao.Mensagem : null,
                    logEntry.Excecao != null ? logEntry.Excecao.StackTrace : null,
                    logEntry.ExcecaoBase != null ? logEntry.ExcecaoBase.Origem: null,
                    logEntry.ExcecaoBase != null ? logEntry.ExcecaoBase.Mensagem : null,
                    logEntry.ExcecaoBase != null ? logEntry.ExcecaoBase.StackTrace : null,
                    logEntry.Categorias);
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta dos eventos de uma operação de log
        /// </summary>
        /// <param name="activityID">Id da atividade/operação de log</param>
        /// <returns>Eventos associados da operação de log</returns>
        public List<LogItem> ConsultarLog(Guid activityID)
        {
            try
            {
                //Instanciação da classe de negócio de log
                Negocio.Log negocio = new Negocio.Log();

                //Chamando classe de negócio para gravação de mensagem de log
                Modelo.LogItem[] itens = negocio.ConsultarLog(activityID);

                //Objeto de retorno
                List<LogItem> retorno = new List<LogItem>();
                
                //Conversão para modelo de serviço
                Modelo.LogItem item;
                for (Int32 i = 0; i < itens.Length; i++)
                {
                    item = itens[i];
                    retorno.Add(new LogItem
                    {
                        ActivityID = item.ActivityID,
                        AssemblyID = item.AssemblyID,
                        Severidade = item.Severidade,
                        Timestamp = item.Timestamp,
                        SeverityID = item.SeverityID,
                        Mensagem = item.Mensagem,
                        Classe = item.Classe,
                        Metodo = item.Metodo,
                        CodigoEntidade = item.CodigoEntidade,
                        GrupoEntidade = item.GrupoEntidade,
                        LoginUsuario = item.LoginUsuario,
                        Assembly = item.Assembly,
                        LinhaCodigo = item.LinhaCodigo,
                        ColunaCodigo = item.ColunaCodigo,
                        Parametros = item.Parametros,
                        Excecao = item.Excecao,
                        Servidor = item.Servidor,
                        LogID = item.LogID
                    });
                }
                return retorno;
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta de um registro de log
        /// </summary>
        /// <param name="logID">Id do registro de log</param>
        /// <returns>Eventos associados da operação de log</returns>
        public LogItem ConsultarLogRegistro(Int32 logID)
        {
            try
            {
                //Instanciação da classe de negócio de log
                Negocio.Log negocio = new Negocio.Log();

                //Chamando classe de negócio para gravação de mensagem de log
                Modelo.LogItem item = negocio.ConsultarRegistro(logID);

                //Objeto de retorno
                List<LogItem> retorno = new List<LogItem>();

                //Conversão para modelo de serviço
                return new LogItem {
                    ActivityID = item.ActivityID,
                    AssemblyID = item.AssemblyID,
                    Severidade = item.Severidade,
                    Timestamp = item.Timestamp,
                    SeverityID = item.SeverityID,
                    Mensagem = item.Mensagem,
                    Classe = item.Classe,
                    Metodo = item.Metodo,
                    CodigoEntidade = item.CodigoEntidade,
                    GrupoEntidade = item.GrupoEntidade,
                    LoginUsuario = item.LoginUsuario,
                    Assembly = item.Assembly,
                    LinhaCodigo = item.LinhaCodigo,
                    ColunaCodigo = item.ColunaCodigo,
                    Parametros = item.Parametros,
                    Excecao = item.Excecao,
                    Servidor = item.Servidor,
                    LogID = item.LogID
                };
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

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
        /// <param name="registroInicial">Índice do registro inicial</param>
        /// <returns>Operações</returns>
        public List<LogOperacao> ConsultarLogOperacoes(Guid? activityID, Int32? assemblyID, String classe,
            String metodo, Int32? codigoEntidade, Int32? grupoEntidade, String loginUsuario, Int32? severityID,
            DateTime? dataInicio, DateTime? dataFim, String filtrarPor, Int32? qtdRegistros, String servidor,
            Int32? registroInicial)
        {
            try
            {
                //Instaciação da classe de negócio do log
                Negocio.Log negocio = new Negocio.Log();

                //Consulta das operações, dados os filtros
                Modelo.LogOperacao[] itens = negocio.ConsultarOperacoes(activityID, assemblyID, classe, metodo,
                    codigoEntidade, grupoEntidade, loginUsuario, severityID, dataInicio, dataFim, filtrarPor, 
                    qtdRegistros, servidor, registroInicial);

                //Objeto de retorno
                List<LogOperacao> retorno = new List<LogOperacao>();

                //Conversão para modelo de serviço
                Modelo.LogOperacao item;
                for (Int32 i = 0; i < itens.Length; i++)
                {
                    item = itens[i];
                    retorno.Add(new LogOperacao
                    {
                        ActivityID = item.ActivityID,
                        Inicio = item.Inicio,
                        Termino = item.Termino,
                        Quantidade = item.Quantidade,
                        PossuiErro = item.PossuiErro,
                        Classe = item.Classe,
                        CodigoEntidade = item.CodigoEntidade,
                        GrupoEntidade = item.GrupoEntidade,
                        LoginUsuario = item.LoginUsuario,
                        Mensagem = item.Mensagem,
                        Metodo = item.Metodo
                    });
                }
                return retorno;
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

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
        public List<LogItem> ConsultarLogRegistros(Guid? activityID, Int32? assemblyID, String classe,
            String metodo, Int32? codigoEntidade, Int32? grupoEntidade, String loginUsuario, Int32? severityID,
            DateTime? dataInicio, DateTime? dataFim, String filtrarPor, Int32? qtdRegistros, String servidor,
            Int32? registroInicial)
        {
            try
            {
                //Instaciação da classe de negócio do log
                Negocio.Log negocio = new Negocio.Log();

                //Consulta das operações, dados os filtros
                Modelo.LogItem[] itens = negocio.ConsultarRegistros(activityID, assemblyID, classe, metodo,
                    codigoEntidade, grupoEntidade, loginUsuario, severityID, dataInicio, dataFim, filtrarPor, 
                    qtdRegistros, servidor, registroInicial);

                //Objeto de retorno
                List<LogItem> retorno = new List<LogItem>();

                //Conversão para modelo de serviço
                Modelo.LogItem item;
                for (Int32 i = 0; i < itens.Length; i++)
                {
                    item = itens[i];
                    retorno.Add(new LogItem
                    {
                        ActivityID = item.ActivityID,
                        AssemblyID = item.AssemblyID,
                        Severidade = item.Severidade,
                        Timestamp = item.Timestamp,
                        SeverityID = item.SeverityID,
                        Mensagem = item.Mensagem,
                        Classe = item.Classe,
                        Metodo = item.Metodo,
                        CodigoEntidade = item.CodigoEntidade,
                        GrupoEntidade = item.GrupoEntidade,
                        LoginUsuario = item.LoginUsuario,
                        Assembly = item.Assembly,
                        LinhaCodigo = item.LinhaCodigo,
                        ColunaCodigo = item.ColunaCodigo,
                        Parametros = item.Parametros,
                        Excecao = item.Excecao,
                        Servidor = item.Servidor,
                        LogID = item.LogID
                    });
                }
                return retorno;
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta os assemblies existentes no log.
        /// </summary>
        /// <returns>Dicionário contendo o ID e a descrição do assembly</returns>
        public Dictionary<Int32, String> ConsultarAssemblies()
        {
            try
            {
                //Instanciação da classe de negócio do log
                Negocio.Log negocio = new Negocio.Log();

                //Consulta e retorno dos assemblies
                return negocio.ConsultarAssemblies();
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta as severidades existentes no log.
        /// </summary>
        /// <returns>Dicionário contendo o ID e a descrição das severidades</returns>
        public Dictionary<Int32, String> ConsultarSeveridades()
        {
            try
            {
                //Instanciação da classe de negócio do log
                Negocio.Log negocio = new Negocio.Log();

                //Consulta e retorno dos assemblies
                return negocio.ConsultarSeveridades();
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta as classes existentes no log.
        /// </summary>
        /// <returns>Lista contendo o nome das classes</returns>
        public List<String> ConsultarClasses()
        {
            try
            {
                //Instanciação da classe de negócio do log
                Negocio.Log negocio = new Negocio.Log();

                //Consulta e retorno das classes
                return negocio.ConsultarClasses();
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta os métodos existentes no log.
        /// </summary>
        /// <returns>Lista contendo o nome dos métodos</returns>
        public List<String> ConsultarMetodos()
        {
            try
            {
                //Instanciação da classe de negócio do log
                Negocio.Log negocio = new Negocio.Log();

                //Consulta e retorno dos métodos
                return negocio.ConsultarMetodos();
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta os servidores existentes no log.
        /// </summary>
        /// <returns>Lista contendo o nome dos servidores</returns>
        public List<String> ConsultarServidores()
        {
            try
            {
                //Instanciação da classe de negócio do log
                Negocio.Log negocio = new Negocio.Log();

                //Consulta e retorno dos servidores
                return negocio.ConsultarServidores();
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta os dados dos filtros
        /// </summary>
        /// <returns>Dados dos filtros</returns>
        public LogFiltros ConsultarFiltros()
        {
            try
            {
                //Instanciação da classe de negócio do log
                var negocio = new Negocio.Log();

                //Prepara e inicia as threads de consulta dos filtros dos logs
                var taskAssemblies = new TaskFactory<Dictionary<Int32, String>>()
                    .StartNew(() => { return negocio.ConsultarAssemblies(); });
                var taskClasses = new TaskFactory<List<String>>()
                    .StartNew(() => { return negocio.ConsultarClasses(); });
                var taskMetodos = new TaskFactory<List<String>>()
                    .StartNew(() => { return negocio.ConsultarMetodos(); });
                var taskServidores = new TaskFactory<List<String>>()
                    .StartNew(() => { return negocio.ConsultarServidores(); });
                var taskSeveridades = new TaskFactory<Dictionary<Int32, String>>()
                    .StartNew(() => { return negocio.ConsultarSeveridades(); });

                //Aguarda a conclusão de todas as threads
                Task.WaitAll(taskAssemblies, taskClasses, taskMetodos, taskServidores, taskSeveridades);

                //Prepara objeto de retorno
                return new LogFiltros
                {
                    Assemblies = taskAssemblies.Result,
                    Classes = taskClasses.Result,
                    Metodos = taskMetodos.Result,
                    Servidores = taskServidores.Result,
                    Severidades = taskSeveridades.Result
                };
            }
            catch (Comum.PortalRedecardException ex)
            {                
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {                
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta o log do extrato
        /// </summary>        
        /// <param name="activityId">Activity ID</param>
        /// <param name="book">Nome do Book</param>
        /// <param name="dataInicio">Data início</param>
        /// <param name="dataFim">Data término</param>
        /// <param name="filtrarPor">Filtrar por</param>
        /// <param name="servidor">Servidor</param>
        public DataSet ConsultarExtrato(Guid? activityId, String book,
            DateTime? dataInicio, DateTime? dataFim, String servidor, String filtrarPor)
        {
            try
            {
                //Instanciação da classe de negócio do log
                Negocio.Log negocio = new Negocio.Log();

                //Consulta e retorno dos assemblies
                return negocio.ConsultarExtrato(activityId, book, dataInicio, dataFim, servidor, filtrarPor);
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta os books do extrato que são tratados pelo método ConsultarExtrato
        /// </summary>        
        public Dictionary<String, String> ConsultarExtratoBooks()
        {
            try
            {
                //Instanciação da classe de negócio do log
                Negocio.Log negocio = new Negocio.Log();

                //Consulta e retorno dos assemblies
                return negocio.ConsultarExtratoBooks();
            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Consulta os últimos estabelecimentos logados
        /// </summary>
        /// <returns>Últimos estabelecimentos logados</returns>
        public List<EstabelecimentosLogados> ConsultaEstabelecimentosLogados()
        {
            try
            {
                //Instanciação da classe de negócio do log
                Negocio.Log negocio = new Negocio.Log();

                List<Redecard.PN.Modelo.EstabelecimentosLogados> estabelecimentos = negocio.ConsultaEstabelecimentosLogados();

                Mapper.CreateMap<Redecard.PN.Modelo.EstabelecimentosLogados, EstabelecimentosLogados>();
                List<EstabelecimentosLogados> retorno = Mapper.Map<List<Redecard.PN.Modelo.EstabelecimentosLogados>, List<EstabelecimentosLogados>>(estabelecimentos);

                return retorno;

            }
            catch (Comum.PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }
    }
}