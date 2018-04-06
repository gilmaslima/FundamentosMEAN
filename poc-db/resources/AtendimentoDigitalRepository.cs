/*
© Copyright 2016 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Constants;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// AtendimentoDigitalRepository
    /// </summary>
    public class AtendimentoDigitalRepository : BaseEntityRepository<AtendimentoDigital>, IAtendimentoDigitalRepository
    {
        /// <summary>
        /// Retorna o nome da biblioteca
        /// </summary>
        protected override String ListName
        {
            get { return Lists.AtendimentoDigital; }
        }

        /// <summary>
        /// Busca os arquivos de acordo com a extensão da pasta especificada
        /// </summary>
        /// <param name="web"></param>
        /// <param name="urlFolder">Ex: AtendimentoDigital/email/faleconosco</param>
        /// <param name="fileExtension">png</param>
        /// <returns>List<AtendimentoDigital></returns>
        public List<AtendimentoDigital> Get(SPWeb web, String urlFolder, String fileExtension)
        {
            SPQuery query = new SPQuery();
            query.ViewAttributes = "Scope=\"FilesOnly\"";
            query.Folder = web.GetFolder(urlFolder);

            query.Query = String.Format(@"<Where>                                                                                                            
                                            <Eq>
                                                <FieldRef Name='File_x0020_Type'/>
                                                <Value Type='Text'>{0}</Value>
                                            </Eq>
                                          </Where>", fileExtension);

            return GetListItems(query, web);
        }

        /// <summary>
        /// Busca o arquivo na biblioteca de acordo com o nome do aruqivo
        /// </summary>
        /// <param name="web"></param>
        /// <param name="urlFolder">Ex: AtendimentoDigital/email/faleconosco</param>
        /// <param name="fileName">template.html</param>
        /// <returns>List<AtendimentoDigital></returns>
        public AtendimentoDigital GetByFileName(SPWeb web, String urlFolder, String fileName)
        {
            SPQuery query = new SPQuery();
            query.ViewAttributes = "Scope=\"FilesOnly\"";
            query.Folder = web.GetFolder(urlFolder);

            query.Query = String.Format(@"<Where>                                                                                                            
                                            <Eq>
                                                <FieldRef Name='FileLeafRef'/>
                                                <Value Type='Text'>{0}</Value>
                                            </Eq>
                                          </Where>", fileName);

            return GetListItems(query, web).FirstOrDefault();
        }

        /// <summary>
        /// Busca o conteúdo de um arquivo de acordo com o nome do mesmo.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="urlFolder">Ex: AtendimentoDigital/email/faleconosco</param>
        /// <param name="fileName">Nome do arquivo. Ex.: template.html</param>
        /// <returns>List<String></returns>
        public String[] GetFileLines(SPWeb web, String urlFolder, String fileName)
        {
            AtendimentoDigital file = GetByFileName(web, urlFolder, fileName);

            if (file == null || file.File == null || !file.File.Exists)
                return new String[0];

            SPFile spFile = file.File;
            List<String> lines = new List<String>();
            using (var reader = new StreamReader(spFile.OpenBinaryStream(), Encoding.UTF8))
            {
                while (reader.Peek() >= 0)
                    lines.Add(reader.ReadLine());
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Busca o conteúdo de um arquivo de acordo com o nome do mesmo.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="urlFolder">Ex: AtendimentoDigital/email/faleconosco</param>
        /// <param name="fileName">Nome do arquivo. Ex.: template.html</param>
        /// <returns>List<AtendimentoDigital></returns>
        public String GetFileContent(SPWeb web, String urlFolder, String fileName)
        {
            AtendimentoDigital file = GetByFileName(web, urlFolder, fileName);

            if (file == null || file.File == null || !file.File.Exists)
                return default(String);

            SPFile spFile = file.File;
            String content = default(String);

            using (var reader = new StreamReader(spFile.OpenBinaryStream(), Encoding.UTF8))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }

        /// <summary>
        /// GatherParameters 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        protected override Dictionary<String, Object> GatherParameters(AtendimentoDigital entity, SPWeb web)
        {
            Dictionary<String, Object> fields = new Dictionary<String, Object>();
            fields.Add(Fields.Title, entity.Title);
            fields.Add(Fields.FileName, entity.File.Name);
            fields.Add(Fields.File, entity.File);
            fields.Add(Fields.ByteFile, entity.ReadyByteFile);

            return fields;
        }

        /// <summary>
        /// PopulateEntity 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override AtendimentoDigital PopulateEntity(SPListItem item)
        {
            AtendimentoDigital entity = new AtendimentoDigital();
            entity.Id = (Int32)item[Fields.Id];
            entity.Title = (String)item[Fields.Title];
            entity.FileName = (String)item.File.Name;
            entity.File = (SPFile)item.File;
            entity.ReadyByteFile = item.File.OpenBinary();

            return entity;
        }
    }
}
