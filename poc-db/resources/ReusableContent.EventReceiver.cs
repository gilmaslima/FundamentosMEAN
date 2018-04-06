using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;

namespace Redecard.Portal.Fechado.EventReceivers {
    /// <summary>
    ///   Esta classe adiciona itens na lista de conteúdos reutilizáveis, assim que a feature for ativada.
    /// </summary>
    /// <remarks>
    ///   The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>
    [Guid("27459dbd-8014-4545-8781-27f95b8b0449")]
    public class ReusableContentEventReceiver : SPFeatureReceiver {
        /// <summary>
        /// Insere itens na lista de conteúdos reutilizáveis.
        /// </summary>
        public override void FeatureActivated(SPFeatureReceiverProperties properties) {
            const string reusableContentListIdPropertyName = "_ReusableContentListId";
            object rootSite = properties.Feature.Parent;
            SPWeb rootWeb = null;
            if (rootSite is SPSite)
                rootWeb = ((SPSite)rootSite).OpenWeb();
            else if (rootSite is SPWeb)
                rootWeb = (SPWeb)rootSite;
            if (rootWeb.IsRootWeb) {
                if (rootWeb == null ||
                    rootWeb.AllProperties == null ||
                    !rootWeb.AllProperties.ContainsKey(reusableContentListIdPropertyName)) return;
                var reusableContentListId = (string)rootWeb.AllProperties[reusableContentListIdPropertyName];
                if (String.IsNullOrEmpty(reusableContentListId)) return;
                var reusableContentListGuid = new Guid(reusableContentListId);
                var reusableContentList = rootWeb.Lists[reusableContentListGuid];

                #region Conteúdo em Português
                /* Inserir contéudo em português */
                SPFolder o_ptBrFolder = CreateFolder(reusableContentList, "pt-BR");

                //<ul class="links">
                //    <li class="first"><a href="#">Perguntas Frequentes</a></li>
                //    <li class="last"><a href="#">Manuais de uso</a></li>
                //</ul>
                string _content = "<ul class=\"links\"><li class=\"first\"><a href=\"/sites/fechado/Paginas/perguntasfrequentes.aspx\">Perguntas Frequentes</a></li><li class=\"last\"><a href=\"/sites/fechado/Paginas/manuaisuso.aspx\">Manuais de uso</a></li></ul>";
                CreateItem(reusableContentList, "Perguntas Frequentes e Manuais de Uso", string.Empty, true, false, _content, o_ptBrFolder);

                //<div class="center">
                //    <p>© 2010 Redecard - Todos os direitos reservados</p>
                //    <ul class="links">
                //        <li><a href="#" title="Segurança e Privacidade">Segurança e Privacidade</a></li>
                //        <li><a href="#" title="Fale Conosco">Fale Conosco</a></li>
                //        <li class="last"><a href="#" title="Mapa do Site">Mapa do Site</a></li>
                //    </ul>
                //</div>
                _content = "<div class=\"center\"><p>© 2010 Redecard - Todos os direitos reservados</p><ul class=\"links\"><li><a href=\"/sites/fechado/Paginas/segurancaprivacidade.aspx\" title=\"Segurança e Privacidade\">Segurança e Privacidade</a></li><li><a href=\"/sites/fechado/Paginas/faleconosco.aspx\" title=\"Fale Conosco\">Fale Conosco</a></li><li class=\"last\"><a href=\"/sites/fechado/Paginas/mapasite.aspx\" title=\"Mapa do Site\">Mapa do Site</a></li></ul></div>";
                CreateItem(reusableContentList, "Redecard - Todos os direitos reservados", string.Empty, true, false, _content, o_ptBrFolder);
                #endregion

                #region Conteúdo em Inglês
                /* Inserir contéudo em português */
                SPFolder o_enUSFolder = CreateFolder(reusableContentList, "en-US");

                //<ul class="links">
                //    <li class="first"><a href="#">Perguntas Frequentes</a></li>
                //    <li class="last"><a href="#">Manuais de uso</a></li>
                //</ul>
                _content = "<ul class=\"links\"><li class=\"first\"><a href=\"/sites/fechado/Paginas/perguntasfrequentes.aspx\">Perguntas Frequentes</a></li><li class=\"last\"><a href=\"/sites/fechado/Paginas/manuaisuso.aspx\">Manuais de uso</a></li></ul>";
                CreateItem(reusableContentList, "Perguntas Frequentes e Manuais de Uso", string.Empty, true, false, _content, o_enUSFolder);

                //<div class="center">
                //    <p>© 2010 Redecard - Todos os direitos reservados</p>
                //    <ul class="links">
                //        <li><a href="#" title="Segurança e Privacidade">Segurança e Privacidade</a></li>
                //        <li><a href="#" title="Fale Conosco">Fale Conosco</a></li>
                //        <li class="last"><a href="#" title="Mapa do Site">Mapa do Site</a></li>
                //    </ul>
                //</div>
                _content = "<div class=\"center\"><p>© 2010 Redecard - Todos os direitos reservados</p><ul class=\"links\"><li><a href=\"/sites/fechado/Paginas/segurancaprivacidade.aspx\" title=\"Segurança e Privacidade\">Segurança e Privacidade</a></li><li><a href=\"/sites/fechado/Paginas/faleconosco.aspx\" title=\"Fale Conosco\">Fale Conosco</a></li><li class=\"last\"><a href=\"/sites/fechado/Paginas/mapasite.aspx\" title=\"Mapa do Site\">Mapa do Site</a></li></ul></div>";
                CreateItem(reusableContentList, "Redecard - Todos os direitos reservados", string.Empty, true, false, _content, o_enUSFolder);
                #endregion

                reusableContentList.Update();
            }
        }

