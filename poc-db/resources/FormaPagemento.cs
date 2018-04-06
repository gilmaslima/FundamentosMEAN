/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos.Corban
{
    /// <summary>
    /// Enumerador com os tipos de Forma de Pagamento Corban para filtro
    /// </summary>
    [DataContract]
    public enum FormaPagemento : short
    {
        /// <summary>
        /// Todos os tipos de pagamento
        /// </summary>
        [EnumMember]
        Todos = 0,

        /// <summary>
        /// Pagamento em Crédito
        /// </summary>
        [EnumMember]
        Credito = 1,

        /// <summary>
        /// Pagamento em Débito
        /// </summary>
        [EnumMember]
        Debito = 2,

        /// <summary>
        /// Pagamento em Dinheiro
        /// </summary>
        [EnumMember]
        Dinheiro = 3
    }
}