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
    /// Serviço para acesso ao componentes ZP de dados cadastrais
    /// </summary>
    public class HISServicoZP_DadosCadastraisRest : ServicoBase, IHISServicoZP_DadosCadastraisRest
    {
        /// <summary>
        /// Obtém Teconologia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZP458CO / Programa ZP458 / TranID IS95
        /// </remarks>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="dataPesquisa">Data da pesquisa no formato AAAA/MM</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de terminais </returns>
        public TerminalBancarioListRest ObterTecnologia(ObterTecnologiaRequest obtertecnologia)
        {
            TerminalBancarioListRest retorno;
            using (Logger Log = Logger.IniciarLog("Obtém Teconologia [ZP458CO]"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Int16 codigoRetorno;
                    var terminais = new List<Servicos.TerminalBancario>();

                    var negocioTerminal = new Negocio.TerminalBancario();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { obtertecnologia.CodigoEntidade, obtertecnologia.DataPesquisa });
                    var modeloTerminais = negocioTerminal.ObterTecnologia(obtertecnologia.CodigoEntidade, obtertecnologia.DataPesquisa, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloTerminais });


                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.TerminalBancario, Servicos.TerminalBancario>();

                    foreach (var modeloTerminal in modeloTerminais)
                    {
                        // Converte Business Entity para Data Contract Entity
                        terminais.Add(Mapper.Map<Modelo.TerminalBancario, Servicos.TerminalBancario>(modeloTerminal));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { terminais });
                    retorno = new TerminalBancarioListRest
                    {
                        Itens = terminais.ToArray(),
                        StatusRetorno = new StatusRetorno
                        {
                            CodigoRetorno = codigoRetorno
                        }
                    };
                    return retorno;
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
