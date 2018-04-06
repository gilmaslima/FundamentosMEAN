using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>Classe modelo de Composição de Resumo de Vendas (RV).</summary>
    /// <remarks>
    /// Encapsula os registros retornadors pelo Serviço HIS do Book:<br/>
    /// - Book BXA740 / Programa XA740 / TranID IS69
    /// </remarks>    
    [DataContract]
    public class ComposicaoRV : ModeloBase
    {
        /// <summary>Quantidade de ocorrências/registros de parcelas retornados</summary>
        [DataMember]
        public Int16 QuantidadeOcorrencias { get; set; }

        /// <summary>Valor da Venda</summary>
        [DataMember]
        public Decimal ValorVenda { get; set; }

        /// <summary>Valor do Cancelamento</summary>
        [DataMember]
        public Decimal ValorCancelamento { get; set; }

        /// <summary>Quantidade de parcelas</summary>
        [DataMember]
        public Int16 QuantidadeParcelas { get; set; }

        /// <summary>Quantidade de Parcelas quitadas</summary>
        [DataMember]
        public Int16 QuantidadeParcelasQuitadas { get; set; }

        /// <summary>Quantidade de parcelas a vencer</summary>
        [DataMember]
        public Int16 QuantidadeParcelasAVencer { get; set; }

        /// <summary>Valor débito</summary>
        [DataMember]
        public Decimal ValorDebito { get; set; }

        /// <summary>Parcelas da Composicao do RV</summary>
        [DataMember]
        public ParcelaRV[] Parcelas { get; set; }
    }

    /// <summary>Classe modelo representando uma parcela de um Resumo de Venda (RV).</summary>
    [DataContract]
    public class ParcelaRV : ModeloBase
    {
        /// <summary>Data da parcela</summary>
        [DataMember]
        public DateTime DataParcela { get; set; }

        /// <summary>Número da parcela</summary>
        [DataMember]
        public Int16 NumeroParcela { get; set; }

        /// <summary>Valor líquido da Parcela</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }
    }
}