using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais.Modelo
{
    public class Trace
    {
        /// <summary>
        /// Tipos de processamento do ISRobô
        /// </summary>
        public enum TipoProcessamento
        {
            /// <summary>
            /// Logs do serviço
            /// </summary>
            Servico,
            /// <summary>
            /// Logs de processamento do cadastro de Entidades
            /// </summary>
            Entidade,
            /// <summary>
            /// Logs de processamento do cadastro de Usuários
            /// </summary>
            Usuario
        }

        /// <summary>
        /// Identifica o tipo do evento do trace
        /// </summary>
        public enum TraceEventType
        {
            /// <summary>
            /// Erro Fatal
            /// </summary>
            Critical = 1,
            /// <summary>
            /// Erro recuperável
            /// </summary>
            Error = 2,
            /// <summary>
            /// Problema não crítico
            /// </summary>
            Warning = 4,
            /// <summary>
            /// Mensagem de informação
            /// </summary>
            Information = 8,
            /// <summary>
            /// Mensagem de trace
            /// </summary>
            Verbose = 16,
            /// <summary>
            /// Início lógico de uma operação
            /// </summary>
            Start = 256,
            /// <summary>
            /// Término lógico de uma operação
            /// </summary>
            Stop = 512,
            /// <summary>
            /// Suspensação de uma operação lógica
            /// </summary>
            Suspend = 1024,
            /// <summary>
            /// Continuação de uma operação lógica
            /// </summary>
            Resume = 2048,
            ///<summary>
            /// Mudança de Correlation
            /// </summary>
            Transfer = 4096
        }

        /// <summary>
        /// Timestamp da gravação do Log
        /// </summary>
        public DateTime DataHora { get; set; }

        /// <summary>
        /// Mensagem gravada no log
        /// </summary>
        public String Mensagem { get; set; }

        /// <summary>
        /// Máquina onde o log foi gerado
        /// </summary>
        public String Maquina { get; set; }

        /// <summary>
        /// Código da Entidade
        /// </summary>
        public Int32 CodigoEntidade { get; set; }

        /// <summary>
        /// Tipo de processamento  do trace
        /// </summary>
        public TipoProcessamento Processamento { get; set; }

        /// <summary>
        /// Servive
        /// </summary>
        public TraceEventType Severidade { get; set; }
    }
}
