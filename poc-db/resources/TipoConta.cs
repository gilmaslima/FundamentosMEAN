/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos.Corban
{
    /// <summary>
    /// Enumerador dos Tipos de Conta Corban para filtragem
    /// </summary>
    [DataContract]
    public enum TipoConta : short
    {
        /// <summary>
        /// Todos os tipos
        /// </summary>
        [EnumMember]
        Todos = 0,

        /// <summary>
        /// Tipo de Conta: Títulos
        /// </summary>
        [EnumMember]
        Titulos = 1,

        /// <summary>
        /// Tipo de Conta: Concessionária
        /// </summary>
        [EnumMember]
        Concessionaria = 2,

        /// <summary>
        /// Tipo de Conta: Tributos
        /// </summary>
        [EnumMember]
        Tributos = 3
    }
}