#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [30/08/2012] – [André Garcia] – [Criação]
- [26/11/2015] – [Rodrigo Rodrigues] – Migração do método ConsultarSQL() para novo projeto (Redecard.PN.Sustentacao.Dados)
*/
#endregion

using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Redecard.PN.Comum;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Redecard.PN.Sustentacao.Dados
{
    /// <summary>
    /// Métodos de usuários que possuem acesso ao banco de dados
    /// </summary>
    public class Administracao : BancoDeDadosBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bancoDados"></param>
        /// <param name="script"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public DataTable[] ConsultarSQL(String bancoDados, String script)
        {
            Database db = this.RecuperarBancoDados(bancoDados);
            DataTable[] collection = null;

            if (!object.ReferenceEquals(db, null))
            {
                using (DbCommand command = db.GetSqlStringCommand(script))
                {
                    DataSet ds = db.ExecuteDataSet(command);
                    if (ds.Tables.Count > 0) {
                        collection = new DataTable[ds.Tables.Count];
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            collection[i] = ds.Tables[i].Copy();
                        }
                    }
                }
            }
            return collection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        private Database RecuperarBancoDados(String bancoDados)
        {
            Database db = null;
            switch (bancoDados)
            {
                case "GSEDM":
                    db = this.SQLServerGSEDM();
                    break;
                case "SQLIS":
                    db = this.SQLServerIS();
                    break;
                case "SQLPN":
                    db = this.SQLServerPN();
                    break;
                case "SYBDR":
                    db = this.SybaseDR();
                    break;
                case "SYBEC":
                    db = this.SybaseEC();
                    break;
                case "SYBGE":
                    db = this.SybaseGE();
                    break;
                case "SYBGS":
                    db = this.SybaseGS();
                    break;
                case "SYBIS":
                    db = this.SybaseIS();
                    break;
                case "SYBTG":
                    db = this.SybaseTG();
                    break;
                case "SYBWM":
                    db = this.SybaseWM();
                    break;
                case "SYBFB":
                    db = this.SybaseFB();
                    break;
                case "SYBWF":
                    db = this.SybaseWF();
                    break;
                case "SYBRQ":
                    db = this.SybaseRQ();
                    break;
                case "ORLRQ":
                    db = this.OracleRQ();
                    break;
                case "ORLDR":
                    db = this.OracleDR();
                    break;
            }
            return db;
        }
    }
}
