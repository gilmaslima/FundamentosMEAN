using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Redecard.PN.Comum;
using Redecard.PN.OutrosServicos.Agentes.COMTINKPlanoContasServicos;
using Redecard.PN.OutrosServicos.Agentes.COMTIZPPlanoContasServicos;
using Redecard.PN.OutrosServicos.Modelo.PlanoContas;

namespace Redecard.PN.OutrosServicos.Agentes
{
    /// <summary>
    /// Classe agente de acesso ao mainframe para as consultas realizadas nos módulos<br/>
    /// - Plano de Contas - Conta Certa<br/>
    /// - Japão - Bônus Rede<br/>
    /// - Turquia - Preço Único.
    /// </summary>
    /// <remarks>
    /// Books utilizados na classe:<br/>
    /// - Book ZPCA1790	/ Programa ZP1790 / TranID ZPP0 / Método ConsultarOfertas<br/>
    /// - Book ZPCA1791	/ Programa ZP1791 / TranID ZPP1 / Método ConsultarMetasOfertas<br/>
    /// - Book ZPCA1792	/ Programa ZP1792 / TranID ZPP2 / Método ConsultarAnosReferencia<br/>
    /// - Book ZPCA1793	/ Programa ZP1793 / TranID ZPP3 / Método ConsultarFaturamentoAnoReferencia<br/>
    /// - Book BKNK0070 / Programa NKC007 / TranID NK07 / Método ConsultarCompensacoesDebitos<br/>
    /// - Book BKZP7030 / Programa ZPC703 / TranID ZPJA / Método ConsultarTipoOfertaAtiva<br/>
    /// - Book BKZP7040 / Programa ZPC704 / TranID ZPJB / Método ConsultarDadosOfertaAceite<br/>
    /// - Book BKZP7050 / Programa ZPC705 / TranID ZPJC / Método ConsultarDadosCelularBonus<br/>
    /// - Book BKZP7060 / Programa ZPC706 / TranID ZPJD / Método ConsultarDadosApuracao<br/>
    /// - Book BKZP7070 / Programa ZPC707 / TranID ZPJE / Método ConsultarDadosApuracaoDetalhes<br/>
    /// - Book BKNK0080 / Programa NKC008 / TranID NK08 / Método ConsultarPlanosPrecoUnicoContratados
    /// </remarks>
    public class PlanoContas: AgentesBase
    {
        /// <summary>
        /// Consulta de ofertas.<br/>        
        /// - Book ZPCA1790	/ Programa ZP1790 / TranID ZPP0 / Método ConsultarOfertas
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1790	/ Programa ZP1790 / TranID ZPP0 / Método ConsultarOfertas
        /// </remarks>
        /// <param name="codigoRetorno">Código de retorno do Serviço</param>
        /// <param name="dataFim">Data de término do período de vigência da pesquisa</param>
        /// <param name="dataInicio">Data de início do período de vigência da pesquisa</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="tipoEstabelecimento">Tipo do estabelecimento</param>
        /// <returns>Ofertas</returns>
        public List<Oferta> ConsultarOfertas(
            Int32 numeroPV,             
            DateTime dataInicio, 
            DateTime dataFim, 
            Int16 tipoEstabelecimento, 
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Plano Contas - Consulta de Ofertas"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { numeroPV, dataInicio, dataFim, tipoEstabelecimento });

                try
                {
                    //Declaração a atribuição inicial de variáveis auxiliares
                    codigoRetorno = default(Int16);
                    String msgRetorno = default(String);
                    Int16 quantidadeOfertas = default(Int16);
                    String indicadorOfertaAtual = default(String);
                    String reservaDados1 = default(String);
                    String reservaDados2 = default(String);
                    String periodoInicio = dataInicio.ToString("dd/MM/yyyy");
                    String periodoFim = dataFim.ToString("dd/MM/yyyy");
                    ZPCA1790_TAB_OFR[] retornoHis = null;
                    List<Oferta> retorno = new List<Oferta>();

                    //Chamando serviço mainframe
                    using (var client = new COMTIZPClient())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { 
                            numeroPV, periodoInicio, periodoFim, tipoEstabelecimento, reservaDados1 });

#if !DEBUG                        
                        retornoHis = client.ConsultarOfertas(
                            out codigoRetorno, out msgRetorno, out quantidadeOfertas, out indicadorOfertaAtual,
                            out reservaDados2, numeroPV, periodoInicio, periodoFim, tipoEstabelecimento, reservaDados1);
#else
                        retornoHis = new ZPCA1790_TAB_OFR[] {
                            new ZPCA1790_TAB_OFR { ZPCA1790_COD_OFR = 11, ZPCA1790_COD_PRO = 11001, ZPCA1790_AGE_ADS = 1100, ZPCA1790_CTA_ADS = "11000-1", ZPCA1790_DAT_ADS = "01/01/2001", ZPCA1790_DES_OFR = "Oferta 1", ZPCA1790_DET_OFR = "Descrição Oferta 1", ZPCA1790_VIG_INI = "01/01/2001", ZPCA1790_VIG_FIM = "01/12/2011" }, 
                            new ZPCA1790_TAB_OFR { ZPCA1790_COD_OFR = 22, ZPCA1790_COD_PRO = 22002, ZPCA1790_AGE_ADS = 2200, ZPCA1790_CTA_ADS = "22000-2", ZPCA1790_DAT_ADS = "02/02/2002", ZPCA1790_DES_OFR = "Oferta 2", ZPCA1790_DET_OFR = "Descrição Oferta 2", ZPCA1790_VIG_INI = "01/02/2002", ZPCA1790_VIG_FIM = "02/12/2012" }, 
                            new ZPCA1790_TAB_OFR { ZPCA1790_COD_OFR = 33, ZPCA1790_COD_PRO = 33003, ZPCA1790_AGE_ADS = 3300, ZPCA1790_CTA_ADS = "33000-3", ZPCA1790_DAT_ADS = "03/03/2003", ZPCA1790_DES_OFR = "Oferta 3", ZPCA1790_DET_OFR = "Descrição Oferta 3", ZPCA1790_VIG_INI = "01/03/2003", ZPCA1790_VIG_FIM = "03/12/2013" }, 
                            new ZPCA1790_TAB_OFR { ZPCA1790_COD_OFR = 44, ZPCA1790_COD_PRO = 44004, ZPCA1790_AGE_ADS = 4400, ZPCA1790_CTA_ADS = "44000-4", ZPCA1790_DAT_ADS = "04/04/2004", ZPCA1790_DES_OFR = "Oferta 4", ZPCA1790_DET_OFR = "Descrição Oferta 4", ZPCA1790_VIG_INI = "01/04/2004", ZPCA1790_VIG_FIM = "04/12/2014" }, 
                            new ZPCA1790_TAB_OFR { ZPCA1790_COD_OFR = 55, ZPCA1790_COD_PRO = 55005, ZPCA1790_AGE_ADS = 5500, ZPCA1790_CTA_ADS = "55000-5", ZPCA1790_DAT_ADS = "05/05/2005", ZPCA1790_DES_OFR = "Oferta 5", ZPCA1790_DET_OFR = "Descrição Oferta 5", ZPCA1790_VIG_INI = "01/05/2005", ZPCA1790_VIG_FIM = "05/12/2015" }
                        };
                        quantidadeOfertas = (Int16)retornoHis.Length;
                        indicadorOfertaAtual = new Random().Next(2) % 2 == 0 ? "S" : "N";
#endif

                        log.GravarLog(EventoLog.RetornoHIS, new { retornoHIS = retornoHis, codigoRetorno, msgRetorno,
                            quantidadeOfertas, indicadorOfertaAtual, reservaDados2 });
                    }

