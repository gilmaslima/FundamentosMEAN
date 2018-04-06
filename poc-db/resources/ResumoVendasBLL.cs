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
using Redecard.PN.Extrato.Modelo.ResumoVendas;
using Redecard.PN.Extrato.Modelo;
using AG = Redecard.PN.Extrato.Agentes.ResumoVendasAG;

namespace Redecard.PN.Extrato.Negocio
{
    public class ResumoVendasBLL : RegraDeNegocioBase
    {
        private static ResumoVendasBLL _Instancia;
        public static ResumoVendasBLL Instancia { get { return _Instancia ?? (_Instancia = new ResumoVendasBLL()); } }

        public List<DebitoCVsAceitos> ConsultarDebitoCVsAceitos(           
            Int32 pv,
            Int32 numeroRV,
            DateTime dataApresentacao,            
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Débito - CVs Aceitos - WACA668"))
            {
                try
                {
                    //Variáveis auxiliares para chamada de agentes
                    Boolean _indicadorRechamada = true;
                    var _rechamada = new Dictionary<String, Object>();
                    status = new StatusRetornoDTO(0, String.Empty, FONTE);

                    //Variável de retorno
                    var _retorno = new List<DebitoCVsAceitos>();

                    //Consulta todos os registros
                    while (_indicadorRechamada)
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente, new { pv, numeroRV, dataApresentacao, _rechamada });

                        var _dados = AG.Instancia.ConsultarDebitoCVsAceitos(
                            pv,
                            numeroRV,
                            dataApresentacao,
                            ref _rechamada,
                            out _indicadorRechamada,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_dados == null || _dados.Count == 0)
                            _indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente, new { _rechamada, _indicadorRechamada, status, _dados });

                        //Se código for diferente de zero, não continua loop e retorna vazio
                        if (status.CodigoRetorno != 0)
                            return null;

                        //Guarda registros desta iteração na lista de retorno
                        if(_dados != null && _dados.Count > 0)
                            _retorno.AddRange(_dados);
                    }

                    return _retorno;
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

