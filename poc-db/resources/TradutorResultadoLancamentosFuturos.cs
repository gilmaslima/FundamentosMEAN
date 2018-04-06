using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoConsultaLancamentosFuturos;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoLancamentosFuturos
    {
        public static List<ConsultarCreditoDebitoRetornoDTO> TraduzirRetornoListaConsultarCreditoDebito(LancamentoFuturo[] list, int quantidadeRegistros)
        {
            List<ConsultarCreditoDebitoRetornoDTO> result = new List<ConsultarCreditoDebitoRetornoDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                ConsultarCreditoDebitoRetornoDTO para = TraduzirRetornoConsultarCreditoDebito(list[i]);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarCreditoDebitoRetornoDTO TraduzirRetornoConsultarCreditoDebito(LancamentoFuturo de)
        {
            DateTime data;
            DateTime.TryParseExact(de.Data, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out data);

            ConsultarCreditoDebitoRetornoDTO para = new ConsultarCreditoDebitoRetornoDTO();
            para.Data = data;
            para.ValorCredito = de.ValorCredito;
            para.ValorDebito = de.ValorDebito;

            return para;
        }

    }
}
