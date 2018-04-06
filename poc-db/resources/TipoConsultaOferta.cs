using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos.PlanoContas
{
    /// <summary>
    /// Enumeração de Tipo de Consulta de Oferta
    /// </summary>
    [DataContract]
    public enum TipoEstabelecimento : short
    {               
        /// <summary>
        /// Matriz
        /// </summary>
        [EnumMember]
        Matriz = 1,
        
        /// <summary>
        /// Filial
        /// </summary>
        [EnumMember]
        Filial = 2
    }
}