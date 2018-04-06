using System;
using System.Collections.Generic;
using Redecard.PN.OutrasEntidades.Modelo;
using Redecard.PN.Comum;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using System.ServiceModel;

namespace Redecard.PN.OutrasEntidades.Dados
{
    public class DadosISLogin : BancoDeDadosBase
    {

        /// <summary>
        /// Consulta login KO usuários do sistema Komerci - SybaseIS
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="senhaUsuario">Senha do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Código de retorno</returns>
        public void AutenticarUsuarioKO(Int32 codigoEntidade, String codigoUsuario, String senhaUsuario, out Int32 codigoRetorno, out String mensagemRetorno)
        {
            using (Logger log = Logger.IniciarLog("Consulta Login KO usuários do sistema Komerci"))
            {
                log.GravarLog(EventoLog.InicioDados);

                try
                {
                    GenericDatabase db = base.SybaseIS();

                    codigoRetorno = 0;

                    using (DbCommand command = db.GetStoredProcCommand("sp_login_acesso_ko"))
                    {
                        // Cria parâmetros necessários
                        db.AddInParameter(command, "@cod_etd", DbType.Int32, codigoEntidade);
                        db.AddInParameter(command, "@cod_usr", DbType.AnsiString, codigoUsuario);
                        db.AddInParameter(command, "@nom_snha_usr", DbType.AnsiString, senhaUsuario);

                        log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });

                        codigoRetorno = db.ExecuteScalar(command).ToString().ToInt32();
                        mensagemRetorno = "";

                        log.GravarLog(EventoLog.RetornoDados);
                        log.GravarLog(EventoLog.FimDados, new { codigoRetorno, mensagemRetorno });
                    }

                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(500, String.Concat(FONTE, ".OutrasEntidades"), ex);
                }
            }
        }

    }
}
