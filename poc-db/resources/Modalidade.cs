/*
© Copyright 2015 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo.Vendas
{
    /// <summary>
    /// Modalidade
    /// </summary>
    [DataContract]
    public enum Modalidade : short
    {
        /// <summary>
        /// Todos
        /// </summary>
        [EnumMember]
        Todos = 0,

        /// <summary>
        /// À Vista
        /// </summary>
        [EnumMember]
        AVista = 1,

        /// <summary>
        /// Pré-Datado
        /// </summary>
        [EnumMember]        
        PreDatado = 2,

        /// <summary>
        /// Trishop
        /// </summary>
        [EnumMember]
        Trishop = 3,

        /// <summary>
        /// Compre & Saque
        /// </summary>
        [EnumMember]
        CompreESaque = 4,

        /// <summary>
        /// Parcele Mais
        /// </summary>
        [EnumMember]
        ParceleMais = 5,

        /// <summary>
        /// Pagamento de Faturas
        /// </summary>
        [EnumMember]
        PagamentoDeFaturas = 6
    }
}