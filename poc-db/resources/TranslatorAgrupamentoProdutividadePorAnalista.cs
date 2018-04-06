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
    /// Este componente publica a classe TranslatorAgrupamentoProdutividadePorAnalista, e expõe métodos para traduzir os dados de agrupamento de produtividade por analista oriundos do webservice
    /// </summary>
    public class TranslatorAgrupamentoProdutividadePorAnalista
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de produtividade por analista para agrupamento oriundos do webservice.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static AgrupamentoProdutividadePorAnalista TranslateproductivityByAnalystToAgrupamentoProdutividadePorAnalistaBusiness(productivityByAnalyst from)
        {
            AgrupamentoProdutividadePorAnalista to = new AgrupamentoProdutividadePorAnalista();

            to.ProdutividadePorAnalista = new List<Redecard.PN.FMS.Modelo.DetalheProdutividadePorAnalista>();
            to.ProdutividadePorAnalista.AddRange(Array.ConvertAll<recordByAnalyst, DetalheProdutividadePorAnalista>(from.recordByAnalistList, TranslatorDetalheProdutividadePorAnalista.TranslateRecordByAnalystProdutividadePorAnalistaBusiness));

            to.QuantidadeTotalCartoesAnalisados = from.totalAnalysedCardAmount;
            to.QuantidadeTotalTransacoesFraudulentas = from.totalFraudulentTransactionAmount;
            to.QuantidadeTotalCartoesFraudulentos = from.totalFraudulentCardAmount;
            to.QuantidadeTotalCartoesNaoFraudulentos = from.totalNotFraudulentCardAmount;
            to.QuantidadeTotalTransacoesNaoFraudulentas = from.totalNotFraudulentTransactionAmount;
            to.ValorTotalFraude = from.totalFraudTotalValue;
            to.ValorTotalNaoFraude = from.totalNotFraudTotalValue;
            to.UsuarioLogin = from.userLogin;

            return to;
        }

    }
}
