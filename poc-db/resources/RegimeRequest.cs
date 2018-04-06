using System;
using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Request
{
    /// <summary>
    /// Classe modelo para requisição dos dados de regime
    /// </summary>
    [DataContract]
    public class RegimeRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Canal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Celula { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Usuario { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int CodigoServico { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Estabelecimento { get; set; }
    }
}
