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
    /// Enumeração utilizado para tipos de período. 
    /// </summary>
    [DataContract]
    public enum TipoPeriodo
    {
        [EnumMember]
        DataTransacao = 1,
        [EnumMember]
        DataEnvioAnalise =2
    }
}
