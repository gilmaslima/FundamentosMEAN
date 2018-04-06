/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.WAExtratoRecargaCelular;
using Redecard.PN.Extrato.Modelo;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.RecargaCelularTR;
using Redecard.PN.Extrato.Modelo.RecargaCelular;

namespace Redecard.PN.Extrato.Agentes
{
    /// <summary>
    /// Classe de acesso aos serviços HIS do WA para o Módulo Extrato - Relatório Recarga de Celular - Detalhes
    /// </summary>
    public class RecargaCelularAG : AgentesBase
    {
        /// <summary>
        /// Instância singleton da classe
        /// </summary>
        private static RecargaCelularAG _Instancia;
        /// <summary>
        /// Instância singleton da classe
        /// </summary>
        public static RecargaCelularAG Instancia { get { return _Instancia ?? (_Instancia = new RecargaCelularAG()); } }

        /// <summary>
        /// Consulta do Relatório de Detalhamento de Recarga de Celular 
        /// </summary>
        /// <param name="pv">Número do PV</param>
        /// <param name="rv">Número do Resumo de Vendas</param>
        /// <param name="dataInicial">Data inicial do período</param>
        /// <param name="dataFinal">Data final do período</param>
        /// <param name="opcaoPesquisa">
        /// Opção de pesquisa:<br/>
        ///     - 1: PV / RV / Data Inicial<br/>
        ///     - 2: PV / Data Inicial / Data Final
        /// </param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="status">Status da consulta</param>        
        /// <returns>Lista de registros da consulta</returns>
        public List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalhe(            
            Int32 pv,
            Int32 rv,
            DateTime dataInicial,
            DateTime dataFinal,
            Int16 opcaoPesquisa,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Recarga de Celular - Detalhe - BKWA2420"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WAC242";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd.MM.yyyy");
                    String _dataFinal = dataFinal.ToString("dd.MM.yyyy");
                    var _registros = new WA242S_DETALHE[230];
                    var _resto = default(String);
                    var _resto2 = default(String);
                    var _codRetorno = default(Int16);
                    var _msgRetorno = default(String);
                    var _codErro = default(Int16);
                    var _qtdTransacoes = default(Int32);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");
                    Decimal _rechamadaNumTran = rechamada.GetValueOrDefault<Decimal>("NumTran");
                    String _rechamadaDatTran = rechamada.GetValueOrDefault<String>("DatTran");
                    String _rechamadaHorTran = rechamada.GetValueOrDefault<String>("HorTran");                    

                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario,
                            opcaoPesquisa, pv, rv, _dataInicial, _dataFinal, _rechamadaIndicador,
                            _rechamadaNumTran, _rechamadaDatTran, _rechamadaHorTran, _resto,
                            _codRetorno, _msgRetorno, _codErro, _qtdTransacoes, _registros, _resto2 });

#if !DEBUG
                        contexto.Cliente.ConsultarRecargaCelularDetalhe(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref opcaoPesquisa,
                            ref pv,
                            ref rv,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _rechamadaIndicador,
                            ref _rechamadaNumTran,
                            ref _rechamadaDatTran,
                            ref _rechamadaHorTran,
                            ref _resto,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _codErro,
                            ref _qtdTransacoes,
                            ref _registros,
                            ref _resto2);
#else
                        _nomePrograma = "WA";
                        _sistema = "IS";
                        _usuario = "xxx";
                        opcaoPesquisa = 1;
                        pv = 0;
                        rv = 0;
                        _dataInicial = "01.01.2014";
                        _dataFinal = "02.04.2014";
                        _rechamadaIndicador = "N";
                        _rechamadaNumTran = 0;
                        _rechamadaDatTran = "0";
                        _rechamadaHorTran = "0";
                        _codRetorno = 0;
                        _msgRetorno = "PESQUISA EFETUADA COM SUCESSO";
                        _codErro = 0;
                        _qtdTransacoes = 1;
                        _registros = new WA242S_DETALHE [] {
                            new WA242S_DETALHE {
                               WA242S_DAT_TRAN = "01.05.2014",
                               WA242S_HOR_TRAN = "09:55:24",
                               WA242S_NUM_CEL = "(11) 99999-2415",
                               WA242S_NUM_NSU = 123,
                               WA242S_DSC_OPER = "TIM",
                               WA242S_STA_CMSS = "Comiss. Aprovada",
                               WA242S_STA_TRAN = "Tran. Aprovada",
                               WA242S_VAL_CMSS = 1235.23m,
                               WA242S_VAL_TRAN = 6234.73m
                            }
                        };
#endif

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario,
                            opcaoPesquisa, pv, rv, _dataInicial, _dataFinal, _rechamadaIndicador,
                            _rechamadaNumTran, _rechamadaDatTran, _rechamadaHorTran, _resto,
                            _codRetorno, _msgRetorno, _codErro, _qtdTransacoes, _registros, _resto2 });
                    }

                    rechamada["Indicador"] = _rechamadaIndicador;
                    rechamada["NumTran"] =_rechamadaNumTran;
                    rechamada["DatTran"] =_rechamadaDatTran;     
                    rechamada["HorTran"] =_rechamadaHorTran;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = String.Compare("S", _rechamadaIndicador, true) == 0;
                    
                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;
                    
                    return TR.ConsultarRecargaCelularDetalheSaida(_qtdTransacoes, _registros);                                        
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