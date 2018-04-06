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
    /// Enumeração utilizado para manipular dados de tipos de alarme
    /// </summary>
    [DataContract]
    public enum TipoAlarme
    {
          [EnumMember]
        NaoAplicavel = 0,
          [EnumMember]
        POC = 1,
          [EnumMember]
        UTL = 2,
          [EnumMember]
        Score = 3		
    }
}
