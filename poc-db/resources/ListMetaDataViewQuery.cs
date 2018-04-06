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
    /// retorna o nó Query do segmento List>MetaData>View do xml.
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListMetaDataViewQuery
    {
        /// <summary>
        /// retorna a classe groupByField
        /// </summary>
        private ListMetaDataViewQueryGroupBy groupByField;
        /// <summary>
        /// retorna o array da classe orderByField
        /// </summary>
        private ListMetaDataViewQueryFieldRef[] orderByField;

        /// <remarks/>
        public ListMetaDataViewQueryGroupBy GroupBy
        {
            get
            {
                return this.groupByField;
            }
            set
            {
                this.groupByField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("FieldRef", IsNullable = false)]
        public ListMetaDataViewQueryFieldRef[] OrderBy
        {
            get
            {
                return this.orderByField;
            }
            set
            {
                this.orderByField = value;
            }
        }
    }
}
