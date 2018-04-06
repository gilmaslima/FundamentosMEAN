/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web.UI;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.HomePage
{    
    /// <summary>
    /// HomePage Segmentada Varejo - Box de Antecipação de Vendas
    /// </summary>
    public partial class VarejoAntecipacao : BaseUserControl
    {
        #region [ Controles ]

        /// <summary>
        /// ucConteudoAviso control.
        /// </summary>
        public ConteudoEditavel UcConteudoAviso { get { return (ConteudoEditavel)ucConteudoAviso; } }

        /// <summary>
        /// ucConteudoRav control.
        /// </summary>
        public ConteudoEditavel UcConteudoRav { get { return (ConteudoEditavel)ucConteudoRav; } }

        #endregion

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
        
        #endregion

        /// <summary>
        /// Verifica se usuário possui permissão para Acesso ao RAV
        /// </summary>
        private Boolean AcessoRAV
        {
            get
            {
                //Código do Serviço de RAV
                //Int32 codigoServico = 10015;
                //Boolean possuiAcesso = base.ValidarServico(codigoServico);
                //return possuiAcesso;
                return Sessao.Contem() && 
                    this.ValidarPagina("/sites/fechado/servicos/Paginas/pn_rav.aspx");
            }
        }

        /// <summary>
        /// Indica se o quadro de RAV deve ser exibido.<br/>
        /// Condição de exibição: modo de edição ou possui RAV.
        /// </summary>
        private Boolean ExibirQuadroRav
        {
            get
            {
                return UcConteudoRav.IsInEditMode || this.AcessoRAV;
            }
        }

        /// <summary>
        /// Indica se o quadro de aviso deve ser exibido.<br/>
        /// Condição de exibição: modo de edição ou possui conteúdo
        /// </summary>
        private Boolean ExibirQuadroAviso
        {
            get
            {
                String conteudo = UcConteudoAviso.Conteudo ?? String.Empty;
                return UcConteudoAviso.IsInEditMode || !String.IsNullOrEmpty(conteudo.Trim());
            }
        }

        #region [ Eventos da Página ]

        /// <summary>
        /// Load da Página
        /// </summary>        
        protected void Page_Load(Object sender, EventArgs e)
        {
            //Não precisa validar userControl
            ValidarPermissao = false;

            //Só exibe controle se possui Acesso RAV ou Quadro de Comunicação
            this.Visible = this.ExibirQuadroRav || this.ExibirQuadroAviso;
            
            //Só exibe frame RAV se possui permissão
            pnlRav.Visible = this.ExibirQuadroRav;

            //Só exibe frame de Quadro Aviso se existir aviso
            pnlAviso.Visible = this.ExibirQuadroAviso;
        }

        #endregion
    }
}