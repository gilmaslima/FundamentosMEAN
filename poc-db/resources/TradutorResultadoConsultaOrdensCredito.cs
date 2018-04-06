using System;
using System.Collections.Generic;
using Redecard.PN.Extrato.Agentes.ServicoConsultaOrdensCredito;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoConsultaOrdensCredito
    {
        #region WACA1106 - Relatório de pagamentos ajustados.
        public static ConsultarOrdensCreditoEnviadosAoBancoRetornoDTO TraduzirRetornoResultadoConsultarOrdensCreditoEnviadosAoBanco(string dados)
        {
            CortadorMensagem cortadorRegistros = new CortadorMensagem(dados);

            Int32 quantidadeRegistros = cortadorRegistros.LerInt32(6);

            if (quantidadeRegistros > 170)
                quantidadeRegistros = 170;

            string[] registros = cortadorRegistros.LerOccurs(171, 170);

            ConsultarOrdensCreditoEnviadosAoBancoRetornoDTO result = new ConsultarOrdensCreditoEnviadosAoBancoRetornoDTO();
            result.Registros = ObterListaConsultarOrdensCreditoEnviadosAoBanco(registros, quantidadeRegistros);

            CortadorMensagem cortadorTotalizador = new CortadorMensagem(dados);
            cortadorTotalizador.LerInt32(6);
            cortadorTotalizador.LerOccurs(172, 170);

            result.Totais = ObterConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO(cortadorTotalizador);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registros"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <returns>DT - ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO, TB - ConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO</returns>
        private static List<BasicDTO> ObterListaConsultarOrdensCreditoEnviadosAoBanco(string[] registros, int quantidadeRegistros)
        {
            List<BasicDTO> result = new List<BasicDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                string registro = registros[i];

                CortadorMensagem cortador = new CortadorMensagem(registro);

                string tipoRegistro = cortador.LerString(2);

                if (tipoRegistro == "DT")
                {
                    ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO dto = ObterConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "TB")
                {
                    ConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO dto = ObterConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
            }

            return result;
        }

        private static ConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO ObterConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO result = new ConsultarOrdensCreditoEnviadosAoBancoAjusteRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            cortador.LerString(38); // filler.
            result.TipoBandeira = cortador.LerString(12);
            cortador.LerString(20); // filler.
            result.DescricaoAjuste = cortador.LerString(60);
            cortador.LerString(40); // filler.

            return result;
        }

        private static ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO ObterConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO result = new ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataEmissao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroResumoVenda = cortador.LerInt32(9);
            result.Tipobandeira = cortador.LerString(12);
            result.StatusOcorrenica = cortador.LerString(20);
            result.DescricaoResumoAjuste = cortador.LerString(60);
            result.ValorCredito = cortador.LerDecimal(15, 2);
            result.IndicadorSinalValor = cortador.LerString(1);
            if (result.IndicadorSinalValor == "D" || result.IndicadorSinalValor == "-")
                result.ValorCredito *= -1;
            result.BancoCredito = cortador.LerInt16(3);
            result.AgenciaCredito = cortador.LerInt32(5);
            result.ContaCorrente = cortador.LerString(10);
            result.CodigoAjuste = cortador.LerInt16(3);
            
            return result;
        }

        private static ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO ObterConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO(CortadorMensagem cortador)
        {
            ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO result = new ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO();
            result.TotalTransacoes = cortador.LerInt32(5);
            result.TotalValorCredito = cortador.LerDecimal(15, 2);

            return result;
        }
        #endregion
      
        #region WACA703 - Resumo de vendas - Cartões de crédito - Vencimentos.
        public static List<ConsultarDisponibilidadeVencimentosODCRetornoDTO> TraduzirRetornoListaConsultarDisponibilidadeVencimentosODC(ResumoVendaOrdemCredito[] list, int quantidadeOcorrencias)
        {
            List<ConsultarDisponibilidadeVencimentosODCRetornoDTO> result = new List<ConsultarDisponibilidadeVencimentosODCRetornoDTO>();

            for (int i = 0; i < quantidadeOcorrencias; i++)
            {
                ConsultarDisponibilidadeVencimentosODCRetornoDTO para = TraduzirRetornoConsultarDisponibilidadeVencimentosODC(list[i]);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarDisponibilidadeVencimentosODCRetornoDTO TraduzirRetornoConsultarDisponibilidadeVencimentosODC(ResumoVendaOrdemCredito de)
        {
            ConsultarDisponibilidadeVencimentosODCRetornoDTO para = new ConsultarDisponibilidadeVencimentosODCRetornoDTO();

            DateTime dataAntecipacao;
            DateTime.TryParseExact(de.DataAntecipacao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAntecipacao);

            DateTime dataVencimento;
            DateTime.TryParseExact(de.DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

            para.DataAntecipacao = dataAntecipacao;
            para.DataVencimento = dataVencimento;
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.NumeroOdc = de.NumeroOdc;
            para.NomeEstabelecimento = de.NomeEstabelecimento;
            para.PrazoRecebimento = de.PrazoRecebimento;
            para.Status = de.Status;
            para.ValorOrdemCredito = de.ValorOrdemCredito;

            return para;
        }
        #endregion
    }
}
