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
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Vendas - Crédito.<br/>
    /// Representa os registros do tipo "D".
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1311 / Programa WA1311 / TranID ISHB
    /// </remarks>
    [DataContract]
    public class CreditoD : Credito
    {
        /// <summary>Data da Apresentação</summary>
        [DataMember]
        public DateTime DataApresentacao { get; set; }

        /// <summary>Data do Vencimento</summary>
        [DataMember]
        public DateTime DataVencimento { get; set; }

        /// <summary>Número do Estabelecimento</summary>
        [DataMember]
        public Int32 NumeroPV { get; set; }

        /// <summary>Número do Resumo de Vendas</summary>
        [DataMember]
        public Int32 NumeroResumo { get; set; }

        /// <summary>Prazo de Recebimento</summary>
        [DataMember]
        public Int16 PrazoRecebimento { get; set; }

        /// <summary>Descrição da Bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Quantidade de Transações</summary>
        [DataMember]
        public Int32 QuantidadeTransacoes { get; set; }

        /// <summary>Descrição do Resumo de Vendas</summary>
        [DataMember]
        public String Descricao { get; set; }

        /// <summary>Valor Apresentado</summary>
        [DataMember]
        public Decimal ValorApresentado { get; set; }

        /// <summary>Valor de Correção</summary>
        [DataMember]
        public Decimal ValorCorrecao { get; set; }

        /// <summary>Valor do Desconto</summary>
        [DataMember]
        public Decimal ValorDesconto { get; set; }

        /// <summary>Valor Líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }
    }
}