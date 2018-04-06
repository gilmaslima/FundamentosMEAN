using log4net;
using Rede.AppConciliador.HistoricoAtividadeServico;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Rede.AppConciliador.LogMonitoracao
{
    /// <summary>
    /// Classe para Registro dos Logs de Monitoração de Envio de Arquivo
    /// </summary>
    public class LogHistorico
    {
        /// <summary>
        /// Objeto de Log
        /// </summary>
        /// <returns></returns>
        private static ILog logger = null;

        /// <summary>
        /// Propriedade estática de Log em Arquivo Físico
        /// </summary>
        static ILog Logger
        {
            get
            {
                if (object.ReferenceEquals(logger, null))
                {
                    logger = LogManager.GetLogger(typeof(LogHistorico));
                }
                return logger;
            }
        }
        
        /// <summary>
        /// Status do envio de Arquivo
        /// </summary>
        public enum StatusEnvio 
        {
            Sucesso = 0,
            Erro = 1
        }

        /// <summary>
        /// Gravar histórico do envio de arquivo
        /// </summary>
        public static void GravarHistorico(String nomeArquivo, DateTime dataHoraInicio, DateTime dataHoraFim, StatusEnvio status, String mensagemErro)
        {
            try
            {
                Int32 codigoAtividadeEnvioArquivo = 16;
                String nomeUsuarioResponsavel = "EnvioArquivosConciliador";

                var historicoEnvio = new HistoricoAtividadeServico.HistoricoEnvioArquivoConciliador()
                {
                    CodigoErro = (Int32)status,
                    MensagemErro = mensagemErro,
                    DataHoraEntrada = dataHoraInicio.ToString(),
                    DataHoraSaida = dataHoraFim.ToString(),
                    Guid = 0,
                    NomeArquivo = nomeArquivo,
                    Status = (Int16)status
                };

                var historico = new HistoricoAtividadeServico.HistoricoAtividade()
                {
                    CodigoEntidade = null,
                    CodigoIdUsuario = null,
                    CodigoTipoAtividade = codigoAtividadeEnvioArquivo,
                    CodigoTipoAtividadeSpecified = true,
                    Codigo = 0,
                    Detalhes = SerializarObjetoXmlString(historicoEnvio),
                    EmailUsuario = null,
                    NomeUsuario = nomeUsuarioResponsavel,
                    PerfilUsuario = null,
                    FuncionalOperador = null,
                    Ip = null,
                    CodigoMatriz = null
                };
                
                using (var client = new HistoricoAtividadeServico.HistoricoAtividadeServicoClient())
                {
                    Int64 codigoHistoricoAtividade = client.GravarHistorico(historico);
                }
            }
            catch (FaultException<GeneralFault> ex)
            {
                Logger.Error("Erro no Serviço do Log de Monitoração", ex);
            }
            catch (Exception ex)
            {
                Logger.Error("Erro ao enviar gravar o Log de Monitoração", ex);
            }
        }

        /// <summary>
        /// Serializa um objeto como String XML
        /// </summary>
        /// <param name="objeto"></param>
        /// <returns></returns>
        private static String SerializarObjetoXmlString(HistoricoAtividadeServico.HistoricoEnvioArquivoConciliador objeto)
        {
            XElement[] xmlHistoricoPropriedades = new XElement[] { 
                new XElement("DataHoraEntrada", objeto.DataHoraEntrada),
                new XElement("DataHoraSaida", objeto.DataHoraSaida),
                new XElement("CodigoErro", objeto.CodigoErro),
                new XElement("NomeArquivo", objeto.NomeArquivo),
                new XElement("MensagemErro", objeto.MensagemErro),
                new XElement("Status", objeto.Status)
            };

            XElement xml = new XElement("Historicos", xmlHistoricoPropriedades);
            
            return xml.ToString();
        }
    }
}
