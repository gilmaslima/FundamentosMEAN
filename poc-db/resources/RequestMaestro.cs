using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Request.Agentes.COMTIXDRequest;

namespace Redecard.PN.Request.Agentes
{
    public class RequestMaestro : RequestBase
    {
        public List<Modelo.Comprovante> BXD201CB(
            Int32 codEstabelecimento,
            Int32 dataInicio,
            Int32 dataFim,
            String sistemaOrigem,
            String codigoTransacao,
            String indicadorPesquisa,
            ref Decimal numeroProcessoI,
            ref String tpProcessoI,
            ref String clProcessoI,
            ref String dtEmissaoI,
            ref Decimal numeroProcessoF,
            ref String tpProcessoF,
            ref String clProcessoF,
            ref String dtEmissaoF,
            ref String indCont,
            out Int16 codigoRetorno,
            out String msgRetorno,
            out Int16 qtdOcorrencias)
        {
            using (Logger Log = Logger.IniciarLog("Comprovantes Pendentes - Débito [BXD201CB/XD201/XDS1]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codEstabelecimento,
                    dataInicio, dataFim, sistemaOrigem, codigoTransacao, indicadorPesquisa,
                    numeroProcessoI, tpProcessoI, clProcessoI, dtEmissaoI, numeroProcessoF,
                    tpProcessoF, clProcessoF, dtEmissaoF, indCont });

                try
                {
                    //Variáveis auxiliares
                    COMTIXDRequest.XD201CB_OCC[] registros;

#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    Random random = new Random(DateTime.Now.Millisecond);

                    qtdOcorrencias = (Int16)("N".Equals(indicadorPesquisa, StringComparison.InvariantCultureIgnoreCase) ? 190 : 55);
                    registros = new COMTIXDRequest.XD201CB_OCC[190];
                    indCont = qtdOcorrencias == 190 ? "S" : "N";
                    codigoRetorno = 0;
                    msgRetorno = "OK";
                    String[] simNao = new String[] { "S", "N" };

                    List<COMTIXDRequest.XD201CB_OCC> lista = new List<COMTIXDRequest.XD201CB_OCC>();
                    #region Preenche dados fake
                    for (Int32 i = 0; i < 245; i++)
                    {
                        lista.Add(new COMTIXDRequest.XD201CB_OCC
                        {
                            XD201CB_CENVSL = (Int16)random.Next(1, 10),
                            XD201CB_DENVSL = "Descrição canal envio ",
                            XD201CB_CENTRAL = 27055253,
                            XD201CB_DSCMOT = "Gerado às " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                            XD201CB_DTENVD = random.Next(0, 2) % 2 == 0 ? random.Next(1, 29) + "." + random.Next(1, 12) + ".2012" : null,
                            XD201CB_SOLCAT = simNao[random.Next(2)],
                            XD201CB_DATVDA = DateTime.Now.ToString("dd.MM.yyyy"),
                            XD201CB_TPPROC = "Tipo processo",
                            XD201CB_CLPROC = "Ciclo processo",
                            XD201CB_CODMOT = 0,
                            XD201CB_DQLDOC = "Qualidade documento",
                            XD201CB_TRNACQ = random.Next(0, 100000000),
                            XD201CB_DATLIM = random.Next(0, 2) % 2 == 0 ? random.Next(1, 29) + "." + random.Next(1, 12) + ".2012" : null,
                            XD201CB_RESVDA = random.Next(100000000, 1000000000),
                            XD201CB_NROPV = random.Next(100000000, 1000000000),
                            XD201CB_NRPROC = i,
                            XD201CB_CQLDOC = (short)random.Next(1, 13),
                            XD201CB_VALVDA = random.Next(1000000000)
                        });
                    }

                    for (Int32 i = 0; i < registros.Length; i++)
                    {
                        registros[i] = new COMTIXDRequest.XD201CB_OCC
                        {
                            XD201CB_CENTRAL = 0,
                            XD201CB_CENVSL = 0,
                            XD201CB_CLPROC = String.Empty,
                            XD201CB_CODMOT = 0,
                            XD201CB_CQLDOC = 0,
                            XD201CB_DATLIM = String.Empty,
                            XD201CB_DATVDA = String.Empty,
                            XD201CB_DENVSL = String.Empty,
                            XD201CB_DQLDOC = String.Empty,
                            XD201CB_DSCMOT = String.Empty,
                            XD201CB_DTENVD = String.Empty,
                            XD201CB_NROPV = 0,
                            XD201CB_NRPROC = 0,
                            XD201CB_RESVDA = 0,
                            XD201CB_SOLCAT = String.Empty,
                            XD201CB_TPPROC = String.Empty,
                            XD201CB_TRNACQ = 0,
                            XD201CB_VALVDA = 0
                        };
                    }

                    Int32 iRegistro = 0;
                    foreach (var item in lista)
                    {
                        if ((numeroProcessoI == 0 || item.XD201CB_NRPROC > numeroProcessoI) && iRegistro < 190)
                            registros[iRegistro++] = item;
                    }

                    #endregion

                    numeroProcessoI = registros.First().XD201CB_NRPROC;
                    tpProcessoI = registros.First().XD201CB_TPPROC;
                    clProcessoI = registros.First().XD201CB_CLPROC;
                    dtEmissaoI = registros.First().XD201CB_DATVDA;
                    numeroProcessoF = registros.Last(r => r.XD201CB_NROPV != 0).XD201CB_NRPROC;
                    tpProcessoF = registros.Last(r => r.XD201CB_NROPV != 0).XD201CB_TPPROC;
                    clProcessoF = registros.Last(r => r.XD201CB_NROPV != 0).XD201CB_CLPROC;
                    dtEmissaoF = registros.Last(r => r.XD201CB_NROPV != 0).XD201CB_DATVDA;
#else
                //Instancia o serviço de acesso ao mainframe
                COMTIXDRequest.COMTIXDClient client = new COMTIXDRequest.COMTIXDClient();
                client.BXD201CB(
                    ref codEstabelecimento,
                    ref dataInicio,
                    ref dataFim,
                    ref sistemaOrigem,
                    ref codigoTransacao,
                    ref indicadorPesquisa,
                    ref numeroProcessoI,
                    ref tpProcessoI,
                    ref clProcessoI,
                    ref dtEmissaoI,
                    ref numeroProcessoF,
                    ref tpProcessoF,
                    ref clProcessoF,
                    ref dtEmissaoF,
                    ref indCont,
                    out codigoRetorno,
                    out msgRetorno,
                    out qtdOcorrencias,
                    out registros);
#endif
                    //Retorna os comprovantes pendentes, convertendo para Modelo
                    List<Modelo.Comprovante> comprovantes = this.PreencherModelo(registros).ToList();

                    Log.GravarLog(EventoLog.FimAgente, new { numeroProcessoI,
                        tpProcessoI, clProcessoI, dtEmissaoI, numeroProcessoF, tpProcessoF,
                        clProcessoF, dtEmissaoF, indCont, codigoRetorno, msgRetorno,
                        qtdOcorrencias, registros, comprovantes });
                    
                    return comprovantes;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<Modelo.RecebimentoCV> BXD202CB(
          Decimal numeroProcesso,
          Int32 codEstabelecimento,
          String origem,
          String transacao,
          out Int16 codRetorno,
          out String msgRetorno,
          out Int16 qtdOcorrencias)
        {
            using (Logger Log = Logger.IniciarLog("BXD202CB - Débito [BXD202CB/XD202/XDS2]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroProcesso, codEstabelecimento, origem, transacao });

                try
                {
                    //Variáveis auxiliares
                    COMTIXDRequest.XD202CB_OCC[] cvs;

#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    Random random = new Random(DateTime.Now.Millisecond);
                    codRetorno = 0;
                    msgRetorno = "OK";
                    qtdOcorrencias = (Int16)random.Next(1, 150);
                    cvs = new COMTIXDRequest.XD202CB_OCC[qtdOcorrencias];
                    for (int i = 0; i < qtdOcorrencias; i++)
                    {
                        cvs[i] = new COMTIXDRequest.XD202CB_OCC
                        {
                            XD202CB_CQLDOC = (Int16)random.Next(1, 5),
                            XD202CB_DQLDOC = "Descrição Qualidade " + random.Next(1000),
                            XD202CB_DTRESP = random.Next(1, 29) + "." + random.Next(1, 12) + ".2012"
                        };
                    }
#else
                //Instanciação do serviço de acesso ao mainframe
                COMTIXDRequest.COMTIXDClient client = new COMTIXDRequest.COMTIXDClient();
                client.BXD202CB(
                    ref numeroProcesso,
                    ref codEstabelecimento,
                    ref origem,
                    ref transacao,
                    out codRetorno,
                    out msgRetorno,
                    out qtdOcorrencias,
                    out cvs);
#endif
                    List<Modelo.RecebimentoCV> recebimentoCV = this.PreencherModelo(cvs).ToList();

                    Log.GravarLog(EventoLog.FimAgente, new { codRetorno, msgRetorno, qtdOcorrencias, cvs, recebimentoCV });

                    return recebimentoCV;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<Modelo.Comprovante> BXD203CB(
            Decimal numeroProcesso,
            Int32 numeroPV,
            Int32 dataInicio,
            Int32 dataFim,
            String origem,
            String transacao,
            String indicePesquisa,
            ref Decimal numeroProcessoI,
            ref String tipoProcessoI,
            ref String cicloProcessoI,
            ref Int32 numeroPVI,
            ref String dataEmissaoI,
            ref Decimal numeroProcessoF,
            ref String tipoProcessoF,
            ref String cicloProcessoF,
            ref Int32 numeroPVF,
            ref String dataEmissaoF,
            ref String indCont,
            out Int16 codigoRetorno,
            out String msgRetorno,
            out Int16 qtdOcorrencias)
        {
            using (Logger Log = Logger.IniciarLog("Histórico de Comprovantes - Débito [BXD203CB/XD203/XDS3]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroProcesso,
                    numeroPV, dataInicio, dataFim, origem, transacao, indicePesquisa,
                    numeroProcessoI, tipoProcessoI, cicloProcessoI, numeroPVI, dataEmissaoI,
                    numeroProcessoF, tipoProcessoF, cicloProcessoF, numeroPVF, dataEmissaoF, indCont });

                try
                {
                    //Variáveis auxiliares
                    COMTIXDRequest.XD203CB_OCC[] requests;

#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    Random random = new Random(DateTime.Now.Millisecond);
                    qtdOcorrencias = new Int16[] { (Int16)random.Next(150), 150 }[random.Next(2)];
                    requests = new COMTIXDRequest.XD203CB_OCC[qtdOcorrencias];
                    indCont = qtdOcorrencias == 150 ? "N" : "S";
                    codigoRetorno = 0;
                    msgRetorno = "OK";

                    String[] simNao = new String[] { "S", "N" };

                    for (Int32 i = 0; i < qtdOcorrencias; i++)
                    {
                        requests[i] = new COMTIXDRequest.XD203CB_OCC
                        {
                            XD203CB_CENVDC = (Int16)random.Next(1, 5),
                            XD203CB_DENVDC = "Descrição canal envio ",
                            XD203CB_CENTRAL = 27055253,
                            XD203CB_DSCMOT = "Gerado às " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                            XD203CB_DTENVD = random.Next(0, 2) % 2 == 0 ? random.Next(1, 29) + "." + random.Next(1, 12) + ".2012" : null,
                            XD203CB_SOLCAT = simNao[random.Next(2)],
                            XD203CB_DATVDA = DateTime.Now.ToString("dd.MM.yyyy"),
                            XD203CB_TPPROC = "Tipo processo",
                            XD203CB_CLPROC = "Ciclo processo",
                            XD203CB_CODMOT = 0,
                            XD203CB_DQLDOC = "Qualidade documento",
                            XD203CB_TRNACQ = 0,
                            XD203CB_RESVDA = random.Next(100000000, 1000000000),
                            XD203CB_NROPV = random.Next(100000000, 1000000000),
                            XD203CB_NRPROC = random.Next(100000000, 1000000000) * 10000m,
                            XD203CB_CQLDOC = (short)random.Next(1, 13),
                            XD203CB_VALVDA = random.Next(1000000000),
                            XD203CB_INDDEB = simNao[random.Next(2)]
                        };
                    }

                    numeroProcessoI = random.Next(1000);
                    tipoProcessoI = "tipo processo I";
                    cicloProcessoI = "ciclo processo I";
                    numeroPVI = numeroPV;
                    dataEmissaoI = string.Format("2012{0}{1}", random.Next(1, 13), random.Next(1, 29));
                    numeroProcessoF = random.Next(1000);
                    tipoProcessoF = "tipo processo F";
                    cicloProcessoF = "ciclo processo F";
                    numeroPVF = numeroPV;
                    dataEmissaoF = string.Format("2012{0}{1}", random.Next(1, 13), random.Next(1, 29));
#else
                //Instancia o serviço de acesso ao mainframe
                COMTIXDRequest.COMTIXDClient client = new COMTIXDRequest.COMTIXDClient();

                client.BXD203CB(
                    ref numeroProcesso,
                    ref numeroPV,
                    ref dataInicio,
                    ref dataFim,
                    ref origem,
                    ref transacao,
                    ref indicePesquisa,
                    ref numeroProcessoI,
                    ref tipoProcessoI,
                    ref cicloProcessoI,
                    ref numeroPVI,
                    ref dataEmissaoI,
                    ref numeroProcessoF,
                    ref tipoProcessoF,
                    ref cicloProcessoF,
                    ref numeroPVF,
                    ref dataEmissaoF,
                    ref indCont,
                    out codigoRetorno,
                    out msgRetorno,
                    out qtdOcorrencias,
                    out requests);
#endif
                    //Retorna o histórico das solicitações
                    List<Modelo.Comprovante> historico = this.PreencherModelo(requests).ToList();

                    Log.GravarLog(EventoLog.FimAgente, new { numeroProcessoI,
                        tipoProcessoI, cicloProcessoI, numeroPVI, dataEmissaoI, numeroProcessoF,
                        tipoProcessoF, cicloProcessoF, numeroPVF, dataEmissaoF, indCont,
                        codigoRetorno, msgRetorno, qtdOcorrencias, requests, historico });

                    return historico;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<Modelo.AvisoDebito> BXD204CB(
            Decimal codProcesso,
            Int32 codEstabelecimento,
            DateTime dataIni,
            DateTime dataFim,
            String origem,
            String transacao,
            ref String indPesq,
            ref Decimal numeroProcessoI,
            ref String tipoProcessoI,
            ref String cicloProcessoI,
            ref Int32 numeroPVI,
            ref String dtEmissaoI,
            ref Decimal numeroProcessoF,
            ref String tipoProcessoF,
            ref String cicloProcessoF,
            ref Int32 numeroPVF,
            ref String dtEmissaoF,
            ref String indCont,
            out Int32 codigoRetorno,
            out Int16 qtdOcorrencias)
        {
            using (Logger Log = Logger.IniciarLog("Avisos de Débito - Débito [BXD204CB/XD204/XDS4]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codProcesso,
                    codEstabelecimento, dataIni, dataFim, origem, transacao,
                    indPesq, numeroProcessoI, tipoProcessoI, cicloProcessoI, numeroPVI,
                    dtEmissaoI, numeroProcessoF, tipoProcessoF, cicloProcessoF, numeroPVF,
                    dtEmissaoF, indCont });

                try
                {
                    //Variáveis auxiliares                
                    COMTIXDRequest.XD204CB_OCC[] itens;
                    Int32 dtIni = dataIni.ToString("yyyyMMdd").ToInt32();
                    Int32 dtFim = dataFim.ToString("yyyyMMdd").ToInt32();
                    Int16 codRetorno;

#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    Random random = new Random(DateTime.Now.Millisecond);
                    Int16[] quantidades = new Int16[] { (Int16)random.Next(100), 100 };
                    String[] simNao = new String[] { "S", "N" };
                    codRetorno = 0;
                    qtdOcorrencias = quantidades[random.Next(2)];
                    indCont = qtdOcorrencias == quantidades.First() ? "N" : "S";

                    itens = new COMTIXDRequest.XD204CB_OCC[qtdOcorrencias];
                    for (int i = 0; i < qtdOcorrencias; i++)
                    {
                        itens[i] = new COMTIXDRequest.XD204CB_OCC
                        {
                            XD204CB_CENTRAL = 27055253,
                            XD204CB_CLPROC = "Ciclo processo",
                            XD204CB_TPPROC = "Tipo processo",
                            XD204CB_VLCANC = random.Next(1000000000),
                            XD204CB_VALVDA = random.Next(1000000000),
                            XD204CB_NRPROC = random.Next(100000000, 1000000000) * 10000m,
                            XD204CB_NROPV = random.Next(100000000, 1000000000),
                            XD204CB_FILLER = "Filler",
                            XD204CB_DATVDA = random.Next(1, 29) + "." + random.Next(1, 12) + ".2012",
                            XD204CB_DTCANC = DateTime.Now.ToString("dd.MM.yyyy"),
                            XD204CB_TRNACQ = random.Next(0, 1000000000),
                            XD204CB_RESVDA = random.Next(0, 1000000000),
                            XD204CB_NROCAR = "NROCAR1273613",
                            XD204CB_CODDEB = Convert.ToInt16(random.Next(0, 5)),
                            XD204CB_INDREQ = simNao[random.Next(1)]
                        };
                    }
#else
                //Instanciação do serviço de acesso ao mainframe                
                COMTIXDRequest.COMTIXDClient client = new COMTIXDRequest.COMTIXDClient();
                
                String dscRetorno = "OK";
                
                client.BXD204CB(
                    ref codProcesso,
                    ref codEstabelecimento,
                    ref dtIni,
                    ref dtFim,
                    ref origem,
                    ref transacao,
                    ref indPesq,
                    ref numeroProcessoI,
                    ref tipoProcessoI,
                    ref cicloProcessoI,
                    ref numeroPVI,
                    ref dtEmissaoI,
                    ref numeroProcessoF,
                    ref tipoProcessoF,
                    ref cicloProcessoF,
                    ref numeroPVF,
                    ref dtEmissaoF,
                    ref indCont,
                    out codRetorno,
                    out dscRetorno,
                    out qtdOcorrencias,
                    out itens);
#endif

                    codigoRetorno = codRetorno;

                    List<Modelo.AvisoDebito> avisos = this.PreencherModelo(itens).ToList();

                    Log.GravarLog(EventoLog.FimAgente, new { indPesq,
                        numeroProcessoI, tipoProcessoI, cicloProcessoI, numeroPVI, dtEmissaoI,
                        numeroProcessoF, tipoProcessoF, cicloProcessoF, numeroPVF, dtEmissaoF,
                        indCont, codigoRetorno, qtdOcorrencias, itens, avisos });

                    return avisos;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public String BXD205CB(
           Int16 codigoMotivoDebito,
           String origem,
           String transacao,
           out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Motivo de Débito [BXD205CB/XD205/XDS5]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codigoMotivoDebito, origem, transacao });

                try
                {
                    //Variáveis auxiliares                
                    Int16 codRetorno;
                    String descricaoMotivoDebito;

#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    Random random = new Random(DateTime.Now.Millisecond);
                    codRetorno = 0;
                    descricaoMotivoDebito = string.Format(
                        "Descrição motivo débito código {0} {1} \0 \0 \0\0\0\0\0\0\0\0 asda", codigoMotivoDebito, random.Next(100000));
#else
                //Instanciação do serviço de acesso ao mainframe
                COMTIXDRequest.COMTIXDClient client = new COMTIXDRequest.COMTIXDClient();
                
                String dscRetorno = "OK";
                
                client.BXD205CB(
                    ref codigoMotivoDebito,
                    ref origem,
                    ref transacao,
                    out codRetorno,
                    out dscRetorno,
                    out descricaoMotivoDebito);
#endif
                    codigoRetorno = codRetorno;
                    descricaoMotivoDebito = descricaoMotivoDebito.Replace("\0", "");

                    Log.GravarLog(EventoLog.FimAgente, new { codigoRetorno, descricaoMotivoDebito });

                    return descricaoMotivoDebito;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #region [ Preencher Modelos ]

        private List<Modelo.Comprovante> PreencherModelo(COMTIXDRequest.XD203CB_OCC[] requests)
        {
            List<Modelo.Comprovante> retorno = new List<Modelo.Comprovante>();

            if (retorno != null)
            {
                Modelo.Comprovante modelo;
                foreach (COMTIXDRequest.XD203CB_OCC request in requests)
                {
                    modelo = new Modelo.Comprovante
                    {
                        CanalEnvio = request.XD203CB_CENVDC,
                        Centralizadora = request.XD203CB_CENTRAL,
                        DataEnvio = this.ParseDateNull(request.XD203CB_DTENVD, "dd.MM.yyyy"),
                        DataLimiteEnvioDocumentos = null,
                        DataVenda = this.ParseDate(request.XD203CB_DATVDA, "dd.MM.yyyy"),
                        IndicadorDebito = "S".Equals(request.XD203CB_INDDEB, StringComparison.InvariantCultureIgnoreCase),
                        Motivo = request.XD203CB_DSCMOT,
                        NumeroCartao = Convert.ToString(request.XD203CB_TRNACQ),
                        PontoVenda = request.XD203CB_NROPV,
                        Processo = request.XD203CB_NRPROC,
                        QualidadeRecebimentoDocumentos = request.XD203CB_DQLDOC,
                        ResumoVenda = request.XD203CB_RESVDA,
                        SolicitacaoAtendida = "S".Equals(request.XD203CB_SOLCAT, StringComparison.InvariantCultureIgnoreCase),
                        ValorVenda = request.XD203CB_VALVDA,
                        FlagNSUCartao = null,
                        TipoCartao = null
                    };
                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        private List<Modelo.RecebimentoCV> PreencherModelo(COMTIXDRequest.XD202CB_OCC[] itens)
        {
            List<Modelo.RecebimentoCV> retorno = new List<Modelo.RecebimentoCV>();

            if (itens != null)
            {
                Modelo.RecebimentoCV modelo;
                foreach (COMTIXDRequest.XD202CB_OCC item in itens)
                {
                    modelo = new Modelo.RecebimentoCV
                    {
                        CodigoRecebimento = item.XD202CB_CQLDOC,
                        DescricaoRecebimento = item.XD202CB_DQLDOC,
                        DataRecebimento = this.ParseDate(item.XD202CB_DTRESP, "dd.MM.yyyy")
                    };
                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        private List<Modelo.Comprovante> PreencherModelo(COMTIXDRequest.XD201CB_OCC[] requests)
        {
            List<Modelo.Comprovante> retorno = new List<Modelo.Comprovante>();

            if (requests != null)
            {
                Modelo.Comprovante modelo;
                foreach (COMTIXDRequest.XD201CB_OCC request in requests)
                {
                    modelo = new Modelo.Comprovante
                    {
                        CanalEnvio = request.XD201CB_CENVSL,
                        DescricaoCanalEnvio = request.XD201CB_DENVSL,
                        Centralizadora = request.XD201CB_CENTRAL,
                        DataEnvio = this.ParseDateNull(request.XD201CB_DTENVD, "dd.MM.yyyy"),
                        DataLimiteEnvioDocumentos = this.ParseDateNull(request.XD201CB_DATLIM, "dd.MM.yyyy"),
                        DataVenda = this.ParseDate(request.XD201CB_DATVDA, "dd.MM.yyyy"),
                        Motivo = request.XD201CB_DSCMOT,
                        NumeroCartao = Convert.ToString(request.XD201CB_TRNACQ),
                        PontoVenda = request.XD201CB_NROPV,
                        Processo = request.XD201CB_NRPROC,
                        QualidadeRecebimentoDocumentos = request.XD201CB_DQLDOC,
                        ResumoVenda = request.XD201CB_RESVDA,
                        SolicitacaoAtendida = "S".Equals(request.XD201CB_SOLCAT, StringComparison.InvariantCultureIgnoreCase),
                        ValorVenda = request.XD201CB_VALVDA,
                        NumeroReferencia = request.XD201CB_CQLDOC.ToString(),
                        FlagNSUCartao = null,
                        IndicadorDebito = "S".Equals(request.XD201CB_SOLCAT, StringComparison.InvariantCultureIgnoreCase),
                        TipoCartao = String.Empty
                    };
                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        private List<Modelo.AvisoDebito> PreencherModelo(COMTIXDRequest.XD204CB_OCC[] itens)
        {
            List<Modelo.AvisoDebito> retorno = new List<Modelo.AvisoDebito>();

            if (itens != null)
            {
                Modelo.AvisoDebito modelo;
                foreach (COMTIXDRequest.XD204CB_OCC item in itens)
                {
                    modelo = new Modelo.AvisoDebito();
                    modelo.Centralizadora = item.XD204CB_CENTRAL;                    
                    modelo.CodigoMotivoDebito = item.XD204CB_CODDEB;
                    modelo.DataCancelamento = this.ParseDate(item.XD204CB_DTCANC, "dd.MM.yyyy");
                    modelo.DataVenda = this.ParseDate(item.XD204CB_DATVDA, "dd.MM.yyyy");
                    modelo.IndicadorRequest = "S".Equals(item.XD204CB_INDREQ, StringComparison.InvariantCultureIgnoreCase);
                    modelo.NumeroCartao = item.XD204CB_NROCAR;
                    modelo.PontoVenda = item.XD204CB_NROPV;
                    modelo.Processo = item.XD204CB_NRPROC;
                    modelo.ResumoVenda = item.XD204CB_RESVDA;
                    modelo.TipoCartao =item.XD204CB_TRNACQ.ToString();
                    
                    modelo.ValorLiquidoCancelamento = item.XD204CB_VLCANC;
                    modelo.ValorVenda = item.XD204CB_VALVDA;
                    modelo.FlagNSUCartao = null;
                    modelo.IndicadorParcela = null;
                    retorno.Add(modelo);
                }
            }

            return retorno;
        }

        #endregion

        /// <summary>
        /// Consulta o total de requests pendentes de Débito.
        /// Utilizado na HomePage Segmentada.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do book:
        /// - Book BKXD0791 / Programa XD791 / Transação XDHS
        /// </remarks>
        /// <param name="numeroPv">Número do Estabelecimento</param>
        /// <returns>Quantidade de requests pendentes Débito</returns>
        public Int32 ConsultarTotalPendentesDebito(Int32 numeroPv)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Total Requests Pendentes - Débito (BKXD0791/XD791/XDHS)"))
            {
                try
                {                    
                    //Variável de retorno
                    Int32 totalRequests = default(Int32);

#if DEBUG
                    totalRequests = new Random().Next(1000);
#else
                    Log.GravarLog(EventoLog.ChamadaHIS, numeroPv);
                    //Chamada mainframe
                    using (var ctx = new ContextoWCF<COMTIXDClient>())
                        ctx.Cliente.ConsultarTotalPendentes(ref numeroPv, ref totalRequests);
                    Log.GravarLog(EventoLog.RetornoHIS, totalRequests);
#endif
                    //Retorno
                    return totalRequests;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
    }
}