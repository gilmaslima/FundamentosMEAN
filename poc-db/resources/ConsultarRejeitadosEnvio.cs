using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarRejeitadosEnvio
    {
        [DataMember]
        public string Timestamp { get; set; }
        [DataMember]
        public short TipoResumoVenda { get; set; }
        [DataMember]
        public int NumeroEstabelecimento { get; set; }
        [DataMember]
        public int NumeroResumoVenda { get; set; }
        [DataMember]
        public DateTime DataApresentacao { get; set; }
    }
}
