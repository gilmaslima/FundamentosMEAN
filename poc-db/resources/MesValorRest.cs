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
    /// Classe Modelo MesValor
    /// </summary>
    [DataContract]
    public class MesValorRest
    {
        #region Mes
        /// <summary>
        /// Mês
        /// </summary>
        [IgnoreDataMember]
        public TipoMeses MesCodigo { get; set; }

        /// <summary>
        /// Descrição do Mês
        /// </summary>
        [DataMember(Name = "Mes", EmitDefaultValue = false)]
        public String MesDescricao
        {
            get
            {
                return MesCodigo != null ? MesCodigo.ToString() : "";
            }
            set { this.MesDescricao = value; }
        }
        #endregion

        /// <summary>
        /// Valor
        /// </summary>
        [DataMember]
        public Double Valor { get; set; }
    }
}
