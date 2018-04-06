/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
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
    public class OSEstendida : OS
    {
        /// <summary>
        /// Endereço atendimento
        /// </summary>
        [DataMember]
        public Endereco EnderecoAtendimento { get; set; }

        /// <summary>
        /// Horário Atendimento
        /// </summary>
        [DataMember]
        public List<Horario> HorarioAtendimento { get; set; }

        /// <summary>
        /// Contato
        /// </summary>
        [DataMember]
        public Contato Contato { get; set; }
    }
}
