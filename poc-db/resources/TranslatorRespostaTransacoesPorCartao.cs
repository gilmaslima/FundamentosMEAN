/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 18/12/2012 – Renao Cara – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente.ServicoFMS;

namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorRespostaTransacoesEstabelecimentoPorCartao, e expõe métodos para traduzir os dados de resposta de transações por cartão dos Estabelecimentos, oriundos do webservice
    /// </summary>
    public static class TranslatorRespostaTransacoesEstabelecimentoPorCartao
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de transações merchant por cartão suspeitas para respostas de transações por cartão dos estabelecimentos. 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static RespostaTransacoesEstabelecimentoPorCartao TranslateMerchantCardSuspectTransactionCompositeToRespostaTransacoesEstabelecimentoPorCartao(merchantCardSuspectTransactionComposite from)
        {
            RespostaTransacoesEstabelecimentoPorCartao to = new RespostaTransacoesEstabelecimentoPorCartao();

            to.NumeroEstabelecimento = from.merchantId;
            to.NomeEstabelecimento = from.merchantTradeName;
            to.QuantidadeHorasPeriodo = from.totalHoursInPeriod;
            to.QuantidadeHorasRecuperadas = from.retrievedHoursAmount;
            to.QuantidadeRegistros = from.recordAmount;
            to.ListaTransacoes = new List<AgrupamentoTransacoesEstabelecimentoCartao>();
            if (from.transactionList != null)
            {
                to.ListaTransacoes.AddRange(Array.ConvertAll<merchantCardSuspectTransaction, AgrupamentoTransacoesEstabelecimentoCartao>(from.transactionList, TranslatorAgrupamentoTransacoesEstabelecimentoCartao.TranslateMerchantCardSuspectTransactionToAgrupamentoTransacoesEstabelecimentoCartao));
            }
            return to;

        }


    }
}
