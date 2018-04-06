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
using System.Reflection;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.ResumoVendasTR;
using Redecard.PN.Extrato.Agentes.WAExtratoResumoVendas;

namespace Redecard.PN.Extrato.Agentes
{
    /// <summary>
    /// Classe Agentes para consultas WA no mainframe para o Resumo de Vendas do módulo Extrato
    /// </summary>
    public class ResumoVendasAG : AgentesBase
    {
        private static ResumoVendasAG _Instancia;
        public static ResumoVendasAG Instancia { get { return _Instancia ?? (_Instancia = new ResumoVendasAG()); } }

        public List<DebitoCVsAceitos> ConsultarDebitoCVsAceitos(
            Int32 pv,
            Int32 numeroRV,
            DateTime dataApresentacao,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Débito - CVs Aceitos - WACA668"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _canalChamador = "I";
                    String _tipoVenda = String.Empty;
                    String _programaChamador = "WACA668";
                    String _numeroPV = pv.ToString("D9");
                    String _numeroRV = numeroRV.ToString("D9");
                    String _dataApresentacao = dataApresentacao.ToString("yyyyMMdd");
                    String _msgRetorno = String.Empty;
                    String _quantidadeOcorrencias = "0";
                    String _programa = "WACA668";
                    String _programaRetornado = String.Empty;
                    String _totalTransacoesRegistradas = "0";
                    String _transacaoRegistrada = "0";
                    String _colunaInicial = "0";
                    WACA668_TB1_LINHA_RV[] _resumosVenda = new WACA668_TB1_LINHA_RV[200];
                    String _timestampResumoVenda = String.Empty;
                    String _codigoTransacao = "0";
                    String _codigoSubTransacao = "0";
                    String _codRetorno = "0";
                    String _chaveContinuacao = "N";
                    String _flagTemRegistro = String.Empty;

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _numeroTransacao = rechamada.GetValueOrDefault<String>("NumeroTransacao", String.Empty);

                    //Consulta serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            _canalChamador,
                            _tipoVenda,
                            _programaChamador,
                            _numeroPV,
                            _numeroRV,
                            _dataApresentacao,
                            _numeroTransacao,
                            _chaveContinuacao,
                            _flagTemRegistro,
                            _timestampResumoVenda,
                            _codigoTransacao,
                            _codigoSubTransacao,
                            _codRetorno,
                            _msgRetorno,
                            _quantidadeOcorrencias,
                            _programa,
                            _programaRetornado,
                            _totalTransacoesRegistradas,
                            _transacaoRegistrada,
                            _colunaInicial
                        });

