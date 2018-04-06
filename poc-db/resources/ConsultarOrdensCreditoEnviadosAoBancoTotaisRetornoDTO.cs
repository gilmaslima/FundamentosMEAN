using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO
    {
        public int TotalTransacoes { get; set; }
        public decimal TotalValorCredito { get; set; }
    }
}
