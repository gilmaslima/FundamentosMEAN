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
using System.ServiceModel;
using System.Text;
using Redecard.PN.FMS.Agente.Helpers;
using Redecard.PN.FMS.Agente.ServicoFMS;
using Redecard.PN.FMS.Agente.Tradutores;
using Redecard.PN.FMS.Comum;
using Redecard.PN.FMS.Comum.Log;
using Redecard.PN.FMS.Modelo;

namespace Redecard.PN.FMS.Agente
{
    public class ServicosFMSAg : IServicosFMS
    {
        /// <summary>
        /// Executa o servico findAnalysedAndNotAnalysedTransactionListByAssociatedTransaction da CPqD
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> PesquisarTransacoesAnalisadaseNaoAnalisadasPorTransacaoAssociada(long identificadorTransacao,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int posicaoPrimeiroRegistro, int quantidadeRegistros,
            CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterio, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            try
            {
                issuerTransaction[] retornoWS;
                List<TransacaoEmissor> listaTransacoes = new List<TransacaoEmissor>();

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findAnalysedAndNotAnalysedTransactionListByAssociatedTransaction");

                    retornoWS = wsClient.findAnalysedAndNotAnalysedTransactionListByAssociatedTransaction(identificadorTransacao,
                        numeroEmissor, grupoEntidade, usuarioLogin, posicaoPrimeiroRegistro, quantidadeRegistros,
                        EnumHelper.EnumToEnum<CriterioOrdemTransacoesPorNumeroCartaoOuAssociada, analysedAndNotAnalysedTransactionListByAssociatedTransactionClassifiedBy>(criterio),
                        EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem),
                        EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findAnalysedAndNotAnalysedTransactionListByAssociatedTransaction", retornoWS);
                }

                listaTransacoes = new List<TransacaoEmissor>();

                if (retornoWS != null)
                    listaTransacoes.AddRange(Array.ConvertAll<issuerTransaction, TransacaoEmissor>(retornoWS, TranslatorTransacaoEmissor.TranslateIssuerTransactionToTransacaoEmissorBusiness));

                return listaTransacoes;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico countAllAnalysedAndNotAnalysedTransactionListByAssociatedTransaction da CPqD
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public long ContarTransacoesAnalisadaseNaoAnalisadasPorTransacaoAssociada(long identificadorTransacao,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoCartao)
        {
            try
            {
                long qtdeRegistros;

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo countAllAnalysedAndNotAnalysedTransactionListByAssociatedTransaction");

                    qtdeRegistros = wsClient.countAllAnalysedAndNotAnalysedTransactionListByAssociatedTransaction(identificadorTransacao,
                        numeroEmissor, grupoEntidade, usuarioLogin, EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo countAllAnalysedAndNotAnalysedTransactionListByAssociatedTransaction", qtdeRegistros);
                }

                return qtdeRegistros;

            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return 0;
            }
        }

        /// <summary>
        /// Executa o servico findAnalysedTransactionListByUserLoginAndPeriod da CPqD
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> PesquisarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal,
            int posicaoPrimeiroRegistro, int quantidadeRegistros, CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo criterio,
            OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            issuerTransaction[] retornoWS;
            List<TransacaoEmissor> listaTransacoes = new List<TransacaoEmissor>();

            try
            {

                if (string.IsNullOrEmpty(usuario))
                    usuario = null;

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findAnalysedTransactionListByUserLoginAndPeriod");

                    retornoWS = wsClient.findAnalysedTransactionListByUserLoginAndPeriod(numeroEmissor,
                        grupoEntidade, usuarioLogin, usuario, dataInicial, dataFinal, posicaoPrimeiroRegistro, quantidadeRegistros,
                        EnumHelper.EnumToEnum<CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo, analysedTransactionListByUserLoginAndPeriodClassifiedBy>(criterio),
                        EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem),
                        EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findAnalysedTransactionListByUserLoginAndPeriod", retornoWS);
                }

                listaTransacoes = new List<TransacaoEmissor>();
                if (retornoWS != null)
                    listaTransacoes.AddRange(Array.ConvertAll<issuerTransaction, TransacaoEmissor>(retornoWS, TranslatorTransacaoEmissor.TranslateIssuerTransactionToTransacaoEmissorBusiness));

                return listaTransacoes;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico countAllAnalysedTransactionListByUserLoginAndPeriod da CPqD
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public long ContarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal, IndicadorTipoCartao tipoCartao)
        {
            long qtdeRegistros;

            if (string.IsNullOrEmpty(usuario))
                usuario = null;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo countAllAnalysedTransactionListByUserLoginAndPeriod");

                    qtdeRegistros = wsClient.countAllAnalysedTransactionListByUserLoginAndPeriod(numeroEmissor,
                        grupoEntidade, usuarioLogin, usuario, dataInicial, dataFinal, EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo countAllAnalysedTransactionListByUserLoginAndPeriod", qtdeRegistros);

                }

                return qtdeRegistros;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return 0;
            }
        }

