using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Sharepoint.Modelo
{
    public class TransacoesSuspeitasAgrupadasPorEstabelecimentoRelatorio
    {
        public string NumeroEstabelecimento { get; set; }
        public string NomeFantasiaEstabelecimento { get; set; }
        public string ValorTotalTransacoes { get; set; }
        public string QuantidadeTotalTransacoes { get; set; }
        public string ValorTransacoesSuspeitasAprovadas { get; set; }
        public string QuantidadeTransacoesSuspeitasAprovadas { get; set; }
        public string ValorTransacoesSuspeitasNegadas { get; set; }
        public string QuantidadeTransacoesSuspeitasNegadas { get; set; }
        public string TipoCartao { get; set; }
    }
}
