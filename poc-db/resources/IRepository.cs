#region Used Namespaces
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository
{
    public interface IRepository<T, TK> : IDisposable where T : DTOItem where TK : Item
    {
        /// <summary>
        ///   Persiste um item na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "item">Item a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        bool Persist(T item);

        /// <summary>
        ///   Deleta um item da lista.
        /// </summary>
        /// <param name = "item">item a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        bool Delete(T item);

        /// <summary>
        ///   Retorna todos os itens da lista.
        /// </summary>
        /// <returns>Todos os itens da lista.</returns>
        List<T> GetAllItems();

        /// <summary>
        ///   Retorna determinados itens da lista baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        List<T> GetItems(Expression<Func<TK, bool>> filter);
    }
}