using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO : BasicDTO
    {
        public string TipoBandeira { get; set; }
        public string DescricaoAjuste { get; set; }
        public int TotalTransacoes { get; set; }
        public decimal TotalValorCredito { get; set; }
    }
}
