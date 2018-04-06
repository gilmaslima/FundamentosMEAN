using System;
using System.Web.Security;
using Redecard.Portal.SharePoint.Client.WCF;
using Redecard.Portal.SharePoint.Client.WCF.SRVLoginLegado;
using System.Diagnostics;

namespace Redecard.Portal.Helper {
    /// <summary>
    /// 
    /// </summary>
    public class RedecardeMembershipProvider : MembershipProvider {

        /// <summary>
        /// 
        /// </summary>
        string _userName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        string _appName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        MembershipUser user = null;

        #region Métodos do Membership Não Implementados

        public override string ApplicationName {
            get {
                return _appName;
            }
            set {
                _appName = value;
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword) {
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
            return false;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
            user = new MembershipUser("r", _userName, _userName, string.Empty, string.Empty,
                string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            status = MembershipCreateStatus.Success;
            return user;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData) {
            return false;
        }

        public override bool EnablePasswordReset {
            get { return false; }
        }

        public override bool EnablePasswordRetrieval {
            get { return false; }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
            totalRecords = 0;
            return null;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
            totalRecords = 0;
            return null;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
            totalRecords = 0;
            return null;
        }

        public override int GetNumberOfUsersOnline() {
            return 0;
        }

        public override string GetPassword(string username, string answer) {
            return string.Empty;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline) {
            return !object.ReferenceEquals(user, null) ? user : null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
            return !object.ReferenceEquals(user, null) ? user : null;
        }

        public override string GetUserNameByEmail(string email) {
            return string.Empty;
        }

        public override int MaxInvalidPasswordAttempts {
            get { return 3; }
        }

        public override int MinRequiredNonAlphanumericCharacters {
            get { return 1; }
        }

        public override int MinRequiredPasswordLength {
            get { return 1; }
        }

        public override int PasswordAttemptWindow {
            get { return 1; }
        }

        public override MembershipPasswordFormat PasswordFormat {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression {
            get { return string.Empty; }
        }

        public override bool RequiresQuestionAndAnswer {
            get { return false; }
        }

        public override bool RequiresUniqueEmail {
            get { return false; }
        }

        public override string ResetPassword(string username, string answer) {
            return string.Empty;
        }

        public override bool UnlockUser(string userName) {
            return true;
        }

        public override void UpdateUser(MembershipUser user) {
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override bool ValidateUser(string username, string password) {

            // {Grupo Entidade};{Numero do PV};{Id do Usuario}
            string[] dadosUsuario = username.Split(';');

            if (dadosUsuario.Length < 3)
                return false;


            string grupoEntidade = dadosUsuario[0];
            int numeroPV = Convert.ToInt32(dadosUsuario[1]);
            string idUsuario = dadosUsuario[2];

            //const int CodigoEstabelecimento = 1;

            RetornoLoginLegadoEstabelecimentoVO retLoginLegado = new RetornoLoginLegadoEstabelecimentoVO();
            retLoginLegado = SharePointWCFHelper.RealizaLoginEstabelecimento(numeroPV ,idUsuario, password);

            if (RedecardHelper.IsLoginSucess(retLoginLegado.CodigoRetorno)) {
                user = new MembershipUser("r", idUsuario, idUsuario, string.Empty, string.Empty,
                        string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
                return true;
            }
            else
                return false;
        }
    }
}