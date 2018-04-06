/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.FMS.Servico.Modelo;
using Redecard.PN.FMS.Comum;

namespace Redecard.PN.FMS.Servico.Tradutor
{
    /// <summary>
    /// Este componente publica a classe TradutorTransacaoEmissor, que expõe métodos para manipular as transações por emissor.
    /// </summary>
    public class TradutorTransacaoEmissor
    {
        /// <summary>
        /// Este método é utilizado para traduzir uma transação por emissor.
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static TransacaoEmissor TraduzirTransacaoEmissor(Redecard.PN.FMS.Modelo.TransacaoEmissor tran)
        {
            TransacaoEmissor objRetorno = new TransacaoEmissor();

            objRetorno.Bandeira = tran.Bandeira;
            objRetorno.CodigoEstabelecimento = tran.CodigoEstabelecimento;
            objRetorno.CodigoMCC = tran.CodigoMCC;
            objRetorno.ComentarioAnalise = tran.ComentarioAnalise;
            objRetorno.DataAnalise = tran.DataAnalise;
            objRetorno.DataEnvioAnalise = tran.DataEnvioAnalise;
            objRetorno.DataTransacao = tran.DataTransacao;
            objRetorno.DescricaoEntryMode = tran.DescricaoEntryMode;
            objRetorno.DescricaoMCC = tran.DescricaoMCC;
            objRetorno.EntryMode = tran.EntryMode;
            objRetorno.IdentificadorTransacao = tran.IdentificadorTransacao;
            objRetorno.NomeEstabelecimento = tran.NomeEstabelecimento;
            objRetorno.NumeroCartao = tran.NumeroCartao;
            objRetorno.ResultadoAutorizacao = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.ResultadoAutorizacao, Redecard.PN.FMS.Servico.Modelo.ResultadoAutorizacao>(tran.ResultadoAutorizacao);
            objRetorno.Score = tran.Score;
            objRetorno.SituacaoBloqueio = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.SituacaoBloqueio, Redecard.PN.FMS.Servico.Modelo.SituacaoBloqueio>(tran.SituacaoBloqueio);
            objRetorno.SituacaoTransacao = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.SituacaoFraude, Redecard.PN.FMS.Servico.Modelo.SituacaoFraude>(tran.SituacaoTransacao);
            objRetorno.TipoAlarme = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.TipoAlarme, Redecard.PN.FMS.Servico.Modelo.TipoAlarme>(tran.TipoAlarme);
            objRetorno.TipoCartao = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.TipoCartao, Redecard.PN.FMS.Servico.Modelo.TipoCartao>(tran.TipoCartao);

            if (tran.TipoResposta != null)
            {
                objRetorno.TipoResposta = new TipoResposta()
                {
                    CodigoResposta = tran.TipoResposta.CodigoResposta,
                    DescricaoResposta = tran.TipoResposta.DescricaoResposta,
                    NomeResposta = tran.TipoResposta.NomeResposta,
                    SituacaoFraude = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.SituacaoFraude, Redecard.PN.FMS.Servico.Modelo.SituacaoFraude>(tran.TipoResposta.SituacaoFraude)
                };
            }
            else
            {
                objRetorno.TipoResposta = new TipoResposta();
            }

            objRetorno.UnidadeFederacao = tran.UnidadeFederacao;
            objRetorno.UsuarioAnalise = tran.UsuarioAnalise;
            objRetorno.Valor = tran.Valor;

            return objRetorno;
        }
        /// <summary>
        /// Este método é utilizado para traduzir as transações por emissor.
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static List<TransacaoEmissor> TraduzirTransacaoEmissor(List<Redecard.PN.FMS.Modelo.TransacaoEmissor> tran)
        {
            List<TransacaoEmissor> para = new List<TransacaoEmissor>();

            foreach (Redecard.PN.FMS.Modelo.TransacaoEmissor t in tran)
            {
                para.Add(TradutorTransacaoEmissor.TraduzirTransacaoEmissor(t));
            }

            return para;
        }
    }
}