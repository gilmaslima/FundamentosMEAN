/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using AutoMapper;
using Redecard.PN.Comum;
using Redecard.PN.OutrosServicos.Servicos.PlanoContas;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Serviço para expor os métodos de consulta mainframe do módulo Plano de Contas / Japão / Turquia
    /// </summary>
    public class HisServicoNkPlanoContas : ServicoBase, IHisServicoNkPlanoContas
    {
        /// <summary>
        /// Retorna as compensações de débitos de aluguel do PV incluídas no Mês/Ano informado.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKNK0070 / Programa NKC007 / TranID NK07 / Método ConsultarCompensacoesDebitos
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKNK0070 / Programa NKC007 / TranID NK07 / Método ConsultarCompensacoesDebitos
        /// </remarks>  
        /// <returns>Compensações Débitos</returns>
        public List<CompensacaoDebitoAluguel> ConsultarCompensacoesDebitos(
            Int32 numeroPv,
            DateTime mesAnoDebito,
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Compensações de débitos - BKNK0070 / NKC007 / NK07"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv, mesAnoDebito });

                //Declaração de variável de retorno
                var retorno = default(List<CompensacaoDebitoAluguel>);

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.PlanoContas();

                    List<Modelo.PlanoContas.CompensacaoDebitoAluguel> retornoModelo =
                        negocio.ConsultarCompensacoesDebitos(numeroPv, mesAnoDebito, out codigoRetorno);

                    //Mapeamento entre classes Modelo de serviço e negócio
                    Mapper.CreateMap<Modelo.PlanoContas.CompensacaoDebitoAluguel, CompensacaoDebitoAluguel>();

                    retorno = Mapper.Map<List<CompensacaoDebitoAluguel>>(retornoModelo);
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

                log.GravarLog(EventoLog.RetornoServico, new { retorno, codigoRetorno });

                return retorno;
            }
        }

        /// <summary>
        /// Retorna a apuração de preço único dos 12 ultimos meses para o PV(Ponto de Venda) informado.<br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKNK0080 / Programa NKC008 / TranID NK08 / Método ConsultarPlanosPrecoUnicoContratados
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKNK0080 / Programa NKC008 / TranID NK08 / Método ConsultarPlanosPrecoUnicoContratados
        /// </remarks>  
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar os planos.</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        /// <returns>Planos Preço Único Contratados</returns>
        public List<PlanoPrecoUnico> ConsultarPlanosPrecoUnicoContratados(
            Int32 numeroPv, 
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Planos Preço Único Contratados - BKNK0080 / NKC008 / NK08"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv });

                try
                {
                    //Declaração de variável de retorno
                    var retorno = default(List<PlanoPrecoUnico>);

                    //Instanciação da classe de negócio
                    var negocio = new Negocio.PlanoContas();

                    List<Modelo.PlanoContas.PlanoPrecoUnico> planosPrecoUnico =
                        negocio.ConsultarPlanosPrecoUnicoContratados(numeroPv, out codigoRetorno);

                    //Mapeamento entre classes Modelo de serviço e negócio
                    Mapper.CreateMap<Modelo.PlanoContas.PlanoPrecoUnico, PlanoPrecoUnico>();
                    Mapper.CreateMap<Modelo.PlanoContas.Equipamento, Equipamento>();

                    retorno = Mapper.Map<List<PlanoPrecoUnico>>(planosPrecoUnico);

                    log.GravarLog(EventoLog.RetornoServico, new { retorno, codigoRetorno });

                    return retorno;
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