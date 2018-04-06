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
    /// Este componente publica a classe ParametrosSistema, que expõe métodos para manipular os parâmetros de sistema.
    /// </summary>
    public class ParametrosSistema
    {
        public int PossuiAcesso { get; set; }
        public int QuantidadeMaximaIntervaloDiasPesquisas { get; set; }
        public int QuantidadeMaximaDiasRetroativosPesquisas { get; set; }
        public int TempoExpiracaoBloqueio { get; set; }
		
    }
}
