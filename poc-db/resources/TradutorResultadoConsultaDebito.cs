using System;
using System.Collections.Generic;
using Redecard.PN.Extrato.Agentes.ServicoConsultaDebito;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoConsultaDebito
    {
        #region WACA799 - Resumo de vendas - CDC.
        public static ConsultarTransacaoDebitoRetornoDTO TraduzirRetornoConsultarTransacaoDebito(int numeroEstabelecimentoAnterior, int numeroResumoVendaAnterior, string pDataApresentacaoAnterior, short quantidadeOcorrencias, int quantidadeComprovanteVenda, decimal valorApresentado, decimal valorLiquido, decimal valorDesconto, string pDataVencimento, string indicadorPreDatado, short codigoTransacao, short subCodigoTransacao, string descricaoTransacao, decimal valorDescontoTaxaCrediario, decimal valorSaque, decimal valorTotalCpmf, decimal valorIof, string tipoResposta, string codigoBandeira)
        {
            DateTime dataApresentacaoAnterior;
            DateTime.TryParseExact(pDataApresentacaoAnterior, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dataApresentacaoAnterior);

            DateTime dataVencimento;
            DateTime.TryParseExact(pDataVencimento, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

            ConsultarTransacaoDebitoRetornoDTO para = new ConsultarTransacaoDebitoRetornoDTO();
            para.NumeroEstabelecimentoAnterior = numeroEstabelecimentoAnterior;
            para.NumeroResumoVendaAnterior = numeroResumoVendaAnterior;
            para.DataApresentacaoAnterior = dataApresentacaoAnterior;
            para.QuantidadeOcorrencias = quantidadeOcorrencias;
            para.QuantidadeComprovanteVenda = quantidadeComprovanteVenda;
            para.ValorApresentado = valorApresentado;
            para.ValorLiquido = valorLiquido;
            para.ValorDesconto = valorDesconto;
            para.DataVencimento = dataVencimento;
            para.IndicadorPreDatado = indicadorPreDatado;
            para.CodigoTransacao = codigoTransacao;
            para.SubCodigoTransacao = subCodigoTransacao;
            para.DescricaoTransacao = descricaoTransacao;
            para.ValorDescontoTaxaCrediario = valorDescontoTaxaCrediario;
            para.ValorSaque = valorSaque;
            para.ValorTotalCpmf = valorTotalCpmf;
            para.ValorIof = valorIof;
            para.TipoResposta = tipoResposta;
            para.CodigoBandeira = codigoBandeira;

            return para;
        }
        #endregion
    }
}