                    //00 = TRANSACAO OK
                    //04 = OFERTAS NAO ENCONTRADAS
                    //08 = DADO DE ENTRADA INVALIDO
                    //99 = OUTROS ERROS
                    if (codigoRetorno == 0)
                    {
                        if (retornoHis != null)
                        {
                            //Remove os elementos excedentes do array retornado pelo mainframe
                            retornoHis = retornoHis.Take(quantidadeOfertas).ToArray();

                            //Mapeamento da estrutura HIS para array da classe de modelo
                            for (Int32 iItem = 0; iItem < retornoHis.Length; iItem++)
                            {
                                ZPCA1790_TAB_OFR itemRetornoHis = retornoHis[iItem];
                                retorno.Add(new Oferta
                                {
                                    AgenciaAdesao = itemRetornoHis.ZPCA1790_AGE_ADS,
                                    CodigoOferta = itemRetornoHis.ZPCA1790_COD_OFR,
                                    CodigoProposta = itemRetornoHis.ZPCA1790_COD_PRO,
                                    ContaAdesao = itemRetornoHis.ZPCA1790_CTA_ADS,
                                    DataAdesao = itemRetornoHis.ZPCA1790_DAT_ADS.ToDate("dd/MM/yyyy", DateTime.MinValue),
                                    DescricaoOferta = itemRetornoHis.ZPCA1790_DET_OFR,
                                    NomeOferta = itemRetornoHis.ZPCA1790_DES_OFR,
                                    PeriodoFinalVigencia = itemRetornoHis.ZPCA1790_VIG_FIM.ToDate("dd/MM/yyyy", DateTime.MinValue),
                                    PeriodoInicialVigencia = itemRetornoHis.ZPCA1790_VIG_INI.ToDate("dd/MM/yyyy", DateTime.MinValue),
                                    IndicadorOfertaAtual = "S".CompareTo(indicadorOfertaAtual) == 0 && iItem == 0
                                });
                            }
                        }
                    }

                    log.GravarLog(EventoLog.RetornoAgente, new { retorno, codigoRetorno });

