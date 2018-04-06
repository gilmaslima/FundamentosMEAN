using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Classe de modelo de response para Entidade
    /// </summary>
    [DataContract]
    public class EntidadeConsultarResponse : BaseResponse
    {
        /// <summary>
        /// Modelo de Entidade
        /// </summary>
        [DataMember]
        public Entidade[] Itens { get; set; }

        /// <summary>
        /// Codigo de Retorno do GE
        /// </summary>
        [DataMember]
        public Int32 CodigoRetornoGE { get; set; }

        /// <summary>
        /// Codigo de Retorno do IS
        /// </summary>
        [DataMember]
        public Int32 CodigoRetornoIS { get; set; }
    }
}