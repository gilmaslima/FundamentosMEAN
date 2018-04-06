using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Modelo.Mensagens
{
    /// <summary>
    /// Classe modelo de resposta sobre a confirmação positiva do primeiro acesso
    /// </summary>
    [DataContract]
    public class ConfirmacaoPositivaPrimeiroAcessoResponse
    {
        /// <summary>
        /// Indicador de retorno OK ou NOK
        /// </summary>
        [DataMember]
        public bool Retorno { get; set; }

        /// <summary>
        /// Código de negócio para retorno
        /// </summary>
        [DataMember]
        public int CodigoRetorno { get; set; }

        /// <summary>
        /// Lista de Perguntas incorretas
        /// </summary>
        [DataMember]
        public Dictionary<Int32, List<Modelo.Pergunta>> PerguntasIncorretas { get; set; }

        /// <summary>
        /// Quantidades de Tentativas restantes
        /// </summary>
        [DataMember]
        public int TentativasRestantes { get; set; }
    }
}
