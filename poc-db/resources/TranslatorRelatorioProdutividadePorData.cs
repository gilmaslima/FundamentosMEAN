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
    /// Este componente publica a classe TranslatorRelatorioProdutividadePorData, e expõe métodos para traduzir os dados de relatório de produtividade por data, oriundos do webservice
    /// </summary>
    public class TranslatorRelatorioProdutividadePorData
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados do relatório de produtividade por data, oriundos do webservice
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static RelatorioProdutividadePorData TranslateRelatorioProdutividadePorDataWSRelatorioProdutividadePorDataBusiness(dateReport from)
        {
            RelatorioProdutividadePorData to = new RelatorioProdutividadePorData();
            to.ProdutividadePorData = new List<AgrupamentoProdutividadePorData>();
            if (from.dateProductivityList != null)
            {
                foreach (productivityByDate registro in from.dateProductivityList)
                {
                    if (registro != null)
                        to.ProdutividadePorData.Add(TranslatorAgrupamentoProdutividadePorData.TranslateAgrupamentoProdutividadePorDataWSAgrupamentoProdutividadePorDataBusiness(registro));
                }
            }
            to.QuantidadeCartoesAnalisados = from.generalTotalAnalysedCardAmount;
            to.ValorTotalFraude = from.generalTotalFraudTotalValue;
            to.QuantidadeTotalCartoesFraudulentos = from.generalTotalFraudulentCardAmount;
            to.QuantidadeTotalTransacoesFraudulentas = from.generalTotalFraudulentTransactionAmount;
            to.ValorTotalNaoFraude = from.generalTotalNotFraudTotalValue;
            to.QuantidadeTotalCartoesNaoFraudulentos = from.generalTotalNotFraudulentCardAmount;
            to.QuantidadeTotalTransacoesNaoFraudulentas = from.generalTotalNotFraudulentTransactionAmount;

            return to;
        }

    }
}
