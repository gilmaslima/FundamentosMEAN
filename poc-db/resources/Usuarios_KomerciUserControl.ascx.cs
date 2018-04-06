using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
//using Redecard.PN.DadosCadastrais.Modelo.Security;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Linq;

namespace Redecard.PN.OutrasEntidades.SharePoint.WebParts.Komerci.Usuarios_Komerci
{
    public partial class Usuarios_KomerciUserControl : UserControlBase
    {
        protected CampoNovoAcesso txtSenha {
            get {
                return (CampoNovoAcesso)txtSenhaNova;
            }
        }

        protected CampoNovoAcesso txtSenhaConf {
            get {
                return (CampoNovoAcesso)txtSenhaNovaConf;
            }
        }

        protected CampoNovoAcesso txtSenhaEditar {
            get {
                return (CampoNovoAcesso)txtSenhaNovaEditar;
            }
        }

        protected CampoNovoAcesso txtSenhaEditarConf {
            get {
                return (CampoNovoAcesso)txtSenhaNovaEditarConf;
            }
        }

        /// <summary>
        /// Lista de usuários para exclusão
        /// </summary>
        private String UsuariosExcluir
        {
            get
            {
                if (ViewState["UsuariosExcluir"] != null)
                    return ViewState["UsuariosExcluir"].ToString();
                else
                    return "";
            }
            set
            {
                ViewState["UsuariosExcluir"] = value;
            }
        }

        private UsuarioServico.UsuarioKomerci UsuarioEditar { get; set; }

        /// <summary>
        /// Inicialização da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                HabilitarListagem();
        }

        /// <summary>
        /// Exclusão em lote dos usuários do Komerci selecionados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Exclusão em Lote de Usuários Komerci"))
            {
                try
                {
                    QuadroConfirmacao quadro = (QuadroConfirmacao)qdConfirmacao;
                    quadro.Continuar += new QuadroConfirmacao.ContinuarHandle(Continuar);
                    quadro.Voltar += new QuadroConfirmacao.VoltarHandle(Voltar);

                    String usuariosExcluir = "";
                    String usuariosMensagem = "";

                    UsuarioServico.UsuarioKomerci usuario = new UsuarioServico.UsuarioKomerci();

                    foreach (RepeaterItem item in rptUsuarios.Items)
                    {
                        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        {
                            CheckBox chk = (CheckBox)item.FindControl("chkExcluir");
                            if (!object.ReferenceEquals(chk, null))
                            {
                                if (chk.Checked)
                                {
                                    LinkButton lnkUsuario = (LinkButton)item.FindControl("btnUsuario");

                                    usuariosMensagem += lnkUsuario.Text + "<br />";
                                    usuariosExcluir += lnkUsuario.Text + "|";
                                }
                            }
                        }
                    }
                    //Guarda no ViewState os usuários que serão excluídos
                    UsuariosExcluir = usuariosExcluir;

                    pnlListarUsuario.Visible = false;
                    pnlCadastrarUsuario.Visible = false;
                    pnlEditarUsuario.Visible = false;
                    pnlConfirmacao.Visible = true;

                    //Você está excluindo o(s) usuário(s):\n" + usuarios + "\nDeseja continuar a operação?
                    quadro.TituloPainel = "Confirmação de Exclusão de Usuários";
                    quadro.MensagemPainel = String.Format("Você está excluindo o(s) usuário(s):<br />{0}<br />Deseja continuar a operação?", usuariosMensagem);
                    quadro.CarregarMensagemQuadro();
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Habilitar tela para inclusão
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnIncluir_Click(object sender, EventArgs e)
        {
            HabilitarCadastro();
        }

        /// <summary>
        /// Enviar o cadastro do Usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviarCadastro_Click(object sender, EventArgs e)
        {
            SalvarUsuario(true);
        }

        /// <summary>
        /// Voltar para listagem de Usuários
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltarCadastro_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format(base.web.ServerRelativeUrl + "/Paginas/pn_Usuarios_Komerci.aspx"));
        }

        /// <summary>
        /// Enviar a alteração do usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviarAlteracao_Click(object sender, EventArgs e)
        {
            SalvarUsuario(false);
        }

