#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [09/05/2012] – [André Garcia] – [Criação]
- [27/10/2016] – [Agnaldo Costa de Almeida] – [Alteração para compatibilidade com a API de Login no Portal]
*/
#endregion

using System;
using System.Web.Security;
using System.ServiceModel;
using System.Collections.Generic;
using System.Configuration;

using Microsoft.SharePoint;

using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using System.Web;

using System.IO;
using System.Net;
using System.Text;


namespace Redecard.PN.DadosCadastrais.SharePoint
{
    /// <summary>
    /// Novo provider de login construído para o Portal Redecard
    /// </summary>
    public class PortalRedecardMembershipProvider : MembershipProvider
    {

        /// <summary>
        /// 
        /// </summary>
        private MembershipUser user = null;

        /// <summary>
        /// Número máximo de tentativas de login até o bloqueio do usuário
        /// </summary>
        private Int32 _maxInvalidPasswordAttempts = 6;

        /// <summary>
        /// Número de tentativas de senha inválida até o bloqueio do usuário
        /// </summary>
        public override Int32 MaxInvalidPasswordAttempts
        {
            get
            {
                return _maxInvalidPasswordAttempts;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(String username, Boolean userIsOnline)
        {
            return !Object.ReferenceEquals(user, null) ? user : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerUserKey"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(Object providerUserKey, Boolean userIsOnline)
        {
            return !Object.ReferenceEquals(user, null) ? user : null;
        }

        /// <summary>
        /// Método usado para validar um usuário no Portal Redecard
        /// </summary>
        /// <param name="username">Nome do usuário no formato {Grupo Entidade};{Numero do PV};{Id do Usuario}</param>
        /// <param name="password">Senha do usuário</param>
        /// <returns></returns>
        public override Boolean ValidateUser(String username, String password)
        {
            using (Logger Log = Logger.IniciarLog("PROVIDER"))
            {
                SharePointUlsLog.LogMensagem("PROVIDER - Inicio");

                Int32 codigoRetorno = 0;

                Int32 codigoGrupoEntidade = -1;
                Int32 codigoEntidade = -1;
                Boolean loginValido = false;
                String codigoNomeUsuario = String.Empty;
                String providerName = ConfigurationManager.AppSettings["MembershipProviderPN"];


                char[] _charOptionArray = { ';' };
                String[] _dadosUsuario = username.Split(_charOptionArray, StringSplitOptions.RemoveEmptyEntries);

                if (_dadosUsuario.Length == 4)
                {
                    codigoGrupoEntidade = Int32.Parse(_dadosUsuario[0]);
                    codigoEntidade = Int32.Parse(_dadosUsuario[1]);
                    codigoNomeUsuario = _dadosUsuario[2];
                    loginValido = Convert.ToBoolean(_dadosUsuario[3]);

                    try
                    {
                        if (loginValido)
                        {
                            user = new MembershipUser(providerName, codigoNomeUsuario, username, String.Empty, String.Empty,
                                    String.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);

                            return true;
                        }
                        else
                            return false;

                    }
                    catch (NullReferenceException ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);

                        this.CriarMensagemRetorno(username, 330);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);

                        this.CriarMensagemRetorno(username, 330);
                        return false;
                    }
                }
                else if (_dadosUsuario.Length == 3) //Remover após implementação do Login no App do Portal
                {
                    codigoGrupoEntidade = Int32.Parse(_dadosUsuario[0]);
                    codigoEntidade = Int32.Parse(_dadosUsuario[1]);
                    codigoNomeUsuario = _dadosUsuario[2];

                    SharePointUlsLog.LogMensagem("PROVIDER - Chamada ao serviço de usuário");
                    Log.GravarMensagem("PROVIDER - Chamada ao serviço de usuário");

                    try
                    {

                        Boolean pvKomerci = false;

                        // Verifica tecnologia Komerci ou DataCash somente para entidade de estabelecimentos 
                        if (codigoGrupoEntidade.Equals(1))
                        {
                            Int32 tecnologia = 0;

                            using (EntidadeServicoClient entidadeServico = new EntidadeServicoClient())
                            {
                                SharePointUlsLog.LogMensagem("PROVIDER - Chamada ao método consulta tecnologia estabelecimento");

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
                                        Log.GravarMensagem(logTipoTecnologia);
                                    }
                                }
                                else
                                {
                                    SharePointUlsLog.LogMensagem("PROVIDER - Entidade não encontrada no GE");
                                    Log.GravarMensagem("PROVIDER - Entidade não encontrada no GE");
                                    this.CriarMensagemRetorno(username, 1102);
                                    return false;
                                }
                            }
                        }

                        using (UsuarioServicoClient usuarioServico = new UsuarioServicoClient())
                        {
                            SharePointUlsLog.LogMensagem("PROVIDER - Chamada ao método validar");
                            Log.GravarMensagem("PROVIDER - Chamada ao método validar");

                            codigoRetorno = usuarioServico.Validar(codigoGrupoEntidade, codigoEntidade, codigoNomeUsuario, password, pvKomerci);
                            // Se o código de retorno for 0 ou 397 o login foi realizado com sucesso
                            if (codigoRetorno == 0 || codigoRetorno == 397)
                            {
                                SharePointUlsLog.LogMensagem("PROVIDER - Retorno válido 0 ou 397 retorna TRUE");
                                Log.GravarMensagem("PROVIDER - Retorno válido 0 ou 397 retorna TRUE");
                                SharePointUlsLog.LogMensagem("PROVIDER - Recupera valor do MembershipProviderPN: " + ConfigurationManager.AppSettings.Get("MembershipProviderPN"));
                                Log.GravarMensagem("PROVIDER - Recupera valor do MembershipProviderPN: " + ConfigurationManager.AppSettings.Get("MembershipProviderPN"));

                                user = new MembershipUser(providerName, codigoNomeUsuario, username, String.Empty, String.Empty,
                                    String.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);

                                return true;
                            }
                            else
                            {
                                SharePointUlsLog.LogMensagem("PROVIDER - Retorno inválido FALSE");
                                Log.GravarMensagem("PROVIDER - Retorno inválido FALSE");
                                this.CriarMensagemRetorno(username, codigoRetorno);
                                return false;
                            }
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);

                        this.CriarMensagemRetorno(username, 330);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);

                        this.CriarMensagemRetorno(username, 330);
                        return false;
                    }
                }
                else
                {
                    // "Dados inválidos para a autenticação do usuário."
                    this.CriarMensagemRetorno(username, 1029);
                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chave"></param>
        /// <param name="codigoRetorno"></param>
        private void CriarMensagemRetorno(String chave, Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("PROVIDER - Chama serviço para incluir retorno Login"))
            {
                SharePointUlsLog.LogMensagem("PROVIDER - Chama serviço para incluir retorno Login");
                using (UsuarioServico.UsuarioServicoClient usuario = new UsuarioServico.UsuarioServicoClient())
                {
                    usuario.InserirErroUsuarioLogin(chave, codigoRetorno);
                }
            }
        }

