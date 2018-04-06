using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EL = Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe de Log responsável por gerar e armazenar as informações de log (informação/erro)
    /// conforme os níveis, categorias, filtros e listeners configurados no arquivo de
    /// configuração de Log do Enterprise Library.    
    /// <example>
    /// Dentro da sintaxe "using" da classe Logger, todas as chamadas de gravação de mensagem no log
    /// (métodos estáticos ou da instância da classe) estarão correlacionadas pelo mesmo
    /// id da atividade (ActivityID).
    /// <code lang="cs">
    /// using(Logger Log = Logger.IniciarLog("Consulta de filiais....")) 
    /// {        
    ///     Log.GravarLog(EventoLog.InicioServico, new { parametro1, parametro2, ... });   
    ///     Logger.GravarLog("Mensagem de log customizada...", new { algumParametro });
    ///     
    ///     try {
    ///         ...
    ///     } catch (PortalRedecardException ex)
    ///         Logger.GravarErro("Mensagem de erro customizada", ex);
    ///     } catch(Exception ex) {
    ///         Log.GravarErro(ex);
    ///     }
    /// }
    /// </code>    
    /// </example>    
    /// </summary>
    public class Logger : IDisposable
    {
        #region [ Propriedades ]

        private EL.Tracer Tracer { get; set; }

        /// <summary>ActivityID atual do Correlation Manager do Trace</summary>
        public static Guid ActivityID { 
            get { return Trace.CorrelationManager.ActivityId; }
            set { Trace.CorrelationManager.ActivityId = value; }
        }

        private String Operacao { get; set; }
        private Guid PreviousActivityID { get; set; }

        #endregion

        #region [ Construtor ]

        /// <summary>Construtor padrão. 
        /// Deve ser utilizado com "using" para manter o mesmo ActivityID nas entradas do log</summary>        
        /// <param name="origem">Origem/Categoria</param>
        /// <param name="activityId">ActivityID que será atribuído ao Tracer</param>
        private Logger(String origem, Guid activityId)
        {
            this.PreviousActivityID = ActivityID;
            this.Tracer = new EL.Tracer(origem, activityId);
        }

        /// <summary>Construtor padrão. 
        /// Deve ser utilizado com "using" para manter o mesmo ActivityID nas entradas do log</summary>        
        /// <param name="origem">Origem/Categoria</param>        
        private Logger(String origem) 
            : this(origem, (ActivityID == null || ActivityID == Guid.Empty) ? Guid.NewGuid() : ActivityID)
        {
        }
        
        private Logger() { }
                
        /// <summary>
        /// Inicia uma nova operação/atividade de log.
        /// Deve ser utilizada com a sintaxe using.
        /// Todas as chamadas aos métodos de gravação de log internas ao escopo do objeto estarão 
        /// correlacionados pelo mesmo ActivityID da operação de nível mais superior.
        /// </summary>
        /// <param name="operacao">Descrição da operação lógica</param>
        /// <returns>Instância da classe Logger</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Logger IniciarLog(String operacao)
        {
            try
            {
                StackFrame sFrame = new StackTrace(true).GetFrame(1);
                String assembly = Path.GetFileNameWithoutExtension(sFrame.GetMethod().Module.Name);
                
                Logger instancia = new Logger(assembly);
                instancia.Operacao = operacao;
                
                Logger.GravarLog("Início: " + operacao, null, 3, TraceEventType.Start);

                return instancia;
            }
            catch { return new Logger(); }
        }

        /// <summary>
        /// Inicia uma nova instância do Log.
        /// Deve ser utilizada com a sintaxe using.
        /// Todas as chamadas aos métodos de gravação de log internas ao escopo do objeto estarão 
        /// correlacionados pelo mesmo ActivityID da operação de nível mais superior.
        /// </summary>
        /// <param name="operacao">Descrição da operação lógica</param>
        /// <returns>Instância da classe Logger</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Logger IniciarNovoLog(String operacao)
        {
            try
            {
                StackFrame sFrame = new StackTrace(true).GetFrame(1);
                String assembly = Path.GetFileNameWithoutExtension(sFrame.GetMethod().Module.Name);

                Logger instancia = new Logger(assembly, Guid.NewGuid());
                instancia.Operacao = operacao;

                Logger.GravarLog("Início: " + operacao, null, 3, TraceEventType.Start);

                return instancia;
            }
            catch { return new Logger(); }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void IDisposable.Dispose()
        {            
            Logger.GravarLog("Término: " + this.Operacao, null, 3, TraceEventType.Stop);
            if(Tracer != null)
                Tracer.Dispose();
            ActivityID = this.PreviousActivityID;
        }
        
        #endregion
        
        #region [ Log de Mensagens - Métodos Estáticos ]

        /// <summary>Método padrão para gravar informações no log</summary>
        /// <param name="mensagem">Mensagem a ser gravada no log</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarLog(String mensagem)
        {
            GravarLog(mensagem, null, 3, TraceEventType.Information);
        }

        /// <summary>Método padrão para gravar informações no log</summary>
        /// <param name="mensagem">Mensagem a ser gravada no log</param>
        /// <param name="severidade">Severidade</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarLog(String mensagem, TraceEventType severidade)
        {
            GravarLog(mensagem, null, 3, severidade);
        }
        
        /// <summary>Método padrão para gravar informações no log</summary>
        /// <param name="mensagem">Mensagem a ser gravada no log</param>
        /// <param name="parametros">Parâmetros adicionais para gravação no log</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarLog(String mensagem, Object parametros)
        {
            GravarLog(mensagem, parametros, 3, TraceEventType.Information);
        }

        /// <summary>Método padrão para gravar informações no log</summary>
        /// <param name="mensagem">Mensagem a ser gravada no log</param>
        /// <param name="parametros">Parâmetros adicionais para gravação no log</param>
        /// <param name="severidade">Severidade</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarLog(String mensagem, Object parametros, TraceEventType severidade)
        {
            GravarLog(mensagem, parametros, 3, severidade);
        }

        /// <summary>Método padrão para gravar informações no log</summary>
        /// <param name="mensagem">Mensagem a ser gravada no log</param>
        /// <param name="parametros">Parâmetros adicionais para gravação no log</param>
        /// <param name="stackFrameIndex">StackFrame Index</param>
        /// <param name="eventType">Tipo/nível do evento de log</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarLog(String mensagem, Object parametros, Int32 stackFrameIndex, TraceEventType eventType)
        {
            try
            {
                //Instancia novo objeto de log
                LogEntry log = new LogEntry();
                log.Severity = eventType;

                //Se log está habilitado e entrada do log é candidata a ser gravada                
                if (EL.Logger.IsLoggingEnabled() && EL.Logger.ShouldLog(log))
                {
                    log.Message = mensagem;
                    PreencherDadosPadrao(ref log, stackFrameIndex);

                    if (parametros != null)
                        log.Parametros = parametros;

                    LogWrite(log);
                }
            }
            catch (Exception e)
            {
                _TratarErroInterno(e);
            }
        }

        /// <summary>Método padrão para gravar informações no log</summary>
        /// <param name="mensagem">Mensagem a ser gravada no log</param>
        /// <param name="parametros">Parâmetros adicionais para gravação no log</param>
        /// <param name="metodo">Método</param>
        /// <param name="eventType">Tipo/nível do evento de log</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarLog(String mensagem, Object parametros, MethodBase metodo, TraceEventType eventType)
        {
            try
            {
                //Instancia novo objeto de log
                LogEntry log = new LogEntry();
                log.Severity = eventType;

                //Se log está habilitado e entrada do log é candidata a ser gravada                
                if (EL.Logger.IsLoggingEnabled() && EL.Logger.ShouldLog(log))
                {
                    log.Message = mensagem;
                    PreencherDadosPadrao(ref log, metodo);

                    if (parametros != null)
                        log.Parametros = parametros;

                    LogWrite(log);
                }
            }
            catch (Exception e)
            {
                _TratarErroInterno(e);
            }
        }

        #endregion

        #region [ Log de Mensagens ]

        /// <summary>Método padrão para gravar informações no log</summary>
        /// <param name="mensagem">Mensagem a ser gravada no log</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GravarMensagem(String mensagem)
        {
            GravarLog(String.Format(mensagem, this.Operacao ?? ""), null, 3, TraceEventType.Information);
        }

        /// <summary>Método padrão para gravar informações no log</summary>
        /// <param name="mensagem">Mensagem a ser gravada no log</param>
        /// <param name="parametros">Parâmetros</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GravarMensagem(String mensagem, Object parametros)
        {
            GravarLog(String.Format(mensagem, this.Operacao ?? ""), parametros, 3, TraceEventType.Information);
        }

        /// <summary>Método para gravação de informações no log, com mensagem padronizadas</summary>
        /// <param name="tipoEvento">Tipo do Evento</param>        
        /// <param name="parametros">Parâmetros</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GravarLog(EventoLog tipoEvento, Object parametros)
        {
            String mensagem = String.Format(GetDescription(tipoEvento), this.Operacao ?? "");
            GravarLog(mensagem, parametros, 3, TraceEventType.Information);
        }
        
        /// <summary>Método para gravação de informações no log, com mensagem padronizadas</summary>
        /// <param name="tipoEvento">Tipo do Evento</param>        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GravarLog(EventoLog tipoEvento)
        {
            String mensagem = String.Format(GetDescription(tipoEvento), this.Operacao ?? "");
            GravarLog(mensagem, null, 3, TraceEventType.Information);
        }

        #endregion

        #region [ Log de Erros - Métodos Estáticos ] 

        /// <summary>Método padrão para gravar erros no log</summary>
        /// <param name="mensagem">Mensagem de erro</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarErro(String mensagem)
        {
            GravarErro(mensagem, null, null, 3);
        }

        /// <summary>Método padrão para gravar erros no log</summary>
        /// <param name="mensagem">Mensagem de erro</param>        
        /// <param name="exc">Exceção gerada</param>        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarErro(String mensagem, Exception exc)
        {
            GravarErro(mensagem, exc, null, 3);
        }

        /// <summary>Método padrão para gravar erros no log</summary>
        /// <param name="mensagem">Mensagem de erro</param>        
        /// <param name="exc">Exceção gerada</param>        
        /// <param name="parametros">Parâmetros</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarErro(String mensagem, Exception exc, Object parametros)
        {
            GravarErro(mensagem, exc, parametros, 3);
        }

        /// <summary>Método interno padrão para gravação de erros no log</summary>
        /// <param name="exc">Exceção</param>
        /// <param name="mensagem">Mensagem</param>
        /// <param name="parametros">Parâmetros</param>
        /// <param name="stackFrameIndex">StackFrame Index</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarErro(String mensagem, Exception exc, Object parametros, Int32 stackFrameIndex)
        {
            try
            {
                //Instancia novo objeto de log
                LogEntry log = new LogEntry();
                log.Severity = TraceEventType.Error;

                //Se log está habilitado e entrada do log é candidata a ser gravada
                if (EL.Logger.IsLoggingEnabled() && EL.Logger.ShouldLog(log))
                {
                    log.Message = mensagem;
                    log.Excecao = exc;
                    log.Parametros = parametros;
                    PreencherDadosPadrao(ref log, stackFrameIndex);

                    LogWrite(log);
                }
            }
            catch (Exception e)
            {
                _TratarErroInterno(e);
            }
        }

        /// <summary>Método interno padrão para gravação de erros no log</summary>
        /// <param name="exc">Exceção</param>
        /// <param name="mensagem">Mensagem</param>
        /// <param name="parametros">Parâmetros</param>
        /// <param name="metodo">Método</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GravarErro(String mensagem, Exception exc, Object parametros, MethodBase metodo)
        {
            try
            {
                //Instancia novo objeto de log
                LogEntry log = new LogEntry();
                log.Severity = TraceEventType.Error;

                //Se log está habilitado e entrada do log é candidata a ser gravada
                if (EL.Logger.IsLoggingEnabled() && EL.Logger.ShouldLog(log))
                {
                    log.Message = mensagem;
                    log.Excecao = exc;
                    log.Parametros = parametros;
                    PreencherDadosPadrao(ref log, metodo);

                    LogWrite(log);
                }
            }
            catch (Exception e)
            {
                _TratarErroInterno(e);
            }
        }

        #endregion

        #region [ Log de Erros ]

        /// <summary>Método padrão para gravar erros no log</summary>
        /// <param name="exc">Exceção gerada</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GravarErro(Exception exc)
        {
            String mensagem = String.Format("Erro durante operação: " + this.Operacao);
            GravarErro(mensagem, exc, null, 3);
        }
        
        #endregion

        #region [ Métodos Auxiliares ]

        internal static void _TratarErroInterno(Exception exc)
        {            
        }

        private static void LogWrite(LogEntry logEntry)
        {
            try
            {
                if (Trace.CorrelationManager.ActivityId == Guid.Empty)
                    Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                EL.Logger.Write(logEntry);
                //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(stateInfo => { EL.Logger.Write((LogEntry)stateInfo); }), logEntry);
            }
            catch (Exception ex) { _TratarErroInterno(ex); }
        }

        private static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        internal static String Serializar<T>(T objeto)
        {
            Serializer serializer = new Serializer(new DirectReflector());
            return serializer.Serialize(objeto, "Parametros");
        }

        /// <summary>Inclui dados padrão na entrada de log.</summary>
        /// <param name="log">Registro de log</param>
        /// <param name="stackFrameIndex">StackFrame index</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void PreencherDadosPadrao(ref LogEntry log, Int32 stackFrameIndex)
        {
            log.TimeStamp = DateTime.Now;

            Sessao sessaoAtual = Sessao.Obtem();
            if (sessaoAtual != null)
            {
                log.CodigoEntidade = sessaoAtual.CodigoEntidade;
                log.GrupoEntidade = sessaoAtual.GrupoEntidade;
                log.LoginUsuario = sessaoAtual.LoginUsuario;
                log.Categories.Add(log.CodigoEntidade.ToString());
            }

            StackFrame sFrame = new StackTrace(true).GetFrame(stackFrameIndex);
            if (sFrame != null)
            {
                MethodBase method = sFrame.GetMethod();
                if (method != null)
                {
                    log.Metodo = method.Name;
                    if (method.DeclaringType != null)                  
                        log.Classe = method.DeclaringType.FullName;
                    if (method.Module != null)                  
                        log.Assembly = Path.GetFileNameWithoutExtension(method.Module.Name);                   
                }

                Int32 lineNumber = sFrame.GetFileLineNumber(), columnNumber = sFrame.GetFileColumnNumber();
                log.LinhaCodigo = lineNumber == 0 ? (Int32?)null : lineNumber;
                log.ColunaCodigo = columnNumber == 0 ? (Int32?)null : columnNumber;

                if (log.Classe.EmptyToNull() != null)
                {
                    log.Categories.Add(log.Classe);
                    if(log.Metodo.EmptyToNull() != null)
                        log.Categories.Add(String.Format("{0}.{1}", log.Classe, log.Metodo));
                }
                log.Categories.Add(log.Assembly);
            }
        }

        /// <summary>Inclui dados padrão na entrada de log.</summary>
        /// <param name="log">Registro de log</param>
        /// <param name="metodo">Método</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void PreencherDadosPadrao(ref LogEntry log, MethodBase metodo)
        {
            log.TimeStamp = DateTime.Now;

            Sessao sessaoAtual = Sessao.Obtem();
            if (sessaoAtual != null)
            {
                log.CodigoEntidade = sessaoAtual.CodigoEntidade;
                log.GrupoEntidade = sessaoAtual.GrupoEntidade;
                log.LoginUsuario = sessaoAtual.LoginUsuario;
                log.Categories.Add(log.CodigoEntidade.ToString());
            }
            
            if (metodo != null)
            {
                log.Metodo = metodo.Name;
                if (metodo.DeclaringType != null)
                    log.Classe = metodo.DeclaringType.FullName;
                if (metodo.Module != null)
                    log.Assembly = Path.GetFileNameWithoutExtension(metodo.Module.Name);
            }

            if (log.Classe.EmptyToNull() != null)
            {
                log.Categories.Add(log.Classe);
                if (log.Metodo.EmptyToNull() != null)
                    log.Categories.Add(String.Format("{0}.{1}", log.Classe, log.Metodo));
            }
            log.Categories.Add(log.Assembly);
        }

        #endregion        
    }
}