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
    /// Este componente publica a classe TranslatorTransacaoEmissor, e expõe métodos para traduzir os dados de transações por emissor, oriundos do webservice
    /// </summary>
    public class TranslatorTransacaoEmissor
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de transações por emissor oriundos do webservice. 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static TransacaoEmissor TranslateIssuerTransactionToTransacaoEmissorBusiness(issuerTransaction from)
        {
            TransacaoEmissor to = new TransacaoEmissor();
            to.Bandeira = from.cardAssoc;
            to.CodigoEstabelecimento = from.merchantId;
            to.CodigoMCC = from.mccCode;
            to.ComentarioAnalise = from.handlingComment;
            to.DataAnalise = from.handlingDate;
            to.DataEnvioAnalise = from.analysedDate;
            to.DescricaoEntryMode = from.entryModeDesc;
            to.DescricaoMCC = from.mccPortDescription;
            to.EntryMode = from.entryMode;
            to.IdentificadorTransacao = from.transactionId;
            to.NomeEstabelecimento = from.merchantName;
            to.NumeroCartao = from.cardAccountNumber;
            to.ResultadoAutorizacao = (ResultadoAutorizacao) from.autorization;
            to.Score = from.score;
            to.SituacaoBloqueio = (SituacaoBloqueio)from.lockStatus;
            to.SituacaoTransacao = (SituacaoFraude)from.fraudSituation;
            to.TipoAlarme = (TipoAlarme)from.alarmType;
            to.TipoCartao = (TipoCartao)from.cardType;
            if (from.issuerResponseType != null)
                to.TipoResposta = TranslatorTipoResposta.TranslateIssuerResponseTypeToTipoRespostaBusiness(from.issuerResponseType);
            to.UnidadeFederacao = from.state;
            to.UsuarioAnalise = from.handlingUserLogin;
            to.Valor = from.valueTransaction;
            to.DataTransacao = from.dateTransaction;
            return to;

        }
        /// <summary>
        ///  Este método é utilizado para traduzir os dados de transações por emissor para o webservice. 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static issuerTransaction TranslateTransacaoEmissorBusinessToIssuerTransaction(TransacaoEmissor from)
        {
            issuerTransaction to = new issuerTransaction();

            to.cardAssoc = from.Bandeira;
            to.merchantId = from.CodigoEstabelecimento;
            to.mccCode = from.CodigoMCC;
            to.handlingComment = from.ComentarioAnalise;
            to.handlingDate = from.DataAnalise;
            to.analysedDate = from.DataEnvioAnalise;
            to.entryModeDesc = from.DescricaoEntryMode;
            to.mccPortDescription = from.DescricaoMCC;
            to.entryMode = from.EntryMode;
            to.transactionId = from.IdentificadorTransacao;
            to.merchantName = from.NomeEstabelecimento;
            to.cardAccountNumber = from.NumeroCartao;
            to.autorization = (int)from.ResultadoAutorizacao;
            to.score = from.Score;
            to.lockStatus = (int)from.SituacaoBloqueio;
            to.fraudSituation = (int)from.SituacaoTransacao;
            to.alarmType = (int)from.TipoAlarme;
            to.cardType = (int)from.TipoCartao;
            if (from.TipoResposta != null)
                to.issuerResponseType = TranslatorTipoResposta.TranslateTipoRespostaBusinessToIssuerResponseType(from.TipoResposta);
            to.state = from.UnidadeFederacao;
            to.handlingUserLogin = from.UsuarioAnalise;
            to.valueTransaction = from.Valor;
            to.dateTransaction = from.DataTransacao;
            return to;
        }

    }
}
