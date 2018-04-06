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
using Rede.PN.AtendimentoDigital.SharePoint.Repository;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Datasource
{
    /// <summary>
    /// Fonte de conteúdo: Subcategorias
    /// </summary>
    public class SubcategoriaDatasource : IDatasource
    {
        /// <summary>
        /// Instância do contexto SPWeb
        /// </summary>
        public SPWeb Web { get; set; }

        /// <summary>
        /// Lock object
        /// </summary>
        private static readonly Object cacheLockGetItems;

        /// <summary>
        /// Construtor estático
        /// </summary>
        static SubcategoriaDatasource()
        {
            cacheLockGetItems = new Object();
        }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="web">Contexto SPWeb</param>
        public SubcategoriaDatasource(SPWeb web)
        {
            this.Web = web;            
        }

        /// <summary>
        /// Obtém o conteúdo que será considerado pela pesquisa
        /// </summary>
        /// <returns>Conteúdos encontrados</returns>
        public List<ContentItem> GetItems(Device device = Device.AllDevices, Boolean useCache = true, Int32 cacheExpiration = 20)
        {
            //chave do cache das subcategorias, exclusiva por dispositivo
            String cacheKey = String.Format("{0}-{1}_{2}", "Subcategoria", device.ToString(), Web.ID.ToString());
            List<ContentItem> items = null;

            lock (cacheLockGetItems)
            {
                //tenta recuperar do cache, se encontrou, retorna items armazenados no cache
                if (useCache)
                {
                    items = CacheAtendimento.GetItem<List<ContentItem>>(cacheKey);
                    if (items != null)
                        return items;
                }

                //se chegou até aqui, não está no cache, então recupera da lista
                if (this.Web != null)
                {
                    //obtém todos os itens da lista de Subcategoria
                    List<DuvidaSubcategoria> subcategorias =
                        new DuvidaSubcategoriaRepository().GetAllItems(this.Web, device.GetDescription());

                    //transforma cada item da lista em um item de conteúdo
                    if (subcategorias != null && subcategorias.Count > 0)
                    {
                        items = new List<ContentItem>();

                        foreach (DuvidaSubcategoria subcategoria in subcategorias)
                        {
                            items.Add(new SubcategoriaItem()
                            {
                                Id = subcategoria.Id,
                                Title = subcategoria.Title ?? String.Empty,
                                Description = subcategoria.Resumo ?? String.Empty
                            });
                        }

                        //armazena no cache
                        if (useCache)
                        {
                            CacheAtendimento.AddItem(cacheKey, items, cacheExpiration);
                        }
                    }
                }
            }

            return items;
        }
    }
}