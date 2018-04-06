using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Servicos
{
    [DataContract]
    public class DetalheFatura
    {
        [DataMember]
        public string Tipo { get; set; }
        [DataMember]
        public Int32 NumeroPV { get; set; }
        [DataMember]
        public decimal Cnpj { get; set; }
        [DataMember]
        public Int32 CodigoAgencia { get; set; }
        [DataMember]
        public string ContaCorrente { get; set; }
        [DataMember]
        public string PeriodoInicial { get; set; }
        [DataMember]
        public string PeriodoFinal { get; set; }
        [DataMember]
        public string QuantidadeDias { get; set; }
        [DataMember]
        public decimal ValorLiquido { get; set; }
        [DataMember]
        public string SiglaProduto { get; set; }
    }
}