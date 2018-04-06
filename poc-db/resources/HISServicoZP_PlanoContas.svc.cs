using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Redecard.PN.Comum;
using Redecard.PN.OutrosServicos.Servicos.PlanoContas;
using Modelo = Redecard.PN.OutrosServicos.Modelo.PlanoContas;

namespace Redecard.PN.OutrosServicos.Servicos
{    
    /// <summary>
    /// Serviço para expor os métodos de consulta mainframe do módulo Plano de Contas / Japão / Turquia
    /// </summary>
    public class HISServicoZP_PlanoContas : ServicoBase, IHISServicoZP_PlanoContas
    {
        /// <summary>
        /// Consulta de ofertas.<br/>        
        /// - Book ZPCA1790	/ Programa ZP1790 / TranID ZPP0 / Método ConsultarOfertas
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1790	/ Programa ZP1790 / TranID ZPP0 / Método ConsultarOfertas
        /// </remarks>
        /// <param name="codigoRetorno">Código de retorno do Serviço</param>
        /// <param name="dataFim">Data de término do período de vigência da pesquisa</param>
        /// <param name="dataInicio">Data de início do período de vigência da pesquisa</param>
        /// <param name="numeroPV">Código do estabelecimento</param>        
        /// <param name="tipoEstabelecimento">Tipo de estabelecimento.</param>
        /// <returns>Ofertas</returns>
        public List<Oferta> ConsultarOfertas(
            Int32 numeroPV,            
            DateTime dataInicio,
            DateTime dataFim,
            TipoEstabelecimento tipoEstabelecimento,
            out Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Plano Contas - Consulta de Ofertas"))
            {
                //Declaração de variável de retorno
                List<PlanoContas.Oferta> retornoServico = null;

                Log.GravarLog(EventoLog.InicioServico, new { numeroPV, dataInicio, dataFim, tipoEstabelecimento });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.PlanoContas negocio = new Negocio.PlanoContas();

                    //Consulta Ofertas
                    List<Modelo.PlanoContas.Oferta> retornoNegocio = negocio.ConsultarOfertas(
                        numeroPV, dataInicio, dataFim, (Int16) tipoEstabelecimento, out codigoRetorno);

                    //Mapeamento para classe modelo de serviço
                    Mapper.CreateMap<Modelo.PlanoContas.Oferta, PlanoContas.Oferta>();
                    Mapper.CreateMap<Modelo.PlanoContas.StatusOferta, PlanoContas.StatusOferta>();
                    retornoServico = Mapper.Map<List<Modelo.PlanoContas.Oferta>, List<PlanoContas.Oferta>>(retornoNegocio);
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

                Log.GravarLog(EventoLog.RetornoServico, new { retornoServico, codigoRetorno });

                return retornoServico;
            }
        }

        /// <summary>
        /// Consulta metas de ofertas.<br/>        
        /// - Book ZPCA1791	/ Programa ZP1791 / TranID ZPP1 / Método ConsultarMetasOfertas
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1791	/ Programa ZP1791 / TranID ZPP1 / Método ConsultarMetasOfertas
        /// </remarks>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="tipoEstabelecimento">Tipo de estabelecimento</param>
        /// <returns>Metas da Oferta</returns>
        public List<MetaOferta> ConsultarMetasOferta(
            Int32 codigoOferta,
            Int32 numeroPV,
            TipoEstabelecimento tipoEstabelecimento,
            Decimal codigoProposta,
            out Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Plano Contas - Consulta de Metas de Oferta"))
            {
                //Declaração de variável de retorno
                List<PlanoContas.MetaOferta> retornoServico = null;

                Log.GravarLog(EventoLog.InicioServico, 
                    new { codigoOferta, numeroPV, tipoEstabelecimento, codigoProposta });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.PlanoContas negocio = new Negocio.PlanoContas();

                    //Consulta Ofertas
                    List<Modelo.PlanoContas.MetaOferta> retornoNegocio = negocio.ConsultarMetasOferta(
                        codigoOferta, numeroPV, (Int16) tipoEstabelecimento, codigoProposta, 
                        out codigoRetorno);

                    //Mapeamento para classe modelo de serviço
                    Mapper.CreateMap<Modelo.PlanoContas.MetaOferta, PlanoContas.MetaOferta>();
                    retornoServico = Mapper.Map<List<Modelo.PlanoContas.MetaOferta>, List<PlanoContas.MetaOferta>>(retornoNegocio);
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

                Log.GravarLog(EventoLog.RetornoServico, new { retornoServico, codigoRetorno });

                return retornoServico;
            }
        }

