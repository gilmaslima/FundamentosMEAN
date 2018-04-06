using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarRejeitadosRetorno
    {
        [DataMember]
        public int Autorizacao { get; set; }
        [DataMember]
        public string Cartao { get; set; }
        [DataMember]
        public DateTime DataComprovanteVenda { get; set; }
        [DataMember]
        public string Descricao { get; set; }
        [DataMember]
        public int NumeroEstabelecimento { get; set; }
        [DataMember]
        public short Sequencia { get; set; }
        [DataMember]
        public decimal Valor { get; set; }
        [DataMember]
        public string IndicadorTokenizacao { get; set; }
    }
}
