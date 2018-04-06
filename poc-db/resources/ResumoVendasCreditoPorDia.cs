/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo.RelatorioValoresConsolidadosVendas
{
    /// <summary>
    /// Turquia - Resumo das vendas crédito por dia.
    /// </summary>
    public class ResumoVendasCreditoPorDia : TotalVendasBase
    {
        /// <summary>
        /// Número do estabelecimento
        /// </summary>
        public Int32 NumeroEstabelecimento { get; set; }

        /// <summary>
        /// Data da venda
        /// </summary>
        public DateTime? DataVenda { get; set; }

        /// <summary>
        /// Data do pagamento
        /// </summary>
        public DateTime? DataPagamento { get; set; }

        /// <summary>
        /// Prazo de recebimento
        /// </summary>
        public Int32 PrazoRecebimento { get; set; }

        /// <summary>
        /// Número do resumo de venda
        /// </summary>
        public Int32 NumeroResumoVenda { get; set; }

        /// <summary>
        /// Quantidade de vendas
        /// </summary>
        public Int32 QuantidadeVendas { get; set; }

        /// <summary>
        /// Tipo de venda
        /// </summary>
        public String TipoVenda { get; set; }

        /// <summary>
        /// Código da bandeira
        /// </summary>
        public Int16 CodigoBandeira { get; set; }

        /// <summary>
        /// Descrição da bandeira
        /// </summary>
        public String DescricaoBandeira { get; set; }
    }
}
