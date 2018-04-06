using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebPartPages;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper.Validacao;
using Redecard.Portal.Helper.Web.Controles;
using Redecard.Portal.Helper.Web.Mails;

namespace Redecard.Portal.Aberto.WebParts.FormularioContato
{
    [ToolboxItemAttribute(false)]
    public class FormularioContato : WebPartFormularioContatoBase
    {
        #region Variáveis privadas das propriedades da WebPart
        private string emailPadraoDestinatario;
        private string msgErroEmail;
        private string msgSucessoEmail;
        private string JScriptHideFields_Key = "__hideFormContatoFields__";
        private string JScriptHideFields_Script = "$(document).ready(function () {{" +
                                                        "$('.ddlcliente:first').change(function () {{" +
                                                            "var value = $(this).val();" +
                                                            "if (value == \"Sim\") {{" +
                                                                "$('.fieldclient').fadeIn();" +
                                                            "}} else {{" +
                                                                "$('.fieldclient').fadeOut();" +
                                                            "}}" +
                                                        "}});" +
                                                        "var svalue = $('.ddlcliente:first').val();" +
                                                        "if (svalue == \"Sim\") {{" +
                                                            "$('.fieldclient').fadeIn();" +
                                                        "}} else {{" +
                                                            "$('.fieldclient').fadeOut();" +
                                                        "}}" +
                                                        "$('#{0}').ForceNumericOnly();" +
                                                        "$('#{1}').ForceNumericOnly();" +
                                                        "$('#{2}').ForceNumericOnly();" +
                                                        "$('#{3}').ForceNumericOnly();" +
                                                    "}});";
        #endregion
        //private static string grupoValidacao = "vgrpFormularioContato";

        private IValidacao<Contato> validadorFormulario = new ValidadorFormularioContato();

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

        /// <summary>
        /// Mensagem padrão de erro no envio do e-mail
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Mensagem de erro no envio do e-mail:")]
        [Personalizable(PersonalizationScope.Shared)]
        public string MsgErroEmail
        {
            get
            {
                return this.msgErroEmail;
            }
            set
            {
                this.msgErroEmail = value;
            }
        }
        
        /// <summary>
        /// Mensagem padrão de erro no envio do e-mail
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Mensagem de sucesso no envio do e-mail:")]
        [Personalizable(PersonalizationScope.Shared)]
        public string MsgSucessoEmail
        {
            get
            {
                return this.msgSucessoEmail;
            }
            set
            {
                this.msgSucessoEmail = value;
            }
        }
        #endregion

        #region Controles do formulário
        private TextBox txtNome;
        private TextBox txtEmail;
        private TextBox txtDDD;
        private TextBox txtTelefone;
        private TextBox txtNumeroEstabelecimento;
        private TextBox txtCNPJCPF;
        private TextBox txtRazaoSocial;
        private DropDownList ddlMotivoContato;
        private DropDownList ddlENossoCliente;
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
        //private RequiredFieldValidator rfvNumeroEstabelecimento;
        //private RegularExpressionValidator rgxNumeroEstabelecimento;
        //private RequiredFieldValidator rfvMotivoContato;
        //private RequiredFieldValidator rfvMensagem;
        //private ValidationSummary vldSumarioValidacao;
        #endregion

        #region Marcações HTML
        private static string quebraDeLinha = "<br/>";
        private static string aberturaLabel = "<label>";
        private static string aberturaLabelClient = "<label class='fieldclient'>";
        private static string fechamentoLabel = "</label>";
        private static string aberturaFieldSet = "<fieldset>";
        private static string fechamentoFieldSet = "</fieldset>";
        #endregion

        /// <summary>
        /// Escrever JScript de controle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            ClientScriptManager cs = this.Page.ClientScript;
            if (!cs.IsClientScriptBlockRegistered(JScriptHideFields_Key))
            {
                cs.RegisterClientScriptBlock(this.GetType(), JScriptHideFields_Key,
                    String.Format(JScriptHideFields_Script, txtDDD.ClientID, txtCNPJCPF.ClientID, txtTelefone.ClientID, txtNumeroEstabelecimento.ClientID), true);
            }
            base.OnPreRender(e);
        }

