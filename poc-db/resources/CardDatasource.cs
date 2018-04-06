/*
© Copyright 2017 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Config;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Datasource
{
    public class CardDatasource : IDatasource
    {
        /// <summary>
        /// Objeto da sessão do usuário
        /// </summary>
        public Sessao Sessao { get; set; }

        /// <summary>
        /// Configuração dos cards
        /// </summary>
        private CardsConfig config;

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public CardDatasource(Sessao sessao, CardsConfig config)
        {
            this.Sessao = sessao;
            this.config = config;
        }

        /// <summary>
        /// Retorna o mapeamento entre Cards e Termos.
        /// </summary>
        /// <returns>Mapeamento Cards vs. Termos</returns>
        public List<ContentItem> GetItems()
        {
            var items = new List<ContentItem>();

            //se não possui sessão, não retorna cards para processamento
            if (this.Sessao == null)
                return items;

            //recupera informações do usuário
            Menu[] menusUsuario = Sessao.Servicos.Flatten(itemMenu => itemMenu.Items).ToArray();
            Int32[] codigoServicosUsuario = menusUsuario.Select(itemMenu => itemMenu.Codigo).ToArray();
            
            //verifica cada configuração de card
            foreach (CardConfig cardConfig in this.config)
            {
                String[] permissoesUrls = cardConfig.PermissoesUrlPaginas ?? new String[0];
                Int32[] permissoesServicos = cardConfig.PermissoesCodigoServicos ?? new Int32[0];

                //verifica se usuário possui acesso de visualização ao card
                Boolean possuiAcessoCard = VerificaAcesso(codigoServicosUsuario, permissoesUrls, permissoesServicos);
                    
                //se possui acesso ao card, adiciona no retorno
                if(possuiAcessoCard)
                {
                    String[] termos = cardConfig.Termos ?? new String[0];
                    foreach (String termo in termos)
                    {
                        var item = new CardItem();
                        item.CardType = cardConfig.Nome;
                        item.Title = termo;
                        item.Priority = cardConfig.Prioridade;
                        
                        SubcardConfig[] subcardsConfig = cardConfig.Subcards ?? new SubcardConfig[0];
                        foreach (SubcardConfig subcardConfig in subcardsConfig)
                        {
                            String[] permissoesUrlPaginasSubcard = subcardConfig.PermissoesUrlPaginas ?? new String[0];
                            Int32[] permissoesServicosSubcard = subcardConfig.PermissoesCodigoServicos ?? new Int32[0];

                            Boolean possuiAcessoSubcard = VerificaAcesso(codigoServicosUsuario, permissoesUrlPaginasSubcard, permissoesServicosSubcard);

                            //se possui acesso ao subcard, inclui no retorno
                            if (possuiAcessoSubcard)
                            {
                                item.Subcards.Add(subcardConfig.Nome);
                            }
                        }

                        items.Add(item);
                    }
                }
            }
            return items;
        }

        /// <summary>
        /// Verifica acesso ao card (validando url ou código do serviço)
        /// Obs: - se card exige alguma URL, então usuário precisa possuir permissão a alguma das URLs
        ///      - se card exibe algum serviço, então usuário precisa possuir permissão a algum dos serviços
        ///      - se card exige alguma URL E algum serviço, então usuário precisa possuir permissão 
        ///        a alguma das URLs E algum dos serviços
        /// </summary>
        /// <param name="servicosUsuario"></param>
        /// <param name="urlsCard"></param>
        /// <param name="servicosCard"></param>
        /// <returns></returns>
        private Boolean VerificaAcesso(Int32[] servicosUsuario, String[] urlsCard, Int32[] servicosCard)
        {
            //se não há permissoesUrls cadastrada, não há restrição por url para visualização do card
            //verifica se usuário possui acesso a ALGUMA página da configuração do card
            Boolean possuiAcessoUrls = urlsCard.Length == 0 || urlsCard.Any(urlPagina => Sessao.VerificarAcessoPagina(urlPagina));
            
            //se não há permissoesServicos cadastrada, não há restrição por código serviço para visualização do card
            //verifica se usuário possui acesso a ALGUM serviço da configuração do card
            Boolean possuiAcessoServicos = servicosCard.Length == 0 || servicosUsuario.Intersect(servicosCard).Any();

            //verifica se usuário possui acesso de visualização ao card
            return possuiAcessoUrls && possuiAcessoServicos;
        }
    }
}
