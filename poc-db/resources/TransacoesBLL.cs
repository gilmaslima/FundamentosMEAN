using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente;

namespace Redecard.PN.FMS.Negocio
{
    /// <summary>
    /// Este componente publica a classe TransacoesBLL, consumida a partir da camada de serviços, e expõe métodos para consultar o webservice, via as classes de agentes, para expor os serviços de Transações para o FMS.
    /// </summary>
    public class TransacoesBLL
    {
        /// <summary>
        /// Este método é utilizado para pesquisar transações suspeitas por emissor e usuário logado. 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="renovarContador"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public TransacoesEmissor PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, int posicaoPrimeiroRegistro, int quantidadeRegistros, bool renovarContador,
            CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin criterio, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();
            TransacoesEmissor transacoesSuspeitasEmissor = fmsClient.PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(numeroEmissor,
                grupoEntidade, usuarioLogin, posicaoPrimeiroRegistro, quantidadeRegistros, criterio, ordem, tipoCartao);

            return transacoesSuspeitasEmissor;
        }

        /// <summary>
        /// Este método é utilizado para pesquisar transações analisadas e não analisadas por transação associada. 
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="renovarContador"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public RespostaListaTransacoes PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada(long identificadorTransacao, int numeroEmissor,
            int grupoEntidade, string usuarioLogin, int posicaoPrimeiroRegistro, int quantidadeRegistros, bool renovarContador,
            CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterio, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            RespostaListaTransacoes listaTransacoesEmissor = new RespostaListaTransacoes();

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            listaTransacoesEmissor.ListaTransacoes = fmsClient.PesquisarTransacoesAnalisadaseNaoAnalisadasPorTransacaoAssociada(identificadorTransacao, numeroEmissor,
                grupoEntidade, usuarioLogin, posicaoPrimeiroRegistro, quantidadeRegistros, criterio, ordem, tipoCartao);

            if (renovarContador)
                listaTransacoesEmissor.QuantidadeRegistros = fmsClient.ContarTransacoesAnalisadaseNaoAnalisadasPorTransacaoAssociada(identificadorTransacao, numeroEmissor,
                grupoEntidade, usuarioLogin, tipoCartao);

            return listaTransacoesEmissor;
        }

        /// <summary>
        /// Este método é utilizado para pesquisar e bloquear transações por transação associada. 
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tempoBloqueio"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> PesquisarEBloquearTransacoesPorTransacaoAssociada(long identificadorTransacao,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int tempoBloqueio, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterio,
            OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            List<TransacaoEmissor> listaTransacoesEmissor;

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            listaTransacoesEmissor = fmsClient.PesquisarTransacoesPorTransacaoAssociada(identificadorTransacao,
            numeroEmissor, grupoEntidade, usuarioLogin, tempoBloqueio, tipoCartao, criterio, ordem);

