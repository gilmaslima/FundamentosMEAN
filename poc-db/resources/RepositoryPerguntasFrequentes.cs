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
    ///   Responsável por qualquer ação com a lista "Perguntas Frequentes".
    /// </summary>
    public class RepositoryPerguntasFrequentes : RepositoryItem, IRepository<DTOPerguntaFrequente, PerguntasFrequentesItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Perguntas Frequentes' utilizando um novo DataContext.
        /// </summary>
        public RepositoryPerguntasFrequentes() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Perguntas Frequentes' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryPerguntasFrequentes(GeneratedModelDataContext dataContext) : base(dataContext) {}
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste a pergunta frequente na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "perguntaFrequente">Pergunta Frequente a ser persistida.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOPerguntaFrequente perguntaFrequente)
        {
            return !perguntaFrequente.ID.HasValue ? Add(DTOToModel(perguntaFrequente)) : Update(DTOToModel(perguntaFrequente));
        }

        /// <summary>
        ///   Deleta uma pergunta frequente da lista.
        /// </summary>
        /// <param name = "perguntaFrequente">Pergunta Frequente a ser deletada.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOPerguntaFrequente perguntaFrequente)
        {
            try
            {
                var perguntaFrequenteItem = DataContext.PerguntasFrequentes.Where(n => n.Id.Equals(perguntaFrequente.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (perguntaFrequenteItem == null) return false;

                DataContext.PerguntasFrequentes.DeleteOnSubmit(perguntaFrequenteItem);
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
        ///   Retorna todos os itens da lista "Perguntas Frequentes".
        /// </summary>
        /// <returns>Todos os itens da lista "Perguntas Frequentes".</returns>
        public List<DTOPerguntaFrequente> GetAllItems()
        {
            try
            {
                var perguntasFrequentes = new List<DTOPerguntaFrequente>();
                DataContext.PerguntasFrequentes.ToList().ForEach(perguntaFrequente => perguntasFrequentes.Add(ModelToDTO(perguntaFrequente)));
                return perguntasFrequentes;
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
        ///   Retorna determinados itens da lista "Perguntas Frequentes" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOPerguntaFrequente> GetItems(Expression<Func<PerguntasFrequentesItem, bool>> filter)
        {
            try
            {
                var perguntasFrequentes = new List<DTOPerguntaFrequente>();
                DataContext.PerguntasFrequentes.Where(filter).ToList().ForEach(
                    perguntaFrequente => perguntasFrequentes.Add(ModelToDTO(perguntaFrequente)));
                return perguntasFrequentes;
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
        ///   Adiciona uma nova pergunta frequente na lista.
        /// </summary>
        /// <param name = "perguntaFrequente">Pergunta Frequente a ser inserida.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(PerguntasFrequentesItem perguntaFrequente)
        {
            try
            {
                DataContext.PerguntasFrequentes.InsertOnSubmit(perguntaFrequente);
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
        ///   Atualiza uma pergunta frequente da lista.
        /// </summary>
        /// <param name = "perguntaFrequente">Pergunta Frequente a ser atualizada.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(PerguntasFrequentesItem perguntaFrequente)
        {
            try
            {
                var perguntaFrequenteItem = DataContext.PerguntasFrequentes.Where(n => n.Id.Equals(perguntaFrequente.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (perguntaFrequenteItem == null) return false;

                //Atualiza os campos
                perguntaFrequenteItem.Id = perguntaFrequente.Id;
                perguntaFrequenteItem.Title = perguntaFrequente.Title;
                perguntaFrequenteItem.Resposta = perguntaFrequente.Resposta;
                perguntaFrequenteItem.Área = perguntaFrequente.Área;
                perguntaFrequenteItem.Assunto = perguntaFrequente.Assunto;
                perguntaFrequenteItem.Ordem = perguntaFrequente.Ordem;

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
        ///   Transforma uma DTOPerguntaFrequente na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "perguntaFrequente">Pergunta Frequente a ser transformada.</param>
        /// <returns>A entidade transformada para PerguntasFrequentesItem.</returns>
        private static PerguntasFrequentesItem DTOToModel(DTOPerguntaFrequente perguntaFrequente)
        {
            var perguntaFrequenteItem = new PerguntasFrequentesItem
                                        {
                                            Id = perguntaFrequente.ID,
                                            Title = perguntaFrequente.Pergunta,
                                            Resposta = perguntaFrequente.Resposta,
                                            Área = perguntaFrequente.Area,
                                            Assunto = perguntaFrequente.Assunto,
                                            Ordem = perguntaFrequente.Ordem
                                        };

            return perguntaFrequenteItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo PerguntasFrequentesItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "perguntaFrequenteItem">Pergunta Frequente a ser transformada.</param>
        /// <returns>A entidade do modelo transformada para DTOPerguntaFrequente.</returns>
        private static DTOPerguntaFrequente ModelToDTO(PerguntasFrequentesItem perguntaFrequenteItem)
        {
            var perguntaFrequente = new DTOPerguntaFrequente
                                    {
                                        ID = perguntaFrequenteItem.Id,
                                        Pergunta = perguntaFrequenteItem.Title,
                                        Resposta = perguntaFrequenteItem.Resposta,
                                        Area = perguntaFrequenteItem.Área,
                                        Assunto = perguntaFrequenteItem.Assunto,
                                        Ordem = perguntaFrequenteItem.Ordem
                                    };

            return perguntaFrequente;
        }
        #endregion
    }
}