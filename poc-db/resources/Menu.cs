#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [23/05/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Linq;
using System.Collections.Generic;

using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.Negocio
{

    /// <summary>
    /// Classe de menu do Portal Redecard
    /// </summary>
    public class Menu : RegraDeNegocioBase
    {
        /// <summary>
        /// Método usado para obter os items de menu do banco de dados para o usuário
        /// especificado
        /// </summary>
        public List<Modelo.Menu> Consultar(String codigoUsuario, Int32 codigoGrupoEntidade, Int32 codigoEntidade, Int32 codigoIdUsuario)
        {
            try
            {
                Dados.Menu menuDados = new Dados.Menu();
                List<Modelo.MenuInfo> items = menuDados.Consultar(codigoUsuario, codigoGrupoEntidade, codigoEntidade, codigoIdUsuario);

                List<Modelo.Menu> menuItems = new List<Modelo.Menu>();

                if (items.Count > 0)
                {
                    var rootItems = items.Where(x => x.CodigoServicoPai == 0);
                    foreach (Modelo.MenuInfo rootItem in rootItems)
                    {
                        Modelo.Menu menu = new Modelo.Menu()
                        {
                            Codigo = rootItem.CodigoServico,
                            Texto = rootItem.NomeServico,
                            Observacoes = rootItem.Observacoes,
                            FlagMenu = rootItem.FlagMenu,
                            FlagFooter = rootItem.FlagFooter
                        };

                        rootItem.Paginas.ForEach(delegate (Modelo.PaginaInfo paginaInfo) {
                            menu.Paginas.Add(new Modelo.PaginaMenu()
                            {
                                TextoBotao = paginaInfo.DescricaoBotao,
                                Url = paginaInfo.Url,
                                Navegacao = paginaInfo.Navegacao
                            });
                        });

                        this.CarregarItemsFilho(menu, rootItem, items);
                        // Carregar menu pai
                        menuItems.Add(menu);
                    }
                }
                return menuItems;
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Carregar items filhos do menu de navegação
        /// </summary>
        protected void CarregarItemsFilho(Modelo.Menu root, Modelo.MenuInfo rItem, List<Modelo.MenuInfo> items)
        {
            var rootItems = items.Where(x => x.CodigoServicoPai == rItem.CodigoServico);

            foreach (Modelo.MenuInfo rootItem in rootItems)
            {
                Modelo.Menu menu = new Modelo.Menu()
                {
                    Codigo = rootItem.CodigoServico,
                    Texto = rootItem.NomeServico,
                    Observacoes = rootItem.Observacoes,
                    FlagMenu = rootItem.FlagMenu,
                    FlagFooter = rootItem.FlagFooter
                };

                rootItem.Paginas.ForEach(delegate(Modelo.PaginaInfo paginaInfo)
                {
                    menu.Paginas.Add(new Modelo.PaginaMenu()
                    {
                        TextoBotao = paginaInfo.DescricaoBotao,
                        Url = paginaInfo.Url,
                        Navegacao = paginaInfo.Navegacao
                    });
                });

                this.CarregarItemsFilho(menu, rootItem, items);
                root.Items.Add(menu);
            }
        }
    }
}
