using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarRetencao
    {
        public static ConsultarRetencaoEnvioDTO TraduzirEnvioConsultarRetencao(ConsultarRetencaoEnvio de)
        {
            ConsultarRetencaoEnvioDTO para = new ConsultarRetencaoEnvioDTO();
            para.DataInicial = de.DataInicial;
            para.DataFinal = de.DataFinal;
            para.CodigoBandeira = de.CodigoBandeira;
            para.Estabelecimentos = de.Estabelecimentos;

            return para;
        }

        public static ConsultarRetencaoRetorno TraduzirRetornoConsultarRetencao(RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarRetencaoTotaisRetornoDTO> retornoPesquisa)
        {
            ConsultarRetencaoRetorno para = new ConsultarRetencaoRetorno();
            para.Registros = TraduzirRetornoListaConsultarRetencao(retornoPesquisa.Registros);
            para.Totais = TraduzirRetornoConsultarRetencaoTotais(retornoPesquisa.Totalizador);
            para.QuantidadeTotalRegistros = retornoPesquisa.QuantidadeTotalRegistros;

            return para;
        }

        private static ConsultarRetencaoTotaisRetorno TraduzirRetornoConsultarRetencaoTotais(ConsultarRetencaoTotaisRetornoDTO de)
        {
            ConsultarRetencaoTotaisRetorno para = new ConsultarRetencaoTotaisRetorno();
            para.TotalTransacoes = de.TotalTransacoes;
            para.TotalProcessos = de.TotalProcessos;
            para.TotalValorProcesso = de.TotalValorProcesso;
            para.TotalValorRetencao = de.TotalValorRetencao;

            return para;
        }

        /// <summary>
        /// PR - ConsultarRetencaoNumeroProcessoRetorno, DC - ConsultarRetencaoDetalheProcessoCreditoRetorno, DD - ConsultarRetencaoDetalheProcessoDebitoRetorno, D1 - ConsultarRetencaoDescricaoComValorRetorno, D2 - ConsultarRetencaoDescricaoSemValorRetorno
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<BasicContract> TraduzirRetornoListaConsultarRetencao(List<BasicDTO> list)
        {
            List<BasicContract> result = new List<BasicContract>();

            foreach (BasicDTO de in list)
            {
                string tipoRegistro = de.TipoRegistro;

                if (tipoRegistro == "PR")
                {
                    ConsultarRetencaoNumeroProcessoRetorno para = TraduzirRetornoConsultarRetencaoNumeroProcesso((ConsultarRetencaoNumeroProcessoRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "DC")
                {
                    ConsultarRetencaoDetalheProcessoCreditoRetorno para = TraduzirRetornoConsultarRetencaoDetalheProcessoCredito((ConsultarRetencaoDetalheProcessoCreditoRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "DD")
                {
                    ConsultarRetencaoDetalheProcessoDebitoRetorno para = TraduzirRetornoConsultarRetencaoDetalheProcessoDebito((ConsultarRetencaoDetalheProcessoDebitoRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "D1" || tipoRegistro == "DCT_D1" || tipoRegistro == "DDT_D1")
                {
                    ConsultarRetencaoDescricaoComValorRetorno para = TraduzirRetornoConsultarRetencaoDescricaoComValor((ConsultarRetencaoDescricaoComValorRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "D2" || tipoRegistro == "DCT_D2" || tipoRegistro == "DDT_D2")
                {
                    ConsultarRetencaoDescricaoSemValorRetorno para = TraduzirRetornoConsultarRetencaoDescricaoSemValor((ConsultarRetencaoDescricaoSemValorRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "HDD1" || tipoRegistro == "HDD2" || tipoRegistro == "HDC1" || tipoRegistro == "HDC2")
                {
                    result.Add(new BasicContract() { TipoRegistro = tipoRegistro });
                }
            }

            return result;
        }

        private static ConsultarRetencaoNumeroProcessoRetorno TraduzirRetornoConsultarRetencaoNumeroProcesso(ConsultarRetencaoNumeroProcessoRetornoDTO de)
        {
            ConsultarRetencaoNumeroProcessoRetorno para = new ConsultarRetencaoNumeroProcessoRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.NumeroProcesso = de.NumeroProcesso;
            para.ValorTotalProcesso = de.ValorTotalProcesso;
            para.QuantidadeDetalheProcesso = de.QuantidadeDetalheProcesso;

            return para;
        }

        private static ConsultarRetencaoDetalheProcessoCreditoRetorno TraduzirRetornoConsultarRetencaoDetalheProcessoCredito(ConsultarRetencaoDetalheProcessoCreditoRetornoDTO de)
        {
            ConsultarRetencaoDetalheProcessoCreditoRetorno para = new ConsultarRetencaoDetalheProcessoCreditoRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataProcesso = de.DataProcesso;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorRetencao = de.ValorRetencao;

            return para;
        }

        private static ConsultarRetencaoDetalheProcessoDebitoRetorno TraduzirRetornoConsultarRetencaoDetalheProcessoDebito(ConsultarRetencaoDetalheProcessoDebitoRetornoDTO de)
        {
            ConsultarRetencaoDetalheProcessoDebitoRetorno para = new ConsultarRetencaoDetalheProcessoDebitoRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataProcesso = de.DataProcesso;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorRetencao = de.ValorRetencao;

            return para;
        }

        private static ConsultarRetencaoDescricaoComValorRetorno TraduzirRetornoConsultarRetencaoDescricaoComValor(ConsultarRetencaoDescricaoComValorRetornoDTO de)
        {
            ConsultarRetencaoDescricaoComValorRetorno para = new ConsultarRetencaoDescricaoComValorRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataProcesso = de.DataProcesso;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorRetencao = de.ValorRetencao;

            return para;
        }

        private static ConsultarRetencaoDescricaoSemValorRetorno TraduzirRetornoConsultarRetencaoDescricaoSemValor(ConsultarRetencaoDescricaoSemValorRetornoDTO de)
        {
            ConsultarRetencaoDescricaoSemValorRetorno para = new ConsultarRetencaoDescricaoSemValorRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataProcesso = de.DataProcesso;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorRetencao = de.ValorRetencao;

            return para;
        }
    }
}