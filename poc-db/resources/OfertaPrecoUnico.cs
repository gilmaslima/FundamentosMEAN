using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    /// <summary>
    /// Classe de mapeamento da Oferta de uma proposta.
    /// </summary>
    [DataContract]
    [Serializable]
    public class OfertaPrecoUnico
    {
        /// <summary>
        /// Tipo de Pessoa
        /// </summary>
        [DataMember]
        public Char CodigoTipoPessoa { get; set; }

        /// <summary>
        /// Número do CNPJ/CPF do estabelecimento
        /// </summary>
        [DataMember]
        public Int64 NumeroCNPJ { get; set; }

        /// <summary>
        /// Número da Sequencia da Proposta
        /// </summary>
        [DataMember]
        public Int32 NumeroSequenciaProposta { get; set; }

        /// <summary>
        /// Código da Oferta
        /// </summary>
        [DataMember]
        public Int32 CodigosOfertaPrecoUnico { get; set; }

        /// <summary>
        /// Usuário que efetuou a operação
        /// </summary>
        [DataMember]
        public String CodigoUsuario { get; set; }

        /// <summary>
        /// Data e Hora da última atualização do registro
        /// </summary>
        [DataMember]
        public DateTime DataHoraUltimaAtualizacao { get; set; }
    }
}
