/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Result;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    public class Search : HandlerBase
    {
        [HttpGet, HttpPost, HttpNoSession(true)]
        public HandlerResponse SearchTerm()
        {
            try 
            {
                //Recupera parâmetros de entrada
                String term = base.Request["term"];
                String config = (base.Request["config"] ?? String.Empty).Trim();

                QueryConfig queryConfig = default(QueryConfig);
                if(!String.IsNullOrWhiteSpace(config))
                    queryConfig = QueryConfig.JsonDeserialize(config);

                //prepara engine de busca
                SearchPortal search = new SearchPortal(base.CurrentSPContext.Web, base.Sessao, queryConfig);

                //executa busca para o termo pesquisado
                SearchResult result = search.Search(term);

                return new HandlerResponse(result);
            }
            catch(ArgumentException ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro genérico 1 durante pesquisa", ex);
                return new HandlerResponse(301, "Erro genérico 1 durante pesquisa"); 
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
		        Logger.GravarErro("Erro genérico 2 durante pesquisa", ex);
                return new HandlerResponse(301, "Erro genérico 2 durante pesquisa"); 
            }
        }        
    }
}