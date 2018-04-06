using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Redecard.Portal.Helper.Conversores;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper;

namespace Redecard.Portal.Aberto.WebParts.RedecardPromocoes
{
    /// <summary>
    /// ToolPart customizada para WebPart de Conheça as Promoções
    /// </summary>
    public class GerenciamentoPromocoesToolPart : Microsoft.SharePoint.WebPartPages.ToolPart
    {
        #region Variaveis

        private static string mensagemInstrucao = "Informe os itens de promoção no campo abaixo. Para cada item, informe o perfil e o link correlacionado no seguinte formato: <strong>perfil;link</strong>. Para incluir mais intens pressione ENTER e digite cada promoção em um linha. Para encerrar o gerenciamento dos itens, pressione o botão Ir.";

        //Controles da ToolPart
        private Label lblPromocoes;
        private TextBox txtPromocoes;
        private LiteralControl ltlQuebra;

        #endregion

        #region Propriedades 

        /// <summary>
        /// Obtém a referência ao controle WebPart que carrega este container ToolPart
        /// Útil para leitura/escrita de propriedades públicas da WebPart
        /// (Parent Container)
        /// </summary>
        private RedecardPromocoes WebPart
        {
            get
            {
                return this.ParentToolPane.SelectedWebPart as RedecardPromocoes;
            }
        }

        #endregion

        #region Construtor

        //Método Construtor
        public GerenciamentoPromocoesToolPart()
        { }

        #endregion

        #region Eventos

        /// <summary>
        /// Inicialização da ToolPart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            //Assinala o título no topo da seção customizada           
            this.Title = "Gerenciar itens da lista de Promoções";                        

            base.OnInit(e);
        }

        /// <summary>
        /// Método disparado pelos botões Aplicar e Ir que são padrões da área de edição da ToolPart
        /// </summary>
        public override void ApplyChanges()
        {
            this.WebPart.Promocao = this.txtPromocoes.Text.Trim();            
        }

        /// <summary>
        /// Método disparado automaticamente para renderização do controles na área customizada
        /// </summary>
        protected override void CreateChildControls()
        {
            this.lblPromocoes = new Label();
            this.lblPromocoes.Text = mensagemInstrucao;

            this.ltlQuebra = new LiteralControl();
            this.ltlQuebra.Text = "<br />";

            this.txtPromocoes = new TextBox();
            this.txtPromocoes.ID = "txtPromocoes";            
            this.txtPromocoes.TextMode = TextBoxMode.MultiLine;
            this.txtPromocoes.Width = Unit.Pixel(300);
            this.txtPromocoes.Height = Unit.Pixel(400);
            this.txtPromocoes.Rows = 10;

            this.txtPromocoes.Text = this.WebPart.Promocao;

            this.Controls.Add(lblPromocoes);
            this.Controls.Add(ltlQuebra);
            this.Controls.Add(txtPromocoes);      

            base.CreateChildControls();
        }

        #endregion
    }
}


