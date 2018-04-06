using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoConsultaSaldosEmAberto
    {

        public static List<BasicDTO> TraduzirResultadoDetalhe(String dados, ref Decimal valorTotalLiquido)
        {
            List<BasicDTO> retorno = new List<BasicDTO>();


            CortadorMensagem cortador = new CortadorMensagem(dados);

            Decimal totalLiquido = cortador.LerDecimal(15, 2);
            Int16 quantidadeRegistros = cortador.LerInt16(4);

            if (quantidadeRegistros > 400)
            {
                quantidadeRegistros = 400;
            }
            string[] registros = cortador.LerOccurs(73, 400);

            retorno = ObterDetalheSaldosEmAberto(registros, quantidadeRegistros);
            //retorno.Totais = new TotalBandeiraMesSaldosEmAberto();
            valorTotalLiquido += totalLiquido;

            //retorno.ValorLiquido = totalLiquido;
            //retorno.QuantidadeRegistros = quantidadeRegistros;


            return retorno;
        }

        public static List<BasicDTO> ObterDetalheSaldosEmAberto(string[] registros, Int16 quantidadeRegistros)
        {
            List<BasicDTO> lstTotais = new List<BasicDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {

                string registro = registros[i];


                CortadorMensagem cortador = new CortadorMensagem(registro);

                string tipoRegistro = cortador.LerString(2);

                if (tipoRegistro.Equals("DT"))
                {
                    ItemDetalheSaldosEmAbertoDTO item = ObterDetalheDaBandeira(cortador, tipoRegistro);
                    if (item.CodigoBanco != 0 && item.ValorBruto != 0 && item.ValorLiquido != 0)
                        lstTotais.Add(item);

                }
                if (tipoRegistro.Equals("T1"))
                {
                    TotalBandeiraMesSaldosEmAbertoDTO item = ObterTotaisBandeiraMes(cortador, tipoRegistro);
                    if (item.ValorLiquido != 0 && item.CodigoBandeira != 0)
                        lstTotais.Add(item);

                }

                if (tipoRegistro.Equals("T2"))
                {
                    TotalMesSaldosEmAbertoDTO item = ObterTotaisdoMes(cortador, tipoRegistro);
                    lstTotais.Add(item);

                }
            }

            return lstTotais;
        }
        private static ItemDetalheSaldosEmAbertoDTO ObterDetalheDaBandeira(CortadorMensagem cortador, String tipoRegistro)
        {
            ItemDetalheSaldosEmAbertoDTO retorno = new ItemDetalheSaldosEmAbertoDTO();
            retorno.TipoRegistro = tipoRegistro;
            retorno.DataReferencia = cortador.LerData(7, "MM/yyyy");
            retorno.CodigoBandeira = cortador.LerShort(3);
            retorno.CodigoBanco = cortador.LerShort(3);
            retorno.CodigoAgencia = cortador.LerShort(5);
            retorno.ContaCorrente = cortador.LerString(10);
            retorno.CodigoEstabelecimento = cortador.LerInt32(9);
            retorno.ValorBruto = cortador.LerDecimal(15, 2);
            retorno.ValorLiquido = cortador.LerDecimal(15, 2);

            return retorno;
        }
        private static TotalBandeiraMesSaldosEmAbertoDTO ObterTotaisBandeiraMes(CortadorMensagem cortador, String tipoRegistro)
        {
            TotalBandeiraMesSaldosEmAbertoDTO retorno = new TotalBandeiraMesSaldosEmAbertoDTO();
            retorno.TipoRegistro = tipoRegistro;
            retorno.DataReferencia = cortador.LerData(7, "MM/yyyy");
            retorno.CodigoBandeira = cortador.LerShort(3);
            retorno.DescricaoBandeira = cortador.LerString(12);
            retorno.ValorLiquido = cortador.LerDecimal(15, 2);

            return retorno;
        }
        private static TotalMesSaldosEmAbertoDTO ObterTotaisdoMes(CortadorMensagem cortador, String tipoRegistro)
        {
            TotalMesSaldosEmAbertoDTO retorno = new TotalMesSaldosEmAbertoDTO();
            retorno.TipoRegistro = tipoRegistro;
            retorno.DataReferencia = cortador.LerData(7, "MM/yyyy");
            retorno.ValorLiquido = cortador.LerDecimal(15, 2);
            return retorno;
        }

        public static TotalizadorPorBandeiraSaldosEmAbertoDTO TraduzirResultadoTotalPorBandeira(String dados)
        {

            TotalizadorPorBandeiraSaldosEmAbertoDTO retorno = new TotalizadorPorBandeiraSaldosEmAbertoDTO();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            retorno.ValorLiquido = cortador.LerDecimal(15, 2);
            retorno.QuantidadeBandeiras = cortador.LerShort(4);

            string[] registros = cortador.LerOccurs(32, 500);

            retorno.TotaisBandeira = ObterTotalBandeira(registros, retorno.QuantidadeBandeiras);
            return retorno;
        }
        private static List<TotalPorBandeiraSaldosEmAbertoDTO> ObterTotalBandeira(string[] registros, Int16 quantidadeRegistros)
        {
            List<TotalPorBandeiraSaldosEmAbertoDTO> lstReteorno = new List<TotalPorBandeiraSaldosEmAbertoDTO>();


            TotalPorBandeiraSaldosEmAbertoDTO item;
            CortadorMensagem cortador;
            for (int i = 0; i < quantidadeRegistros; i++)
            {
                string registro = registros[i];
                registro = registro.Replace("\0", "");
                if (registro.Length > 0)
                {
                    cortador = new CortadorMensagem(registro);
                    item = new TotalPorBandeiraSaldosEmAbertoDTO();

                    item.CodigoBandeira = cortador.LerShort(3);
                    item.DescricaoBandeira = cortador.LerString(12);
                    item.TotalBandeira = cortador.LerDecimal(15, 2);
                    if (item.TotalBandeira > 0)
                        lstReteorno.Add(item);
                }
            }
            return lstReteorno;
        }



        public static List<PeriodoDisponivelDTO> TraduzirResultadoPeriodosDisponiveis(String dados)
        {
            List<PeriodoDisponivelDTO> lstPeriodos = new List<PeriodoDisponivelDTO>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int16 quantidadeRegistros = cortador.LerInt16(3);

            if (quantidadeRegistros > 100)
            {
                quantidadeRegistros = 100;
            }
            string[] registros = cortador.LerOccurs(31, 100);

            return ObterListaPeriodosDisponiveis(registros, quantidadeRegistros);
        }
        private static List<PeriodoDisponivelDTO> ObterListaPeriodosDisponiveis(string[] registros, Int32 quantidadeRegistros)
        {
            List<PeriodoDisponivelDTO> retorno = new List<PeriodoDisponivelDTO>();

            for (int i = 0; i < quantidadeRegistros; i++)
            {
                String registro = registros[i];

                CortadorMensagem cortador = new CortadorMensagem(registro);

                retorno.Add(ObterPeriodoDisponivel(cortador));
            }

            return retorno;
        }

        private static PeriodoDisponivelDTO ObterPeriodoDisponivel(CortadorMensagem cortador)
        {
            PeriodoDisponivelDTO retorno = new PeriodoDisponivelDTO()
            {
                DataPeriodoInicial = cortador.LerData(7, "MM/yyyy"),
                DataPeriodoFinal = cortador.LerData(7, "MM/yyyy"),
                CodigoSolicitacao = cortador.LerString(17)
            };

            return retorno;
        }

    }
}
