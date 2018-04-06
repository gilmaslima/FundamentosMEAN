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
    ///   Responsável por qualquer ação com a lista "Vídeos".
    /// </summary>
    public class RepositoryVídeos : RepositoryItem, IRepository<DTOVideo, VídeosItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Vídeos' utilizando um novo DataContext.
        /// </summary>
        public RepositoryVídeos() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Vídeos' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryVídeos(GeneratedModelDataContext dataContext) : base(dataContext) {}
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste o video na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "video">Vídeo a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOVideo video)
        {
            return !video.ID.HasValue ? Add(DTOToModel(video)) : Update(DTOToModel(video));
        }

        /// <summary>
        ///   Deleta um video da lista.
        /// </summary>
        /// <param name = "video">Vídeo a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOVideo video)
        {
            try
            {
                var videoItem = DataContext.Vídeos.Where(d => d.Id.Equals(video.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (videoItem == null) return false;

                DataContext.Vídeos.DeleteOnSubmit(videoItem);
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
        ///   Retorna todos os itens da lista "Vídeos".
        /// </summary>
        /// <returns>Todos os itens da lista "Vídeos".</returns>
        public List<DTOVideo> GetAllItems()
        {
            try
            {
                var videos = new List<DTOVideo>();
                DataContext.Vídeos.ToList().ForEach(video => videos.Add(ModelToDTO(video)));
                return videos;
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
        ///   Retorna determinados itens da lista "Vídeos" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOVideo> GetItems(Expression<Func<VídeosItem, bool>> filter)
        {
            try
            {
                var videos = new List<DTOVideo>();
                DataContext.Vídeos.Where(filter).ToList().ForEach(video => videos.Add(ModelToDTO(video)));
                return videos;
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
        ///   Adiciona um novo video na lista.
        /// </summary>
        /// <param name = "video">Vídeo a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(VídeosItem video)
        {
            try
            {
                DataContext.Vídeos.InsertOnSubmit(video);
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
        ///   Atualiza um video da lista.
        /// </summary>
        /// <param name = "video">Vídeo a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(VídeosItem video)
        {
            try
            {
                var videoItem = DataContext.Vídeos.Where(d => d.Id.Equals(video.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (videoItem == null) return false;

                //Atualiza os campos
                videoItem.Id = video.Id;
                videoItem.Title = video.Title;
                videoItem.Url = video.Url;
                videoItem.Descrição = video.Descrição;
                videoItem.DataDeCriação = video.DataDeCriação;
                videoItem.Autor = video.Autor;
                videoItem.NúmeroDeExibições = video.NúmeroDeExibições;

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
        ///   Transforma um DTOVideo na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "video">Vídeo a ser transformado.</param>
        /// <returns>A entidade transformada para VídeosItem.</returns>
        private static VídeosItem DTOToModel(DTOVideo video)
        {
            var videoItem = new VídeosItem
                            {
                                Id = video.ID,
                                Title = video.Titulo,
                                Url = video.Url,
                                Descrição = video.Descricao,
                                DataDeCriação = video.DataDeCriacao,
                                Autor = video.Autor,
                                NúmeroDeExibições = video.NumeroDeExibicoes
                            };

            return videoItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo VídeosItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "videoItem">Vídeo a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTOVideo.</returns>
        private static DTOVideo ModelToDTO(VídeosItem videoItem)
        {
            string urlVideo = string.Empty;

            // verificar campo de URL, no formato padrão, o sharepoint retorna o valor no seguinte formato:
            // [URL], [Descrição]
            if (!String.IsNullOrEmpty(videoItem.Url))
            {

                SPFieldUrlValue urlValue = new SPFieldUrlValue(videoItem.Url);

                urlVideo = urlValue.Url;
            }
            else
                urlVideo = videoItem.Url;

            var video = new DTOVideo
                        {
                            ID = videoItem.Id,
                            Titulo = videoItem.Title,
                            Url = urlVideo,
                            Descricao = videoItem.Descrição,
                            DataDeCriacao = videoItem.DataDeCriação,
                            Autor = videoItem.Autor,
                            NumeroDeExibicoes = videoItem.NúmeroDeExibições
                        };

            return video;
        }
        #endregion
    }
}