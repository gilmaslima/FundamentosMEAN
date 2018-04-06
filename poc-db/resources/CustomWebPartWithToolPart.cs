using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace Redecard.Portal.Aberto.WebParts.CustomWebPartWithToolPart
{
    /// <summary>
    /// Autor: Cristiano Dias
    /// Data criação: 29/09/2010
    /// Descrição: classe de exemplo-base para criação de WebPart que contém customizações no momento de edição
    /// Representa a WebPart que contém ToolPart customizada
    /// URL de apoio: http://www.zimmergren.net/archive/2008/11/29/how-to-custom-web-part-properties-toolpart.aspx
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class CustomWebPartWithToolPart : Microsoft.SharePoint.WebPartPages.WebPart
    {
        private string someInformation;

        /// <summary>
        /// Informação qualquer que a ToolPart pode acessar para fins de leitura/escrita
        /// </summary>
        public string SomeInformation
        {
            get { return someInformation; }
            set { someInformation = value; }
        }

        /// <summary>
        /// Método disparado automaticamente para renderização do controles na WebPart
        /// </summary>
        protected override void CreateChildControls()
        {
            //Instanciação do controle
            TextBox txt = new TextBox();
            txt.ID = "txt";

            //Atribuição da Informação(vide Propriedade SomeInformation) no campo de exemplo
            txt.Text = this.SomeInformation;

            //Adição do controle à WebPArt
            this.Controls.Add(txt);

            base.CreateChildControls();
        }

        /// <summary>
        /// Método para carregamento dos contêineres da parte de edição da WebPart
        /// </summary>
        /// <returns></returns>
        public override ToolPart[] GetToolParts()
        {
            ToolPart[] toolParts = new ToolPart[3];

            toolParts[0] = new WebPartToolPart();
            toolParts[1] = new CustomPropertyToolPart();
            
            //Inclusão da ToolPart customizada
            toolParts[2] = new CustomToolPart();

            return toolParts;
        }
    }
}