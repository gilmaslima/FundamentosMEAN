using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Servicos
{
    /// <summary>
    /// Modelo de estabelecimentos logados no portal
    /// </summary>
    [DataContract]
    public class EstabelecimentosLogados
    {
        /// <summary>
        /// Código do estabelecimento
        /// </summary>
        [DataMember]
        public Int32 CodigoEstabelecimento { get; set; }

        /// <summary>
        /// Data do primeiro acesso do estabelecimento no portal
        /// </summary>
        [DataMember]
        public DateTime PrimeiroAcesso { get; set; }

        /// <summary>
        /// Data do último acesso do estabelecimento no portal
        /// </summary>
        [DataMember]
        public DateTime UltimoAcesso { get; set; }

        /// <summary>
        /// Qauntidade de acessos do estabelecimento no portal
        /// </summary>
        [DataMember]
        public Int32 QuantidadeAcessos { get; set; }
    }
}