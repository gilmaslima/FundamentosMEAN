using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Define os valores de response de dicionário de perguntas.
    /// </summary>
    [DataContract]
    public class CriarUsuarioPvsResponse : BaseResponse
    {
        /// <summary>
        /// Define a propriedade Hash
        /// </summary>
        [DataMember]
        public Guid Hash { get; set; }

        /// <summary>
        /// Define a propriedade EntidadesPossuemUsuarioMaster
        /// </summary>
        [DataMember]
        public Modelo.EntidadeServicoModel[] EntidadesPossuemUsuarioMaster { get; set; }

        /// <summary>
        /// Define a propriedade EntidadesNaoPossuemUsuarioMaster
        /// </summary>
        [DataMember]
        public Modelo.EntidadeServicoModel[] EntidadesNaoPossuemUsuarioMaster { get; set; }
    }
}