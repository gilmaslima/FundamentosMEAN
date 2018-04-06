using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Script.Serialization;
using System.ComponentModel;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    /// <summary>
    /// Classe para facilitar a impressão de elementos HTML
    /// </summary>
    [ParseChildren(true, "ChildControls")]
    public partial class Impressao : UserControlBase
    {
        /// <summary>
        /// JS Serializer
        /// </summary>
        /// <example>
        /// <%@ Register TagName="Impressao" TagPrefix="Redecard" Src="~/_controltemplates/Comum/Impressao.ascx" %>
        /// 
        /// <Redecard:Impressao ID="ucImpressao" runat="server" SeletorJS="#tblTerminais" ExibirTitulo="true" 
        ///     ExibirLogoRedecard="true" FecharPopup="false">
        ///     <link rel="stylesheet" type="text/css" href="/_layouts/Redecard.Comum/CSS_Adicional.css" />
        /// </Redecard:Impressao>
        /// 
        /// <asp:Button ID="btnTeste" runat="server" Text="Imprimir" 
        ///     OnClientClick="ucImpressao.imprimir('Relatório de Teste'); return false;" />
        /// </example>
        private static JavaScriptSerializer _jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get { return _jsSerializer ?? (_jsSerializer = new JavaScriptSerializer()); }
        }
        
        /// <summary>
        /// Exibe ou não o Logo da Redecard na impressão.
        /// Default: true
        /// </summary>
        public Boolean ExibirLogoRedecard { get; set; }

        /// <summary>
        /// Exibe ou não o Título do Relatório.
        /// Default: true
        /// </summary>
        public Boolean ExibirTitulo { get; set; }

        /// <summary>
        /// Selector JS JQuery dos controles que serão incluídos na impressão.
        /// </summary>
        public String SeletorJS { get; set; }

        /// <summary>
        /// Modo de impressão.
        /// </summary>
        public PrintMode Modo { get; set; }

        /// <summary>
        /// Fecha ou mantém a janela aberta após a impressão.
        /// </summary>
        public Boolean FecharPopup { get; set; }

        /// <summary>
        /// Enumerador do modo de funcionamento da impressão
        /// </summary>
        public enum PrintMode
        {            
            /// <summary>
            /// Popup
            /// </summary>
            Popup = 0,

            /// <summary>
            /// IFrame
            /// </summary>
            Iframe            
        }

        /// <summary>
        /// Coleção de elementos que será adicionada ao controle
        /// </summary>
        public ControlCollection ChildControls
        {
            get
            {
                return ChildPlaceHolder.Controls;
            }
        }

        /// <summary>
        /// Atribui o valor default das variáveis
        /// </summary>
        public Impressao()
        {
            this.ExibirLogoRedecard = true;
            this.ExibirTitulo = true;
            this.Modo = PrintMode.Popup;
            this.FecharPopup = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Grava parâmetros de configuração da impressão no hidden field
            var config = new { this.ExibirLogoRedecard, this.ExibirTitulo, this.SeletorJS, Modo = this.Modo.ToString().ToLower(), this.FecharPopup };
            hdnConfigImpressao.Value = JsSerializer.Serialize(config);           

            //registra variáveis JS do controle 
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "impressao_init_" + this.ClientID,
                    "try { " +
                    "   var " + this.ID + " = _Impressao.get('#" + this.ClientID + "'); " +
                    "   var " + this.ClientID + " = _Impressao.get('#" + this.ClientID + "'); " +
                    "} catch(e) { }", true);
        }
    }
}