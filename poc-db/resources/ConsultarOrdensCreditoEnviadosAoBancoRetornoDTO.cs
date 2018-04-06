using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarOrdensCreditoEnviadosAoBancoRetornoDTO
    {
        /// <summary>
        /// DT - ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO, TB - ConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO
        /// </summary>
        public List<BasicDTO> Registros { get; set; }
        public ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO Totais { get; set; }
    }
}
