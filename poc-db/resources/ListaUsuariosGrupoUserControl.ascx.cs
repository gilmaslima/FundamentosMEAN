using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.ServiceModel;
using System.Linq;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.ListaUsuariosGrupo
{

    /// <summary>
    /// Classe de listagem de usuários de uma entidade e exclusão em lote
    /// </summary>
    public partial class ListaUsuariosGrupoUserControl : UserControlBase
    {
        /// <summary>
        /// Lista de usuários (IDs) para exclusão
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

        /// <summary>
        /// Carregamento da Página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Verifica se o usuário é MASTER
                if (!object.ReferenceEquals(base.SessaoAtual, null) && base.SessaoAtual.UsuarioMaster())
                {
                    lblMensagem.Visible = false;
                    this.CarregarUsuarios();
                }
                else
                {
                    this.SetarAviso("Você não possui permissão para acessar este serviço.<br/><br/>Por favor, entre em contato com o seu usuário Master");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetarAviso(String aviso)
        {
            lblMensagem.Text = aviso;
            lblMensagem.Visible = true;
            pnlErro.Visible = true;
            pnlTabelaUsuarios.Visible = false;
        }

        /// <summary>
        /// Carrega usuários no Repeater
        /// </summary>
        private void CarregarUsuarios()
        {
            using (Logger Log = Logger.IniciarLog("Cadastro de Usuários no Portal"))
            {
                try
                {
                    // Chamada ao serviço
                    using (var usuarioServico = new UsuarioServico.UsuarioServicoClient())
                    {
                        // Variável para recuperar o código de retorno
                        Int32 codigoRetorno;

                        // Recupera os usuários da entidade logada
                        var usuarios = usuarioServico.ConsultarPorEntidade(out codigoRetorno, this.ObtemEntidade());

                        // Caso o código de retorno seja <> de 0 ocorreu um erro
                        if (codigoRetorno > 0)
                            base.ExibirPainelExcecao("UsuarioServico.ConsultarPorEntidade", codigoRetorno);
                        else
                        {
                            // Bind no Repeater
                            rptUsuarios.DataSource = usuarios.Where(u => String.Compare("Operacional", u.Codigo, true) != 0).ToList();
                            rptUsuarios.DataBind();
                            CarregarPaginacao();
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
        /// Obtem entidade objeto entidade do usuário logado
        /// </summary>
        /// <returns>Objeto entidade preechido</returns>
        private UsuarioServico.Entidade ObtemEntidade()
        {
            // Cria Entidade
            var entidade = new UsuarioServico.Entidade();
            entidade.Codigo = base.SessaoAtual.CodigoEntidade;

            // Cria Grupo da Entidade
            var grupoEntidade = new UsuarioServico.GrupoEntidade();
            grupoEntidade.Codigo = base.SessaoAtual.GrupoEntidade;  // Grupo Entidade
            entidade.GrupoEntidade = grupoEntidade;

            return entidade;
        }

        /// <summary>
        /// Apresenta as informações no repetar.
        /// </summary>
        protected void rptUsuarios_OnItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Recupera objetos da linha
                var chkUsr = (CheckBox)e.Item.FindControl("chkUsr");
                var lblUsuario = (Label)e.Item.FindControl("lblUsuarioDescricao");
                var lnkUsuario = (HyperLink)e.Item.FindControl("lnkUsuarioDescricao");
                var lblTipoUsuario = (Label)e.Item.FindControl("lblTipoUsuario");

                // Converte item no objeto Usuario
                UsuarioServico.Usuario usuario = (UsuarioServico.Usuario)e.Item.DataItem;

                // Atribui o código do usuário a CheckBox
                chkUsr.Attributes.Add("Codigo", usuario.CodigoIdUsuario.ToString());
                chkUsr.Attributes.Add("Nome", usuario.Codigo);

                // Caso o usuário logado é o item listado o mesmo não poderá ser editado
                if (base.SessaoAtual.LoginUsuario.ToLower().Contains(usuario.Codigo.ToLowerInvariant()))
                    //base.web.CurrentUser.LoginName.ToLowerInvariant().Contains(usuario.Codigo)) // ||
                //usuario.TipoUsuario.ToLowerInvariant() == "m") // (Não)Agora é possível editar perfil de um usuário master(Alteração Ana Infante)
                { 
                    chkUsr.Visible = false;
                    lnkUsuario.Visible = false;

                    lblUsuario.Text = usuario.Codigo;
                    lblUsuario.Visible = true;
                }
                else
                {   
                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString["CodigoUsuario"] = usuario.Codigo;

                    lnkUsuario.NavigateUrl = string.Format(base.web.ServerRelativeUrl + "/Paginas/pn_EditarUsuario.aspx?dados={0}", queryString.ToString());
                    lnkUsuario.Text = usuario.Codigo;

                    lblUsuario.Visible = false;
                }

                lblTipoUsuario.Text = usuario.TipoUsuario == "M" ? "Master" : "Padrão";
            }
            else
            {
                ((Control)e.Item.FindControl("row")).Visible = false;
            }
        }

        /// <summary>
        /// Exclui os usuários selecionados
        /// </summary>
        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Exclusão de usuários selecionados"))
            {
                try
                {
                    QuadroConfirmacao quadro = (QuadroConfirmacao)qdConfirmacao;
                    quadro.Continuar += new QuadroConfirmacao.ContinuarHandle(Continuar);
                    quadro.Voltar += new QuadroConfirmacao.VoltarHandle(Voltar);

                    String usuariosExcluir = "";
                    String usuariosMensagem = "";

                    foreach (RepeaterItem item in rptUsuarios.Items)
                    {
                        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        {
                            CheckBox chk = (CheckBox)item.FindControl("chkUsr");
                            if (!object.ReferenceEquals(chk, null))
                            {
                                if (chk.Checked)
                                {
                                    usuariosMensagem += chk.Attributes["Nome"] + "<br />";
                                    usuariosExcluir += chk.Attributes["Codigo"] + "|";
                                }
                            }
                        }
                    }
                    //Guarda no ViewState os usuários que serão excluídos
                    UsuariosExcluir = usuariosExcluir;

                    pnlTabelaUsuarios.Visible = false;
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
        /// Redireciona para tela de inclusão de usuário
        /// </summary>
        protected void btnIncluir_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_EditarUsuario.aspx");
        }

        /// <summary>
        /// Carrega a função para paginação da tabela de usuários
        /// </summary>
        protected void CarregarPaginacao()
        {
            //Adiciona a função de JS de paginação do grid para as funções a serem executadas após renderização
            //pageResultTable(idTabela, idDivPai, pageIndex, pageSize, pagesPerView)
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tblUsuarios', 1, 10, 5);", true);
        }

        /// <summary>
        /// Continuar Ação de Exclusão
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void Continuar(object sender, EventArgs args)
        {
            using (Logger Log = Logger.IniciarLog("Continuar ação de exclusão"))
            {
                try
                {
                    String usuariosSelecionados = "";

                    // Recupera os códigos dos usuários selecionados e concatena com o separador "|"
                    // Retira o último sparador
                    usuariosSelecionados = UsuariosExcluir.Substring(0, UsuariosExcluir.Length - 1);

                    //var entidadeSessao = ObtemEntidade();

                    // Chama serviço de exclusão de usuário em lote
                    using (var usuarioServico = new UsuarioServico.UsuarioServicoClient())
                    {
                        //Após a entrada do Novo Acesso, a exclusão passa a ser feita pelo ID
                        //Int32 retorno = usuarioServico.ExcluirEmLote(usuariosSelecionados, entidadeSessao.Codigo, entidadeSessao.GrupoEntidade.Codigo);
                        Int32 retorno = usuarioServico.ExcluirEmLoteNovoAcesso(usuariosSelecionados);

                        Panel[] paineis = new Panel[2]{
                            pnlTabelaUsuarios,
                            pnlErro
                    };

                        // Verifica código do indicador proveniente da procedure executada
                        // Caso seja maior que zero ocorreu um erro de negócio
                        if (retorno > 0)
                            base.ExibirPainelExcecao("UsuarioServico.ExcluirEmLote", retorno);
                        else
                            base.ExibirPainelConfirmacaoAcao("Cadastro do Portal", "Usuário(s) excluído(s) com sucesso.", SPUtility.GetPageUrlPath(HttpContext.Current), paineis);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    pnlTabelaUsuarios.Visible = true;
                    pnlConfirmacao.Visible = false;
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    pnlTabelaUsuarios.Visible = true;
                    pnlConfirmacao.Visible = false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    pnlTabelaUsuarios.Visible = true;
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
                Logger.GravarErro("Cancelando ação de exclusão de usuários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }
    }
}
