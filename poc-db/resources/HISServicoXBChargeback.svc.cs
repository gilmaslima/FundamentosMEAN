#region Histórico do Arquivo
/*
(c) Copyright [2015] Rede S.A.
Autor       : [Daniel Torres]
Empresa     : [Iteris]
Histórico   :
- [13/03/2015] – [Daniel Torres] – [Criação]
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
    /// Serviço para acesso ao componente XD do módulo Request.<br/>
    /// Referente ao Tipo de Venda Débito.
    /// </summary>
    /// <example>
    /// <code lang="cs">
    /// try
    /// {
    ///     using (HISServicoXB_RequestClient clientXD = new HISServicoXB_RequestClient())
    ///     {
    ///         List&lt;Servicos.Comprovante&gt; comprovantes = 
    ///             clientXB.Cached_ConsultarDebitoPendente(..., ..., ...);
    ///
    ///         /* tratamento/lógica */
    ///     }
    /// }
    /// catch (FaultException&lt;XBRequestServico.GeneralFault&gt; ex)
    /// {
    ///     /* tratamento de erro do serviço XD */ 
    /// }
    /// catch(Exception ex)
    /// {
    ///     /* tratamento genérico de erros não tratados previamente */
    /// }
    /// </code>
    /// </example>
    public class HISServicoXBChargeback : ServicoBase, IHISServicoXBChargeback
    {

        //XA Idêntico e/ou XA + XD Com parâmetro novo (XA750 + XD205)

        /// <summary>
        /// Consulta do motivo do débito
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKXB411 / Programa XBS411 / TranID XB86
        /// </remarks>
        /// <param name="codMotivoDebito">Código do motivo do débito</param>
        /// <param name="origem">Origem (Exemplo: "IS")</param>
        /// <param name="transacao">Transação (Exemplo: "XB86")</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <returns>Descrição do motivo do débito</returns>
        public String ConsultarMotivoDebito(
            Int16 codMotivoDebito,
            String origem,
            String transacao,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Consulta Motivo de Débito [BKXB411/XBS411]"))
            {
                log.GravarLog(EventoLog.InicioServico, new { codMotivoDebito, origem, transacao });

                try
                {
                    //Executa consulta no mainframe
                    String motivoDebito = new Negocio.ChargebackXBBLL().ConsultarMotivoDebito(
                        codMotivoDebito,
                        origem,
                        transacao,
                        out codigoRetorno);

                    log.GravarLog(EventoLog.FimServico, new { codigoRetorno, motivoDebito });

                    return motivoDebito;
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

        //XA + XD Juntos (XA791 + XD791)

        /// <summary>
        /// Consulta o total de requests pendentes
        /// Utilizado na HomePage Segmentada.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do book:
        /// - Book BKXB422 / Programa XBS422 / Transação XB97
        /// </remarks>
        /// <param name="numeroPv">Número do Estabelecimento</param>
        /// <param name="tipoProduto">Tipo do produto</param>
        /// <returns>Quantidade de requests pendentes</returns>
        public Int32 ConsultarTotalPendentes(Int32 numeroPv, Int16 tipoProduto)
        {
            using (var log = Logger.IniciarLog("Consultar Total Requests Pendentes (BKXB422/XBS422)"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv, tipoProduto });

                //Variável de retorno;
                Int32 totalPendentes = default(Int32);

                try
                {
                    totalPendentes = new Negocio.ChargebackXBBLL().ConsultarTotalPendentesDebito(numeroPv, tipoProduto);
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

                log.GravarLog(EventoLog.RetornoServico, totalPendentes);

                return totalPendentes;
            }
        }

        // XA380
        /// <summary>
        /// Consulta de canal de um estabelecimento.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKXB412 / Programa XBS412 / TranID XB87
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
            using (Logger log = Logger.IniciarLog("Consulta de Canal - Como ser avisado [BKXB412/XBS412]"))
            {
                log.GravarLog(EventoLog.InicioServico, new { codEstabelecimento, origem });

                try
                {
                    //Declaração de variáveis auxiliares
                    Int16 codCanal = default(Int16);
                    String descricaoCanal = String.Empty;

                    //Instanciação da classe de negócio e execução de consulta de canal do estabelecimento
                    codigoRetorno = new Negocio.ChargebackXBBLL().ConsultarCanal(
                        codEstabelecimento, origem, ref codCanal, ref descricaoCanal, ref codigoOcorrencia);

                    //Preparação dos objetos de retorno
                    Canal canal = new Canal();
                    canal.DescricaoCanal = descricaoCanal;
                    if (Enum.IsDefined(typeof(CanalRecebimento), (Int32)codCanal))
                        canal.CanalRecebimento = (CanalRecebimento)codCanal;

                    log.GravarLog(EventoLog.FimServico, new { canal, codigoRetorno, codigoOcorrencia });

                    return canal;
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

        //  XA390
        /// <summary>
        /// Atualiza o Canal de recebimento de avisos
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKXB413 / Programa XBS413 / TranID XB88
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
            using (Logger log = Logger.IniciarLog("Como ser avisado - Atualizar Canal [BKXB413/XBS413]"))
            {
                log.GravarLog(EventoLog.InicioServico, new { codEstabelecimento, canalRecebimento, origem });

                try
                {
                    Int32 codRetorno = new Negocio.ChargebackXBBLL()
                        .AtualizarCanal(codEstabelecimento, (Int16)canalRecebimento, origem, out msgRetorno);

                    log.GravarLog(EventoLog.FimServico, new { codRetorno, msgRetorno });

                    return codRetorno;
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

        //  XA740
        /// <summary>
        /// Retorna a composição do RV.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKXB414 / Programa XBS414 / TranID XB89
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
            using (Logger log = Logger.IniciarLog("Composição de Resumo de Vendas [BKXB414/XBS414]"))
            {
                log.GravarLog(EventoLog.InicioServico, new { codEstabelecimento, codProcesso, origem, filler, codigoOcorrencia });

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
                    List<Modelo.ParcelaRV> modelosComposicaoRV = new Negocio.ChargebackXBBLL().ComposicaoRV(
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

                    log.GravarLog(EventoLog.FimServico, new { composicaoRV, codigoOcorrencia, codigoRetorno, filler });

                    return composicaoRV;
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
        /// Preencher modelo do servico Composição RV
        /// </summary>
        /// <param name="modelos">Lista de objetos</param>
        /// <returns>Retorna lista de objetos no modelo</returns>
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

        // XA760
        /// <summary>
        /// Consulta do Log de Recebimento de CV.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="HISServicoXA_Request.RecebimentoCV"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKXB415 / Programa XBS415 / TranID XB90
        /// </remarks>
        /// <param name="idPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Log de recebimento de CV</returns>
        public List<Servicos.RecebimentoCV> RecebimentoCV(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Recebimento de Comprovante de Vendas [BKXB415/XBS415]"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    idPesquisa,
                    registroInicial,
                    qtdRegistrosRetornar,
                    qtdRegistrosPesquisar,
                    codEstabelecimento,
                    codProcesso,
                    origem
                });

                try
                {

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
                        idPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        //Recupera os parâmetros da consulta anterior no cache
                        if (parametros != null)
                            filler = (String)parametros["filler"];

                        //Executa consulta no mainframe
                        List<Modelo.RecebimentoCV> itens = new Negocio.ChargebackXBBLL().RecebimentoCV(
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
                            idPesquisa, itens, possuiMaisRegistros, parametros);
                    }

                    //Retorna os dados do cache
                    List<Modelo.RecebimentoCV> dados = CacheAdmin.RecuperarPesquisa<Modelo.RecebimentoCV>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, dados });

                    //Preparação dos objetos de retorno
                    return this.PreencherModeloServico(dados).ToList();
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
        /// Preencher modelo servico do recebimento cv
        /// </summary>
        /// <param name="modelos">Lista de objetos</param>
        /// <returns>Retorna lista de objetos no modelo</returns>
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

        //  XD202
        /// <summary>
        /// Consulta log de recebimento dos documentos.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKXB423 / Programa XBS423 / TranID XB98
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.ConsultarLogRecDoc"/>
        /// <param name="idPesquisa">Identificador da pesquisa</param>
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
        public List<Servicos.RecebimentoCV> ConsultarLogRecDoc(
            Guid idPesquisa,
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
            using (Logger log = Logger.IniciarLog("Consulta log de respostas aos estabelecimentos [BKXB423/XBS423]"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    idPesquisa,
                    registroInicial,
                    qtdRegistrosRetornar,
                    qtdRegistrosPesquisar,
                    codProcesso,
                    codEstabelecimento,
                    origem,
                    transacao
                });

                try
                {
                    //Atibuição de retorno padrão dos dados de saída
                    qtdTotalRegistrosCache = 0;
                    codigoRetorno = 0;

                    //Variáveis auxiliares
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    String msgRetorno = default(String);
                    Int16 qtdOcorrencias = default(Int16);
                    Int16 codRetorno = default(Int16);

                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.RecebimentoCV>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        //Consulta realizada no mainframe
                        List<Modelo.RecebimentoCV> itens = new Negocio.ChargebackXBBLL().ConsultarLogRecDoc(
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
                            idPesquisa, itens, possuiMaisRegistros, parametros);
                    }

                    //Recupera os dados do cache
                    List<Modelo.RecebimentoCV> retorno = CacheAdmin.RecuperarPesquisa<Modelo.RecebimentoCV>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, retorno });

                    //Retorna os objetos mapeados para modelos de serviço
                    return this.PreencherModeloServico(retorno).ToList();
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

        //XA Com mudanças

        //BXA770
        /// <summary>
        /// Consulta de avisos de débito pendentes.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="HISServicoXA_Request.ConsultarDebitoPendente"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKXB416 / Programa XBS416 / TranID XB91
        /// </remarks>
        /// <param name="idPesquisa">Identificador da pesquisa</param>
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
        public List<Servicos.AvisoDebito> ConsultarDebitoPendente(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            String programa,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Passar os débitos pendentes para o estabelecimento [BKXB416/XBS416]"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    idPesquisa,
                    registroInicial,
                    qtdRegistrosRetornar,
                    qtdRegistrosPesquisar,
                    codEstabelecimento,
                    programa,
                    origem
                });

                try
                {
                    //Instanciação da classe agente
                    Negocio.ChargebackXBBLL negocio = new Negocio.ChargebackXBBLL();

                    //Declaração de variáveis auxiliares
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    Boolean possuiMaisRegistros = true;
                    Int16 qtdOcorrencias = default(Int16);
                    String filler = default(String);
                    Int64 codigoOcorrencia = default(Int64);

                    //Rechamada
                    Decimal codProcesso = default(Decimal);
                    Int16 tipo = default(Int16);
                    Int16 codCiclo = default(Int16);
                    Int16 codSequencia = default(Int16);

                    Decimal codUltimoProcesso = default(Decimal);
                    Int16 tipoUltimo = default(Int16);
                    Int16 codUltimoCiclo = default(Int16);
                    Int16 codUltimaSequencia = default(Int16);

                    //Atribuição de retorno padrão
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;
                    
                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.AvisoDebito>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        //Recupera os dados da consulta anterior, se existir, para a próxima consulta
                        if (parametros != null)
                        {
                            possuiMaisRegistros = (Boolean)parametros["possuiMaisRegistros"];
                            qtdOcorrencias = (Int16)parametros["qtdOcorrencias"];
                            filler = (String)parametros["filler"];
                            codigoOcorrencia = (Int64)parametros["codigoOcorrencia"];

                            //Rechamada
                            codUltimoProcesso = (Decimal)parametros["codUltimoProcesso"];
                            tipoUltimo = (Int16)parametros["tipoUltimo"];
                            codUltimoCiclo = (Int16)parametros["codUltimoCiclo"];
                            codUltimaSequencia = (Int16)parametros["codUltimaSequencia"];

                            codProcesso = codUltimoProcesso;
                            tipo = tipoUltimo;
                            codCiclo = codUltimoCiclo;
                            codSequencia = codUltimaSequencia;
                        }

                        //Consulta de débitos pendentes
                        List<Modelo.AvisoDebito> itens = negocio.ConsultarDebitoPendente(
                            codEstabelecimento,
                            codProcesso,
                            tipo, 
                            codCiclo, 
                            codSequencia,
                            programa,
                            origem,
                            ref possuiMaisRegistros,
                            ref codUltimoProcesso,
                            ref tipoUltimo, 
                            ref codUltimoCiclo, 
                            ref codUltimaSequencia,
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
                        parametros["qtdOcorrencias"] = qtdOcorrencias;
                        parametros["filler"] = filler;
                        parametros["codigoOcorrencia"] = codigoOcorrencia;

                        //Rechamada
                        parametros["codUltimoProcesso"] = codUltimoProcesso;
                        parametros["tipoUltimo"] = tipoUltimo;
                        parametros["codUltimoCiclo"] = codUltimoCiclo;
                        parametros["codUltimaSequencia"] = codUltimaSequencia;

                        //Atualiza os dados no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.AvisoDebito>(Cache.Request,
                            idPesquisa, itens, possuiMaisRegistros, parametros);
                    }

                    //Retorna os dados do cache
                    List<Modelo.AvisoDebito> dados = CacheAdmin.RecuperarPesquisa<Modelo.AvisoDebito>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, dados });

                    //Converte para modelo de serviço                
                    return this.PreencherModeloServico(dados).ToList();
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
        /// Preencher modelo servico de consultar debito pendente
        /// </summary>
        /// <param name="modelos">Lista de objetos</param>
        /// <returns>Lista de objetos no modelo</returns>
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

        //BXA780
        /// <summary> 
        /// Histórico de comprovantes do request - BKXB417<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <seealso cref="HISServicoXA_Request.HistoricoRequest"/>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKXB417 / Programa XBS417 / TranID XB92
        /// </remarks>
        /// <param name="idPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="codProcesso">Código do Processo.</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="dataIni">Data de início do período a ser consultado</param>
        /// <param name="dataFim">Data final do período a ser consultado</param>
        /// <param name="codProcesso">Código do processo a ser consultado</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Histórico dos comprovantes</returns>
        public List<Servicos.Comprovante> HistoricoRequest(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            String programa,
            DateTime dataIni,
            DateTime dataFim,
            Decimal codProcesso,
            String origem,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Histórico de comprovantes do request [BKXB417/XBS417]"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    idPesquisa,
                    registroInicial,
                    qtdRegistrosRetornar,
                    qtdRegistrosPesquisar,
                    codEstabelecimento,
                    programa,
                    dataIni,
                    dataFim,
                    codProcesso,
                    origem
                });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.ChargebackXBBLL negocio = new Negocio.ChargebackXBBLL();

                    //Declaração de variáveis auxiliares
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    Boolean possuiMaisRegistros = true;
                    //Decimal ultimoRegistro = default(Decimal);
                    Int16 qtdOcorrencias = default(Int16);
                    String filler = default(String);
                    Int64 codigoOcorrencia = default(Int64);

                    //Rechamada
                    Int16 tipo = default(Int16);
                    Int16 codCiclo = default(Int16);
                    Int16 codSequencia = default(Int16);

                    Decimal codUltimoProcesso = default(Decimal);
                    Int16 tipoUltimo = default(Int16);
                    Int16 codUltimoCiclo = default(Int16);
                    Int16 codUltimaSequencia = default(Int16);

                    //Atribuição de valores padrão
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;

                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.Comprovante>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        if (parametros != null)
                        {
                            possuiMaisRegistros = (Boolean)parametros["possuiMaisRegistros"];
                            //ultimoRegistro = (Decimal)parametros["ultimoRegistro"];
                            qtdOcorrencias = (Int16)parametros["qtdOcorrencias"];
                            filler = (String)parametros["filler"];
                            codigoOcorrencia = (Int64)parametros["codigoOcorrencia"];
                            //codProcesso = ultimoRegistro;

                            //Rechamada
                            codUltimoProcesso = (Decimal)parametros["codUltimoProcesso"];
                            tipoUltimo = (Int16)parametros["tipoUltimo"];
                            codUltimoCiclo = (Int16)parametros["codUltimoCiclo"];
                            codUltimaSequencia = (Int16)parametros["codUltimaSequencia"];

                            codProcesso = codUltimoProcesso;
                            tipo = tipoUltimo;
                            codCiclo = codUltimoCiclo;
                            codSequencia = codUltimaSequencia;
                        }

                        //Executa consulta no mainframe
                        List<Modelo.Comprovante> requests = negocio.HistoricoRequest(
                            codEstabelecimento,
                            codProcesso,
                            tipo,
                            codCiclo,
                            codSequencia,
                            programa,
                            dataIni,
                            dataFim,
                            origem,
                            ref possuiMaisRegistros,
                            ref codUltimoProcesso,
                            ref tipoUltimo,
                            ref codUltimoCiclo,
                            ref codUltimaSequencia,
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
                        parametros["qtdOcorrencias"] = qtdOcorrencias;
                        parametros["filler"] = filler;
                        parametros["codigoOcorrencia"] = codigoOcorrencia;

                        //Armazena os parâmetros - Rechamada
                        parametros["codUltimoProcesso"] = codUltimoProcesso;
                        parametros["tipoUltimo"] = tipoUltimo;
                        parametros["codUltimoCiclo"] = codUltimoCiclo;
                        parametros["codUltimaSequencia"] = codUltimaSequencia;

                        //Atualiza os dados no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.Comprovante>(Cache.Request,
                            idPesquisa, requests, possuiMaisRegistros, parametros);
                    }

                    //Retorna os dados do cache
                    List<Modelo.Comprovante> dados = CacheAdmin.RecuperarPesquisa<Modelo.Comprovante>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, dados });

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

        //XA790
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
        /// <param name="idPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="programa">Origem / Programa a ser executado</param>
        /// <param name="origem">Sistema de origem</param>        
        /// <param name="processo">Número do processo para filtro</param>
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Lista contendo os Comprovantes de Crédito pendentes.</returns>
        public List<Servicos.Comprovante> ConsultarRequestPendente(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            String programa,
            String origem,
            Decimal? processo,
            out Int32 qtdTotalRegistrosCache,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Comprovantes Pendentes - Crédito (cache)"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    idPesquisa,
                    registroInicial,
                    qtdRegistrosRetornar,
                    qtdRegistrosPesquisar,
                    codEstabelecimento,
                    programa,
                    origem
                });

                Int32 registroInicialCache = 0;
                Int32 qtdRegistrosRetornarCache = Int16.MaxValue;
                Int32 qtdRegistrosPesquisarCache = Int16.MaxValue;

                if (!processo.HasValue || processo.Value <= 0)
                {
                    registroInicialCache = registroInicial;
                    qtdRegistrosRetornarCache = qtdRegistrosRetornar;
                    qtdRegistrosPesquisarCache = qtdRegistrosPesquisar;
                }

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.ChargebackXBBLL();

                    //Variáveis utilizadas na pesquisa
                    Boolean possuiMaisRegistros = false;
                    Int16 qtdLinhasOcorrencia = default(Int16);
                    Int32 qtdTotalOcorrencias = default(Int32);
                    String filler = default(String);
                    Int64 codOcorrencia = default(Int64);
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                    //Rechamada
                    Decimal codProcesso = default(Decimal);
                    Int16 tipo = default(Int16);
                    Int16 codCiclo = default(Int16);
                    Int16 codSequencia = default(Int16);

                    Decimal codUltimoProcesso = default(Decimal);
                    Int16 tipoUltimo = default(Int16);
                    Int16 codUltimoCiclo = default(Int16);
                    Int16 codUltimaSequencia = default(Int16);

                    //Atribuição de retorno padrão
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;

                    //Verifica se há necessidade de buscar a pesquisa no mainframe
                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.Comprovante>(Cache.Request,
                        idPesquisa, registroInicialCache, qtdRegistrosPesquisarCache, out parametros))
                    {
                        if (parametros != null)
                        {
                            possuiMaisRegistros = (Boolean)parametros["possuiMaisRegistros"];
                            qtdLinhasOcorrencia = (Int16)parametros["qtdLinhasOcorrencia"];
                            qtdTotalOcorrencias = (Int32)parametros["qtdTotalOcorrencias"];
                            filler = (String)parametros["filler"];
                            codOcorrencia = (Int64)parametros["codOcorrencia"];

                            //Rechamada
                            codUltimoProcesso = (Decimal)parametros["codUltimoProcesso"];
                            tipoUltimo = (Int16)parametros["tipoUltimo"];
                            codUltimoCiclo = (Int16)parametros["codUltimoCiclo"];
                            codUltimaSequencia = (Int16)parametros["codUltimaSequencia"];

                            codProcesso = codUltimoProcesso;
                            tipo = tipoUltimo;
                            codCiclo = codUltimoCiclo;
                            codSequencia = codUltimaSequencia;
                        }

                        //Executa consulta no mainframe
                        List<Modelo.Comprovante> retorno = negocio.ConsultarRequestPendente(
                            codEstabelecimento, codProcesso, tipo, codCiclo, codSequencia, programa, origem,
                            ref possuiMaisRegistros, ref codUltimoProcesso, ref tipoUltimo, ref codUltimoCiclo,
                            ref codUltimaSequencia, ref qtdLinhasOcorrencia, ref qtdTotalOcorrencias,
                            ref filler, ref codOcorrencia, out codigoRetorno);

                        //Em caso de erro ou sem dados de retorno
                        if (codigoRetorno != 0 || retorno == null || retorno.Count == 0)
                            return new List<Servicos.Comprovante>();

                        //Armazena parâmetros de volta no cache
                        parametros = new Dictionary<string, object>();
                        parametros["possuiMaisRegistros"] = possuiMaisRegistros;
                        parametros["qtdLinhasOcorrencia"] = qtdLinhasOcorrencia;
                        parametros["qtdTotalOcorrencias"] = qtdTotalOcorrencias;
                        parametros["filler"] = filler;
                        parametros["codOcorrencia"] = codOcorrencia;

                        //Rechamada
                        parametros["codUltimoProcesso"] = codUltimoProcesso;
                        parametros["tipoUltimo"] = tipoUltimo;
                        parametros["codUltimoCiclo"] = codUltimoCiclo;
                        parametros["codUltimaSequencia"] = codUltimaSequencia;

                        //Atualiza a lista de dados no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.Comprovante>(Cache.Request,
                            idPesquisa, retorno, possuiMaisRegistros, parametros);
                    }

                    //Retorna os dados do cache
                    List<Modelo.Comprovante> dados = CacheAdmin.RecuperarPesquisa<Modelo.Comprovante>(Cache.Request,
                        idPesquisa, registroInicialCache, qtdRegistrosRetornarCache, out qtdTotalRegistrosCache);

                    if (processo.HasValue && processo.Value > 0)
                    {
                        // aplica filtro por processo
                        // e retorna segundo o critério de páginação
                        dados = dados
                            .Where(x => Decimal.Equals(x.Processo, processo.Value))
                            .Skip(registroInicial)
                            .Take(qtdRegistrosRetornar).ToList();

                        qtdTotalRegistrosCache = dados.Count;
                    }

                    log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, dados });

                    //Converte para modelo de serviço
                    return this.PreencherModeloServico(dados).ToList();
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
        /// Preencher modelo do servico Consultar Request Pendente
        /// </summary>
        /// <param name="modelos">Lista de objetos</param>
        /// <returns>Retorna lista de objetos no modelo padrão</returns>
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
                    comprovante.CodigoMotivoProcesso = modelo.CodigoMotivoProcesso;
                    retorno.Add(comprovante);
                }
            }

            return retorno;
        }

        //XD201
        /// <summary>
        /// Consulta comprovante solicitacao pendente de processos - BKXB419.
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache,
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKXB419 / Programa XBS419 / TranID XB94
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.ConsultarDebitoPendente"/>
        /// <param name="idPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial (zero-based index)</param>
        /// <param name="qtdRegistrosRetornar">Quantidade de registros a serem retornados</param>
        /// <param name="qtdRegistrosPesquisar">Quantidade de registros que serão pesquisados</param>
        /// <param name="codEstabelecimento">Código de estabelecimento</param>
        /// <param name="dataInicio">Data inicial a ser considerada</param>
        /// <param name="dataFim">Data final a ser considerada</param>
        /// <param name="sistemaOrigem">Sistema de Origem (Exemplo: "IS", "IZ")</param>
        /// <param name="codigoTransacao">Transação (Exemplo: "XDS1")</param>
        /// <param name="processo">Número do processo para filtro</param>
        /// <param name="qtdTotalRegistrosCache">Saída: quantidade de registros em cache</param>
        /// <param name="codigoRetorno">Saída: código de retorno</param>
        /// <returns>Solicitações de processos pendentes.</returns>
        public List<Servicos.Comprovante> ConsultaSolicitacaoPendente(
            Guid idPesquisa,
            Int32 registroInicial,
            Int32 qtdRegistrosRetornar,
            Int32 qtdRegistrosPesquisar,
            Int32 codEstabelecimento,
            DateTime dataInicio,
            DateTime dataFim,
            String sistemaOrigem,
            String codigoTransacao,
            Decimal? processo,
            out Int32 qtdTotalRegistrosCache,
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Consulta comprovante solicitacao pendente de processos [BKXB419/XBS419]"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    idPesquisa,
                    registroInicial,
                    qtdRegistrosRetornar,
                    qtdRegistrosPesquisar,
                    codEstabelecimento,
                    dataInicio,
                    dataFim,
                    sistemaOrigem,
                    codigoTransacao
                });

                Int32 registroInicialCache = 0;
                Int32 qtdRegistrosRetornarCache = Int16.MaxValue;
                Int32 qtdRegistrosPesquisarCache = Int16.MaxValue;

                if (!processo.HasValue || processo.Value <= 0)
                {
                    registroInicialCache = registroInicial;
                    qtdRegistrosRetornarCache = qtdRegistrosRetornar;
                    qtdRegistrosPesquisarCache = qtdRegistrosPesquisar;
                }

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.ChargebackXBBLL negocio = new Negocio.ChargebackXBBLL();

                    //Variáveis utilizadas na pesquisa
                    Decimal numeroProcessoInicio = default(Decimal);
                    Int16 tipoProcessoInicio = default(Int16);
                    Int16 cicloProcessoInicio = default(Int16);
                    Int16 codigoSequenciaInicio = default(Int16);
                    String dataEmissaoInicio = default(String);
                    Decimal numeroProcessoFim = default(Decimal);
                    Int16 tipoProcessoFim = default(Int16);
                    Int16 cicloProcessoFim = default(Int16);
                    Int16 codigoSequenciaFinal = default(Int16);
                    String dataEmissaoFim = default(String);
                    String msgRetorno = default(String);
                    String possuiMaisRegistros = "S";
                    Int16 qtdOcorrencias = default(Int16);
                    Boolean indicadorRechamada = default(Boolean);
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                    //Atribui retorno padrão para os dados de saída                
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;

                    //Verifica se a pesquisa está no cache
                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.Comprovante>(Cache.Request,
                        idPesquisa, registroInicialCache, qtdRegistrosPesquisarCache, out parametros))
                    {
                        Boolean possuiPesquisa = CacheAdmin.PossuiPesquisa<Modelo.Comprovante>(Cache.Request, idPesquisa);
                        String indicadorPesquisa = possuiPesquisa ? "P" : "N";

                        if (parametros != null)
                        {
                            numeroProcessoInicio = (Decimal)parametros["numeroProcessoInicio"];
                            tipoProcessoInicio = (Int16)parametros["tipoProcessoInicio"];
                            cicloProcessoInicio = (Int16)parametros["cicloProcessoInicio"];
                            codigoSequenciaInicio = (Int16)parametros["codigoSequenciaInicio"];
                            dataEmissaoInicio = (String)parametros["dataEmissaoInicio"];
                            numeroProcessoFim = (Decimal)parametros["numeroProcessoFim"];
                            tipoProcessoFim = (Int16)parametros["tipoProcessoFim"];
                            cicloProcessoFim = (Int16)parametros["cicloProcessoFim"];
                            codigoSequenciaFinal = (Int16)parametros["codigoSequenciaFinal"];
                            dataEmissaoFim = (String)parametros["dataEmissaoFim"];
                            possuiMaisRegistros = (String)parametros["possuiMaisRegistros"];
                            qtdOcorrencias = (Int16)parametros["qtdOcorrencias"];
                        }

                        //Executa consulta no mainframe
                        List<Modelo.Comprovante> retorno = negocio.ConsultaSolicitacaoPendente(
                            codEstabelecimento, dataInicio.ToString("yyyyMMdd").ToInt32(), dataFim.ToString("yyyyMMdd").ToInt32(),
                            sistemaOrigem, codigoTransacao, indicadorPesquisa, ref numeroProcessoInicio, ref tipoProcessoInicio,
                            ref cicloProcessoInicio, ref codigoSequenciaInicio, ref dataEmissaoInicio, ref numeroProcessoFim,
                            ref tipoProcessoFim, ref cicloProcessoFim, ref codigoSequenciaFinal, ref dataEmissaoFim,
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
                        indicadorRechamada = String.Compare("S", possuiMaisRegistros, true) == 0;

                        //Atualiza a lista de dados no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.Comprovante>(Cache.Request,
                            idPesquisa, retorno, indicadorRechamada, parametros);
                    }

                    //Recupera os dados da consulta do cache
                    List<Modelo.Comprovante> dados = CacheAdmin.RecuperarPesquisa<Modelo.Comprovante>(Cache.Request,
                        idPesquisa, registroInicialCache, qtdRegistrosRetornarCache, out qtdTotalRegistrosCache);

                    if (processo.HasValue && processo.Value > 0)
                    {
                        // aplica filtro por processo
                        // e retorna segundo o critério de páginação
                        dados = dados
                            .Where(x => Decimal.Equals(x.Processo, processo.Value))
                            .Skip(registroInicial)
                            .Take(qtdRegistrosRetornar).ToList();

                        qtdTotalRegistrosCache = dados.Count;
                    }

                    log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, dados });

                    return this.PreencherModeloServico(dados).ToList();
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

        //XD203
        /// <summary>
        /// Consulta histórico de solicitações.<br/>
        /// A consulta é paginada e utiliza o cache de dados do AppFabric Cache, 
        /// retornando apenas o intervalo de registros solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKXB420 / Programa XBS420 / TranID XB95
        /// </remarks>
        /// <seealso cref="HISServicoXD_Request.ConsultarHistoricoSolicitacoes"/>
        /// <param name="idPesquisa">Identificador da pesquisa</param>
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
        public List<Servicos.Comprovante> ConsultarHistoricoSolicitacoes(
            Guid idPesquisa,
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
            using (Logger log = Logger.IniciarLog("Consulta histórico de solicitações [BKXB420/XBS420]"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    idPesquisa,
                    registroInicial,
                    qtdRegistrosRetornar,
                    qtdRegistrosPesquisar,
                    numeroProcesso,
                    numeroPV,
                    dataInicio,
                    dataFim,
                    origem,
                    transacao
                });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.ChargebackXBBLL negocio = new Negocio.ChargebackXBBLL();

                    Dictionary<String, Object> parametros = null;

                    Decimal numeroProcessoInicio = default(Decimal);
                    Int16 tipoProcessoInicio = default(Int16);
                    Int16 cicloProcessoInicio = default(Int16);
                    Int16 codigoSequenciaInicio = default(Int16); //Add
                    Int32 numeroPVInicio = default(Int32);
                    String dataEmissaoInicio = default(String);
                    Decimal numeroProcessoFim = default(Decimal);
                    Int16 tipoProcessoFim = default(Int16);
                    Int16 cicloProcessoFim = default(Int16);
                    Int16 codigoSequenciaFim = default(Int16); //Add
                    Int32 numeroPVFim = default(Int32);
                    String dataEmissaoFim = default(String);
                    String possuiMaisRegistros = "S";
                    String msgRetorno = default(String);
                    Int16 qtdOcorrencias = default(Int16);
                    Boolean indicadorRechamada = default(Boolean);

                    //Atribui retorno padrão para os dados de saída                
                    codigoRetorno = 0;
                    qtdTotalRegistrosCache = 0;

                    //Verifica se a pesquisa está no cache
                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.Comprovante>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        String indPesquisa = CacheAdmin.PossuiPesquisa<Modelo.Comprovante>(Cache.Request, idPesquisa) ? "P" : "N";
                        if (parametros != null)
                        {
                            numeroProcessoInicio = (Decimal)parametros["numeroProcessoInicio"];
                            tipoProcessoInicio = (Int16)parametros["tipoProcessoInicio"];
                            cicloProcessoInicio = (Int16)parametros["cicloProcessoInicio"];
                            codigoSequenciaInicio = (Int16)parametros["codigoSequenciaInicio"]; //Add
                            numeroPVInicio = (Int32)parametros["numeroPVInicio"];
                            dataEmissaoInicio = (String)parametros["dataEmissaoInicio"];
                            numeroProcessoFim = (Decimal)parametros["numeroProcessoFim"];
                            tipoProcessoFim = (Int16)parametros["tipoProcessoFim"];
                            cicloProcessoFim = (Int16)parametros["cicloProcessoFim"];
                            codigoSequenciaFim = (Int16)parametros["codigoSequenciaFim"];  //Add
                            numeroPVFim = (Int32)parametros["numeroPVFim"];
                            dataEmissaoFim = (String)parametros["dataEmissaoFim"];
                            possuiMaisRegistros = (String)parametros["possuiMaisRegistros"];
                            msgRetorno = (String)parametros["msgRetorno"];
                            qtdOcorrencias = (Int16)parametros["qtdOcorrencias"];
                        }

                        //Realiza a consulta no mainframe
                        List<Modelo.Comprovante> retorno = negocio.ConsultarHistoricoSolicitacoes(
                            numeroProcesso, numeroPV, dataInicio.ToString("yyyyMMdd").ToInt32(), dataFim.ToString("yyyyMMdd").ToInt32(),
                            origem, transacao, indPesquisa, ref numeroProcessoInicio, ref tipoProcessoInicio, ref cicloProcessoInicio,
                            ref codigoSequenciaInicio, ref numeroPVInicio, ref dataEmissaoInicio, ref numeroProcessoFim, ref tipoProcessoFim,
                            ref cicloProcessoFim, ref codigoSequenciaFim, ref numeroPVFim, ref dataEmissaoFim, ref possuiMaisRegistros,
                            out qtdOcorrencias, out codigoRetorno, out msgRetorno);

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
                        indicadorRechamada = String.Compare("S", possuiMaisRegistros, true) == 0;

                        //Armazena no cache                    
                        CacheAdmin.AtualizarPesquisa<Modelo.Comprovante>(Cache.Request, idPesquisa, retorno,
                            indicadorRechamada, parametros);
                    }

                    //Retorna os registros solicitados do cache da pesquisa                                    
                    List<Modelo.Comprovante> itens = CacheAdmin.RecuperarPesquisa<Modelo.Comprovante>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, itens });

                    return this.PreencherModeloServico(itens).ToList();
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

        //XD204
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
        /// <param name="idPesquisa">Identificador da pesquisa</param>
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
        public List<Servicos.AvisoDebito> ConsultarAvisosDebito(
            Guid idPesquisa,
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
            using (Logger log = Logger.IniciarLog("Consulta aviso de debitos [BKXB421/XBS421]"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    idPesquisa,
                    registroInicial,
                    qtdRegistrosRetornar,
                    qtdRegistrosPesquisar,
                    codProcesso,
                    codEstabelecimento,
                    dataInicio,
                    dataFim,
                    origem,
                    transacao
                });

                try
                {
                    //Instanciação da classe de negócio
                    Negocio.ChargebackXBBLL negocio = new Negocio.ChargebackXBBLL();

                    //Dicionário auxiliar que irá armazenar os parâmetros de saída das consultas
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                    //Atribui retorno padrão para os dados de saída
                    qtdTotalRegistrosCache = 0;
                    codigoRetorno = 0;

                    //Declaração de variáveis de saída da consulta do mainframe
                    Decimal numeroProcessoInicio = default(Decimal);
                    String tipoProcessoInicio = default(String);
                    Int16 cicloProcessoInicio = default(Int16);
                    Int16 codigoSequenciaInicio = default(Int16); //Add
                    Int32 numeroPVInicio = default(Int32);
                    String dtEmissaoInicio = default(String);
                    Decimal numeroProcessoFinal = default(Decimal);
                    Int16 tipoProcessoFinal = default(Int16);
                    Int32 cicloProcessoFinal = default(Int32);
                    Int16 codigoSequenciaFinal = default(Int16); //Add
                    Int32 numeroPVFinal = default(Int32);
                    String dtEmissaoFinal = default(String);
                    String possuiMaisRegistros = "S";
                    Int16 qtdOcorrencias = default(Int16);
                    Boolean indicadorRechamada = default(Boolean);

                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.AvisoDebito>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosPesquisar, out parametros))
                    {
                        if (parametros != null)
                        {
                            numeroProcessoInicio = (Decimal)parametros["numeroProcessoInicio"];
                            tipoProcessoInicio = (String)parametros["tipoProcessoInicio"];
                            cicloProcessoInicio = (Int16)parametros["cicloProcessoInicio"];
                            codigoSequenciaInicio = (Int16)parametros["codigoSequenciaInicio"]; //Add
                            numeroPVInicio = (Int32)parametros["numeroPVInicio"];
                            dtEmissaoInicio = (String)parametros["dtEmissaoInicio"];
                            numeroProcessoFinal = (Decimal)parametros["numeroProcessoFinal"];
                            tipoProcessoFinal = (Int16)parametros["tipoProcessoFinal"];
                            cicloProcessoFinal = (Int16)parametros["cicloProcessoFinal"];
                            codigoSequenciaFinal = (Int16)parametros["codigoSequenciaFinal"]; //Add
                            numeroPVFinal = (Int32)parametros["numeroPVFinal"];
                            dtEmissaoFinal = (String)parametros["dtEmissaoFinal"];
                            qtdOcorrencias = (Int16)parametros["qtdOcorrencias"];
                        }

                        String indPesquisa = CacheAdmin.PossuiPesquisa<Modelo.AvisoDebito>(Cache.Request, idPesquisa) ? "P" : "N";

                        //Executa a busca no mainframe
                        List<Modelo.AvisoDebito> avisos = negocio.ConsultarAvisosDebito(
                            codProcesso, codEstabelecimento, dataInicio, dataFim, origem, transacao, indPesquisa,
                            ref numeroProcessoInicio, ref tipoProcessoInicio, ref cicloProcessoInicio, ref codigoSequenciaInicio,
                            ref numeroPVInicio, ref dtEmissaoInicio, ref numeroProcessoFinal, ref tipoProcessoFinal, ref cicloProcessoFinal,
                            ref codigoSequenciaFinal, ref numeroPVFinal, ref dtEmissaoFinal, ref possuiMaisRegistros, out qtdOcorrencias,
                            out codigoRetorno);

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
                        indicadorRechamada = String.Compare("S", possuiMaisRegistros, true) == 0;

                        //Atualiza os dados da pesquisa no cache
                        CacheAdmin.AtualizarPesquisa<Modelo.AvisoDebito>(Cache.Request,
                            idPesquisa, avisos, indicadorRechamada, parametros);
                    }

                    //Recupera os dados do cache
                    List<Modelo.AvisoDebito> retorno = CacheAdmin.RecuperarPesquisa<Modelo.AvisoDebito>(Cache.Request,
                        idPesquisa, registroInicial, qtdRegistrosRetornar, out qtdTotalRegistrosCache);

                    log.GravarLog(EventoLog.FimServico, new { qtdTotalRegistrosCache, codigoRetorno, retorno });

                    //Retorna os objetos mapeados para modelos de serviço
                    return this.PreencherModeloServico(retorno).ToList();
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