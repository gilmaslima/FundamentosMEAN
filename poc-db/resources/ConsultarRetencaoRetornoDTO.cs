using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
    /// </summary>
    public class ConsultarRetencaoRetornoDTO
    {
        /// <summary>
        /// PR - ConsultarRetencaoNumeroProcessoRetornoDTO, DC - ConsultarRetencaoDetalheProcessoCreditoRetornoDTO, DD - ConsultarRetencaoDetalheProcessoDebitoRetornoDTO, D1 - ConsultarRetencaoDescricaoComValorRetornoDTO, D2 - ConsultarRetencaoDescricaoSemValorRetornoDTO
        /// </summary>
        public List<BasicDTO> Registros { get; set; }
        public ConsultarRetencaoTotaisRetornoDTO Totais { get; set; }
    }
}
