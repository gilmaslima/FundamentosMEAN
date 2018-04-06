#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [04/06/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Microsoft.IdentityModel.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using Redecard.Portal.SharePoint.Client.WCF;
using Redecard.Portal.SharePoint.Client.WCF.SRVLoginLegado;
using Microsoft.SharePoint.Utilities;
using ModeloLogin = Redecard.PN.DadosCadastrais.SharePoint.Login.Modelo;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginClass
    {
        /// <summary>
        /// Chave de cache para configuração do login
        /// </summary>
        private const string chaveCache = "__listaPVsPiloto";

        /// <summary>
        /// Cancela Login do usuário
        /// </summary>
        private void CancelarLogin()
        {
            SPIisSettings iisSettingsWithFallback = SPContext.Current.Site.WebApplication.GetIisSettingsWithFallback(SPUrlZone.Default);

            if (iisSettingsWithFallback.UseClaimsAuthentication)
                FederatedAuthentication.SessionAuthenticationModule.SignOut();
        }

        /// <summary>
        /// Criar a sessão do usuário no Portal de Serviços Redecard
        /// </summary>
        /// <returns></returns>
        public Int32 CriarSessaoUsuario(String codigoNomeUsuario, Int32 iCodigoGrupoEntidade, Int32 iCodigoEntidade, String senha)
        {
            return this.CriarSessaoUsuario(codigoNomeUsuario, iCodigoGrupoEntidade, iCodigoEntidade, senha, false, false, null);
        }

        /// <summary>
        /// Criar a sessão do usuário no Portal de Serviços Redecard com informações da API
        /// </summary>
        /// <returns></returns>
        public Int32 CriarSessaoUsuario(String codigoNomeUsuario, Int32 iCodigoGrupoEntidade, Int32 iCodigoEntidade, String senha,
            PortalApi.Modelo.LoginEstabelecimentoRetorno loginEstabelecimento, PortalApi.Modelo.LoginOutrasEntidadesRetorno loginOutrasEntidades)
        {
            return this.CriarSessaoUsuario(codigoNomeUsuario, iCodigoGrupoEntidade, iCodigoEntidade, senha, false, false, null, loginEstabelecimento, loginOutrasEntidades);
        }

        /// <summary>
        /// Gerar novo token de Sessão para o Portal Legado
        /// </summary>
        /// <returns></returns>
        private String GerarToken()
        {
            SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criar Novo GUID de Sessão para Login de Outras Entidades");
            return String.Concat("{", Guid.NewGuid().ToString(), "}");
        }

        /// <summary>
        /// Gerar novo número de sessão do Portal Legado
        /// </summary>
        /// <returns></returns>
        private Int32 GeraRandom()
        {
            Random random = new Random();
            return random.Next(1, 99999999);
        }

        /// <summary>
        /// Criar a sessão do usuário no Portal de Serviços Redecard
        /// </summary>
        /// <param name="codigoNomeUsuario"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="senha"></param>
        /// <param name="usuarioPiloto"></param>
        /// <param name="funcional">Número da funcional, se Usuário Atendimento</param>
        /// <param name="loginEntidade">Informações do Login para Estabelecimento</param>
        /// <returns></returns>
        public Int32 CriarSessaoUsuarioPiloto(String codigoNomeUsuario, Int32 codigoGrupoEntidade, Int32 codigoEntidade,
            String senha, Boolean usuarioPiloto, String funcional, PortalApi.Modelo.LoginOutrasEntidadesRetorno loginEntidade)
        {
            try
            {
                Int32 codigoRetornoUsuario = 0;
                Sessao sessao = null;

                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criar instância dos serviços");
                UsuarioServico.Usuario usuario = null;

                SharePointUlsLog.LogMensagem(
                    String.Format("SESSÃO USUÁRIO - entidadeClient.Consultar - {0} : {1} : {2}", codigoEntidade, codigoGrupoEntidade, codigoNomeUsuario));

                //Código da entidade.
                //No caso de Central de Atendimento/Piloto, o usuário "atendimento" é sempre da entidade 1
                Int32 codigoEntidadeUsuario = codigoEntidade;
                codigoEntidadeUsuario = 1;

                if (loginEntidade == null)
                {
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        usuario = ctx.Cliente.ConsultarDadosUsuario(
                            out codigoRetornoUsuario,
                            codigoGrupoEntidade,
                            codigoEntidadeUsuario,
                            codigoNomeUsuario);
                }
                else
                {
                    if (loginEntidade != null)
                        usuario = this.MapearModeloLoginApi(loginEntidade);

                    //TODO: AAL - Necessário revisar estrutura gerada pela API para remover a chamada do WCF
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        usuario.Menu = ctx.Cliente.ConsultarMenu(codigoNomeUsuario, codigoGrupoEntidade, codigoEntidadeUsuario, usuario.CodigoIdUsuario);
                }


                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Verifica códigos de retorno");

                if (codigoRetornoUsuario != 0)
                {
                    this.CancelarLogin();
                    if (codigoRetornoUsuario > 0)
                        return codigoRetornoUsuario;
                    return 0;
                }
                else
                {
                    SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criar sessão Central de Atendimento");
                    sessao = CriarSessaoCentralAtendimento(codigoNomeUsuario, codigoGrupoEntidade, codigoEntidade, true, funcional);

                    if (sessao == null)
                        throw new Exception("Objeto de sessão inválido");

                    if (usuario != null)
                    {
                        SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criar sessão Central de Atendimento");
                        //Boolean pvEhPiloto = LoginClass.VerificarPiloto(codigoEntidade);

                        CadastrarServicos(sessao, usuario.Menu);
                        CadastrarMenu(sessao, usuario.Menu);
                        CadastrarPermissoes(sessao, usuario.Paginas);

                        if (loginEntidade != null)
                        {
                            sessao.TokenApi = loginEntidade.AccessToken;
                            sessao.TipoTokenApi = loginEntidade.TokenType;
                        }
                    }

                    SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - HttpContext.Current.Session.Add; Session.Timeout = 15");
                    //Tempo da sessão do usuário - PCI: 15 min
                    HttpContext.Current.Session.Timeout = 15;
                    HttpContext.Current.Session.Add(Sessao.ChaveSessao, sessao);

                    //Registra login no histórico/log
                    Historico.Login(sessao, usuario != null ? usuario.DataInclusao : (DateTime?)null);

                    return 0;
                }
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro ao criar a sessão do usuário", ex);
                this.CancelarLogin();
                throw new PortalRedecardException(1047, "LoginClass.CriarSessaoUsuario");
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro ao criar a sessão do usuário", ex);
                this.CancelarLogin();
                throw new PortalRedecardException(1047, "LoginClass.CriarSessaoUsuario");
            }
            catch (NullReferenceException ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro ao criar a sessão do usuário", ex);
                this.CancelarLogin();
                throw new PortalRedecardException(1047, "LoginClass.CriarSessaoUsuario");
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro ao criar a sessão do usuário", ex);
                this.CancelarLogin();
                throw new PortalRedecardException(1047, "LoginClass.CriarSessaoUsuario");
            }
        }

        /// <summary>
        /// Verificar se o PV está na lista de PVs Piloto
        /// </summary>
        /// <param name="codigoNumeroPv"></param>
        /// <returns></returns>
        public static Boolean VerificarPiloto(Int32 codigoNumeroPv)
        {
            try
            {
                //Preparação do objeto de retorno contendo a lista de domínios bloqueados
                List<ModeloLogin.PvPiloto> pvsPiloto = null;
                Boolean cacheUp = true;

                try
                {
                    //var cacheWarnings = CacheAdmin.ObterObjetos(Cache.Warnings);
                    pvsPiloto = CacheAdmin.Recuperar<List<ModeloLogin.PvPiloto>>(Comum.Cache.RedeTokenLogin, chaveCache);
                }
                catch (PortalRedecardException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    cacheUp = false;
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    cacheUp = false;
                }

                if (pvsPiloto == null || pvsPiloto.Count == 0)
                {
                    pvsPiloto = ObterListaPvsPiloto();
                    if (cacheUp)
                    {
                        CacheAdmin.Adicionar<List<ModeloLogin.PvPiloto>>(Comum.Cache.RedeTokenLogin, chaveCache, pvsPiloto);
                    }
                }

                if (pvsPiloto != null && pvsPiloto.Count > 0)
                {
                    ModeloLogin.PvPiloto pv = pvsPiloto.FirstOrDefault(p => p.NumeroPV == codigoNumeroPv);

                    if (pv != null)
                    {
                        SharePointUlsLog.LogMensagem("PV é Piloto");
                        return true;
                    }
                    else
                    {
                        SharePointUlsLog.LogMensagem("PV não é Piloto");
                        return false;
                    }
                }
                else
                {
                    SharePointUlsLog.LogMensagem("Nenhum PV na Lista Piloto");
                    return false;
                }
            }
            catch (SPException ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro ao validar se PV é Piloto", ex);
                return false;
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro ao validar se PV é Piloto", ex);
                return false;
            }
        }

        private static List<ModeloLogin.PvPiloto> ObterListaPvsPiloto()
        {
            List<ModeloLogin.PvPiloto> pvsPiloto = null;
            SharePointUlsLog.LogMensagem("Consulta a lista com os PVs Piloto");

            SPSite objSite = SPContext.Current.Site;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite sites = new SPSite(SPUtility.GetFullUrl(objSite, "/sites/fechado/")))
                using (SPWeb web = sites.OpenWeb())
                {
                    String nomeLista = "PVs Piloto";
                    SPList lista = web.Lists.TryGetList(nomeLista);

                    web.AllowUnsafeUpdates = true;

                    if (lista != null)
                    {
                        //Query para recuperar apenas os registros ativos
                        SPQuery query = new SPQuery();
                        query.QueryThrottleMode = SPQueryThrottleOption.Override;
                        query.Query = String.Concat(
                                     "<OrderBy>",
                                         "<FieldRef Name=\"ID\" Ascending=\"TRUE\" />",
                                     "</OrderBy>");

                        SharePointUlsLog.LogMensagem("Consultar Lista de PVs Piloto");
                        SPListItemCollection itensSp = lista.GetItems(query);

                        //Caso tenha sido encontrado algum item.
                        if (itensSp != null && itensSp.Count > 0)
                        {
                            pvsPiloto = new List<ModeloLogin.PvPiloto>();

                            pvsPiloto.AddRange(itensSp.Cast<SPListItem>().Select(item => new ModeloLogin.PvPiloto
                            {
                                NumeroPV = Convert.ToInt64(item["NumeroPV"]),
                                Habilitar = Convert.ToBoolean(item["Habilitar"])
                            }).ToList());
                        }
                    }
                }
            });

            return pvsPiloto;
        }

        /// <summary>
        /// Lista de PVs Piloto
        /// </summary>
        //private static SPList ListaPvsPiloto
        //{
        //    get
        //    {
        //        SPList lista = null;

        //        try
        //        {
        //            //SPUtility.ValidateFormDigest();

        //            //Recupera a lista de "PVs Piloto" em sites/fechado
        //            SPSecurity.RunWithElevatedPrivileges(delegate ()
        //            {
        //                using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
        //                {
        //                    using (SPWeb spWeb = spSite.AllWebs[""])
        //                    {
        //                        spWeb.AllowUnsafeUpdates = true;
        //                        spWeb.ValidateFormDigest();

        //                        lista = spWeb.Lists.TryGetList("PVs Piloto");
        //                    }
        //                }
        //            });
        //        }
        //        catch (SPException ex)
        //        {
        //            Logger.GravarLog(ex.ToString(), System.Diagnostics.TraceEventType.Error);
        //            SharePointUlsLog.LogErro(ex);
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.GravarLog(ex.ToString(), System.Diagnostics.TraceEventType.Error);
        //            SharePointUlsLog.LogErro(ex);
        //        }

        //        return lista;
        //    }
        //}

        /// <summary>
        /// Criar a sessão do usuário no Portal de Serviços Redecard
        /// </summary>
        /// <param name="codigoNomeUsuario"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="senha"></param>
        /// <param name="usuarioAtendimento"></param>
        /// <param name="usuarioAtendimentoEPS"></param>
        /// <param name="funcional">Número da funcional, se Usuário Atendimento</param>
        /// <param name="loginEstabelecimento">Informações do Login para Estabelecimento</param>
        /// <param name="loginOutrasEntidades">Informações do Login para outras Entidades</param>
        /// <returns></returns>
        public Int32 CriarSessaoUsuario(String codigoNomeUsuario, Int32 codigoGrupoEntidade, Int32 codigoEntidade,
            String senha, Boolean usuarioAtendimento, Boolean usuarioAtendimentoEPS, String funcional,
            PortalApi.Modelo.LoginEstabelecimentoRetorno loginEstabelecimento = null, PortalApi.Modelo.LoginOutrasEntidadesRetorno loginOutrasEntidades = null)
        {
            try
            {
                Int32 codigoRetornoUsuario = 0;
                Sessao sessao = null;

                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criar instância dos serviços");
                UsuarioServico.Usuario usuario = null;

                SharePointUlsLog.LogMensagem(
                    String.Format("SESSÃO USUÁRIO - entidadeClient.Consultar - {0} : {1} : {2}", codigoEntidade, codigoGrupoEntidade, codigoNomeUsuario));

                try
                {
                    if ((loginEstabelecimento == null && loginOutrasEntidades == null)
                        || (usuarioAtendimento || usuarioAtendimentoEPS))
                    {
                        //Código da entidade.
                        //No caso de Central de Atendimento, o usuário "atendimento" é sempre da entidade 1
                        Int32 codigoEntidadeUsuario = codigoEntidade;
                        if (usuarioAtendimento || usuarioAtendimentoEPS)
                            codigoEntidadeUsuario = 1;

                        using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                            usuario = ctx.Cliente.ConsultarDadosUsuario(
                                out codigoRetornoUsuario,
                                codigoGrupoEntidade,
                                codigoEntidadeUsuario,
                                codigoNomeUsuario);
                    }
                    else
                    {
                        if (loginEstabelecimento != null)
                            usuario = this.MapearModeloLoginApi(loginEstabelecimento);
                        else if (loginOutrasEntidades != null)
                            usuario = this.MapearModeloLoginApi(loginOutrasEntidades);

                        //TODO: AAL - Necessário revisar estrutura gerada pela API para remover a chamada do WCF
                        using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                            usuario.Menu = ctx.Cliente.ConsultarMenu(codigoNomeUsuario, codigoGrupoEntidade, codigoEntidade, usuario.CodigoIdUsuario);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    this.CancelarLogin();
                    throw ex;
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    this.CancelarLogin();
                    throw ex;
                }

                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Verifica códigos de retorno");

                if (codigoRetornoUsuario != 0)
                {
                    this.CancelarLogin();
                    if (codigoRetornoUsuario > 0)
                        return codigoRetornoUsuario;
                    return 0;
                }
                else
                {
                    SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Verifica tipo de Entidade e Usuário");
                    if (usuarioAtendimento)
                    {
                        SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criar sessão Central de Atendimento");
                        sessao = CriarSessaoCentralAtendimento(codigoNomeUsuario, codigoGrupoEntidade, codigoEntidade, usuarioAtendimento, funcional);
                    }
                    else if (usuarioAtendimentoEPS)
                    {
                        SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criar sessão Central de Atendimento EPS");
                        sessao = CriarSessaoCentralAtendimentoEPS(funcional, codigoGrupoEntidade, codigoEntidade, usuarioAtendimento);
                    }
                    else if (codigoGrupoEntidade == 1 || codigoGrupoEntidade == 6) // Estabelecimento ou Banco SPB
                    {
                        SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criar sessão Estabelecimento ou Banco SPB");
                        sessao = CriarSessaoUsuarioEstabelecimento(codigoNomeUsuario, codigoGrupoEntidade, codigoEntidade, usuarioAtendimento, usuario);
                    }
                    else
                    {
                        SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criar Sessão Outras Entidades");
                        sessao = CriarSessaoOutrasEntidadeseLegado(codigoNomeUsuario, codigoGrupoEntidade, codigoEntidade, senha, usuario);
                    }

                    if (sessao == null)
                        throw new Exception("Objeto de sessão inválido");

                    if (usuario != null)
                    {
                        //Boolean pvEhPiloto = LoginClass.VerificarPiloto(codigoEntidade);

                        CadastrarServicos(sessao, usuario.Menu);
                        CadastrarMenu(sessao, usuario.Menu);
                        CadastrarPermissoes(sessao, usuario.Paginas);

                        if (loginEstabelecimento != null)
                        {
                            sessao.TokenApi = loginEstabelecimento.AccessToken;
                            sessao.TipoTokenApi = loginEstabelecimento.TokenType;
                        }
                        else if (loginOutrasEntidades != null)
                        {
                            sessao.TokenApi = loginOutrasEntidades.AccessToken;
                            sessao.TipoTokenApi = loginOutrasEntidades.TokenType;
                        }
                    }

                    SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - HttpContext.Current.Session.Add; Session.Timeout = 15");
                    //Tempo da sessão do usuário - PCI: 15 min
                    HttpContext.Current.Session.Timeout = 15;
                    HttpContext.Current.Session.Add(Sessao.ChaveSessao, sessao);

                    //Registra login no histórico/log
                    Historico.Login(sessao, usuario != null ? usuario.DataInclusao : (DateTime?)null);

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                SharePointUlsLog.LogErro(ex);
                this.CancelarLogin();
                throw new PortalRedecardException(1047, "LoginClass.CriarSessaoUsuario");
            }
        }

        private Usuario MapearModeloLoginApi(PortalApi.Modelo.LoginOutrasEntidadesRetorno loginOutrasEntidades)
        {
            Usuario usuario = new Usuario();

            usuario.Entidade = new Entidade();
            usuario.Entidade.NomeEntidade = loginOutrasEntidades.Entidade.Nome;
            usuario.Entidade.Status = String.Empty;
            usuario.Entidade.CNPJEntidade = String.Empty;
            usuario.Entidade.Email = String.Empty;
            usuario.Entidade.TransacionaDolar = default(Boolean);
            usuario.Entidade.UF = String.Empty;
            usuario.Entidade.Tecnologia = 0;

            usuario.SenhaMigrada = loginOutrasEntidades.Usuario.SenhaMigrada;
            usuario.DataUltimoAcesso = Convert.ToDateTime(loginOutrasEntidades.Usuario.DataUltimoAcesso);
            usuario.TipoUsuario = loginOutrasEntidades.Usuario.TipoUsuario;
            usuario.Descricao = loginOutrasEntidades.Usuario.Descricao;
            usuario.CodigoIdUsuario = loginOutrasEntidades.Usuario.CodigoId;
            usuario.Email = loginOutrasEntidades.Usuario.Email;
            usuario.EmailSecundario = loginOutrasEntidades.Usuario.EmailSecundario;
            usuario.EmailTemporario = loginOutrasEntidades.Usuario.EmailTemporario;
            usuario.Status = new Status();

            if (!String.IsNullOrEmpty(loginOutrasEntidades.Usuario.Cpf))
                usuario.CPF = Convert.ToInt64(loginOutrasEntidades.Usuario.Cpf);

            if (!String.IsNullOrEmpty(loginOutrasEntidades.Usuario.Ddd))
                usuario.DDDCelular = Convert.ToInt32(loginOutrasEntidades.Usuario.Ddd);

            if (!String.IsNullOrEmpty(loginOutrasEntidades.Usuario.Celular))
                usuario.Celular = Convert.ToInt32(loginOutrasEntidades.Usuario.Celular);

            usuario.Status.Codigo = loginOutrasEntidades.Usuario.StatusCodigo;
            usuario.Legado = loginOutrasEntidades.Usuario.IndicadorLegado;
            usuario.ExibirMensagemLiberacaoAcesso = loginOutrasEntidades.Usuario.IndicadorMensagemLiberacao;

            List<UsuarioServico.Pagina> paginas = new List<UsuarioServico.Pagina>();
            foreach (PortalApi.Modelo.PermissaoRetorno permissao in loginOutrasEntidades.Autorizacao.Pemissoes)
            {
                paginas.Add(new UsuarioServico.Pagina() { Caminho = permissao.Caminho, Descricao = permissao.Descricao });
            }
            usuario.Paginas = paginas.ToArray();

            //TODO: AAL - Conferir estrutura correta para os menus retornados pela API
            //List<UsuarioServico.Menu> menus = new List<UsuarioServico.Menu>();
            //foreach (PortalApi.MenuRetorno menu in loginEstabelecimento.Autorizacao.Menu)
            //{
            //    menus.Add(new UsuarioServico.Menu()
            //    {
            //        Codigo = menu.CodigoServico,
            //        Descricao = menu.DescricaoBotao,
            //        FlagMenu = menu.IndicadorMenu,
            //        Observacoes = menu.Observacao,
            //        ServicoBasico = menu.IndicadorServicoBasico,
            //        Texto = menu.Nome
            //    });
            //}
            //usuario.Menu = menus.ToArray();

            return usuario;
        }

        private Usuario MapearModeloLoginApi(PortalApi.Modelo.LoginEstabelecimentoRetorno loginEstabelecimento)
        {
            Usuario usuario = new Usuario();

            usuario.Entidade = new Entidade();
            usuario.Entidade.NomeEntidade = loginEstabelecimento.Entidade.Nome;
            usuario.Entidade.Status = loginEstabelecimento.Entidade.Status;
            usuario.Entidade.CNPJEntidade = loginEstabelecimento.Entidade.Cnpj;
            usuario.Entidade.Email = loginEstabelecimento.Entidade.Email;
            usuario.Entidade.TransacionaDolar = loginEstabelecimento.Entidade.TransacionaDolar;
            usuario.Entidade.UF = loginEstabelecimento.Entidade.Uf;
            usuario.Entidade.Tecnologia = loginEstabelecimento.Entidade.CodigoTecnologia;
            usuario.Entidade.IndicadorDataCash = loginEstabelecimento.Entidade.IndicadorDataCash;
            usuario.Entidade.DataAtivacaoDataCash = Convert.ToDateTime(loginEstabelecimento.Entidade.DataAtivacaoDataCash);
            usuario.Entidade.CodigoSegmento = loginEstabelecimento.Entidade.CodigoSegmento[0];
            usuario.Entidade.CodigoGrupoRamo = loginEstabelecimento.Entidade.CodigoGrupoRamo;
            usuario.Entidade.CodigoRamoAtividade = loginEstabelecimento.Entidade.CodigoRamo;
            usuario.Entidade.CodigoCanal = loginEstabelecimento.Entidade.CodigoCanal;
            usuario.Entidade.CodigoCelula = loginEstabelecimento.Entidade.CodigoCelula;
            usuario.Entidade.Recarga = loginEstabelecimento.Entidade.Recarga;
            usuario.Entidade.CodigoMatriz = loginEstabelecimento.Entidade.CodigoMatriz;

            // consultar EAdquirencia no GE, não foi transferido o serviço para a base do PN porque ocorreu a migração
            // durante o processo de construção da carga
            try
            {
                using (EntidadeServico.EntidadeServicoClient client = new EntidadeServico.EntidadeServicoClient())
                {
                    int codigoRetorno = 0;
                    usuario.Entidade.ServicoEadquirencia = client.ValidarPossuiEAdquirencia(out codigoRetorno, loginEstabelecimento.Entidade.CodigoEntidade);
                }
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                usuario.Entidade.ServicoEadquirencia = false;
                Logger.GravarErro("Erro ao validar o EAdquirencia no login", ex);
            }
            catch (Exception ex)
            {
                usuario.Entidade.ServicoEadquirencia = false;
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro ao validar o EAdquirencia no login", ex);
            }

            usuario.SenhaMigrada = loginEstabelecimento.Usuario.SenhaMigrada;
            usuario.DataUltimoAcesso = Convert.ToDateTime(loginEstabelecimento.Usuario.DataUltimoAcesso);
            usuario.TipoUsuario = loginEstabelecimento.Usuario.TipoUsuario;
            usuario.Descricao = loginEstabelecimento.Usuario.Descricao;
            usuario.CodigoIdUsuario = loginEstabelecimento.Usuario.CodigoId;
            usuario.Email = loginEstabelecimento.Usuario.Email;
            usuario.EmailSecundario = loginEstabelecimento.Usuario.EmailSecundario;
            usuario.EmailTemporario = loginEstabelecimento.Usuario.EmailTemporario;
            usuario.Status = new Status();

            if (!String.IsNullOrEmpty(loginEstabelecimento.Usuario.Cpf))
                usuario.CPF = Convert.ToInt64(loginEstabelecimento.Usuario.Cpf);

            if (!String.IsNullOrEmpty(loginEstabelecimento.Usuario.Ddd))
                usuario.DDDCelular = Convert.ToInt32(loginEstabelecimento.Usuario.Ddd);

            if (!String.IsNullOrEmpty(loginEstabelecimento.Usuario.Celular))
                usuario.Celular = Convert.ToInt32(loginEstabelecimento.Usuario.Celular);

            usuario.Status.Codigo = loginEstabelecimento.Usuario.StatusCodigo;
            usuario.Legado = loginEstabelecimento.Usuario.IndicadorLegado;
            usuario.ExibirMensagemLiberacaoAcesso = loginEstabelecimento.Usuario.IndicadorMensagemLiberacao;

            List<UsuarioServico.Pagina> paginas = new List<UsuarioServico.Pagina>();
            foreach (PortalApi.Modelo.PermissaoRetorno permissao in loginEstabelecimento.Autorizacao.Pemissoes)
            {
                paginas.Add(new UsuarioServico.Pagina() { Caminho = permissao.Caminho, Descricao = permissao.Descricao });
            }
            usuario.Paginas = paginas.ToArray();

            //TODO: AAL - Conferir estrutura correta para os menus retornados pela API
            //List<UsuarioServico.Menu> menus = new List<UsuarioServico.Menu>();
            //foreach (PortalApi.MenuRetorno menu in loginEstabelecimento.Autorizacao.Menu)
            //{
            //    menus.Add(new UsuarioServico.Menu()
            //    {
            //        Codigo = menu.CodigoServico,
            //        Descricao = menu.DescricaoBotao,
            //        FlagMenu = menu.IndicadorMenu,
            //        Observacoes = menu.Observacao,
            //        ServicoBasico = menu.IndicadorServicoBasico,
            //        Texto = menu.Nome
            //    });
            //}
            //usuario.Menu = menus.ToArray();

            return usuario;
        }

        /// <summary>
        /// Criar sessão de Login para Outras Entidades e Grava sessão no Portal Legado
        /// </summary>
        /// <returns></returns>
        private Sessao CriarSessaoOutrasEntidadeseLegado(String codigoNomeUsuario, Int32 iCodigoGrupoEntidade, Int32 iCodigoEntidade, String senhaCriptografada, UsuarioServico.Usuario usuario)
        {
            try
            {
                String token = GerarToken();
                String nmSvd = "sem_definicao";
                Int32 text = GeraRandom();

                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criação da Sessão Outras Entidades - new Sessao()");
                Sessao sessao = new Sessao()
                {
                    CodigoEntidade = iCodigoEntidade,
                    NomeEntidade = usuario.Entidade.NomeEntidade,
                    StatusPV = usuario.Entidade.Status,
                    GrupoEntidade = iCodigoGrupoEntidade,
                    LoginUsuario = codigoNomeUsuario,
                    IDSessao = text, // Só usado quando o PV não é "Estabelecimento" ou "Central de Atendimento"
                    UltimoAcesso = usuario.DataUltimoAcesso,
                    TipoUsuario = (!object.ReferenceEquals(usuario, null) ? usuario.TipoUsuario : "P"),
                    NomeUsuario = (!object.ReferenceEquals(usuario, null) ? usuario.Descricao : String.Empty),
                    CNPJEntidade = usuario.Entidade.CNPJEntidade,
                    UsuarioAtendimento = false,
                    EmailEntidade = usuario.Entidade.Email,
                    TransacionaDolar = usuario.Entidade.TransacionaDolar,
                    UFEntidade = usuario.Entidade.UF,
                    Tecnologia = usuario.Entidade.Tecnologia,
                    TokenLegado = token,
                    CodigoIdUsuario = usuario.CodigoIdUsuario,
                    CodigoStatus = (Redecard.PN.Comum.Enumerador.Status)usuario.Status.Codigo.ToString().ToInt32Null(1),
                    Legado = usuario.Legado,
                };

#if !DEBUG
                //Dictionary<object, object> dictionary = new Dictionary<object, object>();

                //dictionary.Add("cod_usr", codigoNomeUsuario);
                //dictionary.Add("nu_grupoentidade", iCodigoGrupoEntidade);
                //dictionary.Add("nu_pdv", iCodigoEntidade);
                //dictionary.Add("LOGIN_NU_PDV", iCodigoEntidade);

                #region Dados Retirados da Procedures SPGE6002 (Não aplicável para Outras Entidades)
                //if (dadosLoginLegado.ValorParametros.ContainsKey("TIP_ETD"))
                //{
                //    dictionary.Add("nu_tipoentidade", (int)dadosLoginLegado.ValorParametros["TIP_ETD"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("NOM_FAT_PDV"))
                //{
                //    dictionary.Add("strNomeEstab", (string)dadosLoginLegado.ValorParametros["NOM_FAT_PDV"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("NUM_CGC_ESTB"))
                //{
                //    dictionary.Add("strCPF_CGC", Convert.ToDecimal(dadosLoginLegado.ValorParametros["NUM_CGC_ESTB"]));
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("NM_UF_LOJA"))
                //{
                //    dictionary.Add("nm_uf", (string)dadosLoginLegado.ValorParametros["NM_UF_LOJA"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("SERVICOPRE"))
                //{
                //    dictionary.Add("pre_autoriz", (string)dadosLoginLegado.ValorParametros["SERVICOPRE"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("TRABDOLAR"))
                //{
                //    dictionary.Add("strDolar", (string)dadosLoginLegado.ValorParametros["TRABDOLAR"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("TIP_USR"))
                //{
                //    dictionary.Add("strTipoUsr", (string)dadosLoginLegado.ValorParametros["TIP_USR"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("CODCTGR"))
                //{
                //    dictionary.Add("StatusPV", (string)dadosLoginLegado.ValorParametros["CODCTGR"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("EML_ETD"))
                //{
                //    dictionary.Add("txtEmailIdentPositiva", (string)dadosLoginLegado.ValorParametros["EML_ETD"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("SERVICOPRE"))
                //{
                //    dictionary.Add("ServicoPre", (string)dadosLoginLegado.ValorParametros["SERVICOPRE"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("SERVICOAVS"))
                //{
                //    dictionary.Add("ServicoAVS", (string)dadosLoginLegado.ValorParametros["SERVICOAVS"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("SERVICOSER"))
                //{
                //    dictionary.Add("ServicoSerasa", (string)dadosLoginLegado.ValorParametros["SERVICOSER"]);
                //}
                //if (dadosLoginLegado.ValorParametros.ContainsKey("CENTRAL"))
                //{
                //    dictionary.Add("centralizador", (string)dadosLoginLegado.ValorParametros["CENTRAL"]);
                //}
                #endregion

                //dictionary.Add("txtnu_pdv", iCodigoEntidade);
                //dictionary.Add("txtUsuario", codigoNomeUsuario);
                //dictionary.Add("txtCiteWeb", senhaCriptografada);
                //dictionary.Add("txtGrupoEntidade", iCodigoGrupoEntidade);
                //dictionary.Add("lngGrupoEntidadeErr", iCodigoGrupoEntidade);
                //dictionary.Add("strSenhaTemp", senhaCriptografada);
                //dictionary.Add("str_user", codigoNomeUsuario);
                //dictionary.Add("SessionID", text);

                //dictionary.Add("strTelAtendimento", "0800-784433");
                //dictionary.Add("strTelAutorizacao", "0800-784422");
                //dictionary.Add("strTelCentralRav", "0800-784446");
                //dictionary.Add("SitLogin", "E");
                //dictionary.Add("fzyTmpSenhaExp", 0);
                //dictionary.Add("monitora_sessao", 1);

                //if(iCodigoGrupoEntidade != 9)
                //{
                //    SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Chamar servico Maistro");
                //    using (ControleLoginLegadoServiceClient client = new ControleLoginLegadoServiceClient())
                //    {
                //        if (!client.CriarSessaoLegadoEstabelecimento(token, iCodigoEntidade, nmSvd, codigoNomeUsuario, dictionary, text.ToString()))
                //        {
                //            sessao = null;
                //        }
                //    }
                //}
#endif
                return sessao;
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                this.CancelarLogin();
                throw ex;
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                this.CancelarLogin();
                throw ex;
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                this.CancelarLogin();
                throw new PortalRedecardException(1047, "LoginClass.CriarSessaoUsuario");
            }
        }

        /// <summary>
        /// Criar sessão de Login para o usuário &quot;Estabelecimento&quot;
        /// </summary>
        /// <returns></returns>
        private Sessao CriarSessaoUsuarioEstabelecimento(String codigoNomeUsuario, Int32 iCodigoGrupoEntidade, Int32 iCodigoEntidade, Boolean usuarioAtendimento, UsuarioServico.Usuario usuario)
        {
            try
            {
                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Não é central de atendimento");
                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criação da Sessão - new Sessao()");

                Sessao sessao = new Sessao()
                {
                    CodigoEntidade = iCodigoEntidade,
                    NomeEntidade = usuario.Entidade.NomeEntidade,
                    StatusPV = usuario.Entidade.Status,
                    GrupoEntidade = iCodigoGrupoEntidade,
                    LoginUsuario = codigoNomeUsuario,
                    IDSessao = 0, // Só usado quando o PV não é "Estabelecimento" ou "Central de Atendimento"
                    UltimoAcesso = usuario.DataUltimoAcesso,
                    TipoUsuario = (!object.ReferenceEquals(usuario, null) ? usuario.TipoUsuario : "P"),
                    NomeUsuario = (!object.ReferenceEquals(usuario, null) ? usuario.Descricao : String.Empty),
                    CNPJEntidade = usuario.Entidade.CNPJEntidade,
                    UsuarioAtendimento = usuarioAtendimento,
                    EmailEntidade = usuario.Entidade.Email,
                    TransacionaDolar = usuario.Entidade.TransacionaDolar,
                    UFEntidade = usuario.Entidade.UF,
                    Tecnologia = usuario.Entidade.Tecnologia,
                    TecnologiaDataCash = usuario.Entidade.IndicadorDataCash,
                    TecnologiaDataCashDataAtivacao = usuario.Entidade.DataAtivacaoDataCash,
                    ServicoEadquirencia = usuario.Entidade.ServicoEadquirencia,
                    SenhaMigrada = usuario.SenhaMigrada,
                    CodigoSegmento = usuario.Entidade.CodigoSegmento,
                    CodigoIdUsuario = usuario.CodigoIdUsuario,
                    Email = usuario.Email,
                    EmailSecundario = usuario.EmailSecundario,
                    EmailTemporario = usuario.EmailTemporario,
                    CPF = usuario.CPF,
                    DDDCelular = usuario.DDDCelular,
                    Celular = usuario.Celular,
                    CodigoStatus = (Redecard.PN.Comum.Enumerador.Status)usuario.Status.Codigo.ToString().ToInt32Null(1),
                    Legado = usuario.Legado,
                    MigrarDepois = false,
                    CodigoGrupoRamo = usuario.Entidade.CodigoGrupoRamo,
                    CodigoRamoAtividade = usuario.Entidade.CodigoRamoAtividade,
                    CodigoCanal = usuario.Entidade.CodigoCanal,
                    CodigoCelula = usuario.Entidade.CodigoCelula,
                    Recarga = usuario.Entidade.Recarga,
                    CodigoMatriz = usuario.Entidade.CodigoMatriz,
                    ExibirMensagemLiberacaoAcessoCompleto = usuario.ExibirMensagemLiberacaoAcesso
                };

                return sessao;
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                this.CancelarLogin();
                throw ex;
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                this.CancelarLogin();
                throw ex;
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                this.CancelarLogin();
                throw new PortalRedecardException(1047, "LoginClass.CriarSessaoUsuario");
            }
        }

        /// <summary>
        /// Criar sessão de Login para o usuário &quot;Central de Atendimento&quot;
        /// </summary>
        /// <returns></returns>
        private Sessao CriarSessaoCentralAtendimento(String codigoUsuario, Int32 codigoGrupoEntidade,
            Int32 codigoEntidadeImpersonada, Boolean usuarioAtendimento, String funcional)
        {
            try
            {
                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - É Central de Atendimento");

                Int32 codigoRetornoEntidade = 0;
                Int32 tecnologia = 0;
                EntidadeServico.Entidade ent = null;

                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Recuperar Dados do PV e Tecnologia");

                //Consulta os dados da entidade impersonada
                using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    ent = ctx.Cliente.ConsultarDadosPV(out codigoRetornoEntidade, codigoEntidadeImpersonada);
                    tecnologia = ctx.Cliente.ConsultarTecnologiaEstabelecimento(out codigoRetornoEntidade, codigoEntidadeImpersonada);
                }

                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criação da Sessão - new Sessao()");
                Sessao sessao = new Sessao()
                {
                    CodigoEntidade = codigoEntidadeImpersonada,
                    NomeEntidade = ent.NomeEntidade,
                    StatusPV = ent.Status,
                    GrupoEntidade = codigoGrupoEntidade,
                    LoginUsuario = codigoUsuario,
                    IDSessao = 0, // Só usado quando o PV não é "Estabelecimento" ou "Central de Atendimento" ou "Piloto"
                    UltimoAcesso = DateTime.Now,
                    TipoUsuario = "P",
                    NomeUsuario = "Central de Atendimento",
                    CNPJEntidade = ent.CNPJEntidade,
                    UsuarioAtendimento = usuarioAtendimento,
                    EmailEntidade = ent.Email,
                    TransacionaDolar = ent.TransacionaDolar,
                    UFEntidade = ent.UF,
                    Tecnologia = tecnologia,
                    TecnologiaDataCash = ent.IndicadorDataCash,
                    TecnologiaDataCashDataAtivacao = ent.DataAtivacaoDataCash,
                    CodigoSegmento = ent.CodigoSegmento,
                    CodigoGrupoRamo = ent.CodigoGrupoRamo,
                    CodigoRamoAtividade = ent.CodigoRamoAtividade,
                    CodigoCanal = ent.CodigoCanal,
                    CodigoCelula = ent.CodigoCelula,
                    Recarga = ent.Recarga,
                    CodigoMatriz = ent.CodigoMatriz,
                    Funcional = funcional
                };

                return sessao;
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                this.CancelarLogin();
                throw ex;
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                this.CancelarLogin();
                throw ex;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                SharePointUlsLog.LogErro(ex);
                this.CancelarLogin();
                throw new PortalRedecardException(1047, "LoginClass.CriarSessaoUsuario");
            }
        }

        /// <summary>
        /// Criar sessão de Login para o usuário &quot;Central de Atendimento&quot;
        /// </summary>
        /// <returns></returns>
        private Sessao CriarSessaoCentralAtendimentoEPS(String codigoUsuario,
            Int32 codigoGrupoEntidade, Int32 codigoEntidadeImpersonada, Boolean usuarioAtendimento)
        {
            try
            {
                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - É Central de Atendimento EPS");

                //SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Recuperar Dados do PV e Tecnologia");

                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criação da Sessão EPS - new Sessao()");
                SharePointUlsLog.LogMensagem(String.Format("SESSÃO USUÁRIO - Criação da Sessão EPS - {0}: {1}", "CodigoEntidade", codigoEntidadeImpersonada));
                SharePointUlsLog.LogMensagem(String.Format("SESSÃO USUÁRIO - Criação da Sessão EPS - {0}: {1}", "GrupoEntidade", codigoGrupoEntidade));
                SharePointUlsLog.LogMensagem(String.Format("SESSÃO USUÁRIO - Criação da Sessão EPS - {0}: {1}", "LoginUsuario", codigoUsuario));
                SharePointUlsLog.LogMensagem(String.Format("SESSÃO USUÁRIO - Criação da Sessão EPS - {0}: {1}", "NomeUsuario", codigoUsuario));
                SharePointUlsLog.LogMensagem(String.Format("SESSÃO USUÁRIO - Criação da Sessão EPS - {0}: {1}", "UsuarioAtendimento", usuarioAtendimento));

                Sessao sessao = new Sessao()
                {
                    CodigoEntidade = codigoEntidadeImpersonada,
                    NomeEntidade = "EPS Central de Atendimento",
                    StatusPV = "A",
                    GrupoEntidade = codigoGrupoEntidade,
                    LoginUsuario = codigoUsuario,
                    IDSessao = 0, // Só usado quando o PV não é "Estabelecimento" ou "Central de Atendimento"
                    UltimoAcesso = DateTime.Now,
                    TipoUsuario = "M",
                    NomeUsuario = codigoUsuario,
                    UsuarioAtendimento = usuarioAtendimento
                };

                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Criação da Sessão EPS - Criada!");

                return sessao;
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                SharePointUlsLog.LogErro("SESSÃO USUÁRIO - ERRO EntidadeServico - CriarSessaoCentralAtendimentoEPS");
                this.CancelarLogin();
                throw ex;
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                SharePointUlsLog.LogErro("SESSÃO USUÁRIO - ERRO UsuarioServico - CriarSessaoCentralAtendimentoEPS");
                this.CancelarLogin();
                throw ex;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro ao criar sessão do usuário", ex);
                SharePointUlsLog.LogErro("SESSÃO USUÁRIO - ERRO Genérico - CriarSessaoCentralAtendimentoEPS");
                SharePointUlsLog.LogErro(ex);
                this.CancelarLogin();
                throw new PortalRedecardException(1047, "LoginClass.CriarSessaoUsuario");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void CadastrarPermissoes(Sessao sessao, UsuarioServico.Pagina[] paginasAcesso)
        {
            SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Cadastrar Permissões INÍCIO");
            if (!object.ReferenceEquals(paginasAcesso, null) && paginasAcesso.Length > 0)
            {
                for (Int32 i = 0; i < paginasAcesso.Length; i++)
                {
                    UsuarioServico.Pagina pagina = paginasAcesso[i];
                    sessao.Paginas.Add(new Comum.Pagina()
                    {
                        TextoBotao = pagina.NomeLink,
                        Url = pagina.Caminho
                    });
                }
            }

            /* 21-08-2017: AAL - Desabilitada verificação de Piloto para Conta Corrente/Diário de Vendas
            if (!pvEhPiloto.HasValue)
                pvEhPiloto = LoginClass.VerificarPiloto(sessao.CodigoEntidade);

            if (pvEhPiloto.HasValue && !pvEhPiloto.Value)
            {
                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Cadastrar Permissões - Verificar se PV está no Piloto");
                Comum.Pagina pagina = sessao.Paginas.Find(p => p.Url.Contains("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=13"));
                if (pagina != null)
                    sessao.Paginas.Remove(pagina);
            }
			*/

            if (!sessao.PVMatriz)
            {
                SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Cadastrar Permissões - Verificar se PV é Matriz para exibir o Conciliador");
                Comum.Pagina pagina = sessao.Paginas.Find(p => p.Url.ToLower().Contains("controlrede"));
                if (pagina != null)
                    sessao.Paginas.Remove(pagina);
            }

            SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Cadastrar Permissões FIM");
        }

        /// <summary>
        /// Criar os cookies de sessão do SharePoint
        /// </summary>
        private void CriarCookies(string sToken, int sessionId)
        {
            HttpCookieCollection cookies = HttpContext.Current.Request.Cookies;
            List<string> dicKeys = new List<string>();
            dicKeys.AddRange(cookies.AllKeys);

            this.CriarOuAtualizarCookie("ISSession", sToken, dicKeys);
            this.CriarOuAtualizarCookie("ISSessionID", Convert.ToString(sessionId), dicKeys);
        }

        /// <summary>
        /// Cria ou atualiza o valor de um cookie
        /// </summary>
        /// <param name="sToken"></param>
        /// <param name="dicKeys"></param>
        private void CriarOuAtualizarCookie(string key, string value, List<string> dicKeys)
        {
            if (!dicKeys.Contains(key))
                HttpContext.Current.Response.SetCookie(new HttpCookie(key, value));
            else
                HttpContext.Current.Response.Cookies[key].Value = value;
        }

        /// <summary>
        /// Cria sessão (IS SQL Server) para as variaveis do legado "services.redecard".
        /// </summary>
        private Int32 CriarSessaoLegado(String codigoNomeUsuario, Int32 iCodigoGrupoEntidade, Int32 iCodigoEntidade, String senha, Int32 sessaoID)
        {
            if (iCodigoGrupoEntidade == 1) // Estabelecimento 
            {
                String senhaCript = EncriptadorSHA1.EncryptString(senha);

                // Criar sessão no legado
                RetornoLoginLegadoEstabelecimentoVO retorno = SharePointWCFHelper.RealizaLoginEstabelecimento(iCodigoEntidade, codigoNomeUsuario, senhaCript);
                if (retorno.CodigoRetorno == 0)
                {
                    String token = SharePointWCFHelper.ObterNovoTokenSessaoLegado();
                    sessaoID = SharePointWCFHelper.CriarSessaoLegadoEstabelecimento(token,
                        codigoNomeUsuario, senhaCript, retorno);

                    // criar cookies para manter compatibilidade
                    this.CriarCookies(token, sessaoID);
                }
            }
            return sessaoID;
        }

        /// <summary>
        /// Cadastra os menus na variável da Sessao
        /// </summary>
        public static void CadastrarMenu(Sessao sessao, UsuarioServico.Menu[] menu)
        {
            SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Cadastrar Menu INÍCIO");
            List<Comum.Menu> menus = TratarMenu(sessao, menu);
            SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Cadastrar Serviços MEIO");
            //Remove os menus que não são "menu"
            menus.RemoveAll(x => x.FlagMenu == false);
            sessao.Menu.AddRange(menus);

            SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Cadastrar Serviços FIM");
        }

        /// <summary>
        /// Cadastra os serviços na variável Sessao
        /// </summary>
        public static void CadastrarServicos(Sessao sessao, UsuarioServico.Menu[] menu)
        {
            SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Cadastrar Serviços INÍCIO");
            List<Comum.Menu> menus = TratarMenu(sessao, menu);
            SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Cadastrar Serviços MEIO");
            sessao.Servicos.AddRange(menus);
            SharePointUlsLog.LogMensagem("SESSÃO USUÁRIO - Cadastrar Serviços FIM");
        }

        /// <summary>
        /// Monta o menu do usuário, tratando especificamente os casos de:
        /// - DataCash / E-Rede
        /// - Komerci
        /// - Mobile Rede
        /// - PV Cancelado
        /// - PV Piloto
        /// </summary>
        /// <param name="sessao">Sessão</param>
        /// <param name="menu">Menu</param>
        /// <returns>Menu tratado</returns>
        private static List<Comum.Menu> TratarMenu(Sessao sessao, UsuarioServico.Menu[] menu)
        {
            List<Comum.Menu> menus = new List<Comum.Menu>();

            if (!Object.ReferenceEquals(menu, null) && menu.Length > 0)
            {
                //Booleanos relacionados ao PV
                Boolean pvAdquirencia = sessao.PossuiEadquirencia;
                Boolean pvDataCash = sessao.PossuiDataCash;
                Boolean pvCancelado = sessao.StatusPVCancelado();
                Boolean pvKomerci = sessao.PossuiKomerci;
                Boolean pvMobile = sessao.Tecnologia == 154; //PV contratou Mobile Rede
                Boolean pvCarteiraDigital = sessao.PVCarteiraDigital;
                Boolean pvRedepay = pvKomerci && sessao.PVCarteiraDigital;

                //Boolean pvEhPiloto = LoginClass.VerificarPiloto(sessao.CodigoEntidade);

                foreach (UsuarioServico.Menu item in menu)
                {
                    //Identifica o menu atual
                    String textoMenu = item.Texto.Trim();
                    Boolean menuExtrato = String.Compare("vendas", textoMenu, true) == 0;
                    Boolean menuMinhaConta = String.Compare("meu estabelecimento", textoMenu, true) == 0
                        || String.Compare("dados cadastrais", textoMenu, true) == 0;
                    Boolean menuKomerci = String.Compare("komerci", textoMenu, true) == 0;
                    Boolean menuMobile = String.Compare("mobile rede", textoMenu, true) == 0;
                    Boolean menuDataCash = String.Compare("e-rede", textoMenu, true) == 0;
                    Boolean menuEadquirencia = String.Compare("eadquirencia", textoMenu, true) == 0;
                    Boolean menuControlRede = String.Compare("control rede", textoMenu, true) == 0;
                    Boolean menuServicos = String.Compare("serviços", textoMenu, true) == 0;

                    // menu "control rede" movido para a raiz
                    // mantendo validação se o PV pode acessar o serviço do control rede
                    if (menuControlRede && !sessao.PVMatriz)
                        continue;

                    // oculta os menus e-Rede/e.Rede para PV Redepay
                    if (pvRedepay && (menuDataCash || menuEadquirencia))
                        continue;

                    // oculta o menu "serviços" para PV Komerci, e.Rede, e-Rede e Redepay
                    if (menuServicos && (pvKomerci || pvKomerci && pvAdquirencia || pvDataCash) && !pvRedepay)
                        continue;

                    // Converte o texto do menu adquirência para e-rede
                    if (menuEadquirencia)
                        textoMenu = "e.Rede";

                    //Converte modelo Menu do UsuarioServico para modelo Menu da Comum
                    var itemMenuSessao = new Comum.Menu()
                    {
                        Texto = textoMenu,
                        Observacoes = item.Observacoes,
                        FlagMenu = item.FlagMenu,
                        Codigo = item.Codigo,
                        FlagFooter = item.FlagFooter
                    };

                    //Converte as Páginas do UsuarioServico para modelo Pagina da Comum
                    foreach (PaginaMenu pagina in item.Paginas)
                        itemMenuSessao.Paginas.Add(new Comum.Pagina
                        {
                            TextoBotao = pagina.TextoBotao,
                            Url = pagina.Url,
                            Navegacao = pagina.Navegacao
                        });

                    if (item.Items.Length > 0)
                        CarregarMenuFilhos(itemMenuSessao, item, sessao.PVMatriz, sessao);

                    //PV cancelado só possui acesso ao Extrato e Minha Conta
                    if (pvCancelado)
                    {
                        if (menuExtrato || menuMinhaConta)
                            menus.Add(itemMenuSessao);
                    }
                    else
                    {
                        // PV que possui eadquirência não poderá ter Komerci/nem datacash
                        if (pvAdquirencia)
                        {
                            if (!menuKomerci && !menuMobile && !menuDataCash)
                                menus.Add(itemMenuSessao);
                        }
                        // PV que possui e-Rede/DataCash não poderá ter Komerci nem Mobile
                        else if (pvDataCash)
                        {
                            if (!menuEadquirencia && !menuKomerci && !menuMobile)
                                menus.Add(itemMenuSessao);
                        }
                        // PV que possui Komerci não poderá ter e-Rede/DataCash nem Mobile
                        else if (pvKomerci)
                        {
                            if (!menuEadquirencia && !menuDataCash && !menuMobile)
                            {
                                //Não exibe menu Komerci se o PV for Carteira Digital
                                if (!menuKomerci || !pvCarteiraDigital)
                                    menus.Add(itemMenuSessao);
                            }
                        }
                        // Caso o estabelecimento tenha contratado o Mobile Rede
                        else if (pvMobile)
                        {
                            if (!menuEadquirencia && !menuKomerci && !menuDataCash)
                                menus.Add(itemMenuSessao);
                        }
                        // Caso o PV não tenha Komerci, nem e-Rede/DataCash, nem Mobile                        
                        else
                        {
                            if (!menuEadquirencia && !menuKomerci && !menuDataCash && !menuMobile)
                                menus.Add(itemMenuSessao);
                        }
                    }
                }
            }

            return menus;
        }

        /// <summary>
        /// Remove recursivamente todos os itens de menu que possuam determinada url de Página.
        /// </summary>
        /// <param name="itemMenu">Menu que será percorrido recursivamente</param>
        /// <param name="urlExata">Indica se deve considerar a URL exata ou parcial na comparação</param>
        /// <param name="urlPagina">URL (exata ou parcial) da página</param>
        private static void RemoverItemMenuPorUrlPagina(ref Comum.Menu itemMenu, String urlPagina, Boolean urlExata)
        {
            Int32 quantidadeItens = itemMenu.Items != null ? itemMenu.Items.Count : 0;
            for (Int32 indiceItem = 0; indiceItem < quantidadeItens; indiceItem++)
            {
                Comum.Menu item = itemMenu.Items[indiceItem];

                var removerItem = urlExata ?
                    item.Paginas.Any(pagina => String.Compare(pagina.Url, urlPagina, true) == 0) :
                    item.Paginas.Any(pagina => pagina.Url.Contains(urlPagina));

                if (removerItem)
                    itemMenu.Items.Remove(item);

                RemoverItemMenuPorUrlPagina(ref item, urlPagina, urlExata);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="item"></param>
        /// <param name="pvMatriz">Indicador se o PV é Matriz ou não</param>
        /// <param name="sessao">Dados da sessão do usuário</param>
        private static void CarregarMenuFilhos(
            Redecard.PN.Comum.Menu root,
            UsuarioServico.Menu item,
            Boolean pvMatriz,
            Sessao sessao)
        {
            Boolean menuServicos = String.Compare("serviços", item.Texto.Trim(), true) == 0;
            Boolean pvRedepay = sessao.PossuiKomerci && sessao.PVCarteiraDigital;
            List<String> submenusServicoRestringirRedepay = new List<String>() {
                "consultar pagueaqui itaú",
                "consultar banco emissor",
                "consulta de credenciamento voucher"
            };

            foreach (UsuarioServico.Menu menuItem in item.Items)
            {
                // não exibe submenus de serviço para Redepay
                Boolean ocultarSubmenuServico = !submenusServicoRestringirRedepay
                    .Any(x => menuItem.Texto.ToLower().Contains(x));
                if (menuServicos && ocultarSubmenuServico && pvRedepay)
                    continue;

                if ((menuItem.Texto.ToLower().Equals("control rede") && !pvMatriz))
                {
                    continue;
                }

                Comum.Menu subItem = new Comum.Menu()
                {
                    Texto = menuItem.Texto,
                    Observacoes = menuItem.Observacoes,
                    FlagMenu = menuItem.FlagMenu,
                    Codigo = menuItem.Codigo,
                    FlagFooter = menuItem.FlagFooter
                };

                //Validando se o PV pode acessar o serviço de Piloto do Conta Corrente
                //20/07/2017 - RSA - Valida se o PV é piloto para acessar o novo extrato SPA
                if (
                    //(subItem.Texto.Equals("Conta Corrente") && !pvEhPiloto) || 
                    (subItem.Texto.ToLower().Equals("control rede") && !pvMatriz)
                    //|| (subItem.Texto.ToLower().Equals("consultar extratos (novo)") && !pvEhPiloto)
                    )
                {
                    continue;
                }
                else
                {
                    foreach (PaginaMenu pagina in menuItem.Paginas)
                        subItem.Paginas.Add(new Comum.Pagina()
                        {
                            TextoBotao = pagina.TextoBotao,
                            Url = pagina.Url,
                            Navegacao = pagina.Navegacao
                        });

                    if (menuItem.Items.Length > 0)
                        CarregarMenuFilhos(subItem, menuItem, pvMatriz, sessao);

                    root.Items.Add(subItem);
                }
            }
        }
    }
}
