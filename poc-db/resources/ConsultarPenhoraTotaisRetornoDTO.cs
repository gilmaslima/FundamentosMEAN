using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
    /// </summary>
    public class ConsultarPenhoraTotaisRetornoDTO
    {
        public int TotalTransacoes { get; set; }
        public int TotalProcessos { get; set; }
        public decimal TotalValorProcesso { get; set; }
        public decimal TotalValorPenhorado { get; set; }
    }
}
