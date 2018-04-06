using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Redecard.PN.Comum;
using AutoMapper;
using System.Configuration;

namespace Redecard.PN.GerencieExtrato.Servicos
{

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GerencieExtrato" in code, svc and config file together.
    public class GerencieExtrato : ServicoBase, IGerencieExtrato
    {
        /// <summary>
        /// Lista extratos emitidos
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA740 / Programa WA740 / TranID IS27
        /// </remarks>
        /// <param name="codigoEstabelecimento">Código do estabelecimento.</param>
        /// <param name="numeroExtrato">Número do Extrato</param>
        /// <param name="totalRegistros">Total de registros</param>
        /// <param name="ts_reg">Campo da área operacionaç</param>
        /// <param name="mensagem">Descrição do código de retorno</param>
        /// <param name="qtdOcorrencias">Quantidade de extratos/ocorrências</param>
        /// <param name="codigoRetorno">codigo de retorno, quando diferente de zero informa que ocorreu um erro durante a execução do serviço.</param>
        /// <returns>Retona lista de extratos emitidos.</returns>
        public List<ExtratoEmitido> ListaExtratos(Int32 codigoEstabelecimento, Int32 numeroExtrato, ref Int16 totalRegistros, ref Int16 ts_reg, ref String mensagem, ref Int16 qtdOcorrencias, ref Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Lista extratos emitidos [WACA740]"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    String programa = String.Empty;
                    Log.GravarLog(EventoLog.ChamadaAgente, new
                    {
                        programa,
                        codigoEstabelecimento,
                        numeroExtrato,
                        totalRegistros,
                        ts_reg,
                        mensagem,
                        qtdOcorrencias,
                        codigoRetorno
                    });


                    List<Modelo.ExtratoEmitido> lista = new Negocio.GerencieExtrato().ListaExtratos(programa,
                            codigoEstabelecimento, numeroExtrato,
                            ref totalRegistros,
                            ref ts_reg,
                            ref mensagem,
                            ref qtdOcorrencias,
                            ref codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoAgente, new
                    {
                        codigoRetorno,
                        mensagem,
                        totalRegistros,
                        ts_reg,
                        qtdOcorrencias,
                    });

                    List<ExtratoEmitido> result = this.PreenceModelo(lista);
                    Log.GravarLog(EventoLog.FimServico, new { result });
                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Consulta os dados do extrato solicitado.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA742 / Programa WA742 / TranID IS28
        /// </remarks>
        /// <param name="codigoEstabelecimento">Código do estabelecimento.</param>
        /// <param name="numeroExtrato">Número do Extrato</param>
        /// <param name="tipoAcesso">Pode ser :<br/>
        /// <b>AVANCA</b>:Retornará o próximo registro existênte do mesmo extrato, quando for a primeira chamada sera retornado o registro informado  na chave de entrada.<br/>
        /// <b>VOLTA</b>:Retornará o próximo registro anterior existente no extratos esta opção so é processada quando se fez outras chamadas  anterior com opção "avanca".<br/>
        /// </param>
        /// <param name="codigoRetorno">codigo de retorno, quando diferente de zero informa que ocorreu um erro durante a execução do serviço.</param>
        /// <param name="mensagem">Descrição do código de retorno</param>
        /// <param name="sequencia">Informar somente quando tipoacesso = vazio</param>
        /// <returns></returns>
        public List<Extrato> ConsultarExtrato(
            ref Int32 codigoEstabelecimento,
            ref Int32 numeroExtrato,
            ref String tipoAcesso,
            ref Int16 codigoRetorno,
            ref String mensagem,
            ref Int32 sequencia)
        {

            String programa = "IS";
            short linhaTela = 0;
            short linhaAtual = 0;
            short linhaInicial = 0;
            short linhaFinal = 0;
            short coluna = 0;
            String chaveAnterior = string.Empty;
            int quantidadeLinhas = 0;
            short totalRegistros = 0;
            short registro = 0;
            String filler = string.Empty;
            String temMaisRegistros = string.Empty;
            String quantidadeOcorrencias = "0";
            Int32 maxSequencial = ConfigurationManager.AppSettings["MaxSequencial"].ToString().ToInt32();

            using (Logger Log = Logger.IniciarLog("Consulta extrato [WACA742]"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    List<Modelo.Extrato> lista = new List<Modelo.Extrato>();
                    temMaisRegistros = "N";

                    Log.GravarLog(EventoLog.ChamadaAgente, new
                    {
                        programa,
                        codigoEstabelecimento,
                        numeroExtrato,
                        sequencia,
                        tipoAcesso,
                        linhaTela,
                        linhaAtual,
                        linhaInicial,
                        linhaFinal,
                        coluna,
                        chaveAnterior,
                        quantidadeLinhas,
                        totalRegistros,
                        registro,
                        filler,
                        mensagem,
                        codigoRetorno,
                        temMaisRegistros,
                        quantidadeOcorrencias
                    });


                    List<Modelo.Extrato> listaAux = new Negocio.GerencieExtrato().ConsultarExtrato(ref  programa,
                            ref  codigoEstabelecimento,
                            ref  numeroExtrato, ref  sequencia, ref  tipoAcesso,
                            ref  linhaTela, ref  linhaAtual, ref  linhaInicial, ref  linhaFinal,
                            ref  coluna, ref  chaveAnterior, ref  quantidadeLinhas,
                            ref  totalRegistros, ref  registro, ref  filler, ref  mensagem,
                            ref  codigoRetorno, ref  temMaisRegistros, ref  quantidadeOcorrencias);
                    lista.AddRange(listaAux);

                    Int32 cnt = 1;
                    //se existe mais registro pesquisa
                    while (temMaisRegistros.Equals("S") && cnt <= maxSequencial)
                    {
                        listaAux = new Negocio.GerencieExtrato().ConsultarExtrato(ref  programa,
                                ref  codigoEstabelecimento,
                                ref  numeroExtrato, ref  sequencia, ref  tipoAcesso,
                                ref  linhaTela, ref  linhaAtual, ref  linhaInicial, ref  linhaFinal,
                                ref  coluna, ref  chaveAnterior, ref  quantidadeLinhas,
                                ref  totalRegistros, ref  registro, ref  filler, ref  mensagem,
                                ref  codigoRetorno, ref  temMaisRegistros, ref  quantidadeOcorrencias);
                        lista.AddRange(listaAux);
                        cnt++;
                    }
                    if (cnt > maxSequencial)
                    {
                        throw new PortalRedecardException(50514, "GerencieExtratoServico.ConsultarExtrato");
                    }
                    Log.GravarLog(EventoLog.RetornoAgente, new { codigoRetorno, mensagem, lista });

                    List<Extrato> result = this.PreenceModelo(lista);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Obter status por pv da emissao do extrato papel
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BWA364 / Programa WA364 / TranID IS19
        /// </remarks>
        /// <param name="solicita">Lista de Status de Emissão dos Pv´s</param>
        /// <param name="codigoRetorno">codigo de retorno, quando diferente de zero informa que ocorreu um erro durante a execução do serviço.</param>
        /// <param name="mensagemRetorno">Descrição do código de retorno</param>
        public void ObterExtratoPapel(ref List<StatusEmissao> solicita, ref short codigoRetorno, ref String mensagemRetorno)
        {
            string sistema = "IS";

            int qtdPular = 200;
            int atual = 0;

            List<StatusEmissao> dadosNovo = new List<StatusEmissao>();
            using (Logger Log = Logger.IniciarLog("Obter status por pv da emissao do extrato papel [BWA364]"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { solicita });
                try
                {

                    while (atual < solicita.Count)
                    {
                        var suaLista = (from t0 in solicita select t0).Skip(atual).Take(qtdPular).ToList();
                        atual += qtdPular;
                        //Popula a lista até completar 200 itens
                        while (suaLista.Count < qtdPular)
                        {
                            var i = new Servicos.StatusEmissao();
                            i.PontoVenda = "0";
                            i.SituacaoCobranca = " ";
                            i.Status = " ";
                            i.CodigoRetornoPV = 0;

                            suaLista.Add(i);
                        }

                        Mapper.CreateMap<Servicos.StatusEmissao, Modelo.StatusEmissao>();

                        var novaLista = suaLista.ConvertAll<Modelo.StatusEmissao>(x => Mapper.Map<Modelo.StatusEmissao>(x));
                        Log.GravarLog(EventoLog.ChamadaAgente, new {   sistema,  novaLista,  codigoRetorno,  mensagemRetorno });

                        new Negocio.GerencieExtrato().ObterExtratoPapel(ref  sistema, ref novaLista, ref  codigoRetorno, ref  mensagemRetorno);

                        Log.GravarLog(EventoLog.RetornoAgente, new { sistema, novaLista, codigoRetorno, mensagemRetorno });

                        Mapper.CreateMap<Modelo.StatusEmissao, Servicos.StatusEmissao>();

                        dadosNovo.AddRange(novaLista.ConvertAll<Servicos.StatusEmissao>(x => Mapper.Map<Servicos.StatusEmissao>(x)).Where(x => x.PontoVenda.ToInt32() > 0));

                    }

                    solicita = dadosNovo;

                    Log.GravarLog(EventoLog.FimServico, new { solicita });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Efetua consulta/modificação/exclusão de recebimento de extrato por email.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WA453CA / Programa WA453 / TranID IS62
        /// </remarks>
        /// <param name="tipo">
        /// <list type="table">
        /// <listheader>Tipo da operação cadastral:</listheader>
        /// <item>
        /// <term>"I"</term>
        /// <description>Inclusão</description>
        /// </item>
        /// <item><term>"A"</term><description>Alteração</description></item> 
        /// <item><term>"E"</term><description>Exclusão</description></item> 
        /// <item><term>"C"</term><description>Consulta</description></item> 
        /// </list>
        /// </param>
        /// <param name="codigoEstabelecimento">Código do estabelecimento solicitante.</param>
        /// <param name="cnpjSolicitante">CNPJ do Solicitante</param>
        /// <param name="periodicidadeEnvio">
        ///  <list type="table">
        /// <listheader>Periodicidade do envio</listheader>
        /// <item><term>'D'</term><description>Diaria</description></item> 
        /// <item><term>'S'</term><description>Semanal</description></item> 
        /// <item><term>'Q'</term><description>Quinzenal</description></item> 
        ///</list>
        /// </param>
        /// <param name="diaEnvio">
        /// <list type="bullet">
        /// <listheader>Dia do envio (se semanal)</listheader>
        /// <item><description>'DOM'</description></item> 
        /// <item><description>'SEG'</description></item> 
        /// <item><description>'TER'</description></item> 
        /// <item><description>'QUA'</description></item> 
        /// <item><description>'QUI'</description></item> 
        /// <item><description>'SEX'</description></item> 
        /// <item><description>'SAB'</description></item> </list>
        /// </param><list type="table"><listheader>Tipo de PV</listheader>
        /// <item><term>'0'</term><description>Proprio</description></item> 
        /// <item><term>'1'</term><description>Centralizadora</description></item> 
        /// <item><term>'2</term><description> Matriz</description></item> 
        /// <item><term>'3'</term><description>Consignadora</description></item> 
        /// <item><term>'4'</term><description>Mesmo CNPJ</description></item> 
        /// </list></param>
        /// <param name="tipoSolicitacao">
        /// <list type="table"><listheader>Tipo de solicitacao :</listheader>
        /// <item><term>'T'</term><description>Total</description></item> 
        /// <item><term>'P'</term><description>Parcial</description></item> 
        /// </list></param>
        /// <param name="nomeUsuario">Nome do usuario</param>
        /// <param name="nomeEmailrecebimento">Nome do e-mail</param>
        /// <param name="fraseCriptografada">Frase criptografada</param>
        /// <param name="lstCadeia">PV da cadeia (TABELA C/ 1200)</param>
        /// <param name="codigoBoxes">Codigo dos boxes</param>
        /// <param name="codigoRetorno">codigo de retorno, quando diferente de zero informa que ocorreu um erro durante a execução do serviço.</param>
        /// <param name="mensagemRetorno">Mensagem de retorno, informa o erro que ocorreu durante a execução do serviço.</param>
        /// <param name="quantidadePvs">Se consulta, quantidade de itens retornados. <br/> Se inclusao ou alteracao, quantidade de PVS incluidos</param>
        /// <param name="identificadorContinuacao">"S" Existem mais dados / "N" nao existem mais dados</param>
        /// <param name="acao"></param>
        public void Extrato_Email(ref String tipo, ref int codigoEstabelecimento, ref decimal cnpjSolicitante,
            ref String periodicidadeEnvio, ref String diaEnvio, ref String tipoPVSolicitante, ref String tipoSolicitacao,
            ref String nomeUsuario, ref String nomeEmailrecebimento, ref String fraseCriptografada,
            ref List<CadeiaPV> lstCadeia,
            ref List<String> codigoBoxes,
            ref Int32 codigoRetorno,
            ref String mensagemRetorno,
            ref String quantidadePvs,
            ref String identificadorContinuacao, ref String acao)
        {
            using (Logger Log = Logger.IniciarLog("consulta/modificação/exclusão de recebimento de extrato por email.[WA453CA]"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    String codigoErro = string.Empty;
                    String[] codBoxes = new String[30];
                    List<Modelo.CadeiaPV> lista;
                    Mapper.CreateMap<Servicos.CadeiaPV, Modelo.CadeiaPV>();

                    lista = Mapper.Map<List<Servicos.CadeiaPV>, List<Modelo.CadeiaPV>>(lstCadeia);


                    //Completa a lista de itens para enviar para o servidor (Caso contrário, irá retornar erro)
                    while (lista.Count < 1200)
                        lista.Add(new Modelo.CadeiaPV() { NumeroPV = "0" });

                    //Completa a lista de boxes para enviar para o servidor (Caso contrário, irá retornar erro)
                    for (Int16 cnt = 0; cnt < 30; cnt++)
                    {
                        if (codigoBoxes[cnt] != null)
                            codBoxes[cnt] = codigoBoxes[cnt];
                        else
                            codBoxes[cnt] = "0";
                    }
                    Log.GravarLog(EventoLog.ChamadaAgente, new {  tipo, codigoEstabelecimento, cnpjSolicitante, periodicidadeEnvio,
                        diaEnvio, tipoPVSolicitante, tipoSolicitacao, nomeUsuario, nomeEmailrecebimento, fraseCriptografada,
                        lista,codBoxes, codigoErro, quantidadePvs, identificadorContinuacao, acao, mensagemRetorno});

                    new Negocio.GerencieExtrato().Extrato_Email(ref tipo, ref codigoEstabelecimento, ref cnpjSolicitante, ref periodicidadeEnvio,
                        ref diaEnvio, ref tipoPVSolicitante, ref tipoSolicitacao, ref nomeUsuario, ref nomeEmailrecebimento, ref fraseCriptografada,
                        ref lista,
                        ref codBoxes, ref codigoErro, ref quantidadePvs, ref identificadorContinuacao, ref acao, ref mensagemRetorno);

                    Log.GravarLog(EventoLog.RetornoAgente, new
                    {
                        tipo,
                        codigoEstabelecimento,
                        cnpjSolicitante,
                        periodicidadeEnvio,
                        diaEnvio,
                        tipoPVSolicitante,
                        tipoSolicitacao,
                        nomeUsuario,
                        nomeEmailrecebimento,
                        fraseCriptografada,
                        lista,
                        codBoxes,
                        codigoErro,
                        quantidadePvs,
                        identificadorContinuacao,
                        acao,
                        mensagemRetorno
                    });

                    codigoRetorno = codigoErro.ToInt32();
                    codigoBoxes = codBoxes.ToList();
                    Mapper.CreateMap<Modelo.CadeiaPV, Servicos.CadeiaPV>();


                    lstCadeia = Mapper.Map<List<Modelo.CadeiaPV>, List<Servicos.CadeiaPV>>(lista);

                    Log.GravarLog(EventoLog.FimServico, new { lstCadeia });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                      new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BWA367E / Programa WA367 / TranID ISA5
        /// </remarks>
        /// <param name="dados">Lista de DadosPV </param>
        public void InibirExtPapel(ref List<DadosPV> dados)
        {
            String sistema = "IS";

            int qtdPular = 80;
            int atual = 0;

            List<DadosPV> dadosNovo = new List<DadosPV>();

            using (Logger Log = Logger.IniciarLog("Inibir Extrato Papel [BWA367E]"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { dados });
                try
                {

                    while (atual < dados.Count)
                    {
                        var suaLista = (from t0 in dados select t0).Skip(atual).Take(qtdPular).ToList();
                        atual += qtdPular;

                        while (suaLista.Count < 80)
                        {
                            var i = new Servicos.DadosPV();
                            i.PontoVenda = 0;
                            i.Status = "I";
                            i.CodigoOperador = "INTERNET";
                            i.DDD = 0;
                            i.Telefone = 0;
                            i.Solicitante = string.Empty;
                            i.IndicadorSituacaoCobranca = string.Empty;
                            i.TextoIsencaoCobranca = string.Empty;
                            i.CodigoErro = "0";
                            i.MensagemErro = string.Empty;
                            suaLista.Add(i);
                        }

                        Mapper.CreateMap<Servicos.DadosPV, Modelo.DadosPV>();

                        var novaLista = suaLista.ConvertAll<Modelo.DadosPV>(x => Mapper.Map<Modelo.DadosPV>(x));

                        Log.GravarLog(EventoLog.ChamadaAgente, new { sistema, novaLista });

                        new Negocio.GerencieExtrato().InibirExtPapel(ref sistema, ref novaLista);

                        Log.GravarLog(EventoLog.RetornoAgente, new { sistema, novaLista });
      
                        Mapper.CreateMap<Modelo.DadosPV, Servicos.DadosPV>();

                        dadosNovo.AddRange(novaLista.ConvertAll<Servicos.DadosPV>(x => Mapper.Map<Servicos.DadosPV>(x)).Where(x => x.PontoVenda > 0));

                    }
                    dados = dadosNovo;
                    Log.GravarLog(EventoLog.FimAgente, new { dados });

              
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        List<ExtratoEmitido> PreenceModelo(List<Modelo.ExtratoEmitido> lista)
        {
            List<ExtratoEmitido> lstRetorno = new List<ExtratoEmitido>();
            ExtratoEmitido entidade = null;
            foreach (Modelo.ExtratoEmitido item in lista)
            {
                entidade = new ExtratoEmitido();
                entidade.Descricao = item.Descricao;
                entidade.Identificacao = item.Identificacao;
                entidade.Recuperado = item.Recuperado;
                entidade.DataInicial = item.DataInicial;
                entidade.DataFinal = item.DataFinal;
                lstRetorno.Add(entidade);
            }
            return lstRetorno;
        }

        List<Extrato> PreenceModelo(List<Modelo.Extrato> lista)
        {
            List<Extrato> lstRetorno = new List<Extrato>();
            Extrato entidade = null;
            foreach (Modelo.Extrato item in lista)
            {
                entidade = new Extrato();
                entidade.LinhaExtrato = item.LinhaExtrato;
                entidade.NumeroBox = item.NumeroBox;

                lstRetorno.Add(entidade);
            }
            return lstRetorno;
        }

        List<StatusEmissao> PreenceModelo(List<Modelo.StatusEmissao> lista)
        {
            List<StatusEmissao> lstRetorno = new List<StatusEmissao>();
            StatusEmissao entidade = null;
            foreach (Modelo.StatusEmissao item in lista)
            {
                entidade = new StatusEmissao();
                entidade.PontoVenda = item.PontoVenda;
                entidade.SituacaoCobranca = item.SituacaoCobranca;
                entidade.Status = item.Status;
                entidade.CodigoRetornoPV = item.CodigoRetornoPV;

                lstRetorno.Add(entidade);
            }
            return lstRetorno;
        }

        List<CadeiaPV> PreenceModelo(List<Modelo.CadeiaPV> lista)
        {
            List<CadeiaPV> lstRetorno = new List<CadeiaPV>();
            CadeiaPV entidade = null;
            foreach (Modelo.CadeiaPV item in lista)
            {
                entidade = new CadeiaPV();
                entidade.NumeroPV = item.NumeroPV;

                lstRetorno.Add(entidade);
            }
            return lstRetorno;
        }

        /// <summary>
        /// Retorna as informações de relatório de preço único, identificação, descrição e período.<br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKWA2890 / Programa WAC289 / TranID WAGA / Método ConsultarRelatorioPrecoUnico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2890 / Programa WAC289 / TranID WAGA / Método ConsultarRelatorioPrecoUnico
        /// </remarks>
        /// <param name="numeroPv">Número do Ponto de Venda (Número Estabelecimento) que se deseja consultar.</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        /// <returns>Retorna identificação, descrição e período do relatório de Preço Único</returns>  
        public List<ExtratoRelatorioPrecoUnico> ConsultarRelatorioPrecoUnico(
           Int32 numeroPv,
           out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Relatório Preço Único - BKWA2890 / WAC289 / WAGA"))
            {
                log.GravarLog(EventoLog.InicioServico, numeroPv);

                //Declaração de variável de retorno
                var retorno = default(List<ExtratoRelatorioPrecoUnico>);
                var possuiRechamada = default(Boolean);
                var rechamada = default(Dictionary<String,Object>);
                Int32 count = 1;
                var retornoModelo = default(List<Modelo.ExtratoRelatorioPrecoUnico>);
                var listAux = default(List<Modelo.ExtratoRelatorioPrecoUnico>);
                Int32 maxSequencial = ConfigurationManager.AppSettings["MaxSequencial"].ToInt32();           

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.GerencieExtrato();
                    retornoModelo = new List<Modelo.ExtratoRelatorioPrecoUnico>();

                    listAux = negocio.ConsultarRelatorioPrecoUnico(
                        numeroPv, ref rechamada, out possuiRechamada, out codigoRetorno);
                    retornoModelo.AddRange(listAux);

                    while (possuiRechamada && count <= maxSequencial)
                    {
                        listAux = negocio.ConsultarRelatorioPrecoUnico(
                            numeroPv, ref rechamada, out possuiRechamada, out codigoRetorno);
                        retornoModelo.AddRange(listAux);   
                        count++;
                    }

                    //Mapeamento entre classes Modelo de serviço e negócio
                    Mapper.CreateMap<Modelo.ExtratoRelatorioPrecoUnico, ExtratoRelatorioPrecoUnico>();

                    retorno = Mapper.Map<List<ExtratoRelatorioPrecoUnico>>(retornoModelo);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.RetornoServico, new { retorno, codigoRetorno });

                return retorno;
            }
        }

        /// <summary>
        /// Retorna o detalhamento do relatório Preço Único. <br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKWA2900 / Programa WAC290 / TranID WABG / Método DetalharRelatorioPrecoUnico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2900 / Programa WAC290 / TranID WABG / Método DetalharRelatorioPrecoUnico
        /// </remarks>
        /// <param name="numeroPv">Número do Ponto de Venda (Número Estabelecimento) que se deseja consultar.</param>
        /// <param name="mesAnoRelatorio">Mês e ano que se deseja consultar.</param>
        /// <param name="flagVsam">Flag retornado pela consulta de relatório Preço Único</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        /// <returns>Relatório detalhado Preço Único </returns>
        public List<RelatorioDetalhadoPrecoUnico> DetalharRelatorioPrecoUnico(
            Int32 numeroPv,
            DateTime mesAnoRelatorio,
            Int16 flagVsam,
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Relatório Detalhado Preço Único - BKWA2900 / WAC290 / WABG"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv, mesAnoRelatorio, flagVsam });

                //Declaração de variável de retorno
                var retorno = default(List<RelatorioDetalhadoPrecoUnico>);
                var possuiRechamada = default(Boolean);
                var rechamada = default(Dictionary<String,Object>);
                Int32 count = 1;
                var listAux = default(List<Modelo.RelatorioDetalhadoPrecoUnico>);
                var retornoModelo = default(List<Modelo.RelatorioDetalhadoPrecoUnico>);
                Int32 maxSequencial = ConfigurationManager.AppSettings["MaxSequencial"].ToString().ToInt32();  

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.GerencieExtrato();
                    retornoModelo = new List<Modelo.RelatorioDetalhadoPrecoUnico>();

                    listAux =  negocio.DetalharRelatorioPrecoUnico(
                        numeroPv, mesAnoRelatorio, flagVsam, ref rechamada, out possuiRechamada, out codigoRetorno);
                    retornoModelo.AddRange(listAux);

                    while (possuiRechamada && count <= maxSequencial)
                    {
                        listAux = negocio.DetalharRelatorioPrecoUnico(
                            numeroPv, mesAnoRelatorio, flagVsam, ref rechamada, out possuiRechamada, out codigoRetorno);
                        retornoModelo.AddRange(listAux);
                        count++;
                    }

                    //Mapeamento entre classes Modelo de serviço e negócio
                    Mapper.CreateMap<Modelo.RelatorioDetalhadoPrecoUnico, RelatorioDetalhadoPrecoUnico>();

                    retorno = Mapper.Map<List<RelatorioDetalhadoPrecoUnico>>(retornoModelo);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.RetornoServico, new { retorno, codigoRetorno });

                return retorno;
            }
        }

        /// <summary>
        /// Solicita a recuperação de Relatório Preço Único baseado em um período.<br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKWA2910 / Programa WAS291 / TranID WAGC / Método SolicitarRelatorioPrecoUnico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2910 / Programa WAS291 / TranID WAGC / Método SolicitarRelatorioPrecoUnico
        /// </remarks>
        /// <param name="numeroPv">Número do Ponto de Venda (Número Estabelecimento) que se deseja solicitar o relatório.</param>
        /// <param name="mesAnoDe">Mês inicial que relatório deve contemplar.</param>
        /// <param name="mesAnoAte">Mês final até o qual o relatório deve contemplar.</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        public void SolicitarRelatorioPrecoUnico(
            Int32 numeroPv,
            DateTime mesAnoDe,
            DateTime mesAnoAte,
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Solicitar Relatório Preço Único - BKWA2910 / WAS291 / WAGC"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv, mesAnoDe, mesAnoAte });

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.GerencieExtrato();
                    negocio.SolicitarRelatorioPrecoUnico(numeroPv, mesAnoDe, mesAnoAte, out codigoRetorno);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.RetornoServico, codigoRetorno);
            }
        }

    }
}

