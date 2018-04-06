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
using Redecard.PN.FMS.Comum;

namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorCriteriosSelecao, e expõe métodos para traduzir os dados de critérios de seleção, oriundos do webservice.
    /// </summary>
    public class TranslatorCriteriosSelecao
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de critérios de seleção do webservice para perfil de critérios
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static CriteriosSelecao TranslateProfileCriteriaToCriterioSelecaoBusiness(profileCriteria from)
        {
            CriteriosSelecao to = new CriteriosSelecao();

            to.TipoAlarme = new List<TipoAlarme>();

            if (from.alarmTypeList != null)
            {
                foreach (int tipoAlarme in from.alarmTypeList)
                {
                    to.TipoAlarme.Add((TipoAlarme) tipoAlarme);
                }
            }

            to.ResultadoAutorizacao = new List<CriterioResultadoAutorizacao>();
            if (from.authorization != null)
            {
                foreach (int autorizacao in from.authorization)
                {
                    to.ResultadoAutorizacao.Add((CriterioResultadoAutorizacao)autorizacao);
                }
            }
            to.CriterioClassificacao = (CriterioClassificacao)from.classifiedBy;
            to.CriterioClassificacaoEstabelecimento = (CriterioClassificacaoEstabelecimento)from.merchantClassifiedBy;
            to.InicioFaixaScore = from.startScoreRange;
            to.FimFaixaScore = from.endScoreRange;

            to.EntryModes = new List<EntryMode>();

            if (from.entryModeList != null)
            {
                foreach (entryMode entry in from.entryModeList)
                {
                    to.EntryModes.Add(TranslatorEntryMode.TranslateEntryModeWSMEntryModeBusiness(entry));
                }
            }


            to.EntryModesSelecionados = new List<EntryMode>();

            if (from.entryModeSelectedList != null)
            {
                foreach (entryMode entrySelecionado in from.entryModeSelectedList)
                {
                    to.EntryModesSelecionados.Add(TranslatorEntryMode.TranslateEntryModeWSMEntryModeBusiness(entrySelecionado));
                }
            }
            to.MCCsSelecionados = new List<MCC>();

            if (from.mccCodeSelectedList != null)
            {
                foreach (issuerMCC mccSelecionado in from.mccCodeSelectedList)
                {
                    to.MCCsSelecionados.Add(TranslatorMCC.TranslateMCCWSMCCBusiness(mccSelecionado));
                }
            }
            to.EstabelecimentosSelecionados = new List<long>();

            if (from.merchantIdList != null)
            {
                foreach (long estabalecimento in from.merchantIdList)
                {
                    to.EstabelecimentosSelecionados.Add(estabalecimento);
                }
            }

            to.RangeBinsSelecionados = new List<FaixaBin>();

            if (from.rangeBinSelectedList != null)
            {
                foreach (issuerRangeBin rangeSelecionados in from.rangeBinSelectedList)
                {
                    to.RangeBinsSelecionados.Add(TranslatorFaixaBin.TranslateFaixaBinWSFaixaBinBusiness(rangeSelecionados));
                }
            }

            to.UF = new List<string>();

            if (from.stateList != null)
            {
                foreach (string UF in from.stateList)
                {
                    to.UF.Add(UF);
                }
            }
            to.UFsSelecionadas = new List<string>();

            if (from.stateSelectedList != null)
            {
                foreach (string UFSelecionadas in from.stateSelectedList)
                {
                    to.UFsSelecionadas.Add(UFSelecionadas);
                }
            }

            to.TipoTransacaoSelecionadas = new List<TipoTransacao>();

            if(from.transactionType != null)
            {
                foreach (int tipoTransacao in from.transactionType)
                {
                    to.TipoTransacaoSelecionadas.Add((TipoTransacao)tipoTransacao);
                }
            }

            to.SituacoesCartaoSelecionados = new List<SituacaoCartao>();

            if (from.cardStatus != null)
            {
                foreach (cardStatus status in from.cardStatus)
                {
                    to.SituacoesCartaoSelecionados.Add(EnumHelper.EnumToEnum<cardStatus, SituacaoCartao>(status));
                }
            }

            to.ValorTransacaoInicial = from.valueTransactionFrom;
            to.ValorTransacaoFinal = from.valueTransactionTo;
            to.Usuario = from.user;

            return to;
        }

        /// <summary>
        /// Este método é utilizado para traduzir os dados de critérios de seleção do webservice para perfil de critérios de transação
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static profileCriteria TransactionCriterioSelecaoBusinessToProfileCriteria(CriteriosSelecao from)
        {
            LogHelper.GravarLogIntegracao("Critério de seleção atualizado (AG):", from);

            profileCriteria to = new profileCriteria();
            if (from.TipoAlarme != null)
            {
                to.alarmTypeList= new int?[from.TipoAlarme.Count];
                int contador = 0;
                foreach (TipoAlarme tipoAlarme in from.TipoAlarme)
                {
                    to.alarmTypeList[contador] = (int)tipoAlarme;
                    contador++;
                }
            }

            if ((from.ResultadoAutorizacao != null) && (from.ResultadoAutorizacao.Count > 0))
            {
                to.authorization = new int?[from.ResultadoAutorizacao.Count];
                int contador = 0;
                foreach (CriterioResultadoAutorizacao autorizacao in from.ResultadoAutorizacao)
                {
                    to.authorization[contador] = (int)autorizacao;
                    contador++;
                }
            }
            else
            {
                to.authorization = null;
            }

            to.classifiedBy = (int)from.CriterioClassificacao;
            to.merchantClassifiedBy = EnumHelper.EnumToEnum<CriterioClassificacaoEstabelecimento, merchantClassifiedBy>(from.CriterioClassificacaoEstabelecimento);

            to.startScoreRangeSpecified = (from.InicioFaixaScore != 0);
            to.startScoreRange =  from.InicioFaixaScore;
            to.endScoreRangeSpecified = (from.FimFaixaScore != 0);
            to.endScoreRange = from.FimFaixaScore;

            to.entryModeSelectedList = Array.ConvertAll<EntryMode, entryMode>(from.EntryModesSelecionados.ToArray<EntryMode>(), TranslatorEntryMode.TranslateEntryModeBusinessEntryModeWS);

            to.mccCodeSelectedList = Array.ConvertAll<MCC, issuerMCC>(from.MCCsSelecionados.ToArray<MCC>(), TranslatorMCC.TranslateMCCBusinessMCCWS);

            if (from.EstabelecimentosSelecionados.Count > 0)
            {
                to.merchantIdList = new long?[from.EstabelecimentosSelecionados.Count];
                int contador = 0;
                foreach (long estabalecimento in from.EstabelecimentosSelecionados)
                {
                    to.merchantIdList[contador] = estabalecimento;
                    contador++;
                }
            }

            to.rangeBinSelectedList = Array.ConvertAll<FaixaBin, issuerRangeBin>(from.RangeBinsSelecionados.ToArray<FaixaBin>(), TranslatorFaixaBin.TranslateFaixaBinBusinessFaixaBinWS);

            to.stateSelectedList = from.UFsSelecionadas.ToArray<string>() ;

            if ((from.TipoTransacaoSelecionadas != null) && (from.TipoTransacaoSelecionadas.Count > 0))
            {
                to.transactionType = new int?[from.TipoTransacaoSelecionadas.Count];

                int contador = 0;
                foreach (TipoTransacao tipoTransacao in from.TipoTransacaoSelecionadas)
                {
                    to.transactionType[contador] = (int)tipoTransacao;
                    contador++;
                }
            }
            else
            {
                to.transactionType = null;
            }

            to.valueTransactionFromSpecified = (from.ValorTransacaoInicial != 0);
            to.valueTransactionFrom = from.ValorTransacaoInicial;
            to.valueTransactionToSpecified = (from.ValorTransacaoFinal != 0);
            to.valueTransactionTo = from.ValorTransacaoFinal;

            if ((from.SituacoesCartaoSelecionados != null) && (from.SituacoesCartaoSelecionados.Count > 0))
            {
                to.cardStatus = new cardStatus?[from.SituacoesCartaoSelecionados.Count];

                int contador = 0;
                foreach (SituacaoCartao status in from.SituacoesCartaoSelecionados)
                {
                    to.cardStatus[contador] = EnumHelper.EnumToEnum<SituacaoCartao, cardStatus>(status);
                }
            }
            else
            {
                to.cardStatus = null;
            }

            to.user = from.Usuario;

            return to;
        }
    }
}
