using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1116 - Consultar por transação - Carta.
    /// </summary>
    public class ConsultarCartasRetornoDTO
    {
        public short CodigoMotivo { get; set; }
        public DateTime DataCancelamento { get; set; }
        public DateTime DataVenda { get; set; }
        public string DescricaoMotivo { get; set; }
        public string NumeroCartao { get; set; }
        public int NumeroEstabelecimento { get; set; }
        public decimal NumeroNsu { get; set; }
        public decimal NumeroProcesso { get; set; }
        public int NumeroResumo { get; set; }
        public decimal ValorAjuste { get; set; }
        public decimal ValorCancelamento { get; set; }
        public decimal ValorDebito { get; set; }
        public decimal ValorTransacao { get; set; }
    }
}
