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
    /// Enumeração utilizado para resultados de autorização.
    /// </summary>
    [DataContract]
    public enum ResultadoAutorizacao
    {
          [EnumMember]
        NaoAplicavel = 0,
          [EnumMember]
        Autorizado = 1,
          [EnumMember]
        NaoAutorizado =2
    }
}
