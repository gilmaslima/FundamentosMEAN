using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rede.PN.LoginPortal.CacheService.Models;
using NLog;

namespace Rede.PN.LoginPortal.CacheService.Helper
{
    public sealed class AddCache
    {
        private static Logger logCache = LogManager.GetCurrentClassLogger();

        private static volatile AddCache instance;
        static readonly Object locker = new Object();

        private static AddCache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = new AddCache();
                    }
                }
                return instance;
            }
        }

        public static DataCache GetCache(String cacheName = "default")
        {
            return AddCache.Instance.GetCacheInternal(cacheName);
        }

        private DataCacheFactory factory;

        private AddCache()
        {
            this.factory = new DataCacheFactory();
        }

        private DataCache GetCacheInternal(string cacheName = "default")
        {
            return this.factory.GetCache(cacheName);
        }

        /// <summary>
        /// Retorna o item do DataCache tipado.
        /// </summary>
        /// <typeparam name="T">Tipo do retorno.</typeparam>
        /// <param name="key">Chave para recuperação do cache.</param>
        /// <param name="cacheName">Qual cache será buscado o item.</param>
        /// <returns>Valor do cache tipado.</returns>
        public static T GetCacheItem<T>(String key, String cacheName = "default")
        {
            Object item = null;
            ObjetoCache objCache = default(ObjetoCache);

            try
            {
                DataCache cache = AddCache.GetCache(cacheName);
                objCache = (ObjetoCache)cache.Get(key);

                if (objCache != null)
                {
                    
                    item = objCache;
                                  
                   
                    if (string.IsNullOrEmpty(objCache.NotRenew))
                    {

                        string strTtl = string.IsNullOrEmpty(objCache.Ttl) ? ConfigurationManager.AppSettings["TTLCache"].ToString() : objCache.Ttl;

                        //Força atualização do TTL
                        cache.ResetObjectTimeout(key, TimeSpan.FromMinutes(Convert.ToDouble(strTtl)));
                        
                    } 
                    
                    return (T)item;
                }

                return default(T);
            }
            catch (DataCacheException ex)
            {
                logCache.Error(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                logCache.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Retorna o item de cache tipado.
        /// </summary>
        /// <typeparam name="T">Tipo esperado para o item de cache.</typeparam>
        /// <param name="key">Chave do item no cache.</param>
        /// <param name="lockTimeout">timeout do lock do item.</param>
        /// <param name="lockHandle">Parâmetro com modificador out referente ao objeto de lock que deve ser 
        /// usado para desbloquear o item do cache.</param>
        /// <param name="cacheName">Nome do cache ao qual deseja recuperar o item.</param>
        /// <returns>Valor tipado recuperado do cache.</returns>
        public static T GetCacheItem<T>(String key, TimeSpan lockTimeout, out DataCacheLockHandle lockHandle, String cacheName = "default")
        {
            Object item = null;
            DataCache cache = null;

            try
            {
                cache = AddCache.GetCache(cacheName);
                item = cache.GetAndLock(key, lockTimeout, out lockHandle);
            }
            catch (DataCacheException ex)
            {
                lockHandle = null;
                if (ex.ErrorCode == DataCacheErrorCode.KeyDoesNotExist || ex.ErrorCode == DataCacheErrorCode.ObjectLocked)
                    item = null;
                else
                {
                    logCache.Error(ex);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                logCache.Error(ex);
                throw ex;
            }

            if (item != null)
            {
                //Força atualização do TTL
                cache.ResetObjectTimeout(key, TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["TTLCache"].ToString())));
                return (T)item;
            }

            return default(T);
        }

        /// <summary>
        /// Adiciona ou Atualiza item no cache.
        /// </summary>
        /// <typeparam name="T">Tipagem do item.</typeparam>
        /// <param name="key">Chave do item no cache.</param>
        /// <param name="value">Valor do item no cache.</param>
        /// <param name="cacheName">Nome do cache onde o item será inserido.</param>
        public static void PutCacheItem(String key, ObjetoCache value, TimeSpan ttl, String cacheName = "default")
        {
            try
            {
                DataCache cache = AddCache.GetCache(cacheName);
                cache.Put(key, value, ttl);                
            }
            catch (DataCacheException ex)
            {
                logCache.Error(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                logCache.Error(ex);
                throw ex;
            }
        }
        
        /// <summary>
        /// Adiciona ou Atualiza item no cache.
        /// </summary>
        /// <typeparam name="T">Tipagem do item.</typeparam>
        /// <param name="key">Chave do item no cache.</param>
        /// <param name="value">Valor do item no cache.</param>
        /// <param name="cacheName">Nome do cache onde o item será inserido.</param>
        public static void PutCacheItem<T>(String key, T value, String cacheName = "default")
        {
            try
            {
               DataCache cache = AddCache.GetCache(cacheName);
               //string strTtl = string.IsNullOrEmpty(ttl) ? ConfigurationManager.AppSettings["TTLCache"].ToString() : ttl;
               TimeSpan TTLCache = TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["TTLCache"].ToString()));
               cache.Put(key, value, TTLCache); //Força gravação de todos os itens com o TTL do Config
            }
            catch (DataCacheException ex)
            {
                logCache.Error(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                logCache.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Adiciona ou Atualiza item no cache.
        /// </summary>
        /// <typeparam name="T">Tipagem do item.</typeparam>
        /// <param name="key">Chave do item no cache.</param>
        /// <param name="value">Valor do item no cache.</param>
        /// <param name="timeout">Tempo limite que o item permanecerá no cache.</param>
        /// <param name="cacheName">Nome do cache onde o item será inserido.</param>
        public static void PutCacheItem<T>(String key, T value, TimeSpan timeout, String cacheName = "default")
        {
            DataCache cache = AddCache.GetCache(cacheName);
            cache.Put(key, value, timeout);
        }


         /// <summary>
        /// Atualiza e desbloqueia o item em cache.
        /// </summary>
        /// <typeparam name="T">Tipagem do item.</typeparam>
        /// <param name="key">Chave do item no cache.</param>
        /// <param name="value">Valor do item a ser atualizado.</param>
        /// <param name="lockHandle">Objeto de gerenciamento de lock para desbloqueio do item.</param>
        /// <param name="cacheName">Nome do cache ao qual o item pertence.</param>
        public static void UpdateAndUnLockedCache<T>(String key, T value, DataCacheLockHandle lockHandle, String cacheName = "default")
        {
            DataCache cache = AddCache.GetCache(cacheName);
            cache.PutAndUnlock(key, value, lockHandle);
        }

        /// <summary>
        /// Montagem de chaves de nomes compostos separados por delimitador para transações.
        /// </summary>
        /// <param name="list">Lista tipada com as strings para montagem.</param>
        /// <returns></returns>
        public static String MountAuthorizationKey(List<String> list)
        {
            return String.Join("_", list);
        }

        /// <summary>
        /// Remove item do Cache.
        /// </summary>
        /// <param name="key">Chave do item a ser removido.</param>
        /// <param name="cacheName">De qual cache será removido o item.</param>
        /// <returns>Booleano indicando se foi excluído com sucesso.</returns>
        public static Boolean RemoveCacheItem(String key, String cacheName = "default")
        {
            try
            {
                DataCache cache = AddCache.GetCache(cacheName);
                return cache.Remove(key);
            }
            catch (DataCacheException ex)
            {
                logCache.Error(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                logCache.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Desbloqueio do cache.
        /// </summary>
        /// <param name="key">Chave do item a ser desbloqueado.</param>
        /// <param name="lockHandle">DataCacheLockHandle utilizado no bloqueio.</param>
        /// <param name="cacheName">Nome do cache onde está o item.</param>
        public static void UnlockCache(String key, DataCacheLockHandle lockHandle, String cacheName = "default")
        {
            DataCache cache = AddCache.GetCache(cacheName);
            cache.Unlock(key, lockHandle);
        }

        /// <summary>
        /// Busca todos os itens de determinado Cache.
        /// </summary>
        /// <param name="cacheName">Qual o CacheStore será realizada a pesquisa.</param>
        /// <returns>Dicionário com uma Tupla referente ao item de cache. Essa tupla possui
        /// 2 itens, nome da region e o valor do item.</returns>
        public static Dictionary<String, Tuple<String, Object>> GetAllItens(string cacheName = "default")
        {
            DataCache cache = AddCache.GetCache(cacheName);
            Dictionary<String, Tuple<String, Object>> result = new Dictionary<String, Tuple<String, Object>>();

            foreach (string region in cache.GetSystemRegions())
            {
                foreach (var kvp in cache.GetObjectsInRegion(region))
                {
                    result.Add(kvp.Key, new Tuple<String, Object>(region, kvp.Value));
                    //Console.WriteLine("data item ('{0}','{1}') in region {2} of cache {3}", kvp.Key, kvp.Value.ToString(), region, cacheName);
                }
            }

            return result;
        }      

    }
}