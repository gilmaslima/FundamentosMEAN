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
    /// Este componente publica a classe TranslatorRespostaTransacoesPorCartao, e expõe métodos para traduzir os dados de respostas de transações por cartão oriundos do webservice
    /// </summary>
    public static class TranslatorRespostaTransacoesPorCartao
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de transações por cartão suspeitas para respostas de transações por cartão. 
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public static RespostaTransacoesPorCartao TranslateCardSuspectTransactionCompositeToRespostaTransacoesPorCartao(cardSuspectTransactionComposite de)
        {
            RespostaTransacoesPorCartao para = new RespostaTransacoesPorCartao();

            para.QuantidadeHorasPeriodo = de.totalHoursInPeriod;
            para.QuantidadeHorasRecuperadas = de.retrievedHoursAmount;
            para.QuantidadeRegistros = de.recordAmount;
            para.ListaTransacoes = new List<AgrupamentoTransacoesCartao>();
            if (de.transactionList != null)
                para.ListaTransacoes.AddRange(Array.ConvertAll<cardSuspectTransaction, AgrupamentoTransacoesCartao>(de.transactionList, TranslatorAgrupamentoTransacoesCartao.TranslateCardSuspectTransactionToAgrupamentoTransacoesCartao));

            return para;

        }


    }
}
