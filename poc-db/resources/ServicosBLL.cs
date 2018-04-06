/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo.Servicos;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;
using AG = Redecard.PN.Extrato.Agentes.ServicosAG;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// Classe de negócio para consultas WA do Módulo Extrato - Relatório de Serviços
    /// </summary>
    /// <remarks>
    /// Books utilizados pelos métodos da classe:<br/>
    /// - Book WACA1210 / Programa WA1210 / TranID ISFK / Método ConsultarSerasaAvs<br/>
    /// - Book WACA1260	/ Programa WA1260 / TranID ISGH / Método ConsultarGateway<br/>
    /// - Book WACA1261	/ Programa WA1261 / TranID ISGI / Método ConsultarBoleto<br/>
    /// - Book WACA1262	/ Programa WA1262 / TranID ISGJ / Método ConsultarAnaliseRisco<br/>
    /// - Book WACA1263	/ Programa WA1263 / TranID ISGK / Método ConsultarManualReview<br/>
    /// - Book BKWA1260 / Programa WA2400 / TranID ISGH / Método ConsultarNovoPacote<br/>
    /// - Book BKWA2410 / Programa WA241  / TranID ISIA / Método ConsultarRecargaCelular
    /// </remarks>    
    public class ServicosBLL : RegraDeNegocioBase
    {
#if DEBUG        
        private Int32 MAX_REGISTROS_TESTE = Int32.MaxValue;
#endif

        /// <summary>
        /// Instância singleton da classe
        /// </summary>
        private static ServicosBLL _Instancia;
        /// <summary>
        /// Instância singleton da classe
        /// </summary>
        public static ServicosBLL Instancia { get { return _Instancia ?? (_Instancia = new ServicosBLL()); } }

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Gateway.
        /// </summary>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Gateway</returns>
        public List<Gateway> ConsultarGateway(            
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,           
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta - Gateway - WACA1260"))
            {
                try
                {
                    var rechamada = new Dictionary<String, Object>();
                    var quantidadeTotal = default(Int16);
                    var indicadorRechamada = default(Boolean);
                    var retorno = new List<Gateway>();

                    do
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente, 
                            new { pvs, dataInicial, dataFinal, rechamada, quantidadeTotal, indicadorRechamada });

                        var _retorno = AG.Instancia.ConsultarGateway(
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out quantidadeTotal,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_retorno == null || _retorno.Count == 0)
                            indicadorRechamada = false;

                        //Se chamada retornou com erro, cancela as rechamadas subsequentes
                        if (status.CodigoRetorno != 0 && status.CodigoRetorno != 10 && status.CodigoRetorno != 60)
                            indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente,
                            new { _retorno, rechamada, indicadorRechamada, quantidadeTotal, status });

                        if (_retorno != null && _retorno.Count > 0)
                            retorno.AddRange(_retorno);
                    }
                    while (indicadorRechamada);

#if DEBUG
                    for (int i = 0; i < new Random().Next(11, 250); i++)
                        retorno.Add(retorno[0].ObterCopia());
                    retorno = retorno.Take(MAX_REGISTROS_TESTE).ToList();
                    for (int i = 0; i < retorno.Count; i++)
                        retorno[i].QuantidadeContratada = i;                    
#endif

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Boleto.
        /// </summary>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Boleto</returns>
        public List<Boleto> ConsultarBoleto(            
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta - Boleto - WACA1261"))
            {
                try
                {
                    var rechamada = new Dictionary<String, Object>();
                    var quantidadeTotal = default(Int16);
                    var indicadorRechamada = default(Boolean);
                    var retorno = new List<Boleto>();

                    do
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente,
                            new { pvs, dataInicial, dataFinal, rechamada, quantidadeTotal, indicadorRechamada });

                        var _retorno = AG.Instancia.ConsultarBoleto(
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out quantidadeTotal,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_retorno == null || _retorno.Count == 0)
                            indicadorRechamada = false;

                        //Se chamada retornou com erro, cancela as rechamadas subsequentes
                        if (status.CodigoRetorno != 0 && status.CodigoRetorno != 10 && status.CodigoRetorno != 60)
                            indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente, 
                            new { _retorno, rechamada, indicadorRechamada, quantidadeTotal, status });

                        if (_retorno != null && _retorno.Count > 0)
                            retorno.AddRange(_retorno);
                    }
                    while (indicadorRechamada);

#if DEBUG
                    for (int i = 0; i < new Random().Next(11, 250); i++)                   
                        retorno.Add(retorno[0].ObterCopia());
                    retorno = retorno.Take(MAX_REGISTROS_TESTE).ToList();
                    for (int i = 0; i < retorno.Count; i++)
                        retorno[i].QuantidadeContratada = i;
#endif

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Análise de Risco.
        /// </summary>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Análise de Risco</returns>
        public List<AnaliseRisco> ConsultarAnaliseRisco(            
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta - Análise Risco - WACA1262"))
            {
                try
                {
                    var rechamada = new Dictionary<String, Object>();
                    var quantidadeTotal = default(Int16);
                    var indicadorRechamada = default(Boolean);
                    var retorno = new List<AnaliseRisco>();

                    do
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente,
                            new { pvs, dataInicial, dataFinal, rechamada, quantidadeTotal, indicadorRechamada });

                        var _retorno = AG.Instancia.ConsultarAnaliseRisco(
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out quantidadeTotal,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_retorno == null || _retorno.Count == 0)
                            indicadorRechamada = false;

                        //Se chamada retornou com erro, cancela as rechamadas subsequentes
                        if (status.CodigoRetorno != 0 && status.CodigoRetorno != 10 && status.CodigoRetorno != 60)
                            indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente,
                            new { _retorno, rechamada, indicadorRechamada, quantidadeTotal, status });

                        if (_retorno != null && _retorno.Count > 0)
                            retorno.AddRange(_retorno);
                    }
                    while (indicadorRechamada);

#if DEBUG
                    for (int i = 0; i < new Random().Next(11, 250); i++)
                        retorno.Add(retorno[0].ObterCopia());
                    retorno = retorno.Take(MAX_REGISTROS_TESTE).ToList();
                    for (int i = 0; i < retorno.Count; i++)
                        retorno[i].QuantidadeContratada = i;
#endif

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Manual Review.
        /// </summary>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Manual Review</returns>
        public List<ManualReview> ConsultarManualReview(            
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta - Manual Review - WACA1263"))
            {
                try
                {
                    var rechamada = new Dictionary<String, Object>();
                    var quantidadeTotal = default(Int16);
                    var indicadorRechamada = default(Boolean);
                    var retorno = new List<ManualReview>();

                    do
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente,
                                new { pvs, dataInicial, dataFinal, rechamada, quantidadeTotal, indicadorRechamada });

                        var _retorno = AG.Instancia.ConsultarManualReview(
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out quantidadeTotal,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_retorno == null || _retorno.Count == 0)
                            indicadorRechamada = false;

                        //Se chamada retornou com erro, cancela as rechamadas subsequentes
                        if (status.CodigoRetorno != 0 && status.CodigoRetorno != 10 && status.CodigoRetorno != 60)
                            indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente,
                            new { _retorno, rechamada, indicadorRechamada, quantidadeTotal, status });

                        if (_retorno != null && _retorno.Count > 0)
                            retorno.AddRange(_retorno);
                    }
                    while (indicadorRechamada);

#if DEBUG
                    for (int i = 0; i < new Random().Next(11, 250); i++)
                        retorno.Add(retorno[0].ObterCopia());
                    retorno = retorno.Take(MAX_REGISTROS_TESTE).ToList();
                    for (int i = 0; i < retorno.Count; i++)
                        retorno[i].QuantidadeContratada = i;
#endif

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Novo Pacote.
        /// </summary>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Novo Pacote</returns>
        public List<NovoPacote> ConsultarNovoPacote(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta - Novo Pacote - BKWA1260"))
            {
                try
                {
                    var rechamada = new Dictionary<String, Object>();
                    var quantidadeTotal = default(Int16);
                    var indicadorRechamada = default(Boolean);
                    var retorno = new List<NovoPacote>();

                    do
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente,
                            new { pvs, dataInicial, dataFinal, rechamada, quantidadeTotal, indicadorRechamada });

                        var _retorno = AG.Instancia.ConsultarNovoPacote(
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out quantidadeTotal,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_retorno == null || _retorno.Count == 0)
                            indicadorRechamada = false;

                        //Se chamada retornou com erro, cancela as rechamadas subsequentes
                        if (status.CodigoRetorno != 0 && status.CodigoRetorno != 10 && status.CodigoRetorno != 60)
                            indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente,
                            new { _retorno, rechamada, indicadorRechamada, quantidadeTotal, status });

                        if (_retorno != null && _retorno.Count > 0)
                            retorno.AddRange(_retorno);
                    }
                    while (indicadorRechamada);

#if DEBUG
                    for (int i = 0; i < new Random().Next(11, 250); i++)
                        retorno.Add(retorno[0].ObterCopia());
                    retorno = retorno.Take(MAX_REGISTROS_TESTE).ToList();
                    for (int i = 0; i < retorno.Count; i++)
                        retorno[i].QuantidadeContratada = i;
#endif

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Serasa/AVS.
        /// </summary>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Serasa/AVS</returns>
        public List<SerasaAvs> ConsultarSerasaAvs(           
           List<Int32> pvs,
           DateTime dataInicial,
           DateTime dataFinal,
           out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta - SerasaAVS - WACA1210"))
            {
                try
                {
                    var rechamada = new Dictionary<String, Object>();
                    var quantidadeTotal = default(Int16);
                    var indicadorRechamada = default(Boolean);
                    var retorno = new List<SerasaAvs>();

                    do
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente, new { 
                            pvs, dataInicial, dataFinal, rechamada, quantidadeTotal, indicadorRechamada });

                        var _retorno = AG.Instancia.ConsultarSerasaAVS(
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out quantidadeTotal,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_retorno == null || _retorno.Count == 0)
                            indicadorRechamada = false;

                        //Se chamada retornou com erro, cancela as rechamadas subsequentes
                        if (status.CodigoRetorno != 0 && status.CodigoRetorno != 10 && status.CodigoRetorno != 60)
                            indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente,
                            new { _retorno, rechamada, indicadorRechamada, quantidadeTotal, status });

                        if (_retorno != null && _retorno.Count > 0)
                            retorno.AddRange(_retorno);
                    }
                    while (indicadorRechamada);

#if DEBUG
                    for (int i = 0; i < new Random().Next(11, 250); i++)
                    {
                        retorno.Add(retorno[0].ObterCopia());
                        retorno.Add(retorno[1].ObterCopia());
                    }
                    retorno = retorno.Take(MAX_REGISTROS_TESTE).ToList();
                    for (int i = 0; i < retorno.Count; i++)
                        retorno[i].QuantidadeMinimaConsultas = i;
#endif

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta utilizada no Relatório de Serviços - Recarga de Celular.
        /// </summary>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Serviços - Recarga de Celular</returns>
        public List<RecargaCelular> ConsultarRecargaCelular(
           List<Int32> pvs,
           DateTime dataInicial,
           DateTime dataFinal,
           out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta - Recarga Celular - BKWA2410"))
            {
                try
                {
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var retorno = new List<RecargaCelular>();

                    do
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente,
                                new { pvs, dataInicial, dataFinal, rechamada, indicadorRechamada });

                        var _retorno = AG.Instancia.ConsultarRecargaCelular(
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_retorno == null || _retorno.Count == 0)
                            indicadorRechamada = false;

                        //Se chamada retornou com erro, cancela as rechamadas subsequentes
                        if (status.CodigoRetorno != 0 && status.CodigoRetorno != 10 && status.CodigoRetorno != 60)
                            indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente,
                            new { _retorno, rechamada, indicadorRechamada, status });

                        if(_retorno != null && _retorno.Count > 0)
                            retorno.AddRange(_retorno);
                    }
                    while (indicadorRechamada);

#if DEBUG
                    for (int i = 0; i < new Random().Next(11, 250); i++)
                        retorno.Add(retorno[0].ObterCopia());
                    retorno = retorno.Take(MAX_REGISTROS_TESTE).ToList();
                    for (int i = 0; i < retorno.Count; i++)
                        retorno[i].QuantidadeTransacao = i;
#endif

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
    }
}
