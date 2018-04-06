using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{
    /// <summary>
    /// 
    /// </summary>
    public class LogTracing
    {
        /// <summary>
        /// Nome do Arquivo do Log
        /// </summary>
        public String ArquivoLog { get; set; }

        /// <summary>
        /// Traces carregados do arquivo
        /// </summary>
        public List<Modelo.Trace> TracesLog { get; set; }

        /// <summary>
        /// Carrega o arquivo de log de tracing num objeto
        /// </summary>
        /// <param name="arquivoLog"></param>
        /// <returns></returns>
        public static List<Modelo.Trace> Load(StreamReader arquivoLog)
        {
            LogTracing log = new LogTracing();
            List<Modelo.Trace> logsDia = new List<Modelo.Trace>();

            try
            {
                String conteudoLog = arquivoLog.ReadToEnd();

                String[] delimitador = new String[] { "#F#" };
                String[] traces = conteudoLog.Split(delimitador, StringSplitOptions.None);
                String valor = "";
                foreach (String trace in traces)
                {
                    Modelo.Trace traceLog = new Modelo.Trace();
                    DateTime data = new DateTime();
                    Int32 codigo = 0;
                    String dadosTrace = trace.Replace("\r\n", "");

                    CultureInfo formato = CultureInfo.CreateSpecificCulture("en-US");

                    if (!String.IsNullOrEmpty(dadosTrace))
                    {
                        Int32 indexInicio = 0;
                        Int32 tamanho = 0;

                        //Data
                        indexInicio = dadosTrace.IndexOf("Data:") + 5;
                        tamanho = dadosTrace.IndexOf("Message:") - indexInicio;
                        valor = dadosTrace.Substring(indexInicio, tamanho);
                        if (DateTime.TryParse(valor, formato, DateTimeStyles.None, out data))
                            traceLog.DataHora = data;
                        else
                            traceLog.DataHora = data;

                        //Message
                        indexInicio = dadosTrace.IndexOf("Message:") + 8;
                        tamanho = dadosTrace.IndexOf("Sev:") - indexInicio;
                        valor = dadosTrace.Substring(indexInicio, tamanho);
                        traceLog.Mensagem = valor;

                        //Severidade
                        indexInicio = dadosTrace.IndexOf("Sev:") + 4;
                        tamanho = dadosTrace.IndexOf("Maq:") - indexInicio;
                        valor = dadosTrace.Substring(indexInicio, tamanho);
                        traceLog.Severidade = (Modelo.Trace.TraceEventType)Enum.Parse(typeof(Modelo.Trace.TraceEventType), valor);

                        //Máquina
                        indexInicio = dadosTrace.IndexOf("Maq:") + 4;
                        tamanho = dadosTrace.IndexOf("Thread:") - indexInicio;
                        valor = dadosTrace.Substring(indexInicio, tamanho);
                        traceLog.Maquina = valor;

                        //Entidade
                        indexInicio = dadosTrace.IndexOf("Ent:") + 4;
                        tamanho = dadosTrace.IndexOf("Proc:") - indexInicio;
                        valor = dadosTrace.Substring(indexInicio, tamanho);
                        if (Int32.TryParse(valor, out codigo))
                            traceLog.CodigoEntidade = codigo;
                        else
                            traceLog.CodigoEntidade = codigo;

                        //Processo
                        indexInicio = dadosTrace.IndexOf("Proc:") + 5;
                        tamanho = dadosTrace.Length - indexInicio;
                        valor = dadosTrace.Substring(indexInicio, tamanho);
                        traceLog.Processamento = (Modelo.Trace.TipoProcessamento)Enum.Parse(typeof(Modelo.Trace.TipoProcessamento), valor);

                        logsDia.Add(traceLog);
                    }
                }

                arquivoLog.Close();

            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Erro no Log do ISRobo", ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro no Log do ISRobo", ex);
            }
            finally
            {
                if (!object.ReferenceEquals(arquivoLog, null))
                {
                    arquivoLog.Close();
                }
            }

            return logsDia;
        }
    }
}