        /// <summary>
        /// Cria as pastas de idiomas na lista de Conteúdos Reutilizáveis
        /// </summary>
        /// <param name="listName"></param>
        private static SPFolder CreateFolder(SPList reusableContentList, string folderName) {
            if (!reusableContentList.EnableFolderCreation) {
                reusableContentList.EnableFolderCreation = true;
                reusableContentList.Update();
            }
            // criar pasta de idioma
            string sNewListUrl = reusableContentList.ParentWeb.Url + "/" + reusableContentList.RootFolder.Url + "/" + folderName;
            SPListItem newFolder = reusableContentList.Items.Add("", SPFileSystemObjectType.Folder, folderName);
            newFolder.Update();
            return newFolder.Folder;
        }

        /// <summary>
        /// Cria um novo item na lista de conteúdos reutilizáveis.
        /// </summary>
        /// <param name="reusableContentList">SPList dos conteúdos reutilizáveis.</param>
        /// <param name="titulo">Título do Item.</param>
        /// <param name="comentarios">Comentários sobre o item.</param>
        /// <param name="atualizacaoAutomatica">Indica se o item será atualizado automaticamente durante seu uso.</param>
        /// <param name="menuSuspenso">Indica se o item aparecerá no menu suspenso.</param>
        /// <param name="htmlReutilizavel">Conteúdo em HTML do item reutilizável.</param>
        private static void CreateItem(SPList reusableContentList, string titulo, string comentarios, bool atualizacaoAutomatica,
                                       bool menuSuspenso, string htmlReutilizavel, SPFolder oFolder) {
            var item = reusableContentList.AddItem(oFolder.Url, SPFileSystemObjectType.File);
            //Título
            item[new Guid("fa564e0f-0c70-4ab9-b863-0177e6ddd247")] = titulo;
            //Comentários
            item[new Guid("9da97a8a-1da5-4a77-98d3-4bc10456e700")] = comentarios;
            //Atualização Automática
            item[new Guid("e977ed93-da24-4fcc-b77d-ac34eea7288f")] = atualizacaoAutomatica;
            //Mostrar no menu suspenso
            item[new Guid("32e03f99-6949-466a-a4a6-057c21d4b516")] = menuSuspenso;
            //HTML Reutilizável
            item[new Guid("82dd22bf-433e-4260-b26e-5b8360dd9105")] = htmlReutilizavel;
            item.Update();
        }
    }
}