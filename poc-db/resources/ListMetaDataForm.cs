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
    /// Classe responsável por guardar as informações do atributo Forms do Schema.xml.
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListMetaDataForm
    {
        /// <summary>
        /// retorna o tipo de form do xml (DisplayForm, EditForm, NewForm).
        /// </summary>
        private string typeField;
        /// <summary>
        /// retorna a url de form do xml.
        /// </summary>
        private string urlField;
        /// <summary>
        /// retorna a setup de form do xml.
        /// </summary>
        private string setupPathField;
        /// <summary>
        /// retorna a zona de webpart de form do xml.
        /// </summary>
        private string webPartZoneIDField;

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
    }
}
