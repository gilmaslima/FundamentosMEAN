using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoConsultaSuspensao
    {
        #region WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos suspensos.
        public static ConsultarSuspensaoRetornoDTO TraduzirRetornoConsultarSuspensao(string dados)
        {
            CortadorMensagem cortador = new CortadorMensagem(dados);

            int quantidadeRegistros = cortador.LerInt32(6);

            if (quantidadeRegistros > 215)
            {
                quantidadeRegistros = 215;
            }

            string[] registros = cortador.LerOccurs(138, 215);

            ConsultarSuspensaoRetornoDTO result = new ConsultarSuspensaoRetornoDTO();
            result.Registros = ObterListaConsultarSuspensao(registros, quantidadeRegistros);
            result.Totais = ObterConsultarSuspensaoTotaisRetornoDTO(cortador);

            return result;
        }

        /// <summary>
        /// DT = ConsultarSuspensaoDetalheRetornoDTO, T1 = ConsultarSuspensaoTotalBandeiraDiaRetornoDTO, T2 = ConsultarSuspensaoTotalDiaRetornoDTO, T3 = ConsultarSuspensaoTotalBandeiraRetornoDTO, T4 = ConsultarSuspensaoTotalPeriodoRetornoDTO
        /// </summary>
        /// <param name="registros"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <returns></returns>
        private static List<BasicDTO> ObterListaConsultarSuspensao(string[] registros, int quantidadeRegistros)
        {
            List<BasicDTO> result = new List<BasicDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                string registro = registros[i];

                CortadorMensagem cortador = new CortadorMensagem(registro);

                string tipoRegistro = cortador.LerString(2);

                // DT = ConsultarSuspensaoDetalheRetornoDTO
                // T1 = ConsultarSuspensaoTotalBandeiraDiaRetornoDTO
                // T2 = ConsultarSuspensaoTotalDiaRetornoDTO
                // T4 = ConsultarSuspensaoTotalPeriodoRetornoDTO

                if (tipoRegistro == "DT")
                {
                    ConsultarSuspensaoDetalheRetornoDTO dto = ObterConsultarSuspensaoDetalheRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "T1")
                {
                    ConsultarSuspensaoTotalBandeiraDiaRetornoDTO dto = ObterConsultarSuspensaoTotalBandeiraDiaRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "T2")
                {
                    ConsultarSuspensaoTotalDiaRetornoDTO dto = ObterConsultarSuspensaoTotalDiaRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "T3")
                {
                    ConsultarSuspensaoTotalBandeiraRetornoDTO dto = ObterConsultarSuspensaoTotalBandeiraRetornoDTO(cortador);

                    result.Add(dto);
                }
                else if (tipoRegistro == "T4")
                {
                    ConsultarSuspensaoTotalPeriodoRetornoDTO dto = ObterConsultarSuspensaoTotalPeriodoRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
            }

            return result;
        }

        private static ConsultarSuspensaoDetalheRetornoDTO ObterConsultarSuspensaoDetalheRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarSuspensaoDetalheRetornoDTO result = new ConsultarSuspensaoDetalheRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataSuspensao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(36);
            result.ValorSuspensao = cortador.LerDecimal(15, 2);
            result.CodigoBanco = cortador.LerInt16(3);
            result.CodigoAgencia = cortador.LerInt16(5);
            result.NumeroConta = cortador.LerString(10);

            return result;
        }

        private static ConsultarSuspensaoTotalBandeiraDiaRetornoDTO ObterConsultarSuspensaoTotalBandeiraDiaRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarSuspensaoTotalBandeiraDiaRetornoDTO result = new ConsultarSuspensaoTotalBandeiraDiaRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataSuspensao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(36);
            result.ValorSuspensao = cortador.LerDecimal(15, 2);
            result.CodigoBanco = cortador.LerInt16(3);
            result.CodigoAgencia = cortador.LerInt16(5);
            result.NumeroConta = cortador.LerString(10);

            return result;
        }

        private static ConsultarSuspensaoTotalDiaRetornoDTO ObterConsultarSuspensaoTotalDiaRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarSuspensaoTotalDiaRetornoDTO result = new ConsultarSuspensaoTotalDiaRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataSuspensao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(36);
            result.ValorSuspensao = cortador.LerDecimal(15, 2);
            result.CodigoBanco = cortador.LerInt16(3);
            result.CodigoAgencia = cortador.LerInt16(5);
            result.NumeroConta = cortador.LerString(10);

            return result;
        }

        private static ConsultarSuspensaoTotalPeriodoRetornoDTO ObterConsultarSuspensaoTotalPeriodoRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarSuspensaoTotalPeriodoRetornoDTO result = new ConsultarSuspensaoTotalPeriodoRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataSuspensao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(36);
            result.ValorSuspensao = cortador.LerDecimal(15, 2);
            result.CodigoBanco = cortador.LerInt16(3);
            result.CodigoAgencia = cortador.LerInt16(5);
            result.NumeroConta = cortador.LerString(10);

            return result;
        }

        private static ConsultarSuspensaoTotalBandeiraRetornoDTO ObterConsultarSuspensaoTotalBandeiraRetornoDTO(CortadorMensagem cortador)
        {
            ConsultarSuspensaoTotalBandeiraRetornoDTO result = new ConsultarSuspensaoTotalBandeiraRetornoDTO();
            result.DataSuspensao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(36);
            result.ValorSuspensao = cortador.LerDecimal(15, 2);
            result.CodigoBanco = cortador.LerInt16(3);
            result.CodigoAgencia = cortador.LerInt16(5);
            result.NumeroConta = cortador.LerString(10);

            return result;
        }

        private static ConsultarSuspensaoTotaisRetornoDTO ObterConsultarSuspensaoTotaisRetornoDTO(CortadorMensagem cortador)
        {
            ConsultarSuspensaoTotaisRetornoDTO result = new ConsultarSuspensaoTotaisRetornoDTO();
            result.TotalTransacoes = cortador.LerInt32(5);
            result.TotalValorSuspencao = cortador.LerDecimal(15, 2);

            return result;
        }
        #endregion

        #region WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
        public static ConsultarPenhoraRetornoDTO TraduzirRetornoConsultarPenhora(string dados)
        {
            CortadorMensagem cortador = new CortadorMensagem(dados);

            int quantidadeRegistros = cortador.LerInt32(6);

            if (quantidadeRegistros > 190)
            {
                quantidadeRegistros = 190;
            }

            string[] registros = cortador.LerOccurs(154, 190);

            ConsultarPenhoraRetornoDTO result = new ConsultarPenhoraRetornoDTO();
            result.Registros = ObterListaConsultarPenhora(registros, quantidadeRegistros);
            result.Totais = ObterConsultarPenhoraTotaisRetornoDTO(cortador);

            return result;
        }

        /// <summary>
        /// PR - ConsultarPenhoraNumeroProcessoRetornoDTO, DT - ConsultarPenhoraDetalheProcessoCreditoRetornoDTO, T1 - ConsultarPenhoraTotalBandeiraRetornoDTO, TP - ConsultarPenhoraTotalSemBandeiraRetornoDTO
        /// </summary>
        /// <param name="registros"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <returns></returns>
        private static List<BasicDTO> ObterListaConsultarPenhora(string[] registros, int quantidadeRegistros)
        {
            List<BasicDTO> result = new List<BasicDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                string registro = registros[i];

                CortadorMensagem cortador = new CortadorMensagem(registro);

                string tipoRegistro = cortador.LerString(2);

                if (tipoRegistro == "PR")
                {
                    ConsultarPenhoraNumeroProcessoRetornoDTO dto = ObterConsultarPenhoraNumeroProcessoRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "DT")
                {
                    ConsultarPenhoraDetalheProcessoCreditoRetornoDTO dto = ObterConsultarPenhoraDetalheProcessoCreditoRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "T1")
                {
                    ConsultarPenhoraTotalBandeiraRetornoDTO dto = ObterConsultarPenhoraTotalBandeiraRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "TP")
                {
                    ConsultarPenhoraTotalSemBandeiraRetornoDTO dto = ObterConsultarPenhoraTotalSemBandeiraRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
            }

            return result;
        }

        private static ConsultarPenhoraNumeroProcessoRetornoDTO ObterConsultarPenhoraNumeroProcessoRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarPenhoraNumeroProcessoRetornoDTO result = new ConsultarPenhoraNumeroProcessoRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.NumeroProcesso = cortador.LerString(15);
            result.ValorTotalProcesso = cortador.LerDecimal(15, 2);
            result.QuantidadeDetalheProcesso = cortador.LerInt32(5);
            cortador.LerString(115); // filler.

            return result;
        }

        private static ConsultarPenhoraDetalheProcessoCreditoRetornoDTO ObterConsultarPenhoraDetalheProcessoCreditoRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarPenhoraDetalheProcessoCreditoRetornoDTO result = new ConsultarPenhoraDetalheProcessoCreditoRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataProcesso = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(70);
            result.ValorPenhorado = cortador.LerDecimal(15, 2);

            return result;
        }

        private static ConsultarPenhoraTotalBandeiraRetornoDTO ObterConsultarPenhoraTotalBandeiraRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarPenhoraTotalBandeiraRetornoDTO result = new ConsultarPenhoraTotalBandeiraRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataProcesso = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(70);
            result.ValorPenhorado = cortador.LerDecimal(15, 2);

            return result;
        }

        private static ConsultarPenhoraTotalSemBandeiraRetornoDTO ObterConsultarPenhoraTotalSemBandeiraRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarPenhoraTotalSemBandeiraRetornoDTO result = new ConsultarPenhoraTotalSemBandeiraRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataProcesso = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(70);
            result.ValorPenhorado = cortador.LerDecimal(15, 2);

            return result;
        }

        private static ConsultarPenhoraTotaisRetornoDTO ObterConsultarPenhoraTotaisRetornoDTO(CortadorMensagem cortador)
        {
            ConsultarPenhoraTotaisRetornoDTO result = new ConsultarPenhoraTotaisRetornoDTO();
            result.TotalTransacoes = cortador.LerInt32(5);
            result.TotalProcessos = cortador.LerInt32(5);
            result.TotalValorProcesso = cortador.LerDecimal(15, 2);
            result.TotalValorPenhorado = cortador.LerDecimal(15, 2);

            return result;
        }
        #endregion

        #region WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
        public static ConsultarRetencaoRetornoDTO TraduzirRetornoConsultarRetencao(string dados)
        {
            CortadorMensagem cortador = new CortadorMensagem(dados);

            int quantidadeRegistros = cortador.LerInt32(6);

            if (quantidadeRegistros > 180)
            {
                quantidadeRegistros = 180;
            }

            string[] registros = cortador.LerOccurs(164, 180);

            ConsultarRetencaoRetornoDTO result = new ConsultarRetencaoRetornoDTO();
            result.Registros = ObterListaConsultarRetencao(registros, quantidadeRegistros);
            result.Totais = ObterConsultarRetencaoTotaisRetornoDTO(cortador);

            return result;
        }

        /// <summary>
        /// PR - ConsultarRetencaoNumeroProcessoRetornoDTO, DC - ConsultarRetencaoDetalheProcessoCreditoRetornoDTO, DD - ConsultarRetencaoDetalheProcessoDebitoRetornoDTO, D1 - ConsultarRetencaoDescricaoComValorRetornoDTO, D2 - ConsultarRetencaoDescricaoSemValorRetornoDTO
        /// </summary>
        /// <param name="registros"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <returns></returns>
        private static List<BasicDTO> ObterListaConsultarRetencao(string[] registros, int quantidadeRegistros)
        {
            List<BasicDTO> result = new List<BasicDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                string registro = registros[i];

                CortadorMensagem cortador = new CortadorMensagem(registro);

                string tipoRegistro = cortador.LerString(2);

                if (tipoRegistro == "PR")
                {
                    ConsultarRetencaoNumeroProcessoRetornoDTO dto = ObterConsultarRetencaoNumeroProcessoRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "DC")
                {
                    ConsultarRetencaoDetalheProcessoCreditoRetornoDTO dto = ObterConsultarRetencaoDetalheProcessoCreditoRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "DD")
                {
                    ConsultarRetencaoDetalheProcessoDebitoRetornoDTO dto = ObterConsultarRetencaoDetalheProcessoDebitoRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "D1")
                {
                    ConsultarRetencaoDescricaoComValorRetornoDTO dto = ObterConsultarRetencaoDescricaoComValorRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
                else if (tipoRegistro == "D2")
                {
                    ConsultarRetencaoDescricaoSemValorRetornoDTO dto = ObterConsultarRetencaoDescricaoSemValorRetornoDTO(cortador, tipoRegistro);

                    result.Add(dto);
                }
            }

            return result;
        }

        private static ConsultarRetencaoNumeroProcessoRetornoDTO ObterConsultarRetencaoNumeroProcessoRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarRetencaoNumeroProcessoRetornoDTO result = new ConsultarRetencaoNumeroProcessoRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.NumeroProcesso = cortador.LerString(15);
            result.ValorTotalProcesso = cortador.LerDecimal(15, 2);
            result.QuantidadeDetalheProcesso = cortador.LerInt32(5);

            return result;
        }

        private static ConsultarRetencaoDetalheProcessoCreditoRetornoDTO ObterConsultarRetencaoDetalheProcessoCreditoRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarRetencaoDetalheProcessoCreditoRetornoDTO result = new ConsultarRetencaoDetalheProcessoCreditoRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataProcesso = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(80);
            result.ValorRetencao = cortador.LerDecimal(15, 2);

            return result;
        }

        private static ConsultarRetencaoDetalheProcessoDebitoRetornoDTO ObterConsultarRetencaoDetalheProcessoDebitoRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarRetencaoDetalheProcessoDebitoRetornoDTO result = new ConsultarRetencaoDetalheProcessoDebitoRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataProcesso = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(80);
            result.ValorRetencao = cortador.LerDecimal(15, 2);

            return result;
        }

        private static ConsultarRetencaoDescricaoComValorRetornoDTO ObterConsultarRetencaoDescricaoComValorRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarRetencaoDescricaoComValorRetornoDTO result = new ConsultarRetencaoDescricaoComValorRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataProcesso = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(80);
            result.ValorRetencao = cortador.LerDecimal(15, 2);

            return result;
        }

        private static ConsultarRetencaoDescricaoSemValorRetornoDTO ObterConsultarRetencaoDescricaoSemValorRetornoDTO(CortadorMensagem cortador, string tipoRegistro)
        {
            ConsultarRetencaoDescricaoSemValorRetornoDTO result = new ConsultarRetencaoDescricaoSemValorRetornoDTO();
            result.TipoRegistro = tipoRegistro;
            result.DataProcesso = cortador.LerData(10, "dd.MM.yyyy");
            result.DataApresentacao = cortador.LerData(10, "dd.MM.yyyy");
            result.DataVencimento = cortador.LerData(10, "dd.MM.yyyy");
            result.NumeroEstabelecimento = cortador.LerInt32(9);
            result.NumeroRV = cortador.LerInt32(9);
            result.TipoBandeira = cortador.LerString(12);
            result.QuantidadeTransacoes = cortador.LerInt32(5);
            result.DescricaoResumo = cortador.LerString(80);
            result.ValorRetencao = cortador.LerDecimal(15, 2);

            return result;
        }

        private static ConsultarRetencaoTotaisRetornoDTO ObterConsultarRetencaoTotaisRetornoDTO(CortadorMensagem cortador)
        {
            ConsultarRetencaoTotaisRetornoDTO result = new ConsultarRetencaoTotaisRetornoDTO();
            result.TotalTransacoes = cortador.LerInt32(5);
            result.TotalProcessos = cortador.LerInt32(5);
            result.TotalValorProcesso = cortador.LerDecimal(15, 2);
            result.TotalValorRetencao = cortador.LerDecimal(15, 2);

            return result;
        }
        #endregion
    }
}
