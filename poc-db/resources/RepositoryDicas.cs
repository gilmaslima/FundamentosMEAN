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
    ///   Responsável por qualquer ação com a lista "Dicas".
    /// </summary>
    public class RepositoryDicas : RepositoryItem, IRepository<DTODica, DicasItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Dicas' utilizando um novo DataContext.
        /// </summary>
        public RepositoryDicas() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Dicas' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryDicas(GeneratedModelDataContext dataContext) : base(dataContext) {}
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste a dica na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "dica">Dica a ser persistida.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTODica dica)
        {
            return !dica.ID.HasValue ? Add(DTOToModel(dica)) : Update(DTOToModel(dica));
        }

        /// <summary>
        ///   Deleta uma dica da lista.
        /// </summary>
        /// <param name = "dica">Dica a ser deletada.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTODica dica)
        {
            try
            {
                var dicaItem = DataContext.Dicas.Where(n => n.Id.Equals(dica.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (dicaItem == null) return false;

                DataContext.Dicas.DeleteOnSubmit(dicaItem);
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
        ///   Retorna todos os itens da lista "Dicas".
        /// </summary>
        /// <returns>Todos os itens da lista "Dicas".</returns>
        public List<DTODica> GetAllItems()
        {
            try
            {
                var dicas = new List<DTODica>();
                DataContext.Dicas.ToList().ForEach(dica => dicas.Add(ModelToDTO(dica)));
                return dicas;
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
        ///   Retorna determinados itens da lista "Dicas" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTODica> GetItems(Expression<Func<DicasItem, bool>> filter)
        {
            try
            {
                var dicas = new List<DTODica>();
                DataContext.Dicas.Where(filter).ToList().ForEach(dica => dicas.Add(ModelToDTO(dica)));
                return dicas;
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
        ///   Adiciona uma nova dica na lista.
        /// </summary>
        /// <param name = "dica">Dica a ser inserida.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(DicasItem dica)
        {
            try
            {
                DataContext.Dicas.InsertOnSubmit(dica);
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
        ///   Atualiza uma dica da lista.
        /// </summary>
        /// <param name = "dica">Dica a ser atualizada.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(DicasItem dica)
        {
            try
            {
                var dicaItem = DataContext.Dicas.Where(n => n.Id.Equals(dica.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (dicaItem == null) return false;

                //Atualiza os campos
                dicaItem.Id = dica.Id;
                dicaItem.Title = dica.Title;
                dicaItem.Resumo = dica.Resumo;
                dicaItem.Dica = dica.Dica;
                dicaItem.TipoDaDica = dica.TipoDaDica;
                dicaItem.Categoria = dica.Categoria;
                dicaItem.NúmeroDeExibições = dica.NúmeroDeExibições;                

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
        ///   Transforma uma DTODica na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "dica">Dica a ser transformada.</param>
        /// <returns>A entidade transformada para DicasItem.</returns>
        private static DicasItem DTOToModel(DTODica dica)
        {
            var dicaItem = new DicasItem
                           {
                               Id = dica.ID,
                               Title = dica.Titulo,
                               Resumo = dica.Resumo,
                               Dica = dica.Dica,
                               TipoDaDica = dica.TipoDaDica,
                               Categoria = dica.Categoria,
                               NúmeroDeExibições = dica.NumeroDeExibicoes
                           };

            return dicaItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo DicasItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "dicaItem">Dica a ser transformada.</param>
        /// <returns>A entidade do modelo transformada para DTODica.</returns>
        private static DTODica ModelToDTO(DicasItem dicaItem)
        {
            var dica = new DTODica
                       {
                           ID = dicaItem.Id,
                           Titulo = dicaItem.Title,
                           Resumo = dicaItem.Resumo,
                           Dica = dicaItem.Dica,
                           TipoDaDica = dicaItem.TipoDaDica,
                           Categoria = dicaItem.Categoria,
                           NumeroDeExibicoes = dicaItem.NúmeroDeExibições,
                           DataPublicacao = dicaItem.DataPublicacao
                       };

            return dica;
        }
        #endregion
    }
}