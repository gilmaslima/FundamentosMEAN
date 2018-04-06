using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarResumosDeVenda
    {
        public static ConsultarResumosDeVendaEnvioDTO TraduzirEnvioConsultarResumosDeVenda(ConsultarResumosDeVendaEnvio de)
        {
            ConsultarResumosDeVendaEnvioDTO para = new ConsultarResumosDeVendaEnvioDTO();
            para.NumeroResumoVenda = de.NumeroResumoVenda;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.DataApresentacao = de.DataApresentacao;

            return para;
        }

        public static List<ConsultarResumosDeVendaRetorno> TraduzirRetornoListaConsultarResumosDeVenda(List<ConsultarResumosDeVendaRetornoDTO> list)
        {
            List<ConsultarResumosDeVendaRetorno> result = new List<ConsultarResumosDeVendaRetorno>();

            foreach (ConsultarResumosDeVendaRetornoDTO de in list)
            {
                ConsultarResumosDeVendaRetorno para = TraduzirRetornoConsultarResumosDeVenda(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarResumosDeVendaRetorno TraduzirRetornoConsultarResumosDeVenda(ConsultarResumosDeVendaRetornoDTO de)
        {
            ConsultarResumosDeVendaRetorno para = new ConsultarResumosDeVendaRetorno();
            para.Detalhe = de.Detalhe;
            para.NumeroMes = de.NumeroMes;
            para.Timestamp = de.Timestamp;
            para.TipoResumoVenda = de.TipoResumoVenda;
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