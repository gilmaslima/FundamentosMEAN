using System;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Retorno do URLBack
    /// </summary>
    [DataContract]
    public class URLBackResponse : BaseResponse
    {
        /// <summary>
        /// Dados de URLBack
        /// </summary>
        [DataMember]
        public URLBack URLBack { get; set; }
    }
}