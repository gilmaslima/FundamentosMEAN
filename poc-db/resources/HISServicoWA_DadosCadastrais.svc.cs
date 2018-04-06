using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using AutoMapper;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componentes WA de dados cadastrais
    /// </summary>
    public class HISServicoWA_DadosCadastrais : ServicoBase, IHISServicoWA_DadosCadastrais
    {
        /// <summary>
        /// Gera carta para envio de uma nova senha para um usuário de um PV
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA085 / Programa WA085 / TranID IS61
        /// </remarks>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="codigoUsuario">Código do Usuário</param>
        /// <param name="senha">Senha</param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="codigoCarta">Código da Carta</param>
        /// <param name="tipoEnvio">Tipo de Envio</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        public void GerarCarta(Int32 numeroPV,
            String codigoUsuario,
            String senha,
            String nomeUsuario,
            Int16 codigoCarta,
            String tipoEnvio,
            String endereco,
            out Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Gera carta para envio de uma nova senha para um usuário de um PV [WACA085]"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var cartaPVUsuario = new Negocio.Entidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        numeroPV,
                        codigoUsuario,
                        senha,
                        nomeUsuario,
                        codigoCarta,
                        tipoEnvio,
                        endereco
                    });

                    cartaPVUsuario.GerarCarta(
                        numeroPV,
                        codigoUsuario,
                        senha,
                        nomeUsuario,
                        codigoCarta,
                        tipoEnvio,
                        endereco,
                        out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
    }
}
