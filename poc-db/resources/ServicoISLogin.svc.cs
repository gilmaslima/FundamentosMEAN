using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.OutrasEntidades.Modelo;
using Redecard.PN.OutrasEntidades.Negocio;

namespace Redecard.PN.OutrasEntidades.Servicos
{
    /// <summary>
    /// Serviço para acesso ao serviço PB do módulo Outras Entidades
    /// </summary>
    public class ServicoISLogin : ServicoBase, IServicoISLogin
    {


        /// <summary>
        /// Consulta login KO usuários do sistema Komerci - Migração do wsLogin da sigla IS
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="senhaUsuario">Senha do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Código de retorno</returns>
        public void AutenticarUsuarioKO(Int32 codigoEntidade, String codigoUsuario, String senhaUsuario, out Int32 codigoRetorno, out String mensagemRetorno, bool senhaCriptografada)
        {
            using (Logger log = Logger.IniciarLog("Consulta Login KO usuários do sistema Komerci"))
            {
                log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioUsuario = new Negocio.NegocioISLogin();

                    log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoUsuario });

                    negocioUsuario.AutenticarUsuarioKO(codigoEntidade, codigoUsuario, senhaUsuario, out codigoRetorno, out mensagemRetorno, senhaCriptografada);

                    log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, mensagemRetorno });
                    log.GravarLog(EventoLog.FimServico, new { codigoRetorno, mensagemRetorno });

                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }



    }
}
