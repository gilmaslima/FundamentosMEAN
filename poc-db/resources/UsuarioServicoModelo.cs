using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    [DataContract]
    public class UsuarioServicoModelo
    {
        [DataMember]
        public String Codigo { get; set; }
        [DataMember]
        public Int32 CodigoGrupoEntidade { get; set; }
        [DataMember]
        public Int32 CodigoEntidade { get; set; }
        [DataMember]
        public Int32 CodigoServico { get; set; }
        [DataMember]
        public DateTime DataInclusao { get; set; }
        [DataMember]
        public String NomeResponsavel { get; set; }
        [DataMember]
        public DateTime DataUltimaAlteracao { get; set; }
        [DataMember]
        public String NomeResponsavelAlteracao { get; set; }

    }
}