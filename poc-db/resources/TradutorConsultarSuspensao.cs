using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarSuspensao
    {
        public static ConsultarSuspensaoEnvioDTO TraduzirEnvioConsultarSuspensao(ConsultarSuspensaoEnvio de)
        {
            ConsultarSuspensaoEnvioDTO para = new ConsultarSuspensaoEnvioDTO();
            para.DataInicial = de.DataInicial;
            para.DataFinal = de.DataFinal;
            para.CodigoBandeira = de.CodigoBandeira;
            para.Estabelecimentos = de.Estabelecimentos;

            return para;
        }

        public static ConsultarSuspensaoRetorno TraduzirRetornoConsultarSuspensao(RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarSuspensaoTotaisRetornoDTO> retornoPesquisa)
        {
            ConsultarSuspensaoRetorno para = new ConsultarSuspensaoRetorno();
            para.Registros = TraduzirRetornoListaConsultarSuspensao(retornoPesquisa.Registros);
            para.Totais = TraduzirRetornoConsultarSuspensaoTotais(retornoPesquisa.Totalizador);
            para.QuantidadeTotalRegistros = retornoPesquisa.QuantidadeTotalRegistros;

            return para;
        }

        private static ConsultarSuspensaoTotaisRetorno TraduzirRetornoConsultarSuspensaoTotais(ConsultarSuspensaoTotaisRetornoDTO de)
        {
            ConsultarSuspensaoTotaisRetorno para = new ConsultarSuspensaoTotaisRetorno();
            para.TotalTransacoes = de.TotalTransacoes;
            para.TotalValorSuspencao = de.TotalValorSuspencao;

            return para;
        }

        private static ConsultarSuspensaoTotalBandeiraRetorno TraduzirRetornoConsultarSuspensaoTotalBandeira(ConsultarSuspensaoTotalBandeiraRetornoDTO de)
        {
            ConsultarSuspensaoTotalBandeiraRetorno para = new ConsultarSuspensaoTotalBandeiraRetorno();
            para.DataSuspensao = de.DataSuspensao;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorSuspensao = de.ValorSuspensao;
            para.CodigoBanco = de.CodigoBanco;
            para.CodigoAgencia = de.CodigoAgencia;
            para.NumeroConta = de.NumeroConta;

            return para;
        }

        public static List<BasicContract> TraduzirRetornoListaConsultarSuspensao(List<BasicDTO> list)
        {
            List<BasicContract> result = new List<BasicContract>();

            foreach (BasicDTO de in list)
            {
                string tipoRegistro = de.TipoRegistro;

                if (tipoRegistro == "DT")
                {
                    ConsultarSuspensaoDetalheRetorno para = TraduzirRetornoConsultarSuspensaoDetalhe((ConsultarSuspensaoDetalheRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "T1")
                {
                    ConsultarSuspensaoTotalBandeiraDiaRetorno para = TraduzirRetornoConsultarSuspensaoTotalBandeiraDia((ConsultarSuspensaoTotalBandeiraDiaRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "T2")
                {
                    ConsultarSuspensaoTotalDiaRetorno para = TraduzirRetornoConsultarSuspensaoTotalDia((ConsultarSuspensaoTotalDiaRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "T3")
                {
                    ConsultarSuspensaoTotalBandeiraRetorno para = TraduzirRetornoConsultarSuspensaoTotalBandeira((ConsultarSuspensaoTotalBandeiraRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "T4")
                {
                    ConsultarSuspensaoTotalPeriodoRetorno para = TraduzirRetornoConsultarSuspensaoTotalPeriodo((ConsultarSuspensaoTotalPeriodoRetornoDTO)de);

                    result.Add(para);
                }
            }

            return result;
        }

        private static ConsultarSuspensaoDetalheRetorno TraduzirRetornoConsultarSuspensaoDetalhe(ConsultarSuspensaoDetalheRetornoDTO de)
        {
            ConsultarSuspensaoDetalheRetorno para = new ConsultarSuspensaoDetalheRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataSuspensao = de.DataSuspensao;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorSuspensao = de.ValorSuspensao;
            para.CodigoBanco = de.CodigoBanco;
            para.CodigoAgencia = de.CodigoAgencia;
            para.NumeroConta = de.NumeroConta;

            return para;
        }

        private static ConsultarSuspensaoTotalBandeiraDiaRetorno TraduzirRetornoConsultarSuspensaoTotalBandeiraDia(ConsultarSuspensaoTotalBandeiraDiaRetornoDTO de)
        {
            ConsultarSuspensaoTotalBandeiraDiaRetorno para = new ConsultarSuspensaoTotalBandeiraDiaRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataSuspensao = de.DataSuspensao;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorSuspensao = de.ValorSuspensao;
            para.CodigoBanco = de.CodigoBanco;
            para.CodigoAgencia = de.CodigoAgencia;
            para.NumeroConta = de.NumeroConta;

            return para;
        }

        private static ConsultarSuspensaoTotalDiaRetorno TraduzirRetornoConsultarSuspensaoTotalDia(ConsultarSuspensaoTotalDiaRetornoDTO de)
        {
            ConsultarSuspensaoTotalDiaRetorno para = new ConsultarSuspensaoTotalDiaRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataSuspensao = de.DataSuspensao;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorSuspensao = de.ValorSuspensao;
            para.CodigoBanco = de.CodigoBanco;
            para.CodigoAgencia = de.CodigoAgencia;
            para.NumeroConta = de.NumeroConta;

            return para;
        }

        private static ConsultarSuspensaoTotalPeriodoRetorno TraduzirRetornoConsultarSuspensaoTotalPeriodo(ConsultarSuspensaoTotalPeriodoRetornoDTO de)
        {
            ConsultarSuspensaoTotalPeriodoRetorno para = new ConsultarSuspensaoTotalPeriodoRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataSuspensao = de.DataSuspensao;
            para.DataApresentacao = de.DataApresentacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroRV = de.NumeroRV;
            para.TipoBandeira = de.TipoBandeira;
            para.QuantidadeTransacoes = de.QuantidadeTransacoes;
            para.DescricaoResumo = de.DescricaoResumo;
            para.ValorSuspensao = de.ValorSuspensao;
            para.CodigoBanco = de.CodigoBanco;
            para.CodigoAgencia = de.CodigoAgencia;
            para.NumeroConta = de.NumeroConta;

            return para;
        }
    }
}