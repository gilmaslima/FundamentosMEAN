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
    ///   Responsável por qualquer ação com a lista "Enquetes - Respostas".
    /// </summary>
    public class RepositoryEnqueteRespostas : RepositoryItem, IRepository<DTOEnqueteResposta, EnqueteRespostasItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Enquetes - Respostas" utilizando um novo DataContext.
        /// </summary>
        public RepositoryEnqueteRespostas() { }

        /// <summary>
        ///   Inicializa o repositório da lista de "Enquetes - Respostas" utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryEnqueteRespostas(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste a enquete resposta na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "enqueteResposta">Enquete Resposta a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOEnqueteResposta enqueteResposta)
        {
            return !enqueteResposta.ID.HasValue ? Add(DTOToModel(enqueteResposta)) : Update(DTOToModel(enqueteResposta));
        }

        /// <summary>
        ///   Deleta uma enquete resposta da lista.
        /// </summary>
        /// <param name = "enqueteResposta">Enquete Resposta a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOEnqueteResposta enqueteResposta)
        {
            try
            {
                var enqueteRespostaItem = DataContext.EnqueteRespostas.Where(c => c.Id.Equals(enqueteResposta.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (enqueteRespostaItem == null) return false;

                DataContext.EnqueteRespostas.DeleteOnSubmit(enqueteRespostaItem);
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
        ///   Retorna todos os itens da lista "EnqueteRespostas".
        /// </summary>
        /// <returns>Todos os itens da lista "EnqueteRespostas".</returns>
        public List<DTOEnqueteResposta> GetAllItems()
        {
            try
            {
                var enqueteRespostas = new List<DTOEnqueteResposta>();
                DataContext.EnqueteRespostas.ToList().ForEach(enqueteResposta => enqueteRespostas.Add(ModelToDTO(enqueteResposta)));
                return enqueteRespostas;
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
        ///   Retorna determinados itens da lista "EnqueteRespostas" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOEnqueteResposta> GetItems(Expression<Func<EnqueteRespostasItem, bool>> filter)
        {
            try
            {
                var enqueteRespostas = new List<DTOEnqueteResposta>();
                DataContext.EnqueteRespostas.Where(filter).ToList().ForEach(enqueteResposta => enqueteRespostas.Add(ModelToDTO(enqueteResposta)));
                return enqueteRespostas;
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
        ///   Adiciona uma nova enquete resposta na lista.
        /// </summary>
        /// <param name = "enqueteResposta">Enquete Resposta a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(EnqueteRespostasItem enqueteResposta)
        {
            try
            {
                DataContext.EnqueteRespostas.InsertOnSubmit(enqueteResposta);
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
        ///   Atualiza uma enquete resposta da lista.
        /// </summary>
        /// <param name = "enqueteResposta">Enquete Resposta a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(EnqueteRespostasItem enqueteResposta)
        {
            try
            {
                var enqueteRespostaItem = DataContext.EnqueteRespostas.Where(c => c.Id.Equals(enqueteResposta.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (enqueteRespostaItem == null) return false;

                //Atualiza os campos
                enqueteRespostaItem.Id = enqueteResposta.Id;
                enqueteRespostaItem.Title = enqueteResposta.Title;
                enqueteRespostaItem.Resposta = enqueteResposta.Resposta;

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
        ///   Transforma um DTOEnqueteResposta na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "enqueteResposta">Enquete Resposta a ser transformado.</param>
        /// <returns>A entidade transformada para EnqueteRespostasItem.</returns>
        private static EnqueteRespostasItem DTOToModel(DTOEnqueteResposta enqueteResposta)
        {
            var enqueteRespostaItem = new EnqueteRespostasItem
                             {
                                 Id = enqueteResposta.ID,
                                 Title = enqueteResposta.Titulo,
                                 Resposta = enqueteResposta.Resposta
                             };

            return enqueteRespostaItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo EnqueteRespostasItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "enqueteRespostaItem">Enquete Resposta a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTOEnqueteResposta.</returns>
        private static DTOEnqueteResposta ModelToDTO(EnqueteRespostasItem enqueteRespostaItem)
        {
            var enqueteResposta = new DTOEnqueteResposta
                         {
                             ID = enqueteRespostaItem.Id,
                             Titulo = enqueteRespostaItem.Title,
                             Resposta = enqueteRespostaItem.Resposta,
                             RespostaEntity = enqueteRespostaItem
                         };

            return enqueteResposta;
        }
        #endregion
    }
}
