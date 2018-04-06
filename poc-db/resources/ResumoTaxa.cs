using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.DadosCadastrais.Servicos.Modelos
{
    [DataContract]
    public class ResumoTaxa
    {
        [DataMember(Name = "agencia", EmitDefaultValue = false)]
        public String Agencia { get; set; }
        [DataMember(Name = "banco", EmitDefaultValue = false)]
        public String Banco { get; set; }
        [DataMember(Name = "bandeira", EmitDefaultValue = false)]
        public String Bandeira { get; set; }
        [DataMember(Name = "conta", EmitDefaultValue = false)]
        public String Conta { get; set; }
        [DataMember(Name = "ordem", EmitDefaultValue = false)]
        public Int16 Ordem { get; set; }
        [DataMember(Name = "produto", EmitDefaultValue = false)]
        public String Produto { get; set; }
        [DataMember(Name = "produtoDescricao", EmitDefaultValue = false)]
        public String ProdutoDescricao { get; set; }
        [DataMember(Name = "produtoSegmento", EmitDefaultValue = false)]
        public String ProdutoSegmento { get; set; }
        [DataMember(Name = "taxa_antiga", EmitDefaultValue = false)]
        public TaxaAntiga[] TaxaAntiga { get; set; }
    }
}