using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoConsultaHomepage;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoConsultaHomepage
    {
        public static List<ConsultarTransacoesCreditoDebitoRetornoDTO> TraduzirRetornoListaConsultarTransacoesCreditoDebito(DadoValor[] list, int quantidadeRegistros)
        {
            List<ConsultarTransacoesCreditoDebitoRetornoDTO> result = new List<ConsultarTransacoesCreditoDebitoRetornoDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                ConsultarTransacoesCreditoDebitoRetornoDTO para = TraduzirRetornoConsultarTransacoesCreditoDebito(list[i]);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarTransacoesCreditoDebitoRetornoDTO TraduzirRetornoConsultarTransacoesCreditoDebito(DadoValor de)
        {
            DateTime data;
            DateTime.TryParseExact(de.Data, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out data);

            ConsultarTransacoesCreditoDebitoRetornoDTO para = new ConsultarTransacoesCreditoDebitoRetornoDTO();
            para.Data = data;
            para.ValorCredito = de.ValorCredito;
            para.ValorDebito = de.ValorDebito;

            return para;
        }
    }
}
