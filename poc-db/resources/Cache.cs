/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Runtime.Caching;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core
{
    /// <summary>
    /// Classe para implementação de cache em memória
    /// </summary>
    public class CacheAtendimento
    {
        /// <summary>
        /// Objeto de cache 
        /// </summary>
        private static MemoryCache cache;

        /// <summary>
        /// Objeto para thread-safe
        /// </summary>
        private static readonly Object cacheLock;

        /// <summary>
        /// Construtor estático para inicialização do cache
        /// </summary>
        static CacheAtendimento()
        {
            cache = new MemoryCache("Rede.PN.AtendimentoDigital.SharePoint");
            cacheLock = new Object();
        }

        /// <summary>
        /// Adiciona item no cache
        /// </summary>
        /// <param name="key">Chave do objeto</param>
        /// <param name="value">Valor do objeto</param>
        /// <param name="expirationMinutes">Tempo de expiração. Default: 20 minutos</param>
        public static void AddItem<T>(String key, T value, Int32 expirationMinutes = 20)
        {
            lock (cacheLock)
            {
                cache.Add(key, value, DateTime.Now.AddMinutes(expirationMinutes));
            }
        }

        /// <summary>
        /// Remoção do item do cache.
        /// </summary>
        /// <param name="key">Chave do item que será removido.</param>
        public static void RemoveItem(String key)
        {
            lock (cacheLock)
            {
                cache.Remove(key);
            }
        }

        /// <summary>
        /// Recupera o item do cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        public static T GetItem<T>(String key, Boolean remove = false)
        {
            lock (cacheLock)
            {
                T res = (T) cache.Get(key);                

                if (res != null && remove)
                    cache.Remove(key);

                return res;
            }
        }
    }
}