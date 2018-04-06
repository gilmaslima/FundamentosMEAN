/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
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
    public class OS
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

        /// <summary>
        /// Classificação
        /// </summary>
        [DataMember]
        public TipoClassificacao Classificacao { get; set; }

        /// <summary>
        /// Sistema
        /// </summary>
        [DataMember]
        public TipoOrigem Sistema { get; set; }

        /// <summary>
        /// Situação
        /// </summary>
        [DataMember]
        public TipoOSSituacao Situacao { get; set; }

        /// <summary>
        /// Data situação
        /// </summary>
        [DataMember]
        public DateTime DataSituacao { get; set; }

        /// <summary>
        /// Prioridade
        /// </summary>
        [DataMember]
        public TipoPrioridade Prioridade { get; set; }

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
