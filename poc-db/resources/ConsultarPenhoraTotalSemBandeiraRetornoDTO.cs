using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
    /// </summary>
    public class ConsultarPenhoraTotalSemBandeiraRetornoDTO : BasicDTO
    {
        public DateTime DataProcesso { get; set; }
        public DateTime DataApresentacao { get; set; }
        public DateTime DataVencimento { get; set; }
        public int NumeroEstabelecimento { get; set; }
        public int NumeroRV { get; set; }
        public string TipoBandeira { get; set; }
        public int QuantidadeTransacoes { get; set; }
        public string DescricaoResumo { get; set; }
        public decimal ValorPenhorado { get; set; }
    }
}
