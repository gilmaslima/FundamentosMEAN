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
    /// Este componente publica a classe MCC, que expõe métodos para manipular as propriedades de merchant category code.
    /// </summary>
    public class MCC
    {
        public string CodigoMCC { get; set; }
        public string DescricaoMCC { get; set; }
    }
}
