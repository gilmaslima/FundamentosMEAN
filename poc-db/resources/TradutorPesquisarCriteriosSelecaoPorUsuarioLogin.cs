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
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Servico.Modelo.Transacoes;
using Redecard.PN.FMS.Comum;

namespace Redecard.PN.FMS.Servico.Tradutor
{
    /// <summary>
    /// Este componente publica a classe TradutorPesquisarCriteriosSelecaoPorUsuarioLogin, que expõe métodos para manipular as pesquisas de critérios de seleção por usuário logado.
    /// </summary>
    public class TradutorPesquisarCriteriosSelecaoPorUsuarioLogin
    {

        #region paraPesquisarCriteriosSelecaoPorUsuarioLoginRetorno
        /// <summary>
        /// Este método é utilizado para traduzir critérios de seleção.
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public static PesquisarCriteriosSelecaoPorUsuarioLoginRetorno TraduzCriteriosSelecao(CriteriosSelecao de)
        {
            PesquisarCriteriosSelecaoPorUsuarioLoginRetorno para = new PesquisarCriteriosSelecaoPorUsuarioLoginRetorno();
            para.CriterioClassificacao = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.CriterioClassificacao ,Redecard.PN.FMS.Servico.Modelo.CriterioClassificacao> (de.CriterioClassificacao);
            para.EntryModes = TraduzEntryModes(de.EntryModes);
            para.EntryModesSelecionados =  TraduzEntryModes(de.EntryModesSelecionados);
            para.EstabelecimentosSelecionados = de.EstabelecimentosSelecionados;
            para.FimFaixaScore = de.FimFaixaScore;
            para.InicioFaixaScore = de.InicioFaixaScore;
            para.MCCsSelecionados = TraduzMCC( de.MCCsSelecionados);
            para.RangeBinsSelecionados = TraduzFaixaBin(de.RangeBinsSelecionados);
            para.ResultadoAutorizacao = TraduzirResultadoAutorizacao(de.ResultadoAutorizacao);
            para.SituacoesCartaoSelecionados = SituacoesCartaoSelecionados(de.SituacoesCartaoSelecionados);
            para.TipoAlarme = TraduzTipoAlarme(de.TipoAlarme);
            para.TipoTransacaoSelecionadas = TraduzTipoTransacao(de.TipoTransacaoSelecionadas);
            para.UF = de.UF;
            para.UFsSelecionadas = de.UFsSelecionadas;
            para.Usuario = de.Usuario;
            para.ValorTransacaoFinal = de.ValorTransacaoFinal;
            para.ValorTransacaoInicial = de.ValorTransacaoInicial;
            return para;
        }
        /// <summary>
        /// Este método é utilizado para traduzir merchant category code.
        /// </summary>
        /// <param name="deList"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Servico.Modelo.MCC> TraduzMCC(List<Redecard.PN.FMS.Modelo.MCC> deList)
        {
            if (deList == null)
            {
                return new List<Redecard.PN.FMS.Servico.Modelo.MCC>();
            }
            List<Redecard.PN.FMS.Servico.Modelo.MCC> retorno = new List<Redecard.PN.FMS.Servico.Modelo.MCC>();
            foreach (Redecard.PN.FMS.Modelo.MCC de in deList)
            {
                retorno.Add(new Redecard.PN.FMS.Servico.Modelo.MCC()
                {
                   CodigoMCC = de.CodigoMCC,
                    DescricaoMCC = de.DescricaoMCC
                });
            }
            return retorno;
        }
        /// <summary>
        /// Este método é utilizado para traduzir faixa bin.
        /// </summary>
        /// <param name="deList"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Servico.Modelo.FaixaBin> TraduzFaixaBin(List<Redecard.PN.FMS.Modelo.FaixaBin> deList)
        {
            if (deList == null)
            {
                return new List<Redecard.PN.FMS.Servico.Modelo.FaixaBin>();
            }
            List<Redecard.PN.FMS.Servico.Modelo.FaixaBin> retorno = new List<Redecard.PN.FMS.Servico.Modelo.FaixaBin>();
            foreach (Redecard.PN.FMS.Modelo.FaixaBin de in deList)
            {
                retorno.Add(new Redecard.PN.FMS.Servico.Modelo.FaixaBin()
                {
                    ValorFinal = de.ValorFinal,
                    ValorInicial = de.ValorInicial
                });
            }
            return retorno;
        }
        /// <summary>
        /// Este método é utilizado para traduzir entry modes.
        /// </summary>
        /// <param name="deList"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Servico.Modelo.EntryMode> TraduzEntryModes(List<Redecard.PN.FMS.Modelo.EntryMode> deList)
        {
            if (deList == null)
            {
                return new List<Redecard.PN.FMS.Servico.Modelo.EntryMode>();
            }
            List<Redecard.PN.FMS.Servico.Modelo.EntryMode> retorno = new List<Redecard.PN.FMS.Servico.Modelo.EntryMode>();
            foreach(Redecard.PN.FMS.Modelo.EntryMode de in deList)
            {
                retorno.Add(new Redecard.PN.FMS.Servico.Modelo.EntryMode()
                {
                    Codigo = de.Codigo,
                    Descricao = de.Descricao
                });
            }
            return retorno;
        }


