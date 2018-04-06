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
    /// Este componente publica a classe FaixaBin, que expõe métodos para manipular as proriedade de faixa bin.
    /// </summary>
    public class FaixaBin
    {
        public string ValorInicial { get; set; }
        public string ValorFinal { get; set; }
    }
}
