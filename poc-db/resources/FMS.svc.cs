using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Redecard.PN.FMS.Comum;
using Redecard.PN.FMS.Comum.Helpers;
using Redecard.PN.FMS.Negocio;
using Redecard.PN.FMS.Servico.Helpers;
using Redecard.PN.FMS.Servico.Modelo;
using Redecard.PN.FMS.Servico.Modelo.CadastroIPs;
using Redecard.PN.FMS.Servico.Modelo.CriterioSelecao;
using Redecard.PN.FMS.Servico.Modelo.Merchant;
using Redecard.PN.FMS.Servico.Modelo.Transacoes;

namespace Redecard.PN.FMS.Servico
{
    /// <summary>
    /// - Publica o serviço 'FMS' para chamada aos serviços do webservice referentes ao FMS - PN.
    /// </summary>
    public class FMS : Redecard.PN.Comum.ServicoBase, IFMS
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    {
        public Redecard.PN.FMS.Servico.Modelo.ParametrosSistema ListaParametrosSistema(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@início WCF] ListaParametrosSistema [@@@ínicio WCF] [numeroEmissor: {0}, grupoEntidade: {1}, usuarioLogin: {2}]",
                    numeroEmissor.ToString(), grupoEntidade.ToString(), usuarioLogin));

                ParametrosSistemaBLL bll = new ParametrosSistemaBLL();

                Redecard.PN.FMS.Modelo.ParametrosSistema retornoServico = bll.ListaParametrosSistema(numeroEmissor, grupoEntidade, usuarioLogin);

                Redecard.PN.FMS.Servico.Modelo.ParametrosSistema retorno = new Redecard.PN.FMS.Servico.Modelo.ParametrosSistema();

                PropertyHelper.CopiaPropriedades<Redecard.PN.FMS.Modelo.ParametrosSistema, Redecard.PN.FMS.Servico.Modelo.ParametrosSistema>(retornoServico, ref retorno);

                LogHelper.GravarTraceLog(string.Format("[@@@fim WCF] ListaParametrosSistema [@@@fim WCF] [{0}]", SerializadorHelper.SerializarDados(retorno)));

                return retorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Método usado para pesquisar transações por número de cartão.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno PesquisarTransacoesPorNumeroCartaoEEstabelecimento(PesquisarTransacoesPorNumeroCartaoEEstabelecimentoEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesPorNumeroEEstabelecimentoCartao [@@@ínicio WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno objRetorno = new PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno();

                TransacoesBLL objNegocio = new TransacoesBLL();

                Redecard.PN.FMS.Modelo.RespostaListaTransacoesEmissor listaEmissor = objNegocio.PesquisarTransacoesPorNumeroCartaoEEstabelecimento(envio.NumeroCartao, envio.NumeroEmissor, envio.GrupoEntidade, envio.UsuarioLogin, envio.TempoBloqueio,
                        EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.CriterioOrdemTransacoesPorNumeroCartaoOuAssociada, Redecard.PN.FMS.Modelo.CriterioOrdemTransacoesPorNumeroCartaoOuAssociada>(envio.Criterio),
                        EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.OrdemClassificacao, Redecard.PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem),
                        EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.IndicadorTipoCartao, Redecard.PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoCartao),
                        envio.NumeroEstabelecimento);

                if (listaEmissor != null && listaEmissor.ListaTransacoesEmissor != null
                    && listaEmissor.ListaTransacoesEmissor.Count > 0)
                {
                    objRetorno.ListaTransacoesEmissor = Tradutor.TradutorTransacaoEmissor.TraduzirTransacaoEmissor(listaEmissor.ListaTransacoesEmissor);
                    objRetorno.SegundosRestanteBloqueio = listaEmissor.SegundosRestanteBloqueio;
                    objRetorno.TipoRespostaListaEmissor = EnumHelper.EnumToEnum<PN.FMS.Modelo.TipoRespostaListaEmissor, PN.FMS.Servico.Modelo.TipoRespostaListaEmissor>(listaEmissor.TipoRespostaListaEmissor);
                }
                else
                {
                    objRetorno.ListaTransacoesEmissor = new List<Modelo.TransacaoEmissor>();
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesPorNumeroEEstabelecimentoCartao [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa o servico analyzeTransactionList
        /// analyzeTransactionList(java.util.List<FraudComposite> fraudList, EnumTypes.CardTypeIndicator transactionType) 
        /// Este método permite confirmar uma lista de transaÃ§Ãµes como fraude ou nÃ£o.
        /// </summary>
        /// <param name="listaRespostaAnalise"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public int AtualizarResultadoAnalise(AtualizarResultadoAnaliseEnvio envio)
        {
            int retorno;

            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] AtualizarResultadoAnalise [@@@ínicio WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));
                TransacoesBLL objNegocio = new TransacoesBLL();

                retorno = objNegocio.AtualizarResultadoAnalise(Tradutor.TradutorAtualizarResultadoAnalise.TraduzirListaRespostaAnalise(envio.ListaRespostaAnalise), EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.IndicadorTipoCartao, Redecard.PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoCartao));

                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] AtualizarResultadoAnalise [@@@FIM WCF]: [{0}]", retorno));
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return 0;
            }
            return retorno;
        }
        /// <summary>
        /// Método usado para desbloquear cartão.
        /// </summary>
        /// <param name="envio"></param>
        public void DesbloquearCartao(DesbloquearCartaoEnvio envio)
        {

            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] DesbloquearCartao [@@@ínicio WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                TransacoesBLL objNegocio = new TransacoesBLL();
                objNegocio.DesbloquearCartao(envio.NumeroEmissor, envio.GrupoEntidade, envio.UsuarioLogin, envio.IdentificadorTransacao);

                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] DesbloquearCartao [@@@FIM WCF]"));

            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
            }
        }
        /// <summary>
        /// Método uasdo para bloquear o cartão
        /// </summary>
        /// <param name="envio"></param>
        public void BloquearCartao(BloquearCartaoEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog("[@@@INÍCIO WCF] BloquearCartao lock [@@@INÍCIO WCF]");

                TransacoesBLL objNegocio = new TransacoesBLL();
                int tempoBloqueioEmSegundos = 60000;//Verificar
                objNegocio.BloquearCartao(envio.NumeroEmissor, envio.GrupoEntidade, envio.UsuarioLogin, envio.IdentificadorTransacao, tempoBloqueioEmSegundos);

                LogHelper.GravarTraceLog("[@@@FIM WCF]  BloquearCartao lock  [@@@FIM WCF]");
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
            }

        }

        #region PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin tela AnalisaTransacoesSuspeitasUserControl.ascx servico da CPQD findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin
        /// <summary>
        /// PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin tela AnalisaTransacoesSuspeitasUserControl.ascx servico da CPQD findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno objRetorno = new PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno();

                TransacoesBLL objNegocio = new TransacoesBLL();
                Redecard.PN.FMS.Modelo.TransacoesEmissor retornoEMissor = objNegocio.PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(envio.NumeroEmissor, envio.GrupoEntidade, envio.UsuarioLogin, envio.PosicaoPrimeiroRegistro, envio.QuantidadeRegistros,
                        envio.RenovarContador,
                        EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin, Redecard.PN.FMS.Modelo.CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin>(envio.Criterio),
                        EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.OrdemClassificacao, Redecard.PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem),
                        EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.IndicadorTipoCartao, Redecard.PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoCartao));
                if (retornoEMissor != null && retornoEMissor.ListaTransacoesEmissor != null && retornoEMissor.ListaTransacoesEmissor.Count > 0)
                {
                    objRetorno = Tradutor.TradutorPesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin.TraduzirModeloToServicoRespostaListaTransacoesEmissor(retornoEMissor);
                }
                else if (retornoEMissor != null)
                {
                    objRetorno.ListaTransacoesEmissor = new List<PN.FMS.Servico.Modelo.TransacaoEmissor>();
                    objRetorno.QuantidadeTotalRegistros = 0;
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        #endregion


        /// <summary>
        /// Referente ao serviço do GPQD = swbr.com.cpqd.gifa.issuertransaction.domain.IssuerResponseType
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public PesquisaTipoRespostaRetorno PesquisarTiposResposta(PesquisaTipoRespostaEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTiposResposta [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                PesquisaTipoRespostaRetorno objRetorno = new PesquisaTipoRespostaRetorno();

                TipoRespostaBLL bll = new TipoRespostaBLL();

                List<Redecard.PN.FMS.Modelo.TipoResposta> listaTipoResposta = bll.PesquisarListaTiposResposta(
                    envio.UsuarioLogin,
                    envio.NumeroEmissor,
                    envio.GrupoEntidade);

                objRetorno.ListaTipoResposta = new List<TipoResposta>();
                if (listaTipoResposta != null && listaTipoResposta.Count > 0)
                {
                    objRetorno.ListaTipoResposta.AddRange(listaTipoResposta.ConvertAll<TipoResposta>(Tradutor.TradutorTipoResposta.TraduzirTipoResposta));
                }

                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTiposResposta [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        /// <summary>
        /// Método usado para pesquisar os critérios de seleção do usuário logado.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public PesquisarCriteriosSelecaoPorUsuarioLoginRetorno PesquisarCriteriosSelecaoPorUsuarioLogin(PesquisarCriteriosSelecaoPorUsuarioLoginEnvio envio)
        {
            try
            {
                SerializadorHelper.SerializarDados(envio);
                PesquisarCriteriosSelecaoPorUsuarioLoginRetorno objRetorno = new PesquisarCriteriosSelecaoPorUsuarioLoginRetorno();
                LogHelper.GravarTraceLog(string.Format("[@@@INÍCIO FMS] PesquisarCriteriosSelecaoPorUsuarioLogin [@@@INÍCIO WCF] Dados : [{0}]", SerializadorHelper.SerializarDados(envio)));

                CriterioSelecaoBLL bll = new CriterioSelecaoBLL();

                Redecard.PN.FMS.Modelo.CriteriosSelecao criterio = bll.PesquisarCriteriosSelecaoPorUsuarioLogin(envio.NumeroEmissor, envio.GrupoEntidade, envio.UsuarioLogin, envio.Usuario);
                if (criterio != null)
                {
                    objRetorno = Tradutor.TradutorPesquisarCriteriosSelecaoPorUsuarioLogin.TraduzCriteriosSelecao(criterio);
                }

                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarCriteriosSelecaoPorUsuarioLogin [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Método usado para pesquisar o Merchant por código de categoria
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public PesquisarMCCRetorno PesquisarMerchantPorCodigoCategoria(PesquisarMCCenvio envio)
        {
            try
            {
                SerializadorHelper.SerializarDados(envio);
                PesquisarMCCRetorno objRetorno = new PesquisarMCCRetorno();
                LogHelper.GravarTraceLog(string.Format("[@@@INÍCIO FMS] PesquisarMerchantPorCodigoCategoria [@@@INÍCIO WCF] Dados : [{0}]", SerializadorHelper.SerializarDados(envio)));

                MccBLL bll = new MccBLL();
                Redecard.PN.FMS.Modelo.RespostaListaMCC resposta = bll.PesquisarListaMCC(
                    envio.NumeroEmissor,
                    envio.GrupoEntidade,
                    envio.UsuarioLogin,
                    envio.CodigoMCC,
                    envio.DescricaoMCC,
                    envio.PosicaoPrimeiroRegistro,
                    envio.QuantidadeMaximaRegistros,
                    envio.RenovarContador);
                if (resposta != null && resposta.ListaMCC != null && resposta.ListaMCC.Count > 0)
                {
                    objRetorno.MCCs = Tradutor.TradutorPesquisarCriteriosSelecaoPorUsuarioLogin.TraduzMCC(resposta.ListaMCC);
                }
                else
                {
                    resposta.ListaMCC = new List<PN.FMS.Modelo.MCC>();
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarMerchantPorCodigoCategoria [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        /// <summary>
        /// Método usado para pesquisar por gange bin do emissor.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public PesquisarRangeBinPorEmissorRetorno PesquisarRangeBinPorEmissor(PesquisarRangeBinPorEmissorEnvio envio)
        {
            try
            {
                SerializadorHelper.SerializarDados(envio);
                PesquisarRangeBinPorEmissorRetorno objRetorno = new PesquisarRangeBinPorEmissorRetorno();
                LogHelper.GravarTraceLog(string.Format("[@@@INÍCIO FMS] PesquisarRangeBinPorEmissor [@@@INÍCIO WCF] Dados : [{0}]", SerializadorHelper.SerializarDados(envio)));

                RangeBinBLL bll = new RangeBinBLL();
                Redecard.PN.FMS.Modelo.RespostaListaFaixaBin r = bll.PesquisarRangeBinPorEmissor(envio.numeroEmissor, envio.grupoEntidade, envio.usuarioLogin, envio.ica, envio.posicaoPrimeiroRegistro, envio.quantidadeMaximaRegistro, envio.renovarContador);
                if (r != null && r.ListaFaixaBin != null && r.ListaFaixaBin.Count > 0)
                {
                    objRetorno.ListaFaixaBin = Tradutor.TradutorPesquisarCriteriosSelecaoPorUsuarioLogin.TraduzFaixaBin(r.ListaFaixaBin);
                }
                else
                {
                    objRetorno.ListaFaixaBin = new List<Modelo.FaixaBin>();
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarRangeBinPorEmissor [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        /// <summary>
        /// Método usado para pesquisar usuários por emissor.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        public string[] PesquisarUsuariosPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {

                LogHelper.GravarTraceLog(string.Format("[@@@INÍCIO FMS] PesquisarUsuariosPorEmissor [@@@INÍCIO WCF] Dados :"));
                UsuarioBLL bll = new UsuarioBLL();
                return bll.PesquisarUsuariosPorEmissor(numeroEmissor, grupoEntidade, usuarioLogin);
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        /// <summary>
        /// Atualiza critérios de seleção
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="retornoAtualizado"></param>
        public void AtualizarCriteriosSelecao(int numeroEmissor, string usuarioLogin, int grupoEntidade, PesquisarCriteriosSelecaoPorUsuarioLoginRetorno retornoAtualizado)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@INÍCIO FMS] AtualizarCriteriosSelecao [@@@INÍCIO WCF] Dados : {0}", retornoAtualizado));
                CriterioSelecaoBLL bll = new CriterioSelecaoBLL();
                Redecard.PN.FMS.Modelo.CriteriosSelecao criterioSelecao = Tradutor.TradutorPesquisarCriteriosSelecaoPorUsuarioLogin.TraduzDePesquisarCriteriosSelecaoPorUsuarioLoginRetornoParaCriteriosSelecao(retornoAtualizado);
                LogHelper.GravarLogIntegracao("Critério de seleção atualizado (WCF):", criterioSelecao);
                bll.AtualizarCriteriosSelecao(numeroEmissor, grupoEntidade, usuarioLogin, criterioSelecao);
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return;
            }
        }

        #region PesquisarTransacoesPorSituacaoEPeriodo tela ConsultaTransacoesPorPeriodoSituacaoUserControl.ascx servico da CPQD findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin
        /// <summary>
        /// Método de utiliza o servico da CPQD findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public TransacoesEmissorRetornoComQuantidade PesquisarTransacoesPorSituacaoEPeriodo(PesquisarTransacoesPorSituacaoEPeriodoEnvio envio)
        {
            try
            {
                TransacoesEmissorRetornoComQuantidade retorno = new TransacoesEmissorRetornoComQuantidade();
                LogHelper.GravarTraceLog(string.Format("[@@@INÍCIO FMS] PesquisarTransacoesPorSituacaoEPeriodo [@@@INÍCIO WCF] Dados : {0}", SerializadorHelper.SerializarDados(envio)));
                TransacoesBLL bll = new TransacoesBLL();
                Redecard.PN.FMS.Modelo.RespostaListaTransacoes resposta = bll.PesquisarTransacoesPorSituacaoEPeriodo(
                     envio.NumeroEmissor,
                     envio.GrupoEntidade,
                     envio.UsuarioLogin,
                     EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.SituacaoAnalisePesquisa, Redecard.PN.FMS.Modelo.SituacaoAnalisePesquisa>(envio.Situacao),
                     EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.TipoPeriodo, Redecard.PN.FMS.Modelo.TipoPeriodo>(envio.TipoPeriodo), 
                     envio.DataInicial, envio.DataFinal,
                     envio.PosicaoPrimeiroRegistro,
                     envio.QuantidadeRegistros,
                     envio.RenovarContador,
                     EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.CriterioOrdemTransacoesPorSituacaoEPeriodo, Redecard.PN.FMS.Modelo.CriterioOrdemTransacoesPorSituacaoEPeriodo>(envio.CriterioOrdem),//envio.CriterioOrdem,
                     EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.OrdemClassificacao, Redecard.PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem),
                     EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.IndicadorTipoCartao, Redecard.PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoCartao)
                     );
                if (resposta != null && resposta.ListaTransacoes != null && resposta.ListaTransacoes.Count > 0)
                {
                    retorno = Tradutor.TradutorTransacoesEmissorRetornoComQuantidade.TraduzirModeloToServicoTransacoesEmissorRetornoComQuantidade(resposta);
                }
                else
                {
                    retorno.ListaTransacoes = new List<Modelo.TransacaoEmissor>();
                    retorno.QuantidadeRegistros = 0;
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesPorSituacaoEPeriodo [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(resposta)));
                return retorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Método utilizado para pesquisar transacões analisadas pelo usuário e pelo período
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public TransacoesEmissorRetornoComQuantidade PesquisarTransacoesAnalisadasPorUsuarioEPeriodo(PesquisarTransacoesAnalisadasPorUsuarioEPeriodoEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesAnalisadasPorUsuarioEPeriodo [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                TransacoesBLL bll = new TransacoesBLL();

                Redecard.PN.FMS.Modelo.RespostaListaTransacoes transacoes = bll.PesquisarTransacoesAnalisadasPorUsuarioEPeriodo(
                    envio.NumeroEmissor,
                    envio.GrupoEntidade,
                    envio.UsuarioLogin,
                    envio.Usuario,
                    envio.DataInicial,
                    envio.DataFinal,
                    envio.PosicaoPrimeiroRegistro,
                    envio.QuantidadeRegistros,
                    envio.RenovarContador,
                    EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo, Redecard.PN.FMS.Modelo.CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo>(envio.CriterioOrdenacao),
                    EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.OrdemClassificacao, Redecard.PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem),
                    EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.IndicadorTipoCartao, Redecard.PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoCartao));

                TransacoesEmissorRetornoComQuantidade objRetorno = new TransacoesEmissorRetornoComQuantidade();
                if (transacoes != null && transacoes.ListaTransacoes.Count > 0)
                {
                    objRetorno = Tradutor.TradutorTransacoesEmissorRetornoComQuantidade.TraduzirModeloToServicoTransacoesEmissorRetornoComQuantidade(transacoes);
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesAnalisadasPorUsuarioEPeriodo [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        /// <summary>
        /// Método utilizado para o gerar o relatório de produtividade por data.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public Redecard.PN.FMS.Servico.Modelo.RelatorioProdutividadePorData PesquisarRelatorioProdutividadePorData(PesquisarRelatorioProdutividadeEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarRelatorioProdutividadePorData [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                ProdutividadeBLL bll = new ProdutividadeBLL();

                PN.FMS.Modelo.RelatorioProdutividadePorData retorno = bll.PesquisarRelatorioProdutividadePorData(envio.NumeroEmissor,
                    envio.GrupoEntidade,
                    envio.UsuarioLogin,
                    envio.Usuario,
                    envio.DataInicial,
                    envio.DataFinal,
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.CriterioOrdemProdutividade, PN.FMS.Modelo.CriterioOrdemProdutividade>(envio.Criterio),
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.OrdemClassificacao, PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem));

                Redecard.PN.FMS.Servico.Modelo.RelatorioProdutividadePorData objRetorno = new Redecard.PN.FMS.Servico.Modelo.RelatorioProdutividadePorData();

                if (retorno != null && retorno.ProdutividadePorData != null && retorno.ProdutividadePorData.Count > 0)
                {

                    PropertyHelper.CopiaPropriedades<PN.FMS.Modelo.RelatorioProdutividadePorData, Redecard.PN.FMS.Servico.Modelo.RelatorioProdutividadePorData>(retorno, ref objRetorno);

                    objRetorno.ProdutividadePorData = new List<Modelo.AgrupamentoProdutividadePorData>();
                    foreach (PN.FMS.Modelo.AgrupamentoProdutividadePorData agrupamento in retorno.ProdutividadePorData)
                    {
                        Redecard.PN.FMS.Servico.Modelo.AgrupamentoProdutividadePorData agrRetorno = new Modelo.AgrupamentoProdutividadePorData();

                        PropertyHelper.CopiaPropriedades<PN.FMS.Modelo.AgrupamentoProdutividadePorData, Redecard.PN.FMS.Servico.Modelo.AgrupamentoProdutividadePorData>(agrupamento, ref agrRetorno);

                        agrRetorno.ProdutividadePorData = new List<Modelo.ProdutividadePorData>();

                        PropertyHelper.CopiaLista<PN.FMS.Modelo.ProdutividadePorData, Redecard.PN.FMS.Servico.Modelo.ProdutividadePorData>(agrupamento.ProdutividadePorData, agrRetorno.ProdutividadePorData);

                        objRetorno.ProdutividadePorData.Add(agrRetorno);
                    }
                }
                else
                {
                    retorno.ProdutividadePorData = new List<PN.FMS.Modelo.AgrupamentoProdutividadePorData>();
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarRelatorioProdutividadePorData [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        /// <summary>
        /// Método utilizado para gerar o relatório de produtividade por analista.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public Redecard.PN.FMS.Servico.Modelo.RelatorioProdutividadePorAnalista RelatorioProdutividadePorAnalista(PesquisarRelatorioProdutividadeEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] RelatorioProdutividadePorAnalista [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                ProdutividadeBLL bll = new ProdutividadeBLL();
                PN.FMS.Modelo.RelatorioProdutividadePorAnalista retorno = bll.PesquisarRelatorioProdutividadePorAnalista(envio.NumeroEmissor,
                    envio.GrupoEntidade,
                    envio.UsuarioLogin,
                    envio.Usuario,
                    envio.DataInicial,
                    envio.DataFinal,
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.CriterioOrdemProdutividade, PN.FMS.Modelo.CriterioOrdemProdutividade>(envio.Criterio),
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.OrdemClassificacao, PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem));

                Redecard.PN.FMS.Servico.Modelo.RelatorioProdutividadePorAnalista objRetorno = new Redecard.PN.FMS.Servico.Modelo.RelatorioProdutividadePorAnalista();
                if (retorno != null && retorno.AgrupamentoProdutividadePorAnalista != null && retorno.AgrupamentoProdutividadePorAnalista.Count > 0)
                {
                    PropertyHelper.CopiaPropriedades<PN.FMS.Modelo.RelatorioProdutividadePorAnalista, Redecard.PN.FMS.Servico.Modelo.RelatorioProdutividadePorAnalista>(retorno, ref objRetorno);

                    objRetorno.AgrupamentoProdutividadePorAnalista = new List<Modelo.AgrupamentoProdutividadePorAnalista>();
                    foreach (PN.FMS.Modelo.AgrupamentoProdutividadePorAnalista agrupamento in retorno.AgrupamentoProdutividadePorAnalista)
                    {
                        Redecard.PN.FMS.Servico.Modelo.AgrupamentoProdutividadePorAnalista agrRetorno = new Modelo.AgrupamentoProdutividadePorAnalista();

                        PropertyHelper.CopiaPropriedades<PN.FMS.Modelo.AgrupamentoProdutividadePorAnalista, Redecard.PN.FMS.Servico.Modelo.AgrupamentoProdutividadePorAnalista>(agrupamento, ref agrRetorno);

                        agrRetorno.ProdutividadePorAnalista = new List<Modelo.DetalheProdutividadePorAnalista>();

                        PropertyHelper.CopiaLista<PN.FMS.Modelo.DetalheProdutividadePorAnalista, Redecard.PN.FMS.Servico.Modelo.DetalheProdutividadePorAnalista>(agrupamento.ProdutividadePorAnalista, agrRetorno.ProdutividadePorAnalista);

                        objRetorno.AgrupamentoProdutividadePorAnalista.Add(agrRetorno);
                    }
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] RelatorioProdutividadePorAnalista [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }

        #region PesquisarTransacoesEstabelecimentoAgrupadasPorCartao tela TransacoesSuspeitasEstabelecimentoAgrupadasPorCartao.ascx servico da CPQD findSuspectMerchantTransactionSummarizedByCard

        /// <summary>
        /// Pesquisa os dados da página TransacoesSuspeitasEstabelecimentoAgrupadasPorCartao.ascx.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimentoPorCartao PesquisarTransacoesEstabelecimentoAgrupadasPorCartao(TransacaoEstabelecimentoPorCartaoEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesEstabelecimentoAgrupadasPorCartao [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));


                TransacoesBLL bll = new TransacoesBLL();
                Redecard.PN.FMS.Modelo.RespostaTransacoesEstabelecimentoPorCartao resposta = bll.PesquisarTransacoesEstabelecimentoAgrupadasPorCartao(envio.NumeroEstabelecimento,
                      envio.NumeroEmissor,
                      envio.GrupoEntidade,
                      envio.UsuarioLogin,
                      envio.PrimeiroRegistro,
                      envio.QuantidadeMaximaRegistros,
                      EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao, PN.FMS.Modelo.CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao>(envio.ModoClassificacao),
                      EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.OrdemClassificacao, PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem),
                      EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.IndicadorTipoCartao, PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoTransacao)
                      );

                Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimentoPorCartao objRetorno = new Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimentoPorCartao();
                objRetorno.ListaTransacoes = new List<Modelo.Transacoes.AgrupamentoTransacoesEstabelecimentoCartao>();
                if (resposta != null && resposta.ListaTransacoes != null && resposta.ListaTransacoes.Count > 0)
                {
                    PropertyHelper.CopiaPropriedades<Redecard.PN.FMS.Modelo.RespostaTransacoesEstabelecimentoPorCartao, Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimentoPorCartao>(resposta, ref objRetorno);

                    PropertyHelper.CopiaLista<Redecard.PN.FMS.Modelo.AgrupamentoTransacoesEstabelecimentoCartao, Redecard.PN.FMS.Servico.Modelo.Transacoes.AgrupamentoTransacoesEstabelecimentoCartao>(resposta.ListaTransacoes, objRetorno.ListaTransacoes);
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesEstabelecimentoAgrupadasPorCartao [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        #endregion

        /// <summary>
        ///  Exportar transações estabelecimento agrupadas por cartão (exportSuspectMerchantTransactionSummarizedByCard).
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimento PesquisarTransacoesAgrupadasPorEstabelecimento(TransacaoEstabelecimentoEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesAgrupadasPorEstabelecimento [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                TransacoesBLL bll = new TransacoesBLL();
                Redecard.PN.FMS.Modelo.RespostaTransacoesEstabelecimento resposta = bll.PesquisarTransacoesAgrupadasPorEstabelecimento(envio.NumeroEmissor,
                    envio.GrupoEntidade,
                    envio.UsuarioLogin,
                    envio.PrimeiroRegistro,
                    envio.QuantidadeMaximaRegistros,
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.CriterioOrdemTransacoesAgrupadasPorEstabelecimento, PN.FMS.Modelo.CriterioOrdemTransacoesAgrupadasPorEstabelecimento>(envio.ModoClassificacao),
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.OrdemClassificacao, PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem),
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.IndicadorTipoCartao, PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoTransacao)
                      );

                Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimento objRetorno = new Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimento();
                objRetorno.ListaTransacoes = new List<Modelo.Transacoes.AgrupamentoTransacaoEstabelecimento>();

                if (resposta != null && resposta.ListaTransacoes != null && resposta.ListaTransacoes.Count > 0)
                {
                    PropertyHelper.CopiaPropriedades<Redecard.PN.FMS.Modelo.RespostaTransacoesEstabelecimento, Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimento>(resposta, ref objRetorno);
                    PropertyHelper.CopiaLista<Redecard.PN.FMS.Modelo.AgrupamentoTransacaoEstabelecimento, Redecard.PN.FMS.Servico.Modelo.Transacoes.AgrupamentoTransacaoEstabelecimento>(resposta.ListaTransacoes, objRetorno.ListaTransacoes);
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesAgrupadasPorEstabelecimento [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }

        #region PesquisarTransacoesPorCartao tela AnalisaTransacoesSuspeitasPorCartao.ascx servico da CPQD findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin
        /// <summary>
        ///  Exportar transações estabelecimento agrupadas por cartão (exportSuspectMerchantTransactionSummarizedByCard).
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesPorCartao PesquisarTransacoesPorCartao(TransacaoPorCartaoEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesPorCartao [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                TransacoesBLL bll = new TransacoesBLL();
                Redecard.PN.FMS.Modelo.RespostaTransacoesPorCartao resposta = bll.PesquisarTransacoesPorCartao(envio.NumeroEstabelecimento,
                    envio.NumeroEmissor,
                    envio.GrupoEntidade,
                    envio.UsuarioLogin,
                    envio.PrimeiroRegistro,
                    envio.QuantidadeMaximaRegistros,
                     EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.CriterioOrdemTransacoesAgrupadasPorCartao, PN.FMS.Modelo.CriterioOrdemTransacoesAgrupadasPorCartao>(envio.ModoClassificacao),
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.OrdemClassificacao, PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem),
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.IndicadorTipoCartao, PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoTransacao)
                      );

                Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesPorCartao objRetorno = new Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesPorCartao();

                objRetorno.ListaTransacoes = new List<Modelo.Transacoes.AgrupamentoTransacoesCartao>();

                if (resposta != null)
                {
                    PropertyHelper.CopiaPropriedades<Redecard.PN.FMS.Modelo.RespostaTransacoesPorCartao, Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesPorCartao>(resposta, ref objRetorno);
                    PropertyHelper.CopiaLista<Redecard.PN.FMS.Modelo.AgrupamentoTransacoesCartao, Redecard.PN.FMS.Servico.Modelo.Transacoes.AgrupamentoTransacoesCartao>(resposta.ListaTransacoes, objRetorno.ListaTransacoes);
                }

                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesPorCartao [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        #endregion

        #region Bloqueio e desbloqueio estabelecimento

        /// <summary>
        /// Chama o serviço 
        /// Bloquear estabelecimento (registerMerchantInAnalysis).
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
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] BloquearEstabelecimento [@@@FIM WCF]: [{0}]", numeroEmissor.ToString()));
                TransacoesBLL bll = new TransacoesBLL();
                bll.BloquearEstabelecimento(numeroEmissor, grupoEntidade, usuarioLogin, numeroEstabelecimento, tempoLimite);
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF]  Retornando DesbloquearEstabelecimento [@@@FIM WCF]: "));
                return;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return;
            }
        }

        /// <summary>
        /// Chama o serviço 
        /// Bloquear estabelecimento (registerMerchantInAnalysis).
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="tempoLimite"></param>
        public void DesbloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] DesbloquearEstabelecimento [@@@FIM WCF]: [{0}]", numeroEmissor.ToString()));
                TransacoesBLL bll = new TransacoesBLL();
                bll.DesbloquearEstabelecimento(numeroEmissor, grupoEntidade, usuarioLogin, numeroEstabelecimento);
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] Retornando DesbloquearEstabelecimento [@@@FIM WCF]: "));
                return;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return;
            }
        }

        #endregion

        #region Bloqueios e desbloqueios de de sessao das pesquisas de transacoes
        /// <summary>
        /// Método usado para descartar a sessão da pesquisa de Transaões suspeitas por cartão.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] DescartarSessaoPesquisaTransacoesSuspeitasPorCartao [@@@FIM WCF]: [{0}]", numeroEmissor.ToString()));
                TransacoesBLL bll = new TransacoesBLL();
                bll.DescartarSessaoPesquisaTransacoesSuspeitasPorCartao(numeroEmissor, grupoEntidade, usuarioLogin);
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] Retornando DescartarSessaoPesquisaTransacoesSuspeitasPorCartao [@@@FIM WCF]: "));
                return;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return;
            }
        }
        /// <summary>
        /// Método usado para descartar a sessão da pesquisa de Transaões suspeitas de estabelecimentos por cartão.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao [@@@FIM WCF]: [{0}]", numeroEmissor.ToString()));
                TransacoesBLL bll = new TransacoesBLL();
                bll.DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao(numeroEmissor, grupoEntidade, usuarioLogin);
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] Retornando DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao [@@@FIM WCF]: "));
                return;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return;
            }
        }
        /// <summary>
        ///  Método usado para descartar a sessão da pesquisa de Transaões suspeitas de estabelecimentos.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento [@@@FIM WCF]: [{0}]", numeroEmissor.ToString()));
                TransacoesBLL bll = new TransacoesBLL();
                bll.DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(numeroEmissor, grupoEntidade, usuarioLogin);
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] Retornando DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento [@@@FIM WCF]: "));
                return;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return;
            }
        }
        /// <summary>
        /// Método usado para descartar a sessão da pesquisa de Transaões suspeitas.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitas(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] DescartarSessaoPesquisaTransacoesSuspeitas [@@@FIM WCF]: [{0}]", numeroEmissor.ToString()));
                TransacoesBLL bll = new TransacoesBLL();
                bll.DescartarSessaoPesquisaTransacoesSuspeitas(numeroEmissor, grupoEntidade, usuarioLogin);
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] Retornando DescartarSessaoPesquisaTransacoesSuspeitas [@@@FIM WCF]: "));
                return;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return;
            }
        }
        #endregion

        #region Cadastro de IPs
        /// <summary>
        /// Método utilizado para incluir um ip ou mais ips na lista de ips autorizados.
        /// </summary>
        /// <param name="listaIPs"></param>
        /// <param name="flagIpsAutorizados"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        public void IncluirListaIps(List<IPsAutorizados> listaIPs, bool flagIpsAutorizados, int numeroEmissor, int grupoEntidade)
        {
            try
            {
                LogHelper.GravarLogIntegracao("[@@@WCF - LISTA IP AUTORIZADO (WCF)]", listaIPs);

                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] IncluirListaIps [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(listaIPs)));

                IpsBLL bll = new IpsBLL();

                List<Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados> listaParaBll = Tradutor.TradutorCadastroIP.TraduzirListaIpAutorizado(listaIPs);

                LogHelper.GravarLogIntegracao("[@@@WCF - LISTA IP AUTORIZADO (BLL)]", listaIPs);

                bll.InsereIps(listaParaBll, flagIpsAutorizados, numeroEmissor, grupoEntidade);

                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] Retornando IncluirListaIps [@@@FIM WCF]: "));

            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return;
            }
        }
        /// <summary>
        /// Método utilizado para buscar os ips na lista de ips autorizados.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        public List<Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados> BuscarListaIPs(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@INÍCIO WCF] BuscarListaIPs [@@@INÍCIO WCF]: "));
                
                IpsBLL bll = new IpsBLL();

                List<Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados> listaIpAutorizado = bll.BuscaIps(numeroEmissor, grupoEntidade, usuarioLogin);

                List<Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados> retorno = Tradutor.TradutorCadastroIP.TraduzirListaIpAutorizadoModeloParaServico(listaIpAutorizado);

                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] BuscarListaIPs [@@@FIM WCF]: {0} ", SerializadorHelper.SerializarDados(retorno)));

                return retorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }

        #endregion

        #region Pesquisar transações por transação associada (findTransactionListByAssociatedTransaction).
        /// <summary>
        /// Pesquisar transações por transação associada (findTransactionListByAssociatedTransaction).
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> PesquisarTransacoesPorTransacaoAssociada(PesquisarTransacoesPorTransacaoAssociadaEnvio envio)
        {
            try
            {
                LogHelper.GravarLogIntegracao("[@@@início WCF] PesquisarTransacoesPorTransacaoAssociada [@@@início WCF]", envio);

                TransacoesBLL bll = new TransacoesBLL();

                List<Redecard.PN.FMS.Modelo.TransacaoEmissor> listaTransacoesEmissor = bll.PesquisarTransacoesPorTransacaoAssociada(
                    envio.IdentificadorTransacao, envio.NumeroEmissor, envio.GrupoEntidade, envio.UsuarioLogin, envio.TempoBloqueio,
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.IndicadorTipoCartao, PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoCartao),
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.CriterioOrdemTransacoesPorNumeroCartaoOuAssociada, PN.FMS.Modelo.CriterioOrdemTransacoesPorNumeroCartaoOuAssociada>(envio.Criterio),
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.OrdemClassificacao, PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem)
                );

                List<TransacaoEmissor> result = Tradutor.TradutorTransacaoEmissor.TraduzirTransacaoEmissor(listaTransacoesEmissor);

                LogHelper.GravarLogIntegracao("[@@@fim WCF] PesquisarTransacoesPorTransacaoAssociada [@@@fim WCF]", result);

                return result;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        #endregion

        #region PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada (findAnalysedAndNotAnalysedTransactionListByAssociatedTransaction).
        /// <summary>
        /// Método que utiliza o serviço findAnalysedAndNotAnalysedTransactionListByAssociatedTransaction
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public TransacoesEmissorRetornoComQuantidade PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada(PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociadaEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@INÍCIO WCF] PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada [@@@INÍCIO WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                TransacoesBLL bll = new TransacoesBLL();

                Redecard.PN.FMS.Modelo.RespostaListaTransacoes transacoes = bll.PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada(envio.IdentificadorTransacao,
                    envio.NumeroEmissor,
                    envio.GrupoEntidade,
                    envio.UsuarioLogin,
                    envio.PosicaoPrimeiroRegistro,
                    envio.QuantidadeRegistros,
                    envio.RenovarContador,
                    EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.CriterioOrdemTransacoesPorNumeroCartaoOuAssociada, Redecard.PN.FMS.Modelo.CriterioOrdemTransacoesPorNumeroCartaoOuAssociada>(envio.Criterio),
                    EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.OrdemClassificacao, Redecard.PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem),
                    EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.IndicadorTipoCartao, Redecard.PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoCartao));

                TransacoesEmissorRetornoComQuantidade result = new TransacoesEmissorRetornoComQuantidade();
                
                if (transacoes != null && transacoes.ListaTransacoes.Count > 0)
                {
                    result = Tradutor.TradutorTransacoesEmissorRetornoComQuantidade.TraduzirModeloToServicoTransacoesEmissorRetornoComQuantidade(transacoes);
                }

                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(result)));

                return result;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        #endregion


        #region Exportação.
        /// <summary>
        /// Exporta os dados da página AnalisaTransacoesSuspeitas.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin(PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno objRetorno = new PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno();

                TransacoesBLL objNegocio = new TransacoesBLL();

                Redecard.PN.FMS.Modelo.TransacoesEmissor retornoTransacoes = objNegocio.ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin(envio.NumeroEmissor, envio.GrupoEntidade, envio.UsuarioLogin,
                EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.IndicadorTipoCartao, Redecard.PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoCartao));
                if (retornoTransacoes != null && retornoTransacoes.ListaTransacoesEmissor != null && retornoTransacoes.ListaTransacoesEmissor.Count > 0)
                {
                    objRetorno = Tradutor.TradutorPesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin.TraduzirModeloToServicoRespostaListaTransacoesEmissor(retornoTransacoes);
                }
                else
                {
                    objRetorno.ListaTransacoesEmissor = new List<Modelo.TransacaoEmissor>();
                    objRetorno.QuantidadeTotalRegistros = 0;
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Exporta os dados da página ConsultaTransacoesPorPeriodoSituacao.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<Redecard.PN.FMS.Servico.Modelo.TransacaoEmissor> ExportarTransacoesPorSituacaoEPeriodo(PesquisarTransacoesPorSituacaoEPeriodoEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@INÍCIO FMS] ExportarTransacoesPorSituacaoEPeriodo [@@@INÍCIO WCF] Dados : {0}", SerializadorHelper.SerializarDados(envio)));
                TransacoesBLL bll = new TransacoesBLL();
                List<Redecard.PN.FMS.Modelo.TransacaoEmissor> listaResposta = bll.ExportarTransacoesPorSituacaoEPeriodo(
                     envio.NumeroEmissor,
                     envio.GrupoEntidade,
                     envio.UsuarioLogin,
                     EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.SituacaoAnalisePesquisa, Redecard.PN.FMS.Modelo.SituacaoAnalisePesquisa>(envio.Situacao),
                     EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.TipoPeriodo, Redecard.PN.FMS.Modelo.TipoPeriodo>(envio.TipoPeriodo),//envio.TipoPeriodo, 
                     envio.DataInicial,
                     envio.DataFinal,
                     EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.IndicadorTipoCartao, Redecard.PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoCartao) //envio.TipoCartao
                     );
                List<Redecard.PN.FMS.Servico.Modelo.TransacaoEmissor> retorno = new List<Modelo.TransacaoEmissor>();

                if (listaResposta != null && listaResposta.Count > 0)
                {
                    retorno = Tradutor.TradutorTransacaoEmissor.TraduzirTransacaoEmissor(listaResposta);
                }
                //LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] ExportarTransacoesPorSituacaoEPeriodo [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(retorno)));
                return retorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Exporta os dados da página ConsultaTransacoesPorPeriodoUsuario.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<Redecard.PN.FMS.Servico.Modelo.TransacaoEmissor> ExportarTransacoesAnalisadasPorUsuarioEPeriodo(PesquisarTransacoesAnalisadasPorUsuarioEPeriodoEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] ExportarTransacoesAnalisadasPorUsuarioEPeriodo [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                TransacoesBLL bll = new TransacoesBLL();

                List<Redecard.PN.FMS.Modelo.TransacaoEmissor> transacoes = bll.ExportarTransacoesAnalisadasPorUsuarioEPeriodo(
                    envio.NumeroEmissor,
                    envio.GrupoEntidade,
                    envio.UsuarioLogin,
                    envio.Usuario,
                    envio.DataInicial,
                    envio.DataFinal,
                    envio.PosicaoPrimeiroRegistro,
                    envio.QuantidadeRegistros,
                    EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo, Redecard.PN.FMS.Modelo.CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo>(envio.CriterioOrdenacao),
                    EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.OrdemClassificacao, Redecard.PN.FMS.Modelo.OrdemClassificacao>(envio.Ordem),
                    EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.IndicadorTipoCartao, Redecard.PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoCartao));

                List<Redecard.PN.FMS.Servico.Modelo.TransacaoEmissor> objRetorno = new List<Modelo.TransacaoEmissor>();

                if (transacoes != null && transacoes.Count > 0)
                {
                    objRetorno = Tradutor.TradutorTransacaoEmissor.TraduzirTransacaoEmissor(transacoes);
                }

                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] ExportarTransacoesAnalisadasPorUsuarioEPeriodo [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Exporta os dados da página TransacoesSuspeitasEstabelecimentoAgrupadasPorCartao.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimentoPorCartao ExportarTransacoesEstabelecimentoAgrupadasPorCartao(TransacaoEstabelecimentoPorCartaoEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] ExportarTransacoesEstabelecimentoAgrupadasPorCartao [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));


                TransacoesBLL bll = new TransacoesBLL();
                Redecard.PN.FMS.Modelo.RespostaTransacoesEstabelecimentoPorCartao resposta = bll.ExportarTransacoesEstabelecimentoAgrupadasPorCartao(envio.NumeroEstabelecimento,
                      envio.NumeroEmissor,
                      envio.GrupoEntidade,
                      envio.UsuarioLogin,
                      EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.IndicadorTipoCartao, PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoTransacao)
                      );

                Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimentoPorCartao objRetorno = new Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimentoPorCartao();
                objRetorno.ListaTransacoes = new List<Modelo.Transacoes.AgrupamentoTransacoesEstabelecimentoCartao>();

                if (resposta != null && resposta.ListaTransacoes != null && resposta.ListaTransacoes.Count > 0)
                {
                    PropertyHelper.CopiaPropriedades<Redecard.PN.FMS.Modelo.RespostaTransacoesEstabelecimentoPorCartao, Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimentoPorCartao>(resposta, ref objRetorno);
                    PropertyHelper.CopiaLista<Redecard.PN.FMS.Modelo.AgrupamentoTransacoesEstabelecimentoCartao, Redecard.PN.FMS.Servico.Modelo.Transacoes.AgrupamentoTransacoesEstabelecimentoCartao>(resposta.ListaTransacoes, objRetorno.ListaTransacoes);
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] ExportarTransacoesEstabelecimentoAgrupadasPorCartao [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }

        /// <summary>
        /// Exporta os dados da página TransacoesSuspeitasAgrupadasPorCartao.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesPorCartao ExportarTransacoesAgrupadasPorCartao(TransacaoPorCartaoEnvio envio)
        {
            try
            {
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] ExportarTransacoesAgrupadasPorCartao [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(envio)));

                TransacoesBLL bll = new TransacoesBLL();
                Redecard.PN.FMS.Modelo.RespostaTransacoesPorCartao resposta = bll.ExportarTransacoesAgrupadasPorCartao(
                    envio.NumeroEmissor,
                    envio.GrupoEntidade,
                    envio.UsuarioLogin,
                    EnumHelper.EnumToEnum<PN.FMS.Servico.Modelo.IndicadorTipoCartao, PN.FMS.Modelo.IndicadorTipoCartao>(envio.TipoTransacao)
                      );

                Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesPorCartao objRetorno = new Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesPorCartao();
                objRetorno.ListaTransacoes = new List<Modelo.Transacoes.AgrupamentoTransacoesCartao>();
                if (resposta != null && resposta.ListaTransacoes != null && resposta.ListaTransacoes.Count > 0)
                {
                    PropertyHelper.CopiaPropriedades<Redecard.PN.FMS.Modelo.RespostaTransacoesPorCartao, Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesPorCartao>(resposta, ref objRetorno);
                    PropertyHelper.CopiaLista<Redecard.PN.FMS.Modelo.AgrupamentoTransacoesCartao, Redecard.PN.FMS.Servico.Modelo.Transacoes.AgrupamentoTransacoesCartao>(resposta.ListaTransacoes, objRetorno.ListaTransacoes);
                }
                LogHelper.GravarTraceLog(string.Format("[@@@FIM WCF] ExportarTransacoesAgrupadasPorCartao [@@@FIM WCF]: [{0}]", SerializadorHelper.SerializarDados(objRetorno)));
                return objRetorno;
            }
            catch (Exception ex)
            {
                ManipuladorExcecaoCamadaNegocio.TrataExcecao(ex);
                return null;
            }
        }
        #endregion
    }
}
