using System;
/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System.Collections.Generic;
using System.Linq;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Datasource
{
    /// <summary>
    /// Fonte de conteúdo: Serviços
    /// </summary>
    public class ServicoDatasource : IDatasource
    {
        /// <summary>
        /// Objeto da sessão do usuário
        /// </summary>
        public Sessao Sessao { get; set; }

        /// <summary>
        /// Item de menu da Home
        /// </summary>
        public ServicoItem MenuHome { get; set; }
        
        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="sessao"></param>
        public ServicoDatasource(Sessao sessao)
        {
            this.Sessao = sessao;

            this.MenuHome = new ServicoItem();
            this.MenuHome.Title = "Home";
            this.MenuHome.Url = "/sites/fechado";
            this.MenuHome.Description = "Home";
            this.MenuHome.Codigo = 0;
        }

        /// <summary>
        /// Obtém o conteúdo que será considerado pela pesquisa
        /// </summary>
        /// <returns>Conteúdos encontrados</returns>
        public List<ContentItem> GetItems()
        {
            var items = new List<ContentItem>();

            if (this.Sessao != null)
            {
                //transforma o objeto de sessão do usuário em itens de conteúdo
                foreach (Menu menu in this.Sessao.Menu)
                    ProcessarItemMenu(ref items, menu, null);
            }

            return items;
        }

        /// <summary>
        /// Método recursivo para transformar a estrutura de menus em itens de conteúdo
        /// </summary>
        /// <param name="itens">Itens de conteúdo</param>
        /// <param name="menu">Menu</param>
        /// <param name="itemPai">Item de conteúdo pai</param>
        private void ProcessarItemMenu(ref List<ContentItem> itens, Menu menu, ServicoItem itemPai)
        {
            ServicoItem resultado = new ServicoItem();
            resultado.Description = menu.Observacoes;
            resultado.Codigo = menu.Codigo;

            if (itemPai != null)
            {
                resultado.Title = itemPai.Title + " > " + menu.Texto;
                resultado.NiveisMenu.AddRange(itemPai.NiveisMenu);
            }
            else
            {
                resultado.Title = menu.Texto;
            }
            
            resultado.NiveisMenu.Add(menu.Texto);

            if (menu.Paginas != null && menu.Paginas.Count > 0 && menu.FlagMenu)
            {
                Pagina pagina = menu.Paginas.FirstOrDefault(p => p.Navegacao && !String.IsNullOrWhiteSpace(p.Url));
                if (pagina != null)
                {
                    resultado.Url = pagina.Url;
                    itens.Add(resultado);
                }
            }
            
            foreach (Menu menuFilho in menu.Items)
                ProcessarItemMenu(ref itens, menuFilho, resultado);
        }
    }
}