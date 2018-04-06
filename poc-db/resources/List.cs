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
    /// retorna o nó List do xml.
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/sharepoint/", IsNullable = false)]
    public partial class List
    {
        /// <summary>
        /// retorna a classe metaDataField
        /// </summary>
        private ListMetaData metaDataField;
        /// <summary>
        /// retorna o campo titleField
        /// </summary>
        private string titleField;
        /// <summary>
        /// retorna o campo folderCreationField
        /// </summary>
        private string folderCreationField;
        /// <summary>
        /// retorna o campo directionField
        /// </summary>
        private string directionField;
        /// <summary>
        /// retorna o campo urlField
        /// </summary>
        private string urlField;
        /// <summary>
        /// retorna o campo baseTypeField
        /// </summary>
        private byte baseTypeField;
        /// <summary>
        /// retorna o campo enableContentTypesField
        /// </summary>
        private string enableContentTypesField;

        /// <remarks/>
        public ListMetaData MetaData
        {
            get
            {
                return this.metaDataField;
            }
            set
            {
                this.metaDataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FolderCreation
        {
            get
            {
                return this.folderCreationField;
            }
            set
            {
                this.folderCreationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Direction
        {
            get
            {
                return this.directionField;
            }
            set
            {
                this.directionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte BaseType
        {
            get
            {
                return this.baseTypeField;
            }
            set
            {
                this.baseTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string EnableContentTypes
        {
            get
            {
                return this.enableContentTypesField;
            }
            set
            {
                this.enableContentTypesField = value;
            }
        }
    }
}
