/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 18/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente.ServicoFMS;
using Redecard.PN.FMS.Comum;

namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorAgrupamentoTransacoesCartao, e expõe métodos para traduzir os dados de agrupamento de transações por cartão, oriundos do webservice
    /// </summary>
    public class TranslatorAgrupamentoTransacoesCartao
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de cartões suspeitos para para agrupamento de transações de cartão. oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static AgrupamentoTransacoesCartao TranslateCardSuspectTransactionToAgrupamentoTransacoesCartao(cardSuspectTransaction from)
        {
            AgrupamentoTransacoesCartao to = new AgrupamentoTransacoesCartao();

            to.NumeroCartao = from.cardAccountNumber;
            to.DataTransacaoSuspeitaMaisRecente = from.date;
            to.QuantidadeTransacoesSuspeitasAprovadas = from.approvedTransactionAmount;
            to.QuantidadeTransacoesSuspeitasNegadas = from.declinedTransactionAmount;
            to.ValorTotalTransacoes = from.totalValue;
            to.ValorTransacoesSuspeitasAprovadas = from.approvedTransactionValue;
            to.SituacaoCartao = EnumHelper.EnumToEnum<cardStatus, SituacaoCartao>(from.cardStatus);
            to.TipoCartao = EnumHelper.EnumToEnum<cardTypeIndicator, IndicadorTipoCartao>(from.index);
            to.Score = from.score;
            to.ValorTransacoesSuspeitasNegadas = from.declinedTransactionValue;
            
            return to;
        }

    }
}
