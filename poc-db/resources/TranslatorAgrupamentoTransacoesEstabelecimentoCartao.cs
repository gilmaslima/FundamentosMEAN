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
    /// Este componente publica a classe TranslatorAgrupamentoTransacoesEstabelecimentoCartao, e expõe métodos para traduzir os dados de agrupamento de transações por estabelecimento, oriundos do webservice
    /// </summary>
    public class TranslatorAgrupamentoTransacoesEstabelecimentoCartao
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de transações em cartões merchant suspeitos para agrupamento de transações em cartão por estabelecimento, oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static AgrupamentoTransacoesEstabelecimentoCartao TranslateMerchantCardSuspectTransactionToAgrupamentoTransacoesEstabelecimentoCartao(merchantCardSuspectTransaction from)
        {
            AgrupamentoTransacoesEstabelecimentoCartao to = new AgrupamentoTransacoesEstabelecimentoCartao();

            to.NumeroCartao = from.cardAccountNumber;
            to.QuantidadeTotalTransacoes = from.transactionAmount;
            to.QuantidadeTransacoesAprovadas = from.approvedTransactionAmount;
            to.QuantidadeTransacoesSuspeitas = from.declinedTransactionAmount;
            to.ValorTotalTransacoes = from.transactionValue;
            to.ValorTransacoesAprovadas = from.approvedTransactionValue;
            to.ValorTransacoesSuspeitas = from.declinedTransactionValue;
            to.TipoCartao = EnumHelper.EnumToEnum<cardTypeIndicator, IndicadorTipoCartao>(from.index);
            to.Situacao = EnumHelper.EnumToEnum<cardStatus, SituacaoCartao>(from.cardStatus);

            return to;
        }

    }
}
