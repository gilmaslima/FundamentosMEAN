using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarDetalhesResumoDeVenda
    {
        public static ConsultarDetalhesResumoDeVendaEnvioDTO TraduzirEnvioConsultarDetalhesResumoDeVenda(ConsultarDetalhesResumoDeVendaEnvio de)
        {
            ConsultarDetalhesResumoDeVendaEnvioDTO para = new ConsultarDetalhesResumoDeVendaEnvioDTO();
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroResumoVenda = de.NumeroResumoVenda;
            para.DataApresentacao = de.DataApresentacao;

            return para;
        }

        public static List<ConsultarDetalhesResumoDeVendaRetorno> TraduzirRetornoListaConsultarDetalhesResumoDeVenda(List<ConsultarDetalhesResumoDeVendaRetornoDTO> list)
        {
            List<ConsultarDetalhesResumoDeVendaRetorno> result = new List<ConsultarDetalhesResumoDeVendaRetorno>();

            foreach (ConsultarDetalhesResumoDeVendaRetornoDTO de in list)
            {
                ConsultarDetalhesResumoDeVendaRetorno para = TraduzirRetornoConsultarDetalhesResumoDeVenda(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarDetalhesResumoDeVendaRetorno TraduzirRetornoConsultarDetalhesResumoDeVenda(ConsultarDetalhesResumoDeVendaRetornoDTO de)
        {
            ConsultarDetalhesResumoDeVendaRetorno para = new ConsultarDetalhesResumoDeVendaRetorno();
            // WACA700
            para.Detalhe = de.Detalhe;
            para.NumeroMes = de.NumeroMes;
            para.Timestamp = de.Timestamp;
            para.TipoResumoVenda = de.TipoResumoVenda;

            // WACA701
            para.ResumoVenda = de.ResumoVenda;
            para.ValorApresentado = de.ValorApresentado;
            para.QuantidadeComprovantesVenda = de.QuantidadeComprovantesVenda;
            para.ValorApurado = de.ValorApurado;
            para.DataApresentacaoRetornado = de.DataApresentacaoRetornado;
            para.ValorDesconto = de.ValorDesconto;
            para.DataProcessamento = de.DataProcessamento;
            para.ValorGorjetaTaxaEmbarque = de.ValorGorjetaTaxaEmbarque;
            para.TipoResumo = de.TipoResumo;
            para.ValorCotacao = de.ValorCotacao;
            para.TipoMoeda = de.TipoMoeda;
            para.IndicadorTaxaEmbarque = de.IndicadorTaxaEmbarque;

            return para;
        }
    }
}