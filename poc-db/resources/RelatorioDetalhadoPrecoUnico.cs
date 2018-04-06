/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using System;


namespace Redecard.PN.GerencieExtrato.Modelo
{
    /// <summary>
    /// Turquia - Relatório Detalhado Preço Único
    /// </summary>
    public class RelatorioDetalhadoPrecoUnico
    {
        /// <summary>
        /// Número do Estabelecimento
        /// </summary>
        public Int32 NumeroEstabelecimento { get; set; }

        /// <summary>
        /// Número Sequencial Único - NSU
        /// </summary>
        public Decimal NumeroSequencialUnico { get; set; }

        /// <summary>
        /// Número da TID - (Identificador da Transação)
        /// </summary>
        public String NumeroTid { get; set; }

        /// <summary>
        /// Número do cartão da venda (Mascarado)
        /// </summary>
        public String NumeroCartaoMascarado { get; set; }

        /// <summary>
        /// Código do produto (Crédigo e/ou Débito).
        /// </summary>
        public String CodigoProduto { get; set; }

        /// <summary>
        /// Tipo da venda
        /// </summary>
        public String TipoVenda { get; set; }

        /// <summary>
        /// Pais de origem da transação
        /// </summary>
        public String PaisOrigem { get; set; }

        /// <summary>
        /// Hora da venda
        /// </summary>
        public DateTime? HoraVenda { get; set; }

        /// <summary>
        /// Tipo de Captura
        /// </summary>
        public String TipoCaptura { get; set; }

        /// <summary>
        /// Data da venda
        /// </summary>
        public DateTime? DataVenda { get; set; }

        /// <summary>
        /// Valor bruto da venda
        /// </summary>
        public Decimal ValorVenda { get; set; }

        /// <summary>
        /// Valor do desconto
        /// </summary>
        public Decimal ValorDesconto { get; set; }

        /// <summary>
        /// Valor do Líquido (valor bruto (-) valor desconto)
        /// </summary>
        public Decimal ValorLiquido { get; set; }

        /// <summary>
        /// Código da bandeira
        /// </summary>
        public Int16 CodigoBandeira { get; set; }

        /// <summary>
        /// Descrição da bandeira
        /// </summary>
        public String DescricaoBandeira { get; set; }

        /// <summary>
        /// Número do resumo da venda que foi processada.
        /// </summary>
        public Int32 NumeroResumoVenda { get; set; }

        /// <summary>
        /// Número da parcela DE
        /// </summary>
        public Int16 NumeroParcelaDe { get; set; }

        /// <summary>
        /// Número da parcela ATÉ
        /// </summary>
        public Int16 NumeroParcelaAte { get; set; }

        /// <summary>
        /// Data do vencimento
        /// </summary>
        public DateTime? DataVencimento { get; set; }

        /// <summary>
        /// Prazo de pagamento
        /// </summary>
        public Int16 PrazoPagamento { get; set; }

        /// <summary>
        /// Status da venda
        /// </summary>
        public String StatusVenda { get; set; }       

    }
}
