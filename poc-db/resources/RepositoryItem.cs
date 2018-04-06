#region Used Namespaces
using System;
using Microsoft.SharePoint;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository {
    /// <summary>
    ///   Repositório base para todos os repositórios de listas.
    /// </summary>
    public abstract class RepositoryItem : IDisposable {
        #region Properties
        /// <summary>
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </summary>
        protected GeneratedModelDataContext DataContext { get; private set; }
        #endregion

        #region Constructors
        ///// <summary>
        /////   Construtor que cria um novo DataContext.
        ///// </summary>
        //protected RepositoryItem()
        //{
        //    // O contexto é vinculado ao site pai porque todas as listas estão direcionadas no pai
        //    DataContext = new GeneratedModelDataContext(SPContext.Current.Site.Url);
        //}

        /// <summary>
        /// Construtor que cria um novo DataContext.
        /// </summary>
        protected RepositoryItem() {
            //Variação atual.
            var currentLanguage = SPContext.Current.Web.Language;
            //Encontra o RootWeb
            var siteurl = SPContext.Current.Site.RootWeb.Url;
            SPSecurity.RunWithElevatedPrivileges(delegate() {
                using (SPWeb adminSite = new SPSite(siteurl).OpenWeb()) {
                    //Para cada site de nível 2 (Root da variação)
                    //foreach (SPWeb spWeb in adminSite.Webs) {
                    //    //Verifico se é o root da web atual, pela linguagem.
                    //    if (!spWeb.Language.Equals(currentLanguage)) continue;
                    //    //Caso positivo, instancia o DataContext utilizando o root da variação.
                    //    DataContext = new GeneratedModelDataContext(spWeb.Url);
                    //}
                    if (currentLanguage == 1046) 
                        DataContext = new GeneratedModelDataContext(siteurl + "/pt-BR");
                    else if (currentLanguage == 1033) 
                        DataContext = new GeneratedModelDataContext(siteurl + "/en-US");
                    else 
                        DataContext = new GeneratedModelDataContext(siteurl + "/es-ES");
                }
            });
        }


        /// <summary>
        ///   Construtor que utiliza um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        protected RepositoryItem(GeneratedModelDataContext dataContext) {
            DataContext = dataContext;
        }
        #endregion

        #region Methods

        #region Dispose
        /// <summary>
        ///   Libera os recursos utilizados.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Libera o Data Context.
        ///   Método virtual para que as classes filhas implementem o dispose em caso
        ///   de recursos que necessitem liberação.
        /// </summary>
        /// <param name = "disposing">Indica se está em processo de liberação.</param>
        protected virtual void Dispose(bool disposing) {
            if (!disposing) return;

            if (DataContext == null) return;

            DataContext.Dispose();
            DataContext = null;
        }
        #endregion

        #endregion
    }
}