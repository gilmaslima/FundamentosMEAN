using Redecard.PN.OutrosServicos.Servicos.PlanoContas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.OutrosServicos.Servicos.Modelos
{
    [DataContract]
    public class DetalheOferta : Oferta
    {
        /// <summary>
        /// Data de solicitação do cancelamento
        /// </summary>
        [DataMember]
        public DateTime? DataSolicitacaoCancelamento { get; set; }

        /// <summary>
        /// Data de cancelamento
        /// </summary>
        [DataMember]
        public DateTime? DataCancelamento { get; set; }

        /// <summary>
        /// Canal de Contratacao
        /// </summary>
        [DataMember]
        public String DescricaoCanal { get; set; }

        /// <summary>
        /// Número CNPJ
        /// </summary>
        [DataMember]
        public Decimal NumeroCnpj { get; set; }

        /// <summary>
        /// Quantidade de terminais no pacote
        /// </summary>
        [DataMember]
        public Int32 QuantidadeTerminais { get; set; }

        /// <summary>
        /// Tipo de Tecnologia
        /// </summary>
        [DataMember]
        public String TipoTecnologia { get; set; }

        /// <summary>
        /// Valor aluguel primeiro terminal
        /// </summary>
        [DataMember]
        public Decimal ValorAluguelPrimeiroTerminal { get; set; }

        /// <summary>
        /// Valor aluguel demais terminais
        /// </summary>
        [DataMember]
        public Decimal ValorAluguelDemaisTerminais { get; set; }
    }
}