/*
© Copyright 2016 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/

using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// ListItemRepository
    /// </summary>
    public class ListItemRepository : IListItemRepository
    {
        #region Métodos público
        /// <summary>
        /// Add item na lista
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listName"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SPListItem Add(SPWeb web, String listName, Dictionary<String, Object> fields)
        {
            SPListItem newItem = web.Lists[listName].Items.Add();

            foreach (String key in fields.Keys)
            {
                newItem[key] = fields[key];
            }

            newItem.Update();

            return newItem;
        }

        /// <summary>
        /// Get itens da lista de acordo com query
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public SPListItemCollection Get(SPWeb web, String listName, SPQuery query)
        {
            SPListItemCollection collection = null;

            collection = web.Lists[listName].GetItems(query);

            return collection;
        }


        /// <summary>
        /// Get item da lista pelo Id especificado
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listName"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPListItem Get(SPWeb web, String listName, Int32 Id)
        {
            SPListItem item = null;

            item = web.Lists[listName].GetItemById(Id);

            return item;
        }

        /// <summary>
        /// Atualiza item da lista
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listName"></param>
        /// <param name="listItemId"></param>
        /// <param name="fields"></param>
        public void Update(SPWeb web, String listName, Int32 listItemId, Dictionary<String, Object> fields)
        {
            SPListItem item = null;
            SPListItemCollection collection = null;

            collection = web.Lists[listName].GetItems(BuildQuery(listItemId));

            if (collection != null && collection.Count > 0)
            {
                item = collection[0];

                foreach (String key in fields.Keys)
                {
                    item[key] = fields[key];
                }

                item.Update();
            }
        }

        /// <summary>
        /// Delete item da lista pelo id especificado
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listName"></param>
        /// <param name="listItemId"></param>
        public void Delete(SPWeb web, String listName, Int32 listItemId)
        {
            SPListItem item = null;
            SPListItemCollection collection = null;

            collection = web.Lists[listName].GetItems(BuildQuery(listItemId));

            if (collection != null && collection.Count > 0)
            {
                item = collection[0];

                item.Delete();
            }
        }

        #endregion

        #region Métodos Privado

        /// <summary>
        /// Monta query para buscar o item pelo id especificado
        /// </summary>
        /// <param name="listItemId"></param>
        /// <returns></returns>
        private static SPQuery BuildQuery(Int32 listItemId)
        {
            SPQuery query = new SPQuery();
            query.Query = string.Format(CultureInfo.InvariantCulture, "<Where><Eq><FieldRef Name='ID'/><Value Type='Integer'>{0}</Value></Eq></Where>", listItemId);

            return query;
        }

        #endregion

    }
}
