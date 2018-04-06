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
    /// retorna o nó View do segmento List>MetaData do xml.
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListMetaDataView
    {
        /// <summary>
        /// retorna a classe toolbarField
        /// </summary>
        private ListMetaDataViewToolbar toolbarField;
        /// <summary>
        /// retorna a classe xslLinkField
        /// </summary>
        private ListMetaDataViewXslLink xslLinkField;
        /// <summary>
        /// retorna a classe jSLinkField
        /// </summary>
        private string jSLinkField;
        /// <summary>
        /// retorna a classe rowLimitField
        /// </summary>
        private ListMetaDataViewRowLimit rowLimitField;
        /// <summary>
        /// retorna o array  da classe viewFieldsField
        /// </summary>
        private ListMetaDataViewFieldRef[] viewFieldsField;
        /// <summary>
        /// retorna a classe queryField
        /// </summary>
        private ListMetaDataViewQuery queryField;
        /// <summary>
        /// retorna a classe parameterBindingsField
        /// </summary>
        private ListMetaDataViewParameterBinding[] parameterBindingsField;
        /// <summary>
        /// retorna o campo baseViewIDField
        /// </summary>
        private byte baseViewIDField;
        /// <summary>
        /// retorna o campo typeField
        /// </summary>
        private string typeField;
        /// <summary>
        /// retorna o campo mobileViewField
        /// </summary>
        private string mobileViewField;
        /// <summary>
        /// retorna o campo tabularViewField
        /// </summary>
        private string tabularViewField;
        /// <summary>
        /// retorna o campo webPartZoneIDField
        /// </summary>
        private string webPartZoneIDField;
        /// <summary>
        /// retorna o campo displayNameField
        /// </summary>
        private string displayNameField;
        /// <summary>
        /// retorna o campo defaultViewField
        /// </summary>
        private string defaultViewField;
        /// <summary>
        /// retorna o campo mobileDefaultViewField
        /// </summary>
        private string mobileDefaultViewField;
        /// <summary>
        /// retorna o campo setupPathField
        /// </summary>
        private string setupPathField;
        /// <summary>
        /// retorna o campo imageUrlField
        /// </summary>
        private string imageUrlField;
        /// <summary>
        /// retorna o campo urlField
        /// </summary>
        private string urlField;

        /// <remarks/>
        public ListMetaDataViewToolbar Toolbar
        {
            get
            {
                return this.toolbarField;
            }
            set
            {
                this.toolbarField = value;
            }
        }

        /// <remarks/>
        public ListMetaDataViewXslLink XslLink
        {
            get
            {
                return this.xslLinkField;
            }
            set
            {
                this.xslLinkField = value;
            }
        }

        /// <remarks/>
        public string JSLink
        {
            get
            {
                return this.jSLinkField;
            }
            set
            {
                this.jSLinkField = value;
            }
        }

        /// <remarks/>
        public ListMetaDataViewRowLimit RowLimit
        {
            get
            {
                return this.rowLimitField;
            }
            set
            {
                this.rowLimitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("FieldRef", IsNullable = false)]
        public ListMetaDataViewFieldRef[] ViewFields
        {
            get
            {
                return this.viewFieldsField;
            }
            set
            {
                this.viewFieldsField = value;
            }
        }

        /// <remarks/>
        public ListMetaDataViewQuery Query
        {
            get
            {
                return this.queryField;
            }
            set
            {
                this.queryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ParameterBinding", IsNullable = false)]
        public ListMetaDataViewParameterBinding[] ParameterBindings
        {
            get
            {
                return this.parameterBindingsField;
            }
            set
            {
                this.parameterBindingsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte BaseViewID
        {
            get
            {
                return this.baseViewIDField;
            }
            set
            {
                this.baseViewIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MobileView
        {
            get
            {
                return this.mobileViewField;
            }
            set
            {
                this.mobileViewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TabularView
        {
            get
            {
                return this.tabularViewField;
            }
            set
            {
                this.tabularViewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string WebPartZoneID
        {
            get
            {
                return this.webPartZoneIDField;
            }
            set
            {
                this.webPartZoneIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DefaultView
        {
            get
            {
                return this.defaultViewField;
            }
            set
            {
                this.defaultViewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MobileDefaultView
        {
            get
            {
                return this.mobileDefaultViewField;
            }
            set
            {
                this.mobileDefaultViewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SetupPath
        {
            get
            {
                return this.setupPathField;
            }
            set
            {
                this.setupPathField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ImageUrl
        {
            get
            {
                return this.imageUrlField;
            }
            set
            {
                this.imageUrlField = value;
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
    }
}
