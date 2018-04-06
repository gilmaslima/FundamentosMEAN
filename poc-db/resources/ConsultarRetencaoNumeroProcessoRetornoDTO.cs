using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
    /// </summary>
    public class ConsultarRetencaoNumeroProcessoRetornoDTO : BasicDTO
    {
        public string NumeroProcesso { get; set; }
        public decimal ValorTotalProcesso { get; set; }
        public int QuantidadeDetalheProcesso { get; set; }
    }
}