                        contexto.Cliente.ConsultarDebitoCVsAceitos(
                            ref _canalChamador,
                            ref _tipoVenda,
                            ref _programaChamador,
                            ref _numeroPV,
                            ref _numeroRV,
                            ref _dataApresentacao,
                            ref _numeroTransacao,
                            ref _chaveContinuacao,
                            ref _flagTemRegistro,
                            ref _timestampResumoVenda,
                            ref _codigoTransacao,
                            ref _codigoSubTransacao,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _quantidadeOcorrencias,
                            ref _programa,
                            ref _programaRetornado,
                            ref _totalTransacoesRegistradas,
                            ref _transacaoRegistrada,
                            ref _colunaInicial,
                            out _resumosVenda);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            _canalChamador,
                            _tipoVenda,
                            _programaChamador,
                            _numeroPV,
                            _numeroRV,
                            _dataApresentacao,
                            _numeroTransacao,
                            _chaveContinuacao,
                            _flagTemRegistro,
                            _timestampResumoVenda,
                            _codigoTransacao,
                            _codigoSubTransacao,
                            _codRetorno,
                            _msgRetorno,
                            _quantidadeOcorrencias,
                            _programa,
                            _programaRetornado,
                            _totalTransacoesRegistradas,
                            _transacaoRegistrada,
                            _colunaInicial,
                            _resumosVenda
                        });
                    }

                    //Preparação dos dados de retorno do mainframe                                        
                    rechamada["NumeroTransacao"] = _numeroTransacao;

                    status = new StatusRetornoDTO(_codRetorno.ToInt16(0), _msgRetorno, FONTE_METODO);
                    indicadorRechamada = (_quantidadeOcorrencias.ToInt32(0) > 0 && _chaveContinuacao != "F");

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarDebitoCVsAceitosSaida(_resumosVenda, _quantidadeOcorrencias.ToInt32(0));
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
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Débito/CDC - Ajustes - WACA748"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _msgRetorno = default(String);
                    String _quantidadeAjustes = default(String);
                    WACA748_TAB_RET[] _ajustes = new WACA748_TAB_RET[100];
                    String _numeroPV = pv.ToString("D9");
                    String _resumo = resumo.ToString("D9");
                    String _dataApresentacao = dataApresentacao.ToString("yyyyMMdd");
                    String _codRetorno = default(String);

                    //Consulta serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            _numeroPV,
                            _resumo,
                            _dataApresentacao,
                            tipoResumo,
                            _codRetorno,
                            _msgRetorno,
                            _quantidadeAjustes
                        });

                        contexto.Cliente.ConsultarDebitoCDCAjustes(
                            ref _numeroPV,
                            ref _resumo,
                            ref _dataApresentacao,
                            ref tipoResumo,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _quantidadeAjustes,
                            out _ajustes);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            _numeroPV,
                            _resumo,
                            _dataApresentacao,
                            tipoResumo,
                            _codRetorno,
                            _msgRetorno,
                            _quantidadeAjustes,
                            _ajustes
                        });
                    }

                    status = new StatusRetornoDTO(_codRetorno.ToInt16(0), _msgRetorno, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarDebitoCDCAjusteSaida(_ajustes, _quantidadeAjustes.ToInt32(0));
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
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Crédito - CVs Aceitos - WACA706"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS                                        
                    String _numeroPV = pv.ToString("D9");
                    String _numeroRV = numeroRV.ToString("D9");
                    String _msgRetorno = String.Empty;
                    FILLER2 _dataApresentacao = new FILLER2
                    {
                        WACA706_DT_APRE_ANO = dataApresentacao.ToString("yyyy"),
                        WACA706_DT_APRE_MES = dataApresentacao.ToString("MM"),
                        WACA706_DT_APRE_DIA = dataApresentacao.ToString("dd")
                    };
                    WACA706_TB1_LINHA_RV[] _comprovantes = new WACA706_TB1_LINHA_RV[240];
                    String _codRetorno = default(String);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _chaveContinuacao = rechamada.GetValueOrDefault<String>("ChaveContinuacao", "".PadLeft(26, ' '));
                    String _numeroMes = rechamada.GetValueOrDefault<String>("NumeroMes", numeroMes.ToString("D2"));
                    String _flagTemRegistro = rechamada.GetValueOrDefault<String>("FlagTemRegistro", String.Empty);
                    String _quantidadeOcorrencias = rechamada.GetValueOrDefault<String>("QuantidadeOcorrencias", "0");
                    String _quantidadeResumoVenda = rechamada.GetValueOrDefault<String>("QuantidadeResumoVenda", "0");
                    String _programa = rechamada.GetValueOrDefault<String>("Programa", String.Empty);
                    String _programaRetornado = rechamada.GetValueOrDefault<String>("ProgramaRetornado", String.Empty);
                    String _totalTransacoesRegistradas = rechamada.GetValueOrDefault<String>("TotalTransacoesRegistradas", "0");
                    String _transacaoRegistrada = rechamada.GetValueOrDefault<String>("TransacaoRegistrada", "0");
                    String _colunaInicial = rechamada.GetValueOrDefault<String>("ColunaInicial", "0");
                    String _tipoMoeda = rechamada.GetValueOrDefault<String>("TipoMoeda", String.Empty);

                    //Consulta serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            timestamp,
                            tipoResumoVenda,
                            _numeroPV,
                            _numeroRV,
                            _dataApresentacao,
                            _chaveContinuacao,
                            _numeroMes,
                            _codRetorno,
                            _msgRetorno,
                            _flagTemRegistro,
                            _quantidadeOcorrencias,
                            _quantidadeResumoVenda,
                            _programa,
                            _programaRetornado,
                            _totalTransacoesRegistradas,
                            _transacaoRegistrada,
                            _colunaInicial,
                            _tipoMoeda
                        });

                        contexto.Cliente.ConsultarCreditoCVsAceitos(
                            ref timestamp,
                            ref tipoResumoVenda,
                            ref _numeroPV,
                            ref _numeroRV,
                            _dataApresentacao,
                            ref _chaveContinuacao,
                            ref _numeroMes,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _flagTemRegistro,
                            ref _quantidadeOcorrencias,
                            ref _quantidadeResumoVenda,
                            ref _programa,
                            ref _programaRetornado,
                            ref _totalTransacoesRegistradas,
                            ref _transacaoRegistrada,
                            ref _colunaInicial,
                            ref _tipoMoeda,
                            out _comprovantes);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            timestamp,
                            tipoResumoVenda,
                            _numeroPV,
                            _numeroRV,
                            _dataApresentacao,
                            _chaveContinuacao,
                            _numeroMes,
                            _codRetorno,
                            _msgRetorno,
                            _flagTemRegistro,
                            _quantidadeOcorrencias,
                            _quantidadeResumoVenda,
                            _programa,
                            _programaRetornado,
                            _totalTransacoesRegistradas,
                            _transacaoRegistrada,
                            _colunaInicial,
                            _tipoMoeda,
                            _comprovantes
                        });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["ChaveContinuacao"] = _chaveContinuacao;
                    rechamada["NumeroMes"] = _numeroMes;
                    rechamada["FlagTemRegistro"] = _flagTemRegistro;
                    rechamada["QuantidadeOcorrencias"] = _quantidadeOcorrencias;
                    rechamada["QuantidadeResumoVenda"] = _quantidadeResumoVenda;
                    rechamada["Programa"] = _programa;
                    rechamada["ProgramaRetornado"] = _programaRetornado;
                    rechamada["TotalTransacoesRegistradas"] = _totalTransacoesRegistradas;
                    rechamada["TransacaoRegistrada"] = _transacaoRegistrada;
                    rechamada["ColunaInicial"] = _colunaInicial;
                    rechamada["TipoMoeda"] = _tipoMoeda;

                    status = new StatusRetornoDTO(_codRetorno.ToInt16(0), _msgRetorno, FONTE_METODO);
                    indicadorRechamada = !String.IsNullOrEmpty(_chaveContinuacao.Trim());

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarCreditoCVsAceitosSaida(_comprovantes, _quantidadeOcorrencias.ToInt32(0));
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
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Crédito - Ajustes - WACA704"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS                    
                    String _numeroPV = pv.ToString("D9");
                    String _tipoRV = tipoRV.ToString();
                    String _numeroRV = numeroRV.ToString("D9");
                    FILLER _dataApresentacao = new FILLER
                    {
                        WACA704_DT_APRE_ANO = dataApresentacao.ToString("yyyy"),
                        WACA704_DT_APRE_DIA = dataApresentacao.ToString("dd"),
                        WACA704_DT_APRE_MES = dataApresentacao.ToString("MM")
                    };
                    String _msgRetorno = default(String);
                    String _areaFixa = default(String);
                    String _codRetorno = default(String);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _chaveContinuacao = rechamada.GetValueOrDefault<String>("ChaveContinuacao", String.Empty);
                    String _quantidadeMensagensErro = rechamada.GetValueOrDefault<String>("QuantidadeMensagensErro", "0");
                    String _flagTemRegistro = rechamada.GetValueOrDefault<String>("FlagTemRegistro", String.Empty);
                    String _quantidadeOcorrencias = rechamada.GetValueOrDefault<String>("QuantidadeOcorrencias", "0");
                    String _quantidadeResumosVenda = rechamada.GetValueOrDefault<String>("QuantidadeResumosVenda", "0");
                    String _programa = rechamada.GetValueOrDefault<String>("Programa", String.Empty);
                    String _programaRetorno = rechamada.GetValueOrDefault<String>("ProgramaRetorno", String.Empty);
                    String _transacaoRegistrada = rechamada.GetValueOrDefault<String>("TransacaoRegistrada", "0");
                    String _totalTransacoesRegistradas = rechamada.GetValueOrDefault<String>("TotalTransacoesRegistradas", "0");
                    String _colunaInicial = rechamada.GetValueOrDefault<String>("ColunaInicial", "0");
                    String _contadorTela = rechamada.GetValueOrDefault<String>("ContadorTela", "0");

                    //Consulta serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            timestamp,
                            _numeroPV,
                            _tipoRV,
                            _numeroRV,
                            _dataApresentacao,
                            _chaveContinuacao,
                            _quantidadeMensagensErro,
                            _codRetorno,
                            _msgRetorno,
                            _flagTemRegistro,
                            _quantidadeOcorrencias,
                            _quantidadeResumosVenda,
                            _programa,
                            _programaRetorno,
                            _transacaoRegistrada,
                            _totalTransacoesRegistradas,
                            _colunaInicial,
                            _contadorTela
                        });

                        contexto.Cliente.ConsultarCreditoAjustes(
                            ref timestamp,
                            ref _numeroPV,
                            ref _tipoRV,
                            ref _numeroRV,
                            ref _dataApresentacao,
                            ref _chaveContinuacao,
                            ref _quantidadeMensagensErro,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _flagTemRegistro,
                            ref _quantidadeOcorrencias,
                            ref _quantidadeResumosVenda,
                            ref _programa,
                            ref _programaRetorno,
                            ref _transacaoRegistrada,
                            ref _totalTransacoesRegistradas,
                            ref _colunaInicial,
                            ref _contadorTela,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            timestamp,
                            _numeroPV,
                            _tipoRV,
                            _numeroRV,
                            _dataApresentacao,
                            _chaveContinuacao,
                            _quantidadeMensagensErro,
                            _codRetorno,
                            _msgRetorno,
                            _flagTemRegistro,
                            _quantidadeOcorrencias,
                            _quantidadeResumosVenda,
                            _programa,
                            _programaRetorno,
                            _transacaoRegistrada,
                            _totalTransacoesRegistradas,
                            _colunaInicial,
                            _contadorTela,
                            _areaFixa
                        });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["ChaveContinuacao"] = _chaveContinuacao;
                    rechamada["QuantidadeMensagensErro"] = _quantidadeMensagensErro;
                    rechamada["FlagTemRegistro"] = _flagTemRegistro;
                    rechamada["QuantidadeOcorrencias"] = _quantidadeOcorrencias;
                    rechamada["QuantidadeResumosVenda"] = _quantidadeResumosVenda;
                    rechamada["Programa"] = _programa;
                    rechamada["ProgramaRetorno"] = _programaRetorno;
                    rechamada["TransacaoRegistrada"] = _transacaoRegistrada;
                    rechamada["TotalTransacoesRegistradas"] = _totalTransacoesRegistradas;
                    rechamada["ColunaInicial"] = _colunaInicial;
                    rechamada["ContadorTela"] = _contadorTela;

                    status = new StatusRetornoDTO(_codRetorno.ToInt16(0), _msgRetorno, FONTE_METODO);

                    // [AGA] - Alteração da rechamada conforme solicitação do José Castro [Chamado #10324]
                    //indicadorRechamada = "S".Equals((_chaveContinuacao ?? String.Empty).Trim(), StringComparison.InvariantCultureIgnoreCase);
                    if (!object.ReferenceEquals(_chaveContinuacao, null))
                    {
                        _chaveContinuacao = _chaveContinuacao.Trim();
                        indicadorRechamada = !String.IsNullOrEmpty(_chaveContinuacao);
                    }
                    else
                    {
                        indicadorRechamada = false;
                    }


                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarCreditoAjustesSaida(_areaFixa, _quantidadeOcorrencias.ToInt32(0));
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
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Construcard - CVs Aceitos - WACA797"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS                                        
                    String _programaChamador = "WACA797";
                    String _numeroPV = pv.ToString("D9");
                    String _numeroRV = numeroRV.ToString("D9");
                    String _dataApresentacao = dataApresentacao.ToString("yyyyMMdd");
                    String _msgRetorno = String.Empty;
                    WACA797_TB1_LINHA_RV[] _comprovantes = new WACA797_TB1_LINHA_RV[240];
                    String _codRetorno = "0";

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _numeroTransacao = rechamada.GetValueOrDefault<String>("NumeroTranscao", "0");
                    String _chaveContinuacao = rechamada.GetValueOrDefault<String>("ChaveContinuacao", String.Empty);
                    String _flagTemRegistro = rechamada.GetValueOrDefault<String>("FlagTemRegistro", String.Empty);
                    String _codigoRetornoSql = rechamada.GetValueOrDefault<String>("CodigoRetornoSQL", "0");
                    String _quantidadeOcorrencias = rechamada.GetValueOrDefault<String>("QuantidadeOcorrencias", "0");
                    String _programa = rechamada.GetValueOrDefault<String>("Programa", String.Empty);
                    String _codigoDePgmRetornado = rechamada.GetValueOrDefault<String>("CodigoProgramaRetornado", String.Empty);
                    String _totalTransacoesRegistradas = rechamada.GetValueOrDefault<String>("TransacoesRegistradas", "0");
                    String _transacaoRegistrada = rechamada.GetValueOrDefault<String>("TransacaoRegistrada", "0");
                    String _colunaInicial = rechamada.GetValueOrDefault<String>("ColunaInicial", "0");

                    //Consulta serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            _programaChamador,
                            _numeroPV,
                            _numeroRV,
                            _dataApresentacao,
                            _numeroTransacao,
                            _flagContinua = _chaveContinuacao,
                            _flagTemRegistro,
                            _codRetorno,
                            _codigoRetornoSql,
                            _msgRetorno,
                            _quantidadeOcorrencias,
                            _programa,
                            _codigoDePgmRetornado,
                            _totalTransacoesRegistradas,
                            _transacaoRegistrada,
                            _colunaInicial
                        });

                        contexto.Cliente.ConsultarConstrucardCVsAceitos(
                            ref _programaChamador,
                            ref _numeroPV,
                            ref _numeroRV,
                            ref _dataApresentacao,
                            ref _numeroTransacao,
                            ref _chaveContinuacao,
                            ref _flagTemRegistro,
                            ref _codRetorno,
                            ref _codigoRetornoSql,
                            ref _msgRetorno,
                            ref _quantidadeOcorrencias,
                            ref _programa,
                            ref _codigoDePgmRetornado,
                            ref _totalTransacoesRegistradas,
                            ref _transacaoRegistrada,
                            ref _colunaInicial,
                            out _comprovantes);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            _programaChamador,
                            _numeroPV,
                            _numeroRV,
                            _dataApresentacao,
                            _numeroTransacao,
                            _chaveContinuacao,
                            _flagTemRegistro,
                            _codRetorno,
                            _codigoRetornoSql,
                            _msgRetorno,
                            _quantidadeOcorrencias,
                            _programa,
                            _codigoDePgmRetornado,
                            _totalTransacoesRegistradas,
                            _transacaoRegistrada,
                            _colunaInicial,
                            _comprovantes
                        });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["NumeroTranscao"] = _numeroTransacao;
                    rechamada["ChaveContinuacao"] = _chaveContinuacao;
                    rechamada["FlagTemRegistro"] = _flagTemRegistro;
                    rechamada["CodigoRetornoSQL"] = _codigoRetornoSql;
                    rechamada["QuantidadeOcorrencias"] = _quantidadeOcorrencias;
                    rechamada["Programa"] = _programa;
                    rechamada["CodigoProgramaRetornado"] = _codigoDePgmRetornado;
                    rechamada["TransacoesRegistradas"] = _totalTransacoesRegistradas;
                    rechamada["TransacaoRegistrada"] = _transacaoRegistrada;
                    rechamada["ColunaInicial"] = _colunaInicial;

                    status = new StatusRetornoDTO(_codRetorno.ToInt16(0), _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_chaveContinuacao ?? String.Empty).Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarConstrucardCVsAceitos(_comprovantes, _quantidadeOcorrencias.ToInt32(0));
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
        /// <param name="origemPesquisaResumo">Tipo/Origem da pesquisa</param>
        /// <param name="status">Status retorno da consulta</param>
        /// <returns>Totais do Resumo de Vendas de Recarga de Celular</returns>
        public RecargaCelularResumo ConsultarRecargaCelularResumo(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            RecargaCelularResumoOrigem origemPesquisaResumo,
            out StatusRetornoDTO status)
        {
            String fonteMetodo = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger log = Logger.IniciarLog("Resumo de Vendas - Recarga de Celular - Resumo - BKWA2430"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS                                        
                    String programaChamador = "WA243";
                    String sistema = "IS";
                    String usuario = "xxx";
                    String dataDoPagamento = dataPagamento.ToString("dd.MM.yyyy");
                    var resto = default(String);
                    var origemPesquisa = (Int16)origemPesquisaResumo;

                    //Variáveis para retorno de chamada HIS
                    var resto1 = default(String);
                    var msgRetorno = String.Empty;
                    var codRetorno = default(Int16);
                    var codErro = default(Int16);
                    var dataReferencia = default(Int32);
                    var dataProcessamento = default(String);
                    var qtdTransacoes = default(Int32);
                    var valorTransacoes = default(Decimal);
                    var valorDesconto = default(Decimal);
                    var valorComissao = default(Decimal);

                    //Consulta serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            programaChamador,
                            sistema,
                            usuario,
                            numeroRv,
                            numeroPv,
                            dataDoPagamento,
                            resto,
                            origemPesquisa
                        });

