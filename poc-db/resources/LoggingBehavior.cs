using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;
using System.Diagnostics;
using System.Xml.Linq;
using System.Reflection;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// LoggingBehavior
    /// </summary>
    public class LoggingBehavior : IInterceptionBehavior
    {
        /// <summary>
        /// GetRequiredInterfaces Method
        /// </summary>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// Método invocado ao interceptar o método original.
        /// </summary>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {            
            //Monta nome do método/classe
            String classMethodName = String.Format("{0}.{1}", input.MethodBase.ReflectedType.FullName, input.MethodBase.Name);
            String methodName = input.MethodBase.Name;

            IMethodReturn retorno = null;
            Stopwatch watch = new Stopwatch();

            try
            {
                //Log do início do método, com os parâmetros de chamada
                Logger.GravarLog(String.Format("Início do Método '{0}'", classMethodName),
                    new { Input = input.Arguments }, input.MethodBase, TraceEventType.Start);

                //Contabilização do tempo e execução do método
                watch.Start();
                retorno = getNext()(input, getNext);
                watch.Stop();

                //Log da exceção, caso ocorra
                if (retorno != null && retorno.Exception != null)
                {
                    StringBuilder message = new StringBuilder();
                    if (retorno.Exception is PortalRedecardException)
                    {
                        var portalExcecao = retorno.Exception as PortalRedecardException;
                        var dadosExcecao = new { portalExcecao.Codigo, portalExcecao.Fonte };
                        Logger.GravarErro(retorno.Exception.Message, retorno.Exception,
                            new { Retorno = dadosExcecao, Chamada = input.Arguments }, input.MethodBase);
                    }
                    else
                        Logger.GravarErro(retorno.Exception.Message, retorno.Exception, 
                            new { Chamada = input.Arguments }, input.MethodBase);
                }
            }
            catch (Exception ex)
            {
                //Log da exceção gerada durante interceptação do método
                Logger.GravarErro(ex.Message, ex, new { Chamada = input.Arguments }, input.MethodBase);
                throw;
            }
            finally
            {
                //Log dos parâmetros de retorno do método
                if (retorno != null)
                    Logger.GravarLog(String.Format("Fim do Método '{0}' ({1})", classMethodName,
                        new DateTime().Add(watch.Elapsed).ToString("HH:mm:ss.fff")),
                        retorno != null ? new { Retorno = retorno.ReturnValue, retorno.Outputs } : null,
                        input.MethodBase, TraceEventType.Stop);
                else
                    Logger.GravarLog(String.Format("Fim do Método '{0}' ({1})", classMethodName,
                        new DateTime().Add(watch.Elapsed).ToString("HH:mm:ss.fff")), null,
                        input.MethodBase, TraceEventType.Stop);
            }

            return retorno;
        }

        /// <summary>
        /// Flag indicando se irá executar interception
        /// </summary>
        public bool WillExecute
        {
            get { return true; }
        }     
    }
}
