/*
© Copyright 2017 Redecard S.A.
Autor : MNE
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Classe Modelo OSEstendida
    /// </summary>
    [DataContract]
    public class OSEstendidaRest : OSRest
    {
        /// <summary>
        /// Endereço atendimento
        /// </summary>
        [DataMember]
        public EnderecoRest EnderecoAtendimento { get; set; }

        /// <summary>
        /// Horário Atendimento
        /// </summary>
        [DataMember]
        public List<HorarioRest> HorarioAtendimento { get; set; }

        /// <summary>
        /// Contato
        /// </summary>
        [DataMember]
        public Contato Contato { get; set; }
    }
}