        /// <summary>
        /// Consulta Apuração - Lista de Ano(s) de Referência(s).<br/>
        /// - Book ZPCA1792	/ Programa ZP1792 / TranID ZPP2 / Método ConsultarAnosReferencia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1792	/ Programa ZP1792 / TranID ZPP2 / Método ConsultarAnosReferencia
        /// </remarks>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>        
        /// <param name="tipoEstabelecimento">Tipo de estabelecimento</param>
        /// <returns>Anos Referência da Oferta</returns>
        public List<Int16> ConsultarAnosReferencia(
            Int32 codigoOferta,
            Int32 numeroPV,
            TipoEstabelecimento tipoEstabelecimento,
            Decimal codigoProposta,
            out Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Plano Contas - Consulta de Anos de Referência"))
            {
                //Declaração de variável de retorno
                List<Int16> retorno = null;

                Log.GravarLog(EventoLog.InicioServico, 
                    new { codigoOferta, numeroPV, tipoEstabelecimento, codigoProposta });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.PlanoContas negocio = new Negocio.PlanoContas();

                    //Consulta Anos de Referência
                    retorno = negocio.ConsultarAnosReferencia(codigoOferta, numeroPV, 
                        (Int16) tipoEstabelecimento, codigoProposta, out codigoRetorno);                    
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

                Log.GravarLog(EventoLog.RetornoServico, new { retorno, codigoRetorno });

                return retorno;
            }
        }

        /// <summary>
        /// Consulta Apuração - Faturamento Ano/Mês Referência.<br/>
        /// - Book ZPCA1793	/ Programa ZP1793 / TranID ZPP3 / Método ConsultarFaturamentoAnoReferencia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1793	/ Programa ZP1793 / TranID ZPP3 / Método ConsultarFaturamentoAnoReferencia
        /// </remarks>
        /// <param name="anoReferencia">Ano referência</param>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="tipoEstabelecimento">Tipo de estabelecimento</param>
        /// <returns>Faturamentos no Ano</returns>
        public List<Faturamento> ConsultarFaturamento(
            Int32 codigoOferta,
            Int32 numeroPV,            
            TipoEstabelecimento tipoEstabelecimento,
            Decimal codigoProposta,
            Int16 anoReferencia,
            out Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Plano Contas - Consulta Faturamento de Ano Referência"))
            {
                //Declaração de variável de retorno
                List<PlanoContas.Faturamento> retornoServico = null;

                Log.GravarLog(EventoLog.InicioServico, 
                    new { codigoOferta, numeroPV, tipoEstabelecimento, codigoProposta, anoReferencia });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.PlanoContas negocio = new Negocio.PlanoContas();

                    //Consulta faturamento
                    List<Modelo.PlanoContas.Faturamento> retornoNegocio =
                        negocio.ConsultarFaturamento(codigoOferta, numeroPV, (Int16) tipoEstabelecimento, 
                        codigoProposta, anoReferencia, out codigoRetorno);

                    //Mapeamento para classe modelo de serviço
                    Mapper.CreateMap<Modelo.PlanoContas.Faturamento, PlanoContas.Faturamento>();
                    Mapper.CreateMap<Modelo.PlanoContas.StatusElegibilidade, PlanoContas.StatusElegibilidade>();
                    retornoServico = Mapper.Map<List<Modelo.PlanoContas.Faturamento>, List<PlanoContas.Faturamento>>(retornoNegocio);
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

                Log.GravarLog(EventoLog.RetornoServico, new { retornoServico, codigoRetorno });

                return retornoServico;
            }
        }

