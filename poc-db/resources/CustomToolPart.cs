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
    /// Representa a área de propriedades customizadas.
    /// URL de apoio: http://www.zimmergren.net/archive/2008/11/29/how-to-custom-web-part-properties-toolpart.aspx
    /// </summary>
    public sealed class CustomToolPart : Microsoft.SharePoint.WebPartPages.ToolPart
    {
        private TextBox txtInfo; //referência aos controles do container

        /// <summary>
        /// Método disparado automaticamente para renderização do controles na área customizada
        /// </summary>
        protected override void CreateChildControls()
        {
            //Instanciação dos controles a adicionar no container
            this.txtInfo = new TextBox();
            this.txtInfo.ID = "txtInfo";
            this.txtInfo.Text = this.WebPart.SomeInformation;

            //Adição do controle no container
            this.Controls.Add(this.txtInfo);

            base.CreateChildControls();
        }

        /// <summary>
        /// Inicialização da ToolPart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            //Assinala o título no topo da seção customizada
            this.Title = "Título da seção customizada aqui";

            base.OnInit(e);
        }

        /// <summary>
        /// Obtém a referência ao controle WebPart que carrega este container ToolPart
        /// Útil para leitura/escrita de propriedades públicas da WebPart
        /// (Parent Container)
        /// </summary>
        private CustomWebPartWithToolPart WebPart
        {
            get
            {
                return this.ParentToolPane.SelectedWebPart as CustomWebPartWithToolPart;
            }
        }

        /// <summary>
        /// Método disparado pelos botões Aplicar e Ir que são padrões da área de edição da ToolPart
        /// </summary>
        public override void ApplyChanges()
        {
            //Persiste as informações na WebPart (parent) a partir dos controles da ToolPart
            this.WebPart.SomeInformation = this.txtInfo.Text;
        }
    }
}