using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarMotivoDebito
    {
        public static ConsultarMotivoDebitoEnvioDTO TraduzirEnvioConsultarMotivoDebito(ConsultarMotivoDebitoEnvio de)
        {
            ConsultarMotivoDebitoEnvioDTO para = new ConsultarMotivoDebitoEnvioDTO();
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.DataPesquisa = de.DataPesquisa;
            para.Timestamp = de.Timestamp;
            para.NumeroDebito = de.NumeroDebito;
            para.TipoPesquisa = de.TipoPesquisa;
            if(de.Versao.HasValue)
                para.Versao = de.Versao.ToString();

            return para;
        }

        public static ConsultarMotivoDebitoRetorno TraduzirRetornoConsultarMotivoDebito(ConsultarMotivoDebitoRetornoDTO de)
        {
            ConsultarMotivoDebitoRetorno para = new ConsultarMotivoDebitoRetorno();
            para.DataInclusao = de.DataInclusao;
            para.NumeroEstabelecimentoOrigem = de.NumeroEstabelecimentoOrigem;
            para.MotivoDebito = de.MotivoDebito;
            para.ReferenciaProcessamento = de.ReferenciaProcessamento;
            para.Resumo = de.Resumo;
            para.BandeiraResumoVendas = de.BandeiraResumoVendas;
            para.DataVenda = de.DataVenda;
            para.Cartao = de.Cartao;
            para.NumeroComprovanteVenda = de.NumeroComprovanteVenda;
            para.ValorConta = de.ValorConta;
            para.Parcela = de.Parcela;
            para.FormasPagamento = TraduzirRetornoListaConsultarMotivoDebitoFormaPagamento(de.FormasPagamento);

            return para;
        }

        public static List<ConsultarMotivoDebitoFormaPagamentoRetorno> TraduzirRetornoListaConsultarMotivoDebitoFormaPagamento(List<ConsultarMotivoDebitoFormaPagamentoRetornoDTO> list)
        {
            List<ConsultarMotivoDebitoFormaPagamentoRetorno> result = new List<ConsultarMotivoDebitoFormaPagamentoRetorno>();

            foreach (ConsultarMotivoDebitoFormaPagamentoRetornoDTO de in list)
            {
                ConsultarMotivoDebitoFormaPagamentoRetorno para = TraduzirRetornoConsultarMotivoDebitoFormaPagamento(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarMotivoDebitoFormaPagamentoRetorno TraduzirRetornoConsultarMotivoDebitoFormaPagamento(ConsultarMotivoDebitoFormaPagamentoRetornoDTO de)
        {
            ConsultarMotivoDebitoFormaPagamentoRetorno para = new ConsultarMotivoDebitoFormaPagamentoRetorno();
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