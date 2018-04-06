/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Constants;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    public class DuvidaCategoriaRepository : BaseEntityRepository<DuvidaCategoria>, IDuvidaCategoriaRepository
    {
        /// <summary>
        /// Retorna o nome da biblioteca
        /// </summary>
        protected override String ListName
        {
            get { return Lists.DuvidaCategoria; }
        }

        /// <summary>
        /// GatherParameters 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        protected override Dictionary<String, object> GatherParameters(DuvidaCategoria entity, SPWeb web)
        {
            Dictionary<String, object> fields = new Dictionary<String, object>();
            fields.Add(Fields.Title, entity.Title);
            return fields;
        }

        /// <summary>
        /// PopulateEntity 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override DuvidaCategoria PopulateEntity(SPListItem item)
        {
            DuvidaCategoria entity = new DuvidaCategoria();
            entity.Id = (Int32)item[Fields.Id];
            entity.Title = (String)item[Fields.Title];
            return entity;
        }

        /// <summary>
        /// Obtém todos os items
        /// </summary>
        /// <param name="web">Contexto</param>
        /// <param name="canalExibicao">Canal de exibição</param>
        /// <returns>List<DuvidaSubcategoria></returns>
        public List<DuvidaCategoria> GetAllItems(SPWeb web, String canalExibicao = null)
        {
            SPQuery query = new SPQuery();

            if (!String.IsNullOrWhiteSpace(canalExibicao))
            {
                query.Query = String.Format(@"<Where>                                                                                                            
                                            <Contains>
                                                <FieldRef Name='{0}'/>
                                                <Value Type='Text'>{1}</Value>
                                            </Contains>
                                          </Where>", Fields.CanaisExibicao, canalExibicao);
            }

            query.ViewFields = String.Format(@"
                <FieldRef Name='{0}' />
                <FieldRef Name='{1}' />",
                Fields.Id, Fields.Title);
            return GetListItems(query, web);
        }
    }
}
