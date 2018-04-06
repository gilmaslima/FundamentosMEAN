/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Software e Consultoria.
*/


using System;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Representa Corpo de requisição para inclusão de solicitação material.
    /// </summary>
    [DataContract]
    public class IncluirKitRequest
    {
        /// <summary>
        /// Kits (Material / Sinalização)
        /// </summary>
        [DataMember]
        public Kit[] Kits { get; set; }

        /// <summary>
        /// Kits (Material / Sinalização)
        /// </summary>
        [DataMember]
        public Int32 CodigoPV { get; set; }

        /// <summary>
        /// Kits (Material / Sinalização)
        /// </summary>
        [DataMember]
        public String DescricaoPV { get; set; }

        /// <summary>
        /// Kits (Material / Sinalização)
        /// </summary>
        [DataMember]
        public String Usuario { get; set; }

        /// <summary>
        /// Kits (Material / Sinalização)
        /// </summary>
        [DataMember]
        public String Solicitante { get; set; }

        /// <summary>
        /// Kits (Material / Sinalização)
        /// </summary>
        [DataMember]
        public Boolean EnderecoTemporario { get; set; }

        /// <summary>
        /// Kits (Material / Sinalização)
        /// </summary>
        [DataMember]
        public Endereco Endereco { get; set; }
    }
}