using System;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Representa um motivo do material de venda
    /// </summary>
    [DataContract]
    public class Motivo
    {
        /// <summary>
        /// Código do Motivo
        /// </summary>
        [DataMember]
        public Int32 CodigoMotivo { get; set; }

        /// <summary>
        /// Descrição do Motivo
        /// </summary>
        [DataMember]
        public String DescricaoMotivo { get; set; }

        /// <summary>
        /// Identificar de texto do Motivo
        /// </summary>
        [DataMember]
        public String IdentificadorMotivo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DescricaoMotivo;
        }
    }
}