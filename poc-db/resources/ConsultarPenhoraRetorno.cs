using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
    /// </summary>
    [DataContract]
    public class ConsultarPenhoraRetorno
    {
        /// <summary>
        /// PR - ConsultarPenhoraNumeroProcessoRetorno, DT - ConsultarPenhoraDetalheProcessoCreditoRetorno, T1 - ConsultarPenhoraTotalBandeiraRetorno, TP - ConsultarPenhoraTotalSemBandeiraRetorno
        /// </summary>
        [DataMember]
        public List<BasicContract> Registros { get; set; }
        [DataMember]
        public ConsultarPenhoraTotaisRetorno Totais { get; set; }
        [DataMember]
        public int QuantidadeTotalRegistros { get; set; }
    }
}
