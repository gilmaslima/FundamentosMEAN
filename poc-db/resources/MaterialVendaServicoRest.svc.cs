/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Software e Consultoria.
*/


using System;
using AutoMapper;
using System.Linq;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Collections.Generic;
using Redecard.PN.OutrosServicos.ContratoDados;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Serviço de consultas e solicitações de Material de Vendas
    /// </summary>
    public class MaterialVendaServicoRest : ServicoBase, IMaterialVendaServicoRest
    {
        /// <summary>
        /// Lista as últimas remessas enviadas para o estabelecimento
        /// </summary>
        public ResponseBaseList<Remessa> ConsultarUltimasRemessas(String codigoPV)
        {
            using (Logger log = Logger.IniciarLog("Lista as últimas remessas enviadas para o estabelecimento"))
            {
                log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();
                    log.GravarLog(EventoLog.ChamadaNegocio, new { codigoPV });

                    List<Modelo.Remessa> remessas = negocioMaterialVenda.ConsultarUltimasRemessas(codigoPV.ToInt32());
                    log.GravarLog(EventoLog.RetornoNegocio, new { remessas });

                    List<Servicos.Remessa> remessasServ = new List<Servicos.Remessa>();


                    if (!object.ReferenceEquals(remessas, null) && remessas.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Remessa, Servicos.Remessa>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();

                        remessas.ForEach(delegate(Modelo.Remessa r)
                        {
                            remessasServ.Add(Mapper.Map<Servicos.Remessa>(r));
                        });
                    }

                    log.GravarLog(EventoLog.FimServico, new { remessas = remessasServ });

                    return new ResponseBaseList<Remessa> { Itens = remessasServ.ToArray() };
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

        /// <summary>
        /// Lista as próximas remessas enviadas para o estabelecimento
        /// </summary>
        public ResponseBaseList<Remessa> ConsultarProximasRemessas(String codigoPV)
        {
            using (Logger log = Logger.IniciarLog("Lista as próximas remessas enviadas para o estabelecimento"))
            {
                log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    log.GravarLog(EventoLog.ChamadaNegocio, new { codigoPV });
                    List<Modelo.Remessa> remessas = negocioMaterialVenda.ConsultarProximasRemessas(codigoPV.ToInt32());
                    log.GravarLog(EventoLog.RetornoNegocio, new { remessas });

                    List<Servicos.Remessa> remessasServ = new List<Servicos.Remessa>();

                    if (!object.ReferenceEquals(remessas, null) && remessas.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Remessa, Servicos.Remessa>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();

                        remessas.ForEach(delegate(Modelo.Remessa r)
                        {
                            remessasServ.Add(Mapper.Map<Servicos.Remessa>(r));
                        });
                    }

                    log.GravarLog(EventoLog.FimServico, new { remessas = remessasServ });

                    return new ResponseBaseList<Remessa> { Itens = remessasServ.ToArray() }; ;
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

        /// <summary>
        /// Lista os motivos das remessas de Material de venda
        /// </summary>
        public ResponseBaseList<Motivo> ConsultarMotivos()
        {
            using (Logger log = Logger.IniciarLog("Lista os motivos das remessas de Material de venda"))
            {
                log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    log.GravarLog(EventoLog.ChamadaNegocio, new { });
                    List<Modelo.Motivo> motivos = negocioMaterialVenda.ConsultarMotivos();
                    log.GravarLog(EventoLog.RetornoNegocio, new { motivos });

                    List<Servicos.Motivo> motivosServ = new List<Servicos.Motivo>();

                    if (!object.ReferenceEquals(motivos, null) && motivos.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        motivos.ForEach(delegate(Modelo.Motivo r)
                        {
                            motivosServ.Add(Mapper.Map<Servicos.Motivo>(r));
                        });
                    }

                    log.GravarLog(EventoLog.FimServico, new { motivos = motivosServ });
                    return new ResponseBaseList<Motivo> { Itens = motivosServ.ToArray() };
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

        /// <summary>
        /// consultar kits de material de vendas para o estabelecimento
        /// </summary>
        /// <param name="codigoPV"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public ResponseBaseList<Kit> ConsultarKitsVendas(String codigoPV)
        {
            using (Logger log = Logger.IniciarLog("consultar kits de material de vendas para o estabelecimento"))
            {
                log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    log.GravarLog(EventoLog.ChamadaNegocio, new { numeroPV = codigoPV });
                    List<Modelo.Kit> kits = negocioMaterialVenda.ConsultarKitsVendas(codigoPV.ToInt32());
                    log.GravarLog(EventoLog.RetornoNegocio, new { kits });

                    List<Servicos.Kit> kitsServ = new List<Servicos.Kit>();

                    if (!object.ReferenceEquals(kits, null) && kits.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        kits.ForEach(delegate(Modelo.Kit r)
                        {
                            kitsServ.Add(Mapper.Map<Servicos.Kit>(r));
                        });
                    }

                    log.GravarLog(EventoLog.FimServico, new { kits = kitsServ });

                    return new ResponseBaseList<Kit> { Itens = kitsServ.ToArray() };
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
        
        /// <summary>
        /// consultar kits de material de sinalização para o estabelecimento
        /// </summary>
        /// <param name="codigoPV"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public ResponseBaseList<Kit> ConsultarKitsSinalizacao(String codigoPV)
        {
            using (Logger log = Logger.IniciarLog("Consultar kits de material de sinalização para o estabelecimento"))
            {
                log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    log.GravarLog(EventoLog.ChamadaNegocio, new { numeroPV = codigoPV });
                    List<Modelo.Kit> kits = negocioMaterialVenda.ConsultarKitsSinalizacao(codigoPV.ToInt32());
                    log.GravarLog(EventoLog.RetornoNegocio, new { kits });

                    List<Servicos.Kit> kitsServ = new List<Servicos.Kit>();

                    if (!object.ReferenceEquals(kits, null) && kits.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        kits.ForEach(delegate(Modelo.Kit r)
                        {
                            kitsServ.Add(Mapper.Map<Servicos.Kit>(r));
                        });
                    }
                    log.GravarLog(EventoLog.FimServico, new { kits = kitsServ });
                    return new ResponseBaseList<Kit> { Itens = kitsServ.ToArray() };
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
       
        /// <summary>
        /// consultar kits de material de tecnologia para o estabelecimento
        /// </summary>
        /// <param name="codigoPV"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public ResponseBaseList<Kit> ConsultarKitsTecnologia(String codigoPV)
        {
            using (Logger log = Logger.IniciarLog("Consultar kits de material de tecnologia para o estabelecimento"))
            {
                log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    log.GravarLog(EventoLog.ChamadaNegocio, new { numeroPV = codigoPV });
                    List<Modelo.Kit> kits = negocioMaterialVenda.ConsultarKitsTecnologia(codigoPV.ToInt32());
                    log.GravarLog(EventoLog.RetornoNegocio, new { kits });

                    List<Servicos.Kit> kitsServ = new List<Servicos.Kit>();

                    if (!object.ReferenceEquals(kits, null) && kits.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        kits.ForEach(delegate(Modelo.Kit r)
                        {
                            kitsServ.Add(Mapper.Map<Servicos.Kit>(r));
                        });
                    }
                    log.GravarLog(EventoLog.FimServico, new { kits = kitsServ });

                    return new ResponseBaseList<Kit> { Itens = kitsServ.ToArray() };
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

        /// <summary>
        /// consultar kits de material de apoio para o estabelecimento
        /// </summary>
        /// <param name="codigoPV"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public ResponseBaseList<Kit> ConsultarKitsApoio(String codigoPV)
        {
            using (Logger log = Logger.IniciarLog("consultar kits de material de apoio para o estabelecimento"))
            {
                log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    log.GravarLog(EventoLog.ChamadaNegocio, new { numeroPV = codigoPV });
                    List<Modelo.Kit> kits = negocioMaterialVenda.ConsultarKitsApoio(codigoPV.ToInt32());
                    log.GravarLog(EventoLog.RetornoNegocio, new { kits });

                    List<Servicos.Kit> kitsServ = new List<Servicos.Kit>();

                    if (!object.ReferenceEquals(kits, null) && kits.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        kits.ForEach(delegate(Modelo.Kit r)
                        {
                            kitsServ.Add(Mapper.Map<Servicos.Kit>(r));
                        });
                    }
                    log.GravarLog(EventoLog.FimServico, new { kits = kitsServ });

                    return new ResponseBaseList<Kit> { Itens = kitsServ.ToArray() };
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

        /// <summary>
        /// Consulta a quantidade máxima de Kits de Sinalização que podem ser solicitadas
        /// </summary>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public ResponseBaseItem<String> ConsultarQuantidadeMaximaKitsSinalizacao()
        {
            using (Logger log = Logger.IniciarLog("Consulta a quantidade máxima de Kits de Sinalização que podem ser solicitadas"))
            {
                log.GravarLog(EventoLog.InicioServico);
                try
                {
                    log.GravarLog(EventoLog.ChamadaNegocio);
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();
                    Int32 codigoRetorno;
                    var result = negocioMaterialVenda.ConsultarQuantidadeMaximaKitsSinalizacao(out codigoRetorno);

                    log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    log.GravarLog(EventoLog.FimServico, new { result });

                    return new ResponseBaseItem<String>
                    {
                        Item = result,
                        StatusRetorno = (StatusRetorno)codigoRetorno,
                        Mensagem = codigoRetorno != 0 ? "Erro ao efetuar a operação." : ""
                    };
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
       
        /// <summary>
        /// Consulta a quantidade máxima de Kits de Apoio que podem ser solicitadas
        /// </summary>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public ResponseBaseItem<String> ConsultarQuantidadeMaximaKitsApoio()
        {
            using (Logger log = Logger.IniciarLog("Consulta a quantidade máxima de Kits de Apoio que podem ser solicitadas"))
            {
                log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    log.GravarLog(EventoLog.ChamadaNegocio);
                    Int32 codigoRetorno;
                    var result = negocioMaterialVenda.ConsultarQuantidadeMaximaKitsApoio(out codigoRetorno);
                    log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    log.GravarLog(EventoLog.FimServico, new { result });
                    return new ResponseBaseItem<String>
                    {
                        Item = result,
                        StatusRetorno = (StatusRetorno)codigoRetorno,
                        Mensagem = codigoRetorno != 0 ? "Erro ao efetuar a operação." : ""
                    };
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
       
        /// <summary>
        /// Consultar a composição de um Kit para o estabelecimento
        /// </summary>
        public ResponseBaseList<Material> ConsultarComposicaoKit(String codigoKit)
        {
            using (Logger log = Logger.IniciarLog("Consultar a composição de um Kit para o estabelecimento"))
            {
                log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    log.GravarLog(EventoLog.ChamadaNegocio, new { codigoKit });
                    List<Modelo.Material> materiais = negocioMaterialVenda.ConsultarComposicaoKit(codigoKit.ToInt32());
                    log.GravarLog(EventoLog.RetornoNegocio, new { materiais });

                    List<Servicos.Material> materiaisServ = new List<Servicos.Material>();

                    if (!object.ReferenceEquals(materiais, null) && materiais.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Material, Servicos.Material>();
                        materiais.ForEach(delegate(Modelo.Material r)
                        {
                            materiaisServ.Add(Mapper.Map<Servicos.Material>(r));
                        });
                    }
                    log.GravarLog(EventoLog.FimServico, new { materiais = materiaisServ });

                    return new ResponseBaseList<Material> { Itens = materiaisServ.ToArray() };
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

        /// <summary>
        /// Inclui uma nova solicitação de material de venda/tecnologia/apoio/sinalização do
        /// estabelecimento especificado.
        /// </summary>
        /// <returns>
        /// Código de retorno, caso 0 todas as solicitações foram incluídas com sucesso, caso contrário, 
        /// retorna o código de erro de inclusão.
        /// </returns>
        public ResponseBaseItem<Int32> IncluirKit(IncluirKitRequest request)
        {
            using (Logger log = Logger.IniciarLog("Inclui uma nova solicitação de material de venda/tecnologia/apoio/sinalização do estabelecimento especificado."))
            {
                log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Modelo.Endereco enderecoMod = null;
                    Modelo.Kit[] modelosKits = new Modelo.Kit[request.Kits.Length];

                    Int32 retorno = 0;

                    if (!object.ReferenceEquals(request.Endereco, null))
                    {
                        Mapper.CreateMap<Servicos.Endereco, Modelo.Endereco>();
                        enderecoMod = Mapper.Map<Modelo.Endereco>(request.Endereco);
                    }

                    if (request.Kits.Length > 0)
                    {
                        Mapper.CreateMap<Servicos.Kit, Modelo.Kit>();
                        Mapper.CreateMap<Servicos.Motivo, Modelo.Motivo>();
                        Int32 i = 0;

                        foreach (Servicos.Kit kit in request.Kits)
                        {
                            modelosKits[i] = Mapper.Map<Modelo.Kit>(kit);
                            i++;
                        }

                        Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                        log.GravarLog(EventoLog.ChamadaNegocio, new { modelosKits, request.CodigoPV, request.DescricaoPV, request.Usuario, request.Solicitante, request.EnderecoTemporario, enderecoMod });

                        retorno = negocioMaterialVenda.IncluirKit(modelosKits, request.CodigoPV, request.DescricaoPV, request.Usuario, request.Solicitante, request.EnderecoTemporario, enderecoMod);
                    }
                    log.GravarLog(EventoLog.RetornoNegocio);

                    log.GravarLog(EventoLog.FimServico, new { retorno });
                    
		    return new ResponseBaseItem<Int32>
                    {
                        Item = retorno,
                        StatusRetorno = (StatusRetorno)retorno,
                        Mensagem = retorno != 0 ? "Erro ao efetuar a solicitação" : ""
                    };
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

        /// <summary>
        /// Consultar a informações resumidas para a rotina de solicitação de materiais.
        /// </summary>
        public ResponseBaseItem<InfoMateriaisResponse> ConsultarInfoMateriais(String codigoPV) {
            using (Logger log = Logger.IniciarLog("consultar informações resumidas para rotina se solicitação de materiais para um estabelecimento."))
            {
                log.GravarLog(EventoLog.InicioServico);
                try
                {
                    InfoMateriaisResponse retorno = new InfoMateriaisResponse();
                    
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    log.GravarLog(EventoLog.ChamadaNegocio, new { codigoPV });
                    List<Modelo.Kit> materiais = negocioMaterialVenda.ConsultarKitsVendas(codigoPV.ToInt32());
                    retorno.PossuiKitsMateriais = materiais != null && materiais.Any();
                    log.GravarLog(EventoLog.RetornoNegocio, new { materiais });

                    log.GravarLog(EventoLog.ChamadaNegocio, new { codigoPV });
                    List<Modelo.Kit> sinalizacao = negocioMaterialVenda.ConsultarKitsSinalizacao(codigoPV.ToInt32());
                    retorno.PossuikitsSinalizacao = sinalizacao != null && sinalizacao.Any();
                    log.GravarLog(EventoLog.RetornoNegocio, new { sinalizacao });

                    //PEDIDOS EM ABERTO
                    log.GravarLog(EventoLog.ChamadaNegocio, new { codigoPV });
                    List<Modelo.Remessa> remessas = negocioMaterialVenda.ConsultarProximasRemessas(codigoPV.ToInt32());
                    log.GravarLog(EventoLog.RetornoNegocio, new { remessas });

                    retorno.PossuiSolicitacaoMaterialAberta = remessas != null && 
                                                              remessas.Any(a => a.Kit != null &&
                                                                                retorno.PossuiKitsMateriais &&
                                                                                materiais.Select(s => (String.IsNullOrEmpty(s.DescricaoKit) ? "" : s.DescricaoKit).Trim())
                                                                                                     .Contains((String.IsNullOrEmpty(a.Kit.DescricaoKit) ? "" : a.Kit.DescricaoKit).Trim()));
                    retorno.PossuiSolicitacaoSinalizacaoAberta = remessas != null &&
                                                                 remessas.Any(a => a.Kit != null && 
                                                                                   retorno.PossuikitsSinalizacao &&              
                                                                                   sinalizacao.Select(s => (String.IsNullOrEmpty(s.DescricaoKit) ? "" : s.DescricaoKit).Trim())
                                                                                                                           .Contains((String.IsNullOrEmpty(a.Kit.DescricaoKit) ? "" : a.Kit.DescricaoKit).Trim()));
                    //

                    log.GravarLog(EventoLog.FimServico, new { retorno });

                    return new ResponseBaseItem<InfoMateriaisResponse> { Item = retorno };
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