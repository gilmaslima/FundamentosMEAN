/*
© Copyright 2017 Rede S.A.
Autor	: Francisco Mazali
Empresa	: Iteris Consultoria e Software LTDA
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo.Vendas;
using Redecard.PN.Extrato.Servicos.Vendas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL = Redecard.PN.Extrato.Negocio.VendasBLL;
using ContratoDadosVendas = Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;
using DTO = Redecard.PN.Extrato.Modelo.Vendas;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Vendas.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1310 / Programa WA1310 / TranID ISHA / Método ConsultarCreditoTotalizadores<br/>
    /// - Book WACA1311 / Programa WA1311 / TranID ISHB / Método ConsultarCredito<br/>
    /// - Book WACA1312 / Programa WA1312 / TranID ISHC / Método ConsultarDebitoTotalizadores<br/>
    /// - Book WACA1313 / Programa WA1313 / TranID ISHD / Método ConsultarDebito<br/>
    /// - Book WACA1314 / Programa WA1314 / TranID ISHE / Método ConsultarConstrucardTotalizadores<br/>
    /// - Book WACA1315 / Programa WA1315 / TranID ISHF / Método ConsultarConstrucard
    /// - Book BKWA2610	/ Programa WAC261 / TranID WAAF / Método ConsultarRecargaCelularPvFisicoTotalizadores<br/>
    /// - Book BKWA2620	/ Programa WAC262 / TranID WAAG / Método ConsultarRecargaCelularPvFisico<br/>
    /// - Book BKWA2630	/ Programa WAC263 / TranID WAAH / Método ConsultarRecargaCelularPvLogicoTotalizadores<br/>
    /// - Book BKWA2640	/ Programa WAC264 / TranID WAAI / Método ConsultarRecargaCelularPvLogico
    /// </remarks>
    public class HISServicoWAExtratoVendasRest : ServicoBaseExtrato, IHISServicoWAExtratoVendasRest
    {
        #region [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

        /// <summary>
        /// Consulta de Relatório de Vendas - Crédito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        public RelatorioVendasCreditoResponse ConsultarRelatorioCredito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioVendasCreditoResponse retorno = new RelatorioVendasCreditoResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoDadosVendas.ResponseBaseItem<CreditoTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarCreditoTotalizadores(
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoDadosVendas.ResponseBaseList<Credito>> funcaoRegistros = () =>
            {
                return ConsultarCredito(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retornoFuncoes = base.ConsultaMultiThreadRest(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            retorno.Registros = retornoFuncoes.Item2.Itens;
            retorno.Totalizador = retornoFuncoes.Item1.Item;
            retorno.StatusRelatorio = retornoFuncoes.Item2.StatusRetorno;
            retorno.StatusTotalizador = retornoFuncoes.Item1.StatusRetorno;
            retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

            return retorno;
        }

        /// <summary>
        /// Consulta de Relatório de Vendas - Débito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="modalidade">Modalidade</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        public RelatorioVendasDebitoResponse ConsultarRelatorioDebito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            Modalidade modalidade,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            DebitoTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioVendasDebitoResponse retorno = new RelatorioVendasDebitoResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoDadosVendas.ResponseBaseItem<DebitoTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarDebitoTotalizadores(
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    modalidade,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoDadosVendas.ResponseBaseList<Debito>> funcaoRegistros = () =>
            {
                return ConsultarDebito(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    modalidade,
                    pvs);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retornoFuncoes = base.ConsultaMultiThreadRest(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            retorno.Registros = retornoFuncoes.Item2.Itens;
            retorno.Totalizador = retornoFuncoes.Item1.Item;
            retorno.StatusRelatorio = retornoFuncoes.Item2.StatusRetorno;
            retorno.StatusTotalizador = retornoFuncoes.Item1.StatusRetorno;
            retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

            return retorno;
        }

        /// <summary>
        /// Consulta de Relatório de Vendas - Construcard.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        public RelatorioVendasConstrucardResponse ConsultarRelatorioConstrucard(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ConstrucardTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros)
        {

            RelatorioVendasConstrucardResponse retorno = new RelatorioVendasConstrucardResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoDadosVendas.ResponseBaseItem<ConstrucardTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarConstrucardTotalizadores(
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoDadosVendas.ResponseBaseList<Construcard>> funcaoRegistros = () =>
            {
                return ConsultarConstrucard(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retornoFuncoes = base.ConsultaMultiThreadRest(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            retorno.Registros = retornoFuncoes.Item2.Itens;
            retorno.Totalizador = retornoFuncoes.Item1.Item;
            retorno.StatusRelatorio = retornoFuncoes.Item2.StatusRetorno;
            retorno.StatusTotalizador = retornoFuncoes.Item1.StatusRetorno;
            retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

            return retorno;
        }

        /// <summary>
        /// Consulta de Relatório de Vendas - Recarga de Celular - PV Físico.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </remarks>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        public RelatorioVendasRecargaCelularResponse ConsultarRelatorioRecargaCelularPvFisico(
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioVendasRecargaCelularResponse retorno = new RelatorioVendasRecargaCelularResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoDadosVendas.ResponseBaseItem<RecargaCelularTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarRecargaCelularPvFisicoTotalizadores(
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoDadosVendas.ResponseBaseList<RecargaCelularPvFisico>> funcaoRegistros = () =>
            {
                return ConsultarRecargaCelularPvFisico(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retornoFuncoes = base.ConsultaMultiThreadRest(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            retorno.Registros = retornoFuncoes.Item2 != null ? retornoFuncoes.Item2.Itens : null;
            retorno.Totalizador = retornoFuncoes.Item1.Item;
            retorno.StatusRelatorio = retornoFuncoes.Item2.StatusRetorno;
            retorno.StatusTotalizador = retornoFuncoes.Item1.StatusRetorno;
            retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

            return retorno;
        }

        /// <summary>
        /// Consulta de Relatório de Vendas - Recarga de Celular - PV Lógico.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </remarks>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        public RelatorioVendasRecargaCelularResponse ConsultarRelatorioRecargaCelularPvLogico(
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioVendasRecargaCelularResponse retorno = new RelatorioVendasRecargaCelularResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoDadosVendas.ResponseBaseItem<RecargaCelularTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarRecargaCelularPvLogicoTotalizadores(
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoDadosVendas.ResponseBaseList<RecargaCelularPvLogico>> funcaoRegistros = () =>
            {
                return ConsultarRecargaCelularPvLogico(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retornoFuncoes = base.ConsultaMultiThreadRest(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            retorno.Registros = retornoFuncoes.Item2 != null ? retornoFuncoes.Item2.Itens : null;
            retorno.Totalizador = retornoFuncoes.Item1.Item;
            retorno.StatusRelatorio = retornoFuncoes.Item2.StatusRetorno;
            retorno.StatusTotalizador = retornoFuncoes.Item1.StatusRetorno;
            retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

            return retorno;
        }

        /// <summary>
        /// Consulta de Relatório de Vendas - Recarga de Celular - PV Lógico ou PV Físico.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </remarks>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoPv">Tipo do PV (Físico ou Lógico)</param>
        public RelatorioVendasRecargaCelularResponse ConsultarRelatorioRecargaCelular(
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            RecargaCelularTipoPv tipoPv,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioVendasRecargaCelularResponse retorno = new RelatorioVendasRecargaCelularResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoDadosVendas.ResponseBaseItem<RecargaCelularTotalizador>> funcaoTotalizador = () =>
           {
               if (tipoPv == RecargaCelularTipoPv.PvFisico)
               {
                   return ConsultarRecargaCelularPvFisicoTotalizadores(
                       dataInicial,
                       dataFinal,
                       pvs);
               }
               else if (tipoPv == RecargaCelularTipoPv.PvLogico)
               {
                   return ConsultarRecargaCelularPvLogicoTotalizadores(
                      dataInicial,
                      dataFinal,
                      pvs);
               }
               else
               {
                   quantidadeTotalRegistros = 0;
                   return new ContratoDadosVendas.ResponseBaseItem<RecargaCelularTotalizador>();
               }
           };

            //Preparação da Func para consulta dos registros
            Func<ContratoDadosVendas.ResponseBaseList<RecargaCelular>> funcaoRegistros = () =>
            {
                var registrosRecarga = default(ContratoDadosVendas.ResponseBaseList<RecargaCelular>);

                if (tipoPv == RecargaCelularTipoPv.PvFisico)
                {
                    ContratoDadosVendas.ResponseBaseList<RecargaCelularPvFisico> registrosPvFisico = ConsultarRecargaCelularPvFisico(
                        registroInicial,
                        quantidadeRegistros,
                        quantidadeTotalRegistros,
                        dataInicial,
                        dataFinal,
                        pvs);

                    if (registrosPvFisico != null)
                    {
                        registrosRecarga.Itens = registrosPvFisico.Itens;
                        registrosRecarga.StatusRetorno = registrosPvFisico.StatusRetorno;
                    }
                }
                else if (tipoPv == RecargaCelularTipoPv.PvLogico)
                {
                    ContratoDadosVendas.ResponseBaseList<RecargaCelularPvLogico> registrosPvLogico = ConsultarRecargaCelularPvLogico(
                        registroInicial,
                        quantidadeRegistros,
                        quantidadeTotalRegistros,
                        dataInicial,
                        dataFinal,
                        pvs);

                    if (registrosPvLogico != null)
                    {
                        registrosRecarga.Itens = registrosPvLogico.Itens;
                        registrosRecarga.StatusRetorno = registrosPvLogico.StatusRetorno;
                    }
                }
                else
                {
                    quantidadeTotalRegistros = 0;
                    registrosRecarga = new ContratoDadosVendas.ResponseBaseList<RecargaCelular>();
                }

                return registrosRecarga;
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retornoFuncoes = base.ConsultaMultiThreadRest(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            retorno.Registros = retornoFuncoes.Item2.Itens;
            retorno.Totalizador = retornoFuncoes.Item1.Item;
            retorno.StatusRelatorio = retornoFuncoes.Item2.StatusRetorno;
            retorno.StatusTotalizador = retornoFuncoes.Item1.StatusRetorno;
            retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

            return retorno;
        }

        #endregion [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

        #region [ Vendas - Crédito - Totalizadores - WACA1310 / WA1310 / ISHA ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Crédito.<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Vendas - Crédito</returns>
        public ContratoDadosVendas.ResponseBaseItem<CreditoTotalizador> ConsultarCreditoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Vendas - Crédito - Totalizadores - WACA1310"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, dataInicial, dataFinal, pvs });

                ContratoDadosVendas.ResponseBaseItem<CreditoTotalizador> retorno = new ContratoDadosVendas.ResponseBaseItem<CreditoTotalizador>();

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.CreditoTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarCreditoTotalizadores(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.OK;

                    //Conversão de DTO para modelo de serviço
                    retorno.Item = CreditoTotalizador.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Vendas - Crédito - Registros - WACA1311 / WA1311 / ISHB ]

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Crédito.<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Vendas - Crédito.</returns>
        public ContratoDadosVendas.ResponseBaseList<Credito> ConsultarCredito(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Vendas - Crédito - WACA1311"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                ContratoDadosVendas.ResponseBaseList<Credito> retorno = new ContratoDadosVendas.ResponseBaseList<Credito>();

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Credito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarCredito(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            retorno.QuantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Credito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Converte DTO para modelo de serviço
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.OK;
                    retorno.Itens = Credito.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Vendas - Débito - Totalizadores - WACA1312 / WA1312 / ISHC ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Débito.<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="tipoVenda">Tipo de Venda</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Vendas - Débito</returns>
        public ContratoDadosVendas.ResponseBaseItem<DebitoTotalizador> ConsultarDebitoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            Modalidade tipoVenda,
            List<Int32> pvs)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Débito - Totalizadores - WACA1312"))
            {
                log.GravarLog(EventoLog.InicioServico,
                    new { codigoBandeira, dataInicial, dataFinal, tipoVenda, pvs });

                ContratoDadosVendas.ResponseBaseItem<DebitoTotalizador> retorno = new ContratoDadosVendas.ResponseBaseItem<DebitoTotalizador>();

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.DebitoTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarDebitoTotalizadores(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            (DTO.Modalidade)tipoVenda,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.OK;
                    //Conversão de DTO para modelo de serviço
                    retorno.Item = DebitoTotalizador.FromDTO(dadosConsulta);

                    log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Vendas - Débito - Registros - WACA1313 / WA1313 / ISHD ]

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Débito.<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="tipoVenda">Tipo de Venda</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Vendas - Débito.</returns>
        public ContratoDadosVendas.ResponseBaseList<Debito> ConsultarDebito(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            DebitoTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            Modalidade tipoVenda,
            List<Int32> pvs)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Débito - WACA1313"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    tipoVenda,
                    pvs
                });

                ContratoDadosVendas.ResponseBaseList<Debito> retorno = new ContratoDadosVendas.ResponseBaseList<Debito>();

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(tipoRegistro);
                    idCache.Append(tipoVenda);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Debito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDebito(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            (DTO.Modalidade)tipoVenda,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            retorno.QuantidadeTotalRegistros = 0;
                            log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == DebitoTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.DebitoDT).ToList();
                        else if (tipoRegistro == DebitoTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.DebitoA1).ToList();
                        else if (tipoRegistro == DebitoTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.DebitoA2).ToList();

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Debito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);


                    //Converte DTO para modelo de serviço
                    retorno.Itens = Debito.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.OK;

                    log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });

                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }
                return retorno;
            }
        }

        #endregion

        #region [ Vendas - Construcard - Totalizadores - WACA1314 / WA1314 / ISHE ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Construcard.<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Vendas - Construcard</returns>
        public ContratoDadosVendas.ResponseBaseItem<ConstrucardTotalizador> ConsultarConstrucardTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Vendas - Construcard - Totalizadores - WACA1314"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, dataInicial, dataFinal, pvs });
                ContratoDadosVendas.ResponseBaseItem<ConstrucardTotalizador> retorno = new ContratoDadosVendas.ResponseBaseItem<ConstrucardTotalizador>();
                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.ConstrucardTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarConstrucardTotalizadores(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Conversão de DTO para modelo de serviço
                    retorno.Item = ConstrucardTotalizador.FromDTO(dadosConsulta);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Vendas - Construcard - Registros - WACA1315 / WA1315 / ISHF ]

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Construcard.<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Vendas - Construcard.</returns>
        public ContratoDadosVendas.ResponseBaseList<Construcard> ConsultarConstrucard(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            ConstrucardTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Vendas - Construcard - WACA1315"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                ContratoDadosVendas.ResponseBaseList<Construcard> retorno = new ContratoDadosVendas.ResponseBaseList<Construcard>();

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(tipoRegistro);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Construcard>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarConstrucard(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            retorno.QuantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == ConstrucardTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.ConstrucardDT).ToList();
                        else if (tipoRegistro == ConstrucardTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.ConstrucardA1).ToList();
                        else if (tipoRegistro == ConstrucardTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.ConstrucardA2).ToList();

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Construcard>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.OK;

                    //Converte DTO para modelo de serviço
                    retorno.Itens = Construcard.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Recarga de Celular - PV Físico - Totalizadores - BKWA2610 / WAC261 / WAAF ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Recarga de Celular de PV Físico.<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF
        /// </remarks>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Vendas - Recarga de Celular de PV Físico</returns>
        public ContratoDadosVendas.ResponseBaseItem<RecargaCelularTotalizador> ConsultarRecargaCelularPvFisicoTotalizadores(
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Recarga de Celular PV Físico - Totalizadores - BKWA2610"))
            {
                log.GravarLog(EventoLog.InicioServico, new { dataInicial, dataFinal, pvs });

                ContratoDadosVendas.ResponseBaseItem<RecargaCelularTotalizador> retorno = new ContratoDadosVendas.ResponseBaseItem<RecargaCelularTotalizador>();

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    DTO.RecargaCelularTotalizador dadosConsulta =
                        CacheAdmin.Recuperar<DTO.RecargaCelularTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarRecargaCelularPvFisicoTotalizadores(
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.OK;

                    //Conversão de DTO para modelo de serviço
                    retorno.Item = RecargaCelularTotalizador.FromDTO(dadosConsulta);

                    log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Recarga de Celular - PV Físico - Registros - BKWA2620 / WAC262 / WAAG ]

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Recarga de Celular de PV Físico.<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Vendas - Recarga de Celular de PV Físico.</returns>
        public ContratoDadosVendas.ResponseBaseList<RecargaCelularPvFisico> ConsultarRecargaCelularPvFisico(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Recarga de Celular de PV Físico - BKWA2620"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                ContratoDadosVendas.ResponseBaseList<RecargaCelularPvFisico> retorno = new ContratoDadosVendas.ResponseBaseList<RecargaCelularPvFisico>();

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RecargaCelularPvFisico>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        List<DTO.RecargaCelularPvFisico> dadosConsulta = BLL.Instancia.ConsultarRecargaCelularPvFisico(
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            retorno.QuantidadeTotalRegistros = 0;
                            log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    List<DTO.RecargaCelularPvFisico> dadosCache =
                        CacheAdmin.RecuperarPesquisa<DTO.RecargaCelularPvFisico>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.OK;

                    //Converte DTO para modelo de serviço
                    retorno.Itens = RecargaCelular.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

                    log.GravarLog(EventoLog.FimServico, new { status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Recarga de Celular - PV Lógico - Totalizadores - BKWA2630 / WAC263 / WAAH ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Recarga de Celular de PV Lógico.<br/>
        /// - Book BKWA2630 / Programa WAC263 / TranID WAAH
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2630 / Programa WAC263 / TranID WAAH
        /// </remarks>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Vendas - Recarga de Celular de PV Lógico</returns>
        public ContratoDadosVendas.ResponseBaseItem<RecargaCelularTotalizador> ConsultarRecargaCelularPvLogicoTotalizadores(
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Recarga de Celular PV Lógico - Totalizadores - BKWA2630"))
            {
                log.GravarLog(EventoLog.InicioServico, new { dataInicial, dataFinal, pvs });

                ContratoDadosVendas.ResponseBaseItem<RecargaCelularTotalizador> retorno = new ContratoDadosVendas.ResponseBaseItem<RecargaCelularTotalizador>();

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    DTO.RecargaCelularTotalizador dadosConsulta =
                        CacheAdmin.Recuperar<DTO.RecargaCelularTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarRecargaCelularPvLogicoTotalizadores(
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.OK;

                    //Conversão de DTO para modelo de serviço
                    retorno.Item = RecargaCelularTotalizador.FromDTO(dadosConsulta);

                    log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Recarga de Celular - PV Lógico - Registros - BKWA2640 / WAC264 / WAAI ]

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Recarga de Celular de PV Lógico.<br/>
        /// - Book BKWA2640 / Programa WAC264 / TranID WAAI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2640 / Programa WAC264 / TranID WAAI
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Vendas - Recarga de Celular de PV Lógico.</returns>
        public ContratoDadosVendas.ResponseBaseList<RecargaCelularPvLogico> ConsultarRecargaCelularPvLogico(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Recarga de Celular de PV Lógico - BKWA2640"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                ContratoDadosVendas.ResponseBaseList<RecargaCelularPvLogico> retorno = new ContratoDadosVendas.ResponseBaseList<RecargaCelularPvLogico>();

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RecargaCelularPvLogico>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        List<DTO.RecargaCelularPvLogico> dadosConsulta = BLL.Instancia.ConsultarRecargaCelularPvLogico(
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            retorno.QuantidadeTotalRegistros = 0;
                            log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    List<DTO.RecargaCelularPvLogico> dadosCache =
                        CacheAdmin.RecuperarPesquisa<DTO.RecargaCelularPvLogico>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.OK;

                    //Converte DTO para modelo de serviço
                    retorno.Itens = RecargaCelular.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeRegistros;

                    log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });

                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoDadosVendas.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }
        #endregion
    }
}
