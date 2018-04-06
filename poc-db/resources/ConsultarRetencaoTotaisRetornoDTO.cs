using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
    /// </summary>
    public class ConsultarRetencaoTotaisRetornoDTO
    {
        public int TotalTransacoes { get; set; }
        public int TotalProcessos { get; set; }
        public decimal TotalValorProcesso { get; set; }
        public decimal TotalValorRetencao { get; set; }
    }
}
