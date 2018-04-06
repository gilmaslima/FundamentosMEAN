#region Used Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Linq;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Microsoft.SharePoint;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository
{
    /// <summary>
    ///   Responsável por qualquer ação com a lista "Sons".
    /// </summary>
    public class RepositorySons : RepositoryItem, IRepository<DTOSom, SonsItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Sons' utilizando um novo DataContext.
        /// </summary>
        public RepositorySons() { }

        /// <summary>
        ///   Inicializa o repositório da lista de "Sons' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositorySons(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste o som na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "som">Som a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOSom som)
        {
            return !som.ID.HasValue ? Add(DTOToModel(som)) : Update(DTOToModel(som));
        }

        /// <summary>
        ///   Deleta um som da lista.
        /// </summary>
        /// <param name = "som">Som a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOSom som)
        {
            try
            {
                var somItem = DataContext.Sons.Where(d => d.Id.Equals(som.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (somItem == null) return false;

                DataContext.Sons.DeleteOnSubmit(somItem);
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
        ///   Retorna todos os itens da lista "Sons".
        /// </summary>
        /// <returns>Todos os itens da lista "Sons".</returns>
        public List<DTOSom> GetAllItems()
        {
            try
            {
                var sons = new List<DTOSom>();
                DataContext.Sons.ToList().ForEach(som => sons.Add(ModelToDTO(som)));
                return sons;
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
        ///   Retorna determinados itens da lista "Sons" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOSom> GetItems(Expression<Func<SonsItem, bool>> filter)
        {
            try
            {
                var sons = new List<DTOSom>();
                DataContext.Sons.Where(filter).ToList().ForEach(som => sons.Add(ModelToDTO(som)));
                return sons;
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
        ///   Adiciona um novo som na lista.
        /// </summary>
        /// <param name = "som">Som a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(SonsItem som)
        {
            try
            {
                DataContext.Sons.InsertOnSubmit(som);
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
        ///   Atualiza um som da lista.
        /// </summary>
        /// <param name = "som">Som a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(SonsItem som)
        {
            try
            {
                var somItem = DataContext.Sons.Where(d => d.Id.Equals(som.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (somItem == null) return false;

                //Atualiza os campos
                somItem.Id = som.Id;
                somItem.Title = som.Title;
                somItem.Url = som.Url;
                somItem.Descrição = som.Descrição;
                somItem.Data = som.Data;
                somItem.TipoDoSom = som.TipoDoSom;
                somItem.NúmeroDeExecuções = som.NúmeroDeExecuções;
                somItem.Anexos = som.Anexos;

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
        ///   Transforma um DTOSom na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "som">Som a ser transformado.</param>
        /// <returns>A entidade transformada para SonsItem.</returns>
        private static SonsItem DTOToModel(DTOSom som)
        {
            var somItem = new SonsItem
            {
                Id = som.ID,
                Title = som.Titulo,
                Url = som.Url,
                Descrição = som.Descricao,
                Data = som.Data,
                TipoDoSom = som.TipoDoSom,
                NúmeroDeExecuções = som.NumeroDeExecucoes,
                Anexos = new string[] { som.Anexos },
            };

            return somItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo SonsItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "somItem">Som a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTOSom.</returns>
        private static DTOSom ModelToDTO(SonsItem somItem)
        {
            string urlSom = string.Empty;
            // verificar campo de URL, no formato padrão, o sharepoint retorna o valor no seguinte formato:
            // [URL], [Descrição]
            if (!String.IsNullOrEmpty(somItem.Url))
            {
                SPFieldUrlValue urlValue = new SPFieldUrlValue(somItem.Url);
                urlSom = urlValue.Url;
            }
            else
                urlSom = somItem.Url;

            var som = new DTOSom
            {
                ID = somItem.Id,
                Titulo = somItem.Title,
                Url = urlSom,
                Descricao = somItem.Descrição,
                Data = somItem.Data,
                TipoDoSom = somItem.TipoDoSom,
                NumeroDeExecucoes = somItem.NúmeroDeExecuções,
                Anexos = object.ReferenceEquals(somItem.Anexos, null) ? string.Empty : somItem.Anexos.FirstOrDefault()
            };

            return som;
        }
        #endregion
    }
}