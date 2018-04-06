using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Request
{
    /// <summary>
    /// Classe modelo para requisição do envio de email
    /// </summary>
    [DataContract]
    public class ComprovanteRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Assunto { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ConteudoHtml { get; set; }
    }
}
