/*
© Copyright 2017 Rede S.A.
Autor	: Dhouglas Oliveira
Empresa	: Iteris Consultoria e Software LTDA
*/

using AutoMapper;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Linq;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Serviço de faixada para comunicação com os serviços de Oferta do UK
    /// </summary>
    public class ServicoOferta : ServicoBase, IServicoOferta
    {
        /// <summary>
        /// Consultar Contratos/Ofertas através do UK
        /// </summary>
        /// <returns></returns>
        public List<Servicos.PlanoContas.Oferta> ConsultarOfertas(Int32 codigoEntidade)
        {
            using (Logger log = Logger.IniciarNovoLog("Consultar Contratos/Ofertas através do UK"))
            {
                log.GravarLog(EventoLog.InicioServico, new { codigoEntidade });

                List<Servicos.PlanoContas.Oferta> ofertasServico = default(List<PlanoContas.Oferta>);

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoEntidade);

                    //Retorna os dados da pesquisa do cache
                    ofertasServico = CacheAdmin.Recuperar<List<Servicos.PlanoContas.Oferta>>(Cache.Geral, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (ofertasServico == null)
                    {
                        //Mapeamentos do Modelo de Negocio e de Serviço
                        Mapper.CreateMap<Modelo.PlanoContas.Oferta, PlanoContas.Oferta>();
                        Mapper.CreateMap<Modelo.PlanoContas.StatusOferta, PlanoContas.StatusOferta>();

                        //Executa pesquisa
                        List<Modelo.PlanoContas.Oferta> ofertasModelo = new Negocio.OfertaUk().ConsultarOfertas(codigoEntidade);
                        ofertasServico = Mapper.Map<List<PlanoContas.Oferta>>(ofertasModelo);
                        if (ofertasServico == null)
                            ofertasServico = new PlanoContas.Oferta[0].ToList();

                        //Adiciona os dados retornados da consulta em cache, por 6 horas
                        CacheAdmin.Adicionar(Cache.Geral, idCache.ToString(), ofertasServico, new TimeSpan(6, 0, 0));
                    }
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

                return ofertasServico;
            }
        }

        /// <summary>
        /// Consultar o Contrato da Oferta
        /// </summary>
        /// <param name="codigoOferta">Código da Oferta a consultar o Contrato</param>
        /// <param name="codigoProposta">Código de Proposta da Oferta</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        public ContratoOferta ConsultarContratoOferta(Int32 codigoOferta, Int64 codigoProposta, Int64 codigoEntidade)
        {
            using (Logger log = Logger.IniciarNovoLog("Consultar o Contrato da Oferta através do UK"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    codigoEntidade,
                    codigoProposta,
                    codigoOferta
                });

                Servicos.ContratoOferta contratoServico = default(Servicos.ContratoOferta);

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoOferta);
                    idCache.Append(codigoProposta);
                    idCache.Append(codigoEntidade);

                    //Retorna os dados da pesquisa do cache
                    contratoServico = CacheAdmin.Recuperar<Servicos.ContratoOferta>(Cache.Geral, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (contratoServico == null)
                    {
                        //Mapeamentos do Modelo de Negocio e de Serviço
                        Mapper.CreateMap<Modelo.ContratoOferta, Servicos.ContratoOferta>();

                        //Executa pesquisa
                        Modelo.ContratoOferta contratoModelo = new Negocio.OfertaUk().ConsultarContratoOferta(codigoOferta, codigoProposta, codigoEntidade);
                        contratoServico = Mapper.Map<Servicos.ContratoOferta>(contratoModelo);

                        //Adiciona os dados retornados da consulta em cache, por 6 horas
                        CacheAdmin.Adicionar(Cache.Geral, idCache.ToString(), contratoServico, new TimeSpan(6, 0, 0));
                    }
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

                return contratoServico;
            }
        }

        /// <summary>
        /// Verificar se algum dos PVs passados possui Oferta de Taxa
        /// </summary>
        /// <param name="pvs">Lista de PVs para verificar</param>
        /// <returns>
        /// <para>True - 1 ou mais dos PVs possui Oferta de Taxa</para>
        /// <para>False - nenhum dos PVs possui Oferta de Taxa</para>
        /// </returns>
        public Boolean PossuiOfertaTaxa(List<Int32> pvs)
        {
            using (Logger log = Logger.IniciarNovoLog("Verificar se algum dos PVs passados possui Oferta de Taxa no UK"))
            {
                log.GravarLog(EventoLog.InicioServico,
                              new { pvs });

                Boolean possui = false;

                try
                {
                    Negocio.OfertaUk ofertaNegocio = new Negocio.OfertaUk();

                    possui = ofertaNegocio.PossuiOfertaTaxa(pvs);
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

                return possui;
            }
        }

        /// <summary>
        /// Obter listagem de Sazonalidades da Oferta no UK
        /// </summary>
        /// <param name="codigoOferta">Código da Oferta</param>
        /// <param name="codigoContrato">Código do Contrato</param>
        /// <param name="codigoEstruturaMeta">Código de Estrutura Meta</param>
        /// <returns></returns>
        public List<SazonalidadeOferta> ConsultarSazonalizades(Int32 codigoOferta, Int64 codigoContrato, Int32 codigoEstruturaMeta)
        {
            using (Logger log = Logger.IniciarNovoLog("Consultar Contratos/Ofertas através do UK"))
            {
                log.GravarLog(EventoLog.InicioServico,
                              new
                              {
                                  codigoOferta,
                                  codigoContrato,
                                  codigoEstruturaMeta
                              });

                List<SazonalidadeOferta> sazonalidadesServico = default(List<SazonalidadeOferta>);

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoOferta);
                    idCache.Append(codigoContrato);
                    idCache.Append(codigoEstruturaMeta);

                    //Retorna os dados da pesquisa do cache
                    sazonalidadesServico = CacheAdmin.Recuperar<List<SazonalidadeOferta>>(Cache.Geral, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (sazonalidadesServico == null)
                    {
                        //Mapeamentos do Modelo de Negocio e de Serviço
                        Mapper.CreateMap<Modelo.SazonalidadeOferta, SazonalidadeOferta>();

                        List<Modelo.SazonalidadeOferta> sazonalidadesModelo = new Negocio.OfertaUk()
                            .ConsultarSazonalizades(codigoOferta, codigoContrato, codigoEstruturaMeta);

                        sazonalidadesServico = Mapper.Map<List<SazonalidadeOferta>>(sazonalidadesModelo);
                        if (sazonalidadesServico == null)
                            sazonalidadesServico = new SazonalidadeOferta[0].ToList();

                        //Adiciona os dados retornados da consulta em cache, por 6 horas
                        CacheAdmin.Adicionar(Cache.Geral, idCache.ToString(), sazonalidadesServico, new TimeSpan(6, 0, 0));
                    }
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

                return sazonalidadesServico;
            }
        }

        /// <summary>
        /// Listar os ramos da Oferta do Estabelecimento
        /// </summary>
        /// <param name="codigoOferta"></param>
        /// <param name="codigoProposta"></param>
        /// <param name="numeroCnpj"></param>
        public List<RamosAtividadeOferta> ConsultarRamosOferta(Int64 codigoOferta, Int64 codigoProposta, Int64 numeroCnpj)
        {
            using (Logger log = Logger.IniciarNovoLog("Listar os ramos da Oferta do Estabelecimento através do UK"))
            {
                log.GravarLog(EventoLog.InicioServico,
                              new
                              {
                                  codigoOferta,
                                  codigoProposta,
                                  numeroCnpj
                              });

                List<RamosAtividadeOferta> ramosServico = default(List<RamosAtividadeOferta>);

                try
                {
                    //Mapeamentos do Modelo de Negocio e de Serviço
                    Mapper.CreateMap<Modelo.RamosAtividadeOferta, RamosAtividadeOferta>();

                    Negocio.OfertaUk ofertaNegocio = new Negocio.OfertaUk();

                    List<Modelo.RamosAtividadeOferta> ramosModelo = ofertaNegocio.ConsultarRamosOferta(codigoOferta,
                                    codigoProposta,
                                    numeroCnpj);

                    ramosServico = Mapper.Map<List<RamosAtividadeOferta>>(ramosModelo);
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

                return ramosServico;
            }
        }

        /// <summary>
        /// Consultar as faixas de meta da Oferta
        /// </summary>
        /// <param name="contrato">informações do Contrato da Oferta</param>
        /// <param name="sazonalidade">Sazonalidade selecionadada da Oferta</param>
        public List<FaixaMetaOferta> ConsultarFaixasMeta(ContratoOferta contrato, SazonalidadeOferta sazonalidade)
        {
            using (Logger log = Logger.IniciarNovoLog("Listar os ramos da Oferta do Estabelecimento através do UK"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    contrato,
                    sazonalidade
                });

                List<FaixaMetaOferta> faixasServico = default(List<FaixaMetaOferta>);

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    if (contrato != null)
                    {
                        idCache.Append(contrato.CodigoContrato);
                        idCache.Append(contrato.CodigoEstruturaMeta);
                        idCache.Append(contrato.CodigoOferta);
                    }
                    else idCache.Append("null");
                    if (sazonalidade != null)
                    {
                        idCache.Append(sazonalidade.MesInicio);
                        idCache.Append(sazonalidade.AnoInicio);
                    }
                    else idCache.Append("null");

                    //Retorna os dados da pesquisa do cache
                    faixasServico = CacheAdmin.Recuperar<List<FaixaMetaOferta>>(Cache.Geral, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (faixasServico == null)
                    {
                        //Mapeamentos do Modelo de Negocio e de Serviço
                        Mapper.CreateMap<Modelo.FaixaMetaOferta, FaixaMetaOferta>();

                        Mapper.CreateMap<ContratoOferta, Modelo.ContratoOferta>();
                        Mapper.CreateMap<SazonalidadeOferta, Modelo.SazonalidadeOferta>();

                        Modelo.ContratoOferta contratoModelo = Mapper.Map<Modelo.ContratoOferta>(contrato);
                        Modelo.SazonalidadeOferta sazonalidadeModelo = Mapper.Map<Modelo.SazonalidadeOferta>(sazonalidade);

                        List<Modelo.FaixaMetaOferta> faixasModelo = new Negocio.OfertaUk().ConsultarFaixasMeta(contratoModelo, sazonalidadeModelo);

                        faixasServico = Mapper.Map<List<FaixaMetaOferta>>(faixasModelo);
                        if (faixasServico == null)
                            faixasServico = new FaixaMetaOferta[0].ToList();

                        //Adiciona os dados retornados da consulta em cache, por 6 horas
                        CacheAdmin.Adicionar(Cache.Geral, idCache.ToString(), faixasServico, new TimeSpan(6, 0, 0));
                    }
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

                return faixasServico;
            }
        }

        /// <summary>
        /// Consultar as Taxas Crédito da Meta de uma Oferta
        /// </summary>
        /// <param name="contrato">Contrato da Oferta</param>
        /// <param name="sazonalidade">Sazonalidade da Oferta</param>
        /// <param name="numeroEstabelecimento">Número do Estabelecimento</param>
        /// <param name="codigoRamo">Código do Ramo</param>
        /// <param name="codigoFaixa">Código da Faixa</param>
        /// <returns>List of Modelo.ProdutoBandeiraMeta</returns>
        public List<ProdutoBandeiraMeta> ConsultarTaxasCredito(ContratoOferta contrato,
            SazonalidadeOferta sazonalidade,
            Int64 numeroEstabelecimento,
            Int32? codigoRamo,
            Int32 codigoFaixa)
        {
            using (Logger log = Logger.IniciarNovoLog(" Consultar as Taxas Crédito da Meta de uma Oferta através do UK"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    contrato,
                    sazonalidade,
                    numeroEstabelecimento,
                    codigoRamo,
                    codigoFaixa
                });

                List<ProdutoBandeiraMeta> produtosServico = default(List<ProdutoBandeiraMeta>);

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    if (contrato != null)
                    {
                        idCache.Append(contrato.CodigoContrato);
                        idCache.Append(contrato.CodigoEstruturaMeta);
                        idCache.Append(contrato.CodigoOferta);
                    }
                    else idCache.Append("null");
                    if (sazonalidade != null)
                    {
                        idCache.Append(sazonalidade.MesInicio);
                        idCache.Append(sazonalidade.AnoInicio);
                    }
                    else idCache.Append("null");
                    idCache.Append(numeroEstabelecimento);
                    idCache.Append(codigoRamo);
                    idCache.Append(codigoFaixa);


                    //Retorna os dados da pesquisa do cache
                    produtosServico = CacheAdmin.Recuperar<List<ProdutoBandeiraMeta>>(Cache.Geral, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (produtosServico == null)
                    {
                        //Mapeamentos do Modelo de Negocio e de Serviço
                        Mapper.CreateMap<Modelo.TaxaMeta, TaxaMeta>();
                        Mapper.CreateMap<Modelo.ProdutoBandeiraMeta, ProdutoBandeiraMeta>();
                        Mapper.CreateMap<ContratoOferta, Modelo.ContratoOferta>();
                        Mapper.CreateMap<SazonalidadeOferta, Modelo.SazonalidadeOferta>();

                        Modelo.ContratoOferta contratoModelo = Mapper.Map<Modelo.ContratoOferta>(contrato);
                        Modelo.SazonalidadeOferta sazonalidadeModelo = Mapper.Map<Modelo.SazonalidadeOferta>(sazonalidade);

                        List<Modelo.ProdutoBandeiraMeta> produtosModelo = new Negocio.OfertaUk().ConsultarTaxasCredito(
                            contratoModelo,
                            sazonalidadeModelo,
                            numeroEstabelecimento,
                            codigoRamo,
                            codigoFaixa);

                        produtosServico = Mapper.Map<List<ProdutoBandeiraMeta>>(produtosModelo);
                        if (produtosServico == null)
                            produtosServico = new ProdutoBandeiraMeta[0].ToList();

                        //Adiciona os dados retornados da consulta em cache, por 6 horas
                        CacheAdmin.Adicionar(Cache.Geral, idCache.ToString(), produtosServico, new TimeSpan(6, 0, 0));
                    }
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

                return produtosServico;
            }
        }

        /// <summary>
        /// Consultar as Taxas Débito da Meta de uma Oferta
        /// </summary>
        /// <param name="contrato">Contrato da Oferta</param>
        /// <param name="sazonalidade">Sazonalidade da Oferta</param>
        /// <param name="numeroEstabelecimento">Número do Estabelecimento</param>
        /// <param name="codigoRamo">Código do Ramo</param>
        /// <param name="codigoFaixa">Código da Faixa</param>
        /// <returns>List of Modelo.ProdutoBandeiraMeta</returns>
        public List<ProdutoBandeiraMeta> ConsultarTaxasDebito(ContratoOferta contrato,
            SazonalidadeOferta sazonalidade,
            Int64 numeroEstabelecimento,
            Int32? codigoRamo,
            Int32 codigoFaixa)
        {
            using (Logger log = Logger.IniciarNovoLog(" Consultar as Taxas Débito da Meta de uma Oferta através do UK"))
            {
                log.GravarLog(EventoLog.InicioServico,
                              new
                              {
                                  contrato,
                                  sazonalidade,
                                  numeroEstabelecimento,
                                  codigoRamo,
                                  codigoFaixa
                              });

                List<ProdutoBandeiraMeta> produtosServico = default(List<ProdutoBandeiraMeta>);

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    if (contrato != null)
                    {
                        idCache.Append(contrato.CodigoContrato);
                        idCache.Append(contrato.CodigoEstruturaMeta);
                        idCache.Append(contrato.CodigoOferta);
                    }
                    else idCache.Append("null");
                    if (sazonalidade != null)
                    {
                        idCache.Append(sazonalidade.MesInicio);
                        idCache.Append(sazonalidade.AnoInicio);
                    }
                    else idCache.Append("null");
                    idCache.Append(numeroEstabelecimento);
                    idCache.Append(codigoRamo);
                    idCache.Append(codigoFaixa);

                    //Retorna os dados da pesquisa do cache
                    produtosServico = CacheAdmin.Recuperar<List<ProdutoBandeiraMeta>>(Cache.Geral, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (produtosServico == null)
                    {
                        //Mapeamentos do Modelo de Negocio e de Serviço
                        Mapper.CreateMap<Modelo.TaxaMeta, TaxaMeta>();
                        Mapper.CreateMap<Modelo.ProdutoBandeiraMeta, ProdutoBandeiraMeta>();
                        Mapper.CreateMap<ContratoOferta, Modelo.ContratoOferta>();
                        Mapper.CreateMap<SazonalidadeOferta, Modelo.SazonalidadeOferta>();

                        Modelo.ContratoOferta contratoModelo = Mapper.Map<Modelo.ContratoOferta>(contrato);
                        Modelo.SazonalidadeOferta sazonalidadeModelo = Mapper.Map<Modelo.SazonalidadeOferta>(sazonalidade);

                        List<Modelo.ProdutoBandeiraMeta> produtosModelo = new Negocio.OfertaUk().ConsultarTaxasDebito(
                            contratoModelo,
                            sazonalidadeModelo,
                            numeroEstabelecimento,
                            codigoRamo,
                            codigoFaixa);

                        produtosServico = Mapper.Map<List<ProdutoBandeiraMeta>>(produtosModelo);
                        if (produtosServico == null)
                            produtosServico = new ProdutoBandeiraMeta[0].ToList();

                        //Adiciona os dados retornados da consulta em cache, por 6 horas
                        CacheAdmin.Adicionar(Cache.Geral, idCache.ToString(), produtosServico, new TimeSpan(6, 0, 0));
                    }
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

                return produtosServico;
            }
        }

        /// <summary>
        /// Listar os históricos da Oferta
        /// <param name="contrato">Contrato com as informações de Código de Proposta e Código de Oferta</param>
        /// </summary>
        public List<HistoricoOferta> ConsultarHistoricoOferta(ContratoOferta contrato)
        {
            using (Logger log = Logger.IniciarNovoLog("Listar os históricos da Oferta através do UK"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    contrato
                });

                List<HistoricoOferta> historicosServico = default(List<HistoricoOferta>);

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    if (contrato != null)
                    {
                        idCache.Append(contrato.CodigoContrato);
                        idCache.Append(contrato.CodigoEstruturaMeta);
                        idCache.Append(contrato.CodigoOferta);
                        idCache.Append(contrato.CodigoProposta);
                    }
                    else idCache.Append("null");

                    //Retorna os dados da pesquisa do cache
                    historicosServico = CacheAdmin.Recuperar<List<HistoricoOferta>>(Cache.Geral, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (historicosServico == null)
                    {
                        //Mapeamentos do Modelo de Negocio e de Serviço
                        Mapper.CreateMap<Modelo.HistoricoOferta, HistoricoOferta>();
                        Mapper.CreateMap<Modelo.TipoEstabelecimentoHistoricoOferta, TipoEstabelecimentoHistoricoOferta>();
                        Mapper.CreateMap<ContratoOferta, Modelo.ContratoOferta>();

                        Modelo.ContratoOferta contratoModelo = Mapper.Map<Modelo.ContratoOferta>(contrato);
                        List<Modelo.HistoricoOferta> historicosModelo = new Negocio.OfertaUk().ConsultarHistoricoOferta(contratoModelo);

                        historicosServico = Mapper.Map<List<HistoricoOferta>>(historicosModelo);
                        if (historicosServico == null)
                            historicosServico = new HistoricoOferta[0].ToList();

                        //Adiciona os dados retornados da consulta em cache, por 6 horas
                        CacheAdmin.Adicionar(Cache.Geral, idCache.ToString(), historicosServico, new TimeSpan(6, 0, 0));
                    }
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

                return historicosServico;
            }
        }

        /// <summary>
        /// Listar os estabelecimentos dos históricos da Oferta no UK
        /// </summary>
        /// <param name="historico">Informações do Histórico da Ofeta</param>
        /// <returns></returns>
        public List<EstabelecimentoHistoricoOferta> ConsultarEstabelecimentosOferta(HistoricoOferta historico)
        {
            using (Logger log = Logger.IniciarNovoLog("Listar os históricos da Oferta através do UK"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    historico
                });

                List<EstabelecimentoHistoricoOferta> estabelecimentosServico = default(List<EstabelecimentoHistoricoOferta>);

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    if (historico != null)
                    {
                        idCache.Append(historico.CodigoProposta);
                        idCache.Append(historico.CodigoOferta);
                        idCache.Append(historico.DataAceitePropostaOferta);
                        idCache.Append(historico.MesReferenciaApuracao);
                        idCache.Append(historico.AnoReferenciaApuracao);
                    }
                    else idCache.Append("null");

                    //Retorna os dados da pesquisa do cache
                    estabelecimentosServico = CacheAdmin.Recuperar<List<EstabelecimentoHistoricoOferta>>(Cache.Geral, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (estabelecimentosServico == null)
                    {
                        //Mapeamentos do Modelo de Negocio e de Serviço
                        Mapper.CreateMap<Modelo.EstabelecimentoHistoricoOferta, EstabelecimentoHistoricoOferta>();

                        Mapper.CreateMap<HistoricoOferta, Modelo.HistoricoOferta>();
                        Mapper.CreateMap<TipoEstabelecimentoHistoricoOferta, Modelo.TipoEstabelecimentoHistoricoOferta>();

                        Modelo.HistoricoOferta historicoModelo = Mapper.Map<Modelo.HistoricoOferta>(historico);

                        Negocio.OfertaUk ofertaNegocio = new Negocio.OfertaUk();

                        List<Modelo.EstabelecimentoHistoricoOferta> estabelecimentosModelo = ofertaNegocio.ConsultarEstabelecimentosOferta(historicoModelo);

                        estabelecimentosServico = Mapper.Map<List<EstabelecimentoHistoricoOferta>>(estabelecimentosModelo);
                        if (estabelecimentosServico == null)
                            estabelecimentosServico = new EstabelecimentoHistoricoOferta[0].ToList();

                        //Adiciona os dados retornados da consulta em cache, por 6 horas
                        CacheAdmin.Adicionar(Cache.Geral, idCache.ToString(), estabelecimentosServico, new TimeSpan(6, 0, 0));
                    }
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

                return estabelecimentosServico;
            }
        }

        /// <summary>
        /// Consultar as Taxas Crédito da Meta do Histórico da Oferta no UK
        /// </summary>
        /// <param name="historico">Histórico</param>
        /// <returns></returns>
        public List<ProdutoBandeiraMeta> ConsultarTaxasCreditoHistorico(HistoricoOferta historico)
        {
            using (Logger log = Logger.IniciarNovoLog("Consultar as Taxas Crédito da Meta do Histórico da Oferta no UK"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    historico
                });

                List<ProdutoBandeiraMeta> produtosServico = default(List<ProdutoBandeiraMeta>);

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    if (historico != null)
                    {
                        idCache.Append(historico.CodigoProposta);
                        idCache.Append(historico.CodigoOferta);
                        idCache.Append(historico.DataAceitePropostaOferta);
                        idCache.Append(historico.MesReferenciaApuracao);
                        idCache.Append(historico.AnoReferenciaApuracao);
                    }
                    else idCache.Append("null");

                    //Retorna os dados da pesquisa do cache
                    produtosServico = CacheAdmin.Recuperar<List<ProdutoBandeiraMeta>>(Cache.Geral, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (produtosServico == null)
                    {
                        //Mapeamentos do Modelo de Negocio e de Serviço
                        Mapper.CreateMap<Modelo.TaxaMeta, TaxaMeta>();
                        Mapper.CreateMap<Modelo.ProdutoBandeiraMeta, ProdutoBandeiraMeta>();
                        Mapper.CreateMap<HistoricoOferta, Modelo.HistoricoOferta>();
                        Mapper.CreateMap<TipoEstabelecimentoHistoricoOferta, Modelo.TipoEstabelecimentoHistoricoOferta>();

                        Modelo.HistoricoOferta historicoModelo = Mapper.Map<Modelo.HistoricoOferta>(historico);
                        List<Modelo.ProdutoBandeiraMeta> produtosModelo = new Negocio.OfertaUk().ConsultarTaxasCreditoHistorico(historicoModelo);

                        produtosServico = Mapper.Map<List<ProdutoBandeiraMeta>>(produtosModelo);
                        if (produtosServico == null)
                            produtosServico = new ProdutoBandeiraMeta[0].ToList();

                        //Adiciona os dados retornados da consulta em cache, por 6 horas
                        CacheAdmin.Adicionar(Cache.Geral, idCache.ToString(), produtosServico, new TimeSpan(6, 0, 0));
                    }
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

                return produtosServico;
            }
        }

        /// <summary>
        /// Consultar as Taxas Débito da Meta do Histórico da Oferta no UK
        /// </summary>
        /// <param name="historico">Histórico</param>
        /// <returns></returns>
        public List<ProdutoBandeiraMeta> ConsultarTaxasDebitoHistorico(HistoricoOferta historico)
        {
            using (Logger log = Logger.IniciarNovoLog("Consultar as Taxas Débito da Meta do Histórico da Oferta no UK"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    historico
                });

                List<ProdutoBandeiraMeta> produtosServico = default(List<ProdutoBandeiraMeta>);

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    if (historico != null)
                    {
                        idCache.Append(historico.CodigoProposta);
                        idCache.Append(historico.CodigoOferta);
                        idCache.Append(historico.DataAceitePropostaOferta);
                        idCache.Append(historico.MesReferenciaApuracao);
                        idCache.Append(historico.AnoReferenciaApuracao);
                    }
                    else idCache.Append("null");

                    //Retorna os dados da pesquisa do cache
                    produtosServico = CacheAdmin.Recuperar<List<ProdutoBandeiraMeta>>(Cache.Geral, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (produtosServico == null)
                    {
                        //Mapeamentos do Modelo de Negocio e de Serviço
                        Mapper.CreateMap<Modelo.TaxaMeta, TaxaMeta>();
                        Mapper.CreateMap<Modelo.ProdutoBandeiraMeta, ProdutoBandeiraMeta>();
                        Mapper.CreateMap<HistoricoOferta, Modelo.HistoricoOferta>();
                        Mapper.CreateMap<TipoEstabelecimentoHistoricoOferta, Modelo.TipoEstabelecimentoHistoricoOferta>();

                        Modelo.HistoricoOferta historicoModelo = Mapper.Map<Modelo.HistoricoOferta>(historico);
                        List<Modelo.ProdutoBandeiraMeta> produtosModelo = new Negocio.OfertaUk().ConsultarTaxasDebitoHistorico(historicoModelo);

                        produtosServico = Mapper.Map<List<ProdutoBandeiraMeta>>(produtosModelo);
                        if (produtosServico == null)
                            produtosServico = new ProdutoBandeiraMeta[0].ToList();

                        //Adiciona os dados retornados da consulta em cache, por 6 horas
                        CacheAdmin.Adicionar(Cache.Geral, idCache.ToString(), produtosServico, new TimeSpan(6, 0, 0));
                    }
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

                return produtosServico;
            }
        }
    }
}
