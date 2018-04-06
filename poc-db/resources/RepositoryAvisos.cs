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
    ///   Responsável por qualquer ação com a lista "Avisos".
    /// </summary>
    public class RepositoryAvisos : RepositoryItem, IRepository<DTOAviso, AvisosItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Avisos" utilizando um novo DataContext.
        /// </summary>
        public RepositoryAvisos() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Avisos" utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryAvisos(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste o aviso na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "aviso">Aviso a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOAviso aviso)
        {
            return !aviso.ID.HasValue ? Add(DTOToModel(aviso)) : Update(DTOToModel(aviso));
        }

        /// <summary>
        ///   Deleta um aviso da lista.
        /// </summary>
        /// <param name = "aviso">Aviso a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOAviso aviso)
        {
            try
            {
                var avisoItem = DataContext.Avisos.Where(c => c.Id.Equals(aviso.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (avisoItem == null) return false;

                DataContext.Avisos.DeleteOnSubmit(avisoItem);
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
        ///   Retorna todos os itens da lista "Avisos".
        /// </summary>
        /// <returns>Todos os itens da lista "Avisos".</returns>
        public List<DTOAviso> GetAllItems()
        {
            try
            {
                var avisos = new List<DTOAviso>();
                DataContext.Avisos.ToList().ForEach(aviso => avisos.Add(ModelToDTO(aviso)));
                return avisos;
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
        ///   Retorna determinados itens da lista "Avisos" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOAviso> GetItems(Expression<Func<AvisosItem, bool>> filter)
        {
            try
            {
                var avisos = new List<DTOAviso>();
                DataContext.Avisos.Where(filter).ToList().ForEach(aviso => avisos.Add(ModelToDTO(aviso)));
                return avisos;
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
        ///   Adiciona um novo aviso na lista.
        /// </summary>
        /// <param name = "aviso">Aviso a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(AvisosItem aviso)
        {
            try
            {
                DataContext.Avisos.InsertOnSubmit(aviso);
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
        ///   Atualiza um aviso da lista.
        /// </summary>
        /// <param name = "aviso">Aviso a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(AvisosItem aviso)
        {
            try
            {
                var avisoItem = DataContext.Avisos.Where(c => c.Id.Equals(aviso.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (avisoItem == null) return false;

                //Atualiza os campos
                avisoItem.Id = aviso.Id;
                avisoItem.Title = aviso.Title;
                avisoItem.Descrição = aviso.Descrição;

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
        ///   Transforma um DTOAviso na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "aviso">Aviso a ser transformado.</param>
        /// <returns>A entidade transformada para AvisosItem.</returns>
        private static AvisosItem DTOToModel(DTOAviso aviso)
        {
            var avisoItem = new AvisosItem
            {
                Id = aviso.ID,
                Title = aviso.Titulo,
                Descrição = aviso.Descricao,
                Audiencia = aviso.Audiencia
            };

            return avisoItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo AvisosItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "avisoItem">Aviso a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTOAviso.</returns>
        private static DTOAviso ModelToDTO(AvisosItem avisoItem)
        {
            var aviso = new DTOAviso
            {
                ID = avisoItem.Id,
                Titulo = avisoItem.Title,
                Descricao = avisoItem.Descrição,
                Audiencia = avisoItem.Audiencia
            };

            return aviso;
        }
        #endregion
    }
}
