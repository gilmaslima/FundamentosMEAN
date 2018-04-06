using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class Cnae
    {

        [DataMember]
        public String CodigoCNAESerasa { get; set; }

        [DataMember]
        public String CodigoSubClasseCNAE { get; set; }

        [DataMember]
        public String NomeSubClasseCNAE { get; set; }

        [DataMember]
        public String CodigoClasseCNAE { get; set; }

        [DataMember]
        public Int32? CodigoGrupoRamo { get; set; }

        [DataMember]
        public Int32? CodigoRamoAtivididade { get; set; }
    
    }
}
