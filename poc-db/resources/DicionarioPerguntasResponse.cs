using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Define os valores de response de dicionário de perguntas.
    /// </summary>
    [DataContract]
    public class DicionarioPerguntasResponse : BaseResponse
    {
        /// <summary>
        /// Define a propriedade Validado
        /// </summary>
        [DataMember]
        public Boolean Validado { get; set; }

        /// <summary>
        /// Define a propriedade Perguntas
        /// </summary>
        [DataMember]
        public Dictionary<Int32, List<Modelo.Pergunta>> Perguntas { get; set; }
    }
}