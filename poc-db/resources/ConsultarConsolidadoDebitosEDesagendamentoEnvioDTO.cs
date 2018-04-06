using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarConsolidadoDebitosEDesagendamentoEnvioDTO
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int[] Estabelecimentos { get; set; }
        public String Versao { get; set; }
    }
}
