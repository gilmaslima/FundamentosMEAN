using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// ListaEntidadesModelResponse
    /// </summary>
    public class ListaEntidadesModelResponse : BaseResponse
    {
        /// <summary>
        /// Modelo de Entidade
        /// </summary>
        [DataMember]
        public Modelo.EntidadeServicoModel[] Entidades { get; set; }
    }
}