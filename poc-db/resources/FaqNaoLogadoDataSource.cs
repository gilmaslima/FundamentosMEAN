/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content;
using System.Linq;
using System.Web.Script.Serialization;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content.Public;
using System.Collections;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Datasource.Public
{
    /// <summary>
    /// Fonte de conteúdo: Categorias
    /// </summary>
    public class FaqNaoLogadoDatasource : IDatasource
    {
        /// <summary>
        /// Instância do contexto SPWeb
        /// </summary>
        public SPWeb Web { get; set; }

        /// <summary>
        /// Configuração para incluir as categorias no escopo de busca da FAQ 
        /// </summary>
        public Boolean EscopoCategoria { get; set; }

        /// <summary>
        /// Configuração para incluir as subcategorias no escopo de busca da FAQ 
        /// </summary>
        public Boolean EscopoSubcategoria { get; set; }

        /// <summary>
        /// Configuração para incluir as perguntas no escopo de busca da FAQ 
        /// </summary>
        public Boolean EscopoPerguntas { get; set; }

        /// <summary>
        /// Lock object
        /// </summary>
        private static readonly Object cacheLockGetItems;

        /// <summary>
        /// Construtor estático
        /// </summary>
        static FaqNaoLogadoDatasource()
        {
            cacheLockGetItems = new Object();
        }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="web">Contexto SPWeb</param>
        /// <param name="escopoCategorias">inclui as categorias no escopo da busca</param>
        /// <param name="escopoSubcategorias">inclui as subcategorias no escopo da busca</param>
        /// <param name="escopoPerguntas">incluir as perguntas no escopo da busca</param>
        /// <param name="escopoCategoriaId">id da categoria do escopo</param>
        /// <param name="escopoSubcategoriaId">id do subcategoria do escopo</param>
        public FaqNaoLogadoDatasource(SPWeb web, 
                                      Boolean escopoCategorias = true,
                                      Boolean escopoSubcategorias = true,
                                      Boolean escopoPerguntas = true)
        {
            this.Web = web;
            this.EscopoCategoria = escopoCategorias;
            this.EscopoSubcategoria = escopoSubcategorias;
            this.EscopoPerguntas = escopoPerguntas;
        }

        /// <summary>
        /// Obtém o conteúdo que será considerado pela pesquisa
        /// </summary>
        /// <returns>Conteúdos encontrados</returns>
        public List<ContentItem> GetItems(Device device = Device.AllDevices,
                                          Int32? escopoCategoriaId = null,
                                          Int32? escopoSubcategoriaId = null,
                                          Boolean useCache = true, 
                                          Int32 cacheExpiration = 20)
        {
            //chave do cache das categorias, exclusiva por dispositivo
            String cacheKeyCategorias = String.Format("{0}-{1}_{2}", "CategoriasNaoLogado", device.ToString(), Web.ID.ToString());
            String cacheKeySubcategorias = String.Format("{0}-{1}_{2}", "SubcategoriasNaoLogado", device.ToString(), Web.ID.ToString());
            String cacheKeyPerguntas = String.Format("{0}-{1}_{2}", "PerguntasNaoLogado", device.ToString(), Web.ID.ToString());

            List<ContentItem> items = new List<ContentItem>();
            List<CategoriaNaoLogadoItem> itensCategorias = null;
            List<SubcategoriaNaoLogadoItem> itensSubcategorias = null;
            List<PerguntaRespostaNaoLogadoItem> itensPerguntas = null;

            lock (cacheLockGetItems)
            {
                //tenta recuperar do cache, se encontrou, retorna items armazenados no cache
                if (useCache)
                {
                    itensCategorias = CacheAtendimento.GetItem<List<CategoriaNaoLogadoItem>>(cacheKeyCategorias);
                    itensSubcategorias = CacheAtendimento.GetItem<List<SubcategoriaNaoLogadoItem>>(cacheKeySubcategorias);
                    itensPerguntas = CacheAtendimento.GetItem<List<PerguntaRespostaNaoLogadoItem>>(cacheKeyPerguntas);
                }

                //se chegou até aqui, não está no cache, então recupera do arquivo
                if (this.Web != null)
                {
                    //obtém arquivo JSON de FAQ                    
                    String jsonContent = new AtendimentoDigitalRepository().GetFileContent(this.Web, "AtendimentoDigital/data", "ad-faq.js");                    

                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

                    Dictionary<String, dynamic> jsonObject = jsSerializer
                        .Deserialize<Dictionary<String, dynamic>>(jsonContent);

                    //faz a leitura das categorias do arquivo JSON e converte para modelo da aplicação
                    //Dictionary<String, dynamic> jsonFaq = jsonObject["faq"] as Dictionary<String, dynamic>;
                    if (itensCategorias == null)
                    {
                        itensCategorias = new List<CategoriaNaoLogadoItem>();

                        Dictionary<String, dynamic> jsonCategorias = jsonObject["categorias"] as Dictionary<String, dynamic>;
                        foreach (String idCategoria in jsonCategorias.Keys)
                        {
                            Dictionary<String, dynamic> categoriaJson = jsonCategorias[idCategoria] as Dictionary<String, dynamic>;
                            CategoriaNaoLogadoItem item = new CategoriaNaoLogadoItem()
                            {
                                Id = (categoriaJson["id"] as Int32?).Value,
                                Title = categoriaJson["nome"] as String,
                                Description = categoriaJson["nome"] as String,
                                Icone = categoriaJson["icone"] as String,
                                Subcategorias = (categoriaJson["subcategorias"] as ArrayList).Cast<Int32>().ToList()
                            };
                            itensCategorias.Add(item);
                        }

                        //armazena no cache
                        if (useCache)
                        {
                            CacheAtendimento.AddItem(cacheKeyCategorias, itensCategorias, cacheExpiration);
                        }
                    }

                    //faz a leitura das subcategorias do arquivo JSON e converte para modelo da aplicação
                    if (itensSubcategorias == null)
                    {
                        itensSubcategorias = new List<SubcategoriaNaoLogadoItem>();

                        Dictionary<String, dynamic> jsonSubcategorias = jsonObject["subcategorias"] as Dictionary<String, dynamic>;
                        foreach (String idSubcategoria in jsonSubcategorias.Keys)
                        {
                            Dictionary<String, dynamic> subcategoriaJson = jsonSubcategorias[idSubcategoria] as Dictionary<String, dynamic>;
                            SubcategoriaNaoLogadoItem item = new SubcategoriaNaoLogadoItem()
                            {
                                Id = (subcategoriaJson["id"] as Int32?).Value,
                                Title = subcategoriaJson["nome"] as String,
                                Description = subcategoriaJson["nome"] as String,
                                Categoria = (subcategoriaJson["categoria"] as Int32?).Value,
                                Perguntas = (subcategoriaJson["perguntas"] as ArrayList).Cast<Int32>().ToList()
                            };
                            itensSubcategorias.Add(item);
                        }

                        //armazena no cache
                        if (useCache)
                        {
                            CacheAtendimento.AddItem(cacheKeySubcategorias, itensSubcategorias, cacheExpiration);
                        }
                    }

                    //faz a leitura das perguntas do arquivo JSON e converte para modelo da aplicação
                    if (itensPerguntas == null)
                    {
                        itensPerguntas = new List<PerguntaRespostaNaoLogadoItem>();

                        Dictionary<String, dynamic> jsonPerguntas = jsonObject["perguntas"] as Dictionary<String, dynamic>;

                        foreach (String idPergunta in jsonPerguntas.Keys)
                        {
                            Dictionary<String, dynamic> perguntaJson = jsonPerguntas[idPergunta] as Dictionary<String, dynamic>;
                            PerguntaRespostaNaoLogadoItem item = new PerguntaRespostaNaoLogadoItem()
                            {
                                Id = (perguntaJson["id"] as Int32?).Value,
                                Subcategoria = (perguntaJson["subcategoria"] as Int32?).Value,
                                PalavraChave = perguntaJson["palavraChave"] as String,
                                PalavrasChaves = (perguntaJson["palavrasChaves"] as ArrayList).Cast<String>().ToList(),
                                UrlAmigavel = perguntaJson["urlAmigavel"] as String,
                                PageTitle = perguntaJson["pageTitle"] as String,
                                Title = perguntaJson["pergunta"] as String,
                                Descricao = perguntaJson["description"] as String,
                                Description = perguntaJson["resposta"] as String,
                                RespostaResumida = perguntaJson["respostaResumida"] as String,
                                Videos = (perguntaJson["videos"] as ArrayList).Cast<String>().ToList()
                            };
                            itensPerguntas.Add(item);
                        }

                        //armazena no cache
                        if (useCache)
                        {
                            CacheAtendimento.AddItem(cacheKeyPerguntas, itensPerguntas, cacheExpiration);
                        }
                    }                    
                }
            }

            //de acordo com o escopo da FAQ, inclui itens no retorno
            if (this.EscopoCategoria)
            {
                items.AddRange(itensCategorias);
            }

            //se o escopo é subcategoria, verifica se precisa filtrar por categoria
            if (this.EscopoSubcategoria)
            {
                if (escopoCategoriaId.HasValue)
                    items.AddRange(itensSubcategorias.Where(item => item.Categoria == escopoCategoriaId.Value));
                else
                    items.AddRange(itensSubcategorias);
            }

            //se o escopo é pergunta, verifica se precisa filtrar por subcategoria
            if (this.EscopoPerguntas)
            {
                if (escopoSubcategoriaId.HasValue)
                    items.AddRange(itensPerguntas.Where(item => item.Subcategoria == escopoSubcategoriaId.Value));
                else
                    items.AddRange(itensPerguntas);
            }

            return items;
        }
    }
}