/*
© Copyright 2014 Rede S.A.
Autor : Evandro Coutinho
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.ListSchema
{
    /// <summary>
    /// retorna o nó ContentTypes do segmento List>MetaData do xml.
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListMetaDataContentTypes
    {
        /// <summary>
        /// retorna o array da classe contentTypeField.
        /// </summary>
        private ListMetaDataContentTypesContentType contentTypeField;

        /// <remarks/>
        public ListMetaDataContentTypesContentType ContentType
        {
            get
            {
                return this.contentTypeField;
            }
            set
            {
                this.contentTypeField = value;
            }
        }
    }
}
