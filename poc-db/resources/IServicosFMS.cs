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
using Redecard.PN.FMS.Comum;
using Redecard.PN.FMS.Comum.Log;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente.ServicoFMS;
using Redecard.PN.FMS.Agente.Tradutores;
using System.ServiceModel;

namespace Redecard.PN.FMS.Agente
{
    /// <summary>
    /// Essa interface define as classes usadas pelo serviço FMS
    /// </summary>
    public interface IServicosFMS
    {
    
        /// <summary>
        /// Este método é utilizado para pesquisar transações analisadas e não analisadas por transação associada
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        List<TransacaoEmissor> PesquisarTransacoesAnalisadaseNaoAnalisadasPorTransacaoAssociada(long identificadorTransacao,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int posicaoPrimeiroRegistro, int quantidadeRegistros,
            CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterio, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao);
        /// <summary>
        ///  Este método é utilizado para contar o número de transações analisadase e não analisadas por transação associada.
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        long ContarTransacoesAnalisadaseNaoAnalisadasPorTransacaoAssociada(long identificadorTransacao,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoCartao);
        /// <summary>
        /// Este método é utilizado para pesquisar transações analisadas por usuário e período
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
        List<TransacaoEmissor> PesquisarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal,
            int posicaoPrimeiroRegistro, int quantidadeRegistros, CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo criterio,
            OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao);
        /// <summary>
        /// Este método é utilizado para contar transações analisadas por usuário e período
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        long ContarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal, IndicadorTipoCartao tipoCartao);
        /// <summary>
        /// Este método é utilizado para pesquisar transações por situação e período
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
        /// <param name="criterioOrdem"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        List<TransacaoEmissor> PesquisarTransacoesPorSituacaoEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal,
            int posicaoPrimeiroRegistro, int quantidadeRegistros, CriterioOrdemTransacoesPorSituacaoEPeriodo criterioOrdem, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao);
        /// <summary>
        /// Este método é utilizado para contar transações por situaçãoo e período
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
        long ContarTransacoesPorSituacaoEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal,
            IndicadorTipoCartao tipoCartao);
        /// <summary>
        /// Este método é utilizado para pesquisar transacoes suspeitas por emissor no login do usuario.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        TransacoesEmissor PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, int posicaoPrimeiroRegistro, int quantidadeMaximaRegistros,
            CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin criterio, OrdemClassificacao ordem,
             IndicadorTipoCartao tipoCartao);
        /// <summary>
        /// Este método é utilizado para pesquisar transações por transação associada.
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tempoBloqueio"></param>
        /// <param name="tipoCartao"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        List<TransacaoEmissor> PesquisarTransacoesPorTransacaoAssociada(long identificadorTransacao,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int tempoBloqueio, IndicadorTipoCartao tipoCartao,
            CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterio, OrdemClassificacao ordem);
        /// <summary>
        /// Este método é utilizado para pesquisar transações por número de cartão.
        /// </summary>
        /// <param name="numeroCartao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tempoBloqueio"></param>
        /// <param name="tipoCartao"></param>
        /// <param name="criterioOrdem"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        RespostaListaTransacoesEmissor PesquisarTransacoesPorNumeroCartaoEEstabelecimento(string numeroCartao,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int tempoBloqueio, IndicadorTipoCartao tipoCartao,
            CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterioOrdem, OrdemClassificacao ordem, long numeroEstabelecimento);
        /// <summary>
        /// Este método é utilizado para  pesquisar usuários por emissor
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        string[] PesquisarUsuariosPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin);
        /// <summary>
        /// Este método é utilizado para atualizar resultado da análise
        /// </summary>
        /// <param name="listaRespostaAnalise"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        int AtualizarResultadoAnalise(List<RespostaAnalise> listaRespostaAnalise, IndicadorTipoCartao tipoCartao);
        /// <summary>
        /// Este método é utilizado para pesquisar lista de tipos de resposta
        /// </summary>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <returns></returns>
        List<TipoResposta> PesquisarListaTiposResposta(string usuarioLogin, int numeroEmissor,
        int grupoEntidade);
        /// <summary>
        /// Este método é utilizado para  bloquear o cartao.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="identificadorTransacao"></param>
        /// <param name="tempoBloqueioEmSegundos"></param>
        void BloquearCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            long identificadorTransacao, int tempoBloqueioEmSegundos);
        /// <summary>
        /// Este método é utilizado para desbloquear o cartao.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="identificadorTransacao"></param>
        void DesbloquearCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin, long identificadorTransacao);
        /// <summary>
        /// Este método é utilizado para pesquisar um relatório de produtividade por analista.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        RelatorioProdutividadePorAnalista PesquisarRelatorioProdutividadePorAnalista(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal,
            CriterioOrdemProdutividade criterio, OrdemClassificacao ordem);
        /// <summary>
        /// Este método é utilizado para pesquisar o relatório de produtividade por data.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        RelatorioProdutividadePorData PesquisarRelatorioProdutividadePorData(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal,
            CriterioOrdemProdutividade criterio, OrdemClassificacao ordem);
        /// <summary>
        /// Este método é utilizado para listar parâmetros do sistema.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        ParametrosSistema ListaParametrosSistema(int numeroEmissor, int grupoEntidade,
            string usuarioLogin);
        /// <summary>
        /// Este método é utilizado para pesquisar range bin por emissor.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="ica"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistro"></param>
        /// <returns></returns>
        List<FaixaBin> PesquisarRangeBinPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            long ica, int posicaoPrimeiroRegistro, int quantidadeMaximaRegistro);
        /// <summary>
        /// Este método é utilizado para contar range bin por emissor.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="ica"></param>
        /// <returns></returns>
        long ContarRangeBinPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            long ica);
        /// <summary>
        /// Este método é utilizado para pesquisar registros de merchant category code.
        /// </summary>
        /// <param name="codigoMCC"></param>
        /// <param name="descricaoMCC"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <returns></returns>
        List<MCC> PesquisarListaMCC(long? codigoMCC, string descricaoMCC,
            int posicaoPrimeiroRegistro, int quantidadeMaximaRegistros);
        /// <summary>
        /// Este método é utilizado para contar registros de merchant category code.
        /// </summary>
        /// <param name="codigoMCC"></param>
        /// <param name="descricaoMCC"></param>
        /// <returns></returns>
        long ContarRegistrosMCC(long? codigoMCC, string descricaoMCC);
        /// <summary>
        /// Este método é utilizado para pesquisar critérios seleção por usuario logado.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        CriteriosSelecao PesquisarCriteriosSelecaoPorUsuarioLogin(int numeroEmissor,
           int grupoEntidade, string usuarioLogin, string usuario);
        /// <summary>
        /// Este método é utilizado para atualizar critérios de seleção.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="criterio"></param>
        void AtualizarCriteriosSelecao(int numeroEmissor, string usuarioLogin, int grupoEntidade, CriteriosSelecao criterio);
        /// <summary>
        /// Este método é utilizado para pesquisar transações estabelecimento agrupadas por cartão.
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
        RespostaTransacoesEstabelecimentoPorCartao PesquisarTransacoesEstabelecimentoAgrupadasPorCartao(long numeroEstabelecimento,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int primeiroRegistro, int quantidadeMaximaRegistros,
            CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoTransacao);
        /// <summary>
        /// Este método é utilizado para pesquisar transações agrupadas por cartão.
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
        RespostaTransacoesPorCartao PesquisarTransacoesAgrupadasPorCartao
            (int numeroEmissor, int grupoEntidade, string usuarioLogin, int primeiroRegistro,int quantidadeMaximaRegistros, CriterioOrdemTransacoesAgrupadasPorCartao modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoTransacao);
        /// <summary>
        /// Este método é utilizado para exportar transações estabelecimento agrupadas por cartão.
        /// </summary>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        RespostaTransacoesEstabelecimentoPorCartao ExportarTransacoesEstabelecimentoAgrupadasPorCartao
            (long numeroEstabelecimento, int numeroEmissor, int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoTransacao);
        /// <summary>
        /// Este método é utilizado para exportar transações suspeitas por emissor e usuário logado.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        TransacoesEmissor ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoCartao);
        /// <summary>
        /// Este método é utilizado para exportar transações agrupadas por cartão.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        RespostaTransacoesPorCartao ExportarTransacoesAgrupadasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoTransacao);
        /// <summary>
        /// Este método é utilizado para exportar transacoes analisadas por usuario e periodo.
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
        TransacaoEmissor[] ExportarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor, int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal,
            int posicaoPrimeiroRegistro, int quantidadeRegistros, CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo criterio,
            OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao);
        /// <summary>
        /// Este método é utilizado para pesquisar transações agrupadas por estabelecimento.
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
        RespostaTransacoesEstabelecimento PesquisarTransacoesAgrupadasPorEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            int primeiroRegistro, int quantidadeMaximaRegistros, CriterioOrdemTransacoesAgrupadasPorEstabelecimento modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao);
        /// <summary>
        /// Este método é utilizado para descartar sessao pesquisa transacoes suspeitas por cartão.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        void DescartarSessaoPesquisaTransacoesSuspeitasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin);
        /// <summary>
        /// Este método é utilizado para descartar a sessao da pesquisa transações suspeitas por estabelecimento por cartao.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin);
        /// <summary>
        /// Este método é utilizado para descartar a sessão da pesquisa transações suspeitas por estabelecimento.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin);
        /// <summary>
        /// Este método é utilizado para descartar sessão da pesquisa de transações suspeitas.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        void DescartarSessaoPesquisaTransacoesSuspeitas(int numeroEmissor, int grupoEntidade, string usuarioLogin);
        /// <summary>
        /// Este método é utilizado para bloquear o estabelecimento.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="tempoLimite"></param>
        void BloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento, int tempoLimite);
        /// <summary>
        /// Este método é utilizado para desbloquear o estabelecimento
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="tempoLimite"></param>
        void DesbloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento);
        /// <summary>
        /// Este método é utilizado para exportar transações por situação e período
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
        List<TransacaoEmissor> ExportarTransacoesPorSituacaoEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal,
            IndicadorTipoCartao tipoCartao);
    }
}
