#region Histórico do Arquivo
/*
(c) Copyright [2016] Redecard S.A.
Autor       : [Raphael Ivo]
Empresa     : [Iteris]
Histórico   :
- [22/06/2016] – [Raphael Ivo] – [Criação]
*/
#endregion

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.Modelo
{
    /// <summary>
    /// Classe que representa campos da tabela TBPN026
    /// </summary>
    [Serializable]
    [DataContract]
    public class RetornoCancelamento
    {
        /// <summary>
        /// Propriedade que representa cod_etd_usr_rspn da tabela TBPN026
        /// </summary>
        [DataMember]
        public Int32? CodigoPv { get; set; }
        /// <summary>
        /// Propriedade que representa cod_etd_mtz_usr_rspn da tabela TBPN026
        /// </summary>
        [DataMember]
        public Int32? CodigoPvAcesso { get; set; }

        /// <summary>
        /// Propriedade que representa emal_usr_rspn da tabela TBPN026
        /// </summary>
        [DataMember]
        public String Email { get; set; }

        /// <summary>
        /// Propriedade que representa ip_usr_rspn da tabela TBPN026
        /// </summary>
        [DataMember]
        public String Ip { get; set; }

        /// <summary>
        /// Propriedade que representa dth_inclusao da tabela TBPN026
        /// </summary>
        [DataMember]
        public DateTime DataInclusao { get; set; }
    }
}
