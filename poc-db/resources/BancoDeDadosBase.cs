using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.OleDb;

using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OracleClient;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe base de Dados.
    /// </summary>
    public class BancoDeDadosBase
    {
        /// <summary>
        /// Constante com o nome completo da classe
        /// </summary>
        /// <value>Redecard.PN.Dados</value>
        public const String FONTE = "Redecard.PN.Dados";

        /// <summary>
        /// Siglas dos bancos de dados que poderão ser acessados.
        /// </summary>
        public enum ChaveStringConexao
        {
            /// <summary>Banco SybaseGS</summary>
            SybaseGS,
            /// <summary>Banco SybaseIS</summary>
            SybaseIS,
            /// <summary>Banco SybaseTG</summary>
            SybaseTG,
            /// <summary>Banco SybaseGE</summary>
            SybaseGE,
            /// <summary>Banco SybaseEC</summary>
            SybaseEC,
            /// <summary>Banco SybaseDR</summary>
            SybaseDR,
            /// <summary>Banco SybaseWM</summary>
            SybaseWM,
            /// <summary>Banco SQLServerIS</summary>
            SQLServerIS,
            /// <summary>Banco SQLServerPN</summary>
            SQLServerPN,
            /// <summary>Banco SQLServerGSEDM</summary>
            SQLServerGSEDM,
            /// <summary>Banco SybaseFB</summary>
            SybaseFB,
            /// <summary>Banco SybaseWF</summary>
            SybaseWF,
            /// <summary>Banco SybaseRQ</summary>
            SybaseRQ,
            /// <summary>Banco OracleRQ</summary>
            OracleRQ,
            /// <summary>Banco OracleRQ</summary>
            OracleDR,
            /// <summary>Banco SQLServer DBPNLog001</summary>
            SQLServerLog,
            /// <summary>
            /// Banco de Usuários SQLServer da Sigla IZ
            /// </summary>
            SQLServerIZ_INDB001,
            /// <summary>
            /// Banco de Sessão SQLServer da Sigla IZ
            /// </summary>
            SQLServerIZ_IZDB001
        }      

        /// <summary>
        /// Utiliza o SyncPass para recuperar a senha e criar a string de conexão.
        /// </summary>
        /// <param name="chave">Enum ChaveStringConexao que conterá as siglas dos bancos de dados que poderão ser acessados</param>
        /// <param name="arquivoPWD">Nome do pwd dos bancos de dados que poderão ser acessados</param>
        /// <returns>GenericDatabase</returns>
        private GenericDatabase RetornarBancoDeDados(ChaveStringConexao chave, String arquivoPWD, DbProviderFactory factory)
        {
            return new GenericDatabase(
                Util.RetornaConnectionStringSyncPass(
                    ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(ChaveStringConexao), chave)].ConnectionString, 
                    arquivoPWD),
                factory);
        }

        /// <summary>
        /// Utiliza o SyncPass para recuperar a senha e criar a string de conexão.
        /// </summary>
        /// <param name="chave">Enum ChaveStringConexao que conterá as siglas dos bancos de dados que poderão ser acessados</param>
        /// <param name="arquivoPWD">Nome do pwd dos bancos de dados que poderão ser acessados</param>
        /// <returns>GenericDatabase</returns>
        private GenericDatabase RetornarBancoDeDados(ChaveStringConexao chave, String arquivoPWD)
        {
            return this.RetornarBancoDeDados(chave, arquivoPWD, OleDbFactory.Instance);
        }

        /// <summary>
        /// Utiliza o SyncPass para recuperar a senha e criar a string de conexão Oracle.
        /// </summary>
        /// <param name="chave">Enum ChaveStringConexao que conterá as siglas dos bancos de dados que poderão ser acessados</param>
        /// <param name="arquivoPWD">Nome do pwd dos bancos de dados que poderão ser acessados</param>
        /// <returns>OracleDatabase</returns>
        private OracleDatabase RetornarBancoDeDadosOracle(ChaveStringConexao chave, String arquivoPWD)
        {
            return new OracleDatabase(
                Util.RetornaConnectionStringSyncPass(
                    ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(ChaveStringConexao), chave)].ConnectionString,
                    arquivoPWD));
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Sybase IS.
        /// </summary>
        /// <returns>Banco de dados Sysbase IS</returns>
        [Obsolete("Por favor, utilize o método SQLServerPN.")]
        public GenericDatabase SybaseIS()
        {
            return this.RetornarBancoDeDados(ChaveStringConexao.SybaseIS, 
                ConfigurationManager.AppSettings.Get("PWDSybaseIS"), SybaseFactory.Instance);
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Sybase TG.
        /// </summary>
        /// <returns>Banco de dados Sysbase TG</returns>
        public GenericDatabase SybaseTG()
        {
            return this.RetornarBancoDeDados(ChaveStringConexao.SybaseTG,
                ConfigurationManager.AppSettings.Get("PWDSybaseTG"), SybaseFactory.Instance);
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Sybase GS.
        /// </summary>
        /// <returns>Banco de dados Sysbase TG</returns>
        public GenericDatabase SybaseGS()
        {
            return this.RetornarBancoDeDados(ChaveStringConexao.SybaseGS,
            ConfigurationManager.AppSettings.Get("PWDSybaseGS"), SybaseFactory.Instance);
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Sybase GE.
        /// </summary>
        /// <returns>Banco de dados Sysbase GE</returns>
        public GenericDatabase SybaseGE()
        {
            return this.RetornarBancoDeDados(ChaveStringConexao.SybaseGE,
                ConfigurationManager.AppSettings.Get("PWDSybaseGE"), SybaseFactory.Instance);
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Sybase EC.
        /// </summary>
        /// <returns>Banco de dados Sysbase EC</returns>
        public GenericDatabase SybaseEC()
        {
            return this.RetornarBancoDeDados(ChaveStringConexao.SybaseEC,
                ConfigurationManager.AppSettings.Get("PWDSybaseEC"), SybaseFactory.Instance);
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Sybase DR.
        /// </summary>
        /// <returns>Banco de dados Sysbase DR</returns>
        public GenericDatabase SybaseDR()
        {
            return this.RetornarBancoDeDados(ChaveStringConexao.SybaseDR,
                ConfigurationManager.AppSettings.Get("PWDSybaseDR"), SybaseFactory.Instance);
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Sybase WM.
        /// </summary>
        /// <returns>Banco de dados Sysbase WM</returns>
        public GenericDatabase SybaseWM()
        {
            return this.RetornarBancoDeDados(ChaveStringConexao.SybaseWM,
                ConfigurationManager.AppSettings.Get("PWDSybaseWM"), SybaseFactory.Instance);
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Sybase FB.
        /// </summary>
        /// <returns>Banco de dados Sysbase FB</returns>
        public GenericDatabase SybaseFB()
        {
            return this.RetornarBancoDeDados(ChaveStringConexao.SybaseFB,
                ConfigurationManager.AppSettings.Get("PWDSybaseFB"), SybaseFactory.Instance);
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Sybase WF.
        /// </summary>
        /// <returns>Banco de dados Sysbase WF</returns>
        public GenericDatabase SybaseWF()
        {
            return this.RetornarBancoDeDados(ChaveStringConexao.SybaseWF,
                ConfigurationManager.AppSettings.Get("PWDSybaseWF"), SybaseFactory.Instance);
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Sybase RQ.
        /// </summary>
        /// <returns>Banco de dados Sysbase RQ</returns>
        public GenericDatabase SybaseRQ()
        {
            return this.RetornarBancoDeDados(ChaveStringConexao.SybaseRQ,
                ConfigurationManager.AppSettings.Get("PWDSybaseRQ"), SybaseFactory.Instance);
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Oracle RQ.
        /// </summary>
        /// <returns>Banco de dados Sysbase RQ</returns>
        public OracleDatabase OracleRQ()
        {
            return this.RetornarBancoDeDadosOracle(ChaveStringConexao.OracleRQ,
                ConfigurationManager.AppSettings.Get("PWDOracleRQ"));
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do Oracle DR.
        /// </summary>
        /// <returns>Banco de dados Sysbase DR</returns>
        public OracleDatabase OracleDR()
        {
            return this.RetornarBancoDeDadosOracle(ChaveStringConexao.OracleDR,
                ConfigurationManager.AppSettings.Get("PWDOracleDR"));
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do SQL Server IS.
        /// </summary>
        /// <returns>Banco de dados SQL Server IS</returns>
        [Obsolete("Por favor, utilize o método SQLServerPN.")]
        public SqlDatabase SQLServerIS()
        {
            String stringConexao = Util.RetornaConnectionStringSyncPass(
                ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(ChaveStringConexao), ChaveStringConexao.SQLServerIS)].ConnectionString,
                ConfigurationManager.AppSettings.Get("PWDSQLServerIS"));

            SqlDatabase sqlServerDB = new SqlDatabase(stringConexao);

            return sqlServerDB;
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do SQL Server PN do Sybase.
        /// </summary>
        /// <returns>Banco de dados SQL Server PN</returns>
        public SqlDatabase SQLServerPN()
        {
            String stringConexao = Util.RetornaConnectionStringSyncPass(
                ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(ChaveStringConexao), ChaveStringConexao.SQLServerPN)].ConnectionString,
                ConfigurationManager.AppSettings.Get("PWDSQLServerPN"));

            SqlDatabase sqlServerDB = new SqlDatabase(stringConexao);

            return sqlServerDB;
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do SQL Server IZ INDB001.
        /// </summary>
        public SqlDatabase SQLServerIZ_INDB001()
        {
            String stringConexao = Util.RetornaConnectionStringSyncPass(
                ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(ChaveStringConexao), ChaveStringConexao.SQLServerIZ_INDB001)].ConnectionString,
                ConfigurationManager.AppSettings.Get("PWDSQLServerIZ_INDB001"));

            SqlDatabase sqlServerDB = new SqlDatabase(stringConexao);

            return sqlServerDB;
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do SQL Server IZ IZDB001.
        /// </summary>
        public SqlDatabase SQLServerIZ_IZDB001()
        {
            String stringConexao = Util.RetornaConnectionStringSyncPass(
                ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(ChaveStringConexao), ChaveStringConexao.SQLServerIZ_IZDB001)].ConnectionString,
                ConfigurationManager.AppSettings.Get("PWDSQLServerIZ_IZDB001"));

            SqlDatabase sqlServerDB = new SqlDatabase(stringConexao);

            return sqlServerDB;
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do SQL Server GSEDM.
        /// </summary>
        /// <returns>Banco de dados SQL Server GSEDM</returns>
        public SqlDatabase SQLServerGSEDM()
        {
            String stringConexao = Util.RetornaConnectionStringSyncPass(
                ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(ChaveStringConexao), ChaveStringConexao.SQLServerGSEDM)].ConnectionString,
                ConfigurationManager.AppSettings.Get("PWDSQLServerGSEDM"));

            SqlDatabase sqlServerDB = new SqlDatabase(stringConexao);

            return sqlServerDB;
        }

        /// <summary>
        /// Método para retornar a conexão de banco de dados do SQL Server de Log.
        /// </summary>
        /// <returns>Banco de dados SQL Server DB PN Log 001</returns>
        public SqlDatabase SQLServerLog()
        {
            String stringConexao = Util.RetornaConnectionStringSyncPass(
                ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(ChaveStringConexao), ChaveStringConexao.SQLServerLog)].ConnectionString,
                ConfigurationManager.AppSettings.Get("PWDSQLServerLog"));

            SqlDatabase sqlServerDB = new SqlDatabase(stringConexao);

            return sqlServerDB;
        }
    }

    /// <summary>
    /// Classe base de Dados
    /// </summary>
    public class BancoDeDadosBase<Interface, Class> : BancoDeDadosBase where Class : Interface
    {
        /// <summary>
        /// Obtém instância da classe
        /// </summary>
        public static Interface Instancia { get { return ClassFactory.GetInstance<Interface, Class>(); } }
    }
}
