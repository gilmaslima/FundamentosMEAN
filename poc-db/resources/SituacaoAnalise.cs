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
    /// Enumeração utilizado para situações de análise de pesquisa.
    /// </summary>
    [DataContract]
    public enum SituacaoAnalisePesquisa
    {
        [EnumMember]
        Analisada = 1,
        [EnumMember]
        NaoAnalisada = 2,
        [EnumMember]
        Ambos = 3
    }
}
