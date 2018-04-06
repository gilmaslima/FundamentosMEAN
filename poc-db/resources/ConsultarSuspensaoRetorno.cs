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
    public class ConsultarSuspensaoRetorno
    {
        /// <summary>
        /// DT - ConsultarSuspensaoDetalheRetorno, T1 - ConsultarSuspensaoTotalBandeiraDiaRetorno, T2 - ConsultarSuspensaoTotalDiaRetorno, T3 - ConsultarSuspensaoTotalBandeiraRetorno, T4 - ConsultarSuspensaoTotalPeriodoRetorno
        /// </summary>
        [DataMember]
        public List<BasicContract> Registros { get; set; }
        [DataMember]
        public ConsultarSuspensaoTotaisRetorno Totais { get; set; }
        [DataMember]
        public int QuantidadeTotalRegistros { get; set; }

    }
}
