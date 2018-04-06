using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarSaldosEmAbertoDTO
    {
        public List<BasicDTO> Registro { get; set; }
        public TotalizadorPorBandeiraSaldosEmAbertoDTO Totais { get; set; }

        public decimal ValorLiquido { get; set; }
        public Int32 QuantidadeRegistros { get; set; }  
    }
}
