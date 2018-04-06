/*
© Copyright 2017 Redecard S.A.
Autor : MNE
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.Terminal
{
    [DataContract]
    public class TerminalRest : TerminalStatusRest
    {
        /// <summary>
        /// Família equipamento
        /// </summary>
        [DataMember]
        public String FamiliaEquipamento { get; set; }

        /// <summary>
        /// Tipo equipamento
        /// </summary>
        [DataMember]
        public String TipoEquipamento { get; set; }

        /// <summary>
        /// Tecnologia
        /// </summary>
        [DataMember]
        public String Tecnologia { get; set; }
    }
}
