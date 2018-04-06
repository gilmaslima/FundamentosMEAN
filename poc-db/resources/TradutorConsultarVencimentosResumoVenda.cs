using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    /// <summary>
    /// WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
    /// </summary>
    public static class TradutorConsultarVencimentosResumoVenda
    {
        public static ConsultarVencimentosResumoVendaEnvioDTO TraduzirEnvioConsultarVencimentosResumoVenda(ConsultarVencimentosResumoVendaEnvio de)
        {
            ConsultarVencimentosResumoVendaEnvioDTO para = new ConsultarVencimentosResumoVendaEnvioDTO();
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroResumoVenda = de.NumeroResumoVenda;
            para.DataApresentacao = de.DataApresentacao;

            return para;
        }

        public static List<ConsultarVencimentosResumoVendaRetorno> TraduzirRetornoListaConsultarVencimentosResumoVenda(List<ConsultarVencimentosResumoVendaRetornoDTO> list)
        {
            List<ConsultarVencimentosResumoVendaRetorno> result = new List<ConsultarVencimentosResumoVendaRetorno>();

            foreach (ConsultarVencimentosResumoVendaRetornoDTO de in list)
            {
                ConsultarVencimentosResumoVendaRetorno para = TraduzirRetornoConsultarVencimentosResumoVenda(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarVencimentosResumoVendaRetorno TraduzirRetornoConsultarVencimentosResumoVenda(ConsultarVencimentosResumoVendaRetornoDTO de)
        {
            ConsultarVencimentosResumoVendaRetorno para = new ConsultarVencimentosResumoVendaRetorno();
            para.DataVencimento = de.DataVencimento;
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
    }
}