        /// <summary>
        /// Excluir o usuário em tela de edição
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcluirUsuario_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Exclusão de Usuário em Edição"))
            {
                try
                {
                    // Chama serviço de exclusão de usuário em lote
                    using (var servicoCliente = new UsuarioServico.UsuarioServicoClient())
                    {
                        Panel[] paineis = new Panel[3]{
                        pnlListarUsuario,
                        pnlEditarUsuario,
                        pnlCadastrarUsuario
                    };

                        Int32 retorno = servicoCliente.ExcluirKomerciEmLoteEC(txtUsuarioAlterar.Text.Trim(), this.ObtemEntidade(), SessaoAtual.LoginUsuario);

                        if (retorno > 0)
                            base.ExibirPainelExcecao("UsuarioServico.ExcluirKomerciEmLoteEC", retorno);
                        else
                            base.ExibirPainelConfirmacaoAcao("Confirmação exclusão", "Usuário excluído com sucesso.", SPUtility.GetPageUrlPath(HttpContext.Current), paineis);
                        //base.Alert("Usuário excluído com sucesso.", SPUtility.GetPageUrlPath(HttpContext.Current));
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Voltar para a tela de listagem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltarAlteracao_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format(base.web.ServerRelativeUrl + "/Paginas/pn_Usuarios_Komerci.aspx"));
        }

        /// <summary>
        /// Habilita a tela de listagem
        /// </summary>
        private void HabilitarListagem()
        {
            pnlCadastrarUsuario.Visible = false;
            pnlEditarUsuario.Visible = false;
            pnlListarUsuario.Visible = true;

            CarregarUsuarios();
        }

        /// <summary>
        /// Carrega a listagem de usuários
        /// </summary>
        private void CarregarUsuarios()
        {
            using (Logger Log = Logger.IniciarLog("Carregar Usuários"))
            {
                try
                {
                    Int32 codigoRetorno;

                    // Chamada ao serviço
                    using (var usuarioKomerciServico = new UsuarioServico.UsuarioServicoClient())
                    {
                        // Recupera os usuários da entidade logada
                        var usuarios = usuarioKomerciServico.ConsultarKomerciPorEntidadeEC(out codigoRetorno, this.ObtemEntidade());

                        //Verifica se retornou erro, e descarta erro 2 porque não é consulta de um usuário, é lista
                        if (codigoRetorno > 0 && codigoRetorno != 2)
                        {
                            base.ExibirPainelExcecao("UsuarioServico.ConsultarKomerciPorEntidadeEC", codigoRetorno);
                            rptUsuarios.DataSource = usuarios;
                            rptUsuarios.DataBind();
                            rptUsuarios.Visible = false;
                        }
                        else
                        {
                            if (usuarios.Length > 0)
                            {
                                rptUsuarios.DataSource = usuarios;
                                rptUsuarios.DataBind();
                                rptUsuarios.Visible = true;
                                pnlSemUsuarios.Visible = false;
                            }
                            else
                            {
                                rptUsuarios.Visible = false;
                                pnlSemUsuarios.Visible = true;
                                QuadroAviso aviso = (QuadroAviso)qdAvisoSemUsuarios;
                                aviso.ClasseImagem = "icone-aviso";
                                aviso.CarregarMensagem("Aviso", "Nenhum usuário cadastrado.");
                            }
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Habilita a tela de Cadastro
        /// </summary>
        private void HabilitarCadastro()
        {
            pnlCadastrarUsuario.Visible = true;
            pnlListarUsuario.Visible = false;
            pnlEditarUsuario.Visible = false;

            CarregarCadastro();
        }

        /// <summary>
        /// Configura a tela de Cadastro
        /// </summary>
        private void CarregarCadastro()
        {
            txtNome.Text = "";
            txtUsuario.Text = "";
            txtSenha.Text = "";
            txtSenhaConf.Text = "";
        }

        /// <summary>
        /// Habilita a tela de Edição
        /// </summary>
        private void HabilitarEdicao()
        {
            pnlCadastrarUsuario.Visible = false;
            pnlListarUsuario.Visible = false;
            pnlEditarUsuario.Visible = true;

            CarregarEdicao();
        }

        /// <summary>
        /// Configura a tela de Cadastro
        /// </summary>
        private void CarregarEdicao()
        {

            txtNomeAlterar.Text = UsuarioEditar.Descricao;
            txtUsuarioAlterar.Text = UsuarioEditar.Codigo;
            txtUsuarioAlterar.Enabled = false;
            txtSenhaEditar.Text = "";
            txtSenhaEditar.Enabled = false;
            txtSenhaEditarConf.Text = "";
            txtSenhaEditarConf.Enabled = false;

            HabilitarAlteracaoSenha();

        }

        /// <summary>
        /// Entidade do Usuário logado
        /// </summary>
        /// <returns></returns>
        private UsuarioServico.Entidade ObtemEntidade()
        {
            // Cria Entidade
            var entidade = new UsuarioServico.Entidade();
            entidade.Codigo = SessaoAtual.CodigoEntidade;

            // Cria Grupo da Entidade
            var grupoEntidade = new UsuarioServico.GrupoEntidade();
            grupoEntidade.Codigo = SessaoAtual.GrupoEntidade;  // Grupo Entidade
            entidade.GrupoEntidade = grupoEntidade;

            return entidade;
        }

        /// <summary>
        /// Salvar as alterações do usuário
        /// </summary>
        /// <param name="gravar">Gravar ou Atualizar o Usuário. True para incluir; False para atualizar</param>
        private void SalvarUsuario(bool gravar)
        {
            using (Logger Log = Logger.IniciarLog("Salvar alterações do Usuário"))
            {
                try
                {
                    Int32 codigoRetorno;

                    if (gravar)
                    {
                        String _senha = null;

                        if (!String.IsNullOrEmpty(txtSenha.Text) && !String.IsNullOrEmpty(txtSenhaConf.Text))
                            _senha = Crypto(txtSenha.Text);//EncriptadorSHA1.EncryptString(txtSenha.Text);
                        else
                        {
                            base.ExibirPainelExcecao("SharePoint.ProdutoDistribuicao", 1358);
                            //base.Alert("A senha deve conter, no mínimo, seis caracteres, e, no máximo, vinte caracteres.", false);
                            return;
                        }

                        UsuarioServico.UsuarioKomerci usuario = new UsuarioServico.UsuarioKomerci();
                        usuario.Codigo = txtUsuario.Text.Trim();
                        usuario.Descricao = txtNome.Text.Trim();
                        usuario.Entidade = ObtemEntidade();
                        usuario.NomeResponsavel = SessaoAtual.LoginUsuario; //SessaoAtual.NomeUsuario;
                        usuario.TipoUsuario = "M";
                        usuario.Senha = _senha;

                        // Chamar servico de inserção
                        using (var servicoCliente = new UsuarioServico.UsuarioServicoClient())
                        {
                            Panel[] paineis = new Panel[3]{
                            pnlListarUsuario,
                            pnlEditarUsuario,
                            pnlCadastrarUsuario
                        };

                            codigoRetorno = servicoCliente.InserirKomerciEC(usuario);

                            if (codigoRetorno > 0)
                                base.ExibirPainelExcecao("UsuarioServico.InserirKomerciEC", codigoRetorno);
                            else
                                base.ExibirPainelConfirmacaoAcao("Confirmação inclusão", "Usuário incluído com sucesso.", SPUtility.GetPageUrlPath(HttpContext.Current), paineis);
                            //base.Alert("Usuário incluído com sucesso.", SPUtility.GetPageUrlPath(HttpContext.Current));
                        }
                    }
                    else
                    {
                        using (var servicoCliente = new UsuarioServico.UsuarioServicoClient())
                        {
                            //Busca o usuário que está sendo editado para atualizar suas informações
							UsuarioEditar = servicoCliente.ConsultarKomerciPorCodigoEntidadeEC(out codigoRetorno, txtUsuarioAlterar.Text.Trim(), this.ObtemEntidade())[0];

                            // Caso o código de retorno seja <> de 0 ocorreu um erro
                            if (codigoRetorno > 0)
								base.ExibirPainelExcecao("UsuarioServico.ConsultarKomerciPorCodigoEntidadeEC", codigoRetorno);
                            else
                            {
                                string _senha = null;
                                if (chkAlterarSenha.Checked)
                                    if (!String.IsNullOrEmpty(txtSenhaEditar.Text) && !String.IsNullOrEmpty(txtSenhaEditarConf.Text))
                                        _senha = Crypto(txtSenhaEditar.Text);//EncriptadorSHA1.EncryptString(txtSenha.Text);
                                    else
                                        _senha = UsuarioEditar.Senha;
                                else
                                    _senha = UsuarioEditar.Senha;

                                UsuarioServico.UsuarioKomerci usuario = UsuarioEditar;
                                usuario.Descricao = txtNomeAlterar.Text;
                                usuario.Entidade = ObtemEntidade();
                                usuario.NomeResponsavel = SessaoAtual.LoginUsuario; //SessaoAtual.NomeUsuario;
                                usuario.Senha = _senha;

                                Panel[] paineis = new Panel[3]{
                                pnlListarUsuario,
                                pnlEditarUsuario,
                                pnlCadastrarUsuario
                            };

                                // Chamar servico de inserção
                                codigoRetorno = servicoCliente.AtualizarKomerciEC(usuario);

                                if (codigoRetorno > 0)
                                    base.ExibirPainelExcecao("UsuarioServico.AtualizarKomerciEC", codigoRetorno);
                                else
                                    base.ExibirPainelConfirmacaoAcao("Confirmação atualização", "Usuário atualizado com sucesso.", SPUtility.GetPageUrlPath(HttpContext.Current), paineis);
                                //base.Alert("Usuário alterado com sucesso.", SPUtility.GetPageUrlPath(HttpContext.Current));
                            }
                        }
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Habilita os campos de senha para alteração
        /// </summary>
        private void HabilitarAlteracaoSenha()
        {
            txtSenhaEditar.Enabled = txtSenhaEditarConf.Enabled = chkAlterarSenha.Checked;
        }

        /// <summary>
        /// Implementa função server para validação de caracteres repetidos na senha
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ctvCaracterRepetido_ServerValidate(object source, ServerValidateEventArgs args)
        {
            String senha = args.Value;
            Char caracter1 = args.Value.ToString()[0];
            Char caracter2 = args.Value.ToString()[1];

            for (int i = 2; i < args.Value.Length; i++)
            {
                if (senha[i].Equals(caracter1) && senha[i].Equals(caracter2))
                {
                    args.IsValid = false;
                    return;
                }
                else
                {
                    caracter1 = senha[i - 2];
                    caracter2 = senha[i - 1];
                }
            }
            args.IsValid = true;
        }

        /// <summary>
        /// Implementa função server para validação de caracteres repetidos na senha
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ctvCarecterRepetidoAlterar_ServerValidate(object source, ServerValidateEventArgs args)
        {
            String senha = args.Value;
            Char caracter1 = args.Value.ToString()[0];
            Char caracter2 = args.Value.ToString()[1];

            for (int i = 2; i < args.Value.Length; i++)
            {
                if (senha[i].Equals(caracter1) && senha[i].Equals(caracter2))
                {
                    args.IsValid = false;
                    return;
                }
                else
                {
                    caracter1 = senha[i - 2];
                    caracter2 = senha[i - 1];
                }
            }
            args.IsValid = true;
        }
        /// <summary>
        /// Preencher as linhas do grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptUsuarios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RepeaterItem item = e.Item;

                UsuarioServico.UsuarioKomerci usuario = (UsuarioServico.UsuarioKomerci)e.Item.DataItem;

                LinkButton btnUsuario = (LinkButton)item.FindControl("btnUsuario");
                if (btnUsuario != null)
                {
                    btnUsuario.CommandArgument = usuario.Codigo.ToString();
                    btnUsuario.Text = usuario.Codigo;
                }

                LinkButton btnNomeUsuario = (LinkButton)item.FindControl("btnNomeUsuario");
                if (btnNomeUsuario != null)
                {
                    btnNomeUsuario.CommandArgument = usuario.Codigo.ToString();
                    btnNomeUsuario.Text = usuario.Descricao;
                }
            }
        }

        /// <summary>
        /// Direciona para edição do Usuário selecionado
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void rptUsuarios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "LinkUsuario")
            {
                Int32 codigoRetorno;
                using (var servicoCliente = new UsuarioServico.UsuarioServicoClient())
                {
					UsuarioEditar = servicoCliente.ConsultarKomerciPorCodigoEntidadeEC(out codigoRetorno, e.CommandArgument.ToString(), this.ObtemEntidade())[0];

                    if (codigoRetorno > 0)
						base.ExibirPainelExcecao("UsuarioServico.ConsultarKomerciPorCodigoEntidadeEC", codigoRetorno);
                    else
                        HabilitarEdicao();
                }
            }
        }

        /// <summary>
        /// Continuar Ação de Exclusão
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void Continuar(object sender, EventArgs args)
        {
            using (Logger Log = Logger.IniciarLog("Continuar ação de Exclusão"))
            {
                try
                {
                    String usuariosSelecionados = "";

                    // Recupera os códigos dos usuários selecionados e concatena com o separador "|"
                    // Retira o último sparador
                    usuariosSelecionados = UsuariosExcluir.Substring(0, UsuariosExcluir.Length - 1);

                    // Chama serviço de exclusão de usuário em lote
                    using (var servicoCliente = new UsuarioServico.UsuarioServicoClient())
                    {
                        Int32 codigoRetorno = servicoCliente.ExcluirKomerciEmLoteEC(usuariosSelecionados, this.ObtemEntidade(), SessaoAtual.LoginUsuario);

                        Panel[] paineis = new Panel[3]{
                        pnlListarUsuario,
                        pnlEditarUsuario,
                        pnlCadastrarUsuario
                    };

                        if (codigoRetorno > 0)
                            base.ExibirPainelExcecao("UsuarioServico.ExcluirKomerciEmLoteEC", codigoRetorno);
                        else
                            base.ExibirPainelConfirmacaoAcao("Confirmação exclusão", "Usuário(s) excluído(s) com sucesso.", SPUtility.GetPageUrlPath(HttpContext.Current), paineis);
                        //base.Alert("Usuário(s) excluído(s) com sucesso.", SPUtility.GetPageUrlPath(HttpContext.Current));
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    pnlListarUsuario.Visible = true;
                    pnlEditarUsuario.Visible = false;
                    pnlCadastrarUsuario.Visible = false;
                    pnlConfirmacao.Visible = false;
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    pnlListarUsuario.Visible = true;
                    pnlEditarUsuario.Visible = false;
                    pnlCadastrarUsuario.Visible = false;
                    pnlConfirmacao.Visible = false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    pnlListarUsuario.Visible = true;
                    pnlEditarUsuario.Visible = false;
                    pnlCadastrarUsuario.Visible = false;
                    pnlConfirmacao.Visible = false;
                }
            }
        }

        /// <summary>
        /// Cancela a ação de exclusão de usuários
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void Voltar(object sender, EventArgs args)
        {
            try
            {
                Response.Redirect(SPUtility.GetPageUrlPath(HttpContext.Current));
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante cancelamento de exclusão de usuários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Método de criptografia de Senha do Komerci
        /// </summary>
        /// <param name="strTexto">senha a ser criptografado</param>
        /// <returns></returns>
        public String Crypto(String strTexto)
        {
            String senha; //Dim senha As String
            Int32 v1, v2; //Dim v1, v2 As Integer
            Double r1; //Dim r1 As Double
            Byte i; //Dim i As Byte
            v1 = strTexto.Length;//v1 = Len(strTexto)
            senha = "NOPQRSTUVW" + strTexto.Trim().ToUpper(); //senha = Right("NOPQRSTUVW" + Trim(UCase(strTexto)), 10)
            senha = senha.Substring(senha.Length - 10, 10);
            v2 = 0;//v2 = 0

            for (i = 0; i <= senha.Length - 1; i++) //For i = 1 To Len(senha)
            {
                //v2 = v2 + (Asc(CStr(Mid(senha, i, 1))) * i)
                v2 = v2 + ((int)Convert.ToChar(senha.Substring(i, 1)) * (i + 1));
            }
            //r1 = Fix(((v2 / 10) * v1) * Asc(Right(senha, 1)))
            r1 = Math.Floor(Convert.ToDouble((((v2 / 10m) * v1) * (int)Convert.ToChar(senha.Substring(senha.Length - 1, 1)))));


            int tamret = r1.ToString().Length;
            return tamret <= 20 ? r1.ToString() : r1.ToString().Substring(0, 20);// Substring(0, r1.ToString().Length); //Crypto = Mid(CStr(r1), 1, 20)
        }

    }
}
