using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using AutoMapper;
using Redecard.PN.Emissores.Negocio;

namespace Redecard.PN.Emissores.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "HisServicoZpEmissores" in code, svc and config file together.
    public class HisServicoZpEmissores : ServicoBase, IHisServicoZpEmissores
    {        /// <summary>
        /// Consulta Trava de Domicilio por Periodo
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        /// <param name="PV"></param>
        /// <returns></returns>
        public EntidadeConsultaTrava ConsultaPeriodo( Int16 numEmissor, Int32 codigoProduto, Int16 mes, Int16 ano, out Int16 codRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultaPeriodo"))
            {
                Modelos.EntidadeConsultaTrava entidadeConsultaTrava = null;
                try
                {
                    codRetorno = 0;
                    Log.GravarLog(EventoLog.InicioServico, new { numEmissor, codigoProduto, mes, ano });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numEmissor, codigoProduto, mes, ano });
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    //Enquanto for necessário executar pesquisa

                    entidadeConsultaTrava = new NegocioEmissores().ConsultaPeriodo(numEmissor, codigoProduto, mes, ano, out codRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { entidadeConsultaTrava });


                    Mapper.CreateMap<Modelos.EntidadeConsultaTrava, EntidadeConsultaTrava>();
                    Mapper.CreateMap<Modelos.DadosConsultaTrava, DadosConsultaTrava>();

                    EntidadeConsultaTrava retorno = Mapper.Map<Modelos.EntidadeConsultaTrava, EntidadeConsultaTrava>(entidadeConsultaTrava);

                    Log.GravarLog(EventoLog.RetornoServico, new { retorno, codRetorno });
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



        public EntidadeConsultaTrava ConsultarTotaisCobranca(Int16 funcao,
            Int32 numPv,
            decimal cnpj,
            String dataDe,
            String dataAte,
            Int16 codigoBanco,
            Int32 codigoProduto,
            Int16 anoCompetencia,
            Int16 mesCompetencia,
            decimal precoMedioReferencia,
            out Int16 codigoRetorno,
            out String mensagemRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultarTotaisCobranca"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new
                    {
                        funcao,
                        numPv,
                        cnpj,
                        dataDe,
                        dataAte,
                        codigoBanco,
                        codigoProduto,
                        anoCompetencia,
                        mesCompetencia,
                        precoMedioReferencia
                    });

                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        funcao,
                        numPv,
                        cnpj,
                        dataDe,
                        dataAte,
                        codigoBanco,
                        codigoProduto,
                        anoCompetencia,
                        mesCompetencia,
                        precoMedioReferencia
                    });

                    Modelos.EntidadeConsultaTrava entidadeConsultaTrava = new NegocioEmissores().ConsultarTotaisCobranca(funcao, numPv, cnpj, dataDe, dataAte,
                    codigoBanco, codigoProduto, anoCompetencia, mesCompetencia, precoMedioReferencia, out codigoRetorno, out mensagemRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { entidadeConsultaTrava });

                    Mapper.CreateMap<Modelos.EntidadeConsultaTrava, EntidadeConsultaTrava>();
                    Mapper.CreateMap<Modelos.DadosConsultaTrava, DadosConsultaTrava>();

                    EntidadeConsultaTrava retorno = Mapper.Map<Modelos.EntidadeConsultaTrava, EntidadeConsultaTrava>(entidadeConsultaTrava);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno, codigoRetorno });
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

        public List<InformacaoCobranca> ConsultaInformacaoCobranca(Int16 numEmissor, Int16 codigoProduto, Int16 mes, Int16 ano, out Int16 codigoErro)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultaInformacaoCobranca"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numEmissor, codigoProduto, mes, ano });

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numEmissor, codigoProduto, mes, ano });

                    List<Modelos.InformacaoCobranca> listaRetorno = new NegocioEmissores().ConsultaInformacaoCobranca(numEmissor, codigoProduto, mes, ano, out codigoErro);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaRetorno, codigoErro });

                    Mapper.CreateMap<Modelos.InformacaoCobranca, InformacaoCobranca>();

                    List<InformacaoCobranca> retorno = Mapper.Map<List<Modelos.InformacaoCobranca>, List<InformacaoCobranca>>(listaRetorno);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno, codigoErro });
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


        public List<InformacaoPVCobrada> ConsultarInformacoesPVCobranca(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            short funcao,
            int numeroPv,
            decimal cnpj,
            string datade,
            string datate,
            short codBanco,
            int codigoProduto,
            short anoCompetencia,
            short mesCompetencia,
            decimal faixaInicialFaturamento,
            decimal faixaFinalFaturamento,
            decimal fatorMultiplicador,
            out short codigoRetorno, out string mensagemRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultarInformacoesPVCobranca"))
            {
                codigoRetorno = 0;
                mensagemRetorno = string.Empty;
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new
                    {
                        guidPesquisa,
                        registroInicial,
                        quantidadeRegistros,
                        quantidadeTotalRegistros,
                        funcao,
                        numeroPv,
                        cnpj,
                        datade,
                        datate,
                        codBanco,
                        codigoProduto,
                        anoCompetencia,
                        mesCompetencia,
                        faixaInicialFaturamento,
                        faixaFinalFaturamento,
                        fatorMultiplicador
                    });
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<Modelos.InformacaoPVCobrada>(Cache.Emissores,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {

                        var lstRetornoConsulta = new NegocioEmissores().ConsultarInformacoesPVCobranca(funcao, numeroPv, cnpj,
                        datade, datate, codBanco, codigoProduto, anoCompetencia, mesCompetencia, faixaInicialFaturamento,
                        faixaFinalFaturamento, fatorMultiplicador,
                        ref rechamada, out indicadorRechamada,
                        out codigoRetorno, out mensagemRetorno);

                        if (codigoRetorno != 0)
                        {
                            quantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, mensagemRetorno, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Emissores, guidPesquisa, lstRetornoConsulta, indicadorRechamada, rechamada);

                    }

                    Mapper.CreateMap<Modelos.InformacaoPVCobrada, InformacaoPVCobrada>();

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<Modelos.InformacaoPVCobrada>(Cache.Emissores,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    List<InformacaoPVCobrada> lstRetorno = Mapper.Map<List<Modelos.InformacaoPVCobrada>, List<InformacaoPVCobrada>>(dadosCache);

                    codigoRetorno = 0;
                    mensagemRetorno = string.Empty;

                    Log.GravarLog(EventoLog.RetornoServico, new { lstRetorno, codigoRetorno, quantidadeTotalRegistros });
                    return lstRetorno;

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

        public List<InformacaoDetalhada> ConsultarInformacoesDetalhadas(Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros, short codBanco, int codigoProduto, short anoCompetencia, short mesCompetencia, decimal faixaInicialFaturamento, decimal faixaFinalFaturamento, out short codigoRetorno, out string mensagemRetorno)
        {
            codigoRetorno = 0;
            mensagemRetorno = string.Empty;
            using (Logger Log = Logger.IniciarLog("Serviço ConsultarInformacoesDetalhadas"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new
                    {
                        guidPesquisa,
                        registroInicial,
                        quantidadeRegistros,
                        quantidadeTotalRegistros,
                        codBanco,
                        codigoProduto,
                        anoCompetencia,
                        mesCompetencia,
                        faixaInicialFaturamento,
                        faixaFinalFaturamento
                    });

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<Modelos.InformacaoDetalhada>(Cache.Emissores,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {

                        var lstRetornoConsulta = new NegocioEmissores().ConsultarInformacoesDetalhadas(codBanco, codigoProduto, anoCompetencia, mesCompetencia, faixaInicialFaturamento, faixaFinalFaturamento,
                        ref rechamada, out indicadorRechamada, out codigoRetorno, out mensagemRetorno);

                        if (codigoRetorno != 0)
                        {
                            quantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, mensagemRetorno, quantidadeTotalRegistros });
                            return null;
                        }


                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Emissores, guidPesquisa, lstRetornoConsulta, indicadorRechamada, rechamada);
                    }
                    Mapper.CreateMap<Modelos.InformacaoDetalhada, InformacaoDetalhada>();

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<Modelos.InformacaoDetalhada>(Cache.Emissores,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);


                    List<InformacaoDetalhada> retorno = Mapper.Map<List<Modelos.InformacaoDetalhada>, List<InformacaoDetalhada>>(dadosCache);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno, codigoRetorno, quantidadeTotalRegistros });
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

        public List<DetalheFatura> ConsultarDetalheFatura(Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros, Int16 codBanco, Int32 codigoProduto, Int16 anoCompetencia,
            Int16 mesCompetencia, Decimal faixaInicialFaturamento, Decimal faixaFinalFaturamento, Int32 pvOriginal,
            out Int16 codigoRetorno, out String mensagemRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultarDetalheFatura"))
            {
                codigoRetorno = 0;
                mensagemRetorno = string.Empty;
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new
                    {
                        guidPesquisa,
                        registroInicial,
                        quantidadeRegistros,
                        quantidadeTotalRegistros,
                        codBanco,
                        codigoProduto,
                        anoCompetencia,
                        mesCompetencia,
                        faixaInicialFaturamento,
                        faixaFinalFaturamento,
                        pvOriginal
                    });
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<Modelos.DetalheFatura>(Cache.Emissores,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {

                        var lstRetornoConsulta = new NegocioEmissores().ConsultarDetalheFatura(codBanco,
                        codigoProduto,
                        anoCompetencia,
                        mesCompetencia,
                        faixaInicialFaturamento,
                        faixaFinalFaturamento,
                        pvOriginal,
                        ref rechamada, out indicadorRechamada,
                        out codigoRetorno, out mensagemRetorno);

                        if (codigoRetorno != 0)
                        {
                            quantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, mensagemRetorno, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Emissores, guidPesquisa, lstRetornoConsulta, indicadorRechamada, rechamada);

                    }

                    Mapper.CreateMap<Modelos.DetalheFatura, DetalheFatura>();

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<Modelos.DetalheFatura>(Cache.Emissores,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    List<DetalheFatura> lstRetorno = Mapper.Map<List<Modelos.DetalheFatura>, List<DetalheFatura>>(dadosCache);

                    codigoRetorno = 0;
                    mensagemRetorno = string.Empty;

                    Log.GravarLog(EventoLog.RetornoServico, new { lstRetorno, codigoRetorno, quantidadeTotalRegistros });
                    return lstRetorno;

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
