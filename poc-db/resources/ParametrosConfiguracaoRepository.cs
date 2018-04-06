using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// ParametrosConfiguracaoRepository
    /// </summary>
    public class ParametrosConfiguracaoRepository: BaseEntityRepository<ParametrosConfiguracao>, IParametrosConfiguracao
    {        
        /// <summary>
        /// Retorna o nome da lista
        /// </summary>
        protected override String ListName
        {
            get { return Lists.ParametrosConfiguracao; }
        }

        /// <summary>
        ///  Busca item de acordo com name
        /// </summary>
        /// <param name="web"></param>
        /// <returns>List<ParametrosConfiguracao></returns>
        public ParametrosConfiguracao Get(SPWeb web, String name)
        {
            SPQuery query = new SPQuery();
            query.RowLimit = 1;

            query.Query = String.Format(@"<Where>                                                                                                            
                                            <Eq>
                                                <FieldRef Name='name'/>
                                                <Value Type='Text'>{0}</Value>
                                            </Eq>
                                          </Where>", name);

            //return GetListItems(query, web).First<ParametrosConfiguracao>();
            return GetListItems(query, web).FirstOrDefault<ParametrosConfiguracao>();
        }

        /// <summary>
        /// Busca todos os itens da lista
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public List<ParametrosConfiguracao> GetAll(SPWeb web)
        {
            SPQuery query = new SPQuery();
            query.RecurrenceOrderBy = true;

            return GetListItems(query, web);
        }

        /// <summary>
        /// GatherParameters
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        protected override Dictionary<String, Object> GatherParameters(ParametrosConfiguracao entity, SPWeb web)
        {
            Dictionary<String, Object> fields = new Dictionary<String, Object>();
            fields.Add(Fields.Id, entity.Id);
            fields.Add(Fields.Title, entity.Title);
            fields.Add(Fields.Descricao, entity.Descricao);
            fields.Add(Fields.Ativo, entity.Ativo);
            fields.Add(Fields.Name, entity.Name);
            fields.Add(Fields.Valor, entity.Valor);
            fields.Add(Fields.CanaisExibicao, entity.CanaisExibicao);
            fields.Add(Fields.Categoria, entity.Categoria);
            
            return fields;
        }
        
        /// <summary>
        /// PopulateEntity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override ParametrosConfiguracao PopulateEntity(SPListItem item)
        {
            ParametrosConfiguracao entity = new ParametrosConfiguracao();
            entity.Id = (Int32)item[Fields.Id];
            entity.Title = (String)item[Fields.Title];
            entity.Descricao = (String)item[Fields.Descricao];
            entity.Ativo = (Boolean)item[Fields.Ativo];
            entity.Name = (String)item[Fields.Name];
            entity.Valor = (String)item[Fields.Valor];
            entity.CanaisExibicao = (String)item[Fields.CanaisExibicao];
            entity.Categoria = (String)item[Fields.Categoria];
            
            return entity;
        }

        
    }
}
