using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login
{
    public partial class EsqueciUsuario : UserControl
    {
        public enum TipoRecuperacao
        {
            Senha = 1,
            Usuario = 2,
            Cadastro = 3
        };

        private TipoRecuperacao Recuperacao{get; set;}

        private String Senha   { get; set;}
        private String Usuario { get; set;}
        private String NumPDV  { get; set;}
        private String NumCNPJ { get; set;}

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

            if (Senha.Equals("") && Usuario.Equals(""))
            {
                lblSenha.Visible = false;
                txtSenha.Visible = false;
            }

            hdnSenha.Value = Senha;
            hdnUsuario.Value = Usuario;
            txtNumPDV.Text = NumPDV;
            txtNumCNPJ.Text = NumCNPJ;
                        
            //<if strDolar = "N" then %>
            //<%=Dados_Bancarios()%>
            //<%elseif strDolar = "S" then%>
            //<%=Dados_Pessoais()%>
            //<%end if%>

        }

        protected void btnContinuar_Click(object sender, ImageClickEventArgs e)
        {
            String alerta = ValidarTelaMensagem();
	
	        if (alerta.Length > 0 )
            {
                //HabilitarRecuperacaoUsuario
            }
            else
            {
                ScriptManager.RegisterStartupScript(this,this.GetType(), "ScriptContinuar", String.Format("alert('{0}');", alerta),false);
            }
	    }

        private String ValidarTelaMensagem()
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

            if (String.IsNullOrEmpty(Senha.Trim()) && String.IsNullOrEmpty(Usuario.Trim()))
            {
                if (String.IsNullOrEmpty(txtSenha.Text))
                    mensagemAlerta += "Você deve digitar a senha. \n";
            }

            return mensagemAlerta;
        }

        private void HabilitarRecuperacaoSenha()
        {
            pnlEsqueciUsuario.Visible = false;
            pnlCadEstabelecimento.Visible = true;
        }
    }
}