#if !DEBUG
                        contexto.Cliente.ConsultarRecargaCelularResumo(
                            ref programaChamador,
                            ref sistema,
                            ref usuario,
                            ref numeroRv,
                            ref numeroPv,
                            ref dataDoPagamento,
                            ref origemPesquisa,
                            ref resto,
                            ref codRetorno,
                            ref msgRetorno,
                            ref codErro,
                            ref dataReferencia,
                            ref dataProcessamento,
                            ref qtdTransacoes,
                            ref valorTransacoes,
                            ref valorDesconto,
                            ref valorComissao,
                            ref resto1);
#else
                        codRetorno = 0;
                        msgRetorno = "PESQUISA REALIZADA COM SUCESSO";
                        codErro = 0;
                        dataReferencia = 052014;
                        dataProcessamento = "23.06.2014";
                        qtdTransacoes = 1;
                        valorTransacoes = 4213.23m;
                        valorDesconto = 42.63m;
                        valorComissao = 56.22m;
#endif

                        log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            msgRetorno,
                            codRetorno,
                            dataReferencia,
                            codErro,
                            dataProcessamento,
                            qtdTransacoes,
                            valorTransacoes,
                            valorDesconto,
                            valorComissao,
                            resto1
                        });
                    }

                    status = new StatusRetornoDTO(codRetorno, msgRetorno, fonteMetodo);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (codRetorno != 0)
                        return null;
                    else
                    {
                        //Preparação dos dados de retorno do mainframe
                        return new RecargaCelularResumo
                        {
                            DataProcessamento = dataProcessamento.ToDateTimeNull("dd.MM.yyyy"),
                            DataReferencia = dataReferencia.ToString("D6").ToDateTimeNull("MMyyyy"),
                            QuantidadeTransacao = qtdTransacoes,
                            ValorTotalComissao = valorComissao,
                            ValorTotalDesconto = valorDesconto,
                            ValorTotalTransacao = valorTransacoes
                        };
                    }
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
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Recarga de Celular - Vencimentos - BKWA2440"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS                                        
                    String _programaChamador = "WA244";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataPagamento = dataPagamento.ToString("dd.MM.yyyy");
                    var _resto = default(String);

                    //Variáveis para retorno de chamada HIS
                    var _dataPagamentoRetorno = default(String);
                    var _dataAntecipacao = default(String);
                    var _numeroOc = default(Decimal);
                    var _statusComissao = default(String);
                    var _valorLiquido = default(Decimal);
                    var _resto1 = default(String);
                    var _msgRetorno = String.Empty;
                    var _codRetorno = default(Int16);
                    var _codErro = default(Int16);

                    //Consulta serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            _programaChamador,
                            _sistema,
                            _usuario,
                            numeroRv,
                            numeroPv,
                            _dataPagamento,
                            _resto
                        });

