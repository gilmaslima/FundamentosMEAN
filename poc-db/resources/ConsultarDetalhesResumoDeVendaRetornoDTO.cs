using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarDetalhesResumoDeVendaRetornoDTO
    {
        // WACA700
        public string Detalhe { get; set; }
        public short NumeroMes { get; set; }
        public string Timestamp { get; set; }
        public string TipoResumoVenda { get; set; }

        // WACA701
        public int ResumoVenda { get; set; }
        public decimal ValorApresentado { get; set; }
        public short QuantidadeComprovantesVenda { get; set; }
        public decimal ValorApurado { get; set; }
        public DateTime DataApresentacaoRetornado { get; set; }
        public decimal ValorDesconto { get; set; }
        public DateTime DataProcessamento { get; set; }
        public decimal ValorGorjetaTaxaEmbarque { get; set; }
        public string TipoResumo { get; set; }
        public decimal ValorCotacao { get; set; }
        public string TipoMoeda { get; set; }
        public string IndicadorTaxaEmbarque { get; set; }
    }
}
