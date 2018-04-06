using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarDetalhamentoDebitosRetornoDTO
    {
        public List<ConsultarDetalhamentoDebitosDetalheRetornoDTO> Registros { get; set; }
        public ConsultarDetalhamentoDebitosTotaisRetornoDTO Totais { get; set; }
    }
}
