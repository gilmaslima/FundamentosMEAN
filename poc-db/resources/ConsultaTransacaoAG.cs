using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.MEExtratoConsultaTransacao;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.ConsultaTransacaoTR;
using System.Reflection;
using Redecard.PN.Extrato.Modelo.ConsultaTransacao;

namespace Redecard.PN.Extrato.Agentes
{
    public class ConsultaTransacaoAG : AgentesBase
    {
        private static ConsultaTransacaoAG _Instancia;
        public static ConsultaTransacaoAG Instancia { get { return _Instancia ?? (_Instancia = new ConsultaTransacaoAG()); } }

        public List<Debito> ConsultarDebito(
            Int32 numeroPV,
            DateTime dataInicial,
            DateTime dataFinal,
            String numeroCartao, //obrigatório se NSU não informado
            Int64 nsuAcquirer, //obrigatório se cartão não informado
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Dados - Débito - MEC084CO"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "IS89";
                    String _dataInicial = dataInicial.ToString("yyyyMMdd");
                    String _dataFinal = dataFinal.ToString("yyyyMMdd");
                    String _nsuAcquirer = nsuAcquirer.ToString("D13");
                    String _numeroPV = numeroPV.ToString("D9");                    
                    String _areaFixa = default(String);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaNsuAcquirer = rechamada.GetValueOrDefault<String>("NsuAcquirer");
                    String _rechamadaDataTransacao = rechamada.GetValueOrDefault<String>("DataTransacao");
                    String _rechamadaTipoDebito = rechamada.GetValueOrDefault<String>("TipoDebito");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Declaração de variáveis de saída            
                    String _nomeServico = default(String);
                    String _codigoRetorno = default(String);
                    String _codigoErroSQL = default(String);
                    String _msgRetorno = default(String);
                    String _qtdRegistros = default(String);
                    String _indicadorTokenizacao = default(String);

                    using (COMTIMEClient client = new COMTIMEClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            _nomePrograma, numeroPV, _dataInicial, _dataFinal, numeroCartao, _nsuAcquirer, _rechamadaIndicador,
                            _rechamadaNsuAcquirer, _rechamadaDataTransacao, _rechamadaTipoDebito, _nomeServico, _codigoRetorno,
                            _codigoErroSQL, _msgRetorno, _qtdRegistros, _areaFixa, _indicadorTokenizacao });

