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

namespace Redecard.PN.FMS.Servico.Modelo
{
    /// <summary>
    /// Enumeração utilizado para critérios de ordem de transações de estabelecimento agrupadas por cartão.
    /// </summary>
    [DataContract]
    public enum CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao
    {
        [EnumMember]
        Valor = 0,
        [EnumMember]
        Quantidade = 1,
        [EnumMember]
        Cartao = 2,
        [EnumMember]
        ValorTransacoesSuspeitasAprovadas = 3,
        [EnumMember]
        QuantidadeTransacoesSuspeitasAprovadas = 4,
        [EnumMember]
        ValorTransacoesSuspeitasNegadas = 5,
        [EnumMember]
        QuantidadeTransacoesSuspeitasNegadas = 6,
        [EnumMember]
        SituacaoCartao = 7
               
    }
}
