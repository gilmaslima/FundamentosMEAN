using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo.AntecipacaoRAV;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public class AntecipacaoRAVTR : ITradutor
    {
        #region [ Antecipação RAV - Totalizadores - WACA1330 / WA1330 / ISHU ]

        public static String ConsultarTotalizadoresEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(2993, ' '));

            return dados.ToString();
        }

        public static RAVTotalizador ConsultarTotalizadoresSaida(String dados)
        {
            RAVTotalizador retorno = new RAVTotalizador();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(34, 99);

            foreach (String valor in valores)
            {
                if (retorno.Valores.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new RAVTotalizadorValor
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        CodigoBandeira = cortadorRegistro.LerInt32(3),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            cortador.LerString(100);

            retorno.Totais.TipoRegistro = cortador.LerString(2);
            retorno.Totais.TotalValorLiquido = cortador.LerDecimal(15, 2);

            return retorno;
        }

        #endregion

        #region [ Antecipação RAV - WACA1331 / WA1331 / ISHV ]

        public static String ConsultarEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(4993, ' '));

            return dados.ToString();
        }

        public static List<RAV> ConsultarSaida(String dados)
        {
            List<RAV> retorno = new List<RAV>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(43, 740);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new RAV
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        DataAntecipacao = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        CodigoBandeira = cortadorRegistro.LerInt32(2),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        ValorAntecipacao = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Antecipação RAV - Detalhe - Totalizadores - WACA1332 / WA1332 / ISHX ]

        public static String ConsultarDetalheTotalizadoresEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(2993, ' '));

            return dados.ToString();
        }

        public static RAVDetalheTotalizador ConsultarDetalheTotalizadoresSaida(String dados)
        {
            RAVDetalheTotalizador retorno = new RAVDetalheTotalizador();
            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(34, 99);

            foreach (String valor in valores)
            {
                if (retorno.Valores.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new RAVDetalheTotalizadorValor
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        CodigoBandeira = cortadorRegistro.LerInt32(3),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            cortador.LerString(100);

            retorno.Totais.TipoRegistro = cortador.LerString(2);
            retorno.Totais.TotalValorBruto = cortador.LerDecimal(15, 2);
            retorno.Totais.TotalValorLiquido = cortador.LerDecimal(15, 2);
            retorno.Totais.TotalValorDisponivel = cortador.LerDecimal(15, 2);

            return retorno;
        }
        #endregion

        #region [ Antecipação RAV - Detalhe - WACA1333 / WA1333 / ISHY ]

        public static String ConsultarDetalheEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(4993, ' '));

            return dados.ToString();
        }

        public static List<RAVDetalhe> ConsultarDetalheSaida(String dados)
        {
            List<RAVDetalhe> retorno = new List<RAVDetalhe>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(293, 100);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    String tipoRegistro = cortadorRegistro.LerString(2);

                    if (tipoRegistro == "DT")
                    {
                        retorno.Add(new RAVDetalheDT
                        {
                            TipoRegistro = tipoRegistro,
                            DataAntecipacao = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            DataApresentacao = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            NumeroPV = cortadorRegistro.LerInt32(9),
                            NumeroResumo = cortadorRegistro.LerInt32(9),
                            StatusOc = cortadorRegistro.LerString(20),
                            QuantidadeTransacoesRV = cortadorRegistro.LerInt32(5),
                            Bandeira = cortadorRegistro.LerString(12),
                            DescricaoResumo = cortadorRegistro.LerString(70),
                            ValorTotalResumo = cortadorRegistro.LerDecimal(13, 2),
                            ValorDisponivelRAV = cortadorRegistro.LerDecimal(13, 2),
                            ValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            ValorLiquidoAntecipado = cortadorRegistro.LerDecimal(13, 2),
                            IndicadorSinalValor = cortadorRegistro.LerString(1),
                            BancoCredito = cortadorRegistro.LerInt32(3),
                            AgenciaCredito = cortadorRegistro.LerInt32(5),
                            ContaCredito = cortadorRegistro.LerString(10),
                            CodigoBandeira = cortadorRegistro.LerInt32(2),
                            TmsOc = cortadorRegistro.LerString(26),
                            CodigoProdutoAntecipacao = cortadorRegistro.LerInt16(4),
                            DescricaoCessaoCredito = cortadorRegistro.LerString(25)
                        });
                    }
                    else if (tipoRegistro == "A1")
                    {
                        cortadorRegistro.LerString(73);
                        retorno.Add(new RAVDetalheA1
                        {
                            TipoRegistro = tipoRegistro,
                            Bandeira = cortadorRegistro.LerString(12),
                            DescricaoTotalDiarioBandeira = cortadorRegistro.LerString(70),
                            TotalValorTotalResumo = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorDisponivelRAV = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorLiquidoAntecipado = cortadorRegistro.LerDecimal(13, 2),
                            IndicadorSinalValor = cortadorRegistro.LerString(1),
                            BancoCredito = cortadorRegistro.LerInt32(3),
                            AgenciaCredito = cortadorRegistro.LerInt32(5),
                            ContaCredito = cortadorRegistro.LerString(10),
                            CodigoBandeira = cortadorRegistro.LerInt32(2),
                            TmsOcd = cortadorRegistro.LerString(26),
                            CodigoProdutoAntecipacao = cortadorRegistro.LerInt16(4),
                            DescricaoCessaoCredito = cortadorRegistro.LerString(25)
                        });
                    }
                    else if (tipoRegistro == "A2")
                    {
                        cortadorRegistro.LerString(73);
                        retorno.Add(new RAVDetalheA2
                        {
                            TipoRegistro = tipoRegistro,
                            Bandeira = cortadorRegistro.LerString(12),
                            DescricaoTotalDiarioBandeira = cortadorRegistro.LerString(70),
                            TotalValorTotalResumo = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorDisponivelRAV = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorLiquidoAntecipado = cortadorRegistro.LerDecimal(13, 2),
                            IndicadorSinalValor = cortadorRegistro.LerString(1),
                            BancoCredito = cortadorRegistro.LerInt32(3),
                            AgenciaCredito = cortadorRegistro.LerInt32(5),
                            ContaCredito = cortadorRegistro.LerString(10),
                            CodigoBandeira = cortadorRegistro.LerInt32(2),
                            TmsOcd = cortadorRegistro.LerString(26),
                            CodigoProdutoAntecipacao = cortadorRegistro.LerInt16(4),
                            DescricaoCessaoCredito = cortadorRegistro.LerString(25)
                        });
                    }
                }
            }

            return retorno;
        }

        #endregion
    }
}
