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
    /// 
    /// </summary>
    public class RepositoryRedecardClique : RepositoryItem, IRepository<DTORedecardClique, RedecardEmUmCliqueItem>
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public RepositoryRedecardClique() {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        public RepositoryRedecardClique(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliqueItem"></param>
        /// <returns></returns>
        public bool Persist(DTORedecardClique cliqueItem)
        {
            return !cliqueItem.ID.HasValue ? Add(DTOToModel(cliqueItem)) : Update(DTOToModel(cliqueItem));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliqueItem"></param>
        /// <returns></returns>
        public bool Delete(DTORedecardClique cliqueItem)
        {
            try
            {
                var cliqueItemDel = DataContext.RedecardEmUmClique.Where(n => n.Id.Equals(cliqueItem.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (cliqueItemDel == null) return false;

                DataContext.RedecardEmUmClique.DeleteOnSubmit(cliqueItemDel);
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
        /// 
        /// </summary>
        /// <returns></returns>
        public List<DTORedecardClique> GetAllItems()
        {
            try
            {
                var items = new List<DTORedecardClique>();
                DataContext.RedecardEmUmClique.ToList().ForEach(item => items.Add(ModelToDTO(item)));
                return items;
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
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<DTORedecardClique> GetItems(Expression<Func<RedecardEmUmCliqueItem, bool>> filter)
        {
            try
            {
                var items = new List<DTORedecardClique>();
                DataContext.RedecardEmUmClique.Where(filter).ToList().ForEach(item => items.Add(ModelToDTO(item)));
                return items;
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
        /// 
        /// </summary>
        /// <param name="cliqueItem"></param>
        /// <returns></returns>
        private bool Add(RedecardEmUmCliqueItem cliqueItem)
        {
            try
            {
                DataContext.RedecardEmUmClique.InsertOnSubmit(cliqueItem);
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
        /// 
        /// </summary>
        /// <param name="cliqueItem"></param>
        /// <returns></returns>
        private bool Update(RedecardEmUmCliqueItem cliqueItem)
        {
            try
            {
                var itemCliqueEdit = DataContext.RedecardEmUmClique.Where(n => n.Id.Equals(cliqueItem.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (itemCliqueEdit == null) return false;

                //Atualiza os campos
                itemCliqueEdit.Id = cliqueItem.Id;
                itemCliqueEdit.Title = cliqueItem.Title;
                itemCliqueEdit.EndereçoDeAcesso = cliqueItem.EndereçoDeAcesso;

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
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static RedecardEmUmCliqueItem DTOToModel(DTORedecardClique item)
        {
            var itemClique = new RedecardEmUmCliqueItem
                           {
                               Id = item.ID,
                               Title = item.Titulo,
                               EndereçoDeAcesso = item.Url
                           };

            return itemClique;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemClique"></param>
        /// <returns></returns>
        private static DTORedecardClique ModelToDTO(RedecardEmUmCliqueItem itemClique)
        {
            var item = new DTORedecardClique
                       {
                           ID = itemClique.Id,
                           Titulo = itemClique.Title,
                           Url = itemClique.EndereçoDeAcesso
                       };

            return item;
        }
        #endregion
    }
}