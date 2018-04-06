using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.Boston.Servicos
{
    [DataContract]
    public class DadosSerasa
    {
        [DataMember]
        public Int32 CodigoRetorno { get; set; }
        [DataMember]
        public String RazaoSocial { get; set; }
        [DataMember]
        public DateTime DataFundacao { get; set; }
        [DataMember]
        public Int32 CodigoRamoAtividade { get; set; }
        [DataMember]
        public Int32 CodigoGrupoAtuacao { get; set; }
        [DataMember]
        public String CNAE { get; set; }
        [DataMember]
        public List<ProprietarioSerasa> Proprietarios { get; set; }
        
    }
}
