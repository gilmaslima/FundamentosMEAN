using System.Runtime.Serialization;

namespace Rede.PN.HomePage.SharePoint {
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class Valor {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal ValorLiquido { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal ValorBruto { get; set; }
    }
}