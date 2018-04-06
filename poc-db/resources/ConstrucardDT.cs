/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.Extrato.Modelo.Vendas
{
    /// <summary>
    /// Classe modelo para Relatório de Vendas - Construcard - Detalhe
    /// </summary>
    public class ConstrucardDT : Construcard
    {
        /// <summary>
        /// Data da Venda
        /// </summary>
        public DateTime DataVenda { get; set; }

        /// <summary>
        /// Data de Vencimento
        /// </summary>
        public DateTime DataVencimento { get; set; }

        /// <summary>
        /// Número do Estabelecimento
        /// </summary>
        public Int32 NumeroPV { get; set; }

        /// <summary>
        /// Número do Resumo
        /// </summary>
        public Int32 NumeroResumo { get; set; }

        /// <summary>
        /// Bandeira
        /// </summary>
        public String Bandeira { get; set; }

        /// <summary>
        /// Quantidade de Transações do RV
        /// </summary>
        public Int32 QuantidadeTransacoesRV { get; set; }

        /// <summary>
        /// Descrição do Resumo
        /// </summary>
        public String DescricaoResumo { get; set; }

        /// <summary>
        /// Valor Apresentado
        /// </summary>
        public Decimal ValorApresentado { get; set; }

        /// <summary>
        /// Valor de Desconto
        /// </summary>
        public Decimal ValorDesconto { get; set; }

        /// <summary>
        /// Valor Liquido
        /// </summary>
        public Decimal ValorLiquido { get; set; }

        /// <summary>
        /// Indicador de Sinal do Valor
        /// </summary>
        public String DebitoCredito { get; set; }

        /// <summary>
        /// Banco de Crédito
        /// </summary>
        public Int32 BancoCredito { get; set; }

        /// <summary>
        /// Agência de Crédito
        /// </summary>
        public Int32 AgenciaCredito { get; set; }

        /// <summary>
        /// Conta de Crédito
        /// </summary>
        public String ContaCredito { get; set; }
    }
}