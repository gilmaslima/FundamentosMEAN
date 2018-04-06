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
    /// retorna o nó FieldRef do segmento List>MetaData>View do xml.
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListMetaDataViewFieldRef
    {
        /// <summary>
        /// retorna o nameField
        /// </summary>
        private string nameField;

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
    }
}
