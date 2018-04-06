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
    ///   Responsável por qualquer ação com a lista "Dúvidas Frequentes".
    /// </summary>
    public class RepositoryDuvidasFrequentes : RepositoryItem, IRepository<DTODuvidaFrequente, DúvidasFrequentesItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Dúvidas Frequentes" utilizando um novo DataContext.
        /// </summary>
        public RepositoryDuvidasFrequentes() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Dúvidas Frequentes" utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryDuvidasFrequentes(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste a dúvida frequente na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "duvidafrequente">Dúvida Frequente a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTODuvidaFrequente duvidafrequente)
        {
            return !duvidafrequente.ID.HasValue ? Add(DTOToModel(duvidafrequente)) : Update(DTOToModel(duvidafrequente));
        }

        /// <summary>
        ///   Deleta uma dúvida frequente da lista.
        /// </summary>
        /// <param name = "duvidafrequente">Dúvida Frequente a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTODuvidaFrequente duvidafrequente)
        {
            try
            {
                var duvidafrequenteItem = DataContext.DúvidasFrequentes.Where(c => c.Id.Equals(duvidafrequente.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (duvidafrequenteItem == null) return false;

                DataContext.DúvidasFrequentes.DeleteOnSubmit(duvidafrequenteItem);
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
        ///   Retorna todos os itens da lista "DúvidasFrequentes".
        /// </summary>
        /// <returns>Todos os itens da lista "DúvidasFrequentes".</returns>
        public List<DTODuvidaFrequente> GetAllItems()
        {
            try
            {
                var duvidasfrequentes = new List<DTODuvidaFrequente>();
                DataContext.DúvidasFrequentes.ToList().ForEach(duvidafrequente => duvidasfrequentes.Add(ModelToDTO(duvidafrequente)));
                return duvidasfrequentes;
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
        ///   Retorna determinados itens da lista "DúvidasFrequentes" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTODuvidaFrequente> GetItems(Expression<Func<DúvidasFrequentesItem, bool>> filter)
        {
            try
            {
                var duvidasfrequentes = new List<DTODuvidaFrequente>();
                DataContext.DúvidasFrequentes.Where(filter).ToList().ForEach(duvidafrequente => duvidasfrequentes.Add(ModelToDTO(duvidafrequente)));
                return duvidasfrequentes;
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
        ///   Adiciona uma nova dúvida frequente na lista.
        /// </summary>
        /// <param name = "duvidafrequente">Dúvida Frequente a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(DúvidasFrequentesItem duvidafrequente)
        {
            try
            {
                DataContext.DúvidasFrequentes.InsertOnSubmit(duvidafrequente);
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
        ///   Atualiza uma dúvida frequente da lista.
        /// </summary>
        /// <param name = "duvidafrequente">Dúvida Frequente a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(DúvidasFrequentesItem duvidafrequente)
        {
            try
            {
                var duvidafrequenteItem = DataContext.DúvidasFrequentes.Where(c => c.Id.Equals(duvidafrequente.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (duvidafrequenteItem == null) return false;

                //Atualiza os campos
                duvidafrequenteItem.Id = duvidafrequente.Id;
                duvidafrequenteItem.Title = duvidafrequente.Title;
                duvidafrequenteItem.Assunto = duvidafrequente.Assunto;
                duvidafrequenteItem.Pergunta = duvidafrequente.Pergunta;
                duvidafrequenteItem.Resposta = duvidafrequente.Resposta;
                duvidafrequenteItem.Ordem = duvidafrequente.Ordem;

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
        ///   Transforma um DTODuvidaFrequente na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "duvidafrequente">Dúvida Frequente a ser transformado.</param>
        /// <returns>A entidade transformada para DúvidasFrequentesItem.</returns>
        private static DúvidasFrequentesItem DTOToModel(DTODuvidaFrequente duvidafrequente)
        {
            var duvidafrequenteItem = new DúvidasFrequentesItem
            {
                Id = duvidafrequente.ID,
                Title = duvidafrequente.Titulo,
                Assunto = duvidafrequente.Assunto,
                Pergunta = duvidafrequente.Pergunta,
                Resposta = duvidafrequente.Resposta,
                Ordem = duvidafrequente.Ordem
            };

            return duvidafrequenteItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo DúvidasFrequentesItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "duvidafrequenteItem">Dúvida Frequente a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTODuvidaFrequente.</returns>
        private static DTODuvidaFrequente ModelToDTO(DúvidasFrequentesItem duvidafrequenteItem)
        {
            var duvidafrequente = new DTODuvidaFrequente
            {
                ID = duvidafrequenteItem.Id,
                Titulo = duvidafrequenteItem.Title,
                Pergunta = duvidafrequenteItem.Pergunta,
                Resposta = duvidafrequenteItem.Resposta,
                Ordem = duvidafrequenteItem.Ordem,
                Assunto = duvidafrequenteItem.Assunto
            };

            return duvidafrequente;
        }
        #endregion
    }
}
