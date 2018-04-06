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
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo
{
    /// <summary>
    /// Enumeração utilizado para critérios de resultado de autorização.
    /// </summary>
    [DataContract]
    public enum CriterioResultadoAutorizacao
    {
        [EnumMember]
        Aprovada = 0,
        [EnumMember]
        Negada = 1
    }
}
