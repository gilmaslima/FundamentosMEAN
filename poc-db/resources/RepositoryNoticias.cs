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

namespace Redecard.Portal.Aberto.Model.Repository {
    /// <summary>
    ///   Responsável por qualquer ação com a lista "Notícias".
    /// </summary>
    public class RepositoryNoticias : RepositoryItem, IRepository<DTONoticia, NotíciasItem> {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Notícias' utilizando um novo DataContext.
        /// </summary>
        public RepositoryNoticias() { }

        /// <summary>
        ///   Inicializa o repositório da lista de "Notícias' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryNoticias(GeneratedModelDataContext dataContext) : base(dataContext) { }
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste a notícia na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "noticia">Notícia a ser persistida.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTONoticia noticia) {
            return !noticia.ID.HasValue ? Add(DTOToModel(noticia)) : Update(DTOToModel(noticia));
        }

        /// <summary>
        ///   Deleta uma notícia da lista.
        /// </summary>
        /// <param name = "noticia">Notícia a ser deletada.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTONoticia noticia) {
            try {
                var noticiaItem = DataContext.Notícias.Where(n => n.Id.Equals(noticia.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (noticiaItem == null) return false;

                DataContext.Notícias.DeleteOnSubmit(noticiaItem);
                DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

                return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
            }
            catch (Exception exception) {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Retorna todos os itens da lista "Notícias".
        /// </summary>
        /// <returns>Todos os itens da lista "Notícias".</returns>
        public List<DTONoticia> GetAllItems() {
            try {
                var noticias = new List<DTONoticia>();
                DataContext.Notícias.ToList().ForEach(noticia => noticias.Add(ModelToDTO(noticia)));
                return noticias;
            }
            catch (Exception exception) {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Retorna determinados itens da lista "Notícias" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTONoticia> GetItems(Expression<Func<NotíciasItem, bool>> filter) {
            try {
                var noticias = new List<DTONoticia>();
                DataContext.Notícias.Where(filter).ToList().ForEach(noticia => noticias.Add(ModelToDTO(noticia)));
                return noticias;
            }
            catch (Exception exception) {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///   Adiciona uma nova notícia na lista.
        /// </summary>
        /// <param name = "noticia">Notícia a ser inserida.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(NotíciasItem noticia) {
            try {
                DataContext.Notícias.InsertOnSubmit(noticia);
                DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

                return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
            }
            catch (Exception exception) {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Atualiza uma notícia da lista.
        /// </summary>
        /// <param name = "noticia">Notícia a ser atualizada.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(NotíciasItem noticia) {
            try {
                var noticiaItem = DataContext.Notícias.Where(n => n.Id.Equals(noticia.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (noticiaItem == null) return false;

                //Atualiza os campos
                noticiaItem.Id = noticia.Id;
                noticiaItem.Title = noticia.Title;
                noticiaItem.Descrição = noticia.Descrição;
                noticiaItem.Data = noticia.Data;
                noticiaItem.DataDeExpiração = noticia.DataDeExpiração;
                noticiaItem.Hiperlink = noticia.Hiperlink;

                DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

                return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
            }
            catch (Exception exception) {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Transforma uma DTONoticia na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "noticia">Notícia a ser transformada.</param>
        /// <returns>A entidade transformada para NotíciasItem.</returns>
        private static NotíciasItem DTOToModel(DTONoticia noticia) {
            var noticiaItem = new NotíciasItem {
                Id = noticia.ID,
                Title = noticia.Titulo,
                Descrição = noticia.Descricao,
                Data = noticia.Data,
                DataDeExpiração = noticia.DataDeExpiracao,
                Hiperlink = noticia.Hiperlink
            };

            return noticiaItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo NotíciasItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "noticiaItem">Notícia a ser transformada.</param>
        /// <returns>A entidade do modelo transformada para DTONoticia.</returns>
        private static DTONoticia ModelToDTO(NotíciasItem noticiaItem) {
            string sUrl = string.Empty;
            // verificar campo de URL, no formato padrão, o sharepoint retorna o valor no seguinte formato:
            // [URL], [Descrição]
            if (!String.IsNullOrEmpty(noticiaItem.Hiperlink)) {
                SPFieldUrlValue urlValue = new SPFieldUrlValue(noticiaItem.Hiperlink);
                sUrl = urlValue.Url;
            }
            else
                sUrl = "#";

            var noticia = new DTONoticia {
                ID = noticiaItem.Id,
                Titulo = noticiaItem.Title,
                Descricao = noticiaItem.Descrição,
                Data = noticiaItem.Data,
                DataDeExpiracao = noticiaItem.DataDeExpiração,
                Hiperlink = sUrl
            };

            return noticia;
        }
        #endregion
    }
}