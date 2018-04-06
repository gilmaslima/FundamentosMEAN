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
    ///   Responsável por qualquer ação com a lista "Enquetes - Perguntas".
    /// </summary>
    public class RepositoryEnquetePerguntas : RepositoryItem, IRepository<DTOEnquetePergunta, EnquetePerguntasItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Enquetes - Perguntas" utilizando um novo DataContext.
        /// </summary>
        public RepositoryEnquetePerguntas() { }

        /// <summary>
        ///   Inicializa o repositório da lista de "Enquetes - Perguntas" utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryEnquetePerguntas(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste a enquete pergunta na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "enquetePergunta">Enquete Perguntas a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOEnquetePergunta enquetePergunta)
        {
            return !enquetePergunta.ID.HasValue ? Add(DTOToModel(enquetePergunta)) : Update(DTOToModel(enquetePergunta));
        }

        /// <summary>
        ///   Deleta uma enquete pergunta da lista.
        /// </summary>
        /// <param name = "enquetePergunta">Enquete Pergunta a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOEnquetePergunta enquetePergunta)
        {
            try
            {
                var enquetePerguntaItem = DataContext.EnquetePerguntas.Where(c => c.Id.Equals(enquetePergunta.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (enquetePerguntaItem == null) return false;

                DataContext.EnquetePerguntas.DeleteOnSubmit(enquetePerguntaItem);
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
        ///   Retorna todos os itens da lista "EnquetePerguntas".
        /// </summary>
        /// <returns>Todos os itens da lista "EnquetePerguntas".</returns>
        public List<DTOEnquetePergunta> GetAllItems()
        {
            try
            {
                var enquetePerguntas = new List<DTOEnquetePergunta>();
                DataContext.EnquetePerguntas.ToList().ForEach(enquetePergunta => enquetePerguntas.Add(ModelToDTO(enquetePergunta)));
                return enquetePerguntas;
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
        ///   Retorna determinados itens da lista "EnquetePerguntas" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOEnquetePergunta> GetItems(Expression<Func<EnquetePerguntasItem, bool>> filter)
        {
            try
            {
                var enquetePerguntas = new List<DTOEnquetePergunta>();
                DataContext.EnquetePerguntas.Where(filter).ToList().ForEach(enquetePergunta => enquetePerguntas.Add(ModelToDTO(enquetePergunta)));
                return enquetePerguntas;
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
        ///   Adiciona uma nova enquete perguntas na lista.
        /// </summary>
        /// <param name = "enquetePergunta">Enquete Pergunta a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(EnquetePerguntasItem enquetePergunta)
        {
            try
            {
                DataContext.EnquetePerguntas.InsertOnSubmit(enquetePergunta);
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
        ///   Atualiza uma enquete pergunta da lista.
        /// </summary>
        /// <param name = "enquetePergunta">Enquete Pergunta a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(EnquetePerguntasItem enquetePergunta)
        {
            try
            {
                var enquetePerguntaItem = DataContext.EnquetePerguntas.Where(c => c.Id.Equals(enquetePergunta.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (enquetePerguntaItem == null) return false;

                //Atualiza os campos
                enquetePerguntaItem.Id = enquetePergunta.Id;
                enquetePerguntaItem.Title = enquetePergunta.Title;
                enquetePerguntaItem.Pergunta = enquetePergunta.Pergunta;
                enquetePerguntaItem.Ativo = enquetePergunta.Ativo;
                enquetePerguntaItem.DataDeInício = enquetePergunta.DataDeInício;
                enquetePerguntaItem.DataDeFim = enquetePergunta.DataDeFim;

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
        ///   Transforma um DTOEnquetePergunta na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "enquetePergunta">Enquete Pergunta a ser transformado.</param>
        /// <returns>A entidade transformada para EnquetePerguntasItem.</returns>
        private static EnquetePerguntasItem DTOToModel(DTOEnquetePergunta enquetePergunta)
        {
            var enquetePerguntaItem = new EnquetePerguntasItem
            {
                Id = enquetePergunta.ID,
                Title = enquetePergunta.Titulo,
                Pergunta = enquetePergunta.Pergunta,
                Ativo = enquetePergunta.Ativo,
                DataDeInício = enquetePergunta.DataDeInicio,
                DataDeFim = enquetePergunta.DataDeFim
            };

            return enquetePerguntaItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo EnquetePerguntasItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "enquetePerguntaItem">Enquete Pergunta a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTOEnquetePergunta.</returns>
        private static DTOEnquetePergunta ModelToDTO(EnquetePerguntasItem enquetePerguntaItem)
        {
            var enquetePergunta = new DTOEnquetePergunta
            {
                ID = enquetePerguntaItem.Id,
                Titulo = enquetePerguntaItem.Title,
                Pergunta = enquetePerguntaItem.Pergunta,
                Ativo = enquetePerguntaItem.Ativo,
                DataDeInicio = enquetePerguntaItem.DataDeInício,
                DataDeFim = enquetePerguntaItem.DataDeFim,
                PerguntaEntity = enquetePerguntaItem
            };

            return enquetePergunta;
        }
        #endregion

    }
}
