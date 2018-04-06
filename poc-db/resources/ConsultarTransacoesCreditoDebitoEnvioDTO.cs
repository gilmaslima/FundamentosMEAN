using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1107 - Home - Últimas Vendas.
    /// </summary>
    public class ConsultarTransacoesCreditoDebitoEnvioDTO
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> Estabelecimentos { get; set; }
    }
}
