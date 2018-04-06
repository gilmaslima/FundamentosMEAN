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
    /// Este componente publica a classe TranslatorRespostaTransacoesEstabelecimento, e expõe métodos para traduzir os dados de resposta de transações por estabelecimentos, oriundos do webservice
    /// </summary>
    public static class TranslatorRespostaTransacoesEstabelecimento
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de Merchant Categoy Code suspeitos para resposta de transações por estabelecimento. 
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public static RespostaTransacoesEstabelecimento TranslateMerchantSuspectTransactionCompositeToRespostaTransacoesEstabelecimento(merchantSuspectTransactionComposite de)
        {
            RespostaTransacoesEstabelecimento para = new RespostaTransacoesEstabelecimento();

            para.QuantidadeHorasTotalPeriodo = de.totalHoursInPeriod;
            para.QuantidadeHorasRecuperadas = de.retrievedHoursAmount;
            para.QuantidadeTransacoes = de.recordAmount;
            para.ListaTransacoes = new List<AgrupamentoTransacaoEstabelecimento>();
            if (de.merchantRecordList != null)
                para.ListaTransacoes.AddRange(Array.ConvertAll<merchantSuspectRecord, AgrupamentoTransacaoEstabelecimento>(de.merchantRecordList, TranslatorAgrupamentoTransacoesEstabelecimento.TranslateMerchantSuspectRecordToAgrupamentoTransacoesEstabelecimento));
            
            return para;

        }


    }
}
