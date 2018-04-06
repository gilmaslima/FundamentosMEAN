using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
    /// </summary>
    public class ConsultarPenhoraNumeroProcessoRetornoDTO : BasicDTO
    {
        public string NumeroProcesso { get; set; }
        public decimal ValorTotalProcesso { get; set; }
        public int QuantidadeDetalheProcesso { get; set; }
    }
}
