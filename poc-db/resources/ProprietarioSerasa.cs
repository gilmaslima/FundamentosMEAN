using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.Boston.Servicos
{
    [DataContract]
    public class ProprietarioSerasa
    {
        [DataMember]
        public String CPFCNPJ { get; set; }
        [DataMember]
        public String Nome { get; set; }
        [DataMember]
        public String Participacao { get; set; }
        [DataMember]
        public String TipoPessoa { get; set; }
    }
}
