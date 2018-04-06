using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1108 - Home - Lançamentos futuros.
    /// </summary>
    public class ConsultarCreditoDebitoEnvioDTO
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> Estabelecimentos { get; set; }
    }
}
