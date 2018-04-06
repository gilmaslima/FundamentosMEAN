using System;
using System.Linq;
using System.Web.UI;
using Redecard.PN.Comum;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Microsoft.SharePoint;
using System.ServiceModel;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Permissoes
{

    /// <summary>
    /// Web Part de permissões do usuário do Portal de Serviços, permite o cadastramento e a 
    /// alteração de permissões para um usuário do Portal Redecard.
    /// </summary>
    public partial class PermissoesUserControl : UserControlBase
    {

        /// <summary>
        /// Carregamento da página, verificar se é um edição/inclusão de usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // verificar o acesso de usuário master
                if (!object.ReferenceEquals(this.SessaoAtual, null) && this.SessaoAtual.UsuarioMaster())
                {
                    using (Logger Log = Logger.IniciarLog("Permissões - Carregando página"))
                    {
                        this.CarregarServicos();
                        this.VerificarTipoOperacao();
                        this.CarregarValidacoes();
                    }
                }
                else
                    this.SetarAviso("Você deve ser um usuário master para acessar este serviço.");
            }
        }

        /// <summary>
        /// Ajusta os controles de validações de acordo com PV Komerci
        /// </summary>
        private void CarregarValidacoes()
        {
            using (Logger log = Logger.IniciarLog("Início dos ajustes dos controles de validações de acordo com PV Komerci"))
            {
                try
                {
                    if (base.SessaoAtual.PossuiKomerci)
                    {
                        revTamanhoSenha.Text = "A senha deve conter, no mínimo, 8 caracteres, e, no máximo, 20 caracteres e sem caracteres especiais.";
                        revTamanhoSenha.ValidationExpression = "[\\w\\s]{8,20}$";

                        revSenhaAlt.Text = "A senha deve conter, no mínimo, 8 caracteres, e, no máximo, 20 caracteres e sem caracteres especiais.";
                        revSenhaAlt.ValidationExpression = "[\\w\\s]{8,20}$";

                        revCaracteresSenha.ValidationExpression = "^.*(?=.{8,20})(?=.*[a-zA-Z])(?=.*[0-9]).*$";
                        revSenhaAltNumeros.ValidationExpression = "^.*(?=.{8,20})(?=.*[a-zA-Z])(?=.*[0-9]).*$";
                    }
                    else
                    {
                        revTamanhoSenha.Text = "A senha deve conter, no mínimo, 6 caracteres, e, no máximo, 20 caracteres e sem caracteres especiais.";
                        revTamanhoSenha.ValidationExpression = "[\\w\\s]{6,20}$";

                        revSenhaAlt.Text = "A senha deve conter, no mínimo, 6 caracteres, e, no máximo, 20 caracteres e sem caracteres especiais.";
                        revSenhaAlt.ValidationExpression = "[\\w\\s]{6,20}$";

                        revCaracteresSenha.ValidationExpression = "^.*(?=.{6,20})(?=.*[a-zA-Z])(?=.*[0-9]).*$";
                        revSenhaAltNumeros.ValidationExpression = "^.*(?=.{6,20})(?=.*[a-zA-Z])(?=.*[0-9]).*$";
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "MensagemHelp", "MensagemHelp(" + SessaoAtual.PossuiKomerci.ToString().ToLower() + ");", true);
                }
                catch (Exception ex)
                { 
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibe a área de aviso do controle
        /// </summary>
        private void SetarAviso(String aviso)
        {
            ltError.Text = aviso;
            pnlErro.Visible = true;
            pnlEditarUsuario.Visible = false;
            pnlAdicionarUsuario.Visible = false;
            trvServicosPanel.Visible = false;
        }

        /// <summary>
        /// Verifica se é um alteração/inclusão de usuário
        /// </summary>
        private void VerificarTipoOperacao()
        {
            using (Logger Log = Logger.IniciarLog("Verificando se é alteração/inclusão de usuário"))
            {
                try
                {
                    string[] keys = Request.QueryString.AllKeys;

                    if (keys.Contains<string>("dados"))
                    {
                        pnlEditarUsuario.Visible = true;
                        QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                        this.CarregarDadosUsuario(queryString["CodigoUsuario"]);

                        // recuperar lista de serviços selecionados
                        ViewState["servicosAtuais"] = this.ObterServicosSelecionados();
                    }
                    else
                    {
                        pnlAdicionarUsuario.Visible = true;
                    }
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
        /// Carrega o nome e o login do usuário especificado pela query string de abertura
        /// </summary>
        private void CarregarDadosUsuario(string userName)
        {
            using (Logger Log = Logger.IniciarLog("Carregando nome/login do usuário"))
            {
                try
                {
                    Int32 codigoRetorno;

                    using (UsuarioServico.UsuarioServicoClient usuarioClient = new UsuarioServico.UsuarioServicoClient())
                    {
                        UsuarioServico.Entidade entidade = new UsuarioServico.Entidade()
                        {
                            Codigo = base.SessaoAtual.CodigoEntidade,
                            GrupoEntidade = new UsuarioServico.GrupoEntidade() { Codigo = base.SessaoAtual.GrupoEntidade }
                        };

                        UsuarioServico.Usuario[] usuarios = usuarioClient.ConsultarPorCodigoEntidade(out codigoRetorno, userName, entidade);

                        if (codigoRetorno > 0)
                            base.ExibirPainelExcecao("UsuarioServico.Consultar", codigoRetorno);
                        else
                        {
                            lblNomeCompleto.Text = usuarios[0].Descricao;
                            lblUsuario.Text = userName;
                            ltrCodigoIdUsuario.Text = usuarios[0].CodigoIdUsuario.ToString();
                            this.CarregarPermissoesUsuario(userName, usuarios[0].CodigoIdUsuario);

                            // Caso o usuário seja master o treeview não ficará habilitado
                            if (usuarios[0].TipoUsuario == "M")
                            {
                                chkAll.Attributes["disabled"] = "disabled";
                                _trvServicos.Enabled = false;
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
        /// Carrega o menu de navegação/permissão no portal de serviços
        /// </summary>
        /// <param name="userName"></param>
        private void CarregarPermissoesUsuario(String userName, Int32 codigoIdUsuario)
        {
            using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
            {
                UsuarioServico.Menu[] menuPermissoes = client.ConsultarMenu(userName, 
                    base.SessaoAtual.GrupoEntidade, 
                    base.SessaoAtual.CodigoEntidade,
                    codigoIdUsuario);

                IEnumerable<TreeNode> nodes = _trvServicos.Nodes.Cast<TreeNode>();
                this.CarregarPermissoesUsuario(menuPermissoes, nodes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuPermissoes"></param>
        private void CarregarPermissoesUsuario(UsuarioServico.Menu[] menuPermissoes, IEnumerable<TreeNode> nodes)
        {
            using (Logger Log = Logger.IniciarLog("Carregando permissões de usuário"))
            {
                try
                {
                    if (menuPermissoes.Length > 0 && nodes.Count() > 0)
                    {
                        for (int i = 0; i < menuPermissoes.Length; i++)
                        {
                            UsuarioServico.Menu menu = menuPermissoes[i];

                            TreeNode node = nodes.FirstOrDefault(x => x.Value == menu.Codigo.ToString());
                            if (!object.ReferenceEquals(node, null))
                                node.Checked = true;

                            if (menu.Items != null && node != null && menu.Items.Count() > 0)
                            {
                                this.CarregarPermissoesUsuario(menu.Items, node.ChildNodes.Cast<TreeNode>());
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
        /// Carregar a lista de servicos disponíveis para o grupo de entidade atual
        /// </summary>
        private void CarregarServicos()
        {
            using (Logger Log = Logger.IniciarLog("Carregando serviços"))
            {
                try
                {
                    // Retornar todos os serviços para montagem da TreeView
                    using (AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient())
                    {
                        AdministracaoServico.Servico[] _servicos = client.ConsultarPorGrupoEntidade(SessaoAtual.GrupoEntidade);

                        if (!object.ReferenceEquals(_servicos, null) && _servicos.Length > 0)
                        {
                            //TreeNode rootNode = _trvServicos.Nodes[0]; // recuperar o item principal da Árvore
                            var _rootServicos = _servicos.Where(x => x.ServicoPai == 0);
                            foreach (AdministracaoServico.Servico _servico in _rootServicos)
                            {
                                TreeNode rootServicoNode = new TreeNode()
                                {
                                    Value = _servico.Codigo.ToString(),
                                    Text = _servico.DescricaoMenu,
                                    SelectAction = TreeNodeSelectAction.None
                                };
                                this.CarregarServicosFilho(rootServicoNode, _servico.Codigo, _servicos);
                                _trvServicos.Nodes.Add(rootServicoNode);
                            }
                            _trvServicos.Visible = true;
                        }
                    }
                }
                catch (FaultException<AdministracaoServico.GeneralFault> ex)
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
        /// Carrega árvore de serviços com Serviços filhos
        /// </summary>
        /// <param name="rootServicoNode"></param>
        /// <param name="_servicos"></param>
        protected void CarregarServicosFilho(TreeNode rootServicoNode, Int32 codServicoPai, AdministracaoServico.Servico[] _servicos)
        {
            using (Logger Log = Logger.IniciarLog("Carregando serviços filhos"))
            {
                try
                {
                    var _rootServicos = _servicos.Where(x => x.ServicoPai == codServicoPai);
                    foreach (AdministracaoServico.Servico _servico in _rootServicos)
                    {
                        TreeNode childServicoNode = new TreeNode()
                        {
                            Value = _servico.Codigo.ToString(),
                            Text = _servico.DescricaoMenu,
                            SelectAction = TreeNodeSelectAction.None
                        };
                        this.CarregarServicosFilho(childServicoNode, _servico.Codigo, _servicos);
                        rootServicoNode.ChildNodes.Add(childServicoNode);
                    }
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
        /// Salva as alterações no banco do IS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Salvar(object sender, EventArgs e)
        {
            Panel[] paineis = new Panel[5]{
                pnlErro,
                pnlAdicionarUsuario,
                pnlEditarUsuario,
                trvServicosPanel,
                pnlBotoes
            };

            if (pnlEditarUsuario.Visible)
            {
                // Atualizar usuário existente
                if (this.EditarUsuario())
                {
                    // Voltar para página de origem
                    String urlVoltar = base.web.ServerRelativeUrl + "/Paginas/pn_CadastroSenhaSite.aspx";
                    base.ExibirPainelConfirmacaoAcao("Cadastro do Portal", "Informações atualizadas com sucesso.", urlVoltar, paineis);
                    //base.Alert("Informações atualizadas com sucesso.", "/sites/fechado/minhaconta/Paginas/pn_CadastroSenhaSite.aspx");
                    //this.Voltar(btnCancelar, new EventArgs());
                }
            }
            else
            {
                // Adicionar novo usuário
                if (this.AdicionarUsuario())
                {
                    // Voltar para página de origem
                    String urlVoltar = base.web.ServerRelativeUrl + "/Paginas/pn_CadastroSenhaSite.aspx";
                    base.ExibirPainelConfirmacaoAcao("Cadastro do Portal", "Usuário incluído com sucesso.", urlVoltar, paineis);

                    // Voltar para página de origem
                    //base.Alert("Usuário incluído com sucesso.", "/sites/fechado/minhaconta/Paginas/pn_CadastroSenhaSite.aspx");
                    //this.Voltar(btnCancelar, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Alterar senha no banco do IS
        /// </summary>
        protected void AlterarSenha(object sender, EventArgs e)
        {
            Panel[] paineis = new Panel[5]{
                pnlErro,
                pnlAdicionarUsuario,
                pnlEditarUsuario,
                trvServicosPanel,
                pnlBotoes
            };

            if (pnlEditarUsuario.Visible)
            {
                // Atualizar usuário existente
                if (this.EditarSenhaUsuario())
                {
                    // Voltar para página de origem
                    String urlVoltar = base.web.ServerRelativeUrl + "/Paginas/pn_CadastroSenhaSite.aspx";
                    base.ExibirPainelConfirmacaoAcao("Cadastro do Portal", "Senha atualizada com sucesso.", urlVoltar, paineis, "icone-green");
                    //base.Alert("Informações atualizadas com sucesso.", "/sites/fechado/minhaconta/Paginas/pn_CadastroSenhaSite.aspx");
                    //this.Voltar(btnCancelar, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Adiciona um novo usuário
        /// </summary>
        private Boolean AdicionarUsuario()
        {
            using (Logger Log = Logger.IniciarLog("Adiciona novo usuário"))
            {
                try
                {
                    Int32 codigoRetorno = 0;

                    // verifica se é para atualizar a senha
                    String _senha = null;

                    if (!String.IsNullOrEmpty(txtConfirmacaoSenha.Text) && !String.IsNullOrEmpty(txtSenha.Text))
                        _senha = EncriptadorSHA1.EncryptString(txtSenha.Text);

                    // recuperar lista de serviços selecionados
                    String _servicos = null;
                    _servicos = this.ObterServicosSelecionados();

                    // contar total de nós e comparar com os nós selecionados, caso sejam iguais, o sistema
                    // deve incluir um usuário master
                    Int32 totalServicos = this.ContarTotalNos(_trvServicos.Nodes);
                    Int32 totalSelecionados = _trvServicos.CheckedNodes.Count;

                    // Chamar servico de atualizacao
                    using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
                    {

                        codigoRetorno = client.InserirUsuario(SessaoAtual.PossuiKomerci,
                             SessaoAtual.GrupoEntidade,
                             SessaoAtual.CodigoEntidade.ToString(),
                             txtUsuario.Text,
                             txtNomeCompleto.Text,
                             (totalServicos == totalSelecionados ? "M" : "P"),
                             _senha,
                             _servicos);
                    }

                    if (codigoRetorno > 0)
                    {
                        base.ExibirPainelExcecao("UsuarioServico.InserirUsuario", codigoRetorno);
                        return false;
                    }

                    return true;
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }

        /// <summary>
        /// Salva as alterações feitas no usuário
        /// </summary>
        private Boolean EditarUsuario()
        {
            using (Logger Log = Logger.IniciarLog("Salvando alterações feitas no usuário"))
            {
                try
                {
                    // recuperar lista de serviços selecionados
                    string _servicos = null;
                    _servicos = this.ObterServicosSelecionados();

                    // contar total de nós e comparar com os nós selecionados, caso sejam iguais, o sistema
                    // deve incluir um usuário master
                    Int32 totalServicos = this.ContarTotalNos(_trvServicos.Nodes);
                    Int32 totalSelecionados = _trvServicos.CheckedNodes.Count;
                    Int32 codigoRetorno = 0;

                    // Chamar servico de atualizacao
                    using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
                    {
                        codigoRetorno = client.Atualizar(SessaoAtual.PossuiKomerci,
                            SessaoAtual.GrupoEntidade,
                            SessaoAtual.CodigoEntidade.ToString(),
                            lblUsuario.Text,
                            lblNomeCompleto.Text,
                            (totalServicos == totalSelecionados ? "M" : "P"),
                            "", // A senha não será atualizada pela procedure
                            _servicos,
                            ltrCodigoIdUsuario.Text.ToInt32());
                    }

                    if (codigoRetorno > 0)
                    {
                        base.ExibirPainelExcecao("UsuarioServico.AtualizarPermissoes", codigoRetorno);
                        return false;
                    }
                    else
                    {
                        SessaoAtual.SenhaMigrada = true;
                    }

                    return true;
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }

        /// <summary>
        /// Salva as alterações de senha do usuário
        /// </summary>
        private Boolean EditarSenhaUsuario()
        {
            using (Logger Log = Logger.IniciarLog("Editando senha do usuário"))
            {
                try
                {
                    // verifica se é para atualizar a senha
                    string _senha = null;
                    if (!String.IsNullOrEmpty(txtConfirmacaoSenhaAlt.Text) && !String.IsNullOrEmpty(txtSenhaAlt.Text))
                    {
                        _senha = EncriptadorSHA1.EncryptString(txtSenhaAlt.Text);
                    }

                    Int32 codigoRetorno = 0;

                    // Chamar servico de atualizacao
                    using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
                    {
                        codigoRetorno = client.AtualizarSenhaUsuario(
                            SessaoAtual.GrupoEntidade,
                            SessaoAtual.CodigoEntidade,
                            lblUsuario.Text,
                            ltrCodigoIdUsuario.Text.ToInt32Null(0).Value,
                            _senha, 
                            this.SessaoAtual.PossuiKomerci, 
                            false);
                    }

                    if (codigoRetorno > 0)
                    {
                        base.ExibirPainelExcecao("UsuarioServico.AtualizarSenhaUsuario", codigoRetorno);
                        return false;
                    }

                    return true;
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string ObterServicosSelecionados()
        {
            using (Logger Log = Logger.IniciarLog("Obtendo serviços selecionados"))
            {
                try
                {
                    string _servicos = string.Empty;
                    if (_trvServicos.CheckedNodes.Count > 0)
                    {
                        for (int i = 0; i < _trvServicos.CheckedNodes.Count; i++)
                        {
                            TreeNode node = _trvServicos.CheckedNodes[i];
                            if (String.IsNullOrEmpty(_servicos))
                                _servicos = node.Value;
                            else
                                _servicos += String.Format(",{0}", node.Value);
                        }
                        return _servicos;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                return null;
            }
        }

        /// <summary>
        /// Retornar para a URl de destino
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Voltar(object sender, EventArgs e)
        {
            SPUtility.Redirect("/sites/fechado/minhaconta/Paginas/pn_CadastroSenhaSite.aspx", SPRedirectFlags.Default, HttpContext.Current);
        }


        /// <summary>
        /// Contar o total de nós (serviços) disponíveis na árvore de permissões
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private Int32 ContarTotalNos(TreeNodeCollection nodes)
        {
            int rootNodes = nodes.Count;

            foreach (TreeNode node in nodes)
            {
                rootNodes += this.ContarTotalNos(node.ChildNodes);
            }

            return rootNodes;
        }

        /// <summary>
        /// Validação se há caracteres repetidos na senha do usuário
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ctvCaracterRepetidoAlt_ServerValidate(object source, ServerValidateEventArgs args)
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
        /// Validação se há caracteres repetidos na senha do usuário
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
    }
}