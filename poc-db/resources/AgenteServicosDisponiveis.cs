
/*
 © Copyright 2017 Rede S.A.
   Autor : Rodrigo Coelho - rodrigo.oliveira@iteris.com.br
   Empresa : Iteris
 */

using Rede.PN.ZeroDolar.Agentes.GEServicos;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.ZeroDolar.Agentes {

    /// <summary>
    /// Classe agente que conecta ao serviço do GE para listagem, contratação e cancelamento de serviços
    /// </summary>
    public class AgenteServicosDisponiveis {

        /// <summary>
        /// Lista os serviços disponíveis para o pv informado
        /// </summary>
        /// <param name="numeroPV">Número do PV</param>
        /// <returns></returns>
        public ListaServicosPV[] ListarServicosDisponiveis(int numeroPV) {

            using (var log = Logger.IniciarLog("ListaServicosDisponiveis - Agente")) {
                log.GravarLog(EventoLog.InicioAgente, new { numeroPV });

                try {
                    using (ServicoPortalGEServicosClient client = new ServicoPortalGEServicosClient()) {
                        return client.ListaServicosPV(numeroPV);
                    }
                } catch (TimeoutException ex) {
                    log.GravarErro(ex);
                    throw ex;
                } catch (Exception ex) {
                    log.GravarErro(ex);
                    throw ex;
                } finally {
                    log.GravarLog(EventoLog.FimAgente);
                }
            }

        }

        /// <summary>
        /// Realiza a contratação de um serviço disponível
        /// </summary>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="codigoServico">Código do Serviço</param>
        /// <param name="codigoCanal">Código do Canal</param>
        /// <param name="codigoCelula">Código da Célula</param>
        /// <param name="usuario">Nome do usuário</param>
        /// <returns></returns>
        public ResponseBase ContratarServico(int numeroPV, int codigoServico, int codigoCanal, int codigoCelula, string usuario) {

            using (var log = Logger.IniciarLog("ContratarServico - Agente")) {
                log.GravarLog(EventoLog.InicioAgente, new { numeroPV, codigoServico, codigoCanal, codigoCelula, usuario });

                try {
                    using (ServicoPortalGEServicosClient client = new ServicoPortalGEServicosClient()) {
                        return client.ContrataServico(new ContrataServicoRequest() {
                            CodigoServico = codigoServico,
                            CodigoRegime = 0,
                            Canal = codigoCanal,
                            Celula = codigoCelula,
                            NumeroPV = numeroPV,
                            Usuario = usuario
                        });                        
                    }
                } catch (TimeoutException ex) {
                    log.GravarErro(ex);
                    throw ex;
                } catch (Exception ex) {
                    log.GravarErro(ex);
                    throw ex;
                } finally {
                    log.GravarLog(EventoLog.FimAgente);
                }
            }
        }


     
        /// <summary>
        /// Cancela um serviço contratado
        /// </summary>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="codigoServico">Código do Serviço</param>
        /// <param name="usuario">Nome do Usuário</param>
        /// <returns></returns>
        public CodErroDescricaoErro CancelarServico(int numeroPV, int codigoServico, string usuario) {

            using (var log = Logger.IniciarLog("CancelarServico - Agente")) {
                log.GravarLog(EventoLog.InicioAgente, new { numeroPV, codigoServico, usuario });

                try {
                    using (ServicoPortalGEServicosClient client = new ServicoPortalGEServicosClient()) {
                        return client.Cancelar(numeroPV, codigoServico, usuario);
                    }
                } catch (TimeoutException ex) {
                    log.GravarErro(ex);
                    throw ex;
                } catch (Exception ex) {
                    log.GravarErro(ex);
                    throw ex;
                } finally {
                    log.GravarLog(EventoLog.FimAgente);
                }
            }
        }

        /// <summary>
        /// Reativar um serviço cancelado
        /// </summary>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="codigoServico">Código do Serviço</param>
        /// <param name="usuario">Nome do usuário</param>
        /// <returns></returns>
        public CodErroDescricaoErro ReativarServico(int numeroPV, int codigoServico, string usuario) {

            using (var log = Logger.IniciarLog("ReativarServico - Agente")) {
                log.GravarLog(EventoLog.InicioAgente, new { numeroPV, codigoServico, usuario });

                try {
                    using (ServicoPortalGEServicosClient client = new ServicoPortalGEServicosClient()) {
                        return client.Reativar(numeroPV, codigoServico, usuario);
                    }
                } catch (TimeoutException ex) {
                    log.GravarErro(ex);
                    throw ex;
                } catch (Exception ex) {
                    log.GravarErro(ex);
                    throw ex;
                } finally {
                    log.GravarLog(EventoLog.FimAgente);
                }
            }
        }

    }
}
