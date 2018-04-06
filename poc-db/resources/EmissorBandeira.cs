using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Servicos
{
    /// <summary>
    /// Classe dos Códigos Emissor Bandeira (ICAs e BIDs)
    /// </summary>
    [DataContract]
    public class EmissorBandeira
    {
        /// <summary>
        /// Código ICA/BID
        /// </summary>
        [DataMember]
        public Int64 Codigo { get; set; }
    }
}