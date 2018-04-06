using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.SharePoint
{
    [Serializable]
    public class TransacaoDadosConsultaDTO
    {
        public int NumeroEstabelecimento { get; set; }
        public string TipoVenda { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string NumeroCartao { get; set; }
        public decimal Nsu { get; set; }
        public String TID { get; set; }
    }
}
