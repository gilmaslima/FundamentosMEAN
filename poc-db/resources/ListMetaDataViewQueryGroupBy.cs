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
    /// retorna o nó GroupBy do segmento List>MetaData>View>Query do xml.
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListMetaDataViewQueryGroupBy
    {
        /// <summary>
        /// retorna a classe fieldRefField
        /// </summary>
        private ListMetaDataViewQueryGroupByFieldRef fieldRefField;

        /// <summary>
        /// retorna o campo collapseField.
        /// </summary>
        private string collapseField;

        /// <summary>
        /// retorna o campo groupLimitField
        /// </summary>
        private byte groupLimitField;

        /// <remarks/>
        public ListMetaDataViewQueryGroupByFieldRef FieldRef
        {
            get
            {
                return this.fieldRefField;
            }
            set
            {
                this.fieldRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Collapse
        {
            get
            {
                return this.collapseField;
            }
            set
            {
                this.collapseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte GroupLimit
        {
            get
            {
                return this.groupLimitField;
            }
            set
            {
                this.groupLimitField = value;
            }
        }
    }
}
