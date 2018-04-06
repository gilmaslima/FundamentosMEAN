using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
    /// </summary>
    public class ConsultarRetencaoEnvioDTO
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public short CodigoBandeira { get; set; }
        public List<int> Estabelecimentos { get; set; }
    }
}
