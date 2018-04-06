#region Used Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Linq;
using Redecard.Portal.Fechado.Model.Repository.DTOs;
#endregion

namespace Redecard.Portal.Fechado.Model.Repository
{
    /// <summary>
    ///   Responsável por qualquer ação com a lista "Links Favoritos".
    /// </summary>
    public class RepositoryLinksFavoritos : RepositoryItem, IRepository<DTOLinkFavorito, LinksFavoritosItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Links Favoritos" utilizando um novo DataContext.
        /// </summary>
        public RepositoryLinksFavoritos() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Links Favoritos" utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryLinksFavoritos(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste o link favorito na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "linkfavorito">Link Favorito a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOLinkFavorito linkfavorito)
        {
            return !linkfavorito.ID.HasValue ? Add(DTOToModel(linkfavorito)) : Update(DTOToModel(linkfavorito));
        }

        /// <summary>
        ///   Deleta um link favorito da lista.
        /// </summary>
        /// <param name = "linkfavorito">Link Favorito a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOLinkFavorito linkfavorito)
        {
            try
            {
                var linkfavoritoItem = DataContext.LinksFavoritos.Where(c => c.Id.Equals(linkfavorito.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (linkfavoritoItem == null) return false;

                DataContext.LinksFavoritos.DeleteOnSubmit(linkfavoritoItem);
                DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

                return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
            }
            catch (Exception exception)
            {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Retorna todos os itens da lista "LinksFavoritos".
        /// </summary>
        /// <returns>Todos os itens da lista "LinksFavoritos".</returns>
        public List<DTOLinkFavorito> GetAllItems()
        {
            try
            {
                var linksfavoritos = new List<DTOLinkFavorito>();
                DataContext.LinksFavoritos.ToList().ForEach(linkfavorito => linksfavoritos.Add(ModelToDTO(linkfavorito)));
                return linksfavoritos;
            }
            catch (Exception exception)
            {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Retorna determinados itens da lista "LinksFavoritos" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOLinkFavorito> GetItems(Expression<Func<LinksFavoritosItem, bool>> filter)
        {
            try
            {
                var linksfavoritos = new List<DTOLinkFavorito>();
                DataContext.LinksFavoritos.Where(filter).ToList().ForEach(linkfavorito => linksfavoritos.Add(ModelToDTO(linkfavorito)));
                return linksfavoritos;
            }
            catch (Exception exception)
            {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///   Adiciona um novo link favorito na lista.
        /// </summary>
        /// <param name = "linkfavorito">Link Favorito a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(LinksFavoritosItem linkfavorito)
        {
            try
            {
                DataContext.LinksFavoritos.InsertOnSubmit(linkfavorito);
                DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

                return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
            }
            catch (Exception exception)
            {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Atualiza um link favorito da lista.
        /// </summary>
        /// <param name = "linkfavorito">Link Favorito a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(LinksFavoritosItem linkfavorito)
        {
            try
            {
                var linkfavoritoItem = DataContext.LinksFavoritos.Where(c => c.Id.Equals(linkfavorito.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (linkfavoritoItem == null) return false;

                //Atualiza os campos
                linkfavoritoItem.Id = linkfavorito.Id;
                linkfavoritoItem.Title = linkfavorito.Title;
                linkfavoritoItem.Hiperlink = linkfavorito.Hiperlink;

                DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

                return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
            }
            catch (Exception exception)
            {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Transforma um DTOLinkFavorito na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "linkfavorito">Link Favorito a ser transformado.</param>
        /// <returns>A entidade transformada para LinksFavoritosItem.</returns>
        private static LinksFavoritosItem DTOToModel(DTOLinkFavorito linkfavorito)
        {
            var linkfavoritoItem = new LinksFavoritosItem
            {
                Id = linkfavorito.ID,
                Title = linkfavorito.Titulo,
                Hiperlink = linkfavorito.Hiperlink
            };

            return linkfavoritoItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo LinksFavoritosItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "linkfavoritoItem">Link Favorito a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTOLinkFavorito.</returns>
        private static DTOLinkFavorito ModelToDTO(LinksFavoritosItem linkfavoritoItem)
        {
            var linkfavorito = new DTOLinkFavorito
            {
                ID = linkfavoritoItem.Id,
                Titulo = linkfavoritoItem.Title,
                Hiperlink = linkfavoritoItem.Hiperlink
            };

            return linkfavorito;
        }
        #endregion

    }
}
