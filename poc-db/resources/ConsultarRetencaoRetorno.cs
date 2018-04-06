using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
    /// </summary>
    [DataContract]
    public class ConsultarRetencaoRetorno
    {
        /// <summary>
        /// PR - ConsultarRetencaoNumeroProcessoRetorno, DC - ConsultarRetencaoDetalheProcessoCreditoRetorno, DD - ConsultarRetencaoDetalheProcessoDebitoRetorno, D1 - ConsultarRetencaoDescricaoComValorRetorno, D2 - ConsultarRetencaoDescricaoSemValorRetorno
        /// </summary>
        [DataMember]
        public List<BasicContract> Registros { get; set; }
        [DataMember]
        public ConsultarRetencaoTotaisRetorno Totais { get; set; }
        [DataMember]
        public int QuantidadeTotalRegistros { get; set; }
    }
}
