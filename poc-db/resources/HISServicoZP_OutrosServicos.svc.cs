using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.OutrosServicos.Servicos.GEServicos;
using AutoMapper;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrosServicos.Servicos
{
    public class HISServicoZP_OutrosServicos : ServicoBase, IHISServicoZP_OutrosServicos
    {


        /// <summary>
        /// Recupera o código de Regime de pagamento do estabelecimento
        /// </summary>
        /// <returns></returns>
        private short RecuperarCodigoRegime(int numeroPV)
        {
            //Recupera o número do PV da sessão atual do usuário
            short result = 1;
            using (GEServicos.ServicoServicosClient svcGE = new GEServicos.ServicoServicosClient())
            {
                result = svcGE.RecuperarCodigoRegime(numeroPV, 1);
            }

            return result;
        }	



        /// <summary>
        /// Lista os regimes no MainFrame acordo com o código de Regime
        /// </summary>
        /// <remarks>        
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZP128CB / Programa ZP128 / TranID IS59         
        /// </remarks>
        /// <param name="regimes">Lista de regimes disponíveis</param>
        /// <param name="codigoServico">Código de Serviço</param>
        /// <returns>Código de retorno
        /// Erros codigoRetorno: 0 - RETORNO OK. 1 - ERRO DE PARAMETROS. 2 - ERRO DE ARQUIVOS
        /// </returns>
        public Int16 ListarRegime(out RegimeFranquia[] regimesFranquia, Int16 codigoServico)
        {
            using (Logger Log = Logger.IniciarLog("Lista os regimes no MainFrame acordo com o código de Regime [ZP128CB]"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Mapper.CreateMap<Modelo.RegimeFranquia, Servicos.RegimeFranquia>();
                    Mapper.CreateMap<Modelo.FaixaConsultaFranquia, Servicos.FaixaConsultaFranquia>();
                    Modelo.RegimeFranquia[] regimesModelo;
                    var regimeNegocio = new Negocio.Regime();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoServico });

                    Int16 codigoRetorno = 0;
                    codigoRetorno = regimeNegocio.ListarRegime(out regimesModelo, codigoServico);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, regimesModelo });


                    List<Servicos.RegimeFranquia> listaRegimes = new List<RegimeFranquia>();
                    foreach (Modelo.RegimeFranquia regime in regimesModelo)
                    {
                        listaRegimes.Add(Mapper.Map<Modelo.RegimeFranquia, Servicos.RegimeFranquia>(regime));
                    }

                    regimesFranquia = listaRegimes.ToArray();

                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, regimesFranquia });

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
        /// <summary>
        /// Consulta informações do Regime
        /// </summary>
        /// <remarks>        
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZP034CB / Programa ZP034 / TranID IS58         
        /// </remarks>
        /// <param name="codigoRetorno">Código de mensagem de erro</param>
        /// <param name="numeroPV">Número do PV</param>
        /// <returns>Regime de franquia com:
        /// Código de Serviço = 1</returns>
        public RegimeFranquia ConsultarRegime(out Int16 codigoRetorno, int numeroPV)
        {
            using (Logger Log = Logger.IniciarLog("Consulta informações do Regime [ZP034CB]"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    short codigoRegime = RecuperarCodigoRegime(numeroPV);

                    Mapper.CreateMap<Modelo.RegimeFranquia, Servicos.RegimeFranquia>();
                    Mapper.CreateMap<Modelo.FaixaConsultaFranquia, Servicos.FaixaConsultaFranquia>();
                    Modelo.RegimeFranquia regimesModelo = new Modelo.RegimeFranquia();
                    Servicos.RegimeFranquia regimeServico = null;
                    var regimeNegocio = new Negocio.Regime();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoRegime });

                    regimesModelo = regimeNegocio.ConsultarRegime(out codigoRetorno, codigoRegime);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, regimesModelo });

                    regimeServico = Mapper.Map<Servicos.RegimeFranquia>(regimesModelo);

                    Log.GravarLog(EventoLog.FimServico, new { regimeServico });


                    return (regimeServico);
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
