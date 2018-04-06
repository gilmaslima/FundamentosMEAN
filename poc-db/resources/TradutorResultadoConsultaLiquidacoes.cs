using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoConsultaLiquidacoes;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoConsultaLiquidacoes
    {
        /// <summary>
        /// Traduzir o objeto retornado pelo book WACA1252
        /// </summary>
        /// <param name="dataInclusao"></param>
        /// <param name="numeroEstabelecimentoOrigem"></param>
        /// <param name="motivoDebito"></param>
        /// <param name="referenciaProcessamento"></param>
        /// <param name="resumo"></param>
        /// <param name="bandeiraResumoVendas"></param>
        /// <param name="dataVenda"></param>
        /// <param name="cartao"></param>
        /// <param name="numeroComprovanteVenda"></param>
        /// <param name="valorConta"></param>
        /// <param name="parcela"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ConsultarMotivoDebitoRetornoDTO TraduzirRetornoConsultarMotivoDebito(string dataInclusao, int numeroEstabelecimentoOrigem, string motivoDebito, string referenciaProcessamento, string resumo, string bandeiraResumoVendas, string dataVenda, string cartao, string numeroComprovanteVenda, string valorConta, string parcela, short quantidadeRegistros, ExtratoLiquidacao[] list)
        {
            ConsultarMotivoDebitoRetornoDTO result = new ConsultarMotivoDebitoRetornoDTO();
            result.DataInclusao = dataInclusao;
            result.NumeroEstabelecimentoOrigem = numeroEstabelecimentoOrigem;
            result.MotivoDebito = motivoDebito;
            result.ReferenciaProcessamento = referenciaProcessamento;
            result.Resumo = resumo;
            result.BandeiraResumoVendas = bandeiraResumoVendas;
            result.DataVenda = dataVenda;
            result.Cartao = cartao;
            result.NumeroComprovanteVenda = numeroComprovanteVenda;
            result.ValorConta = valorConta;
            result.Parcela = parcela;
            result.FormasPagamento = TraduzirRetornoListaConsultarMotivoDebitoFormaPagamento(list, quantidadeRegistros);

            return result;
        }

        public static List<ConsultarMotivoDebitoFormaPagamentoRetornoDTO> TraduzirRetornoListaConsultarMotivoDebitoFormaPagamento(ExtratoLiquidacao[] list, short quantidadeRegistros)
        {
            List<ConsultarMotivoDebitoFormaPagamentoRetornoDTO> result = new List<ConsultarMotivoDebitoFormaPagamentoRetornoDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                ConsultarMotivoDebitoFormaPagamentoRetornoDTO para = TraduzirRetornoConsultarMotivoDebitoFormaPagamento(list[i]);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarMotivoDebitoFormaPagamentoRetornoDTO TraduzirRetornoConsultarMotivoDebitoFormaPagamento(ExtratoLiquidacao de)
        {
            ConsultarMotivoDebitoFormaPagamentoRetornoDTO para = new ConsultarMotivoDebitoFormaPagamentoRetornoDTO();
            para.BandeiraResumoVendasDebitado = de.BandeiraResumoVendasDebitado;
            para.DataPagamento = de.DataPagamento;
            para.EstabelecimentoDebito = de.EstabelecimentoDebito;
            para.MeioPagamento = de.MeioPagamento;
            para.ResumoVendaDebito = de.ResumoVendaDebito;
            para.SinalDebitoCredito = de.SinalDebitoCredito;
            para.ValorComplementar = de.ValorComplementar;
            para.ValorDebito = de.ValorDebito;
            para.ValorPendente = de.ValorPendente;

            return para;
        }
    }
}
