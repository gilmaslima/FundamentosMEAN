using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrasEntidades.Modelo
{
    [DataContract]
    public class PropostaPorCNPJCPF : ModeloBase
    {
        [DataMember]
        public Int32 CodigoRetorno { get; set; }

        [DataMember]
        public String DescricaoRetorno { get; set; }

        [DataMember]
        public String Data { get; set; }

        [DataMember]
        public String Hora { get; set; }

        [DataMember]
        public String NomeRazaoSocial { get; set; }

        [DataMember]
        public Int32 CodigoCelula { get; set; }

        [DataMember]
        public Int32 CodigoAgenciaCanalFiliacao { get; set; }
    }
}
