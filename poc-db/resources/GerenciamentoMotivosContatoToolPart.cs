using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Redecard.Portal.Helper.Conversores;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper.Validacao;

namespace Redecard.Portal.Aberto.WebParts.FormularioContato
{
    public class GerenciamentoMotivosContatoToolPart : ToolPart
    {
        private static string mensagemInstrucao = "Informe os itens de motivo de contato no campo abaixo. Para cada item, informe a descrição do motivo e e-mail correlacionado no seguinte formato: motivo;contato e pressione ENTER. Em seguida, pressione ENTER. Para encerrar o gerenciamento dos itens, pressione o botão \"Ir\"";
        private static string cabecalhoInstrucaoCorrecao = "Por favor, corrija o(s) seguinte(s) erro(s):";
        private IValidacao<string> validador; //Referencia ao objeto de validação dos motivos de contato informados
        
        //Controles da ToolPart
        protected TextBox txtMotivosContato;
        private LiteralControl ltlInstrucao;

        /// <summary>
        /// Construtor padrão da ToolPart
        /// </summary>
        /// <param name="validador">Objeto que implementa funcionalidade de validação dos itens informados na seção customizada da WebPart</param>
        public GerenciamentoMotivosContatoToolPart(IValidacao<string> validador)
        {
            if (validador == null)
                throw new NullReferenceException("Informe um objeto de validação de Itens de Motivo de Contato");

            this.validador = validador;
        }

        /// <summary>
        /// Inicialização da ToolPart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            //Assinala o título no topo da seção customizada
            this.Title = "Gerenciar itens da lista de Motivos de Contato";

            base.OnInit(e);
        }

        /// <summary>
        /// Método disparado pelos botões Aplicar e Ir que são padrões da área de edição da ToolPart
        /// </summary>
        public override void ApplyChanges()
        {
            SumarioValidacao resultadoValidacao = this.validador.Validar(this.txtMotivosContato.Text);

            if (resultadoValidacao.Valido)
                this.WebPart.MotivosContato = this.txtMotivosContato.Text.Trim();
            else
            {
                this.WebPart.Page.ClientScript.RegisterStartupScript(typeof(Page), "_erroValidacaoMotivosContato", string.Format("<script>alert('{0}');</script>", MontadorMensagemErroUtil.Montar(resultadoValidacao, GerenciamentoMotivosContatoToolPart.cabecalhoInstrucaoCorrecao)));
                this.CancelChanges();
            }
        }

        /// <summary>
        /// Obtém a referência ao controle WebPart que carrega este container ToolPart
        /// Útil para leitura/escrita de propriedades públicas da WebPart
        /// (Parent Container)
        /// </summary>
        private FormularioContato WebPart
        {
            get
            {
                return this.ParentToolPane.SelectedWebPart as FormularioContato;
            }
        }

        /// <summary>
        /// Método disparado automaticamente para renderização do controles na área customizada
        /// </summary>
        protected override void CreateChildControls()
        {
            //Instanciação dos controles a adicionar no container
            this.ltlInstrucao = new LiteralControl(GerenciamentoMotivosContatoToolPart.mensagemInstrucao);
            this.ltlInstrucao.ID = "ltlInstrucao";

            this.txtMotivosContato = new TextBox();
            this.txtMotivosContato.ID = "txtMotivosContato";
            this.txtMotivosContato.TextMode = TextBoxMode.MultiLine;
            this.txtMotivosContato.Width = Unit.Pixel(300);
            this.txtMotivosContato.Height = Unit.Pixel(400);
            this.txtMotivosContato.Rows = 10;
            //Atribuição da Informação(vide Propriedade MotivosContato) no campo
            this.txtMotivosContato.Text = this.WebPart.MotivosContato;

            //Adição dos controles no container
            this.Controls.Add(this.ltlInstrucao);
            this.Controls.Add(this.txtMotivosContato);
            
            base.CreateChildControls();
        }
    }
}