        public List<DebitoCDCAjuste> ConsultarDebitoConstrucardAjustes(
            Int32 pv,
            Int32 resumo,
            DateTime dataApresentacao,
            String tipoResumo,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Débito/CDC - Ajustes - WACA748"))
            {
                try
                {
                    //Variáveis auxiliares para chamada de agentes            
                    status = new StatusRetornoDTO(0, String.Empty, FONTE);

                    //Variável de retorno
                    List<DebitoCDCAjuste> _retorno = null;

                    //Efetua consulta no mainframe
                    Log.GravarLog(EventoLog.ChamadaAgente, new { pv, resumo, dataApresentacao, tipoResumo });

                    _retorno = AG.Instancia.ConsultarDebitoConstrucardAjustes(pv, resumo, dataApresentacao, tipoResumo, out status);

                    Log.GravarLog(EventoLog.RetornoAgente, new { status, _retorno });

                    //Se código for diferente de zero, não continua loop e retorna vazio
                    if (status.CodigoRetorno != 0)
                        return null;

                    return _retorno;
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

        public List<CreditoCVsAceitos> ConsultarCreditoCVsAceitos(
            Int32 pv,
            Int32 numeroRV,
            Int16 numeroMes,
            DateTime dataApresentacao,
            String timestamp,
            String tipoResumoVenda,            
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Crédito - CVs Aceitos - WACA706"))
            {
                try
                {
                    //Variáveis auxiliares para chamada de agentes
                    Boolean _indicadorRechamada = true;
                    var _rechamada = new Dictionary<String, Object>();
                    status = new StatusRetornoDTO(0, String.Empty, FONTE);

                    //Variável de retorno
                    var _retorno = new List<CreditoCVsAceitos>();

                    //Consulta todos os registros
                    while (_indicadorRechamada)
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente, new
                        {
                            pv,
                            numeroRV,
                            numeroMes,
                            dataApresentacao,
                            timestamp,
                            tipoResumoVenda,
                            _rechamada
                        });

                        var _dados = AG.Instancia.ConsultarCreditoCVsAceitos(
                            pv,
                            numeroRV,
                            numeroMes,
                            dataApresentacao,
                            timestamp,
                            tipoResumoVenda,
                            ref _rechamada,
                            out _indicadorRechamada,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_dados == null || _dados.Count == 0)
                            _indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente, new { _rechamada, _indicadorRechamada, status, _dados });

                        //Se código for diferente de zero, não continua loop e retorna vazio
                        if (status.CodigoRetorno != 0)
                            return null;

                        //Guarda registros desta iteração na lista de retorno
                        if(_dados != null && _dados.Count > 0)
                            _retorno.AddRange(_dados);
                    }

                    return _retorno;
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

        public List<CreditoAjustes> ConsultarCreditoAjustes(
            String timestamp,
            Int32 pv,
            Int16 tipoRV,
            Int32 numeroRV,
            DateTime dataApresentacao,            
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Crédito - Ajustes - WACA704"))
            {
                try
                {
                    //Variáveis auxiliares para chamada de agentes
                    Boolean _indicadorRechamada = true;
                    var _rechamada = new Dictionary<String, Object>();
                    status = new StatusRetornoDTO(0, String.Empty, FONTE);

                    //Variável de retorno
                    var _retorno = new List<CreditoAjustes>();

                    //Consulta todos os registros
                    while (_indicadorRechamada)
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente, new { timestamp, pv, tipoRV, numeroRV, dataApresentacao, _rechamada });

                        var _dados = AG.Instancia.ConsultarCreditoAjustes(
                            timestamp,
                            pv,
                            tipoRV,
                            numeroRV,
                            dataApresentacao,
                            ref _rechamada,
                            out _indicadorRechamada,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_dados == null || _dados.Count == 0)
                            _indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente, new { _rechamada, _indicadorRechamada, status, _dados });

                        //Se código for diferente de zero, não continua loop e retorna vazio
                        if (status.CodigoRetorno != 0)
                            return null;

                        //Guarda registros desta iteração na lista de retorno
                        if(_dados != null && _dados.Count > 0)
                            _retorno.AddRange(_dados);
                    }

                    return _retorno;
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

        public List<ConstrucardCVsAceitos> ConsultarConstrucardCVsAceitos(
            Int32 pv,
            Int32 numeroRV,
            DateTime dataApresentacao,            
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Construcard - CVs Aceitos - WACA797"))
            {
                try
                {
                    //Variáveis auxiliares para chamada de agentes
                    Boolean _indicadorRechamada = true;
                    var _rechamada = new Dictionary<String, Object>();
                    status = new StatusRetornoDTO(0, String.Empty, FONTE);

                    //Variável de retorno
                    var _retorno = new List<ConstrucardCVsAceitos>();

                    //Consulta todos os registros
                    while (_indicadorRechamada)
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente, new { pv, numeroRV, dataApresentacao, _rechamada });

                        var _dados = AG.Instancia.ConsultarConstrucardCVsAceitos(
                            pv,
                            numeroRV,
                            dataApresentacao,
                            ref _rechamada,
                            out _indicadorRechamada,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_dados == null || _dados.Count == 0)
                            _indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente, new { _rechamada, _indicadorRechamada, status, _dados });

                        //Se código for diferente de zero, não continua loop e retorna vazio
                        if (status.CodigoRetorno != 0)
                            return null;

                        //Guarda registros desta iteração na lista de retorno
                        if(_dados != null && _dados.Count > 0)
                            _retorno.AddRange(_dados);
                    }

                    return _retorno;
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
        /// Consulta os totais de um Resumo de Vendas de Recarga de Celular.
        /// </summary>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="origemPesquisa">Origem/Tipo da pesquisa</param>
        /// <param name="status">Status retorno da consulta</param>
        /// <returns>Totais do Resumo de Vendas de Recarga de Celular</returns>
        public RecargaCelularResumo ConsultarRecargaCelularResumo(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            RecargaCelularResumoOrigem origemPesquisa,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Resumo de Vendas - Recarga de Celular - Resumo - BKWA2430"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, new { numeroPv, numeroRv, dataPagamento, origemPesquisa });

                    var dados = AG.Instancia.ConsultarRecargaCelularResumo(
                        numeroPv, 
                        numeroRv, 
                        dataPagamento,
                        origemPesquisa,
                        out status);

                    log.GravarLog(EventoLog.RetornoAgente, new { dados, status });

                    return dados;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public RecargaCelularVencimento ConsultarRecargaCelularVencimentos(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Recarga de Celular - Vencimentos - BKWA2440"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { numeroPv, numeroRv, dataPagamento });

                    var _dados = AG.Instancia.ConsultarRecargaCelularVencimentos(
                        numeroPv,
                        numeroRv,                        
                        dataPagamento,
                        out status);

                    Log.GravarLog(EventoLog.RetornoAgente, new { _dados, status });

                    return _dados;
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

        private List<RecargaCelularAjuste> ConsultarRecargaCelularAjustes(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            Int16 tipoPesquisa,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Recarga Celular - Ajustes - BKWA2450"))
            {
                try
                {
                    //Variáveis auxiliares para chamada de agentes
                    Boolean _indicadorRechamada = true;
                    var _rechamada = new Dictionary<String, Object>();
                    status = new StatusRetornoDTO(0, String.Empty, FONTE);

                    //Variável de retorno
                    var _retorno = new List<RecargaCelularAjuste>();

                    //Consulta todos os registros
                    while (_indicadorRechamada)
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente, new { numeroPv, numeroRv, dataPagamento, _rechamada });

                        var _dados = AG.Instancia.ConsultarRecargaCelularAjustes(
                            tipoPesquisa,
                            numeroPv,
                            numeroRv,                            
                            dataPagamento,
                            ref _rechamada,
                            out _indicadorRechamada,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_dados == null || _dados.Count == 0)
                            _indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente, new { _rechamada, _indicadorRechamada, status, _dados });

                        //Se código for diferente de zero, não continua loop e retorna vazio
                        if (status.CodigoRetorno != 0)
                            return null;

                        //Guarda registros desta iteração na lista de retorno
                        if (_dados != null && _dados.Count > 0)
                            _retorno.AddRange(_dados);
                    }

                    return _retorno;
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

        public List<RecargaCelularAjusteCredito> ConsultarRecargaCelularAjustesCredito(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            out StatusRetornoDTO status)
        {
            //Consulta os Ajustes Crédito (tipoPesquisa = 1)
            List<RecargaCelularAjuste> ajustes = 
                this.ConsultarRecargaCelularAjustes(numeroPv, numeroRv, dataPagamento, 1, out status);

            //Garante que a coleção não seja nula
            if (ajustes == null)
                ajustes = new List<RecargaCelularAjuste>();

            //Cast para Ajuste Crédito
            return ajustes.Where(ajuste => ajuste is RecargaCelularAjusteCredito)
                .Cast<RecargaCelularAjusteCredito>().ToList();
        }

        public List<RecargaCelularAjusteDebito> ConsultarRecargaCelularAjustesDebito(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            out StatusRetornoDTO status)
        {
            //Consulta os Ajustes Débito (tipoPesquisa = 2)
            List<RecargaCelularAjuste> ajustes =
                this.ConsultarRecargaCelularAjustes(numeroPv, numeroRv, dataPagamento, 2, out status);

            //Garante que a coleção não seja nula
            if (ajustes == null)
                ajustes = new List<RecargaCelularAjuste>();

            //Cast para Ajuste Débito
            return ajustes.Where(ajuste => ajuste is RecargaCelularAjusteDebito)
                .Cast<RecargaCelularAjusteDebito>().ToList();
        }

        public List<RecargaCelularComprovante> ConsultarRecargaCelularComprovantes(
           Int32 numeroPv,
           Int32 numeroRv,
           DateTime dataPagamento,
           out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Recarga Celular - Comprovantes - BKWA2460"))
            {
                try
                {
                    //Variáveis auxiliares para chamada de agentes
                    Boolean _indicadorRechamada = true;
                    var _rechamada = new Dictionary<String, Object>();
                    status = new StatusRetornoDTO(0, String.Empty, FONTE);

                    //Variável de retorno
                    var _retorno = new List<RecargaCelularComprovante>();

                    //Consulta todos os registros
                    while (_indicadorRechamada)
                    {
                        Log.GravarLog(EventoLog.ChamadaAgente, new { numeroPv, numeroRv, dataPagamento, _rechamada });

                        var _dados = AG.Instancia.ConsultarRecargaCelularComprovantes(
                            numeroPv,
                            numeroRv,                            
                            dataPagamento,
                            ref _rechamada,
                            out _indicadorRechamada,
                            out status);

                        //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                        if (_dados == null || _dados.Count == 0)
                            _indicadorRechamada = false;

                        Log.GravarLog(EventoLog.RetornoAgente, new { _rechamada, _indicadorRechamada, status, _dados });

                        //Se código for diferente de zero, não continua loop e retorna vazio
                        if (status.CodigoRetorno != 0)
                            return null;

                        //Guarda registros desta iteração na lista de retorno
                        if (_dados != null && _dados.Count > 0)
                            _retorno.AddRange(_dados);
                    }

                    return _retorno;
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