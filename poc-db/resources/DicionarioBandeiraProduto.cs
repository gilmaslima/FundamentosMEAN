using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.DadosCadastrais.Servicos.Modelos
{
    [DataContract]
    public class DicionarioBandeiraProduto
    {
        [DataMember(Name = "bandeira", EmitDefaultValue = false)]
        public String Bandeira { get; set; }
        [DataMember(Name = "descricao", EmitDefaultValue = false)]
        public String Descricao { get; set; }
        [DataMember(Name = "ordem", EmitDefaultValue = false)]
        public Int16 Ordem { get; set; }
        [DataMember(Name = "produto", EmitDefaultValue = false)]
        public String Produto { get; set; }
        [DataMember(Name = "segmento", EmitDefaultValue = false)]
        public String Segmento { get; set; }
    }
}