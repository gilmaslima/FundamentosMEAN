using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Rede.PN.Cancelamento.Modelo
{
    /// <summary>
    /// Tipo de Cancelamento
    /// </summary>
    [Serializable]
    [DataContract]
    public enum TipoCancelamento
    {
        /// <summary>
        /// Total
        /// </summary>
        [EnumMember]
        [Description("Total")]
        Total,

        /// <summary>
        /// Parcial
        /// </summary>
        [EnumMember]
        [Description("Parcial")]
        Parcial
    }
}
