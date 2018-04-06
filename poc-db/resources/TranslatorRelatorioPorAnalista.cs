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
    /// Este componente publica a classe TranslatorRelatorioPorAnalista, e expõe métodos para traduzir os dados de relatório por analista oriundos do webservice
    /// </summary>
    public class TranslatorRelatorioPorAnalista
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados do analista oriundos do webservice para relatório de produtovidade por analista
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static RelatorioProdutividadePorAnalista TranslateRelatorioPorAnalistaWSRelatorioPorAnalistaBusiness(analystReport from)
        {
            RelatorioProdutividadePorAnalista to = new RelatorioProdutividadePorAnalista();
            to.AgrupamentoProdutividadePorAnalista = new List<AgrupamentoProdutividadePorAnalista>();
            foreach (productivityByAnalyst registro in from.analystProductivityList)
            {
                to.AgrupamentoProdutividadePorAnalista.Add(TranslatorAgrupamentoProdutividadePorAnalista.TranslateproductivityByAnalystToAgrupamentoProdutividadePorAnalistaBusiness(registro));
            }

            to.QuantidadeTotalCartoesAnalisados = from.generalTotalAnalysedCardAmount;
            to.QuantidadeTotalCartoesFraudulentos = from.generalTotalFraudulentCardAmount;
            to.QuantidadeTotalTransacoesFraudulentas = from.generalTotalFraudulentTransactionAmount;
            to.ValorTotalNaoFraude = from.generalTotalNotFraudTotalValue;
            to.QuantidadeTotalCartoesNaoFraudulentos = from.generalTotalNotFraudulentCardAmount;
            to.QuantidadeTotalTransacoesNaoFraudulentas = from.generalTotalNotFraudulentTransactionAmount;
            return to;
        }

    
    }
}