        #region Métodos abstratos da classe "MembershipProvider" ainda não implantados

        public override String ApplicationName
        {
            get;
            set;
        }

        public override MembershipUser CreateUser(String username, String password, String email, String passwordQuestion, String passwordAnswer, Boolean isApproved, Object providerUserKey, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.ProviderError;
            return null;
        }

        public override Boolean ChangePassword(String username, String oldPassword, String newPassword)
        {
            return false;
        }

        public override Boolean ChangePasswordQuestionAndAnswer(String username, String password, String newPasswordQuestion, String newPasswordAnswer)
        {
            return false;
        }

        public override Boolean DeleteUser(String username, Boolean deleteAllRelatedData)
        {
            return false;
        }

        public override Boolean EnablePasswordReset
        {
            get
            {
                return true;
            }
        }

        public override Boolean EnablePasswordRetrieval
        {
            get
            {
                return true;
            }
        }

        public override MembershipUserCollection FindUsersByEmail(String emailToMatch, Int32 pageIndex, Int32 pageSize, out Int32 totalRecords)
        {
            totalRecords = 0;
            return null;
        }

        public override MembershipUserCollection FindUsersByName(String usernameToMatch, Int32 pageIndex, Int32 pageSize, out Int32 totalRecords)
        {
            totalRecords = 0;
            return null;
        }

        public override MembershipUserCollection GetAllUsers(Int32 pageIndex, Int32 pageSize, out Int32 totalRecords)
        {
            totalRecords = 0;
            return null;
        }

        public override Int32 GetNumberOfUsersOnline()
        {
            return 0;
        }

        public override String GetPassword(String username, String answer)
        {
            return String.Empty;
        }

        public override String GetUserNameByEmail(String email)
        {
            return String.Empty;
        }

        public override Int32 MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return 0;
            }
        }

        public override Int32 MinRequiredPasswordLength
        {
            get
            {
                return 0;
            }
        }

        public override Int32 PasswordAttemptWindow
        {
            get
            {
                return 0;
            }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return MembershipPasswordFormat.Hashed;
            }
        }

        public override String PasswordStrengthRegularExpression
        {
            get
            {
                return String.Empty;
            }
        }

        public override Boolean RequiresQuestionAndAnswer
        {
            get
            {
                return false;
            }
        }

        public override Boolean RequiresUniqueEmail
        {
            get
            {
                return true;
            }
        }

        public override String ResetPassword(String username, String answer)
        {
            return String.Empty;
        }

        public override Boolean UnlockUser(String userName)
        {
            return false;
        }

        public override void UpdateUser(MembershipUser user)
        {
        }

        #endregion
    }
}
