using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo.ValoresPagos;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public class ValoresPagosTR : ITradutor
    {
        #region [ Valores Pagos - Crédito - Totalizadores - WACA1316 / WA1316 / ISHG ]

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

        public static CreditoTotalizador ConsultarCreditoTotalizadoresSaida(String dados)
        {
            CreditoTotalizador retorno = new CreditoTotalizador();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            return new CreditoTotalizador
            {
                TipoRegistro = cortador.LerString(2),
                TotalValorLiquido = cortador.LerDecimal(15, 2)
            };
        }

        #endregion

        #region [ Valores Pagos - Crédito - WACA1317 / WA1317 / ISHH ]

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

        public static List<Credito> ConsultarCreditoSaida(String dados)
        {
            List<Credito> retorno = new List<Credito>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(76, 420);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new Credito
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        DataBaixa = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        NumeroPV = cortadorRegistro.LerInt32(9),
                        NumeroOcu = cortadorRegistro.LerInt32(9),
                        ValorDesconto = cortadorRegistro.LerDecimal(9, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2),
                        BancoCredito = cortadorRegistro.LerInt32(3),
                        AgenciaCredito = cortadorRegistro.LerInt32(5),
                        ContaCredito = cortadorRegistro.LerString(10)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Valores Pagos - Crédito Detalhe - Totalizadores - WACA1318 / WA1318 / ISHI ]

        public static CreditoDetalheTotalizador ConsultarCreditoDetalheTotalizadoresSaida(String dados)
        {
            CreditoDetalheTotalizador retorno = new CreditoDetalheTotalizador();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(48, 99);

            foreach (String valor in valores)
            {
                if (retorno.Valores.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new CreditoDetalheTotalizadorValor
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        ValorBruto = cortadorRegistro.LerDecimal(15, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            cortador.LerString(100);

            retorno.Totais.TipoRegistro = cortador.LerString(2);
            retorno.Totais.TotalValorBruto = cortador.LerDecimal(15, 2);
            retorno.Totais.TotalValorLiquido = cortador.LerDecimal(15, 2);

            return retorno;
        }

        #endregion

        #region [ Valores Pagos - Crédito Detalhe - WACA1319 / WA1319 / ISHJ ]

        public static List<CreditoDetalhe> ConsultarCreditoDetalheSaida(String dados)
        {
            List<CreditoDetalhe> retorno = new List<CreditoDetalhe>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(103, 310);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new CreditoDetalhe
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        DataApresentacao = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        NumeroPV = cortadorRegistro.LerInt32(9),
                        NumeroResumo = cortadorRegistro.LerInt32(9),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        QuantidadeTransacao = cortadorRegistro.LerInt32(3),
                        TipoResumo = cortadorRegistro.LerInt32(1),
                        PrazoRecebimento = cortadorRegistro.LerInt32(2),
                        ValorApresentacao = cortadorRegistro.LerDecimal(15, 2),
                        ValorDesconto = cortadorRegistro.LerDecimal(9, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Valores Pagos - Débito - Totalizadores - WACA1320 / WA1320 / ISHK ]

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

        public static DebitoTotalizador ConsultarDebitoTotalizadoresSaida(String dados)
        {
            DebitoTotalizador retorno = new DebitoTotalizador();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(31, 99);

            foreach (String valor in valores)
            {
                if (retorno.Valores.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new DebitoTotalizadorValor
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
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

        #region [ Valores Pagos - Débito - WACA1321 / WA1321 / ISHL ]

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

        public static List<Debito> ConsultarDebitoSaida(String dados)
        {
            List<Debito> retorno = new List<Debito>();
            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(124, 255);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new Debito
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        DataPagamento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        NumeroPV = cortadorRegistro.LerInt32(9),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        DescricaoLancamento = cortadorRegistro.LerString(30),
                        TmsLancamento = cortadorRegistro.LerString(26),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2),
                        BancoCredito = cortadorRegistro.LerInt32(3),
                        AgenciaCredito = cortadorRegistro.LerInt32(5),
                        ContaCredito = cortadorRegistro.LerString(10)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Valores Pagos - Débito Detalhe - Totalizadores - WACA1322 / WA1322 / ISHM ]
     
        public static DebitoDetalheTotalizador ConsultarDebitoDetalheTotalizadoresSaida(String dados)
        {
            DebitoDetalheTotalizador retorno = new DebitoDetalheTotalizador();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(48, 99);

            foreach (String valor in valores)
            {
                if (retorno.Valores.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new DebitoDetalheTotalizadorValor
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        ValorBruto = cortadorRegistro.LerDecimal(15, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            cortador.LerString(100);

            retorno.Totais.TipoRegistro = cortador.LerString(2);
            retorno.Totais.TotalValorBrutoVenda = cortador.LerDecimal(15, 2);
            retorno.Totais.TotalValorLiquido = cortador.LerDecimal(15, 2);

            return retorno;
        }

        #endregion

        #region [ Valores Pagos - Débito Detalhe - WACA1323 / WA1323 / ISHN ]
     
        public static List<DebitoDetalhe> ConsultarDebitoDetalheSaida(String dados)
        {
            List<DebitoDetalhe> retorno = new List<DebitoDetalhe>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(124, 250);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new DebitoDetalhe
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        TimestampFinanceiro = cortadorRegistro.LerString(26),                      
                        DataVenda = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        NumeroPV = cortadorRegistro.LerInt32(9),
                        NumeroRV = cortadorRegistro.LerInt32(9),
                        DataRV = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        QuantidadeTransacao = cortadorRegistro.LerInt32(4),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        ValorVenda = cortadorRegistro.LerDecimal(14, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(14, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion
    }
}