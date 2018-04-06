using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.Portal.Helper.Web.Mails;
using System.Text;

namespace Redecard.Portal.Aberto.WebParts.FormularioImprensa
{
    public partial class FormularioImprensaUserControl : UserControl
    {

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private FormularioImprensa WebPart
        {
            get
            {
                return this.Parent as FormularioImprensa;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private String De
        {
            get
            {
                return this.WebPart.De;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private String Para
        {
            get
            {
                return this.WebPart.Para;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private String Assunto
        {
            get
            {
                return this.WebPart.Assunto;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                trMensagem.Visible = false;
        }


        /// <summary>
        /// Cria mensagem que será enviada no e-mail
        /// </summary>
        public String CriarMensagem()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Olá<br><br>");
            sb.Append("O jornalista abaixo acessou o Portal userede.com.br e se cadastrou para receber notícias sobre a Rede.<br>");
            sb.Append("Veja abaixo os dados de cadastro:<br><br>");
            sb.Append("Nome do veículo:&nbsp;");
            sb.Append(txtNomeVeiculo.Text);
            sb.Append("<br>Nome do jornalista:&nbsp;");
            sb.Append(txtSeuNome.Text);
            sb.Append("<br>E-mail:&nbsp;");
            sb.Append(txtEmail.Text);
            sb.Append("<br>Telefone:&nbsp;(");
            sb.Append(txtDDD.Text);
            sb.Append(") ");
            sb.Append(txtTelefone.Text);
            sb.Append("<br>Pauta:&nbsp;");
            sb.Append(txtPauta.Text);

            return sb.ToString();
        }

        protected void Salvar(object sender, EventArgs e)
        {
            EmailUtils.EnviarEmail(null,
                this.De,
                this.Para,
                "",
                "",
                this.Assunto,
                true,
                this.CriarMensagem());

            trMensagem.Visible = true;
        }
    }
}
