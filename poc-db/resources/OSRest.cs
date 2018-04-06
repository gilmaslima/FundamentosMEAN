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
    /// Classe Modelo OS
    /// </summary>
    [DataContract]
    public class OSRest
    {
        /// <summary>
        /// Número
        /// </summary>
        [DataMember]
        public String Numero { get; set; }

        /// <summary>
        /// Fct
        /// </summary>
        [DataMember]
        public String Fct { get; set; }

        #region Classificação
        /// <summary>
        /// Classificação
        /// </summary>
        [IgnoreDataMember]
        public TipoClassificacao ClassificacaoCodigo { get; set; }

        /// <summary>
        /// Descrição da Classificação
        /// </summary>
        [DataMember(Name = "Classificacao", EmitDefaultValue = false)]
        public String ClassificacaoDescricao
        {
            get
            {
                return ClassificacaoCodigo != null ? ClassificacaoCodigo.ToString() : "";
            }
            set { this.ClassificacaoDescricao = value; }
        }
        #endregion

        #region Sistema
        /// <summary>
        /// Sistema
        /// </summary>
        [IgnoreDataMember]       
        public TipoOrigem SistemaCodigo { get; set; }

        /// <summary>
        /// Descricao do Sistema
        /// </summary>
        [DataMember(Name = "Sistema", EmitDefaultValue = false)]
        public String SistemaDescricao
        {
            get
            {
                return SistemaCodigo != null ? SistemaCodigo.ToString() : "";
            }
            set { this.SistemaDescricao = value; }
        }
        #endregion

        #region Situação
        /// <summary>
        /// Situação
        /// </summary>
        [IgnoreDataMember]
        public TipoOSSituacao SituacaoCodigo { get; set; }

        /// <summary>
        /// Descricao da Situação
        /// </summary>
        [DataMember(Name = "Situacao", EmitDefaultValue = false)]
        public String SituacaoDescricao
        {
            get
            {
                return SituacaoCodigo != null ? SituacaoCodigo.ToString() : "";
            }
            set { this.SituacaoDescricao = value; }
        }
        #endregion

        /// <summary>
        /// Data situação
        /// </summary>
        [DataMember]
        public DateTime DataSituacao { get; set; }

        #region Prioridade
        /// <summary>
        /// Prioridade
        /// </summary>
        [IgnoreDataMember]
        public TipoPrioridade PrioridadeCodigo { get; set; }

        /// <summary>
        /// Descrição da Prioridade
        /// </summary>
        [DataMember(Name = "Prioridade", EmitDefaultValue = false)]
        public String PrioridadeDescricao
        {
            get
            {
                return PrioridadeCodigo != null ? PrioridadeCodigo.ToString() : "";
            }
            set { this.PrioridadeDescricao = value; }
        }
        #endregion

        /// <summary>
        /// Data programada
        /// </summary>
        [DataMember]
        public DateTime? DataProgramada { get; set; }

        /// <summary>
        /// Data atendimento
        /// </summary>
        [DataMember]
        public DateTime? DataAtendimento { get; set; }

        /// <summary>
        /// Motivo cancelamento
        /// </summary>
        [DataMember]
        public String MotivoCancelamento { get; set; }
    }
}
