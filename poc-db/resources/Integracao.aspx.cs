#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [29/05/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using Redecard.PN.Comum;
using System.ServiceModel;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Microsoft.SharePoint.IdentityModel;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login;
using Redecard.PN.DadosCadastrais.SharePoint.AdministracaoServico;
using Redecard.PN.DadosCadastrais.SharePoint.Login;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    /// <summary>
    /// Página para configuração dos serviços e dos menus no Portal de Serviços
    /// </summary>
    public class Integracao : ApplicationPageBaseAnonima
    {
        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlErro;

        /// <summary>
        /// Exibe erro no painel
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        private void ExibirErro(String mensagem)
        {
            pnlErro.Controls.Add(base.RetornarPainelExcecao(mensagem));
            pnlErro.Visible = true;
        }

        /// <summary>
        /// Exibe erro no painel
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        private void ExibirErro(String fonte, Int32 codigo)
        {
            pnlErro.Controls.Add(base.RetornarPainelExcecao(fonte, codigo));
            pnlErro.Visible = true;
        }

        /// <summary>
        /// Carregamento da página fazer o login e criar a sessão com o número do PV
        /// informado na QueryString Criptografada
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            String redirectUrl = String.Empty;
            try
            {
                //Recupera dados da queryString
                String queryString = Request.QueryString["dados"];
                String tipo = Request.QueryString["tipo"];

                Boolean usuarioAtendimento = String.IsNullOrEmpty(tipo) || String.Compare("estab", tipo, true) == 0;
                Boolean usuarioAtendimentoEps = String.Compare("eps", tipo, true) == 0;
                Boolean usuarioPiloto = String.Compare("piloto", tipo, true) == 0;

                //Se não existe queryString criptografada, interrompe processo de login/integração
                if (String.IsNullOrEmpty(queryString))
                    return;
                
                var query                       = new QueryStringSegura(queryString);
                var codigoEntidadeImpersonada   = default(Int32);
                var codigoGrupoEntidade         = default(Int32);
                var codigoEntidade              = default(Int32);
                var codigoUsuario               = default(String);
                var senha                       = default(String);
                var funcional                   = default(String);

                //Hashes de integração da Intranet
                String operadorHash = "1BC7918A2757C2460DD3C19D46EC6E0314DB4A";
                String operadorSemHash = "Rdm4ns@zpws2";

                //Impersonate de Estabelecimento para Central de Atendimento - Grupos Entidade 14 (Perfil 1) ou 16 (Perfil 2)
                if (usuarioAtendimento)
                {
                    codigoEntidadeImpersonada   = query["operadorPV"].ToInt32(0);   //PV da entidade impersonada
                    codigoEntidade              = 1;                                //Cód. entidade "fake" de Atendimento
                    codigoUsuario               = query["operadorLogin"];           //Usuário do operador ("atendimento")
                    funcional                   = query["operadorFuncional"];       //Funcional do operador
                    senha                       = query["operadorHashSenha"];       //Senha

                    if (query["operadorHashSenha"].Equals(operadorHash))
                        senha = operadorSemHash;

                    //Recupera o perfil de atendimento associado à funcional do operador (14 ou 16)
                    //Perfil perfil = ObterCodigoGrupoEntidade(funcional);
                    //if (perfil != null)
                        codigoGrupoEntidade = 14;//perfil.GrupoEntidade; ALTERAÇÃO SOLICITADA POR CANAIS/CICOTTI MANTER SOMENTE PERFIL 1 - 21/08/14
                    //else
                    //{
                    //    this.ExibirErro(String.Format("Não foi possível encontrar o perfil da funcional {0}.", funcional));
                    //    return;
                    //}
                }
                //Impersonate de EPS para Central de Atendimento - Grupo Entidade 15
                else if (usuarioAtendimentoEps)
                {
                    codigoGrupoEntidade         = 15;                           //Central de Atendimento EPS
                    codigoEntidade              = 1;                            //EPS Atendimento
                    codigoUsuario               = "epsatendimento";             //Usuário EPS Atendimento
                    funcional                   = query["operadorFuncional"];   //Funcional do operador
                    
                    if (query["operadorHashSenha"].Equals(operadorHash))
                        senha = operadorSemHash;
                }
                else if (usuarioPiloto)
                {
                    codigoEntidadeImpersonada = query["operadorPV"].ToInt32(0);   //PV da entidade impersonada
                    codigoEntidade = 1;                                //Cód. entidade "fake" de Atendimento
                    codigoUsuario = query["operadorLogin"];           //Usuário do operador ("atendimento")
                    funcional = query["operadorFuncional"];       //Funcional do operador

                    codigoGrupoEntidade = 17; //PILOTO

                    if (query["operadorHashSenha"].Equals(operadorHash))
                        senha = operadorSemHash;
                }
                else
                {
                    this.ExibirErro("Grupo de entidade não informado.");
                    return;
                }

                Boolean statusLogin = false;
                PortalApi.LoginApi loginApi = new PortalApi.LoginApi();

                //Verifica a partir de uma lista no Sharepoint se deve consumir o login pela nova API ou WCF
                bool utilizaApiLogin = ConfiguracaoLogin.UtilizaApiLogin();

                if (utilizaApiLogin)
                {
                    SharePointUlsLog.LogMensagem("LOGIN - Validar na API");

                    if(usuarioAtendimento)
                        statusLogin = loginApi.ValidarLogin(codigoUsuario, senha, codigoEntidadeImpersonada, codigoGrupoEntidade);
                    else
                        statusLogin = loginApi.ValidarLogin(codigoUsuario, senha, codigoEntidade, codigoGrupoEntidade);
                }
                else
                {
                    SharePointUlsLog.LogMensagem("Aplicar criptografia");
                    // Aplicar criptografia
                    senha = EncriptadorSHA1.EncryptString(senha);

                    SharePointUlsLog.LogMensagem("LOGIN - Validar no WCF");
                    statusLogin = ValidaLoginWcf(codigoGrupoEntidade, codigoEntidade, codigoUsuario, senha);
                }

                SharePointUlsLog.LogMensagem("Fazer chamada de autenticação");
                String _login = String.Format("{0};{1};{2};{3}", codigoGrupoEntidade, codigoEntidade, codigoUsuario, statusLogin);

                if (SPClaimsUtility.AuthenticateFormsUser(Request.Url, _login, senha))
                {
                    SharePointUlsLog.LogMensagem("Criar sessão do usuário");
                        
                    // Usuário Autenticado, criar objeto de sessão no AppFabric e redirecionar para o Portal de Serviços
                    var login = new LoginClass();
                    var codigoRetorno = default(Int32);

                    if (usuarioAtendimento)
                        codigoRetorno = login.CriarSessaoUsuario(codigoUsuario, codigoGrupoEntidade,
                            codigoEntidadeImpersonada, senha, usuarioAtendimento, usuarioAtendimentoEps, funcional, null, loginApi.InformacoesLoginOutrasEntidades);
                    else if (usuarioAtendimentoEps)
                        codigoRetorno = login.CriarSessaoUsuario(codigoUsuario, codigoGrupoEntidade, 
                            codigoEntidade, senha, usuarioAtendimento, usuarioAtendimentoEps, funcional);
                    else if (usuarioPiloto)
                        codigoRetorno = login.CriarSessaoUsuarioPiloto(codigoUsuario, codigoGrupoEntidade, codigoEntidadeImpersonada,
                            senha, usuarioPiloto, funcional, loginApi.InformacoesLoginOutrasEntidades);

                    if (codigoRetorno > 0)
                    {
                        SharePointUlsLog.LogMensagem("Código de retorno do login maior que 0");
                        this.ExibirErro("EntidadeServico.Consultar", codigoRetorno);
                        return;
                    }

                    SharePointUlsLog.LogMensagem("Login OK, redirecionar o usuário para o Portal Fechado");
                    // Redirecionar o usuário para o Portal Fechado
                    redirectUrl = "/sites/fechado";
                }
                else
                {
                    SharePointUlsLog.LogMensagem("Recupera mensagem de retorno do banco de dados.");
                    SharePointUlsLog.LogMensagem("Acessa serviço de usuario ConsultarErroUsuarioLogin.");
                        
                    UsuarioServico.TrataErroUsuarioLogin trataErro = null;

                    using(var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        trataErro = ctx.Cliente.ConsultarErroUsuarioLogin(codigoUsuario);

                    SharePointUlsLog.LogMensagem("Verifica retorno do ConsultarErroUsuarioLogin.");

                    if (trataErro != null)
                    {
                    SharePointUlsLog.LogMensagem("Exibe retorno do banco de dados.");
                        this.ExibirErro("Redecard.PN.DadosCadastrais.SharePoint.Login.Logar", trataErro.Codigo);

                        SharePointUlsLog.LogMensagem("Exclui retorno do banco de dados.");

                        using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                            ctx.Cliente.ExcluirErroUsuarioLogin(codigoUsuario, trataErro.Codigo);
                    }
                    else
                    {
                        SharePointUlsLog.LogMensagem("Exibe erro genérico SharePoint.");
                        this.ExibirErro(FONTE, CODIGO_ERRO);
                    }
                }
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                this.ExibirErro(ex.Fonte, ex.Codigo);
            }            
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                SharePointUlsLog.LogErro(ex.GetBaseException().Message);
                this.ExibirErro(ex.Reason.ToString(), ex.Code.Name.ToInt32());
            }
            catch (FaultException ex)
            {
                SharePointUlsLog.LogErro(ex.GetBaseException().Message);
                this.ExibirErro(ex.Reason.ToString(), ex.Code.Name.ToInt32());
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                this.ExibirErro(FONTE, CODIGO_ERRO);
            }

            if (!String.IsNullOrEmpty(redirectUrl))
            {
                SharePointUlsLog.LogMensagem("Redireciona usuário.");
                SPUtility.Redirect(redirectUrl, SPRedirectFlags.CheckUrl, HttpContext.Current);
            }
        }

        /// <summary>
        /// Busca o perfil do usuário fazendo login
        /// </summary>
        /// <param name="funcional">Funcional do operador</param>
        /// <returns>Perfil associado à funcional do usuário</returns>
        private AdministracaoServico.Perfil ObterCodigoGrupoEntidade(String funcional)
        {
            var perfil = new AdministracaoServico.Perfil();

            using (var log = Logger.IniciarLog("Get Código Grupo Entidade"))
            {
                log.GravarLog(EventoLog.ChamadaServico, funcional);

                try
                {
                    using (var contexto = new ContextoWCF<AdministracaoServico.AdministracaoServicoClient>())
                        perfil = contexto.Cliente.ConsultarFuncionalGrupoEntidade(funcional);
                }
                catch (FaultException<AdministracaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }

                log.GravarLog(EventoLog.RetornoServico, perfil);
            }

            return perfil;
        }

        /// <summary>
        /// Valida o login do usuário no serviço WCF
        /// </summary>
        /// <param name="codigoGrupoEntidade"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoNomeUsuario"></param>
        /// <param name="password"></param>
        private Boolean ValidaLoginWcf(int codigoGrupoEntidade, int codigoEntidade, string codigoNomeUsuario, string password)
        {
            try
            {
                SharePointUlsLog.LogMensagem("WCF LOGIN - Chamada ao serviço de usuário");

                Boolean pvKomerci = false;
                Int32 codigoRetorno = 0;

                // Verifica tecnologia Komerci ou DataCash somente para entidade de estabelecimentos 
                if (codigoGrupoEntidade.Equals(1))
                {
                    Int32 tecnologia = 0;

                    using (EntidadeServicoClient entidadeServico = new EntidadeServicoClient())
                    {
                        SharePointUlsLog.LogMensagem("WCF LOGIN - Chamada ao método consulta tecnologia estabelecimento");

                        // Verificar se tem DataCash. (Nunca um PV poderá ter Komerci e DataCash)
                        Int32 codigoRetornoEntidade = 0;
                        EntidadeServico.Entidade ent = entidadeServico.ConsultarDadosPV(out codigoRetornoEntidade, codigoEntidade);

                        if (!Object.ReferenceEquals(null, ent) && codigoRetornoEntidade == 0)
                        {
                            if (String.IsNullOrEmpty(ent.IndicadorDataCash))
                                ent.IndicadorDataCash = String.Empty;

                            // Caso o indicador = S e a data de ativação diferente de nula(DateTime.MinValue) o PV tem DataCash
                            if (ent.IndicadorDataCash.Equals("S") && !ent.DataAtivacaoDataCash.Equals(DateTime.MinValue))
                                pvKomerci = true;

                            if (!pvKomerci)
                            {
                                // Verificação caso o PV tenha Komerci
                                tecnologia = entidadeServico.ConsultarTecnologiaEstabelecimento(out codigoRetorno, codigoEntidade);
                                pvKomerci = (tecnologia == 25 || tecnologia == 26 || tecnologia == 23);

                                String logTipoTecnologia = String.Format("Log - Tecnologia: {0}; PvKomerci: {1}", tecnologia, pvKomerci.ToString());
                                SharePointUlsLog.LogMensagem(logTipoTecnologia);
                            }
                        }
                        else
                        {
                            SharePointUlsLog.LogMensagem("WCF LOGIN - Entidade não encontrada no GE");
                            //Log.GravarMensagem("WCF LOGIN - Entidade não encontrada no GE");
                            //this.CriarMensagemRetorno(username, 1102);
                            return false;
                        }
                    }
                }

                using (UsuarioServicoClient usuarioServico = new UsuarioServicoClient())
                {
                    SharePointUlsLog.LogMensagem("WCF LOGIN - Chamada ao método validar");

                    codigoRetorno = usuarioServico.Validar(codigoGrupoEntidade, codigoEntidade, codigoNomeUsuario, password, pvKomerci);
                    // Se o código de retorno for 0 ou 397 o login foi realizado com sucesso
                    if (codigoRetorno == 0 || codigoRetorno == 397)
                    {
                        SharePointUlsLog.LogMensagem("WCF LOGIN - Retorno válido 0 ou 397 retorna TRUE");

                        return true;
                    }
                    else
                    {
                        SharePointUlsLog.LogMensagem("WCF LOGIN - Retorno inválido FALSE");
                        //this.CriarMensagemRetorno(username, codigoRetorno);
                        return false;
                    }
                }
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                SharePointUlsLog.LogErro(ex.GetBaseException());

                //this.CriarMensagemRetorno(username, ex.Detail.Codigo);
                //codigoRetorno = ex.Detail.Codigo;
                return false;
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                SharePointUlsLog.LogErro(ex.GetBaseException());

                //this.CriarMensagemRetorno(username, ex.Detail.Codigo);
                //codigoRetorno = ex.Detail.Codigo;
                return false;
            }
        }
    }
}