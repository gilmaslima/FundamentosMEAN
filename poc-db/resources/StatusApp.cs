using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rede.PN.LoginPortal.CacheService.Models
{
    public class StatusApp
    {
        /// <summary>
        /// status da requisição
        /// </summary>
        public string StatusCode { get; set; }


       /// <summary>
       /// mensagem da requisição
       /// </summary>
       public string Message { get; set; }
    }
}