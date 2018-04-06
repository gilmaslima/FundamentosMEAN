#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Alexandre Shiroma]
Empresa     : [Iteris]
Histórico   :
- [24/07/2012] – [Alexandre Shiroma] – [Criação]
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Request.Negocio;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente XD do módulo Request.<br/>
    /// Referente ao Tipo de Venda Débito.
    /// </summary>
    /// <example>
    /// <code lang="cs">
    /// try
    /// {
    ///     using (HISServicoXD_RequestClient clientXD = new HISServicoXD_RequestClient())
    ///     {
    ///         List&lt;Servicos.Comprovante&gt; comprovantes = 
    ///             clientXD.Cached_ConsultarDebitoPendente(..., ..., ...);
    ///
    ///         /* tratamento/lógica */
    ///     }
    /// }
    /// catch (FaultException&lt;XDRequestServico.GeneralFault&gt; ex)
    /// {
    ///     /* tratamento de erro do serviço XD */ 
    /// }
    /// catch(Exception ex)
    /// {
    ///     /* tratamento genérico de erros não tratados previamente */
    /// }
    /// </code>
    /// </example>
    public class HISServicoXD_Request : ServicoBase, IHISServicoXD_Request
    {
        #region [ Utilizando Cache ]

        /// <summary>
        /// Consulta solicitações de processos pendentes.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache,
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXD201CB / Programa XD201 / TranID XDS1
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.ConsultarDebitoPendente"/>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="dataInicio">Data inicial a ser considerada</param>
        /// <param name="dataFim">Data final a ser considerada</param>
        /// <param name="sistemaOrigem">Sistema de Origem (Exemplo: "IS", "IZ")</param>
        /// <param name="codigoTransacao">Transação (Exemplo: "XDS1")</param>
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Solicitações de processos pendentes.</returns>
        public List<Servicos.Comprovante> Cached_ConsultarDebitoPendente(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            DateTime dataInicio,
            DateTime dataFim,
            String sistemaOrigem,
            String codigoTransacao,
            out Int32 qtdTotalRegistrosCache,
            out Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Comprovantes Pendentes - Débito (cache)"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { IdPesquisa,
                    registroInicial, qtdRegistrosRetornar, qtdRegistrosPesquisar, codEstabelecimento,
                    dataInicio, dataFim, sistemaOrigem, codigoTransacao });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.RequestMaestro negocio = new Negocio.RequestMaestro();

                    //Variáveis utilizadas na pesquisa
                    Decimal numeroProcessoInicio = default(Decimal);
                    String tipoProcessoInicio = default(String);
                    String cicloProcessoInicio = default(String);
                    String dataEmissaoInicio = default(String);
                    Decimal numeroProcessoFim = default(Decimal);
                    String tipoProcessoFim = default(String);
                    String cicloProcessoFim = default(String);
                    String dataEmissaoFim = default(String);
                    String msgRetorno = default(String);
                    String possuiMaisRegistros = "S";
                    Int16 qtdOcorrencias = default(Int16);
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                    //Atribui retorno padrão para os dados de saída                
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;
                    
                    //Verifica se a pesquisa está no cache
                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.Comprovante>(Cache.Request, IdPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        Boolean possuiPesquisa = CacheAdmin.PossuiPesquisa<Modelo.Comprovante>(Cache.Request, IdPesquisa);
                        String indicadorPesquisa = possuiPesquisa ? "P" : "N";

                        if (parametros != null)
                        {
                            numeroProcessoInicio = (Decimal)parametros["numeroProcessoInicio"];
                            tipoProcessoInicio = (String)parametros["tipoProcessoInicio"];
                            cicloProcessoInicio = (String)parametros["cicloProcessoInicio"];
                            dataEmissaoInicio = (String)parametros["dataEmissaoInicio"];
                            numeroProcessoFim = (Decimal)parametros["numeroProcessoFim"];
                            tipoProcessoFim = (String)parametros["tipoProcessoFim"];
                            cicloProcessoFim = (String)parametros["cicloProcessoFim"];
                            dataEmissaoFim = (String)parametros["dataEmissaoFim"];
                            possuiMaisRegistros = (String)parametros["possuiMaisRegistros"];
                            qtdOcorrencias = (Int16)parametros["qtdOcorrencias"];
                        }

                        //Executa consulta no mainframe
                        List<Modelo.Comprovante> retorno = negocio.ConsultarDebitoPendente(
                            codEstabelecimento, dataInicio.ToString("yyyyMMdd").ToInt32(), dataFim.ToString("yyyyMMdd").ToInt32(),
                            sistemaOrigem, codigoTransacao, indicadorPesquisa,
                            ref numeroProcessoInicio, ref tipoProcessoInicio, ref cicloProcessoInicio, ref dataEmissaoInicio,
                            ref numeroProcessoFim, ref tipoProcessoFim, ref cicloProcessoFim, ref dataEmissaoFim,
                            ref possuiMaisRegistros, out qtdOcorrencias, out codigoRetorno, out msgRetorno);

                        //Em caso de erro ou sem dados de retorno
                        if (codigoRetorno != 0 || retorno == null || retorno.Count == 0)
                            return new List<Servicos.Comprovante>();

                        //Armazena de volta no cache                    
                        parametros = new Dictionary<String, Object>();
                        parametros["numeroProcessoInicio"] = numeroProcessoInicio;
                        parametros["tipoProcessoInicio"] = tipoProcessoInicio;
                        parametros["cicloProcessoInicio"] = cicloProcessoInicio;
                        parametros["dataEmissaoInicio"] = dataEmissaoInicio;
                        parametros["numeroProcessoFim"] = numeroProcessoFim;
                        parametros["tipoProcessoFim"] = tipoProcessoFim;
                        parametros["cicloProcessoFim"] = cicloProcessoFim;
                        parametros["dataEmissaoFim"] = dataEmissaoFim;
                        parametros["possuiMaisRegistros"] = possuiMaisRegistros;
                        parametros["qtdOcorrencias"] = qtdOcorrencias;

                        //Atualiza a lista de dados no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.Comprovante>(Cache.Request, IdPesquisa, retorno,
                            "S".Equals(possuiMaisRegistros, StringComparison.InvariantCultureIgnoreCase), parametros);
                    }

                    //Recupera os dados da consulta do cache
                    List<Modelo.Comprovante> dados = CacheAdmin.RecuperarPesquisa<Modelo.Comprovante>(Cache.Request, IdPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    Log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, dados });
                    
                    return this.PreencherModeloServico(dados).ToList();
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
        /// Consulta histórico de solicitações.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXD203CB / Programa XD203 / TranID XDS3
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.ConsultarHistoricoSolicitacoes"/>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="numeroProcesso">Número do processo</param>
        /// <param name="numeroPV">Código do Estabelecimento / Número do PV</param>
        /// <param name="dataInicio">Data inicial do período a ser considerado na consulta</param>
        /// <param name="dataFim">Data final do período a ser considerado na consulta</param>
        /// <param name="origem">Origem (Exemplo: "IS" - Portal / "IZ" - Intranet)</param>
        /// <param name="transacao">Código da transação (Exemplo: "XDS3")</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Histórico dos comprovantes.</returns>
        public List<Servicos.Comprovante> Cached_ConsultarHistoricoSolicitacoes(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Decimal numeroProcesso,
            Int32 numeroPV,
            DateTime dataInicio,
            DateTime dataFim,
            String origem,
            String transacao,
            out Int32 qtdTotalRegistrosCache,
            out Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Histórico de Comprovantes - Débito (cache)"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { IdPesquisa,
                    registroInicial, qtdRegistrosRetornar, qtdRegistrosPesquisar, numeroProcesso,
                    numeroPV, dataInicio, dataFim, origem, transacao });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.RequestMaestro negocio = new Negocio.RequestMaestro();

                    Dictionary<String, Object> parametros = null;

                    Decimal numeroProcessoInicio = default(Decimal);
                    String tipoProcessoInicio = default(String);
                    String cicloProcessoInicio = default(String);
                    Int32 numeroPVInicio = default(Int32);
                    String dataEmissaoInicio = default(String);
                    Decimal numeroProcessoFim = default(Decimal);
                    String tipoProcessoFim = default(String);
                    String cicloProcessoFim = default(String);
                    Int32 numeroPVFim = default(Int32);
                    String dataEmissaoFim = default(String);
                    String possuiMaisRegistros = "S";
                    String msgRetorno = default(String);
                    Int16 qtdOcorrencias = default(Int16);

                    //Atribui retorno padrão para os dados de saída                
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;
                                        
                    //Verifica se a pesquisa está no cache
                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.Comprovante>(Cache.Request, IdPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        String indPesquisa = CacheAdmin.PossuiPesquisa<Modelo.Comprovante>(Cache.Request, IdPesquisa) ? "P" : "N";
                        if (parametros != null)
                        {
                            numeroProcessoInicio = (Decimal)parametros["numeroProcessoInicio"];
                            tipoProcessoInicio = (String)parametros["tipoProcessoInicio"];
                            cicloProcessoInicio = (String)parametros["cicloProcessoInicio"];
                            numeroPVInicio = (Int32)parametros["numeroPVInicio"];
                            dataEmissaoInicio = (String)parametros["dataEmissaoInicio"];
                            numeroProcessoFim = (Decimal)parametros["numeroProcessoFim"];
                            tipoProcessoFim = (String)parametros["tipoProcessoFim"];
                            cicloProcessoFim = (String)parametros["cicloProcessoFim"];
                            numeroPVFim = (Int32)parametros["numeroPVFim"];
                            dataEmissaoFim = (String)parametros["dataEmissaoFim"];
                            possuiMaisRegistros = (String)parametros["possuiMaisRegistros"];
                            msgRetorno = (String)parametros["msgRetorno"];
                            qtdOcorrencias = (Int16)parametros["qtdOcorrencias"];
                        }

                        //Realiza a consulta no mainframe
                        List<Modelo.Comprovante> retorno = negocio.ConsultarHistoricoSolicitacoes(
                            numeroProcesso, numeroPV, dataInicio.ToString("yyyyMMdd").ToInt32(), dataFim.ToString("yyyyMMdd").ToInt32(),
                            origem, transacao, indPesquisa,
                            ref numeroProcessoInicio, ref tipoProcessoInicio, ref cicloProcessoInicio, ref numeroPVInicio, ref dataEmissaoInicio,
                            ref numeroProcessoFim, ref tipoProcessoFim, ref cicloProcessoFim, ref numeroPVFim, ref dataEmissaoFim,
                            ref possuiMaisRegistros, out qtdOcorrencias,
                            out codigoRetorno, out msgRetorno);

                        //Em caso de erro ou sem dados de retorno
                        if (codigoRetorno != 0 || retorno == null || retorno.Count == 0)
                            return new List<Servicos.Comprovante>();

                        //Armazena de volta no cache                    
                        parametros = new Dictionary<String, Object>();
                        parametros["numeroProcessoInicio"] = numeroProcessoInicio;
                        parametros["tipoProcessoInicio"] = tipoProcessoInicio;
                        parametros["cicloProcessoInicio"] = cicloProcessoInicio;
                        parametros["numeroPVInicio"] = numeroPVInicio;
                        parametros["dataEmissaoInicio"] = dataEmissaoInicio;
                        parametros["numeroProcessoFim"] = numeroProcessoFim;
                        parametros["tipoProcessoFim"] = tipoProcessoFim;
                        parametros["cicloProcessoFim"] = cicloProcessoFim;
                        parametros["numeroPVFim"] = numeroPVFim;
                        parametros["dataEmissaoFim"] = dataEmissaoFim;
                        parametros["possuiMaisRegistros"] = possuiMaisRegistros;
                        parametros["msgRetorno"] = msgRetorno;
                        parametros["qtdOcorrencias"] = qtdOcorrencias;

                        //Armazena no cache                    
                        CacheAdmin.AtualizarPesquisa<Modelo.Comprovante>(Cache.Request, IdPesquisa, retorno,
                            "S".Equals(possuiMaisRegistros, StringComparison.InvariantCultureIgnoreCase), parametros);
                    }

                    //Retorna os registros solicitados do cache da pesquisa                                    
                    List<Modelo.Comprovante> itens = CacheAdmin.RecuperarPesquisa<Modelo.Comprovante>(Cache.Request, 
                        IdPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    Log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, itens });

                    return this.PreencherModeloServico(itens).ToList();
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
        /// Consulta de avisos de débito.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="HISServicoXD_Request.ConsultarAvisosDebito"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXD204CB / Programa XD204 / TranID XDS4
        /// </remarks>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="dataInicio">Data inicial do período a ser considerado na consulta</param>
        /// <param name="dataFim">Data final do período a ser considerado na consulta</param>
        /// <param name="origem">Sistema de Origem ("IS" - Portal / "IZ" - Intranet)</param>
        /// <param name="transacao">Código da transação (Exemplo: "XDS4")</param>
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno da consulta</param>
        /// <returns>Avisos de débito</returns>
        public List<Servicos.AvisoDebito> Cached_ConsultarAvisosDebito(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Decimal codProcesso,
            Int32 codEstabelecimento,
            DateTime dataInicio,
            DateTime dataFim,
            String origem,
            String transacao,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Avisos de Débito - Débito (cache)"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { IdPesquisa,
                    registroInicial, qtdRegistrosRetornar, qtdRegistrosPesquisar, codProcesso, 
                    codEstabelecimento, dataInicio, dataFim, origem, transacao });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.RequestMaestro negocio = new Negocio.RequestMaestro();

                    //Dicionário auxiliar que irá armazenar os parâmetros de saída das consultas
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                    //Atribui retorno padrão para os dados de saída
                    qtdTotalRegistrosCache = 0;
                    codigoRetorno = 0;

                    //Declaração de variáveis de saída da consulta do mainframe
                    Decimal numeroProcessoInicio = default(Decimal);
                    String tipoProcessoInicio = default(String);
                    String cicloProcessoInicio = default(String);
                    Int32 numeroPVInicio = default(Int32);
                    String dtEmissaoInicio = default(String);
                    Decimal numeroProcessoFinal = default(Decimal);
                    String tipoProcessoFinal = default(String);
                    String cicloProcessoFinal = default(String);
                    Int32 numeroPVFinal = default(Int32);
                    String dtEmissaoFinal = default(String);
                    String possuiMaisRegistros = "S";
                    Int16 qtdOcorrencias = default(Int16);
                    
                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.AvisoDebito>(Cache.Request, IdPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        if (parametros != null)
                        {
                            numeroProcessoInicio = (Decimal)parametros["numeroProcessoInicio"];
                            tipoProcessoInicio = (String)parametros["tipoProcessoInicio"];
                            cicloProcessoInicio = (String)parametros["cicloProcessoInicio"];
                            numeroPVInicio = (Int32)parametros["numeroPVInicio"];
                            dtEmissaoInicio = (String)parametros["dtEmissaoInicio"];
                            numeroProcessoFinal = (Decimal)parametros["numeroProcessoFinal"];
                            tipoProcessoFinal = (String)parametros["tipoProcessoFinal"];
                            cicloProcessoFinal = (String)parametros["cicloProcessoFinal"];
                            numeroPVFinal = (Int32)parametros["numeroPVFinal"];
                            dtEmissaoFinal = (String)parametros["dtEmissaoFinal"];
                            qtdOcorrencias = (Int16)parametros["qtdOcorrencias"];
                        }

                        String indPesquisa = CacheAdmin.PossuiPesquisa<Modelo.AvisoDebito>(Cache.Request, IdPesquisa) ? "P" : "N";

                        //Executa a busca no mainframe
                        List<Modelo.AvisoDebito> avisos = negocio.ConsultarAvisosDebito(
                            codProcesso, codEstabelecimento, dataInicio, dataFim, origem, transacao, indPesquisa,
                            ref numeroProcessoInicio, ref tipoProcessoInicio, ref cicloProcessoInicio, ref numeroPVInicio, ref dtEmissaoInicio,
                            ref numeroProcessoFinal, ref tipoProcessoFinal, ref cicloProcessoFinal, ref numeroPVFinal, ref dtEmissaoFinal,
                            ref possuiMaisRegistros, out qtdOcorrencias, out codigoRetorno);

                        //Em caso de erro ou sem dados de retorno
                        if (codigoRetorno != 0 || avisos == null || avisos.Count == 0)
                            return new List<Servicos.AvisoDebito>();

                        //Armazena no cache                    
                        parametros = new Dictionary<String, Object>();
                        parametros["numeroProcessoInicio"] = numeroProcessoInicio;
                        parametros["tipoProcessoInicio"] = tipoProcessoInicio;
                        parametros["cicloProcessoInicio"] = cicloProcessoInicio;
                        parametros["numeroPVInicio"] = numeroPVInicio;
                        parametros["dtEmissaoInicio"] = dtEmissaoInicio;
                        parametros["numeroProcessoFinal"] = numeroProcessoFinal;
                        parametros["tipoProcessoFinal"] = tipoProcessoFinal;
                        parametros["cicloProcessoFinal"] = cicloProcessoFinal;
                        parametros["numeroPVFinal"] = numeroPVFinal;
                        parametros["dtEmissaoFinal"] = dtEmissaoFinal;
                        parametros["qtdOcorrencias"] = qtdOcorrencias;

                        //Atualiza os dados da pesquisa no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.AvisoDebito>(Cache.Request, IdPesquisa, avisos,
                            "S".Equals(possuiMaisRegistros, StringComparison.InvariantCultureIgnoreCase), parametros);
                    }

                    //Recupera os dados do cache
                    List<Modelo.AvisoDebito> retorno = CacheAdmin.RecuperarPesquisa<Modelo.AvisoDebito>(Cache.Request, 
                        IdPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    Log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, retorno });

                    //Retorna os objetos mapeados para modelos de serviço
                    return this.PreencherModeloServico(retorno).ToList();
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
        /// Consulta log de recebimento dos documentos.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXD202CB / Programa XD202 / TranID XDS2
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.ConsultarLogRecDoc"/>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="codEstabelecimento">Código do estabelecimento / PV</param>
        /// <param name="origem">Sistema de Origem ("IS" - Portal / "IZ" - Intranet)</param>
        /// <param name="transacao">Código da transação (Exemplo: "XDS2")</param>
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno da consulta</param>
        /// <returns>Log do recebimento de CV</returns>
        public List<Servicos.RecebimentoCV> Cached_ConsultarLogRecDoc(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Decimal codProcesso,
            Int32 codEstabelecimento,
            String origem,
            String transacao,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Log Recebimento de Documentos - Débito (cache)"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { IdPesquisa,
                    registroInicial, qtdRegistrosRetornar, qtdRegistrosPesquisar, codProcesso,
                    codEstabelecimento, origem, transacao});

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.RequestMaestro negocio = new Negocio.RequestMaestro();

                    //Atibuição de retorno padrão dos dados de saída
                    qtdTotalRegistrosCache = 0;
                    codigoRetorno = 0;

                    //Variáveis auxiliares
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    String msgRetorno = default(String);
                    Int16 qtdOcorrencias = default(Int16);
                    Int16 codRetorno = default(Int16);

                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.RecebimentoCV>(Cache.Request, IdPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        //Consulta realizada no mainframe
                        List<Modelo.RecebimentoCV> itens = negocio.ConsultarLogRecDoc(
                            codProcesso,
                            codEstabelecimento,
                            origem,
                            transacao,
                            out codRetorno,
                            out msgRetorno,
                            out qtdOcorrencias);

                        //Em caso de erro ou sem dados de retorno
                        if (codRetorno != 0 || itens == null || itens.Count == 0)
                            return new List<Servicos.RecebimentoCV>();

                        Boolean possuiMaisRegistros = qtdOcorrencias >= 500;

                        //Atualiza os dados da pesquisa no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.RecebimentoCV>(Cache.Request, 
                            IdPesquisa, itens, possuiMaisRegistros, parametros);
                    }

                    //Recupera os dados do cache
                    List<Modelo.RecebimentoCV> retorno = CacheAdmin.RecuperarPesquisa<Modelo.RecebimentoCV>(Cache.Request,
                        IdPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    Log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, retorno });

                    //Retorna os objetos mapeados para modelos de serviço
                    return this.PreencherModeloServico(retorno).ToList();
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

        #endregion

        /// <summary>
        /// Consulta do motivo do débito
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXD205CB / Programa XD205 / TranID XDS5
        /// </remarks>
        /// <param name="codMotivoDebito">Código do motivo do débito</param>
        /// <param name="origem">Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XDS5")</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Descrição do motivo do débito</returns>
        public String ConsultarMotivoDebito(
            Int16 codMotivoDebito,
            String origem,
            String transacao,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Motivo de Débito - Débito"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codMotivoDebito, origem, transacao });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.RequestMaestro negocioRequest = new Negocio.RequestMaestro();

                    //Executa consulta no mainframe
                    String motivoDebito = negocioRequest.ConsultarMotivoDebito(
                        codMotivoDebito,
                        origem,
                        transacao,
                        out codigoRetorno);

                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, motivoDebito });

                    return motivoDebito;
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
        /// Consulta log de recebimento dos documentos.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXD202CB / Programa XD202 / TranID XDS2
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.Cached_ConsultarLogRecDoc"/>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="codEstabelecimento">Código do estabelecimento / PV</param>
        /// <param name="origem">Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XDS2")</param>
        /// <param name="qtdOcorrencias">Quantidade de registros na área de retorno</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Log do recebimento de CV</returns>
        public List<Servicos.RecebimentoCV> ConsultarLogRecDoc(
            Decimal codProcesso,
            Int32 codEstabelecimento,
            String origem,
            String transacao,
            ref Int16 qtdOcorrencias,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Log Recebimento de Documentos - Débito"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codProcesso, codEstabelecimento, origem, transacao, qtdOcorrencias });

                try
                {
                    //Declaração de variáveis auxiliares
                    Int16 codRetorno;
                    String dscRetorno;

                    //Instanciação da classe de negócio
                    Negocio.RequestMaestro negocioRequest = new Negocio.RequestMaestro();

                    //Consulta realizada no mainframe
                    List<Modelo.RecebimentoCV> modeloRecebimentoCVs = negocioRequest.ConsultarLogRecDoc(
                        codProcesso,
                        codEstabelecimento,
                        origem,
                        transacao,
                        out codRetorno,
                        out dscRetorno,
                        out qtdOcorrencias);

                    //Preparação dos objetos de retorno
                    codigoRetorno = codRetorno;

                    Log.GravarLog(EventoLog.FimServico, new { qtdOcorrencias, codigoRetorno, modeloRecebimentoCVs });

                    return this.PreencherModeloServico(modeloRecebimentoCVs).ToList();
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
        /// Consulta histórico de solicitações.
        /// </summary>
        /// <seealso cref="HISServicoXD_Request.Cached_ConsultarHistoricoSolicitacoes"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXD203CB / Programa XD203 / TranID XDS3
        /// </remarks>
        /// <param name="numeroProcesso">Número do processo</param>
        /// <param name="numeroPV">Código do Estabelecimento / Número do PV</param>
        /// <param name="dataInicio">Data inicial do período a ser considerado na consulta</param>
        /// <param name="dataFim">Data final do período a ser considerado na consulta</param>
        /// <param name="origem">Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XDS3")</param>
        /// <param name="indicadorPesquisa">
        /// Indicador de pesquisa
        ///     ' ' ou 'N'    = Primeira Chamada
        ///     'P'           = Próxima Página
        ///     'I'           = Pesquisa Intervalo      
        /// </param>
        /// <param name="processosInicioFim">Marcadores dos processos retornados (início e fim), para execução de consultas complementares</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>
        /// <param name="qtdOcorrencias">Quantidade de Ocorrências</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Histórico de comprovantes</returns>
        public List<Servicos.Comprovante> ConsultarHistoricoSolicitacoes(
            Decimal numeroProcesso,
            Int32 numeroPV,
            DateTime dataInicio,
            DateTime dataFim,
            String origem,
            String transacao,
            String indicadorPesquisa,
            ref Servicos.ProcessosInicioFim processosInicioFim,
            ref Boolean possuiMaisRegistros,
            out Int16 qtdOcorrencias,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Histórico de Solicitações - Débito"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroProcesso,
                    numeroPV, dataInicio, dataFim, origem, transacao,
                    indicadorPesquisa, processosInicioFim, possuiMaisRegistros });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.RequestMaestro negocioRequest = new Negocio.RequestMaestro();

                    //Declaração de variáveis auxiliares
                    Int16 codRetorno;
                    String dscRetorno;
                    String indCont = possuiMaisRegistros ? "S" : "N";

                    //Executa consulta no mainframe do histórico das solicitações
                    List<Modelo.Comprovante> requestsModelo = negocioRequest.ConsultarHistoricoSolicitacoes(
                        numeroProcesso,
                        numeroPV,
                        dataInicio.ToString("yyyyMMdd").ToInt32(),
                        dataFim.ToString("yyyyMMdd").ToInt32(),
                        origem,
                        transacao,
                        indicadorPesquisa,
                        ref processosInicioFim.NumeroProcessoInicio,
                        ref processosInicioFim.TipoProcessoInicio,
                        ref processosInicioFim.CicloProcessoInicio,
                        ref processosInicioFim.NumeroPVInicio,
                        ref processosInicioFim.DataEmissaoInicio,
                        ref processosInicioFim.NumeroProcessoFim,
                        ref processosInicioFim.TipoProcessoFim,
                        ref processosInicioFim.CicloProcessoFim,
                        ref processosInicioFim.NumeroPVFim,
                        ref processosInicioFim.DataEmissaoFim,
                        ref indCont,
                        out qtdOcorrencias,
                        out codRetorno,
                        out dscRetorno);

                    //Preparação dos objetos de retorno
                    codigoRetorno = codRetorno;
                    possuiMaisRegistros = "S".Equals(indCont, StringComparison.InvariantCultureIgnoreCase);

                    Log.GravarLog(EventoLog.FimServico, new { processosInicioFim,
                        possuiMaisRegistros, qtdOcorrencias, codigoRetorno, requestsModelo });

                    return this.PreencherModeloServico(requestsModelo).ToList();
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
        /// Consulta solicitações de processos pendentes.
        /// </summary>
        /// <seealso cref="HISServicoXD_Request.Cached_ConsultarDebitoPendente"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXD201CB / Programa XD201 / TranID XDS1
        /// </remarks>
        /// <param name="codEstabelecimento">Código do estabelecimento / PV</param>
        /// <param name="dataInicio">Data inicial a ser considerada</param>
        /// <param name="dataFim">Data final a ser considerada</param>
        /// <param name="sistemaOrigem">Sistema de Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XDS1")</param>
        /// <param name="indicadorPesquisa">
        /// Indicador de pesquisa:
        ///     'N' ou ' ' = Primeira chamada
        ///     'P'        = Próxima chamada
        ///     'I'        = Pesquisa intervalo</param>
        /// <param name="processoInicioFim">Marcadores dos processos retornados (início e fim), para execução de consultas complementares</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>        
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Solicitações de processos pendentes</returns>
        public List<Servicos.Comprovante> ConsultarDebitoPendente(
           Int32 codEstabelecimento,
           DateTime dataInicio,
           DateTime dataFim,
           String sistemaOrigem,
           String transacao,
           String indicadorPesquisa,
           ref Servicos.ProcessosInicioFim processoInicioFim,
           ref Boolean possuiMaisRegistros,
           ref Int16 qtdOcorrencias,
           out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Comprovantes Pendentes - Débito"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codEstabelecimento,
                    dataInicio, dataFim, sistemaOrigem, transacao,
                    indicadorPesquisa, processoInicioFim, possuiMaisRegistros, qtdOcorrencias });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.RequestMaestro negocioRequest = new Negocio.RequestMaestro();

                    //Declaração de variáveis auxiliares
                    String dscRetorno;
                    Int16 codRetorno;
                    String strPossuiMaisRegistros = possuiMaisRegistros ? "S" : "N";

                    //Executa consulta de comprovantes pendentes
                    List<Modelo.Comprovante> requestsModelo = negocioRequest.ConsultarDebitoPendente(
                        codEstabelecimento,
                        dataInicio.ToString("yyyyMMdd").ToInt32(),
                        dataFim.ToString("yyyyMMdd").ToInt32(),
                        sistemaOrigem,
                        transacao,
                        indicadorPesquisa,
                        ref processoInicioFim.NumeroProcessoInicio,
                        ref processoInicioFim.TipoProcessoInicio,
                        ref processoInicioFim.CicloProcessoInicio,
                        ref processoInicioFim.DataEmissaoInicio,
                        ref processoInicioFim.NumeroProcessoFim,
                        ref processoInicioFim.TipoProcessoFim,
                        ref processoInicioFim.CicloProcessoFim,
                        ref processoInicioFim.DataEmissaoFim,
                        ref strPossuiMaisRegistros,
                        out qtdOcorrencias,
                        out codRetorno,
                        out dscRetorno);

                    //Preparação de objetos retorno
                    codigoRetorno = codRetorno;
                    possuiMaisRegistros = "S".Equals(strPossuiMaisRegistros, StringComparison.InvariantCultureIgnoreCase);

                    Log.GravarLog(EventoLog.FimServico, new { processoInicioFim,
                        possuiMaisRegistros, qtdOcorrencias, codigoRetorno, requestsModelo });

                    return this.PreencherModeloServico(requestsModelo).ToList();
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
        /// Consulta de avisos de débito.
        /// </summary>
        /// <seealso cref="HISServicoXD_Request.Cached_ConsultarAvisosDebito"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXD204CB / Programa XD204 / TranID XDS4
        /// </remarks>
        /// <param name="codProcesso">Código do Processo</param>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="dataIni">Data inicial do período a ser considerado na consulta</param>
        /// <param name="dataFim">Data final do período a ser considerado na consulta</param>
        /// <param name="origem">Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XDS4")</param>
        /// <param name="indicadorPesquisa">
        /// Indicador de pesquisa:
        ///     'N' ou ' ' = Primeira chamada
        ///     'P'        = Próxima chamada
        ///     'I'        = Pesquisa intervalo
        /// </param>
        /// <param name="processosInicioFim">Marcadores dos processos retornados (início e fim), para execução de consultas complementares</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Avisos de débito</returns>
        public List<Servicos.AvisoDebito> ConsultarAvisosDebito(
           Decimal codProcesso,
           Int32 codEstabelecimento,
           DateTime dataIni,
           DateTime dataFim,
           String origem,
           String transacao,
           String indicadorPesquisa,
           ref Servicos.ProcessosInicioFim processosInicioFim,
           ref Boolean possuiMaisRegistros,
           out Int16 qtdOcorrencias,
           out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Avisos de Débito - Débito"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codProcesso,
                    codEstabelecimento, dataIni, dataFim, origem,
                    transacao, indicadorPesquisa, processosInicioFim, possuiMaisRegistros });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.RequestMaestro negocioRequest = new Negocio.RequestMaestro();

                    //Declaração de variáveis auxiliares                
                    String indCont = possuiMaisRegistros ? "S" : "N";

                    //Executa consulta dos avisos de débito no mainframe
                    List<Modelo.AvisoDebito> requestsModelo = negocioRequest.ConsultarAvisosDebito(
                        codProcesso,
                        codEstabelecimento,
                        dataIni,
                        dataFim,
                        origem,
                        transacao,
                        indicadorPesquisa,
                        ref processosInicioFim.NumeroProcessoInicio,
                        ref processosInicioFim.TipoProcessoInicio,
                        ref processosInicioFim.CicloProcessoInicio,
                        ref processosInicioFim.NumeroPVInicio,
                        ref processosInicioFim.DataEmissaoInicio,
                        ref processosInicioFim.NumeroProcessoFim,
                        ref processosInicioFim.TipoProcessoFim,
                        ref processosInicioFim.CicloProcessoFim,
                        ref processosInicioFim.NumeroPVFim,
                        ref processosInicioFim.DataEmissaoFim,
                        ref indCont,
                        out qtdOcorrencias,
                        out codigoRetorno);

                    //Preparação dos objetos de retorno                
                    possuiMaisRegistros = "S".Equals(indCont, StringComparison.InvariantCultureIgnoreCase);

                    Log.GravarLog(EventoLog.FimServico, new { processosInicioFim,
                        possuiMaisRegistros, qtdOcorrencias, codigoRetorno });

                    return this.PreencherModeloServico(requestsModelo).ToList();
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

        #region [ Privados - Mapeamento "Modelo Negócio" -> "Modelo Serviço" ]

        private List<Servicos.RecebimentoCV> PreencherModeloServico(List<Modelo.RecebimentoCV> modelos)
        {
            List<Servicos.RecebimentoCV> retorno = new List<Servicos.RecebimentoCV>();

            if (modelos != null)
            {
                Servicos.RecebimentoCV recebimentoCV;
                foreach (Modelo.RecebimentoCV modelo in modelos)
                {
                    if (modelo == null) continue;
                    recebimentoCV = new RecebimentoCV
                    {
                        CodigoRecebimento = modelo.CodigoRecebimento,
                        DataRecebimento = modelo.DataRecebimento,
                        DescricaoRecebimento = modelo.DescricaoRecebimento
                    };
                    retorno.Add(recebimentoCV);
                }
            }

            return retorno;
        }

        private List<Servicos.Comprovante> PreencherModeloServico(List<Modelo.Comprovante> modelosRequest)
        {
            List<Servicos.Comprovante> retorno = new List<Servicos.Comprovante>();

            if (modelosRequest != null)
            {
                Servicos.Comprovante comprovante;
                foreach (Modelo.Comprovante modelo in modelosRequest)
                {
                    if (modelo == null) continue;
                    comprovante = new Comprovante();
                    if (modelo.CanalEnvio.HasValue && Enum.IsDefined(typeof(CanalEnvio), (Int32)modelo.CanalEnvio))
                        comprovante.CanalEnvio = (CanalEnvio)modelo.CanalEnvio;                    
                    comprovante.Centralizadora = modelo.Centralizadora;
                    comprovante.DataEnvio = modelo.DataEnvio;
                    comprovante.DataLimiteEnvioDocumentos = modelo.DataLimiteEnvioDocumentos;
                    comprovante.DataVenda = modelo.DataVenda;
                    comprovante.DescricaoCanalEnvio = modelo.DescricaoCanalEnvio;
                    comprovante.FlagNSUCartao = modelo.FlagNSUCartao;
                    comprovante.IndicadorDebito = modelo.IndicadorDebito;
                    comprovante.Motivo = modelo.Motivo;
                    comprovante.NumeroCartao = modelo.NumeroCartao;
                    comprovante.PontoVenda = modelo.PontoVenda;
                    comprovante.Processo = modelo.Processo;
                    comprovante.QualidadeRecebimentoDocumentos = modelo.QualidadeRecebimentoDocumentos;
                    comprovante.ResumoVenda = modelo.ResumoVenda;
                    comprovante.SolicitacaoAtendida = modelo.SolicitacaoAtendida;
                    comprovante.TipoCartao = modelo.TipoCartao;
                    comprovante.ValorVenda = modelo.ValorVenda;
                    retorno.Add(comprovante);
                };
            }
            return retorno;
        }

        private List<Servicos.AvisoDebito> PreencherModeloServico(List<Modelo.AvisoDebito> modelos)
        {
            List<Servicos.AvisoDebito> retorno = new List<Servicos.AvisoDebito>();

            if (modelos != null)
            {
                Servicos.AvisoDebito avisoDebito;
                foreach (Modelo.AvisoDebito modelo in modelos)
                {
                    if (modelo == null) continue;
                    avisoDebito = new AvisoDebito
                    {
                        Centralizadora = modelo.Centralizadora,
                        CodigoMotivoDebito = modelo.CodigoMotivoDebito,
                        DataCancelamento = modelo.DataCancelamento,
                        DataVenda = modelo.DataVenda,
                        FlagNSUCartao = modelo.FlagNSUCartao,
                        IndicadorParcela = modelo.IndicadorParcela,
                        IndicadorRequest = modelo.IndicadorRequest,
                        NumeroCartao = modelo.NumeroCartao,
                        PontoVenda = modelo.PontoVenda,
                        Processo = modelo.Processo,
                        ResumoVenda = modelo.ResumoVenda,
                        TipoCartao = modelo.TipoCartao,
                        ValorLiquidoCancelamento = modelo.ValorLiquidoCancelamento,
                        ValorVenda = modelo.ValorVenda
                    };
                    retorno.Add(avisoDebito);
                }
            }

            return retorno;
        }

        #endregion

        /// <summary>
        /// Consulta o total de requests pendentes de Débito.
        /// Utilizado na HomePage Segmentada.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do book:
        /// - Book BKXD0791 / Programa XD791 / Transação XDHS
        /// </remarks>
        /// <param name="numeroPv">Número do Estabelecimento</param>
        /// <returns>Quantidade de requests pendentes Débito</returns>
        public Int32 ConsultarTotalPendentesDebito(Int32 numeroPv)
        {
            using (var Log = Logger.IniciarLog("Consultar Total Requests Pendentes - Débito (BKXD0791/XD791/XDHS)"))
            {
                Log.GravarLog(EventoLog.InicioServico, numeroPv);

                //Variável de retorno;
                Int32 totalPendentes = default(Int32);

                try
                {
                    totalPendentes = new Negocio.RequestMaestro().ConsultarTotalPendentesDebito(numeroPv);
                }
                catch (PortalRedecardException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.RetornoServico, totalPendentes);

                return totalPendentes;
            }
        }
    }
}