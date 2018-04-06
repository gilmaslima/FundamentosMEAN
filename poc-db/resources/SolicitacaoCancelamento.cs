using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Rede.PN.Cancelamento.Modelo
{
    /// <summary>
    /// Classe de solicitação de cancelamento
    /// </summary>
    [Serializable]
    [DataContract]
    public class SolicitacaoCancelamento
    {
        /// <summary>
        /// Número do estabelcimento que realizou o cancelamento
        /// </summary>
        [DataMember]
        public Int32 NumeroEstabelecimentoCancelamento { get; set; }

        /// <summary>
        /// Número do estabelcimento da venda
        /// </summary>
        [DataMember]
        public Int32 NumeroEstabelecimentoVenda { get; set; }

        /// <summary>
        /// Tipo da venda
        /// </summary>
        [DataMember]
        public TipoVenda TipoVenda { get; set; }

        /// <summary>
        /// Tipo de venda detalhado
        /// </summary>
        [DataMember]
        public String TipoVendaDetalhado { get; set; }

        /// <summary>
        /// Data da Venda
        /// </summary>
        [DataMember]
        public DateTime DataVenda { get; set; }

        /// <summary>
        /// Data da Venda no formato texto
        /// </summary>
        [DataMember]
        public String DataVendaTexto { get; set; }

        /// <summary>
        /// Número do cartão ou NSU/CV
        /// </summary>
        [DataMember]
        public String NSU { get; set; }

        /// <summary>
        /// Valor Bruto da Venda
        /// </summary>
        [DataMember]
        public Decimal ValorBruto { get; set; }

        /// <summary>
        /// Saldo Disponível
        /// </summary>
        [DataMember]
        public Decimal SaldoDisponivel { get; set; }

        /// <summary>
        /// Tipo de Cancelamento
        /// </summary>
        [DataMember]
        public TipoCancelamento TipoCancelamento { get; set; }

        /// <summary>
        /// Tipo de Cancelamento Detalhado
        /// </summary>
        [DataMember]
        public String TipoCancelamentoDetalhado { get; set; }

        /// <summary>
        /// Valor de Cancelamento
        /// </summary>
        [DataMember]
        public Decimal ValorCancelamento { get; set; }

        /// <summary>
        /// Número do Aviso de Cancelamento
        /// </summary>
        [DataMember]
        public String NumeroAvisoCancelamento { get; set; }

        /// <summary>
        /// Origem
        /// </summary>
        [DataMember]
        public String Origem { get; set; }

        /// <summary>
        /// Número Autorização Emissor
        /// </summary>
        [DataMember]
        public String NumeroAutorizacao { get; set; }

        /// <summary>
        /// Número do Mês (somente crédito)
        /// </summary>
        [DataMember]
        public Int16 NumeroMes { get; set; }

        /// <summary>
        /// Timestamp Transação (somente crédito)
        /// </summary>
        [DataMember]
        public String TimestampTransacao { get; set; }

        /// <summary>
        /// Tipo de Transação
        /// </summary>
        [DataMember]
        public TipoTransacao TipoTransacao { get; set; }

        /// <summary>
        /// Código do Ramo de Atividade do Estabelecimento
        /// </summary>
        [DataMember]
        public Int32 CodigoRamoAtividade { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public String Status { get; set; }

        /// <summary>
        /// Data do Cancelamento
        /// </summary>
        [DataMember]
        public DateTime DataCancelamento { get; set; }

        /// <summary>
        /// Linha
        /// </summary>
        [DataMember]
        public Int32 Linha { get; set; }
    }
}
