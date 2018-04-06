using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Conversores;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper.Validacao;
using Redecard.Portal.Helper.Web.Controles;
using Redecard.Portal.Helper.Web.Mails;

namespace Redecard.Portal.Aberto.WebParts.FormularioCanalDenuncias
{
    [ToolboxItemAttribute(false)]
    public class FormularioCanalDenuncias : WebPartFormularioContatoBase
    {
        #region Variáveis privadas das propriedades da WebPart
        private string emailPadraoDestinatario;
        #endregion
        //private static string grupoValidacao = "vgrpFormularioContato";

        private IValidacao<Contato> validadorFormulario = new ValidadorFormularioCanalDenuncias();

        #region Propriedades da Web Part
        /// <summary>
        /// Endereço padrão de e-mail destinatário
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Endereço padrão de e-mail destinatário")]
        [Personalizable(PersonalizationScope.Shared)]
        public string EmailPadraoDestinatario
        {
            get
            {
                return this.emailPadraoDestinatario;
            }
            set
            {
                this.emailPadraoDestinatario = value;
            }
        }
        #endregion

        #region Controles do formulário
        private TextBox txtNome;
        private TextBox txtEmail;
        private TextBox txtDDD;
        private TextBox txtTelefone;
        private DropDownList ddlMotivoContato;
        private TextBox txtMensagem;
        private Button btnEnviar;
        private Literal ltlMensagem;

        //private RequiredFieldValidator rfvNome;
        //private RequiredFieldValidator rfvEmail;
        //private RegularExpressionValidator rgxEmail;
        //private RequiredFieldValidator rfvDDD;
        //private RegularExpressionValidator rgxDDD;
        //private RequiredFieldValidator rfvTelefone;
        //private RegularExpressionValidator rgxTelefone;
        //private RequiredFieldValidator rfvMotivoContato;
        //private RequiredFieldValidator rfvMensagem;
        //private ValidationSummary vldSumarioValidacao;
        #endregion

        #region Marcações HTML
        private static string quebraDeLinha = "<br/>";
        private static string aberturaLabel = "<label>";
        private static string fechamentoLabel = "</label>";
        private static string aberturaFieldSet = "<fieldset>";
        private static string fechamentoFieldSet = "</fieldset>";
        #endregion

