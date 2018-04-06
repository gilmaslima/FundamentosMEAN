using AutoMapper;
using Redecard.PN.Comum;
using Redecard.PN.OutrosServicos.Servicos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Serviço para expor os métodos de consulta mainframe do módulo Conta Certa
    /// </summary>
    public class HISServicoZPContaCerta : ServicoBase, IHISServicoZPContaCerta
    {
        /// <summary>
        /// Consulta detalhe de ofertas.<br/>        
        /// - Book ZPL05000 / Programa ZPC050 / TranID ZPC0 / Método: ConsultarContratoVigencia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPL05000 / Programa ZPC050 / TranID ZPC0 / Método: ConsultarContratoVigencia
        /// </remarks>
        /// <param name="numeroPV"></param>
        /// <param name="codSitContrato"></param>
        /// <param name="dataFimVigencia"></param>
        /// <param name="codRetorno"></param>
        /// <returns>Detalhe da oferta</returns>
        public Modelos.DetalheOferta ConsultarContratoVigencia(Int32 numeroPV, Int16 codSitContrato, DateTime dataFimVigencia, out Int16 codRetorno)
        {
            using (Logger log = Logger.IniciarLog("Conta Certa - ConsultarContratoVigencia"))
            {
                DetalheOferta retorno = default(DetalheOferta);

                log.GravarLog(EventoLog.InicioServico, new { numeroPV, codSitContrato, dataFimVigencia });

                try
                {
                    Negocio.ContaCerta negocio = new Negocio.ContaCerta();

                    Modelo.DetalheOferta detalhe = negocio.ConsultarContratoVigencia(numeroPV, codSitContrato, dataFimVigencia, out codRetorno);

                    //Mapeamento para classe modelo de serviço
                    Mapper.CreateMap<Modelo.DetalheOferta, DetalheOferta>();
                    Mapper.CreateMap<Modelo.PlanoContas.StatusOferta, PlanoContas.StatusOferta>();

                    retorno = Mapper.Map<Modelo.DetalheOferta, DetalheOferta>(detalhe);
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

                log.GravarLog(EventoLog.RetornoServico, new { retorno, codRetorno });

                return retorno;
            }
        }

        /// <summary>
        /// Consulta historico do detalhe de ofertas.<br/>        
        /// - Book ZPL05100 / Programa ZPC051 / TranID ZPC1 / Método: ConsultarContratoHistorico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPL05100 / Programa ZPC051 / TranID ZPC1 / Método: ConsultarContratoHistorico
        /// </remarks>
        /// <param name="numeroPV"></param>
        /// <param name="codSitContrato"></param>
        /// <param name="codRetorno"></param>
        /// <returns>Lista de histórico do detalhe de oferta</returns>
        public List<Modelos.DetalheOferta> ConsultarContratoHistorico(Int32 numeroPV, Int16 codSitContrato, out Int16 codRetorno)
        {
            using (Logger log = Logger.IniciarLog("Conta Certa - ConsultarContratoHistorico"))
            {
                List<DetalheOferta> retorno = default(List<DetalheOferta>);

                log.GravarLog(EventoLog.InicioServico, new { numeroPV, codSitContrato });

                try
                {
                    Negocio.ContaCerta negocio = new Negocio.ContaCerta();

                    List<Modelo.DetalheOferta> historico = negocio.ConsultarContratoHistorico(numeroPV, codSitContrato, out codRetorno);

                    //Mapeamento para classe modelo de serviço
                    Mapper.CreateMap<Modelo.DetalheOferta, DetalheOferta>();
                    Mapper.CreateMap<Modelo.PlanoContas.StatusOferta, PlanoContas.StatusOferta>();

                    retorno = Mapper.Map<List<Modelo.DetalheOferta>, List<DetalheOferta>>(historico);
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

                log.GravarLog(EventoLog.RetornoServico, new { retorno, codRetorno });

                return retorno;
            }
        }

        /// <summary>
        /// Verifica se o estabelecimento contém um histórico de ofertas
        /// </summary>
        /// <param name="numeroPV"></param>
        /// <param name="codSitContrato"></param>
        /// <param name="codRetorno"></param>
        /// <returns>Verdadeiro ou falso</returns>
        public Boolean ContemHistorico(Int32 numeroPV, Int16 codSitContrato, out Int16 codRetorno)
        {
            using (Logger log = Logger.IniciarLog("Conta Certa - ContemHistorico"))
            {
                Boolean retorno = default(Boolean);

                log.GravarLog(EventoLog.InicioServico, new { numeroPV, codSitContrato });

                try
                {
                    Negocio.ContaCerta negocio = new Negocio.ContaCerta();

                    retorno = negocio.ConsultarContratoHistorico(numeroPV, codSitContrato, out codRetorno).Count > 0;

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

                log.GravarLog(EventoLog.RetornoServico, new { retorno, codRetorno });

                return retorno;
            }
        }
    }
}