#if !DEBUG
                        contexto.Cliente.ConsultarRecargaCelularVencimentos(                            
                            ref _programaChamador,
                            ref _sistema,
                            ref _usuario,
                            ref numeroRv,
                            ref numeroPv,
                            ref _dataPagamento,
                            ref _resto,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _codErro,                           
                            ref _dataPagamentoRetorno,
                            ref _dataAntecipacao,
                            ref _numeroOc,
                            ref _statusComissao,
                            ref _valorLiquido,
                            ref _resto1);
#else
                        _codRetorno = 0;
                        _msgRetorno = "PESQUISA REALIZADA COM SUCESSO";
                        _codErro = 0;
                        _dataPagamento = "12.07.2013";
                        _dataAntecipacao = "28.12.2014";
                        _numeroOc = 584;
                        _statusComissao = "Status comissão";
                        _valorLiquido = 8734.23m;
#endif

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            _msgRetorno,
                            _codErro,
                            _dataPagamentoRetorno,
                            _dataAntecipacao,
                            _numeroOc,
                            _statusComissao,
                            _valorLiquido,
                            _resto1
                        });
                    }

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (_codRetorno != 0)
                        return null;
                    else
                    {
                        //Preparação dos dados de retorno do mainframe
                        return new RecargaCelularVencimento
                        {
                            DataAntecipacao = _dataAntecipacao.ToDateTimeNull("dd.MM.yyyy"),
                            DataPagamento = _dataPagamentoRetorno.ToDateTimeNull("dd.MM.yyyy"),
                            NumeroOc = _numeroOc,
                            StatusComissao = _statusComissao,
                            ValorLiquido = _valorLiquido
                        };
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<RecargaCelularAjuste> ConsultarRecargaCelularAjustes(
            Int16 tipoPesquisa,
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Recarga de Celular - Ajustes - BKWA2450"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS                                        
                    String _programaChamador = "WA245";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataPagamento = dataPagamento.ToString("dd.MM.yyyy");
                    var _resto = default(String);
                    var _codRetorno = default(Int16);
                    var _msgRetorno = default(String);
                    var _codErro = default(Int16);
                    var _qtdOcorrencias = default(Int32);
                    var _registros = new WA245S_DETALHE[250];
                    var _resto1 = default(String);
                    var _tipoPesquisa = tipoPesquisa;

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador", String.Empty);
                    String _rechamadaTmsAjs = rechamada.GetValueOrDefault<String>("TmsAjs", String.Empty);

                    //Consulta serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            _programaChamador,
                            _sistema,
                            _usuario,
                            _tipoPesquisa,
                            numeroRv,
                            numeroPv,
                            _dataPagamento,
                            _rechamadaIndicador,
                            _rechamadaTmsAjs,
                            _resto
                        });

