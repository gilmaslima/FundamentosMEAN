using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    /// <summary>
    /// WACA799 - Resumo de vendas - CDC.
    /// </summary>
    public static class TradutorConsultarTransacaoDebito
    {
        public static ConsultarTransacaoDebitoEnvioDTO TraduzirEnvioConsultarTransacaoDebito(ConsultarTransacaoDebitoEnvio de)
        {
            ConsultarTransacaoDebitoEnvioDTO para = new ConsultarTransacaoDebitoEnvioDTO();
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroResumoVenda = de.NumeroResumoVenda;
            para.DataApresentacao = de.DataApresentacao;
            
            return para;
        }

        public static ConsultarTransacaoDebitoRetorno TraduzirRetornoConsultarTransacaoDebito(ConsultarTransacaoDebitoRetornoDTO de)
        {
            ConsultarTransacaoDebitoRetorno para = new ConsultarTransacaoDebitoRetorno();
            para.NumeroEstabelecimentoAnterior = de.NumeroEstabelecimentoAnterior;
            para.NumeroResumoVendaAnterior = de.NumeroResumoVendaAnterior;
            para.DataApresentacaoAnterior = de.DataApresentacaoAnterior;
            para.QuantidadeOcorrencias = de.QuantidadeOcorrencias;
            para.QuantidadeComprovanteVenda = de.QuantidadeComprovanteVenda;
            para.ValorApresentado = de.ValorApresentado;
            para.ValorLiquido = de.ValorLiquido;
            para.ValorDesconto = de.ValorDesconto;
            para.DataVencimento = de.DataVencimento;
            para.IndicadorPreDatado = de.IndicadorPreDatado;
            para.CodigoTransacao = de.CodigoTransacao;
            para.SubCodigoTransacao = de.SubCodigoTransacao;
            para.DescricaoTransacao = de.DescricaoTransacao;
            para.ValorDescontoTaxaCrediario = de.ValorDescontoTaxaCrediario;
            para.ValorSaque = de.ValorSaque;
            para.ValorTotalCpmf = de.ValorTotalCpmf;
            para.ValorIof = de.ValorIof;
            para.TipoResposta = de.TipoResposta;
            para.CodigoBandeira = de.CodigoBandeira;

            return para;
        }
    }
}