                        client.ConsultarDebito(
                            ref _nomePrograma,
                            ref _numeroPV,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref numeroCartao,
                            ref _nsuAcquirer,
                            ref _rechamadaIndicador,
                            ref _rechamadaNsuAcquirer,
                            ref _rechamadaDataTransacao,
                            ref _rechamadaTipoDebito,
                            ref _nomeServico,
                            ref _codigoRetorno,
                            ref _codigoErroSQL,
                            ref _msgRetorno,
                            ref _qtdRegistros,
                            out _areaFixa,
                            ref _indicadorTokenizacao);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            _nomePrograma, numeroPV, _dataInicial, _dataFinal, numeroCartao, _nsuAcquirer, _rechamadaIndicador,
                            _rechamadaNsuAcquirer, _rechamadaDataTransacao, _rechamadaTipoDebito, _nomeServico, _codigoRetorno,
                            _codigoErroSQL, _msgRetorno, _qtdRegistros, _areaFixa, _indicadorTokenizacao
                        });
                    }

                    //Preparação dos dados retornados
                    rechamada["NsuAcquirer"] = _rechamadaNsuAcquirer;
                    rechamada["DataTransacao"] = _rechamadaDataTransacao;
                    rechamada["TipoDebito"] = _rechamadaTipoDebito;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codigoRetorno.ToInt16(0), _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarDebitoSaida(_areaFixa, _qtdRegistros.ToInt32(0), _indicadorTokenizacao);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE);
                }
            }
        }

        public List<Credito> ConsultarCredito(
            Int32 numeroPV,
            DateTime dataInicial,
            DateTime dataFinal,
            String numeroCartao, //obrigatório se NSU não informado
            Int64 nsu, //obrigatório se cartão não informado ou para Komerci
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Dados - Crédito - MEC119CO"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "IS89";
                    String _dataInicial = dataInicial.ToString("dd.MM.yyyy");
                    String _dataFinal = dataFinal.ToString("dd.MM.yyyy");
                    String _numeroPV = numeroPV.ToString("D9");                    
                    String _areaFixa = default(String);
                    String _nsu = nsu.ToString("D12");

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaDataTransacao = rechamada.GetValueOrDefault<String>("DataTransacao");
                    String _rechamadaTimestampTransacao = rechamada.GetValueOrDefault<String>("TimestampTransacao");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Declaração de variáveis de saída
                    String _nomeServico = default(String);
                    String _codigoRetorno = default(String);
                    String _codigoErroSQL = default(String);
                    String _msgRetorno = default(String);
                    String _qtdRegistros = default(String);
                    String _indicadorTokenizacao = default(String);

                    using (COMTIMEClient client = new COMTIMEClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            _nomePrograma, numeroPV, _dataInicial, _dataFinal, numeroCartao, _nsu, _rechamadaIndicador,
                            _rechamadaDataTransacao, _rechamadaTimestampTransacao, _nomeServico, _codigoRetorno,
                            _codigoErroSQL, _msgRetorno, _qtdRegistros, _areaFixa, _indicadorTokenizacao
                        });

                        client.ConsultarCredito(
                            ref _nomePrograma,
                            ref _numeroPV,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref numeroCartao,
                            ref _nsu,
                            ref _rechamadaIndicador,
                            ref _rechamadaDataTransacao,
                            ref _rechamadaTimestampTransacao,
                            ref _nomeServico,
                            ref _codigoRetorno,
                            ref _codigoErroSQL,
                            ref _msgRetorno,
                            ref _qtdRegistros,
                            out _areaFixa,
                            ref _indicadorTokenizacao);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            _nomePrograma, numeroPV, _dataInicial, _dataFinal, numeroCartao, _nsu, _rechamadaIndicador,
                            _rechamadaDataTransacao, _rechamadaTimestampTransacao, _nomeServico, _codigoRetorno,
                            _codigoErroSQL, _msgRetorno, _qtdRegistros, _areaFixa, _indicadorTokenizacao
                        });
                    }

                    //Preparação dos dados retornados
                    rechamada["DataTransacao"] = _rechamadaDataTransacao;
                    rechamada["TimestampTransacao"] = _rechamadaTimestampTransacao;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codigoRetorno.ToInt16(0), _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarCreditoSaida(_areaFixa, _qtdRegistros.ToInt32(0), _indicadorTokenizacao);                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE);
                }
            }
        }

        public DebitoTID ConsultarDebitoTID(
            String idDataCash,            
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Dados - Débito TID - MEC324CO"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "IS89";
                    
                    //Declaração de variáveis de saída
                    String _nomeServ = default(String);
                    String _codRetorno = default(String);
                    String _codErro = default(String);
                    String _dsMensagem = default(String);
                    String _numRV = default(String);
                    String _nuDatacash = default(String);
                    String _numCartao = default(String);
                    String _numTransacao = default(String);
                    String _numPV = default(String);
                    String _dtTransacao = default(String);
                    String _dtRV = default(String);
                    String _numParcelas = default(String);
                    String _valorTransacao = default(String);
                    String _numAutorizacao = default(String);
                    String _codProduto = default(String);
                    String _IdCancelamento = default(String);
                    String _timestampTransacao = default(String);
                    String _dsBandeira = default(String);
                    String _qntCancelamento = default(String);
                    String _areaFixa = default(String);
                    String _idDataCash = idDataCash.PadLeft(20, '0');
                    String _indicadorTokenizacao = default(String);

                    using (COMTIMEClient client = new COMTIMEClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            _nomePrograma, _idDataCash, _nomeServ, _codRetorno, _codErro, _dsMensagem, _numRV, _nuDatacash, 
                            _numCartao, _numTransacao, _numPV, _dtTransacao, _dtRV, _numParcelas, _valorTransacao, _numAutorizacao, 
                            _codProduto, _IdCancelamento, _timestampTransacao, _dsBandeira, _qntCancelamento, _areaFixa, _indicadorTokenizacao
                        });

                        client.ConsultarDebitoTID(
                            ref _nomePrograma,
                            ref _idDataCash,                            
                            ref _nomeServ,
                            ref _codRetorno,
                            ref _codErro,
                            ref _dsMensagem,
                            ref _numRV,
                            ref _nuDatacash,
                            ref _numCartao,
                            ref _numTransacao,
                            ref _numPV,
                            ref _dtTransacao,
                            ref _dtRV,
                            ref _numParcelas,
                            ref _valorTransacao,
                            ref _numAutorizacao,
                            ref _codProduto,
                            ref _IdCancelamento,
                            ref _timestampTransacao,
                            ref _dsBandeira,
                            ref _qntCancelamento,
                            out _areaFixa,
                            ref _indicadorTokenizacao);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            _nomePrograma, _idDataCash, _nomeServ, _codRetorno, _codErro, _dsMensagem, _numRV, _nuDatacash,
                            _numCartao, _numTransacao, _numPV, _dtTransacao, _dtRV, _numParcelas, _valorTransacao, _numAutorizacao,
                            _codProduto, _IdCancelamento, _timestampTransacao, _dsBandeira, _qntCancelamento, _areaFixa, _indicadorTokenizacao
                        });
                    }

                    //Preparação dos dados retornados
                    status = new StatusRetornoDTO(_codRetorno.ToInt16(0), _dsMensagem, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    DebitoTID dados = new DebitoTID();
                    dados.Bandeira = _dsBandeira;
                    dados.CodigoProdutoVenda = _codProduto;
                    dados.DataResumo = _dtRV.ToString().ToDate("yyyyMMdd");
                    dados.DataTransacao = _dtTransacao.ToString().ToDate("yyyyMMdd");
                    dados.IdCancelamento = _IdCancelamento.ToInt16(0);
                    dados.NumeroAutorizacaoBanco = _numAutorizacao;
                    dados.NumeroCartao = _numCartao;
                    dados.NsuAquirer = new CortadorMensagem(_numTransacao).LerDecimal(13, 0);
                    dados.NumeroParcelas = _numParcelas.ToInt16(0);
                    dados.NumeroPV = _numPV.ToInt32(0);
                    dados.QuantidadeCancelamento = _qntCancelamento.ToInt16(0);
                    dados.NumeroResumoVendas = _numRV.ToInt32(0);
                    dados.NumeroIdDataCash = _nuDatacash;
                    dados.TimestampTransacao = _timestampTransacao;
                    dados.ValorTransacao = new CortadorMensagem(_valorTransacao).LerDecimal(11, 2);
                    dados.Cancelamentos = TR.ConsultarDebitoTIDSaida(_areaFixa, _qntCancelamento.ToInt32(0));
                    dados.indicadorTokenizacao = _indicadorTokenizacao;


                    switch ((dados.CodigoProdutoVenda ?? "").Trim().ToUpper())
                    {
                        case "MLO": dados.DescricaoProdutoVenda = "MAESTRO LOCAL"; break;
                        case "PMA": dados.DescricaoProdutoVenda = "PARCELE MAIS"; break;
                        case "CDC": dados.DescricaoProdutoVenda = "CDC"; break;
                        case "TRI": dados.DescricaoProdutoVenda = "TRISHOP"; break;
                        case "MRE": dados.DescricaoProdutoVenda = "MAESTRO RECEPTIVE"; break;
                        case "C&S": dados.DescricaoProdutoVenda = "COMPRE E SAQUE"; break;
                        case "CST": dados.DescricaoProdutoVenda = "CONSTRUCARD"; break;
                        case "DIS": dados.DescricaoProdutoVenda = "DISTRIBUTION"; break;
                        case "AVI": dados.DescricaoProdutoVenda = "A VISTA"; break;
                        case "PRE": dados.DescricaoProdutoVenda = "PRE-DATADO"; break;
                    }

                    if (Enum.IsDefined(typeof(DebitoTidTipoCancelamento), (Int32)dados.IdCancelamento))
                        dados.TipoCancelamento = (DebitoTidTipoCancelamento)dados.IdCancelamento;
                    else dados.TipoCancelamento = DebitoTidTipoCancelamento.NaoIdentificado;

                    return dados;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE);
                }
            }
        }

        public CreditoTID ConsultarCreditoTID(
            String idDataCash,            
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Dados - Crédito TID - MEC323CO"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "IS89";
                    
                    //Declaração de variáveis de saída
                    String _nomeProgramaExecucao = default(String);
                    String _codigoRetorno = default(String);
                    String _codigoErroSQL = default(String);
                    String _msgRetorno = default(String);
                    String _numeroIdDatacash = default(String);
                    String _numeroPV = default(String);
                    String _numeroCartao = default(String);
                    String _dataTransacao = default(String);
                    String _valorTransacao = default(String);
                    String _autorizacaoVenda = default(String);
                    String _numeroResumoVenda = default(String);
                    String _quantidadeParcelas = default(String);
                    String _codigoProdutoVenda = default(String);
                    String _idCancelamento = default(String);
                    String _dataResumo = default(String);
                    String _timestampTransacao = default(String);
                    String _descricaoBandeira = default(String);
                    String _quantidadeCancelamento = default(String);
                    String _areaFixa = default(String);
                    String _idDataCash = idDataCash.PadLeft(20, '0');
                    string _indicadorTokenizacao = default(String);

                    using (COMTIMEClient client = new COMTIMEClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            _nomePrograma, _idDataCash, _nomeProgramaExecucao, _codigoRetorno, _codigoErroSQL, _msgRetorno, 
                            _numeroIdDatacash, _numeroPV, _numeroCartao, _dataTransacao, _valorTransacao, _autorizacaoVenda,
                            _numeroResumoVenda, _quantidadeParcelas, _codigoProdutoVenda, _idCancelamento, _dataResumo,
                            _timestampTransacao, _descricaoBandeira, _quantidadeCancelamento, _areaFixa, _indicadorTokenizacao
                        });

                        client.ConsultarCreditoTID(
                            ref _nomePrograma,
                            ref _idDataCash,                            
                            ref _nomeProgramaExecucao,
                            ref _codigoRetorno,
                            ref _codigoErroSQL,
                            ref _msgRetorno,
                            ref _numeroIdDatacash,
                            ref _numeroPV,
                            ref _numeroCartao,
                            ref _dataTransacao,
                            ref _valorTransacao,
                            ref _autorizacaoVenda,
                            ref _numeroResumoVenda,
                            ref _quantidadeParcelas,
                            ref _codigoProdutoVenda,
                            ref _idCancelamento,
                            ref _dataResumo,
                            ref _timestampTransacao,
                            ref _descricaoBandeira,
                            ref _quantidadeCancelamento,
                            out _areaFixa,
                            ref _indicadorTokenizacao);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            _nomePrograma, _idDataCash, _nomeProgramaExecucao, _codigoRetorno, _codigoErroSQL, _msgRetorno, 
                            _numeroIdDatacash, _numeroPV, _numeroCartao, _dataTransacao, _valorTransacao, _autorizacaoVenda, 
                            _numeroResumoVenda, _quantidadeParcelas, _codigoProdutoVenda, _idCancelamento, _dataResumo, 
                            _timestampTransacao, _descricaoBandeira, _quantidadeCancelamento, _areaFixa, _indicadorTokenizacao
                        });
                    }

                    //Preparação dos dados retornados
                    status = new StatusRetornoDTO(_codigoRetorno.ToInt16(), _msgRetorno, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    CreditoTID dados = new CreditoTID();
                    dados.NumeroIdDataCash = _numeroIdDatacash;
                    dados.NumeroPV = _numeroPV.ToInt32(0);
                    dados.AutorizacaoVenda = _autorizacaoVenda;
                    dados.Bandeira = _descricaoBandeira;
                    dados.CodigoProdutoVenda = _codigoProdutoVenda;
                    dados.DataResumo = _dataResumo.ToString().ToDate("yyyyMMdd");
                    dados.DataTransacao = _dataTransacao.ToString().ToDate("yyyyMMdd");
                    dados.IDCancelamento = _idCancelamento.ToInt16(0);
                    dados.NumeroCartao = _numeroCartao;
                    dados.QuantidadeParcelas = _quantidadeParcelas.ToInt16(0);
                    dados.QuantidadeCancelamento = _quantidadeCancelamento.ToInt16(0);
                    dados.NumeroResumoVendas = _numeroResumoVenda.ToInt32(0);
                    dados.TimestampTransacao = _timestampTransacao;
                    dados.ValorTransacao = new CortadorMensagem(_valorTransacao).LerDecimal(11, 2);
                    dados.Cancelamentos = TR.ConsultarCreditoTIDSaida(_areaFixa, _quantidadeCancelamento.ToInt32(0));
                    dados.IndicadorTokenizacao = _indicadorTokenizacao;

                    switch ((dados.CodigoProdutoVenda ?? "").Trim().ToUpper())
                    {
                        case "ROT": dados.DescricaoProdutoVenda = "ROTATIVO"; break;
                        case "PCJ": dados.DescricaoProdutoVenda = "PARCELADO COM JUROS"; break;
                        case "PSJ": dados.DescricaoProdutoVenda = "PARCELADO SEM JUROS"; break;
                    }

                    if (Enum.IsDefined(typeof(CreditoTidTipoCancelamento), (Int32)dados.IDCancelamento))
                        dados.TipoCancelamento = (CreditoTidTipoCancelamento)dados.IDCancelamento;
                    else dados.TipoCancelamento = CreditoTidTipoCancelamento.NaoIdentificado;

                    return dados;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE);
                }
            }
        }
    }
}
