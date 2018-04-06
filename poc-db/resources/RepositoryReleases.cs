#region Used Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Linq;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository
{
    /// <summary>
    ///   Responsável por qualquer ação com a lista "Releases".
    /// </summary>
    public class RepositoryReleases : RepositoryItem, IRepository<DTORelease, ReleasesItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Releases' utilizando um novo DataContext.
        /// </summary>
        public RepositoryReleases() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Releases' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryReleases(GeneratedModelDataContext dataContext) : base(dataContext) {}
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste o release na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "release">Release a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTORelease release)
        {
            return !release.ID.HasValue ? Add(DTOToModel(release)) : Update(DTOToModel(release));
        }

        /// <summary>
        ///   Deleta um release da lista.
        /// </summary>
        /// <param name = "release">Release a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTORelease release)
        {
            try
            {
                var releaseItem = DataContext.Releases.Where(d => d.Id.Equals(release.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (releaseItem == null) return false;

                DataContext.Releases.DeleteOnSubmit(releaseItem);
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
        ///   Retorna todos os itens da lista "Releases".
        /// </summary>
        /// <returns>Todos os itens da lista "Releases".</returns>
        public List<DTORelease> GetAllItems()
        {
            try
            {
                var releases = new List<DTORelease>();
                DataContext.Releases.ToList().ForEach(release => releases.Add(ModelToDTO(release)));
                return releases;
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
        ///   Retorna determinados itens da lista "Releases" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTORelease> GetItems(Expression<Func<ReleasesItem, bool>> filter)
        {
            try
            {
                var releases = new List<DTORelease>();
                DataContext.Releases.Where(filter).ToList().ForEach(release => releases.Add(ModelToDTO(release)));
                return releases;
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
        ///   Adiciona um novo release na lista.
        /// </summary>
        /// <param name = "release">Release a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(ReleasesItem release)
        {
            try
            {
                DataContext.Releases.InsertOnSubmit(release);
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
        ///   Atualiza um release da lista.
        /// </summary>
        /// <param name = "release">Release a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(ReleasesItem release)
        {
            try
            {
                var releaseItem = DataContext.Releases.Where(d => d.Id.Equals(release.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (releaseItem == null) return false;

                //Atualiza os campos
                releaseItem.Id = release.Id;
                releaseItem.Title = release.Title;
                releaseItem.Descrição = release.Descrição;
                releaseItem.Data = release.Data;
                releaseItem.Hiperlink = release.Hiperlink;

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
        ///   Transforma um DTORelease na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "release">Release a ser transformado.</param>
        /// <returns>A entidade transformada para ReleasesItem.</returns>
        private static ReleasesItem DTOToModel(DTORelease release)
        {
            var releaseItem = new ReleasesItem
                              {
                                  Id = release.ID,
                                  Title = release.Titulo,
                                  Descrição = release.Descricao,
                                  Data = release.Data,
                                  Hiperlink = release.Hiperlink
                              };

            return releaseItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo ReleasesItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "releaseItem">Release a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTORelease.</returns>
        private static DTORelease ModelToDTO(ReleasesItem releaseItem)
        {
            var release = new DTORelease
                          {
                              ID = releaseItem.Id,
                              Titulo = releaseItem.Title,
                              Descricao = releaseItem.Descrição,
                              Data = releaseItem.Data,
                              Hiperlink = releaseItem.Hiperlink
                          };

            return release;
        }
        #endregion
    }
}