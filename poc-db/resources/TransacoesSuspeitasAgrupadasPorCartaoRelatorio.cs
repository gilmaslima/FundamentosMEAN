using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Sharepoint.Modelo
{
    public class TransacoesSuspeitasAgrupadasPorCartaoRelatorio
    {
        public string NumeroCartao { get; set; }
        public string DataHoraTransacaoSuspeita { get; set; }
        public string Score { get; set; }
        public string ValorTransacoesSuspeitas { get; set; }
        public string ValorTransacoesSuspeitasAprovadas { get; set; }
        public string QuantidadeTransacoesSuspeitasAprovadas { get; set; }
        public string ValorTransacoesSuspeitasNegadas { get; set; }
        public string QuantidadeTransacoesSuspeitasNegadas { get; set; }
        public string TipoCartao { get; set; }
    }
}
