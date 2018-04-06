/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.WAExtratoHomePage;
using Redecard.PN.Extrato.Modelo.HomePage;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    /// <summary>
    /// Classe tradutora dos objetos de chamada/retorno HIS.
    /// </summary>
    public class HomePageTR : ITradutor
    {
        #region [ HomePage - Vendas Crédito - BKWA2470 / WAC247 / WAG4 ]

        /// <summary>
        /// Preparação dos parâmetros para chamada do serviço de consulta de Vendas a Crédito da HomePage
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <returns>Lista de PVs convertida para objeto da consulta</returns>
        public static List<WAC247E_NUM_PVS> ConsultarVendasCreditoEntrada(
            List<Int32> pvs)
        {
            //Cria array com os PVs enviados
            List<WAC247E_NUM_PVS> retorno = 
                pvs.Select(pv => new WAC247E_NUM_PVS { WAC247E_NUM_PV = pv }).ToList();

            //Preenche retorno para completar com 3000 repetições
            retorno.AddRange(Enumerable.Repeat(
                new WAC247E_NUM_PVS { WAC247E_NUM_PV = 0 }, 3000 - retorno.Count));

            return retorno;
        }

        /// <summary>
        /// Tratamento do retorno da consulta de Vendas a Crédito da HomePage
        /// </summary>
        /// <param name="totalApresentado">Total apresentado no período</param>
        /// <param name="totalLiquido">Total líquido no período</param>
        /// <param name="totalDesconto">Total de desconto no período</param>
        /// <param name="totalTransacoes">Total de transações no período</param>
        /// <param name="registros">Totais por bandeira</param>
        /// <param name="quantidadeBandeiras">Quantidade de bandeiras retornadas</param>
        /// <returns>Consulta de Vendas a Crédito</returns>
        public static VendasCredito ConsultarVendasCreditoSaida(
            Decimal totalApresentado,
            Decimal totalLiquido,
            Decimal totalDesconto,
            Decimal totalTransacoes,
            List<WAC247S_OCOR> registros,
            Int32 quantidadeBandeiras)
        {
            var retorno = new VendasCredito();
            
            //Preenche totais do período
            retorno.Totalizador = new Vendas();
            retorno.Totalizador.QuantidadeTransacoes = totalTransacoes;
            retorno.Totalizador.ValorApresentado = totalApresentado;
            retorno.Totalizador.ValorDesconto = totalDesconto;
            retorno.Totalizador.ValorLiquido = totalLiquido;

            //Preenche totais por bandeira
            if (registros != null && registros.Count > 0)
            {
                //Filtra apenas os registros com dados
                registros = registros.Take(quantidadeBandeiras).ToList();

                //Dos registros com dados, os que possuírem código bandeira informado, são totalizadores por bandeira
                List<WAC247S_OCOR> totaisPorBandeira = registros.Where(totalBandeira => totalBandeira.WAC247S_CD_BNDR != 0).ToList();

                //Dos registros com dados, o primeiro que possuir código bandeira = 0, é o "Outras Bandeiras"
                WAC247S_OCOR? totalOutrasBandeiras = null;
                if(registros.Any(totalBandeira => totalBandeira.WAC247S_CD_BNDR == 0))
                    totalOutrasBandeiras = registros.Where(totalBandeira => totalBandeira.WAC247S_CD_BNDR == 0).First();

                retorno.Vendas = new List<Vendas>();

                //Preenche totais por bandeira
                foreach (WAC247S_OCOR totalPorBandeira in totaisPorBandeira)
                {
                    retorno.Vendas.Add(new Vendas
                    {
                        DescricaoBandeira = totalPorBandeira.WAC247S_DS_BNDR,
                        CodigoBandeira = totalPorBandeira.WAC247S_CD_BNDR,
                        QuantidadeTransacoes = totalPorBandeira.WAC247S_QTD_TRAN,
                        ValorApresentado = totalPorBandeira.WAC247S_VL_APRES,
                        ValorDesconto = totalPorBandeira.WAC247S_VL_DESC,
                        ValorLiquido = totalPorBandeira.WAC247S_VL_LIQ
                    });
                }

                //Preenche total de Outras Bandeiras, se possuir outras bandeiras
                if (totalOutrasBandeiras.HasValue)
                {
                    retorno.OutrasBandeiras = new Vendas
                    {
                        DescricaoBandeira = totalOutrasBandeiras.Value.WAC247S_DS_BNDR,
                        CodigoBandeira = totalOutrasBandeiras.Value.WAC247S_CD_BNDR,
                        QuantidadeTransacoes = totalOutrasBandeiras.Value.WAC247S_QTD_TRAN,
                        ValorApresentado = totalOutrasBandeiras.Value.WAC247S_VL_APRES,
                        ValorDesconto = totalOutrasBandeiras.Value.WAC247S_VL_DESC,
                        ValorLiquido = totalOutrasBandeiras.Value.WAC247S_VL_LIQ
                    };
                }
            }

            return retorno;
        }

        #endregion

        #region [ HomePage - Vendas Débito - BKWA2480 / WAC248 / WAG5 ]

        /// <summary>
        /// Preparação dos parâmetros para chamada do serviço de consulta de Vendas a Débito da HomePage
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <returns>Lista de PVs convertida para objeto da consulta</returns>
        public static List<WAC248E_NUM_PVS> ConsultarVendasDebitoEntrada(
            List<Int32> pvs)
        {
            //Cria array com os PVs enviados
            List<WAC248E_NUM_PVS> retorno =
                pvs.Select(pv => new WAC248E_NUM_PVS { WAC248E_NUM_PV = pv }).ToList();

            //Preenche retorno para completar com 3000 repetições
            retorno.AddRange(Enumerable.Repeat(
                new WAC248E_NUM_PVS { WAC248E_NUM_PV = 0 }, 3000 - retorno.Count));

            return retorno;
        }

        /// <summary>
        /// Tratamento do retorno da consulta de Vendas a Débito da HomePage
        /// </summary>
        /// <param name="totalApresentado">Total apresentado no período</param>
        /// <param name="totalLiquido">Total líquido no período</param>
        /// <param name="totalDesconto">Total de desconto no período</param>
        /// <param name="totalTransacoes">Total de transações no período</param>
        /// <param name="registros">Totais por bandeira</param>
        /// <param name="quantidadeBandeiras">Quantidade de bandeiras retornadas</param>
        /// <returns>Consulta de Vendas a Débito</returns>
        public static VendasDebito ConsultarVendasDebitoSaida(
            Decimal totalApresentado,
            Decimal totalLiquido,
            Decimal totalDesconto,
            Decimal totalTransacoes,
            List<WAC248S_OCOR> registros,
            Int32 quantidadeBandeiras)
        {
            var retorno = new VendasDebito();

            //Preenche totais do período
            retorno.Totalizador = new Vendas();
            retorno.Totalizador.QuantidadeTransacoes = totalTransacoes;
            retorno.Totalizador.ValorApresentado = totalApresentado;
            retorno.Totalizador.ValorDesconto = totalDesconto;
            retorno.Totalizador.ValorLiquido = totalLiquido;

            //Preenche totais por bandeira
            if (registros != null && registros.Count > 0)
            {
                //Filtra apenas os registros com dados
                registros = registros.Take(quantidadeBandeiras).ToList();

                //Dos registros com dados, os que possuírem código bandeira informado, são totalizadores por bandeira
                List<WAC248S_OCOR> totaisPorBandeira = registros.Where(totalBandeira => totalBandeira.WAC248S_CD_BNDR != 0).ToList();

                //Dos registros com dados, o primeiro que possuir código bandeira = 0, é o "Outras Bandeiras"
                WAC248S_OCOR? totalOutrasBandeiras = null;
                if (registros.Any(totalBandeira => totalBandeira.WAC248S_CD_BNDR == 0))
                    totalOutrasBandeiras = registros.Where(totalBandeira => totalBandeira.WAC248S_CD_BNDR == 0).First();

                retorno.Vendas = new List<Vendas>();

                //Preenche totais por bandeira
                foreach (WAC248S_OCOR totalPorBandeira in totaisPorBandeira)
                {
                    retorno.Vendas.Add(new Vendas
                    {
                        DescricaoBandeira = totalPorBandeira.WAC248S_DS_BNDR,
                        CodigoBandeira = totalPorBandeira.WAC248S_CD_BNDR,
                        QuantidadeTransacoes = totalPorBandeira.WAC248S_QTD_TRAN,
                        ValorApresentado = totalPorBandeira.WAC248S_VL_APRES,
                        ValorDesconto = totalPorBandeira.WAC248S_VL_DESC,
                        ValorLiquido = totalPorBandeira.WAC248S_VL_LIQ
                    });
                }

                //Preenche total de Outras Bandeiras, se possuir outras bandeiras
                if (totalOutrasBandeiras.HasValue)
                {
                    retorno.OutrasBandeiras = new Vendas
                    {
                        DescricaoBandeira = totalOutrasBandeiras.Value.WAC248S_DS_BNDR,
                        CodigoBandeira = totalOutrasBandeiras.Value.WAC248S_CD_BNDR,
                        QuantidadeTransacoes = totalOutrasBandeiras.Value.WAC248S_QTD_TRAN,
                        ValorApresentado = totalOutrasBandeiras.Value.WAC248S_VL_APRES,
                        ValorDesconto = totalOutrasBandeiras.Value.WAC248S_VL_DESC,
                        ValorLiquido = totalOutrasBandeiras.Value.WAC248S_VL_LIQ
                    };
                }
            }

            return retorno;
        }

        #endregion

        #region [ HomePage - Lançamentos Futuros - BKWA2490 / WAC249 / WAG6 ]

        /// <summary>
        /// Geração da String AREA-FIXA para a consulta dos Lançamentos Futuros na HomePage
        /// </summary>
        /// <param name="pvs">Lista de estabelecimentos</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <returns>Área fixa de entrada</returns>
        public static String ConsultarLancamentosFuturosEntrada(
            List<Int32> pvs, Int32 codigoBandeira)
        {
            var areaFixa = new StringBuilder();

            areaFixa.Append(pvs.Count.ToString("D5"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                areaFixa.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            areaFixa.Append("".PadRight(27, ' '));
            areaFixa.Append(codigoBandeira.ToString("D2"));
            areaFixa.Append("".PadRight(2942, ' '));

            return areaFixa.ToString();
        }

        /// <summary>
        /// Tratamento da string AREA-FIXA retornada na consulta de Lançamentos Futuros da HomePage
        /// </summary>
        /// <param name="areaFixa">String AREA-FIXA retornada na consulta</param>
        /// <param name="codigoRetorno">OUTPUT: código de retorno</param>
        /// <param name="codigoErro">OUTPUT: código de erro</param>
        /// <param name="mensagemErro">OUTPUT: mensagem de retorno</param>
        /// <returns>Lançamentos Futuros</returns>
        public static LancamentosFuturos ConsultarLancamentosFuturosSaida(
            String areaFixa,
            out Int16 codigoRetorno,
            out Int16 codigoErro,
            out String mensagemErro)
        {
            //Inicialização da variável de retorno
            var retorno = new LancamentosFuturos();
            retorno.Resumos = new List<Resumo>();

            var cortador = new CortadorMensagem(areaFixa);            
            //Status da consulta
            codigoRetorno = cortador.LerInt16(2);
            codigoErro = cortador.LerInt16(3);
            mensagemErro = cortador.LerString(70);

            //Totais do período
            Int32 quantidadeRvs = cortador.LerInt32(7);            
            retorno.TotalBruto = cortador.LerDecimal(15, 2);
            retorno.TotalLiquido = cortador.LerDecimal(15, 2);

            //Registros / Total por Data de Recebimento
            String[] occurs = cortador.LerOccurs(42, 300);

            foreach(String occur in occurs)
            {
                if (retorno.Resumos.Count < quantidadeRvs)
                {
                    var cortadorRegistro = new CortadorMensagem(occur);
                    retorno.Resumos.Add(new Resumo
                    {
                        DataRecebimento = cortadorRegistro.LerData(8, "yyyyMMdd"),
                        ValorBruto = cortadorRegistro.LerDecimal(15, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ HomePage - Valores Pagos - BKWA2500 / WAC250 / WAG7 ]

        /// <summary>
        /// Geração da String AREA-FIXA para a consulta dos Valores Pagos na HomePage
        /// </summary>
        /// <param name="pvs">Lista de estabelecimentos</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <returns>Área fixa de entrada</returns>
        public static String ConsultarValoresPagosEntrada(
            List<Int32> pvs, Int32 codigoBandeira)
        {
            var areaFixa = new StringBuilder();

            areaFixa.Append(pvs.Count.ToString("D5"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                areaFixa.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            areaFixa.Append("".PadRight(27, ' '));
            areaFixa.Append(codigoBandeira.ToString("D2"));
            areaFixa.Append("".PadRight(2940, ' '));

            return areaFixa.ToString();
        }

        /// <summary>
        /// Tratamento da string AREA-FIXA retornada na consulta de Valores Pagos da HomePage
        /// </summary>
        /// <param name="areaFixa">String AREA-FIXA retornada na consulta</param>
        /// <param name="codigoRetorno">OUTPUT: código de retorno</param>
        /// <param name="codigoErro">OUTPUT: código de erro</param>
        /// <param name="mensagemErro">OUTPUT: mensagem de retorno</param>
        /// <returns>Valores Pagos</returns>
        public static ValoresPagos ConsultarValoresPagosSaida(
            String areaFixa,
            out Int16 codigoRetorno,
            out Int16 codigoErro,
            out String mensagemErro)
        {
            //Inicilização da variável de retorno
            var retorno = new ValoresPagos();
            retorno.Resumos = new List<Resumo>();

            var cortador = new CortadorMensagem(areaFixa);
            //Status da consulta
            codigoRetorno = cortador.LerInt16(2);
            codigoErro = cortador.LerInt16(3);
            mensagemErro = cortador.LerString(70);

            //Totais do período
            Int32 quantidadeRvs = cortador.LerInt32(7);
            retorno.TotalBruto = cortador.LerDecimal(15, 2);
            retorno.TotalLiquido = cortador.LerDecimal(15, 2);

            //Registros / Total por Data de Recebimento
            String[] occurs = cortador.LerOccurs(42, 300);

            foreach (String occur in occurs)
            {
                if (retorno.Resumos.Count < quantidadeRvs)
                {
                    var cortadorRegistro = new CortadorMensagem(occur);
                    retorno.Resumos.Add(new Resumo
                    {
                        DataRecebimento = cortadorRegistro.LerData(8, "yyyyMMdd"),
                        ValorBruto = cortadorRegistro.LerDecimal(15, 2),
                        ValorLiquido = cortadorRegistro.LerDecimal(15, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion
    }
}