#if !DEBUG
                        contexto.Cliente.ConsultarRecargaCelularAjustes(
                            ref _programaChamador, 
                            ref _sistema,
                            ref _usuario,
                            ref _tipoPesquisa,
                            ref numeroRv,
                            ref numeroPv,
                            ref _dataPagamento,
                            ref _rechamadaIndicador,
                            ref _rechamadaTmsAjs,
                            ref _resto,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _codErro,
                            ref _qtdOcorrencias,
                            ref _registros,
                            ref _resto1);
#else
                        _programaChamador = "WA245";
                        _sistema = "IS";
                        _usuario = "xxx";
                        _tipoPesquisa = 1;
                        numeroRv = 123;
                        numeroPv = 123;
                        _dataPagamento = "20.05.2014";
                        _rechamadaIndicador = "N";
                        _rechamadaTmsAjs = "0";
                        _codRetorno = 0;
                        _msgRetorno = "PESQUISA EFETUADA COM SUCESSO";
                        _codErro = 0;
                        _qtdOcorrencias = 1;
                        _registros = new WA245S_DETALHE[] {
                            new WA245S_DETALHE { 
                               WA245S_COD_AJS = 123,
                               WA245S_DAT_REC = "20.05.2015",
                               WA245S_DAT_REF = "10.2020",
                               WA245S_DES_AJS = "Descr. Ajuste",
                               WA245S_DES_ORIG = "Descr. Origem",
                               WA245S_NUM_PV_AJ = 1250191,
                               WA245S_VAL_AJU = 3539.23m,
                               WA245S_VAL_VDA = 944.22m
                            },
                            new WA245S_DETALHE {
                               WA245S_COD_AJS = 123,
                               WA245S_DAT_REC = "20.05.2015",
                               WA245S_DAT_REF = "10.2020",
                               WA245S_DES_AJS = "Descr. Ajuste",
                               WA245S_DES_ORIG = "Descr. Origem",
                               WA245S_NUM_PV_AJ = 1250191,
                               WA245S_VAL_AJU = 3539.23m,
                               WA245S_VAL_VDA = 944.22m
                            }
                        };
