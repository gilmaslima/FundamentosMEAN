using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Security;
using Sybase.Data.AseClient;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Factory utilizado pelo Entlib para conectar com o Sybase
    /// </summary>
    public sealed class SybaseFactory : DbProviderFactory
    {
        /// <summary>
        /// Instância da factory utilizado pelo Entlib para conectar com o Sybase
        /// </summary>
        public static readonly SybaseFactory Instance = new SybaseFactory();

        #region Métodos

        /// <summary>
        /// 
        /// </summary>
        private SybaseFactory() { }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanCreateDataSourceEnumerator
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DbCommand CreateCommand()
        {
            return new AseCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new AseCommandBuilder();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            return new AseConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new SqlConnectionStringBuilder();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            return new AseDataAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DbParameter CreateParameter()
        {
            return new AseParameter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override CodeAccessPermission CreatePermission(System.Security.Permissions.PermissionState state)
        {
            return new SqlClientPermission(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return SqlDataSourceEnumerator.Instance;
        }

        #endregion
    }
}
