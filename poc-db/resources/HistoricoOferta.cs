/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Classe modelo de Histórico da Oferta
    /// </summary>
    [DataContract]
    public class HistoricoOferta
    {
        /// <summary>
        /// Ano de Faixa do Histórico
        /// </summary>
        [DataMember]
        public Int32? AnoFaixa { get; set; }

        /// <summary>
        /// Ano de Referência de Apuração
        /// </summary>
        [DataMember]
        public Int32 AnoReferenciaApuracao { get; set; }

        /// <summary>
        /// Código de Estrutura de Meta
        /// </summary>
        [DataMember]
        public Int64? CodigoEstruturaMeta { get; set; }

        /// <summary>
        /// Código de Faixa de Meta do Histórico
        /// </summary>
        [DataMember]
        public Int64? CodigoFaixaMeta { get; set; }

        /// <summary>
        /// Código da Oferta da Proposta
        /// </summary>
        [DataMember]
        public Int64 CodigoOferta { get; set; }

        /// <summary>
        /// Código da Proposta
        /// </summary>
        [DataMember]
        public Int64 CodigoProposta { get; set; }

        /// <summary>
        /// Data de aceita da Proposta
        /// </summary>
        [DataMember]
        public DateTime DataAceitePropostaOferta { get; set; }

        /// <summary>
        /// Descrição da Mensagem de Apuração
        /// </summary>
        [DataMember]
        public String DescricaoMensagemApuracao { get; set; }

        /// <summary>
        /// Mês de Início da Faixa
        /// </summary>
        [DataMember]
        public Int32? MesInicioFaixa { get; set; }

        /// <summary>
        /// Mês de Referência da apuração
        /// </summary>
        [DataMember]
        public Int32 MesReferenciaApuracao { get; set; }

        /// <summary>
        /// Número do Grupo Comercial
        /// </summary>
        [DataMember]
        public Int64? NumeroGrupoComercial { get; set; }

        /// <summary>
        /// Número do Estabelecimento
        /// </summary>
        [DataMember]
        public Int64? NumeroPontoVenda { get; set; }

        /// <summary>
        /// Número do Ponto de Venda Matriz
        /// </summary>
        [DataMember]
        public Int64? NumeroPontoVendaMatriz { get; set; }

        /// <summary>
        /// Descrição do Período de Apuração
        /// </summary>
        [DataMember]
        public String PeriodoApuracao { get; set; }

        /// <summary>
        /// Indica de a oferta possuía carência no período do histórico
        /// </summary>
        [DataMember]
        public Boolean PossuiCarencia { get; set; }

        /// <summary>
        /// Quantidade de Pontos de Venda
        /// </summary>
        [DataMember]
        public Int64 QuantidadePontosVenda { get; set; }

        /// <summary>
        /// Indicador de se recebeu benefício
        /// </summary>
        [DataMember]
        public Boolean RecebeuBeneficio { get; set; }

        /// <summary>
        /// Tipo do Estabelecimento
        /// </summary>
        [DataMember]
        public TipoEstabelecimentoHistoricoOferta TipoEstabelecimento { get; set; }

        /// <summary>
        /// Descrição do Tipo do Estabelecimento
        /// </summary>
        [DataMember]
        public String DescricaoTipoEstabelecimento { get; set; }

        /// <summary>
        /// Valor de faturamento da oferta no período do Histórico
        /// </summary>
        [DataMember]
        public Double ValorFaturamento { get; set; }

        /// <summary>
        /// Valor Final da Faixa no período do Histórico
        /// </summary>
        [DataMember]
        public Double? ValorFinalFaixa { get; set; }

        /// <summary>
        /// Valor Inicial da Faixa no período do Histórico
        /// </summary>
        [DataMember]
        public Double? ValorInicialFaixa { get; set; }
    }
}