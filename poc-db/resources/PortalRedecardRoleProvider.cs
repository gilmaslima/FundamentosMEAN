#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [11/05/2012] – [André Garcia] – [Criação]
*/
#endregion

using System.Web.Security;

namespace Redecard.PN.DadosCadastrais.SharePoint {

    /// <summary>
    /// Novo provider de perfis construído para o Portal Redecard
    /// </summary>
    public class PortalRedecardRoleProvider : RoleProvider {

        #region Métodos Não Implantados

        public override void AddUsersToRoles(string[] usernames, string[] roleNames) {            
        }

        public override string ApplicationName {
            get;
            set;
        }

        public override void CreateRole(string roleName) {
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
            return null;
        }

        public override string[] GetAllRoles() {
            return null;
        }

        public override string[] GetRolesForUser(string username) {
            return null;
        }

        public override string[] GetUsersInRole(string roleName) {
            return null;
        }

        public override bool IsUserInRole(string username, string roleName) {
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
        }

        public override bool RoleExists(string roleName) {
            return false;
        }

        #endregion
    }
}