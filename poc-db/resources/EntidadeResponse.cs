using System;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Classe de modelo de response para Entidade
    /// </summary>
    [DataContract]
    public class EntidadeResponse : BaseResponse
    {
        /// <summary>
        /// Modelo de Entidade
        /// </summary>
        [DataMember]
        public Entidade Entidade { get; set; }
    }
}