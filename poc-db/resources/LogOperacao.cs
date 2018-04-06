using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Servicos
{
    /// <summary>
    /// Classe modelo para consulta de operações armazenadas no log
    /// </summary>
    [DataContract]
    public class LogOperacao
    {
        /// <summary>
        /// Id da atividade/operação de log
        /// </summary>
        [DataMember]
        public Guid ActivityID { get; set; }

        /// <summary>
        /// Horário de início da operação de log
        /// </summary>
        [DataMember]
        public DateTime Inicio { get; set; }

        /// <summary>
        /// Horário de término da operação de log
        /// </summary>
        [DataMember]
        public DateTime Termino { get; set; }

        /// <summary>
        /// Quantidade de eventos associados à operação de log
        /// </summary>
        [DataMember]
        public Int32 Quantidade { get; set; }

        /// <summary>
        /// Flag indicando se algum evento da operação apresentou erro
        /// </summary>
        [DataMember]
        public Boolean PossuiErro { get; set; }

        /// <summary>
        /// Login do usuário que gerou a operação
        /// </summary>
        [DataMember]
        public String LoginUsuario { get; set; }

        /// <summary>
        /// Código da entidade do usuário que gerou a operação
        /// </summary>
        [DataMember]
        public Int32? CodigoEntidade { get; set; }

        /// <summary>
        /// Grupo da entidade do usuário que gerou a operação
        /// </summary>
        [DataMember]
        public Int32? GrupoEntidade { get; set; }

        /// <summary>
        /// Mensagem do primeiro evento da operação
        /// </summary>
        [DataMember]
        public String Mensagem { get; set; }

        /// <summary>
        /// Classe do primeiro evento da operação
        /// </summary>
        [DataMember]
        public String Classe { get; set; }

        /// <summary>
        /// Método do primeiro evento da operação
        /// </summary>
        [DataMember]
        public String Metodo { get; set; }
    }
}