using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Negocio
{
    /// <summary>
    /// Negócio Informações Comerciais
    /// </summary>
    public class InformacoesComerciais : RegraDeNegocioBase
    {
        /// <summary>
        /// Retorna informações de condições comerciais caso não haja aceite realizado
        /// </summary>
        /// <param name="numeroPv"></param>
        /// <returns></returns>
        public Modelo.InformacaoComercial.InformacaoComercial Consultar(Int64 numeroPv)
        {
            Modelo.InformacaoComercial.InformacaoComercial retorno = new Modelo.InformacaoComercial.InformacaoComercial();
            try
            {
                // Retorna listagem do objeto Modelo.Terminal preenchido
                retorno = new Agentes.InformacaoComercial().Consultar(numeroPv);

                if (retorno.NumeroPDV != null && retorno.NumeroPDV != default(Decimal?))
                {
                    var agenteZP = new Agentes.TerminalContratado();

                    retorno.ValorServico = agenteZP.ObterValorServico(302);

                    retorno.Terminais = agenteZP.ConsultarListaTerminais(numeroPv).ToArray();

                    var dadosPrecoUnico = agenteZP.ObterDadosPrecoUnico(numeroPv);

                    if (dadosPrecoUnico.CodigoOferta != 0)
                    {
                        Modelo.InformacaoComercial.PrecoUnico precoUnico = new Modelo.InformacaoComercial.PrecoUnico()
                        {
                            ContemPrecoUnico = false,
                            ContemPrecoUnicoFlex = false
                        };

                        List<Modelo.InformacaoComercial.TerminalPrecoUnico> listaTerminaisPrecoUnico = new List<Modelo.InformacaoComercial.TerminalPrecoUnico>();
                        var featurePrecoUnico = dadosPrecoUnico.Features.FirstOrDefault();

                        if (String.Compare(featurePrecoUnico.IndicadorProdutoFlex, "S", true) == 0)
                        {
                            precoUnico.ContemPrecoUnico = false;
                            precoUnico.ContemPrecoUnicoFlex = true;
                            precoUnico.ValorAVista = precoUnico.PrimeiraParcela = featurePrecoUnico.PercentualTaxa1 > 0 ? (featurePrecoUnico.PercentualTaxa1 / 100) : featurePrecoUnico.PercentualTaxa1;
                            precoUnico.ParcelaAdicional = featurePrecoUnico.PercentualTaxa2 > 0 ? (featurePrecoUnico.PercentualTaxa2 / 100) : featurePrecoUnico.PercentualTaxa2;
                            if (dadosPrecoUnico.Terminais != null && dadosPrecoUnico.Terminais.Count() > 0)
                            {
                                dadosPrecoUnico.Terminais.ToList().ForEach(t => listaTerminaisPrecoUnico.Add(new Modelo.InformacaoComercial.TerminalPrecoUnico()
                                {
                                    ValorFaturamentoContrato = dadosPrecoUnico.ValorFaturamentoContrato,
                                    ValorPrecoUnicoComFlex = dadosPrecoUnico.ValorPrecoUnicoComFlex,
                                    ValorPrecoUnicoSemFlex = dadosPrecoUnico.ValorPrecoUnicoSemFlex,
                                    QuantidadeEquipamento = t.QuantidadeEquipamento.ToString(),
                                    TipoEquipamento = t.TipoEquipamento
                                }));
                            }
                        }
                        else
                        {
                            precoUnico.ContemPrecoUnico = true;
                            precoUnico.ContemPrecoUnicoFlex = false;

                            if (dadosPrecoUnico.Terminais != null && dadosPrecoUnico.Terminais.Count() > 0)
                            {
                                dadosPrecoUnico.Terminais.ToList().ForEach(t => listaTerminaisPrecoUnico.Add(new Modelo.InformacaoComercial.TerminalPrecoUnico()
                                {
                                    ValorFaturamentoContrato = dadosPrecoUnico.ValorFaturamentoContrato,
                                    ValorPrecoUnicoComFlex = dadosPrecoUnico.ValorPrecoUnicoComFlex,
                                    ValorPrecoUnicoSemFlex = dadosPrecoUnico.ValorPrecoUnicoSemFlex,
                                    QuantidadeEquipamento = t.QuantidadeEquipamento.ToString(),
                                    TipoEquipamento = t.TipoEquipamento
                                }));
                            }
                        }
                        precoUnico.Terminais = listaTerminaisPrecoUnico.ToArray();
                        retorno.DadosPrecoUnico = precoUnico;
                    }
                }
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
            return retorno;
        }

        /// <summary>
        /// Retorna informações de condições comerciais
        /// </summary>
        /// <param name="numeroPv"></param>
        /// <returns></returns>
        public Modelo.InformacaoComercial.InformacaoComercial Recuperar(Int64 numeroPv)
        {
            Modelo.InformacaoComercial.InformacaoComercial retorno = new Modelo.InformacaoComercial.InformacaoComercial();
            try
            {
                // Retorna listagem do objeto Modelo.Terminal preenchido
                retorno = new Agentes.InformacaoComercial().Recuperar(numeroPv);

                var agenteZP = new Agentes.TerminalContratado();

                retorno.ValorServico = agenteZP.ObterValorServico(302);

                retorno.Terminais = agenteZP.ConsultarListaTerminais(numeroPv).ToArray();

                var dadosPrecoUnico = agenteZP.ObterDadosPrecoUnico(numeroPv);
                if (dadosPrecoUnico.CodigoOferta != 0)
                {
                    Modelo.InformacaoComercial.PrecoUnico precoUnico = new Modelo.InformacaoComercial.PrecoUnico()
                    {
                        ContemPrecoUnico = false,
                        ContemPrecoUnicoFlex = false
                    };

                    List<Modelo.InformacaoComercial.TerminalPrecoUnico> listaTerminaisPrecoUnico = new List<Modelo.InformacaoComercial.TerminalPrecoUnico>();
                    var featurePrecoUnico = dadosPrecoUnico.Features.FirstOrDefault();

                    if (String.Compare(featurePrecoUnico.IndicadorProdutoFlex, "S", true) == 0)
                    {
                        precoUnico.ContemPrecoUnico = false;
                        precoUnico.ContemPrecoUnicoFlex = true;
                        precoUnico.ValorAVista = precoUnico.PrimeiraParcela = featurePrecoUnico.PercentualTaxa1 > 0 ? (featurePrecoUnico.PercentualTaxa1 / 100) : featurePrecoUnico.PercentualTaxa1;
                        precoUnico.ParcelaAdicional = featurePrecoUnico.PercentualTaxa2 > 0 ? (featurePrecoUnico.PercentualTaxa2 / 100) : featurePrecoUnico.PercentualTaxa2;
                        if (dadosPrecoUnico.Terminais != null && dadosPrecoUnico.Terminais.Count() > 0)
                        {
                            dadosPrecoUnico.Terminais.ToList().ForEach(t => listaTerminaisPrecoUnico.Add(new Modelo.InformacaoComercial.TerminalPrecoUnico()
                            {
                                ValorFaturamentoContrato = dadosPrecoUnico.ValorFaturamentoContrato,
                                ValorPrecoUnicoComFlex = dadosPrecoUnico.ValorPrecoUnicoComFlex,
                                ValorPrecoUnicoSemFlex = dadosPrecoUnico.ValorPrecoUnicoSemFlex,
                                QuantidadeEquipamento = t.QuantidadeEquipamento.ToString(),
                                TipoEquipamento = t.TipoEquipamento
                            }));
                        }
                    }
                    else
                    {
                        precoUnico.ContemPrecoUnico = true;
                        precoUnico.ContemPrecoUnicoFlex = false;

                        if (dadosPrecoUnico.Terminais != null && dadosPrecoUnico.Terminais.Count() > 0)
                        {
                            dadosPrecoUnico.Terminais.ToList().ForEach(t => listaTerminaisPrecoUnico.Add(new Modelo.InformacaoComercial.TerminalPrecoUnico()
                            {
                                ValorFaturamentoContrato = dadosPrecoUnico.ValorFaturamentoContrato,
                                ValorPrecoUnicoComFlex = dadosPrecoUnico.ValorPrecoUnicoComFlex,
                                ValorPrecoUnicoSemFlex = dadosPrecoUnico.ValorPrecoUnicoSemFlex,
                                QuantidadeEquipamento = t.QuantidadeEquipamento.ToString(),
                                TipoEquipamento = t.TipoEquipamento
                            }));
                        }
                    }
                    precoUnico.Terminais = listaTerminaisPrecoUnico.ToArray();
                    retorno.DadosPrecoUnico = precoUnico;
                }
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
            return retorno;
        }

        /// <summary>
        /// Altera o status do aceite de condições comerciais
        /// </summary>
        /// <param name="codigoUsuario"></param>
        /// <param name="numeroPv"></param>
        /// <param name="status"></param>
        public void AlterarStatus(Int64 codigoUsuario, Int64 numeroPv, String status)
        {
            try
            {
                // Instancia classe agente
                new Agentes.InformacaoComercial().AlterarStatusAceite(codigoUsuario, numeroPv, status);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
    }
}
