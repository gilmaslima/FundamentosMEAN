using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarOrdensCreditoEnviadosAoBancoEnvioDTO
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public short CodigoBandeira { get; set; }
        public List<int> Estabelecimentos { get; set; }
    }
}
