/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.HomePage
{
    /// <summary>
    /// HomePage Segmentada - Box de Quadro de Atalhos
    /// </summary>
    public partial class Atalhos : BaseUserControl
    {
        #region [ Propriedades Públicas ]
        
        /// <summary>
        /// Getter/Setter para atributo "class" do controle
        /// </summary>
        [CssClassProperty]
        public String CssClass
        {
            get { return pnlControle.CssClass; }
            set { pnlControle.CssClass = value; }
        }

        /// <summary>
        /// Tipo da HomePage: EMP/IBBA ou Varejo
        /// </summary>
        public TipoHomePage Tipo { get; set; }

        /// <summary>
        /// String de Configuração dos Atalhos do Box
        /// </summary>
        public String ConfiguracaoAtalhos { get; set; }

        #endregion
        
        #region [ Eventos da Página ]

        /// <summary>
        /// Load da Página
        /// </summary>        
        protected void Page_Load(object sender, EventArgs e)
        {
            //Não precisa validar userControl
            ValidarPermissao = false;

            //Carrega os atalhos da Home, conforme string de configuração dos atalhos
            Boolean possuiAtalho = CarregarAtalhosHome(rptAtalhos, this.ConfiguracaoAtalhos);
            this.Visible = possuiAtalho;
        }

        #endregion
    }
}