                    return retorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta metas de ofertas.<br/>        
        /// - Book ZPCA1791	/ Programa ZP1791 / TranID ZPP1 / Método ConsultarMetasOfertas
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1791	/ Programa ZP1791 / TranID ZPP1 / Método ConsultarMetasOfertas
        /// </remarks>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="tipoEstabelecimento">Tipo do estabelecimento</param>
        /// <returns>Metas da Oferta</returns>
        public List<MetaOferta> ConsultarMetasOferta(
            Int32 codigoOferta, 
            Int32 numeroPV,             
            Int16 tipoEstabelecimento, 
            Decimal codigoProposta, 
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Plano Contas - Consulta de Metas de Oferta"))
            {
                log.GravarLog(EventoLog.InicioAgente, 
                    new { codigoOferta, numeroPV, tipoEstabelecimento, codigoProposta });

                try
                {
                    //Declaração a atribuição inicial de variáveis auxiliares
                    codigoRetorno = default(Int16);
                    String msgRetorno = default(String);
                    Int16 quantidadeMetas = default(Int16);
                    String reservaDados1 = default(String);
                    String reservaDados2 = default(String);                    
                    ZPCA1791_TAB_MET[] retornoHis = null;
                    List<MetaOferta> retorno = new List<MetaOferta>();

                    //Chamando serviço mainframe
                    using (var client = new COMTIZPClient())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { codigoOferta, numeroPV, 
                            codigoProposta, tipoEstabelecimento, reservaDados1 });

#if !DEBUG
                        retornoHis = client.ConsultarMetasOferta(out codigoRetorno, out msgRetorno, 
                            out quantidadeMetas, out reservaDados2, codigoOferta, numeroPV,
                            codigoProposta, tipoEstabelecimento, reservaDados1);
#else
                        retornoHis = new ZPCA1791_TAB_MET[] { 
                            new ZPCA1791_TAB_MET { ZPCA1791_VAL_OFT = 0, ZPCA1791_VAL_INI = 0, ZPCA1791_VAL_FIM = 5000, ZPCA1791_PRC_OFT = 1, ZPCA1791_QTD_TRM = 1 },
                            new ZPCA1791_TAB_MET { ZPCA1791_VAL_OFT = 10, ZPCA1791_VAL_INI = 5000, ZPCA1791_VAL_FIM = 10000, ZPCA1791_PRC_OFT = 100, ZPCA1791_QTD_TRM = 250 },
                            new ZPCA1791_TAB_MET { ZPCA1791_VAL_OFT = 20, ZPCA1791_VAL_INI = 10000, ZPCA1791_VAL_FIM = 15000, ZPCA1791_PRC_OFT = 200, ZPCA1791_QTD_TRM = 350 },
                            new ZPCA1791_TAB_MET { ZPCA1791_VAL_OFT = 30, ZPCA1791_VAL_INI = 15000, ZPCA1791_VAL_FIM = 20000, ZPCA1791_PRC_OFT = 300, ZPCA1791_QTD_TRM = 0 },
                            new ZPCA1791_TAB_MET { ZPCA1791_VAL_OFT = 40, ZPCA1791_VAL_INI = 20000, ZPCA1791_VAL_FIM = 25000, ZPCA1791_PRC_OFT = 0, ZPCA1791_QTD_TRM = 550 },
                            new ZPCA1791_TAB_MET { ZPCA1791_VAL_OFT = 50, ZPCA1791_VAL_INI = 25000, ZPCA1791_VAL_FIM = 0, ZPCA1791_PRC_OFT = 500, ZPCA1791_QTD_TRM = 650 },
                        };
                        quantidadeMetas = (Int16)retornoHis.Length;
#endif

                        log.GravarLog(EventoLog.RetornoHIS, 
                            new { retornoHIS = retornoHis, codigoRetorno, msgRetorno, quantidadeMetas, reservaDados2 });
                    }

                    //00 = TRANSACAO OK
                    //04 = METAS NAO ENCONTRADAS
                    //08 = DADO DE ENTRADA INVALIDO
                    //99 = OUTROS ERROS
                    if (codigoRetorno == 0)
                    {
                        if (retornoHis != null)
                        {
                            //Remove os elementos excedentes do array retornado pelo mainframe
                            retornoHis = retornoHis.Take(quantidadeMetas).ToArray();

                            //Mapeamento da estrutura HIS para array da classe de modelo
                            foreach (ZPCA1791_TAB_MET itemRetornoHIS in retornoHis)
                            {
                                retorno.Add(new MetaOferta
                                {
                                    ValorFinal = itemRetornoHIS.ZPCA1791_VAL_FIM / 100m,
                                    ValorInicial = itemRetornoHIS.ZPCA1791_VAL_INI / 100m,
                                    ValorOferta = itemRetornoHIS.ZPCA1791_VAL_OFT / 100m,
                                    PercentualOferta = itemRetornoHIS.ZPCA1791_PRC_OFT / 100m,
                                    QuantidadeTerminais = itemRetornoHIS.ZPCA1791_QTD_TRM
                                });
                            }
                        }
                    }

                    log.GravarLog(EventoLog.RetornoAgente, new { retorno, codigoRetorno });

                    return retorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta Apuração - Lista de Ano(s) de Referência(s).<br/>
        /// - Book ZPCA1792	/ Programa ZP1792 / TranID ZPP2 / Método ConsultarAnosReferencia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1792	/ Programa ZP1792 / TranID ZPP2 / Método ConsultarAnosReferencia
        /// </remarks>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="tipoEstabelecimento">Tipo do estabelecimento</param>
        /// <returns>Anos Referência da Oferta</returns>
        public List<Int16> ConsultarAnosReferencia(
            Int32 codigoOferta, 
            Int32 numeroPV,
            Int16 tipoEstabelecimento, 
            Decimal codigoProposta, 
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Plano Contas - Consulta de Anos de Referência"))
            {
                log.GravarLog(EventoLog.InicioAgente, 
                    new { codigoOferta, numeroPV, tipoEstabelecimento, codigoProposta });

                try
                {
                    //Declaração a atribuição inicial de variáveis auxiliares
                    codigoRetorno = default(Int16);
                    String msgRetorno = default(String);
                    Int16 quantidadeAnos = default(Int16);
                    String reservaDados1 = default(String);
                    String reservaDados2 = default(String);
                    ZPCA1792_TAB_ANO[] retornoHis = null;
                    List<Int16> retorno = new List<Int16>();

                    //Chamando serviço mainframe
                    using (var client = new COMTIZPClient())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { codigoOferta, numeroPV, 
                            codigoProposta, tipoEstabelecimento, reservaDados1 });

#if !DEBUG
                        retornoHis = client.ConsultarAnosReferencia(out codigoRetorno, out msgRetorno, 
                            out quantidadeAnos, out reservaDados2, codigoOferta, numeroPV,
                            codigoProposta, tipoEstabelecimento, reservaDados1);
#else                        
                        retornoHis = new ZPCA1792_TAB_ANO[] {
                            new ZPCA1792_TAB_ANO { ZPCA1792_ANO_REF = 2000 },
                            new ZPCA1792_TAB_ANO { ZPCA1792_ANO_REF = 2002 },
                            new ZPCA1792_TAB_ANO { ZPCA1792_ANO_REF = 2007 },
                            new ZPCA1792_TAB_ANO { ZPCA1792_ANO_REF = 2012 },
                            new ZPCA1792_TAB_ANO { ZPCA1792_ANO_REF = 2013 }
                        };
                        quantidadeAnos = (Int16) retornoHis.Length;
#endif
                        log.GravarLog(EventoLog.RetornoHIS, new { retornoHIS = retornoHis, codigoRetorno, msgRetorno, 
                            quantidadeAnos, reservaDados2 });
                    }

                    //00 = TRANSACAO OK
                    //04 = ANOS REFERENCIA NAO ENCONTRADOS
                    //08 = DADO DE ENTRADA INVALIDO
                    //99 = OUTROS ERROS
                    if (codigoRetorno == 0)
                    {
                        if (retornoHis != null)
                        {
                            //Remove os elementos excedentes do array retornado pelo mainframe
                            retornoHis = retornoHis.Take(quantidadeAnos).ToArray();

                            //Mapeamento da estrutura HIS para array
                            foreach (ZPCA1792_TAB_ANO itemRetornoHIS in retornoHis)
                                retorno.Add(itemRetornoHIS.ZPCA1792_ANO_REF);                           
                        }
                    }

                    log.GravarLog(EventoLog.RetornoAgente, new { retorno, codigoRetorno });

                    return retorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta Apuração - Faturamento Ano/Mês Referência.<br/>
        /// - Book ZPCA1793	/ Programa ZP1793 / TranID ZPP3 / Método ConsultarFaturamentosAnoReferencia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1793	/ Programa ZP1793 / TranID ZPP3 / Método ConsultarFaturamentosAnoReferencia
        /// </remarks>
        /// <param name="anoReferencia">Ano referência</param>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="tipoEstabelecimento">Tipo do estabelecimento</param>
        /// <returns>Faturamentos no Ano</returns>
        public List<Faturamento> ConsultarFaturamento(
            Int32 codigoOferta, 
            Int32 numeroPV,             
            Int16 tipoEstabelecimento, 
            Decimal codigoProposta, 
            Int16 anoReferencia,
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Plano Contas - Consulta Faturamento de Ano Referência"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { codigoOferta, numeroPV, 
                    tipoEstabelecimento, codigoProposta, anoReferencia });

                try
                {
                    //Declaração a atribuição inicial de variáveis auxiliares
                    codigoRetorno = default(Int16);
                    String msgRetorno = default(String);
                    Int16 quantidadeFaturamentos = default(Int16);
                    String reservaDados1 = default(String);
                    String reservaDados2 = default(String);
                    ZPCA1793_TAB_FAT[] retornoHis = null;
                    List<Faturamento> retorno = new List<Faturamento>();

                    //Chamando serviço mainframe
                    using (var client = new COMTIZPClient())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { codigoOferta, numeroPV,
                            codigoProposta, tipoEstabelecimento, anoReferencia, reservaDados1 });

#if !DEBUG
                        retornoHis = client.ConsultarFaturamentosAnoReferencia(out codigoRetorno, out msgRetorno,
                            out quantidadeFaturamentos, out reservaDados2, codigoOferta, numeroPV,
                            codigoProposta, tipoEstabelecimento, anoReferencia, reservaDados1);
#else
                        retornoHis = new ZPCA1793_TAB_FAT[] {                             
                                new ZPCA1793_TAB_FAT { ZPCA1793_MES_REF = 1, ZPCA1793_BCO_CRE = 341, ZPCA1793_VAL_ALG = 1000, ZPCA1793_VAL_FAT = 1100, ZPCA1793_IND_ELGV = 90, ZPCA1793_CTA_CRE = "11345-6", ZPCA1793_AGE_CRE = 1111 },
                                new ZPCA1793_TAB_FAT { ZPCA1793_MES_REF = 7, ZPCA1793_BCO_CRE = 123, ZPCA1793_VAL_ALG = 4000, ZPCA1793_VAL_FAT = 4100, ZPCA1793_IND_ELGV = 93, ZPCA1793_CTA_CRE = "44345-6", ZPCA1793_AGE_CRE = 4444 },
                                new ZPCA1793_TAB_FAT { ZPCA1793_MES_REF = 10, ZPCA1793_BCO_CRE = 412, ZPCA1793_VAL_ALG = 5000, ZPCA1793_VAL_FAT = 5100, ZPCA1793_IND_ELGV = 95, ZPCA1793_CTA_CRE = "55345-6", ZPCA1793_AGE_CRE = 5555 },
                                new ZPCA1793_TAB_FAT { ZPCA1793_MES_REF = 3, ZPCA1793_BCO_CRE = 531, ZPCA1793_VAL_ALG = 2000, ZPCA1793_VAL_FAT = 2100, ZPCA1793_IND_ELGV = 96, ZPCA1793_CTA_CRE = "22345-6", ZPCA1793_AGE_CRE = 2222 },
                                new ZPCA1793_TAB_FAT { ZPCA1793_MES_REF = 12, ZPCA1793_BCO_CRE = 231, ZPCA1793_VAL_ALG = 6000, ZPCA1793_VAL_FAT = 6100, ZPCA1793_IND_ELGV = 92, ZPCA1793_CTA_CRE = "66345-6", ZPCA1793_AGE_CRE = 6666 },
                                new ZPCA1793_TAB_FAT { ZPCA1793_MES_REF = 4, ZPCA1793_BCO_CRE = 632, ZPCA1793_VAL_ALG = 3000, ZPCA1793_VAL_FAT = 3100, ZPCA1793_IND_ELGV = 94, ZPCA1793_CTA_CRE = "33345-6", ZPCA1793_AGE_CRE = 3333 }
                        };
                        quantidadeFaturamentos = (Int16)retornoHis.Length;
#endif
                        
                        log.GravarLog(EventoLog.ChamadaHIS, new { retornoHIS = retornoHis, codigoRetorno, 
                            msgRetorno, quantidadeFaturamentos, reservaDados2 });
                    }

