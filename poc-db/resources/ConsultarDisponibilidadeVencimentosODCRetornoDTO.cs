using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarDisponibilidadeVencimentosODCRetornoDTO
    {
        public DateTime DataAntecipacao { get; set; }
        public DateTime DataVencimento { get; set; }
        public int NumeroEstabelecimento { get; set; }
        public int NumeroOdc { get; set; }
        public string NomeEstabelecimento { get; set; }
        public short PrazoRecebimento { get; set; }
        public short Status { get; set; }
        public decimal ValorOrdemCredito { get; set; }
    }
}
