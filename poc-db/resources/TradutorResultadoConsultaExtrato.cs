using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoConsultaExtrato;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoConsultaExtrato
    {
        #region WACA150 - Relatório de débitos e desagendamentos - Consolidado.
        public static ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO TraduzirRetornoConsultarConsolidadoDebitosEDesagendamento(string chavePesquisa, decimal valorPendenteDebito, string sinalDebitoCreditoPendenteDebito, decimal valorPendenteLiquido, string sinalDebitoCreditoPendenteLiquido, decimal valorPendente, string sinalDebitoCreditoPendente, decimal valorLiquidadoDebito, string debitoCreditoLiquidadoDebito, decimal valorLiquidadoLiquido, string sinalDebitoCreditoLiquidadoLiquido)
        {
            sinalDebitoCreditoPendenteDebito = sinalDebitoCreditoPendenteDebito == "C" || sinalDebitoCreditoPendenteDebito == "+" ? "+" : "-";
            sinalDebitoCreditoPendenteLiquido = sinalDebitoCreditoPendenteLiquido == "C" || sinalDebitoCreditoPendenteLiquido == "+" ? "+" : "-";
            sinalDebitoCreditoPendente = sinalDebitoCreditoPendente == "C" || sinalDebitoCreditoPendente == "+" ? "+" : "-";
            debitoCreditoLiquidadoDebito = debitoCreditoLiquidadoDebito == "C" || debitoCreditoLiquidadoDebito == "+" ? "+" : "-";
            sinalDebitoCreditoLiquidadoLiquido = sinalDebitoCreditoLiquidadoLiquido == "C" || sinalDebitoCreditoLiquidadoLiquido == "+" ? "+" : "-";

            ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO para = new ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO();
            para.ChavePesquisa = chavePesquisa;
            para.ValorPendenteDebito = valorPendenteDebito * (sinalDebitoCreditoPendenteDebito.Equals("+") ? 1 : -1);
            para.ValorPendenteLiquido = valorPendenteLiquido * (sinalDebitoCreditoPendenteLiquido.Equals("+") ? 1 : -1);
            para.ValorPendente = valorPendente * (sinalDebitoCreditoPendente.Equals("+") ? 1 : -1);
            para.ValorLiquidadoDebito = valorLiquidadoDebito * (debitoCreditoLiquidadoDebito.Equals("+") ? 1 : -1);
            para.ValorLiquidadoLiquido = valorLiquidadoLiquido * (sinalDebitoCreditoLiquidadoLiquido.Equals("+") ? 1 : -1);

            return para;
        }
        #endregion

        #region WACA151 - Relatório de débitos e desagendamentos - Detalhe.
        public static ConsultarDetalhamentoDebitosRetornoDTO TraduzirRetornoConsultarDetalhamentoDebitos(string dados)
        {
            CortadorMensagem cortador = new CortadorMensagem(dados);

            int quantidadeRegistros = cortador.LerInt32(3);

            if (quantidadeRegistros > 160)
            {
                quantidadeRegistros = 160;
            }

            string[] registros = cortador.LerOccurs(187, 160);

            ConsultarDetalhamentoDebitosRetornoDTO result = new ConsultarDetalhamentoDebitosRetornoDTO();
            result.Registros = TraduzirRetornoListaConsultarDetalhamentoDebitosDetalhe(registros, quantidadeRegistros);
            result.Totais = TraduzirRetornoConsultarDetalhamentoDebitosTotais(registros, quantidadeRegistros);

            return result;
        }

        private static ConsultarDetalhamentoDebitosTotaisRetornoDTO TraduzirRetornoConsultarDetalhamentoDebitosTotais(string[] registros, int quantidadeRegistros)
        {
            ConsultarDetalhamentoDebitosTotaisRetornoDTO result = new ConsultarDetalhamentoDebitosTotaisRetornoDTO();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                String registro = registros[i];

                CortadorMensagem cortador = new CortadorMensagem(registro);

                string tipoRegistro = cortador.LerString(1);

                if (tipoRegistro == "T")
                {
                    ConsultarDetalhamentoDebitosDetalheRetornoDTO dto = ObterConsultarDetalhamentoDebitosDetalheRetornoDTO(cortador);

                    result.TotalValorDevido = dto.ValorDebito;
                    result.TotalValorCompensado = dto.ValorCompensado;

                    break;
                }
            }

            return result;
        }

        public static List<ConsultarDetalhamentoDebitosDetalheRetornoDTO> TraduzirRetornoListaConsultarDetalhamentoDebitosDetalhe(string[] registros, int quantidadeRegistros)
        {
            List<ConsultarDetalhamentoDebitosDetalheRetornoDTO> result = new List<ConsultarDetalhamentoDebitosDetalheRetornoDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                String registro = registros[i];

                CortadorMensagem cortador = new CortadorMensagem(registro);

                string tipoRegistro = cortador.LerString(1);

                if (tipoRegistro == "D")
                {
                    ConsultarDetalhamentoDebitosDetalheRetornoDTO dto = ObterConsultarDetalhamentoDebitosDetalheRetornoDTO(cortador);

                    result.Add(dto);
                }
            }

            return result;
        }

        private static ConsultarDetalhamentoDebitosDetalheRetornoDTO ObterConsultarDetalhamentoDebitosDetalheRetornoDTO(CortadorMensagem cortador)
        {
            ConsultarDetalhamentoDebitosDetalheRetornoDTO result = new ConsultarDetalhamentoDebitosDetalheRetornoDTO();
            result.DataInclusao = cortador.LerData(10, "dd/MM/yyyy", true);
            result.EstabelecimentoOrigem = cortador.LerInt32(9);
            result.MotivoDebito = cortador.LerString(30);
            result.IndicadorDesagendamento = cortador.LerString(1);
            result.ProcessoReferente = cortador.LerString(15);
            result.Resumo = cortador.LerString(10);
            result.Bandeira = cortador.LerString(12);
            result.ValorDebito = cortador.LerDecimal(14, 2) * (cortador.LerString(1) == "+" ? 1 : -1);
            result.ValorCompensado = cortador.LerDecimal(14, 2) * (cortador.LerString(1) == "+" ? 1 : -1);
            result.ValorPendente = cortador.LerDecimal(14, 2) * (cortador.LerString(1) == "+" ? 1 : -1);
            result.DataPagamento = cortador.LerData(10, "dd/MM/yyyy", true);
            result.Timestamp = cortador.LerString(26);
            result.NumeroDebito = cortador.LerDecimal(11, 0);
            result.IndicadorDebitoCredito = cortador.LerString(1);

            return result;
        }
        #endregion
    }
}
