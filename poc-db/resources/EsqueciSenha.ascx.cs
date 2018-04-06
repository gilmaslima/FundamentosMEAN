using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login
{
    public partial class EsqueciSenha : UserControl
    {
        public enum TipoRecuperacao
        {
            Senha = 1,
            Usuario = 2,
            Cadastro = 3
        };

        private TipoRecuperacao Recuperacao { get; set; }

        private String Senha { get; set; }
        private String Usuario { get; set; }
        private String NumPDV { get; set; }
        private String NumCNPJ { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Senha = Request.QueryString["senha"].ToString();
            Usuario = Request.QueryString["usuario"].ToString();
            NumPDV = Request.QueryString["txtnupdv"].ToString();
            NumCNPJ = Request.QueryString["txtnucnpj"].ToString();

            if (Senha.Equals("1"))
                Recuperacao = TipoRecuperacao.Senha;// Titulo = "RECUPERAR SENHA"
            else if (Usuario.Equals("1"))
                Recuperacao = TipoRecuperacao.Usuario; // Titulo = "RECUPERAR USUARIO"
            else
                Recuperacao = TipoRecuperacao.Cadastro; //Titulo = "CADASTRE-SE"

            hdnSenha.Value = Senha;
            hdnUsuario.Value = Usuario;
            txtNumPDV.Text = NumPDV;
            txtNumCNPJ.Text = NumCNPJ;
        }

        protected void btnContinuar_Click(object sender, ImageClickEventArgs e)
        {
            string alerta = ValidarTelaMensagem();

            if (alerta.Length > 0)
            {
                HabilitarRecuperacaoSenha();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ScriptContinuar", String.Format("alert('{0}');", alerta), false);
            }
        }

        private string ValidarTelaMensagem()
        {
            String mensagemAlerta = "";

            int numPDV;
            int.TryParse(txtNumPDV.Text, out numPDV);

            if (int.TryParse(txtNumPDV.Text, out numPDV))
                if (numPDV == 0)
                {
                    mensagemAlerta = "Você deve digitar o código do seu estabelecimento. \n";
                }

            int numCNPJ;
            int.TryParse(txtNumPDV.Text, out numCNPJ);

            if (int.TryParse(txtNumPDV.Text, out numCNPJ))
                if (numCNPJ == 0)
                {
                    mensagemAlerta += "Você deve digitar o Número do CNPJ / CPF do estabelecimento.\n";
                }

            if (string.IsNullOrEmpty(txtUsuario.Text))
            {
                mensagemAlerta += "Você deve digitar o usuário.";
            }

            return mensagemAlerta;
        }

        private void HabilitarRecuperacaoSenha()
        {
            pnlEsqueciSenha.Visible = false;
            pnlCadEstabelecimento.Visible = true;
        }
    }
}
