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
    public class RepositoryCompartilhe : RepositoryItem, IRepository<DTOCompartilhe, CompartilheItem>
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public RepositoryCompartilhe() {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        public RepositoryCompartilhe(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compartilheItem"></param>
        /// <returns></returns>
        public bool Persist(DTOCompartilhe compartilheItem)
        {
            return !compartilheItem.ID.HasValue ? Add(DTOToModel(compartilheItem)) : Update(DTOToModel(compartilheItem));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compartilheItem"></param>
        /// <returns></returns>
        public bool Delete(DTOCompartilhe compartilheItem)
        {
            try
            {
                var compartilheItemDel = DataContext.RedecardEmUmClique.Where(n => n.Id.Equals(compartilheItem.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (compartilheItemDel == null) return false;

                DataContext.RedecardEmUmClique.DeleteOnSubmit(compartilheItemDel);
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
        public List<DTOCompartilhe> GetAllItems()
        {
            try
            {
                var items = new List<DTOCompartilhe>();
                DataContext.Compartilhe.ToList().ForEach(item => items.Add(ModelToDTO(item)));
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
        public List<DTOCompartilhe> GetItems(Expression<Func<CompartilheItem, bool>> filter)
        {
            try
            {
                var items = new List<DTOCompartilhe>();
                DataContext.Compartilhe.Where(filter).ToList().ForEach(item => items.Add(ModelToDTO(item)));
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
        /// <param name="compartilheItem"></param>
        /// <returns></returns>
        private bool Add(CompartilheItem compartilheItem)
        {
            try
            {
                DataContext.Compartilhe.InsertOnSubmit(compartilheItem);
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
        /// <param name="compartilheItem"></param>
        /// <returns></returns>
        private bool Update(CompartilheItem compartilheItem)
        {
            try
            {
                var itemCompartilheEdit = DataContext.Compartilhe.Where(n => n.Id.Equals(compartilheItem.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (itemCompartilheEdit == null) return false;

                //Atualiza os campos
                itemCompartilheEdit.Id = compartilheItem.Id;
                itemCompartilheEdit.Title = compartilheItem.Title;
                itemCompartilheEdit.FormatoDeURL = compartilheItem.FormatoDeURL;
                itemCompartilheEdit.ClasseDeEstilo = compartilheItem.ClasseDeEstilo;
                itemCompartilheEdit.Visível = compartilheItem.Visível;

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
        private static CompartilheItem DTOToModel(DTOCompartilhe item)
        {
            var itemCompartilhe = new CompartilheItem
                           {
                               Id = item.ID,
                               Title = item.Titulo,
                               FormatoDeURL = item.Url,
                               ClasseDeEstilo = item.Class,
                               Visível = item.Visivel
                           };

            return itemCompartilhe;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemCompartilhe"></param>
        /// <returns></returns>
        private static DTOCompartilhe ModelToDTO(CompartilheItem itemCompartilhe)
        {
            var item = new DTOCompartilhe
                       {
                           ID = itemCompartilhe.Id,
                           Titulo = itemCompartilhe.Title,
                           Url = itemCompartilhe.FormatoDeURL,
                           Class = itemCompartilhe.ClasseDeEstilo,
                           Visivel = (bool)itemCompartilhe.Visível
                       };

            return item;
        }
        #endregion
    }
}