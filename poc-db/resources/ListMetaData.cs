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
    /// retorna o nó MetaData do segmento List do xml.
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListMetaData
    {
        /// <summary>
        /// retorna a classe contentTypesField
        /// </summary>
        private ListMetaDataContentTypes contentTypesField;
        /// <summary>
        /// retorna o array da classe fieldsField
        /// </summary>
        private Field[] fieldsField;
        /// <summary>
        /// retorna o array da classe viewsField
        /// </summary>
        private ListMetaDataView[] viewsField;
        /// <summary>
        /// retorna o array da classe formsField
        /// </summary>
        private ListMetaDataForm[] formsField;

        /// <remarks/>
        public ListMetaDataContentTypes ContentTypes
        {
            get
            {
                return this.contentTypesField;
            }
            set
            {
                this.contentTypesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Field", IsNullable = false)]
        public Field[] Fields
        {
            get
            {
                return this.fieldsField;
            }
            set
            {
                this.fieldsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("View", IsNullable = false)]
        public ListMetaDataView[] Views
        {
            get
            {
                return this.viewsField;
            }
            set
            {
                this.viewsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Form", IsNullable = false)]
        public ListMetaDataForm[] Forms
        {
            get
            {
                return this.formsField;
            }
            set
            {
                this.formsField = value;
            }
        }
    }
}
