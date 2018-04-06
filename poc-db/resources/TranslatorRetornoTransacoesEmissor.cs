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
using Redecard.PN.FMS.Agente.ServicoFMS;
using Redecard.PN.FMS.Agente.Tradutores;
using Redecard.PN.FMS.Modelo;

namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorRetornoTransacoesEmissor, e expõe métodos para traduzir os dados de retorno de transações por emissor, oriundos do webservice.
    /// </summary>
    public static class TranslatorRetornoTransacoesEmissor
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de retorno de transações por emissor encontrados, oriundos do webservice, para uma lista de tipos de resposta do emissor.
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public static TransacoesEmissor TranslateWSFindCountCompositeToTipoRespostaListaEmissorBusiness(findCountComposite de)
        {
            TransacoesEmissor para = new TransacoesEmissor();
            para.QuantidadeHorasRecuperadas = de.retrievedHoursAmount;
            para.QuantidadeHorasTotalPeriodo = de.totalHoursInPeriod;
            para.QuantidadeTotalRegistros = de.associatedCount;
            if (de.issuerTxList != null)
            {
                para.ListaTransacoesEmissor = new List<Modelo.TransacaoEmissor>();
                para.ListaTransacoesEmissor.AddRange(Array.ConvertAll<issuerTransaction, TransacaoEmissor>(de.issuerTxList, TranslatorTransacaoEmissor.TranslateIssuerTransactionToTransacaoEmissorBusiness));
            }

            return para;

        }


    }
}
