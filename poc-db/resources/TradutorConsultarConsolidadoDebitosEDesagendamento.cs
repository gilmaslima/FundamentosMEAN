using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarConsolidadoDebitosEDesagendamento
    {
        public static ConsultarConsolidadoDebitosEDesagendamentoEnvioDTO TraduzirEnvioConsultarConsolidadoDebitosEDesagendamento(ConsultarConsolidadoDebitosEDesagendamentoEnvio de)
        {
            ConsultarConsolidadoDebitosEDesagendamentoEnvioDTO para = new ConsultarConsolidadoDebitosEDesagendamentoEnvioDTO();
            para.DataInicial = de.DataInicial;
            para.DataFinal = de.DataFinal;
            para.Estabelecimentos = de.Estabelecimentos;
            if(de.Versao.HasValue)
                para.Versao = de.Versao.ToString();

            return para;
        }

        public static ConsultarConsolidadoDebitosEDesagendamentoRetorno TraduzirRetornoConsultarConsolidadoDebitosEDesagendamento(ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO de)
        {
            ConsultarConsolidadoDebitosEDesagendamentoRetorno para = new ConsultarConsolidadoDebitosEDesagendamentoRetorno();
            para.ChavePesquisa = de.ChavePesquisa;
            para.ValorPendenteDebito = de.ValorPendenteDebito;
            para.ValorPendenteLiquido = de.ValorPendenteLiquido;
            para.ValorPendente = de.ValorPendente;
            para.ValorLiquidadoDebito = de.ValorLiquidadoDebito;
            para.ValorLiquidadoLiquido = de.ValorLiquidadoLiquido;

            return para;
        }
    }
}