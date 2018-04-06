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
    ///   Responsável por qualquer ação com a lista "Prêmios".
    /// </summary>
    public class RepositoryPrêmios : RepositoryItem, IRepository<DTOPremio, PrêmiosItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Prêmios' utilizando um novo DataContext.
        /// </summary>
        public RepositoryPrêmios() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Prêmios' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryPrêmios(GeneratedModelDataContext dataContext) : base(dataContext) {}
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste o premio na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "premio">Prêmio a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOPremio premio)
        {
            return !premio.ID.HasValue ? Add(DTOToModel(premio)) : Update(DTOToModel(premio));
        }

        /// <summary>
        ///   Deleta um premio da lista.
        /// </summary>
        /// <param name = "premio">Prêmio a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOPremio premio)
        {
            try
            {
                var premioItem = DataContext.Prêmios.Where(d => d.Id.Equals(premio.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (premioItem == null) return false;

                DataContext.Prêmios.DeleteOnSubmit(premioItem);
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
        ///   Retorna todos os itens da lista "Prêmios".
        /// </summary>
        /// <returns>Todos os itens da lista "Prêmios".</returns>
        public List<DTOPremio> GetAllItems()
        {
            try
            {
                var premios = new List<DTOPremio>();
                DataContext.Prêmios.ToList().ForEach(premio => premios.Add(ModelToDTO(premio)));
                return premios;
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
        ///   Retorna determinados itens da lista "Prêmios" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOPremio> GetItems(Expression<Func<PrêmiosItem, bool>> filter)
        {
            try
            {
                var premios = new List<DTOPremio>();
                DataContext.Prêmios.Where(filter).ToList().ForEach(premio => premios.Add(ModelToDTO(premio)));
                return premios;
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
        ///   Adiciona um novo premio na lista.
        /// </summary>
        /// <param name = "premio">Prêmio a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(PrêmiosItem premio)
        {
            try
            {
                DataContext.Prêmios.InsertOnSubmit(premio);
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
        ///   Atualiza um premio da lista.
        /// </summary>
        /// <param name = "premio">Prêmio a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(PrêmiosItem premio)
        {
            try
            {
                var premioItem = DataContext.Prêmios.Where(d => d.Id.Equals(premio.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (premioItem == null) return false;

                //Atualiza os campos
                premioItem.Id = premio.Id;
                premioItem.Title = premio.Title;
                premioItem.Imagem = premio.Imagem;
                premioItem.Descrição = premio.Descrição;
                premioItem.Data = premio.Data;
                premioItem.Categoria = premio.Categoria;

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
        ///   Transforma um DTOPremio na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "premio">Prêmio a ser transformado.</param>
        /// <returns>A entidade transformada para PrêmiosItem.</returns>
        private static PrêmiosItem DTOToModel(DTOPremio premio)
        {
            var premioItem = new PrêmiosItem
                             {
                                 Id = premio.ID,
                                 Title = premio.Titulo,
                                 Imagem = premio.Imagem,
                                 Descrição = premio.Descricao,
                                 Data = premio.Data,
                                 Categoria = premio.Categoria
                             };

            return premioItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo PrêmiosItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "premioItem">Prêmio a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTOPremio.</returns>
        private static DTOPremio ModelToDTO(PrêmiosItem premioItem)
        {
            string sUrl = string.Empty;
            // verificar campo de URL, no formato padrão, o sharepoint retorna o valor no seguinte formato:
            // [URL], [Descrição]
            if (!String.IsNullOrEmpty(premioItem.Imagem)) {
                SPFieldUrlValue urlValue = new SPFieldUrlValue(premioItem.Imagem);
                sUrl = urlValue.Url;
            }
            else
                sUrl = premioItem.Imagem;

            var premio = new DTOPremio
                         {
                             ID = premioItem.Id,
                             Titulo = premioItem.Title,
                             Imagem = sUrl,
                             Descricao = premioItem.Descrição,
                             Data = premioItem.Data,
                             Categoria = premioItem.Categoria
                         };

            return premio;
        }
        #endregion
    }
}