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
    ///   Responsável por qualquer ação com a lista "Downloads".
    /// </summary>
    public class RepositoryDownloads : RepositoryItem, IRepository<DTODownload, DownloadsItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Downloads' utilizando um novo DataContext.
        /// </summary>
        public RepositoryDownloads() { }

        /// <summary>
        ///   Inicializa o repositório da lista de "Downloads' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryDownloads(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste o download na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "download">Download a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTODownload download)
        {
            return !download.ID.HasValue ? Add(DTOToModel(download)) : Update(DTOToModel(download));
        }

        /// <summary>
        ///   Deleta um download da lista.
        /// </summary>
        /// <param name = "download">Download a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTODownload download)
        {
            try
            {
                var downloadItem = DataContext.Downloads.Where(d => d.Id.Equals(download.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (downloadItem == null) return false;

                DataContext.Downloads.DeleteOnSubmit(downloadItem);
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
        ///   Retorna todos os itens da lista "Downloads".
        /// </summary>
        /// <returns>Todos os itens da lista "Downloads".</returns>
        public List<DTODownload> GetAllItems()
        {
            try
            {
                var downloads = new List<DTODownload>();
                DataContext.Downloads.ToList().ForEach(download => downloads.Add(ModelToDTO(download)));
                return downloads;
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
        ///   Retorna determinados itens da lista "Downloads" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTODownload> GetItems(Expression<Func<DownloadsItem, bool>> filter)
        {
            try
            {
                var downloads = new List<DTODownload>();
                DataContext.Downloads.Where(filter).ToList().ForEach(download => downloads.Add(ModelToDTO(download)));
                return downloads;
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
        ///   Adiciona um novo download na lista.
        /// </summary>
        /// <param name = "download">Download a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(DownloadsItem download)
        {
            try
            {
                DataContext.Downloads.InsertOnSubmit(download);
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
        ///   Atualiza um download da lista.
        /// </summary>
        /// <param name = "download">Download a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(DownloadsItem download)
        {
            try
            {
                var downloadItem = DataContext.Downloads.Where(d => d.Id.Equals(download.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (downloadItem == null) return false;

                //Atualiza os campos
                downloadItem.Id = download.Id;
                downloadItem.Title = download.Title;
                downloadItem.Anexos = download.Anexos;
                downloadItem.Descrição = download.Descrição;
                downloadItem.Área = download.Área;
                downloadItem.Assunto = download.Assunto;
                downloadItem.NúmeroDeDownloads = download.NúmeroDeDownloads;
                downloadItem.TipoDeDownload = download.TipoDeDownload;

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
        ///   Transforma um DTODownload na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "download">Download a ser transformado.</param>
        /// <returns>A entidade transformada para DownloadsItem.</returns>
        private static DownloadsItem DTOToModel(DTODownload download)
        {
            var downloadItem = new DownloadsItem
            {
                Id = download.ID,
                Title = download.Titulo,
                Anexos = new string[] { download.Anexos },
                Descrição = download.Descricao,
                Área = download.Area,
                Assunto = download.Assunto,
                NúmeroDeDownloads = download.NumeroDeDownloads,
                TipoDeDownload = download.TipoDeDownload
            };

            return downloadItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo DownloadsItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "downloadItem">Download a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTODownload.</returns>
        private static DTODownload ModelToDTO(DownloadsItem downloadItem)
        {
            var download = new DTODownload
            {
                ID = downloadItem.Id,
                Titulo = downloadItem.Title,
                Anexos = !object.ReferenceEquals(downloadItem.Anexos, null) && !object.ReferenceEquals(downloadItem.Anexos.FirstOrDefault(),null) ? downloadItem.Anexos.FirstOrDefault() : string.Empty,
                Descricao = downloadItem.Descrição,
                Area = downloadItem.Área,
                Assunto = downloadItem.Assunto,
                NumeroDeDownloads = downloadItem.NúmeroDeDownloads,
                TipoDeDownload = downloadItem.TipoDeDownload
            };

            return download;
        }
        #endregion
    }
}