/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace Rede.PN.AtendimentoDigital.SharePoint.WebParts.AtendimentoDigital.HtmlContentLoader
{
    /// <summary>
    /// WebPart para carregamento dinâmico de conteúdo HTML de arquivos hospedados em bibliotecas
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class HtmlContentLoader : WebPart
    {
        /// <summary>
        /// Nome do arquivo
        /// </summary>
        [WebBrowsable(true),
        WebDisplayName("Caminho do arquivo"),
        WebDescription("Caminho do arquivo relativo ao WebSite informado, incluindo caminho da pasta. Ex: 'AtendimentoDigital/html/Atendimento.html'"),
        Personalizable(PersonalizationScope.Shared),
        Category("Arquivo HTML")]
        public String FilePath { get; set; }

        /// <summary>
        /// Url do website. Se não informado utiliza o website atual do contexto.
        /// </summary>
        [WebBrowsable(true),
        WebDisplayName("URL do WebSite"),
        WebDescription("URL relativa do subsite (ex: /sites/fechado) dentro do site collection atual. Se não informado, utiliza o website do contexto"),
        Personalizable(PersonalizationScope.Shared),
        Category("Arquivo HTML")]
        public String WebSite { get; set; }

        /// <summary>
        /// Encoding do arquivo.
        /// </summary>
        [WebBrowsable(true),
        WebDisplayName("File Encoding"),
        WebDescription("Encoding do arquivo. Valor default: UTF8"),
        Personalizable(PersonalizationScope.Shared),
        Category("Arquivo HTML")]
        public FileEncoding FileEncoding { get; set; }

        /// <summary>
        /// CreateChildControls
        /// </summary>
        protected override void CreateChildControls()
        {
            String fileContent = default(String);

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                try
                {                    
                    if (String.IsNullOrWhiteSpace(this.FilePath))
                    {
                        fileContent = String.Format(
                            "O preenchimento da propriedade 'FilePath' é obrigatório.<br/>" +
                            "Verificar configuração da propriedade 'FilePath' da WebPart.");
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(this.WebSite))
                        {
                            fileContent = GetFileContent(SPContext.Current.Web, this.FilePath, this.FileEncoding);
                        }
                        else
                        {
                            String webSite = this.WebSite;
                            if (!webSite.StartsWith("/"))
                                webSite = String.Concat("/", webSite);

                            using (SPWeb web = SPContext.Current.Site.OpenWeb(webSite))
                                fileContent = GetFileContent(web, this.FilePath, this.FileEncoding);
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    fileContent = String.Format(
                        "Exceção durante carregamento de conteúdo do arquivo.<br/>" +
                        "Website: {0}<br/>" +
                        "FilePath: {1}<br/>" +
                        "Exceção: {2}<br/>" +
                        "Stack Trace: {3}<br/>",
                        this.WebSite, this.FilePath, ex.Message, ex.StackTrace);
                }
                catch (Exception ex)
                {
                    fileContent = String.Format(
                       "Exceção durante carregamento de conteúdo do arquivo.<br/>" +
                       "Website: {0}<br/>" +
                       "FilePath: {1}<br/>" +
                       "Exceção: {2}<br/>" +
                       "Stack Trace: {3}<br/>",
                       this.WebSite, this.FilePath, ex.Message, ex.StackTrace);
                }
            });

            if (!String.IsNullOrWhiteSpace(fileContent))
                this.Controls.Add(new LiteralControl(fileContent));
        }

        /// <summary>
        /// Recupera o conteúdo do arquivo.
        /// </summary>
        /// <param name="web">Website</param>
        /// <param name="filePath">Caminho relativo do arquivo no subsite (incluindo nome de pasta/biblioteca)</param>
        /// <param name="fileEncoding">File encoding</param>
        /// <returns>Obtém conteúdo do arquivo</returns>
        private static String GetFileContent(SPWeb web, String filePath, FileEncoding fileEncoding)
        {
            String folderPath = Path.GetDirectoryName(filePath).TrimStart(new char[] { '\\' });
            String fileName = Path.GetFileName(filePath);

            if (String.IsNullOrWhiteSpace(folderPath))
                return String.Format(
                    "O valor '{0}' para a propriedade 'FilePath' não possui uma pasta válida",
                    filePath);

            if (String.IsNullOrWhiteSpace(fileName))
                return String.Format(
                    "O valor '{0}' para a propriedade 'FilePath' não possui um nome de arquivo válido",
                    filePath);

            SPFolder folder = web.GetFolder(folderPath);
            if(folder == null || !folder.Exists)
                return String.Format(
                    "Pasta '{0}' não encontrada no website {1}" +
                    "Verificar configuração da propriedade 'FilePath' da WebPart.",
                    folderPath, web.Url);

            SPFile file = folder.Files[fileName];
            if(file == null || !file.Exists)
                return String.Format(
                    "Arquivo '{0}' não encontrado na pasta '{1}' do website '{2}'.<br/>" +
                    "Verificar configuração da propriedade 'FilePath' da WebPart.",
                    fileName, folderPath, web.Url);

            //leitura do conteúdo do arquivo da biblioteca
            using (StreamReader reader = new StreamReader(file.OpenBinaryStream(), GetEncoding(fileEncoding)))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Encoding
        /// </summary>
        /// <returns>Encoding</returns>
        private static Encoding GetEncoding(FileEncoding fileEncoding)
        {
            switch (fileEncoding)
            {
                case FileEncoding.ASCII:
                    return Encoding.ASCII;
                case FileEncoding.BigEndianUnicode:
                    return Encoding.BigEndianUnicode;
                case FileEncoding.UT7:
                    return Encoding.UTF7;
                case FileEncoding.UT8:
                    return Encoding.UTF8;
                case FileEncoding.UTF32:
                    return Encoding.UTF32;
                case FileEncoding.Unicode:
                    return Encoding.Unicode;
                case FileEncoding.Default:
                default:
                    return Encoding.Default;
            }
        }
    }
}