using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Microsoft.ApplicationServer.Caching;
using Redecard.PN.Comum.GZip;
using System.ComponentModel;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe para armazenamento e recuperação de objetos do AppFabric Cache.
    /// </summary>
    /// <remarks>
    /// Para realização de consultas paginadas com acesso ao Mainframe, pode-se utilizar o seguinte padrão:
    /// <code lang="cs">            
    /// //Variáveis utilizadas na pesquisa
    /// Boolean possuiMaisRegistros = false;
    /// Decimal codUltimoProcesso = default(Decimal);    
    /// Dictionary&lt;String, Object&gt; parametros = new Dictionary&lt;String, Object&gt;();
    ///
    /// //Atribuição de retorno padrão
    /// codigoRetorno = 0;
    /// quantidadeTotalRegistrosCache = 0;
    ///
    /// //Verifica se há necessidade de buscar a pesquisa no mainframe
    /// while (CacheAdmin.DeveExecutarPesquisa&lt;Int32&gt;(IdPesquisa, registroInicial, quantidadeRegistrosPesquisar, out parametros))
    /// {
    ///     //Recuperação dos parâmetros de saída da última consulta
    ///     if (parametros != null)    
    ///         codProcesso = (Decimal)parametros["codUltimoProcesso"];    
    ///
    ///     //Executa consulta no mainframe
    ///     List&lt;Int32&gt; retorno = negocio.Consultar(codEstabelecimento, ref codUltimoProcesso, out possuiMaisRegistros, out codigoRetorno);
    ///
    ///     //Em caso de erro ou sem dados de retorno, retorna lista vazia
    ///     if (codigoRetorno != 0 || retorno == null || retorno.Count == 0)
    ///         return new Int32[] {};
    ///
    ///     //Armazena parâmetros de volta no cache
    ///     parametros = new Dictionary&lt;string, object&gt;();
    ///     parametros["codUltimoProcesso"] = codUltimoProcesso;
    ///
    ///     //Atualiza a lista de dados no cache
    ///     CacheAdmin.AtualizarPesquisa&lt;Int32&gt;(IdPesquisa, retorno, possuiMaisRegistros, parametros);
    /// }
    ///
    /// //Retorna os dados do cache
    /// List&lt;Int32&gt; dados = CacheAdmin.RecuperarPesquisa&lt;Int32&gt;(
    ///     IdPesquisa, registroInicial, quantidadeRegistrosRetornar, out quantidadeTotalRegistrosCache);
    ///
    /// //Converte para modelo de serviço
    /// return this.PreencherModeloServico(dados).ToList();
    /// </code>
    /// </remarks>
    public partial class CacheAdmin
    {
        #region [ Propriedades - Singletons ]

        private static DataCacheFactory _dataCacheFactory = null;
        private static DataCacheFactory DataCacheFactory
        {
            get
            {
                if (_dataCacheFactory == null)
                {
                    List<DataCacheServerEndpoint> servers = new List<DataCacheServerEndpoint>();

                    String servidoresCache = ConfigurationManager.AppSettings["ServidorAppFabricCache"].ToString();
                    String[] servidoresCacheSplit = servidoresCache.Split('|');

                    foreach (String servidor in servidoresCacheSplit)
                        servers.Add(new DataCacheServerEndpoint(servidor, 22233));

                    DataCacheFactoryConfiguration configuration = new DataCacheFactoryConfiguration();
                    configuration.Servers = servers;
                    configuration.LocalCacheProperties = new DataCacheLocalCacheProperties();

                    // As propriedades abaixo foram definidas devido ao HIS(Mainframe) - Informações de Extrato muito grande
                    configuration.TransportProperties.MaxBufferPoolSize = Int32.MaxValue;
                    configuration.TransportProperties.MaxBufferSize = Int32.MaxValue;
                    configuration.TransportProperties.ReceiveTimeout = new TimeSpan(0, 10, 0);
                    configuration.TransportProperties.ChannelInitializationTimeout = new TimeSpan(0, 10, 0);

                    DataCacheSecurity security;

                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["AplicarSegurancaCache"]))
                        security = new DataCacheSecurity(DataCacheSecurityMode.Transport,
                            DataCacheProtectionLevel.EncryptAndSign);
                    else
                        security = new DataCacheSecurity(DataCacheSecurityMode.None,
                            DataCacheProtectionLevel.None);

                    DataCacheClientLogManager.ChangeLogLevel(System.Diagnostics.TraceLevel.Off);

                    configuration.SecurityProperties = security;

                    _dataCacheFactory = new DataCacheFactory(configuration);
                }
                return _dataCacheFactory;
            }
        }

        private static Object lockObj = new Object();

        private static Dictionary<String, DataCache> _dataCaches = null;
        private static Dictionary<String, DataCache> DataCaches
        {
            get { return _dataCaches ?? (_dataCaches = new Dictionary<String, DataCache>()); }
        }

        #endregion

        #region [ Adição de Objetos no Cache ]
        
        /// <summary>
        /// Adiciona objeto no Cache Geral/default.
        /// </summary>
        /// <param name="chave">Chave</param>
        /// <param name="objeto">Objeto que será armazenado</param>
        public static void Adicionar<T>(String chave, T objeto)
        {            
            var cache = ObterCache(Cache.Geral);
            cache.Add(chave, objeto);                
        }

        /// <summary>
        /// Adiciona objeto no Cache Geral/default.
        /// </summary>
        /// <param name="chave">Chave</param>
        /// <param name="objeto">Objeto que será armazenado</param>
        /// <param name="tempo">Tempo que o objeto ficará armazenado em cache</param>
        public static void Adicionar<T>(String chave, T objeto, TimeSpan tempo)
        {
            var cache = ObterCache(Cache.Geral);
            cache.Add(chave, objeto, tempo);
        }

        /// <summary>
        /// Adiciona objeto no Cache.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto</typeparam>
        /// <param name="cache">Cache onde o objeto será adicionado</param>
        /// <param name="chave">Chave do objeto no cache</param>
        /// <param name="objeto">Objeto que será armazenado</param>
        /// <param name="tempo">[Opcional] Tempo para expiração</param>
        public static void Adicionar<T>(Cache cache, String chave, T objeto, TimeSpan? tempo = null)
        {
            //Obtém o cache
            var dataCache = ObterCache(cache);

            //Se for Geral/default, é necessário serializar o objeto
            if (cache == Cache.Geral)
            {
                //variável "conteudo" contém objeto serializado, se objeto não for nulo
                object conteudo = null;
                if (objeto != null)
                    conteudo = Util.SerializarDado(objeto);

                //Adiciona objeto ao cache
                if (tempo.HasValue)
                    dataCache.Add(chave, conteudo, tempo.Value);
                else
                    dataCache.Add(chave, conteudo);
            }
            else
            {
                //Adiciona objeto ao cache
                if (tempo.HasValue)
                    dataCache.Add(chave, objeto, tempo.Value);
                else
                    dataCache.Add(chave, objeto);
            }
        }

        #endregion

        #region [ Alteração de Objetos no Cache ]

        /// <summary>
        /// Alterar valor no cache Geral/default.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="chave">Chave</param>
        /// <param name="objeto">Objeto alterado</param>
        public static void Alterar<T>(String chave, T objeto)
        {
            var cache = ObterCache(Cache.Geral);
            cache.Put(chave, objeto);
        }

        /// <summary>Alterar o valor de um objeto no cache.</summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="cache">Cache que contém o objeto</param>
        /// <param name="chave">Chave identificadora do objeto no cache</param>
        /// <param name="objeto">Objeto que será atualizado no cache</param>
        /// <param name="tempo">[Opcional] Tempo para expiração do objeto no cache</param>
        public static void Alterar<T>(Cache cache, String chave, T objeto, TimeSpan? tempo = null)
        {
            //Obtém o cache
            var dataCache = ObterCache(cache);

            //Se for cache Geral/default, precisa serializar o objeto
            if (cache == Cache.Geral)
            {
                //Variável "conteudo" conterá o objeto serializado, case objeto não seja nulo
                object conteudo = null;
                if (objeto != null)
                    conteudo = Util.SerializarDado(objeto);

                //Atualiza objeto no cache
                if (tempo.HasValue)
                    dataCache.Put(chave, conteudo, tempo.Value);
                else
                    dataCache.Put(chave, conteudo);
            }
            else
            {
                //Atualiza objeto no cache
                if (tempo.HasValue)
                    dataCache.Put(chave, objeto, tempo.Value);
                else
                    dataCache.Put(chave, objeto);
            }
        }

        #endregion

        #region [ Recuperação de Objetos do Cache ]

        /// <summary>
        /// Recupera valor do objeto do cache Geral/default.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto</typeparam>
        /// <param name="chave">Chave</param>
        /// <returns>Tipo de objeto passado</returns>
        public static T Recuperar<T>(String chave)
        {
            var cache = ObterCache(Cache.Geral);
            return (T)cache.Get(chave);
        }

        /// <summary>Recupera o valor de um objeto do cache</summary>
        /// <typeparam name="T">Tipo de objeto</typeparam>
        /// <param name="cache">Cache onde está armazenado o objeto</param>
        /// <param name="chave">Chave do objeto no cache</param>
        /// <returns>Objeto armazenado em cache</returns>
        public static T Recuperar<T>(Cache cache, String chave)
        {
            //Obtém o cache
            var dataCache = ObterCache(cache);

            //Se for cache Geral/default, deve de-serializar o objeto, pois foi serializado ao ser adicionado no cache
            if (cache == Cache.Geral)
            {
                Byte[] conteudo = (Byte[])dataCache.Get(chave);
                if (conteudo != null)
                    return Util.DeserializarDado<T>(conteudo);
                else
                    return default(T);
            }
            else
                return (T)dataCache.Get(chave);
        }

        #endregion

        #region [ Remoção de Objetos do Cache ]

        /// <summary>
        /// Remove objeto do cache Geral/default.
        /// </summary>
        /// <param name="chave">Chave</param>
        /// <returns>TRUE / FALSE</returns>
        public static Boolean Remover(String chave)
        {
            try
            {
                var cache = ObterCache(Cache.Geral);            
                return cache.Remove(chave);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Remoção de objeto do cache.
        /// </summary>
        /// <param name="cache">Cache onde o objeto está armazenado</param>
        /// <param name="chave">Chave identificador do objeto no cache</param>
        public static Boolean Remover(Cache cache, String chave)
        {
            try
            {
                //Obtém o cache
                var dataCache = ObterCache(cache);

                //Remove objeto do cache
                return dataCache.Remove(chave);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region [ Métodos Auxiliares - Gerenciamento de Cache ]
               
        /// <summary>Obtém um DataCache referente à um determinado cache.</summary>
        /// <param name="cache">Cache a ser retornado</param>
        /// <returns>DataCache do cache solicitado</returns>
        private static DataCache ObterCache(Cache cache)
        {
            String cacheName = cache.GetDescription();

            lock (lockObj)
            {
                if (!DataCaches.ContainsKey(cacheName))
                {
                    if (cache == Cache.Geral)
                        DataCaches.Add(cacheName, DataCacheFactory.GetDefaultCache());
                    else
                        DataCaches.Add(cacheName, DataCacheFactory.GetCache(cacheName));
                }
            }
            return DataCaches[cacheName];
        }

        /// <summary>
        /// Retorna uma lista contendo as chaves dos objetos presentes 
        /// nas "SystemRegions" de um determinado cache.        
        /// </summary>
        /// <param name="cache">Cache</param>
        /// <returns>Lista de chaves dos objetos do cache</returns>
        public static List<String> ObterObjetos(Cache cache)
        {
            //Obtém o cache
            var dataCache = ObterCache(cache);

            //Retorna todas as chaves dos objetos presentes no cache (nas SystemRegions)
            return dataCache.GetSystemRegions()
                .SelectMany(region => dataCache.GetObjectsInRegion(region))
                .Select(obj => obj.Key)
                .ToList();
        }

        /// <summary>
        /// Retorna a coleção/enumeração de caches.
        /// </summary>
        /// <returns>Enumeração de cache</returns>
        public static Dictionary<Cache, String> ObterCaches()
        {
            Dictionary<Cache, String> retorno = new Dictionary<Cache, String>();
            var caches = Enum.GetValues(typeof(Cache)).Cast<Cache>();
            foreach (Cache cache in caches)
                retorno.Add(cache, cache.GetDescription());
            return retorno;
        }

        /// <summary>
        /// Limpa os objetos armazenados em determinado cache.
        /// </summary>
        /// <param name="cache">Cache que será limpado</param>
        /// <returns>TRUE: cache limpo com sucesso. FALSE: erro ou cache inexistente</returns>
        public static Boolean LimparCache(Cache cache)
        {
            try
            {
                //Obtém o cache
                DataCache dataCache = ObterCache(cache);
                
                if (dataCache != null)
                {
                    //Se for Geral, limpa todos os objetos
                    if (cache == Cache.Geral)
                    {
                        var objetosGeral = ObterObjetos(Cache.Geral);
                        foreach (String objetoGeral in objetosGeral)
                            Remover(Cache.Geral, objetoGeral);
                        return true;
                    }
                    else
                    {
                        //Listagem das SystemRegions
                        IEnumerable<String> regions = dataCache.GetSystemRegions();

                        //Para cada region, executa limpeza
                        if (regions != null)
                            foreach (var region in regions)
                                dataCache.ClearRegion(region);

                        //Sucesso na limpeza do cache
                        return true;
                    }
                }
                else //cache não encontrado                
                    return false;
            }
            catch
            {
                //erro: retorna falso
                return false;
            }
        }   
              
        #endregion

        #region [ Cache de Pesquisa ]

        #region [ Cache de Pesquisa - Métodos Estáticos Privados ]

#if DEBUG
        //Para desenvolvimento, não usa o AppFabric Cache, e sim, uma "pseudo-sessão" (dicionário estático)
        private static Dictionary<String, Object> _session;
        private static Dictionary<String, Object> Session
        {
            get
            {
                if (_session == null)
                    _session = new Dictionary<string, object>();
                return _session;
            }
        }
#endif

        /// <summary>Retorna o objeto pesquisa armazenado com o IdPesquisa informado</summary>
        /// <typeparam name="T">Tipo dos itens da coleção</typeparam>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <returns>Objeto pesquisa</returns>
        private static CachePesquisa<T> GetCachePesquisa<T>(Cache cache, Guid IdPesquisa)
        {
#if DEBUG
            if (Session.ContainsKey(IdPesquisa.ToString()))
                return (CachePesquisa<T>)Session[IdPesquisa.ToString()];
            else
                return null;
#else
            return CacheAdmin.Recuperar<CachePesquisa<T>>(cache, IdPesquisa.ToString());
#endif
        }

        /// <summary>Atualiza a pesquisa no cache</summary>
        /// <typeparam name="T">Tipo dos itens da coleção</typeparam>
        /// <param name="cachePesquisa">Objeto que será armazenado no cache</param>
        private static void SetCachePesquisa<T>(Cache cache, CachePesquisa<T> cachePesquisa)
        {
#if DEBUG
            Session[cachePesquisa.IdPesquisa.ToString()] = cachePesquisa;
#else
            if (CacheAdmin.PossuiPesquisa<T>(cache, cachePesquisa.IdPesquisa))
                CacheAdmin.Alterar(cache, cachePesquisa.IdPesquisa.ToString(), cachePesquisa);
            else
                CacheAdmin.Adicionar(cache, cachePesquisa.IdPesquisa.ToString(), cachePesquisa);
#endif
        }

        #endregion
        
        /// <summary>Verifica se a pesquisa já está presente no cache</summary>
        /// <typeparam name="T">Tipo dos itens da coleção</typeparam>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <returns>TRUE: pesquisa existe no cache; FALSE: pesquisa nunca foi armazenada no cache, ou expirou</returns>
        public static Boolean PossuiPesquisa<T>(Cache cache, Guid IdPesquisa)
        {
            return GetCachePesquisa<T>(cache, IdPesquisa) != null;
        }

        /// <summary>Verifica se é necessário executar pesquisa ou se os dados solicitados já estão em cache</summary>
        /// <seealso cref="Redecard.PN.Comum.CacheAdmin.AtualizarPesquisa"/>
        /// <seealso cref="Redecard.PN.Comum.CacheAdmin.RecuperarPesquisa"/>
        /// <typeparam name="T">Tipo dos itens da coleção</typeparam>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="indiceRegistro">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="parametros">Parâmetros auxiliares para execução da próxima consulta</param>
        /// <returns>TRUE: necessário executar pesquisa; FALSE: pesquisa já está no cache, pode ser recuperada diretamente</returns>
        public static Boolean DeveExecutarPesquisa<T>(Cache cache, Guid IdPesquisa, Int32 indiceRegistro, Int32 quantidadeRegistros, out Dictionary<String, Object> parametros)
        {
            //Retorno default
            parametros = null;

            quantidadeRegistros++;
            if (quantidadeRegistros < 0)
                quantidadeRegistros = Int32.MaxValue;

            //Verifica se possui a pesquisa no cache. Se não possui, precisa buscar no cache
            CachePesquisa<T> cachePesquisa = GetCachePesquisa<T>(cache, IdPesquisa);
            if (cachePesquisa == null)
                return true;

            //Parâmetros de saída da última consulta realizada
            parametros = cachePesquisa.Parametros;

            //Verifica se o intervalo já está no cache. Se não estiver, indica que é necessário executar pesquisa
            Int32 registroInicial = indiceRegistro;
            Int32 registroFinal = indiceRegistro + quantidadeRegistros;
            if (registroInicial < cachePesquisa.QuantidadeRegistros && registroFinal < cachePesquisa.QuantidadeRegistros)
                return false;

            //Se não existem mais registros a serem retornados, não precisa executar pesquisa
            if (!cachePesquisa.PossuiMaisRegistros)
                return false;
            //Caso contrário, existem dados que ainda não foram cacheados
            else
                return true;
        }

        /// <summary>Atualiza os dados do cache da pesquisa</summary>
        /// <seealso cref="Redecard.PN.Comum.CacheAdmin.DeveExecutarPesquisa"/>
        /// <seealso cref="Redecard.PN.Comum.CacheAdmin.RecuperarPesquisa"/>
        /// <typeparam name="T">Tipo dos itens da coleção de registros</typeparam>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="dados">Dados a serem incluídos no cache da pesquisa</param>
        /// <param name="possuiMaisRegistros">Flag indicando se existirão mais registros para consulta e adição ao cache</param>
        /// <param name="parametros">Parâmetros auxiliares para execução da próxima consulta</param>
        /// <param name="cache">Cache que será consultado</param>
        /// <param name="validaQuantidadeRegistros">Valida se quantidade de registros é maior que 0</param>
        public static void AtualizarPesquisa<T>(Cache cache, Guid IdPesquisa, List<T> dados, Boolean possuiMaisRegistros, Dictionary<String, Object> parametros, Boolean validaQuantidadeRegistros)
        {
            //Busca o cache da pesquisa; Se não possui cache para a pesquisa, cria
            CachePesquisa<T> cachePesquisa = GetCachePesquisa<T>(cache, IdPesquisa) ?? new CachePesquisa<T>(IdPesquisa);

            //Cria a nova página de pesquisa
            CachePagina<T> novaPagina = new CachePagina<T>();
            novaPagina.Dados = dados.ToArray();
            novaPagina.RegistroInicial = cachePesquisa.QuantidadeRegistros;

            //Inclui a nova página na pesquisa
            cachePesquisa.Paginas.Add(novaPagina);

            //Atualiza informações para próxima consulta, se não possui mais registros, considera que não possui mais registros            
            cachePesquisa.PossuiMaisRegistros = possuiMaisRegistros && (!validaQuantidadeRegistros || dados.Count > 0);

            //Armazena parâmetros da última consulta
            cachePesquisa.Parametros = parametros;

            //Atualiza o cache da pesquisa no AppFabric
            SetCachePesquisa(cache, cachePesquisa);
        }

        /// <summary>Atualiza os dados do cache da pesquisa</summary>
        /// <seealso cref="Redecard.PN.Comum.CacheAdmin.DeveExecutarPesquisa"/>
        /// <seealso cref="Redecard.PN.Comum.CacheAdmin.RecuperarPesquisa"/>
        /// <typeparam name="T">Tipo dos itens da coleção de registros</typeparam>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="dados">Dados a serem incluídos no cache da pesquisa</param>
        /// <param name="possuiMaisRegistros">Flag indicando se existirão mais registros para consulta e adição ao cache</param>
        /// <param name="parametros">Parâmetros auxiliares para execução da próxima consulta</param>
        /// <param name="cache">Cache que será consultado</param>        
        public static void AtualizarPesquisa<T>(Cache cache, Guid IdPesquisa, List<T> dados, Boolean possuiMaisRegistros, Dictionary<String, Object> parametros)
        {
            AtualizarPesquisa(cache, IdPesquisa, dados, possuiMaisRegistros, parametros, true);
        }

        /// <summary>Recupera todos os registros da pesquisa</summary>
        /// <seealso cref="Redecard.PN.Comum.CacheAdmin.AtualizarPesquisa"/>
        /// <seealso cref="Redecard.PN.Comum.CacheAdmin.DeveExecutarPesquisa"/>
        /// <typeparam name="T">Tipo dos itens da coleção</typeparam>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <returns>Lista de itens da coleção</returns>
        public static List<T> RecuperarPesquisa<T>(Cache cache, Guid IdPesquisa)
        {
            //Verifica se possui a pesquisa no cache            
            CachePesquisa<T> cachePesquisa = GetCachePesquisa<T>(cache, IdPesquisa);
            if (cachePesquisa == null)
                return null;

            //Obtém os dados da pesquisa
            return cachePesquisa.Recuperar();
        }

        /// <summary>Recupera os registros da pesquisa</summary>
        /// <seealso cref="Redecard.PN.Comum.CacheAdmin.AtualizarPesquisa"/>
        /// <seealso cref="Redecard.PN.Comum.CacheAdmin.DeveExecutarPesquisa"/>
        /// <typeparam name="T">Tipo dos itens da coleção</typeparam>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeRegistrosTotalCache">Quantidade de registros totais em cache</param>
        /// <returns>Lista de itens da coleção</returns>
        public static List<T> RecuperarPesquisa<T>(Cache cache, Guid IdPesquisa, Int32 registroInicial, Int32 quantidadeRegistros, out Int32 quantidadeRegistrosTotalCache)
        {
            quantidadeRegistrosTotalCache = 0;

            //Verifica se possui a pesquisa no cache            
            CachePesquisa<T> cachePesquisa = GetCachePesquisa<T>(cache, IdPesquisa);
            if (cachePesquisa == null)
                return null;

            //Obtém os dados da pesquisa
            quantidadeRegistrosTotalCache = cachePesquisa.QuantidadeRegistros;
            return cachePesquisa.Recuperar(registroInicial, quantidadeRegistros);
        }
        
        /// <summary>Classe para abstração de uma Pesquisa no Cache.
        /// É composta por um conjunto sequencial de páginas.</summary>
        /// <typeparam name="T">Tipo genérico armazenado na pesquisa.</typeparam>
        [Serializable]
        internal class CachePesquisa<T>
        {
            /// <summary>Id da Pesquisa</summary>
            public Guid IdPesquisa { get; set; }

            /// <summary>Parâmetro auxiliar que deve ser informado no momento da inclusão de uma página na pesquisa (Atualizar),
            /// para que seja possível ao cache identificar se há necessidade de solicitar uma nova página da pesquisa.</summary>
            public Boolean PossuiMaisRegistros { get; set; }

            /// <summary>Parâmetros adicionais que são mantidos em cache, 
            /// para execução das consultas/páginas subsequentes e consequente incrementação dos dados armazenados.</summary>
            public Dictionary<String, Object> Parametros { get; set; }

            private List<CachePagina<T>> _paginas;
            /// <summary>Páginas de dados que compõem a pesquisa.</summary>            
            public List<CachePagina<T>> Paginas
            {
                get
                {
                    if (_paginas == null)
                        _paginas = new List<CachePagina<T>>();
                    return _paginas;
                }
                set
                {
                    _paginas = value;
                }
            }

            /// <summary>Quantidade Total de registros armazenado na pesquisa.</summary>
            public Int32 QuantidadeRegistros { get { return Paginas.Sum(pagina => pagina.QuantidadeRegistros); } }

            /// <summary>Construtor padrão</summary>
            /// <param name="IdPesquisa">Id da Pesquisa</param>
            public CachePesquisa(Guid IdPesquisa)
            {
                this.IdPesquisa = IdPesquisa;
            }

            /// <summary>Retorna um intervalo de dados da lista.</summary>
            /// <param name="registroInicial">Índice do registro inicial (zero-based index).</param>
            /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados.</param>            
            /// <returns>Lista contendo os dados do intervalo solicitado</returns>
            public List<T> Recuperar(Int32 registroInicial, Int32 quantidadeRegistros)
            {
                Int32 registroFinal = registroInicial + quantidadeRegistros;

                //Lista de retorno
                List<T> dados = new List<T>();

                //Percorre todas as páginas, retornando o intervalo de dados solicitado
                foreach (CachePagina<T> pagina in this.Paginas)
                {
                    //If para verificar se o intervalo solicitado é iniciado na página atual
                    if (pagina.PossuiRegistro(registroInicial))
                    {
                        T[] dadosPagina = pagina.Dados;
                        dados.AddRange(dadosPagina
                            .Skip(registroInicial - pagina.RegistroInicial)
                            .Take(quantidadeRegistros));
                    }
                    //If para verificar se a página atual está dentro do intervalo solicitado
                    else if (registroInicial <= pagina.RegistroInicial && registroFinal >= pagina.RegistroFinal)
                    {
                        T[] dadosPagina = pagina.Dados;
                        dados.AddRange(dadosPagina);
                    }
                    //If para verificar se o intervalo solicitado é finalizado na página atual
                    else if (pagina.PossuiRegistro(registroFinal))
                    {
                        T[] dadosPagina = pagina.Dados;
                        dados.AddRange(dadosPagina);                            
                    }

                    //Se já atingiu a quantidade de registros necessária, interrompe laço foreach
                    if(dados.Count >= quantidadeRegistros)
                        break;
                }

                //Retorna os dados selecionados, garantindo que o número máximo não seja extrapolado
                return dados.Take(quantidadeRegistros).ToList();
            }

            /// <summary>Retorna o Array completo de dados na pesquisa.</summary>            
            /// <remarks>Array completo dos dados da pesquisa</remarks>
            public List<T> Recuperar()
            {
                //Obtém os dados da pesquisa
                List<T> dados = new List<T>();
                foreach (CachePagina<T> pagina in this.Paginas)
                    dados.AddRange(pagina.Dados);
                return dados;
            }
        }

        /// <summary>Representa uma página de uma pesquisa.</summary>
        /// <typeparam name="T">Tipo genérico da coleção de objetos armazenada na página.</typeparam>
        [Serializable]
        internal class CachePagina<T>
        {
            private byte[] _dados;
            /// <summary>Array de dados. Internamente é realizada a compactação do array.</summary>
            public T[] Dados
            {
                get
                {
                    if (_dados == null)
                        return null;
                    return Util.DeserializarDados<T>(GZip.Helper.DescompactarConteudo(_dados));
                }
                set
                {
                    _quantidadeRegistros = value.Length;
                    _dados = GZip.Helper.CompactarConteudo(Util.SerializarDados(value));
                }
            }

            private Int32 _registroInicial;
            /// <summary>Índice do primeiro registro</summary>
            public Int32 RegistroInicial
            {
                get { return _registroInicial; }
                set { _registroInicial = value; }
            }

            private Int32 _quantidadeRegistros;
            /// <summary>Quantidade de registros contidos na página.</summary>
            public Int32 QuantidadeRegistros
            {
                get
                {
                    return _quantidadeRegistros;
                }
            }

            public Int32 RegistroFinal
            {
                get
                {
                    return _registroInicial + _quantidadeRegistros;
                }
            }

            public Boolean PossuiRegistro(Int32 indiceRegistro)
            {
                return indiceRegistro >= RegistroInicial && indiceRegistro <= RegistroFinal;
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// Enumerador para as áreas de cache existentes
    /// </summary>
    public enum Cache
    {
        /// <summary>
        /// Cache Geral
        /// Nome do cache AppFabric: default        
        /// </summary>
        /// <remarks>Utilizado em:
        ///     <list type="bullet">
        ///         <item><description>Módulo Dados Cadastrais: login - consulta de grupos de Estabelecimento (ComboBox)</description></item>
        ///     </list>
        /// </remarks>
        [Description("default")]
        Geral,

        /// <summary>
        /// Cache de Sessão (AppFabric Session State Provider).
        /// Nome do cache AppFabric: RedecardPNSession
        /// </summary>        
        [Description("RedecardPNSession")]        
        Sessao,

        /// <summary>
        /// Cache do módulo Extrato
        /// Nome do cache AppFabric: RedecardPNExtrato
        /// </summary>
        /// <remarks>Utilizado em:
        ///     <list type="bullet">
        ///         <item><description>Módulo Extrato: consulta de Relatórios</description></item>    
        ///     </list>
        /// </remarks>
        [Description("RedecardPNExtrato")]
        Extrato,

        /// <summary>
        /// Cache do módulo Request/Chargeback
        /// Nome do cache AppFabric: RedecardPNRequest
        /// </summary>
        /// <remarks>Utilizado em:
        ///     <list type="bullet">
        ///         <item><description>Módulo Request: consulta de Histórico de Comprovantes</description></item>
        ///         <item><description>Módulo Request: consulta de Avisos de Débito</description></item>
        ///         <item><description>Módulo Request: consulta de Comprovantes Pendentes</description></item>
        ///     </list>
        /// </remarks>
        [Description("RedecardPNRequest")]
        Request,
       
        /// <summary>
        /// Cache do módulo Home Page Vendedora
        /// Nome do cache AppFabric: RedecardPNHome
        /// </summary>
        /// <remarks>Utilizado em:
        ///     <list type="bullet">
        ///         <item><description>Módulo Extrato: Home - Lançamentos Futuros</description></item>
        ///         <item><description>Módulo Extrato: Home - Últimas Vendas</description></item>
        ///         <item><description>Módulo RAV: RAV Disponível</description></item>
        ///     </list>
        /// </remarks>
        [Description("RedecardPNHome")]
        Home,

        /// <summary>
        /// Cache de Consulta de Filiais
        /// Nome do cache AppFabric: RedecardPNFiliais
        /// </summary>
        /// <remarks>Utilizado em:
        ///     <list type="bullet">
        ///         <item><description>Módulo Comum: consulta de filiais</description></item>
        ///     </list>
        /// </remarks>
        [Description("RedecardPNFiliais")]
        Filiais,

        /// <summary>
        /// Cache de Sessão DataCash (AppFabric Session State Provider)
        /// Nome do cache AppFabric: RedecardPNDataCashSession
        /// </summary>
        /// <remarks>Utilizado na integração entre Portal Redecard PN e DataCash</remarks>
        [Description("RedecardPNDataCashSession")]
        DataCashSessao,

        /// <summary>
        /// Cache de Sessão de Integração DataCash vs. Portal        
        /// Nome do cache AppFabric: RedecardPNDataCashIntegracao
        /// </summary>
        /// <remarks>Utilizado na integração entre Portal Redecard PN e DataCash</remarks>
        [Description("RedecardPNDataCashIntegracao")]
        DataCashIntegracao,

        /// <summary>
        /// Cache geral para Datacach
        /// Nome do cache AppFabric: RedecardPNDataCash
        /// </summary>
        [Description("RedecardPNDataCash")]
        DataCash,

        /// <summary>
        /// Cache geral para TravaDomicilio
        /// Nome do cache AppFabric: RedecardPNEmissores
        /// </summary>
        [Description("RedecardPNEmissores")]
        Emissores,

        /// <summary>
        /// Cache geral para Fidelidade
        /// Nome do cache AppFabric: RedecardPNFidelidade
        /// </summary>
        [Description("RedePNFidelidade")]
        Fidelidade
    }
}
