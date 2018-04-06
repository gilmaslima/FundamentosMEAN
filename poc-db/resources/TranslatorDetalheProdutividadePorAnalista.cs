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


namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorDetalheProdutividadePorAnalista, e expõe métodos para traduzir os dados de detalhe de produtividade por analista oriundos do webservice
    /// </summary>
    public class TranslatorDetalheProdutividadePorAnalista
    {
        /// <summary>
        /// Este método é utilizado para traduzir os os dados de detalhe de produtividade por analista oriundos do registro do analista.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static DetalheProdutividadePorAnalista TranslateRecordByAnalystProdutividadePorAnalistaBusiness(recordByAnalyst from)
        {
            DetalheProdutividadePorAnalista to = new DetalheProdutividadePorAnalista();
            to.Data = from.date;
            to.QuantidadeCartoesAnalisados = from.analysedCardAmount;
            to.ValorFraude = from.fraudTotalValue;
            to.QuantidadeCartoesFraudulentos = from.fraudulentCardAmount;
            to.QuantidadeTransacoesFraudulentas = from.fraudulentTransactionAmount;
            to.ValorNaoFraude = from.notFraudTotalValue;
            to.QuantidadeCartoesNaoFraudulentos = from.notFraudulentCardAmount;
            to.QuantidadeTransacoesNaoFraudulentas = from.notFraudulentTransactionAmount;

            return to;
        }

    }
}
