using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarDetalhamentoDebitosDetalheRetornoDTO
    {
        public string TipoRegistro { get; set; }
        public DateTime DataInclusao { get; set; }
        public int EstabelecimentoOrigem { get; set; }
        public string MotivoDebito { get; set; }
        public string IndicadorDesagendamento { get; set; }
        public string ProcessoReferente { get; set; }
        public string Resumo { get; set; }
        public string Bandeira { get; set; }
        public decimal ValorDebito { get; set; }
        public decimal ValorCompensado { get; set; }
        public decimal ValorPendente { get; set; }
        public DateTime DataPagamento { get; set; }
        public string Timestamp { get; set; }
        public decimal NumeroDebito { get; set; }
        public string IndicadorDebitoCredito { get; set; }
    }
}
