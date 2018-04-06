using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos.
    /// </summary>
    public class ConsultarSuspensaoTotalBandeiraRetornoDTO : BasicDTO
    {
        public DateTime DataSuspensao { get; set; }
        public DateTime DataApresentacao { get; set; }
        public DateTime DataVencimento { get; set; }
        public int NumeroEstabelecimento { get; set; }
        public int NumeroRV { get; set; }
        public string TipoBandeira { get; set; }
        public int QuantidadeTransacoes { get; set; }
        public string DescricaoResumo { get; set; }
        public decimal ValorSuspensao { get; set; }
        public short CodigoBanco { get; set; }
        public short CodigoAgencia { get; set; }
        public string NumeroConta { get; set; }
    }
}