        /// <summary>
        /// Método disparado automaticamente para renderização do controles na área customizada
        /// </summary>
        protected override void CreateChildControls()
        {
            #region Instanciação dos controles
            LiteralControl ltlQuebra = new LiteralControl(FormularioCanalDenuncias.quebraDeLinha);
            LiteralControl ltlAberturaLabelPadrao = new LiteralControl(FormularioCanalDenuncias.aberturaLabel);
            LiteralControl ltlFechamentoLabel = new LiteralControl(FormularioCanalDenuncias.fechamentoLabel);
            LiteralControl ltlAberturaFieldSetPadrao = new LiteralControl(FormularioCanalDenuncias.aberturaFieldSet);
            LiteralControl ltlFechamentoFieldSet = new LiteralControl(FormularioCanalDenuncias.fechamentoFieldSet);
            LiteralControl ltlEnvieNosUmaMensagem = new LiteralControl("<p class=\"textDestaque\">" + RedecardHelper.ObterResource("formCanalDenuncias_EnvieNosUmaMensagem") + "</p>");
            LiteralControl ltlCamposMarcadosAsterisco = new LiteralControl("<p>" + RedecardHelper.ObterResource("formCanalDenuncias_CamposMarcadosAsteriscoObrigatorio") + "</p>");

            Panel pnlBoxCenter = new Panel();
            pnlBoxCenter.CssClass = "center";
            pnlBoxCenter.ID = "divBoxCenter";

            Panel pnlBoxLeft = new Panel();
            pnlBoxLeft.CssClass = "left";
            pnlBoxLeft.ID = "divBoxLeft";

            Panel pnlBoxCadastro = new Panel();
            pnlBoxCadastro.CssClass = "boxFormParceiros";
            pnlBoxCadastro.ID = "divBoxFormParceiros";

            Panel pnlFormularioCadastro = new Panel();
            pnlFormularioCadastro.CssClass = "frmTestimonials";
            pnlFormularioCadastro.ID = "divCamposFormulario";

            this.txtNome = new TextBox();
            this.txtNome.ID = "txtNome";
            this.txtNome.MaxLength = 255;
            //this.txtNome.ValidationGroup = FormularioContato.grupoValidacao;

            //this.rfvNome = new RequiredFieldValidator();
            //this.rfvNome.ID = "rfvNome";
            //this.rfvNome.ControlToValidate = "txtNome";
            //this.rfvNome.ErrorMessage = "(*) Campo Nome obrigatório";
            //this.rfvNome.Text = "*";
            //this.rfvNome.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rfvNome.SetFocusOnError = true;
            //this.rfvNome.Display = ValidatorDisplay.Dynamic;

            this.txtEmail = new TextBox();
            this.txtEmail.ID = "txtEmail";
            this.txtEmail.MaxLength = 255;
            //this.txtEmail.ValidationGroup = FormularioContato.grupoValidacao;

            //this.rfvEmail = new RequiredFieldValidator();
            //this.rfvEmail.ID = "rfvEmail";
            //this.rfvEmail.ControlToValidate = "txtEmail";
            //this.rfvEmail.ErrorMessage = "(*) Campo Email obrigatório";
            //this.rfvEmail.Text = "*";
            //this.rfvEmail.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rfvEmail.SetFocusOnError = true;
            //this.rfvEmail.Display = ValidatorDisplay.Dynamic;
            //this.rfvEmail.Enabled = false;

            //this.rgxEmail = new RegularExpressionValidator();
            //this.rgxEmail.ID = "rgxEmail";
            //this.rgxEmail.ControlToValidate = "txtEmail";
            //this.rgxEmail.ErrorMessage = "(*) Campo Email inválido";
            //this.rgxEmail.Text = "*";
            //this.rgxEmail.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rgxEmail.SetFocusOnError = true;
            //this.rgxEmail.Display = ValidatorDisplay.Dynamic;
            //this.rgxEmail.ValidationExpression = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

            this.txtDDD = new TextBox();
            this.txtDDD.ID = "txtDDD";
            this.txtDDD.CssClass = "ddd";
            this.txtDDD.MaxLength = 5;
            //this.txtDDD.ValidationGroup = FormularioContato.grupoValidacao;

            //this.rfvDDD = new RequiredFieldValidator();
            //this.rfvDDD.ID = "rfvDDD";
            //this.rfvDDD.ControlToValidate = "txtDDD";
            //this.rfvDDD.ErrorMessage = "(*) Campo DDD obrigatório";
            //this.rfvDDD.Text = "*";
            //this.rfvDDD.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rfvDDD.SetFocusOnError = true;
            //this.rfvDDD.Display = ValidatorDisplay.Dynamic;
            //this.rfvDDD.Enabled = false;

            //this.rgxDDD = new RegularExpressionValidator();
            //this.rgxDDD.ID = "rgxDDD";
            //this.rgxDDD.ControlToValidate = "txtDDD";
            //this.rgxDDD.ErrorMessage = "(*) Campo DDD inválido";
            //this.rgxDDD.Text = "*";
            //this.rgxDDD.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rgxDDD.SetFocusOnError = true;
            //this.rgxDDD.Display = ValidatorDisplay.Dynamic;
            //this.rgxDDD.ValidationExpression = @"^[0-9]{1,5}$";

            this.txtTelefone = new TextBox();
            this.txtTelefone.ID = "txtTelefone";
            this.txtTelefone.CssClass = "tel";
            this.txtTelefone.MaxLength = 15;
            //this.txtTelefone.ValidationGroup = FormularioContato.grupoValidacao;

            //this.rfvTelefone = new RequiredFieldValidator();
            //this.rfvTelefone.ID = "rfvTelefone";
            //this.rfvTelefone.ControlToValidate = "txtTelefone";
            //this.rfvTelefone.ErrorMessage = "(*) Campo Telefone obrigatório";
            //this.rfvTelefone.Text = "*";
            //this.rfvTelefone.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rfvTelefone.SetFocusOnError = true;
            //this.rfvTelefone.Display = ValidatorDisplay.Dynamic;
            //this.rfvTelefone.Enabled = false;

            //this.rgxTelefone = new RegularExpressionValidator();
            //this.rgxTelefone.ID = "rgxTelefone";
            //this.rgxTelefone.ControlToValidate = "txtTelefone";
            //this.rgxTelefone.ErrorMessage = "(*) Campo Telefone inválido";
            //this.rgxTelefone.Text = "*";
            //this.rgxTelefone.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rgxTelefone.SetFocusOnError = true;
            //this.rgxTelefone.Display = ValidatorDisplay.Dynamic;
            //this.rgxTelefone.ValidationExpression = @"^[0-9]{1,15}$";

            this.PopularListaMotivosContato(ref this.ddlMotivoContato);
            if (!object.ReferenceEquals(this.ddlMotivoContato, null))
            {
                this.ddlMotivoContato.ID = "ddlMotivoContato";
                //this.ddlMotivoContato.ValidationGroup = FormularioContato.grupoValidacao;
            }

            //this.rfvMotivoContato = new RequiredFieldValidator();
            //this.rfvMotivoContato.ID = "rfvMotivoContato";
            //this.rfvMotivoContato.ControlToValidate = "ddlMotivoContato";
            //this.rfvMotivoContato.ErrorMessage = "(*) Campo Motivo Contato obrigatório";
            //this.rfvMotivoContato.Text = "*";
            //this.rfvMotivoContato.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rfvMotivoContato.SetFocusOnError = true;
            //this.rfvMotivoContato.Display = ValidatorDisplay.Dynamic;
            //this.rfvMotivoContato.InitialValue = FormularioContato.valorPadraoSelecaoMotivo;

            this.txtMensagem = new TextBox();
            this.txtMensagem.ID = "txtMensagem";
            this.txtMensagem.TextMode = TextBoxMode.MultiLine;
            this.txtMensagem.Rows = 6;
            //this.txtMensagem.ValidationGroup = FormularioContato.grupoValidacao;

            //this.rfvMensagem = new RequiredFieldValidator();
            //this.rfvMensagem.ID = "rfvMensagem";
            //this.rfvMensagem.ControlToValidate = "txtMensagem";
            //this.rfvMensagem.ErrorMessage = "(*) Campo Mensagem obrigatório";
            //this.rfvMensagem.Text = "*";
            //this.rfvMensagem.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rfvMensagem.SetFocusOnError = true;
            //this.rfvMensagem.Display = ValidatorDisplay.Dynamic;

            //this.vldSumarioValidacao = new ValidationSummary();
            //this.vldSumarioValidacao.ID = "vldSumarioValidacao";
            //this.vldSumarioValidacao.ValidationGroup = FormularioContato.grupoValidacao;
            //this.vldSumarioValidacao.HeaderText = "Por favor, corrija o(s) seguinte(s) erro(s):";
            //this.vldSumarioValidacao.ShowMessageBox = true;
            //this.vldSumarioValidacao.ShowSummary = false;

            this.btnEnviar = new Button();
            this.btnEnviar.Text = RedecardHelper.ObterResource("enviar");
            this.btnEnviar.CssClass = "btOk";
            //this.btnEnviar.ValidationGroup = FormularioContato.grupoValidacao;
            this.btnEnviar.Click += new EventHandler(this.btnEnviar_Click);

            this.ltlMensagem = new Literal();
            this.ltlMensagem.ID = "ltlMensagem";
            #endregion

            //#region Montagem do Layout do form

            //#region HTML Template
            //*
            // * Modelo de formulário

            //<div class="boxFormParceiros">
            //    <form class="frmTestimonials" id="formTeste"> -> form vira DIV (ALTERAR O CSS!)
            //        <fieldset>
            //            <p class="textDestaque">Envie-nos uma mensagem</p>
            //            <p>Os campos marcados com asterisco (*) são de preenchimento obrigatório.</p>
            //            <label>Seu nome:<br /> <input type="text" name="cpf_1" /></label>
            //            <label>Seu e-mail:<br /> <input type="text" name="cpf_2" /></label>
            //            <label class="campTel">Telefone de contato:<br /> <input type="text" name="ddd" class="ddd" /><input type="text" name="tel" class="tel" /><span>(Somente números)</span></label>
            //            <label>* Motivo do contato:<br /> <select><option>Selecione uma Opção</option><option>Selecione uma Opção</option><option>Selecione uma Opção</option></select></label>
            //            <label class="labelDivisao">* Sua mensagem:<br /> <textarea></textarea></label>  
            //            <label><input type="submit" class="btOk" value="ENVIAR" /></label>                                                                 
            //        </fieldset>
            //    </form>
            //</div>

            //Fim Modelo de formulário
            // */
            //#endregion

            pnlFormularioCadastro.Controls.Add(ltlAberturaFieldSetPadrao);
            //Texto Envie-nos uma mensagem
            pnlFormularioCadastro.Controls.Add(ltlEnvieNosUmaMensagem);

            //Texto Os Campos marcados com asterisco...
            pnlFormularioCadastro.Controls.Add(ltlCamposMarcadosAsterisco);

            //Campo Nome
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formCanalDenuncias_Campo_SeuNome")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.txtNome);
            //pnlFormularioCadastro.Controls.Add(this.rfvNome);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo Email
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formCanalDenuncias_Campo_SeuEmail")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.txtEmail);
            //pnlFormularioCadastro.Controls.Add(this.rfvEmail);
            //pnlFormularioCadastro.Controls.Add(this.rgxEmail);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo telefone
            pnlFormularioCadastro.Controls.Add(new LiteralControl("<label class=\"campTel\">"));
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formCanalDenuncias_Campo_TelefoneContato")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.txtDDD);
            //pnlFormularioCadastro.Controls.Add(this.rfvDDD);
            //pnlFormularioCadastro.Controls.Add(this.rgxDDD);
            pnlFormularioCadastro.Controls.Add(this.txtTelefone);
            //pnlFormularioCadastro.Controls.Add(this.rfvTelefone);
            //pnlFormularioCadastro.Controls.Add(this.rgxTelefone);
            pnlFormularioCadastro.Controls.Add(new LiteralControl("<span>" + RedecardHelper.ObterResource("formCanalDenuncias_Campo_SomenteNumeros") + "</span>"));
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo Motivo do Contato
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formCanalDenuncias_Campo_MotivoContato")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.ddlMotivoContato);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo Mensagem
            pnlFormularioCadastro.Controls.Add(new LiteralControl("<label class=\"labelDivisao\">"));
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formCanalDenuncias_Campo_SuaMensagem")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.txtMensagem);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Botão Enviar
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.btnEnviar);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());

