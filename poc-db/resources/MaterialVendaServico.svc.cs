#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [12/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using AutoMapper;
using Redecard.PN.Comum;
using System.ServiceModel;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Serviço de consultas e solicitações de Material de Vendas
    /// </summary>
    public class MaterialVendaServico : ServicoBase, IMaterialVendaServico
    {
        /// <summary>
        /// Lista as últimas remessas enviadas para o estabelecimento
        /// </summary>
        public List<Remessa> ConsultarUltimasRemessas(Int32 codigoPV)
        {
            using (Logger Log = Logger.IniciarLog("Lista as últimas remessas enviadas para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoPV });

                    List<Modelo.Remessa> remessas = negocioMaterialVenda.ConsultarUltimasRemessas(codigoPV);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { remessas });

                    List<Servicos.Remessa> _remessas = new List<Servicos.Remessa>();


                    if (!object.ReferenceEquals(remessas, null) && remessas.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Remessa, Servicos.Remessa>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();

                        remessas.ForEach(delegate(Modelo.Remessa r)
                        {
                            _remessas.Add(Mapper.Map<Servicos.Remessa>(r));
                        });
                    }

                    Log.GravarLog(EventoLog.FimServico, new { _remessas });

                    return _remessas;
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
        /// Lista as próximas remessas enviadas para o estabelecimento
        /// </summary>
        public List<Remessa> ConsultarProximasRemessas(Int32 codigoPV)
        {
            using (Logger Log = Logger.IniciarLog("Lista as próximas remessas enviadas para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoPV });
                    List<Modelo.Remessa> remessas = negocioMaterialVenda.ConsultarProximasRemessas(codigoPV);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { remessas });

                    List<Servicos.Remessa> _remessas = new List<Servicos.Remessa>();

                    if (!object.ReferenceEquals(remessas, null) && remessas.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Remessa, Servicos.Remessa>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();

                        remessas.ForEach(delegate(Modelo.Remessa r)
                        {
                            _remessas.Add(Mapper.Map<Servicos.Remessa>(r));
                        });
                    }

                    Log.GravarLog(EventoLog.FimServico, new { _remessas });

                    return _remessas;
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
        /// Lista os motivos das remessas de Material de venda
        /// </summary>
        public List<Motivo> ConsultarMotivos()
        {
            using (Logger Log = Logger.IniciarLog("Lista os motivos das remessas de Material de venda"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { });
                    List<Modelo.Motivo> remessas = negocioMaterialVenda.ConsultarMotivos();
                    Log.GravarLog(EventoLog.RetornoNegocio, new { });

                    List<Servicos.Motivo> _remessas = new List<Servicos.Motivo>();

                    if (!object.ReferenceEquals(remessas, null) && remessas.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        remessas.ForEach(delegate(Modelo.Motivo r)
                        {
                            _remessas.Add(Mapper.Map<Servicos.Motivo>(r));
                        });
                    }

                    Log.GravarLog(EventoLog.FimServico, new { _remessas });
                    return _remessas;
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
        /// consultar kits de material de vendas para o estabelecimento
        /// </summary>
        /// <param name="numeroPV"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public List<Kit> ConsultarKitsVendas(int numeroPV)
        {
            using (Logger Log = Logger.IniciarLog("consultar kits de material de vendas para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroPV });
                    List<Modelo.Kit> remessas = negocioMaterialVenda.ConsultarKitsVendas(numeroPV);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { remessas });

                    List<Servicos.Kit> _remessas = new List<Servicos.Kit>();

                    if (!object.ReferenceEquals(remessas, null) && remessas.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        remessas.ForEach(delegate(Modelo.Kit r)
                        {
                            _remessas.Add(Mapper.Map<Servicos.Kit>(r));
                        });
                    }

                    Log.GravarLog(EventoLog.FimServico, new { _remessas });

                    return _remessas;
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
        /// consultar kits de material de sinalização para o estabelecimento
        /// </summary>
        /// <param name="numeroPV"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public List<Kit> ConsultarKitsSinalizacao(Int32 numeroPV)
        {
            using (Logger Log = Logger.IniciarLog("Consultar kits de material de sinalização para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroPV });
                    List<Modelo.Kit> remessas = negocioMaterialVenda.ConsultarKitsSinalizacao(numeroPV);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { remessas });

                    List<Servicos.Kit> _remessas = new List<Servicos.Kit>();

                    if (!object.ReferenceEquals(remessas, null) && remessas.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        remessas.ForEach(delegate(Modelo.Kit r)
                        {
                            _remessas.Add(Mapper.Map<Servicos.Kit>(r));
                        });
                    }
                    Log.GravarLog(EventoLog.FimServico, new { _remessas });
                    return _remessas;
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
        /// consultar kits de material de tecnologia para o estabelecimento
        /// </summary>
        /// <param name="numeroPV"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public List<Kit> ConsultarKitsTecnologia(Int32 numeroPV)
        {
            using (Logger Log = Logger.IniciarLog("Consultar kits de material de tecnologia para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroPV });
                    List<Modelo.Kit> remessas = negocioMaterialVenda.ConsultarKitsTecnologia(numeroPV);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { remessas });

                    List<Servicos.Kit> _remessas = new List<Servicos.Kit>();

                    if (!object.ReferenceEquals(remessas, null) && remessas.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        remessas.ForEach(delegate(Modelo.Kit r)
                        {
                            _remessas.Add(Mapper.Map<Servicos.Kit>(r));
                        });
                    }
                    Log.GravarLog(EventoLog.FimServico, new { _remessas });

                    return _remessas;
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
        /// consultar kits de material de apoio para o estabelecimento
        /// </summary>
        /// <param name="numeroPV"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public List<Kit> ConsultarKitsApoio(Int32 numeroPV)
        {
            using (Logger Log = Logger.IniciarLog("consultar kits de material de apoio para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroPV });
                    List<Modelo.Kit> remessas = negocioMaterialVenda.ConsultarKitsApoio(numeroPV);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { remessas });

                    List<Servicos.Kit> _remessas = new List<Servicos.Kit>();

                    if (!object.ReferenceEquals(remessas, null) && remessas.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Kit, Servicos.Kit>();
                        Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                        remessas.ForEach(delegate(Modelo.Kit r)
                        {
                            _remessas.Add(Mapper.Map<Servicos.Kit>(r));
                        });
                    }
                    Log.GravarLog(EventoLog.FimServico, new { _remessas });

                    return _remessas;
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
        /// Consulta a quantidade máxima de Kits de Sinalização que podem ser solicitadas
        /// </summary>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public String ConsultarQuantidadeMaximaKitsSinalizacao(out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta a quantidade máxima de Kits de Sinalização que podem ser solicitadas"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio);
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    var result = negocioMaterialVenda.ConsultarQuantidadeMaximaKitsSinalizacao(out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    Log.GravarLog(EventoLog.FimServico, new { result });
                    return result;
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
        /// Consulta a quantidade máxima de Kits de Apoio que podem ser solicitadas
        /// </summary>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public String ConsultarQuantidadeMaximaKitsApoio(out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta a quantidade máxima de Kits de Apoio que podem ser solicitadas"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    Log.GravarLog(EventoLog.ChamadaNegocio);
                    var result = negocioMaterialVenda.ConsultarQuantidadeMaximaKitsApoio(out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    Log.GravarLog(EventoLog.FimServico, new { result });
                    return result;
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
        /// Consultar a composição de um Kit para o estabelecimento
        /// </summary>
        public List<Material> ConsultarComposicaoKit(Int32 codigoKit)
        {
            using (Logger Log = Logger.IniciarLog("Consultar a composição de um Kit para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoKit });
                    List<Modelo.Material> materiais = negocioMaterialVenda.ConsultarComposicaoKit(codigoKit);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { materiais });

                    List<Servicos.Material> _materiais = new List<Servicos.Material>();

                    if (!object.ReferenceEquals(materiais, null) && materiais.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Material, Servicos.Material>();
                        materiais.ForEach(delegate(Modelo.Material r)
                        {
                            _materiais.Add(Mapper.Map<Servicos.Material>(r));
                        });
                    }
                    Log.GravarLog(EventoLog.FimServico, new { _materiais });

                    return _materiais;
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
        /// Inclui uma nova solicitação de material de venda/tecnologia/apoio/sinalização do
        /// estabelecimento especificado.
        /// </summary>
        /// <returns>
        /// Código de retorno, caso 0 todas as solicitações foram incluídas com sucesso, caso contrário, 
        /// retorna o código de erro de inclusão.
        /// </returns>
        public Int32 IncluirKit(Servicos.Kit[] kits, Int32 codigoPV, String descricaoPV, String usuario, String solicitante, Boolean enderecoTemporario, Modelo.Endereco endereco = null)
        {
            using (Logger Log = Logger.IniciarLog("Inclui uma nova solicitação de material de venda/tecnologia/apoio/sinalização do estabelecimento especificado."))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Modelo.Endereco m_endereco = null;
                    Modelo.Kit[] modelosKits = new Modelo.Kit[kits.Length];

                    Int32 retorno = 0;

                    if (!object.ReferenceEquals(endereco, null))
                    {
                        Mapper.CreateMap<Servicos.Endereco, Modelo.Endereco>();
                        m_endereco = Mapper.Map<Modelo.Endereco>(endereco);
                    }

                    if (kits.Length > 0)
                    {
                        Mapper.CreateMap<Servicos.Kit, Modelo.Kit>();
                        Mapper.CreateMap<Servicos.Motivo, Modelo.Motivo>();
                        Int32 i = 0;

                        foreach (Servicos.Kit kit in kits)
                        {
                            modelosKits[i] = Mapper.Map<Modelo.Kit>(kit);
                            i++;
                        }

                        Negocio.MaterialVenda negocioMaterialVenda = new Negocio.MaterialVenda();
                        if (!object.ReferenceEquals(m_endereco, null))
                        {
                            Log.GravarLog(EventoLog.ChamadaNegocio, new { modelosKits, codigoPV, descricaoPV, usuario, solicitante, enderecoTemporario, m_endereco });

                            retorno = negocioMaterialVenda.IncluirKit(modelosKits, codigoPV, descricaoPV, usuario, solicitante, enderecoTemporario, m_endereco);
                        }
                        else
                        {
                            Log.GravarLog(EventoLog.ChamadaNegocio, new { modelosKits, codigoPV, descricaoPV, usuario, solicitante, enderecoTemporario });

                            retorno = negocioMaterialVenda.IncluirKit(modelosKits, codigoPV, descricaoPV, usuario, solicitante, enderecoTemporario);
                        }
                    }
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico, new { retorno });
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