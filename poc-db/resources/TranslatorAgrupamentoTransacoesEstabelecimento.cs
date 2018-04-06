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
    /// Este componente publica a classe TranslatorAgrupamentoTransacoesEstabelecimento, e expõe métodos para traduzir os dados de agrupamento de tyransações por estabelecimento, oriundos do webservice
    /// </summary>
    public class TranslatorAgrupamentoTransacoesEstabelecimento
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de registros suspeitos para agrupamento de transações por estabelecimentos, oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static AgrupamentoTransacaoEstabelecimento TranslateMerchantSuspectRecordToAgrupamentoTransacoesEstabelecimento(merchantSuspectRecord from)
        {
            AgrupamentoTransacaoEstabelecimento to = new AgrupamentoTransacaoEstabelecimento();

            to.NomeFantasiaEstabelecimento = from.merchantTradeName;
            to.NumeroEstabelecimento = from.merchantId;
            to.QuantidadeTotalTransacoes = from.transactionAmount;
            to.ValorTotalTransacoes = from.transactionValue;
            to.TipoCartao = EnumHelper.EnumToEnum<cardTypeIndicator, IndicadorTipoCartao>(from.index);
            to.QuantidadeTransacoesSuspeitasAprovadas = from.approvedTransactionAmount;
            to.QuantidadeTransacoesSuspeitasNegadas = from.declinedTransactionAmount;
            to.ValorTransacoesSuspeitasAprovadas = from.approvedTransactionValue;
            to.ValorTransacoesSuspeitasNegadas = from.declinedTransactionValue;

            return to;
        }

    }
}
