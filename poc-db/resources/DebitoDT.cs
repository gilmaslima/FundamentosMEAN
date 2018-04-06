/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Vendas
{
    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Vendas - Débito.<br/>
    /// Representa os registros do tipo "DT".
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1313 / Programa WA1313 / TranID ISHD
    /// </remarks>
    [DataContract]
    public class DebitoDT : Debito
    {
        /// <summary>Data da Venda</summary>
        [DataMember]
        public DateTime DataVenda { get; set; }

        /// <summary>Data de Vencimento</summary>
        [DataMember]
        public DateTime DataVencimento { get; set; }

        /// <summary>Número do Estabelecimento</summary>
        [DataMember]
        public Int32 NumeroPV { get; set; }

        /// <summary>Número do Resumo de Vendas</summary>
        [DataMember]
        public Int32 NumeroResumo { get; set; }

        /// <summary>Descrição da Bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Quantidade de Transações do RV</summary>
        [DataMember]
        public Int32 QuantidadeTransacoesRV { get; set; }

        /// <summary>Descrição do Resumo de Venda</summary>
        [DataMember]
        public String DescricaoResumo { get; set; }

        /// <summary>Valor Apresentado</summary>
        [DataMember]
        public Decimal ValorApresentado { get; set; }

        /// <summary>Valor do Saque</summary>
        [DataMember]
        public Decimal ValorSaque { get; set; }

        /// <summary>Valor do Desconto</summary>
        [DataMember]
        public Decimal ValorDesconto { get; set; }

        /// <summary>Valor Líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Banco de Crédito</summary>
        [DataMember]
        public Int32 BancoCredito { get; set; }

        /// <summary>Agência de Crédito</summary>
        [DataMember]
        public Int32 AgenciaCredito { get; set; }

        /// <summary>Conta de Crédito</summary>
        [DataMember]
        public String ContaCredito { get; set; }
    }
}