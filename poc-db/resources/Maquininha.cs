using Redecard.PN.Comum;
using Redecard.PN.OutrosServicos.Agentes.MaquininhaContaCerta;
using Redecard.PN.OutrosServicos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redecard.PN.OutrosServicos.Negocio
{
    /// <summary>
    /// Regras de negócio envolvando os métodos para consulta aos dados da oferta maquininha conta certa junto ao ZP
    /// </summary>
    public class Maquininha : RegraDeNegocioBase
    {
        /// <summary>
        /// Consulta o contrato da oferta maquininha conta certa
        /// </summary>
        /// <param name="numEstabelecimento">Código do estabelecimento</param>
        /// <param name="dataFimVigencia">Data fim de vigência</param>
        /// <param name="codigoStatusContrato">Código da situação do contrato</param>
        /// <returns>Sumário do contrato maquininha conta certa</returns>
        public MaquininhaContrato ConsultaContrato(Int32 numEstabelecimento, DateTime dataFimVigencia, Int16? codigoStatusContrato)
        {
            using (Logger log = Logger.IniciarLog("Maquininha Conta Certa - ConsultaContrato"))
            {
                try
                {
                    Int16 codSitContratoZP = 0;
                    if (codigoStatusContrato.HasValue)
                    {
                        // de...para do código da situação do contrato UK vs ZP
                        switch (codigoStatusContrato.Value)
                        {
                            case 0:
                                codSitContratoZP = 1;
                                break;
                            case 1:
                                codSitContratoZP = 3;
                                break;
                            case 2:
                                codSitContratoZP = 2;
                                break;
                            default:
                                codSitContratoZP = 0;
                                break;
                        }
                    }

                    ContratoMaquininha contratoZP = new Agentes.Maquininha().ConsultaContrato(
                        numEstabelecimento, dataFimVigencia, codSitContratoZP);

                    MaquininhaContrato retorno = new MaquininhaContrato
                    {
                        Cnpj = contratoZP.Cnpj,
                        CodigoCanalItau = contratoZP.CodigoCanalItau,
                        CodigoMotivoRecusaContrato = contratoZP.CodigoMotivoRecusaContrato,
                        CodigoOfertaPacote = contratoZP.CodigoOfertaPacote,
                        CodigoSituacaoContrato = contratoZP.CodigoSituacaoContrato,
                        CodigoTecnologia = contratoZP.CodigoTecnologia,
                        CodigoUsuarioInclusaoOuAlteracao = contratoZP.CodigoUsuarioInclusaoOuAlteracao,
                        DataAtivacao = contratoZP.DataAtivacao,
                        DataCancelamento = contratoZP.DataCancelamento,
                        DataContrato = contratoZP.DataContrato,
                        DataFimVigencia = contratoZP.DataFimVigencia,
                        DataInicioVigencia = contratoZP.DataInicioVigencia,
                        DataSolicitacaoCancelamento = contratoZP.DataSolicitacaoCancelamento,
                        DataUsuarioInclusaoAlteracao = contratoZP.DataUsuarioInclusaoAlteracao,
                        Descricao = contratoZP.Descricao,
                        DescricaoCanalItau = contratoZP.DescricaoCanalItau,
                        DescricaoMotivoRecusaContrato = contratoZP.DescricaoMotivoRecusaContrato,
                        NumEstabelecimento = contratoZP.NumEstabelecimento,
                        QtdTerminaisDoPacote = contratoZP.QtdTerminaisDoPacote,
                        ValorBaseDemaisTerminais = contratoZP.ValorBaseDemaisTerminais,
                        ValorBasePrimeiroTerminal = contratoZP.ValorBasePrimeiroTerminal,
                        ValorPacoteBasico = contratoZP.ValorPacoteBasico,
                        ValorTerminais = contratoZP.ValorTerminais,
                        ValorTotalPacote = contratoZP.ValorTotalPacote
                    };

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta as faixas de faturamento das metas do contrato maquininha conta certa
        /// </summary>
        /// <param name="numPdv">Código do estabelecimento</param>
        /// <returns>Faixa de faturamento das metas</returns>
        public List<MaquininhaMetas> ConsultaMetas(Int32 numPdv)
        {
            using (Logger log = Logger.IniciarLog("Maquininha Conta Certa - ConsultaMetas"))
            {
                try
                {
                    Metas[] metasZP = new Agentes.Maquininha().ConsultaMetas(numPdv);
                    if (metasZP == null)
                        return new List<MaquininhaMetas>();

                    List<MaquininhaMetas> metas = metasZP.Select(x =>
                    {
                        return new MaquininhaMetas
                        {
                            NumeroPdv = x.NumeroPdv,
                            ValorComboMaquininha = x.ValorComboMaquininha,
                            ValorComboPacote = x.ValorComboPacote,
                            ValorDescontoPacote = x.ValorDescontoPacote,
                            ValorMetaFinal = x.ValorMetaFinal,
                            ValorMetaInicial = x.ValorMetaInicial
                        };
                    }).ToList();

                    return metas;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta o histórico de apuração
        /// </summary>
        /// <param name="numPdv">Código do estabelecimento</param>
        /// <returns>Lista com o histórico de apuração</returns>
        public List<MaquininhaHistoricoApuracao> ConsultaHistoricoApuracao(Int32 numPdv)
        {
            using (Logger log = Logger.IniciarLog("Maquininha Conta Certa - ConsultaHistoricoApuracao"))
            {
                try
                {
                    HistoricoApuracao[] historicoZP = new Agentes.Maquininha().ConsultaHistoricoApuracao(numPdv);
                    if (historicoZP == null)
                        return new List<MaquininhaHistoricoApuracao>();

                    List<MaquininhaHistoricoApuracao> historico = historicoZP.Select(x =>
                    {
                        return new MaquininhaHistoricoApuracao
                        {
                            DataMesApuracao = x.DataMesApuracao,
                            DataMesReferencia = x.DataMesReferencia,
                            ValorAluguelMaquininha = x.ValorAluguelMaquininha,
                            ValorFaturamentoApurado = x.ValorFaturamentoApurado,
                            ValorFinalFaixaDesconto = x.ValorFinalFaixaDesconto,
                            ValorInicialFaixaDesconto = x.ValorInicialFaixaDesconto,
                            ValorPacoteBasico = x.ValorPacoteBasico,
                            ValorTotalPacote = x.ValorTotalPacote
                        };
                    }).ToList();

                    return historico;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
    }
}
