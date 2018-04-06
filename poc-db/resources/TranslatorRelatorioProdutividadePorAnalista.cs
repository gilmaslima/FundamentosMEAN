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
using Redecard.PN.FMS.Agente.Tradutores;
using Redecard.PN.FMS.Agente.ServicoFMS;

namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorRelatorioProdutividadePorAnalista, e expõe métodos para traduzir os dados de relatório de produtividade por analista oriundos do webservice
    /// </summary>
    public class TranslatorRelatorioProdutividadePorAnalista
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados do relatório de produtividade por analista 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static RelatorioProdutividadePorAnalista TranslateAnalystReportToRelatorioProdutividadePorAnalistaBusiness(analystReport from)
        {
            RelatorioProdutividadePorAnalista to = new RelatorioProdutividadePorAnalista();
            to.AgrupamentoProdutividadePorAnalista = new List<AgrupamentoProdutividadePorAnalista>();

            if (from.analystProductivityList != null)
            {
                to.AgrupamentoProdutividadePorAnalista.AddRange(Array.ConvertAll<productivityByAnalyst, AgrupamentoProdutividadePorAnalista>(from.analystProductivityList, TranslatorAgrupamentoProdutividadePorAnalista.TranslateproductivityByAnalystToAgrupamentoProdutividadePorAnalistaBusiness)); 
            }

            to.ValorTotalFraude = from.generalTotalFraudTotalValue;
            to.QuantidadeTotalCartoesFraudulentos = from.generalTotalFraudulentCardAmount;
            to.QuantidadeTotalTransacoesFraudulentas = from.generalTotalFraudulentTransactionAmount;
            to.ValorTotalNaoFraude = from.generalTotalNotFraudTotalValue;
            to.QuantidadeTotalCartoesNaoFraudulentos = from.generalTotalNotFraudulentCardAmount;
            to.QuantidadeTotalTransacoesNaoFraudulentas = from.generalTotalNotFraudulentTransactionAmount;
            to.QuantidadeTotalCartoesAnalisados = from.generalTotalAnalysedCardAmount;
            to.QuantidadeTotalRegistros = from.totalRecordAmount;

            return to;
        }

      
    }
}
