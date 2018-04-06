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
    ///   Responsável por qualquer ação com a lista "Perguntas Frequentes".
    /// </summary>
    public class RepositoryPerguntasFrequentesEcommerce : RepositoryItem, IRepository<DTOPerguntaFrequenteEcommerce, PerguntasFrequentesEcommerceItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Perguntas Frequentes' utilizando um novo DataContext.
        /// </summary>
        public RepositoryPerguntasFrequentesEcommerce() { }

        /// <summary>
        ///   Inicializa o repositório da lista de "Perguntas Frequentes' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryPerguntasFrequentesEcommerce(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste a pergunta frequente na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "perguntaFrequente">Pergunta Frequente a ser persistida.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOPerguntaFrequenteEcommerce perguntaFrequenteEcommerce)
        {
            return !perguntaFrequenteEcommerce.ID.HasValue ? Add(DTOToModel(perguntaFrequenteEcommerce)) : Update(DTOToModel(perguntaFrequenteEcommerce));
        }

        /// <summary>
        ///   Deleta uma pergunta frequente da lista.
        /// </summary>
        /// <param name = "perguntaFrequente">Pergunta Frequente a ser deletada.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOPerguntaFrequenteEcommerce perguntaFrequenteEcommerce)
        {
            try
            {
                var perguntaFrequenteEcommerceItem = DataContext.PerguntasFrequentesEcommerce.Where(n => n.Id.Equals(perguntaFrequenteEcommerce.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (perguntaFrequenteEcommerceItem == null) return false;

                DataContext.PerguntasFrequentesEcommerce.DeleteOnSubmit(perguntaFrequenteEcommerceItem);
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
        public List<DTOPerguntaFrequenteEcommerce> GetAllItems()
        {
            try
            {
                var perguntasFrequentesEcommerce = new List<DTOPerguntaFrequenteEcommerce>();
                DataContext.PerguntasFrequentesEcommerce.ToList().ForEach(perguntaFrequenteEcommerce => perguntasFrequentesEcommerce.Add(ModelToDTO(perguntaFrequenteEcommerce)));
                return perguntasFrequentesEcommerce;
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
        public List<DTOPerguntaFrequenteEcommerce> GetItems(Expression<Func<PerguntasFrequentesEcommerceItem, bool>> filter)
        {
            try
            {
                var perguntasFrequentesEcommerce = new List<DTOPerguntaFrequenteEcommerce>();
                DataContext.PerguntasFrequentesEcommerce.Where(filter).ToList().ForEach(
                    perguntaFrequenteEcommerce => perguntasFrequentesEcommerce.Add(ModelToDTO(perguntaFrequenteEcommerce)));
                return perguntasFrequentesEcommerce;
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
        private bool Add(PerguntasFrequentesEcommerceItem perguntaFrequenteEcommerce)
        {
            try
            {
                DataContext.PerguntasFrequentesEcommerce.InsertOnSubmit(perguntaFrequenteEcommerce);
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
        private bool Update(PerguntasFrequentesEcommerceItem perguntaFrequenteEcommerce)
        {
            try
            {
                var perguntaFrequenteEcommerceItem = DataContext.PerguntasFrequentesEcommerce.Where(n => n.Id.Equals(perguntaFrequenteEcommerce.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (perguntaFrequenteEcommerceItem == null) return false;

                //Atualiza os campos
                perguntaFrequenteEcommerceItem.Id = perguntaFrequenteEcommerce.Id;
                perguntaFrequenteEcommerceItem.Title = perguntaFrequenteEcommerce.Title;
                perguntaFrequenteEcommerceItem.Resposta = perguntaFrequenteEcommerce.Resposta;
                perguntaFrequenteEcommerceItem.Área = perguntaFrequenteEcommerce.Área;
                perguntaFrequenteEcommerceItem.Assunto = perguntaFrequenteEcommerce.Assunto;
                perguntaFrequenteEcommerceItem.Ordem = perguntaFrequenteEcommerce.Ordem;

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
        private static PerguntasFrequentesEcommerceItem DTOToModel(DTOPerguntaFrequenteEcommerce perguntaFrequenteEcommerce)
        {
            var perguntaFrequenteEcommerceItem = new PerguntasFrequentesEcommerceItem
            {
                Id = perguntaFrequenteEcommerce.ID,
                Title = perguntaFrequenteEcommerce.Pergunta,
                Resposta = perguntaFrequenteEcommerce.Resposta,
                Área = perguntaFrequenteEcommerce.Area,
                Assunto = perguntaFrequenteEcommerce.Assunto,
                Ordem = perguntaFrequenteEcommerce.Ordem
            };

            return perguntaFrequenteEcommerceItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo PerguntasFrequentesItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "perguntaFrequenteItem">Pergunta Frequente a ser transformada.</param>
        /// <returns>A entidade do modelo transformada para DTOPerguntaFrequente.</returns>
        private static DTOPerguntaFrequenteEcommerce ModelToDTO(PerguntasFrequentesEcommerceItem perguntaFrequenteEcommerceItem)
        {
            var perguntaFrequenteEcommerce = new DTOPerguntaFrequenteEcommerce
            {
                ID = perguntaFrequenteEcommerceItem.Id,
                Pergunta = perguntaFrequenteEcommerceItem.Title,
                Resposta = perguntaFrequenteEcommerceItem.Resposta,
                Area = perguntaFrequenteEcommerceItem.Área,
                Assunto = perguntaFrequenteEcommerceItem.Assunto,
                Ordem = perguntaFrequenteEcommerceItem.Ordem
            };

            return perguntaFrequenteEcommerce;
        }
        #endregion
    }
}
