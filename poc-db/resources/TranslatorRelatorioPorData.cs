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
    /// Este componente publica a classe TranslatorRelatorioPorData, e expõe métodos para traduzir os dados de relatório por data, oriundos do webservice
    /// </summary>
    public class TranslatorRelatorioPorData
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de relatorio por data oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static ProdutividadePorData TranslateRelatorioPorDataWSRelatorioPorDataBusiness(recordByDate from)
        {
            ProdutividadePorData to = new ProdutividadePorData();

            to.UsuarioLogin = from.userLogin;
            to.QuantidadeCartoesAnalisados = from.analysedCardAmount;
            to.ValorFraude = from.fraudTotalValue;
            to.QuantidadeCartoesFraudulentos = from.fraudulentCardAmount;
            to.QuantidadeTransacoesFraudulentas = from.fraudulentTransactionAmount;
            to.ValorNaoFraude = from.notFraudTotalValue;
            to.QuantidadeCartoesNaoFraudulentos = from.notFraudulentCardAmount;
            to.QuantidadeTransacoesNaoFraudulentas = from.notFraudulentTransactionAmount;

            return to;
        }

        /// <summary>
        /// Este método é utilizado para traduzir os dados de relatorio por data oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static recordByDate TranslateRelatorioPorDataBusinessRelatorioDataWS(ProdutividadePorData from)
        {
            recordByDate to = new recordByDate();

            to.userLogin = from.UsuarioLogin;
            to.analysedCardAmount = from.QuantidadeCartoesAnalisados;
            to.fraudTotalValue = from.ValorFraude;
            to.fraudulentCardAmount = from.QuantidadeCartoesFraudulentos;
            to.fraudulentTransactionAmount = from.QuantidadeTransacoesFraudulentas;
            to.notFraudTotalValue = from.ValorNaoFraude;
            to.notFraudulentCardAmount = from.QuantidadeCartoesNaoFraudulentos;
            to.notFraudulentTransactionAmount = from.QuantidadeTransacoesNaoFraudulentas;

            return to;
        }
    }
}
