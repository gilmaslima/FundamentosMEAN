using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class RetornoPesquisaComTotalizadorDTO<TRetornoDTO, TTotalizadorDTO>
    {
        public List<TRetornoDTO> Registros { get; set; }
        public TTotalizadorDTO Totalizador { get; set; }
        public int QuantidadeTotalRegistros { get; set; }
    }
}
