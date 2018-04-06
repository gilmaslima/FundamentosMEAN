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
using Redecard.PN.FMS.Comum;

namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorRespostaAnalise, e expõe métodos para traduzir os dados de análise de respostas, oriundos do webservice
    /// </summary>
    public class TranslatorRespostaAnalise
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de análise de resposta. 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static RespostaAnalise TranslateRespostaAnaliseWSRespostaAnaliseBusiness(fraudComposite from)
        {
            RespostaAnalise to = new RespostaAnalise();
            to.NumeroEmissor =  from.cardIssuingAgentNumber;
            to.Comentario = from.comment;
            to.GrupoEntidade = from.entityGroup;
            to.EhFraude =  (from.fraudStatus == 1);
            to.TipoResposta = new TipoResposta() { CodigoResposta = from.responseTypeId, SituacaoFraude = (SituacaoFraude) from.fraudStatus+1 };
            to.IdentificadorTransacao = from.transactionId;
            to.UsuarioLogin = from.userLogin;
            to.TipoAlarme =  (TipoAlarme)from.alarmType;
            return to;
        }
        /// <summary>
        /// Este método é utilizado para traduzir os dados de análise de resposta. 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static fraudComposite TranslateRespostaAnaliseBusinessRespostaAnaliseWS(RespostaAnalise from)
        {
            fraudComposite to = new fraudComposite();

            to.cardIssuingAgentNumber = from.NumeroEmissor;
            to.comment = from.Comentario;
            to.entityGroup = from.GrupoEntidade;
            if (from.TipoResposta != null)
            {
                to.responseTypeId = (int)from.TipoResposta.CodigoResposta;
                to.responseTypeIdSpecified = true;
                to.fraudStatus = ConverterSituacaoTransacao(from.TipoResposta.SituacaoFraude);
                
            }
            else
            {
                to.responseTypeIdSpecified = false;
            }
            to.transactionId = from.IdentificadorTransacao;
            to.userLogin = from.UsuarioLogin;
            to.alarmType = (int) from.TipoAlarme;

            return to;
        }



        public static int ConverterSituacaoTransacao(SituacaoFraude situacao)
        {
            switch (situacao)
            {
                case SituacaoFraude.NaoFraude:
                    return 0;
                case SituacaoFraude.Fraude:
                    return 1;
                case SituacaoFraude.EmAnalise:
                    return 2;
                default:
                    return 0;

            }

        }
    }
}
