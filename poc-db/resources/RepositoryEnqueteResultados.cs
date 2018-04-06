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
    ///   Responsável por qualquer ação com a lista "Enquetes - Resultados".
    /// </summary>
    //public class RepositoryEnqueteResultados : RepositoryItem, IRepository<DTOEnqueteResultado, EnquetesResultadosItem>
    //{
    //    #region Constructors
    //    /// <summary>
    //    ///   Inicializa o repositório da lista de "Enquetes - Resultados" utilizando um novo DataContext.
    //    /// </summary>
    //    public RepositoryEnqueteResultados() {}

    //    /// <summary>
    //    ///   Inicializa o repositório da lista de "Enquetes - Resultados" utilizando um DataContext já criado.
    //    /// </summary>
    //    /// <param name = "dataContext">
    //    ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
    //    ///   Permite a utilização de LINQ to SharePoint.
    //    /// </param>
    //    public RepositoryEnqueteResultados(GeneratedModelDataContext dataContext) : base(dataContext) { }
    //    #endregion

    //    #region Public Methods
    //    /// <summary>
    //    ///   Persiste a enquete resultado na lista, atualizando ou inserindo.
    //    /// </summary>
    //    /// <param name = "enqueteResultado">Enquete Resultado a ser persistido.</param>
    //    /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
    //    public bool Persist(DTOEnqueteResultado enqueteResultado)
    //    {
    //        return !enqueteResultado.ID.HasValue ? Add(DTOToModel(enqueteResultado)) : Update(DTOToModel(enqueteResultado));
    //    }

    //    /// <summary>
    //    ///   Deleta uma enquete resultado da lista.
    //    /// </summary>
    //    /// <param name = "enqueteResultado">Enquete Resultado a ser deletado.</param>
    //    /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
    //    public bool Delete(DTOEnqueteResultado enqueteResultado)
    //    {
    //        try
    //        {
    //            var enqueteResultadoItem = DataContext.EnquetesResultados.Where(c => c.Id.Equals(enqueteResultado.ID)).FirstOrDefault();

    //            //Caso não tenha encontrado o item a ser deletado.
    //            if (enqueteResultadoItem == null) return false;

    //            DataContext.EnquetesResultados.DeleteOnSubmit(enqueteResultadoItem);
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
    //    ///   Retorna todos os itens da lista "EnqueteResultados".
    //    /// </summary>
    //    /// <returns>Todos os itens da lista "EnqueteResultados".</returns>
    //    public List<DTOEnqueteResultado> GetAllItems()
    //    {
    //        try
    //        {
    //            var enqueteResultados = new List<DTOEnqueteResultado>();
    //            DataContext.EnquetesResultados.ToList().ForEach(enqueteResultado => enqueteResultados.Add(ModelToDTO(enqueteResultado)));
    //            return enqueteResultados;
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
    //    ///   Retorna determinados itens da lista "EnqueteResultados" baseado em um filtro.
    //    /// </summary>
    //    /// <param name = "filter">Filtro a ser utilizado na busca.</param>
    //    /// <returns>Lista com os itens encontrados.</returns>
    //    public List<DTOEnqueteResultado> GetItems(Expression<Func<EnquetesResultadosItem, bool>> filter)
    //    {
    //        try
    //        {
    //            var enqueteResultados = new List<DTOEnqueteResultado>();
    //            DataContext.EnquetesResultados.Where(filter).ToList().ForEach(enqueteResultado => enqueteResultados.Add(ModelToDTO(enqueteResultado)));
    //            return enqueteResultados;
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
    //    ///   Adiciona uma nova enquete resultado na lista.
    //    /// </summary>
    //    /// <param name = "enqueteResultado">Enquete Resultado a ser inserido.</param>
    //    /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
    //    private bool Add(EnquetesResultadosItem enqueteResultado)
    //    {
    //        try
    //        {
    //            DataContext.EnquetesResultados.InsertOnSubmit(enqueteResultado);
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
    //    ///   Atualiza uma enquete resultado da lista.
    //    /// </summary>
    //    /// <param name = "enqueteResultado">Enquete Resultado a ser atualizado.</param>
    //    /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
    //    private bool Update(EnquetesResultadosItem enqueteResultado)
    //    {
    //        try
    //        {
    //            var enqueteResultadoItem = DataContext.EnquetesResultados.Where(c => c.Id.Equals(enqueteResultado.Id)).FirstOrDefault();

    //            //Caso não tenha encontrado o item a ser atualizado
    //            if (enqueteResultadoItem == null) return false;

    //            //Atualiza os campos
    //            enqueteResultadoItem.Id = enqueteResultado.Id;
    //            enqueteResultadoItem.Title = enqueteResultado.Title;

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
    //    ///   Transforma um DTOEnqueteResultado na entidade do modelo correspondente.
    //    /// </summary>
    //    /// <param name = "enqueteResultado">Enquete Resultado a ser transformado.</param>
    //    /// <returns>A entidade transformada para EnqueteResultadosItem.</returns>
    //    private static EnquetesResultadosItem DTOToModel(DTOEnqueteResultado enqueteResultado)
    //    {
    //        var enqueteResultadoItem = new EnquetesResultadosItem
    //        {
    //            Id = enqueteResultado.ID,
    //            Title = enqueteResultado.Titulo
    //        };

    //        return enqueteResultadoItem;
    //    }

    //    /// <summary>
    //    ///   Transforma uma entidade do modelo EnqueteResultadosItem na entidade DTO correspondente.
    //    /// </summary>
    //    /// <param name = "enqueteResultadoItem">Enquete Resultado a ser transformado.</param>
    //    /// <returns>A entidade do modelo transformada para DTOEnqueteResultado.</returns>
    //    private static DTOEnqueteResultado ModelToDTO(EnquetesResultadosItem enqueteResultadoItem)
    //    {
    //        var enqueteResultado = new DTOEnqueteResultado
    //        {
    //            ID = enqueteResultadoItem.Id,
    //            Titulo = enqueteResultadoItem.Title
    //        };

    //        return enqueteResultado;
    //    }
    //    #endregion

    //}
}
