using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos.
    /// </summary>
    public class ConsultarSuspensaoTotaisRetornoDTO
    {
        public int TotalTransacoes { get; set; }
        public decimal TotalValorSuspencao { get; set; }
    }
}
