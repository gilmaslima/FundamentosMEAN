using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarOrdensCreditoEnviadosAoBanco
    {
        public static ConsultarOrdensCreditoEnviadosAoBancoEnvioDTO TraduzirEnvioConsultarOrdensCreditoEnviadosAoBanco(ConsultarOrdensCreditoEnviadosAoBancoEnvio de)
        {
            ConsultarOrdensCreditoEnviadosAoBancoEnvioDTO para = new ConsultarOrdensCreditoEnviadosAoBancoEnvioDTO();
            para.DataInicial = de.DataInicial;
            para.DataFinal = de.DataFinal;
            para.CodigoBandeira = de.CodigoBandeira;
            para.Estabelecimentos = de.Estabelecimentos;

            return para;
        }

        public static ConsultarOrdensCreditoEnviadosAoBancoRetorno TraduzirRetornoConsultarOrdensCreditoEnviadosAoBanco(RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO> retornoPesquisa)
        {
            ConsultarOrdensCreditoEnviadosAoBancoRetorno para = new ConsultarOrdensCreditoEnviadosAoBancoRetorno();
            para.Registros = TraduzirRetornoListaConsultarOrdensCreditoEnviadosAoBanco(retornoPesquisa.Registros);
            para.Totais = TraduzirRetornoConsultarOrdensCreditoEnviadosAoBancoTotais(retornoPesquisa.Totalizador);
            para.QuantidadeTotalRegistros = retornoPesquisa.QuantidadeTotalRegistros;

            return para;
        }

        private static ConsultarOrdensCreditoEnviadosAoBancoTotaisRetorno TraduzirRetornoConsultarOrdensCreditoEnviadosAoBancoTotais(ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO de)
        {
            ConsultarOrdensCreditoEnviadosAoBancoTotaisRetorno para = new ConsultarOrdensCreditoEnviadosAoBancoTotaisRetorno();
            para.TotalTransacoes = de.TotalTransacoes;
            para.TotalValorCredito = de.TotalValorCredito;

            return para;
        }

        public static List<BasicContract> TraduzirRetornoListaConsultarOrdensCreditoEnviadosAoBanco(List<BasicDTO> list)
        {
            List<BasicContract> result = new List<BasicContract>();

            foreach (BasicDTO de in list)
            {
                string tipoRegistro = de.TipoRegistro;

                if (tipoRegistro == "DT")
                {
                    ConsultarOrdensCreditoEnviadosAoBancoDetalheRetorno para = TraduzirRetornoConsultarOrdensCreditoEnviadosAoBancoDetalhe((ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO)de);

                    result.Add(para);
                }
                else if (tipoRegistro == "TB")
                {
                    ConsultarOrdensCreditoEnviadosAoBancoAjusteRetorno para = TraduzirRetornoConsultarOrdensCreditoEnviadosAoBancoAjuste((ConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO)de);
                }
            }

            return result;
        }

        private static ConsultarOrdensCreditoEnviadosAoBancoDetalheRetorno TraduzirRetornoConsultarOrdensCreditoEnviadosAoBancoDetalhe(ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO de)
        {
            ConsultarOrdensCreditoEnviadosAoBancoDetalheRetorno para = new ConsultarOrdensCreditoEnviadosAoBancoDetalheRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.DataEmissao = de.DataEmissao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroResumoVenda = de.NumeroResumoVenda;
            para.Tipobandeira = de.Tipobandeira;
            para.StatusOcorrenica = de.StatusOcorrenica;
            para.DescricaoResumoAjuste = de.DescricaoResumoAjuste;
            para.ValorCredito = de.ValorCredito;
            para.IndicadorSinalValor = de.IndicadorSinalValor;
            para.BancoCredito = de.BancoCredito;
            para.AgenciaCredito = de.AgenciaCredito;
            para.ContaCorrente = de.ContaCorrente;
            para.IndicadorRecarga = de.IndicadorRecarga;
            para.CodigoAjuste = de.CodigoAjuste;

            return para;
        }

        private static ConsultarOrdensCreditoEnviadosAoBancoAjusteRetorno TraduzirRetornoConsultarOrdensCreditoEnviadosAoBancoAjuste(ConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO de)
        {
            ConsultarOrdensCreditoEnviadosAoBancoAjusteRetorno para = new ConsultarOrdensCreditoEnviadosAoBancoAjusteRetorno();
            para.TipoRegistro = de.TipoRegistro;
            para.TipoBandeira = de.TipoBandeira;
            para.DescricaoAjuste = de.DescricaoAjuste;
            para.TotalTransacoes = de.TotalTransacoes;
            para.TotalValorCredito = de.TotalValorCredito;

            return para;
        }
    }
}