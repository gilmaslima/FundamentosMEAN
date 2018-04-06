using System;
using System.Collections.Generic;
using System.Linq;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.ServicoConsultaOrdensCredito;
using Redecard.PN.Extrato.Agentes.Tradutores;
using Redecard.PN.Extrato.Comum.Helper;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes
{
    public class ConsultarOrdensCreditoAG : AgentesBase
    {
        #region WACA1106 - Relatório de pagamentos ajustados.
        /// <summary>
        /// WACA1106 - Relatório de pagamentos ajustados.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarOrdensCreditoEnviadosAoBancoRetornoDTO ConsultarOrdensCreditoEnviadosAoBanco(out StatusRetornoDTO statusRetornoDTO, ConsultarOrdensCreditoEnviadosAoBancoEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarOrdensCreditoEnviadosAoBanco";
            using (Logger Log = Logger.IniciarLog("Relatório de pagamentos ajustados [WACA1106]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                try
                {
                    ConsultarOrdensCreditoEnviadosAoBancoRetornoDTO result = null;

                    using (OrdensCredClient cliente = new OrdensCredClient())
                    {
                        string programaChamador = "WACA1106";
                        string sistema = "IS";
                        string usuario = "xxx";
                        string dataInicial = envio.DataInicial.ToString("dd/MM/yyyy");
                        string dataFinal = envio.DataFinal.ToString("dd/MM/yyyy");
                        string mensagemRetorno = string.Empty;
                        string indicadorReChamada = string.Empty;
                        short quantidadeTransacoes = 0;
                        short numeroTransacaoEnviada = 0;
                        string reservaDados = string.Empty;
                        string dados;

                        do
                        {
                            dados = (envio.Estabelecimentos.Count.ToString("0000") + 
                                    envio.CodigoBandeira.ToString("000") +                                           
                                    String.Join("", from i in envio.Estabelecimentos select i.ToString("000000000")).PadRight(27007, '0')).PadRight(30000, ' ');

                            Log.GravarLog(EventoLog.ChamadaHIS, new { programaChamador, sistema, usuario, dataInicial, dataFinal, mensagemRetorno, indicadorReChamada, quantidadeTransacoes, numeroTransacaoEnviada, reservaDados, dados});

                            short codigoRetorno;

                            if (!TesteHelper.IsAmbienteDesenvolvimento())
                            {
                                codigoRetorno = cliente.ConsultarAjustesACredito(programaChamador, sistema, ref usuario, dataInicial, dataFinal, ref mensagemRetorno, ref indicadorReChamada, ref quantidadeTransacoes, ref numeroTransacaoEnviada, ref reservaDados, ref dados);

                            }
                            else
                            {
                                codigoRetorno = 0;

                                dados = "000004" +
                                    "DT30.03.201204.04.2012000045039001084726MASTERCARD  01                  DEV CRED PGTO MAIOR                                         00000000000015921C104015043000011778067" +
                                    "DT30.03.201204.04.2012000352241001084287MASTERCARD  01                  DEV CRED PGTO MAIOR                                         00000000000000464C237000850002403390123" +
                                    "DT30.03.201204.04.2012001531336001085676MASTERCARD  01                  DEV CRED PGTO MAIOR                                         00000000000003900C341015800000001582067" +
                                    "TB00000000000000000000000000000000000000                                Total de Ajustes no Período                                 00000000000020285C00000000          " + 
                                    "                      000000000000000000                                                                                            00000000000000000 00000000                       000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000                                000000000000000000                                                                                            00000000000000000 00000000          0000000000000000020285";
                            }

                            Log.GravarLog(EventoLog.RetornoHIS, new { usuario, mensagemRetorno, indicadorReChamada, quantidadeTransacoes, numeroTransacaoEnviada, reservaDados, dados });

                            statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);

                            if (codigoRetorno != 0)
                            {
                                return null;
                            }

                            ConsultarOrdensCreditoEnviadosAoBancoRetornoDTO retornoDTO = TradutorResultadoConsultaOrdensCredito.TraduzirRetornoResultadoConsultarOrdensCreditoEnviadosAoBanco(dados);

                            if (result == null)
                            {
                                result = retornoDTO;
                            }
                            else
                            {
                                result.Registros.AddRange(retornoDTO.Registros);
                            }

                        } while (indicadorReChamada == "S");
                    }

                    Log.GravarLog(EventoLog.FimAgente, new { result });

                    return result;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion
        
        #region WACA703 - Resumo de vendas - Cartões de crédito - Vencimentos.
        public List<ConsultarDisponibilidadeVencimentosODCRetornoDTO> ConsultarDisponibilidadeVencimentosODC(out StatusRetornoDTO statusRetornoDTO, ConsultarDisponibilidadeVencimentosODCEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarDisponibilidadeVencimentosODC";
            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de crédito - Vencimentos [WACA703]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {
                    List<ConsultarDisponibilidadeVencimentosODCRetornoDTO> result = new List<ConsultarDisponibilidadeVencimentosODCRetornoDTO>();

                    using (OrdensCredClient cliente = new OrdensCredClient())
                    {
                        short quantidadeMensagensErro=default(short);
                        string mensagemRetorno=default(string);
                        string flagTemRegistro=default(string);
                        short quantidadeOcorrencias = 0;
                        int quantidadeResumosVenda=default(int);
                        string programa = default(string);
                        string programaRetorno = default(string);
                        short totalTransacoesRegistradas = default(short);
                        short totalRegistros = default(short);
                        short colunaInicial = default(short);
                        short contadorTela = default(short);
                        string indexadorsExistenciaReal = default(string);
                        ResumoVendaOrdemCredito[] resumosVenda = new ResumoVendaOrdemCredito[350];
                        string timestamp = envio.ChaveContinua;
                        int numeroEstabelecimento = envio.NumeroEstabelecimento;
                        int numeroResumoVenda = envio.NumeroResumoVenda;
                        string dataApresentacao = envio.DataApresentacao.ToString("ddMMyyyy");
                        string chaveContinua = string.Empty;

                        do
                        {
                            short codigoRetorno;

                            Log.GravarLog(EventoLog.ChamadaHIS, new { timestamp, numeroEstabelecimento, numeroResumoVenda, dataApresentacao, chaveContinua, quantidadeOcorrencias, quantidadeResumosVenda, programa, programaRetorno, totalTransacoesRegistradas, totalRegistros, colunaInicial, contadorTela, indexadorsExistenciaReal, resumosVenda });
                            if (!TesteHelper.IsAmbienteDesenvolvimento())
                            {
                                codigoRetorno = (short)cliente.ConsultarDisponibilidadeVencimentosODC(out quantidadeMensagensErro, out mensagemRetorno, out flagTemRegistro, timestamp, numeroEstabelecimento, numeroResumoVenda, dataApresentacao, chaveContinua, ref quantidadeOcorrencias, out quantidadeResumosVenda, out programa, out programaRetorno, out totalTransacoesRegistradas, out totalRegistros, out colunaInicial, out contadorTela, out indexadorsExistenciaReal, ref resumosVenda);
                            }
                            else
                            {
                                codigoRetorno = 0;
                                mensagemRetorno = string.Empty;
                                quantidadeOcorrencias = 3;

                                resumosVenda = new ResumoVendaOrdemCredito[3];

                                resumosVenda[0].DataAntecipacao = "01/01/2001";
                                resumosVenda[0].DataVencimento = "01/01/2001";
                                resumosVenda[0].NumeroEstabelecimento = 111111111;
                                resumosVenda[0].NumeroOdc = 1111111;
                                resumosVenda[0].NomeEstabelecimento = "Nome do estabelecimento 001";
                                resumosVenda[0].PrazoRecebimento = 11;
                                resumosVenda[0].Status = 11;
                                resumosVenda[0].ValorOrdemCredito = 00000000009999999M;

                                resumosVenda[1].DataAntecipacao = "02/02/2002";
                                resumosVenda[1].DataVencimento = "02/02/2002";
                                resumosVenda[1].NumeroEstabelecimento = 222222222;
                                resumosVenda[1].NumeroOdc = 2222222;
                                resumosVenda[1].NomeEstabelecimento = "Nome do estabelecimento 002";
                                resumosVenda[1].PrazoRecebimento = 22;
                                resumosVenda[1].Status = 12;
                                resumosVenda[1].ValorOrdemCredito = 00000000009999999M;

                                resumosVenda[2].DataAntecipacao = "03/03/2003";
                                resumosVenda[2].DataVencimento = "03/03/2003";
                                resumosVenda[2].NumeroEstabelecimento = 999999999;
                                resumosVenda[2].NumeroOdc = 9999999;
                                resumosVenda[2].NomeEstabelecimento = "Nome do estabelecimento 003";
                                resumosVenda[2].PrazoRecebimento = 99;
                                resumosVenda[2].Status = 13;
                                resumosVenda[2].ValorOrdemCredito = 00000000009999999M;
                            }

                            Log.GravarLog(EventoLog.RetornoHIS, new { quantidadeMensagensErro, mensagemRetorno, flagTemRegistro, quantidadeOcorrencias, quantidadeResumosVenda, programa, programaRetorno, totalTransacoesRegistradas, totalRegistros, colunaInicial, contadorTela, indexadorsExistenciaReal, resumosVenda });

                            statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);

                            if (codigoRetorno != 0)
                            {
                                Log.GravarLog(EventoLog.FimAgente, new { result, statusRetornoDTO });

                                return null;
                            }

                            result.AddRange(TradutorResultadoConsultaOrdensCredito.TraduzirRetornoListaConsultarDisponibilidadeVencimentosODC(resumosVenda, quantidadeOcorrencias));

                        } while (chaveContinua == "S");
                    }

                    Log.GravarLog(EventoLog.FimAgente, new { result });


                    return result;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion
    }
}
