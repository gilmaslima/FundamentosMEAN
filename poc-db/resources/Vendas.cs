/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.Extrato.Modelo.HomePage
{
    /// <summary>
    /// Classe base para as Vendas a Crédito e Vendas a Débito utilizados na HomePage
    /// </summary>
    public class Vendas
    {
        /// <summary>Descrição da bandeira</summary>
        public String DescricaoBandeira { get; set; }

        /// <summary>Código da bandeira</summary>
        public Int16? CodigoBandeira { get; set; }

        /// <summary>Quantidade de Transações</summary>
        public Decimal QuantidadeTransacoes { get; set; }
        
        /// <summary>Valor apresentado</summary>
        public Decimal ValorApresentado { get; set; }

        /// <summary>Valor desconto</summary>
        public Decimal ValorDesconto { get; set; }

        /// <summary>Valor líquido</summary>
        public Decimal ValorLiquido { get; set; }
    }
}