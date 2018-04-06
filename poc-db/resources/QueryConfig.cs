/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model
{
    /// <summary>
    /// Classe para configuração da pesquisa que será realizada
    /// </summary>
    public class QueryConfig
    {
        /// <summary>
        /// Bloqueador utilizado para garantir a sincronia entre threads concorrentes.
        /// </summary>
        private static readonly Object bloqueador = new Object();

        #region [ Constantes ]

        /// <summary>
        /// Caminho da pasta dos arquivos de configuração da busca
        /// </summary>
        public static String ConfigFolder { get { return "AtendimentoDigital/search"; } }

        /// <summary>
        /// Caminho do arquivo de configuração de pesos
        /// </summary>
        public static String ConfigFile { get { return "search-config.txt"; } }

        #endregion

        /// <summary>
        /// Serializador de javascript
        /// </summary>
        private static JavaScriptSerializer jsSerializer;

        /// <summary>
        /// Identificação do dispositivo: "web", "iOS", "Android"
        /// </summary>
        public Device? Device { get; set; }

        /// <summary>
        /// Paginação
        /// </summary>
        public Dictionary<String, Pagination> Pagination { get; set; }

        /// <summary>
        /// Se deve utilizar sinônimos com base no dicionário de sinônimos
        /// </summary>
        public Boolean? UseSynonyms { get; set; }
        
        /// <summary>
        /// Se deve corrigir as palavras automaticamente com base no dicionário de correções
        /// </summary>
        public Boolean? AutoCorrectWords { get; set; }

        /// <summary>
        /// Se deve ignorar as palavras automaticamente com base no dicionário de palavras a serem ignoradas
        /// </summary>
        public Boolean? AutoIgnoreWords { get; set; }

        /// <summary>
        /// Se utiliza promoções de resultados
        /// TODO ASH: nice to have: implementar lógica de promoção de resultados
        /// </summary>
        ///public Boolean UsePromotions { get; set; }

        /// <summary>
        /// Escopos a serem considerados: "categoria", "subcategoria", "perguntaResposta", "servico",
        /// "public/faq", "public/categoria", "public/subcategoria", "public/perguntaResposta"
        /// </summary>
        public List<String> Scopes { get; set; }

        /// <summary>
        /// Flag indicando se deve utilizar cache
        /// </summary>
        public Boolean? UseCache { get; set; }

        /// <summary>
        /// Configuração do peso das ocorrências encontradas
        /// </summary>
        public Weights Weights { get; set; }

        /// <summary>
        /// Construtor estático
        /// </summary>
        public QueryConfig()
        {
            jsSerializer = new JavaScriptSerializer();
        }

        /// Deserializa instância de configuração a partir de um JSON de entrada
        /// </summary>
        /// <param name="serialized">JSON representando o objeto de configuração</param>
        /// <returns>Instância C# do objeto deserializado</returns>
        public static QueryConfig JsonDeserialize(String serialized)
        {
            QueryConfig config = default(QueryConfig);

            if (String.IsNullOrWhiteSpace(serialized))
                return config;
            
            try
            {
                config = jsSerializer.Deserialize<QueryConfig>(serialized);
            }
            catch (InvalidOperationException ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro genérico durante deserialização de configuração da pesquisa", ex, serialized);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro genérico durante deserialização de configuração da pesquisa", ex, serialized);
            }

            return config;
        }

        /// <summary>
        /// Obtém configuração padrão.
        /// </summary>
        /// <returns>Configuração padrão</returns>
        private static QueryConfig GetTemplate()
        {            
            return new QueryConfig()
            {
                AutoCorrectWords = true,
                AutoIgnoreWords = true,
                Device = Model.Device.Web,
                Pagination = new Dictionary<String,Pagination>() {
                    { "perguntaResposta", new Pagination(100, 0) },
                    { "servico", new Pagination(100, 0) },
                    { "card", new Pagination(100, 0) }
                },
                Scopes = new[] { "perguntaResposta", "servico", "card" }.ToList(),
                UseCache = true,
                UseSynonyms = true,
                Weights = new Weights()
                {
                    Title = new Hit()
                    {
                        ExactMatch = 500,
                        Match = 200,                        
                        PartialMatch = 50,
                        NoMatch = 0
                    },
                    Description = new Hit()
                    {
                        ExactMatch = 100,
                        Match = 40,                        
                        PartialMatch = 10,
                        NoMatch = 0
                    }
                }
            };
        }

        /// <summary>
        /// Inicializa propriedades do objeto para configuração padrão.
        /// Copia propriedades do "template" para "config"
        /// </summary>
        /// <returns>Configuração padrão</returns>
        private static QueryConfig Merge(QueryConfig config, QueryConfig template)
        {
            config = config ?? template;
            config.AutoCorrectWords = config.AutoCorrectWords ?? template.AutoCorrectWords;
            config.AutoIgnoreWords = config.AutoIgnoreWords ?? template.AutoIgnoreWords;
            config.Device = config.Device ?? template.Device;
            config.UseSynonyms = config.UseSynonyms ?? template.UseSynonyms;
            config.UseCache = config.UseCache ?? template.UseCache;
            config.Pagination = config.Pagination ?? template.Pagination;
            config.Scopes = config.Scopes ?? template.Scopes;

            foreach(String scope in config.Scopes)
            {
                if (!config.Pagination.ContainsKey(scope))
                    config.Pagination.Add(scope, new Pagination());

                if (template.Pagination.ContainsKey(scope))
                {
                    Pagination templatePagination = template.Pagination[scope];
                    Pagination pagination = config.Pagination[scope];
                    pagination.RowsPerPage = pagination.RowsPerPage ?? templatePagination.RowsPerPage;
                    pagination.StartRow = pagination.StartRow ?? templatePagination.StartRow;
                }   
            }

            config.Weights = config.Weights ?? template.Weights;
            config.Weights.Title = config.Weights.Title ?? template.Weights.Title;
            config.Weights.Description = config.Weights.Description ?? template.Weights.Description;

            config.Weights.Title.ExactMatch = config.Weights.Title.ExactMatch ?? template.Weights.Title.ExactMatch;
            config.Weights.Title.Match = config.Weights.Title.Match ?? template.Weights.Title.Match;
            config.Weights.Title.PartialMatch = config.Weights.Title.PartialMatch ?? template.Weights.Title.PartialMatch;
            config.Weights.Title.NoMatch = config.Weights.Title.NoMatch ?? template.Weights.Title.NoMatch;

            config.Weights.Description.ExactMatch = config.Weights.Description.ExactMatch ?? template.Weights.Description.ExactMatch;
            config.Weights.Description.Match = config.Weights.Description.Match ?? template.Weights.Description.Match;
            config.Weights.Description.PartialMatch = config.Weights.Description.PartialMatch ?? template.Weights.Description.PartialMatch;
            config.Weights.Description.NoMatch = config.Weights.Description.NoMatch ?? template.Weights.Description.NoMatch;

            return config;
        }

        /// <summary>
        /// Obtém configuração default
        /// </summary>
        /// <param name="web">SPWeb</param>
        private static QueryConfig GetDefaultConfig(SPWeb web)
        {
            var cacheKey = "QueryConfig_" + web.ID.ToString();
            var config = default(QueryConfig);

            lock (bloqueador)
            {
                //tenta recuperar do cache
                config = CacheAtendimento.GetItem<QueryConfig>(cacheKey);

                //Se configuração default não está em cache, busca do SharePoint e adiciona em cache
                if (config == default(QueryConfig))
                {
                    //inicializa config com base em template inicial
                    config = GetTemplate();

                    //Recupera referência para do biblioteca AtendimentoDigital/search
                    var repository = new AtendimentoDigitalRepository();
                    String defaultSPConfigFile = repository.GetFileContent(web, ConfigFolder, ConfigFile);

                    //se encontra, deserializa json
                    if (!String.IsNullOrWhiteSpace(defaultSPConfigFile))
                    {
                        QueryConfig spDefaultConfig = QueryConfig.JsonDeserialize(defaultSPConfigFile);
                        if (spDefaultConfig != null)
                        {
                            //realiza o merge com o template
                            config = QueryConfig.Merge(spDefaultConfig, config);
                        }
                    }
                    
                    //adiciona em cache
                    CacheAtendimento.AddItem(cacheKey, config);
                }
            }

            return config;
        }


        #region [ Métodos estáticos públicos ]

        /// <summary>
        /// Obtém a instância da configuração de sinônimos
        /// </summary>
        /// <param name="web">SPWeb</param>
        /// <param name="useCache">Se utiliza cache ou não</param>
        /// <returns>Configuração de sinônimos</returns>
        public static QueryConfig GetConfig(SPWeb web, QueryConfig customConfig)
        {
            return QueryConfig.Merge(customConfig, QueryConfig.GetDefaultConfig(web));
        }

        #endregion
    }
}