        #endregion
        #region paraCriteriosSelecao
        /// <summary>
        /// Este método é utilizado para traduzir o retorno da pesquisa de critérios de seleção por usuário logado para critérios de seleção.
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public static CriteriosSelecao TraduzDePesquisarCriteriosSelecaoPorUsuarioLoginRetornoParaCriteriosSelecao(PesquisarCriteriosSelecaoPorUsuarioLoginRetorno de)
        {
            CriteriosSelecao para = new CriteriosSelecao();
            para.CriterioClassificacao = EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.CriterioClassificacao, Redecard.PN.FMS.Modelo.CriterioClassificacao>(de.CriterioClassificacao);
            para.EntryModes = TraduzEntryModes(de.EntryModes);
            para.EntryModesSelecionados = TraduzEntryModes(de.EntryModesSelecionados);
            para.EstabelecimentosSelecionados = de.EstabelecimentosSelecionados;
            para.FimFaixaScore = de.FimFaixaScore;
            para.InicioFaixaScore = de.InicioFaixaScore;
            para.MCCsSelecionados = TraduzMCC(de.MCCsSelecionados);
            para.RangeBinsSelecionados = TraduzFaixaBin(de.RangeBinsSelecionados);
            para.ResultadoAutorizacao = TraduzirResultadoAutorizacao(de.ResultadoAutorizacao);
            para.SituacoesCartaoSelecionados = SituacoesCartaoSelecionados(de.SituacoesCartaoSelecionados);
            para.TipoAlarme = TraduzTipoAlarme(de.TipoAlarme);
            para.TipoTransacaoSelecionadas = TraduzTipoTransacao(de.TipoTransacaoSelecionadas);
            para.UF = de.UF;
            para.UFsSelecionadas = de.UFsSelecionadas;
            para.Usuario = de.Usuario;
            para.ValorTransacaoFinal = de.ValorTransacaoFinal;
            para.ValorTransacaoInicial = de.ValorTransacaoInicial;
            return para;
        }
        /// <summary>
        /// Este método é utilizado paratraduzir os tipos de transação.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Modelo.TipoTransacao> TraduzTipoTransacao(List<Redecard.PN.FMS.Servico.Modelo.TipoTransacao> list)
        {
            List<Redecard.PN.FMS.Modelo.TipoTransacao> result = new List<Redecard.PN.FMS.Modelo.TipoTransacao>();

            if (list == null)
            {
                return result;
            }

            foreach (Redecard.PN.FMS.Servico.Modelo.TipoTransacao de in list)
            {
                Redecard.PN.FMS.Modelo.TipoTransacao para = EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.TipoTransacao, Redecard.PN.FMS.Modelo.TipoTransacao>(de);

                result.Add(para);
            }

            return result;
        }
        /// <summary>
        /// Este método é utilizado paratraduzir os tipos de transação.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Servico.Modelo.TipoTransacao> TraduzTipoTransacao(List<Redecard.PN.FMS.Modelo.TipoTransacao> list)
        {
            List<Redecard.PN.FMS.Servico.Modelo.TipoTransacao> result = new List<Redecard.PN.FMS.Servico.Modelo.TipoTransacao>();

            if (list == null)
            {
                return result;
            }

            foreach (Redecard.PN.FMS.Modelo.TipoTransacao de in list)
            {
                Redecard.PN.FMS.Servico.Modelo.TipoTransacao para = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.TipoTransacao, Redecard.PN.FMS.Servico.Modelo.TipoTransacao>(de);

                result.Add(para);
            }

            return result;
        }
        /// <summary>
        /// Este método é utilizado para traduzir os tipos de alarme.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Modelo.TipoAlarme> TraduzTipoAlarme(List<Redecard.PN.FMS.Servico.Modelo.TipoAlarme> list)
        {
            List<Redecard.PN.FMS.Modelo.TipoAlarme> result = new List<Redecard.PN.FMS.Modelo.TipoAlarme>();

            if (list == null)
            {
                return result;
            }

            foreach (Redecard.PN.FMS.Servico.Modelo.TipoAlarme de in list)
            {
                Redecard.PN.FMS.Modelo.TipoAlarme para = EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.TipoAlarme, Redecard.PN.FMS.Modelo.TipoAlarme>(de);

                result.Add(para);
            }

            return result;
        }
        /// <summary>
        /// Este método é utilizado para traduzir os tipos de alarme.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Servico.Modelo.TipoAlarme> TraduzTipoAlarme(List<Redecard.PN.FMS.Modelo.TipoAlarme> list)
        {
            List<Redecard.PN.FMS.Servico.Modelo.TipoAlarme> result = new List<Redecard.PN.FMS.Servico.Modelo.TipoAlarme>();

            if (list == null)
            {
                return result;
            }

            foreach (Redecard.PN.FMS.Modelo.TipoAlarme de in list)
            {
                Redecard.PN.FMS.Servico.Modelo.TipoAlarme para = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.TipoAlarme, Redecard.PN.FMS.Servico.Modelo.TipoAlarme>(de);

                result.Add(para);
            }

            return result;
        }
        /// <summary>
        /// Este método é utilizado para definir as situacõess dos cartões selecionados.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Modelo.SituacaoCartao> SituacoesCartaoSelecionados(List<Redecard.PN.FMS.Servico.Modelo.SituacaoCartao> list)
        {
            List<Redecard.PN.FMS.Modelo.SituacaoCartao> result = new List<Redecard.PN.FMS.Modelo.SituacaoCartao>();

            if (list == null)
            {
                return result;
            }

            foreach (Redecard.PN.FMS.Servico.Modelo.SituacaoCartao de in list)
            {
                Redecard.PN.FMS.Modelo.SituacaoCartao para = EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.SituacaoCartao, Redecard.PN.FMS.Modelo.SituacaoCartao>(de);

                result.Add(para);
            }

            return result;
        }
        /// <summary>
        /// Este método é utilizado para definir a situacoes dos cartões selecionados.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Servico.Modelo.SituacaoCartao> SituacoesCartaoSelecionados(List<Redecard.PN.FMS.Modelo.SituacaoCartao> list)
        {
            List<Redecard.PN.FMS.Servico.Modelo.SituacaoCartao> result = new List<Redecard.PN.FMS.Servico.Modelo.SituacaoCartao>();

            if (list == null)
            {
                return result;
            }

            foreach (Redecard.PN.FMS.Modelo.SituacaoCartao de in list)
            {
                Redecard.PN.FMS.Servico.Modelo.SituacaoCartao para = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.SituacaoCartao, Redecard.PN.FMS.Servico.Modelo.SituacaoCartao>(de);

                result.Add(para);
            }

            return result;
        }
        /// <summary>
        /// Este método é utilizado para traduzir os resultados de autorizacao.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Modelo.CriterioResultadoAutorizacao> TraduzirResultadoAutorizacao(List<Redecard.PN.FMS.Servico.Modelo.CriterioResultadoAutorizacao> list)
        {
            List<Redecard.PN.FMS.Modelo.CriterioResultadoAutorizacao> result = new List<Redecard.PN.FMS.Modelo.CriterioResultadoAutorizacao>();

            if (list == null)
            {
                return result;
            }

            foreach (Redecard.PN.FMS.Servico.Modelo.CriterioResultadoAutorizacao de in list)
            {
                Redecard.PN.FMS.Modelo.CriterioResultadoAutorizacao para = EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.CriterioResultadoAutorizacao, Redecard.PN.FMS.Modelo.CriterioResultadoAutorizacao>(de);

                result.Add(para);
            }

            return result;
        }
        /// <summary>
        /// Este método é utilizado para traduzir resultados de autorizacao.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Servico.Modelo.CriterioResultadoAutorizacao> TraduzirResultadoAutorizacao(List<Redecard.PN.FMS.Modelo.CriterioResultadoAutorizacao> list)
        {
            List<Redecard.PN.FMS.Servico.Modelo.CriterioResultadoAutorizacao> result = new List<Redecard.PN.FMS.Servico.Modelo.CriterioResultadoAutorizacao>();

            if (list == null)
            {
                return result;
            }

            foreach (Redecard.PN.FMS.Modelo.CriterioResultadoAutorizacao de in list)
            {
                Redecard.PN.FMS.Servico.Modelo.CriterioResultadoAutorizacao para = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.CriterioResultadoAutorizacao, Redecard.PN.FMS.Servico.Modelo.CriterioResultadoAutorizacao>(de);

                result.Add(para);
            }

            return result;
        }
        /// <summary>
        /// Este método é utilizado para traduzir merchant category codes.
        /// </summary>
        /// <param name="deList"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Modelo.MCC> TraduzMCC(List<Redecard.PN.FMS.Servico.Modelo.MCC> deList)
        {
            if (deList == null)
            {
                return new List<Redecard.PN.FMS.Modelo.MCC>();
            }
            List<Redecard.PN.FMS.Modelo.MCC> retorno = new List<Redecard.PN.FMS.Modelo.MCC>();
            foreach (Redecard.PN.FMS.Servico.Modelo.MCC de in deList)
            {
                retorno.Add(new Redecard.PN.FMS.Modelo.MCC()
                {
                    CodigoMCC = de.CodigoMCC,
                    DescricaoMCC = de.DescricaoMCC
                });
            }
            return retorno;
        }
        /// <summary>
        /// Este método é utilizado para traduz faixa bin.
        /// </summary>
        /// <param name="deList"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Modelo.FaixaBin> TraduzFaixaBin(List<Redecard.PN.FMS.Servico.Modelo.FaixaBin> deList)
        {
            if (deList == null)
            {
                return new List<Redecard.PN.FMS.Modelo.FaixaBin>();
            }
            List<Redecard.PN.FMS.Modelo.FaixaBin> retorno = new List<Redecard.PN.FMS.Modelo.FaixaBin>();
            foreach (Redecard.PN.FMS.Servico.Modelo.FaixaBin de in deList)
            {
                retorno.Add(new Redecard.PN.FMS.Modelo.FaixaBin()
                {
                    ValorFinal = de.ValorFinal,
                    ValorInicial = de.ValorInicial
                });
            }
            return retorno;
        }
        /// <summary>
        /// Este método é utilizado para traduz entry modes.
        /// </summary>
        /// <param name="deList"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Modelo.EntryMode> TraduzEntryModes(List<Redecard.PN.FMS.Servico.Modelo.EntryMode> deList)
        {
            if (deList == null)
            {
                return new List<Redecard.PN.FMS.Modelo.EntryMode>();
            }
            List<Redecard.PN.FMS.Modelo.EntryMode> retorno = new List<Redecard.PN.FMS.Modelo.EntryMode>();
            foreach (Redecard.PN.FMS.Servico.Modelo.EntryMode de in deList)
            {
                retorno.Add(new Redecard.PN.FMS.Modelo.EntryMode()
                {
                    Codigo = de.Codigo,
                    Descricao = de.Descricao
                });
            }
            return retorno;
        }


        #endregion
    }
}