/*
© Copyright 2017 Rede S.A.
Autor : Mário de O. Neto
Empresa : Iteris Consultoria e Software
*/
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;

#if DEBUG
using Rede.PN.AtendimentoDigital.SharePoint.EntidadeServico2;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Enums;
#else
using Rede.PN.AtendimentoDigital.SharePoint.EntidadeServico;
#endif

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Handler para consulta de Taxas Crédito, Débito, Voucher
    /// </summary>
    public class Taxas : HandlerBase
    {
        /// <summary>
        /// Url da página de consulta de Taxas
        /// </summary>
        private const String UrlTaxas = "/sites/fechado/minhaconta/Paginas/taxas-e-ofertas-contratadas.aspx";

        /// <summary>
        /// Consulta Taxas
        /// </summary>
        /// <returns>Retorna Dados das Taxas para operações Credito/Débitos</returns>
        [HttpGet]
        [Authorize(UrlTaxas)]
        public HandlerResponse ConsultarTaxas()
        {
            using (Logger log = Logger.IniciarLog("Preencher tarifas"))
            {
                try
                {
                    //Codigos de Retorno.
                    var codStatusCredito = default(Int32);
                    var codStatusDebito = default(Int32);
                    var codStatusVoucher = default(Int32);
                    var codStatusFlex = default(Int32);
                    
                    //Entidades de Dados
                    var dadosBancariosCredito = default(List<DadosBancarios>);
                    var dadosBancariosDebito = default(List<DadosBancarios>);
                    var dadosBancariosVoucher = default(List<DadosBancarios>);
                    var dadosProdutosFlex = default(List<ProdutoFlex>);
                   
                    //Consulta Serviço
                    using (var contexto = new ContextoWCF<EntidadeServicoClient>())
                    {
                        //Consulta os dados bancários de Crédito da Entidade. 
                        //Tipo dados "C" representa Crédito. "D" para buscar somente débito. "V" para buscar somente voucher
                        dadosBancariosCredito = contexto.Cliente.ConsultarDadosBancarios(
                            base.Sessao.CodigoEntidade, "C", out codStatusCredito) ?? new List<DadosBancarios>();

                        dadosBancariosDebito = contexto.Cliente.ConsultarDadosBancarios(
                            base.Sessao.CodigoEntidade, "D", out codStatusDebito) ?? new List<DadosBancarios>();

                        dadosBancariosVoucher = contexto.Cliente.ConsultarDadosBancarios(
                            base.Sessao.CodigoEntidade, "V", out codStatusVoucher) ?? new List<DadosBancarios>();

                        //Consulta todos os produtos Flex para a entidade
                        dadosProdutosFlex = contexto.Cliente.ConsultarProdutosFlex
                            (base.Sessao.CodigoEntidade, null, null, out codStatusFlex) ?? new List<ProdutoFlex>();
                    }

                    if (codStatusCredito > 0)
                    {
                        Logger.GravarErro("Erro ao consultar taxas de crédito", null, codStatusCredito);
                        return new HandlerResponse(codStatusCredito, "Erro ao consultar taxas");
                    }

                    if (codStatusDebito > 0)
                    {
                        Logger.GravarErro("Erro ao consultar taxas de débito", null, codStatusDebito);
                        return new HandlerResponse(codStatusCredito, "Erro ao consultar taxas");
                    }

                    if (codStatusVoucher > 0)
                    {
                        Logger.GravarErro("Erro ao consultar taxas de voucher", null, codStatusVoucher);
                        return new HandlerResponse(codStatusCredito, "Erro ao consultar taxas");
                    }

                    if (codStatusFlex > 0 && codStatusFlex != 32180)
                    {
                        Logger.GravarErro("Erro ao consultar produtos flex ", null, codStatusFlex);
                        return new HandlerResponse(codStatusCredito, "Erro ao consultar taxas");
                    }

                    Object retorno = MontaRetornoGlobal(
                        dadosBancariosCredito,
                        dadosBancariosDebito,
                        dadosBancariosVoucher,
                        dadosProdutosFlex);

                    return new HandlerResponse(retorno);
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    return new HandlerResponse(301, "Erro ao consultar Taxas.",
                        new
                        {
                            Codigo = ex.Detail.Codigo,
                            CodeName = ex.Code != null ? ex.Code.Name : null,
                            CodeSubcode = ex.Code != null ? ex.Code.SubCode : null
                        }, null
                    );
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro ao consultar taxas", ex);
                    return new HandlerResponse(HandlerBase.CodigoErro, "Erro ao consultar taxas");
                }
            }
        }

        /// <summary>
        /// Carrega os Dados de Retorno do Serviço
        /// </summary>
        //Entidades de Dados
        private static Object MontaRetornoGlobal(
            List<DadosBancarios> dadosCredito,
            List<DadosBancarios> dadosDebito,
            List<DadosBancarios> dadosVoucher,
            List<ProdutoFlex> dadosFlex)
        {
            // SKU: Nao deve conter as bandeiras AMEX(69), ELO(70) e ELO(71)
            dadosCredito.RemoveAll(dadoCredito => dadoCredito.CodigoCartao.In("69", "70", "71"));
            dadosDebito.RemoveAll(dadoDebito => dadoDebito.CodigoCartao.In("69", "70", "71"));
            dadosVoucher.RemoveAll(dadoVoucher => dadoVoucher.CodigoCartao.In("69", "70", "71"));
            
            //Filtra os Produtos Flex, removendo os cancelados (Situação = "C")
            dadosFlex.RemoveAll(dadoFlex => dadoFlex.IndicadorSituacaoRegistro.In(true, "C"));

            return new
            {
                Credito = MontarCredito(dadosCredito, dadosFlex),
                Debito = MontarDebito(dadosDebito),
                Voucher = MontarVoucher(dadosVoucher)
            };
        }

        #region Credito
        /// <summary>
        /// Monta crédito
        /// </summary>
        private static List<Core.Taxas.Bandeira> MontarCredito(List<DadosBancarios> dadosBancarios, List<ProdutoFlex> produtosFlex)
        {
            var bandeiras = new List<Core.Taxas.Bandeira>();
            var ptBR = new CultureInfo("pt-BR");

            Core.Taxas.Bandeira bandeira = null;
            foreach (DadosBancarios dado in dadosBancarios)
            {
                Int32 codigoCartao = dado.CodigoCartao.ToInt32(0);
                Int32 codigoFeature = dado.CodigoFEAT.ToInt32(0);

                if (codigoCartao != 61 && codigoFeature != 88)
                {
                    // se é a primeira vez ou se ja nao é a sequencia da mesma bandeira
                    if ((bandeira == null) || (bandeira.Codigo != codigoCartao))
                    {
                        if (bandeira != null)
                            bandeiras.Add(bandeira);

                        bandeira = new Core.Taxas.Bandeira()
                        {
                            Taxas = new List<Core.Taxas.Taxa>(),
                            Flex = new List<Core.Taxas.Flex>(),
                            Codigo = dado.CodigoCartao.ToInt32()
                        };

                        //Monta registros para tabela de produtos flex, filtrando pelo código do cartão
                        foreach (ProdutoFlex flex in produtosFlex.Where(flex => flex.CodigoCCA == bandeira.Codigo))
                        {
                            var itemFlex = new Core.Taxas.Flex()
                            {
                                RecebimentoAntecipado = flex.DescricaoFeature.ToLower(),
                                Parcelas = String.Format("{0} a {1}", flex.CodigoPatamarInicio, flex.CodigoPatamarFim),
                                Fator1 = String.Format(ptBR, "{0:N2}", flex.ValorPrecoVariante1),
                                Fator2 = String.Format(ptBR, "{0:N2}", flex.ValorPrecoVariante2),
                                Prazo = flex.QuantidadePrazoProduto
                            };

                            //Obtém o objeto DadosBancarios para o produto flex atual
                            DadosBancarios dadosFlex = dadosBancarios.FirstOrDefault(d =>
                                d.CodigoFEAT.ToInt32(-1) == flex.CodigoFeature && flex.CodigoCCA == bandeira.Codigo);

                            //Cálculo da taxa
                            itemFlex.Taxa = String.Format(ptBR, "{0:N2}",
                                CalcularFlex(flex.ValorPrecoVariante1, flex.ValorPrecoVariante2, flex.CodigoPatamarFim,
                                dadosFlex != null ? dadosFlex.Taxa : 0));

                            bandeira.Flex.Add(itemFlex);
                        }
                    }

                    bandeira.Nome = dado.DescricaoCartao;
                    bandeira.NomeBanco = dado.NomeBanco;
                    bandeira.CodigoAgencia = dado.CodigoAgencia;
                    bandeira.ContaAtualizada = dado.ContaAtualizada;

                    //Monta registro para tabela de Pagamentos
                    var taxa = new Core.Taxas.Taxa();
                    taxa.ModalidadeVenda = dado.DescricaoFEAT.ToLower();
                    taxa.ValorTaxa = dado.Taxa;
                    taxa.Tarifa = dado.Tarifa;
                    taxa.Prazo = String.Format("{0} dias", dado.TemTarifa ? dado.PercentualTarifa : dado.PercentualTaxa);

                    if (String.IsNullOrWhiteSpace(dado.NumeroLimite))
                    {
                        taxa.Parcelas = "-";
                    }
                    else if (dado.MinimoParcelas == dado.MaximoParcelas)
                    {
                        if (dado.MinimoParcelas == 1)
                            taxa.Parcelas = String.Format("{0} Parcela", dado.MinimoParcelas);
                        else
                            taxa.Parcelas = String.Format("{0} Parcelas", dado.MinimoParcelas);
                    }
                    else
                        taxa.Parcelas = String.Format("De {0} a {1} Parcelas", dado.MinimoParcelas, dado.MaximoParcelas);

                    bandeira.Taxas.Add(taxa);
                }
            }
            return bandeiras;
        }


        /// <summary>
        /// Cálculo Flex: Fator1 + Fator2 (N-1), onde N é o número de parcelas,
        /// considerando ainda que a taxa final é %MDR + %Flex.
        /// </summary>
        /// <param name="fator1">Fator 1</param>
        /// <param name="fator2">Fator 2</param>
        /// <param name="numeroParcelas">Número de Parcelas</param>
        /// <param name="taxaMDR">Taxa MDR</param>
        /// <returns>Taxa final (FLEX + MDR)</returns>
        private static Decimal CalcularFlex(Decimal fator1, Decimal fator2, Int32 numeroParcelas, Decimal taxaMDR)
        {
            var flex = fator1 + fator2 * (numeroParcelas - 1);
            var taxaFinal = flex + taxaMDR;
            return taxaFinal;
        }

        #endregion

        #region Debito

        /// <summary>
        /// Monta dados Débito
        /// </summary>
        /// <param name="dadosBancarios">Dados bancários</param>
        /// <returns></returns>
        private static List<Core.Taxas.Bandeira> MontarDebito(List<DadosBancarios> dadosBancarios)
        {
            var bandeiras = new List<Core.Taxas.Bandeira>();
            
            Core.Taxas.Bandeira bandeira = null;

            foreach (DadosBancarios dado in dadosBancarios)
            {
                // se é a primeira vez ou se ja nao é a sequencia da mesma bandeira
                if ((bandeira == null) || (bandeira.Codigo != dado.CodigoCartao.ToInt32(0)))
                {
                    if (bandeira != null)
                        bandeiras.Add(bandeira);

                    bandeira = new Core.Taxas.Bandeira();
                    bandeira.Taxas = new List<Core.Taxas.Taxa>();
                    bandeira.Flex = new List<Core.Taxas.Flex>();
                    bandeira.Codigo = dado.CodigoCartao.ToInt32();
                }

                bandeira.Nome = dado.DescricaoCartao;
                bandeira.NomeBanco = dado.NomeBanco;
                bandeira.CodigoAgencia = dado.CodigoAgencia;
                bandeira.ContaAtualizada = dado.ContaAtualizada;

                var taxa = new Core.Taxas.Taxa();
                taxa.ModalidadeVenda = dado.DescricaoFEAT.ToLower();
                taxa.ValorTaxa = dado.Taxa;

                if (!dado.CodigoFEAT.Equals("5") && !dado.TemTarifa)
                {
                    if (!dado.TemTarifa)
                        taxa.Prazo = String.Format("{0} dias", dado.PercentualTaxa);
                    else
                        taxa.Prazo = String.Format("{0} dias", dado.PercentualTarifa);
                    taxa.Tarifa = dado.Tarifa;
                }
                else
                {
                    taxa.Prazo = "De 2 a 6 Parcelas";
                    taxa.ValorTaxaEmissor = dado.Tarifa;
                    taxa.Predatado = true;
                }

                bandeira.Taxas.Add(taxa);
            }

            return bandeiras;
        }
        #endregion

        #region Voucher

        /// <summary>
        /// Monta retorno Voucher
        /// </summary>
        private static List<Core.Taxas.Bandeira> MontarVoucher(List<DadosBancarios> dadosBancarios)
        {
            var bandeiras = new List<Core.Taxas.Bandeira>();

            Core.Taxas.Bandeira bandeira = null;
            foreach (DadosBancarios dado in dadosBancarios)
            {
                if (!dado.CodigoCartao.ToInt32().Equals(61) && !dado.CodigoFEAT.ToInt32().Equals(88))
                {
                    // se é a primeira vez ou se ja nao é a sequencia da mesma bandeira
                    if ((bandeira == null) || (bandeira.Codigo != dado.CodigoCartao.ToInt32()))
                    {
                        if (bandeira != null)
                            bandeiras.Add(bandeira);

                        bandeira = new Core.Taxas.Bandeira();
                        bandeira.Codigo = String.Format("{0}{1}", dado.CodigoCartao, dado.CodigoFEAT).ToInt32();
                    }

                    bandeira.Nome = dado.DescricaoFEAT;
                }
            }
            return bandeiras;
        }
        #endregion
    }
}