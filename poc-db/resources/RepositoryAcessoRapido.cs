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
    ///   Responsável por qualquer ação com a lista "Acesso Rápido".
    /// </summary>
    public class RepositoryAcessoRapido : RepositoryItem, IRepository<DTOAcessoRapido, AcessoRápidoItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Acesso Rápido" utilizando um novo DataContext.
        /// </summary>
        public RepositoryAcessoRapido() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Acesso Rápido" utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryAcessoRapido(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste o acesso rápido na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "acessoRapido">Acesso Rápido a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOAcessoRapido acessoRapido)
        {
            return !acessoRapido.ID.HasValue ? Add(DTOToModel(acessoRapido)) : Update(DTOToModel(acessoRapido));
        }

        /// <summary>
        ///   Deleta um acesso rápido da lista.
        /// </summary>
        /// <param name = "acessoRapido">Acesso Rápido a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOAcessoRapido acessoRapido)
        {
            try
            {
                var acessoRapidoItem = DataContext.AcessoRápido.Where(c => c.Id.Equals(acessoRapido.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (acessoRapidoItem == null) return false;

                DataContext.AcessoRápido.DeleteOnSubmit(acessoRapidoItem);
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
        ///   Retorna todos os itens da lista "Acesso Rápido".
        /// </summary>
        /// <returns>Todos os itens da lista "Acesso Rápido".</returns>
        public List<DTOAcessoRapido> GetAllItems()
        {
            try
            {
                var acessoRapidos = new List<DTOAcessoRapido>();
                DataContext.AcessoRápido.ToList().ForEach(acessoRapido => acessoRapidos.Add(ModelToDTO(acessoRapido)));
                return acessoRapidos;
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
        ///   Retorna determinados itens da lista "Acesso Rápido" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOAcessoRapido> GetItems(Expression<Func<AcessoRápidoItem, bool>> filter)
        {
            try
            {
                var acessoRapidos = new List<DTOAcessoRapido>();
                DataContext.AcessoRápido.Where(filter).ToList().ForEach(acessoRapido => acessoRapidos.Add(ModelToDTO(acessoRapido)));
                return acessoRapidos;
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
        ///   Adiciona um novo acesso rápido na lista.
        /// </summary>
        /// <param name = "acessoRapido">Acesso Rápido a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(AcessoRápidoItem acessoRapido)
        {
            try
            {
                DataContext.AcessoRápido.InsertOnSubmit(acessoRapido);
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
        ///   Atualiza um acesso rápido da lista.
        /// </summary>
        /// <param name = "acessoRapido">Acesso Rápido a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(AcessoRápidoItem acessoRapido)
        {
            try
            {
                var acessoRapidoItem = DataContext.AcessoRápido.Where(c => c.Id.Equals(acessoRapido.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (acessoRapidoItem == null) return false;

                //Atualiza os campos
                acessoRapidoItem.Id = acessoRapido.Id;
                acessoRapidoItem.Title = acessoRapido.Title;
                acessoRapidoItem.Hiperlink = acessoRapido.Hiperlink;

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
        ///   Transforma um DTOAcessoRapido na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "acessoRapido">Acesso Rápido a ser transformado.</param>
        /// <returns>A entidade transformada para AcessoRápidoItem.</returns>
        private static AcessoRápidoItem DTOToModel(DTOAcessoRapido acessoRapido)
        {
            var acessoRapidoItem = new AcessoRápidoItem
            {
                Id = acessoRapido.ID,
                Title = acessoRapido.Titulo,
                Hiperlink = acessoRapido.Hiperlink
            };

            return acessoRapidoItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo AcessoRápidoItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "acessoRapidoItem">Acesso Rápido a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTOAcessoRapido.</returns>
        private static DTOAcessoRapido ModelToDTO(AcessoRápidoItem acessoRapidoItem)
        {
            var acessoRapido = new DTOAcessoRapido
            {
                ID = acessoRapidoItem.Id,
                Titulo = acessoRapidoItem.Title,
                Hiperlink = acessoRapidoItem.Hiperlink
            };

            return acessoRapido;
        }
        #endregion
    }
}
