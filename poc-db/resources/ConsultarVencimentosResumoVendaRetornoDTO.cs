using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
    /// </summary>
    public class ConsultarVencimentosResumoVendaRetornoDTO
    {
        public DateTime DataVencimento { get; set; }
        public short NumeroPeca { get; set; }
        public short QuantidadePecas { get; set; }
        public short QuantidadeComprovanteVenda { get; set; }
        public string Descricao { get; set; }
        public decimal ValorApresentado { get; set; }
        public decimal ValorLiquido { get; set; }
        public int Banco { get; set; }
        public int Agencia { get; set; }
        public string Conta { get; set; }
    }
}
