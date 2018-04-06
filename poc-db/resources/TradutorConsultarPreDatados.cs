using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    /// <summary>
    /// WACA617 - Resumo de vendas - Cartões de débito.
    /// </summary>
    public static class TradutorConsultarPreDatados
    {
        public static ConsultarPreDatadosEnvioDTO TraduzirEnvioConsultarPreDatados(ConsultarPreDatadosEnvio de)
        {
            ConsultarPreDatadosEnvioDTO para = new ConsultarPreDatadosEnvioDTO();
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroResumoVenda = de.NumeroResumoVenda;
            para.DataApresentacao = de.DataApresentacao;

            return para;
        }

        public static List<ConsultarPreDatadosRetorno> TraduzirRetornoListaConsultarPreDatados(List<ConsultarPreDatadosRetornoDTO> list)
        {
            List<ConsultarPreDatadosRetorno> result = new List<ConsultarPreDatadosRetorno>();

            foreach (ConsultarPreDatadosRetornoDTO de in list)
            {
                ConsultarPreDatadosRetorno para = TraduzirRetornoConsultarPreDatados(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarPreDatadosRetorno TraduzirRetornoConsultarPreDatados(ConsultarPreDatadosRetornoDTO de)
        {
            ConsultarPreDatadosRetorno para = new ConsultarPreDatadosRetorno();
            para.CodigoSubTransacao = de.CodigoSubTransacao;
            para.CodigoTransacao = de.CodigoTransacao;
            para.DataVencimento = de.DataVencimento;
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
    }
}