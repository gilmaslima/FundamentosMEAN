/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Vendas
{
    /// <summary>
    /// Enumerador para o tipo do PV de Recarga de Celular (PV Físico ou PV Lógico)
    /// </summary>
    [DataContract]
    public enum RecargaCelularTipoPv
    {
        /// <summary>
        /// Valor não definido
        /// </summary>
        [EnumMember]
        NaoDefinido = 0,

        /// <summary>
        /// PV Físico
        /// </summary>
        [EnumMember]
        PvFisico = 1,

        /// <summary>
        /// PV Lógico
        /// </summary>
        [EnumMember]
        PvLogico = 2
    }
}