using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarDisponibilidadeVencimentosODC
    {
        public static ConsultarDisponibilidadeVencimentosODCEnvioDTO TraduzirEnvioConsultarDisponibilidadeVencimentosODC(ConsultarDisponibilidadeVencimentosODCEnvio de)
        {
            ConsultarDisponibilidadeVencimentosODCEnvioDTO para = new ConsultarDisponibilidadeVencimentosODCEnvioDTO();
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroResumoVenda = de.NumeroResumoVenda;
            para.DataApresentacao = de.DataApresentacao;
            para.ChaveContinua = de.ChaveContinua;

            return para;
        }

        public static List<ConsultarDisponibilidadeVencimentosODCRetorno> TraduzirRetornoListaConsultarDisponibilidadeVencimentosODC(List<ConsultarDisponibilidadeVencimentosODCRetornoDTO> list)
        {
            List<ConsultarDisponibilidadeVencimentosODCRetorno> result = new List<ConsultarDisponibilidadeVencimentosODCRetorno>();

            foreach (ConsultarDisponibilidadeVencimentosODCRetornoDTO de in list)
            {
                ConsultarDisponibilidadeVencimentosODCRetorno para = TraduzirRetornoConsultarDisponibilidadeVencimentosODC(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarDisponibilidadeVencimentosODCRetorno TraduzirRetornoConsultarDisponibilidadeVencimentosODC(ConsultarDisponibilidadeVencimentosODCRetornoDTO de)
        {
            ConsultarDisponibilidadeVencimentosODCRetorno para = new ConsultarDisponibilidadeVencimentosODCRetorno();
            para.DataAntecipacao = de.DataAntecipacao;
            para.DataVencimento = de.DataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroOdc = de.NumeroOdc;
            para.NomeEstabelecimento = de.NomeEstabelecimento;
            para.PrazoRecebimento = de.PrazoRecebimento;
            para.Status = de.Status;
            para.ValorOrdemCredito = de.ValorOrdemCredito;

            return para;
        }
    }
}