#endif

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            _programaChamador,
                            _sistema,
                            _usuario,
                            _tipoPesquisa,
                            numeroRv,
                            numeroPv,
                            _dataPagamento,
                            _rechamadaIndicador,
                            _rechamadaTmsAjs,
                            _resto,
                            _codRetorno,
                            _msgRetorno,
                            _codErro,
                            _qtdOcorrencias,
                            _registros,
                            _resto1
                        });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["Indicador"] = _rechamadaIndicador;
                    rechamada["TmsAjs"] = _rechamadaTmsAjs;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = String.Compare("S", _rechamadaIndicador, true) == 0;

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarRecargaCelularAjustes(_registros, _qtdOcorrencias, tipoPesquisa);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<RecargaCelularComprovante> ConsultarRecargaCelularComprovantes(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Recarga de Celular - Comprovantes - BKWA2460"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS   
                    String _programa = "WA246";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataPagamento = dataPagamento.ToString("dd.MM.yyyy");
                    var _resto = default(String);
                    var _codRetorno = default(Int16);
                    var _msgRetorno = default(String);
                    var _codErro = default(Int16);
                    var _qtdOcorrencias = default(Int32);
                    var _registros = new WA246S_DETALHE[250];
                    var _resto1 = default(String);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador", String.Empty);
                    String _rechamadaDatTran = rechamada.GetValueOrDefault<String>("DatTran", String.Empty);
                    String _rechamadaHorTran = rechamada.GetValueOrDefault<String>("HorTran", String.Empty);
                    Decimal _rechamadaNumServ = rechamada.GetValueOrDefault<Decimal>("NumServ");

                    //Consulta serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            _programa,
                            _sistema,
                            _usuario,
                            numeroRv,
                            numeroPv,
                            _dataPagamento,
                            _rechamadaIndicador,
                            _rechamadaDatTran,
                            _rechamadaHorTran,
                            _rechamadaNumServ,
                            _resto
                        });

