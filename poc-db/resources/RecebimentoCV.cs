using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Classe modelo de Recebimento de Comprovante de Venda utilizada nas telas do módulo Request.    
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS dos Books:<br/>
    /// - Book BXD202CB / Programa XD202 / TranID XDS2<br/>
    /// - Book BXA760 / Programa XA760 / TranID IS66
    /// </remarks>  
    [DataContract]
    public class RecebimentoCV : ModeloBase
    {
        /// <summary>Código do Recebimento de CV</summary>
        [DataMember]
        public Int16 CodigoRecebimento { get; set; }

        /// <summary>Descrição do Recebimento de CV</summary>
        [DataMember]
        public String DescricaoRecebimento { get; set; }

        /// <summary>Data do Recebimento de CV</summary>
        [DataMember]
        public DateTime DataRecebimento { get; set; }
    }
}