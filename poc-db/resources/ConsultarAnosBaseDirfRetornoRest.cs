﻿using Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarAnosBaseDirfRetornoRest : ResponseBase
    {
        [DataMember(Name = "anosBase", EmitDefaultValue = false)]
        public List<short> AnosBase { get; set; }   
    }
}   