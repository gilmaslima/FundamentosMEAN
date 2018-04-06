/*
(c) Copyright [2015] Rede S.A.
Autor       : [Daniel Torres]
Empresa     : [Iteris]
Histórico   :
- [13/03/2015] – [Daniel Torres] – [Criação]
- [14/10/2015] – [Dhouglas Lombello] – [Alteração de books durante homologação]
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Redecard.PN.Comum;
using Redecard.PN.Request.Agentes.COMTIXBChargeback;

namespace Redecard.PN.Request.Agentes
{
    /// <summary>
    /// Classe Agentes para o projeto Chargeback XB
    /// </summary>
    public class ChargebackXBAG : RequestBase
    {
        //XA Idêntico e/ou XA + XD Com parâmetro novo

        //XA + XD Juntos (XD205 + XA750)
        /// <summary>
        /// Consulta a descrição do motivo de débito - BKXB411
        /// </summary>
        /// <param name="codigoMotivoDebito">Codigo motivo debito</param>
        /// <param name="origem">Origem</param>
        /// <param name="transacao">Transacao</param>
        /// <param name="codigoRetorno">Codigo retorno</param>
        /// <returns>Retorna string de descrição do motivo</returns>
        public String ConsultaDescricaoMotivoDebito(
            Int16 codigoMotivoDebito, String origem, String transacao, out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Consulta Motivo de Débito [BKXB411/XBS411]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { codigoMotivoDebito, origem, transacao });

                try
                {
                    //Variáveis auxiliares                
                    var descricaoMotivoDebito = default(String);
                    var dscRetorno = "OK";
                    var codRetorno = default(Int16);

                    //Instanciação do serviço de acesso ao mainframe                    
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        codRetorno = ctx.Cliente.BKXB411(
                            out dscRetorno,
                            out descricaoMotivoDebito,
                            codigoMotivoDebito,
                            origem,
                            transacao);

                    descricaoMotivoDebito = descricaoMotivoDebito.Replace("\0", String.Empty);
                    codigoRetorno = (Int16)codRetorno;

                    log.GravarLog(EventoLog.FimAgente, new { codigoRetorno, descricaoMotivoDebito });

                    return descricaoMotivoDebito;
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

        //XA + XD Juntos (XA791 + XD791)
        /// <summary>
        /// Consulta o total de requests pendentes - BKXB422
        /// </summary>
        /// <param name="numeroPv">Número Pv</param>
        /// <param name="tipoProduto">Tipo do produto (débito ou crédito)</param>
        /// <returns></returns>
        public Int32 ConsultarTotalPendentes(Int32 numeroPv, Int16 tipoProduto)
        {
            using (Logger log = Logger.IniciarLog("Consultar Total Requests Pendentes (BKXB422/XBS422)"))
            {
                try
                {
                    //Variável de retorno
                    var totalRequests = default(Int32);
                    var codigoRetorno = default(Int16);
                    var dscRetorno = "OK";

                    log.GravarLog(EventoLog.InicioAgente, numeroPv);
                    //Chamada mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        codigoRetorno = ctx.Cliente.BKXB422(out dscRetorno, out totalRequests, numeroPv, tipoProduto);

                    log.GravarLog(EventoLog.FimAgente, new { codigoRetorno, dscRetorno, totalRequests });
                    //Retorno
                    return totalRequests;
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

        // XA380
        /// <summary>
        /// Consulta canal atual de recebimento de solicitacoes / cartas de debito - BKXB412
        /// </summary>
        /// <param name="codEstabelecimento">Código estabelecimento</param>
        /// <param name="origem">Origem</param>
        /// <param name="codigoCanal">Codigo Canal</param>
        /// <param name="descricaoCanal">Descrição canal</param>
        /// <param name="codigoOcorrencia">Codigo ocorrência</param>
        /// <returns>Retorna o código de retorno e outros valores por referência</returns>
        public Int32 ConsultarCanal(
            Int32 codEstabelecimento, String origem,
            ref Int16 codigoCanal, ref String descricaoCanal, ref Int64 codigoOcorrencia)
        {
            using (Logger log = Logger.IniciarLog("Como ser avisado - Consulta Canal [BKXB412/XBS412]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codEstabelecimento,
                    origem,
                    codigoCanal,
                    descricaoCanal,
                    codigoOcorrencia
                });

                try
                {
                    //Variáveis auxiliares
                    var msgRetorno = default(XB412_MSG_RETORNO);

                    //Efetua consulta do canal, retornando objeto contendo mensagem de retorno
                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        msgRetorno = ctx.Cliente.BKXB412(out codigoCanal, out descricaoCanal, codEstabelecimento, origem);

                    log.GravarLog(EventoLog.FimAgente, new { codigoCanal, descricaoCanal, codigoOcorrencia, msgRetorno });

                    //Retornando o código de retorno
                    return msgRetorno.XB412_COD_RETORNO;
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

        //  XA390
        /// <summary>
        /// Atualizar canal de envio do estabelecimento - BKXB413
        /// </summary>
        /// <param name="codEstabelecimento">Código estabelecimento</param>
        /// <param name="origem">Origem</param>
        /// <param name="canalRecebimento">Canal recebimento</param>
        /// <param name="msgRetorno">Mensagem retorno</param>
        /// <returns>Retorna o código de retorno e outros valores por referência</returns>
        public Int32 AtualizarCanal(Int32 codEstabelecimento, String origem, Int16 canalRecebimento, out String msgRetorno)
        {
            using (Logger log = Logger.IniciarLog("Atualizar Canal [BKXB413/XBS413]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { codEstabelecimento, origem, canalRecebimento });

                try
                {
                    var retorno = default(XB413_MSG_RETORNO);

                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        retorno = ctx.Cliente.BKXB413(codEstabelecimento, origem, canalRecebimento);

                    msgRetorno = retorno.XB413_DESC_RETORNO;

                    log.GravarLog(EventoLog.FimAgente, new { msgRetorno, retorno });

                    return retorno.XB413_COD_RETORNO;
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

        //  XA740
        /// <summary>
        /// Busca composição do resumo - BKXB414
        /// </summary>
        /// <param name="codEstabelecimento">codigo estabelecimento</param>
        /// <param name="codProcesso">codigo processo</param>
        /// <param name="origem">Origem</param>
        /// <param name="transacao">Transação</param>
        /// <param name="qtdOcorrencias">Quantidade ocorrencias</param>
        /// <param name="valorVenda">Valor venda</param>
        /// <param name="valorCancelamento">Valor cancelamento</param>
        /// <param name="qtdParcelas">Quantidade parcelas</param>
        /// <param name="qtdParcelasQuitadas">Quantidade parcelas quitadas</param>
        /// <param name="qtdParcelasAVencer">Quantidade parcelas a vencer</param>
        /// <param name="valorDeb">Valor débito</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoRetorno">Codigo retorno</param>
        /// <returns>Retorna lista de objetos ParcelaRV</returns>
        public List<Modelo.ParcelaRV> ComposicaoRV(
            Int32 codEstabelecimento, Decimal codProcesso, String origem, String transacao,
            out Int16 qtdOcorrencias, out Decimal valorVenda, out Decimal valorCancelamento, out Int16 qtdParcelas,
            out Int16 qtdParcelasQuitadas, out Int16 qtdParcelasAVencer, out Decimal valorDeb, out String filler,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Composição de Resumo de Vendas [BKXB414/XBS414]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { codEstabelecimento, codProcesso, origem, transacao });

                try
                {
                    //Variáveis auxiliares
                    var itens = new List<XB414_DADOS>();
                    var retorno = default(XB414_MSG_RETORNO);

                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        retorno = ctx.Cliente.BKXB414(
                            out qtdOcorrencias,
                            out itens,
                            out valorVenda,
                            out valorCancelamento,
                            out qtdParcelas,
                            out qtdParcelasQuitadas,
                            out qtdParcelasAVencer,
                            out valorDeb,
                            out filler,
                            codEstabelecimento,
                            codProcesso,
                            origem,
                            transacao);

                    //Mapeia itens de retorno para modelos de negócio
                    codigoRetorno = retorno.XB414_COD_RETORNO;

                    List<Modelo.ParcelaRV> parcelaRV = this.PreencherModelo(itens).ToList();

                    log.GravarLog(EventoLog.FimAgente, new
                    {
                        qtdOcorrencias,
                        valorVenda,
                        valorCancelamento,
                        qtdParcelas,
                        qtdParcelasQuitadas,
                        qtdParcelasAVencer,
                        valorDeb,
                        filler,
                        codigoRetorno,
                        parcelaRV,
                        retorno,
                        itens
                    });

                    return parcelaRV;
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
        /// Preenche modelo para método de busca de composição do resumo
        /// </summary>
        /// <param name="itens">itens</param>
        /// <returns>Retorna itens no modelo específico</returns>
        private List<Modelo.ParcelaRV> PreencherModelo(List<XB414_DADOS> itens)
        {
            var retorno = new List<Modelo.ParcelaRV>();

            if (itens != null)
            {
                foreach (XB414_DADOS item in itens)
                {
                    var modelo = new Modelo.ParcelaRV();
                    modelo.DataParcela = this.ParseDate(item.XB414_DT_CRE_PAR, "yyyyMMdd");
                    modelo.NumeroParcela = item.XB414_NU_PAR;
                    modelo.ValorLiquido = item.XB414_VL_LIQ_RES_PAR;
                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        // XA760
        /// <summary>
        /// Consulta da log de recebimento de comprovante de vendas(cv's) - BKXB415
        /// </summary>
        /// <param name="codEstabelecimento">Código estabelecimento</param>
        /// <param name="codProcesso">Código processo</param>
        /// <param name="flTemReg">Flag tem registro</param>
        /// <param name="qtdOcorrencias">Quantidade ocorrencias</param>
        /// <param name="filler">Filler</param>
        /// <param name="sistemaOrigem">Sistema origem</param>
        /// <param name="transacaoOrigem">Transação origem</param>
        /// <param name="codigoRetorno">Código Retorno</param>
        /// <returns>Retorna lista de objetos RecebimentoCV</returns>
        public List<Modelo.RecebimentoCV> RecebimentoCV(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            out Int16 flTemReg,
            out Int16 qtdOcorrencias,
            out String filler,
            String sistemaOrigem,
            String transacaoOrigem,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Recebimento de Comprovante de Vendas [BKXB415/XBS415]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { codEstabelecimento, codProcesso, sistemaOrigem, transacaoOrigem });

                try
                {
                    //Variáveis auxiliares
                    var itens = default(List<XB415_LINHA_REG>);
                    var retorno = default(XB415_RETORNO);

                    //Retorno do log de recebimento de CVs

                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        retorno = ctx.Cliente.BKXB415(
                            out flTemReg,
                            out qtdOcorrencias,
                            out itens,
                            out filler,
                            codEstabelecimento,
                            codProcesso,
                            sistemaOrigem,
                            transacaoOrigem);

                    codigoRetorno = retorno.XB415_COD_RETORNO;

                    List<Modelo.RecebimentoCV> cv = this.PreencherModelo(itens).ToList();

                    log.GravarLog(EventoLog.FimAgente, new
                    {
                        flTemReg,
                        qtdOcorrencias,
                        filler,
                        codigoRetorno,
                        cv,
                        retorno,
                        itens
                    });

                    return cv;
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
        /// Preenche modelo para método de consulta log comprovante de vendas
        /// </summary>
        /// <param name="itens">itens</param>
        /// <returns>Retorna itens no modelo específico</returns>
        private List<Modelo.RecebimentoCV> PreencherModelo(List<XB415_LINHA_REG> itens)
        {
            var retorno = new List<Modelo.RecebimentoCV>();

            if (itens != null)
            {
                foreach (XB415_LINHA_REG item in itens)
                {
                    var modelo = new Modelo.RecebimentoCV
                    {
                        CodigoRecebimento = item.XB415_COD_RCBM_MF,
                        DescricaoRecebimento = item.XB415_DSC_RCBM_MF,
                        DataRecebimento = this.ParseDate(item.XB415_DAT_RCBM_MF, "yyyyMMdd")
                    };
                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        //  XD202
        /// <summary>
        /// Consulta log de respostas aos estabelecimentos - BKXB423
        /// </summary>
        /// <param name="numeroProcesso">numero Processo</param>
        /// <param name="codEstabelecimento">Código estabelecimento</param>
        /// <param name="origem">origem</param>
        /// <param name="transacao">Transacao</param>
        /// <param name="codRetorno">Codigo Retorno</param>
        /// <param name="msgRetorno">Mensagem Retorno</param>
        /// <param name="qtdOcorrencias">Quantidade Ocorrencias</param>
        /// <returns>Retorna lista de Recebimentos de Comprovante de Vendas</returns>
        public List<Modelo.RecebimentoCV> ConsultaLogRespostasEstabelecimentos(
            Decimal numeroProcesso,
            Int32 codEstabelecimento,
            String origem,
            String transacao,
            out Int16 codRetorno,
            out String msgRetorno,
            out Int16 qtdOcorrencias)
        {
            using (Logger log = Logger.IniciarLog("Consulta log de respostas aos estabelecimentos [BKXB423/XBS423]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { numeroProcesso, codEstabelecimento, origem, transacao });

                try
                {
                    //Variáveis auxiliares
                    var cvs = default(List<XB423_OCC>);

                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        ctx.Cliente.BKXB423(
                            ref numeroProcesso,
                            ref codEstabelecimento,
                            ref origem,
                            ref transacao,
                            out codRetorno,
                            out msgRetorno,
                            out qtdOcorrencias,
                            out cvs);

                    List<Modelo.RecebimentoCV> recebimentoCV = this.PreencherModelo(cvs).ToList();

                    log.GravarLog(EventoLog.FimAgente, new { codRetorno, msgRetorno, qtdOcorrencias, cvs, recebimentoCV });

                    return recebimentoCV;
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
        /// Preenche modelo para método de consulta log de respostas aos estabelecimentos
        /// </summary>
        /// <param name="itens">itens</param>
        /// <returns>Retorna itens no modelo específico</returns>
        private List<Modelo.RecebimentoCV> PreencherModelo(List<XB423_OCC> itens)
        {
            var retorno = new List<Modelo.RecebimentoCV>();

            if (itens != null)
            {
                foreach (XB423_OCC item in itens)
                {
                    var modelo = new Modelo.RecebimentoCV
                    {
                        CodigoRecebimento = item.XB423_CQLDOC,
                        DescricaoRecebimento = item.XB423_DQLDOC,
                        DataRecebimento = this.ParseDate(item.XB423_DTRESP, "dd.MM.yyyy")
                    };
                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        //XA Com mudanças

        //BXA770
        /// <summary>
        /// Passar os débitos pendentes para o estabelecimento - BKXB416
        /// </summary>
        /// <param name="codEstabelecimento">Código estabelecimento</param>
        /// <param name="codProcesso">Código processo</param>
        /// <param name="tipo">Tipo</param>
        /// <param name="codCiclo">Codigo ciclo</param>
        /// <param name="codSequencia">Código sequência</param>
        /// <param name="origem">Origem</param>
        /// <param name="transacao">Transação</param>
        /// <param name="flTemReg">Flag Tem Registro</param>
        /// <param name="codUltimoProcesso">Código último processo</param>
        /// <param name="tipoUltimo">Tipo último</param>
        /// <param name="codUltimoCiclo">Código Último Ciclo</param>
        /// <param name="codUltimaSequencia">Código Última Sequência</param>
        /// <param name="qtdOcorrencias">Quantidade ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoRetorno">Código retorno</param>
        /// <returns>Retorna lista de débitos pendentes</returns>
        public List<Modelo.AvisoDebito> ConsultarDebitoPendente(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            Int16 tipo,
            Int16 codCiclo,
            Int16 codSequencia,
            String origem,
            String transacao,
            out Int16 flTemReg,
            out Decimal codUltimoProcesso,
            out Int16 tipoUltimo,
            out Int16 codUltimoCiclo,
            out Int16 codUltimaSequencia,
            out Int16 qtdOcorrencias,
            out String filler,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Passar os débitos pendentes para o estabelecimento [BKXB416/XBS416]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codEstabelecimento,
                    codProcesso,
                    tipo,
                    codCiclo,
                    codSequencia,
                    origem,
                    transacao
                });

                try
                {
                    //Variáveis auxiliares
                    var itens = default(List<XB416_LINHA_DEBITO>);
                    var retorno = default(XB416_MSG_RETORNO);

                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        retorno = ctx.Cliente.BKXB416(
                            out flTemReg,
                            out codUltimoProcesso,
                            out tipoUltimo,
                            out codUltimoCiclo,
                            out codUltimaSequencia,
                            out qtdOcorrencias,
                            out itens,
                            out filler,
                            codEstabelecimento,
                            codProcesso,
                            tipo,
                            codCiclo,
                            codSequencia,
                            origem,
                            transacao);

                    //Mapeia itens de retorno para modelos de negócio
                    codigoRetorno = retorno.XB416_COD_RETORNO;

                    List<Modelo.AvisoDebito> avisosDebito = this.PreencherModelo(itens).ToList();

                    log.GravarLog(EventoLog.FimAgente, new
                    {
                        flTemReg,
                        codUltimoProcesso,
                        tipoUltimo,
                        codUltimoCiclo,
                        codUltimaSequencia,
                        qtdOcorrencias,
                        filler,
                        codigoRetorno,
                        avisosDebito,
                        retorno,
                        itens
                    });

                    return avisosDebito;
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
        /// /// Preencher modelo para método Consultar débito pendente
        /// </summary>
        /// <param name="itens"></param>
        /// <returns></returns>
        private List<Modelo.AvisoDebito> PreencherModelo(List<XB416_LINHA_DEBITO> itens)
        {
            var retorno = new List<Modelo.AvisoDebito>();

            if (itens != null)
            {
                foreach (XB416_LINHA_DEBITO item in itens)
                {
                    var modelo = new Modelo.AvisoDebito
                    {
                        Centralizadora = item.XB416_CENTRAL,
                        CodigoMotivoDebito = item.XB416_MOT_DEBITO,
                        DataCancelamento = this.ParseDate(item.XB416_DT_DEBITO, "yyyyMMdd"),
                        DataVenda = this.ParseDate(item.XB416_DT_TRANSACAO, "yyyyMMdd"),
                        FlagNSUCartao = item.XB416_FLAG_NSU_CARTAO.FirstOrDefault(),
                        IndicadorParcela = String.Compare("S", item.XB416_IND_PARC, true) == 0,
                        IndicadorRequest = String.Compare("S", item.XB416_IND_REQ, true) == 0,
                        NumeroCartao = item.XB416_NR_CARTAO,
                        PontoVenda = item.XB416_NUM_PDV,
                        Processo = item.XB416_PROCESSO,
                        ResumoVenda = item.XB416_NR_RESUMO,
                        TipoCartao = item.XB416_TP_CARTAO,
                        ValorLiquidoCancelamento = item.XB416_VL_DEBITO,
                        ValorVenda = item.XB416_VL_TRANSACAO
                    };

                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        //BXA780
        /// <summary>
        /// Histórico de comprovantes do request - BKXB417
        /// </summary>
        /// <param name="codEstabelecimento">Código estabelecimento</param>
        /// <param name="dataIni">Data inicio</param>
        /// <param name="dataFim">Data fim</param>
        /// <param name="codProcesso">Código processo</param>
        /// <param name="tipo">Tipo</param>
        /// <param name="codCiclo">Código ciclo</param>
        /// <param name="codSequencia">Código sequência</param>
        /// <param name="origem">Origem</param>
        /// <param name="transacao">Transação</param>
        /// <param name="temReg">Tem registro</param>
        /// <param name="codUltimoProcesso">Código Último processo</param>
        /// <param name="tipoUltimo">Tipo Último</param>
        /// <param name="codUltimoCiclo">Código ultimo ciclo</param>
        /// <param name="codUltimaSequencia">Código ultima sequencia</param>
        /// <param name="qtdOcorrencias">Quantidade ocorrencias</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Codigo Ocorrencia</param>
        /// <param name="codigoRetorno">Código retorno</param>
        /// <returns></returns>
        public List<Modelo.Comprovante> HistoricoRequest(
            Int32 codEstabelecimento,
            DateTime dataIni,
            DateTime dataFim,
            Decimal codProcesso,
            Int16 tipo,
            Int16 codCiclo,
            Int16 codSequencia,
            String origem,
            String transacao,
            ref Int16 temReg,
            ref Decimal codUltimoProcesso,
            ref Int16 tipoUltimo,
            ref Int16 codUltimoCiclo,
            ref Int16 codUltimaSequencia,
            ref Int16 qtdOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Histórico de comprovantes do request [BKXB417/XBS417]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codEstabelecimento,
                    dataIni,
                    dataFim,
                    codProcesso,
                    tipo,
                    codCiclo,
                    codSequencia,
                    origem,
                    transacao,
                    temReg,
                    codUltimoProcesso,
                    tipoUltimo,
                    codUltimoCiclo,
                    codUltimaSequencia,
                    qtdOcorrencias,
                    filler,
                    codigoOcorrencia
                });

                try
                {
                    //Variáveis auxiliares
                    var requests = default(List<BKXB0417_LINHA_REQUEST>);
                    String descRetorno = String.Empty;
                    Int32 dataInicio = dataIni.ToString("yyyyMMdd").ToInt32();
                    Int32 dataFinal = dataFim.ToString("yyyyMMdd").ToInt32();
                    Int16 codRetorno = 0;
                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        codRetorno = ctx.Cliente.BKXB417(
                            out descRetorno,
                            out temReg,
                            out codUltimoProcesso,
                            out tipoUltimo,
                            out codUltimoCiclo,
                            out codUltimaSequencia,
                            out qtdOcorrencias,
                            out requests,
                            out filler,
                            codEstabelecimento,
                            dataIni.ToString("yyyyMMdd").ToInt32(),
                            dataFim.ToString("yyyyMMdd").ToInt32(),
                            codProcesso,
                            tipo,
                            codCiclo,
                            codSequencia,
                            origem,
                            transacao);

                    //Retorna o histórico de comprovantes, convertendo para Modelo
                    List<Modelo.Comprovante> comprovantes = this.PreencherModelo(requests).ToList();
                    codigoRetorno = codRetorno;

                    log.GravarLog(EventoLog.FimAgente, new
                    {
                        codEstabelecimento,
                        dataInicio,
                        dataFinal,
                        codProcesso,
                        tipo,
                        codCiclo,
                        codSequencia,
                        origem,
                        transacao,
                        codigoRetorno,
                        descRetorno,
                        temReg,
                        codUltimoProcesso,
                        tipoUltimo,
                        codUltimoCiclo,
                        codUltimaSequencia,
                        qtdOcorrencias,
                        requests,
                        filler
                    });

                    return comprovantes;
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
        /// Preencher modelo para método Historico Request
        /// </summary>
        /// <param name="requests">requests</param>
        /// <returns>Retorna lista de objetos de Request</returns>
        private List<Modelo.Comprovante> PreencherModelo(List<BKXB0417_LINHA_REQUEST> requests)
        {
            var retorno = new List<Modelo.Comprovante>();

            if (requests != null)
            {
                foreach (BKXB0417_LINHA_REQUEST request in requests)
                {
                    var comprovante = new Modelo.Comprovante
                    {
                        CanalEnvio = request.BKXB0417_CANAL_ENVIO,
                        Centralizadora = request.BKXB0417_CENTRAL,
                        DataEnvio = this.ParseDate(request.BKXB0417_DT_ENVIO, "yyyyMMdd"),
                        DataVenda = this.ParseDate(request.BKXB0417_DT_TRANSACAO, "yyyyMMdd"),
                        FlagNSUCartao = request.BKXB0417_FLAG_NSU_CARTAO.FirstOrDefault(),
                        IndicadorDebito = String.Compare("S", request.BKXB0417_IND_DEB, true) == 0,
                        Motivo = request.BKXB0417_DESC_MOTIVO,
                        NumeroCartao = request.BKXB0417_NR_CARTAO,
                        PontoVenda = request.BKXB0417_NUM_PDV,
                        Processo = request.BKXB0417_PROCESSO_S,
                        QualidadeRecebimentoDocumentos = request.BKXB0417_QUAL_RECEB,
                        ResumoVenda = request.BKXB0417_NR_RESUMO,
                        SolicitacaoAtendida = String.Compare("S", request.BKXB0417_SOL_ATENDIDA, true) == 0,
                        TipoCartao = request.BKXB0417_TP_CARTAO,
                        ValorVenda = request.BKXB0417_VL_TRANSACAO,
                        DataLimiteEnvioDocumentos = null
                    };
                    retorno.Add(comprovante);
                }
            }

            return retorno;
        }

        //BXA790
        /// <summary>
        /// Passar os requests pendentes para estabelecimento - BKXB418
        /// </summary>
        /// <param name="codEstabelecimento">Codigo estabelecimento</param>
        /// <param name="codProcesso">Codigo processo</param>
        /// <param name="origem">Origem</param>
        /// <param name="transacao">Transação</param>
        /// <param name="flTemReg">Flag tem registro</param>
        /// <param name="codUltimoProcesso">Codigo ultimo processo</param>
        /// <param name="qtdLinhasOcorrencia">Quantidade linhas ocorrencia</param>
        /// <param name="qtdTotalOcorrencias">Quantidade total ocorrencias</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoRetorno">Codigo retorno</param>
        /// <returns>Retorna lista de objetos do tipo comprovante</returns>
        public List<Modelo.Comprovante> ConsultarRequestPendente(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            Int16 tipo,
            Int16 codCiclo,
            Int16 codSequencia,
            String origem,
            String transacao,
            ref Int16 flTemReg,
            ref Decimal codUltimoProcesso,
            ref Int16 tipoUltimo,
            ref Int16 codUltimoCiclo,
            ref Int16 codUltimaSequencia,
            ref Int16 qtdLinhasOcorrencia,
            ref Int32 qtdTotalOcorrencias,
            ref String filler,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Comprovantes Pendentes para estabelecimento [BKXB418/XBS418]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codEstabelecimento,
                    codProcesso,
                    origem,
                    transacao,
                    flTemReg,
                    codUltimoProcesso,
                    tipoUltimo,
                    codUltimoCiclo,
                    codUltimaSequencia,
                    qtdLinhasOcorrencia,
                    qtdTotalOcorrencias,
                    filler
                });

                try
                {
                    //Variáveis auxiliares
                    var requests = default(List<XB418_LINHA_REQUEST>);
                    var retorno = default(XB418_MSG_RETORNO);

                    //Executa consulta de comprovantes pendentes
                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        ctx.Cliente.BKXB418(
                            ref codEstabelecimento,
                            ref codProcesso,
                            ref tipo,
                            ref codCiclo,
                            ref codSequencia,
                            ref origem,
                            ref transacao,
                            out retorno,
                            ref flTemReg,
                            ref codUltimoProcesso,
                            ref tipoUltimo,
                            ref codUltimoCiclo,
                            ref codUltimaSequencia,
                            ref qtdLinhasOcorrencia,
                            ref qtdTotalOcorrencias,
                            out requests,
                            ref filler);

                    //Converte mensagem de retorno para Modelo
                    codigoRetorno = retorno.XB418_COD_RETORNO;

                    //Retorna comprovantes pendentes, convertendo itens para Modelo
                    List<Modelo.Comprovante> comprovantes = this.PreencherModelo(requests).ToList();

                    log.GravarLog(EventoLog.FimAgente, new
                    {
                        flTemReg,
                        codUltimoProcesso,
                        qtdLinhasOcorrencia,
                        qtdTotalOcorrencias,
                        filler,
                        requests,
                        retorno,
                        comprovantes
                    });

                    return comprovantes;
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
        /// Preencher modelo para método Consultar Request Pendente
        /// </summary>
        /// <param name="requests">requests</param>
        /// <returns>Retorna lista de objetos de Comprovantes</returns>
        private List<Modelo.Comprovante> PreencherModelo(List<XB418_LINHA_REQUEST> requests)
        {
            List<Modelo.Comprovante> retorno = new List<Modelo.Comprovante>();

            if (requests != null)
            {
                foreach (XB418_LINHA_REQUEST request in requests)
                {
                    var comprovante = new Modelo.Comprovante();
                    comprovante.Processo = request.XB418_PROCESSO_S;
                    comprovante.CanalEnvio = request.XB418_CANAL_ENVIO;
                    comprovante.Centralizadora = request.XB418_CENTRAL;
                    comprovante.DataEnvio = this.ParseDate(request.XB418_DT_ENVIO, "yyyyMMdd");
                    comprovante.DataLimiteEnvioDocumentos = this.ParseDate(request.XB418_DT_LIMITE_ENVO, "yyyyMMdd");
                    comprovante.DataVenda = this.ParseDate(request.XB418_DT_TRANSACAO, "yyyyMMdd");
                    comprovante.FlagNSUCartao = request.XB418_FLAG_NSU_CARTAO.FirstOrDefault();
                    comprovante.Motivo = request.XB418_DESC_MOTIVO;
                    comprovante.NumeroCartao = request.XB418_NR_CARTAO;
                    comprovante.PontoVenda = request.XB418_NUM_PDV;
                    comprovante.QualidadeRecebimentoDocumentos = request.XB418_QUAL_RECEB;
                    comprovante.ResumoVenda = request.XB418_NR_RESUMO;
                    comprovante.SolicitacaoAtendida = String.Compare("S", request.XB418_SOL_ATENDIDA, true) == 0;
                    comprovante.TipoCartao = request.XB418_TP_CARTAO;
                    comprovante.ValorVenda = request.XB418_VL_TRANSACAO;
                    comprovante.IndicadorDebito = null;
                    comprovante.NumeroReferencia = request.XB418_NUM_REF.ToString();
                    retorno.Add(comprovante);
                }
            }

            return retorno;
        }

        //XD Com mudanças

        //XD201
        /// <summary>
        /// Consulta comprovante solicitacao pendente de processos - BKXB419
        /// </summary>
        /// <param name="codEstabelecimento">codEstabelecimento</param>
        /// <param name="dataInicio">dataInicio</param>
        /// <param name="dataFim">dataFim</param>
        /// <param name="sistemaOrigem">sistemaOrigem</param>
        /// <param name="codigoTransacao">codigoTransacao</param>
        /// <param name="indicadorPesquisa">indicadorPesquisa</param>
        /// <param name="numeroProcessoI">numeroProcessoI</param>
        /// <param name="tpProcessoI">tpProcessoI</param>
        /// <param name="clProcessoI">clProcessoI</param>
        /// <param name="codigoSequenciaI">codigoSequenciaI</param>
        /// <param name="dtEmissaoI">dtEmissaoI</param>
        /// <param name="numeroProcessoF">numeroProcessoF</param>
        /// <param name="tpProcessoF">tpProcessoF</param>
        /// <param name="clProcessoF">clProcessoF</param>
        /// <param name="codigoSequenciaF">codigoSequenciaF</param>
        /// <param name="dtEmissaoF">dtEmissaoF</param>
        /// <param name="indCont">indCont</param>
        /// <param name="codigoRetorno">codigoRetorno</param>
        /// <param name="msgRetorno">msgRetorno</param>
        /// <param name="qtdOcorrencias">qtdOcorrencias</param>
        /// <returns>Retorna lista de objetos Comprovantes</returns>
        public List<Modelo.Comprovante> ConsultaSolicitacaoPendente(
            Int32 codEstabelecimento,
            Int32 dataInicio,
            Int32 dataFim,
            String sistemaOrigem,
            String codigoTransacao,
            String indicadorPesquisa,
            ref Decimal numeroProcessoI,
            ref Int16 tpProcessoI,
            ref Int16 clProcessoI,
            ref Int16 codigoSequenciaI,
            ref String dtEmissaoI,
            ref Decimal numeroProcessoF,
            ref Int16 tpProcessoF,
            ref Int16 clProcessoF,
            ref Int16 codigoSequenciaF,
            ref String dtEmissaoF,
            ref String indCont,
            out Int16 codigoRetorno,
            out String msgRetorno,
            out Int16 qtdOcorrencias)
        {
            using (Logger log = Logger.IniciarLog("Consulta comprovante solicitacao pendente de processos [BKXB419/XBS419]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codEstabelecimento,
                    dataInicio,
                    dataFim,
                    sistemaOrigem,
                    codigoTransacao,
                    indicadorPesquisa,
                    numeroProcessoI,
                    tpProcessoI,
                    clProcessoI,
                    codigoSequenciaI,
                    dtEmissaoI,
                    numeroProcessoF,
                    tpProcessoF,
                    clProcessoF,
                    codigoSequenciaF,
                    dtEmissaoF,
                    indCont
                });

                try
                {
                    //Variáveis auxiliares
                    var registros = default(List<XB419_OCC>);

                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        ctx.Cliente.BKXB419(
                            ref codEstabelecimento,
                            ref dataInicio,
                            ref dataFim,
                            ref sistemaOrigem,
                            ref codigoTransacao,
                            ref indicadorPesquisa,
                            ref numeroProcessoI,
                            ref tpProcessoI,
                            ref clProcessoI,
                            ref codigoSequenciaI,
                            ref dtEmissaoI,
                            ref numeroProcessoF,
                            ref tpProcessoF,
                            ref clProcessoF,
                            ref codigoSequenciaF,
                            ref dtEmissaoF,
                            ref indCont,
                            out codigoRetorno,
                            out msgRetorno,
                            out qtdOcorrencias,
                            out registros);

                    //Retorna os comprovantes pendentes, convertendo para Modelo
                    List<Modelo.Comprovante> comprovantes = this.PreencherModelo(registros).ToList();

                    log.GravarLog(EventoLog.FimAgente, new
                    {
                        numeroProcessoI,
                        tpProcessoI,
                        clProcessoI,
                        codigoSequenciaI,
                        dtEmissaoI,
                        numeroProcessoF,
                        tpProcessoF,
                        clProcessoF,
                        dtEmissaoF,
                        indCont,
                        codigoRetorno,
                        msgRetorno,
                        qtdOcorrencias,
                        registros,
                        comprovantes
                    });

                    return comprovantes;
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
        /// Preencher modelo para método consultar solicitação pendente
        /// </summary>
        /// <param name="requests">requests</param>
        /// <returns>Retorna lista de objetos de Consulta Solicitacao Pendente</returns>
        private List<Modelo.Comprovante> PreencherModelo(List<XB419_OCC> requests)
        {
            var retorno = new List<Modelo.Comprovante>();

            if (requests != null)
            {
                foreach (XB419_OCC request in requests)
                {
                    var modelo = new Modelo.Comprovante
                    {
                        CanalEnvio = request.XB419_CENVSL,
                        DescricaoCanalEnvio = request.XB419_DENVSL,
                        Centralizadora = request.XB419_CENTRAL,
                        DataEnvio = this.ParseDateNull(request.XB419_DTENVD, "dd.MM.yyyy"),
                        DataLimiteEnvioDocumentos = this.ParseDateNull(request.XB419_DATLIM, "dd.MM.yyyy"),
                        DataVenda = this.ParseDate(request.XB419_DATVDA, "dd.MM.yyyy"),
                        Motivo = request.XB419_DSCMOT,
                        NumeroCartao = Convert.ToString(request.XB419_CQLDOC),
                        PontoVenda = request.XB419_NROPV,
                        Processo = request.XB419_NRPROC,
                        QualidadeRecebimentoDocumentos = request.XB419_DQLDOC,
                        ResumoVenda = request.XB419_RESVDA,
                        SolicitacaoAtendida = String.Compare("S", request.XB419_SOLCAT, true) == 0,
                        ValorVenda = request.XB419_VALVDA,
                        NumeroReferencia = Convert.ToString(request.XB419_TRNACQ),
                        FlagNSUCartao = null,
                        IndicadorDebito = String.Compare("S", request.XB419_SOLCAT, true) == 0,
                        TipoCartao = String.Empty,
                        CodigoMotivoProcesso = request.XB419_CODMOT
                    };
                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        //XD203
        /// <summary>
        /// Consulta histórico de solicitações - BKXB420
        /// </summary>
        /// <param name="numeroProcesso">numeroProcesso</param>
        /// <param name="numeroPV">numeroPV</param>
        /// <param name="dataInicio">dataInicio</param>
        /// <param name="dataFim">dataFim</param>
        /// <param name="origem">origem</param>
        /// <param name="transacao">transacao</param>
        /// <param name="indicePesquisa">indicePesquisa</param>
        /// <param name="numeroProcessoI">numeroProcessoI</param>
        /// <param name="tipoProcessoI">tipoProcessoI</param>
        /// <param name="cicloProcessoI">cicloProcessoI</param>
        /// <param name="codSequenciaI">codSequenciaI</param>
        /// <param name="numeroPVI">numeroPVI</param>
        /// <param name="dataEmissaoI">dataEmissaoI</param>
        /// <param name="numeroProcessoF">numeroProcessoF</param>
        /// <param name="tipoProcessoF">tipoProcessoF</param>
        /// <param name="cicloProcessoF">cicloProcessoF</param>
        /// <param name="codSequenciaF">codSequenciaF</param>
        /// <param name="numeroPVF">numeroPVF</param>
        /// <param name="dataEmissaoF">dataEmissaoF</param>
        /// <param name="indCont">indCont</param>
        /// <param name="codigoRetorno">codigoRetorno</param>
        /// <param name="msgRetorno">msgRetorno</param>
        /// <param name="qtdOcorrencias">qtdOcorrencias</param>
        /// <returns>Retorna lista de objetos Comprovante</returns>
        public List<Modelo.Comprovante> ConsultarHistoricoSolicitacoes(
            Decimal numeroProcesso,
            Int32 numeroPV,
            Int32 dataInicio,
            Int32 dataFim,
            String origem,
            String transacao,
            String indicePesquisa,
            ref Decimal numeroProcessoI,
            ref Int16 tipoProcessoI,
            ref Int16 cicloProcessoI,
            ref Int16 codSequenciaI,
            ref Int32 numeroPVI,
            ref String dataEmissaoI,
            ref Decimal numeroProcessoF,
            ref Int16 tipoProcessoF,
            ref Int16 cicloProcessoF,
            ref Int16 codSequenciaF,
            ref Int32 numeroPVF,
            ref String dataEmissaoF,
            ref String indCont,
            out Int16 codigoRetorno,
            out String msgRetorno,
            out Int16 qtdOcorrencias)
        {
            using (Logger log = Logger.IniciarLog("Consulta histórico de solicitações [BKXB420/XBS420]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    numeroProcesso,
                    numeroPV,
                    dataInicio,
                    dataFim,
                    origem,
                    transacao,
                    indicePesquisa,
                    numeroProcessoI,
                    tipoProcessoI,
                    codSequenciaI,
                    cicloProcessoI,
                    numeroPVI,
                    dataEmissaoI,
                    numeroProcessoF,
                    tipoProcessoF,
                    cicloProcessoF,
                    codSequenciaF,
                    numeroPVF,
                    dataEmissaoF,
                    indCont
                });

                try
                {
                    //Variáveis auxiliares
                    var requests = default(List<XB420_OCC>);

                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        ctx.Cliente.BKXB420(
                            ref numeroProcesso,
                            ref numeroPV,
                            ref dataInicio,
                            ref dataFim,
                            ref origem,
                            ref transacao,
                            ref indicePesquisa,
                            ref numeroProcessoI,
                            ref tipoProcessoI,
                            ref codSequenciaI,
                            ref cicloProcessoI,
                            ref numeroPVI,
                            ref dataEmissaoI,
                            ref numeroProcessoF,
                            ref tipoProcessoF,
                            ref cicloProcessoF,
                            ref codSequenciaF,
                            ref numeroPVF,
                            ref dataEmissaoF,
                            ref indCont,
                            out codigoRetorno,
                            out msgRetorno,
                            out qtdOcorrencias,
                            out requests);

                    //Retorna o histórico das solicitações
                    List<Modelo.Comprovante> historico = this.PreencherModelo(requests).ToList();

                    log.GravarLog(EventoLog.FimAgente, new
                    {
                        numeroProcessoI,
                        tipoProcessoI,
                        codSequenciaI,
                        cicloProcessoI,
                        numeroPVI,
                        dataEmissaoI,
                        numeroProcessoF,
                        tipoProcessoF,
                        cicloProcessoF,
                        numeroPVF,
                        dataEmissaoF,
                        indCont,
                        codigoRetorno,
                        msgRetorno,
                        qtdOcorrencias,
                        requests,
                        historico
                    });

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


        /// <summary>
        /// Preencher modelo para método Consulta Historico Solicitacoes
        /// </summary>
        /// <param name="requests">requests</param>
        /// <returns>Retorna lista de objetos de Comprovantes</returns>
        private List<Modelo.Comprovante> PreencherModelo(List<XB420_OCC> requests)
        {
            var retorno = new List<Modelo.Comprovante>();

            if (retorno != null)
            {
                foreach (XB420_OCC request in requests)
                {
                    var modelo = new Modelo.Comprovante
                    {
                        CanalEnvio = request.XB420_CENVDC,
                        Centralizadora = request.XB420_CENTRAL,
                        DataEnvio = this.ParseDateNull(request.XB420_DTENVD, "dd.MM.yyyy"),
                        DataLimiteEnvioDocumentos = null,
                        DataVenda = this.ParseDate(request.XB420_DATVDA, "dd.MM.yyyy"),
                        IndicadorDebito = String.Compare("S", request.XB420_INDDEB, true) == 0,
                        Motivo = request.XB420_DSCMOT,
                        NumeroCartao = Convert.ToString(request.XB420_TRNACQ),
                        PontoVenda = request.XB420_NROPV,
                        Processo = request.XB420_NRPROC,
                        QualidadeRecebimentoDocumentos = request.XB420_DQLDOC,
                        ResumoVenda = request.XB420_RESVDA,
                        SolicitacaoAtendida = String.Compare("S", request.XB420_SOLCAT, true) == 0,
                        ValorVenda = request.XB420_VALVDA,
                        FlagNSUCartao = null,
                        TipoCartao = null
                    };
                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        //XD204
        /// <summary>
        /// Consulta aviso de debitos - BKXB421
        /// </summary>
        /// <param name="codProcesso">codProcesso</param>
        /// <param name="codEstabelecimento">codEstabelecimento</param>
        /// <param name="dataIni">dataIni</param>
        /// <param name="dataFim">dataFim</param>
        /// <param name="origem">origem</param>
        /// <param name="transacao">transacao</param>
        /// <param name="indPesq">indPesq</param>
        /// <param name="numeroProcessoI">numeroProcessoI</param>
        /// <param name="tipoProcessoI">tipoProcessoI</param>
        /// <param name="cicloProcessoI">cicloProcessoI</param>
        /// <param name="codSequenciaI">codSequenciaI</param>
        /// <param name="numeroPVI">numeroPVI</param>
        /// <param name="dtEmissaoI">dtEmissaoI</param>
        /// <param name="numeroProcessoF">numeroProcessoF</param>
        /// <param name="tipoProcessoF">tipoProcessoF</param>
        /// <param name="cicloProcessoF">cicloProcessoF</param>
        /// <param name="codSequenciaF">codSequenciaF</param>
        /// <param name="numeroPVF">numeroPVF</param>
        /// <param name="dtEmissaoF">dtEmissaoF</param>
        /// <param name="indCont">indCont</param>
        /// <param name="codigoRetorno">codigoRetorno</param>
        /// <param name="qtdOcorrencias">qtdOcorrencias</param>
        /// <returns>Retorna lista de objetos de aviso de débito</returns>
        public List<Modelo.AvisoDebito> ConsultaAvisoDebito(
            Decimal codProcesso,
            Int32 codEstabelecimento,
            DateTime dataIni,
            DateTime dataFim,
            String origem,
            String transacao,
            ref String indPesq,
            ref Decimal numeroProcessoI,
            ref String tipoProcessoI,
            ref Int16 cicloProcessoI,
            ref Int16 codSequenciaI,
            ref Int32 numeroPVI,
            ref String dtEmissaoI,
            ref Decimal numeroProcessoF,
            ref Int16 tipoProcessoF,
            ref Int32 cicloProcessoF,
            ref Int16 codSequenciaF,
            ref Int32 numeroPVF,
            ref String dtEmissaoF,
            ref String indCont,
            out Int32 codigoRetorno,
            out Int16 qtdOcorrencias)
        {
            using (Logger log = Logger.IniciarLog("Consulta aviso de debitos [BKXB421/XBS421]"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codProcesso,
                    codEstabelecimento,
                    dataIni,
                    dataFim,
                    origem,
                    transacao,
                    indPesq,
                    numeroProcessoI,
                    tipoProcessoI,
                    cicloProcessoI,
                    codSequenciaI,
                    numeroPVI,
                    dtEmissaoI,
                    numeroProcessoF,
                    tipoProcessoF,
                    cicloProcessoF,
                    codSequenciaF,
                    numeroPVF,
                    dtEmissaoF,
                    indCont
                });

                try
                {
                    //Variáveis auxiliares                
                    var itens = default(List<XB421_OCC>);
                    Int32 dtIni = dataIni.ToString("yyyyMMdd").ToInt32();
                    Int32 dtFim = dataFim.ToString("yyyyMMdd").ToInt32();
                    var codRetorno = default(Int16);

                    String dscRetorno = "OK";

                    //Instancia o serviço de acesso ao mainframe
                    using (var ctx = new ContextoWCF<COMTIXBClient>())
                        ctx.Cliente.BKXB421(
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
                            ref codSequenciaI,
                            ref numeroPVI,
                            ref dtEmissaoI,
                            ref numeroProcessoF,
                            ref tipoProcessoF,
                            ref cicloProcessoF,
                            ref codSequenciaF,
                            ref numeroPVF,
                            ref dtEmissaoF,
                            ref indCont,
                            out codRetorno,
                            out dscRetorno,
                            out qtdOcorrencias,
                            out itens);

                    codigoRetorno = codRetorno;

                    List<Modelo.AvisoDebito> avisos = this.PreencherModelo(itens).ToList();

                    log.GravarLog(EventoLog.FimAgente, new
                    {
                        indPesq,
                        numeroProcessoI,
                        tipoProcessoI,
                        cicloProcessoI,
                        codSequenciaI,
                        numeroPVI,
                        dtEmissaoI,
                        numeroProcessoF,
                        tipoProcessoF,
                        cicloProcessoF,
                        numeroPVF,
                        dtEmissaoF,
                        indCont,
                        codigoRetorno,
                        qtdOcorrencias,
                        itens,
                        avisos
                    });

                    return avisos;
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
        /// Preencher modelo para método consulta aviso de debitos
        /// </summary>
        /// <param name="requests">requests</param>
        /// <returns>Retorna lista de objetos de Aviso de debitos</returns>
        private List<Modelo.AvisoDebito> PreencherModelo(List<XB421_OCC> itens)
        {
            var retorno = new List<Modelo.AvisoDebito>();

            if (itens != null)
            {
                foreach (XB421_OCC item in itens)
                {
                    var modelo = new Modelo.AvisoDebito();
                    modelo.Centralizadora = item.XB421_CENTRAL;
                    modelo.CodigoMotivoDebito = item.XB421_CODDEB;
                    modelo.DataCancelamento = this.ParseDate(item.XB421_DTCANC, "dd.MM.yyyy");
                    modelo.DataVenda = this.ParseDate(item.XB421_DATVDA, "dd.MM.yyyy");
                    modelo.IndicadorRequest = String.Compare("S", item.XB421_INDREQ, true) == 0;
                    modelo.NumeroCartao = item.XB421_NROCAR;
                    modelo.PontoVenda = item.XB421_NROPV;
                    modelo.Processo = item.XB421_NRPROC;
                    modelo.ResumoVenda = item.XB421_RESVDA;
                    modelo.TipoCartao = item.XB421_TRNACQ.ToString();
                    modelo.ValorLiquidoCancelamento = item.XB421_VLCANC;
                    modelo.ValorVenda = item.XB421_VALVDA;
                    modelo.FlagNSUCartao = null;
                    modelo.IndicadorParcela = null;
                    retorno.Add(modelo);
                }
            }

            return retorno;
        }
    }
}