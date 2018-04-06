using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data;
using Rede.PN.Cancelamento.Modelo;


namespace Rede.PN.Cancelamento.Dados
{
    public class CancelamentoDados : BancoDeDadosBase
    {
        #region [ Constantes ]

        /// <summary>
        /// Código de erro genérico de banco de dados
        /// </summary>
        public const Int32 CODIGO_ERRO = 500;

        /// <summary>
        /// Fonte
        /// </summary>
        public new const String FONTE = "Rede.PN.Cancelamento.Dados";

        #endregion

        /// <summary>
        /// Incluir Lista de Solicitação de Cancelamento no Banco de dados PN
        /// </summary>
        /// <param name="lstSolicitacaoCancelamento">Lista de Solicitação de Cancelamento</param>
        /// <param name="ip">valor Ip da máquina que solicitou o cancelamento</param>
        /// <param name="usuario">usuário que solicitou o cancelamento</param>
        public void IncluirCancelamentosPn(List<Modelo.SolicitacaoCancelamento> lstSolicitacaoCancelamento, string ip, string usuario)
        {
            char tipoTransacao = ' ';
            char tipoCancelamento = ' ';
            
            //Inserir no BD as informações da lista de Cancelamento
            SqlDatabase db = base.SQLServerPN();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                try
                {
                    foreach (var objCancelamento in lstSolicitacaoCancelamento)
                    {
                        //tipoTransacao = (objCancelamento.TipoTransacao == TipoTransacao.Rot || objCancelamento.TipoTransacao == TipoTransacao.Par) ? 'C' : 'D';
                        tipoTransacao = (objCancelamento.TipoVenda == TipoVenda.Credito) ? 'C' : 'D';
                        tipoCancelamento = (objCancelamento.TipoCancelamento == TipoCancelamento.Total) ? 'T' : 'P';

                        try
                        {
                            DbCommand cmd = db.GetStoredProcCommand("sp_ins_cancelamento_pn");

                            db.AddInParameter(cmd, "dat_canc", DbType.DateTime, DateTime.Now);
                            db.AddInParameter(cmd, "nro_aviso", DbType.Int64, objCancelamento.NumeroAvisoCancelamento);
                            db.AddInParameter(cmd, "nsu", DbType.String, objCancelamento.NSU);
                            db.AddInParameter(cmd, "val_venda", DbType.Decimal, objCancelamento.ValorBruto);
                            db.AddInParameter(cmd, "dat_venda", DbType.DateTime, objCancelamento.DataVenda);
                            db.AddInParameter(cmd, "tipo_tran", DbType.String, tipoTransacao);
                            db.AddInParameter(cmd, "tipo_canc", DbType.String, tipoCancelamento);
                            db.AddInParameter(cmd, "status", DbType.String, "Cancelado");
                            db.AddInParameter(cmd, "val_canc", DbType.Decimal, objCancelamento.ValorCancelamento);
                            db.AddInParameter(cmd, "nro_aut", DbType.String, objCancelamento.NumeroAutorizacao);
                            db.AddInParameter(cmd, "cod_etd_canc", DbType.Int32, objCancelamento.NumeroEstabelecimentoCancelamento);
                            db.AddInParameter(cmd, "cod_etd_vend", DbType.Int32, objCancelamento.NumeroEstabelecimentoVenda);
                            db.AddInParameter(cmd, "usr_canc", DbType.String, usuario);
                            db.AddInParameter(cmd, "ip", DbType.String, ip);
                            db.AddInParameter(cmd, "meio_comun", DbType.Int32, 3);
                            //db.AddInParameter(cmd, "dat_anul", DbType.DateTime, DateTime.Now);
                            //db.AddInParameter(cmd, "cod_etd_anul", DbType.Int32, numeroPontoVenda);
                            //db.AddInParameter(cmd, "usr_anul", DbType.String, usuario);
                            //db.AddInParameter(cmd, "ip_anul", DbType.String, ip);


                            //Adicionar valores "DBNull.Value" nos parametros nulos
                            foreach (DbParameter parameter in cmd.Parameters)
                                if (parameter.Value == null)
                                    parameter.Value = DBNull.Value;

                            //Executa Query
                            db.ExecuteNonQuery(cmd);

                        }
                        catch (Exception exc)
                        {

                            throw;
                        }
                    }

                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Consulta Cancelamentos PN
        /// </summary>
        /// <param name="tipoTransacao">Tipo de transação</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data Final</param>
        /// <param name="ip">ip</param>
        /// <returns>Retorna lista de cancelamentos PN</returns>
        public List<Modelo.CancelamentoPn> ConsultarCancelamentosPn(String tipoTransacao, Int32 numeroPontoVenda, DateTime dataInicial, DateTime dataFinal, String ip)
        {
            List<Modelo.CancelamentoPn> cancelamentos = new List<Modelo.CancelamentoPn>();

            using (var log = Logger.IniciarLog("Consulta Cancelamentos PN - Dados"))
            {
                log.GravarLog(EventoLog.InicioDados, new
                {
                    tipoTransacao,
                    numeroPontoVenda,
                    dataInicial,
                    dataFinal,
                    ip
                });

                try
                {
                    SqlDatabase db = base.SQLServerPN();
                    using (DbCommand cmd = db.GetStoredProcCommand("sp_cons_cancelamentos_pn"))
                    {
                        if (!String.IsNullOrEmpty(tipoTransacao))
                            db.AddInParameter(cmd, "tipo_tran", DbType.String, tipoTransacao);
                        if (!String.IsNullOrEmpty(ip))
                            db.AddInParameter(cmd, "ip", DbType.String, ip);
                        if (numeroPontoVenda != 0)
                            db.AddInParameter(cmd, "cod_etd_vend", DbType.Int32, numeroPontoVenda);

                        db.AddInParameter(cmd, "dat_inicio", DbType.Date, dataInicial.Date);
                        db.AddInParameter(cmd, "dat_final", DbType.Date, dataFinal.Date);

                        using (DataSet ds = db.ExecuteDataSet(cmd))
                        {
                            if (ds.Tables.Count > 0)
                            {
                                IDataReader leitor = ds.CreateDataReader(ds.Tables[0]);

                                while (leitor.Read())
                                {
                                    cancelamentos.Add(new Modelo.CancelamentoPn
                                    {
                                        DataCancelamento = (DateTime)leitor["dat_canc"],
                                        NumeroAviso = (Int64)leitor["nro_aviso"],
                                        Nsu = leitor["nsu"].ToString(),
                                        ValorVenda = (Decimal)leitor["val_venda"],
                                        DataVenda = (DateTime)leitor["dat_venda"],
                                        TipoTransacao = leitor["tipo_tran"].ToString().First(),
                                        TipoCancelamento = leitor["tipo_canc"].ToString().First(),
                                        Status = leitor["status"].ToString(),
                                        ValorCancelamento = (Decimal)leitor["val_canc"],
                                        NumeroAutorizacao = leitor["nro_aut"] != null ? leitor["nro_aut"].ToString() : String.Empty,
                                        NumeroEstabelecimentoCancelamento = (Int32)leitor["cod_etd_canc"],
                                        NumeroEstabelecimentoVenda = (Int32)leitor["cod_etd_vend"],
                                        UsuarioCancelamento = leitor["usr_canc"].ToString(),
                                        Ip = leitor["ip"].ToString(),
                                        MeioComunicacao = (Int32)leitor["meio_comun"]
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                log.GravarLog(EventoLog.FimDados, new
                {
                    cancelamentos
                });
            }

            return cancelamentos;
        }

        /// <summary>
        /// Anula cancelamentos PN
        /// </summary>
        /// <param name="listaNumerosAvisos">Lista de números de avisos</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <param name="usuario">Usuário</param>
        /// <param name="ip">Ip</param>
        public void AnularCancelamentosPn(List<Int64> listaNumerosAvisos, Int32 numeroPontoVenda, String usuario, String ip)
        {
            using (var log = Logger.IniciarLog("Anular Cancelamentos PN - Dados"))
            {
                log.GravarLog(EventoLog.InicioDados, new
                {
                    listaNumerosAvisos, numeroPontoVenda, usuario, ip
                });

                try
                {
                    SqlDatabase db = base.SQLServerPN();
                    using (DbCommand cmd = db.GetStoredProcCommand("sp_alt_cancelamento_pn"))
                    {
                        foreach (var numeroAviso in listaNumerosAvisos)
                        {
                            cmd.Parameters.Clear();
                            db.AddInParameter(cmd, "nro_aviso", DbType.Int64, numeroAviso);
                            db.AddInParameter(cmd, "dat_anul", DbType.DateTime, DateTime.Now);
                            db.AddInParameter(cmd, "cod_etd_anul", DbType.Int32, numeroPontoVenda);
                            db.AddInParameter(cmd, "usr_anul", DbType.String, usuario);
                            db.AddInParameter(cmd, "ip_anul", DbType.String, ip);

                            Int32 codigoErro = db.ExecuteNonQuery(cmd);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                log.GravarLog(EventoLog.FimDados, new
                {
                    
                });
            }
        }
    }
}
