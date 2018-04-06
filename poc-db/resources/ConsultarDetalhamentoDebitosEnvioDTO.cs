using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarDetalhamentoDebitosEnvioDTO
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string TipoPesquisa { get; set; }
        public short CodigoBandeira { get; set; }
        public List<int> Estabelecimentos { get; set; }
        public string ChavePesquisa { get; set; }
        public String Versao { get; set; }
    }
}
