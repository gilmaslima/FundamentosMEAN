using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
    /// </summary>
    public class ConsultarPenhoraRetornoDTO
    {
        /// <summary>
        /// PR - ConsultarPenhoraNumeroProcessoRetornoDTO, DT - ConsultarPenhoraDetalheProcessoCreditoRetornoDTO, T1 - ConsultarPenhoraTotalBandeiraRetornoDTO, TP - ConsultarPenhoraTotalSemBandeiraRetornoDTO
        /// </summary>
        public List<BasicDTO> Registros { get; set; }
        public ConsultarPenhoraTotaisRetornoDTO Totais { get; set; }
    }
}
