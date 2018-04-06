#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [29/05/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using Redecard.PN.Comum;
using System.Collections.Generic;
using System.Reflection;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{

    /// <summary>
    /// Página para configuração dos serviços e dos menus no Portal de Serviços
    /// </summary>
    public partial class Servicos : ApplicationPageBaseAutenticadaWindows
    {
        #region [ Propriedades da Página ]
        /// <summary>Dados de páginas que foram alterados</summary>
        public List<AdministracaoServico.Pagina> ListaPaginasAlteradas
        {
            get
            {
                if (ViewState["ListaPaginasAlteradas"] == null)
                    ViewState["ListaPaginasAlteradas"] = new List<AdministracaoServico.Pagina>();
                return (List<AdministracaoServico.Pagina>)ViewState["ListaPaginasAlteradas"];
            }
            set
            {
                ViewState["ListaPaginasAlteradas"] = value;
            }
        }
        public bool Adicao
        {
            get { return ViewState["adicao"] == null ? false : (bool)ViewState["adicao"]; }
            set { ViewState["adicao"] = value; }
        }

        public AdministracaoServico.Servico EntServicoSelecionado
        {
            get
            {
                if (ViewState["EntServicoSelecionado"] == null)
                    ViewState["EntServicoSelecionado"] = new AdministracaoServico.Servico();
                return (AdministracaoServico.Servico)ViewState["EntServicoSelecionado"];
            }
            set
            {
                ViewState["EntServicoSelecionado"] = value;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.ChecarUsuarioAdministrador();
                this.CarregarServicos();
            }

            this.chkChangeParent.Visible = true;
            this.pnlChangeParent.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ChecarUsuarioAdministrador()
        {
            if (!SPContext.Current.Web.UserIsWebAdmin)
                throw new UnauthorizedAccessException("Somente usuários administradores podem configurar a navegacão do Portal de Serviços.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ServicoSelecionado(object sender, EventArgs e)
        {
            CarregarServicoSelecionado();
            Adicao = false;

            // verificar se o serviço selecionado contám filhos para 
            // exibir o botão de remover serviço
            this.HabilitarBotaoRemoverServico();

            // exibe o container para alteração de serviço pai
            this.PreencheParentTarget(this.trvServicos);
        }

        /// <summary></summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AdicionarServico(object sender, EventArgs e)
        {
            pnlAdicionarServico.Visible = true;
            EntServicoSelecionado = new AdministracaoServico.Servico();
            LimparControles(pnlAdicionarServico);
            btnAdicionarPagina.Enabled = false;
            LimparListaPaginas();
            Adicao = true;

            this.chkChangeParent.Visible = false;
            this.pnlChangeParent.Visible = false;
        }

        /// <summary>
        /// Emite um altera para o usuário
        /// </summary>
        /// <param name="message">Mensagem para o alerta</param>
        /// <param name="tipoMensagem">Tipo de alerta a ser exibido ao usuário</param>
        private void ShowClientMessage(string message, ClientMessageType tipoMensagem = ClientMessageType.Alerta)
        {
            string jsFunction = string.Empty;

            switch (tipoMensagem)
            {
                case ClientMessageType.Erro:
                    jsFunction = "EmiteAlertaErro";
                    break;
                case ClientMessageType.Sucesso:
                    jsFunction = "EmiteAlertaSucesso";
                    break;
                case ClientMessageType.Alerta:
                default:
                    jsFunction = "EmiteAlerta";
                    break;
            }

            // codifica a mensagem para exibição ao usuário
            message = Server.HtmlEncode(message);

            // registra o script na página
            string script = String.Format(
                "$(function() {{ window.setTimeout(function() {{ {0}(\"{1}\"); }}, 500);  }});", 
                jsFunction, message);
            System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, typeof(string), "alerta", script, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BotaoAdicionarServico(object sender, EventArgs e)
        {
            try
            {
                Int32 servicoID = 0;
                if (!object.ReferenceEquals(trvServicos.SelectedNode, null))
                {
                    servicoID = Int32.Parse(trvServicos.SelectedValue);
                }

                // obtém e valida flag para exibir on footer (regra: flag de exibir no menu também deve estar ativa)
                Boolean flgExibirNoFooter = chkMenuServico.Checked && chkExibirNoFooter.Checked;
                if (!this.ValidarCheckExibirFooter(flgExibirNoFooter))
                {
                    return;
                }

                // adiciona flag ao serviço
                EntServicoSelecionado.ExibirNofooter = flgExibirNoFooter;

                //AdministracaoServico.Servico servico = new AdministracaoServico.Servico();
                EntServicoSelecionado.Descricao = txtDescricaoServico.Text;
                EntServicoSelecionado.ExibirUsuarioCancelado = chkUsuarioCanceladoServico.Checked;
                EntServicoSelecionado.ExibirUsuarioCanceladoSpecified = true;
                EntServicoSelecionado.Ativo = chkAtivoServico.Checked;
                EntServicoSelecionado.AtivoSpecified = true;
                EntServicoSelecionado.Observacoes = txtObservacaoServico.Text;
                EntServicoSelecionado.ExibirNoMenu = chkMenuServico.Checked;
                EntServicoSelecionado.ExibirNoMenuSpecified = true;
                EntServicoSelecionado.Posicao = Int32.Parse(txtPosicaoServico.Text);
                EntServicoSelecionado.PosicaoSpecified = true;
                EntServicoSelecionado.DescricaoMenu = txtMenuServico.Text;
                EntServicoSelecionado.ServicoPai = Adicao ? servicoID : EntServicoSelecionado.ServicoPai;
                EntServicoSelecionado.ServicoPaiSpecified = true;
                EntServicoSelecionado.ServicoBasico = chkServicoBasico.Checked;

                // valida a intenção de alterar o menu pai
                if (!Adicao && this.chkChangeParent.Checked)
                {
                    Int32? servicoSelecionado = null;

                    // chama método para alteração do menu pai
                    if (!this.ChangeParent(out servicoSelecionado))
                        return;

                    if (servicoSelecionado.HasValue)
                        EntServicoSelecionado.ServicoPai = servicoSelecionado.Value;
                }

                using (AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient())
                {
                    if (Adicao)
                    {
                        client.InserirServico(EntServicoSelecionado);
                    }
                    else
                    {
                        client.AtualizarServico(EntServicoSelecionado);
                    }
                }

                trvServicos.Nodes.Clear();
                ResetarPagina();

                // exibe mensagem de sucesso
                ShowClientMessage("Dados salvos com sucesso", ClientMessageType.Sucesso);
            }
            catch (Exception ex)
            {

                ShowClientMessage(ex.Message, ClientMessageType.Erro);
            }
        }

        private void ResetarPagina()
        {
            this.CarregarServicos();
            pnlAdicionar.Visible = false;
            pnlAdicionarServico.Visible = false;
            btnAdicionarPagina.Enabled = true;
            this.LimparListaPaginas();

        }
        protected void CancelarAdicaoServico(object sender, EventArgs e)
        {
            LimparControles(pnlAdicionarServico);
            ResetarPagina();

        }
        /// <summary>
        /// 
        /// </summary>
        private void HabilitarBotaoRemoverServico()
        {
            if (!object.ReferenceEquals(trvServicos.SelectedNode, null) && trvServicos.SelectedNode.ChildNodes.Count < 1)
                btnRemoverServico.Enabled = true;
            else
            {
                btnRemoverServico.Enabled = false;
                this.CarregarPaginas();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ExibirPanelAdicionarPagina(object sender, EventArgs e)
        {
            pnlAdicionar.Visible = true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AdicionarPagina(object sender, EventArgs e)
        {
            try
            {
                if (!object.ReferenceEquals(EntServicoSelecionado, null))
                {
                    Int32 servicoID = EntServicoSelecionado.Codigo;
                    AdministracaoServico.Pagina pagina = new AdministracaoServico.Pagina();
                    pagina.Caminho = txtCaminho.Text;
                    pagina.NomeLink = txtLink.Text;
                    pagina.Menu = chkMenu.Checked;
                    pagina.MenuSpecified = true;

                    using (AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient())
                    {
                        client.InserirPagina(pagina, servicoID);
                    }

                    this.CarregarPaginas();
                    LimparControles(pnlAdicionar);
                    pnlAdicionar.Visible = false;
                }

            }
            catch (Exception ex)
            {

                ShowClientMessage(ex.Message, ClientMessageType.Erro);
            }
        }
        protected void CancelarInsercao(object sender, EventArgs e)
        {
            LimparControles(pnlAdicionar);
            pnlAdicionar.Visible = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void CarregarPaginas()
        {
            try
            {
                if (!object.ReferenceEquals(EntServicoSelecionado, null))
                {
                    Int32 servicoID = EntServicoSelecionado.Codigo;
                    using (AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient())
                    {

                        //Limpa a lista de páginas alteradas
                        ListaPaginasAlteradas = null;

                        //Carrega o Grid
                        AdministracaoServico.Pagina[] paginas = client.ConsultarPaginas(servicoID);
                        gdvPaginas.DataSource = paginas;
                        gdvPaginas.DataBind();

                        pnlGvdPaginasInstrucao.Visible = paginas != null && paginas.Length > 0;
                    }
                }

            }
            catch (Exception ex)
            {
                ShowClientMessage(ex.Message, ClientMessageType.Erro);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gdvDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int paginaID = (int)gdvPaginas.DataKeys[e.RowIndex].Value;
            this.RemoverPagina(paginaID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RemoverServico(object sender, EventArgs e)
        {
            try
            {
                if (!object.ReferenceEquals(EntServicoSelecionado, null))
                {
                    Int32 servicoID = EntServicoSelecionado.Codigo;
                    using (AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient())
                    {
                        client.RemoverServico(servicoID);
                    }

                    trvServicos.Nodes.Clear();

                    this.CarregarServicos();
                    ResetarPagina();
                }

            }
            catch (Exception ex)
            {

                ShowClientMessage(ex.Message, ClientMessageType.Erro);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LimparListaPaginas()
        {
            pnlAdicionar.Visible = false;
            gdvPaginas.DataSource = null;
            gdvPaginas.DataBind();

            pnlGvdPaginasInstrucao.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paginaID"></param>
        private void RemoverPagina(int paginaID)
        {
            try
            {
                using (AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient())
                {
                    client.RemoverPagina(paginaID);
                }
                this.CarregarPaginas();

            }
            catch (Exception ex)
            {
                ShowClientMessage(ex.Message, ClientMessageType.Erro);
            }
        }

        /// <summary>
        /// Lista os serviços disponíveis no Portal Redecard
        /// </summary>
        private void CarregarServicos()
        {
            try
            {
                AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient();
                AdministracaoServico.Servico[] servicos = client.ConsultarPorGrupoEntidade(0); // Retornar todos os serviços para montagem da TreeView
                if (!object.ReferenceEquals(servicos, null) && servicos.Length > 0)
                {
                    var servicosRoot = servicos.Where(x => x.ServicoPai == 0);
                    foreach (AdministracaoServico.Servico servico in servicosRoot)
                    {
                        TreeNode rootServicoNode = new TreeNode()
                        {
                            Value = servico.Codigo.ToString(),
                            Text = string.Format("{0} ({1})", servico.DescricaoMenu, servico.Codigo.ToString()),
                            SelectAction = TreeNodeSelectAction.Select,
                            Expanded = true
                        };
                        this.CarregarServicosFilho(rootServicoNode, servico.Codigo, servicos);
                        trvServicos.Nodes.Add(rootServicoNode);
                    }
                    trvServicos.Visible = true;
                }

            }
            catch (Exception ex)
            {
                ShowClientMessage(ex.Message, ClientMessageType.Erro);
            }
        }
        /// <summary>
        /// Carrega os dados do serviço selecionado
        /// </summary>
        private void CarregarServicoSelecionado()
        {
            Int32 servicoID = Int32.Parse(trvServicos.SelectedValue);

            LimparControles(pnlAdicionarServico);

            //AdministracaoServico.Servico servico = new AdministracaoServico.Servico();
            using (AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient())
            {
                EntServicoSelecionado = client.ConsultarServico(servicoID);
            }
            txtCodigoServico.Text = EntServicoSelecionado.Codigo.ToString();
            txtDescricaoServico.Text = EntServicoSelecionado.Descricao;
            chkUsuarioCanceladoServico.Checked = EntServicoSelecionado.ExibirUsuarioCancelado;
            chkAtivoServico.Checked = EntServicoSelecionado.Ativo;
            txtObservacaoServico.Text = EntServicoSelecionado.Observacoes;
            chkMenuServico.Checked = EntServicoSelecionado.ExibirNoMenu;
            chkServicoBasico.Checked = EntServicoSelecionado.ServicoBasico;
            txtPosicaoServico.Text = EntServicoSelecionado.Posicao.ToString();
            txtMenuServico.Text = EntServicoSelecionado.DescricaoMenu;

            // marca flag para exibir no footer (desde ue o flag de exibir no menu também esteja ativo)
            chkExibirNoFooter.Checked = EntServicoSelecionado.ExibirNoMenu && EntServicoSelecionado.ExibirNofooter;

            this.CarregarPaginas();
            pnlAdicionar.Visible = false;
            pnlAdicionarServico.Visible = true;

        }

        /// <summary>
        /// Lista os serviços disponíveis no Portal Redecard
        /// </summary>
        /// <param name="rootServicoNode"></param>
        /// <param name="servicos"></param>
        protected void CarregarServicosFilho(TreeNode rootServicoNode, Int32 codServicoPai, AdministracaoServico.Servico[] servicos)
        {
            var servicosRoot = servicos.Where(x => x.ServicoPai == codServicoPai);
            foreach (AdministracaoServico.Servico servico in servicosRoot)
            {
                TreeNode childServicoNode = new TreeNode()
                {
                    Value = servico.Codigo.ToString(),
                    Text = string.Format("{0} ({1})", servico.DescricaoMenu, servico.Codigo.ToString()),
                    SelectAction = TreeNodeSelectAction.Select,
                    Expanded = true
                };
                this.CarregarServicosFilho(childServicoNode, servico.Codigo, servicos);
                rootServicoNode.ChildNodes.Add(childServicoNode);
            }
        }
        private void LimparControles(Control controlePai)
        {
            foreach (Control controle in controlePai.Controls)
            {
                if (controle is TextBox)
                {
                    ((TextBox)controle).Text = string.Empty;
                }
                if (controle is CheckBox)
                {
                    ((CheckBox)controle).Checked = false;
                }
                if (controle.Controls.Count > 0)
                {
                    LimparControles(controle);
                }
            }

        }
        protected void gdvEditing(object sender, GridViewEditEventArgs e)
        {
            gdvPaginas.EditIndex = e.NewEditIndex;
            CarregarPaginas();
        }
        protected void gdvRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int codigoPagina = gdvPaginas.DataKeys[e.RowIndex].Value.ToString().ToInt32();

            AdministracaoServico.Pagina pagina = new AdministracaoServico.Pagina();
            GridViewRow row = gdvPaginas.Rows[e.RowIndex];
            TextBox NomeLink = (TextBox)row.Cells[0].Controls[0];
            TextBox txtcaminho = (TextBox)row.Cells[1].Controls[0];
            CheckBox chkMenu = (CheckBox)row.Cells[2].Controls[0];

            pagina.Codigo = codigoPagina;
            pagina.CodigoSpecified = true;
            pagina.Caminho = txtcaminho.Text;
            pagina.NomeLink = NomeLink.Text;
            pagina.Menu = chkMenu.Checked;
            pagina.MenuSpecified = true;

            using (AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient())
            {
                if (pagina.Menu.GetValueOrDefault(false))
                {
                    // zera o identificador de navegação de todas as páginas do serviço atual
                    var servicoPaginas = client.ConsultarPaginas(EntServicoSelecionado.Codigo);
                    if (servicoPaginas != null)
                    {
                        foreach (var item in servicoPaginas)
                        {
                            // seleciona apenas as páginas em que a flag de navegação estiver marcada
                            if (!item.Menu.GetValueOrDefault(false))
                                continue;

                            // envia ao WCF para atualização com a flag desativada
                            item.Menu = false;
                            client.AtualizarPagina(item);
                        }
                    }
                }

                client.AtualizarPagina(pagina);
            }
            gdvPaginas.EditIndex = -1;
            CarregarPaginas();
        }
        protected void gdvCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gdvPaginas.EditIndex = -1;
            CarregarPaginas();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gdvDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton l = (LinkButton)e.Row.FindControl("lnkRemover");
                if (l != null)
                {
                    l.Attributes.Add("onclick", "javascript:return " + "confirm('Você tem certeza que deseja remover a página " + DataBinder.Eval(e.Row.DataItem, "Caminho") + "?')");
                }
            }
        }

        /// <summary>
        /// Evento de click para o botão btnChangeParent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private Boolean ChangeParent(out Int32? codigoSelecionado)
        {
            codigoSelecionado = null;

            var checkedNodes = this.trvChangeParentTarget.CheckedNodes;

            #region Valida a seleção

            // valida: deve ter ao menos 1 item selecionado
            if (checkedNodes.Count < 1)
            {
                this.ShowClientMessage("Selecione um item para continuar", ClientMessageType.Erro);
                return false;
            }

            // valida: só pode ter 1 item selecionado
            if (checkedNodes.Count > 1)
            {
                this.ShowClientMessage("Selecione apenas um item para continuar", ClientMessageType.Erro);
                return false;
            }

            #endregion

            var targetNode = checkedNodes[0];
            var sourceNode = this.trvServicos.SelectedNode;

            // impede movimentação para dentro do próprio menu selecionado
            if (string.Compare(targetNode.Value, sourceNode.Value) == 0
                || this.ValidaNodeSelecionado(sourceNode.ChildNodes, targetNode.Value))
            {
                this.ShowClientMessage("Não é permitido mover um item para dentro de si mesmo", ClientMessageType.Erro);
                return false;
            }

            codigoSelecionado = targetNode.Value.ToInt32Null(null);
            return true;
        }

        /// <summary>
        /// Preenche treeview para seleção de item de serviço de destino
        /// </summary>
        /// <param name="trvSource">Treeview da qual serão copiados os nodes</param>
        protected void PreencheParentTarget(TreeView trvSource)
        {
            this.trvChangeParentTarget.Nodes.Clear();

            TreeNode rootNode = new TreeNode()
            {
                Text = "[root]",
                Value = "0",
                Expanded = true,
                ToolTip = "Selecione este item se for necessário realocar um item para a posição principal (sem pai)"
            };

            foreach (TreeNode node in trvSource.Nodes)
            {
                rootNode.ChildNodes.Add(this.GetNode(node));
            }
            
            this.trvChangeParentTarget.Nodes.Add(rootNode);
        }

        /// <summary>
        /// Obtém node adaptado para a listagem de seleção
        /// </summary>
        /// <param name="sourceNode">Node corrente</param>
        /// <param name="parentNode">Node pai</param>
        /// <returns></returns>
        private TreeNode GetNode(TreeNode sourceNode, TreeNode parentNode = null)
        {
            // define atributos iniciais
            TreeNode node = new TreeNode()
            {
                Checked = false,
                Selected = sourceNode.Selected,
                Text = sourceNode.Text,
                Value = sourceNode.Value,
                Expanded = true
            };

            // trata item selecionado
            if (node.Selected)
            {
                // marca como checkado o item pai
                if (parentNode != null)
                    parentNode.Checked = true;

                // identifica o item selecionado na label
                lblItemSelecionado.Text = node.Text;
            }

            // percorre por itens filhos
            foreach (TreeNode child in sourceNode.ChildNodes)
                node.ChildNodes.Add(this.GetNode(child, node));

            return node;
        }

        /// <summary>
        /// Valida se existe algum node selecionado na listagem
        /// </summary>
        /// <param name="listTreeNode">Lista de nodes que será vistoriada</param>
        /// <param name="valueToCompare">Valor a ser comparado</param>
        /// <returns></returns>
        private Boolean ValidaNodeSelecionado(TreeNodeCollection listTreeNode, string valueToCompare)
        {
            // valida lista informada
            if (listTreeNode == null)
                return false;

            // valida valor informado
            if (string.IsNullOrWhiteSpace(valueToCompare))
                return false;

            // percorre a listagem de nodes
            foreach (TreeNode node in listTreeNode)
            {
                // compara o item corrente + itens filhos
                if (string.Compare(node.Value, valueToCompare, true) == 0 
                    || this.ValidaNodeSelecionado(node.ChildNodes, valueToCompare))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Valida a seleção da checkbox para exibir serviço no footer
        /// </summary>
        /// <param name="flgExibirNoFooter">Flag correspondente à seleção do checkbox</param>
        /// <returns>TRUE: validação OK, podendo prosseguir</returns>
        private Boolean ValidarCheckExibirFooter(Boolean flgExibirNoFooter)
        {
            String clientScriptKey = new Guid().ToString();
            AdministracaoServico.Servico[] servicos = null;

            try
            {
                // client para consulta 
                AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient();

                // obtém e valida os itens retornados pelo serviço
                servicos = client.ConsultarPorGrupoEntidade(0);
            }
            catch (PortalRedecardException ex)
            {
                ShowClientMessage(ex.Message, ClientMessageType.Erro);
                return false;
            }
            catch (Exception ex)
            {
                ShowClientMessage(ex.Message, ClientMessageType.Erro);
                return false;
            }

            // valida se foi retornado algum serviço
            if (!(servicos != null && servicos.Length > 0))
                return false;

            // cenário em que o usuário marcou a checkbox
            if (!EntServicoSelecionado.ExibirNofooter && flgExibirNoFooter)
            {
                if (!ValidarCheckExibirFooterBottomUp(EntServicoSelecionado.Codigo, servicos))
                {
                    // apresenta a mensagem ao usuário
                    this.ShowClientMessage(String.Format("Marque o check \\'{0}\\' dos itens pais para continuar", this.chkExibirNoFooter.Text), ClientMessageType.Erro);

                    // retorna o estado do checkbox
                    this.chkExibirNoFooter.Checked = false;

                    // retorno inverso: inválido se houver algum item filho selecionado
                    return false;
                }
            }
            // cenário em que o usuário desmarcou a checkbox
            else if (EntServicoSelecionado.ExibirNofooter && !flgExibirNoFooter)
            {
                // valida recursivamente em modo top > down se possui algum serviço filho selecionado
                if (ValidarCheckExibirFooterTopDown(EntServicoSelecionado.Codigo, servicos))
                {
                    // apresenta a mensagem ao usuário
                    this.ShowClientMessage(String.Format("Desmarque o check \\'{0}\\' dos itens filhos para continuar", this.chkExibirNoFooter.Text), ClientMessageType.Erro);

                    // retorna o estado do checkbox
                    this.chkExibirNoFooter.Checked = true;

                    // retorno inverso: inválido se houver algum item filho selecionado
                    return false;
                }
            }
            
            // cenário default ou que não houve qualquer alteração na checkbox
            return true;
        }

        /// <summary>
        /// Validação recursiva, de pai a filhos, para verificar se existe algum item marcado para exibir no footer
        /// </summary>
        /// <param name="codigoServicoAtual">Código do serviço corrente no processo recursivo</param>
        /// <param name="servicos">Relação de serviços retornados pela camada de WCF</param>
        /// <returns>TRUE: encontrou algum item filho marcado para aparecer no footer</returns>
        private Boolean ValidarCheckExibirFooterTopDown(Int32 codigoServicoAtual, AdministracaoServico.Servico[] servicos)
        {
            // valida serviços
            if (!(servicos != null && servicos.Length > 0))
                return false;

            // obtém e valida a relação de serviços filhos que estejam: 
            // - marcados para aparecer no footer
            // - possuam como pai o serviço atual
            var servicosFilhos = servicos.Where(x => x.ServicoPai == codigoServicoAtual && x.ExibirNofooter).ToArray();
            if (servicosFilhos.Length > 1)
                return true;

            // percorre a relação de itens filhos para validar se há algum selecionado
            foreach (var item in servicosFilhos)
            {
                if (this.ValidarCheckExibirFooterTopDown(item.Codigo, servicos))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Validação recursiva para verificar se todos os pais também estão marcados para aparecer no footer
        /// </summary>
        /// <param name="codigoServicoAtual">Código do serviço corrente no processo recursivo</param>
        /// <param name="servicos">Relação de serviços retornados pela camada de WCF</param>
        /// <returns>FALSE: encontrou algum pai não marcado para aparecer no footer ou os parâmetros informados são inválidos</returns>
        private Boolean ValidarCheckExibirFooterBottomUp(Int32 codigoServicoAtual, AdministracaoServico.Servico[] servicos)
        {
            // valida serviços
            if (!(servicos != null && servicos.Length > 0))
                return false;

            // obtém e valida o serviço atual
            var servicoAtual = servicos.FirstOrDefault(x => x.Codigo == codigoServicoAtual);
            if (servicoAtual == null)
                return false;

            // valida o serviço pai (OK se o serviço não tem pai - ou é root)
            if (servicoAtual.ServicoPai == 0)
                return true;

            // obtém e valida o serviço pai
            var servicoPai = servicos.FirstOrDefault(x => x.Codigo == servicoAtual.ServicoPai);
            if (servicoPai == null)
                return true;

            // invalida caso o pai não esteja marcado para aparecer no footer
            if (!servicoPai.ExibirNofooter)
                return false;

            // se o serviço pai possui pai, prossegue recursivamente
            if (servicoPai.ServicoPai != 0)
                return this.ValidarCheckExibirFooterBottomUp(servicoPai.Codigo, servicos);

            return true;
        }
    }

    /// <summary>
    /// Enum para o tipo de mensagem a ser exibida em clientside
    /// </summary>
    public enum ClientMessageType
    {
        Alerta,
        Erro,
        Sucesso
    }
}