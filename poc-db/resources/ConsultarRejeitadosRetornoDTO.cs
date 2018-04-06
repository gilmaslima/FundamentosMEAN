using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarRejeitadosRetornoDTO
    {
        public int Autorizacao { get; set; }
        public string Cartao { get; set; }
        public DateTime DataComprovanteVenda { get; set; }
        public string Descricao { get; set; }
        public int NumeroEstabelecimento { get; set; }
        public short Sequencia { get; set; }
        public decimal Valor { get; set; }
        public string IndicadorTokenizacao { get; set; }
    }
}
