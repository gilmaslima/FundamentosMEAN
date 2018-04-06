#region Histórico do Arquivo
/*
(c) Copyright [2016] Redecard S.A.
Autor       : [Denis Missias]
Empresa     : [Iteris]
Histórico   :
- [20/06/2016] – [Denis Missias] – [Criação]
*/
#endregion

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.Modelo
{
    /// <summary>
    /// Classe que representa campos da tabela TBPN003 e TBPN002
    /// </summary>
    [Serializable]
    [DataContract]
    public class UsuarioPV
    {
        /// <summary>
        /// Propriedade que representa cod_usr_id da tabela TBPN003
        /// </summary>
        [DataMember]
        public Int32 ID { get; set; }

        /// <summary>
        /// Propriedade que representa cod_usr da tabela TBPN003
        /// </summary>
        [DataMember]
        public String Codigo { get; set; }

        /// <summary>
        /// Propriedade que representa des_usr da tabela TBPN003
        /// </summary>
        [DataMember]
        public String Nome { get; set; }

        /// <summary>
        /// Propriedade que representa nom_eml_usr da tabela TBPN003
        /// </summary>
        [DataMember]
        public String Email { get; set; }

        /// <summary>
        /// Propriedade que representa cod_status da tabela TBPN003
        /// </summary>
        [DataMember]
        public Int32 Status { get; set; }

        /// <summary>
        /// Propriedade que representa qtd_snha_inv da tabela TBPN003
        /// </summary>
        [DataMember]
        public Int32 QtdTentativasLoginErro { get; set; }

        /// <summary>
        /// Propriedade que representa dth_ult_acesso da tabela TBPN003
        /// </summary>
        [DataMember]
        public DateTime UltimoLogin { get; set; }

        /// <summary>
        /// Propriedade que representa dth_ult_atlz da tabela TBPN003
        /// </summary>
        [DataMember]
        public DateTime UltimaAlteracao { get; set; }

        /// <summary>
        /// Propriedade que representa cod_etd da tabela TBPN002
        /// </summary>
        [DataMember]
        public Int32 CodigoPV { get; set; }

        /// <summary>
        /// Propriedade que representa des_etd da tabela TBPN002
        /// </summary>
        [DataMember]
        public String NomePV { get; set; }
    }
}
