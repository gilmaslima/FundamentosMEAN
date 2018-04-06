using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.DadosCadastrais.Servicos.Modelos
{
    [DataContract]
    public class TaxaAntiga
    {
        [DataMember(Name = "modalidade", EmitDefaultValue = false)]
        public String Modalidade { get; set; }
        [DataMember(Name = "taxas", EmitDefaultValue = false)]
        public Taxas[] Taxas { get; set; }
    }
}