#if !DEBUG
                        contexto.Cliente.ConsultarRecargaCelularComprovantes(
                            ref _programa,
                            ref _sistema,
                            ref _usuario,
                            ref numeroRv,
                            ref numeroPv,
                            ref _dataPagamento,
                            ref _rechamadaIndicador,
                            ref _rechamadaDatTran,
                            ref _rechamadaHorTran,
                            ref _rechamadaNumServ,
                            ref _resto,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _codErro,
                            ref _qtdOcorrencias,
                            ref _registros,
                            ref _resto1);
#else
                        _programa = "WA246";
                        _sistema = "IS";
                        _usuario = "xxx";
                        numeroRv = 5123;
                        numeroPv = 1250191;
                        _dataPagamento = "20.03.2014";
                        _rechamadaIndicador = "N";
                        _rechamadaDatTran = "0";
                        _rechamadaHorTran = "0";
                        _rechamadaNumServ = 0;
                        _codRetorno = 0;
                        _msgRetorno = "PESQUISA REALIZADA COM SUCESSO";
                        _codErro = 0;
                        _qtdOcorrencias = 91;
                        {
                            var _registrosList = new List<WA246S_DETALHE>();
                            for (Int32 i = 0; i < _qtdOcorrencias; i++)
                                _registrosList.Add(
                                     new WA246S_DETALHE
                                     {
                                         WA246S_DAT_TRAN = "06.09.2012",
                                         WA246S_HOR_TRAN = "12:51:22",
                                         WA246S_NUM_CEL = "(11) 99932-1234",
                                         WA246S_DSC_OPER = "VIVO",
                                         WA246S_NUM_TRAN = i + 1,
                                         WA246S_STA_TRAN = "St. Tran.",
                                         WA246S_VAL_CMSS = 983.21m,
                                         WA246S_VAL_TRAN = 9283.51m
                                     });
                            _registros = _registrosList.ToArray();
                        }
#endif

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            _programa,
                            _sistema,
                            _usuario,
                            numeroRv,
                            numeroPv,
                            _dataPagamento,
                            _rechamadaIndicador,
                            _rechamadaDatTran,
                            _rechamadaHorTran,
                            _rechamadaNumServ,
                            _resto,
                            _codRetorno,
                            _msgRetorno,
                            _codErro,
                            _qtdOcorrencias,
                            _registros,
                            _resto1
                        });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["Indicador"] = _rechamadaIndicador;
                    rechamada["DatTran"] = _rechamadaDatTran;
                    rechamada["HorTran"] = _rechamadaHorTran;
                    rechamada["NumServ"] = _rechamadaNumServ;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = String.Compare("S", _rechamadaIndicador, true) == 0;

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarRecargaCelularComprovantes(_registros, _qtdOcorrencias);
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