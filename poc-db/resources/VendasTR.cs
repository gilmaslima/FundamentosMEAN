using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.WAExtratoVendas;
using Redecard.PN.Extrato.Modelo.Vendas;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public abstract class VendasTR : ITradutor
    {
        #region [ Vendas - Crédito (Totalizadores) - WACA1310 / WA1310 / ISHA ]

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

            retorno.QuantidadeRegistros = cortador.LerInt32(6);
            retorno.Valores = new List<CreditoTotalizadorValor>();

            String[] valores = cortador.LerOccurs(82, 99);

            foreach (String valor in valores)
            {
                if (retorno.Valores.Count < retorno.QuantidadeRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new CreditoTotalizadorValor()
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        Bandeira = cortadorRegistro.LerString(12),
                        ValorApresentado = cortadorRegistro.LerDecimal(15, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2),
                        ValorDescontado = cortadorRegistro.LerDecimal(15, 2),
                        ValorCorrecao = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            cortador.LerString(100);

            retorno.TipoRegistro = cortador.LerString(2);
            retorno.ValorApresentado = cortador.LerDecimal(15, 2);
            retorno.ValorLiquido = cortador.LerDecimal(15, 2);
            retorno.ValorDescontado = cortador.LerDecimal(15, 2);
            retorno.ValorCorrecao = cortador.LerDecimal(15, 2);

            return retorno;
        }

        #endregion

        #region [ Vendas - Crédito - WACA1311 / WA1311 / ISHB ]

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

            String tipoRegistro = cortador.LerString(1);
            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(141, 255);
            foreach(String valor in valores)            
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new CreditoD()
                    {
                        DataApresentacao = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        NumeroPV = cortadorRegistro.LerInt32(9),
                        NumeroResumo = cortadorRegistro.LerInt32(9),
                        PrazoRecebimento = cortadorRegistro.LerInt16(2),
                        Bandeira = cortadorRegistro.LerString(12),
                        QuantidadeTransacoes = cortadorRegistro.LerInt32(5),
                        Descricao = cortadorRegistro.LerString(20),
                        ValorApresentado = cortadorRegistro.LerDecimal(14, 2),
                        ValorCorrecao = cortadorRegistro.LerDecimal(14, 2),
                        ValorDesconto = cortadorRegistro.LerDecimal(14, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(14, 2)
                    });
                }
            }

            return retorno.ToList();
        }

        #endregion

        #region [ Vendas - Débito (Totalizadores) - WACA1312 / WA1312 / ISHC ]

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

            retorno.QuantidadeRegistros = cortador.LerInt32(6);
            retorno.Valores = new List<DebitoTotalizadorValor>();

            String[] valores = cortador.LerOccurs(65, 99);

            foreach(String valor in valores)
            {
                if (retorno.Valores.Count < retorno.QuantidadeRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new DebitoTotalizadorValor()
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        Bandeira = cortadorRegistro.LerString(12),
                        ValorApresentado = cortadorRegistro.LerDecimal(15, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2),
                        ValorDescontado = cortadorRegistro.LerDecimal(15, 2)                        
                    });
                }
            }

            cortador.LerString(100);

            retorno.TipoRegistro = cortador.LerString(2);
            retorno.ValorApresentado = cortador.LerDecimal(15, 2);
            retorno.ValorLiquido = cortador.LerDecimal(15, 2);
            retorno.ValorDescontado = cortador.LerDecimal(15, 2);

            return retorno;
        }

        #endregion

        #region [ Vendas - Débito - WACA1313 / WA1313 / ISHD ]

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
            String[] valores = cortador.LerOccurs(213, 150);
            
            foreach(String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    String tipoRegistro = cortadorRegistro.LerString(2);

                    if (tipoRegistro.Equals("DT"))
                    {
                        retorno.Add(new DebitoDT()
                        {
                            DataVenda = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            NumeroPV = cortadorRegistro.LerInt32(9),
                            NumeroResumo = cortadorRegistro.LerInt32(9),
                            Bandeira = cortadorRegistro.LerString(12),
                            QuantidadeTransacoesRV = cortadorRegistro.LerInt32(5),
                            DescricaoResumo = cortadorRegistro.LerString(70),
                            ValorApresentado = cortadorRegistro.LerDecimal(15, 2),
                            ValorSaque = cortadorRegistro.LerDecimal(15, 2),
                            ValorDesconto = cortadorRegistro.LerDecimal(15, 2),
                            ValorLiquido = cortadorRegistro.LerDecimal(15, 2),
                            BancoCredito = cortadorRegistro.LerInt32(3),
                            AgenciaCredito = cortadorRegistro.LerInt32(5),
                            ContaCredito = cortadorRegistro.LerString(10)
                        });
                    }
                    else if (tipoRegistro.Equals("A1"))
                    {
                        retorno.Add(new DebitoA1()
                        {
                            DataApresentacao = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            NumeroPV = cortadorRegistro.LerInt32(9),
                            NumeroResumo = cortadorRegistro.LerInt32(9),
                            Bandeira = cortadorRegistro.LerString(12),
                            QuantidadeTransacoesRV = cortadorRegistro.LerInt32(5),
                            DescricaoResumo = cortadorRegistro.LerString(100),
                            Filler = cortadorRegistro.LerString(20),
                            DebitoCredito = cortadorRegistro.LerString(1),
                            ValorLiquido = cortadorRegistro.LerDecimal(15, 2),
                            Filler2 = cortadorRegistro.LerString(18)
                        });
                    }
                    else if (tipoRegistro.Equals("A2"))
                    {
                        retorno.Add(new DebitoA2()
                        {
                            DataApresentacao = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            NumeroPV = cortadorRegistro.LerInt32(9),
                            NumeroResumo = cortadorRegistro.LerInt32(9),
                            Bandeira = cortadorRegistro.LerString(12),
                            QuantidadeTransacoesRV = cortadorRegistro.LerInt32(5),
                            DescricaoResumo = cortadorRegistro.LerString(100),
                            Filler = cortadorRegistro.LerString(56)
                        });
                    }
                }
            }

            return retorno;
        }

        #endregion

        #region [ Vendas - Construcard (Totalizadores) - WACA1314 / WA1314 / ISHE ]

        public static String ConsultarConstrucardTotalizadoresEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(2993, ' '));

            return dados.ToString();
        }

        public static ConstrucardTotalizador ConsultarConstrucardTotalizadoresSaida(String dados)
        {
            ConstrucardTotalizador retorno = new ConstrucardTotalizador();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            retorno.QuantidadeRegistros = cortador.LerInt32(6);
            retorno.Valores = new List<ConstrucardTotalizadorValor>();

            String[] valores = cortador.LerOccurs(65, 99);

            foreach(String valor in valores)
            {
                if (retorno.Valores.Count < retorno.QuantidadeRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Valores.Add(new ConstrucardTotalizadorValor()
                    {
                        TipoRegistro = cortadorRegistro.LerString(2),
                        Bandeira = cortadorRegistro.LerString(12),
                        ValorBruto = cortadorRegistro.LerDecimal(15, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2),
                        ValorDescontado = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            cortador.LerString(100);

            retorno.TipoRegistro = cortador.LerString(2);
            retorno.ValorBruto = cortador.LerDecimal(15, 2);
            retorno.ValorLiquido = cortador.LerDecimal(15, 2);
            retorno.ValorDescontado = cortador.LerDecimal(15, 2);

            return retorno;
        }

        #endregion

        #region [ Vendas - Construcard - WACA1315 / WA1315 / ISHF ]

        public static String ConsultarConstrucardEntrada(Int32 codigoBandeira, List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(codigoBandeira.ToString("D3"));
            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(4993, ' '));

            return dados.ToString();
        }

        public static List<Construcard> ConsultarConstrucardSaida(String dados)
        {
            List<Construcard> retorno = new List<Construcard>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(191, 165);
            
            foreach(String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);

                    String tipoRegistro = cortadorRegistro.LerString(2);

                    if (tipoRegistro.Equals("DT"))
                    {
                        retorno.Add(new ConstrucardDT()
                        {
                            DataVenda = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                            NumeroPV = cortadorRegistro.LerInt32(9),
                            NumeroResumo = cortadorRegistro.LerInt32(9),
                            QuantidadeTransacoesRV = cortadorRegistro.LerInt32(5),
                            Bandeira = cortadorRegistro.LerString(12),
                            DescricaoResumo = cortadorRegistro.LerString(70),
                            ValorApresentado = cortadorRegistro.LerDecimal(13, 2),
                            ValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            ValorLiquido = cortadorRegistro.LerDecimal(13, 2),
                            DebitoCredito = cortadorRegistro.LerString(1),
                            BancoCredito = cortadorRegistro.LerInt32(3),
                            AgenciaCredito = cortadorRegistro.LerInt32(5),
                            ContaCredito = cortadorRegistro.LerString(10)
                        });
                    }
                    else if (tipoRegistro.Equals("A1"))
                    {
                        retorno.Add(new ConstrucardA1()
                        {
                            ChaveRegistro = cortadorRegistro.LerString(43),
                            Bandeira = cortadorRegistro.LerString(12),
                            Descricao = cortadorRegistro.LerString(70),
                            ValorApresentado = cortadorRegistro.LerDecimal(13, 2),
                            ValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            ValorLiquido = cortadorRegistro.LerDecimal(13, 2),
                            DebitoCredito = cortadorRegistro.LerString(1),
                            BancoCredito = cortadorRegistro.LerInt32(3),
                            AgenciaCredito = cortadorRegistro.LerInt32(5),
                            ContaCredito = cortadorRegistro.LerString(10)
                        });
                    }
                    else if (tipoRegistro.Equals("A2"))
                    {
                        retorno.Add(new ConstrucardA2()
                        {
                            ChaveRegistro = cortadorRegistro.LerString(43),
                            Bandeira = cortadorRegistro.LerString(12),
                            Descricao = cortadorRegistro.LerString(70),
                            ValorApresentado = cortadorRegistro.LerDecimal(13, 2),
                            ValorDesconto = cortadorRegistro.LerDecimal(13, 2),
                            ValorLiquido = cortadorRegistro.LerDecimal(13, 2),
                            DebitoCredito = cortadorRegistro.LerString(1),
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

        #region [ Vendas - Recarga Celular - PV Físico - BKWA2610 / WAC261 / WAAF ]

        /// <summary>
        /// Prepara dados de envio para consulta dos totalizadores do
        /// Relatório de Vendas de Recarga de Celular de PV Físico
        /// </summary>
        /// <param name="pvs">Lista de PVs solicitada na pesquisa</param>
        /// <param name="quantidadePvs">Quantidade de PVs sendo enviados</param>
        /// <param name="listaPvs">Lista de PVs tratada para envio ao mainframe</param>
        public static void ConsultarRecargaCelularPvFisicoTotalizadoresEntrada(
            List<Int32> pvs,
            out Int32 quantidadePvs,
            out WAC261E_FILLER[] listaPvs)
        {
            //Garantir lista não-nula
            if (pvs == null)
                pvs = new List<Int32>();

            //Recupera quantidade de PVs na lista enviada
            quantidadePvs = pvs.Count;

            //Preenche lista com 3000 registros
            while (pvs.Count < 3000)
                pvs.Add(0);

            //Cria objeto de envio para chamada
            listaPvs = pvs
                .Take(3000) //garante exatamente 3000 registros
                .Select(pv => new WAC261E_FILLER { WAC261E_NUM_PV = pv }) //transforma em WAC261E_FILLER
                .ToArray(); //converte para Array
        }

        /// <summary>
        /// Monta objeto de retorno da consulta dos totalizadores do
        /// Relatório de Recarga de Celular de PV Físico
        /// </summary>
        /// <param name="totalValorBrutoRecarga">Valor total de Recarga</param>
        /// <param name="totalValorLiquidoComissao">Valor total de Comissão</param>
        /// <returns>Totalizadores do relatório</returns>
        public static RecargaCelularTotalizador ConsultarRecargaCelularPvFisicoTotalizadoresSaida(
            Decimal totalValorBrutoRecarga,
            Decimal totalValorLiquidoComissao)
        {
            return new RecargaCelularTotalizador()
            {
                TotalValorBrutoRecarga = totalValorBrutoRecarga,
                TotalValorLiquidoComissao = totalValorLiquidoComissao
            };
                }

        #endregion

        #region [ Vendas - Recarga Celular - PV Físico - BKWA2620 / WAC262 / WAAG ]

        /// <summary>
        /// Prepara área de envio ao mainframe para consulta de registros do 
        /// Relatório de Vendas de Recarga de Celular para PV Físico.
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <returns>Área para envio ao mainframe</returns>
        public static String ConsultarRecargaCelularPvFisicoEntrada(List<Int32> pvs)
        {
            //Garantir lista não-nula
            if(pvs == null)
                pvs = new List<Int32>();

            StringBuilder areaEntrada = new StringBuilder();

            //Quantidade de PVs
            areaEntrada.Append(pvs.Count.ToString("D5"));

            //PVs concatenados, com 9 dígitos por PV (preenche com zeros à esquerda)
            for (Int32 indexPv = 0; indexPv < 3000; indexPv++)
                areaEntrada.Append(pvs.ElementAtOrDefault(indexPv).ToString("D9"));

            return areaEntrada.ToString();
                }

        /// <summary>
        /// Converte área de retorno recebida pelo mainframe para 
        /// modelo de negócio dos registros do Relatório de Vendas de Recarga
        /// de Celular para PV Físico.
        /// </summary>
        /// <param name="areaRetorno">Área de retorno</param>
        /// <returns>Modelo de negócio</returns>
        public static List<RecargaCelularPvFisico> ConsultarRecargaCelularPvFisicoSaida(String areaRetorno)
        {
            var retorno = new List<RecargaCelularPvFisico>();
            var cortador = new CortadorMensagem(areaRetorno);

            String tipoRegistro = cortador.LerString(1);
            Int32 quantidadeRegistros = cortador.LerInt32(3);
            String[] areaRegistros = cortador.LerOccurs(172, 180).Take(quantidadeRegistros).ToArray();

            foreach (String areaRegistro in areaRegistros)
            {
                var cortadorRegistro = new CortadorMensagem(areaRegistro);

                retorno.Add(new RecargaCelularPvFisico
                {
                    NumeroEstabelecimento = cortadorRegistro.LerInt32(9),
                    NsuRecarga = cortadorRegistro.LerInt32(9),
                    DataHoraRecarga = cortadorRegistro.LerString(8 + 8).ToDate("ddMMyyyy'00'HHmmss"),
                    NumeroRV = cortadorRegistro.LerString(9),
                    TipoVenda = cortadorRegistro.LerString(40),
                    NomeOperadora = cortadorRegistro.LerString(20),
                    NumeroCelular = cortadorRegistro.LerString(15),
                    ValorBrutoRecarga = cortadorRegistro.LerDecimal(15, 2),
                    ValorLiquidoComissao = cortadorRegistro.LerDecimal(15, 2),
                    StatusComissao = cortadorRegistro.LerString(20)
                });
            }

            return retorno;
        }

        #endregion

        #region [ Vendas - Recarga Celular - PV Lógico - BKWA2630 / WAC263 / WAAH ]

        /// <summary>
        /// Prepara dados de envio para consulta dos totalizadores do
        /// Relatório de Vendas de Recarga de Celular de PV Lógico
        /// </summary>
        /// <param name="pvs">Lista de PVs solicitada na pesquisa</param>
        /// <param name="quantidadePvs">Quantidade de PVs sendo enviados</param>
        /// <param name="listaPvs">Lista de PVs tratada para envio ao mainframe</param>
        public static void ConsultarRecargaCelularPvLogicoTotalizadoresEntrada(
            List<Int32> pvs,
            out Int32 quantidadePvs,
            out WAC263E_FILLER[] listaPvs)
        {
            //Garantir lista não-nula
            if (pvs == null)
                pvs = new List<Int32>();

            //Recupera quantidade de PVs na lista enviada
            quantidadePvs = pvs.Count;

            //Preenche lista com 3000 registros
            while (pvs.Count < 3000)
                pvs.Add(0);

            //Cria objeto de envio para chamada
            listaPvs = pvs
                .Take(3000) //garante exatamente 3000 registros
                .Select(pv => new WAC263E_FILLER { WAC263E_NUM_PV = pv }) //transforma em WAC263E_FILLER
                .ToArray(); //converte para Array
        }

        /// <summary>
        /// Monta objeto de retorno da consulta dos totalizadores do
        /// Relatório de Recarga de Celular de PV Lógico
        /// </summary>
        /// <param name="totalValorBrutoRecarga">Valor total de Recarga</param>
        /// <param name="totalValorLiquidoComissao">Valor total de Comissão</param>
        /// <returns>Totalizadores do relatório</returns>
        public static RecargaCelularTotalizador ConsultarRecargaCelularPvLogicoTotalizadoresSaida(
            Decimal totalValorBrutoRecarga,
            Decimal totalValorLiquidoComissao)
        {
            return new RecargaCelularTotalizador()
            {
                TotalValorBrutoRecarga = totalValorBrutoRecarga,
                TotalValorLiquidoComissao = totalValorLiquidoComissao
            };
        }

        #endregion

        #region [ Vendas - Recarga Celular - PV Lógico - BKWA2640 / WAC264 / WAAI ]

        /// <summary>
        /// Prepara área de envio ao mainframe para consulta de registros do 
        /// Relatório de Vendas de Recarga de Celular para PV Lógico.
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <returns>Área para envio ao mainframe</returns>
        public static String ConsultarRecargaCelularPvLogicoEntrada(List<Int32> pvs)
        {
            //Garantir lista não-nula
            if (pvs == null)
                pvs = new List<Int32>();

            StringBuilder areaEntrada = new StringBuilder();

            //Quantidade de PVs
            areaEntrada.Append(pvs.Count.ToString("D5"));

            //PVs concatenados, com 9 dígitos por PV (preenche com zeros à esquerda)
            for (Int32 indexPv = 0; indexPv < 3000; indexPv++)
                areaEntrada.Append(pvs.ElementAtOrDefault(indexPv).ToString("D9"));

            return areaEntrada.ToString();
    }

        /// <summary>
        /// Converte área de retorno recebida pelo mainframe para 
        /// modelo de negócio dos registros do Relatório de Vendas de Recarga
        /// de Celular para PV Lógico.
        /// </summary>
        /// <param name="areaRetorno">Área de retorno</param>
        /// <returns>Modelo de negócio</returns>
        public static List<RecargaCelularPvLogico> ConsultarRecargaCelularPvLogicoSaida(String areaRetorno)
        {
            var retorno = new List<RecargaCelularPvLogico>();
            var cortador = new CortadorMensagem(areaRetorno);

            String tipoRegistro = cortador.LerString(1);
            Int32 quantidadeRegistros = cortador.LerInt32(3);
            String[] areaRegistros = cortador.LerOccurs(172, 180).Take(quantidadeRegistros).ToArray();

            foreach (String areaRegistro in areaRegistros)
            {
                var cortadorRegistro = new CortadorMensagem(areaRegistro);

                retorno.Add(new RecargaCelularPvLogico
                {
                    NumeroEstabelecimento = cortadorRegistro.LerInt32(9),
                    NsuRecarga = cortadorRegistro.LerInt32(9),
                    DataHoraRecarga = cortadorRegistro.LerString(8 + 8).ToDate("ddMMyyyy'00'HHmmss"),
                    NumeroRV = cortadorRegistro.LerInt32(9),
                    TipoVenda = cortadorRegistro.LerString(40),
                    NomeOperadora = cortadorRegistro.LerString(20),
                    NumeroCelular = cortadorRegistro.LerString(15),
                    ValorBrutoRecarga = cortadorRegistro.LerDecimal(15, 2),
                    ValorLiquidoComissao = cortadorRegistro.LerDecimal(15, 2),
                    StatusComissao = cortadorRegistro.LerString(20)
                });
            }

            return retorno;
        }

        #endregion
    }
    }
