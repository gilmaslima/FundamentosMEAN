/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Constants;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    public class DuvidaPerguntaRespostaRepository : BaseEntityRepository<DuvidaPerguntaResposta>, IDuvidaPerguntaRespostaRepository
    {
        /// <summary>
        /// Retorna o nome da biblioteca
        /// </summary>
        protected override String ListName
        {
            get { return Lists.DuvidaPerguntaResposta; }
        }

        /// <summary>
        /// GatherParameters 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        protected override Dictionary<String, object> GatherParameters(DuvidaPerguntaResposta entity, SPWeb web)
        {
            Dictionary<String, object> fields = new Dictionary<String, object>();
            fields.Add(Fields.Title, entity.Title);
            fields.Add(Fields.Resposta, entity.Resposta);
            fields.Add(Fields.TituloReduzido, entity.TituloReduzido);
            fields.Add(Fields.Ordem, entity.Ordem);
            fields.Add(Fields.URLVideo, entity.URLVideo);
            return fields;
        }

        /// <summary>
        /// PopulateEntity 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override DuvidaPerguntaResposta PopulateEntity(SPListItem item)
        {
            DuvidaPerguntaResposta entity = new DuvidaPerguntaResposta();
            entity.Id = (Int32)item[Fields.Id];
            entity.Title = (String)item[Fields.Title];
            entity.Resposta = (String)item[Fields.Resposta];
            entity.RespostaTexto = ConvertSimpleHtmlToText(entity.Resposta);
            entity.TituloReduzido = (String)item[Fields.TituloReduzido];
            entity.Ordem = Convert.ToInt32((Double)item[Fields.Ordem]);
            entity.URLVideo = (String)item[Fields.URLVideo];            
            return entity;
        }

        /// <summary>
        /// Converte uma string HTML (de um RichTextField) para plain text.
        /// </summary>
        /// <param name="html">Conteúdo html de um campo RichTextField</param>
        /// <returns>Conteúdo em string</returns>
        private String ConvertSimpleHtmlToText(String html)
        {
            try
            {
                return SPHttpUtility.ConvertSimpleHtmlToText(html, -1);
            }
            catch (NullReferenceException ex)
            {
                Logger.GravarErro("Erro durante leitura de campo html", ex);
                return html;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante leitura de campo html", ex);
                return html;
            }
        }

        /// <summary>
        /// Obtém todos os items
        /// </summary>
        /// <param name="web">Contexto</param>
        /// <param name="canalExibicao">Canal de exibição</param>
        /// <returns>List<DuvidaSubcategoria></returns>
        public List<DuvidaPerguntaResposta> GetAllItems(SPWeb web, String canalExibicao = null)
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
                <FieldRef Name='{1}' />
                <FieldRef Name='{2}' />
                <FieldRef Name='{3}' />
                <FieldRef Name='{4}' />
                <FieldRef Name='{5}' />",
                Fields.Id, Fields.Title, Fields.Resposta, Fields.TituloReduzido, Fields.Ordem, Fields.URLVideo);

            return GetListItems(query, web);
        }
    }
}