                    //00 = TRANSACAO OK
                    //04 = FATURAMENTO NAO ENCONTRADO
                    //08 = DADO DE ENTRADA INVALIDO
                    //99 = OUTROS ERROS
                    if (codigoRetorno == 0)
                    {
                        if (retornoHis != null)
                        {
                            //Mapeamento da estrutura HIS para array da classe de modelo
                            foreach (ZPCA1793_TAB_FAT itemRetornoHIS in retornoHis)
                            {                                                                
                                if (retorno.Count < quantidadeFaturamentos)
                                {
                                    retorno.Add(new Faturamento
                                    {
                                        NumeroPV = numeroPV,                                        
                                        AgenciaRecebimento = itemRetornoHIS.ZPCA1793_AGE_CRE,
                                        ContaRecebimento = itemRetornoHIS.ZPCA1793_CTA_CRE,
                                        StatusElegibilidadeInt = itemRetornoHIS.ZPCA1793_IND_ELGV,
                                        StatusElegibilidade = Enum.IsDefined(typeof(StatusElegibilidade), itemRetornoHIS.ZPCA1793_IND_ELGV)
                                            ? (StatusElegibilidade) itemRetornoHIS.ZPCA1793_IND_ELGV : StatusElegibilidade.NAO_IDENTIFICADO,
                                        MesReferencia = itemRetornoHIS.ZPCA1793_MES_REF,
                                        ValorAluguel = itemRetornoHIS.ZPCA1793_VAL_ALG / 100,
                                        ValorFaturamento = itemRetornoHIS.ZPCA1793_VAL_FAT / 100,
                                        AnoReferencia = anoReferencia,
                                        BancoRecebimento = itemRetornoHIS.ZPCA1793_BCO_CRE
                                    });
                                }                                                             
                            }
                        }
                    }

                    log.GravarLog(EventoLog.RetornoAgente, new { retorno, codigoRetorno });

