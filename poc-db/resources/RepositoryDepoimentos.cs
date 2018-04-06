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
    ///   Responsável por qualquer ação com a lista "Depoimentos".
    /// </summary>
    public class RepositoryDepoimentos : RepositoryItem, IRepository<DTODepoimento, DepoimentosItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Depoimentos' utilizando um novo DataContext.
        /// </summary>
        public RepositoryDepoimentos() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Depoimentos' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryDepoimentos(GeneratedModelDataContext dataContext) : base(dataContext) {}
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste o depoimento na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "depoimento">Depoimento a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTODepoimento depoimento)
        {
            return !depoimento.ID.HasValue ? Add(DTOToModel(depoimento)) : Update(DTOToModel(depoimento));
        }

        /// <summary>
        ///   Deleta um depoimento da lista.
        /// </summary>
        /// <param name = "depoimento">Depoimento a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTODepoimento depoimento)
        {
            try
            {
                var depoimentoItem = DataContext.Depoimentos.Where(d => d.Id.Equals(depoimento.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (depoimentoItem == null) return false;

                DataContext.Depoimentos.DeleteOnSubmit(depoimentoItem);
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
        ///   Retorna todos os itens da lista "Depoimentos".
        /// </summary>
        /// <returns>Todos os itens da lista "Depoimentos".</returns>
        public List<DTODepoimento> GetAllItems()
        {
            try
            {
                var depoimentos = new List<DTODepoimento>();
                DataContext.Depoimentos.ToList().ForEach(depoimento => depoimentos.Add(ModelToDTO(depoimento)));
                return depoimentos;
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
        ///   Retorna determinados itens da lista "Depoimentos" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTODepoimento> GetItems(Expression<Func<DepoimentosItem, bool>> filter)
        {
            try
            {
                var depoimentos = new List<DTODepoimento>();
                DataContext.Depoimentos.Where(filter).ToList().ForEach(depoimento => depoimentos.Add(ModelToDTO(depoimento)));
                return depoimentos;
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
        ///   Adiciona um novo depoimento na lista.
        /// </summary>
        /// <param name = "depoimento">Depoimento a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(DepoimentosItem depoimento)
        {
            try
            {
                DataContext.Depoimentos.InsertOnSubmit(depoimento);
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
        ///   Atualiza um depoimento da lista.
        /// </summary>
        /// <param name = "depoimento">Depoimento a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(DepoimentosItem depoimento)
        {
            try
            {
                var depoimentoItem = DataContext.Depoimentos.Where(d => d.Id.Equals(depoimento.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (depoimentoItem == null) return false;

                //Atualiza os campos
                depoimentoItem.Id = depoimento.Id;
                depoimentoItem.Title = depoimento.Title;
                depoimentoItem.Depoimento = depoimento.Depoimento;
                depoimentoItem.Ramo = depoimento.Ramo;
                depoimentoItem.ProdutoServico = depoimento.ProdutoServico;
                depoimentoItem.Estado = depoimento.Estado;

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
        ///   Transforma um DTODepoimento na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "depoimento">Depoimento a ser transformado.</param>
        /// <returns>A entidade transformada para DepoimentosItem.</returns>
        private static DepoimentosItem DTOToModel(DTODepoimento depoimento)
        {
            var depoimentoItem = new DepoimentosItem
                                 {
                                     Id = depoimento.ID,
                                     Title = depoimento.DadosDoAutor,
                                     Depoimento = depoimento.Depoimento,
                                     Ramo = depoimento.Ramo,
                                     ProdutoServico = depoimento.ProdutoServico,
                                     Estado = depoimento.Estado
                                 };

            return depoimentoItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo DepoimentosItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "depoimentoItem">Depoimento a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTODepoimento.</returns>
        private static DTODepoimento ModelToDTO(DepoimentosItem depoimentoItem)
        {
            var depoimento = new DTODepoimento
                             {
                                 ID = depoimentoItem.Id,
                                 DadosDoAutor = depoimentoItem.Title,
                                 Depoimento = depoimentoItem.Depoimento,
                                 Ramo = depoimentoItem.Ramo,
                                 ProdutoServico = depoimentoItem.ProdutoServico,
                                 Estado = depoimentoItem.Estado
                             };

            return depoimento;
        }
        #endregion
    }
}