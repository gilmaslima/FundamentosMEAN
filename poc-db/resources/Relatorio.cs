using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.SharePoint.Modelo
{
    public enum TipoVenda
    {
        Credito = 0,
        Debito = 1,
        Construcard = 2,
        Todos = 3,
        RecargaCelular = 4,
        Corban = 5,
        Zolkin = 6
    }

    public enum TipoRelatorio
    {
        Vendas = 0,
        ValoresPagos = 1,
        OrdensCredito = 2,
        PagamentosAjustados = 3,
        LancamentosFuturos = 4,
        AntecipacoesRAV = 5,
        DebitosDesagendamentos = 6,
        Servicos = 7,
        SuspensosRetidosPenhorados = 8,
        SaldosEmAberto = 9,
        RecargaCelular = 10,
        ValoresConsolidadosVendas = 11,
        Estorno = 12,
        ContaCorrente = 13
    }

    /// <summary>Estrutura que representa um relatório de Extrato</summary>
    public struct Relatorio
    {
        public TipoRelatorio TipoRelatorio;
        public TipoVenda TipoVenda;
        public String ControleRelatorio;
        public String ControleDetalheRelatorio;
        public String Nome;
        public String NomeDetalhe;
        public Int32 Versao;

        public Relatorio(TipoRelatorio relatorio, TipoVenda tipoVenda)
        {
            ControleDetalheRelatorio = null;
            ControleRelatorio = null;
            Nome = null;
            NomeDetalhe = null;
            TipoRelatorio = relatorio;
            TipoVenda = tipoVenda;
            Versao = 1;
        }
        /* 
         * Tabela de Relátorios e Tipos de Venda
         * -----------------------------------------------------
         * Relatórios
         *      Item                            Valor
         * -----------------------------------------------------
         *      Vendas                          0
         *      Valores Pagos                   1
         *      Ordens de Crédito               2
         *      Pagamentos Ajustados            3
         *      Lançamentos Futuros             4
         *      RAV (Antecipações)              5
         *      Débitos e Desagendamentos       6
         *      Débitos e Serviços              7
         *      Suspensos Penhorados Retidos    8
         *      Saldos em Aberto                9
         *      Recarga de Celular              10
         *      Estorno                         12
         * -----------------------------------------------------
         * Tipos de Venda
         *      Item                            Valor
         * -----------------------------------------------------
         *      Crédito                         0
         *      Débito                          1
         *      Construcard                     2
         *      Corban                          5
         *      Zolkin                          6
         * -----------------------------------------------------
         */
        private static List<Relatorio> relatorios;
        public static List<Relatorio> Relatorios
        {
            get
            {
                if (relatorios == null)
                {
                    relatorios = new List<Relatorio>();
                    relatorios.AddRange(new Relatorio[] {
                
                        // Pagamentos Ajustados
                        new Relatorio(TipoRelatorio.PagamentosAjustados, TipoVenda.Credito) { 
                            Versao              = 1,
                            ControleRelatorio   = "~/_controltemplates/Redecard.PN.Extrato/PagamentosAjustadosUserControl.ascx", 
                            Nome                = "relatório de pagamentos ajustados"
                        },
                        //  Débitos e Desagendamentos
                        new Relatorio(TipoRelatorio.DebitosDesagendamentos, TipoVenda.Credito) { 
                            Versao              = 1,
                            ControleRelatorio   = "~/_controltemplates/Redecard.PN.Extrato/RelatorioDebitosDesagendamentosUserControl.ascx", 
                            Nome                = "relatório de débitos e desagendamentos"
                        },

                        //Créditos Suspensos, Retidos e Penhorados
                        new Relatorio(TipoRelatorio.SuspensosRetidosPenhorados, TipoVenda.Credito) { 
                            Versao              = 1,
                            ControleRelatorio   = "~/_controltemplates/Redecard.PN.Extrato/CredSuspRetPenUserControl.ascx", 
                            Nome                = "relatório de créditos suspensos, retidos e penhorados"
                        },

                        //Saldos em aberto
                        new Relatorio(TipoRelatorio.SaldosEmAberto, TipoVenda.Credito) { 
                            Versao              = 1,
                            ControleRelatorio   = "~/_controltemplates/Redecard.PN.Extrato/RelatorioSaldosEmAbertoUserControl.ascx", 
                            Nome                = "Saldos em Aberto"
                        },

                        // Valores Pagos
                        new Relatorio(TipoRelatorio.ValoresPagos, TipoVenda.Credito) {
                            Versao                      = 2,
                            ControleRelatorio           = "~/_controltemplates/ExtratoV2/ValoresPagos/RelatorioCredito.ascx",
                            ControleDetalheRelatorio    = "~/_controltemplates/ExtratoV2/ValoresPagos/RelatorioCreditoDetalhe.ascx",
                            Nome                        = "relatório de valores pagos - crédito"
                        },
                        new Relatorio(TipoRelatorio.ValoresPagos, TipoVenda.Debito) {
                            Versao                      = 2,
                            ControleRelatorio           = "~/_controltemplates/ExtratoV2/ValoresPagos/RelatorioDebito.ascx",
                            ControleDetalheRelatorio    = "~/_controltemplates/ExtratoV2/ValoresPagos/RelatorioDebitoDetalhe.ascx",
                            Nome                        = "relatório de valores pagos - débito"
                        },                        

                        // Conta Corrente
                        new Relatorio(TipoRelatorio.ContaCorrente, TipoVenda.Todos) {
                            Versao              = 1,
                            ControleRelatorio   = "~/_controltemplates/ExtratoV2/ContaCorrente/RelatorioContaCorrente.ascx",                            
                            Nome                = "relatório Conta Corrente"
                        },

                        // Vendas
                        new Relatorio(TipoRelatorio.Vendas, TipoVenda.Credito) { 
                            Versao              = 2,
                            ControleRelatorio   = "~/_controltemplates/ExtratoV2/Vendas/RelatorioCredito.ascx",                            
                            Nome                = "relatório de vendas - crédito"
                        },
                        new Relatorio(TipoRelatorio.Vendas, TipoVenda.Construcard) { 
                            Versao              = 2,
                            ControleRelatorio   = "~/_controltemplates/ExtratoV2/Vendas/RelatorioConstrucard.ascx",                             
                            Nome                = "relatório de vendas - Construcard"
                        },
                        new Relatorio(TipoRelatorio.Vendas, TipoVenda.Debito) { 
                            Versao              = 2,
                            ControleRelatorio   = "~/_controltemplates/ExtratoV2/Vendas/RelatorioDebito.ascx",                             
                            Nome                = "relatório de vendas - débito" 
                        },
                        new Relatorio(TipoRelatorio.Vendas, TipoVenda.RecargaCelular) {
                            Versao              = 2,
                            ControleRelatorio   = "~/_controltemplates/ExtratoV2/Vendas/RelatorioRecargaCelular.ascx",
                            Nome                = "relatório de vendas - recarga de celular"
                        },

                        // Lançamentos Futuros            
                        new Relatorio(TipoRelatorio.LancamentosFuturos, TipoVenda.Credito) { 
                            Versao                      = 2,
                            ControleRelatorio           = "~/_controltemplates/ExtratoV2/LancamentosFuturos/RelatorioCredito.ascx",
                            ControleDetalheRelatorio    = "~/_controltemplates/ExtratoV2/LancamentosFuturos/RelatorioCreditoDetalhe.ascx",
                            Nome                        = "relatório de lançamentos futuros - crédito"
                        },
                        new Relatorio(TipoRelatorio.LancamentosFuturos, TipoVenda.Debito) { 
                            Versao              = 2,
                            ControleRelatorio   = "~/_controltemplates/ExtratoV2/LancamentosFuturos/RelatorioDebito.ascx",                             
                            Nome                = "relatório de lançamentos futuros - débito - pré-datado"
                        },

                        // RAV (Antecipações)
                        new Relatorio(TipoRelatorio.AntecipacoesRAV, TipoVenda.Credito) { 
                            Versao                      = 2,
                            ControleRelatorio           = "~/_controltemplates/ExtratoV2/AntecipacaoRAV/RelatorioRAV.ascx",
                            ControleDetalheRelatorio    = "~/_controltemplates/ExtratoV2/AntecipacaoRAV/RelatorioRAVDetalhe.ascx",
                            Nome                        = "relatório de antecipações"
                        },

                        // Ordens de Crédito
                        new Relatorio(TipoRelatorio.OrdensCredito, TipoVenda.Credito) {
                            Versao                      = 2,
                            ControleRelatorio           = "~/_controltemplates/ExtratoV2/OrdensCredito/RelatorioCredito.ascx",
                            ControleDetalheRelatorio    = "~/_controltemplates/ExtratoV2/OrdensCredito/RelatorioCreditoDetalhe.ascx",
                            Nome                        = "relatório de ordens de crédito"
                        },

                        // Relatório de Serviços
                        new Relatorio(TipoRelatorio.Servicos, TipoVenda.Credito) { 
                            Versao              = 1,
                            ControleRelatorio   = "~/_controltemplates/ExtratoV2/Servicos/RelatorioServicos.ascx", 
                            Nome                = "relatório de serviços"
                        },

                        // Recarga de Celular
                        new Relatorio(TipoRelatorio.RecargaCelular, TipoVenda.Credito) {
                            Versao                      = 2,
                            ControleDetalheRelatorio    = "~/_controltemplates/ExtratoV2/RecargaCelular/RelatorioDetalhe.ascx",
                            NomeDetalhe                 = "relatório detalhado - recarga de celular"
                        },

                       // Valores consolidados de vendas.
                        new Relatorio(TipoRelatorio.ValoresConsolidadosVendas, TipoVenda.Todos) {
                            Versao                      = 2,
                            ControleRelatorio           = "~/_controltemplates/ExtratoV2/ValoresConsolidados/RelatorioValoresConsolidados.ascx",
                            ControleDetalheRelatorio    = "", //Setado dinâmicamente no cotrole pelo método AlterarControleDetalheRelatorio.
                            Nome                        = "relatório preço único"
                        },

                        // Valores vendas crédito
                        new Relatorio(TipoRelatorio.ValoresConsolidadosVendas, TipoVenda.Credito) {
                            Versao                      = 2,
                            ControleRelatorio           = "~/_controltemplates/ExtratoV2/ValoresConsolidados/RelatorioVendasCredito.ascx",
                            ControleDetalheRelatorio    = "~/_controltemplates/ExtratoV2/ValoresConsolidados/RelatorioVendasCreditoDetalhe.ascx",
                            Nome                        = "relatório de vendas crédito"
                        },

                        // Valores vendas débito
                        new Relatorio(TipoRelatorio.ValoresConsolidadosVendas, TipoVenda.Debito) {
                            Versao                      = 2,
                            ControleRelatorio           = "~/_controltemplates/ExtratoV2/ValoresConsolidados/RelatorioVendasDebito.ascx",
                            ControleDetalheRelatorio    = "~/_controltemplates/ExtratoV2/ValoresConsolidados/RelatorioVendasDebitoDetalhe.ascx",
                            Nome                        = "relatório de vendas débito"
                        },

                        // Estornos
                        //new Relatorio(TipoRelatorio.Estorno, TipoVenda.Todos) {
                        //    Versao                      = 2,
                        //    ControleRelatorio           = "~/_controltemplates/ExtratoV2/Estorno/RelatorioEstornos.ascx",
                        //    Nome                        = "Relatório de Estorno"
                        //},
                        //new Relatorio(TipoRelatorio.Estorno, TipoVenda.Construcard) {
                        //    Versao                      = 2,
                        //    ControleRelatorio           = "~/_controltemplates/ExtratoV2/Estorno/RelatorioEstornos.ascx",
                        //    Nome                        = "Relatório de Estorno - Construcard"
                        //},
                        //new Relatorio(TipoRelatorio.Estorno, TipoVenda.Corban) {
                        //    Versao                      = 2,
                        //    ControleRelatorio           = "~/_controltemplates/ExtratoV2/Estorno/RelatorioEstornos.ascx",
                        //    Nome                        = "Relatório de Estorno - Corban"
                        //},
                        new Relatorio(TipoRelatorio.Estorno, TipoVenda.Credito) {
                            Versao                      = 2,
                            ControleRelatorio           = "~/_controltemplates/ExtratoV2/Estorno/RelatorioEstornos.ascx",
                            Nome                        = "relatório de estorno - crédito"
                        },
                        new Relatorio(TipoRelatorio.Estorno, TipoVenda.Debito) {
                            Versao                      = 2,
                            ControleRelatorio           = "~/_controltemplates/ExtratoV2/Estorno/RelatorioEstornos.ascx",
                            Nome                        = "relatório de estorno - débito"
                        },
                        //new Relatorio(TipoRelatorio.Estorno, TipoVenda.RecargaCelular) {
                        //    Versao                      = 2,
                        //    ControleRelatorio           = "~/_controltemplates/ExtratoV2/Estorno/RelatorioEstornos.ascx",
                        //    Nome                        = "Relatório de Estorno - Recarda de Celular"
                        //},
                        //new Relatorio(TipoRelatorio.Estorno, TipoVenda.Zolkin) {
                        //    Versao                      = 2,
                        //    ControleRelatorio           = "~/_controltemplates/ExtratoV2/Estorno/RelatorioEstornos.ascx",
                        //    Nome                        = "Relatório de Estorno - Zolkin"
                        //},


                    });
                }

                return relatorios;
            }

            set { relatorios = value; }
        }

        public static Relatorio Obter(TipoRelatorio relatorio, TipoVenda tipoVenda)
        {
            return Relatorios.FirstOrDefault(r => r.TipoRelatorio == relatorio && r.TipoVenda == tipoVenda);
        }

        /// <summary>
        /// Alterar a propriedade ControleDetalheRelatorio da estrutura baseado no tipo de relatório e tipo de venda.
        /// </summary>
        /// <param name="tipoRelatorio">Tipo do relatório.</param>
        /// <param name="tipoVenda">Tipo da venda.</param>
        /// <param name="novaUrl">Nova URL para o controle detalhe relatório..</param>
        public static void AlterarControleDetalheRelatorio(TipoRelatorio tipoRelatorio, TipoVenda tipoVenda, String novaUrl, String nomeDetalhe)
        {
            Relatorio item = Obter(tipoRelatorio, tipoVenda);

            //Remove o item para altera-lo.
            relatorios.Remove(item);
            item.NomeDetalhe = nomeDetalhe;
            item.ControleDetalheRelatorio = novaUrl;

            //Adiciona novamente o item alterado.
            relatorios.Add(item);
        }
    }
}