                    return retorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Retorna as compensações de débitos de aluguel do PV incluídas no Mês/Ano informado.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKNK0070 / Programa NKC007 / TranID NK07 / Método ConsultarCompensacoesDebitos
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKNK0070 / Programa NKC007 / TranID NK07 / Método ConsultarCompensacoesDebitos
        /// </remarks>  
        /// <returns>Compensações Débitos</returns>
        public List<CompensacaoDebitoAluguel> ConsultarCompensacoesDebitos(
            Int32 numeroPv, 
            DateTime mesAnoDebito, 
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Compensações de débitos - BKNK0070 / NKC007 / NK07"))
            {
                try
                {
                    //Preparação de parâmetros de chamada e retorno HIS
                    var sqlCode = default(Int16);
                    var mensagem = default(String);
                    var quantidadeOcorrencias = default(Int16);
                    var ocorrencias = default(BNK007S_DEBITO_CMPS[]);
                    var reservaSaida = default(String);
                    var reservaEntrada = default(String);
                    var retorno = new List<CompensacaoDebitoAluguel>();
                    var mesDebito = mesAnoDebito.ToString("MMyyyy").ToInt32();

                    //Chamada HIS
                    using (var ctx = new ContextoWCF<COMTINKClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { numeroPv, mesAnoDebito, reservaEntrada });

#if !DEBUG
                        codigoRetorno = ctx.Cliente.ConsultarCompensacoesDebitos(
                            out sqlCode,
                            out mensagem,
                            out quantidadeOcorrencias,
                            out ocorrencias,
                            out reservaSaida,
                            numeroPv,
                            mesDebito,
                            reservaEntrada);
#else
                        var r = new Random();
                        codigoRetorno = 0;
                        quantidadeOcorrencias = (Int16)r.Next(0, 10);
                        ocorrencias = new BNK007S_DEBITO_CMPS[quantidadeOcorrencias];
                        for (Int32 iOcc = 0; iOcc < quantidadeOcorrencias; iOcc++)
                        {
                            ocorrencias[iOcc].BNK007S_DAT_CMPS_DEB = DateTime.Today.AddDays(r.Next(100)).ToString("ddMMyyyy").ToInt32();
                            ocorrencias[iOcc].BNK007S_IND_NET = new[] { "C", "D", "O" }[r.Next(3)];
                            ocorrencias[iOcc].BNK007S_MEIO_PGT = String.Concat("Descrição tipo pgto ", iOcc);
                            ocorrencias[iOcc].BNK007S_NRO_RES_DEB = r.Next(999999999);
                            ocorrencias[iOcc].BNK007S_VAL_CMPS_DEB = (Decimal)r.NextDouble() * r.Next(1000);
                        }
#endif

                        log.GravarLog(EventoLog.RetornoHIS,
                            new { codigoRetorno, sqlCode, mensagem, quantidadeOcorrencias, ocorrencias, reservaSaida });
                    }

                    if (codigoRetorno == 0 && ocorrencias != null)
                    {
                        //Separa apenas as ocorrências retornadas com 
                        //dados preenchidos e converte para modelo de negócio
                        retorno = ocorrencias
                            .Take(quantidadeOcorrencias)
                            .Select(ocorrencia =>
                                new CompensacaoDebitoAluguel
                                {
                                    DataCompensacao = ocorrencia.BNK007S_DAT_CMPS_DEB.ToString("D8").ToDateTimeNull("ddMMyyyy"),
                                    DescricaoTipoPagamento = ocorrencia.BNK007S_MEIO_PGT,
                                    IndicadorNet = ocorrencia.BNK007S_IND_NET,
                                    NumeroResumoVenda = ocorrencia.BNK007S_NRO_RES_DEB,
                                    ValorCompensado = ocorrencia.BNK007S_VAL_CMPS_DEB
                                })
                            .ToList();
                    }

                    return retorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta o tipo de oferta ativa para o estabelecimento<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7030 / Programa ZPC703 / TranID ZPJA / Método ConsultarTipoOfertaAtiva
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7030 / Programa ZPC703 / TranID ZPJA / Método ConsultarTipoOfertaAtiva
        /// </remarks>
        /// <returns>Consulta tipo de oferta ativa</returns>
        public Int16 ConsultarTipoOfertaAtiva(
            Int32 numeroPv, 
            out TipoOferta tipoOferta)
        {
            using (Logger log = Logger.IniciarLog("Tipo de Oferta Ativa - BKZP7030 / ZPC703 / ZPJA"))
            {
                try
                {
                    //Valor padrão do retorno
                    tipoOferta = TipoOferta.SemOferta;

                    //Preparação de parâmetros de chamada e retorno do HIS
                    var codigoRetorno = default(Int16);
                    var usuario = "PN";
                    var sqlCode = default(Int16);
                    var pgmRetorno = default(String);
                    var seqRetorno = default(Int16);
                    var dscRetorno = default(String);
                    var indicadorOferta = default(Int16);
                    var mensagemOferta = default(String);
                    var reserva = default(String);

                    //Chamada HIS
                    using (var ctx = new ContextoWCF<COMTIZPClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { usuario, numeroPv, reserva });

#if !DEBUG
                        codigoRetorno = ctx.Cliente.ConsultarTipoOfertaAtiva(
                            out sqlCode,
                            out pgmRetorno,
                            out seqRetorno,
                            out dscRetorno,
                            out indicadorOferta,
                            out mensagemOferta,
                            usuario,
                            numeroPv,
                            ref reserva);
#else
                        var r = new Random();
                        codigoRetorno = 0;
                        indicadorOferta = (Int16)r.Next(Enum.GetValues(typeof(TipoOferta)).Length);
                        //indicadorOferta = (Int16)TipoOferta.OfertaJapao;
#endif

                        log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, sqlCode, pgmRetorno, 
                            seqRetorno, dscRetorno, indicadorOferta, mensagemOferta, reserva });
                    }

                    //Converte para enumerador de retorno do tipo da oferta
                    if (codigoRetorno == 0)
                        tipoOferta = (TipoOferta)indicadorOferta;

                    return codigoRetorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta os dados da oferta no aceite<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7040 / Programa ZPC704 / TranID ZPJB / Método ConsultarDadosOfertaAceite
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7040 / Programa ZPC704 / TranID ZPJB / Método ConsultarDadosOfertaAceite
        /// </remarks>
        /// <returns>Consultar dados da oferta aceite</returns>
        public List<FaixaOfertaNoAceite> ConsultarDadosOfertaAceite(
            Int32 numeroPv, 
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Dados da Oferta no Aceite - BKZP7040 / ZPC704 / ZPJB"))
            {
                try
                {
                    //Preparação de parâmetros de chamada e retorno do HIS
                    var usuario = "PN";
                    var sqlCode = default(Int16);
                    var pgmRetorno = default(String);
                    var seqRetorno = default(Int16);
                    var dscRetorno = default(String);
                    var quantidadeFaixas = default(Int16);
                    var faixas = default(ZP704S_OCC_FXA[]);
                    var reserva = default(String);
                    var retorno = new List<FaixaOfertaNoAceite>();

                    //Chamada HIS
                    using (var ctx = new ContextoWCF<COMTIZPClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { usuario, numeroPv, reserva });

#if !DEBUG
                        codigoRetorno = ctx.Cliente.ConsultarDadosOfertaAceite(
                            out sqlCode,
                            out pgmRetorno,
                            out seqRetorno,
                            out dscRetorno,
                            out quantidadeFaixas,
                            out faixas,
                            usuario,
                            numeroPv,
                            ref reserva);
#else
                        var r = new Random();
                        codigoRetorno = 0;
                        quantidadeFaixas = (Int16)r.Next(1, 11);
                        faixas = new ZP704S_OCC_FXA[quantidadeFaixas];
                        for (Int32 iOcc = 0; iOcc < quantidadeFaixas; iOcc++)
                        {
                            faixas[iOcc].ZP704S_COD_MTA = (Int16)r.Next(999);
                            faixas[iOcc].ZP704S_VAL_BONUS = (Decimal)r.NextDouble() * r.Next(1000);
                            faixas[iOcc].ZP704S_VAL_FIM = (Decimal)r.NextDouble() * r.Next(1000);
                            faixas[iOcc].ZP704S_VAL_INI = (Decimal)r.NextDouble() * r.Next(1000);
                            faixas[iOcc].ZP704S_QTD_TEC = (Int16) r.Next(1, 3);
                            faixas[iOcc].ZP704S_OCC_TEC = new ZP704S_OCC_TEC[faixas[iOcc].ZP704S_QTD_TEC];
                            for (Int32 iTec = 0; iTec < faixas[iOcc].ZP704S_QTD_TEC; iTec++)
                            {
                                faixas[iOcc].ZP704S_OCC_TEC[iTec].ZP704S_COD_EQP = new[] { "POS", "POO" }[r.Next(2)];
                                faixas[iOcc].ZP704S_OCC_TEC[iTec].ZP704S_VAL_ALG = (Decimal)r.NextDouble() * r.Next(1000);
                            }
                        }
#endif

                        log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, sqlCode, 
                            pgmRetorno, seqRetorno, dscRetorno, quantidadeFaixas, faixas, reserva });
                    }

                    if (codigoRetorno == 0 && faixas != null)
                    {
                        //Separa apenas as faixas retornadas com 
                        //dados preenchidos e converte para modelo de negócio
                        retorno = faixas
                            .Take(quantidadeFaixas)
                            .Select(faixa => new FaixaOfertaNoAceite
                            {
                                CodigoMeta = faixa.ZP704S_COD_MTA,
                                ValorBonus = faixa.ZP704S_VAL_BONUS,
                                ValorInicial = faixa.ZP704S_VAL_INI,
                                ValorFinal = faixa.ZP704S_VAL_FIM,
                                Equipamentos = faixa.ZP704S_OCC_TEC
                                    .Take(faixa.ZP704S_QTD_TEC)
                                    .Select(equipamento => new FaixaOfertaNoAceiteEquipamento
                                    {
                                        Codigo = equipamento.ZP704S_COD_EQP,
                                        ValorAluguel = equipamento.ZP704S_VAL_ALG
                                    }).ToList()
                            }).ToList();
                    }

                    return retorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta dados de celular e bônus.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7050 / Programa ZPC705 / TranID ZPJC / Método ConsultarDadosCelularBonus
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7050 / Programa ZPC705 / TranID ZPJC / Método ConsultarDadosCelularBonus
        /// </remarks>
        /// <returns>Consultar dados celular bônus</returns>
        public List<OfertaCelular> ConsultarDadosCelularBonus(
            Int32 numeroPv, 
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Dados Celular Bônus - BKZP7050 / ZPC705 / ZPJC"))
            {
                try
                {
                    //Preparação de parâmetros de chamada e retorno do HIS
                    var sqlCode = default(Int16);
                    var pgmRetorno = default(String);
                    var seqRetorno = default(Int16);
                    var dscRetorno = default(String);
                    var quantidadeCelulares = default(Int16);
                    var celulares = default(BKZP705_OCC_CEL[]);
                    var usuario = "PN";
                    var reserva = default(String);
                    var retorno = new List<OfertaCelular>();

                    //Chamada HIS
                    using (var ctx = new ContextoWCF<COMTIZPClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { usuario, numeroPv, reserva });

#if !DEBUG
                        codigoRetorno = ctx.Cliente.ConsultarDadosCelularBonus(
                            out sqlCode,
                            out pgmRetorno,
                            out seqRetorno,
                            out dscRetorno,
                            out quantidadeCelulares,
                            out celulares,
                            usuario,
                            numeroPv,
                            ref reserva);
#else
                        var r = new Random();
                        codigoRetorno = 0;
                        quantidadeCelulares = (Int16)r.Next(6);
                        celulares = new BKZP705_OCC_CEL[quantidadeCelulares];
                        for (Int32 iOcc = 0; iOcc < quantidadeCelulares; iOcc++)
                        {
                            celulares[iOcc].BKZP705_COD_OPER = r.Next(1, 99);
                            celulares[iOcc].BKZP705_DDD_CEL = (Int16) r.Next(1, 99);
                            celulares[iOcc].BKZP705_NOM_OPER = String.Concat("Operadora ", iOcc);
                            celulares[iOcc].BKZP705_NRO_CEL = r.Next(10000000, 999999999);
                            celulares[iOcc].BKZP705_PRC_BONUS = (Decimal)r.NextDouble() * r.Next(1000);
                            celulares[iOcc].BKZP705_VAL_BONUS = (Decimal)r.NextDouble() * r.Next(1000);
                        }
#endif

                        log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, sqlCode, 
                            pgmRetorno, seqRetorno, dscRetorno, quantidadeCelulares, celulares, reserva });
                    }

                    if (codigoRetorno == 0 && celulares != null)
                    {
                        //Separa apenas as faixas retornadas com 
                        //dados preenchidos e converte para modelo de negócio
                        retorno = celulares
                            .Take(quantidadeCelulares)
                            .Select(celular => new OfertaCelular
                            {
                                CodigoOperadora = celular.BKZP705_COD_OPER,
                                DddCelular = celular.BKZP705_DDD_CEL,
                                NomeOperadora = celular.BKZP705_NOM_OPER,
                                NumeroCelular = celular.BKZP705_NRO_CEL,
                                PercentualBonus = celular.BKZP705_PRC_BONUS,
                                ValorBonus = celular.BKZP705_VAL_BONUS
                            }).ToList();
                    }

                    return retorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta dados de apuração.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7060 / Programa ZPC706 / TranID ZPJD / Método ConsultarDadosApuracao
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7060 / Programa ZPC706 / TranID ZPJD / Método ConsultarDadosApuracao
        /// </remarks>
        /// <returns>Dados de apuração</returns>
        public List<OfertaDadosApuracao> ConsultarDadosApuracao(
            Int32 numeroPv, 
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Dados Apuração - BKZP7060 / ZPC706 / ZPJD"))
            {
                try
                {
                    //Preparação de parâmetros de chamada e retorno do HIS
                    var sqlCode = default(Int16);
                    var pgmRetorno = default(String);
                    var seqRetorno = default(Int16);
                    var dscRetorno = default(String);
                    var quantidadeOcorrencias = default(Int16);
                    var ocorrencias = default(BKZP706_OCC_APUR[]);
                    var usuario = "PN";
                    var reserva = default(String);
                    var retorno = new List<OfertaDadosApuracao>();

                    //Chamada HIS
                    using (var ctx = new ContextoWCF<COMTIZPClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { usuario, numeroPv, reserva });

#if !DEBUG
                        codigoRetorno = ctx.Cliente.ConsultarDadosApuracao(
                            out sqlCode,
                            out pgmRetorno,
                            out seqRetorno,
                            out dscRetorno,
                            out quantidadeOcorrencias,
                            out ocorrencias,
                            usuario,
                            numeroPv,
                            ref reserva);
#else
                        var r = new Random();
                        codigoRetorno = 0;
                        quantidadeOcorrencias = (Int16)r.Next(13);
                        ocorrencias = new BKZP706_OCC_APUR[quantidadeOcorrencias];
                        for (Int32 iOcc = 0; iOcc < quantidadeOcorrencias; iOcc++)
                        {
                            ocorrencias[iOcc].BKZP706_COD_META = (Int16)r.Next(9);
                            ocorrencias[iOcc].BKZP706_FIM_APUR = DateTime.Today.AddDays(r.Next(100)).ToString("dd/MM/yyyy");
                            ocorrencias[iOcc].BKZP706_IND_META = new[] { "S", "N" }[r.Next(2)];
                            ocorrencias[iOcc].BKZP706_IND_PGTO = new[] { "S", "N" }[r.Next(2)];
                            ocorrencias[iOcc].BKZP706_INI_APUR = DateTime.Today.AddDays(r.Next(100)).ToString("dd/MM/yyyy");
                            ocorrencias[iOcc].BKZP706_MES_REF = DateTime.Today.AddMonths(r.Next(100)).ToString("MMyyyy").ToInt32();
                            ocorrencias[iOcc].BKZP706_QTD_TERM = (Int16)r.Next(999);
                            ocorrencias[iOcc].BKZP706_VAL_ALUG = (Decimal)r.NextDouble() * r.Next(1000);
                            ocorrencias[iOcc].BKZP706_VAL_BONUS = (Decimal)r.NextDouble() * r.Next(1000);                            
                            ocorrencias[iOcc].BKZP706_VAL_COMP = iOcc % 2 == 0 ?
                                (Decimal)r.NextDouble() * r.Next(1000) : ocorrencias[iOcc].BKZP706_VAL_ALUG;
                            ocorrencias[iOcc].BKZP706_VAL_MTAI = (Decimal)r.NextDouble() * r.Next(1000);
                            ocorrencias[iOcc].BKZP706_VAL_MTAF = ocorrencias[iOcc].BKZP706_VAL_MTAI + (Decimal)r.NextDouble() * r.Next(100);
                            ocorrencias[iOcc].BKZP706_VAL_REAL = (Decimal)r.NextDouble() * r.Next(1000);
                        }
#endif

                        log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, sqlCode, pgmRetorno, 
                            seqRetorno, dscRetorno, quantidadeOcorrencias, ocorrencias, reserva });
                    }

                    if (codigoRetorno == 0 && ocorrencias != null)
                    {
                        //Separa apenas as faixas retornadas com 
                        //dados preenchidos e converte para modelo de negócio
                        retorno = ocorrencias
                            .Take(quantidadeOcorrencias)
                            .Select(ocorrencia => new OfertaDadosApuracao
                            {
                                CodigoMeta = ocorrencia.BKZP706_COD_META,
                                DataInicioApuracao = ocorrencia.BKZP706_INI_APUR.ToDateTimeNull("dd/MM/yyyy"),
                                DataFimApuracao = ocorrencia.BKZP706_FIM_APUR.ToDateTimeNull("dd/MM/yyyy"),
                                IndicadorMeta = String.Compare("S", ocorrencia.BKZP706_IND_META, true) == 0,
                                IndicadorPagamento = String.Compare("S", ocorrencia.BKZP706_IND_PGTO, true) == 0,
                                MesReferencia = ocorrencia.BKZP706_MES_REF.ToString("D6").ToDate("MMyyyy"),
                                QuantidadeTerminais = ocorrencia.BKZP706_QTD_TERM,
                                ValorBonusCreditado = ocorrencia.BKZP706_VAL_BONUS,
                                ValorCompensado = ocorrencia.BKZP706_VAL_COMP,
                                ValorRealizado = ocorrencia.BKZP706_VAL_REAL,
                                ValorFinal = ocorrencia.BKZP706_VAL_MTAF,
                                ValorInicial = ocorrencia.BKZP706_VAL_MTAI,
                                ValorTotalAluguel = ocorrencia.BKZP706_VAL_ALUG
                            })
                            .ToList();
                    }

                    return retorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta detalhes dos dados de apuração.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7070 / Programa ZPC707 / TranID ZPJE / Método ConsultarDadosApuracaoDetalhes
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7070 / Programa ZPC707 / TranID ZPJE / Método ConsultarDadosApuracaoDetalhes
        /// </remarks>
        /// <returns>Detalhes dos dados de apuração</returns>
        public List<OfertaDadosApuracaoDetalhe> ConsultarDadosApuracaoDetalhes(
            Int32 numeroPv, 
            DateTime mesAnoReferencia, 
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Detalhes Dados de Apuração - BKZP7070 / ZPC707 / ZPJE"))
            {
                try
                {
                    //Preparação de parâmetros de chamada e retorno do HIS
                    var sqlCode = default(Int16);
                    var pgmRetorno = default(String);
                    var seqRetorno = default(Int16);
                    var dscRetorno = default(String);
                    var quantidadeOcorrencias = default(Int16);
                    var ocorrencias = default(BKZP707_OCC_APUR[]);
                    var usuario = "PN";
                    var reserva = default(String);
                    var retorno = new List<OfertaDadosApuracaoDetalhe>();
                    var mesReferencia = mesAnoReferencia.ToString("yyyyMM").ToInt32();

                    //Chamada HIS
                    using (var ctx = new ContextoWCF<COMTIZPClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { usuario, numeroPv, mesReferencia = mesAnoReferencia, reserva });

#if !DEBUG
                        codigoRetorno = ctx.Cliente.ConsultarDadosApuracaoDetalhes(
                            out sqlCode,
                            out pgmRetorno,
                            out seqRetorno,
                            out dscRetorno,
                            out quantidadeOcorrencias,
                            out ocorrencias,
                            usuario,
                            numeroPv,
                            mesReferencia,
                            ref reserva);
#else
                        var r = new Random();
                        codigoRetorno = 0;
                        quantidadeOcorrencias = (Int16)r.Next(1, 6);
                        ocorrencias = new BKZP707_OCC_APUR[quantidadeOcorrencias];
                        for(Int32 iOcc=0; iOcc < quantidadeOcorrencias; iOcc++) 
                        {
                            ocorrencias[iOcc].BKZP707_DAT_CRED = DateTime.Today.AddDays(r.Next(100)).ToString("dd/MM/yyyy");
                            ocorrencias[iOcc].BKZP707_DDD_CEL = (Int16)r.Next(99);
                            ocorrencias[iOcc].BKZP707_FIM_APUR = DateTime.Today.AddDays(r.Next(100)).ToString("dd/MM/yyyy");
                            ocorrencias[iOcc].BKZP707_NRO_CEL = r.Next(10000000, 999999999);
                            ocorrencias[iOcc].BKZP707_OBS = new[] { "Crédito efetivado na data", "Não efetivado - Celular inexistente" }[r.Next(2)];
                            ocorrencias[iOcc].BKZP707_PRV_CRED = DateTime.Today.AddDays(r.Next(100)).ToString("dd/MM/yyyy");
                            ocorrencias[iOcc].BKZP707_VAL_BONUS = (Decimal)r.NextDouble() * r.Next(1000);
                        }
#endif

                        log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, sqlCode, 
                            pgmRetorno, seqRetorno, quantidadeOcorrencias, ocorrencias, reserva });
                    }

                    if (codigoRetorno == 0 && ocorrencias != null)
                    {
                        //Regex para manter apenas os números em uma String
                        Regex regNumero = new Regex("[^0-9]");

                        //Separa apenas as faixas retornadas com 
                        //dados preenchidos e converte para modelo de negócio
                        retorno = ocorrencias
                            .Take(quantidadeOcorrencias)
                            .Select(ocorrencia => new OfertaDadosApuracaoDetalhe
                            {
                                DataCredito = regNumero.Replace(ocorrencia.BKZP707_DAT_CRED, String.Empty).ToDateTimeNull("ddMMyyyy"),
                                DddCelular = ocorrencia.BKZP707_DDD_CEL,
                                NumeroCelular = ocorrencia.BKZP707_NRO_CEL,
                                DataFimApuracao = regNumero.Replace(ocorrencia.BKZP707_FIM_APUR, String.Empty).ToDateTimeNull("ddMMyyyy"),
                                DataPrevisaoCredito = regNumero.Replace(ocorrencia.BKZP707_PRV_CRED, String.Empty).ToDateTimeNull("ddMMyyyy"),
                                ValorBonusCreditado = ocorrencia.BKZP707_VAL_BONUS,
                                Observacao = ocorrencia.BKZP707_OBS
                            })
                            .ToList();
                    }

                    return retorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }                
            }
        }

