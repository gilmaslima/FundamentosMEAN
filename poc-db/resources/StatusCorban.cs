/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos.Corban
{
    /// <summary>
    /// Tipos de Status de Transações Corban para filtragem
    /// </summary>
    [DataContract]
    public enum StatusCorban
    {
        /// <summary>
        /// Todos os tipos de status
        /// </summary>
        [EnumMember]
        Todos = ' ',

        /// <summary>
        /// Transações aprovadas/confirmadas
        /// </summary>
        [EnumMember]
        Confirmadas = 'C',

        /// <summary>
        /// Transações desfeitas/estornadas
        /// </summary>
        [EnumMember]
        Estornadas = 'E'
    }
}