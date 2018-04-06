using System;
using System.Collections.Generic;
using Redecard.PN.Extrato.Agentes.ServicoConsultaResumoVendas;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoResumoVendas
    {        
        #region WACA617 - Resumo de vendas - Cartões de débito.
        public static List<ConsultarPreDatadosRetornoDTO> TraduzirRetornoListaConsultarPreDatados(PreDatadoResumoVenda[] list, int quantidadeOcorrencias)
        {
            List<ConsultarPreDatadosRetornoDTO> result = new List<ConsultarPreDatadosRetornoDTO>();

            for (int i = 0; i < quantidadeOcorrencias; i++)
            {
                ConsultarPreDatadosRetornoDTO para = TraduzirRetornoConsultarPreDatados(list[i]);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarPreDatadosRetornoDTO TraduzirRetornoConsultarPreDatados(PreDatadoResumoVenda de)
        {
            DateTime dataVencimento;
            DateTime.TryParseExact(de.DataVencimento, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

            ConsultarPreDatadosRetornoDTO para = new ConsultarPreDatadosRetornoDTO();
            para.CodigoSubTransacao = de.CodigoSubTransacao;
            para.CodigoTransacao = de.CodigoTransacao;
            para.DataVencimento = dataVencimento;
            para.DescontoTaxaCredito = de.DescontoTaxaCredito;
            para.Descricao = de.Descricao;
            para.DescricaoBandeira = de.DescricaoBandeira;
            para.NumeroPeca = de.NumeroPeca;
            para.QuantidadeComprovantesVenda = de.QuantidadeComprovantesVenda;
            para.QuantidadePecas = de.QuantidadePecas;
            para.ReservaDados = de.ReservaDados;
            para.TipoVenda = de.TipoVenda;
            para.ValorApresentado = de.ValorApresentado;
            para.ValorDesconto = de.ValorDesconto;
            para.ValorLiquido = de.ValorLiquido;
            para.ValorSaque = de.ValorSaque;
            para.ValorTotalCpmf = de.ValorTotalCpmf;
            para.ValorTotalIof = de.ValorTotalIof;

            return para;
        }
        #endregion

        #region WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
        public static List<ConsultarVencimentosResumoVendaRetornoDTO> TraduzirRetornoListaConsultarVencimentosResumoVenda(VencimentoResumoVenda[] list, int quantidadeOcorrencias)
        {
            List<ConsultarVencimentosResumoVendaRetornoDTO> result = new List<ConsultarVencimentosResumoVendaRetornoDTO>();

            for (int i = 0; i < quantidadeOcorrencias; i++)
            {
                ConsultarVencimentosResumoVendaRetornoDTO para = TraduzirRetornoConsultarVencimentosResumoVenda(list[i]);

                if (para.DataVencimento == DateTime.MinValue)
                {
                    break;
                }

                result.Add(para);
            }

            return result;
        }

        private static ConsultarVencimentosResumoVendaRetornoDTO TraduzirRetornoConsultarVencimentosResumoVenda(VencimentoResumoVenda de)
        {
            DateTime dataVencimento;
            DateTime.TryParseExact(de.DataVencimento, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

            ConsultarVencimentosResumoVendaRetornoDTO para = new ConsultarVencimentosResumoVendaRetornoDTO();
            para.DataVencimento = dataVencimento;
            para.NumeroPeca = de.NumeroPeca;
            para.QuantidadePecas = de.QuantidadePecas;
            para.QuantidadeComprovanteVenda = de.QuantidadeComprovanteVenda;
            para.Descricao = de.Descricao;
            para.ValorApresentado = de.ValorApresentado;
            para.ValorLiquido = de.ValorLiquido;
            para.Banco = de.Banco;
            para.Agencia = de.Agencia;
            para.Conta = de.Conta;

            return para;
        }
        #endregion

        #region WACA700 - Resumo de vendas - Cartões de crédito.
        public static List<ConsultarWACA700RetornoDTO> TraduzirRetornoListaConsultarWACA700(DetalheResumoVenda[] list, int quantidadeOcorrencias)
        {
            List<ConsultarWACA700RetornoDTO> result = new List<ConsultarWACA700RetornoDTO>();

            for (int i = 1; i < quantidadeOcorrencias; i++)
            {
                ConsultarWACA700RetornoDTO para = TraduzirRetornoConsultarWACA700(list[i]);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarWACA700RetornoDTO TraduzirRetornoConsultarWACA700(DetalheResumoVenda de)
        {
            ConsultarWACA700RetornoDTO para = new ConsultarWACA700RetornoDTO();
            para.Detalhe = de.Detalhe;
            para.NumeroMes = de.NumeroMes;
            para.Timestamp = de.Timestamp;
            para.TipoResumoVenda = de.TipoResumoVenda;

            return para;
        }
        #endregion

        #region WACA701 - Resumo de vendas - Cartões de crédito.
        public static ConsultarWACA701RetornoDTO TraduzirRetornoConsultarWACA701(int resumoVenda, decimal valorApresentado, short quantidadeComprovantesVenda, decimal valorApurado, string dataApresentacaoRetornado, decimal valorDesconto, string dataProcessamento, decimal valorGorjetaTaxaEmbarque, string tipoResumo, decimal valorCotacao, string tipoMoeda, string indicadorTaxaEmbarque)
        {
            DateTime tmpDataApresentacaoRetornado;
            DateTime.TryParseExact(dataApresentacaoRetornado, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out tmpDataApresentacaoRetornado);

            DateTime tmpDataProcessamento;
            DateTime.TryParseExact(dataProcessamento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out tmpDataProcessamento);

            ConsultarWACA701RetornoDTO para = new ConsultarWACA701RetornoDTO();
            para.ResumoVenda = resumoVenda;
            para.ValorApresentado = valorApresentado;
            para.QuantidadeComprovantesVenda = quantidadeComprovantesVenda;
            para.ValorApurado = valorApurado;
            para.DataApresentacaoRetornado = tmpDataApresentacaoRetornado;
            para.ValorDesconto = valorDesconto;
            para.DataProcessamento = tmpDataProcessamento;
            para.ValorGorjetaTaxaEmbarque = valorGorjetaTaxaEmbarque;
            para.TipoResumo = tipoResumo;
            para.ValorCotacao = valorCotacao;
            para.TipoMoeda = tipoMoeda;
            para.IndicadorTaxaEmbarque = indicadorTaxaEmbarque;

            return para;
        }
        #endregion

        #region WACA705 - Resumo de vendas - Cartões de crédito - CV's rejeitados.
        public static List<ConsultarRejeitadosRetornoDTO> TraduzirRetornoListaConsultarRejeitados(ResumoVendaRejeitado[] list, int quantidadeOcorrencias)
        {
            List<ConsultarRejeitadosRetornoDTO> result = new List<ConsultarRejeitadosRetornoDTO>();

            for (int i = 0; i < quantidadeOcorrencias; i++)
            {
                ConsultarRejeitadosRetornoDTO para = TraduzirRetornoConsultarRejeitados(list[i]);

                if (para.DataComprovanteVenda == DateTime.MinValue)
                {
                    break;
                }

                result.Add(para);
            }

            return result;
        }

        private static ConsultarRejeitadosRetornoDTO TraduzirRetornoConsultarRejeitados(ResumoVendaRejeitado de)
        {
            DateTime dataComprovanteVenda;
            DateTime.TryParseExact(de.DataComprovanteVenda, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dataComprovanteVenda);

            ConsultarRejeitadosRetornoDTO para = new ConsultarRejeitadosRetornoDTO();
            para.Autorizacao = de.Autorizacao;
            para.Cartao = de.Cartao;
            para.DataComprovanteVenda = dataComprovanteVenda;
            para.Descricao = de.Descricao;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.Sequencia = de.Sequencia;
            para.Valor = de.Valor;
            para.IndicadorTokenizacao = de.WACA705_ID_TOK;

            return para;
        }
        #endregion
    }
}
