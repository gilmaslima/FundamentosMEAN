using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA617 - Resumo de vendas - Cartões de débito.
    /// </summary>
    public class ConsultarPreDatadosRetornoDTO
    {
        public short CodigoSubTransacao { get; set; }
        public short CodigoTransacao { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal DescontoTaxaCredito { get; set; }
        public string Descricao { get; set; }
        public string DescricaoBandeira { get; set; }
        public short NumeroPeca { get; set; }
        public short QuantidadeComprovantesVenda { get; set; }
        public short QuantidadePecas { get; set; }
        public string ReservaDados { get; set; }
        public string TipoVenda { get; set; }
        public decimal ValorApresentado { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorLiquido { get; set; }
        public decimal ValorSaque { get; set; }
        public decimal ValorTotalCpmf { get; set; }
        public decimal ValorTotalIof { get; set; }
    }
}
