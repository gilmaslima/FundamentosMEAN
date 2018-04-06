using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace Rede.PN.AtendimentoDigital.NodeEdge
{
    /// <summary>
    /// Classe para integração entre NodeJS e .NET, utilizada durante o build do projeto angular.
    /// </summary>
    public class SPDocumentLibrary
    {
        /// <summary>
        /// Atualiza arquivos em uma biblioteca de documentos já existente.
        /// </summary>
        /// <param name="command">
        /// Recebe um objeto no formato:
        /// {
        ///     webSiteUrl: "URL do subsite que contém a biblioteca de documentos"
        ///     documentLibraryName: "nome da biblioteca de documentos",
        ///     files: [ "array de arquivos (caminho absoluto)" ],
        ///     basePath: "caminho raiz dos arquivos, para identificação do caminho relativo do arquivo dentro da biblioteca"
        /// }</param>
        /// <returns></returns>
        public async Task<Object> AtualizarArquivos(dynamic command)
        {
            try
            {
                //Leitura dos parâmetros de entrada
                String webSiteUrl = command.webSiteUrl;
                String documentLibraryName = command.documentLibraryName;
                Object[] files = command.files;
                String basePath = command.basePath;

                //Validações básicas
                if (String.IsNullOrWhiteSpace(webSiteUrl))
                    return Task.FromResult<Object>("Parâmetro 'webSiteUrl' não informado. "
                        + "Deve conter a URL do website que contém a biblioteca de documentos.");
                if (String.IsNullOrWhiteSpace(documentLibraryName))
                    return Task.FromResult<Object>("Parâmetro 'documentLibraryName' não informado. "
                        + "Deve conter o nome da biblioteca de documentos.");
                if (files == null || files.Length == 0)
                    return Task.FromResult<Object>("Parâmetro 'files' não informado ou vazio. "
                        + "Deve conter uma lista de arquivos (absolute paths) que serão atualizados na biblioteca.");
                if (String.IsNullOrWhiteSpace(basePath))
                    return Task.FromResult<Object>("Parâmetro 'basePath' não informado."
                        + "Deve conter o caminho base dos arquivos, para geração do caminho relativo dos arquivos na biblioteca.");

                using (SPSite site = new SPSite(webSiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList lib = web.Lists.TryGetList(documentLibraryName);

                        foreach (String file in files)
                        {
                            if (System.IO.File.Exists(file))
                            {
                                String fileName = Path.GetFileName(file);
                                String relativeFileName = file.Replace(basePath, "").Trim('\\');
                                String relativeFolder = Path.GetDirectoryName(relativeFileName).Replace('\\', '/');

                                CriarEstruturaPastas(lib, lib.RootFolder, relativeFolder);
                                lib.RootFolder.Files.Add(relativeFileName, System.IO.File.ReadAllBytes(file), true);
                            }
                        }
                        lib.Update();
                    }
                }
                return Task.FromResult<Object>(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult<Object>(ex.Message);
            }
        }

        /// <summary>
        /// Cria a estrutura interna de pasta em uma lista
        /// </summary>
        /// <param name="lista">Biblioteca de documentos</param>
        /// <param name="pastaRaiz">Pasta raiz</param>
        /// <param name="urlPasta">Caminho da pasta na biblioteca</param>
        /// <returns>Pasta criada</returns>
        private static SPFolder CriarEstruturaPastas(SPList lista, SPFolder pastaRaiz, String urlPasta)
        {
            String[] nomePastas = urlPasta.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            String nomePasta = nomePastas[0];

            //Obtém a pasta
            SPFolder pastaAtual = pastaRaiz.SubFolders
                .Cast<SPFolder>()
                .FirstOrDefault(f => System.String.Compare(f.Name, nomePasta, StringComparison.OrdinalIgnoreCase) == 0);

            //Se a pasta não existe, cria
            if (pastaAtual == null)
            {
                SPListItem folderItem = lista.Items.Add(
                    pastaRaiz.ServerRelativeUrl, SPFileSystemObjectType.Folder, nomePasta);
                folderItem.SystemUpdate();
                pastaAtual = folderItem.Folder;
            }

            //Cria estrutura abaixo da pasta atual, recursivamente
            if (nomePastas.Length > 1)
            {
                String subFolderUrl = String.Join("/", nomePastas, 1, nomePastas.Length - 1);
                return CriarEstruturaPastas(lista, pastaAtual, subFolderUrl);
            }
            return pastaAtual;
        }
    }
}