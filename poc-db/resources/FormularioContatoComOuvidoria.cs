using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Aberto.WebParts.FormularioContato;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Conversores;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper.Validacao;
using Redecard.Portal.Helper.Web.Controles;
using Redecard.Portal.Helper.Web.Mails;

namespace Redecard.Portal.Aberto.WebParts.FormularioContatoComOuvidoria
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Data criação: 05/08/2010
    /// Descrição: Webpart de formulário de contato com a ouvidoria
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class FormularioContatoComOuvidoria : WebPartFormularioContatoBase
    {
        #region Variáveis privadas das propriedades da WebPart
        private string mensagemNaoEncaminhamentoSolicitacaoEmOutraArea;
        private string linkPaginaFaleConosco = "~/atendimento/faleconosco.aspx";
        private string mensagemCaixaAtencao;
        private string emailPadraoCopiaTodos;
        #endregion

        #region Variáveis privadas
        private IValidacao<Contato> validadorFormulario = new ValidadorFormularioContatoComOuvidoria();
        #endregion

        #region Propriedades da Web Part
        /// <summary>
        /// Mensagem que aparecerá caso o usuário não tenha encaminhado a solicitação em outra área
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Mensagem que aparecerá caso o usuário não tenha encaminhado a solicitação em outra área")]
        [Personalizable(PersonalizationScope.Shared)]
        public string MensagemNaoEncaminhamentoSolicitacaoEmOutraArea
        {
            get
            {
                return this.mensagemNaoEncaminhamentoSolicitacaoEmOutraArea;
            }
            set
            {
                this.mensagemNaoEncaminhamentoSolicitacaoEmOutraArea = value;
            }
        }

        /// <summary>
        /// Link para a página de Fale Conosco
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Link para a página Fale Conosco")]
        [Personalizable(PersonalizationScope.Shared)]
        public string LinkPaginaFaleConosco
        {
            get
            {
                return this.linkPaginaFaleConosco;
            }
            set
            {
                this.linkPaginaFaleConosco = value;
            }
        }

        /// <summary>
        /// Mensagem que é mostrada na caixa de Atenção
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Mensagem que aparecerá na Caixa de Atenção")]
        [Personalizable(PersonalizationScope.Shared)]
        public string MensagemCaixaAtencao
        {
            get
            {
                return this.mensagemCaixaAtencao;
            }
            set
            {
                this.mensagemCaixaAtencao = value;
            }
        }

        /// <summary>
        /// Endereço padrão de e-mail de cópia para todos itens de motivo de contato
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Endereço padrão de cópia para todos itens de motivo de contato")]
        [Personalizable(PersonalizationScope.Shared)]
        [DefaultValue("portalouvidoria@userede.com.br")]
        public string EmailPadraoCopiaTodos
        {
            get
            {
                return this.emailPadraoCopiaTodos;
            }
            set
            {
                this.emailPadraoCopiaTodos = value;
            }
        }

        ///// <summary>
        ///// Mensagem que é mostrada na tela quando o usuário envia o e-mail
        ///// </summary>
        //[WebBrowsable(true)]
        //[Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        //[WebDisplayName("Mensagem de retorno após envio do e-mail")]
        //[Personalizable(PersonalizationScope.Shared)]
        //public string MensagemDeRetornoEnvioEmail
        //{
        //    get
        //    {
        //        return this.mensagemDeRetornoEnvioEmail;
        //    }
        //    set
        //    {
        //        this.mensagemDeRetornoEnvioEmail = value;
        //    }
        //}
        #endregion

        #region Controles do formulário
        private RadioButton rdbSim;
        private RadioButton rdbNao;
        private TextBox txtNumeroProtocoloAtendimento;
        private DropDownList ddlTipoPessoa;
        private TextBox txtNumeroEstabelecimento;
        private TextBox txtCNPJCPF;
        private TextBox txtRazaoSocial;
        private TextBox txtNome;
        private TextBox txtEmail;
        //private TextBox txtDDD;
        private TextBox txtTelefone;
        private DropDownList ddlMotivoContato;
        private TextBox txtMensagem;
        private Button btnEnviar;
        private Literal ltlMensagem;
        Panel pnlBoxCadastro;

        private Literal ltlMensagemCaixaAtencao;
        private HyperLink lnkPaginaFaleConosco;
        #endregion

        #region Marcações HTML
        private static string quebraDeLinha = "<br/>";
        private static string aberturaLabel = "<label>";
        private static string fechamentoLabel = "</label>";
        private static string aberturaFieldSet = "<fieldset>";
        private static string fechamentoFieldSet = "</fieldset>";
        #endregion

        #region Eventos
        /// <summary>
        /// Solicitação de envio do email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            SumarioValidacao sumarioValidacao = this.validadorFormulario.Validar(this.ObterDosDadosDoFormulario());
            this.ltlMensagem.Text = string.Empty;

            if (sumarioValidacao.Valido)
            {
                //Solicitar a montagem do Email

                this.EnviarEmail(this.ObterDosDadosDoFormulario());

                this.LimparCampos();

                this.ltlMensagem.Text = FormularioContatoComOuvidoria.textoPadraoSucessoEnvioEmail;
            }
            else
                Page.ClientScript.RegisterStartupScript(typeof(Page), "_erroEnvioEmail", string.Format("alert('{0}');", MontadorMensagemErroUtil.Montar(sumarioValidacao, FormularioContatoComOuvidoria.textoPadraoCabecalhoErros)), true);
        }

        /// <summary>
        /// Mudança de seleção de opção sobre se já encaminhou solicitação para alguma outra área (radio buttons Sim/Não)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rdbSimNao_CheckedChanged(object sender, EventArgs e)
        {
            //Formulário de contato é mostrado na resposta Sim
            this.pnlBoxCadastro.Visible = this.rdbSim.Checked;

            //Executa limpeza dos campos
            this.LimparCampos();

            this.ConfigurarConteudoCaixaAtencao();
        }
        #endregion

        #region Métodos
        ///<summary>
        ///Método disparado automaticamente para renderização dos controles na WebPart
        ///</summary>
        protected override void CreateChildControls()
        {
            #region Instanciação dos Controles de Formulário
            LiteralControl ltlQuebra = new LiteralControl(FormularioContatoComOuvidoria.quebraDeLinha);
            LiteralControl ltlAberturaLabelPadrao = new LiteralControl(FormularioContatoComOuvidoria.aberturaLabel);
            LiteralControl ltlFechamentoLabel = new LiteralControl(FormularioContatoComOuvidoria.fechamentoLabel);
            LiteralControl ltlAberturaFieldSetPadrao = new LiteralControl(FormularioContatoComOuvidoria.aberturaFieldSet);
            LiteralControl ltlFechamentoFieldSet = new LiteralControl(FormularioContatoComOuvidoria.fechamentoFieldSet);
            LiteralControl ltlEnvieNosUmaMensagem = new LiteralControl("<p class=\"textDestaque\">" + RedecardHelper.ObterResource("formContatoOuvidoria_EnvieNosUmaMensagem") + "</p>");
            LiteralControl ltlCamposMarcadosAsterisco = new LiteralControl("<p>" + RedecardHelper.ObterResource("formContatoOuvidoria_CamposMarcadosAsteriscoObrigatorio") + "</p>");

            Panel pnlBoxCenter = new Panel();
            pnlBoxCenter.CssClass = "center";
            pnlBoxCenter.ID = "divBoxCenter";

            Panel pnlBoxLeft = new Panel();
            pnlBoxLeft.CssClass = "left";
            pnlBoxLeft.ID = "divBoxLeft";

            Panel pnlBoxQuestao = new Panel();
            pnlBoxQuestao.CssClass = "boxQuestao";
            pnlBoxQuestao.ID = "pnlBoxQuestao";

            Panel pnlBoxAtencao = new Panel();
            pnlBoxAtencao.CssClass = "boxAtencao";
            pnlBoxAtencao.ID = "pnlBoxAtencao";

            this.pnlBoxCadastro = new Panel();
            this.pnlBoxCadastro.CssClass = "boxFormParceiros";
            this.pnlBoxCadastro.ID = "divBoxFormParceiros";

            Panel pnlFormularioCadastro = new Panel();
            pnlFormularioCadastro.CssClass = "frmTestimonials";
            pnlFormularioCadastro.ID = "divCamposFormulario";

            this.txtNumeroProtocoloAtendimento = new TextBox();
            this.txtNumeroProtocoloAtendimento.ID = "txtNumeroProtocoloAtendimento";
            this.txtNumeroProtocoloAtendimento.MaxLength = 100;

            this.txtNumeroEstabelecimento = new TextBox();
            this.txtNumeroEstabelecimento.ID = "txtNumeroEstabelecimento";
            this.txtNumeroEstabelecimento.CssClass = "ddd";

            this.ddlTipoPessoa = new DropDownList();
            
            this.PopularListaTipoPessoa();
            if (!object.ReferenceEquals(this.ddlTipoPessoa, null))
                this.ddlTipoPessoa.ID = "ddlTipoPessoa";

            this.txtCNPJCPF = new TextBox();
            this.txtCNPJCPF.ID = "txtCNPJCPF";
            this.txtCNPJCPF.CssClass = "ddd";
            this.txtCNPJCPF.MaxLength = 18;

            this.txtRazaoSocial = new TextBox();
            this.txtRazaoSocial.ID = "txtRazaoSocial";
            this.txtRazaoSocial.MaxLength = 255;

            this.txtNome = new TextBox();
            this.txtNome.ID = "txtNome";
            this.txtNome.MaxLength = 255;

            this.txtEmail = new TextBox();
            this.txtEmail.ID = "txtEmail";
            this.txtEmail.MaxLength = 255;

            //this.txtDDD = new TextBox();
            //this.txtDDD.ID = "txtDDD";
            //this.txtDDD.CssClass = "ddd";
            //this.txtDDD.MaxLength = 5;

            this.txtTelefone = new TextBox();
            this.txtTelefone.ID = "txtTelefone";
            this.txtTelefone.CssClass = "tel";
            this.txtTelefone.MaxLength = 15;

            this.PopularListaMotivosContato(ref this.ddlMotivoContato);
            if(!object.ReferenceEquals(this.ddlMotivoContato,null))
            {
                this.ddlMotivoContato.ID = "ddlMotivoContato";
                //this.pnlFormularioCadastro.Controls.Add(this.ddlMotivoContato);
            }

            this.txtMensagem = new TextBox();
            this.txtMensagem.ID = "txtMensagem";
            this.txtMensagem.TextMode = TextBoxMode.MultiLine;
            this.txtMensagem.Rows = 6;

            this.rdbNao = new RadioButton();
            this.rdbNao.ID = "rbdNao";
            this.rdbNao.Text = RedecardHelper.ObterResource("nao");
            this.rdbNao.GroupName = "grpOuvidoria";
            this.rdbNao.Checked = true;
            this.rdbNao.AutoPostBack = true;
            this.rdbNao.CheckedChanged += new EventHandler(this.rdbSimNao_CheckedChanged);
            //this.rdbNao.Attributes.Add("onclick", "Atendimento.Ouvidoria.MostrarFormulario(false);");

            this.rdbSim = new RadioButton();
            this.rdbSim.ID = "rbdSim";
            this.rdbSim.Text = RedecardHelper.ObterResource("sim");
            this.rdbSim.GroupName = "grpOuvidoria";
            this.rdbSim.Checked = !this.rdbNao.Checked;
            this.rdbSim.AutoPostBack = true;
            this.rdbSim.CheckedChanged += new EventHandler(this.rdbSimNao_CheckedChanged);
            //this.rdbSim.Attributes.Add("onclick", "Atendimento.Ouvidoria.MostrarFormulario(true);");

            this.ltlMensagemCaixaAtencao = new Literal();
            this.lnkPaginaFaleConosco = new HyperLink();
            this.lnkPaginaFaleConosco.Text = RedecardHelper.ObterResource("faleconosco");
            this.lnkPaginaFaleConosco.NavigateUrl = Page.ResolveUrl(this.LinkPaginaFaleConosco);

            this.btnEnviar = new Button();
            this.btnEnviar.Text = RedecardHelper.ObterResource("enviar");
            this.btnEnviar.CssClass = "btOk";
            this.btnEnviar.Click += new EventHandler(this.btnEnviar_Click);

            this.ltlMensagem = new Literal();
            this.ltlMensagem.ID = "ltlMensagem";
            #endregion

            #region Div Questão
            #region HTML Div Questão
            //<div class="boxQuestao">
           //    <p><strong>Você já encaminhou sua solicitação para alguma área da Redecard?</strong></p>
           //    <ul>
           //        <li><input type="radio" name="questao" value="sim" />Sim</li>
           //        <li> <input type="radio" name="questao" value="nao" />Não </li>
           //    </ul>
            //</div> 
            #endregion
            pnlBoxQuestao.Controls.Add(new LiteralControl("<p><strong>" + RedecardHelper.ObterResource("formContatoOuvidoria_JaEncaminhouSuaSolicitacaoAlgumaAreaRedecard") + "</strong></p>"));
            pnlBoxQuestao.Controls.Add(new LiteralControl("<ul>"));
                pnlBoxQuestao.Controls.Add(new LiteralControl("<li>"));
                    pnlBoxQuestao.Controls.Add(this.rdbSim);
                pnlBoxQuestao.Controls.Add(new LiteralControl("</li>"));
                pnlBoxQuestao.Controls.Add(new LiteralControl("<li>"));
                    pnlBoxQuestao.Controls.Add(this.rdbNao);
                pnlBoxQuestao.Controls.Add(new LiteralControl("</li>"));
            pnlBoxQuestao.Controls.Add(new LiteralControl("</ul>"));
            #endregion

            #region Div Atenção
            #region HTML Div Atenção
            //<div class="boxAtencao">
            //    <p class="textTitulo">Atenção!</p>
            //    <p>As reclamações que não foram registradas anteriormente nos meios de contato da Central de Atendimento Redecard serão primeiramente encaminhadas ao Fale Conosco e leo in augue ultrices lacinia. Maecenas cursus arcu eu nulla. Curabitur sodales quam in libero. Maecenas porta viverra nibh. Mauris bibendum ante vitae neque aliquam volutpat. </p>
            //    <p><a href="">Fale Conosco</a></p>
            //</div> 
            #endregion
            pnlBoxAtencao.Controls.Add(ltlAberturaFieldSetPadrao.BasicClone());
            pnlBoxAtencao.Controls.Add(new LiteralControl("<p class=\"textTitulo\">" + RedecardHelper.ObterResource("formContatoOuvidoria_Atencao") + "</p>"));
                pnlBoxAtencao.Controls.Add(new LiteralControl("<p>"));
                pnlBoxAtencao.Controls.Add(this.ltlMensagemCaixaAtencao);
                pnlBoxAtencao.Controls.Add(new LiteralControl("</p>"));
                pnlBoxAtencao.Controls.Add(new LiteralControl("<p>"));
                pnlBoxAtencao.Controls.Add(this.lnkPaginaFaleConosco);
                pnlBoxAtencao.Controls.Add(new LiteralControl("</p>"));
                //pnlBoxAtencao.Controls.Add(new LiteralControl("</p>"));
                //pnlBoxAtencao.Controls.Add(new LiteralControl("<p>As reclamações que não foram registradas anteriormente nos meios de contato da Central de Atendimento Redecard serão primeiramente encaminhadas ao Fale Conosco e leo in auge ultrices lacinia. Maecenas cursus arcu eu nulla. Curabitur sodales quam in libero. Maecenas porta viverra nibh. Mauris bibendum ante vitae neque aliquam volutpat.</p>"));
            pnlBoxAtencao.Controls.Add(ltlFechamentoFieldSet);
            #endregion

            #region Formulário
            #region HTML Form
            /* <div class="boxFormParceiros">
                    <form action="#" method="post" name="" class="frmTestimonials" id="formTeste">
            ------->   <fieldset>
                            <p class="textDestaque">Envie-nos uma mensagem</p>
                            <p>Os campos marcados com asterisco (*) são de preenchimento obrigatório.</p>
                            <label class="campUm">Nº do protocolo de atendimento<br /> <input type="text" name="cpf_1" /></label>
                            <label>* Tipo de pessoa:<br /> <select><option>Selecione uma Opção</option><option>Selecione uma Opção</option><option>Selecione uma Opção</option></select></label>                                    <label  class="campDois">* Nº do estabelecimento<br /> <input type="text" name="ddd" class="ddd" /><span>(Somente números)</span></label>
                            <label  class="campDois">* CNPJ / CPF<br /> <input type="text" name="ddd" class="ddd" /><span>(Somente números)</span></label>
                            <label>Razão social: <strong>Lorem ipsum dolor amet</strong><br /><br />* Seu nome<br /><input type="text" name="tel" class="tel" /></label>
                            <label>* Seu e-mail<br /> <input type="text" name="cpf_1" /></label>
                            <label  class="campDois">* Telefone de contato:<br /> <input type="text" name="cpf_1" /><span>(Somente números)</span></label>
                            <label>* Motivo do contato:<br /> <select><option>Selecione uma Opção</option><option>Selecione uma Opção</option><option>Selecione uma Opção</option></select></label>
                            <label class="labelDivisao">* Sua mensagem:<br /> <textarea></textarea></label>  
                            <label><input type="submit" class="btOk" value="ENVIAR" /></label>                                                                 
                        </fieldset>
                    </form>
                </div>
            */
            #endregion
            pnlFormularioCadastro.Controls.Add(ltlAberturaFieldSetPadrao.BasicClone());
                //Campo Nro. Protocolo de Atendimento
                pnlFormularioCadastro.Controls.Add(ltlEnvieNosUmaMensagem);
                pnlFormularioCadastro.Controls.Add(ltlCamposMarcadosAsterisco);
                pnlFormularioCadastro.Controls.Add(new LiteralControl("<label class=\"campUm\">"));
                pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContatoOuvidoria_Campo_NroProtocoloAtendimento")));
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(this.txtNumeroProtocoloAtendimento);
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

                //Campo Tipo de Pessoa
                pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
                pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContatoOuvidoria_Campo_TipoPessoa")));
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(this.ddlTipoPessoa);
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

                //Campo Nro. Estabelecimento
                pnlFormularioCadastro.Controls.Add(new LiteralControl("<label class=\"campDois\">"));
                pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContatoOuvidoria_Campo_NroEstabelecimento")));
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(this.txtNumeroEstabelecimento);
                pnlFormularioCadastro.Controls.Add(new LiteralControl("<span>" + RedecardHelper.ObterResource("formContatoOuvidoria_Campo_SomenteNumeros") + "</span>"));
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

                //Campo CNPJ/CPF
                pnlFormularioCadastro.Controls.Add(new LiteralControl("<label class=\"campDois\">"));
                pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContatoOuvidoria_Campo_CNPJCPF")));
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(this.txtCNPJCPF);
                pnlFormularioCadastro.Controls.Add(new LiteralControl("<span>" + RedecardHelper.ObterResource("formContatoOuvidoria_Campo_SomenteNumeros") + "</span>"));
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());
                
                //Razão Social
                pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
                pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContatoOuvidoria_Campo_RazaoSocial")));
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(this.txtRazaoSocial);
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

                //Campo Nome
                pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
                pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContatoOuvidoria_Campo_Nome")));
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(this.txtNome);
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

                //Campo Email
                pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
                pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContatoOuvidoria_Campo_Email")));
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(this.txtEmail);
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

                //Campo Telefone de Contato
                pnlFormularioCadastro.Controls.Add(new LiteralControl("<label class=\"campDois\">"));
                pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContatoOuvidoria_Campo_Telefone")));
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                //pnlFormularioCadastro.Controls.Add(this.txtDDD);
                pnlFormularioCadastro.Controls.Add(this.txtTelefone);
                pnlFormularioCadastro.Controls.Add(new LiteralControl("<span>" + RedecardHelper.ObterResource("formContatoOuvidoria_Campo_SomenteNumeros") + "</span>"));
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

                //Campo Motivo do Contato
                pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
                pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContatoOuvidoria_Campo_MotivoContato")));
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(this.ddlMotivoContato);
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

                //Campo mensagem
                pnlFormularioCadastro.Controls.Add(new LiteralControl("<label class=\"labelDivisao\">"));
                pnlFormularioCadastro.Controls.Add(new LiteralControl(RedecardHelper.ObterResource("formContatoOuvidoria_Campo_SuaMensagem")));
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(this.txtMensagem);
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

                //Botão enviar
                pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
                pnlFormularioCadastro.Controls.Add(this.btnEnviar);
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

                //Campo para mensagem de operação
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(ltlQuebra.BasicClone());
                pnlFormularioCadastro.Controls.Add(ltlAberturaLabelPadrao.BasicClone());
                pnlFormularioCadastro.Controls.Add(new LiteralControl("<b>"));
                pnlFormularioCadastro.Controls.Add(this.ltlMensagem);
                pnlFormularioCadastro.Controls.Add(new LiteralControl("</b>"));
                pnlFormularioCadastro.Controls.Add(ltlFechamentoLabel.BasicClone());

            pnlFormularioCadastro.Controls.Add(ltlFechamentoFieldSet.BasicClone());
            #endregion

            #region Junção dos blocos HTML
            pnlBoxLeft.Controls.Add(pnlBoxQuestao);
            pnlBoxLeft.Controls.Add(pnlBoxAtencao);
            pnlBoxCadastro.Controls.Add(pnlFormularioCadastro);
            pnlBoxLeft.Controls.Add(pnlBoxCadastro);
            pnlBoxCenter.Controls.Add(pnlBoxLeft);
            this.Controls.Add(pnlBoxCenter);

            //Dispara método de configuração inicial de visibilidade do formulário de contato com ouvidoria
            this.rdbSimNao_CheckedChanged(this, EventArgs.Empty);

            /* HTML Resultante resumido
            ...
            <div class="center">
                ...
                <div class="left">
                    <div class="boxQuestao">...</div>
                    <div class="boxAtencao">...</div>
                    <div class="boxFormParceiros">
                        <div class="frmTestimonials">
                            ...form aqui...
                        </div>
                    </div>
                </div>
            </div>
            ...
            */
            #endregion
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
            allToolParts[2] = new GerenciamentoMotivoContatoOuvidoriaToolPart(new ValidadorMotivoContato());

            return allToolParts;
        }

        /// <summary>
        /// Atribui o conteúdo da caixa de Atenção com base na seguinte regra:
        /// Se a opção selecionada do RadioButton é Sim, então exibe a mensagem de atenção parametrizada através da propriedade da Web Part "MensagemCaixaAtencao";
        /// Caso contrário, exibe a mensagem parametrizada através da propriedade da Web Part "MensagemNaoEncaminhamentoSolicitacaoEmOutraArea" e disponibiliza um link
        /// também parametrizado através da propriedade da Web Part "LinkPaginaFaleConosco"
        /// </summary>
        private void ConfigurarConteudoCaixaAtencao()
        {
            if (this.rdbSim.Checked)
            {
                this.ltlMensagemCaixaAtencao.Text = this.MensagemCaixaAtencao;
                this.lnkPaginaFaleConosco.Visible = false;
            }
            else if (this.rdbNao.Checked)
            {
                this.ltlMensagemCaixaAtencao.Text = this.MensagemNaoEncaminhamentoSolicitacaoEmOutraArea;
                this.lnkPaginaFaleConosco.Visible = true;
            }
            this.ltlMensagemCaixaAtencao.Text = this.MensagemCaixaAtencao;
        }

        /// <summary>
        /// Popula os itens de motivos de contato na lista indicada por parâmetro
        /// </summary>
        /// <param name="controleLista"></param>
        protected void PopularListaTipoPessoa()
        {
            if (object.ReferenceEquals(this.ddlTipoPessoa, null))
                this.ddlTipoPessoa = new DropDownList();

            this.ddlTipoPessoa.Items.Add(RedecardHelper.ObterResource("formContatoOuvidoria_TipoPessoa_Fisica"));
            this.ddlTipoPessoa.Items.Add(RedecardHelper.ObterResource("formContatoOuvidoria_TipoPessoa_Juridica"));
            this.ddlTipoPessoa.Items.Insert(0,new ListItem(WebPartFormularioContatoBase.textoPadraoSelecaoOpcao,WebPartFormularioContatoBase.valorPadraoSelecaoOpcao));
        }

        private string ObterCorpoEmail(Contato contato)
        {
            string campoNaoInformado = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_NaoInformado");
            string campoNroProtocoloAtendimento = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_NroProtocoloAtendimento");
            string campoTipoPessoa = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_TipoPessoa");
            string campoNroEstabelecimento = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_NroEstabelecimento");
            string campoCNPJCPF = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_CPFCNPJ");
            string campoRazaoSocial = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_RazaoSocial");
            string campoNome = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_Nome");
            string campoEmail = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_Email");
            string campoTelefone = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_Telefone");
            string campoMotivoContato = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_MotivoContato");
            string campoMensagem = RedecardHelper.ObterResource("formContatoOuvidoria_CorpoEmail_Mensagem");

            //TODO: TEMPLATE EMAIL OUVIDORIA PENDENTE!
            string email = string.Format("{0} {1}\n"+
                                         "{2} {3}\n"+
                                         "{4} {5}\n"+
                                         "{6} {7}\n"+
                                         "{8} {9}\n"+
                                         "{10} {11}\n"+
                                         "{12} {13}\n"+
                                         "{14} {15}\n"+"{16} {17}\n"+
                                         "{18} {19}",
                                          campoNroProtocoloAtendimento,string.IsNullOrEmpty(contato.NumeroProtocoloAtendimento) ? campoNaoInformado : contato.NumeroProtocoloAtendimento,
                                          campoTipoPessoa,contato.TipoPessoa,
                                          campoNroEstabelecimento,contato.NumeroEstabelecimento,
                                          campoCNPJCPF,contato.CNPJCPF,
                                          campoRazaoSocial,contato.RazaoSocial,
                                          campoNome,contato.Nome,
                                          campoEmail,contato.Email,
                                          campoTelefone,contato.Telefone,
                                          campoMotivoContato,contato.Motivo,
                                          campoMensagem,contato.Mensagem);
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
                EmailUtils.EnviarEmail(null, "rede@userede.com.br", this.ddlMotivoContato.SelectedValue, this.EmailPadraoCopiaTodos, null, FormularioContatoComOuvidoria.textoPadraoCabecalhoEmail, true, this.ObterCorpoEmail(contato));
            else
                EmailUtils.EnviarEmail(null, "rede@userede.com.br", this.EmailPadraoCopiaTodos, null, null, FormularioContatoComOuvidoria.textoPadraoCabecalhoEmail, true, this.ObterCorpoEmail(contato));
        }

        /// <summary>
        /// Retorna um DTO de contato a partir dos campos do formulário
        /// </summary>
        /// <returns></returns>
        private Contato ObterDosDadosDoFormulario()
        {
            return new Contato()
            {
                NumeroProtocoloAtendimento = this.txtNumeroProtocoloAtendimento.Text.Trim(),
                TipoPessoa = this.ddlTipoPessoa.SelectedItem.Text.Trim(),
                NumeroEstabelecimento = this.txtNumeroEstabelecimento.Text.Trim(),
                CNPJCPF = this.txtCNPJCPF.Text.Trim(),
                RazaoSocial = this.txtRazaoSocial.Text.Trim(),
                Nome = this.txtNome.Text.Trim(),
                Email = this.txtEmail.Text.Trim(),
                Telefone = this.txtTelefone.Text.Trim(),
                //DDD = this.txtDDD.Text.Trim(),
                Motivo = new MotivoContato(this.ddlMotivoContato.SelectedItem.Text, this.ddlMotivoContato.SelectedItem.Value),
                Mensagem = this.txtMensagem.Text.Trim()
            };
        }

        /// <summary>
        /// Configura os campos do formulário em um etado inicial
        /// </summary>
        private void LimparCampos()
        {
            this.txtNumeroProtocoloAtendimento.Text =
            this.txtNumeroEstabelecimento.Text =
            this.txtCNPJCPF.Text =
            this.txtNome.Text =
            this.txtEmail.Text =
            this.txtTelefone.Text =
            //this.txtDDD.Text =
            this.txtMensagem.Text = string.Empty;

            this.ddlTipoPessoa.SelectedIndex = -1;
            this.ddlMotivoContato.SelectedIndex = -1;

            //Põe no foco no primeiro campo texto do formulário se form estiver visível
            if(this.pnlBoxCadastro.Visible)
                this.txtNumeroProtocoloAtendimento.Focus();

            //Habilita o botão de envio do form dependendo do estado do formulário
            this.btnEnviar.Enabled = this.pnlBoxCadastro.Visible;
        }
        #endregion
    }
}