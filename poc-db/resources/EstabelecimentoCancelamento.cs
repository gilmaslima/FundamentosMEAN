using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace Redecard.PN.Cancelamento.Dados
{
    public class EstabelecimentoCancelamento : BancoDeDadosBase
    {
        /// <summary>
        /// Consulta todas as propriedades de um estabelecimento no banco do GE (spge6002) e
        /// </summary>
        /// <returns>Modelo.Entidade</returns>
        public Modelo.EstabelecimentoCancelamento Consultar(Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar todas as propriedades de um estabelecimento no banco do GE"))
            {
                Log.GravarLog(EventoLog.InicioDados, new { codigoEntidade });

                try
                {
                    codigoRetorno = 0;
                    Modelo.EstabelecimentoCancelamento entidade = new Modelo.EstabelecimentoCancelamento();
                    GenericDatabase db = this.SybaseGE();
                    DbCommand command = db.GetStoredProcCommand("spge6002");
                    // adicionar parametros
                    db.AddInParameter(command, "@NUM_PDV", DbType.Int32, codigoEntidade);
                    db.AddInParameter(command, "@COD_ACES_INTN", DbType.AnsiString, null);
                    db.AddInParameter(command, "@PRIM_VEZ", DbType.AnsiString, null);
                    db.AddInParameter(command, "@CLIENTE", DbType.AnsiString, null);
                    db.AddInParameter(command, "@VALIDA_CITE", DbType.AnsiString, "N");
                    db.AddInParameter(command, "@SGL_TIP_PROD", DbType.AnsiString, null);

                    IDataReader leitor = db.ExecuteReader(command);

                    if (leitor.Read())
                    {
                        entidade.RazaoSocial = leitor["RAZAOSOC"].ToString().Trim();
                        entidade.NomeEntidade = leitor["NOMFAT"].ToString().Trim();
                        entidade.Endereco = String.Format("{0} {1} {2}", leitor["ENDLOJA"].ToString().Trim(), leitor["NUMLOJA"].ToString().Trim(), leitor["CPMLOJA"].ToString().Trim());
                        entidade.CEP = String.Format("{0}-{1}", leitor["CEPLOJA"].ToString(), leitor["CCEPLOJA"].ToString());
                        entidade.Bairro = leitor["BRRLOJA"].ToString().Trim();
                        entidade.Cidade = leitor["CIDLOJA"].ToString().Trim();
                        entidade.Estado = leitor["UFLOJA"].ToString().Trim();
                        entidade.Contato = leitor["PESCTO"].ToString().Trim();
                        entidade.Telefone = String.Format("({0}) {1}", leitor["DDD"].ToString().Trim(), leitor["FONE"].ToString().Trim());
                        entidade.Telefone2 = String.Format("({0})", leitor["DDD2"].ToString().Trim(), leitor["FONE2"].ToString().Trim());
                        entidade.FAX = String.Format("{0}", leitor["DDDFAX"].ToString().Trim(), leitor["FAX"].ToString().Trim());
                        entidade.NumeroFax = leitor["FAX"].ToString().Trim();
                        entidade.RamoAtividade = String.Format("{0} - {1}", leitor["RAMO"].ToString().Trim(), leitor["DESRAMO"].ToString().Trim());
                        entidade.Email = leitor["EMAIL"].ToString().Trim();
                        entidade.CNPJEntidade = leitor["CNPJ"].ToString().Trim();
                        entidade.Centralizadora = leitor["CENTRAL"].ToString().Trim().ToUpper().CompareTo("S") == 0 ? true : false;
                    }

                    try
                    {
                        if (!leitor.IsClosed) leitor.Close();
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                    }

                    try
                    {
                        command.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                    }

                    Log.GravarLog(EventoLog.FimDados, new { entidade });

                    return entidade;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(501, FONTE, ex);
                }
            }
        }

    }
}
