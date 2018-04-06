using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarWACA701RetornoDTO
    {
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
