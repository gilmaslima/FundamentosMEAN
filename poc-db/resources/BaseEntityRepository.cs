/*
© Copyright 2016 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/

using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Constants;
using System;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// BaseEntityRepository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseEntityRepository<T> where T : BaseEntity, new()
    {
        /// <summary>
        /// IListItemRepository
        /// </summary>
        private readonly IListItemRepository listItemRepository;

        /// <summary>
        ///ListName
        /// </summary>
        protected abstract String ListName
        {
            get;
        }

        /// <summary>
        /// BaseEntityRepository
        /// </summary>
        protected BaseEntityRepository()
        {
            this.listItemRepository = new ListItemRepository();
        }

        #region Métodos

        /// <summary>
        /// Add item na lista
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        protected Int64 AddListItem(T entity, SPWeb web)
        {
            Dictionary<String, object> fields = GatherParameters(entity, web);

            SPListItem item = listItemRepository.Add(web, this.ListName, fields);

            return (Int64)item[Fields.Id];
        }

        /// <summary>
        /// Busca os itens de acordo com a query
        /// </summary>
        /// <param name="caml"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        protected T GetListItem(SPQuery caml, SPWeb web)
        {
            T entity = null;

            SPListItem item = null;
            SPListItemCollection collection = null;

            collection = listItemRepository.Get(web, this.ListName, caml);

            if (collection != null && collection.Count > 0)
            {
                item = collection[0];
                entity = PopulateEntity(item);
                GetObject(entity, web);
            }

            return entity;
        }

        /// <summary>
        /// GetListItem
        /// </summary>
        /// <param name="caml"></param>
        /// <param name="web"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected T GetListItem(SPQuery caml, SPWeb web, Object fields)
        {
            T entity = null;

            SPListItem item = null;
            SPListItemCollection collection = null;

            collection = listItemRepository.Get(web, this.ListName, caml);

            if (collection != null && collection.Count > 0)
            {
                item = collection[0];
                entity = PopulateEntity(item, fields);
                GetObject(entity, web);
            }

            return entity;
        }

        /// <summary>
        /// GetListItems
        /// </summary>
        /// <param name="caml"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        protected List<T> GetListItems(SPQuery caml, SPWeb web)
        {
            List<T> entities = new List<T>();

            SPListItemCollection listItems = listItemRepository.Get(web, this.ListName, caml);

            for (Int32 i = 0; i < listItems.Count; i++)
            {
                T entity = PopulateEntity(listItems[i]);
                entities.Add(entity);
                GetObject(entity, web);
            }

            return entities;
        }

        /// <summary>
        /// GetListItemById
        /// </summary>
        /// <param name="id"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        protected T GetListItemById(Int32 id, SPWeb web)
        {
            T entity = null;

            SPListItem item = listItemRepository.Get(web, this.ListName, id);

            if (item != null)
            {
                entity = PopulateEntity(item);
                GetObject(entity, web);
            }

            return entity;
        }

        /// <summary>
        /// UpdateListItem
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="web"></param>
        protected void UpdateListItem(T entity, SPWeb web)
        {
            Dictionary<String, object> fields = GatherParameters(entity, web);

            listItemRepository.Update(web, this.ListName, entity.Id, fields);
        }

        /// <summary>
        /// DeleteListItem
        /// </summary>
        /// <param name="id"></param>
        /// <param name="web"></param>
        protected void DeleteListItem(Int32 id, SPWeb web)
        {
            listItemRepository.Delete(web, this.ListName, id);
        }

        /// <summary>
        /// GetFieldName
        /// </summary>
        /// <param name="key"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        public String GetFieldName(Guid key, SPWeb web)
        {
            return web.Lists[this.ListName].Fields[key].InternalName;
        }

        #region Abstract

        /// <summary>
        /// GatherParameters
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="web"></param>
        /// <returns>Dictionary<String, object></returns>
        protected abstract Dictionary<String, object> GatherParameters(T entity, SPWeb web);

        /// <summary>
        /// PopulateEntity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual T PopulateEntity(SPListItem item)
        {
            return default(T);
        }

        /// <summary>
        /// PopulateEntity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected virtual T PopulateEntity(SPListItem item, object fields)
        {
            return default(T);
        }

        /// <summary>
        /// GetObject
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="web"></param>
        protected virtual void GetObject(T entity, SPWeb web)
        {

        }

        #endregion

        #endregion
    }
}
