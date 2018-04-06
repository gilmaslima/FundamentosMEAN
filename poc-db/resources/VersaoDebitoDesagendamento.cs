using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// Versão do progrma a ser chamado no mainframe (ISFx ou ISDx).
    /// ISFx: programas novos
    /// ISDx: programas antigos
    /// </summary>
    [DataContract]
    public enum VersaoDebitoDesagendamento
    {
        /// <summary>ISFx</summary>
        [EnumMember]
        ISF,
        /// <summary>ISDx</summary>
        [EnumMember]
        ISD
    }
}