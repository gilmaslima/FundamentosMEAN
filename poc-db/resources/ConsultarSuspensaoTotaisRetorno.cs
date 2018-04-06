using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos.
    /// </summary>
    [DataContract]
    public class ConsultarSuspensaoTotaisRetorno
    {
        [DataMember]
        public int TotalTransacoes { get; set; }
        [DataMember]
        public decimal TotalValorSuspencao { get; set; }
    }
}
