/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Classe modelo de transações Corban
    /// </summary>
    [DataContract]
    public class TransacaoCorban
    {
        /// <summary>
        /// Número do Estabelecimento
        /// </summary>
        [DataMember]
        public Int32 NumeroEstabelecimento { get; set; }

        /// <summary>
        /// Data do Pagamento
        /// </summary>
        [DataMember]
        public DateTime? DataPagamento { get; set; }

        /// <summary>
        /// Hora do Pagamento
        /// </summary>
        [DataMember]
        public DateTime? HoraPagamento { get; set; }
        
        /// <summary>
        /// Código do Serviço
        /// </summary>
        [DataMember]
        public Decimal CodigoServico { get; set; }

        /// <summary>
        /// Código do tipo da Conta
        /// </summary>
        [DataMember]
        public Int32 CodigoTipoConta { get; set; }

        /// <summary>
        /// Descrição do Tipo de Conta
        /// </summary>
        [DataMember]
        public String DescricaoTipoConta { get; set; }

        /// <summary>
        /// Código da forma de pagamento
        /// </summary>
        [DataMember]
        public Int32 CodigoFormaPagamento { get; set; }

        /// <summary>
        /// Descrição Forma de Pagamento
        /// </summary>
        [DataMember]
        public String DescricaoFormaPagamento { get; set; }

        /// <summary>
        /// Descrição da bandeira
        /// </summary>
        [DataMember]
        public String DescricaoBandeira { get; set; }

        /// <summary>
        /// Código de barras
        /// </summary>
        [DataMember]
        public String CodigoBarras { get; set; }

        /// <summary>
        /// Nome da Operadora 
        /// </summary>
        [DataMember]
        public String NomeOperadora { get; set; }

        /// <summary>
        /// Valor bruto do pagamento
        /// </summary>
        [DataMember]
        public Decimal ValorBrutoPagamento { get; set; }

        /// <summary>
        /// Código do Status da Conta
        /// </summary>
        [DataMember]
        public Int32 CodigoStatusConta { get; set; }

        /// <summary>
        /// Status da Conta
        /// </summary>
        [DataMember]
        public String StatusConta { get; set; }
    }
}