        /// <summary>
        /// Método disparado automaticamente para renderização do controles na área customizada
        /// </summary>
        protected override void CreateChildControls()
        {
            #region Instanciação dos controles
            LiteralControl ltlQuebra = new LiteralControl(FormularioContato.quebraDeLinha);
            LiteralControl ltlAberturaLabelPadrao = new LiteralControl(FormularioContato.aberturaLabel);
            LiteralControl ltlAberturaLabelClientePadrao = new LiteralControl(FormularioContato.aberturaLabelClient);
            LiteralControl ltlFechamentoLabel = new LiteralControl(FormularioContato.fechamentoLabel);
            LiteralControl ltlAberturaFieldSetPadrao = new LiteralControl(FormularioContato.aberturaFieldSet);
            LiteralControl ltlFechamentoFieldSet = new LiteralControl(FormularioContato.fechamentoFieldSet);
            LiteralControl ltlEnvieNosUmaMensagem = new LiteralControl("<p class=\"textDestaque\">" + RedecardHelper.ObterResource("formContato_EnvieNosUmaMensagem") + "</p>");
            LiteralControl ltlCamposMarcadosAsterisco = new LiteralControl("<p>" + RedecardHelper.ObterResource("formContato_CamposMarcadosAsteriscoObrigatorio") + "</p>");

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
            this.txtDDD.MaxLength = 3;
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
            this.txtTelefone.MaxLength = 8;
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

            this.txtNumeroEstabelecimento = new TextBox();
            this.txtNumeroEstabelecimento.ID = "txtNumeroEstabelecimento";
            this.txtNumeroEstabelecimento.MaxLength = 30;
            this.txtNumeroEstabelecimento.Style.Add(HtmlTextWriterStyle.Width, "200px");
            //this.txtNumeroEstabelecimento.ValidationGroup = FormularioContato.grupoValidacao;

            this.txtCNPJCPF = new TextBox();
            this.txtCNPJCPF.ID = "txtCNPJCPF";
            this.txtCNPJCPF.MaxLength = 14;
            this.txtCNPJCPF.Style.Add(HtmlTextWriterStyle.Width, "200px");

            this.txtRazaoSocial = new TextBox();
            this.txtRazaoSocial.ID = "txtRazaoSocial";
            this.txtRazaoSocial.MaxLength = 255;

            //this.rfvNumeroEstabelecimento = new RequiredFieldValidator();
            //this.rfvNumeroEstabelecimento.ID = "rfvNumeroEstabelecimento";
            //this.rfvNumeroEstabelecimento.ControlToValidate = "txtNumeroEstabelecimento";
            //this.rfvNumeroEstabelecimento.ErrorMessage = "(*) Campo NumeroEstabelecimento obrigatório";
            //this.rfvNumeroEstabelecimento.Text = "*";
            //this.rfvNumeroEstabelecimento.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rfvNumeroEstabelecimento.SetFocusOnError = true;
            //this.rfvNumeroEstabelecimento.Display = ValidatorDisplay.Dynamic;
            //this.rfvNumeroEstabelecimento.Enabled = false;

            //this.rgxNumeroEstabelecimento = new RegularExpressionValidator();
            //this.rgxNumeroEstabelecimento.ID = "rgxNumeroEstabelecimento";
            //this.rgxNumeroEstabelecimento.ControlToValidate = "txtNumeroEstabelecimento";
            //this.rgxNumeroEstabelecimento.ErrorMessage = "(*) Campo NumeroEstabelecimento inválido";
            //this.rgxNumeroEstabelecimento.Text = "*";
            //this.rgxNumeroEstabelecimento.ValidationGroup = FormularioContato.grupoValidacao;
            //this.rgxNumeroEstabelecimento.SetFocusOnError = true;
            //this.rgxNumeroEstabelecimento.Display = ValidatorDisplay.Dynamic;
            //this.rgxNumeroEstabelecimento.ValidationExpression = @"^[0-9]{1,255}$";

            this.PopularListaMotivosContato(ref this.ddlMotivoContato);
            if (!object.ReferenceEquals(this.ddlMotivoContato, null))
            {
                this.ddlMotivoContato.ID = "ddlMotivoContato";
                //this.ddlMotivoContato.ValidationGroup = FormularioContato.grupoValidacao;
            }

            if (object.ReferenceEquals(this.ddlENossoCliente, null))
            {
                this.ddlENossoCliente = new DropDownList();
                this.ddlENossoCliente.ID = "ENossoCliente";
                this.ddlENossoCliente.Attributes.Add("class", "ddlcliente");
                this.ddlENossoCliente.Items.Add(new ListItem(RedecardHelper.ObterResource("formContato_CampoDDL_Selecione"), String.Empty));
                this.ddlENossoCliente.Items.Add(new ListItem(RedecardHelper.ObterResource("formContato_CampoDDL_Sim"), "Sim"));
                this.ddlENossoCliente.Items.Add(new ListItem(RedecardHelper.ObterResource("formContato_CampoDDL_Nao"), "Não"));
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
            //            * Este campo foi incluído seguindo solicitação da Redecard
            //            <label>* É nosso cliente?<br /> <select><option value=''>Selecione uma Opção</option><option value='Sim'>Sim</option><option value='Não'>Não</option></select></label>
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

            //Campo É Nosso Cliente?
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContato_Campo_ENossoCliente")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.ddlENossoCliente);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo Número do Estabelecimento
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelClientePadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContato_Campo_NumeroEstabelecimento")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.txtNumeroEstabelecimento);
            //pnlFormularioCadastro.Controls.Add(this.rfvNome);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo CNPJ/CPF
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContato_Campo_CNPJ_CPF")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.txtCNPJCPF);
            //pnlFormularioCadastro.Controls.Add(this.rfvNome);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo Razão Social
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContato_Campo_RazaoSocial")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.txtRazaoSocial);
            //pnlFormularioCadastro.Controls.Add(this.rfvNome);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo Nome
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContato_Campo_SeuNome")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.txtNome);
            //pnlFormularioCadastro.Controls.Add(this.rfvNome);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo Email
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContato_Campo_SeuEmail")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.txtEmail);
            //pnlFormularioCadastro.Controls.Add(this.rfvEmail);
            //pnlFormularioCadastro.Controls.Add(this.rgxEmail);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo telefone
            pnlFormularioCadastro.Controls.Add(new LiteralControl("<label class=\"campTel\">"));
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContato_Campo_TelefoneContato")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.txtDDD);
            //pnlFormularioCadastro.Controls.Add(this.rfvDDD);
            //pnlFormularioCadastro.Controls.Add(this.rgxDDD);
            pnlFormularioCadastro.Controls.Add(this.txtTelefone);
            //pnlFormularioCadastro.Controls.Add(this.rfvTelefone);
            //pnlFormularioCadastro.Controls.Add(this.rgxTelefone);
            pnlFormularioCadastro.Controls.Add(new LiteralControl("<span>" + RedecardHelper.ObterResource("formContato_Campo_SomenteNumeros") + "</span>"));
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo Motivo do Contato
            pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContato_Campo_MotivoContato")));
            pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
            pnlFormularioCadastro.Controls.Add(this.ddlMotivoContato);
            pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            //Campo Mensagem
            pnlFormularioCadastro.Controls.Add(new LiteralControl("<label class=\"labelDivisao\">"));
            pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContato_Campo_SuaMensagem")));
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

            string scriptDisableButton = "$(document).ready(function () {" +
                                                    "$('.btOk').click(function () {" +
                                                                "$(this).hide(); $(this).parent().append(\"<center>Aguarde.....</center>\");});" +
                                                        "});";
            //Page.ClientScript.RegisterStartupScript(typeof(Page), "", scriptDisableButton, true);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "btn", "<script type = 'text/javascript'>" + scriptDisableButton + "</script>"); 
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
            allToolParts[2] = new GerenciamentoMotivosContatoToolPart(new ValidadorMotivoContato());

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
                ECliente = this.ddlENossoCliente.SelectedValue,
                Nome = this.txtNome.Text.Trim(),
                Email = this.txtEmail.Text.Trim(),
                DDD = this.txtDDD.Text.Trim(),
                Telefone = this.txtTelefone.Text.Trim(),
                NumeroEstabelecimento = this.txtNumeroEstabelecimento.Text.Trim(),
                Motivo = new MotivoContato(this.ddlMotivoContato.SelectedItem.Text, this.ddlMotivoContato.SelectedItem.Value),
                CNPJCPF = this.txtCNPJCPF.Text.Trim(),
                RazaoSocial = this.txtRazaoSocial.Text.Trim(),
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
            this.txtNumeroEstabelecimento.Text =
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
            //TODO: TEMPLATE EMAIL FORM CONTATO PENDENTE!

            string campoNome = RedecardHelper.ObterResource("formContato_CorpoEmail_Nome");
            string campoEmail = RedecardHelper.ObterResource("formContato_CorpoEmail_Email");
            string campoTelefone = RedecardHelper.ObterResource("formContato_CorpoEmail_Telefone");
            string campoNroEstabelecimento = RedecardHelper.ObterResource("formContato_CorpoEmail_NroEstabelecimento");
            string campoMensagem = RedecardHelper.ObterResource("formContato_CorpoEmail_Mensagem");
            string campoNaoInformado = RedecardHelper.ObterResource("formContato_CorpoEmail_NaoInformado");
            string campoRazaoSocial = RedecardHelper.ObterResource("formContatoOuvidoria_Campo_RazaoSocial");
            string campoCNPJCPF = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_CPFCNPJ");
            string campoENossoCliente = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_ENossoCliente");

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

            string email = string.Format("{0} {1}<br />" +
                                         "{2} {3}<br />" +
                                         "{4} {5}<br />" +
                                         "{6} {7}<br />" +
                                         "{10} {11}<br />" +
                                         "{12} {13}<br />" +
                                         "{14} {15}<br />" +
                                         "{8}<br />" +
                                         "{9}",
                                          campoNome,
                                          contato.Nome,
                                          campoEmail,
                                          contato.Email,
                                          campoTelefone,
                                          telefone,
                                          campoNroEstabelecimento,
                                          string.IsNullOrEmpty(contato.NumeroEstabelecimento) ? campoNaoInformado : contato.NumeroEstabelecimento,
                                          campoMensagem,
                                          contato.Mensagem,
                                          campoRazaoSocial,
                                          contato.RazaoSocial,
                                          campoCNPJCPF,
                                          contato.CNPJCPF,
                                          campoENossoCliente,
                                          contato.ECliente);
            return email;
        }

        /// <summary>
        /// Envia email respeitando a seguinte regra: Com base na opção de Motivo de Contato selecionado, se a opção contiver um endereço de eimail relacionado,
        /// o email é enviado a este endereço (apontado) e com cópia no endereço de e-mail atribuido na propriedade EmailPadraoDestinatario.
        /// Caso contrário, o email é enviado apenas para o endereço de e-mail atribuido na propriedade EmailPadraoDestinatario
        /// </summary>
        /// <param name="contato"></param>
        private bool EnviarEmail(Contato contato)
        {
            string emailFrom = contato.Email;
            if (this.MotivoSelecionadoContemEmailDestinatario(this.ddlMotivoContato))
            {
                bool rtnEnviarEmail = EmailUtils.EnviarEmail(null, emailFrom, this.ddlMotivoContato.SelectedValue, this.EmailPadraoDestinatario, null, FormularioContato.textoPadraoCabecalhoEmail, true, this.ObterCorpoEmail(contato));
                return rtnEnviarEmail;
            }
            else
            {
                bool rtnEnviarEmail = EmailUtils.EnviarEmail(null, emailFrom, this.EmailPadraoDestinatario, null, null, FormularioContato.textoPadraoCabecalhoEmail, true, this.ObterCorpoEmail(contato));
                return rtnEnviarEmail;
            }
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
                try
                {
                    if (this.EnviarEmail(contato))
                    {
                        this.LimparCampos();
                        //this.ltlMensagem.Text = FormularioContato.textoPadraoSucessoEnvioEmail;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "_sucessEnvioEmail","$('#pnlEmailEnviado, #bgProtecao').show();", true);                       
                    }
                    else
                    {
                        //this.ltlMensagem.Text = this.MsgErroEmail;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "_erroEnvioEmail", "$('#pnlEmailNaoEnviado, #bgProtecao').show();", true);
                    }
                }
                catch
                {
                    //this.ltlMensagem.Text = this.MsgErroEmail;
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "_erroEnvioEmail2", "$('#pnlEmailEnviado, #bgProtecao').show();", true);
                }
            }
            else
                Page.ClientScript.RegisterStartupScript(this.GetType(), "_erroEnvioEmail3", string.Format("alert('{0}');", MontadorMensagemErroUtil.Montar(sumarioValidacao, FormularioContato.textoPadraoCabecalhoErros)), true);
            
        }
    }
}