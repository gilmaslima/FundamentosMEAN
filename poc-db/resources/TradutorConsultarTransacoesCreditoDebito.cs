using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarTransacoesCreditoDebito
    {
        public static ConsultarTransacoesCreditoDebitoEnvioDTO TraduzirEnvioConsultarTransacoesCreditoDebito(ConsultarTransacoesCreditoDebitoEnvio de)
        {
            ConsultarTransacoesCreditoDebitoEnvioDTO para = new ConsultarTransacoesCreditoDebitoEnvioDTO();
            para.DataInicial = de.DataInicial;
            para.DataFinal = de.DataFinal;
            para.Estabelecimentos = de.Estabelecimentos;

            return para;
        }

        public static List<ConsultarTransacoesCreditoDebitoRetorno> TraduzirRetornoListaConsultarTransacoesCreditoDebito(List<ConsultarTransacoesCreditoDebitoRetornoDTO> list)
        {
            List<ConsultarTransacoesCreditoDebitoRetorno> result = new List<ConsultarTransacoesCreditoDebitoRetorno>();

            foreach (ConsultarTransacoesCreditoDebitoRetornoDTO de in list)
            {
                ConsultarTransacoesCreditoDebitoRetorno para = TraduzirRetornoConsultarTransacoesCreditoDebito(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarTransacoesCreditoDebitoRetorno TraduzirRetornoConsultarTransacoesCreditoDebito(ConsultarTransacoesCreditoDebitoRetornoDTO de)
        {
            ConsultarTransacoesCreditoDebitoRetorno para = new ConsultarTransacoesCreditoDebitoRetorno();
            para.Data = de.Data;
            para.ValorCredito = de.ValorCredito;
            para.ValorDebito = de.ValorDebito;

            return para;
        }
    }
}