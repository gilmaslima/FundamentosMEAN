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
    ///   Responsável por qualquer ação com a lista "Dicas de Uso Das Máquininhas".
    /// </summary>
    public class RepositoryDicasUsoMaquininha : RepositoryItem, IRepository<DTODicaUsoMaquininha, DicasDoUsoDaMaquininhaItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Dicas de Uso Das Máquininhas" utilizando um novo DataContext.
        /// </summary>
        public RepositoryDicasUsoMaquininha() { }

        /// <summary>
        ///   Inicializa o repositório da lista de "Dicas de Uso Das Máquininhas" utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryDicasUsoMaquininha(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste a dica de uso da maquininha na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "dicausomaquininha">Cartão a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTODicaUsoMaquininha dicausomaquininha)
        {
            return !dicausomaquininha.ID.HasValue ? Add(DTOToModel(dicausomaquininha)) : Update(DTOToModel(dicausomaquininha));
        }

        /// <summary>
        ///   Deleta uma dica de uso da maquininha da lista.
        /// </summary>
        /// <param name = "dicausomaquininha">Cartão a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTODicaUsoMaquininha dicausomaquininha)
        {
            try
            {
                var dicausomaquininhaItem = DataContext.DicasDoUsoDaMaquininha.Where(c => c.Id.Equals(dicausomaquininha.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (dicausomaquininhaItem == null) return false;

                DataContext.DicasDoUsoDaMaquininha.DeleteOnSubmit(dicausomaquininhaItem);
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
        ///   Retorna todos os itens da lista "DicasDoUsoDaMaquininha".
        /// </summary>
        /// <returns>Todos os itens da lista "DicasDoUsoDaMaquininha".</returns>
        public List<DTODicaUsoMaquininha> GetAllItems()
        {
            try
            {
                var dicasusomaquininha = new List<DTODicaUsoMaquininha>();
                DataContext.DicasDoUsoDaMaquininha.ToList().ForEach(dicausomaquininha => dicasusomaquininha.Add(ModelToDTO(dicausomaquininha)));
                return dicasusomaquininha;
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
        ///   Retorna determinados itens da lista "DicasDoUsoDaMaquininha" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTODicaUsoMaquininha> GetItems(Expression<Func<DicasDoUsoDaMaquininhaItem, bool>> filter)
        {
            try
            {
                var dicasusomaquininha = new List<DTODicaUsoMaquininha>();
                DataContext.DicasDoUsoDaMaquininha.Where(filter).ToList().ForEach(dicausomaquininha => dicasusomaquininha.Add(ModelToDTO(dicausomaquininha)));
                return dicasusomaquininha;
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
        ///   Adiciona uma nova dica de uso da maquininha na lista.
        /// </summary>
        /// <param name = "dicausomaquininha">Cartão a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(DicasDoUsoDaMaquininhaItem dicausomaquininha)
        {
            try
            {
                DataContext.DicasDoUsoDaMaquininha.InsertOnSubmit(dicausomaquininha);
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
        ///   Atualiza uma dica de uso da maquininha da lista.
        /// </summary>
        /// <param name = "dicausomaquininha">Cartão a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(DicasDoUsoDaMaquininhaItem dicausomaquininha)
        {
            try
            {
                var dicausomaquininhaItem = DataContext.DicasDoUsoDaMaquininha.Where(c => c.Id.Equals(dicausomaquininha.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (dicausomaquininhaItem == null) return false;

                //Atualiza os campos
                dicausomaquininhaItem.Id = dicausomaquininha.Id;
                dicausomaquininhaItem.Title = dicausomaquininha.Title;
                dicausomaquininhaItem.Hiperlink = dicausomaquininha.Hiperlink;

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
        ///   Transforma um DTODicaUsoMaquininha na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "dicausomaquininha">Cartão a ser transformado.</param>
        /// <returns>A entidade transformada para DicasDoUsoDaMaquininhaItem.</returns>
        private static DicasDoUsoDaMaquininhaItem DTOToModel(DTODicaUsoMaquininha dicausomaquininha)
        {
            var dicausomaquininhaItem = new DicasDoUsoDaMaquininhaItem
            {
                Id = dicausomaquininha.ID,
                Title = dicausomaquininha.Titulo,
                Hiperlink = dicausomaquininha.Hiperlink
            };

            return dicausomaquininhaItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo DicasDoUsoDaMaquininhaItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "dicausomaquininhaItem">Cartão a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTODicaUsoMaquininha.</returns>
        private static DTODicaUsoMaquininha ModelToDTO(DicasDoUsoDaMaquininhaItem dicausomaquininhaItem)
        {
            var dicausomaquininha = new DTODicaUsoMaquininha
            {
                ID = dicausomaquininhaItem.Id,
                Titulo = dicausomaquininhaItem.Title,
                Hiperlink = dicausomaquininhaItem.Hiperlink
            };

            return dicausomaquininha;
        }
        #endregion
    }
}
