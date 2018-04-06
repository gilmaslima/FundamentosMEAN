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
    /// Este componente publica a classe TranslatorAgrupamentoProdutividadePorData, e expõe métodos para traduzir os dados de agrupamento de produtividade por data oriundos do webservice
    /// </summary>
    public class TranslatorAgrupamentoProdutividadePorData
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de produtividade por Data para agrupamento de produtividade por data, oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static AgrupamentoProdutividadePorData TranslateAgrupamentoProdutividadePorDataWSAgrupamentoProdutividadePorDataBusiness(productivityByDate from)
        {
            AgrupamentoProdutividadePorData to = new AgrupamentoProdutividadePorData();

            to.ProdutividadePorData = new List<Redecard.PN.FMS.Modelo.ProdutividadePorData>();
            if (from.recordByDateList != null)
            {
                foreach (recordByDate registro in from.recordByDateList)
                {
                    to.ProdutividadePorData.Add(TranslatorProdutividadePorData.TranslateProdutividadePorDataWSProdutividadePorDataBusiness(registro));
                }
            }
            to.Data = from.date;
            to.QuantidadeCartoesAnalisados = from.totalAnalysedCardAmount;
            to.QuantidadeTotalCartoesFraudulentos = from.totalFraudulentCardAmount;
            to.QuantidadeTotalCartoesNaoFraudulentos = from.totalNotFraudulentCardAmount;
            to.QuantidadeTotalTransacoesFraudulentas = from.totalFraudulentTransactionAmount;
            to.QuantidadeTotalTransacoesNaoFraudulentas = from.totalNotFraudulentTransactionAmount;
            to.ValorTotalFraude = from.totalFraudTotalValue;
            to.ValorTotalNaoFraude = from.totalNotFraudTotalValue;

            return to;
        }

        /// <summary>
        /// Este método é utilizado para traduzir os dados de agrupamento de produtividade por Data para produtividade por data, oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static productivityByDate TranslateAgrupamentoProdutividadePorDataWSAgrupamentoProdutividadePorDataBusiness(AgrupamentoProdutividadePorData from)
        {
            productivityByDate to = new productivityByDate();
            to.date = from.Data;
            to.recordByDateList = new recordByDate[from.ProdutividadePorData.Count];

            int contador = 0;
            if (from.ProdutividadePorData != null)
            {
                foreach (ProdutividadePorData registro in from.ProdutividadePorData)
                {
                    to.recordByDateList[contador] = TranslatorProdutividadePorData.TranslateProdutividadePorDataBusinessProdutividadePorDataWS(registro);
                }
            }

            to.totalAnalysedCardAmount = from.QuantidadeCartoesAnalisados;
            to.totalFraudulentCardAmount = from.QuantidadeTotalCartoesFraudulentos;
            to.totalNotFraudulentCardAmount = from.QuantidadeTotalCartoesNaoFraudulentos;
            to.totalFraudulentTransactionAmount = from.QuantidadeTotalTransacoesFraudulentas;
            to.totalNotFraudulentTransactionAmount = from.QuantidadeTotalTransacoesNaoFraudulentas;
            to.totalFraudTotalValue = from.ValorTotalFraude;
            to.totalNotFraudTotalValue = from.ValorTotalNaoFraude;

            return to;
        }


    }
}
