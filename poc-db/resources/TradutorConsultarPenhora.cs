using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarPenhora
    {
        public static ConsultarPenhoraEnvioDTO TraduzirEnvioConsultarPenhora(ConsultarPenhoraEnvio de)
        {
            ConsultarPenhoraEnvioDTO para = new ConsultarPenhoraEnvioDTO();
            para.DataInicial = de.DataInicial;
            para.DataFinal = de.DataFinal;
            para.CodigoBandeira = de.CodigoBandeira;
            para.Estabelecimentos = de.Estabelecimentos;

            return para;
        }

        public static ConsultarPenhoraRetorno TraduzirRetornoConsultarPenhora(RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarPenhoraTotaisRetornoDTO> retornoPesquisa)
        {
            ConsultarPenhoraRetorno para = new ConsultarPenhoraRetorno();
            para.Registros = TraduzirRetornoListaConsultarPenhora(retornoPesquisa.Registros);
            para.Totais = TraduzirRetornoConsultarPenhoraTotais(retornoPesquisa.Totalizador);
            para.QuantidadeTotalRegistros = retornoPesquisa.QuantidadeTotalRegistros;

            return para;
        }

        private static ConsultarPenhoraTotaisRetorno TraduzirRetornoConsultarPenhoraTotais(ConsultarPenhoraTotaisRetornoDTO de)
        {
            ConsultarPenhoraTotaisRetorno para = new ConsultarPenhoraTotaisRetorno();
            para.TotalTransacoes = de.TotalTransacoes;
            para.TotalProcessos = de.TotalProcessos;
            para.TotalValorProcesso = de.TotalValorProcesso;
            para.TotalValorPenhorado = de.TotalValorPenhorado;
            
            return para;
        }

        /// <summary>
        /// PR - ConsultarPenhoraNumeroProcessoRetorno, DT - ConsultarPenhoraDetalheProcessoCreditoRetorno, T1 - ConsultarPenhoraTotalBandeiraRetorno, TP - ConsultarPenhoraTotalSemBandeiraRetorno
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<BasicContract> TraduzirRetornoListaConsultarPenhora(List<BasicDTO> list)
        {
            List<BasicContract> result = new List<BasicContract>();

            foreach (BasicDTO de in list)
            {
                string tipoRegistro = de.TipoRegistro;

                if (tipoRegistro == "PR")
                {
                    ConsultarPenhoraNumeroProcessoRetorno para = TraduzirRetornoConsultarPenhoraNumeroProcesso((ConsultarPenhoraNumeroProcessoRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "DT")
                {
                    ConsultarPenhoraDetalheProcessoCreditoRetorno para = TraduzirRetornoConsultarPenhoraDetalheProcessoCredito((ConsultarPenhoraDetalheProcessoCreditoRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "T1")
                {
                    ConsultarPenhoraTotalBandeiraRetorno para = TraduzirRetornoConsultarPenhoraTotalBandeira((ConsultarPenhoraTotalBandeiraRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "TP")
                {
                    ConsultarPenhoraTotalSemBandeiraRetorno para = TraduzirRetornoConsultarPenhoraTotalSemBandeira((ConsultarPenhoraTotalSemBandeiraRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "HDT")
                {
                    result.Add(new BasicContract() { TipoRegistro = tipoRegistro });
                }
            }

            return result;
        }

        private static ConsultarPenhoraNumeroProcessoRetorno TraduzirRetornoConsultarPenhoraNumeroProcesso(ConsultarPenhoraNumeroProcessoRetornoDTO de)
        {
            ConsultarPenhoraNumeroProcessoRetorno para = new ConsultarPenhoraNumeroProcessoRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.NumeroProcesso = de.NumeroProcesso;
            para.ValorTotalProcesso = de.ValorTotalProcesso;
            para.QuantidadeDetalheProcesso = de.QuantidadeDetalheProcesso;

            return para;
        }

        private static ConsultarPenhoraDetalheProcessoCreditoRetorno TraduzirRetornoConsultarPenhoraDetalheProcessoCredito(ConsultarPenhoraDetalheProcessoCreditoRetornoDTO de)
        {
            ConsultarPenhoraDetalheProcessoCreditoRetorno para = new ConsultarPenhoraDetalheProcessoCreditoRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataProcesso = de.DataProcesso;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorPenhorado = de.ValorPenhorado;

            return para;
        }

        private static ConsultarPenhoraTotalBandeiraRetorno TraduzirRetornoConsultarPenhoraTotalBandeira(ConsultarPenhoraTotalBandeiraRetornoDTO de)
        {
            ConsultarPenhoraTotalBandeiraRetorno para = new ConsultarPenhoraTotalBandeiraRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataProcesso = de.DataProcesso;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorPenhorado = de.ValorPenhorado;

            return para;
        }

        private static ConsultarPenhoraTotalSemBandeiraRetorno TraduzirRetornoConsultarPenhoraTotalSemBandeira(ConsultarPenhoraTotalSemBandeiraRetornoDTO de)
        {
            ConsultarPenhoraTotalSemBandeiraRetorno para = new ConsultarPenhoraTotalSemBandeiraRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataProcesso = de.DataProcesso;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorPenhorado = de.ValorPenhorado;

            return para;
        }
    }
}