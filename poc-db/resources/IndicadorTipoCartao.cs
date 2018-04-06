/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo
{
    /// <summary>
    /// Enumeração utilizado para indicadores de tipo de cartão.
    /// </summary>
    [DataContract]
    public enum IndicadorTipoCartao
    {
        [EnumMember]
        Nenhum = 0,
        [EnumMember]
        Debito = 1,
        [EnumMember]
        Credito = 2,
        [EnumMember]
        Ambos = 3
    }
}