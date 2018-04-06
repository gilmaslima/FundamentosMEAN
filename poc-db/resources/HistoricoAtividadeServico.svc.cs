/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using AutoMapper;
using Redecard.PN.Comum;
using Redecard.PN.Servicos.Modelos;
using System.ServiceModel.Web;

namespace Redecard.PN.Servicos
{
    /// <summary>
    /// Serviço para o Histórico/Log de Atividades
    /// </summary>
    public class HistoricoAtividadeServico : ServicoBase, IHistoricoAtividadeServico
    {
        /// <summary>
        /// Consulta os tipos de atividade existentes
        /// </summary>
        /// <param name="exibir">Opcional: se deve filtrar os tipos de atividade visíveis para consulta</param>
        /// <returns>Lista de tipos de atividade</returns>
        public List<TipoAtividade> ConsultarTiposAtividades(Boolean? exibir)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Tipos de Atividades existentes"))
            {
                try
                {
                    //Consulta os tipos de atividade
                    List<Modelo.TipoAtividade> modelo = new Negocio.HistoricoAtividade().Consultar(exibir);

                    //Prepara mapeamentos de retorno
                    Mapper.CreateMap<Modelo.TipoAtividade, TipoAtividade>();

                    return Mapper.Map<List<TipoAtividade>>(modelo);
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
        /// Consulta o Histórico de Atividades
        /// </summary>
        /// <param name="codigoHistorico">Opcional: código do histórico de atividade</param>
        /// <param name="codigoEntidade">Opcional: código da entidade</param>
        /// <param name="codigoTipoAtividade">Opcional: código do tipo de atividade</param>
        /// <param name="dataInicio">Opcional: data de início</param>
        /// <param name="dataFim">Opcional: data de término</param>
        /// <param name="nomeUsuario">Opcional: nome do usuário responsável</param>
        /// <param name="emailUsuario">Opcional: email do usuário responsável</param>
        /// <param name="funcionalOperador">Opcional: Funcional do operador responsável</param>
        /// <param name="tipoAtividadeVisible">Opcional: se deve ser do tipo de atividade visível</param>
        /// <returns>Histórico de Atividade encontrado, dado os filtros</returns>
        public List<HistoricoAtividade> ConsultarHistorico(
            Int64? codigoHistorico, 
            Int32? codigoEntidade, 
            Int64? codigoTipoAtividade, 
            DateTime? dataInicio, 
            DateTime? dataFim, 
            String nomeUsuario,
            String emailUsuario,
            String funcionalOperador,
            Boolean? tipoAtividadeVisible)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Histórico"))
            {
                try
                {
                    //Consulta histórico de atividades
                    List<Modelo.HistoricoAtividade> modelo = new Negocio.HistoricoAtividade()
                        .ConsultarHistorico(codigoHistorico, codigoEntidade, codigoTipoAtividade, dataInicio,
                        dataFim, nomeUsuario, emailUsuario, funcionalOperador, tipoAtividadeVisible);

                    //Prepara mapeamentos de retorno
                    Mapper.CreateMap<Modelo.HistoricoAtividade, HistoricoAtividade>();

                    return Mapper.Map<List<HistoricoAtividade>>(modelo);
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
        /// Grava um registro no histórico de atividades
        /// </summary>
        /// <param name="historico">Dados do registro do histórico de atividade</param>
        /// <returns>Código do registro do histórico de atividade criado</returns>
        public Int64 GravarHistorico(HistoricoAtividade historico)
        {
            using (Logger Log = Logger.IniciarLog("Gravar Histórico"))
            {
                try
                {
                    //Prepara mapeamentos de chamada
                    Mapper.CreateMap<HistoricoAtividade, Modelo.HistoricoAtividade>();

                    var modelo = Mapper.Map<Modelo.HistoricoAtividade>(historico);

                    //Grava registro no histórico de atividades
                    return new Negocio.HistoricoAtividade().GravarHistorico(modelo);
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
        /// Consulta os tipos de relatórios existentes
        /// </summary>
        /// <param name="ativo">Opcional: filtro de relatório pela flag "ativo"</param>
        /// <returns>Dicionário contendo os relatórios disponíveis</returns>
        public Dictionary<Int32, String> ConsultarTiposRelatorios(Boolean? ativo)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Tipos Relatórios"))
            {
                try
                {
                    return new Negocio.HistoricoAtividade().ConsultarTiposRelatorios(ativo);
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
        /// Consulta de Relatório do Histórico de Atividades
        /// </summary>
        /// <param name="codigoTipoRelatorio">Código do tipo de relatório</param>
        /// <param name="data">Data</param>
        /// <param name="codigoEntidade">Código do estabelecimento</param>
        /// <returns>Relatório</returns>
        public DataSet ConsultarRelatorio(Int32 codigoTipoRelatorio, DateTime? data, Int32? codigoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Relatório"))
            {
                try
                {
                    return new Negocio.HistoricoAtividade().ConsultarRelatorio(codigoTipoRelatorio, data, codigoEntidade);
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
        /// Consulta o histórico de envio de arquivos
        /// </summary>
        /// <param name="inicio">Data/Hora de início</param>
        /// <param name="fim">Data/Hora de fim</param>
        /// <returns>Lista de Históricos</returns>
        public ListaHistoricoConciliadorResponse<HistoricoEnvioArquivoConciliador> ConsultarHistoricoEnvioArquivos(String inicio, String fim)
        {
            using (Logger Log = Logger.IniciarLog("Consultar o histórico de envio de arquivos"))
            {
                try
                {
                    DateTime dataInicio = default(DateTime);
                    DateTime dataFim = default(DateTime);

                    AutoMapper.Mapper.CreateMap<Modelo.HistoricoEnvioArquivoConciliador, HistoricoEnvioArquivoConciliador>();
                    AutoMapper.Mapper.CreateMap<Modelo.ListaHistoricoConciliadorResponse<Modelo.HistoricoEnvioArquivoConciliador>, ListaHistoricoConciliadorResponse<HistoricoEnvioArquivoConciliador>>();

                    if (!DateTime.TryParse(inicio, out dataInicio))
                        throw new WebFaultException<String>("Data inicial inválida", System.Net.HttpStatusCode.BadRequest);

                    if (!DateTime.TryParse(fim, out dataFim))
                        throw new WebFaultException<String>("Data final inválida", System.Net.HttpStatusCode.BadRequest);

                    Modelo.ListaHistoricoConciliadorResponse<Modelo.HistoricoEnvioArquivoConciliador> modeloHistorico;

                    modeloHistorico = new Negocio.HistoricoAtividade().ConsultarHistoricoEnvioArquivos(dataInicio, dataFim);

                    return AutoMapper.Mapper.Map<Modelo.ListaHistoricoConciliadorResponse<Modelo.HistoricoEnvioArquivoConciliador>, 
                                                 ListaHistoricoConciliadorResponse<HistoricoEnvioArquivoConciliador>>(modeloHistorico);
                
                }
                catch (WebFaultException<String> ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<String>(
                        ex.Message, System.Net.HttpStatusCode.InternalServerError);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<String>(
                        ex.Message, System.Net.HttpStatusCode.InternalServerError);
                }
            }
        }

        /// <summary>
        /// Consulta o histórico de Contratações/Cancelamentos do Conciliador
        /// </summary>
        /// <param name="inicio">Data/Hora de início</param>
        /// <param name="fim">Data/Hora de fim</param>
        /// <returns>Lista de Históricos</returns>
        public ListaHistoricoConciliadorResponse<HistoricoConciliador> ConsultarHistoricoContratacaoCancelamento(String inicio, String fim)
        {
            using (Logger Log = Logger.IniciarLog("Consulta o histórico de Contratações/Cancelamentos do Conciliador"))
            {
                try
                {
                    DateTime dataInicio = default(DateTime);
                    DateTime dataFim = default(DateTime);

                    AutoMapper.Mapper.CreateMap<Modelo.HistoricoConciliador, HistoricoConciliador>();
                    AutoMapper.Mapper.CreateMap<Modelo.ListaHistoricoConciliadorResponse<Modelo.HistoricoConciliador>, ListaHistoricoConciliadorResponse<HistoricoConciliador>>();

                    if (!DateTime.TryParse(inicio, out dataInicio))
                        throw new WebFaultException<String>("Data inicial inválida", System.Net.HttpStatusCode.BadRequest);

                    if (!DateTime.TryParse(fim, out dataFim))
                        throw new WebFaultException<String>("Data final inválida", System.Net.HttpStatusCode.BadRequest);

                    Modelo.ListaHistoricoConciliadorResponse<Modelo.HistoricoConciliador> modeloHistorico;

                    modeloHistorico = new Negocio.HistoricoAtividade().ConsultarHistoricoContratacaoCancelamento(dataInicio, dataFim);

                    return AutoMapper.Mapper.Map<Modelo.ListaHistoricoConciliadorResponse<Modelo.HistoricoConciliador>,
                                                 ListaHistoricoConciliadorResponse<HistoricoConciliador>>(modeloHistorico);

                }
                catch (WebFaultException<String> ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<String>(
                        ex.Message, System.Net.HttpStatusCode.InternalServerError);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<String>(
                        ex.Message, System.Net.HttpStatusCode.InternalServerError);
                }
            }
        }

    }
}