using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarExtratoDirfEstabelecimentoRetornoDTO
    {
        public decimal ValorCobrado { get; set; }
        public decimal ValorIrRecebido { get; set; }
        public decimal ValorRecebido { get; set; }
        public decimal ValorRepassadoEmissor { get; set; }
    }
}
