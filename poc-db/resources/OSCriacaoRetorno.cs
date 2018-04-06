
/*
© Copyright 2017 Redecard S.A.
Autor : MNE
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Classe para retorno dos dados em dada criação de OS
    /// </summary>
    [DataContract]
    public class OSCriacaoRetorno
    {
        /// <summary>
        /// Numero da OS
        /// </summary>
        [DataMember(Name = "numeroOs", EmitDefaultValue = false)]
        public String NumeroOs { get; set; }

        /// <summary>
        /// Data Programada para atendimento da OS
        /// </summary>
        [DataMember(Name = "dataProgramada", EmitDefaultValue = false)]
        public DateTime? DataProgramada  { get; set; }
    }
}
