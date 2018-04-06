using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;

namespace Redecard.Portal.Aberto.SD.Helpers {

    /// <summary>
    /// Página de apresentação do nome do servidor
    /// </summary>
    public class emailHelper : UnsecuredLayoutsPageBase {

        /// <summary>
        /// 
        /// </summary>
        protected Literal ltErrorDescription;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox txtServer;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox txtPort;

        /// <summary>
        /// 
        /// </summary>
        protected CheckBox chkAuth;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox txtDomain;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox txtUser;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox txtPassword;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox txtFrom;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox txtTo;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox txtBodyEmail;

        /// <summary>
        /// 
        /// </summary>
        protected Button btnSendMail;

        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlError;

        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlSuccess;

        /// <summary>
        /// 
        /// </summary>
        protected void Page_Load(object sender, EventArgs e) {
            // esconder os dois painéis
            pnlError.Visible = false;
            pnlSuccess.Visible = false;
        }

        /// <summary>
        /// Permitir acesso anônimo, essa opção deve ser revista, uma vez que não podemos manter essa
        /// tela aberta ao público anonimo
        /// </summary>
        protected override bool AllowAnonymousAccess {
            get {
                return true;
            }
        }

        /// <summary>
        /// Efetuar o disparo do e-mail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSendMail_Click(object sender, EventArgs e) {
            if (this.EnviarEmail())
                pnlSuccess.Visible = true;
            else
                pnlError.Visible = true;
        }

        /// <summary>
        /// Enviar o e-mail de acordo com as configurações da tela.
        /// OBS****
        /// Já temos um método de disparo no projeto Redecard.Helper, porém, decidimos
        /// usar um novo método para especificar manualmente as configurações de SMTP
        /// </summary>
        private bool EnviarEmail() {
            MailMessage message = new MailMessage();
            message.To.Add(txtTo.Text);
            message.Subject = "Teste de Envio de E-mail";
            message.Body = txtBodyEmail.Text;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            message.From = new MailAddress(txtFrom.Text);
            try {
                SmtpClient client = new SmtpClient();
                if (chkAuth.Checked) {
                    client.Credentials = new NetworkCredential(txtUser.Text,
                        txtPassword.Text, txtDomain.Text);
                }
                client.Send(message);
                return true;
            }
            catch (Exception e) {
                this.CompileError(e);
                return false;
            }
        }

        /// <summary>
        /// Executa o tratamento de Erro da Tela
        /// </summary>
        private void CompileError(Exception e) {
            ltErrorDescription.Text = String.Format("Ocorreu um erro do tipo {0} com a seguinte definição '{1}'. Stack Trace={2}",
                e.GetType().ToString(), e.Message, e.StackTrace);
        }
    }
}