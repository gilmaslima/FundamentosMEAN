using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarDetalhamentoDebitos
    {
        public static ConsultarDetalhamentoDebitosEnvioDTO TraduzirEnvioConsultarDetalhamentoDebitos(ConsultarDetalhamentoDebitosEnvio de)
        {
            ConsultarDetalhamentoDebitosEnvioDTO para = new ConsultarDetalhamentoDebitosEnvioDTO();
            para.DataInicial = de.DataInicial;
            para.DataFinal = de.DataFinal;
            para.TipoPesquisa = de.TipoPesquisa;
            para.CodigoBandeira = de.CodigoBandeira;
            para.Estabelecimentos = de.Estabelecimentos;
            para.ChavePesquisa = de.ChavePesquisa;
            if(de.Versao.HasValue)
                para.Versao = de.Versao.ToString();

            return para;
        }

        public static ConsultarDetalhamentoDebitosRetorno TraduzirRetornoConsultarDetalhamentoDebitos(RetornoPesquisaComTotalizadorDTO<ConsultarDetalhamentoDebitosDetalheRetornoDTO, ConsultarDetalhamentoDebitosTotaisRetornoDTO> retornoPesquisa)
        {
            ConsultarDetalhamentoDebitosRetorno para = new ConsultarDetalhamentoDebitosRetorno();
            para.Registros = TraduzirRetornoListaConsultarDetalhamentoDebitosDetalhe(retornoPesquisa.Registros);
            para.Totais = TraduzirRetornoConsultarDetalhamentoDebitosTotais(retornoPesquisa.Totalizador);
            para.QuantidadeTotalRegistros = retornoPesquisa.QuantidadeTotalRegistros;

            return para;
        }

        private static ConsultarDetalhamentoDebitosTotaisRetorno TraduzirRetornoConsultarDetalhamentoDebitosTotais(ConsultarDetalhamentoDebitosTotaisRetornoDTO de)
        {
            ConsultarDetalhamentoDebitosTotaisRetorno para = new ConsultarDetalhamentoDebitosTotaisRetorno();
            para.TotalValorDevido = de.TotalValorDevido;
            para.TotalValorCompensado = de.TotalValorCompensado;

            return para;
        }

        public static List<ConsultarDetalhamentoDebitosDetalheRetorno> TraduzirRetornoListaConsultarDetalhamentoDebitosDetalhe(List<ConsultarDetalhamentoDebitosDetalheRetornoDTO> list)
        {
            List<ConsultarDetalhamentoDebitosDetalheRetorno> result = new List<ConsultarDetalhamentoDebitosDetalheRetorno>();

            foreach (ConsultarDetalhamentoDebitosDetalheRetornoDTO de in list)
            {
                ConsultarDetalhamentoDebitosDetalheRetorno para = TraduzirRetornoConsultarDetalhamentoDebitosDetalhe(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarDetalhamentoDebitosDetalheRetorno TraduzirRetornoConsultarDetalhamentoDebitosDetalhe(ConsultarDetalhamentoDebitosDetalheRetornoDTO de)
        {
            ConsultarDetalhamentoDebitosDetalheRetorno para = new ConsultarDetalhamentoDebitosDetalheRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataInclusao = de.DataInclusao;
            para.EstabelecimentoOrigem = de.EstabelecimentoOrigem;
            para.MotivoDebito = de.MotivoDebito;
            para.IndicadorDesagendamento = de.IndicadorDesagendamento;
            para.ProcessoReferente = de.ProcessoReferente;
            para.Resumo = de.Resumo;
            para.Bandeira = de.Bandeira;
            para.ValorDebito = de.ValorDebito;
            para.ValorCompensado = de.ValorCompensado;
            para.ValorPendente = de.ValorPendente;
            para.DataPagamento = de.DataPagamento;
            para.Timestamp = de.Timestamp;
            para.NumeroDebito = de.NumeroDebito;
            para.IndicadorDebitoCredito = de.IndicadorDebitoCredito;
            
            return para;
        }
    }
}