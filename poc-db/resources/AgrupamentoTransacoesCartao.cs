/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo.Transacoes
{
    /// <summary>
    /// Este componente publica a classe AgrupamentoTransacoesCartao, que expõe propriedades para manipular dados de agrupamento de transações por cartão. 
    /// </summary>
    [DataContract]
    public class AgrupamentoTransacoesCartao
    {
        [DataMember]
        public long QuantidadeTransacoesSuspeitasAprovadas { get; set; }
        [DataMember]
        public decimal ValorTransacoesSuspeitasAprovadas { get; set; }
        [DataMember]
        public string NumeroCartao { get; set; }
        [DataMember]
        public SituacaoCartao SituacaoCartao { get; set; }
        [DataMember]
        public DateTime DataTransacaoSuspeitaMaisRecente { get; set; }
        [DataMember]
        public long QuantidadeTransacoesSuspeitasNegadas { get; set; }
        [DataMember]
        public decimal ValorTransacoesSuspeitasNegadas { get; set; }
        [DataMember]
        public IndicadorTipoCartao TipoCartao { get; set; }
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public decimal ValorTotalTransacoes { get; set; }
    }
}
