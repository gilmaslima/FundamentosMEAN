using System;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Resposta do serviço de atualização do status do usuário que recupera a senha
    /// </summary>
    [DataContract]
    public class AtualizarRecuperacaoSenhaResponse : BaseResponse
    {
        /// <summary>
        /// Hash de Email da atualização
        /// </summary>
        [DataMember]
        public Guid HashEmail { get; set; }
    }
}