        /// <summary>
        /// Retorna a apuração de preço único dos 12 ultimos meses para o PV(Ponto de Venda) informado.<br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKNK0080 / Programa NKC008  / TranID NK08 / Método ConsultarPlanosPrecoUnicoContratados
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKNK0080 / Programa NKC008 / TranID NK08 / Método ConsultarPlanosPrecoUnicoContratados
        /// </remarks>  
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar os planos.</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        /// <returns>Planos Preço Único Contratados</returns>        
        public List<PlanoPrecoUnico> ConsultarPlanosPrecoUnicoContratados(Int32 numeroPv, out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Planos Preço Único Contratados - BKNK0080 / NKC008 / NK08 "))
            {
                try
                {
                    //Preparação de parâmetros de chamada e retorno HIS
                    var sqlCode = default(Int16);
                    var mensagem = default(String);
                    var quantidadeOcorrencias = default(Int16);
                    var ocorrencias = default(BNK008S_APRC[]);
                    var reservaSaida = default(String);
                    var reservaEntrada = default(String);
                    var retorno = new List<PlanoPrecoUnico>();

                    //Chamada HIS
                    using (var ctx = new ContextoWCF<COMTINKClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { numeroPv, reservaEntrada });

#if !DEBUG
                        codigoRetorno = ctx.Cliente.ConsultarPlanosPrecoUnicoContratados(
                            out sqlCode,
                            out mensagem,
                            out quantidadeOcorrencias,
                            out ocorrencias,
                            numeroPv,
                            reservaEntrada,
                            ref reservaSaida);
#else
                        var r = new Random();
                        codigoRetorno = 0;
                        quantidadeOcorrencias = (Int16)r.Next(0, 10);
                        ocorrencias = new BNK008S_APRC[quantidadeOcorrencias];
                        for (Int32 iOcc = 0; iOcc < quantidadeOcorrencias; iOcc++)
                        {
                            ocorrencias[iOcc].BNK008S_NOM_PLANO = "Plano Preço Único Teste";
                            ocorrencias[iOcc].BNK008S_DSC_OFRT = "Oferta Preço Único Teste";
                            ocorrencias[iOcc].BNK008S_VAL_MSNL = (Decimal)189.00;
                            ocorrencias[iOcc].BNK008S_VAL_FATM_CTR = (Decimal)r.NextDouble() * r.Next(1000);
                            ocorrencias[iOcc].BNK008S_QTD_TCNO = 3; 

                            ocorrencias[iOcc].BNK008S_OCC_TEC = new BNK008S_OCC_TEC [] {
                                                                    new BNK008S_OCC_TEC { BNK008S_COD_EQPM = "POO", 
                                                                        BNK008S_QTD_EQPM = 2, BNK008S_VAL_EQPM = (Decimal)r.NextDouble() * r.Next(1000) },
                                                                    new BNK008S_OCC_TEC { BNK008S_COD_EQPM = "POS", 
                                                                        BNK008S_QTD_EQPM = 1, BNK008S_VAL_EQPM = (Decimal)r.NextDouble() * r.Next(1000) },
                                                                    new BNK008S_OCC_TEC { BNK008S_COD_EQPM = "FLEX", 
                                                                        BNK008S_QTD_EQPM = 1, BNK008S_VAL_EQPM = (Decimal)r.NextDouble() * r.Next(1000) }};

                            ocorrencias[iOcc].BNK008S_VAL_FATM_APRD = (Decimal)r.NextDouble() * r.Next(1000);
                            ocorrencias[iOcc].BNK008S_VAL_FATM_CTR_PRO = (Decimal)r.NextDouble() * r.Next(500);
                            ocorrencias[iOcc].BNK008S_VAL_FATM_EXCD = (Decimal)r.NextDouble() * r.Next(10);
                            ocorrencias[iOcc].BNK008S_VAL_MNSL_CBRD = (Decimal)189.00;
                            ocorrencias[iOcc].BNK008S_VAL_EXCD_CBRD = (Decimal)r.NextDouble() * r.Next(10);
                            ocorrencias[iOcc].BNK008S_ANO_MES_REF = DateTime.Today.AddMonths(r.Next(12)).ToString("yyyyMM").ToInt32();
                            ocorrencias[iOcc].BNK008S_DAT_INI_APRC = ocorrencias[iOcc].BNK008S_ANO_MES_REF.ToString("D6").ToDate("yyyyMM").AddDays(new Int16[] {0, 14}[r.Next(2)]).ToString("dd/MM/yyyy");
                            ocorrencias[iOcc].BNK008S_DAT_FIM_APRC = ocorrencias[iOcc].BNK008S_DAT_INI_APRC.ToDateTimeNull("dd/MM/yyyy").Value.AddDays(new Int16[] {14, 29}[r.Next(2)]).ToString("dd/MM/yyyy");
                            ocorrencias[iOcc].BNK008S_IND_FLX = new String[] {"S", "N"}[r.Next(2)];
                        }
#endif

                        log.GravarLog(EventoLog.RetornoHIS,
                            new { codigoRetorno, sqlCode, mensagem, quantidadeOcorrencias, ocorrencias, reservaSaida });
                    }

                    if (codigoRetorno == 0 && ocorrencias != null)
                    {
                        //Separa apenas as ocorrências retornadas com 
                        //dados preenchidos e converte para modelo de negócio
                        retorno = ocorrencias
                            .Take(quantidadeOcorrencias)
                            .Select(ocorrencia =>
                                new PlanoPrecoUnico
                                {
                                    NomePlano = ocorrencia.BNK008S_NOM_PLANO,
                                    DescricaoOferta = ocorrencia.BNK008S_DSC_OFRT,
                                    ValorMensalidade = ocorrencia.BNK008S_VAL_MSNL,
                                    ValorLimiteFaturamentoContratado = ocorrencia.BNK008S_VAL_FATM_CTR,
                                    QtdTecnologiasCadastradasPacote = ocorrencia.BNK008S_QTD_TCNO,
                                    Equipamentos = ocorrencia.BNK008S_OCC_TEC
                                                   .Take(ocorrencia.BNK008S_QTD_TCNO)
                                                   .Select(equipamento =>
                                                       new Equipamento
                                                       {
                                                           Tipo = equipamento.BNK008S_COD_EQPM,
                                                           QtdTerminais = equipamento.BNK008S_QTD_EQPM,
                                                           Valor = equipamento.BNK008S_VAL_EQPM
                                                       }).ToList(),
                                    ValorFaturamentoApurado = ocorrencia.BNK008S_VAL_FATM_APRD,
                                    ValorExcedenteContratado = ocorrencia.BNK008S_VAL_FATM_EXCD,
                                    ValorMensalidadeCobrada = ocorrencia.BNK008S_VAL_MNSL_CBRD,
                                    ValorCabradoPeloExcedente = ocorrencia.BNK008S_VAL_EXCD_CBRD,
                                    DataInicioApuracao = ocorrencia.BNK008S_DAT_INI_APRC.ToDateTimeNull("dd/MM/yyyy"),
                                    DataFimApuracao = ocorrencia.BNK008S_DAT_FIM_APRC.ToDateTimeNull("dd/MM/yyyy"),
                                    AnoMesReferencia = ocorrencia.BNK008S_ANO_MES_REF.ToString("D6").ToDate("yyyyMM"),
                                    IndicadorFlex = ocorrencia.BNK008S_IND_FLX,
                                    ValorLimiteFaturamentoContratadoProRata = ocorrencia.BNK008S_VAL_FATM_CTR_PRO
                                })
                            .ToList();                        
                    }

                    return retorno;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
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