using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarDisponibilidadeVencimentosODCRetorno
    {
        [DataMember]
        public DateTime DataAntecipacao { get; set; }
        [DataMember]
        public DateTime DataVencimento { get; set; }
        [DataMember]
        public int NumeroEstabelecimento { get; set; }
        [DataMember]
        public int NumeroOdc { get; set; }
        [DataMember]
        public string NomeEstabelecimento { get; set; }
        [DataMember]
        public short PrazoRecebimento { get; set; }
        [DataMember]
        public short Status { get; set; }
        [DataMember]
        public decimal ValorOrdemCredito { get; set; }
    }
}
