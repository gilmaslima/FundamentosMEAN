#region Used Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Linq;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Microsoft.SharePoint;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository
{
    /// <summary>
    ///   Responsável por qualquer ação com a lista "Fotos".
    /// </summary>
    public class RepositoryFotos : RepositoryItem, IRepository<DTOFoto, FotosItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Fotos' utilizando um novo DataContext.
        /// </summary>
        public RepositoryFotos() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Fotos' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryFotos(GeneratedModelDataContext dataContext) : base(dataContext) {}
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste a foto na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "foto">Foto a ser persistida.</param>
        /// <returns>Boolean infotondo se houve algum problema durante a operação.</returns>
        public bool Persist(DTOFoto foto)
        {
            return !foto.ID.HasValue ? Add(DTOToModel(foto)) : Update(DTOToModel(foto));
        }

        /// <summary>
        ///   Deleta uma foto da lista.
        /// </summary>
        /// <param name = "foto">Foto a ser deletada.</param>
        /// <returns>Boolean infotondo se houve algum problema durante a operação.</returns>
        public bool Delete(DTOFoto foto)
        {
            try
            {
                var fotoItem = DataContext.Fotos.Where(n => n.Id.Equals(foto.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (fotoItem == null) return false;

                DataContext.Fotos.DeleteOnSubmit(fotoItem);
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
        ///   Retorna todos os itens da lista "Fotos".
        /// </summary>
        /// <returns>Todos os itens da lista "Fotos".</returns>
        public List<DTOFoto> GetAllItems()
        {
            try
            {
                var fotos = new List<DTOFoto>();
                DataContext.Fotos.ToList().ForEach(foto => fotos.Add(ModelToDTO(foto)));
                return fotos;
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
        ///   Retorna determinados itens da lista "Fotos" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOFoto> GetItems(Expression<Func<FotosItem, bool>> filter)
        {
            try
            {
                var fotos = new List<DTOFoto>();
                DataContext.Fotos.Where(filter).ToList().ForEach(foto => fotos.Add(ModelToDTO(foto)));
                return fotos;
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
        ///   Adiciona uma nova foto na lista.
        /// </summary>
        /// <param name = "foto">Foto a ser inserida.</param>
        /// <returns>Boolean infotondo se houve algum problema durante a operação.</returns>
        private bool Add(FotosItem foto)
        {
            try
            {
                DataContext.Fotos.InsertOnSubmit(foto);
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
        ///   Atualiza uma foto da lista.
        /// </summary>
        /// <param name = "foto">Foto a ser atualizada.</param>
        /// <returns>Boolean infotondo se houve algum problema durante a operação.</returns>
        private bool Update(FotosItem foto)
        {
            try
            {
                var fotoItem = DataContext.Fotos.Where(n => n.Id.Equals(foto.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (fotoItem == null) return false;

                //Atualiza os campos
                fotoItem.Id = foto.Id;
                fotoItem.Title = foto.Title;
                fotoItem.Url = foto.Url;
                fotoItem.Descrição = foto.Descrição;
                fotoItem.Data = foto.Data;
                fotoItem.Galeria = foto.Galeria;

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
        ///   Transforma uma DTOFoto na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "foto">Foto a ser transformada.</param>
        /// <returns>A entidade transformada para FotosItem.</returns>
        private static FotosItem DTOToModel(DTOFoto foto)
        {
            var fotoItem = new FotosItem
                           {
                               Id = foto.ID,
                               Title = foto.Titulo,
                               Url = foto.Url,
                               Descrição = foto.Descricao,
                               Data = foto.Data,
                               Galeria = foto.Galeria
                           };

            return fotoItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo FotosItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "fotoItem">Foto a ser transformada.</param>
        /// <returns>A entidade do modelo transformada para DTOFoto.</returns>
        private static DTOFoto ModelToDTO(FotosItem fotoItem)
        {
            string sUrl = string.Empty;
            // verificar campo de URL, no formato padrão, o sharepoint retorna o valor no seguinte formato:
            // [URL], [Descrição]
            if (!String.IsNullOrEmpty(fotoItem.Url)) {
                SPFieldUrlValue urlValue = new SPFieldUrlValue(fotoItem.Url);
                sUrl = urlValue.Url;
            }
            else
                sUrl = fotoItem.Url;

            var foto = new DTOFoto
                       {
                           ID = fotoItem.Id,
                           Titulo = fotoItem.Title,
                           Url = sUrl,
                           Descricao = fotoItem.Descrição,
                           Data = fotoItem.Data,
                           Galeria = fotoItem.Galeria
                       };

            return foto;
        }
        #endregion
    }
}