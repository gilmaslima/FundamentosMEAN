using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using AutoMapper;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrosServicos.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "HISServicoWM_OutrosServicos" in code, svc and config file together.
    public class HISServicoWM_OutrosServicos : ServicoBase, IHISServicoWM_OutrosServicos
    {
        /// <summary>
        /// Retorna a carta gerada no mainframe para a solicitação cadastrada
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BWM58CO / Programa WMD58 / TranID WM58 
        /// </remarks>
        /// <param name="numeroSolicitacao">Número de solicitação</param>
        /// <param name="codigoTipoServico">Código do tipo de Serviço</param>
        /// <param name="linhasCarta">Objeto com o conteúdo da carta em linhas</param>
        /// <param name="quantidadeLinhasCarta">Quantidade de linhas na carta</param>
        /// <returns>Código de Retorno do mainframe</returns>
        public Int16 Manutencao(Decimal numeroSolicitacao, String codigoTipoServico, out CartaSolicitacao linhasCarta, out Int16 quantidadeLinhasCarta)
        {
            using (Logger Log = Logger.IniciarLog("Retorna a carta gerada no mainframe para a solicitação cadastrada [BWM58CO]"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Int16 codigoRetorno = 0;

                    Mapper.CreateMap<Modelo.CartaSolicitacao, Servicos.CartaSolicitacao>();
                    Modelo.CartaSolicitacao cartaModelo = new Modelo.CartaSolicitacao();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroSolicitacao, codigoTipoServico });
                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    solicitacaoNegocio.Manutencao(numeroSolicitacao, codigoTipoServico, out cartaModelo, out quantidadeLinhasCarta, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { cartaModelo, quantidadeLinhasCarta, codigoRetorno });
                    linhasCarta = Mapper.Map<Modelo.CartaSolicitacao, Servicos.CartaSolicitacao>(cartaModelo);

                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno });

                    return codigoRetorno;
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
