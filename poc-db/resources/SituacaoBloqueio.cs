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

namespace Redecard.PN.FMS.Modelo
{
    /// <summary>
    /// Enumeração utilizado para tipos de situação de bloqueio.
    /// </summary>
    public enum SituacaoBloqueio
    {
        SemBloqueio = 0,
        BloqueadoOutroUsuario = 1,
        BloqueadoMesmoUsuario = 2
    }
}
