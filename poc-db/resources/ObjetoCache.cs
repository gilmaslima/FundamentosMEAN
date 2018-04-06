using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rede.PN.LoginPortal.CacheService.Models
{
    public class ObjetoCache
    {
        /// <summary>
        /// Armazena o objeto da requisição
        /// </summary>
        public Object Body { get; set; }

        /// <summary>
        /// Armazena o tempo de vida do cache
        /// </summary>
        public string Ttl { get; set; }

        /// <summary>
        /// Se possuir valor o cache não é renovado 
        /// <param name="NotRenew"></param>
        /// </summary>
        public string NotRenew { get; set; }

        /// <summary>
        /// Armazena o tempo em hora
        /// </summary>
        public DateTime CurrentTimeTtl { get; set; }

        
    }
}