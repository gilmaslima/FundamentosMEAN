using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Servico
{
    /// <summary>
    /// Classe modelo para gravação de uma mensagem de log
    /// </summary>
    [DataContract]
    public class LogEntry
    {
        /// <summary>
        /// Id do Evento
        /// </summary>
        [DataMember]
        public Int32 EventId { get; set; }

        /// <summary>
        /// Id da prioridade
        /// </summary>
        [DataMember]
        public Int32 Priority { get; set; }

        /// <summary>
        /// Severidade da mensagem
        /// </summary>
        [DataMember]
        public String Severity { get; set; }

        /// <summary>
        /// Título da mensagem
        /// </summary>
        [DataMember]
        public String Title { get; set; }

        /// <summary>
        /// Horário de geração da mensagem
        /// </summary>
        [DataMember]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Nome da máquina/servidor que gerou a mensagem
        /// </summary>
        [DataMember]
        public String MachineName { get; set; }

        /// <summary>
        /// Nome do AppDomain onde a mensagem foi gerada
        /// </summary>
        [DataMember]
        public String AppDomainName { get; set; }

        /// <summary>
        /// Id do processo
        /// </summary>
        [DataMember]
        public String ProcessId { get; set; }

        /// <summary>
        /// Nome do processo
        /// </summary>
        [DataMember]
        public String ProcessName { get; set; }

        /// <summary>
        /// Nome da thread
        /// </summary>
        [DataMember]
        public String ManagedThreadName { get; set; }

        /// <summary>
        /// Nome da thread
        /// </summary>
        [DataMember]
        public String Win32ThreadId { get; set; }

        /// <summary>
        /// Mensagem informativa
        /// </summary>
        [DataMember]
        public String Message { get; set; }

        /// <summary>
        /// Id da atividade/operação.
        /// Este campo é chave, pois relaciona as entradas de log entre si
        /// </summary>
        [DataMember]
        public Guid ActivityId { get; set; }

        /// <summary>
        /// Código da entidade do usuário logado
        /// </summary>
        [DataMember]
        public Int32? CodigoEntidade { get; set; }

        /// <summary>
        /// Código do grupo da entidade do usuário logado
        /// </summary>
        [DataMember]
        public Int32? GrupoEntidade { get; set; }

        /// <summary>
        /// Login do usuário logado
        /// </summary>
        [DataMember]
        public String LoginUsuario { get; set; }

        /// <summary>
        /// Classe onde foi gerada a mensagem de log
        /// </summary>
        [DataMember]
        public String Classe { get; set; }

        /// <summary>
        /// Método onde foi gerada a mensagem de log
        /// </summary>
        [DataMember]
        public String Metodo { get; set; }
   
        /// <summary>
        /// Assembly/DLL que gerou a mensagem
        /// </summary>
        [DataMember]
        public String Assembly { get; set; }

        /// <summary>
        /// Linha no arquivo fonte da classe onde foi gerada a mensagem
        /// </summary>
        [DataMember]
        public Int32? LinhaCodigo { get; set; }

        /// <summary>
        /// Coluna/caractere no arquivo fonte da classe onde foi gerada a mensagem
        /// </summary>
        [DataMember]
        public Int32? ColunaCodigo { get; set; }

        /// <summary>
        /// Parâmetros (serializados)
        /// </summary>
        [DataMember]
        public String Parametros { get; set; }

        /// <summary>
        /// Exceção gerada
        /// </summary>
        [DataMember]
        public ExcecaoLog Excecao { get; set; }

        /// <summary>
        /// Exceção base gerada
        /// </summary>
        [DataMember]
        public ExcecaoLog ExcecaoBase { get; set; }
        
        /// <summary>
        /// Categorias da mensagem de log
        /// </summary>
        [DataMember]
        public List<String> Categorias { get; set; }
    }

    /// <summary>
    /// Classe modelo de exceção para o log
    /// </summary>
    [DataContract]
    public class ExcecaoLog
    {
        /// <summary>
        /// Mensagem da exceção
        /// </summary>
        [DataMember]
        public String Mensagem { get; set; }

        /// <summary>
        /// Origem da exceção
        /// </summary>
        [DataMember]
        public String Origem { get; set; }

        /// <summary>
        /// Pilha de rastreamento da exceção
        /// </summary>
        [DataMember]
        public String StackTrace { get; set; }
    }
}