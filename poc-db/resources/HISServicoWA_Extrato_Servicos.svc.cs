using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Servicos.Servicos;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Modelo;
using DTO = Redecard.PN.Extrato.Modelo.Servicos;
using BLL = Redecard.PN.Extrato.Negocio.ServicosBLL;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Serviços.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1210 / Programa WA1210 / TranID ISFK / Método ConsultarSerasaAvs<br/>
    /// - Book WACA1260	/ Programa WA1260 / TranID ISGH / Método ConsultarGateway<br/>
    /// - Book WACA1261	/ Programa WA1261 / TranID ISGI / Método ConsultarBoleto<br/>
    /// - Book WACA1262	/ Programa WA1262 / TranID ISGJ / Método ConsultarAnaliseRisco<br/>
    /// - Book WACA1263	/ Programa WA1263 / TranID ISGK / Método ConsultarManualReview<br/>
    /// - Book BKWA2410 / Programa WA241  / TranID ISIA / Método ConsultarRecargaCelular
    /// </remarks>
    public class HISServicoWA_Extrato_Servicos : ServicoBaseExtrato, IHISServicoWA_Extrato_Servicos
    {
        #region [ Serviços - Gateway - WACA1260 / WA1260 / ISGH ]

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Gateway.<br/>
        /// - Book WACA1260 / Programa 1260 / TranID ISGH
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Gateway.</returns>
        public List<Gateway> ConsultarGateway(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,            
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Serviços - Gateway - WACA1260"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {
                    guidPesquisa, registroInicial, quantidadeRegistros,
                    dataInicial, dataFinal, pvs });

                try
                {
                    //Chave de cache da pesquisa
                    var chave = guidPesquisa.ToString();

                    //Busca pesquisa no cache
                    List<Gateway> registros = CacheAdmin.Recuperar<List<Gateway>>(Cache.Extrato, chave);

                    //Se registros não encontrado no cache, executa pesquisa
                    if (registros == null)
                    {
                        var statusDTO = default(StatusRetornoDTO);

                        List<DTO.Gateway> registrosDTO = BLL.Instancia.ConsultarGateway(
                           pvs, dataInicial, dataFinal, out statusDTO);

                        //Converte DTO para modelo de serviço
                        registros = Gateway.FromDTO(registrosDTO);
                        status = StatusRetorno.FromDTO(statusDTO);

                        //Adiciona dados no cache
                        CacheAdmin.Adicionar(Cache.Extrato, chave, registros);
                    }
                    else
                        //Gera status "fake", pois dados já foram consultados com sucesso e estão em cache
                        status = new StatusRetorno();

                    registros = registros ?? new List<Gateway>();                    
                    quantidadeTotalRegistros = registros.Count;

                    //Paginação dos registros consultados
                    registros = registros.Skip(registroInicial).Take(quantidadeRegistros).ToList();                       

                    Log.GravarLog(EventoLog.FimServico, new { status, registros, quantidadeTotalRegistros });

                    return registros;
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

        #region [ Serviços - Boleto - WACA1261 / WA1261 / ISGI ]

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - boleto.<br/>
        /// - Book WACA1261 / Programa 1261 / TranID ISGI
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Boleto.</returns>
        public List<Boleto> ConsultarBoleto(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,            
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Serviços - Boleto - WACA1261"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {
                    guidPesquisa, registroInicial, quantidadeRegistros,
                    dataInicial, dataFinal, pvs });

                try
                {
                    //Chave de cache da pesquisa
                    var chave = guidPesquisa.ToString();

                    //Busca pesquisa no cache
                    List<Boleto> registros = CacheAdmin.Recuperar<List<Boleto>>(Cache.Extrato, chave);

                    //Se registros não encontrado no cache, executa pesquisa
                    if (registros == null)
                    {
                        var statusDTO = default(StatusRetornoDTO);

                        List<DTO.Boleto> registrosDTO = BLL.Instancia.ConsultarBoleto(
                           pvs, dataInicial, dataFinal, out statusDTO);

                        //Converte DTO para modelo de serviço
                        registros = Boleto.FromDTO(registrosDTO);
                        status = StatusRetorno.FromDTO(statusDTO);

                        //Adiciona dados no cache
                        CacheAdmin.Adicionar(Cache.Extrato, chave, registros);
                    }
                    else
                        //Gera status "fake", pois dados já foram consultados com sucesso e estão em cache
                        status = new StatusRetorno();

                    registros = registros ?? new List<Boleto>();                    
                    quantidadeTotalRegistros = registros.Count;

                    //Paginação dos registros consultados
                    registros = registros.Skip(registroInicial).Take(quantidadeRegistros).ToList();                       

                    Log.GravarLog(EventoLog.FimServico, new { status, registros, quantidadeTotalRegistros });

                    return registros;
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

        #region [ Serviços - Análise de Risco - WACA1262 / WA1262 / ISGJ ]

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Análise de Risco.<br/>
        /// - Book WACA1262 / Programa 1262 / TranID ISGJ
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Análise de Risco.</returns>
        public List<AnaliseRisco> ConsultarAnaliseRisco(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,            
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Serviços - Análise Risco - WACA1262"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {
                    guidPesquisa, registroInicial, quantidadeRegistros,
                    dataInicial, dataFinal, pvs });

                try
                {
                    //Chave de cache da pesquisa
                    var chave = guidPesquisa.ToString();

                    //Busca pesquisa no cache
                    List<AnaliseRisco> registros = CacheAdmin.Recuperar<List<AnaliseRisco>>(Cache.Extrato, chave);

                    //Se registros não encontrado no cache, executa pesquisa
                    if (registros == null)
                    {
                        var statusDTO = default(StatusRetornoDTO);

                        List<DTO.AnaliseRisco> registrosDTO = BLL.Instancia.ConsultarAnaliseRisco(
                           pvs, dataInicial, dataFinal, out statusDTO);

                        //Converte DTO para modelo de serviço
                        registros = AnaliseRisco.FromDTO(registrosDTO);
                        status = StatusRetorno.FromDTO(statusDTO);

                        //Adiciona dados no cache
                        CacheAdmin.Adicionar(Cache.Extrato, chave, registros);
                    }
                    else
                        //Gera status "fake", pois dados já foram consultados com sucesso e estão em cache
                        status = new StatusRetorno();

                    registros = registros ?? new List<AnaliseRisco>();                    
                    quantidadeTotalRegistros = registros.Count;

                    //Paginação dos registros consultados
                    registros = registros.Skip(registroInicial).Take(quantidadeRegistros).ToList();                       

                    Log.GravarLog(EventoLog.FimServico, new { status, registros, quantidadeTotalRegistros });

                    return registros;
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

        #region [ Serviços - Manual Review - WACA1263 / WA1263 / ISGK ]

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Manual Review.<br/>
        /// - Book WACA1263 / Programa 1263 / TranID ISGK
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Manual Review.</returns>
        public List<ManualReview> ConsultarManualReview(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,            
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Serviços - Manual Review - WACA1263"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {
                    guidPesquisa, registroInicial, quantidadeRegistros,
                    dataInicial, dataFinal, pvs });

                try
                {
                    //Chave de cache da pesquisa
                    var chave = guidPesquisa.ToString();

                    //Busca pesquisa no cache
                    List<ManualReview> registros = CacheAdmin.Recuperar<List<ManualReview>>(Cache.Extrato, chave);

                    //Se registros não encontrado no cache, executa pesquisa
                    if (registros == null)
                    {
                        var statusDTO = default(StatusRetornoDTO);

                        List<DTO.ManualReview> registrosDTO = BLL.Instancia.ConsultarManualReview(
                           pvs, dataInicial, dataFinal, out statusDTO);

                        //Converte DTO para modelo de serviço
                        registros = ManualReview.FromDTO(registrosDTO);
                        status = StatusRetorno.FromDTO(statusDTO);

                        //Adiciona dados no cache
                        CacheAdmin.Adicionar(Cache.Extrato, chave, registros);
                    }
                    else
                        //Gera status "fake", pois dados já foram consultados com sucesso e estão em cache
                        status = new StatusRetorno();

                    registros = registros ?? new List<ManualReview>();                    
                    quantidadeTotalRegistros = registros.Count;

                    //Paginação dos registros consultados
                    registros = registros.Skip(registroInicial).Take(quantidadeRegistros).ToList();                       

                    Log.GravarLog(EventoLog.FimServico, new { status, registros, quantidadeTotalRegistros });

                    return registros;
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

        #region [ Serviços - Novo Pacote - BKWA1260 / WA1240 / ISGH ]

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Novo Pacote.<br/>
        /// - Book BKWA1260 / Programa WA1240 / TranID ISGH
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Novo Pacote.</returns>
        public List<NovoPacote> ConsultarNovoPacote(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,            
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Serviços - Novo Pacote - BKWA1260"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {
                    guidPesquisa, registroInicial, quantidadeRegistros,
                    dataInicial, dataFinal, pvs });

                try
                {
                    //Chave de cache da pesquisa
                    var chave = guidPesquisa.ToString();

                    //Busca pesquisa no cache
                    List<NovoPacote> registros = CacheAdmin.Recuperar<List<NovoPacote>>(Cache.Extrato, chave);

                    //Se registros não encontrado no cache, executa pesquisa
                    if (registros == null)
                    {
                        var statusDTO = default(StatusRetornoDTO);

                        List<DTO.NovoPacote> registrosDTO = BLL.Instancia.ConsultarNovoPacote(
                           pvs, dataInicial, dataFinal, out statusDTO);

                        //Converte DTO para modelo de serviço
                        registros = NovoPacote.FromDTO(registrosDTO);
                        status = StatusRetorno.FromDTO(statusDTO);

                        //Adiciona dados no cache
                        CacheAdmin.Adicionar(Cache.Extrato, chave, registros);
                    }
                    else
                        //Gera status "fake", pois dados já foram consultados com sucesso e estão em cache
                        status = new StatusRetorno();

                    registros = registros ?? new List<NovoPacote>();                    
                    quantidadeTotalRegistros = registros.Count;

                    //Paginação dos registros consultados
                    registros = registros.Skip(registroInicial).Take(quantidadeRegistros).ToList();                       

                    Log.GravarLog(EventoLog.FimServico, new { status, registros, quantidadeTotalRegistros });

                    return registros;
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

        #region [ Serviços - Serasa/AVS - WACA1210 / WA1210 / ISFK ]

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Serasa.<br/>
        /// - Book WACA1210 / Programa WA1210 / TranID ISFK
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Serasa.</returns>
        public List<Serasa> ConsultarSerasa(
           Guid guidPesquisa,
           Int32 registroInicial,
           Int32 quantidadeRegistros,           
           DateTime dataInicial,
           DateTime dataFinal,
           List<Int32> pvs,
           out Int32 quantidadeTotalRegistros,
           out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Serviços - Serasa - WACA1210"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {
                    guidPesquisa, registroInicial, quantidadeRegistros,
                    dataInicial, dataFinal, pvs });

                try
                {
                    //Chave de cache da pesquisa
                    var chave = guidPesquisa.ToString();

                    //Busca pesquisa no cache (Serasa + AVS)
                    List<SerasaAvs> registros = CacheAdmin.Recuperar<List<SerasaAvs>>(Cache.Extrato, chave);

                    //Se registros não encontrado no cache, executa pesquisa
                    if (registros == null)
                    {
                        var statusDTO = default(StatusRetornoDTO);

                        List<DTO.SerasaAvs> registrosDTO = BLL.Instancia.ConsultarSerasaAvs(
                           pvs, dataInicial, dataFinal, out statusDTO);

                        //Converte DTO para modelo de serviço
                        registros = SerasaAvs.FromDTO(registrosDTO);
                        status = StatusRetorno.FromDTO(statusDTO);

                        //Adiciona dados no cache (Serasa + AVS)
                        CacheAdmin.Adicionar(Cache.Extrato, chave, registros);
                    }
                    else
                        //Gera status "fake", pois dados já foram consultados com sucesso e estão em cache
                        status = new StatusRetorno();

                    registros = registros ?? new List<SerasaAvs>();                    
                    
                    //Filtra apenas os registros Serasa, removendo os registros AVS
                    registros = registros.Where(registro => registro is Serasa).ToList();
                    quantidadeTotalRegistros = registros.Count;

                    //Paginação dos registros consultados
                    registros = registros.Skip(registroInicial).Take(quantidadeRegistros).ToList();                       

                    Log.GravarLog(EventoLog.FimServico, new { status, registros, quantidadeTotalRegistros });

                    return registros.Cast<Serasa>().ToList();
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
        /// Consulta utilizada no Relatório de Serviços - AVS.<br/>
        /// - Book WACA1210 / Programa 1210 / TranID ISFK
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - AVS.</returns>
        public List<Avs> ConsultarAvs(
           Guid guidPesquisa,
           Int32 registroInicial,
           Int32 quantidadeRegistros,
           DateTime dataInicial,
           DateTime dataFinal,
           List<Int32> pvs,
           out Int32 quantidadeTotalRegistros,
           out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Serviços - AVS - WACA1210"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {
                    guidPesquisa, registroInicial, quantidadeRegistros,
                    dataInicial, dataFinal, pvs });

                try
                {
                    //Chave de cache da pesquisa
                    var chave = guidPesquisa.ToString();

                    //Busca pesquisa no cache (Serasa + AVS)
                    List<SerasaAvs> registros = CacheAdmin.Recuperar<List<SerasaAvs>>(Cache.Extrato, chave);

                    //Se registros não encontrado no cache, executa pesquisa
                    if (registros == null)
                    {
                        var statusDTO = default(StatusRetornoDTO);

                        List<DTO.SerasaAvs> registrosDTO = BLL.Instancia.ConsultarSerasaAvs(
                           pvs, dataInicial, dataFinal, out statusDTO);

                        //Converte DTO para modelo de serviço
                        registros = SerasaAvs.FromDTO(registrosDTO);
                        status = StatusRetorno.FromDTO(statusDTO);

                        //Adiciona dados no cache (Serasa + AVS)
                        CacheAdmin.Adicionar(Cache.Extrato, chave, registros);
                    }
                    else
                        //Gera status "fake", pois dados já foram consultados com sucesso e estão em cache
                        status = new StatusRetorno();

                    registros = registros ?? new List<SerasaAvs>();
                    
                    //Filtra apenas os registros Serasa, removendo os registros AVS
                    registros = registros.Where(registro => registro is Avs).ToList();
                    quantidadeTotalRegistros = registros.Count;

                    //Paginação dos registros consultados
                    registros = registros.Skip(registroInicial).Take(quantidadeRegistros).ToList();

                    Log.GravarLog(EventoLog.FimServico, new { status, registros, quantidadeTotalRegistros });

                    return registros.Cast<Avs>().ToList();
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

        #region [ Serviços - Recarga de Celular - BKWA2410 / WA241 / ISIA ]

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Recarga de Celular.<br/>
        /// - Book BKWA1260 / Programa WA241 / TranID ISIA
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Recarga de Celular.</returns>
        public List<RecargaCelular> ConsultarRecargaCelular(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out Int32 quantidadeTotalRegistros,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Serviços - Recarga de Celular - BKWA2410"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {
                    guidPesquisa, registroInicial, quantidadeRegistros,
                    dataInicial, dataFinal, pvs });

                try
                {
                    //Chave de cache da pesquisa
                    var chave = guidPesquisa.ToString();

                    //Busca pesquisa no cache
                    List<RecargaCelular> registros = CacheAdmin.Recuperar<List<RecargaCelular>>(Cache.Extrato, chave);

                    //Se registros não encontrado no cache, executa pesquisa
                    if (registros == null)
                    {
                        var statusDTO = default(StatusRetornoDTO);

                        List<DTO.RecargaCelular> registrosDTO = BLL.Instancia.ConsultarRecargaCelular(
                           pvs, dataInicial, dataFinal, out statusDTO);

                        //Converte DTO para modelo de serviço
                        registros = RecargaCelular.FromDTO(registrosDTO);
                        status = StatusRetorno.FromDTO(statusDTO);

                        //Adiciona dados no cache
                        CacheAdmin.Adicionar(Cache.Extrato, chave, registros);
                    }
                    else
                        //Gera status "fake", pois dados já foram consultados com sucesso e estão em cache
                        status = new StatusRetorno();

                    registros = registros ?? new List<RecargaCelular>();
                    quantidadeTotalRegistros = registros.Count;

                    //Paginação dos registros consultados
                    registros = registros.Skip(registroInicial).Take(quantidadeRegistros).ToList();

                    Log.GravarLog(EventoLog.FimServico, new { status, registros, quantidadeTotalRegistros });

                    return registros;
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
    }
}