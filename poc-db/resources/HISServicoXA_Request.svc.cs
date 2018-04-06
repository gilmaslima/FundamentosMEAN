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
using System.ServiceModel;
using Redecard.PN.Comum;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente XA do módulo Request.<br/>
    /// Referente aos Tipo de Venda Crédito.
    /// </summary>
    /// <example>
    /// <code lang="cs">
    /// try
    /// {
    ///     using (HISServicoXA_RequestClient clientXA = new HISServicoXA_RequestClient())
    ///     {
    ///         List&lt;Servicos.Comprovante&gt; comprovantes = 
    ///             clientXA.Cached_ConsultarRequestPendente(..., ..., ...);
    ///
    ///         /* tratamento/lógica */
    ///     }    
    /// }
    /// catch (FaultException&lt;XARequestServico.GeneralFault&gt; ex)
    /// {
    ///     /* tratamento de erro do serviço XA */ 
    /// }
    /// catch(Exception ex)
    /// {
    ///     /* tratamento genérico de erros não tratados previamente */
    /// }
    /// </code>
    /// </example>
    public class HISServicoXA_Request : ServicoBase, IHISServicoXA_Request
    {
        #region [ Cache ]
        /// <summary>
        /// Consulta de Comprovantes de Crédito Pendentes.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>        
        /// <seealso cref="HISServicoXA_Request.ConsultarRequestPendente"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA790 / Programa XA790 / TranID IS68
        /// </remarks>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Lista contendo os Comprovantes de Crédito pendentes.</returns>
        public List<Servicos.Comprovante> Cached_ConsultarRequestPendente(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno)
        {            
            using (Logger Log = Logger.IniciarLog("Comprovantes Pendentes - Crédito (cache)"))
            {
                Log.GravarLog(EventoLog.InicioServico,
                    new { IdPesquisa, registroInicial, qtdRegistrosRetornar, qtdRegistrosPesquisar, 
                        codEstabelecimento, codProcesso, programa, origem });
                
                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.Request();

                    //Variáveis utilizadas na pesquisa
                    Boolean possuiMaisRegistros = false;
                    Decimal codUltimoProcesso = default(Decimal);
                    Int16 qtdLinhasOcorrencia = default(Int16);
                    Int32 qtdTotalOcorrencias = default(Int32);
                    String filler = default(String);
                    Int64 codOcorrencia = default(Int64);
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                    //Atribuição de retorno padrão
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;

                    //Verifica se há necessidade de buscar a pesquisa no mainframe
                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.Comprovante>(Cache.Request,
                        IdPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        if (parametros != null)
                        {
                            possuiMaisRegistros = (Boolean)parametros["possuiMaisRegistros"];
                            codUltimoProcesso = (Decimal)parametros["codUltimoProcesso"];
                            qtdLinhasOcorrencia = (Int16)parametros["qtdLinhasOcorrencia"];
                            qtdTotalOcorrencias = (Int32)parametros["qtdTotalOcorrencias"];
                            filler = (String)parametros["filler"];
                            codOcorrencia = (Int64)parametros["codOcorrencia"];
                            codProcesso = codUltimoProcesso;
                        }

                        //Executa consulta no mainframe
                        List<Modelo.Comprovante> retorno = negocio.ConsultarRequestPendente(
                            codEstabelecimento, codProcesso, programa, origem,
                            ref possuiMaisRegistros, ref codUltimoProcesso, ref qtdLinhasOcorrencia, ref qtdTotalOcorrencias,
                            ref filler, ref codOcorrencia, out codigoRetorno);

                        //Em caso de erro ou sem dados de retorno
                        if (codigoRetorno != 0 || retorno == null || retorno.Count == 0)
                            return new List<Servicos.Comprovante>();

                        //Armazena parâmetros de volta no cache
                        parametros = new Dictionary<string, object>();
                        parametros["possuiMaisRegistros"] = possuiMaisRegistros;
                        parametros["codUltimoProcesso"] = codUltimoProcesso;
                        parametros["qtdLinhasOcorrencia"] = qtdLinhasOcorrencia;
                        parametros["qtdTotalOcorrencias"] = qtdTotalOcorrencias;
                        parametros["filler"] = filler;
                        parametros["codOcorrencia"] = codOcorrencia;

                        //Atualiza a lista de dados no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.Comprovante>(Cache.Request, 
                            IdPesquisa, retorno, possuiMaisRegistros, parametros);
                    }
                    //Retorna os dados do cache
                    List<Modelo.Comprovante> dados = CacheAdmin.RecuperarPesquisa<Modelo.Comprovante>(Cache.Request,
                        IdPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    Log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, dados });

                    //Converte para modelo de serviço
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
        /// Consulta do Log de Recebimento de CV.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="HISServicoXA_Request.RecebimentoCV"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA760 / Programa XA760 / TranID IS66
        /// </remarks>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Log de recebimento de CV</returns>
        public List<Servicos.RecebimentoCV> Cached_RecebimentoCV(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Recebimento Comprovante Venda - Crédito (cache)"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { 
                    IdPesquisa, registroInicial, qtdRegistrosRetornar, qtdRegistrosPesquisar,
                    codEstabelecimento, codProcesso, origem });

                try
                {                    
                    //Instanciação da classe de negócio
                    Negocio.Request negocio = new Negocio.Request();

                    //Declaração de variáveis auxiliares
                    String filler = default(String);
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    Boolean possuiMaisRegistros = true;
                    Int16 qtdOcorrencias = default(Int32);
                    Int64 codOcorrencia = 0;

                    //Atribuição de retorno padrão
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;

                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.RecebimentoCV>(Cache.Request,
                        IdPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        //Recupera os parâmetros da consulta anterior no cache
                        if (parametros != null)
                            filler = (String)parametros["filler"];

                        //Executa consulta no mainframe
                        List<Modelo.RecebimentoCV> itens = negocio.RecebimentoCV(
                            codEstabelecimento,
                            codProcesso,
                            origem,
                            ref possuiMaisRegistros,
                            ref qtdOcorrencias,
                            ref filler,
                            ref codOcorrencia,
                            out codigoRetorno);

                        //Em caso de erro
                        if (codOcorrencia != 0 || codigoRetorno != 0)
                            return new List<Servicos.RecebimentoCV>();

                        //Armazena os parâmetros de saída de volta no cache
                        parametros = new Dictionary<String, Object>();
                        parametros["filler"] = filler;

                        //Atualiza os dados no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.RecebimentoCV>(Cache.Request, 
                            IdPesquisa, itens, possuiMaisRegistros, parametros);
                    }

                    //Retorna os dados do cache
                    List<Modelo.RecebimentoCV> dados = CacheAdmin.RecuperarPesquisa<Modelo.RecebimentoCV>(Cache.Request,
                        IdPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    Log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, dados });

                    //Preparação dos objetos de retorno
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
        /// Consulta de avisos de débito pendentes.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="HISServicoXA_Request.ConsultarDebitoPendente"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA770 / Programa XA770 / TranID IS67
        /// </remarks>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Avisos de débito</returns>
        public List<Servicos.AvisoDebito> Cached_ConsultarDebitoPendente(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Avisos de Débito Pendentes - Crédito (cache)"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { 
                    IdPesquisa, registroInicial, qtdRegistrosRetornar, qtdRegistrosPesquisar,
                    codEstabelecimento, codProcesso, programa, origem });

                try
                {
                    //Instanciação da classe agente
                    Negocio.Request negocio = new Negocio.Request();

                    //Declaração de variáveis auxiliares
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    Boolean possuiMaisRegistros = true;
                    Decimal codUltimoProcesso = default(Decimal);
                    Int16 qtdOcorrencias = default(Int16);
                    String filler = default(String);
                    Int64 codigoOcorrencia = default(Int64);

                    //Atribuição de retorno padrão
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;

                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.AvisoDebito>(Cache.Request, 
                        IdPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        //Recupera os dados da consulta anterior, se existir, para a próxima consulta
                        if (parametros != null)
                        {
                            possuiMaisRegistros = (Boolean)parametros["possuiMaisRegistros"];
                            codUltimoProcesso = (Decimal)parametros["codUltimoProcesso"];
                            qtdOcorrencias = (Int16)parametros["qtdOcorrencias"];
                            filler = (String)parametros["filler"];
                            codigoOcorrencia = (Int64)parametros["codigoOcorrencia"];
                            codProcesso = codUltimoProcesso;
                        }

                        //Consulta de débitos pendentes
                        List<Modelo.AvisoDebito> itens = negocio.ConsultarDebitoPendente(
                            codEstabelecimento,
                            codProcesso,
                            programa,
                            origem,
                            ref possuiMaisRegistros,
                            ref codUltimoProcesso,
                            ref qtdOcorrencias,
                            ref filler,
                            ref codigoOcorrencia,
                            out codigoRetorno);

                        //Em caso de erro
                        if (codigoOcorrencia != 0 || codigoRetorno != 0)
                            return new List<Servicos.AvisoDebito>();

                        //Armazena os dados de saída no cache, para execução da próxima consulta
                        parametros = new Dictionary<String, Object>();
                        parametros["possuiMaisRegistros"] = possuiMaisRegistros;
                        parametros["codUltimoProcesso"] = codUltimoProcesso;
                        parametros["qtdOcorrencias"] = qtdOcorrencias;
                        parametros["filler"] = filler;
                        parametros["codigoOcorrencia"] = codigoOcorrencia;

                        //Atualiza os dados no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.AvisoDebito>(Cache.Request, 
                            IdPesquisa, itens, possuiMaisRegistros, parametros);
                    }

                    //Retorna os dados do cache
                    List<Modelo.AvisoDebito> dados = CacheAdmin.RecuperarPesquisa<Modelo.AvisoDebito>(Cache.Request,
                        IdPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    Log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, dados });

                    //Converte para modelo de serviço                
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
        /// Consulta do histórico de comprovantes de crédito.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="HISServicoXA_Request.HistoricoRequest"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA780 / Programa XA780 / TranID IS39
        /// </remarks>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="dataIni">Data de início do período a ser consultado</param>
        /// <param name="dataFim">Data final do período a ser consultado</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Histórico dos comprovantes</returns>
        public List<Servicos.Comprovante> Cached_HistoricoRequest(
            Guid IdPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            DateTime dataIni,
            DateTime dataFim,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Histórico de Comprovantes - Crédito (cache)"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {
                  IdPesquisa, registroInicial, qtdRegistrosRetornar, qtdRegistrosPesquisar, codEstabelecimento, 
                  codProcesso, programa, dataIni, dataFim, origem });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.Request negocio = new Negocio.Request();

                    //Declaração de variáveis auxiliares
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    Boolean possuiMaisRegistros = true;
                    Decimal ultimoRegistro = default(Decimal);
                    Int16 qtdOcorrencias = default(Int16);
                    String filler = default(String);
                    Int64 codigoOcorrencia = default(Int64);

                    //Atribuição de valores padrão
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;

                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.Comprovante>(Cache.Request, 
                        IdPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        if (parametros != null)
                        {
                            possuiMaisRegistros = (Boolean)parametros["possuiMaisRegistros"];
                            ultimoRegistro = (Decimal)parametros["ultimoRegistro"];
                            qtdOcorrencias = (Int16)parametros["qtdOcorrencias"];
                            filler = (String)parametros["filler"];
                            codigoOcorrencia = (Int64)parametros["codigoOcorrencia"];
                            codProcesso = ultimoRegistro;
                        }

                        //Executa consulta no mainframe
                        List<Modelo.Comprovante> requests = negocio.HistoricoRequest(
                            codEstabelecimento,
                            codProcesso,
                            programa,
                            dataIni,
                            dataFim,
                            origem,
                            ref possuiMaisRegistros,
                            ref ultimoRegistro,
                            ref qtdOcorrencias,
                            ref filler,
                            ref codigoOcorrencia,
                            out codigoRetorno);

                        //Em caso de erro
                        if (codigoOcorrencia != 0 || codigoRetorno != 0)
                            return new List<Servicos.Comprovante>();

                        //Armazena os parâmetros de saída para a próxima consulta
                        parametros = new Dictionary<String, Object>();
                        parametros["possuiMaisRegistros"] = possuiMaisRegistros;
                        parametros["ultimoRegistro"] = ultimoRegistro;
                        parametros["qtdOcorrencias"] = qtdOcorrencias;
                        parametros["filler"] = filler;
                        parametros["codigoOcorrencia"] = codigoOcorrencia;

                        //Atualiza os dados no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.Comprovante>(Cache.Request, 
                            IdPesquisa, requests, possuiMaisRegistros, parametros);
                    }

                    //Retorna os dados do cache
                    List<Modelo.Comprovante> dados = CacheAdmin.RecuperarPesquisa<Modelo.Comprovante>(Cache.Request, 
                        IdPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    Log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, dados });

                    //Converte para modelo de serviço                
                    return this.PreencherModeloServico(dados).ToList();
                }
                catch (PortalRedecardException ex)
                {
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        /// <summary>
        /// Consulta de canal de um estabelecimento.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA380 / Programa XA380 / TranID IS63
        /// </remarks>        
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="codigoOcorrencia">Código de ocorrência. Diferente de 0 indica que houve erro</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Canal do estabelecimento</returns>
        public Servicos.Canal ConsultarCanal(
            Int32 codEstabelecimento,
            String origem,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta de Canal - Como ser avisado"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codEstabelecimento, origem });

                try
                {
                    //Declaração de variáveis auxiliares
                    Int16 codCanal = default(Int16);
                    String descricaoCanal = String.Empty;

                    //Instanciação da classe de negócio e execução de consulta de canal do estabelecimento
                    Negocio.Request negocioRequest = new Negocio.Request();
                    codigoRetorno = negocioRequest.ConsultarCanal(
                        codEstabelecimento, origem, ref codCanal, ref descricaoCanal, ref codigoOcorrencia);

                    //Preparação dos objetos de retorno
                    Canal canal = new Canal();
                    canal.DescricaoCanal = descricaoCanal;
                    if (Enum.IsDefined(typeof(CanalRecebimento), (Int32)codCanal))
                        canal.CanalRecebimento = (CanalRecebimento)codCanal;

                    Log.GravarLog(EventoLog.FimServico, new { canal, codigoRetorno, codigoOcorrencia });

                    return canal;
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
        /// Retorna a composição do RV.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA740 / Programa XA740 / TranID IS69
        /// </remarks>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="filler">Filler</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="codigoOcorrencia">Código de ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Composição RV, com as parcelas e valores</returns>
        public Servicos.ComposicaoRV ComposicaoRV(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Composição do Resumo de Vendas"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codEstabelecimento, codProcesso, origem, filler, codigoOcorrencia });

                try
                {
                    //Declaração de variáveis auxiliares
                    Int16 qtdOcorrencias = default(Int16);
                    Decimal valorVenda = default(Decimal);
                    Decimal valorCancelamento = default(Decimal);
                    Int16 qtdParcelas = default(Int16);
                    Int16 qtdParcelasQuitadas = default(Int16);
                    Int16 qtdParcelasAVencer = default(Int16);
                    Decimal valorDeb = default(Decimal);

                    //Instanciação da classe de negócio e consulta da Composição do RV
                    Negocio.Request negocioRequest = new Negocio.Request();
                    List<Modelo.ParcelaRV> modelosComposicaoRV = negocioRequest.ComposicaoRV(
                        codEstabelecimento,
                        codProcesso,
                        origem,
                        ref qtdOcorrencias,
                        ref valorVenda,
                        ref valorCancelamento,
                        ref qtdParcelas,
                        ref qtdParcelasQuitadas,
                        ref qtdParcelasAVencer,
                        ref valorDeb,
                        ref filler,
                        ref codigoOcorrencia,
                        out codigoRetorno);
                    
                    //Preparação dos objetos de retorno
                    Servicos.ComposicaoRV composicaoRV = new Servicos.ComposicaoRV
                    {
                        Parcelas = this.PreencherModeloServico(modelosComposicaoRV).ToArray(),
                        QuantidadeOcorrencias = qtdOcorrencias,
                        QuantidadeParcelas = qtdParcelas,
                        QuantidadeParcelasAVencer = qtdParcelasAVencer,
                        QuantidadeParcelasQuitadas = qtdParcelasQuitadas,
                        ValorCancelamento = valorCancelamento,
                        ValorDebito = valorDeb,
                        ValorVenda = valorVenda
                    };

                    Log.GravarLog(EventoLog.FimServico, new { composicaoRV, codigoOcorrencia, codigoRetorno, filler });

                    return composicaoRV;
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
        /// Consulta do motivo do débito.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA750 / Programa XA750 / TranID IS65
        /// </remarks>
        /// <param name="codigoMotivoDebito">Código do motivo do débito</param>
        /// <param name="codigoOcorrencia">Código de ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <param name="origem">Sistema de origem</param>
        /// <returns>Descrição do motivo do débito</returns>
        public String MotivoDebito(
            Int32 codigoMotivoDebito,
            String origem,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Motivo do Débito"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoMotivoDebito, origem, codigoOcorrencia });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.Request negocioRequest = new Negocio.Request();
                    
                    //Execução da consulta
                    String motivoDebito = negocioRequest.MotivoDebito(codigoMotivoDebito, origem, ref codigoOcorrencia, out codigoRetorno);

                    Log.GravarLog(EventoLog.FimServico, new { motivoDebito, codigoOcorrencia, codigoRetorno });

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
        /// Consulta do Log de Recebimento de CV.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA760 / Programa XA760 / TranID IS66
        /// </remarks>
        /// <seealso cref="HISServicoXA_Request.Cached_RecebimentoCV"/>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="codigoOcorrencia">Código de Ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Log de recebimento de CV</returns>
        public List<Servicos.RecebimentoCV> RecebimentoCV(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            ref Boolean possuiMaisRegistros,
            ref Int16 qtdOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Recebimento Comprovante de Vendas"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { 
                    codEstabelecimento, codProcesso, origem, possuiMaisRegistros,
                    qtdOcorrencias, filler, codigoOcorrencia });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.Request negocioRequest = new Negocio.Request();

                    //Consulta
                    List<Modelo.RecebimentoCV> modeloRecebimentoCVs = negocioRequest.RecebimentoCV(
                        codEstabelecimento,
                        codProcesso,
                        origem,
                        ref possuiMaisRegistros,
                        ref qtdOcorrencias,
                        ref filler,
                        ref codigoOcorrencia,
                        out codigoRetorno);

                    //Preparação dos objetos de retorno
                    List<RecebimentoCV> recCV = this.PreencherModeloServico(modeloRecebimentoCVs).ToList();

                    Log.GravarLog(EventoLog.FimServico, new {
                        recCV, possuiMaisRegistros, qtdOcorrencias,
                        filler, codigoOcorrencia, codigoRetorno });
                    
                    return recCV;
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
        /// Consulta de avisos de débito pendentes.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA770 / Programa XA770 / TranID IS67
        /// </remarks>
        /// <seealso cref="HISServicoXA_Request.Cached_ConsultarDebitoPendente"/>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>
        /// <param name="codUltimoProcesso">Código do último processo retornado</param>
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Código de Ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Avisos de débito</returns>
        public List<Servicos.AvisoDebito> ConsultarDebitoPendente(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            String origem,
            ref Boolean possuiMaisRegistros,
            ref Decimal codUltimoProcesso,
            ref Int16 qtdOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Avisos de Débito - Crédito"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { 
                    codEstabelecimento, codProcesso, programa, origem,
                    possuiMaisRegistros, codUltimoProcesso, qtdOcorrencias, filler, codigoOcorrencia });

                try
                {
                    //Instanciação da classe agente
                    Negocio.Request negocioRequest = new Negocio.Request();

                    //Consulta de débitos pendentes
                    List<Modelo.AvisoDebito> requests = negocioRequest.ConsultarDebitoPendente(
                        codEstabelecimento,
                        codProcesso,
                        programa,
                        origem,
                        ref possuiMaisRegistros,
                        ref codUltimoProcesso,
                        ref qtdOcorrencias,
                        ref filler,
                        ref codigoOcorrencia,
                        out codigoRetorno);

                    //Preparação de objetos de retorno   
                    List<AvisoDebito> avisosDebito = this.PreencherModeloServico(requests).ToList();

                    Log.GravarLog(EventoLog.FimServico, new { 
                        avisosDebito, possuiMaisRegistros, codUltimoProcesso, qtdOcorrencias,
                        filler, codigoOcorrencia, codigoRetorno });

                    return avisosDebito;
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
        /// Consulta de Comprovantes de Crédito Pendentes.        
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA790 / Programa XA790 / TranID IS68
        /// </remarks>
        /// <seealso cref="HISServicoXA_Request.Cached_ConsultarRequestPendente"/>
        /// <param name="codEstabelecimento">Código do estabelecimento</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>                
        /// <param name="possuiMaisRegistros">Flag indicando o término de registros</param>
        /// <param name="codUltimoProcesso">Código indicando o último processo</param>
        /// <param name="qtdLinhasOcorrencia">Quantidade de linhas na ocorrência</param>
        /// <param name="qtdTotalOcorrencias">Quantidade total de ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Código de Ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Lista contendo os Comprovantes de Crédito pendentes.</returns>
        public List<Servicos.Comprovante> ConsultarRequestPendente(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            String origem,
            ref Boolean possuiMaisRegistros,
            ref Decimal codUltimoProcesso,
            ref Int16 qtdLinhasOcorrencia,
            ref Int32 qtdTotalOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Comprovantes Crédito - Pendentes"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { 
                    codEstabelecimento, codProcesso, programa, origem, possuiMaisRegistros,
                    codUltimoProcesso, qtdLinhasOcorrencia, qtdTotalOcorrencias, filler, codigoOcorrencia });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.Request negocioRequest = new Negocio.Request();

                    //Execução da consulta
                    List<Modelo.Comprovante> requests = negocioRequest.ConsultarRequestPendente(
                        codEstabelecimento,
                        codProcesso,
                        programa,
                        origem,
                        ref possuiMaisRegistros,
                        ref codUltimoProcesso,
                        ref qtdLinhasOcorrencia,
                        ref qtdTotalOcorrencias,
                        ref filler,
                        ref codigoOcorrencia,
                        out codigoRetorno);

                    //Preparação de objetos de retorno
                    List<Comprovante> comprovantes = this.PreencherModeloServico(requests).ToList();

                    Log.GravarLog(EventoLog.FimServico, new { 
                        comprovantes, possuiMaisRegistros, codUltimoProcesso, qtdLinhasOcorrencia,
                        qtdTotalOcorrencias, filler, codigoOcorrencia, codigoRetorno });

                    return comprovantes;
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
        /// Consulta do histórico de comprovantes de crédito
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA780 / Programa XA780 / TranID IS39
        /// </remarks>
        /// <seealso cref="HISServicoXA_Request.Cached_HistoricoRequest"/>
        /// <param name="codEstabelecimento">Código do Estabelecimento / PV</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="programa">Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="dataIni">Data de início do período a ser consultado</param>
        /// <param name="dataFim">Data final do período a ser consultado</param>
        /// <param name="possuiMaisRegistros">Se possui mais registros</param>
        /// <param name="ultimoRegistro">Código do último processo</param>
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Código de Ocorrência (diferente de 0 indica que houve erro)</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Histórico dos comprovantes</returns>
        public List<Servicos.Comprovante> HistoricoRequest(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String programa,
            DateTime dataIni,
            DateTime dataFim,
            String origem,
            ref Boolean possuiMaisRegistros,
            ref Decimal ultimoRegistro,
            ref Int16 qtdOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Histórico de Comprovantes - Crédito"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {
                    codEstabelecimento, codProcesso, programa, dataIni, dataFim, origem, 
                    possuiMaisRegistros, ultimoRegistro, qtdOcorrencias, filler, codigoOcorrencia });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.Request negocioRequest = new Negocio.Request();

                    //Executa consulta no mainframe
                    List<Modelo.Comprovante> requests = negocioRequest.HistoricoRequest(
                        codEstabelecimento,
                        codProcesso,
                        programa,
                        dataIni,
                        dataFim,
                        origem,
                        ref possuiMaisRegistros,
                        ref ultimoRegistro,
                        ref qtdOcorrencias,
                        ref filler,
                        ref codigoOcorrencia,
                        out codigoRetorno);

                    //Preparação dos objetos de retorno
                    List<Comprovante> historicoComprovantes = this.PreencherModeloServico(requests).ToList();

                    Log.GravarLog(EventoLog.FimServico, new { 
                        historicoComprovantes,  possuiMaisRegistros, ultimoRegistro,
                        qtdOcorrencias, filler, codigoOcorrencia, codigoRetorno });

                    return historicoComprovantes;
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
        /// Atualiza o Canal de recebimento de avisos
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BXA390 / Programa XA390 / TranID IS64
        /// </remarks>
        /// <param name="codEstabelecimento">Código do estabelecimento</param>
        /// <param name="origem">Sistema de origem</param>
        /// <param name="canalRecebimento">Canal de recebimento</param>
        /// <param name="msgRetorno">Mensagem de retorno</param>
        /// <returns>Código de retorno</returns>
        public Int32 AtualizarCanal(
            Int32 codEstabelecimento, 
            CanalRecebimento canalRecebimento,
            String origem,
            out String msgRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Como ser avisado - Atualização de canal"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codEstabelecimento, canalRecebimento, origem });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.Request negocioRequest = new Negocio.Request();

                    Int32 codRetorno = negocioRequest.AtualizarCanal(codEstabelecimento, (Int16)canalRecebimento, origem, out msgRetorno);

                    Log.GravarLog(EventoLog.FimServico, new { codRetorno, msgRetorno });

                    return codRetorno;
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

        #region [ Mapeamento "Modelo Negócio" -> "Modelo Serviço" ]

        private List<Servicos.ParcelaRV> PreencherModeloServico(List<Modelo.ParcelaRV> modelos)
        {
            List<Servicos.ParcelaRV> retorno = new List<Servicos.ParcelaRV>();

            if (modelos != null)
            {
                Servicos.ParcelaRV composicao;
                foreach (Modelo.ParcelaRV modelo in modelos)
                {
                    if (modelo == null) continue;
                    composicao = new ParcelaRV();
                    composicao.DataParcela = modelo.DataParcela;
                    composicao.NumeroParcela = modelo.NumeroParcela;
                    composicao.ValorLiquido = modelo.ValorLiquido;
                    retorno.Add(composicao);
                }
            }

            return retorno;
        }

        private List<Servicos.RecebimentoCV> PreencherModeloServico(List<Modelo.RecebimentoCV> modelos)
        {
            List<Servicos.RecebimentoCV> retorno = new List<Servicos.RecebimentoCV>();

            if (modelos != null)
            {
                Servicos.RecebimentoCV recebimento;
                foreach (Modelo.RecebimentoCV modelo in modelos)
                {
                    if (modelo == null) continue;
                    recebimento = new RecebimentoCV();
                    recebimento.CodigoRecebimento = modelo.CodigoRecebimento;
                    recebimento.DataRecebimento = modelo.DataRecebimento;
                    recebimento.DescricaoRecebimento = modelo.DescricaoRecebimento;
                    retorno.Add(recebimento);
                }
            }

            return retorno;
        }

        private List<Servicos.Comprovante> PreencherModeloServico(List<Modelo.Comprovante> modelos)
        {
            List<Servicos.Comprovante> retorno = new List<Servicos.Comprovante>();

            if (modelos != null)
            {
                Servicos.Comprovante comprovante;
                foreach (Modelo.Comprovante modelo in modelos)
                {
                    if (modelo == null) continue;
                    comprovante = new Comprovante();
                    if (Enum.IsDefined(typeof(CanalEnvio), (Int32)modelo.CanalEnvio))
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
                    comprovante.NumeroReferencia = modelo.NumeroReferencia;
                    comprovante.PontoVenda = modelo.PontoVenda;
                    comprovante.Processo = modelo.Processo;
                    comprovante.QualidadeRecebimentoDocumentos = modelo.QualidadeRecebimentoDocumentos;
                    comprovante.ResumoVenda = modelo.ResumoVenda;
                    comprovante.SolicitacaoAtendida = modelo.SolicitacaoAtendida;
                    comprovante.TipoCartao = modelo.TipoCartao;
                    comprovante.ValorVenda = modelo.ValorVenda;
                    retorno.Add(comprovante);
                }
            }

            return retorno;
        }

        private List<Servicos.AvisoDebito> PreencherModeloServico(List<Modelo.AvisoDebito> modelos)
        {
            List<Servicos.AvisoDebito> retorno = new List<Servicos.AvisoDebito>();

            if (modelos != null)
            {
                Servicos.AvisoDebito aviso;
                foreach (Modelo.AvisoDebito modelo in modelos)
                {
                    if (modelo == null) continue;
                    aviso = new AvisoDebito();
                    aviso.Centralizadora = modelo.Centralizadora;
                    aviso.CodigoMotivoDebito = modelo.CodigoMotivoDebito;
                    aviso.DataCancelamento = modelo.DataCancelamento;
                    aviso.DataVenda = modelo.DataVenda;
                    aviso.FlagNSUCartao = modelo.FlagNSUCartao;
                    aviso.IndicadorParcela = modelo.IndicadorParcela;
                    aviso.IndicadorRequest = modelo.IndicadorRequest;
                    aviso.NumeroCartao = modelo.NumeroCartao;
                    aviso.PontoVenda = modelo.PontoVenda;
                    aviso.Processo = modelo.Processo;
                    aviso.ResumoVenda = modelo.ResumoVenda;
                    aviso.TipoCartao = modelo.TipoCartao;
                    aviso.ValorLiquidoCancelamento = modelo.ValorLiquidoCancelamento;
                    aviso.ValorVenda = modelo.ValorVenda;
                    retorno.Add(aviso);
                }
            }

            return retorno;
        }

        #endregion

        /// <summary>
        /// Consulta o total de requests pendentes de Crédito.
        /// Utilizado na HomePage Segmentada.        
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do book:
        /// - Book BKXA0791 / Programa XA791 / Transação XAHS
        /// </remarks>
        /// <param name="numeroPv">Número do Estabelecimento</param>
        /// <returns>Quantidade de requests pendentes Crédito</returns>
        public Int32 ConsultarTotalPendentesCredito(Int32 numeroPv)
        {
            using (var Log = Logger.IniciarLog("Consultar Total Requests Pendentes - Crédito (BKXA0791/XA791/XAHS)"))
            {
                Log.GravarLog(EventoLog.InicioServico, numeroPv);

                //Variável de retorno;
                Int32 totalPendentes = default(Int32);
                
                try
                {                    
                    totalPendentes = new Negocio.Request().ConsultarTotalPendentesCredito(numeroPv);
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