/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 18/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Comum;
using Redecard.PN.FMS.Comum.Log;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente.ServicoFMS;
using Redecard.PN.FMS.Agente.Tradutores;
using System.ServiceModel;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Redecard.PN.FMS.Agente
{
    /// <summary>
    /// Este componente publica a classe ServicosFMSAgStub, que expõe métodos para manipular os agentes
    /// </summary>
    public class ServicosFMSAgStub : IServicosFMS
    {
        /// <summary>
        /// Este método é utilizado para contar transações analisadase não analisadas por transação associada.
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> PesquisarTransacoesAnalisadaseNaoAnalisadasPorTransacaoAssociada(long identificadorTransacao, int numeroEmissor, int grupoEntidade, string usuarioLogin, int posicaoPrimeiroRegistro, int quantidadeRegistros, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterio, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            return PesquisarTransacoesPorNumeroCartao("", 4545, 5456, "dsdas", 546545, IndicadorTipoCartao.Ambos, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada.Mcc, OrdemClassificacao.Ascendente).ListaTransacoesEmissor;
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public long ContarTransacoesAnalisadaseNaoAnalisadasPorTransacaoAssociada(long identificadorTransacao, int numeroEmissor, int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoCartao)
        {
            return PesquisarTransacoesPorNumeroCartao("", 4545, 5456, "dsdas", 546545, IndicadorTipoCartao.Ambos, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada.Mcc, OrdemClassificacao.Ascendente).ListaTransacoesEmissor.Count();
        }
        /// <summary>
        /// Este método é utilizado para pesquisar transações analisadas por usuário e período.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> PesquisarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor, int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal, int posicaoPrimeiroRegistro, int quantidadeRegistros, CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo criterio, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            List<TransacaoEmissor> retorno = new List<TransacaoEmissor>();

            for (int x = 0; x < 12; x++)
            {
                retorno.AddRange(PesquisarTransacoesPorNumeroCartao("", 4545, 5456, "dsdas", 546545, IndicadorTipoCartao.Ambos, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada.Mcc, OrdemClassificacao.Ascendente).ListaTransacoesEmissor);
            }

            if (criterio == CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo.Valor)
            {
                retorno.Sort((lhs, rhs) => Comparer<decimal>.Default.Compare(lhs.Valor, rhs.Valor));
            }
            else if (criterio == CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo.NumeroCartao)
            {
                retorno.Sort((lhs, rhs) => Comparer<string>.Default.Compare(lhs.NumeroCartao, rhs.NumeroCartao));
            }


            return retorno.GetRange(posicaoPrimeiroRegistro, quantidadeRegistros); ;
            
        }
        /// <summary>
        /// Este método é utilizado para contar transações analisadas por usuário e período.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public long ContarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor, int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal, IndicadorTipoCartao tipoCartao)
        {
            return PesquisarTransacoesPorSituacaoEPeriodo(56456, 45456, "", SituacaoAnalisePesquisa.Ambos, TipoPeriodo.DataEnvioAnalise, System.DateTime.Now, System.DateTime.Now,
             0,
             (12 * 9),
             CriterioOrdemTransacoesPorSituacaoEPeriodo.LoginUsuario, OrdemClassificacao.Ascendente, IndicadorTipoCartao.Credito).Count();
        }
        /// <summary>
        /// Este método é utilizado para pesquisar transações por situacao e período.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipoPeriodo"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="criterioOrdem"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> PesquisarTransacoesPorSituacaoEPeriodo(int numeroEmissor, int grupoEntidade, string usuarioLogin, SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal, int posicaoPrimeiroRegistro, int quantidadeRegistros, CriterioOrdemTransacoesPorSituacaoEPeriodo criterioOrdem, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            List<TransacaoEmissor> retorno = new List<TransacaoEmissor>();

            for (int x = 0; x < 12; x++)
            {
                retorno.AddRange(PesquisarTransacoesPorNumeroCartao("", 4545, 5456, "dsdas", 546545, IndicadorTipoCartao.Ambos, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada.Mcc, OrdemClassificacao.Ascendente).ListaTransacoesEmissor);
            }

            return retorno.GetRange(posicaoPrimeiroRegistro, posicaoPrimeiroRegistro + quantidadeRegistros);
            
        }
        /// <summary>
        /// Este método é utilizado para contar transações por situacao e período.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipoPeriodo"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public long ContarTransacoesPorSituacaoEPeriodo(int numeroEmissor, int grupoEntidade, string usuarioLogin, SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal, IndicadorTipoCartao tipoCartao)
        {
            return PesquisarTransacoesPorSituacaoEPeriodo(56456, 45456, "", SituacaoAnalisePesquisa.Ambos, TipoPeriodo.DataEnvioAnalise, System.DateTime.Now, System.DateTime.Now, 
                0, 
                (12*9), 
                CriterioOrdemTransacoesPorSituacaoEPeriodo.NumeroCartao, OrdemClassificacao.Ascendente, IndicadorTipoCartao.Credito).Count();
        }
        /// <summary>
        /// Este método é utilizado para  pesquisar transações suspeitas por emissor e usuário logado.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public TransacoesEmissor PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(int numeroEmissor, int grupoEntidade, string usuarioLogin, int posicaoPrimeiroRegistro, int quantidadeMaximaRegistros, CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin criterio, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            TransacoesEmissor transacoesEmissor = new TransacoesEmissor();

            transacoesEmissor.ListaTransacoesEmissor = this.PesquisarTransacoesPorNumeroCartao(null,
                0, 0, null,
                0,
                IndicadorTipoCartao.Ambos,
                CriterioOrdemTransacoesPorNumeroCartaoOuAssociada.Autorizacao,
                OrdemClassificacao.Ascendente).ListaTransacoesEmissor;
            transacoesEmissor.QuantidadeHorasRecuperadas = 10;
            transacoesEmissor.QuantidadeHorasTotalPeriodo = 60;
            transacoesEmissor.QuantidadeTotalRegistros = transacoesEmissor.ListaTransacoesEmissor.Count;
            return transacoesEmissor;
        }
        /// <summary>
        /// Este método é utilizado para pesquisar transações por transação associada.
        /// </summary>
        /// <param name="identificadorTransacao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tempoBloqueio"></param>
        /// <param name="tipoCartao"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> PesquisarTransacoesPorTransacaoAssociada(long identificadorTransacao, int numeroEmissor, int grupoEntidade, string usuarioLogin, int tempoBloqueio, IndicadorTipoCartao tipoCartao, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterio, OrdemClassificacao ordem)
        {
            List<TransacaoEmissor> lista = new List<TransacaoEmissor>();

            for (int x = 1; x < 10; x++)
            {
                lista.Add(new TransacaoEmissor()
                {
                    Bandeira = new string[] { "Master Card", "Visa"}[x % 2],
                    CodigoEstabelecimento = x * 20,
                    CodigoMCC = x * 5,
                    ComentarioAnalise = new string[] { "Comentario aleatorio 1", "Fraude do cartão", "Roubo", "Estelionato" }[x % 3],
                    DataAnalise = DateTime.Now.AddDays(x * -30),
                    DataEnvioAnalise = DateTime.Now.AddDays(x * -62),
                    DataTransacao = DateTime.Now.AddDays(x * -123),
                    DescricaoEntryMode = "desc entry " + x,
                    DescricaoMCC = "dsa " + x,
                    EntryMode = "1",
                    IdentificadorTransacao = x * 4564564455,
                    NomeEstabelecimento = new string[] { "Casas Bahia", "Mei Mei", "Bar do Carlão", "Zarapeusta" }[x % 3],
                    NumeroCartao = (x * 45645645645645).ToString(),
                    ResultadoAutorizacao = ResultadoAutorizacao.Autorizado,
                    Score = x * 10,
                    SituacaoBloqueio = new SituacaoBloqueio[] {SituacaoBloqueio.BloqueadoOutroUsuario, SituacaoBloqueio.BloqueadoMesmoUsuario, SituacaoBloqueio .SemBloqueio}[x % 3],
                    SituacaoTransacao = new SituacaoFraude[] { SituacaoFraude.Fraude, SituacaoFraude.NaoAplicavel, SituacaoFraude.NaoFraude }[x % 3],
                    TipoAlarme = new TipoAlarme[] { TipoAlarme.NaoAplicavel, TipoAlarme.POC, TipoAlarme.Score, TipoAlarme.UTL }[x % 4]
                    

                });
            }

            return lista;
        }
        /// <summary>
        /// Este método é utilizado para pesquisar transações por numero cartão.
        /// </summary>
        /// <param name="numeroCartao"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tempoBloqueio"></param>
        /// <param name="tipoCartao"></param>
        /// <param name="criterioOrdem"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        public RespostaListaTransacoesEmissor PesquisarTransacoesPorNumeroCartaoEEstabelecimento(string numeroCartao, int numeroEmissor, int grupoEntidade, string usuarioLogin, int tempoBloqueio, IndicadorTipoCartao tipoCartao, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterioOrdem, OrdemClassificacao ordem, long numeroEstabelecimento)
        {
                      
            RespostaListaTransacoesEmissor resposta = new RespostaListaTransacoesEmissor();
            List<TransacaoEmissor> lista = new List<TransacaoEmissor>();
            for (int x = 1; x < 10; x++)
            {
                lista.Add(new TransacaoEmissor()
                {
                    Bandeira = new string[] { "Master Card", "Visa" }[x % 2],
                    CodigoEstabelecimento = x * 100,
                    CodigoMCC = x * 5,
                    ComentarioAnalise = new string[] { "Comentario aleatorio 1", "Fraude do cartão", "Roubo", "Estelionato" }[x % 3],
                    //DataAnalise = new DateTime[] { DateTime.Now, DateTime.Now.AddDays(x), DateTime.Now, DateTime.Now, DateTime.Now.AddDays(x - 1) }[x % 5],
                    DataEnvioAnalise = new DateTime[] { DateTime.Now.AddDays(x - 1), DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now.AddDays(x - 1) }[x % 5],
                    DataTransacao = new DateTime[] { DateTime.Now.AddDays(x - 1), DateTime.Now.AddDays(x), DateTime.Now, DateTime.Now, DateTime.Now }[x % 5],
                    DescricaoEntryMode = "desc entry " + x,
                    DescricaoMCC = "dsa " + x,
                    EntryMode = "1",
                    IdentificadorTransacao = x,
                    NomeEstabelecimento = new string[] { "Casas Bahia", "Mei Mei", "Bar do Carlão", "Zarapeusta", "Generale" }[x % 5],
                    NumeroCartao = (x * 45645645645645).ToString(),
                    ResultadoAutorizacao = ResultadoAutorizacao.Autorizado,
                    Score = x * 10,
                    SituacaoBloqueio = new SituacaoBloqueio[] { SituacaoBloqueio.BloqueadoOutroUsuario, SituacaoBloqueio.BloqueadoMesmoUsuario, SituacaoBloqueio.SemBloqueio }[x % 3],
                    SituacaoTransacao = new SituacaoFraude[] { SituacaoFraude.Fraude, SituacaoFraude.NaoAplicavel, SituacaoFraude.NaoFraude }[x % 3],
                    TipoAlarme = new TipoAlarme[] { TipoAlarme.NaoAplicavel, TipoAlarme.POC, TipoAlarme.Score, TipoAlarme.UTL }[x % 4],
                    TipoCartao = new TipoCartao[] {TipoCartao.Credito, TipoCartao.Debito,TipoCartao.NaoAplicavel}[x%3],
                    TipoResposta = new TipoResposta
                    {
                        CodigoResposta = 0,
                        DescricaoResposta = new string[] { "", "Não Fraude", "Fraude" }[x % 3],
                        NomeResposta = new string[] { "", "Não Fraude", "Fraude" }[x % 3], 
                        SituacaoFraude=new SituacaoFraude[]{SituacaoFraude.Fraude , SituacaoFraude.EmAnalise,SituacaoFraude.NaoFraude, SituacaoFraude.NaoAplicavel}[x%4]
                    },
                    UnidadeFederacao = new string[] { "SP", "RJ", "PR", "PA", "DF", "MG", "MS", "RS", "RN", "SC", "PE" }[x % 11],
                    UsuarioAnalise = new string[] { "William R. Raposo", "Ricardo Gallassi ", "Noriel Vilela", "Napoleão Bonaparte"}[x % 4],
                    Valor = x * 98456
                });
            }
            resposta.ListaTransacoesEmissor = lista;
            resposta.SegundosRestanteBloqueio = 45;
            resposta.TipoRespostaListaEmissor = TipoRespostaListaEmissor.Ok;
            
            return resposta;
        }

        public RespostaListaTransacoesEmissor PesquisarTransacoesPorNumeroCartao(string numeroCartao, int numeroEmissor, int grupoEntidade, string usuarioLogin, int tempoBloqueio, IndicadorTipoCartao tipoCartao, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterioOrdem, OrdemClassificacao ordem)
        {
            RespostaListaTransacoesEmissor resposta = new RespostaListaTransacoesEmissor();
            List<TransacaoEmissor> lista = new List<TransacaoEmissor>();
            for (int x = 1; x < 10; x++)
            {
                lista.Add(new TransacaoEmissor()
                {
                    Bandeira = new string[] { "Master Card", "Visa" }[x % 2],
                    CodigoEstabelecimento = x * 100,
                    CodigoMCC = x * 5,
                    ComentarioAnalise = new string[] { "Comentario aleatorio 1", "Fraude do cartão", "Roubo", "Estelionato" }[x % 3],
                    //DataAnalise = new DateTime[] { DateTime.Now, DateTime.Now.AddDays(x), DateTime.Now, DateTime.Now, DateTime.Now.AddDays(x - 1) }[x % 5],
                    DataEnvioAnalise = new DateTime[] { DateTime.Now.AddDays(x - 1), DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now.AddDays(x - 1) }[x % 5],
                    DataTransacao = new DateTime[] { DateTime.Now.AddDays(x - 1), DateTime.Now.AddDays(x), DateTime.Now, DateTime.Now, DateTime.Now }[x % 5],
                    DescricaoEntryMode = "desc entry " + x,
                    DescricaoMCC = "dsa " + x,
                    EntryMode = "1",
                    IdentificadorTransacao = x,
                    NomeEstabelecimento = new string[] { "Casas Bahia", "Mei Mei", "Bar do Carlão", "Zarapeusta", "Generale" }[x % 5],
                    NumeroCartao = (x * 45645645645645).ToString(),
                    ResultadoAutorizacao = ResultadoAutorizacao.Autorizado,
                    Score = x * 10,
                    SituacaoBloqueio = new SituacaoBloqueio[] { SituacaoBloqueio.BloqueadoOutroUsuario, SituacaoBloqueio.BloqueadoMesmoUsuario, SituacaoBloqueio.SemBloqueio }[x % 3],
                    SituacaoTransacao = new SituacaoFraude[] { SituacaoFraude.Fraude, SituacaoFraude.NaoAplicavel, SituacaoFraude.NaoFraude }[x % 3],
                    TipoAlarme = new TipoAlarme[] { TipoAlarme.NaoAplicavel, TipoAlarme.POC, TipoAlarme.Score, TipoAlarme.UTL }[x % 4],
                    TipoCartao = new TipoCartao[] { TipoCartao.Credito, TipoCartao.Debito, TipoCartao.NaoAplicavel }[x % 3],
                    TipoResposta = new TipoResposta
                    {
                        CodigoResposta = 0,
                        DescricaoResposta = new string[] { "", "Não Fraude", "Fraude" }[x % 3],
                        NomeResposta = new string[] { "", "Não Fraude", "Fraude" }[x % 3],
                        SituacaoFraude = new SituacaoFraude[] { SituacaoFraude.Fraude, SituacaoFraude.EmAnalise, SituacaoFraude.NaoFraude, SituacaoFraude.NaoAplicavel }[x % 4]
                    },
                    UnidadeFederacao = new string[] { "SP", "RJ", "PR", "PA", "DF", "MG", "MS", "RS", "RN", "SC", "PE" }[x % 11],
                    UsuarioAnalise = new string[] { "William R. Raposo", "Ricardo Gallassi ", "Noriel Vilela", "Napoleão Bonaparte" }[x % 4],
                    Valor = x * 98456
                });
            }
            resposta.ListaTransacoesEmissor = lista;
            resposta.SegundosRestanteBloqueio = 45;
            resposta.TipoRespostaListaEmissor = TipoRespostaListaEmissor.Ok;

            return resposta;
        }

        
        /// <summary>
        /// Este método é utilizado para pesquisar usuários por emissor.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        public string[] PesquisarUsuariosPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            return new string[] { "Napoleão Boanaparte", "Herman Gering", "Noriel Vilela", "Ricardo Galassi", "William R. Raposo", "Genghis Kahn", "Cronos", "Manta", "Abadon" };
        }
        /// <summary>
        /// Este método é utilizado para atualizar resultado da análise.
        /// </summary>
        /// <param name="listaRespostaAnalise"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public int AtualizarResultadoAnalise(List<RespostaAnalise> listaRespostaAnalise, IndicadorTipoCartao tipoCartao)
        {
            return listaRespostaAnalise.Count();
        }
        /// <summary>
        /// Este método é utilizado para pesquisar a lista tipos de resposta.
        /// </summary>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <returns></returns>
        public List<TipoResposta> PesquisarListaTiposResposta(string usuarioLogin, int numeroEmissor, int grupoEntidade)
        {

            List<TipoResposta> retorno = new List<TipoResposta>();
            retorno.Add(new TipoResposta()
            {
                CodigoResposta = 4,
                DescricaoResposta = "Roubo",
                SituacaoFraude = Redecard.PN.FMS.Modelo.SituacaoFraude.NaoFraude
                ,
            });


            retorno.Add(new TipoResposta() { 
                CodigoResposta = 1,
                DescricaoResposta = "Falsificação",
                SituacaoFraude = Redecard.PN.FMS.Modelo.SituacaoFraude.Fraude
,
            });

            retorno.Add(new TipoResposta()
            {
                CodigoResposta = 2,
                DescricaoResposta = "Roubo",
                SituacaoFraude = Redecard.PN.FMS.Modelo.SituacaoFraude.Fraude
                ,
            });

            retorno.Add(new TipoResposta()
            {
                CodigoResposta = 3,
                DescricaoResposta = "Adulteração" ,
                    SituacaoFraude = Redecard.PN.FMS.Modelo.SituacaoFraude.Fraude
                ,
            });


            return retorno;
        }

        private static List<long> cartoesBloqueados; 
        /// <summary>
        /// Este método é utilizado para bloquear cartão. 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="identificadorTransacao"></param>
        /// <param name="tempoBloqueioEmSegundos"></param>
        public void BloquearCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin, long identificadorTransacao, int tempoBloqueioEmSegundos)
        {
            if (cartoesBloqueados == null)
            {
                cartoesBloqueados = new List<long>();
            }

            if (!cartoesBloqueados.Contains(identificadorTransacao))
            {
                cartoesBloqueados.Add(identificadorTransacao);
            }
        }
        /// <summary>
        /// Este método é utilizado para desbloquear cartão.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="identificadorTransacao"></param>
        public void DesbloquearCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin, long identificadorTransacao)
        {
            if (cartoesBloqueados == null)
            {
                cartoesBloqueados = new List<long>();
            }

            if (cartoesBloqueados.Contains(identificadorTransacao))
            {
                cartoesBloqueados.Remove(identificadorTransacao);
            }
        }
        /// <summary>
        /// Este método é utilizado para pesquisar o relatório de produtividade por analista. 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        public RelatorioProdutividadePorAnalista PesquisarRelatorioProdutividadePorAnalista(int numeroEmissor, int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal, CriterioOrdemProdutividade criterio, OrdemClassificacao ordem)
        {
            RelatorioProdutividadePorAnalista retorno = new RelatorioProdutividadePorAnalista()
            {
                QuantidadeTotalCartoesAnalisados = 6 * 10,
                QuantidadeTotalCartoesFraudulentos = ((6 * 10) / 2) - 3,
                QuantidadeTotalCartoesNaoFraudulentos = ((6 * 10) / 2) + 3,
                QuantidadeTotalTransacoesFraudulentas = ((6 * 100) / 2) - 30,
                QuantidadeTotalTransacoesNaoFraudulentas = ((6 * 100) / 2) + 30,

                ValorTotalFraude = (((6 * 100) / 2) - 30) * 1000,
                ValorTotalNaoFraude = (((6 * 100) / 2) + 30) * 1000,
                QuantidadeTotalRegistros = 3*5
                };
            retorno.AgrupamentoProdutividadePorAnalista = new List<AgrupamentoProdutividadePorAnalista>();
            string[] usuarios = new string[]{ "Napoleão Bonaparte", "Genghis Kahn", "Sthephen Hawking"};
            for (int x = 1; x <= 3; x++)
            {
                AgrupamentoProdutividadePorAnalista item = new AgrupamentoProdutividadePorAnalista()
                {
                    QuantidadeTotalCartoesAnalisados = x * 10,
                    QuantidadeTotalCartoesFraudulentos = ((x * 10) / 2) - 3,
                    QuantidadeTotalCartoesNaoFraudulentos = ((x * 10) / 2) + 3,
                    QuantidadeTotalTransacoesFraudulentas = ((x * 100) / 2) - 30,
                    QuantidadeTotalTransacoesNaoFraudulentas = ((x * 100) / 2) + 30,
                    UsuarioLogin = usuarios[x - 1],
                    ValorTotalFraude = (((x * 100) / 2) - 30) * 1000,
                    ValorTotalNaoFraude = (((x * 100) / 2) + 30) * 1000
                };

                item.ProdutividadePorAnalista = new List<DetalheProdutividadePorAnalista>();
                for (int y = 1; y <= 5; y++)
                {
                    DetalheProdutividadePorAnalista det = new DetalheProdutividadePorAnalista()
                    {
                      Data = DateTime.Now.AddDays(- y * 10),
                      QuantidadeCartoesAnalisados = item.QuantidadeTotalCartoesAnalisados/y,
                      QuantidadeCartoesFraudulentos = item.QuantidadeTotalCartoesFraudulentos/y,
                      QuantidadeCartoesNaoFraudulentos = item.QuantidadeTotalCartoesNaoFraudulentos/y,
                      QuantidadeTransacoesFraudulentas = item.QuantidadeTotalTransacoesFraudulentas/y,
                      QuantidadeTransacoesNaoFraudulentas = item.QuantidadeTotalTransacoesNaoFraudulentas/y,
                      ValorFraude = item.ValorTotalFraude/y,
                      ValorNaoFraude = item.ValorTotalNaoFraude/y,
                    };

                    item.ProdutividadePorAnalista.Add(det);
                }
                retorno.AgrupamentoProdutividadePorAnalista.Add(item);
            }
            return retorno;
        }
        /// <summary>
        /// Este método é utilizado para pesquisar o relatorio de produtividade por data. 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        public RelatorioProdutividadePorData PesquisarRelatorioProdutividadePorData(int numeroEmissor, int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal, CriterioOrdemProdutividade criterio, OrdemClassificacao ordem)
        {
            RelatorioProdutividadePorData dat = new RelatorioProdutividadePorData();
           // RelatorioProdutividadePorAnalista rel = this.PesquisarRelatorioProdutividadePorAnalista(numeroEmissor, grupoEntidade, usuarioLogin, usuario, dataInicial, dataFinal, CriterioOrdemProdutividade.Data, OrdemClassificacao.Ascendente);
            return dat;
        }
        /// <summary>
        /// Este método é utilizado para listar os parametros sistema.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        public ParametrosSistema ListaParametrosSistema(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            
            ParametrosSistema retorno = new ParametrosSistema();
            retorno.PossuiAcesso = 1;
            retorno.QuantidadeMaximaDiasRetroativosPesquisas = 30;
            retorno.QuantidadeMaximaIntervaloDiasPesquisas = 20;
            return retorno;
        }
        /// <summary>
        /// Este método é utilizado para  pesquisar range bin por emissor.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="ica"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistro"></param>
        /// <returns></returns>
        public List<FaixaBin> PesquisarRangeBinPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin, long ica, int posicaoPrimeiroRegistro, int quantidadeMaximaRegistro)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Este método é utilizado para contar range bin por emissor.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="ica"></param>
        /// <returns></returns>
        public long ContarRangeBinPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin, long ica)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Este método é utilizado para  pesquisar lista de mechant Category codes.
        /// </summary>
        /// <param name="codigoMCC"></param>
        /// <param name="descricaoMCC"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <returns></returns>
        public List<MCC> PesquisarListaMCC(long? codigoMCC, string descricaoMCC, int posicaoPrimeiroRegistro, int quantidadeMaximaRegistros)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Este método é utilizado para contar registros de mechant Category code.
        /// </summary>
        /// <param name="codigoMCC"></param>
        /// <param name="descricaoMCC"></param>
        /// <returns></returns>
        public long ContarRegistrosMCC(long? codigoMCC, string descricaoMCC)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Este método é utilizado para  pesquisar critérios seleção por usuário logado.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public CriteriosSelecao PesquisarCriteriosSelecaoPorUsuarioLogin(int numeroEmissor, int grupoEntidade, string usuarioLogin, string usuario)
        {
            CriteriosSelecao resposta = new CriteriosSelecao()
            {
                CriterioClassificacao = new CriterioClassificacao[] { CriterioClassificacao.DataTransacao, CriterioClassificacao.Score, CriterioClassificacao.Valor }[1],
                CriterioClassificacaoEstabelecimento = CriterioClassificacaoEstabelecimento.QuantidadeTransacoesSuspeitasAprovadas,
                EntryModes = new List<EntryMode>() { 
                    new EntryMode(){
                        Codigo = "1",
                        Descricao = "Entry mode 1"
                    },
                    new EntryMode(){
                        Codigo = "2",
                        Descricao = "Entry mode 2"
                    },
                    new EntryMode(){
                        Codigo = "3",
                        Descricao = "Entry mode 3"
                    },
                    new EntryMode(){
                        Codigo = "4",
                        Descricao = "Entry mode 4"
                    }
                },
                EntryModesSelecionados = new List<EntryMode>() { 
                    new EntryMode(){
                        Codigo = "1",
                        Descricao = "Entry mode 1"
                    },
                    new EntryMode(){
                        Codigo = "3",
                        Descricao = "Entry mode 3"
                    }},
                EstabelecimentosSelecionados = new List<long>() { 4564, 4564, 7894555, 12478457 },
                FimFaixaScore = 456456465,
                InicioFaixaScore = 454564,
                MCCsSelecionados = new List<MCC>() { 
                    new MCC(){
                        CodigoMCC = "1",
                        DescricaoMCC = "MCC 1"
                    },
                    new MCC(){
                        CodigoMCC = "2",
                        DescricaoMCC = "MCC 2"
                    },
                    new MCC(){
                        CodigoMCC = "3",
                        DescricaoMCC = "MCC 3"
                    },
                    new MCC(){
                        CodigoMCC = "4",
                        DescricaoMCC = "MCC 4"
                    }
                },
                RangeBinsSelecionados = new List<FaixaBin>() { 
                    new FaixaBin(){
                        ValorFinal = "456465128",
                        ValorInicial = "8745454465"
                    },
                    new FaixaBin(){
                        ValorFinal = "4233242",
                        ValorInicial = "2312"
                    },
                    new FaixaBin(){
                        ValorFinal = "243242342",
                        ValorInicial = "12231312"
                    },
                    new FaixaBin(){
                        ValorFinal = "45345443",
                        ValorInicial = "12312312"
                    },
                },

                ResultadoAutorizacao = new List<CriterioResultadoAutorizacao>()
                {
                    CriterioResultadoAutorizacao.Aprovada,
                    CriterioResultadoAutorizacao.Negada
                }
                ,
                SituacoesCartaoSelecionados = new List<SituacaoCartao>() { 
                    SituacaoCartao.Analisado
                    
                },
                TipoAlarme = new List<TipoAlarme>() { 
                    TipoAlarme.NaoAplicavel,
                    TipoAlarme.Score
                },

                TipoTransacaoSelecionadas = new List<TipoTransacao>(){
                    TipoTransacao.Debito,
                },
                UF = new List<string>() { "SP", "RJ", "PA", "PR", "RO", "DF", "SC", "RS", "GO" },

                UFsSelecionadas = new List<string>() { "SP", "RJ"},

                ValorTransacaoFinal = 12345,
                ValorTransacaoInicial = 987654321

            };
            return resposta;

        }
        /// <summary>
        /// Este método é utilizado para  atualizar critérios seleção.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="criterio"></param>
        public void AtualizarCriteriosSelecao(int numeroEmissor, string usuarioLogin, int grupoEntidade, CriteriosSelecao criterio)
        {
            
        }
        /// <summary>
        /// Este método é utilizado para  exportar transações suspeitas por emissor e usuário loagdo.
        /// </summary>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <param name="modoClassificacao"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        public RespostaTransacoesEstabelecimentoPorCartao PesquisarTransacoesEstabelecimentoAgrupadasPorCartao(long numeroEstabelecimento, int numeroEmissor, int grupoEntidade, string usuarioLogin, int primeiroRegistro, int quantidadeMaximaRegistros, CriterioOrdemTransacoesEstabelecimentoAgrupadasPorCartao modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoTransacao)
        {
            RespostaTransacoesEstabelecimentoPorCartao resposta = new RespostaTransacoesEstabelecimentoPorCartao();

            resposta.QuantidadeRegistros = 10;
            resposta.ListaTransacoes = new List<AgrupamentoTransacoesEstabelecimentoCartao>();
            for (int x = 1; x < resposta.QuantidadeRegistros; x++)
            {
                resposta.ListaTransacoes.Add(new AgrupamentoTransacoesEstabelecimentoCartao()
                {
                    //NomeFantasiaEstabelecimento = new string[] { "Casas Bahia", "GEnerale", "MEi MEi", "Apple Store", "Fast Shop" }[x % 5],
                    //NumeroEstabelecimento = x * 4564,
                    NumeroCartao = (x * 4564545645654).ToString(),
                    QuantidadeTotalTransacoes = x * 1548,
                    QuantidadeTransacoesSuspeitas = ((x * 1548) / 2) + 5,
                    QuantidadeTransacoesAprovadas = ((x * 1548) / 2) - 5,
                    TipoCartao = new IndicadorTipoCartao[] { IndicadorTipoCartao.Credito, IndicadorTipoCartao.Debito, IndicadorTipoCartao.Ambos }[x % 3],
                    ValorTotalTransacoes = x * 12318,
                    ValorTransacoesAprovadas = x * 44564514,
                    ValorTransacoesSuspeitas = x * 45456748
                });
            }
            resposta.NomeEstabelecimento = "MEi Mei";
            resposta.NumeroEstabelecimento = 666;
            resposta.QuantidadeHorasPeriodo = 24;
            resposta.QuantidadeHorasRecuperadas = 12;
            return resposta;
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <param name="modoClassificacao"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        public RespostaTransacoesPorCartao PesquisarTransacoesAgrupadasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin, int primeiroRegistro, int quantidadeMaximaRegistros, CriterioOrdemTransacoesAgrupadasPorCartao modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoTransacao)
        {
            RespostaTransacoesPorCartao resposta = new RespostaTransacoesPorCartao();
            List<AgrupamentoTransacoesCartao> transCartao = new List<AgrupamentoTransacoesCartao>();

            for (int i = 1; i < 10; i ++ ){
                transCartao.Add(new AgrupamentoTransacoesCartao(){
                        DataTransacaoSuspeitaMaisRecente = new DateTime(),
                        NumeroCartao = (45644564646446465 * i).ToString(),
                        QuantidadeTransacoesSuspeitasAprovadas = i,
                        QuantidadeTransacoesSuspeitasNegadas = 10 - i,
                        Score = i,
                        SituacaoCartao = i % 2 == 0 ? SituacaoCartao.Analisado : SituacaoCartao.NaoAnalisado,
                        TipoCartao = i % 3 == 0 ? IndicadorTipoCartao.Credito: (i % 2 == 0 ? IndicadorTipoCartao.Debito : IndicadorTipoCartao.Ambos),
                        ValorTotalTransacoes = (i * 45644)/(1),
                        ValorTransacoesSuspeitasAprovadas = (i * 4564) / (1 ),
                        ValorTransacoesSuspeitasNegadas = (i * 6) / (1 )
                });
            }
            resposta.ListaTransacoes = transCartao;
            resposta.QuantidadeHorasPeriodo = 8;
            resposta.QuantidadeHorasRecuperadas = 4;
            resposta.QuantidadeRegistros = 8;
            return resposta;
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        public RespostaTransacoesEstabelecimentoPorCartao ExportarTransacoesEstabelecimentoAgrupadasPorCartao(long numeroEstabelecimento, int numeroEmissor, int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoTransacao)
        {
            RespostaTransacoesEstabelecimentoPorCartao resposta = new RespostaTransacoesEstabelecimentoPorCartao();

            resposta.QuantidadeRegistros = 10;
            resposta.ListaTransacoes = new List<AgrupamentoTransacoesEstabelecimentoCartao>();
            for (int x = 1; x < resposta.QuantidadeRegistros; x++)
            {
                resposta.ListaTransacoes.Add(new AgrupamentoTransacoesEstabelecimentoCartao()
                {
                    NumeroCartao = (x * 4564545645654).ToString(),
                    QuantidadeTotalTransacoes = x * 1548,
                    QuantidadeTransacoesSuspeitas = ((x * 1548) / 2) + 5,
                    QuantidadeTransacoesAprovadas = ((x * 1548) / 2) - 5,
                    TipoCartao = new IndicadorTipoCartao[] { IndicadorTipoCartao.Credito, IndicadorTipoCartao.Debito, IndicadorTipoCartao.Ambos }[x % 3],
                    ValorTotalTransacoes = x * 12318,
                    ValorTransacoesAprovadas = x * 44564514,
                    ValorTransacoesSuspeitas = x * 45456748,
                });
            }

            return resposta;
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public TransacoesEmissor ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin(int numeroEmissor, int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoCartao)
        {
            return PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(numeroEmissor, grupoEntidade, usuarioLogin, 0, 100, CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin.Bandeira, OrdemClassificacao.Ascendente, IndicadorTipoCartao.Ambos);
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="tipoTransacao"></param>
        /// <returns></returns>
        public RespostaTransacoesPorCartao ExportarTransacoesAgrupadasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin, IndicadorTipoCartao tipoTransacao)
        {
            return this.PesquisarTransacoesAgrupadasPorCartao(numeroEmissor, grupoEntidade, usuarioLogin, 0, 1000, CriterioOrdemTransacoesAgrupadasPorCartao.Data, OrdemClassificacao.Descendente, IndicadorTipoCartao.Credito);
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public TransacaoEmissor[] ExportarTransacoesAnalisadasPorUsuarioEPeriodo(int numeroEmissor, int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal, int posicaoPrimeiroRegistro, int quantidadeRegistros, CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo criterio, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            List<TransacaoEmissor> retorno = new List<TransacaoEmissor>();

            for (int x = 0; x < 12; x++)
            {
                retorno.AddRange(PesquisarTransacoesPorNumeroCartao("", 4545, 5456, "dsdas", 546545, IndicadorTipoCartao.Ambos, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada.Mcc, OrdemClassificacao.Ascendente).ListaTransacoesEmissor);
            }

            if (criterio == CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo.Valor)
            {
                retorno.Sort((lhs, rhs) => Comparer<decimal>.Default.Compare(lhs.Valor, rhs.Valor));
            }
            else if (criterio == CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo.NumeroCartao)
            {
                retorno.Sort((lhs, rhs) => Comparer<string>.Default.Compare(lhs.NumeroCartao, rhs.NumeroCartao));
            }


            return retorno.ToArray() ;
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <param name="modoClassificacao"></param>
        /// <param name="ordem"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public RespostaTransacoesEstabelecimento PesquisarTransacoesAgrupadasPorEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, int primeiroRegistro, int quantidadeMaximaRegistros, CriterioOrdemTransacoesAgrupadasPorEstabelecimento modoClassificacao, OrdemClassificacao ordem, IndicadorTipoCartao tipoCartao)
        {
            RespostaTransacoesEstabelecimento resposta = new RespostaTransacoesEstabelecimento();

            resposta.QuantidadeTransacoes = 10;
            resposta.ListaTransacoes = new List<AgrupamentoTransacaoEstabelecimento>();
            for (int x = 1; x < resposta.QuantidadeTransacoes; x++)
            {
                resposta.ListaTransacoes.Add(new AgrupamentoTransacaoEstabelecimento()
                {
                    NomeFantasiaEstabelecimento = new string[]{"Casas Bahia", "GEnerale", "MEi MEi", "Apple Store", "Fast Shop"}[x % 5],
                    NumeroEstabelecimento = x * 4564,
                    QuantidadeTotalTransacoes = x * 1548,
                    QuantidadeTransacoesSuspeitasAprovadas = ((x * 1548 )/ 2 ) + 5,
                    QuantidadeTransacoesSuspeitasNegadas =  ((x * 1548 )/ 2 ) - 5,
                    TipoCartao = new IndicadorTipoCartao[] {IndicadorTipoCartao.Credito, IndicadorTipoCartao.Debito, IndicadorTipoCartao.Ambos}[x % 3],
                    ValorTotalTransacoes = x *12318,
                    ValorTransacoesSuspeitasAprovadas = x *44564514,
                    ValorTransacoesSuspeitasNegadas = x * 45456748
                });
            }

            return resposta;
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        public void DescartarSessaoPesquisaTransacoesSuspeitas(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="tempoLimite"></param>
        public void BloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento, int tempoLimite)
        {
            
        }
        /// <summary>
        /// Este método é utilizado para 
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="tempoLimite"></param>
        public void DesbloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento)
        {
            
        }
        /// <summary>
        /// Este método é utilizado para  exportar transacões por situação e período
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipoPeriodo"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public List<TransacaoEmissor> ExportarTransacoesPorSituacaoEPeriodo(int numeroEmissor, int grupoEntidade, string usuarioLogin, SituacaoAnalisePesquisa situacao, TipoPeriodo tipoPeriodo, DateTime dataInicial, DateTime dataFinal, IndicadorTipoCartao tipoCartao)
        {
            throw new NotImplementedException();
        }
    }
}
