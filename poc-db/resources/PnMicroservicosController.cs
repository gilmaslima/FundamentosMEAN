using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.ApplicationServer.Caching;
using NLog;
using System.Configuration;
using Rede.PN.LoginPortal.CacheService.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rede.PN.LoginPortal.CacheService.Models;

namespace Rede.PN.LoginPortal.CacheService.Controllers
{
    public class PnMicroservicosController : ApiController
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public Object GetById(string id)
        {

            logger.Trace("ENTRADA PnMicroservicosController.GetById(string id)");

                       
            string key = id;

            var cacheName = string.Empty;

            try
            {

                logger.Trace("Consultando microservico pelo Id");
                logger.Info("id-{0}", id);

                IEnumerable<string> headerValues;

                if (Request.Headers.TryGetValues("cache_name", out headerValues))
                {
                    cacheName = headerValues.FirstOrDefault();
                } 
                else
                {
                    cacheName = ConfigurationManager.AppSettings["NomeCache"].ToString();
                }


                ObjetoCache microservicoTemp = AddCache.GetCacheItem<ObjetoCache>(string.Concat("id-", key), cacheName);
                

                if (microservicoTemp == null)
                {
                    logger.Trace("Objeto microservico não encontrado. Retornando Object");

                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                Object microservico = JsonConvert.DeserializeObject<Object>(microservicoTemp.Body.ToString());

                logger.Trace("Força a recuperação da chave de microservico para atualizar o TTL no App Fabric Caching");
                logger.Info("acronyme-{0}", microservico);

                
                return Request.CreateResponse(HttpStatusCode.OK, microservico);

            }
            catch (HttpResponseException ex)
            {
                logger.Error(string.Concat("Message: ", ex.Message, " StackTrace: ", ex.StackTrace));
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
            }
            catch (DataCacheException ex)
            {
                string erro = String.Format("Erro no AppFabric Caching ao consultar o objeto microservico (passando id): {0}", ex.Message);
                logger.Error(String.Concat(erro, " StackTrace: ", ex.StackTrace));
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                string erro = String.Format("Erro ao consultar objeto microservico (passando id): {0}", ex.Message);
                logger.Error(String.Concat(erro, " StackTrace: ", ex.StackTrace));
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
            }

        }

       

        [HttpPost]
        public Object Post([FromBody]JObject microservico, string id)
        {
                       
            logger.Trace("ENTRADA PnMicroservicosController.Post([FromBody]Object microservico)");

            try
            {
                if (!ModelState.IsValid)
                {
                    logger.Trace("Objeto microservico inválido");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                string key = id;

                ObjetoCache obj = new ObjetoCache();

                IEnumerable<string> headerValues;
                var notRenew = string.Empty;
                var ttl = string.Empty;
                var cacheName = string.Empty;
                TimeSpan TTLCache = new TimeSpan();

                obj.Body = microservico.ToString();

                if (Request.Headers.TryGetValues("ttl", out headerValues))
                {
                    ttl = headerValues.FirstOrDefault();

                    obj.Ttl = ttl;
                }
                else
                {
                    ttl = ConfigurationManager.AppSettings["TTLCache"].ToString();
                }

                obj.Ttl = ttl;
                var currentT = DateTime.Now;
                obj.CurrentTimeTtl = currentT.AddMinutes(Convert.ToDouble(ttl));
                TTLCache = TimeSpan.FromMinutes(Convert.ToDouble(ttl));

                // (not_renew) Compatibilidade com Backend NodeJs
                if (Request.Headers.TryGetValues("not_renew", out headerValues))
                {
                    notRenew = headerValues.FirstOrDefault();

                    obj.NotRenew = notRenew;
                }

                if (Request.Headers.TryGetValues("cache_name", out headerValues))
                {
                    cacheName = headerValues.FirstOrDefault();
                }
                else
                {
                    cacheName = ConfigurationManager.AppSettings["NomeCache"].ToString();
                }


                logger.Trace("Inserindo objeto microservico no AppFabric.");
                logger.Info("id-{0}", key);


                AddCache.PutCacheItem(string.Concat("id-", key), obj, TTLCache, cacheName);
                                  
                              
                return Request.CreateResponse(HttpStatusCode.OK, microservico);

            }
            catch (HttpResponseException ex)
            {
                logger.Error(string.Concat("Message: ", ex.Message, " StackTrace: ", ex.StackTrace));
               
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, ex.ToString());
            }
            catch (DataCacheException ex)
            {
                string erro = String.Format("Erro no AppFabric Caching ao inserir o objeto microservico: {0}", ex.Message);
                logger.Error(String.Concat(erro, " StackTrace: ", ex.StackTrace));
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, ex.ToString());
            }
            catch (Exception ex)
            {
                string erro = String.Format("Erro ao inserir objeto microservico: {0}", ex.Message);
                logger.Error(String.Concat(erro, " StackTrace: ", ex.StackTrace));
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, ex.ToString());
            }
        }


        // update: PUT api/PnMicroservicos/:id
        // PUT api/PnMicroservicos/5
        [HttpPut]
        public Object Put([FromBody]JObject microservico, string id)
        {

            logger.Trace("ENTRADA PnMicroservicosController.Put([FromBody]Object microservico)");

            try
            {
                if (!ModelState.IsValid)
                {
                    logger.Trace("Objeto microservico inválido");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                string key = id;

                IEnumerable<string> headerValues;

                var cacheName = string.Empty;

                if (Request.Headers.TryGetValues("cache_name", out headerValues))
                {
                    cacheName = headerValues.FirstOrDefault();
                }
                else
                {
                    cacheName = ConfigurationManager.AppSettings["NomeCache"].ToString();
                }

                //Object microTemp = AddCache.GetCacheItem<string>(string.Concat("id-", key), cacheName);
                ObjetoCache microservicoTemp = AddCache.GetCacheItem<ObjetoCache>(string.Concat("id-", key), cacheName);

                if (microservicoTemp == null)
                {
                    logger.Trace("Objeto microservico não encontrado. Retornando Object");

                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                microservicoTemp.Body = microservico.ToString();

                var ttlSpan = TimeSpan.FromMinutes(Convert.ToDouble(microservicoTemp.Ttl));

                if (!string.IsNullOrEmpty(microservicoTemp.NotRenew))
                {
                    var currentT = DateTime.Now;

                    ttlSpan = microservicoTemp.CurrentTimeTtl.Subtract(currentT);
                }


                logger.Trace("Atualizando o objeto microservico no AppFabric.");
                logger.Info("id-{0}", key);


                AddCache.PutCacheItem(string.Concat("id-", key), microservicoTemp, ttlSpan, cacheName);


                return Request.CreateResponse(HttpStatusCode.OK, microservico);

            }
            catch (HttpResponseException ex)
            {
                logger.Error(string.Concat("Message: ", ex.Message, " StackTrace: ", ex.StackTrace));

                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, ex.ToString());
            }
            catch (DataCacheException ex)
            {
                string erro = String.Format("Erro no AppFabric Caching ao atualizar o objeto microservico: {0}", ex.Message);
                logger.Error(String.Concat(erro, " StackTrace: ", ex.StackTrace));
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, ex.ToString());
            }
            catch (Exception ex)
            {
                string erro = String.Format("Erro ao atualizar objeto microservico: {0}", ex.Message);
                logger.Error(String.Concat(erro, " StackTrace: ", ex.StackTrace));
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, ex.ToString());
            }
        }


        
        [HttpDelete]
        public Object Delete(string id)
        {
            logger.Trace("ENTRADA PnMicroservicosController.Delete(string key)");

            string key = id;

            var cacheName = string.Empty;

            StatusApp obj = new StatusApp();

            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    string erro = "key(string) não informado";
                    logger.Trace(erro);
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, erro);
                }

                logger.Trace("Consultando o objeto microservico no AppFabric Caching pela key(id)");
                logger.Info("id-{0}", key);

                IEnumerable<string> headerValues;

                if (Request.Headers.TryGetValues("cache_name", out headerValues))
                {
                    cacheName = headerValues.FirstOrDefault();
                }
                else
                {
                    cacheName = ConfigurationManager.AppSettings["NomeCache"].ToString();
                }

                //string microservicoTemp = AddCache.GetCacheItem<string>(string.Concat("id-", key), cacheName);
                ObjetoCache microservicoTemp = AddCache.GetCacheItem<ObjetoCache>(string.Concat("id-", key), cacheName);


                if (microservicoTemp == null)
                {
                    obj.Message = "Objeto microservico não localizado no AppFabric";
                    logger.Trace(obj.Message);
                    return Request.CreateResponse(HttpStatusCode.NotFound, obj.Message);
                }

                logger.Trace("Removendo o objeto microservico do AppFabric. key(id)");
                logger.Info("id-{0}", key);

                if (!AddCache.RemoveCacheItem(string.Concat("id-", key), ConfigurationManager.AppSettings["NomeCache"].ToString()))
                    logger.Trace("Objeto microservico não pôde ser removido do AppFabric Caching");

                obj.Message = string.Format("Objeto microservico com id-{0} removido com sucesso", key);

                return Request.CreateResponse(HttpStatusCode.OK, obj);
            }
            catch (HttpResponseException ex)
            {
                logger.Error(string.Concat("Message: ", ex.Message, " StackTrace: ", ex.StackTrace));
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, String.Concat("HttpResponseException: ", ex.Message));
            }
            catch (DataCacheException ex)
            {
                obj.Message = String.Format("Erro no AppFabric Caching ao remover o objeto microservico: {0}", ex.Message);
                logger.Error(String.Concat(obj.Message, " StackTrace: ", ex.StackTrace));
                return Request.CreateResponse(HttpStatusCode.NotFound, obj.Message);
            }
            catch (Exception ex)
            {
                obj.Message = String.Format("Erro ao remover objeto microservico: {0}", ex.Message);
                logger.Error(String.Concat(obj.Message, " StackTrace: ", ex.StackTrace));
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, obj.Message);
            }
        }

    }
}
