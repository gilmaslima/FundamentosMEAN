using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Define os valores de response de Alteração de Email.
    /// </summary>
    [DataContract]
    public class AlterarEmailResponse : BaseResponse
    {
        /// <summary>
        /// Define a propriedade Item
        /// </summary>
        [DataMember]
        public Int32 Item { get; set; }

        /// <summary>
        /// Define a propriedade Hash
        /// </summary>
        [DataMember]
        public Guid Hash { get; set; }
    }
}