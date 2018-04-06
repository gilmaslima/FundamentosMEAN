using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarRejeitados
    {
        public static ConsultarRejeitadosEnvioDTO TraduzirEnvioConsultarRejeitados(ConsultarRejeitadosEnvio de)
        {
            ConsultarRejeitadosEnvioDTO para = new ConsultarRejeitadosEnvioDTO();
            para.Timestamp = de.Timestamp;
            para.TipoResumoVenda = de.TipoResumoVenda;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroResumoVenda = de.NumeroResumoVenda;
            para.DataApresentacao = de.DataApresentacao;

            return para;
        }

        public static List<ConsultarRejeitadosRetorno> TraduzirRetornoListaConsultarRejeitados(List<ConsultarRejeitadosRetornoDTO> list)
        {
            List<ConsultarRejeitadosRetorno> result = new List<ConsultarRejeitadosRetorno>();

            foreach (ConsultarRejeitadosRetornoDTO de in list)
            {
                ConsultarRejeitadosRetorno para = TraduzirRetornoConsultarRejeitados(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarRejeitadosRetorno TraduzirRetornoConsultarRejeitados(ConsultarRejeitadosRetornoDTO de)
        {
            ConsultarRejeitadosRetorno para = new ConsultarRejeitadosRetorno();
            para.Autorizacao = de.Autorizacao;
            para.Cartao = de.Cartao;
            para.DataComprovanteVenda = de.DataComprovanteVenda;
            para.Descricao = de.Descricao;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.Sequencia = de.Sequencia;
            para.Valor = de.Valor;
            para.IndicadorTokenizacao = de.IndicadorTokenizacao;

            return para;
        }
    }
}