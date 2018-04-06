using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarExtratoDirfEmissorRetorno
    {
        [DataMember]
        public decimal Cnpj { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public decimal ValorIrEmissor1 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor2 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor3 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor4 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor5 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor6 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor7 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor8 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor9 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor10 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor11 { get; set; }
        [DataMember]
        public decimal ValorIrEmissor12 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor1 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor2 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor3 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor4 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor5 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor6 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor7 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor8 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor9 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor10 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor11 { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor12 { get; set; }
    }
}