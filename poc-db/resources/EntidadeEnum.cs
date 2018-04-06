using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// enum que informa se o PV está no programa de fidelização
    /// </summary>
    [DataContract]
    public enum EntidadeEnum
    {
        /// <summary>
        /// Informa que o PV não tem programa de fidelização
        /// </summary>
        [EnumMember]
        NaoAtende,
        /// <summary>
        /// Informa se o PV já está fidelizado
        /// </summary>
        [EnumMember]
        Fidelizado,
        /// <summary>
        /// Informa se o PV é elegivel à fidelização
        /// </summary>
        [EnumMember]
        Elegivel
    }
}
