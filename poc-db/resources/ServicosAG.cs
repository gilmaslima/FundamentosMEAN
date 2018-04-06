using System;
using System.Collections.Generic;
using System.Reflection;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.WAExtratoServicos;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.Servicos;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.ServicosTR;

namespace Redecard.PN.Extrato.Agentes
{
    /// <summary>
    /// Classe de acesso aos serviços HIS do WA para o Módulo Extrato - Relatório de Serviços 
    /// </summary>
    public class ServicosAG : AgentesBase
    {
        /// <summary>
        /// Instância singleton da classe
        /// </summary>
        private static ServicosAG _Instancia;
        /// <summary>
        /// Instância singleton da classe
        /// </summary>
        public static ServicosAG Instancia { get { return _Instancia ?? (_Instancia = new ServicosAG()); } }

        /// <summary>
        /// Consulta Gateway para o Relatório de Serviços
        /// </summary>
        /// <param name="pvs">Lista de PVs dos estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período</param>
        /// <param name="dataFinal">Data final do período</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="quantidadeTotalTransacoes">Quantidade total de transações</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lista de registros da consulta</returns>
        public List<Gateway> ConsultarGateway(            
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out Int16 quantidadeTotalTransacoes,            
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta - Gateway - WACA1260"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1260";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    Int32 _dataInicial = dataInicial.ToString("yyyyMM").ToInt32();
                    Int32 _dataFinal = dataFinal.ToString("yyyyMM").ToInt32();
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarGatewayEntrada(pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int16 _rechamadaNroTS = rechamada.GetValueOrDefault<Int16>("NroTs");
                    quantidadeTotalTransacoes = rechamada.GetValueOrDefault<Int16>("QtdTs");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, quantidadeTotalTransacoes, _rechamadaNroTS,
                            _resto, _areaFixa });

