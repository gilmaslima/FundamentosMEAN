using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarOrdensCreditoEnviadosAoBancoDetalheRetorno : BasicContract
    {
        [DataMember]
        public DateTime DataEmissao { get; set; }
        [DataMember]
        public DateTime DataVencimento { get; set; }
        [DataMember]
        public int NumeroEstabelecimento { get; set; }
        [DataMember]
        public int NumeroResumoVenda { get; set; }
        [DataMember]
        public string Tipobandeira { get; set; }
        [DataMember]
        public string StatusOcorrenica { get; set; }
        [DataMember]
        public string DescricaoResumoAjuste { get; set; }
        [DataMember]
        public decimal ValorCredito { get; set; }
        [DataMember]
        public string IndicadorSinalValor { get; set; }
        [DataMember]
        public short BancoCredito { get; set; }
        [DataMember]
        public int AgenciaCredito { get; set; }
        [DataMember]
        public string ContaCorrente { get; set; }
        /// <summary>
        /// Indicador de Recarga de Celular
        /// </summary>
        [DataMember]
        public Boolean IndicadorRecarga { get; set; }
        /// <summary>
        /// Código do Ajuste
        /// </summary>
        [DataMember]
        public Int16 CodigoAjuste { get; set; }
    }
}