            return listaTransacoesEmissor;
        }
        /// <summary>
        /// Este método é utilizado para pesquisar transações por número de cartão. 
        /// </summary>
        /// <param name="numeroCartao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tempoBloqueio"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public RespostaListaTransacoesEmissor PesquisarTransacoesPorNumeroCartaoEEstabelecimento(string numeroCartao, 
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int tempoBloqueio, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterio,
            OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao, long numeroEstabelecimento)
        {
            RespostaListaTransacoesEmissor listaTransacoesEmissor;

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            listaTransacoesEmissor = fmsClient.PesquisarTransacoesPorNumeroCartaoEEstabelecimento(numeroCartao,
            numeroEmissor, grupoEntidade, usuarioLogin, tempoBloqueio, tipoCartao, criterio, ordem, numeroEstabelecimento);

            return listaTransacoesEmissor;
        }
        /// <summary>
        /// Este método é utilizado para pesquisar transações analisadas por usuário e período. 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="renovarContador"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public RespostaListaTransacoes PesquisarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal,
            int posicaoPrimeiroRegistro, int quantidadeRegistros, bool renovarContador, CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo criterio,
            OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            RespostaListaTransacoes listaTransacoesEmissor = new RespostaListaTransacoes();

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            listaTransacoesEmissor.ListaTransacoes = fmsClient.PesquisarTransacoesAnalisadasPorUsuarioEPeriodo(numeroEmissor,
            grupoEntidade, usuarioLogin, usuario, dataInicial, dataFinal, posicaoPrimeiroRegistro,
            quantidadeRegistros, criterio, ordem, tipoCartao);

            if (renovarContador)
                listaTransacoesEmissor.QuantidadeRegistros = fmsClient.ContarTransacoesAnalisadasPorUsuarioEPeriodo(numeroEmissor,
                    grupoEntidade, usuarioLogin, usuario, dataInicial, dataFinal, tipoCartao);

            return listaTransacoesEmissor;
        }
        /// <summary>
        /// Este método é utilizado para pesquisar transações por situacao e período. 
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
        /// <param name="renovarContador"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public RespostaListaTransacoes PesquisarTransacoesPorSituacaoEPeriodo(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal,
            int posicaoPrimeiroRegistro, int quantidadeRegistros, bool renovarContador, CriterioOrdemTransacoesPorSituacaoEPeriodo criterio,
            OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            RespostaListaTransacoes listaTransacoesEmissor = new RespostaListaTransacoes();

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            listaTransacoesEmissor.ListaTransacoes = fmsClient.PesquisarTransacoesPorSituacaoEPeriodo(numeroEmissor,
            grupoEntidade, usuarioLogin, situacao, tipoPeriodo, dataInicial, dataFinal, posicaoPrimeiroRegistro,
            quantidadeRegistros, criterio, ordem, tipoCartao);

            if (renovarContador)
                listaTransacoesEmissor.QuantidadeRegistros = fmsClient.ContarTransacoesPorSituacaoEPeriodo(numeroEmissor,
                    grupoEntidade, usuarioLogin, situacao, tipoPeriodo, dataInicial, dataFinal, tipoCartao);

            return listaTransacoesEmissor;
        }

        /// <summary>
        /// Este método é utilizado para desbloquear o cartão.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="identificadorTransacao"></param>
        public void DesbloquearCartao(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, long identificadorTransacao)
        {

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            fmsClient.DesbloquearCartao(numeroEmissor, grupoEntidade, usuarioLogin, identificadorTransacao);

        }

        /// <summary>
        /// Este método é utilizado para bloquear o cartão.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="identificadorTransacao"></param>
        /// <param name="tempoBloqueioEmSegundos"></param>
        public void BloquearCartao(int numeroEmissor, int grupoEntidade,
            string usuarioLogin, long identificadorTransacao, int tempoBloqueioEmSegundos)
        {

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            fmsClient.BloquearCartao(numeroEmissor, grupoEntidade,
            usuarioLogin, identificadorTransacao, tempoBloqueioEmSegundos);

        }

        /// <summary>
        /// Este método é utilizado para atualizar resultado da análise. 
        /// </summary>
        /// <param name="listaRespostaAnalise"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public int AtualizarResultadoAnalise(List<RespostaAnalise> listaRespostaAnalise, IndicadorTipoCartao tipoCartao)
        {

            int quantidadeRegistroAtualizados;

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            quantidadeRegistroAtualizados = fmsClient.AtualizarResultadoAnalise(listaRespostaAnalise, tipoCartao);

            return quantidadeRegistroAtualizados;

        }
        /// <summary>
        /// Este método é utilizado para descartar a sessao de pesquisa de transações suspeitas. 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="loginUsuario"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitas(int numeroEmissor, int grupoEntidade, string loginUsuario)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            fmsClient.DescartarSessaoPesquisaTransacoesSuspeitas(numeroEmissor, grupoEntidade, loginUsuario);

        }
        /// <summary>
        /// Este método é utilizado para descartar a sessao de pesquisa de transações suspeitas do estabelecimento. 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="loginUsuario"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(int numeroEmissor, int grupoEntidade, string loginUsuario)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();
            AgrupamentoTransacoesEstabelecimentoCartao arg = new AgrupamentoTransacoesEstabelecimentoCartao();

            fmsClient.DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(numeroEmissor, grupoEntidade, loginUsuario);

        }
        /// <summary>
        /// Este método é utilizado para descartar a sessao de pesquisa de transações suspeitas de estabelecimento por cartão. 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="loginUsuario"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao(int numeroEmissor, int grupoEntidade, string loginUsuario)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            fmsClient.DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao(numeroEmissor, grupoEntidade, loginUsuario);

        }
        /// <summary>
        /// Este método é utilizado para descartar sessão de pesquisa de transações suspeitas por cartão. 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="loginUsuario"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasPorCartao(int numeroEmissor, int grupoEntidade, string loginUsuario)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            fmsClient.DescartarSessaoPesquisaTransacoesSuspeitasPorCartao(numeroEmissor, grupoEntidade, loginUsuario);

        }

        #region Exportar transações analisadas por usuário e período (exportAnalysedTransactionListByUserLoginAndPeriod).
        /// <summary>
        /// Este método é utilizado para exportar transações analisadas por usuário e período. 
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
        public List<TransacaoEmissor> ExportarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            string usuario, DateTime dataInicial, DateTime dataFinal, int posicaoPrimeiroRegistro, int quantidadeRegistros,
            CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo criterio, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            TransacaoEmissor[] result = fmsClient.ExportarTransacoesAnalisadasPorUsuarioEPeriodo(numeroEmissor, grupoEntidade, usuarioLogin,
                usuario, dataInicial, dataFinal, posicaoPrimeiroRegistro, quantidadeRegistros, criterio, ordem, tipoCartao);

            return result.ToList<TransacaoEmissor>();
        }
        #endregion

        #region Exportar transações estabelecimento agrupadas por cartão (exportSuspectMerchantTransactionSummarizedByCard).
        /// <summary>
        /// Este método é utilizado para exportar transações estabelecimento agrupadas por cartão.
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
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            return fmsClient.ExportarTransacoesEstabelecimentoAgrupadasPorCartao(numeroEstabelecimento, numeroEmissor, grupoEntidade,
                usuarioLogin, tipoTransacao);
        }
        #endregion

        #region Exportar transações suspeitas por emissor e usuário (exportSuspectTransactionListByCardIssuingAgentNumberAndUserLogin).
        /// <summary>
        /// Este método é utilizado para exportar transações suspeitas por emissor e usuário logado. 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public TransacoesEmissor ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            IndicadorTipoCartao tipoCartao)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            return fmsClient.ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin(numeroEmissor, grupoEntidade, usuarioLogin, tipoCartao);
        }
        #endregion

        #region Exportar transações por situação e período (exportTransactionListByPeriodAndStatus).
        /// <summary>
        /// Este método é utilizado para exportar transações por situacao e período.
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
        public List<TransacaoEmissor> ExportarTransacoesPorSituacaoEPeriodo(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal, IndicadorTipoCartao tipoCartao)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            return fmsClient.ExportarTransacoesPorSituacaoEPeriodo(numeroEmissor, grupoEntidade, usuarioLogin, situacao, tipoPeriodo,
                dataInicial, dataFinal, tipoCartao);
        }
        #endregion

        #region Exportar transações agrupadas por cartão (exportTransactionSummarizedByCard).
        /// <summary>
        /// Este método é utilizado para  exportar transações agrupadas por cartão.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        public RespostaTransacoesPorCartao ExportarTransacoesAgrupadasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            IndicadorTipoCartao tipoTransacao)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            return fmsClient.ExportarTransacoesAgrupadasPorCartao(numeroEmissor, grupoEntidade, usuarioLogin, tipoTransacao);
        }
        #endregion

        #region Pesquisar transações estabelecimento agrupadas por cartão (findSuspectMerchantTransactionSummarizedByCard).
        /// <summary>
        /// Este método é utilizado para  pesquisar transações de estabelecimento agrupadas por cartão.
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
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            return fmsClient.PesquisarTransacoesEstabelecimentoAgrupadasPorCartao(numeroEstabelecimento, numeroEmissor, grupoEntidade,
                usuarioLogin, primeiroRegistro, quantidadeMaximaRegistros, modoClassificacao, ordem, tipoTransacao);
        }
        #endregion

        #region Pesquisar transações estabelecimento agrupadas por cartão (findSuspectMerchantTransactionSummarizedByCard).
        /// <summary>
        /// Este método é utilizado para  pesquisar transações por cartão.
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
        public RespostaTransacoesPorCartao PesquisarTransacoesPorCartao(long numeroEstabelecimento,
            int numeroEmissor, int grupoEntidade, string usuarioLogin, int primeiroRegistro, int quantidadeMaximaRegistros,
            CriterioOrdemTransacoesAgrupadasPorCartao modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoTransacao)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();
            return fmsClient.PesquisarTransacoesAgrupadasPorCartao(numeroEmissor, grupoEntidade, usuarioLogin, primeiroRegistro, quantidadeMaximaRegistros,
                 modoClassificacao, ordem, tipoTransacao);
        }
        #endregion

        #region Pesquisar transações suspeitas por emissor e usuário (findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin).
        /// <summary>
        /// Este método é utilizado para pesquisar as transações suspeitas por emissor e usuário logado.
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
        public TransacoesEmissor PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(int numeroEmissor, int grupoEntidade, string usuarioLogin,
            int posicaoPrimeiroRegistro, int quantidadeMaximaRegistros, CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin criterio,
            OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            return fmsClient.PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(numeroEmissor, grupoEntidade, usuarioLogin,
                posicaoPrimeiroRegistro, quantidadeMaximaRegistros, criterio, ordem, tipoCartao);
        }
        #endregion

        #region Pesquisar transações agrupadas por estabelecimento (findSuspectTransactionSummarizedByMerchant).
        /// <summary>
        /// Este método é utilizado para pesquisar as transações agrupadas por estabelecimento. 
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
        public RespostaTransacoesEstabelecimento PesquisarTransacoesAgrupadasPorEstabelecimento(int numeroEmissor, int grupoEntidade,
            string usuarioLogin, int primeiroRegistro, int quantidadeMaximaRegistros,
            CriterioOrdemTransacoesAgrupadasPorEstabelecimento modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            return fmsClient.PesquisarTransacoesAgrupadasPorEstabelecimento(numeroEmissor, grupoEntidade, usuarioLogin, primeiroRegistro,
                quantidadeMaximaRegistros, modoClassificacao, ordem, tipoCartao);
        }
        #endregion

        #region Listar parâmetros do sistema (findSystemParameters).
        /// <summary>
        /// Este método é utilizado para listar os parâmetros do sistema.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        public ParametrosSistema ListaParametrosSistema(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            return fmsClient.ListaParametrosSistema(numeroEmissor, grupoEntidade, usuarioLogin);
        }
        #endregion

        #region Pesquisar transações por transação associada (findTransactionListByAssociatedTransaction).
        /// <summary>
        /// Este método é utilizado para pesquisar as transações por transação associada.
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
        public List<TransacaoEmissor> PesquisarTransacoesPorTransacaoAssociada(long identificadorTransacao, int numeroEmissor, int grupoEntidade,
            string usuarioLogin, int tempoBloqueio, IndicadorTipoCartao tipoCartao, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterio,
            OrdemClassificacao ordem)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            return fmsClient.PesquisarTransacoesPorTransacaoAssociada(identificadorTransacao, numeroEmissor, grupoEntidade, usuarioLogin,
                tempoBloqueio, tipoCartao, criterio, ordem);
        }
        #endregion

        #region Bloquear estabelecimento (registerMerchantInAnalysis).
        /// <summary>
        /// Este método é utilizado para bloquear estabelecimento.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="tempoLimite"></param>
        public void BloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento, int tempoLimite)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            fmsClient.BloquearEstabelecimento(numeroEmissor, grupoEntidade, usuarioLogin, numeroEstabelecimento, tempoLimite);
        }
        #endregion

        #region Desbloquear estabelecimento (releaseMerchantInAnalysis).
        /// <summary>
        /// Este método é utilizado para  desbloquear o estabelecimento.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="tempoLimite"></param>
        public void DesbloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            fmsClient.DesbloquearEstabelecimento(numeroEmissor, grupoEntidade, usuarioLogin, numeroEstabelecimento);
        }
        #endregion
    }
}
