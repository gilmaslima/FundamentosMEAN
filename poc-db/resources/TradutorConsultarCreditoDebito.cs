using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarCreditoDebito
    {
        public static ConsultarCreditoDebitoEnvioDTO TraduzirEnvioConsultarCreditoDebito(ConsultarCreditoDebitoEnvio de)
        {
            ConsultarCreditoDebitoEnvioDTO para = new ConsultarCreditoDebitoEnvioDTO();
            para.DataInicial = de.DataInicial;
            para.DataFinal = de.DataFinal;
            para.Estabelecimentos = de.Estabelecimentos;

            return para;
        }

        public static List<ConsultarCreditoDebitoRetorno> TraduzirRetornoListaConsultarCreditoDebito(List<ConsultarCreditoDebitoRetornoDTO> list)
        {
            List<ConsultarCreditoDebitoRetorno> result = new List<ConsultarCreditoDebitoRetorno>();

            foreach (ConsultarCreditoDebitoRetornoDTO de in list)
            {
                ConsultarCreditoDebitoRetorno para = TraduzirRetornoConsultarCreditoDebito(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarCreditoDebitoRetorno TraduzirRetornoConsultarCreditoDebito(ConsultarCreditoDebitoRetornoDTO de)
        {
            ConsultarCreditoDebitoRetorno para = new ConsultarCreditoDebitoRetorno();
            para.Data = de.Data;
            para.ValorCredito = de.ValorCredito;
            para.ValorDebito = de.ValorDebito;

            return para;
        }
    }
}