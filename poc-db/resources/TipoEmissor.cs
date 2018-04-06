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

namespace PN.FMS.Modelo
{
    /// <summary>
    /// Enumeração utilizado para tipos de emissor.
    /// </summary>
    public enum TipoEmissor
    {
        NaoAplicavel = 0,
        POC = 1,
        UTL = 2,
        Score =3
    }
}
