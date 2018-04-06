using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos.
    /// </summary>
    public class ConsultarSuspensaoRetornoDTO
    {
        /// <summary>
        /// DT - ConsultarSuspensaoDetalheRetornoDTO, T1 - ConsultarSuspensaoTotalBandeiraDiaRetornoDTO, T2 - ConsultarSuspensaoTotalDiaRetornoDTO, T3 - ConsultarSuspensaoTotalBandeiraRetorno, T4 - ConsultarSuspensaoTotalPeriodoRetornoDTO
        /// </summary>
        public List<BasicDTO> Registros { get; set; }
        public ConsultarSuspensaoTotaisRetornoDTO Totais { get; set; }
    }
}
