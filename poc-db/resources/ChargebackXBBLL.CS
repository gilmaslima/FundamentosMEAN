/*
(c) Copyright [2015] Rede S.A.
Autor       : [Daniel Torres]
Empresa     : [Iteris]
Histórico   :
- [13/03/2015] – [Daniel Torres] – [Criação]
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Redecard.PN.Comum;

namespace Redecard.PN.Request.Negocio
{
    public class ChargebackXBBLL : RegraDeNegocioBase
    {
        //XA + XD Juntos (XD205 + XA750)
        /// <summary>
        /// Consulta a descrição do motivo de débito
        /// </summary>
        /// <param name="codMotivoDebito">Código motivo debito</param>
        /// <param name="origem">Origem</param>
        /// <param name="transacao">Transação</param>
        /// <param name="codigoRetorno">Código retorno</param>
        /// <returns>Retorna string de descrição do motivo</returns>
        public String ConsultarMotivoDebito(
            Int16 codMotivoDebito,
            String origem,
            String transacao,
            out Int32 codigoRetorno)
        {

            using (Logger log = Logger.IniciarLog("Consulta Motivo de Débito [BKXB411/XBS411]"))
            {

                log.GravarLog(EventoLog.InicioNegocio, new { codMotivoDebito, origem, transacao });

                try
                {
                    String retorno = new Agentes.ChargebackXBAG().ConsultaDescricaoMotivoDebito(
                                codMotivoDebito,
                                origem,
                                transacao,
                                out codigoRetorno);

                    log.GravarLog(EventoLog.RetornoNegocio, retorno);

                    //Retorna a descrição do motivo do débito
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

        //XA + XD Juntos (XA791 + XD791)
        /// <summary>
        /// Consulta o total de requests pendentes
        /// Utilizado na HomePage Segmentada.
        /// </summary>
        /// <param name="numeroPv">Número do Estabelecimento</param>
        /// <param name="tipoProduto">Tipo do Produto</param>
        /// <returns>Quantidade de requests pendentes</returns>
        public Int32 ConsultarTotalPendentesDebito(Int32 numeroPv, Int16 tipoProduto)
        {
            using (var log = Logger.IniciarLog("Consultar Total Requests Pendentes (BKXB422/XBS422)"))
            {
                try
                {
                    log.GravarLog(EventoLog.InicioNegocio, new { numeroPv, tipoProduto });

                    Int32 totalPendentes = new Agentes.ChargebackXBAG().ConsultarTotalPendentes(numeroPv, tipoProduto);

                    log.GravarLog(EventoLog.RetornoNegocio, totalPendentes);

                    return totalPendentes;
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
            Int32 codEstabelecimento,
            String origem,
            ref Int16 codigoCanal,
            ref String descricaoCanal,
            ref Int64 codigoOcorrencia)
        {
            using (var log = Logger.IniciarLog("Como ser avisado - Consulta Canal [BKXB412/XBS412]"))
            {
                try
                {
                    log.GravarLog(EventoLog.InicioNegocio, new { codEstabelecimento, origem, 
                        codigoCanal, descricaoCanal, codigoOcorrencia });

                    Int32 retorno = new Agentes.ChargebackXBAG().ConsultarCanal(
                        codEstabelecimento, origem, ref codigoCanal, ref descricaoCanal, ref codigoOcorrencia);

                    log.GravarLog(EventoLog.RetornoNegocio, retorno);

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

        //  XA390
        /// <summary>
        /// Atualização de Canal
        /// </summary>
        /// <param name="codEstabelecimento">Código do estabelecimento</param>
        /// <param name="canalRecebimento">Canal do recebimento</param>
        /// <param name="origem">Origem</param>
        /// <param name="msgRetorno">Mensagem de retorno</param>
        /// <returns>Retorno</returns>
        public Int32 AtualizarCanal(
            Int32 codEstabelecimento,
            Int16 canalRecebimento,
            String origem,
            out String msgRetorno)
        {

            using (var log = Logger.IniciarLog("Como ser avisado - Consulta Canal [BKXB412/XBS412]"))
            {
                try
                {
                    log.GravarLog(EventoLog.InicioNegocio, new { codEstabelecimento, canalRecebimento, origem });

                    Int32 retorno = new Agentes.ChargebackXBAG().AtualizarCanal(
                        codEstabelecimento, origem, canalRecebimento, out msgRetorno);

                    log.GravarLog(EventoLog.RetornoNegocio, retorno);

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

        //  XA740
        /// <summary>
        /// Composição RV
        /// </summary>
        /// <param name="codEstabelecimento">Código do estabelecimento</param>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="origem">Origem</param>
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="valorVenda">Valor venda</param>
        /// <param name="valorCanal">Valor canal</param>
        /// <param name="qtdParTot">Quantidade total de parcelas</param>
        /// <param name="qtdParQuit">Quantidade parcelas quitadas</param>
        /// <param name="qtdParAven">Quantidade parcelas a vencer</param>
        /// <param name="valorDeb">Valor débito</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Código ocorrência</param>
        /// <param name="codigoRetorno">Código retorno</param>
        /// <returns>Composições RV</returns>
        public List<Modelo.ParcelaRV> ComposicaoRV(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            ref Int16 qtdOcorrencias,
            ref Decimal valorVenda,
            ref Decimal valorCanal,
            ref Int16 qtdParTot,
            ref Int16 qtdParQuit,
            ref Int16 qtdParAven,
            ref Decimal valorDeb,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (var log = Logger.IniciarLog("Como ser avisado - Consulta Canal [BKXB412/XBS412]"))
            {
                try
                {
                    log.GravarLog(EventoLog.InicioNegocio, new { codEstabelecimento, codProcesso, origem, 
                        qtdOcorrencias, valorCanal, qtdParTot, qtdParQuit, qtdParAven, valorDeb, filler, codigoOcorrencia });

                    //Preenchimento de parâmetros para chamada do agente
                    String transacao = "IS69";

                    //Consulta que retorna a composição do resumo
                    List<Modelo.ParcelaRV> retorno = new Agentes.ChargebackXBAG().ComposicaoRV(
                        codEstabelecimento,
                        codProcesso,
                        origem,
                        transacao,
                        out qtdOcorrencias,
                        out valorVenda,
                        out valorCanal,
                        out qtdParTot,
                        out qtdParQuit,
                        out qtdParAven,
                        out valorDeb,
                        out filler,
                        out codigoRetorno);

                    //Filtra os registros que possuem dados (qtdOcorrencias possui a quantidade de registros retornados na consulta)
                    if (retorno != null && retorno.Count > 0)
                        retorno = retorno.Take(qtdOcorrencias).ToList();

                    //Não retorna lista nula, e sim, lista vazia
                    if (retorno == null)
                        retorno = new List<Modelo.ParcelaRV>();

                    log.GravarLog(EventoLog.RetornoNegocio, retorno);

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

        // XA760
        /// <summary>
        /// Consulta da log de recebimento de comprovante de vendas(cv's) - BKXB415
        /// </summary>
        /// <param name="codEstabelecimento">Código estabelecimento</param>
        /// <param name="codProcesso">Código processo</param>
        /// <param name="origem">Origem</param>
        /// <param name="booFim">boolean fim</param>
        /// <param name="qtdOcorrencias">Quantidade ocorrencias</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Código ocorrência</param>
        /// <param name="codigoRetorno">Código Retorno</param>
        /// <returns>Retorna lista de objetos RecebimentoCV</returns>
        public List<Modelo.RecebimentoCV> RecebimentoCV(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            ref Boolean booFim,
            ref Int16 qtdOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Recebimento de Comprovante de Vendas [BKXB415/XBS415]"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new { codEstabelecimento, codProcesso, 
                    origem, booFim, qtdOcorrencias, filler, codigoOcorrencia });

                try
                {
                    //Preenchimento de parâmetros                
                    String transacao = "IS66";
                    Int16 varTemReg = (Int16)(booFim ? 1 : 0);

                    //Consulta retornando os CVs
                    List<Modelo.RecebimentoCV> retorno = new Agentes.ChargebackXBAG().RecebimentoCV(
                        codEstabelecimento,
                        codProcesso,
                        out varTemReg,
                        out qtdOcorrencias,
                        out filler,
                        origem,
                        transacao,
                        out codigoRetorno);

                    //Filtra os registros que possuem dados (qtdOcorrencias possui a quantidade de registros retornados na consulta)
                    if (retorno != null && retorno.Count > 0)
                        retorno = retorno.Take(qtdOcorrencias).ToList();

                    //Não retorna lista nula, e sim, lista vazia
                    if (retorno == null)
                        retorno = new List<Modelo.RecebimentoCV>();

                    log.GravarLog(EventoLog.RetornoNegocio, retorno);

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
        public List<Modelo.RecebimentoCV> ConsultarLogRecDoc(
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

                log.GravarLog(EventoLog.InicioNegocio, new { numeroProcesso, codEstabelecimento, origem, transacao });

                try
                {

                    //Retorna histórico de comprovantes
                    List<Modelo.RecebimentoCV> retorno = new Agentes.ChargebackXBAG().ConsultaLogRespostasEstabelecimentos(
                        numeroProcesso,
                        codEstabelecimento,
                        origem,
                        transacao,
                        out codRetorno,
                        out msgRetorno,
                        out qtdOcorrencias);

                    //Filtra os registros vazios
                    if (retorno != null && retorno.Count > 0)
                        retorno = retorno.Where(item => item.CodigoRecebimento != 0).ToList();

                    //Não retorna lista nula, e sim, lista vazia
                    if (retorno == null)
                        retorno = new List<Modelo.RecebimentoCV>();

                    log.GravarLog(EventoLog.RetornoNegocio, retorno);

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
        /// <param name="booFim">boolean fim da busca</param>
        /// <param name="codUltimoProcesso">Código último processo</param>
        /// <param name="tipoUltimo">Tipo último</param>
        /// <param name="codUltimoCiclo">Código Último Ciclo</param>
        /// <param name="codUltimaSequencia">Código Última Sequência</param>
        /// <param name="qtdOcorrencias">Quantidade ocorrências</param>
        /// <param name="filler">Filler</param>
        /// /// <param name="codigoOcorrencia">codigo ocorrência</param>
        /// <param name="codigoRetorno">Código retorno</param>
        /// <returns>Retorna lista de débitos pendentes</returns>
        public List<Modelo.AvisoDebito> ConsultarDebitoPendente(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            Int16 tipo,
            Int16 codCiclo,
            Int16 codSequencia,
            String programa,
            String origem,
            ref Boolean booFim,
            ref Decimal codUltimoProcesso,
            ref Int16 tipoUltimo,
            ref Int16 codUltimoCiclo,
            ref Int16 codUltimaSequencia,
            ref Int16 qtdOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Passar os débitos pendentes para o estabelecimento [BKXB416/XBS416]"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new { codEstabelecimento, codProcesso, tipo, 
                    codCiclo, codSequencia, programa, origem, booFim, codUltimoProcesso, tipoUltimo, 
                    codUltimoCiclo, codUltimaSequencia, qtdOcorrencias, filler, codigoOcorrencia });

                try
                {
                    //Preenchimento de parâmetros para chamada do agente                
                    Int16 temReg = (Int16)(booFim ? 1 : 0);
                    String transacao;
                    switch (programa)
                    {
                        case "DEBITO": transacao = "IS67"; break;
                        case "REQUESTPENDENTE": transacao = "IS68"; break;
                        case "REQUESTHISTORICO": transacao = "IS39"; break;
                        default: transacao = "IS67"; break;
                    }


                    List<Modelo.AvisoDebito> itens = new Agentes.ChargebackXBAG().ConsultarDebitoPendente(
                        codEstabelecimento,
                        codProcesso,
                        tipo,
                        codCiclo,
                        codSequencia,
                        origem,
                        transacao,
                        out temReg,
                        out codUltimoProcesso,
                        out tipoUltimo,
                        out codUltimoCiclo,
                        out codUltimaSequencia,
                        out qtdOcorrencias,
                        out filler,
                        out codigoRetorno);

                    //Filtra os registros vazios
                    if (itens != null && itens.Count > 0)
                        itens = itens.Where(item => item.Centralizadora > 0).ToList();

                    //Não retorna lista nula, e sim, lista vazia
                    if (itens == null)
                        itens = new List<Modelo.AvisoDebito>();

                    //"igual 0" : não tem mais registros, "igual 1" : tem mais registros
                    booFim = (temReg == 1);

                    log.GravarLog(EventoLog.RetornoNegocio, itens);

                    return itens;
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

        //BXA780
        /// <summary>
        /// Histórico de comprovantes do request - BKXB417
        /// </summary>
        /// <param name="codEstabelecimento">Código estabelecimento</param>
        /// <param name="codProcesso">Código processo</param>
        /// <param name="tipo">Tipo</param>
        /// <param name="codCiclo">Código ciclo</param>
        /// <param name="codSequencia">Código sequência</param>
        /// <param name="programa">Programa</param>
        /// <param name="dataIni">Data inicio</param>
        /// <param name="dataFim">Data fim</param>
        /// <param name="origem">Origem</param>
        /// <param name="booFim">Boolean fim</param>
        /// <param name="codUltimoProcesso">Código último processo</param>
        /// <param name="tipoUltimo">Tipo último</param>
        /// <param name="codUltimoCiclo">Código último ciclo</param>
        /// <param name="codUltimaSequencia">Código última Sequência</param>
        /// <param name="qtdOcorrencias">Quantidade ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Código ocorrência</param>
        /// <param name="codigoRetorno">Código retorno</param>
        /// <returns>Lista de objetos de Comprovante</returns>
        public List<Modelo.Comprovante> HistoricoRequest(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            Int16 tipo,
            Int16 codCiclo,
            Int16 codSequencia,
            String programa,
            DateTime dataIni,
            DateTime dataFim,
            String origem,
            ref Boolean booFim,
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

                log.GravarLog(EventoLog.InicioNegocio, new { codEstabelecimento, codProcesso, tipo, codCiclo, codSequencia, 
                    programa, dataIni, dataFim, origem, booFim, ultimoRegistro = codUltimoProcesso, tipoUltimo, codUltimoCiclo, 
                    codUltimaSequencia, qtdOcorrencias, filler, codigoOcorrencia });

                try
                {
                    String transacao = default(String);

                    //Identifica o valor do parâmetro transação que será enviado ao serviço mainframe
                    switch (programa)
                    {
                        case "DEBITO": transacao = "IS67"; break;
                        case "REQUESTPENDENTE": transacao = "IS68"; break;
                        case "REQUESTHISTORICO":
                            if (codProcesso != 0)
                                transacao = "IS67";
                            else
                                transacao = "IS39";
                            break;
                        default: transacao = "IS39"; break;
                    }

                    Int16 varTemReg = (Int16)(booFim ? 1 : 0);

                    //Retorna o histórico dos Requests
                    List<Modelo.Comprovante> requests = new Agentes.ChargebackXBAG().HistoricoRequest(
                        codEstabelecimento,
                        dataIni,
                        dataFim,
                        codProcesso,
                        tipo,
                        codCiclo,
                        codSequencia,
                        origem,
                        transacao,
                        ref varTemReg,
                        ref codUltimoProcesso,
                        ref tipoUltimo,
                        ref codUltimoCiclo,
                        ref codUltimaSequencia,
                        ref qtdOcorrencias,
                        ref filler,
                        ref codigoOcorrencia,
                        out codigoRetorno);

                    //Se "0": Não tem mais registros; Se "1": tem mais registros
                    booFim = (varTemReg == 1);

                    //Filtra os registros vazios
                    if (requests != null && requests.Count > 0)
                        requests = requests.Where(request => request.Centralizadora > 0).ToList();

                    //Não retorna lista nula, e sim, lista vazia
                    if (requests == null)
                        requests = new List<Modelo.Comprovante>();

                    log.GravarLog(EventoLog.RetornoNegocio, requests);

                    return requests;
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

        //XA790
        /// <summary>
        /// Consulta request pendente
        /// </summary>
        /// <param name="codEstabelecimento">Código estabelecimento</param>
        /// <param name="codProcesso">Código processo</param>
        /// <param name="tipo">Tipo</param>
        /// <param name="codCiclo">Código ciclo</param>
        /// <param name="codSequencia">Código sequência</param>
        /// <param name="programa">Programa</param>
        /// <param name="origem">Origem</param>
        /// <param name="possuiMaisRegistros">Possui mais registros</param>
        /// <param name="codUltimoReg">[RECHAMADA] Código do último registro</param>
        /// <param name="tipoUltimo">[RECHAMADA] Tipo último registro</param>
        /// <param name="codUltimoCiclo">[RECHAMADA] Código último ciclo</param>
        /// <param name="codUltimaSequencia">[RECHAMADA] Código última sequência</param>
        /// <param name="qtdLinhasOcorrencia">Quantidade de ocorrências</param>
        /// <param name="qtdTotalOcorrencias">Quantidadade total de ocorrências</param>
        /// <param name="filler">Filler</param>
        /// <param name="codigoOcorrencia">Código de ocorrência de erro</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Comprovante</returns>
        public List<Modelo.Comprovante> ConsultarRequestPendente(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            Int16 tipo,
            Int16 codCiclo,
            Int16 codSequencia,
            String programa,
            String origem,
            ref Boolean possuiMaisRegistros,
            ref Decimal codUltimoReg,
            ref Int16 tipoUltimo,
            ref Int16 codUltimoCiclo,
            ref Int16 codUltimaSequencia,
            ref Int16 qtdLinhasOcorrencia,
            ref Int32 qtdTotalOcorrencias,
            ref String filler,
            ref Int64 codigoOcorrencia,
            out Int32 codigoRetorno)
        {

            using (Logger log = Logger.IniciarLog("Consulta Motivo de Débito [BKXB411/XBS411]"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new { codEstabelecimento, codProcesso, tipo, codCiclo, 
                    codSequencia, programa, origem, possuiMaisRegistros, codUltimoReg, tipoUltimo, codUltimoCiclo, 
                    codUltimaSequencia, qtdLinhasOcorrencia, qtdTotalOcorrencias, filler, codigoOcorrencia });

                try
                {
                    String transacao = default(String);
                    Int16 temReg = (short)(possuiMaisRegistros ? 1 : 0);

                    switch (programa)
                    {
                        case "DEBITO": transacao = "IS67"; break;
                        case "REQUESTPENDENTE": transacao = "IS68"; break;
                        case "REQUESTHISTORICO": transacao = "IS39"; break;
                        default: transacao = "IS68"; break;
                    }

                    // Retorna lista de requests pendentes
                    List<Modelo.Comprovante> requests = new Agentes.ChargebackXBAG().ConsultarRequestPendente(
                        codEstabelecimento,
                        codProcesso,
                        tipo,
                        codCiclo,
                        codSequencia,
                        origem,
                        transacao,
                        ref temReg,
                        ref codUltimoReg,
                        ref tipoUltimo,
                        ref codUltimoCiclo,
                        ref codUltimaSequencia,
                        ref qtdLinhasOcorrencia,
                        ref qtdTotalOcorrencias,
                        ref filler,
                        out codigoRetorno);

                    //"igual 0" : não tem mais registros, "igual 1" : tem mais registros
                    possuiMaisRegistros = (temReg == 1);

                    //Remove os requests em branco do retorno (Centralizadora diferente de 0)
                    if (requests != null && requests.Count > 0)
                        requests = requests.Where(req => req.Centralizadora != 0).ToList();

                    //Não retorna lista nula, e sim, lista vazia
                    if (requests == null)
                        requests = new List<Modelo.Comprovante>();

                    log.GravarLog(EventoLog.RetornoNegocio, requests);

                    return requests;
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

        //XD Com mudanças
        //XD201

        /// <summary>
        /// Consulta comprovante solicitacao pendente de processos - BKXB419
        /// </summary>
        /// <param name="codEstabelecimento">Código do estabelecimento</param>
        /// <param name="dataInicio">Data de ínicio</param>
        /// <param name="dataFim">Data de término</param>
        /// <param name="sistemaOrigem">Sistema de origem</param>
        /// <param name="codigoTransacao">Código transação</param>
        /// <param name="indicadorPesquisa">Indicador de pesquisa</param>
        /// <param name="numeroProcessoI">[RECHAMADA] Número processo I</param>
        /// <param name="tpProcessoI">[RECHAMADA] Tipo processo I</param>
        /// <param name="clProcessoI">[RECHAMADA] Ciclo processo I</param>
        /// <param name="codigoSequenciaI">[RECHAMADA] Código sequência I</param>
        /// <param name="dtEmissaoI">[RECHAMADA] Data emissão I</param>
        /// <param name="numeroProcessoF">[RECHAMADA] Número processo F</param>
        /// <param name="tpProcessoF">[RECHAMADA] Tipo processo F</param>
        /// <param name="clProcessoF">[RECHAMADA] Ciclo processo F</param>
        /// <param name="codigoSequenciaF">[RECHAMADA] Código sequência F</param>
        /// <param name="dtEmissaoF">[RECHAMADA] Data emissão F</param>
        /// <param name="indCont">Indicador continuação</param>
        /// <param name="qtdOcorrencias">Quantidade ocorrências</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="msgRetorno">Mensagem de retorno</param>
        /// <returns>Comprovantes</returns>
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
            out Int16 qtdOcorrencias,
            out Int16 codigoRetorno,
            out String msgRetorno)
        {
            using (Logger log = Logger.IniciarLog("Consulta comprovante solicitacao pendente de processos [BKXB419/XBS419]"))
            {

                log.GravarLog(EventoLog.InicioNegocio, new { codEstabelecimento, dataInicio, dataFim, sistemaOrigem, 
                    codigoTransacao, indicadorPesquisa, numeroProcessoI, tpProcessoI, clProcessoI, codigoSequenciaI, 
                    dtEmissaoI, numeroProcessoF, tpProcessoF, clProcessoF, codigoSequenciaF, dtEmissaoF, indCont });

                try
                {
                    //Instancia a classe agente
                    var agente = new Agentes.ChargebackXBAG();

                    //Retorna comprovantes pendentes
                    List<Modelo.Comprovante> comprovantes = agente.ConsultaSolicitacaoPendente(
                        codEstabelecimento,
                        dataInicio,
                        dataFim,
                        sistemaOrigem,
                        codigoTransacao,
                        indicadorPesquisa,
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
                        out qtdOcorrencias);

                    //Filtra os registros vazios (PV igual a zero são desconsiderados)
                    if (comprovantes != null && comprovantes.Count > 0)
                        comprovantes = comprovantes.Where(comprovante => comprovante.PontoVenda != 0).ToList();

                    //Não retorna lista nula, e sim, lista vazia
                    if (comprovantes == null)
                        comprovantes = new List<Modelo.Comprovante>();

                    log.GravarLog(EventoLog.RetornoNegocio, comprovantes);

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

        //XD203

        /// <summary>
        /// Consulta Histórico de Solicitações
        /// </summary>
        /// <param name="numeroProcesso">Número do processo</param>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="dataInicio">Data de início</param>
        /// <param name="dataFim">Data fim</param>
        /// <param name="origem">Origem</param>
        /// <param name="transacao">Transação</param>
        /// <param name="indicePesquisa">Índice pesquisa</param>
        /// <param name="numeroProcessoI">[RECHAMADA] Número do processo I</param>
        /// <param name="tipoProcessoI">[RECHAMADA] Tipo do processo I</param>
        /// <param name="cicloProcessoI">[RECHAMADA] Ciclo do processo I</param>
        /// <param name="codigoSequenciaI">[RECHAMADA] Código de sequência I</param>
        /// <param name="numeroPVI">[RECHAMADA] Número do PV I</param>
        /// <param name="dataEmissaoI">[RECHAMADA] Data de emissão I</param>
        /// <param name="numeroProcessoF">[RECHAMADA] Número de processo F</param>
        /// <param name="tipoProcessoF">[RECHAMADA] Tipo do processo F</param>
        /// <param name="cicloProcessoF">[RECHAMADA] Ciclo processo F</param>
        /// <param name="codigoSequenciaF">[RECHAMADA] Código sequencia F</param>
        /// <param name="numeroPVF">[RECHAMADA] Número do PV</param>
        /// <param name="dataEmissaoF">[RECHAMADA] Data emissão F</param>
        /// <param name="indCont">[RECHAMADA] Indicador continuidade</param>
        /// <param name="qtdOcorrencias">[RECHAMADA] Quantidade ocorrências</param>
        /// <param name="codigoRetorno">[RECHAMADA] Código do retorno</param>
        /// <param name="msgRetorno">[RECHAMADA] Mensagem de retorno</param>
        /// <returns></returns>
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
            ref Int16 codigoSequenciaI,
            ref Int32 numeroPVI,
            ref String dataEmissaoI,
            ref Decimal numeroProcessoF,
            ref Int16 tipoProcessoF,
            ref Int16 cicloProcessoF,
            ref Int16 codigoSequenciaF,
            ref Int32 numeroPVF,
            ref String dataEmissaoF,
            ref String indCont,
            out Int16 qtdOcorrencias,
            out Int16 codigoRetorno,
            out String msgRetorno)
        {

            using (Logger log = Logger.IniciarLog("Consulta histórico de solicitações [BKXB420/XBS420]"))
            {

                log.GravarLog(EventoLog.InicioNegocio, new { numeroProcesso, numeroPV, dataInicio, dataFim, 
                    origem, transacao, indicePesquisa, numeroProcessoI, tipoProcessoI, cicloProcessoI, 
                    codigoSequenciaI, numeroPVI, dataEmissaoI, numeroProcessoF, tipoProcessoF, cicloProcessoF, 
                    codigoSequenciaF, numeroPVF, dataEmissaoF, indCont });

                try
                {
                    //Retorna histórico de comprovantes
                    List<Modelo.Comprovante> itens = new Agentes.ChargebackXBAG().ConsultarHistoricoSolicitacoes(
                        numeroProcesso,
                        numeroPV,
                        dataInicio,
                        dataFim,
                        origem,
                        transacao,
                        indicePesquisa,
                        ref numeroProcessoI,
                        ref tipoProcessoI,
                        ref cicloProcessoI,
                        ref codigoSequenciaI,
                        ref numeroPVI,
                        ref dataEmissaoI,
                        ref numeroProcessoF,
                        ref tipoProcessoF,
                        ref cicloProcessoF,
                        ref codigoSequenciaF,
                        ref numeroPVF,
                        ref dataEmissaoF,
                        ref indCont,
                        out codigoRetorno,
                        out msgRetorno,
                        out qtdOcorrencias);

                    //Filtra os registros vazios
                    if (itens != null && itens.Count > 0)
                        itens = itens.Where(comprovante => comprovante.PontoVenda != 0).ToList();

                    //Não retorna lista nula, e sim, lista vazia
                    if (itens == null)
                        itens = new List<Modelo.Comprovante>();

                    log.GravarLog(EventoLog.RetornoNegocio, itens);

                    return itens;
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


        //XD204
        /// <summary>
        /// Consultar avisos de débito
        /// </summary>
        /// <param name="codProcesso">Código do processo</param>
        /// <param name="codEstabelecimento">Código do estabelecimento</param>
        /// <param name="dataIni">Data início</param>
        /// <param name="dataFim">Data final</param>
        /// <param name="origem">Origem</param>
        /// <param name="transacao">Transação</param>
        /// <param name="indicadorPesquisa">Indicador pesquisa</param>
        /// <param name="numeroProcessoI">[RECHAMADA] Número processo I</param>
        /// <param name="tipoProcessoI">[RECHAMADA] Tipo do processo I</param>
        /// <param name="cicloProcessoI">[RECHAMADA] Ciclo processo I</param>
        /// <param name="codigoSequenciaI">[RECHAMADA] Código sequencial I</param>
        /// <param name="numeroPVI">[RECHAMADA] Número PV I</param>
        /// <param name="dtEmissaoI">[RECHAMADA] Data emissão I</param>
        /// <param name="numeroProcessoF">[RECHAMADA] Número processo F</param>
        /// <param name="tipoProcessoF">[RECHAMADA] Tipo processo F</param>
        /// <param name="cicloProcessoF">[RECHAMADA] Ciclo processo F</param>
        /// <param name="codigoSequenciaF">[RECHAMADA] Código sequencial F</param>
        /// <param name="numeroPVF">[RECHAMADA] Número PV F</param>
        /// <param name="dtEmissaoF">[RECHAMADA] Data emissão F</param>
        /// <param name="indicadorContinua">Indicador continuação</param>
        /// <param name="qtdOcorrencias">Quantidade de ocorrências</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Avisos de débito</returns>
        public List<Modelo.AvisoDebito> ConsultarAvisosDebito(
            Decimal codProcesso,
            Int32 codEstabelecimento,
            DateTime dataIni,
            DateTime dataFim,
            String origem,
            String transacao,
            String indicadorPesquisa,
            ref Decimal numeroProcessoI,
            ref String tipoProcessoI,
            ref Int16 cicloProcessoI,
            ref Int16 codigoSequenciaI,
            ref Int32 numeroPVI,
            ref String dtEmissaoI,
            ref Decimal numeroProcessoF,
            ref Int16 tipoProcessoF,
            ref Int32 cicloProcessoF,
            ref Int16 codigoSequenciaF,
            ref Int32 numeroPVF,
            ref String dtEmissaoF,
            ref String indicadorContinua,
            out Int16 qtdOcorrencias,
            out Int32 codigoRetorno)
        {

            using (Logger log = Logger.IniciarLog("Consulta aviso de debitos [BKXB421/XBS421]"))
            {

                log.GravarLog(EventoLog.InicioNegocio, new { codProcesso, codEstabelecimento, dataIni, dataFim, 
                    origem, transacao, indicadorPesquisa, numeroProcessoI, tipoProcessoI, cicloProcessoI, 
                    codigoSequenciaI, numeroPVI, dtEmissaoI, numeroProcessoF, tipoProcessoF, cicloProcessoF, 
                    codigoSequenciaF, numeroPVF, dtEmissaoF, indicadorContinua });

                try
                {
                    //Instanciação da classe agente
                    var agente = new Agentes.ChargebackXBAG();

                    //Retorna dados
                    List<Modelo.AvisoDebito> avisos = agente.ConsultaAvisoDebito(
                        codProcesso,
                        codEstabelecimento,
                        dataIni,
                        dataFim,
                        origem,
                        transacao,
                        ref indicadorPesquisa,
                        ref numeroProcessoI,
                        ref tipoProcessoI,
                        ref cicloProcessoI,
                        ref codigoSequenciaI,
                        ref numeroPVI,
                        ref dtEmissaoI,
                        ref numeroProcessoF,
                        ref tipoProcessoF,
                        ref cicloProcessoF,
                        ref codigoSequenciaF,
                        ref numeroPVF,
                        ref dtEmissaoF,
                        ref indicadorContinua,
                        out codigoRetorno,
                        out qtdOcorrencias);

                    //Filtra os avisos de débito vazios (ponto de venda = 0)
                    if (avisos != null || avisos.Count == 0)
                        avisos = avisos.Where(aviso => aviso.PontoVenda != 0).ToList();

                    //Não retorna lista nula, e sim, lista vazia
                    if (avisos == null)
                        avisos = new List<Modelo.AvisoDebito>();

                    log.GravarLog(EventoLog.RetornoNegocio, avisos);

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
    }
}