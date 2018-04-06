#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [30/08/2012] – [André Garcia] – [Criação]
- [26/11/2015] – [Rodrigo Rodrigues] – Migração do método ConsultarSQL() para novo projeto (Redecard.PN.Sustentacao.Dados)
- [04/05/2016] – [Valdinei Ribeiro] – [Tratamento da opção de connection string na tela dados]
*/
#endregion

using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Redecard.PN.Comum;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Redecard.PN.Sustentacao.Modelo;
using System.Collections.Generic;

namespace Redecard.PN.Sustentacao.Dados
{
    /// <summary>
    /// Métodos de usuários que possuem acesso ao banco de dados
    /// </summary>
    /// <remarks>
    /// Métodos de usuários que possuem acesso ao banco de dados
    /// </remarks>
    public class SustentacaoAdministracao : BancoDeDadosBase
    {
        #region [ Constantes ]
        /// <summary>
        /// Código de erro genérico de banco de dados
        /// </summary>
        public const Int32 CODIGO_ERRO = 500;

        /// <summary>
        /// Fonte
        /// </summary>
        public new const String FONTE = "Redecard.PN.Sustentacao.Dados";
        #endregion

        /// <summary>
        /// Executa o script no banco de dados informado.
        /// </summary>
        /// <remarks>
        /// Executa o script no banco de dados informado.
        /// </remarks>
        /// <param name="bancoDados">Nome da connection string</param>
        /// <param name="script">Sql que será executado no banco.</param>
        /// <returns>Instância do objeto DataTable</returns>
        public DataTable[] ConsultarSql(String bancoDados, String script)
        {
            string[] conexao = bancoDados.Split('|');
            Database db = null;

            if (String.Compare(conexao[0].ToLower(), "cnx", true) == 0)
            {
                switch (conexao[2].ToLower())
                {
                    case "sybase":
                        db = base.RetornarBancoDeDadosSybase(conexao[1]);
                        break;
                    case "sqlserver":
                        db = base.RetornarBancoDeDadosSqlServer(conexao[1]);
                        break;
                    case "oracle":
                        db = base.RetornarBancoDeDadosOracle(conexao[1]);
                        break;
                    default:
                        db = null;
                        break;
                }
            }
            else
            {
                db = this.RecuperarBancoDados(conexao[0]);
            }

            DataTable[] collection = null;

            if (!object.ReferenceEquals(db, null))
            {
                using (DbCommand command = db.GetSqlStringCommand(script))
                {
                    DataSet ds = db.ExecuteDataSet(command);
                    if (ds.Tables.Count > 0)
                    {
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
        /// Executa um script no BD de SQL Server PN
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public DataTable[] ConsultarQuerySQLServer(String script, String usuarioLogado, String infoOperacao, String nomeBanco)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Dados via Tela Adminsitrativa"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                Database db = this.RecuperarBancoDados(nomeBanco);

                DataTable[] collection = null;

                if (!object.ReferenceEquals(db, null))
                {
                    Log.GravarLog(EventoLog.ChamadaDados, new
                    {
                        UsuarioLogado = usuarioLogado,
                        infoOperacao = infoOperacao
                    });

                    using (DbCommand command = db.GetSqlStringCommand(script))
                    {
                        DataSet ds = db.ExecuteDataSet(command);
                        if (ds.Tables.Count > 0)
                        {
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

        }

        /// <summary>
        /// Retorna uma instäncia do banco de dados baseado no nome da connection string.
        /// </summary>
        /// <remarks>
        /// Retorna uma instäncia do banco de dados baseado no nome da connection string.
        /// </remarks>
        /// <param name="script">Nome da connection string</param>
        /// <returns>Instäncia do objeto DataBase</returns>
        private Database RecuperarBancoDados(String bancoDados)
        {
            Database db = null;

            if (bancoDados.Contains("SQLServer"))
                db = base.ObterSqlServerDataBase(bancoDados);

            else if (bancoDados.Contains("Oracle"))
                db = base.ObterOracleDataBase(bancoDados);

            else if (bancoDados.Contains("Sybase"))
                db = base.ObterSybaseDataBase(bancoDados);

            return db;
        }

        /// <summary>
        /// Método que retorna registros da tabela TBPN003 e TBPN002 
        /// </summary>
        /// <param name="email">Parâmetro de pesquisa</param>
        /// <returns>Lista unificada de usuários e entidades</returns>
        public List<UsuarioPV> ConsultarUsuariosPorEmail(String email, Int32 pv, Int32 grupo)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Usuarios por email"))
            {
                List<UsuarioPV> result = null;

                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    SqlDatabase db = base.SQLServerPN();

                    using (DbCommand command = db.GetStoredProcCommand("sp_cons_usr_ent_por_email"))
                    {
                        db.AddInParameter(command, "@nom_eml_usr", DbType.AnsiString, email);
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, pv);
                        db.AddInParameter(command, "@cod_gru_etd", DbType.Int32, grupo);

                        Log.GravarLog(EventoLog.ChamadaDados, new
                        {
                            command.Parameters
                        });

                        using (DataSet ds = db.ExecuteDataSet(command))
                        {
                            IDataReader dataReader = ds.CreateDataReader();
                            result = new List<UsuarioPV>();

                            while (dataReader.Read())
                            {
                                result.Add(new UsuarioPV()
                                {
                                    ID = dataReader["cod_usr_id"].ToString().ToInt32(),
                                    Codigo = dataReader["cod_usr"].ToString(),
                                    Nome = dataReader["des_usr"].ToString(),
                                    Email = dataReader["nom_eml_usr"].ToString(),
                                    Status = dataReader["cod_status"].ToString().ToInt32(),
                                    QtdTentativasLoginErro = dataReader["qtd_snha_inv"].ToString().ToInt32(),
                                    UltimoLogin = dataReader["dth_ult_acesso"].ToString().ToDate(),
                                    UltimaAlteracao = dataReader["dth_ult_atlz"].ToString().ToDate(),
                                    CodigoPV = dataReader["cod_etd"].ToString().ToInt32(),
                                    NomePV = dataReader["des_etd"].ToString()
                                });
                            }

                            Log.GravarLog(EventoLog.FimDados, new
                            {
                                result
                            });

                            return result;
                        }
                    }
                }
                catch (DbException ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// étodo que retorna registros da tabela TBPN026
        /// </summary>
        /// <param name="dataInclusao"></param>
        /// <param name="codigoPv"></param>
        /// <param name="codigoPvAcesso"></param>
        /// <param name="email"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public List<RetornoCancelamento> ConsultarRetornoCancelamento(DateTime dataInclusao, Int32? codigoPv, Int32? codigoPvAcesso, String email, String ip)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Dados Cancelamento"))
            {
                List<RetornoCancelamento> result = null;

                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    SqlDatabase db = base.SQLServerPN();

                    using (DbCommand command = db.GetStoredProcCommand("sp_cons_cancelamento"))
                    {
                        db.AddInParameter(command, "@dth_inclusao", DbType.DateTime, dataInclusao);
                        db.AddInParameter(command, "@cod_etd_usr_rspn", DbType.Int32, codigoPv);
                        db.AddInParameter(command, "@cod_etd_mtz_usr_rspn", DbType.Int32, codigoPvAcesso);
                        db.AddInParameter(command, "@emal_usr_rspn", DbType.String, email);
                        db.AddInParameter(command, "@ip_usr_rspn", DbType.String, ip);

                        Log.GravarLog(EventoLog.ChamadaDados, new
                        {
                            command.Parameters
                        });

                        using (DataSet ds = db.ExecuteDataSet(command))
                        {
                            IDataReader dataReader = ds.CreateDataReader();
                            result = new List<RetornoCancelamento>();

                            while (dataReader.Read())
                            {
                                result.Add(new RetornoCancelamento()
                                {
                                    CodigoPv = dataReader["cod_etd_usr_rspn"].ToString().ToInt32(),
                                    CodigoPvAcesso = dataReader["cod_etd_mtz_usr_rspn"].ToString().ToInt32(),
                                    Email = dataReader["emal_usr_rspn"].ToString(),
                                    Ip = dataReader["ip_usr_rspn"].ToString(),
                                    DataInclusao = dataReader["dth_inclusao"].ToString().ToDate()
                                });
                            }

                            Log.GravarLog(EventoLog.FimDados, new
                            {
                                result
                            });

                            return result;
                        }
                    }
                }
                catch (DbException ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Método utilizado para alterar status e quantidade de tentativas de senhas na tabela TBPN003
        /// </summary>
        /// <param name="codUsrId">Identificacao do usuario na tabela TBPN003</param>
        /// <returns>Código de retorno</returns>
        public Int32 DesbloquearUsuario(Int32 codUsrId, Int32 codEntidade, String usuarioLogado, String nomEmlUsr)
        {
            using (Logger Log = Logger.IniciarLog("Desbloquear Usuario"))
            {
                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    SqlDatabase db = base.SQLServerPN();

                    using (DbCommand command = db.GetStoredProcCommand("sp_alt_desb_usr"))
                    {
                        db.AddInParameter(command, "@cod_usr_id", DbType.Int32, codUsrId);
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, codEntidade);

                        Log.GravarLog(EventoLog.ChamadaDados, new
                        {
                            UsuarioLogado = usuarioLogado,
                            cod_usr_id = codUsrId,
                            nom_eml_usr = nomEmlUsr,
                            cod_etd = codEntidade
                        });

                        Int32 result = db.ExecuteScalar(command).ToString().ToInt32();

                        Log.GravarLog(EventoLog.FimDados, new
                        {
                            result
                        });

                        return result;
                    }
                }
                catch (DbException ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Método utilizado para alterar status e quantidade de tentativas de senhas na tabela TBPN003
        /// </summary>
        /// <param name="codUsrId">Identificacao do usuario na tabela TBPN003</param>
        /// <returns>Código de retorno</returns>
        public Int32 ExcluirUsuario(Int32 codUsrId, String usuarioLogado, String nomEmlUsr)
        {
            using (Logger Log = Logger.IniciarLog("Excluir Usuario"))
            {
                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    SqlDatabase db = base.SQLServerPN();

                    using (DbCommand command = db.GetStoredProcCommand("sp_exc_usuario_pn"))
                    {
                        db.AddInParameter(command, "@cod_usr_id", DbType.Int32, codUsrId);

                        Log.GravarLog(EventoLog.ChamadaDados, new
                        {
                            UsuarioLogado = usuarioLogado,
                            cod_usr_id = codUsrId,
                            nom_eml_usr = nomEmlUsr
                        });

                        Int32 result = db.ExecuteScalar(command).ToString().ToInt32();

                        Log.GravarLog(EventoLog.FimDados, new
                        {
                            result
                        });

                        return result;
                    }
                }
                catch (DbException ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codUsrId"></param>
        /// <param name="usuarioLogado"></param>
        /// <param name="nomEmlUsr"></param>
        /// <returns></returns>
        public Int32 ExcluirRelEtd(Int32 codUsrId, Int32 codEtd, Int32 grupo, String usuarioLogado, String nomEmlUsr)
        {
            using (Logger Log = Logger.IniciarLog("Excluir Relacionamento da TBPN021"))
            {
                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    SqlDatabase db = base.SQLServerPN();

                    using (DbCommand command = db.GetStoredProcCommand("sp_exc_rel_usr_etd"))
                    {
                        db.AddInParameter(command, "@cod_usr_id", DbType.Int32, codUsrId);
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, codEtd);
                        db.AddInParameter(command, "@cod_gru_etd", DbType.Int32, grupo);

                        Log.GravarLog(EventoLog.ChamadaDados, new
                        {
                            UsuarioLogado = usuarioLogado,
                            cod_usr_id = codUsrId,
                            cod_etd = codEtd,
                            cod_gru_etd = grupo,
                            nom_eml_usr = nomEmlUsr
                        });

                        Int32 result = db.ExecuteScalar(command).ToString().ToInt32();

                        Log.GravarLog(EventoLog.FimDados, new
                        {
                            result
                        });

                        return result;
                    }
                }
                catch (DbException ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Expurgo da tabela de log
        /// </summary>
        /// <remarks>
        /// Expurgo da tabela de log
        /// </remarks>
        public void Expurgo()
        {
            using (Logger Log = Logger.IniciarLog("Expurgo da tabela de log"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    SqlDatabase db = base.SQLServerLog();

                    using (DbCommand command = db.GetStoredProcCommand("sp_expurgo"))
                    {
                        Int32 result = db.ExecuteScalar(command).ToString().ToInt32();

                        Log.GravarLog(EventoLog.RetornoDados);

                        Log.GravarLog(EventoLog.FimDados, new { result });
                    }
                }
                catch (SqlException sqlEx)
                {
                    Log.GravarErro(sqlEx);
                    throw new PortalRedecardException(500, FONTE, sqlEx);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(500, FONTE, ex);
                }
            }
        }
    }
}
