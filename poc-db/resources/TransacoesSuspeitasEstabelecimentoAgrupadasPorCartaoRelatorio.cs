using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Sharepoint.Modelo
{
    public class TransacoesSuspeitasEstabelecimentoAgrupadasPorCartaoRelatorio
    {
        public string Cartao { get; set; }
        public string ValorTotalTransacoesSuspeitas { get; set; }
        public string QuantidadeTransacoesSuspeitas { get; set; }
        public string ValorTransacoesAprovadas { get; set; }
        public string QuantidadeTransacoesAprovadas { get; set; }
        public string ValorTransacoesNegadas { get; set; }
        public string QuantidadeTransacoesNegadas { get; set; }
    }
}
