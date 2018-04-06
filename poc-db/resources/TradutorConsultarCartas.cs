using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarCartas
    {
        public static ConsultarCartasEnvioDTO TraduzirEnvioConsultarCartas(ConsultarCartasEnvio de)
        {
            ConsultarCartasEnvioDTO para = new ConsultarCartasEnvioDTO();
            para.NumeroProcesso = de.NumeroProcesso;
            para.TimestampTransacao = de.TimestampTransacao;
            para.SistemaDados = de.SistemaDados;

            return para;
        }

        public static List<ConsultarCartasRetorno> TraduzirRetornoListaConsultarCartas(List<ConsultarCartasRetornoDTO> list)
        {
            List<ConsultarCartasRetorno> result = new List<ConsultarCartasRetorno>();

            foreach (ConsultarCartasRetornoDTO de in list)
            {
                ConsultarCartasRetorno para = TraduzirRetornoConsultarCartas(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarCartasRetorno TraduzirRetornoConsultarCartas(ConsultarCartasRetornoDTO de)
        {
            ConsultarCartasRetorno para = new ConsultarCartasRetorno();
            para.CodigoMotivo = de.CodigoMotivo;
            para.DataCancelamento = de.DataCancelamento;
            para.DataVenda = de.DataVenda;
            para.DescricaoMotivo = de.DescricaoMotivo;
            para.NumeroCartao = de.NumeroCartao;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroNsu = de.NumeroNsu;
            para.NumeroProcesso = de.NumeroProcesso;
            para.NumeroResumo = de.NumeroResumo;
            para.ValorAjuste = de.ValorAjuste;
            para.ValorCancelamento = de.ValorCancelamento;
            para.ValorDebito = de.ValorDebito;
            para.ValorTransacao = de.ValorTransacao;

            return para;
        }
    }
}