        /// <summary>
        /// Consulta Apuração - Faturamento de uma Oferta (para todos os Anos Referência).<br/>
        /// Consulta todos os Anos Referência, e para cada ano, realiza a consulta (multi-thread) dos faturamentos do ano.<br/>
        /// - Book ZPCA1692	/ Programa ZP1692 / TranID ZPP2 / Método ConsultarAnosReferencia<br/>
        /// - Book ZPCA1693	/ Programa ZP1693 / TranID ZPP3 / Método ConsultarFaturamentoAnoReferencia        
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1692	/ Programa ZP1692 / TranID ZPP2 / Método ConsultarAnosReferencia<br/>
        /// - Book ZPCA1693	/ Programa ZP1693 / TranID ZPP3 / Método ConsultarFaturamentoAnoReferencia
        /// </remarks>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>        
        /// <param name="tipoEstabelecimento">Tipo de estabelecimento</param>
        /// <returns>Faturamentos do ano</returns>
        public List<Faturamento> ConsultarFaturamentos(
            Int32 codigoOferta, 
            Int32 numeroPV,             
            TipoEstabelecimento tipoEstabelecimento, 
            Decimal codigoProposta, 
            out Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Plano Contas - Consulta Faturamentos de uma Oferta/Proposta"))
            {
                //Declaração de variáveis de retorno
                List<Faturamento> retornoServico = null;
                
                Log.GravarLog(EventoLog.InicioServico, 
                    new { codigoOferta, numeroPV, tipoEstabelecimento, codigoProposta });
                
                try
                {
                    //Instanciação da classe de negócio
                    Negocio.PlanoContas negocio = new Negocio.PlanoContas();

                    //Consulta os anos referência para a oferta/proposta
                    List<Int16> anosReferencia =
                        negocio.ConsultarAnosReferencia(codigoOferta, numeroPV, (Int16) tipoEstabelecimento, 
                        codigoProposta, out codigoRetorno);

                    //Se consulta dos anos não foi realizada com sucesso, não complementa com faturamentos de cada ano
                    if (codigoRetorno != 0)
                        return null;

                    //Declaração de variáveis auxiliares para consulta multi-thread (coleção thread-safe)                
                    var retornoNegocio = new ConcurrentQueue<Modelo.PlanoContas.Faturamento>();
                    var excecoes = new ConcurrentQueue<Exception>();
                    var cancelTokenSource = new CancellationTokenSource();
                    var cancelToken = cancelTokenSource.Token;
                    var parallelOptions = new ParallelOptions { 
                        MaxDegreeOfParallelism = 5, 
                        CancellationToken = cancelToken
                    };

                    //Código de retorno que armazena valor caso alguma thread tenha gerado erro
                    Int16 codigoRetornoServico = default(Int16);

                    //Consulta multi-thread dos faturamentos dos anos referência
                    Parallel.ForEach(anosReferencia, parallelOptions, (anoReferencia) => 
                    {
                        try
                        {
                            Int16 codigoRetornoFaturamento = default(Int16);

                            //Se foi solicitado cancelamento (pois código retorno != 0 de outras threads), não executa consulta
                            if (cancelToken.IsCancellationRequested)
                                return;

                            //Consulta dos faturamentos do ano
                            List<Modelo.PlanoContas.Faturamento> faturamentosAno = negocio.ConsultarFaturamento(
                                codigoOferta, numeroPV, (Int16) tipoEstabelecimento, codigoProposta, anoReferencia, 
                                out codigoRetornoFaturamento);

                            //Caso consulta do faturamento não obteve sucesso, cancela demais threads
                            if (codigoRetornoFaturamento != 0)
                            {
                                codigoRetornoServico = codigoRetornoFaturamento;
                                cancelTokenSource.Cancel();
                            }

                            //Adiciona retorno na coleção thread-safe
                            faturamentosAno.ForEach(faturamentoAno => retornoNegocio.Enqueue(faturamentoAno));
                        }
                        catch (Exception ex)
                        {
                            excecoes.Enqueue(ex);
                        }
                    });

                    //Se ocorreu alguma exceção, relança primeira exceção
                    if (excecoes.Count > 0) 
                        throw excecoes.First();

                    //Se ocorreu algum erro durante consulta dos faturamentos, retorna
                    if (codigoRetornoServico != 0)
                    {
                        //Atribui código de retorno das threads para o serviço
                        codigoRetorno = codigoRetornoServico;
                        return null;
                    }

                    //Mapeamento para classe modelo de serviço
                    Mapper.CreateMap<Modelo.PlanoContas.Faturamento, PlanoContas.Faturamento>();
                    Mapper.CreateMap<Modelo.PlanoContas.StatusElegibilidade, PlanoContas.StatusElegibilidade>();
                    retornoServico = Mapper.Map<List<Modelo.PlanoContas.Faturamento>, List<PlanoContas.Faturamento>>(retornoNegocio.ToList());
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

                Log.GravarLog(EventoLog.RetornoServico, new { retornoServico, codigoRetorno });

                return retornoServico;
            }
        }
        
        /// <summary>
        /// Consulta o tipo de oferta ativa para o estabelecimento<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7030 / Programa ZPC703 / TranID ZPJA / Método ConsultarTipoOfertaAtiva
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7030 / Programa ZPC703 / TranID ZPJA / Método ConsultarTipoOfertaAtiva
        /// </remarks>
        /// <returns>Consulta tipo de oferta ativa</returns>
        public Int16 ConsultarTipoOfertaAtiva(
            Int32 numeroPv,
            out TipoOferta tipoOferta)
        {
            using (Logger log = Logger.IniciarLog("Tipo de Oferta Ativa - BKZP7030 / ZPC703 / ZPJA"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv });

                //Declaração de variável de retorno
                var retorno = default(Int16);

                try
                {                    
                    //Variáveis auxiliares
                    var tipoOfertaModelo = default(Modelo.PlanoContas.TipoOferta);

                    //Instanciação da classe de negócio
                    var negocio = new Negocio.PlanoContas();
                    
                    retorno = negocio.ConsultarTipoOfertaAtiva(numeroPv, out tipoOfertaModelo);

                    //Mapeamento entre classes Modelo de serviço e negócio
                    Mapper.CreateMap<Modelo.PlanoContas.TipoOferta, TipoOferta>();

                    tipoOferta = Mapper.Map<TipoOferta>(tipoOfertaModelo);
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

                log.GravarLog(EventoLog.RetornoServico, new { retorno, tipoOferta });

                return retorno;
            }
        }

        /// <summary>
        /// Consulta os dados da oferta no aceite<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7040 / Programa ZPC704 / TranID ZPJB / Método ConsultarDadosOfertaAceite
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7040 / Programa ZPC704 / TranID ZPJB / Método ConsultarDadosOfertaAceite
        /// </remarks>
        /// <returns>Consultar dados da oferta aceite</returns>
        public List<FaixaOfertaNoAceite> ConsultarDadosOfertaAceite(
            Int32 numeroPv,
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Dados da Oferta no Aceite - BKZP7040 / ZPC704 / ZPJB"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv });

                //Declaração de variável de retorno
                var retorno = default(List<FaixaOfertaNoAceite>);

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.PlanoContas();

                    List<Modelo.PlanoContas.FaixaOfertaNoAceite> retornoModelo =
                        negocio.ConsultarDadosOfertaAceite(numeroPv, out codigoRetorno);

                    //Mapeamento entre classes Modelo de serviço e negócio
                    Mapper.CreateMap<Modelo.PlanoContas.FaixaOfertaNoAceite, FaixaOfertaNoAceite>();
                    Mapper.CreateMap<Modelo.PlanoContas.FaixaOfertaNoAceiteEquipamento, FaixaOfertaNoAceiteEquipamento>();                    

                    retorno = Mapper.Map<List<FaixaOfertaNoAceite>>(retornoModelo);
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
        /// Consulta dados de celular e bônus.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7050 / Programa ZPC705 / TranID ZPJC / Método ConsultarDadosCelularBonus
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7050 / Programa ZPC705 / TranID ZPJC / Método ConsultarDadosCelularBonus
        /// </remarks>
        /// <returns>Consultar dados celular bônus</returns>
        public List<OfertaCelular> ConsultarDadosCelularBonus(
            Int32 numeroPv,
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Dados Celular Bônus - BKZP7050 / ZPC705 / ZPJC"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv });

                //Declaração de variável de retorno
                var retorno = default(List<OfertaCelular>);

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.PlanoContas();

                    List<Modelo.PlanoContas.OfertaCelular> retornoModelo =
                        negocio.ConsultarDadosCelularBonus(numeroPv, out codigoRetorno);

                    //Mapeamento entre classes Modelo de serviço e negócio
                    Mapper.CreateMap<Modelo.PlanoContas.OfertaCelular, OfertaCelular>();

                    retorno = Mapper.Map<List<OfertaCelular>>(retornoModelo);
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
        /// Consulta dados de apuração.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7060 / Programa ZPC706 / TranID ZPJD / Método ConsultarDadosApuracao
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7060 / Programa ZPC706 / TranID ZPJD / Método ConsultarDadosApuracao
        /// </remarks>
        /// <returns>Dados de apuração</returns>
        public List<OfertaDadosApuracao> ConsultarDadosApuracao(
            Int32 numeroPv,
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Dados Apuração - BKZP7060 / ZPC706 / ZPJD"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv });

                //Declaração de variável de retorno
                var retorno = default(List<OfertaDadosApuracao>);

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.PlanoContas();

                    List<Modelo.PlanoContas.OfertaDadosApuracao> retornoModelo =
                        negocio.ConsultarDadosApuracao(numeroPv, out codigoRetorno);

                    //Mapeamento entre classes Modelo de serviço e negócio
                    Mapper.CreateMap<Modelo.PlanoContas.OfertaDadosApuracao, OfertaDadosApuracao>();

                    retorno = Mapper.Map<List<OfertaDadosApuracao>>(retornoModelo);
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
        /// Consulta detalhes dos dados de apuração.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7070 / Programa ZPC707 / TranID ZPJE / Método ConsultarDadosApuracaoDetalhes
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7070 / Programa ZPC707 / TranID ZPJE / Método ConsultarDadosApuracaoDetalhes
        /// </remarks>
        /// <returns>Detalhes dos dados de apuração</returns>
        public List<OfertaDadosApuracaoDetalhe> ConsultarDadosApuracaoDetalhes(
            Int32 numeroPv,
            DateTime mesAnoReferencia,
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Detalhes Dados de Apuração - BKZP7070 / ZPC707 / ZPJE"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv, mesReferencia = mesAnoReferencia });

                //Declaração de variável de retorno
                var retorno = default(List<OfertaDadosApuracaoDetalhe>);

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.PlanoContas();

                    List<Modelo.PlanoContas.OfertaDadosApuracaoDetalhe> retornoModelo =
                        negocio.ConsultarDadosApuracaoDetalhes(numeroPv, mesAnoReferencia, out codigoRetorno);

                    //Mapeamento entre classes Modelo de serviço e negócio
                    Mapper.CreateMap<Modelo.PlanoContas.OfertaDadosApuracaoDetalhe, OfertaDadosApuracaoDetalhe>();

                    retorno = Mapper.Map<List<OfertaDadosApuracaoDetalhe>>(retornoModelo);
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
    }
}