                        contexto.Cliente.ConsultarGateway(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref quantidadeTotalTransacoes,
                            ref _rechamadaNroTS,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, quantidadeTotalTransacoes, _rechamadaNroTS,
                            _resto, _areaFixa });
                    }

                    rechamada["NroTs"] = _rechamadaNroTS;
                    rechamada["QtdTs"] = quantidadeTotalTransacoes;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);
                    
                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarGatewaySaida(_areaFixa);                                        
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta Boleto para o Relatório de Serviços
        /// </summary>
        /// <param name="pvs">Lista de PVs dos estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período</param>
        /// <param name="dataFinal">Data final do período</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="quantidadeTotalTransacoes">Quantidade total de transações</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lista de registros da consulta</returns>
        public List<Boleto> ConsultarBoleto(            
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out Int16 quantidadeTotalTransacoes,            
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta - Boleto - WACA1261"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1261";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    Int32 _dataInicial = dataInicial.ToString("yyyyMM").ToInt32();
                    Int32 _dataFinal = dataFinal.ToString("yyyyMM").ToInt32();
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarBoletoEntrada(pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int16 _rechamadaNroTS = rechamada.GetValueOrDefault<Int16>("NroTs");
                    quantidadeTotalTransacoes = rechamada.GetValueOrDefault<Int16>("QtdTs");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, quantidadeTotalTransacoes, _rechamadaNroTS,
                            _resto, _areaFixa });

                        contexto.Cliente.ConsultarBoleto(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref quantidadeTotalTransacoes,
                            ref _rechamadaNroTS,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, quantidadeTotalTransacoes, _rechamadaNroTS,
                            _resto, _areaFixa });
                    }

                    rechamada["NroTs"] = _rechamadaNroTS;
                    rechamada["QtdTs"] = quantidadeTotalTransacoes;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarBoletoSaida(_areaFixa);                                        
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta Análise de Risco para o Relatório de Serviços
        /// </summary>
        /// <param name="pvs">Lista de PVs dos estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período</param>
        /// <param name="dataFinal">Data final do período</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="quantidadeTotalTransacoes">Quantidade total de transações</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lista de registros da consulta</returns>
        public List<AnaliseRisco> ConsultarAnaliseRisco(            
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out Int16 quantidadeTotalTransacoes,            
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta - Análise Risco - WACA1262"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1262";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    Int32 _dataInicial = dataInicial.ToString("yyyyMM").ToInt32();
                    Int32 _dataFinal = dataFinal.ToString("yyyyMM").ToInt32();
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarAnaliseRiscoEntrada(pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int16 _rechamadaNroTS = rechamada.GetValueOrDefault<Int16>("NroTs");
                    quantidadeTotalTransacoes = rechamada.GetValueOrDefault<Int16>("QtdTs");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, quantidadeTotalTransacoes, _rechamadaNroTS,
                            _resto, _areaFixa });

                        contexto.Cliente.ConsultarAnaliseRisco(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref quantidadeTotalTransacoes,
                            ref _rechamadaNroTS,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, quantidadeTotalTransacoes, _rechamadaNroTS,
                            _resto, _areaFixa });
                    }

                    rechamada["NroTs"] = _rechamadaNroTS;
                    rechamada["QtdTs"] = quantidadeTotalTransacoes;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);
                    
                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarAnaliseRiscoSaida(_areaFixa);                                        
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta Manual Review para o Relatório de Serviços
        /// </summary>
        /// <param name="pvs">Lista de PVs dos estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período</param>
        /// <param name="dataFinal">Data final do período</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="quantidadeTotalTransacoes">Quantidade total de transações</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lista de registros da consulta</returns>
        public List<ManualReview> ConsultarManualReview(            
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out Int16 quantidadeTotalTransacoes,            
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta - Manual Review - WACA1263"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1263";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    Int32 _dataInicial = dataInicial.ToString("yyyyMM").ToInt32();
                    Int32 _dataFinal = dataFinal.ToString("yyyyMM").ToInt32();
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarManualReviewEntrada(pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int16 _rechamadaNroTS = rechamada.GetValueOrDefault<Int16>("NroTs");
                    quantidadeTotalTransacoes = rechamada.GetValueOrDefault<Int16>("QtdTs");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, quantidadeTotalTransacoes, _rechamadaNroTS,
                            _resto, _areaFixa });

                        contexto.Cliente.ConsultarManualReview(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref quantidadeTotalTransacoes,
                            ref _rechamadaNroTS,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, quantidadeTotalTransacoes, _rechamadaNroTS,
                            _resto, _areaFixa });
                    }

                    rechamada["NroTs"] = _rechamadaNroTS;
                    rechamada["QtdTs"] = quantidadeTotalTransacoes;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarManualReviewSaida(_areaFixa);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta Novo Pacote para o Relatório de Serviços
        /// </summary>
        /// <param name="pvs">Lista de PVs dos estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período</param>
        /// <param name="dataFinal">Data final do período</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="quantidadeTotalTransacoes">Quantidade total de transações</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lista de registros da consulta</returns>
        public List<NovoPacote> ConsultarNovoPacote(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out Int16 quantidadeTotalTransacoes,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta - Novo Pacote - BKWA1260"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA2400";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    Int32 _dataInicial = dataInicial.ToString("yyyyMM").ToInt32();
                    Int32 _dataFinal = dataFinal.ToString("yyyyMM").ToInt32();
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _areaFixa = TR.ConsultarNovoPacoteEntrada(pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int16 _rechamadaNroTS = rechamada.GetValueOrDefault<Int16>("NroTs");
                    quantidadeTotalTransacoes = rechamada.GetValueOrDefault<Int16>("QtdTs");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new {
                            _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, 
                            quantidadeTotalTransacoes, _rechamadaNroTS, _areaFixa });

                        contexto.Cliente.ConsultarNovoPacote(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _rechamadaIndicador,
                            ref quantidadeTotalTransacoes,
                            ref _rechamadaNroTS,
                            ref _codRetorno,
                            ref _msgRetorno,                            
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new {
                            _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador,
                            quantidadeTotalTransacoes, _rechamadaNroTS, _areaFixa });
                    }

                    rechamada["NroTs"] = _rechamadaNroTS;
                    rechamada["QtdTs"] = quantidadeTotalTransacoes;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = String.Compare("S", _rechamadaIndicador ?? "", true) == 0;

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarNovoPacoteSaida(_areaFixa);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta Serasa/AVS para o Relatório de Serviços
        /// </summary>
        /// <param name="pvs">Lista de PVs dos estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período</param>
        /// <param name="dataFinal">Data final do período</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="quantidadeTotalTransacoes">Quantidade total de transações</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lista de registros da consulta</returns>
        public List<SerasaAvs> ConsultarSerasaAVS(            
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out Int16 quantidadeTotalTransacoes,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta - Serasa/AVS - WACA1210"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WACA1110";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarSerasaAVSEntrada(pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int16 _rechamadaNroTS = rechamada.GetValueOrDefault<Int16>("NroTs");
                    quantidadeTotalTransacoes = rechamada.GetValueOrDefault<Int16>("QtdTs");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _msgRetorno, _rechamadaIndicador, quantidadeTotalTransacoes, _rechamadaNroTS, _resto, _areaFixa });

                        contexto.Cliente.ConsultarSerasaAvs(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref quantidadeTotalTransacoes,
                            ref _rechamadaNroTS,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _msgRetorno, _rechamadaIndicador, quantidadeTotalTransacoes, _rechamadaNroTS, _resto, _areaFixa });
                    }

                    rechamada["NroTs"] = _rechamadaNroTS;
                    rechamada["QtdTs"] = quantidadeTotalTransacoes;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);
                                        
                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarSerasaAVSSaida(_areaFixa);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta Recarga de Celular para o Relatório de Serviços
        /// </summary>
        /// <param name="pvs">Lista de PVs dos estabelecimentos</param>
        /// <param name="dataInicial">Data inicial do período</param>
        /// <param name="dataFinal">Data final do período</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lista de registros da consulta</returns>
        public List<RecargaCelular> ConsultarRecargaCelular(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta - Recarga Celular - BKWA2410"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA241";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd.MM.yyyy");
                    String _dataFinal = dataFinal.ToString("dd.MM.yyyy");
                    Int16 _codRetorno = default(Int16);
                    Int16 _codErro = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarRecargaCelularEntrada(pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");
                    Int32 _rechamadaDatTran = rechamada.GetValueOrDefault<Int32>("DatTran");
                    Int32 _rechamadaNumPv = rechamada.GetValueOrDefault<Int32>("NumPv");
                    Decimal _rechamadaNumTran = rechamada.GetValueOrDefault<Decimal>("NumTran");
                    Int16 _rechamadaNumBlc = rechamada.GetValueOrDefault<Int16>("NumBlc");

                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, 
                            _dataFinal, _codRetorno, _msgRetorno, _codErro, _rechamadaIndicador, _rechamadaDatTran, 
                            _rechamadaNumPv, _rechamadaNumTran, _rechamadaNumBlc, _resto, _areaFixa });

                        contexto.Cliente.ConsultarRecargaCelular(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _codErro,
                            ref _rechamadaIndicador,
                            ref _rechamadaDatTran,
                            ref _rechamadaNumPv,
                            ref _rechamadaNumTran,
                            ref _rechamadaNumBlc,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new {
                            _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal, _codRetorno,
                            _msgRetorno, _codErro, _rechamadaIndicador, _rechamadaDatTran, _rechamadaNumPv,
                            _rechamadaNumTran, _rechamadaNumBlc, _resto, _areaFixa });
                    }

                    rechamada["Indicador"] = _rechamadaIndicador;
                    rechamada["DatTran"] = _rechamadaDatTran;
                    rechamada["NumPv"] = _rechamadaNumPv;
                    rechamada["NumTran"] = _rechamadaNumTran;
                    rechamada["NumBlc"] = _rechamadaNumBlc;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = String.Compare("S", _rechamadaIndicador, true) == 0;

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarRecargaCelularSaida(_areaFixa);
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