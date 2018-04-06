using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo.LancamentosFuturos;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public abstract class LancamentosFuturosTR : ITradutor
    {
        #region [ Lançamentos Futuros - Crédito (Totalizadores) - WACA1324 / WA1324 / ISHO ]

        /// <summary>
        /// Geração da String AREA-FIXA para a consulta do Relatório de Lançamentos Futuros - Crédito - Totalizadores
        /// </summary>
        /// <param name="codigoBandeira">Código da Bandeira</param>
        /// <param name="pvs">Lista de Estabelecimentos</param>
        /// <returns>Área fixa de entrada</returns>
        public static String ConsultarCreditoTotalizadoresEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(2993, ' '));
            
            return dados.ToString();
        }

        /// <summary>
        /// Tratamento da String AREA-FIXA retornada pela consulta do Relatório de Lançamentos Futuros - Crédito - Totalizadores
        /// </summary>
        /// <param name="dados">AREA-FIXA de saída retornada pela consulta</param>
        /// <returns>Dados extraídos da AREA-FIXA</returns>
        public static CreditoTotalizador ConsultarCreditoTotalizadoresSaida(String dados)
        {
            CreditoTotalizador retorno = new CreditoTotalizador();
            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(34, 99);

            foreach (String valor in valores)
            {
                if (retorno.Valores.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorValor = new CortadorMensagem(valor);
                    retorno.Valores.Add(new CreditoTotalizadorValor
                    {                        
                        TipoRegistro = cortadorValor.LerString(2),
                        CodigoBandeira = cortadorValor.LerInt32(3),
                        TipoBandeira = cortadorValor.LerString(12),
                        ValorLiquido = cortadorValor.LerDecimal(15, 2)
                    });
                }
            }

            String resto = cortador.LerString(100);

            retorno.Totais.TipoRegistro = cortador.LerString(2);
            retorno.Totais.ValorLiquido = cortador.LerDecimal(15, 2);

            return retorno;
        }

        #endregion

        #region [ Lançamentos Futuros - Crédito - WACA1325 / WA1325 / ISHP ]

        /// <summary>
        /// Geração da String AREA-FIXA para a consulta do Relatório de Lançamentos Futuros - Crédito
        /// </summary>
        /// <param name="codigoBandeira">Código da Bandeira</param>
        /// <param name="pvs">Lista de Estabelecimentos</param>
        /// <returns>Área fixa de entrada</returns>
        public static String ConsultarCreditoEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(4993, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Tratamento da String AREA-FIXA retornada pela consulta do Relatório de Lançamentos Futuros - Crédito
        /// </summary>
        /// <param name="dados">AREA-FIXA de saída retornada pela consulta</param>
        /// <returns>Dados extraídos da AREA-FIXA</returns>
        public static List<Credito> ConsultarCreditoSaida(String dados)
        {
            List<Credito> retorno = new List<Credito>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(43, 740);
            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new Credito
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        CodigoBandeira = cortadorRegistro.LerInt32(2),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Lançamentos Futuros - Crédito Detalhe (Totalizadores) - WACA1326 / WA1326 / ISHQ ]

        /// <summary>
        /// Geração da String AREA-FIXA para a consulta do Relatório de Lançamentos Futuros - Crédito Detalhe - Totalizadores
        /// </summary>
        /// <param name="codigoBandeira">Código da Bandeira</param>
        /// <param name="pvs">Lista de Estabelecimentos</param>
        /// <returns>Área fixa de entrada</returns>
        public static String ConsultarCreditoDetalheTotalizadoresEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(2993, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Tratamento da String AREA-FIXA retornada pela consulta do Relatório de Lançamentos Futuros - Crédito Detalhe - Totalizadores
        /// </summary>
        /// <param name="dados">AREA-FIXA de saída retornada pela consulta</param>
        /// <returns>Dados extraídos da AREA-FIXA</returns>
        public static CreditoDetalheTotalizador ConsultarCreditoDetalheTotalizadoresSaida(String dados)
        {
            CreditoDetalheTotalizador retorno = new CreditoDetalheTotalizador();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(68, 99);
            foreach (String valor in valores)
            {
                if (retorno.Valores.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new CreditoDetalheTotalizadorValor
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        CodigoBandeira = cortadorRegistro.LerInt32(3),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        ValorBruto = cortadorRegistro.LerDecimal(15, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2),
                        ValorDescontado = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            cortador.LerString(100);

            retorno.Totais.TipoRegistro = cortador.LerString(2);
            retorno.Totais.TotalValorBrutoVenda = cortador.LerDecimal(15, 2);
            retorno.Totais.TotalValorLiquido = cortador.LerDecimal(15, 2);
            retorno.Totais.TotalValorDescontado = cortador.LerDecimal(15, 2);

            return retorno;
        }

        #endregion

        #region [ Lançamentos Futuros - Crédito Detalhe - WACA1327 / WA1327 / ISHR ]

        /// <summary>
        /// Geração da String AREA-FIXA para a consulta do Relatório de Lançamentos Futuros - Crédito Detalhe
        /// </summary>
        /// <param name="codigoBandeira">Código da Bandeira</param>
        /// <param name="pvs">Lista de Estabelecimentos</param>
        /// <returns>Área fixa de entrada</returns>
        public static String ConsultarCreditoDetalheEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(4993, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Tratamento da String AREA-FIXA retornada pela consulta do Relatório de Lançamentos Futuros - Crédito Detalhe
        /// </summary>
        /// <param name="dados">AREA-FIXA de saída retornada pela consulta</param>
        /// <returns>Dados extraídos da AREA-FIXA</returns>
        public static List<CreditoDetalhe> ConsultarCreditoDetalheSaida(String dados)
        {
            List<CreditoDetalhe> retorno = new List<CreditoDetalhe>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] registros = cortador.LerOccurs(197, 120);

            foreach (String registro in registros)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(registro);
                    String tipoRegistro = cortadorRegistro.LerString(2);

                    if (tipoRegistro == "DT")
                    {
                        retorno.Add(new CreditoDetalheDT
                        {
                            TipoRegistro = tipoRegistro,
                            DataVenda = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            PrazoRecebimento = cortadorRegistro.LerInt32(2),
                            NumeroPV = cortadorRegistro.LerInt32(9),
                            NumeroResumo = cortadorRegistro.LerInt32(9),
                            StatusOc = cortadorRegistro.LerString(20),
                            QuantidadeTransacoesRV = cortadorRegistro.LerInt32(5),
                            Bandeira = cortadorRegistro.LerString(12),
                            DescricaoResumo = cortadorRegistro.LerString(70),
                            ValorApresentacaoBruto = cortadorRegistro.LerDecimal(13, 2),
                            ValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            ValorLiquido = cortadorRegistro.LerDecimal(13, 2),
                            IndicadorSinalValor = cortadorRegistro.LerString(1),
                            CodigoBandeira = cortadorRegistro.LerInt32(2)
                        });
                    }
                    else if (tipoRegistro == "A1")
                    {
                        retorno.Add(new CreditoDetalheA1
                        {
                            TipoRegistro = tipoRegistro,
                            ChaveLinhaDetalhe = cortadorRegistro.LerString(65),
                            Bandeira = cortadorRegistro.LerString(12),
                            DescricaoCompensacao = cortadorRegistro.LerString(70),
                            TotalValorApresentacao = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorLiquido = cortadorRegistro.LerDecimal(13, 2),
                            IndicadorSinalValor = cortadorRegistro.LerString(1),
                            CodigoBandeira = cortadorRegistro.LerInt32(2)
                        });
                    }
                    else if (tipoRegistro == "A2")
                    {
                        retorno.Add(new CreditoDetalheA2
                        {
                            TipoRegistro = tipoRegistro,
                            ChaveLinhaDetalhe = cortadorRegistro.LerString(65),
                            Bandeira = cortadorRegistro.LerString(12),
                            DescricaoCompensacao = cortadorRegistro.LerString(70),
                            TotalValorApresentacao = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorLiquido = cortadorRegistro.LerDecimal(13, 2),
                            IndicadorSinalValor = cortadorRegistro.LerString(1),
                            CodigoBandeira = cortadorRegistro.LerInt32(2)
                        });
                    }
                }
            }

            return retorno;
        }

        #endregion

        #region [ Lançamentos Futuros - Débito (Totalizadores) - WACA1328 / WA1328 / ISHS ]

        /// <summary>
        /// Geração da String AREA-FIXA para a consulta do Relatório de Lançamentos Futuros - Débito - Totalizadores
        /// </summary>
        /// <param name="codigoBandeira">Código da Bandeira</param>
        /// <param name="pvs">Lista de Estabelecimentos</param>
        /// <returns>Área fixa de entrada</returns>
        public static String ConsultarDebitoTotalizadoresEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(2993, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Tratamento da String AREA-FIXA retornada pela consulta do Relatório de Lançamentos Futuros - Débito - Totalizadores
        /// </summary>
        /// <param name="dados">AREA-FIXA de saída retornada pela consulta</param>
        /// <returns>Dados extraídos da AREA-FIXA</returns>
        public static DebitoTotalizador ConsultarDebitoTotalizadoresSaida(String dados)
        {
            DebitoTotalizador retorno = new DebitoTotalizador();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(65, 99);

            foreach (String valor in valores)
            {
                if (retorno.Valores.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new DebitoTotalizadorValor
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        ValorVenda = cortadorRegistro.LerDecimal(15, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2),
                        ValorDescontado = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            cortador.LerString(100);

            retorno.Totais.TipoRegistro = cortador.LerString(2);
            retorno.Totais.TotalValorVenda = cortador.LerDecimal(15, 2);
            retorno.Totais.TotalValorLiquido = cortador.LerDecimal(15, 2);
            retorno.Totais.TotalValorDescontado = cortador.LerDecimal(15, 2);

            return retorno;
        }

        #endregion

        #region [ Lançamentos Futuros - Débito - WACA1329 / WA1329 / ISHT ]

        /// <summary>
        /// Geração da String AREA-FIXA para a consulta do Relatório de Lançamentos Futuros - Débito
        /// </summary>
        /// <param name="codigoBandeira">Código da Bandeira</param>
        /// <param name="pvs">Lista de Estabelecimentos</param>
        /// <returns>Área fixa de entrada</returns>
        public static String ConsultarDebitoEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(4993, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Tratamento da String AREA-FIXA retornada pela consulta do Relatório de Lançamentos Futuros - Débito
        /// </summary>
        /// <param name="dados">AREA-FIXA de saída retornada pela consulta</param>
        /// <returns>Dados extraídos da AREA-FIXA</returns>
        public static List<Debito> ConsultarDebitoSaida(String dados)
        {
            List<Debito> retorno = new List<Debito>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(191, 165);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);

                    String tipoRegistro = cortadorRegistro.LerString(2);

                    if (tipoRegistro == "DT")
                    {
                        retorno.Add(new DebitoDT
                        {
                            TipoRegistro = tipoRegistro,
                            DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            NumeroPV = cortadorRegistro.LerInt32(9),
                            DataResumo = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            NumeroResumo = cortadorRegistro.LerInt32(9),
                            QuantidadeTransacoesRV = cortadorRegistro.LerInt32(5),
                            Bandeira = cortadorRegistro.LerString(12),
                            DescricaoResumo = cortadorRegistro.LerString(70),
                            ValorApresentacao = cortadorRegistro.LerDecimal(13, 2),
                            ValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            ValorLiquidoAntecipado = cortadorRegistro.LerDecimal(13, 2),
                            IndicadorSinalValor = cortadorRegistro.LerString(1),
                            BancoCredito = cortadorRegistro.LerInt32(3),
                            AgenciaCredito = cortadorRegistro.LerInt32(5),
                            ContaCredito = cortadorRegistro.LerString(10)
                        });
                    }
                    else if (tipoRegistro == "A1")
                    {
                        retorno.Add(new DebitoA1
                        {
                            TipoRegistro = tipoRegistro,
                            ChaveDetalhe = cortadorRegistro.LerString(43),
                            Bandeira = cortadorRegistro.LerString(12),
                            DescricaoTotalDiarioBandeira = cortadorRegistro.LerString(70),
                            TotalValorTotalResumo = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorLiquidoAntecipado = cortadorRegistro.LerDecimal(13, 2),
                            IndicadorSinalValor = cortadorRegistro.LerString(1),
                            BancoCredito = cortadorRegistro.LerInt32(3),
                            AgenciaCredito = cortadorRegistro.LerInt32(5),
                            ContaCredito = cortadorRegistro.LerString(10)
                        });
                    }
                    else if (tipoRegistro == "A2")
                    {
                        retorno.Add(new DebitoA1
                        {
                            TipoRegistro = tipoRegistro,
                            ChaveDetalhe = cortadorRegistro.LerString(43),
                            Bandeira = cortadorRegistro.LerString(12),
                            DescricaoTotalDiarioBandeira = cortadorRegistro.LerString(70),
                            TotalValorTotalResumo = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            TotalValorLiquidoAntecipado = cortadorRegistro.LerDecimal(13, 2),
                            IndicadorSinalValor = cortadorRegistro.LerString(1),
                            BancoCredito = cortadorRegistro.LerInt32(3),
                            AgenciaCredito = cortadorRegistro.LerInt32(5),
                            ContaCredito = cortadorRegistro.LerString(10)
                        });
                    }
                }
            }

            return retorno;
        }

        #endregion
    }
}