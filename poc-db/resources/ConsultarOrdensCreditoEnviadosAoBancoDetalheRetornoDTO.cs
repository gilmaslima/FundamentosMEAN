using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO : BasicDTO
    {
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public int NumeroEstabelecimento { get; set; }
        public int NumeroResumoVenda { get; set; }
        public string Tipobandeira { get; set; }
        public string StatusOcorrenica { get; set; }
        public string DescricaoResumoAjuste { get; set; }
        public decimal ValorCredito { get; set; }
        public string IndicadorSinalValor { get; set; }
        public short BancoCredito { get; set; }
        public int AgenciaCredito { get; set; }
        public string ContaCorrente { get; set; }        
        public Boolean IndicadorRecarga { get;set; }
        public Int16 CodigoAjuste { get; set; }
    }
}
