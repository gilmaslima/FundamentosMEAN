/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Config;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Datasource;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Result;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;
using Redecard.PN.Comum;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Datasource.Public;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search
{
    /// <summary>
    /// Classe principal para realização de busca para o Módulo Atendimento Digital
    /// </summary>
    public class SearchPortal
    {
        #region [ Propriedades ]

        /// <summary>
        /// Contexto SPWeb
        /// </summary>
        public SPWeb Web { get; set; }

        /// <summary>
        /// Sessão do usuário atual
        /// </summary>
        public Sessao Sessao { get; set; }

        /// <summary>
        /// Configuração da pesquisa
        /// </summary>
        public QueryConfig Config { get; set; }
        
        /// <summary>
        /// Coleção para armazenar o conteúdo a ser pesquisado
        /// </summary>
        public Dictionary<String, List<ContentItem>> ContentItems { get; set; }

        /// <summary>
        /// Configuração de acentuação
        /// </summary>
        public AccentuationConfig AccentuationConfig 
        { 
            get { return AccentuationConfig.GetInstance(this.Web, true); } 
        }

        /// <summary>
        /// Configuração de palavras ignoradas
        /// </summary>
        public IgnoredWordsConfig IgnoredWordsConfig
        {
            get { return IgnoredWordsConfig.GetInstance(this.Web, true); }
        }

        /// <summary>
        /// Configuração dos sinônimos das palavras
        /// </summary>
        public SynonymsConfig SynonymsConfig
        {
            get { return SynonymsConfig.GetInstance(this.Web, true); }
        }

        /// <summary>
        /// Configuração das correções de palavras
        /// </summary>
        public CorrectionsConfig CorrectionsConfig
        {
            get { return CorrectionsConfig.GetInstance(this.Web, true); }
        }

        /// <summary>
        /// Configuração das regras para exibição dos cards
        /// </summary>
        public CardsConfig CardsConfig
        {
            get { return CardsConfig.GetInstance(this.Web, true); }
        }

        #endregion

        #region [ Construtor ]

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="web">Contexto SPWeb</param>
        /// <param name="sessao">Sessão do usuário</param>
        /// <param name="customConfig">Configuração da busca customizada</param>
        public SearchPortal(SPWeb web, Sessao sessao, QueryConfig customConfig)
        {
            this.Web = web;
            this.Sessao = sessao;
            this.Config = QueryConfig.GetConfig(this.Web, customConfig);

            this.InitializeContentItems();
        }

        /// <summary>
        /// Inicialização dos conteúdos que serão analisados
        /// </summary>
        private void InitializeContentItems()
        {
            this.ContentItems = new Dictionary<String, List<ContentItem>>();

            //datasourcer que dependem da sessão
            if (this.Config.Scopes.Contains("servico", StringComparer.InvariantCultureIgnoreCase))
                this.ContentItems["servico"] = new ServicoDatasource(this.Sessao).GetItems();

            if (this.Config.Scopes.Contains("card", StringComparer.InvariantCultureIgnoreCase))
                this.ContentItems["card"] = new CardDatasource(this.Sessao, this.CardsConfig).GetItems();

            //recupera a configuração do dispositivo que executou a busca
            Device device = (this.Config.Device ?? (Device?)Device.AllDevices).Value;
            
            if (this.Config.Scopes.Contains("perguntaResposta", StringComparer.InvariantCultureIgnoreCase))
                this.ContentItems["perguntaResposta"] = new PerguntaRespostaDatasource(this.Web)
                    .GetItems(device, this.Config.UseCache.Value);

            if (this.Config.Scopes.Contains("categoria", StringComparer.InvariantCultureIgnoreCase))
                this.ContentItems["categoria"] = new CategoriaDatasource(this.Web)
                    .GetItems(device, this.Config.UseCache.Value);

            if (this.Config.Scopes.Contains("subcategoria", StringComparer.InvariantCultureIgnoreCase))
                this.ContentItems["subcategoria"] = new SubcategoriaDatasource(this.Web)
                    .GetItems(device, this.Config.UseCache.Value);

            if (this.Config.Scopes.Contains("public/faq", StringComparer.InvariantCultureIgnoreCase))
                this.ContentItems["public/faq"] = new FaqNaoLogadoDatasource(this.Web)
                    .GetItems(device, null, null, this.Config.UseCache.Value);

            if(this.Config.Scopes.Contains("public/categoria", StringComparer.InvariantCultureIgnoreCase))
                this.ContentItems["public/categoria"] = new FaqNaoLogadoDatasource(this.Web, true, false, false)
                    .GetItems(device, null, null, this.Config.UseCache.Value);

            //recupera escopos da pesquisa para subcategorias da área não logada
            {
                Regex regexId = new Regex(@"public/subcategoria(\/)?(\?categoria=(?<idCategoria>\d+))?$", RegexOptions.IgnoreCase);
                Match match = this.Config.Scopes.Select(scope => regexId.Match(scope)).FirstOrDefault(scopeMatch => scopeMatch.Success);
                if (match != null)
                {
                    Int32? idCategoria = match.Groups["idCategoria"].Value.ToInt32Null();
                    this.ContentItems["public/subcategoria"] = new FaqNaoLogadoDatasource(this.Web, false, true, false)
                        .GetItems(device, idCategoria, null, this.Config.UseCache.Value);
                }
            }

            //recupera escopos da pesquisa para perguntas e respostas da área não logada
            {
                Regex regexId = new Regex(@"public/perguntaResposta(\/)?(\?subcategoria=(?<idSubcategoria>\d+))?$", RegexOptions.IgnoreCase);
                Match match = this.Config.Scopes.Select(scope => regexId.Match(scope)).FirstOrDefault(scopeMatch => scopeMatch.Success);
                if (match != null)
                {
                    Int32? idSubcategoria = match.Groups["idSubcategoria"].Value.ToInt32Null();
                    this.ContentItems["public/perguntaResposta"] = new FaqNaoLogadoDatasource(this.Web, false, false, true)
                        .GetItems(device, null, idSubcategoria, this.Config.UseCache.Value);
                }
            }
        }

        #endregion

        #region [ Métodos Públicos ]

        /// <summary>
        /// Efetua a busca de um termo.
        /// </summary>
        /// <param name="term">Termo a ser pesquisado</param>
        /// <returns>Resultado da pesquisa</returns>
        public SearchResult Search(String term)
        {                        
            SearchResult result = new SearchResult();
            result.Term = ProcessTerm(term);
            result.Results = new Dictionary<String, ResultGroup>();
            
            //efetua busca em cada tipo de conteúdo especificado (escopo)
            foreach (String scope in this.ContentItems.Keys)
            {
                Pagination pagination = this.Config.Pagination[scope];
                List<ResultItem> scopeResults = new List<ResultItem>();
                List<ContentItem> contentItems = this.ContentItems[scope];

                //somente processa items do escopos se termo pesquisado for válido
                if (result.Term.IsValid)
                {
                    if (String.Compare(scope, "card", true) == 0)
                        scopeResults.AddRange(this.ComputeCards(result.Term, contentItems.Cast<CardItem>().ToList()));
                    else
                        scopeResults.AddRange(this.Compute(result.Term, contentItems));
                }

                ResultGroup resultGroup = new ResultGroup();
                resultGroup.TotalCount = scopeResults.Count;

                //realiza paginação nos resultados encontrados
                resultGroup.Items = scopeResults
                    .Skip(pagination.StartRow ?? 0)
                    .Take(pagination.RowsPerPage ?? 10)
                    .ToList();

                result.Results.Add(scope, resultGroup);
            }

            return result;
        }
                
        #endregion

        #region [ Métodos Auxiliares Privados ]

        /// <summary>
        /// Processa um termo pesquisado, verificando sinônimos, caracteres equivalentes, 
        /// palavras ignoradas, e correções
        /// </summary>
        /// <param name="term">Termo pesquisado</param>
        /// <returns>Termo processado e analisado</returns>
        private ProcessedTerm ProcessTerm(String term)
        {
            ProcessedTerm processedTerm = new ProcessedTerm();
            processedTerm.Original = term;

            #region [ Normalização ]

            //Prepara termo completo: remove espaços duplicados, trim, lower
            term = NormalizeTerm(term);
            processedTerm.Formatted = term;

            #endregion

            #region [ Palavras ignoradas ]

            //Remove palavras ignoradas, considerando lista de caracteres equivalentes
            if (this.Config.AutoIgnoreWords.Value)
            {
                List<String> palavrasIgnoradas;
                term = ExcludeIgnoredWords(term, out palavrasIgnoradas);
                if (palavrasIgnoradas.Count > 0)
                {
                    processedTerm.IgnoredWords = palavrasIgnoradas;
                }
            }

            #endregion

            #region [ Correção ]

            //Procura correções das palavras, considerando lista de caracteres equivalentes            
            if (this.Config.AutoCorrectWords.Value == true)
            {
                Dictionary<String, String> correcoesAplicadas;
                processedTerm.Corrected = CorrectWords(term, out correcoesAplicadas);
                if (processedTerm.Corrected.Length > 0)
                {
                    term = processedTerm.Corrected;
                    processedTerm.Corrections = correcoesAplicadas;
                }
            }

            #endregion
         
            processedTerm.RegexExactMatch = new Regex("(?<COMPLETO>\\b(" +
                Regex.Replace(AccentuationConfig.GerarRegex(term), "\\s+", "\\s+") + ")\\b)",
                RegexOptions.IgnoreCase);

            //Quebra de palavra em palavra
            List<String> palavras = Regex.Split(term, @"\s+", RegexOptions.IgnoreCase)
                .Where(palavra => !String.IsNullOrWhiteSpace(palavra)).Distinct().ToList();
            processedTerm.Words = palavras;

            #region [ Sinônimos ]

            //Procura sinônimos das palavras, considerando lista de caracteres equivalentes
            if (this.Config.UseSynonyms.Value)
            {
                Dictionary<String, List<String>> sinonimosEncontrados;
                term = IncludeSynonyms(term, out sinonimosEncontrados);
                if (sinonimosEncontrados.Count > 0)
                {
                    processedTerm.Synonyms = sinonimosEncontrados;
                }
            }

            #endregion

            term = term.Replace("#", " ");

            processedTerm.RegexWordMatch = new Regex("(?<EXATA>\\b(" + 
                String.Join("|", AccentuationConfig.GerarRegex(palavras)) + 
                ")\\b)", 
                RegexOptions.IgnoreCase);

            processedTerm.RegexPartialMatch = new Regex("(?<PARCIAL>((\\B(" +
                String.Join("|", AccentuationConfig.GerarRegex(palavras)) + "))|((" +
                String.Join("|", AccentuationConfig.GerarRegex(palavras)) + ")\\B)))", 
                RegexOptions.IgnoreCase);

            //somente marca como termo válido se existirem palavras a serem pesquisadas
            processedTerm.IsValid = processedTerm.Words.Count > 0;

            processedTerm.Processed = term;

            return processedTerm;
        }

        /// <summary>
        /// Analisa cada item do conteúdo especificado e contabiliza 
        /// a quantidade de ocorrências do termo procurado.
        /// </summary>
        /// <param name="term">Termo</param>
        /// <param name="contentItems">Conteúdo a ser analisado</param>
        /// <returns>Resultado da análise do termo no conteúdo informado</returns>
        private List<ResultItem> Compute(ProcessedTerm term, List<ContentItem> contentItems)
        {
            List<ResultItem> results = new List<ResultItem>();
            Boolean ignoreWords = this.Config.AutoIgnoreWords.Value;

            foreach (ContentItem conteudo in contentItems)
            {
                ResultItem resultItem = new ResultItem();                
                resultItem.Content = conteudo;

                String title = conteudo.Title;
                String description = conteudo.Description;

                if (ignoreWords)
                {
                    title = ExcludeIgnoredWords(title);
                    description = ExcludeIgnoredWords(description);
                }                

                resultItem.TitleHits = new Hit();
                resultItem.TitleHits.ExactMatch = term.RegexExactMatch.Matches(title).Count;
                if (resultItem.TitleHits.ExactMatch == 0)
                {
                    resultItem.TitleHits.Match = term.RegexWordMatch.Matches(title).Count;
                    resultItem.TitleHits.PartialMatch = term.RegexPartialMatch.Matches(title).Count;
                }

                resultItem.DescriptionHits = new Hit();
                resultItem.DescriptionHits.ExactMatch = term.RegexExactMatch.Matches(description).Count;
                if (resultItem.DescriptionHits.ExactMatch == 0)
                {
                    resultItem.DescriptionHits.Match = term.RegexWordMatch.Matches(description).Count;
                    resultItem.DescriptionHits.PartialMatch = term.RegexPartialMatch.Matches(description).Count;
                }

                resultItem.TitleHits.NoMatch = 0;
                resultItem.DescriptionHits.NoMatch = 0;
                foreach (String palavra in term.Words)
                {
                    if (!Regex.IsMatch(title, AccentuationConfig.GerarRegex(palavra), RegexOptions.IgnoreCase))
                        resultItem.TitleHits.NoMatch++;
                    if (!Regex.IsMatch(description, AccentuationConfig.GerarRegex(palavra), RegexOptions.IgnoreCase))
                        resultItem.DescriptionHits.NoMatch++;
                }

                resultItem.Rank = this.CalculateRankValue(resultItem);

                results.Add(resultItem);
            }

            return results
                .Where(result => result.Rank > 0)
                .OrderByDescending(result => result.Rank)
                .ToList();
        }

        /// <summary>
        /// Analise os items do conteúdo de cards e verifica quais deven ser
        /// retornados para o termo pesquisado.
        /// </summary>
        /// <param name="term">Termo</param>
        /// <param name="contentItems">Conteúdo a ser analisado</param>
        /// <returns>Resultado da análise do termo no conteúdo informado</returns>
        private List<ResultItem> ComputeCards(ProcessedTerm term, List<CardItem> contentItems)
        {
            List<ResultItem> results = new List<ResultItem>();

            foreach (CardItem conteudo in contentItems)
            {
                ProcessedTerm cardTerm = ProcessTerm(conteudo.Title);
                String regexCardExactMatch = cardTerm.RegexExactMatch.ToString().Replace("#", @"\s+");
                Boolean cardMatched = new Regex(regexCardExactMatch).IsMatch(term.Corrected);

                //retorna card apenas se match do texto é exato
                if (cardMatched)
                {
                    ResultItem resultItem = new ResultItem();
                    resultItem.Content = conteudo;
                    resultItem.Priority = conteudo.Priority;
                    results.Add(resultItem);
                }
            }

            return results
                .OrderBy(result => result.Priority)
                .ToList();
        }

        /// <summary>
        /// Calcula o valor do Rank, com base nas ocorrências encontradas, aplicando
        /// o peso de cada fator.
        /// </summary>
        /// <param name="resultItem">Item a ser analisado</param>
        /// <returns>Valor do Rank calculado</returns>
        private Int32 CalculateRankValue(ResultItem resultItem)
        {
            Int32 rankingValue = 0;
            Int32 negativeRankingValue = 0;

            if(resultItem.TitleHits != null)
            {
                rankingValue +=
                    (resultItem.TitleHits.ExactMatch ?? 0) * this.Config.Weights.Title.ExactMatch.Value +
                    (resultItem.TitleHits.Match ?? 0) * this.Config.Weights.Title.Match.Value +
                    (resultItem.TitleHits.PartialMatch ?? 0) * this.Config.Weights.Title.PartialMatch.Value;
                negativeRankingValue += (resultItem.TitleHits.NoMatch ?? 0) * this.Config.Weights.Title.NoMatch.Value;
            }

            if (resultItem.DescriptionHits != null)
            {
                rankingValue +=
                    (resultItem.DescriptionHits.ExactMatch ?? 0) * this.Config.Weights.Description.ExactMatch.Value +
                    (resultItem.DescriptionHits.Match ?? 0) * this.Config.Weights.Description.Match.Value +
                    (resultItem.DescriptionHits.PartialMatch ?? 0) * this.Config.Weights.Description.PartialMatch.Value;
                negativeRankingValue += (resultItem.DescriptionHits.NoMatch ?? 0) * this.Config.Weights.Description.NoMatch.Value;
            }

            return rankingValue - negativeRankingValue;
        }

         /// <summary>
        /// Remove as palavras ignoradas do texto informado.
        /// </summary>
        /// <param name="word">Termo a ser analisado</param>
        /// <returns>Termo processado, com as palavras ignoradas removidas do termo</returns>
        private String ExcludeIgnoredWords(String word)
        {
            List<String> wordsIgnored = new List<String>();
            return ExcludeIgnoredWords(word, out wordsIgnored);
        }

        /// <summary>
        /// Remove as palavras ignoradas do texto informado.
        /// </summary>
        /// <param name="word">Termo a ser analisado</param>
        /// <param name="wordsIgnored">Palavras ignoradas que foram removidas do termo</param>
        /// <returns>Termo processado, com as palavras ignoradas removidas do termo</returns>
        private String ExcludeIgnoredWords(String word, out List<String> wordsIgnored)
        {
            wordsIgnored = new List<String>();

            //percorre cada palavra ignorada informada na configuração
            foreach (String ignoredWord in this.IgnoredWordsConfig)
            {
                //gera regex para encontrar cada palavra informada
                Regex regex = new Regex("\\b(?<IGNORADA>" + AccentuationConfig.GerarRegex(ignoredWord) + ")\\b", RegexOptions.IgnoreCase);

                //encontrou a palavra ignorada no termo
                if (regex.IsMatch(word))
                {
                    //encontra a palavra ignorada
                    String palavraIgnorada = regex.Match(word).Groups["IGNORADA"].Value;

                    //remove a palavra ignorada do termo e adiciona à coleção de retorno
                    wordsIgnored.Add(palavraIgnorada);
                    word = regex.Replace(word, String.Empty);
                }
            }

            return RemoveDuplicatedSpaces(word);
        }

        /// <summary>
        /// Corrige as palavras do termo pesquisado.
        /// </summary>
        /// <param name="term">Termo pesquisado</param>
        /// <param name="correctionsApplied">Palavras que foram corrigidas</param>
        /// <returns>Termo processado, com as correções das palavras</returns>
        private String CorrectWords(String term, out Dictionary<String, String> correctionsApplied)
        {
            correctionsApplied = new Dictionary<String, String>();

            //percorre cada palavra que deve ser corrigida
            foreach (String word in this.CorrectionsConfig.Keys)
            {
                //regex para encontrar as palavras incorretas
                Regex regexWrongWord = new Regex("\\b(?<INCORRETA>" + String.Join("|", AccentuationConfig.GerarRegex(CorrectionsConfig[word])) + ")\\b", RegexOptions.IgnoreCase);

                //encontrou palavra incorreta
                if (regexWrongWord.IsMatch(term))
                {
                    //substitui a palavra incorreta com a palavra correta
                    String palavraIncorreta = regexWrongWord.Match(term).Groups["INCORRETA"].Value;
                    correctionsApplied.Add(palavraIncorreta, word);
                    term = regexWrongWord.Replace(term, word);
                }
            }
            return term;
        }

        /// <summary>
        /// Inclui os sinônimos no termo pesquisado.
        /// </summary>
        /// <param name="term">Termo pesquisado</param>
        /// <param name="synonymsFound">Lista de sinônimos aplicados ao termo</param>
        /// <returns>Termo processado, com os sinônimos incluídos no termo</returns>
        private String IncludeSynonyms(String term, out Dictionary<String, List<String>> synonymsFound)
        {
            synonymsFound = new Dictionary<String, List<String>>();

            //regex para identificar espaços
            Regex regexSpaces = new Regex("\\s+");

            //percorre cada lista de sinônimos
            foreach (List<String> synonym in this.SynonymsConfig)
            {
                //regex para encontrar um sinônimo
                Regex regexSynonym = new Regex("\\b(?<SINONIMO>" + String.Join("|", AccentuationConfig.GerarRegex(synonym)) + ")\\b", RegexOptions.IgnoreCase);

                //encontrou uma palavra que possui sinônimo
                if (regexSynonym.IsMatch(term))
                {
                    //substitui a palavra pela coleção de sinônimos
                    String wordFound = regexSynonym.Match(term).Groups["SINONIMO"].Value;
                    term = regexSynonym.Replace(term, "(" + String.Join("|", synonym.Select(s => regexSpaces.Replace(s, "#"))) + ")");
                    synonymsFound.Add(wordFound, synonym.Except(new[] { wordFound }).ToList());
                }
            }

            return term;
        }

        #endregion

        #region [ Métodos Auxiliares Privados Estáticos ]

        /// <summary>
        /// Prepara o termo, removendo espaços duplicados e convertendo-o para caixa baixa.
        /// </summary>
        /// <param name="term">Termo a ser analisado</param>
        /// <returns>Termo formatado</returns>
        private static String NormalizeTerm(String term)
        {
            return RemoveDuplicatedSpaces(term ?? String.Empty).Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Remoção de espaços duplicados.
        /// </summary>
        /// <param name="term">Termo</param>
        /// <returns>Termo sem espaços duplicados</returns>
        private static String RemoveDuplicatedSpaces(String term)
        {
            return Regex.Replace(term ?? String.Empty, "[ ]{2,}", " ");
        }

        #endregion
    }
}
