using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Redecard.PN.OutrosServicos.Modelo;

namespace Redecard.PN.OutrosServicos.Dados
{
    public class Regime : BancoDeDadosBase
    {
        /// <summary>
        /// Consulta a Restrição de Serviço para a Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <returns>Dados da Restrição de Regime</returns>
        public RestricaoRegime ConsultarRestricaoRegime(Int32 codigoEntidade, Int32 codigoGrupoEntidade)
        {
            RestricaoRegime restricao = null;


            using (Logger Log = Logger.IniciarLog("Consulta a Restrição de Serviço para a Entidade"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    SqlDatabase db = base.SQLServerIS();

                    using (DbCommand cmd = db.GetStoredProcCommand("SPIS_VerificarRestricao_SEL"))
                    {
                        db.AddInParameter(cmd, "cod_restricao", DbType.Int32, 2);
                        db.AddInParameter(cmd, "cod_gru_etd", DbType.Int32, codigoGrupoEntidade);
                        db.AddInParameter(cmd, "cod_etd", DbType.Int32, codigoEntidade);
                        db.AddInParameter(cmd, "cod_usr", DbType.AnsiString, "INTERNET");

                        Log.GravarLog(EventoLog.ChamadaDados, new { cmd.Parameters });
                        using (IDataReader dr = db.ExecuteReader(cmd))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            if (dr.Read())
                            {
                                restricao = new RestricaoRegime();
                                restricao.CodigoRestricao = dr["cod_restricao"].ToString().ToInt32();
                                restricao.CodigoVersao = dr["cod_versao"].ToString().ToInt32();
                                restricao.TipoRestricao = dr["tip_restricao"].ToString();
                            }
                        }
                    }

                    Log.GravarLog(EventoLog.FimDados, new { restricao });
                    return restricao;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(503, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta Código de Regime atual da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoRetorno">Código de erro retornado pela proc</param>
        /// <returns>Código do Regime</returns>
        public Int32 ConsultarCodigoRegime(Int32 codigoEntidade, out Int32 codigoRetorno)
        {

            using (Logger Log = Logger.IniciarLog("Consulta Código de Regime atual da Entidade"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    GenericDatabase db = base.SybaseGE();
                    Int32 codigoRegime = 0;
                    codigoRetorno = 0;

                    using (DbCommand cmd = db.GetStoredProcCommand("spge6037"))
                    {
                        db.AddInParameter(cmd, "@NUM_PDV", DbType.Int32, codigoEntidade);
                        //Código de Serviço fixo em 1
                        db.AddInParameter(cmd, "@COD_SERV", DbType.Int32, 1);

                        Log.GravarLog(EventoLog.ChamadaDados, new { cmd.Parameters });
                        using (IDataReader dr = db.ExecuteReader(cmd))
                        {
                            Log.GravarLog(EventoLog.RetornoDados, new { codigoRetorno });
                            codigoRetorno = -1;
                            if (dr.Read())
                            {
                                codigoRegime = dr["REGATU"].ToString().ToInt32();
                                codigoRetorno = 0;
                            }

                        }
                    }

                    Log.GravarLog(EventoLog.FimDados, new {codigoRetorno, codigoRegime });
                    return codigoRegime;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(501, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Atualiza o Regime de franquia da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoRegime">Código do regime</param>
        /// <param name="codigoCelula">Código da célula</param>
        /// <param name="codigoCanal">Código do canal</param>
        /// <param name="codigoUsuario">Usuário responsável pela atualização</param>
        /// <returns>Código de erro retornado da procedure spge0362</returns>
        public Int32 AtualizarFranquia(Int32 codigoEntidade, Int32 codigoRegime, Int32 codigoCelula, Int32 codigoCanal,
            String codigoUsuario)
        {

            using (Logger Log = Logger.IniciarLog("Atualiza o Regime de franquia da Entidade"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    GenericDatabase db = base.SybaseGE();
                    Int32 codigoRetorno = 0;

                    using (DbCommand cmd = db.GetStoredProcCommand("spge0362"))
                    {
                        db.AddInParameter(cmd, "@NUM_PDV", DbType.Int32, codigoEntidade);
                        //Código Serviço fixo em 1: Serasa
                        db.AddInParameter(cmd, "@COD_SERV", DbType.Int16, 1);
                        db.AddInParameter(cmd, "@COD_REGIME", DbType.Int16, codigoRegime);
                        db.AddInParameter(cmd, "@COD_CEL", DbType.Int32, codigoCelula);
                        db.AddInParameter(cmd, "@COD_CNL", DbType.Int32, codigoCanal);
                        db.AddInParameter(cmd, "@COD_AGE_FLCO", DbType.Int32, 0);

                        db.AddInParameter(cmd, "@NUM_PDV_BCAR", DbType.Int32, null);
                        db.AddInParameter(cmd, "@COD_MDLD_PGMN", DbType.AnsiString, null);

                        db.AddInParameter(cmd, "@USUARIO", DbType.AnsiString, codigoUsuario.Trim());
                        db.AddInParameter(cmd, "@TIPO_RETORNO", DbType.AnsiString, "R");
                        db.AddInParameter(cmd, "@PERMITIR_CANCELAR", DbType.AnsiString, "N");
                        db.AddInParameter(cmd, "@VERIFICAR_TECNOLOGIA", DbType.AnsiString, "S");

                        Log.GravarLog(EventoLog.ChamadaDados, new { cmd.Parameters });

                        codigoRetorno = db.ExecuteScalar(cmd).ToString().ToInt32();

                        Log.GravarLog(EventoLog.RetornoDados, new { codigoRetorno });
                        Log.GravarLog(EventoLog.FimDados, new { codigoRetorno });

                        return codigoRetorno;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(501, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Inclui/Atualiza a confirmação do usuário
        /// </summary>
        /// <param name="codigoRestricao">Código de restrição</param>
        /// <param name="codigoVersao">Código de versão</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoUsuario">Código do Usuário responsável pela inclusão/atualização</param>
        /// <returns>0</returns>
        public Int16 IncluirAceite(Int32 codigoRestricao, Int32 codigoVersao, Int32 codigoGrupoEntidade, Int32 codigoEntidade, String codigoUsuario)
        {

            using (Logger Log = Logger.IniciarLog("Inclui/Atualiza a confirmação do usuário"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    SqlDatabase db = base.SQLServerIS();
                    using (DbCommand cmd = db.GetStoredProcCommand("SPIS_IncluirAceite_INS"))
                    {
                        db.AddInParameter(cmd, "cod_restricao", DbType.Int32, codigoRestricao);
                        db.AddInParameter(cmd, "cod_versao", DbType.Int32, codigoVersao);
                        db.AddInParameter(cmd, "cod_gru_etd", DbType.Int32, codigoGrupoEntidade);
                        db.AddInParameter(cmd, "cod_etd", DbType.Int32, codigoEntidade);
                        db.AddInParameter(cmd, "cod_usr", DbType.AnsiString, codigoUsuario);
                        Log.GravarLog(EventoLog.ChamadaDados, new { cmd.Parameters });

                        db.ExecuteNonQuery(cmd);

                        Log.GravarLog(EventoLog.RetornoDados);
                        Log.GravarLog(EventoLog.FimDados);
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(503, FONTE, ex);
                }
            }
        }
        /// <summary>
        /// Busca o conteúdo do contrato de acordo com o código da Versão de Regime atual
        /// </summary>
        /// <param name="codigoVersao"></param>
        /// <returns></returns>
        public RestricaoRegime ConsultarContratoRestricao(Int32 codigoVersao)
        {
            RestricaoRegime restricao = null;


            using (Logger Log = Logger.IniciarLog("Busca o conteúdo do contrato de acordo com o código da Versão de Regime atual"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    SqlDatabase db = base.SQLServerIS();

                    using (DbCommand cmd = db.GetStoredProcCommand("SPIS_ListarRestricao_SEL"))
                    {
                        db.AddInParameter(cmd, "cod_restricao", DbType.Int32, 2);
                        db.AddInParameter(cmd, "cod_versao", DbType.Int32, codigoVersao);

                        Log.GravarLog(EventoLog.ChamadaDados, new { cmd.Parameters });
                        using (IDataReader dr = db.ExecuteReader(cmd))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            if (dr.Read())
                            {
                                restricao = new RestricaoRegime();
                                restricao.CodigoRestricao = (Int32)dr["cod_restricao"].ToString().ToInt32Null(0);
                                restricao.DescricaoRestricao = dr["des_restricao"].ToString();
                                restricao.CodigoVersao = dr["cod_versao"].ToString().ToInt32();
                                restricao.TipoRestricao = dr["tip_restricao"].ToString();
                                restricao.CorpoContrato = dr["des_body"].ToString();
                            }
                        }
                    }

                    Log.GravarLog(EventoLog.FimDados, new { restricao });
                    return restricao;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(503, FONTE, ex);
                }
            }
        }
    }
}
