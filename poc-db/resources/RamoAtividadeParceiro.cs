using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class RamoAtividadeParceiro
    {
        [DataMember]
        public Int32 CodigoCca { get; set; }

        [DataMember]
        public Int32 CodigoFeature { get; set; }

        [DataMember]
        public Int32? CodigoGrupoRamo { get; set; }

        [DataMember]
        public Int32? CodigoRamoAtividade { get; set; }

        [DataMember]
        public String NomeRamoAtividadeProdutoParceiro { get; set; }

        [DataMember]
        public String UsuarioAtualizacao { get; set; }

        [DataMember]
        public DateTime DataUltimaAtualizacao { get; set; }
    }
}
