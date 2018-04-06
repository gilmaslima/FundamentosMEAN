using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ResumoRetorno
    {
        [DataMember(Name = "ano", EmitDefaultValue = false)]
        public Int16 Ano{ get; set; }

        [DataMember(Name = "valor", EmitDefaultValue = false)]
        public decimal Valor { get; set; }
    }
}


