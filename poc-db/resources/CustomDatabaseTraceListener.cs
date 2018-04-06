using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using EL = Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Redecard.PN.Comum.TraceListeners
{
    /// <summary>
    /// A <see cref="System.Diagnostics.TraceListener"/> that writes to a database, formatting the output with an <see cref="ILogFormatter"/>.
    /// </summary>
    [ConfigurationElementType(typeof(CustomDatabaseTraceListenerData))]
    public class CustomDatabaseTraceListener : FormattedTraceListenerBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CustomDatabaseTraceListener"/>.
        /// </summary>        
        public CustomDatabaseTraceListener()
            : base()
        {
        }

        /// <summary>
        /// The Write method 
        /// </summary>
        /// <param name="message">The message to log</param>
        public override void Write(string message)
        {
            ExecuteWriteLogStoredProcedure(0, 5, TraceEventType.Information, string.Empty, DateTime.Now, string.Empty,
                                            string.Empty, string.Empty, string.Empty, null, null, message, Guid.Empty,
                                            null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        /// The WriteLine method.
        /// </summary>
        /// <param name="message">The message to log</param>
        public override void WriteLine(string message)
        {
            Write(message);
        }


        /// <summary>
        /// Delivers the trace data to the underlying database.
        /// </summary>
        /// <param name="eventCache">The context information provided by <see cref="System.Diagnostics"/>.</param>
        /// <param name="source">The name of the trace source that delivered the trace data.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="id">The id of the event.</param>
        /// <param name="data">The data to trace.</param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if ((this.Filter == null) || this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                if (data is EL.LogEntry)
                    ExecuteStoredProcedure(data as EL.LogEntry);
                else if (data is string)
                    Write(data as string);
                else
                    base.TraceData(eventCache, source, eventType, id, data);
            }
        }

        /// <summary>
        /// Declare the supported attributes for <see cref="CustomDatabaseTraceListener"/>
        /// </summary>
        protected override string[] GetSupportedAttributes()
        {
            return new string[0] { };
        }

        /// <summary>
        /// Executes the stored procedures
        /// </summary>
        /// <param name="logEntry">The LogEntry to store in the database</param>
        private void ExecuteStoredProcedure(EL.LogEntry logEntry)
        {
            LogEntry log = logEntry as LogEntry;
            if (log != null)
            {
                List<String> categorias = new List<String>();
                if (log.Categories != null && log.Categories.Count > 0)
                    categorias = log.Categories.ToList();

                ExecuteWriteLogStoredProcedure(
                    log.EventId, log.Priority, log.Severity,
                    log.Title, log.TimeStamp, log.MachineName, log.AppDomainName,
                    log.ProcessId, log.ProcessName, log.ManagedThreadName, log.Win32ThreadId,
                    log.Message, log.ActivityId,
                    log.CodigoEntidade, log.GrupoEntidade, log.LoginUsuario,
                    log.Assembly, log.Classe, log.Metodo, log.LinhaCodigo, log.ColunaCodigo,
                    log.Parametros != null ? Logger.Serializar(log.Parametros) : null,
                    log.Excecao, categorias);
            }
        }

        /// <summary>
        /// Executes the WriteLog stored procedure
        /// </summary>
        /// <param name="eventId">The event id for this LogEntry.</param>
        /// <param name="priority">The priority for this LogEntry.</param>
        /// <param name="severity">The severity for this LogEntry.</param>
        /// <param name="title">The title for this LogEntry.</param>
        /// <param name="timeStamp">The timestamp for this LogEntry.</param>
        /// <param name="machineName">The machine name for this LogEntry.</param>
        /// <param name="appDomainName">The appDomainName for this LogEntry.</param>
        /// <param name="processId">The process id for this LogEntry.</param>
        /// <param name="processName">The processName for this LogEntry.</param>
        /// <param name="managedThreadName">The managedthreadName for this LogEntry.</param>
        /// <param name="win32ThreadId">The win32threadID for this LogEntry.</param>
        /// <param name="message">The message for this LogEntry.</param>
        /// <param name="activityId"></param>
        /// <param name="assembly"></param>
        /// <param name="categorias"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="colunaCodigo"></param>        
        /// <param name="excecao"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="linhaCodigo"></param>
        /// <param name="loginUsuario"></param>
        /// <param name="metodo"></param>
        /// <param name="parametros"></param>       
        /// <returns>An integer for the LogEntry Id</returns>
        private void ExecuteWriteLogStoredProcedure(Int32 eventId, Int32 priority, TraceEventType severity, String title, DateTime timeStamp,
                                                    String machineName, String appDomainName, String processId, String processName,
                                                    String managedThreadName, String win32ThreadId, String message,
                                                    Guid activityId, Int32? codigoEntidade, Int32? grupoEntidade, String loginUsuario,
                                                    String assembly, String classe, String metodo, Int32? linhaCodigo, Int32? colunaCodigo,
                                                    String parametros, Exception excecao, List<String> categorias)
        {
            try
            {
                using (LogServico.LogServicoClient servicoLog = new LogServico.LogServicoClient())
                {
                    LogServico.LogEntry log = new LogServico.LogEntry
                    {
                        ActivityId = activityId,
                        AppDomainName = appDomainName,
                        Assembly = assembly,
                        Classe = classe,
                        Categorias = categorias,
                        CodigoEntidade = codigoEntidade,
                        ColunaCodigo = colunaCodigo,
                        EventId = eventId,
                        GrupoEntidade = grupoEntidade,
                        LinhaCodigo = linhaCodigo,
                        LoginUsuario = loginUsuario,
                        MachineName = machineName,
                        ManagedThreadName = managedThreadName,
                        Message = message,
                        Metodo = metodo,
                        Parametros = parametros,
                        Priority = priority,
                        ProcessId = processId,
                        ProcessName = processName,
                        Severity = severity.ToString(),
                        TimeStamp = timeStamp,
                        Title = title,
                        Win32ThreadId = win32ThreadId
                    };

                    if (excecao != null)
                    {
                        log.Excecao = new LogServico.ExcecaoLog
                        {
                            Mensagem = excecao.Message,
                            Origem = excecao.Source,
                            StackTrace = excecao.StackTrace
                        };

                        Exception baseExc = excecao.GetBaseException();
                        if (baseExc != null)
                        {
                            log.ExcecaoBase = new LogServico.ExcecaoLog
                            {
                                Mensagem = baseExc.Message,
                                Origem = baseExc.Source,
                                StackTrace = baseExc.StackTrace
                            };
                        }
                    }
                    servicoLog.GravarLog(log);
                }
            }
            catch (Exception exc)
            {
                Logger._TratarErroInterno(exc);
            }
        }
    }  
}