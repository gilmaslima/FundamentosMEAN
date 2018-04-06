/*
© Copyright 2017 Redecard S.A.
Autor : MNE
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.Terminal
{
    /// <summary>
    /// Classe Modelo TerminalStatus
    /// </summary>
    [DataContract]
    public class TerminalStatusRest : TerminalBase
    {
        /// <summary>
        /// Status
        /// </summary>
        [IgnoreDataMember]
        public TipoTerminalStatus StatusCodigo { get; set; }

        /// <summary>
        /// Descrição do Status
        /// </summary>
        [DataMember(Name = "Status", EmitDefaultValue = false)]
        public String StatusDescricao
        {
            get
            {
                return StatusCodigo != null ? StatusCodigo.ToString() : "";
            }
            set { this.StatusDescricao = value; }
        }
    }
}
