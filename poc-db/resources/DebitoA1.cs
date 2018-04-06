using System;

namespace Redecard.PN.Extrato.Modelo.Vendas
{
    /// <summary>
    /// Classe modelo para Relatório de Vendas - Débito - Ajuste com valor
    /// </summary>
    public class DebitoA1 : Debito
    {
        /// <summary>
        /// Data de Apresentação
        /// </summary>
        public DateTime DataApresentacao { get; set; }

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
        /// 'D' = DEBITO / 'C' = CREDITO
        /// </summary>
        public String DebitoCredito { get; set; }

        /// <summary>
        /// Valor Liquido
        /// </summary>
        public Decimal ValorLiquido { get; set; }

        /// <summary>
        /// Resto
        /// </summary>
        public String Filler { get; set; }

        /// <summary>
        /// Resto
        /// </summary>
        public String Filler2 { get; set; }
    }
}