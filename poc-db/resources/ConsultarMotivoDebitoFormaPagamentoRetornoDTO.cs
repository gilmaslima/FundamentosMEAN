using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarMotivoDebitoFormaPagamentoRetornoDTO
    {
        public string BandeiraResumoVendasDebitado { get; set; }
        public string DataPagamento { get; set; }
        public string EstabelecimentoDebito { get; set; }
        public string MeioPagamento { get; set; }
        public string ResumoVendaDebito { get; set; }
        public string SinalDebitoCredito { get; set; }
        public decimal ValorComplementar { get; set; }
        public decimal ValorDebito { get; set; }
        public decimal ValorPendente { get; set; }
    }
}
