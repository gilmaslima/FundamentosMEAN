/*
© Copyright 2016 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/

using Microsoft.SharePoint;
using System;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// IListItemRepository
    /// </summary>
    public interface IListItemRepository
    {
        /// <summary>
        /// Adiciona um novo SPListItem para a lista especificada.
        /// </summary>        
        SPListItem Add(SPWeb web, String listName, Dictionary<String, object> fields);

        /// <summary>
        /// Retorna um SPListItem baseado no SPListItem.ID.
        /// </summary>
        SPListItem Get(SPWeb web, String listName, Int32 Id);

        /// <summary>Int32
        /// Retorna um SPListItemCollection baseado em uma consulta CAML.
        /// </summary>
        SPListItemCollection Get(SPWeb web, String listName, SPQuery query);

        /// <summary>
        /// Atualiza um SPListItem da lista baseado nos valores informados na coleção de fields.
        /// </summary>
        void Update(SPWeb web, String listName, Int32 listItemId, Dictionary<String, object> fields);

        /// <summary>
        /// Deleta um SPListItem da lista baseado no Id especificado.
        /// </summary>
        void Delete(SPWeb web, String listName, Int32 listItemId);
    }
}
