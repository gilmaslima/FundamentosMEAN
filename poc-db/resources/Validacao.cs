using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Rede.PN.Cancelamento.Modelo
{
    /// <summary>
    /// Validação
    /// </summary>
    [DataContract]
    public class Validacao
    {
        /// <summary>
        /// Código de retorno
        /// </summary>
        [DataMember]
        public Int32 CodigoRetorno { get; set; }

        /// <summary>
        /// Descrição
        /// </summary>
        [DataMember]
        public String Descricao { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public Int32 Status { get; set; }

        /// <summary>
        /// Linha
        /// </summary>
        [DataMember]
        public Int32 Linha { get; set; }
    }
}
