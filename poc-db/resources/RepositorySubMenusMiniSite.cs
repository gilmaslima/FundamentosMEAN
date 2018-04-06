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
    ///   Responsável por qualquer ação com a lista "SubMenus dos Minis Sites".
    /// </summary>
    //public class RepositorySubMenusMiniSite : RepositoryItem, IRepository<DTOSubMenuMiniSite, SubMenuDoMiniSitesItem>
    //{
    //    #region Constructors
    //    /// <summary>
    //    ///   Inicializa o repositório da lista de "SubMenus dos Minis Sites" utilizando um novo DataContext.
    //    /// </summary>
    //    public RepositorySubMenusMiniSite() {}

    //    /// <summary>
    //    ///   Inicializa o repositório da lista de "SubMenus dos Minis Sites" utilizando um DataContext já criado.
    //    /// </summary>
    //    /// <param name = "dataContext">
    //    ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
    //    ///   Permite a utilização de LINQ to SharePoint.
    //    /// </param>
    //    public RepositorySubMenusMiniSite(GeneratedModelDataContext dataContext) : base(dataContext) { }
    //    #endregion

    //    #region Public Methods
    //    /// <summary>
    //    ///   Persiste o sub menu do mini site na lista, atualizando ou inserindo.
    //    /// </summary>
    //    /// <param name = "submenuminisite">SubMenu do MiniSite a ser persistido.</param>
    //    /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
    //    public bool Persist(DTOSubMenuMiniSite submenuminisite)
    //    {
    //        return !submenuminisite.ID.HasValue ? Add(DTOToModel(submenuminisite)) : Update(DTOToModel(submenuminisite));
    //    }

    //    /// <summary>
    //    ///   Deleta um sub menu do mini site da lista.
    //    /// </summary>
    //    /// <param name = "submenuminisite">SubMenu do MiniSite a ser deletado.</param>
    //    /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
    //    public bool Delete(DTOSubMenuMiniSite submenuminisite)
    //    {
    //        try
    //        {
    //            var submenuminisiteItem = DataContext.SubMenuDoMiniSites.Where(c => c.Id.Equals(submenuminisite.ID)).FirstOrDefault();

    //            //Caso não tenha encontrado o item a ser deletado.
    //            if (submenuminisiteItem == null) return false;

    //            DataContext.SubMenuDoMiniSites.DeleteOnSubmit(submenuminisiteItem);
    //            DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

    //            return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
    //        }
    //        catch (Exception exception)
    //        {
    //            Log.WriteEntry(exception, EventLogEntryType.Error);
    //            //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
    //            //chamou este método.
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    ///   Retorna todos os itens da lista "SubMenuDoMiniSites".
    //    /// </summary>
    //    /// <returns>Todos os itens da lista "SubMenuDoMiniSites".</returns>
    //    public List<DTOSubMenuMiniSite> GetAllItems()
    //    {
    //        try
    //        {
    //            var submenuminisites = new List<DTOSubMenuMiniSite>();
    //            DataContext.SubMenuDoMiniSites.ToList().ForEach(submenuminisite => submenuminisites.Add(ModelToDTO(submenuminisite)));
    //            return submenuminisites;
    //        }
    //        catch (Exception exception)
    //        {
    //            Log.WriteEntry(exception, EventLogEntryType.Error);
    //            //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
    //            //chamou este método.
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    ///   Retorna determinados itens da lista "SubMenuDoMiniSites" baseado em um filtro.
    //    /// </summary>
    //    /// <param name = "filter">Filtro a ser utilizado na busca.</param>
    //    /// <returns>Lista com os itens encontrados.</returns>
    //    public List<DTOSubMenuMiniSite> GetItems(Expression<Func<SubMenuDoMiniSitesItem, bool>> filter)
    //    {
    //        try
    //        {
    //            var submenuminisites = new List<DTOSubMenuMiniSite>();
    //            DataContext.SubMenuDoMiniSites.Where(filter).ToList().ForEach(submenuminisite => submenuminisites.Add(ModelToDTO(submenuminisite)));
    //            return submenuminisites;
    //        }
    //        catch (Exception exception)
    //        {
    //            Log.WriteEntry(exception, EventLogEntryType.Error);
    //            //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
    //            //chamou este método.
    //            throw;
    //        }
    //    }
    //    #endregion

    //    #region Private Methods
    //    /// <summary>
    //    ///   Adiciona um novo sub menu do mini site na lista.
    //    /// </summary>
    //    /// <param name = "submenuminisite">SubMenu do MiniSite a ser inserido.</param>
    //    /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
    //    private bool Add(SubMenuDoMiniSitesItem submenuminisite)
    //    {
    //        try
    //        {
    //            DataContext.SubMenuDoMiniSites.InsertOnSubmit(submenuminisite);
    //            DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

    //            return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
    //        }
    //        catch (Exception exception)
    //        {
    //            Log.WriteEntry(exception, EventLogEntryType.Error);
    //            //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
    //            //chamou este método.
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    ///   Atualiza um sub menu do mini site da lista.
    //    /// </summary>
    //    /// <param name = "submenuminisite">SubMenu do MiniSite a ser atualizado.</param>
    //    /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
    //    private bool Update(SubMenuDoMiniSitesItem submenuminisite)
    //    {
    //        try
    //        {
    //            var submenuminisiteItem = DataContext.SubMenuDoMiniSites.Where(c => c.Id.Equals(submenuminisite.Id)).FirstOrDefault();

    //            //Caso não tenha encontrado o item a ser atualizado
    //            if (submenuminisiteItem == null) return false;

    //            //Atualiza os campos
    //            submenuminisiteItem.Id = submenuminisite.Id;
    //            submenuminisiteItem.Title = submenuminisite.Title;
    //            submenuminisiteItem.Hiperlink = submenuminisite.Hiperlink;

    //            DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

    //            return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
    //        }
    //        catch (Exception exception)
    //        {
    //            Log.WriteEntry(exception, EventLogEntryType.Error);
    //            //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
    //            //chamou este método.
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    ///   Transforma um DTOSubMenuMiniSite na entidade do modelo correspondente.
    //    /// </summary>
    //    /// <param name = "submenuminisite">SubMenu do MiniSite a ser transformado.</param>
    //    /// <returns>A entidade transformada para SubMenuDoMiniSitesItem.</returns>
    //    private static SubMenuDoMiniSitesItem DTOToModel(DTOSubMenuMiniSite submenuminisite)
    //    {
    //        var submenuminisiteItem = new SubMenuDoMiniSitesItem
    //        {
    //            Id = submenuminisite.ID,
    //            Title = submenuminisite.Titulo,
    //            Hiperlink = submenuminisite.Hiperlink
    //        };

    //        return submenuminisiteItem;
    //    }

    //    /// <summary>
    //    ///   Transforma uma entidade do modelo SubMenuDoMiniSitesItem na entidade DTO correspondente.
    //    /// </summary>
    //    /// <param name = "submenuminisiteItem">SubMenu do MiniSite a ser transformado.</param>
    //    /// <returns>A entidade do modelo transformada para DTOSubMenuMiniSite.</returns>
    //    private static DTOSubMenuMiniSite ModelToDTO(SubMenuDoMiniSitesItem submenuminisiteItem)
    //    {
    //        var submenuminisite = new DTOSubMenuMiniSite
    //        {
    //            ID = submenuminisiteItem.Id,
    //            Titulo = submenuminisiteItem.Title,
    //            Hiperlink = submenuminisiteItem.Hiperlink
    //        };

    //        return submenuminisite;
    //    }
    //    #endregion
    //}
}
