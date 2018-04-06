using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo.OrdensCredito;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public class OrdensCreditoTR : ITradutor
    {
        #region [ Ordens de Crédito - Totalizadores - WACA1334 / WA1334 / ISHZ ]

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

        public static CreditoTotalizador ConsultarTotalizadoresSaida(String dados)
        {
            CreditoTotalizador retorno = new CreditoTotalizador();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(34, 99);

            foreach (String valor in valores)
            {
                if (retorno.Valores.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new CreditoTotalizadorValor
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

        #region [ Ordens de Crédito - WACA1335 / WA1335 / ISHW ]

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

        public static List<Credito> ConsultarSaida(String dados)
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
                        DataEmissao = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        CodigoBandeira = cortadorRegistro.LerInt32(2),
                        TipoBandeira = cortadorRegistro.LerString(12),
                        ValorOc = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Ordens de Crédito Detalhe - Totalizadores - WACA1336 / WA1336 / ISH0 ]

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

        public static CreditoDetalheTotalizador ConsultarDetalheTotalizadoresSaida(String dados)
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

        #region [ Ordens de Crédito Detalhe - WACA1337 / WA1337 / ISH1 ]

        public static String ConsultarDetalheEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(pvs.Count.ToString("D4"));
            dados.Append(codigoBandeira.ToString("D3"));            
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(4993, ' '));

            return dados.ToString();
        }

        public static List<CreditoDetalhe> ConsultarDetalheSaida(String dados)
        {
            List<CreditoDetalhe> retorno = new List<CreditoDetalhe>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(193, 130);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);

                    String tipoRegistro = cortadorRegistro.LerString(2);

                    if (tipoRegistro == "DT")
                    {
                        retorno.Add(new CreditoDetalheDT
                        {
                            TipoRegistro = tipoRegistro,
                            DataEmissao = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            NumeroPV = cortadorRegistro.LerInt32(9),
                            NumeroResumo = cortadorRegistro.LerInt32(9),
                            QuantidadeCVs = cortadorRegistro.LerInt32(5),                        
                            TipoBandeira = cortadorRegistro.LerString(12),
                            StatusOc = cortadorRegistro.LerString(20),
                            DescricaoResumoAjuste = cortadorRegistro.LerString(60),
                            Prazo = cortadorRegistro.LerInt32(3),
                            ValorBruto = cortadorRegistro.LerDecimal(15, 2),
                            ValorCredito = cortadorRegistro.LerDecimal(15, 2),
                            IndicadorSinalValor = cortadorRegistro.LerString(1),
                            BancoCredito = cortadorRegistro.LerInt32(3),
                            AgenciaCredito = cortadorRegistro.LerInt32(5),
                            ContaCredito = cortadorRegistro.LerString(10)                            
                        });
                    }
                    else if (tipoRegistro == "D1")
                    {                        
                        CreditoDetalheD1 registro = new CreditoDetalheD1();
                        registro.TipoRegistro = tipoRegistro;
                        cortadorRegistro.LerString(43);
                        registro.TipoBandeira = cortadorRegistro.LerString(12);
                        cortadorRegistro.LerString(20);
                        registro.DescricaoAjuste = cortadorRegistro.LerString(60);                                                                                
                        retorno.Add(registro);
                    }
                }
            }

            return retorno;
        }

        #endregion
    }
}
