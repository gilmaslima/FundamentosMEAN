using System;
using System.Collections.Generic;
using System.Linq;
using Redecard.PN.Extrato.Agentes.ServicoConsultaExtrato;
using Redecard.PN.Extrato.Agentes.Tradutores;
using Redecard.PN.Extrato.Comum;
using Redecard.PN.Extrato.Comum.Helper;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Agentes
{
    public class ConsultarExtratosAG : AgentesBase
    {
        #region ISF1 - WACA150 - Relatório de débitos e desagendamentos - Consolidado.
        /// <summary>
        /// WACA150 - Relatório de débitos e desagendamentos - Consolidado.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO ConsultarConsolidadoDebitosEDesagendamento(out StatusRetornoDTO statusRetornoDTO, ConsultarConsolidadoDebitosEDesagendamentoEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarConsolidadoDebitosEDesagendamento";
            using (Logger Log = Logger.IniciarLog("Relatório de débitos e desagendamentos - Consolidado [WACA150-ISF1]"))
            {

                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                try
                {
                    using (ExtratosClient cliente = new ExtratosClient())
                    {
                        string mensagemRetorno = default(string);
                        string chavePesquisa = default(string);
                        decimal valorPendenteDebito = default(decimal);
                        string sinalDebitoCreditoPendenteDebito = default(string);
                        decimal valorPendenteLiquido = default(decimal);
                        string sinalDebitoCreditoPendenteLiquido = default(string);
                        decimal valorPendente = default(decimal);
                        string sinalDebitoCreditoPendente = default(string);
                        decimal valorLiquidadoDebito = default(decimal);
                        string debitoCreditoLiquidadoDebito = default(string);
                        decimal valorLiquidadoLiquido = default(decimal);
                        string sinalDebitoCreditoLiquidadoLiquido = default(string);
                        string programaChamador = "WACA150";
                        string datainicial = envio.DataInicial.ToString("yyyyMMdd");
                        string dataFinal = envio.DataFinal.ToString("yyyyMMdd");
                        string tipoPesquisa = "T";
                        short codigoDaBandeira = 0;

                        int[] estabelecimentos = new int[3000];

                        for (int i = 0; i < envio.Estabelecimentos.Length; i++)
                        {
                            estabelecimentos[i] = envio.Estabelecimentos[i];
                        }

                        short codigoRetorno;
                        Log.GravarLog(EventoLog.ChamadaHIS, new { mensagemRetorno, chavePesquisa, valorPendenteDebito, sinalDebitoCreditoPendenteDebito, valorPendenteLiquido, sinalDebitoCreditoPendenteLiquido, valorPendente, sinalDebitoCreditoPendente, valorLiquidadoDebito, debitoCreditoLiquidadoDebito, valorLiquidadoLiquido, sinalDebitoCreditoLiquidadoLiquido, programaChamador, datainicial, dataFinal, tipoPesquisa, codigoDaBandeira, estabelecimentos });

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            // ASH: segundo Sérgio Andrade, deverão ser utilizadas as novas versões dos programas do mainframe
                            // // AGA: Este código foi substituido pela versão anterior do book, a nova versão
                            // // buscava dados da base online e não estava funcionando corretamente por divergências
                            // // nos ambientes de Simulação/Produção da Redecard (Mainframe)

                            codigoRetorno = cliente.ConsultarConsolidadoDebitosEDesagendamento(out mensagemRetorno, out chavePesquisa, out valorPendenteDebito, out sinalDebitoCreditoPendenteDebito, out valorPendenteLiquido, out sinalDebitoCreditoPendenteLiquido, out valorPendente, out sinalDebitoCreditoPendente, out valorLiquidadoDebito, out debitoCreditoLiquidadoDebito, out valorLiquidadoLiquido, out sinalDebitoCreditoLiquidadoLiquido, programaChamador, datainicial, dataFinal, tipoPesquisa, codigoDaBandeira, estabelecimentos);
                            //codigoRetorno = cliente.WACA150(out mensagemRetorno, out chavePesquisa, out valorPendenteDebito, out sinalDebitoCreditoPendenteDebito, out valorPendenteLiquido, out sinalDebitoCreditoPendenteLiquido, out valorPendente, out sinalDebitoCreditoPendente, out valorLiquidadoDebito, out debitoCreditoLiquidadoDebito, out valorLiquidadoLiquido, out sinalDebitoCreditoLiquidadoLiquido, programaChamador, datainicial, dataFinal, tipoPesquisa, codigoDaBandeira, estabelecimentos);
                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;

                            chavePesquisa = "282828";
                            valorPendenteDebito = 9999999999999.99M;
                            sinalDebitoCreditoPendenteDebito = "D";
                            valorPendenteLiquido = 9999999999999.99M;
                            sinalDebitoCreditoPendenteLiquido = "C";
                            valorPendente = 9999999999999.99M;
                            sinalDebitoCreditoPendente = "D";
                            valorLiquidadoDebito = 9999999999999.99M;
                            debitoCreditoLiquidadoDebito = "C";
                            valorLiquidadoLiquido = 9999999999999.99M;
                            sinalDebitoCreditoLiquidadoLiquido = "D";
                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                        Log.GravarLog(EventoLog.RetornoHIS, new { mensagemRetorno, chavePesquisa, valorPendenteDebito, sinalDebitoCreditoPendenteDebito, valorPendenteLiquido, sinalDebitoCreditoPendenteLiquido, valorPendente, sinalDebitoCreditoPendente, valorLiquidadoDebito, debitoCreditoLiquidadoDebito, valorLiquidadoLiquido, sinalDebitoCreditoLiquidadoLiquido });

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO result = TradutorResultadoConsultaExtrato.TraduzirRetornoConsultarConsolidadoDebitosEDesagendamento(chavePesquisa, valorPendenteDebito, sinalDebitoCreditoPendenteDebito, valorPendenteLiquido, sinalDebitoCreditoPendenteLiquido, valorPendente, sinalDebitoCreditoPendente, valorLiquidadoDebito, debitoCreditoLiquidadoDebito, valorLiquidadoLiquido, sinalDebitoCreditoLiquidadoLiquido);

                        Log.GravarLog(EventoLog.FimAgente, new { result });
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion

        #region ISF2 - WACA151 - Relatório de débitos e desagendamentos - Detalhe.
        /// <summary>
        /// WACA151 - Relatório de débitos e desagendamentos - Detalhe.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarDetalhamentoDebitosRetornoDTO ConsultarDetalhamentoDebitos(out StatusRetornoDTO statusRetornoDTO, ConsultarDetalhamentoDebitosEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarDetalhamentoDebitos";
            using (Logger Log = Logger.IniciarLog("Relatório de débitos e desagendamentos - Detalhe [WACA151-ISF2]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                try
                {
                    ConsultarDetalhamentoDebitosRetornoDTO result = null;

                    using (ExtratosClient cliente = new ExtratosClient())
                    {
                        string mensagemRetorno = string.Empty;
                        string programaChamador = "WA151";
                        string dataInicial = envio.DataInicial.ToString("yyyyMMdd");
                        string dataFinal = envio.DataFinal.ToString("yyyyMMdd");
                        string tipoPesquisa = envio.TipoPesquisa;
                        short codigoBandeira = envio.CodigoBandeira;
                        string indicadorRechamada = string.Empty;
                        string usuario = "xxx";
                        short quantidadeTransacoes = 0;
                        short numeroTransacao = 0;
                        string reservaDados = string.Empty;
                        string dados = string.Empty;

                        do
                        {
                            dados = string.Join("", from i in envio.Estabelecimentos select i.ToString("000000000")).PadRight(27000, '0');

                            Log.GravarLog(EventoLog.ChamadaHIS, new { mensagemRetorno, programaChamador, dataInicial, dataFinal, tipoPesquisa, codigoBandeira, indicadorRechamada, usuario, quantidadeTransacoes, numeroTransacao, dados });

                            short codigoRetorno;

                            if (!TesteHelper.IsAmbienteDesenvolvimento())
                            {
                                codigoRetorno = cliente.ConsultarDetalhamentoDebitos(out mensagemRetorno, programaChamador, dataInicial, dataFinal, tipoPesquisa, codigoBandeira, ref  indicadorRechamada, ref usuario, ref quantidadeTransacoes, ref numeroTransacao, out reservaDados, ref dados);
                            }
                            else
                            {
                                codigoRetorno = 0;

                                dados = "011D02.08.2011007430574CONTESTACAO VDA                00611230233699754730165              0000000000058110+0000000000000000+0000000000058110+01.01.00010000-00-00-00.00.00.00000011214000466 D02.08.2011007430574CONTESTACAO VDA                00611950177199778932170              0000000000002472+0000000000000000+0000000000002472+01.01.00010000-00-00-00.00.00.00000011214000467 D04.08.2011007430574CONTESTACAO VDA                00611270052099780949422              0000000000006502+0000000000000000+0000000000006502+01.01.00010000-00-00-00.00.00.00000011216000572 D04.08.2011007430574CONTESTACAO VDA                00611330041999723149998              0000000000062641+0000000000000000+0000000000062641+01.01.00010000-00-00-00.00.00.00000011216000573 D04.08.2011007430574CONTESTACAO VDA                00611330042099775349444              0000000000031281+0000000000000000+0000000000031281+01.01.00010000-00-00-00.00.00.00000011216000574 D04.08.2011007430574CONTESTACAO VDA                00611330042199775349444              0000000000148960+0000000000000000+0000000000148960+01.01.00010000-00-00-00.00.00.00000011216000575 D04.08.2011007430574CONTESTACAO VDA                00611810174899780949422              0000000000006500+0000000000000000+0000000000006500+01.01.00010000-00-00-00.00.00.00000011216000576 D04.08.2011007430574CONTESTACAO VDA                00611820132599780949422              0000000000006500+0000000000000000+0000000000006500+01.01.00010000-00-00-00.00.00.00000011216000577 D05.08.2011007430574CANCEL.DE VENDAS               00394113630001070649042              0000000000014874+0000000000000000+0000000000014874+01.01.00010000-00-00-00.00.00.00000011217001105 D18.01.2012007430574AL.POS/PINPAD/TX CONECT        12/2011        00736393              0000000000007500+0000000000000000+0000000000007500+01.01.00010000-00-00-00.00.00.00000012018036393 T          000000000                                                                    0000000000345340+0000000000000000+0000000000345340+                                    00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000            000000000                                                                    0000000000000000 0000000000000000 0000000000000000                                     00000000000";
                            }

                            statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);

                            Log.GravarLog(EventoLog.RetornoHIS, new { mensagemRetorno, indicadorRechamada, usuario, quantidadeTransacoes, numeroTransacao, reservaDados, dados });

                            if (codigoRetorno != 0)
                            {

                                return null;
                            }

                            ConsultarDetalhamentoDebitosRetornoDTO retornoDTO = TradutorResultadoConsultaExtrato.TraduzirRetornoConsultarDetalhamentoDebitos(dados);

                            if (result == null)
                            {
                                result = retornoDTO;
                            }
                            else
                            {
                                result.Registros.AddRange(retornoDTO.Registros);

                                result.Totais = retornoDTO.Totais;
                            }

                        } while (indicadorRechamada == "S");
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

        #region ISD1 - WACA150 - Relatório de débitos e desagendamentos - Consolidado.
        /// <summary>
        /// WACA150 - Relatório de débitos e desagendamentos - Consolidado.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO ConsultarConsolidadoDebitosEDesagendamentoISD1(out StatusRetornoDTO statusRetornoDTO, ConsultarConsolidadoDebitosEDesagendamentoEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarConsolidadoDebitosEDesagendamento";
            using (Logger Log = Logger.IniciarLog("Relatório de débitos e desagendamentos - Consolidado [WACA150-ISD1]"))
            {

                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                try
                {
                    using (ExtratosClient cliente = new ExtratosClient())
                    {
                        string mensagemRetorno = default(string);
                        string chavePesquisa = default(string);
                        decimal valorPendenteDebito = default(decimal);
                        string sinalDebitoCreditoPendenteDebito = default(string);
                        decimal valorPendenteLiquido = default(decimal);
                        string sinalDebitoCreditoPendenteLiquido = default(string);
                        decimal valorPendente = default(decimal);
                        string sinalDebitoCreditoPendente = default(string);
                        decimal valorLiquidadoDebito = default(decimal);
                        string debitoCreditoLiquidadoDebito = default(string);
                        decimal valorLiquidadoLiquido = default(decimal);
                        string sinalDebitoCreditoLiquidadoLiquido = default(string);
                        string programaChamador = "WACA150";
                        string datainicial = envio.DataInicial.ToString("yyyyMMdd");
                        string dataFinal = envio.DataFinal.ToString("yyyyMMdd");
                        string tipoPesquisa = "T";
                        short codigoDaBandeira = 0;

                        int[] estabelecimentos = new int[3000];

                        for (int i = 0; i < envio.Estabelecimentos.Length; i++)
                        {
                            estabelecimentos[i] = envio.Estabelecimentos[i];
                        }

                        short codigoRetorno;
                        Log.GravarLog(EventoLog.ChamadaHIS, new { mensagemRetorno, chavePesquisa, valorPendenteDebito, sinalDebitoCreditoPendenteDebito, valorPendenteLiquido, sinalDebitoCreditoPendenteLiquido, valorPendente, sinalDebitoCreditoPendente, valorLiquidadoDebito, debitoCreditoLiquidadoDebito, valorLiquidadoLiquido, sinalDebitoCreditoLiquidadoLiquido, programaChamador, datainicial, dataFinal, tipoPesquisa, codigoDaBandeira, estabelecimentos });

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            // Este código foi substituido pela versão anterior do book, a nova versão
                            // buscava dados da base online e não estava funcionando corretamente por divergências
                            // nos ambientes de Simulação/Produção da Redecard (Mainframe)

                            //codigoRetorno = cliente.ConsultarConsolidadoDebitosEDesagendamento(out mensagemRetorno, out chavePesquisa, out valorPendenteDebito, out sinalDebitoCreditoPendenteDebito, out valorPendenteLiquido, out sinalDebitoCreditoPendenteLiquido, out valorPendente, out sinalDebitoCreditoPendente, out valorLiquidadoDebito, out debitoCreditoLiquidadoDebito, out valorLiquidadoLiquido, out sinalDebitoCreditoLiquidadoLiquido, programaChamador, datainicial, dataFinal, tipoPesquisa, codigoDaBandeira, estabelecimentos);
                            codigoRetorno = cliente.WACA150(out mensagemRetorno, out chavePesquisa, out valorPendenteDebito, out sinalDebitoCreditoPendenteDebito, out valorPendenteLiquido, out sinalDebitoCreditoPendenteLiquido, out valorPendente, out sinalDebitoCreditoPendente, out valorLiquidadoDebito, out debitoCreditoLiquidadoDebito, out valorLiquidadoLiquido, out sinalDebitoCreditoLiquidadoLiquido, programaChamador, datainicial, dataFinal, tipoPesquisa, codigoDaBandeira, estabelecimentos);
                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;

                            chavePesquisa = "282828";
                            valorPendenteDebito = 9999999999999.99M;
                            sinalDebitoCreditoPendenteDebito = "D";
                            valorPendenteLiquido = 9999999999999.99M;
                            sinalDebitoCreditoPendenteLiquido = "C";
                            valorPendente = 9999999999999.99M;
                            sinalDebitoCreditoPendente = "D";
                            valorLiquidadoDebito = 9999999999999.99M;
                            debitoCreditoLiquidadoDebito = "C";
                            valorLiquidadoLiquido = 9999999999999.99M;
                            sinalDebitoCreditoLiquidadoLiquido = "D";
                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                        Log.GravarLog(EventoLog.RetornoHIS, new { mensagemRetorno, chavePesquisa, valorPendenteDebito, sinalDebitoCreditoPendenteDebito, valorPendenteLiquido, sinalDebitoCreditoPendenteLiquido, valorPendente, sinalDebitoCreditoPendente, valorLiquidadoDebito, debitoCreditoLiquidadoDebito, valorLiquidadoLiquido, sinalDebitoCreditoLiquidadoLiquido });

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO result = TradutorResultadoConsultaExtrato.TraduzirRetornoConsultarConsolidadoDebitosEDesagendamento(chavePesquisa, valorPendenteDebito, sinalDebitoCreditoPendenteDebito, valorPendenteLiquido, sinalDebitoCreditoPendenteLiquido, valorPendente, sinalDebitoCreditoPendente, valorLiquidadoDebito, debitoCreditoLiquidadoDebito, valorLiquidadoLiquido, sinalDebitoCreditoLiquidadoLiquido);

                        Log.GravarLog(EventoLog.FimAgente, new { result });
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion

        #region ISD2 - WACA151 - Relatório de débitos e desagendamentos - Detalhe.
        /// <summary>
        /// WACA151 - Relatório de débitos e desagendamentos - Detalhe.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarDetalhamentoDebitosRetornoDTO ConsultarDetalhamentoDebitosISD2(out StatusRetornoDTO statusRetornoDTO, ConsultarDetalhamentoDebitosEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarDetalhamentoDebitos";
            using (Logger Log = Logger.IniciarLog("Relatório de débitos e desagendamentos - Detalhe [WACA151-ISD2]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                try
                {
                    ConsultarDetalhamentoDebitosRetornoDTO result = null;

                    using (ExtratosClient cliente = new ExtratosClient())
                    {
                        string mensagemRetorno = string.Empty;
                        string programaChamador = "WA151";
                        string dataInicial = envio.DataInicial.ToString("yyyyMMdd");
                        string dataFinal = envio.DataFinal.ToString("yyyyMMdd");
                        string tipoPesquisa = envio.TipoPesquisa;
                        short codigoBandeira = envio.CodigoBandeira;
                        string indicadorRechamada = string.Empty;
                        string usuario = "xxx";
                        short quantidadeTransacoes = 0;
                        string sQuantidadeTransacoes = "0";
                        short numeroTransacao = 0;
                        string reservaDados = string.Empty;
                        string dados = string.Empty;
                        WACA151_S_OCC_EXTR_A[] resultado = null;

                        dados = string.Join("", from i in envio.Estabelecimentos select i.ToString("000000000")).PadRight(27000, '0');

                        Log.GravarLog(EventoLog.ChamadaHIS, new { mensagemRetorno, programaChamador, dataInicial, dataFinal, tipoPesquisa, codigoBandeira, indicadorRechamada, usuario, quantidadeTransacoes, numeroTransacao, dados });

                        short codigoRetorno;
                        ConsultarDetalhamentoDebitosRetornoDTO retornoDTO = new ConsultarDetalhamentoDebitosRetornoDTO();

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            // Este código foi substituido pela versão anterior do book, a nova versão
                            // buscava dados da base online e não estava funcionando corretamente por divergências
                            // nos ambientes de Simulação/Produção da Redecard (Mainframe)
                            string sequenciaString = "000000";

                            do
                            {
                                //codigoRetorno = cliente.ConsultarDetalhamentoDebitos(out mensagemRetorno, programaChamador, dataInicial, dataFinal, tipoPesquisa, codigoBandeira, ref  indicadorRechamada, ref usuario, ref quantidadeTransacoes, ref numeroTransacao, out reservaDados, ref dados);
                                codigoRetorno = short.Parse(cliente.WACA151(out mensagemRetorno, out sQuantidadeTransacoes, out resultado, programaChamador, envio.ChavePesquisa, tipoPesquisa, sequenciaString));
                                quantidadeTransacoes = Int16.Parse(sQuantidadeTransacoes);

                                sequenciaString = resultado[quantidadeTransacoes - 1].WACA151_S_SEQUE;

                                // Converter o resultado no objeto DTO
                                ObterDTORetorno(retornoDTO, quantidadeTransacoes, resultado);

                            } while (codigoRetorno == 0 && quantidadeTransacoes >= 170);
                        }
                        else
                        {
                            codigoRetorno = 0;
                            resultado = new WACA151_S_OCC_EXTR_A[1];
                            resultado[0].WACA151_S_BND_RVO = "MULTI";
                            resultado[0].WACA151_S_DEBCRE = "";
                            resultado[0].WACA151_S_DT_INCL = "26/06/2012";
                            resultado[0].WACA151_S_DT_PGTO = "08/02/2013";
                            resultado[0].WACA151_S_INDESG = "S";
                            resultado[0].WACA151_S_MOT_DEB = "CANCEL.DE VENDAS";
                            resultado[0].WACA151_S_PV_ORIG = "035129891";
                            resultado[0].WACA151_S_REF_PROC = "052121781222420";
                            resultado[0].WACA151_S_RESUMO = "096660703";
                            resultado[0].WACA151_S_SEQUE = "000001";
                            resultado[0].WACA151_S_TIMEST = "2013-02-08-00.22.42.670768";
                            resultado[0].WACA151_S_TIP_REG = "L";
                            resultado[0].WACA151_S_VAL_COMP = 301.25m;
                            resultado[0].WACA151_S_VAL_DEB = 301.25m;
                            resultado[0].WACA151_S_VAL_PEND = 0;

                            quantidadeTransacoes = 1;

                            // Converter o resultado no objeto DTO
                            ObterDTORetorno(retornoDTO, quantidadeTransacoes, resultado);
                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                        Log.GravarLog(EventoLog.RetornoHIS, new { mensagemRetorno, indicadorRechamada, usuario, quantidadeTransacoes, numeroTransacao, reservaDados, dados });

                        // Obter totais
                        ObterDTORetornoTotais(retornoDTO);

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        if (result == null)
                        {
                            result = retornoDTO;
                        }
                        else
                        {
                            result.Registros.AddRange(retornoDTO.Registros);
                            result.Totais = retornoDTO.Totais;
                        }
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

        /// <summary>
        /// Preencher objeto de retorno de acordo com as informações retornadas pelo mainframe
        /// </summary>
        private void ObterDTORetorno(ConsultarDetalhamentoDebitosRetornoDTO dto, short quantidadeTransacoes, WACA151_S_OCC_EXTR_A[] resultado)
        {
            if (object.ReferenceEquals(null, dto))
                dto = new ConsultarDetalhamentoDebitosRetornoDTO();

            if (quantidadeTransacoes > 0)
            {
                if (object.ReferenceEquals(dto.Registros, null))
                    dto.Registros = new List<ConsultarDetalhamentoDebitosDetalheRetornoDTO>();

                for (int i = 0; i < quantidadeTransacoes; i++)
                {
                    ConsultarDetalhamentoDebitosDetalheRetornoDTO retorno = new ConsultarDetalhamentoDebitosDetalheRetornoDTO();
                    retorno.Bandeira = resultado[i].WACA151_S_BND_RVO;
                    retorno.DataInclusao = resultado[i].WACA151_S_DT_INCL.ToDate();
                    retorno.DataPagamento = resultado[i].WACA151_S_DT_PGTO.ToDate();
                    retorno.EstabelecimentoOrigem = resultado[i].WACA151_S_PV_ORIG.ToInt32();
                    retorno.IndicadorDebitoCredito = resultado[i].WACA151_S_DEBCRE;
                    retorno.IndicadorDesagendamento = resultado[i].WACA151_S_INDESG;
                    retorno.MotivoDebito = resultado[i].WACA151_S_MOT_DEB;
                    retorno.NumeroDebito = resultado[i].WACA151_S_SEQUE.ToDecimal();
                    retorno.ProcessoReferente = resultado[i].WACA151_S_REF_PROC;
                    retorno.Resumo = resultado[i].WACA151_S_RESUMO;
                    retorno.Timestamp = resultado[i].WACA151_S_TIMEST;
                    retorno.TipoRegistro = resultado[i].WACA151_S_TIP_REG;
                    retorno.ValorCompensado = resultado[i].WACA151_S_VAL_COMP;
                    retorno.ValorDebito = resultado[i].WACA151_S_VAL_DEB;
                    retorno.ValorPendente = resultado[i].WACA151_S_VAL_PEND;
                    dto.Registros.Add(retorno);
                }
            }
        }

        /// <summary>
        /// Fax o calculo dos registros do Mainframe para obter os totais
        /// </summary>
        private void ObterDTORetornoTotais(ConsultarDetalhamentoDebitosRetornoDTO dto)
        {
            if (object.ReferenceEquals(null, dto))
                dto = new ConsultarDetalhamentoDebitosRetornoDTO();

            if (object.ReferenceEquals(dto.Registros, null))
                dto.Registros = new List<ConsultarDetalhamentoDebitosDetalheRetornoDTO>();

            Decimal totalDevido = 0;
            Decimal totalCompensado = 0;

            foreach (ConsultarDetalhamentoDebitosDetalheRetornoDTO retorno in dto.Registros)
            {
                totalDevido += retorno.ValorDebito;
                totalCompensado += retorno.ValorCompensado;
            }

            dto.Totais = new ConsultarDetalhamentoDebitosTotaisRetornoDTO();
            dto.Totais.TotalValorCompensado = totalCompensado;
            dto.Totais.TotalValorDevido = totalDevido;
        }

        #endregion
    }
}