using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarDisponibilidadeVencimentosODCEnvio
    {
        [DataMember]
        public int NumeroEstabelecimento { get; set; }
        [DataMember]
        public int NumeroResumoVenda { get; set; }
        [DataMember]
        public DateTime DataApresentacao { get; set; }
        [DataMember]
        public string ChaveContinua { get; set; }
    }
}
