using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Sharepoint.Modelo
{
    public class AnalisaTransacoesSuspeitasRelatorio
    {
        public string TipoAlarme { get; set; }
        public string Cartao { get; set; }
        public string Valor { get; set; }
        public string DataHoraTransacao { get; set; }
        public string Score { get; set; }
        public string MCC { get; set; }
        public string TipoCartao { get; set; }
        public string Uf { get; set; }
        public string Bandeira { get; set; }
        public string StatusTransacao { get; set; }
    }
}