        /// <summary>
        /// Executa o metodo findTransactionListByPeriodAndStatus
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipoPeriodo"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> PesquisarTransacoesPorSituacaoEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal,
            int posicaoPrimeiroRegistro, int quantidadeRegistros, CriterioOrdemTransacoesPorSituacaoEPeriodo criterioOrdem, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            issuerTransaction[] retornoWS;
            List<TransacaoEmissor> listaTransacoes = new List<TransacaoEmissor>();

            try
            {
                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execução do método findTransactionListByPeriodAndStatus.");

                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: numeroEmissor:", numeroEmissor);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: grupoEntidade:", grupoEntidade);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: usuarioLogin:", usuarioLogin);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: situacaoAnalisePesquisa:", situacao);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: tipoPeriodo:", tipoPeriodo);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: dataInicial:", dataInicial);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: dataFinal:", dataFinal);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: posicaoPrimeiroRegistro:", posicaoPrimeiroRegistro);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: quantidadeRegistros:", quantidadeRegistros);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: indicadorTipoCartao:", tipoCartao);

                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: ordenação - campo:", criterioOrdem);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: ordenação - ordem:", ordem);

                    transactionListByPeriodAndStatusClassifiedBy c = EnumHelper.EnumToEnum<CriterioOrdemTransacoesPorSituacaoEPeriodo, transactionListByPeriodAndStatusClassifiedBy>(criterioOrdem);
                    sortOrderBy o = EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem);

                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: ordenação - campo enviado para o serviço:", c);
                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: ordenação - ordem enviada para o serviço:", o);

                    retornoWS = wsClient.findTransactionListByPeriodAndStatus(numeroEmissor,
                        grupoEntidade, usuarioLogin, (int)situacao, (int)tipoPeriodo, dataInicial, dataFinal, posicaoPrimeiroRegistro,
                        quantidadeRegistros, c,
                        o, EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    LogHelper.GravarLogIntegracao("findTransactionListByPeriodAndStatus: quantidadeTotalRegistrosRetornadoPelaCPQD:", retornoWS.Length);

                    LogHelper.GravarLogIntegracao("Retorno da execução do método findTransactionListByPeriodAndStatus.", retornoWS);
                }

                if (retornoWS != null)
                    listaTransacoes.AddRange(Array.ConvertAll<issuerTransaction, TransacaoEmissor>(retornoWS, TranslatorTransacaoEmissor.TranslateIssuerTransactionToTransacaoEmissorBusiness));

                return listaTransacoes;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o metodo countAllTransactionListByPeriodAndStatus
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipoPeriodo"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public long ContarTransacoesPorSituacaoEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal,
            IndicadorTipoCartao tipoCartao)
        {
            long qtdeRegistros = 0;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo countAllTransactionListByPeriodAndStatus");

                    qtdeRegistros = wsClient.countAllTransactionListByPeriodAndStatus(numeroEmissor,
                        grupoEntidade, usuarioLogin, (int)situacao, (int)tipoPeriodo, dataInicial, dataFinal, EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo countAllTransactionListByPeriodAndStatus", qtdeRegistros);
                }
                return qtdeRegistros;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return 0;
            }
        }

        /// <summary>
        /// Executa o servico findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin da CPqD 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public TransacoesEmissor PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, int posicaoPrimeiroRegistro, int quantidadeMaximaRegistros,
            CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin criterio, OrdemClassificacao ordem,
             IndicadorTipoCartao tipoCartao)
        {

            TransacoesEmissor transacoesEmissor = null;

            try
            {
                findCountComposite retornoWS = null;

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin.");
                    LogHelper.GravarLogIntegracao("findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin: Ordenação - Campo:", criterio);
                    LogHelper.GravarLogIntegracao("findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin: Ordenação - Ordem:", ordem);

                    suspectTransactionClassifiedBy c = EnumHelper.EnumToEnum<CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin, suspectTransactionClassifiedBy>(criterio);
                    sortOrderBy o = EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem);

                    LogHelper.GravarLogIntegracao("findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin: Ordenação - Campo enviado para o serviço:", c);
                    LogHelper.GravarLogIntegracao("findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin: Ordenação - Ordem enviada para o serviço:", o);

                    retornoWS = wsClient.findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin(numeroEmissor, grupoEntidade,
                        usuarioLogin, posicaoPrimeiroRegistro, quantidadeMaximaRegistros,
                        c,
                        o,
                        EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin.", retornoWS);

                }

                if (retornoWS != null)
                    transacoesEmissor = Tradutores.TranslatorRetornoTransacoesEmissor.TranslateWSFindCountCompositeToTipoRespostaListaEmissorBusiness(retornoWS);

                return transacoesEmissor;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico findTransactionListByAssociatedTransaction da CPqD 
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tempoBloqueio"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> PesquisarTransacoesPorTransacaoAssociada(long identificadorTransacao,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int tempoBloqueio, IndicadorTipoCartao tipoCartao,
            CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterio, OrdemClassificacao ordem)
        {
            issuerTransaction[] retornoWS;
            List<TransacaoEmissor> listaEmissor = new List<TransacaoEmissor>();

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findTransactionListByAssociatedTransaction");

                    retornoWS = wsClient.findTransactionListByAssociatedTransaction(identificadorTransacao, numeroEmissor,
                        grupoEntidade, usuarioLogin, tempoBloqueio,
                        EnumHelper.EnumToEnum<CriterioOrdemTransacoesPorNumeroCartaoOuAssociada, transactionListByCardAccountNumberOrAssociatedTransactionClassifiedBy>(criterio),
                        EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem),
                        EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    //intervalo.FinalizarTimerERegistrar(ContadoresPerformance.ContadorFindTransactionListByAssociatedTransaction, ContadoresPerformance.ContadorFindTransactionListByAssociatedTransactionBase);
                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findTransactionListByAssociatedTransaction", retornoWS);
                }

                if (retornoWS != null)
                    listaEmissor.AddRange(Array.ConvertAll<issuerTransaction, TransacaoEmissor>(retornoWS, TranslatorTransacaoEmissor.TranslateIssuerTransactionToTransacaoEmissorBusiness));

                return listaEmissor;

            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico findTransactionListByCardAccountNumber da CPqD 
        /// </summary>
        /// <param name="numeroCartao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tempoBloqueio"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public RespostaListaTransacoesEmissor PesquisarTransacoesPorNumeroCartaoEEstabelecimento(string numeroCartao,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int tempoBloqueio, IndicadorTipoCartao tipoCartao,
            CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterioOrdem, OrdemClassificacao ordem, long numeroEstabelecimento)
        {
            issuerTransactionComposite respostaWS;
            RespostaListaTransacoesEmissor respostaTransacoesEmissor = null;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findTransactionListByCardAccountNumber");
                    long? numeroEstabelecimentoTratado = null;

                    if (numeroEstabelecimento == 0)
                        numeroEstabelecimentoTratado = null;
                    else
                        numeroEstabelecimentoTratado = numeroEstabelecimento;

                    respostaWS = wsClient.findTransactionListByCardAccountNumber(numeroCartao, numeroEmissor, grupoEntidade,
                        usuarioLogin, tempoBloqueio,
                        EnumHelper.EnumToEnum<CriterioOrdemTransacoesPorNumeroCartaoOuAssociada, transactionListByCardAccountNumberOrAssociatedTransactionClassifiedBy>(criterioOrdem),
                        EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem),
                        EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao),
                        numeroEstabelecimentoTratado);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findTransactionListByCardAccountNumber", respostaWS);

                }

                if (respostaWS != null)
                {
                    respostaTransacoesEmissor = TranslatorRespostaListaTransacoesEmissor.TranslateTipoRespostaListaEmissorWSTipoRespostaListaEmissorBusiness(respostaWS);
                }

                switch (respostaTransacoesEmissor.TipoRespostaListaEmissor)
                {
                    case TipoRespostaListaEmissor.CartaoEmAnalisePorOutroUsuario:
                        {
                            throw new FMSException(TipoExcecaoServico.LockException, "Cartao em analise por outro usuario");
                        }
                    case TipoRespostaListaEmissor.CartaoJaAnalisado:
                        {
                            throw new FMSComCampoException(TipoExcecaoServico.Outros, "Cartao ja analisado", 22);
                        }
                }

                return respostaTransacoesEmissor;

            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico findUsersByCardIssuingAgent da CPqD
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        public string[] PesquisarUsuariosPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            string[] retornoWS;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findUsersByCardIssuingAgent");

                    retornoWS = wsClient.findUsersByCardIssuingAgent(numeroEmissor, grupoEntidade, usuarioLogin);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findUsersByCardIssuingAgent", retornoWS);
                }

                return retornoWS;

            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico analyzeTransactionList
        /// </summary>
        /// <param name="listaRespostaAnalise"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public int AtualizarResultadoAnalise(List<RespostaAnalise> listaRespostaAnalise, IndicadorTipoCartao tipoCartao)
        {
            int quantidadeRegistroAtualizados;
            fraudComposite[] fraud;

            try
            {

                fraud = Array.ConvertAll<RespostaAnalise, fraudComposite>(listaRespostaAnalise.ToArray<RespostaAnalise>(), TranslatorRespostaAnalise.TranslateRespostaAnaliseBusinessRespostaAnaliseWS);

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo analyzeTransactionList", fraud);

                    quantidadeRegistroAtualizados = wsClient.analyzeTransactionList(fraud, EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo analyzeTransactionList", quantidadeRegistroAtualizados);
                }

                return quantidadeRegistroAtualizados;

            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return 0;
            }
        }

        /// <summary>
        /// Executa o servico findIssuerResponseTypeList
        /// </summary>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <returns></returns>
        public List<TipoResposta> PesquisarListaTiposResposta(string usuarioLogin, int numeroEmissor,
        int grupoEntidade)
        {
            issuerResponseType[] retornoWS;
            List<TipoResposta> tipoResposta = new List<TipoResposta>();

            try
            {
                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findIssuerResponseTypeList");

                    retornoWS = wsClient.findIssuerResponseTypeList(usuarioLogin, numeroEmissor, grupoEntidade);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findIssuerResponseTypeList", retornoWS);
                }

                if (retornoWS != null)
                    tipoResposta.AddRange(Array.ConvertAll<issuerResponseType, TipoResposta>(retornoWS, TranslatorTipoResposta.TranslateIssuerResponseTypeToTipoRespostaBusiness));

                return tipoResposta;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico @lock da CPqD
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="identificadorTransacao"></param>
        /// <param name="tempoBloqueioEmSegundos"></param>
        public void BloquearCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            long identificadorTransacao, int tempoBloqueioEmSegundos)
        {

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo @lock");

                    wsClient.@lock(numeroEmissor, grupoEntidade, usuarioLogin, identificadorTransacao, tempoBloqueioEmSegundos);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo @lock");
                }
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
            }
        }

        /// <summary>
        /// Executa o servico releaseLock
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="identificadorTransacao"></param>
        public void DesbloquearCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin, long identificadorTransacao)
        {
            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo releaseLock");

                    wsClient.releaseLock(numeroEmissor, grupoEntidade, usuarioLogin, identificadorTransacao);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo releaseLock");
                }
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
            }
        }

        /// <summary>
        /// Executa o servico findAnalystProductivityGroupedByAnalyst
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        public RelatorioProdutividadePorAnalista PesquisarRelatorioProdutividadePorAnalista(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal,
            CriterioOrdemProdutividade criterio, OrdemClassificacao ordem)
        {
            analystReport retornoWS;

            RelatorioProdutividadePorAnalista relatorioProdutividadeAnalista = null;

            if (usuario == "")
                usuario = null;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findAnalystProductivityGroupedByAnalyst");

                    retornoWS = wsClient.findAnalystProductivityGroupedByAnalyst(numeroEmissor, grupoEntidade,
                        usuarioLogin, usuario, dataInicial, dataFinal, EnumHelper.EnumToEnum<CriterioOrdemProdutividade, analystProductivityClassifiedBy>(criterio),
                        EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem));

                    if (retornoWS != null)
                    {
                        relatorioProdutividadeAnalista = TranslatorRelatorioProdutividadePorAnalista.TranslateAnalystReportToRelatorioProdutividadePorAnalistaBusiness(retornoWS);
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findAnalystProductivityGroupedByAnalyst", retornoWS);
                    }
                    else
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findAnalystProductivityGroupedByAnalyst - Não há dados");

                }

                return relatorioProdutividadeAnalista;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico findAnalystProductivityGroupedByDate
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        public RelatorioProdutividadePorData PesquisarRelatorioProdutividadePorData(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal,
            CriterioOrdemProdutividade criterio, OrdemClassificacao ordem)
        {
            dateReport retornoWS;
            RelatorioProdutividadePorData relatorioProdutividadeData = null;

            if (usuario == "")
                usuario = null;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findAnalystProductivityGroupedByDate");

                    retornoWS = wsClient.findAnalystProductivityGroupedByDate(numeroEmissor, grupoEntidade,
                        usuarioLogin, usuario, dataInicial, dataFinal, EnumHelper.EnumToEnum<CriterioOrdemProdutividade, analystProductivityClassifiedBy>(criterio),
                        EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem));

                    if (retornoWS != null)
                    {
                        relatorioProdutividadeData = TranslatorRelatorioProdutividadePorData.TranslateRelatorioProdutividadePorDataWSRelatorioProdutividadePorDataBusiness(retornoWS);
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findAnalystProductivityGroupedByDate", retornoWS);
                    }
                    else
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findAnalystProductivityGroupedByDate - Não há dados");
                }

                return relatorioProdutividadeData;

            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico findSystemParameters da CPqD 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        public ParametrosSistema ListaParametrosSistema(int numeroEmissor, int grupoEntidade,
            string usuarioLogin)
        {

            try
            {
                systemParameterComposite retornoWS;
                ParametrosSistema parametros = new ParametrosSistema();

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findSystemParameters");

                    retornoWS = wsClient.findSystemParameters(numeroEmissor, grupoEntidade, usuarioLogin);
                    wsClient.Close();
                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findSystemParameters", retornoWS);

                }

                if (retornoWS != null)
                    parametros = TranslatorParametroSistema.TranslateParametrosSistemaWSParametrosSistemaBusiness(retornoWS);

                return parametros;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o metodo findRangeBinByCardIssuingAgent
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="ica"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistro"></param>
        /// <returns></returns>
        public List<FaixaBin> PesquisarRangeBinPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            long ica, int posicaoPrimeiroRegistro, int quantidadeMaximaRegistro)
        {
            issuerRangeBin[] retornoWS;
            List<FaixaBin> listaFaixaBin = new List<FaixaBin>();

            try
            {
                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findRangeBinByCardIssuingAgent");

                    retornoWS = wsClient.findRangeBinByCardIssuingAgent(numeroEmissor, grupoEntidade, usuarioLogin, ica, posicaoPrimeiroRegistro, quantidadeMaximaRegistro);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findRangeBinByCardIssuingAgent", retornoWS);
                }

                if (retornoWS != null)
                    listaFaixaBin.AddRange(Array.ConvertAll<issuerRangeBin, FaixaBin>(retornoWS, TranslatorFaixaBin.TranslateFaixaBinWSFaixaBinBusiness));

                return listaFaixaBin;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o metodo countAllRangeBinByCardIssuingAgent
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="ica"></param>
        /// <returns></returns>
        public long ContarRangeBinPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            long ica)
        {
            long qtdeRegistros;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo countAllRangeBinByCardIssuingAgent");

                    qtdeRegistros = wsClient.countAllRangeBinByCardIssuingAgent(numeroEmissor, grupoEntidade, usuarioLogin, ica);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo countAllRangeBinByCardIssuingAgent", qtdeRegistros);
                }

                return qtdeRegistros;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return 0;
            }
        }

        /// <summary>
        /// Executa o servico findMerchantCategoryCodeList
        /// </summary>
        /// <param name="codigoMCC"></param>
        /// <param name="descricaoMCC"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <returns></returns>
        public List<MCC> PesquisarListaMCC(long? codigoMCC, string descricaoMCC,
            int posicaoPrimeiroRegistro, int quantidadeMaximaRegistros)
        {
            issuerMCC[] retornoWS;
            List<MCC> listaMCC = new List<MCC>();

            try
            {
                if (descricaoMCC.Trim() == "")
                    descricaoMCC = null;

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findMerchantCategoryCodeList");

                    retornoWS = wsClient.findMerchantCategoryCodeList(codigoMCC, descricaoMCC, posicaoPrimeiroRegistro, quantidadeMaximaRegistros);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findMerchantCategoryCodeList", retornoWS);
                }

                if (retornoWS != null)
                    listaMCC.AddRange(Array.ConvertAll<issuerMCC, MCC>(retornoWS, TranslatorMCC.TranslateMCCWSMCCBusiness));

                return listaMCC;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico countAllMerchantCategoryCodeList
        /// </summary>
        /// <param name="codigoMCC"></param>
        /// <param name="descricaoMCC"></param>
        /// <returns></returns>
        public long ContarRegistrosMCC(long? codigoMCC, string descricaoMCC)
        {

            long qtdeRegistros;

            try
            {
                long tmpCodigoMCC;

                if (codigoMCC == null)
                {
                    tmpCodigoMCC = 0;
                }
                else
                {
                    tmpCodigoMCC = codigoMCC.Value;
                }

                if (descricaoMCC.Trim() == "")
                    descricaoMCC = null;

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo countAllMerchantCategoryCodeList");

                    qtdeRegistros = wsClient.countAllMerchantCategoryCodeList(tmpCodigoMCC, descricaoMCC);

                    //intervalo.FinalizarTimerERegistrar(ContadoresPerformance.ContadorFindMerchantCategoryCodeList, ContadoresPerformance.ContadorFindMerchantCategoryCodeListBase);
                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo countAllMerchantCategoryCodeList", qtdeRegistros);
                }
                return qtdeRegistros;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return 0;
            }
        }

        /// <summary>
        /// Executa o metodo findProfileCriteriaByUserLogin
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public CriteriosSelecao PesquisarCriteriosSelecaoPorUsuarioLogin(int numeroEmissor,
           int grupoEntidade, string usuarioLogin, string usuario)
        {
            profileCriteria retornoWS;
            CriteriosSelecao criterio;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findProfileCriteriaByUserLogin");

                    retornoWS = wsClient.findProfileCriteriaByUserLogin(numeroEmissor, grupoEntidade, usuarioLogin, usuario);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findProfileCriteriaByUserLogin", retornoWS);
                }

                criterio = TranslatorCriteriosSelecao.TranslateProfileCriteriaToCriterioSelecaoBusiness(retornoWS);

                return criterio;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico updateAnalystProfile
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="criterio"></param>
        public void AtualizarCriteriosSelecao(int numeroEmissor, string usuarioLogin, int grupoEntidade, CriteriosSelecao criterio)
        {
            profileCriteria profile;
            try
            {
                profile = TranslatorCriteriosSelecao.TransactionCriterioSelecaoBusinessToProfileCriteria(criterio);

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo updateAnalystProfile", profile);

                    wsClient.updateAnalystProfile(numeroEmissor, grupoEntidade, usuarioLogin, profile);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo updateAnalystProfile");

                }

            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
            }
        }

        /// <summary>
        /// Executa o servico findSuspectMerchantTransactionSummarizedByCard
        /// </summary>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <param name="modoClassificacao"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        public RespostaTransacoesEstabelecimentoPorCartao PesquisarTransacoesEstabelecimentoAgrupadasPorCartao(long numeroEstabelecimento,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int primeiroRegistro, int quantidadeMaximaRegistros,
            CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoTransacao)
        {

            merchantCardSuspectTransactionComposite retornoWS;

            RespostaTransacoesEstabelecimentoPorCartao respostaTransacoesEstabelecimentoCartao = null;

            try
            {
                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarTraceLog("Iniciando execucao do metodo findSuspectMerchantTransactionSummarizedByCard.");
                    LogHelper.GravarLogIntegracao("findSuspectMerchantTransactionSummarizedByCard.primeiroRegistro:", primeiroRegistro);
                    LogHelper.GravarLogIntegracao("findSuspectMerchantTransactionSummarizedByCard.quantidadeMaximaRegistros:", quantidadeMaximaRegistros);

                    retornoWS = wsClient.findSuspectMerchantTransactionSummarizedByCard(numeroEstabelecimento, numeroEmissor, grupoEntidade,
                        usuarioLogin, primeiroRegistro, quantidadeMaximaRegistros, EnumHelper.EnumToEnum<CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao, cardMerchantClassifiedBy>(modoClassificacao),
                        EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem), EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoTransacao));

                    if (retornoWS != null && retornoWS.transactionList != null)
                    {
                        int quantidadeRegistrosRetornados = retornoWS.transactionList.Length;

                        LogHelper.GravarLogIntegracao("findSuspectMerchantTransactionSummarizedByCard.quantidadeItensRetornadosPelaCPQD:", quantidadeRegistrosRetornados);

                        respostaTransacoesEstabelecimentoCartao = TranslatorRespostaTransacoesEstabelecimentoPorCartao.TranslateMerchantCardSuspectTransactionCompositeToRespostaTransacoesEstabelecimentoPorCartao(retornoWS);
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findSuspectMerchantTransactionSummarizedByCard", retornoWS);
                    }
                    else
                    {
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findSuspectMerchantTransactionSummarizedByCard - Não há dados");
                    }

                }

                return respostaTransacoesEstabelecimentoCartao;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico findSuspectTransactionSummarizedByCard
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <param name="modoClassificacao"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        public RespostaTransacoesPorCartao PesquisarTransacoesAgrupadasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin, int primeiroRegistro,
            int quantidadeMaximaRegistros, CriterioOrdemTransacoesAgrupadasPorCartao modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoTransacao)
        {

            cardSuspectTransactionComposite retornoWS;

            RespostaTransacoesPorCartao respostaTransacoesEstabelecimentoCartao = null;

            try
            {
                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findSuspectTransactionSummarizedByCard.");

                    LogHelper.GravarLogIntegracao("findSuspectTransactionSummarizedByCard.primeiroRegistro:", primeiroRegistro);
                    LogHelper.GravarLogIntegracao("findSuspectTransactionSummarizedByCard.quantidadeMaximaRegistros:", quantidadeMaximaRegistros);

                    retornoWS = wsClient.findSuspectTransactionSummarizedByCard(numeroEmissor, grupoEntidade,
                        usuarioLogin, primeiroRegistro, quantidadeMaximaRegistros, EnumHelper.EnumToEnum<CriterioOrdemTransacoesAgrupadasPorCartao, cardClassifiedBy>(modoClassificacao),
                        EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem), EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoTransacao));

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findSuspectTransactionSummarizedByCard.", retornoWS);

                    if ((retornoWS != null) && (retornoWS.transactionList != null))
                    {
                        int quantidadeRegistrosRetornados = retornoWS.transactionList.Length;

                        LogHelper.GravarLogIntegracao("findSuspectTransactionSummarizedByCard.quantidadeItensRetornadosPelaCPQD:", quantidadeRegistrosRetornados);

                        respostaTransacoesEstabelecimentoCartao = TranslatorRespostaTransacoesPorCartao.TranslateCardSuspectTransactionCompositeToRespostaTransacoesPorCartao(retornoWS);
                                                
                    }
                    else
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findSuspectTransactionSummarizedByCard. - Não há dados");

                }

                return respostaTransacoesEstabelecimentoCartao;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Este método é utilizado para exportar as transações do estabelecimento agrupadas por cartão.
        /// </summary>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        public RespostaTransacoesEstabelecimentoPorCartao ExportarTransacoesEstabelecimentoAgrupadasPorCartao(long numeroEstabelecimento,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoTransacao)
        {

            merchantCardSuspectTransactionComposite retornoWS;

            RespostaTransacoesEstabelecimentoPorCartao respostaTransacoesEstabelecimentoCartao = null;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo exportSuspectMerchantTransactionSummarizedByCard");

                    retornoWS = wsClient.exportSuspectMerchantTransactionSummarizedByCard(numeroEstabelecimento, numeroEmissor, grupoEntidade,
                        usuarioLogin, EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoTransacao));

                    if (retornoWS != null)
                    {
                        respostaTransacoesEstabelecimentoCartao = TranslatorRespostaTransacoesEstabelecimentoPorCartao.TranslateMerchantCardSuspectTransactionCompositeToRespostaTransacoesEstabelecimentoPorCartao(retornoWS);
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo exportSuspectMerchantTransactionSummarizedByCard", retornoWS);
                    }
                    else
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo exportSuspectMerchantTransactionSummarizedByCard - Não há dados");

                }

                return respostaTransacoesEstabelecimentoCartao;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }
        /// <summary>
        /// Este método é utilizado para exportar transações suspeitas por emissor e usuario logado.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public TransacoesEmissor ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoCartao)
        {

            TransacoesEmissor transacoesEmissor = null;

            try
            {
                findCountComposite retornoWS = null;

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo exportSuspectTransactionListByCardIssuingAgentNumberAndUserLogin");

                    retornoWS = wsClient.exportSuspectTransactionListByCardIssuingAgentNumberAndUserLogin(numeroEmissor, grupoEntidade, usuarioLogin, EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo exportSuspectTransactionListByCardIssuingAgentNumberAndUserLogin", retornoWS);

                }

                if (retornoWS != null)
                    transacoesEmissor = Tradutores.TranslatorRetornoTransacoesEmissor.TranslateWSFindCountCompositeToTipoRespostaListaEmissorBusiness(retornoWS);

                return transacoesEmissor;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Este método é utilizado para exportar as transações agrupadas por cartão.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        public RespostaTransacoesPorCartao ExportarTransacoesAgrupadasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoTransacao)
        {

            cardSuspectTransactionComposite retornoWS;

            RespostaTransacoesPorCartao respostaTransacoesEstabelecimentoCartao = null;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findSuspectTransactionSummarizedByCard");

                    retornoWS = wsClient.exportSuspectTransactionSummarizedByCard(numeroEmissor, grupoEntidade,
                        usuarioLogin, EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoTransacao));

                    if (retornoWS != null)
                    {
                        respostaTransacoesEstabelecimentoCartao = TranslatorRespostaTransacoesPorCartao.TranslateCardSuspectTransactionCompositeToRespostaTransacoesPorCartao(retornoWS);
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo exportSuspectTransactionSummarizedByCard", retornoWS);
                    }
                    else
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo exportSuspectTransactionSummarizedByCard - Não há dados");

                }

                return respostaTransacoesEstabelecimentoCartao;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Este método é utilizado para exportar as transações analisadas por usuário e período.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public TransacaoEmissor[] ExportarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor, int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal,
            int posicaoPrimeiroRegistro, int quantidadeRegistros, CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo criterio,
            OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {

            issuerTransaction[] retornoWS;

            TransacaoEmissor[] respostaTransacoesEstabelecimentoCartao = null;
            
            try
            {

                if (string.IsNullOrEmpty(usuario))
                    usuario = null;

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo exportAnalysedTransactionListByUserLoginAndPeriod");

                    retornoWS = wsClient.exportAnalysedTransactionListByUserLoginAndPeriod(numeroEmissor, grupoEntidade,
                        usuarioLogin, usuario, dataInicial, dataFinal, EnumHelper.EnumToEnum<CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo, analysedTransactionListByUserLoginAndPeriodClassifiedBy>(criterio),
                        EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem), EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    if (retornoWS != null)
                    {
                        respostaTransacoesEstabelecimentoCartao = Array.ConvertAll<issuerTransaction, TransacaoEmissor>(retornoWS, TranslatorTransacaoEmissor.TranslateIssuerTransactionToTransacaoEmissorBusiness);
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo exportAnalysedTransactionListByUserLoginAndPeriod", retornoWS);
                    }
                    else
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo exportAnalysedTransactionListByUserLoginAndPeriod - Não há dados");

                }

                return respostaTransacoesEstabelecimentoCartao;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Este método é utilizado para pesquisar as transacões agrupadas por estabelecimento
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <param name="modoClassificacao"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public RespostaTransacoesEstabelecimento PesquisarTransacoesAgrupadasPorEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            int primeiroRegistro, int quantidadeMaximaRegistros, CriterioOrdemTransacoesAgrupadasPorEstabelecimento modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {

            merchantSuspectTransactionComposite retornoWS;

            RespostaTransacoesEstabelecimento respostaTransacoesEstabelecimento = null;

            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {
                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo findSuspectTransactionSummarizedByMerchant");

                    retornoWS = wsClient.findSuspectTransactionSummarizedByMerchant(numeroEmissor, grupoEntidade,
                        usuarioLogin, primeiroRegistro, quantidadeMaximaRegistros, EnumHelper.EnumToEnum<CriterioOrdemTransacoesAgrupadasPorEstabelecimento, merchantClassifiedBy>(modoClassificacao),
                        EnumHelper.EnumToEnum<OrdemClassificacao, sortOrderBy>(ordem), EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    if (retornoWS != null)
                    {
                        respostaTransacoesEstabelecimento = TranslatorRespostaTransacoesEstabelecimento.TranslateMerchantSuspectTransactionCompositeToRespostaTransacoesEstabelecimento(retornoWS);
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findSuspectTransactionSummarizedByMerchant", retornoWS);
                    }
                    else
                        LogHelper.GravarLogIntegracao("Retorno da execucao do metodo findSuspectTransactionSummarizedByMerchant - Não há dados");

                }

                return respostaTransacoesEstabelecimento;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Este método é utilizado para descartar a sessão de pesquisa de transações suspeitas por cartão.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo refreshCardUserSession");

                    wsClient.refreshCardUserSession(numeroEmissor, grupoEntidade, usuarioLogin);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo refreshCardUserSession");
                }
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
            }
        }

        /// <summary>
        /// Este método é utilizado para descartar a sessão da pesquisa de transações suspeitas do estabelecimento por cartao
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo refreshMerchantCardUserSession");

                    wsClient.refreshMerchantCardUserSession(numeroEmissor, grupoEntidade, usuarioLogin);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo refreshMerchantCardUserSession");
                }
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
            }
        }

        /// <summary>
        /// Este método é utilizado para descartar a sessão da pesquisa de transações suspeitas por estabelecimento.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo refreshMerchantUserSession");

                    wsClient.refreshMerchantUserSession(numeroEmissor, grupoEntidade, usuarioLogin);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo refreshMerchantUserSession");
                }
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
            }
        }

        /// <summary>
        /// Este método é utilizado para descartar a sessão da pesquisa de transações suspeitas.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitas(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo refreshUserSession");

                    wsClient.refreshUserSession(numeroEmissor, grupoEntidade, usuarioLogin);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo refreshUserSession");
                }
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
            }
        }

        /// <summary>
        /// Este método é utilizado para bloquear o estabelecimento.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="tempoLimite"></param>
        public void BloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento, int tempoLimite)
        {
            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo registerMerchantInAnalysis");

                    wsClient.registerMerchantInAnalysis(numeroEmissor, grupoEntidade, usuarioLogin, numeroEstabelecimento, tempoLimite);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo registerMerchantInAnalysis");
                }
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
            }
        }

        /// <summary>
        /// Este método é utilizado para desbloquear o estabelecimento.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEstabelecimento"></param>
        public void DesbloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento)
        {
            try
            {

                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo releaseMerchantInAnalysis");

                    wsClient.releaseMerchantInAnalysis(numeroEmissor, grupoEntidade, usuarioLogin, numeroEstabelecimento);

                    LogHelper.GravarLogIntegracao("Retorno da execucao do metodo releaseMerchantInAnalysis");
                }
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
            }
        }

        /// <summary>
        /// Executa o metodo exportTransactionListByPeriodAndStatus
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipoPeriodo"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> ExportarTransacoesPorSituacaoEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal,
            IndicadorTipoCartao tipoCartao)
        {
            issuerTransaction[] retornoWS;
            List<TransacaoEmissor> listaTransacoes = new List<TransacaoEmissor>();

            try
            {
                using (CardIssuingAgentFacadeClient wsClient = ServicoFMSFactory.ObterInstancia())
                {

                    LogHelper.GravarLogIntegracao("Iniciando execucao do metodo exportTransactionListByPeriodAndStatus.");

                    LogHelper.GravarLogIntegracao("exportTransactionListByPeriodAndStatus: numeroEmissor:", numeroEmissor);
                    LogHelper.GravarLogIntegracao("exportTransactionListByPeriodAndStatus: grupoEntidade:", grupoEntidade);
                    LogHelper.GravarLogIntegracao("exportTransactionListByPeriodAndStatus: usuarioLogin:", usuarioLogin);
                    LogHelper.GravarLogIntegracao("exportTransactionListByPeriodAndStatus: situacaoAnalisePesquisa:", situacao);
                    LogHelper.GravarLogIntegracao("exportTransactionListByPeriodAndStatus: tipoPeriodo:", tipoPeriodo);
                    LogHelper.GravarLogIntegracao("exportTransactionListByPeriodAndStatus: dataInicial:", dataInicial);
                    LogHelper.GravarLogIntegracao("exportTransactionListByPeriodAndStatus: dataFinal:", dataFinal);
                    LogHelper.GravarLogIntegracao("exportTransactionListByPeriodAndStatus: indicadorTipoCartao:", tipoCartao);

                    retornoWS = wsClient.exportTransactionListByPeriodAndStatus(numeroEmissor,
                        grupoEntidade, usuarioLogin, (int)situacao, (int)tipoPeriodo, dataInicial, dataFinal, transactionListByPeriodAndStatusClassifiedBy.DATE,
                        sortOrderBy.ASC, EnumHelper.EnumToEnum<IndicadorTipoCartao, cardTypeIndicator>(tipoCartao));

                    LogHelper.GravarLogIntegracao("exportTransactionListByPeriodAndStatus: quantidadeTotalRegistrosRetornadoPelaCPQD:", retornoWS.Length);

                    //LogHelper.GravarLogIntegracao("Retorno da execucao do metodo exportTransactionListByPeriodAndStatus", retornoWS);
                }
                if (retornoWS != null)
                    listaTransacoes.AddRange(Array.ConvertAll<issuerTransaction, TransacaoEmissor>(retornoWS, TranslatorTransacaoEmissor.TranslateIssuerTransactionToTransacaoEmissorBusiness));

                return listaTransacoes;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaServico.TratarExcecao(ex);
                return null;
            }
        }
    }
}
