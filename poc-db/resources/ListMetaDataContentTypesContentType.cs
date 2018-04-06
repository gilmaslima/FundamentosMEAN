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
    /// retorna o nó ContentType do segmento List>MetaData>ContentTypes do xml.
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListMetaDataContentTypesContentType
    {
        /// <summary>
        /// retorna o array da classe fieldRefsField.
        /// </summary>
        private ListMetaDataContentTypesContentTypeFieldRef[] fieldRefsField;
        /// <summary>
        /// retorna o campo idField.
        /// </summary>
        private string idField;
        /// <summary>
        /// retorna o campo nameField.
        /// </summary>
        private string nameField;
        /// <summary>
        /// retorna o campo groupField.
        /// </summary>
        private string groupField;
        /// <summary>
        /// retorna o campo descriptionField.
        /// </summary>
        private string descriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("FieldRef", IsNullable = false)]
        public ListMetaDataContentTypesContentTypeFieldRef[] FieldRefs
        {
            get
            {
                return this.fieldRefsField;
            }
            set
            {
                this.fieldRefsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Group
        {
            get
            {
                return this.groupField;
            }
            set
            {
                this.groupField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }
}
