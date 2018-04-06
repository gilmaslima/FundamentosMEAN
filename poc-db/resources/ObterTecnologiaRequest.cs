using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    [DataContract]
    public class ObterTecnologiaRequest
    {
        [DataMember(Name="codigoEntidade", EmitDefaultValue=false)]
        public Int32 CodigoEntidade { get; set; }

        [DataMember(Name = "dataPesquisa", EmitDefaultValue = false)]
        public Int32 DataPesquisa { get; set; }
    }
}