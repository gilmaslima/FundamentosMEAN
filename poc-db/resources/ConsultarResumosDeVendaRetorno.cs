using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarResumosDeVendaRetorno
    {
        [DataMember]
        public string Detalhe { get; set; }
        [DataMember]
        public short NumeroMes { get; set; }
        [DataMember]
        public string Timestamp { get; set; }
        [DataMember]
        public string TipoResumoVenda { get; set; }
        
        [DataMember]
        public int ResumoVenda { get; set; }
        [DataMember]
        public decimal ValorApresentado { get; set; }
        [DataMember]
        public short QuantidadeComprovantesVenda { get; set; }
        [DataMember]
        public decimal ValorApurado { get; set; }
        [DataMember]
        public DateTime DataApresentacaoRetornado { get; set; }
        [DataMember]
        public decimal ValorDesconto { get; set; }
        [DataMember]
        public DateTime DataProcessamento { get; set; }
        [DataMember]
        public decimal ValorGorjetaTaxaEmbarque { get; set; }
        [DataMember]
        public string TipoResumo { get; set; }
        [DataMember]
        public decimal ValorCotacao { get; set; }
        [DataMember]
        public string TipoMoeda { get; set; }
        [DataMember]
        public string IndicadorTaxaEmbarque { get; set; }
    }
}