            //Rótulo de mensagem
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl("<b>"));
            pnlFormularioCadastro.Controls.Add(this.ltlMensagem);
            pnlFormularioCadastro.Controls.Add(new LiteralControl("</b>"));
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            pnlFormularioCadastro.Controls.Add(ltlFechamentoFieldSet);
            //pnlFormularioCadastro.Controls.Add(this.vldSumarioValidacao);

            pnlBoxCadastro.Controls.Add(pnlFormularioCadastro);
            pnlBoxLeft.Controls.Add(pnlBoxCadastro);
            pnlBoxCenter.Controls.Add(pnlBoxLeft);
            this.Controls.Add(pnlBoxCenter);
            //#endregion
        }

        /// <summary>
        /// Método para carregamento dos contêineres da parte de edição da WebPart
        /// </summary>
        /// <returns></returns>
        public override ToolPart[] GetToolParts()
        {
            ToolPart[] allToolParts = new ToolPart[3];

            allToolParts[0] = new WebPartToolPart(); ;
            allToolParts[1] = new CustomPropertyToolPart();
            allToolParts[2] = new GerenciamentoMotivosContatoCanalDenunciasToolPart(new ValidadorMotivoContato());

            return allToolParts;
        }

        /// <summary>
        /// Retorna um DTO de contato a partir dos campos do formulário
        /// </summary>
        /// <returns></returns>
        private Contato ObterDosDadosDoFormulario()
        {
            return new Contato()
            {
                Nome = this.txtNome.Text.Trim(),
                Email = this.txtEmail.Text.Trim(),
                DDD = this.txtDDD.Text.Trim(),
                Telefone = this.txtTelefone.Text.Trim(),
                Motivo = new MotivoContato(this.ddlMotivoContato.SelectedItem.Text, this.ddlMotivoContato.SelectedItem.Value),
                Mensagem = this.txtMensagem.Text.Trim()
            };
        }

        /// <summary>
        /// Zera os campos do formulário
        /// </summary>
        private void LimparCampos()
        {
            this.txtNome.Text =
            this.txtEmail.Text =
            this.txtDDD.Text =
            this.txtTelefone.Text =
            this.txtMensagem.Text = string.Empty;

            this.ddlMotivoContato.SelectedIndex = -1;
        }

        /// <summary>
        /// Solicita obtenção do corpo do email
        /// </summary>
        /// <param name="contato"></param>
        /// <returns></returns>
        private string ObterCorpoEmail(Contato contato)
        {
            //TODO: TEMPLATE EMAIL FORM CANAL DE DENUNCIAS PENDENTE!

            string campoNome = RedecardHelper.ObterResource("formCanalDenuncias_CorpoEmail_Nome");
            string campoEmail = RedecardHelper.ObterResource("formCanalDenuncias_CorpoEmail_Email");
            string campoTelefone = RedecardHelper.ObterResource("formCanalDenuncias_CorpoEmail_Telefone");
            string campoMotivo = RedecardHelper.ObterResource("formCanalDenuncias_CorpoEmail_MotivoContato");
            string campoMensagem = RedecardHelper.ObterResource("formCanalDenuncias_CorpoEmail_Mensagem");
            string campoNaoInformado = RedecardHelper.ObterResource("formCanalDenuncias_CorpoEmail_NaoInformado");

            string telefone = string.Empty;

            //Campo telefone no formato Telefone: xx - xxxxxxxx ou simplesmente xxxxxxxx
            if (string.IsNullOrEmpty(contato.Telefone))
                telefone = campoNaoInformado;
            else
            {
                if (!string.IsNullOrEmpty(contato.DDD))
                    telefone = string.Format("{0} - {1}", contato.DDD, contato.Telefone);
                else
                    telefone = string.Format("{0}", contato.Telefone);
            }

            string email = string.Format("{0} {1}\n" +
                                         "{2} {3}\n" +
                                         "{4} {5}\n" +
                                         "{6} {7}\n" +
                                         "{8}\n" +
                                         "{9}",
                                          campoNome,
                                          contato.Nome,
                                          campoEmail,
                                          contato.Email,
                                          campoTelefone,
                                          telefone,
                                          campoMotivo,
                                          contato.Motivo.Descricao,
                                          campoMensagem,
                                          contato.Mensagem);
            return email;
        }

        /// <summary>
        /// Envia email respeitando a seguinte regra: Com base na opção de Motivo de Contato selecionado, se a opção contiver um endereço de eimail relacionado,
        /// o email é enviado a este endereço (apontado) e com cópia no endereço de e-mail atribuido na propriedade EmailPadraoDestinatario.
        /// Caso contrário, o email é enviado apenas para o endereço de e-mail atribuido na propriedade EmailPadraoDestinatario
        /// </summary>
        /// <param name="contato"></param>
        private void EnviarEmail(Contato contato)
        {
            if (this.MotivoSelecionadoContemEmailDestinatario(this.ddlMotivoContato))
                EmailUtils.EnviarEmail(null,"rede@userede.com.br",  this.ddlMotivoContato.SelectedValue, this.EmailPadraoDestinatario, null, FormularioCanalDenuncias.textoPadraoCabecalhoEmail, true, this.ObterCorpoEmail(contato));
            else
                EmailUtils.EnviarEmail(null, "rede@userede.com.br", this.EmailPadraoDestinatario, null, null, FormularioCanalDenuncias.textoPadraoCabecalhoEmail, true, this.ObterCorpoEmail(contato));
        }

        /// <summary>
        /// Evento de clique para solicitação de envio de email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            Contato contato = this.ObterDosDadosDoFormulario();
            SumarioValidacao sumarioValidacao = this.validadorFormulario.Validar(contato);
            this.ltlMensagem.Text = string.Empty;

            if (sumarioValidacao.Valido)
            {
                this.EnviarEmail(contato);

                this.LimparCampos();

                this.ltlMensagem.Text = FormularioCanalDenuncias.textoPadraoSucessoEnvioEmail;
            }
            else
                Page.ClientScript.RegisterStartupScript(typeof(Page), "_erroEnvioEmail", string.Format("alert('{0}');", MontadorMensagemErroUtil.Montar(sumarioValidacao, FormularioCanalDenuncias.textoPadraoCabecalhoErros)), true);
        }
    }
}