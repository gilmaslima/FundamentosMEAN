using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.HomePage.SharePoint {
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class Dashboard {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Vendas Vendas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public ValoresPagos ValoresPagos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public LancamentosFuturos LancamentosFuturos { get; set; }
    }
}