/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos.PlanoContas
{
    /// <summary>
    /// Status de uma Oferta
    /// </summary>
    [DataContract]
    public enum StatusOferta : short
    {
        /// <summary>
        /// Oferta Contratada
        /// </summary>
        [EnumMember]
        Contratado = 0,

        /// <summary>
        /// Oferta Cancelada
        /// </summary>
        [EnumMember]
        Cancelado = 1,

        /// <summary>
        /// Oferta Conta Certa Pendente
        /// </summary>
        [EnumMember]
        Pendente = 2
    }
}