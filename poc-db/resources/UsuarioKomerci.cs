#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [30/05/2012] – [Agnaldo Costa] – [Criação]
 *
 * 
*/
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   : Padronização para o tratamento de erro
- [11/06/2012] – [André Rentes] – [Alteração]
 *
 * 
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.Dados
{
    public class UsuarioKomerci : BancoDeDadosBase
    {
        /// <summary>
        /// Consulta usuários do sistema Komerci
        /// </summary>
        /// <param name="codigo">Codigo do usuário</param>
        /// <param name="entidade">Entidade do usuário</param>
        /// <returns>Listagem de usuário(s)</returns>
        public List<Modelo.UsuarioKomerci> Consultar(String codigo, Modelo.Entidade entidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta usuários do sistema Komerci"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    codigoRetorno = 0;

                    var usuario = new Modelo.UsuarioKomerci();

                    SqlDatabase db = base.SQLServerPN();

                    using (DbCommand command = db.GetStoredProcCommand("sp_cons_usuario_ko"))
                    {

                        // Cria parâmetros necessários
                        db.AddInParameter(command, "@cod_usr", DbType.AnsiString, codigo);

                        if (entidade != null)
                        {
                            db.AddInParameter(command, "@cod_etd", DbType.Int32, entidade.Codigo);
                            db.AddInParameter(command, "@cod_gru_etd", DbType.Int32, entidade.GrupoEntidade.Codigo);
                        }

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        using (DataSet ds = db.ExecuteDataSet(command))
                        {
                            if (ds.Tables.Count > 1)
                                codigoRetorno = ds.Tables[1].Rows[0][0].ToString().ToInt16();

                            Log.GravarLog(EventoLog.RetornoDados, new { codigoRetorno });
                            var result = PreencheModelo(ds.CreateDataReader());
                            Log.GravarLog(EventoLog.FimDados, new { codigoRetorno, result });
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(500, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta login KO usuários do sistema Komerci
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="senhaUsuario">Senha do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Código de retorno</returns>
        public void AutenticarUsuarioKO(Int32 codigoEntidade, String codigoUsuario, String senhaUsuario, out Int32 codigoRetorno, out String mensagemRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Login KO usuários do sistema Komerci"))
            {
                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    codigoRetorno = 0;

                    SqlDatabase db = base.SQLServerPN();

                    using (DbCommand command = db.GetStoredProcCommand("sp_login_acesso_ko"))
                    {
                        // Cria parâmetros necessários
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, codigoEntidade);
                        db.AddInParameter(command, "@cod_usr", DbType.AnsiString, codigoUsuario);
                        db.AddInParameter(command, "@nom_snha_usr", DbType.AnsiString, senhaUsuario);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });

                        codigoRetorno = db.ExecuteScalar(command).ToString().ToInt32();
                        mensagemRetorno = "";

                        Log.GravarLog(EventoLog.RetornoDados);
                        Log.GravarLog(EventoLog.FimDados, new { codigoRetorno, mensagemRetorno });
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(500, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Preenche uma lista de Usuários Komerci a partir de um Reader
        /// </summary>
        /// <param name="leitor">DataReader preenchido</param>
        /// <returns>Listagem de usuário(s) do Komerci</returns>
        private List<Modelo.UsuarioKomerci> PreencheModelo(IDataReader leitor)
        {
            Modelo.UsuarioKomerci usuario = null;
            var usuarios = new List<Modelo.UsuarioKomerci>();

            while (leitor.Read())
            {
                usuario = new Modelo.UsuarioKomerci();

                usuario.Codigo = leitor["cod_usr"].ToString();
                usuario.Entidade = new Modelo.Entidade(
                                        leitor["cod_etd"].ToString().ToInt32(),
                                        new Modelo.GrupoEntidade(leitor["cod_gru_etd"].ToString().ToInt32())
                                    );

                usuario.Descricao = leitor["des_usr"].ToString();
                usuario.Senha = leitor["nom_snha_usr"].ToString();

                usuarios.Add(usuario);
            }

            return usuarios;
        }
        /// <summary>
        /// Preenche uma lista de Usuários Komerci a partir de um Reader
        /// </summary>
        /// <param name="leitor">DataReader preenchido</param>
        /// <returns>Listagem de usuário(s) do Komerci</returns>
        private List<Modelo.UsuarioKomerci> PreencheModeloEC(IDataReader leitor)
        {
            Modelo.UsuarioKomerci usuario = null;
            var usuarios = new List<Modelo.UsuarioKomerci>();

            while (leitor.Read())
            {
                usuario = new Modelo.UsuarioKomerci();

                usuario.Codigo = leitor["cod_usr"].ToString();
                usuario.Entidade = new Modelo.Entidade(
                                        leitor["cod_etd"].ToString().ToInt32()
                                    );

                usuario.Descricao = leitor["des_usr"].ToString();
                usuario.Senha = leitor["nom_snha_usr"].ToString();
                usuario.TipoUsuario = leitor["tip_usr"].ToString();

                usuarios.Add(usuario);
            }

            return usuarios;
        }

        /// <summary>
        /// Inseri um usuário do Komerci
        /// </summary>
        /// <param name="usuario">Objeto Usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 1 - Usuario já Existente
        /// 99 - Erro não previsto
        /// </returns>
        public Int32 Inserir(Modelo.UsuarioKomerci usuario, String senha)
        {
            using (Logger Log = Logger.IniciarLog("Inseri um usuário do Komerci"))
            {
                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    SqlDatabase db = base.SQLServerPN();

                    using (DbCommand command = db.GetStoredProcCommand("sp_ins_usuario_ko"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@cod_usr", DbType.AnsiString, usuario.Codigo);
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, usuario.Entidade.Codigo);
                        db.AddInParameter(command, "@cod_gru_etd", DbType.Int32, usuario.Entidade.GrupoEntidade.Codigo);
                        db.AddInParameter(command, "@des_usr", DbType.AnsiString, usuario.Descricao);
                        db.AddInParameter(command, "@nom_snha_usr", DbType.AnsiString, senha);
                        db.AddInParameter(command, "@nom_rspn_ult_atlz", DbType.AnsiString, usuario.NomeResponsavel);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        var result = db.ExecuteScalar(command).ToString().ToInt32();
                        Log.GravarLog(EventoLog.RetornoDados);
                        Log.GravarLog(EventoLog.FimDados, new { result });
                        return result; 
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(500, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Atualiza o usuário do Komerci
        /// </summary>
        /// <param name="usuario">Usuário do Komerci</param>
        /// <param name="senha">Nova Senha do Usuário</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 2 - Usuário não existe
        /// 99 - Erro não previsto
        /// </returns>
        public Int32 Atualizar(Modelo.UsuarioKomerci usuario, String senha)
        {
            using (Logger Log = Logger.IniciarLog("Atualiza o usuário do Komerci"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    SqlDatabase db = base.SQLServerPN();

                    using (DbCommand command = db.GetStoredProcCommand("sp_alt_usuario_ko"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@cod_usr", DbType.AnsiString, usuario.Codigo);
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, usuario.Entidade.Codigo);
                        db.AddInParameter(command, "@cod_gru_etd", DbType.Int32, usuario.Entidade.GrupoEntidade.Codigo);
                        db.AddInParameter(command, "@des_usr", DbType.AnsiString, usuario.Descricao);
                        db.AddInParameter(command, "@nom_snha_usr", DbType.AnsiString, senha);
                        db.AddInParameter(command, "@nom_rspn_ult_atlz", DbType.AnsiString, usuario.NomeResponsavel);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        Int32 result = db.ExecuteScalar(command).ToString().ToInt32();
                        Log.GravarLog(EventoLog.RetornoDados);

                        Log.GravarLog(EventoLog.FimDados, new { result });
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(500, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Exclui usuário(s) do Komerci
        /// </summary>
        /// <param name="codigos">Código(s) do(s) usuário(s) do Komerci</param>
        /// <param name="entidade">Entidade do usuário</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 2 - Usuário não existe
        /// 99 - Erro não previsto
        /// </returns>
        public Int32 ExcluirEmLote(String codigos, Modelo.Entidade entidade)
        {
            using (Logger Log = Logger.IniciarLog("Exclui usuário(s) do Komerci"))
            {
                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    SqlDatabase db = base.SQLServerPN();

                    using (DbCommand command = db.GetStoredProcCommand("sp_exc_usu_ko_lote_pn"))
                    {
                        // Cria parâmetros necessários
                        db.AddInParameter(command, "@cod_usr", DbType.AnsiString, codigos);
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, entidade.Codigo);
                        db.AddInParameter(command, "@cod_gru_etd", DbType.Int32, entidade.GrupoEntidade.Codigo);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        Int32 result = db.ExecuteScalar(command).ToString().ToInt32();
                        Log.GravarLog(EventoLog.RetornoDados);
                        Log.GravarLog(EventoLog.FimDados, new { result });
                        return result; 
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(500, FONTE, ex);
                }
            }
        }

        #region Métodos para acesso à Base EC
        /// <summary>
        /// Lista Usuários Komerci da base EC 
        /// </summary>
        /// <param name="codigo">Codigo do usuário</param>
        /// <param name="entidade">Entidade do usuário</param>
        /// <returns>Listagem de usuário(s)  Komerci da base EC </returns>
        public List<Modelo.UsuarioKomerci> ConsultarUsuarioEC(string codigo, Modelo.Entidade entidade, int local_flg, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Lista Usuários Komerci da base EC "))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    codigoRetorno = 0;

                    GenericDatabase db = base.SybaseEC();

                    using (DbCommand command = db.GetStoredProcCommand("sp_cons_usuario"))
                    {

                        // Cria parâmetros necessários
                        if (!string.IsNullOrEmpty(codigo))
                            db.AddInParameter(command, "@cod_usr", DbType.AnsiString, codigo);
                        else
                            db.AddInParameter(command, "@cod_usr", DbType.AnsiString, DBNull.Value);
                        if (entidade != null)
                        {
                            db.AddInParameter(command, "@cod_etd", DbType.Int32, entidade.Codigo);
                        }
                        else
                        {
                            db.AddInParameter(command, "@cod_etd", DbType.Int32, DBNull.Value);
                        }
                        if (local_flg > 0)
                        {
                            db.AddInParameter(command, "@local_flg", DbType.Int32, local_flg);
                        }

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        using (DataSet ds = db.ExecuteDataSet(command))
                        {
                            if (ds.Tables.Count > 1)
                                codigoRetorno = ds.Tables[1].Rows[0][0].ToString().ToInt16();

                            Log.GravarLog(EventoLog.RetornoDados, new { codigoRetorno });
                            var result = PreencheModeloEC(ds.CreateDataReader());
                            Log.GravarLog(EventoLog.FimDados, new { codigoRetorno, result });

                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(504, FONTE, ex);
                }
            }
        }


        /// <summary>
        /// Atualiza o usuário do Komerci da Base EC
        /// </summary>
        /// <param name="usuario">Usuário do Komerci</param>
        /// <param name="senha">Nova Senha do Usuário</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 2 - Usuário não existe
        /// 99 - Erro não previsto
        /// </returns>
        public Int32 AtualizarUsuarioEC(Modelo.UsuarioKomerci usuario, int local_flg)
        {
            using (Logger Log = Logger.IniciarLog("Atualiza o usuário do Komerci da Base EC"))
            {
                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    GenericDatabase db = base.SybaseEC();

                    using (DbCommand command = db.GetStoredProcCommand("sp_alt_usuario"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@cod_usr", DbType.AnsiString, usuario.Codigo);
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, usuario.Entidade.Codigo);
                        db.AddInParameter(command, "@des_usr", DbType.AnsiString, usuario.Descricao);
                        db.AddInParameter(command, "@tip_usr", DbType.AnsiString, usuario.TipoUsuario);
                        db.AddInParameter(command, "@nom_snha_usr", DbType.AnsiString, usuario.Senha);
                        db.AddInParameter(command, "@nom_rspn_ult_atlz", DbType.AnsiString, usuario.NomeResponsavel);

                        db.AddInParameter(command, "@local_flg", DbType.Int32, local_flg);
                        db.AddParameter(command, "@RETURN_VALUE", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, null);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });

                        object objRet = db.ExecuteScalar(command);
                        Log.GravarLog(EventoLog.RetornoDados, new { objRet });

                        int ret = objRet == null ? (int)db.GetParameterValue(command, "RETURN_VALUE") : objRet.ToString().ToInt32();
                        Log.GravarLog(EventoLog.FimDados, new { ret });
                        return ret;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(504, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Insere um usuário do Komerci na Base EC
        /// </summary>
        /// <param name="usuario">Objeto Usuário</param>
        /// /// <param name="local_flg">Se é Local</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 1 - Usuario já Existente
        /// 99 - Erro não previsto
        /// </returns>
        public Int32 InserirUsuarioEC(Modelo.UsuarioKomerci usuario, int local_flg)
        {
            using (Logger Log = Logger.IniciarLog("Insere um usuário do Komerci na Base EC"))
            {
                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    GenericDatabase db = base.SybaseEC();

                    using (DbCommand command = db.GetStoredProcCommand("sp_ins_usuario"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@cod_usr", DbType.AnsiString, usuario.Codigo);
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, usuario.Entidade.Codigo);
                        db.AddInParameter(command, "@des_usr", DbType.AnsiString, usuario.Descricao);
                        db.AddInParameter(command, "@tip_usr", DbType.AnsiString, usuario.TipoUsuario);
                        db.AddInParameter(command, "@nom_snha_usr", DbType.AnsiString, usuario.Senha);
                        db.AddInParameter(command, "@nom_rspn_ult_atlz", DbType.AnsiString, usuario.NomeResponsavel);

                        db.AddInParameter(command, "@local_flg", DbType.Int32, local_flg);
                        db.AddParameter(command, "@RETURN_VALUE", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, null);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        object objRet = db.ExecuteScalar(command);
                        Log.GravarLog(EventoLog.RetornoDados, new { objRet });

                        int ret = objRet == null ? (int)db.GetParameterValue(command, "RETURN_VALUE") : objRet.ToString().ToInt32();
                        Log.GravarLog(EventoLog.FimDados, new { ret });
                        return ret;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(504, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Exclui usuário(s) do Komerci da base EC
        /// </summary>
        /// <param name="codigo">Código do usuário do Komerci EC</param>
        /// <param name="entidade">Entidade do usuário</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 2 - Usuário não existe
        /// 99 - Erro não previsto
        /// </returns>
        public Int32 ExcluirUsuario(String codigo, Modelo.Entidade entidade, string nomeResponsavel, int local_flg)
        {
            using (Logger Log = Logger.IniciarLog("Exclui usuário(s) do Komerci da base EC"))
            {
                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    GenericDatabase db = base.SybaseEC();

                    using (DbCommand command = db.GetStoredProcCommand("sp_exc_usuario"))
                    {
                        // Cria parâmetros necessários
                        db.AddInParameter(command, "@cod_usr", DbType.AnsiString, codigo);
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, entidade.Codigo);
                        db.AddInParameter(command, "@nom_rspn_ult_atlz", DbType.AnsiString, nomeResponsavel);
                        db.AddInParameter(command, "@local_flg", DbType.Int32, local_flg);
                        db.AddParameter(command, "@RETURN_VALUE", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, null);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        object objRet = db.ExecuteScalar(command);
                        Log.GravarLog(EventoLog.RetornoDados, new { objRet });
                        
                        int ret = objRet == null ? (int)db.GetParameterValue(command, "RETURN_VALUE") : objRet.ToString().ToInt32();
                        Log.GravarLog(EventoLog.FimDados, new { ret });
                        return ret;

                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(504, FONTE, ex);
                }
            }
        }
        #endregion

    }
}
