using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Rede.PN.Cancelamento.Modelo
{
    /// <summary>
    /// Classe Cancelamento PN
    /// </summary>
    [DataContract]
    public class CancelamentoPn
    {
        /// <summary>
        /// Data do cancelamento
        /// </summary>
        [DataMember]
        public DateTime DataCancelamento { get; set; }

        /// <summary>
        /// Número do aviso
        /// </summary>
        [DataMember]
        public Int64 NumeroAviso { get; set; }

        /// <summary>
        /// Número sequêncial único
        /// </summary>
        [DataMember]
        public String Nsu { get; set; }

        /// <summary>
        /// Valor da venda
        /// </summary>
        [DataMember]
        public Decimal ValorVenda { get; set; }

        /// <summary>
        /// Data da venda
        /// </summary>
        [DataMember]
        public DateTime DataVenda { get; set; }

        /// <summary>
        /// Tipo da transação
        /// </summary>
        [DataMember]
        public Char TipoTransacao { get; set; }

        /// <summary>
        /// Tipo do cancelamento
        /// </summary>
        [DataMember]
        public Char TipoCancelamento { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public String Status { get; set; }

        /// <summary>
        /// Valor do Cancelamento
        /// </summary>
        [DataMember]
        public Decimal ValorCancelamento { get; set; }

        /// <summary>
        /// Número da autorização
        /// </summary>
        [DataMember]
        public String NumeroAutorizacao { get; set; }

        /// <summary>
        /// Número do estabelecimento do cancelamento
        /// </summary>
        [DataMember]
        public Int32 NumeroEstabelecimentoCancelamento { get; set; }

        /// <summary>
        /// Número do estabelecimento da venda
        /// </summary>
        [DataMember]
        public Int32 NumeroEstabelecimentoVenda { get; set; }

        /// <summary>
        /// Usuário do cancelamento
        /// </summary>
        [DataMember]
        public String UsuarioCancelamento { get; set; }

        /// <summary>
        /// Ip
        /// </summary>
        [DataMember]
        public String Ip { get; set; }

        /// <summary>
        /// Meio de comunicação
        /// </summary>
        [DataMember]
        public Int32 MeioComunicacao { get; set; }
    }
}
