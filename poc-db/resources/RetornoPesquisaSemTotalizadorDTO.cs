using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class RetornoPesquisaSemTotalizadorDTO<TRetornoDTO>
    {
        public List<TRetornoDTO> Registros { get; set; }
        public int QuantidadeTotalRegistros { get; set; }
    }
}
