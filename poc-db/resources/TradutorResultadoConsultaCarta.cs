using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoConsultaCartas;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoConsultaCarta
    {
        public static List<ConsultarCartasRetornoDTO> TraduzirRetornoListaConsultarCartas(DadoCartaChargeback[] list, short quantidadeRegistros)
        {
            List<ConsultarCartasRetornoDTO> result = new List<ConsultarCartasRetornoDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                ConsultarCartasRetornoDTO para = TraduzirRetornoConsultarCartas(list[i]);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarCartasRetornoDTO TraduzirRetornoConsultarCartas(DadoCartaChargeback de)
        {
            DateTime dataCancelamento;
            if (!DateTime.TryParseExact(de.DataCancelamento, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dataCancelamento))
            {
                if (!DateTime.TryParseExact(de.DataCancelamento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCancelamento))
                {
                    DateTime.TryParseExact(de.DataCancelamento, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dataCancelamento);
                }
            }

            DateTime dataVenda;
            if(!DateTime.TryParseExact(de.DataVenda, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dataVenda))
            {
                if (!DateTime.TryParseExact(de.DataVenda, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVenda))
                {
                    DateTime.TryParseExact(de.DataVenda, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dataVenda);
                }
            }

            ConsultarCartasRetornoDTO para = new ConsultarCartasRetornoDTO();
            para.CodigoMotivo = de.CodigoMotivo;
            para.DataCancelamento = dataCancelamento;
            para.DataVenda = dataVenda;
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
