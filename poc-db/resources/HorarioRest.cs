/*
© Copyright 2017 Redecard S.A.
Autor : MNE
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Classe Modelo Horario
    /// </summary>
    [DataContract]
    public class HorarioRest
    {
        #region TipoDia
        /// <summary>
        /// Dia
        /// </summary>
        [IgnoreDataMember]
        public TipoDia DiaCodigo { get; set; }

        /// <summary>
        /// Descrição do Dia
        /// </summary>
        [DataMember(Name = "Dia", EmitDefaultValue = false)]
        public String DiaDescricao
        {
            get
            {
                return DiaCodigo != null ? DiaCodigo.ToString() : "";
            }
            set { this.DiaDescricao = value; }
        }

        #endregion

        /// <summary>
        /// Início
        /// </summary>
        [DataMember]
        public DateTime Inicio { get; set; }

        /// <summary>
        /// Término
        /// </summary>
        [DataMember]
        public DateTime Termino { get; set; }
    }
}
