#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [24/05/2012] – [André Garcia] – [Criação]
*/
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   : Inserção do tratamento de erro.
- [15/06/2012] – [André Rentes] – [Alteração]
*/
#endregion

using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.ServiceModel;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{

    /// <summary>
    /// Página para configuração dos serviços e dos menus no Portal de Serviços
    /// </summary>
    public class ConfigurarServicos : ApplicationPageBaseAutenticadaWindows
    {

        /// <summary>
        /// 
        /// </summary>
        protected DropDownList ddlGrupoEntidade;

        /// <summary>
        /// 
        /// </summary>
        protected TreeView _trvServicos;
        
        /// <summary>
        /// 
        /// </summary>
        protected System.Web.UI.HtmlControls.HtmlGenericControl dvPainel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    this.ChecarUsuarioAdministrador();
                    this.CarregarGruposEntidade();
                    this.CarregarServicos();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Message));
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                dvPainel.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }

        /// <summary>
        /// Salva as alterações no banco de dados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SalvarServicosEntidade(object sender, EventArgs e)
        {
            try
            {
                String selectedValue = ddlGrupoEntidade.SelectedValue;
                Int32 _selectedValueInt = Int32.Parse(selectedValue);

                int[] codigos = null;
                if (_trvServicos.CheckedNodes.Count > 0)
                {
                    codigos = new int[_trvServicos.CheckedNodes.Count];
                    // concatenar códigos de servico
                    for (int i = 0; i < _trvServicos.CheckedNodes.Count; i++)
                        codigos[i] = Int32.Parse(_trvServicos.CheckedNodes[i].Value);

                }
                AdministracaoServico.AdministracaoServicoClient client = new AdministracaoServico.AdministracaoServicoClient();
                client.SalvarServicosPorGrupoEntidade(_selectedValueInt, codigos);

                dvPainel.InnerText = "Serviços salvos com sucesso.";
            }
            catch (FaultException<AdministracaoServico.GeneralFault> ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarServicos()
        {
            AdministracaoServico.AdministracaoServicoClient client = null;
            try
            {
                client = new AdministracaoServico.AdministracaoServicoClient();
                AdministracaoServico.Servico[] _servicos = client.ConsultarPorGrupoEntidade(0); // Retornar todos os serviços para montagem da TreeView

                if (!object.ReferenceEquals(_servicos, null) && _servicos.Length > 0)
                {
                    TreeNode rootNode = _trvServicos.Nodes[0]; // recuperar o item principal da Árvore
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
                        rootNode.ChildNodes.Add(rootServicoNode);
                    }
                    _trvServicos.Visible = true;
                }
                client.Close();
            }
            catch (FaultException<AdministracaoServico.GeneralFault> ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
                if (!object.ReferenceEquals(client, null))
                    client.Abort();
            }
            catch (Exception exception)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(exception.Message));
                if (!object.ReferenceEquals(client, null))
                    client.Abort();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootServicoNode"></param>
        /// <param name="_servicos"></param>
        protected void CarregarServicosFilho(TreeNode rootServicoNode, Int32 codServicoPai, AdministracaoServico.Servico[] _servicos)
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
        protected void CarregarDadosServicos(object sender, EventArgs e)
        {
            String selectedValue = ((DropDownList)sender).SelectedValue;
            Int32 _selectedValueInt = Int32.Parse(selectedValue);

            if (_selectedValueInt < 1)
            {
                _trvServicos.Enabled = false;
                this.LimparArvore();
            }
            else
            {
                _trvServicos.Enabled = true;
                this.ConsultarNodesGrupoEntidade(_selectedValueInt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_selectedValueInt"></param>
        private void ConsultarNodesGrupoEntidade(Int32 _selectedValueInt)
        {
            this.LimparArvore();

            AdministracaoServico.AdministracaoServicoClient client = null;
            try
            {
                // Pesquisar os serviços que o grupo entidade possui acesso
                client = new AdministracaoServico.AdministracaoServicoClient();
                AdministracaoServico.Servico[] _servicos = client.ConsultarPorGrupoEntidade(_selectedValueInt);
                if (!object.ReferenceEquals(_servicos, null) && _servicos.Length > 0)
                {
                    _trvServicos.Enabled = true;

                    // rodar os nós da árviore e marcar os que possuirem correspondentes no objeto "_servicos"
                    this.MarcarPermissoes(_trvServicos.Nodes, _servicos);
                }
                client.Close();
            }
            catch (FaultException<AdministracaoServico.GeneralFault> ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
                if (!object.ReferenceEquals(client, null))
                    client.Abort();
            }
            catch (Exception exception)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(exception.Message));
                if (!object.ReferenceEquals(client, null))
                    client.Abort();
            }
        }

        /// <summary>
        /// Busca os nós que possuem correspondentes no objeto "_servicos" e seta
        /// esse nós com o valor True
        /// </summary>
        private void MarcarPermissoes(TreeNodeCollection nodes, AdministracaoServico.Servico[] _servicos)
        {
            foreach (TreeNode node in nodes)
            {
                object servico = _servicos.FirstOrDefault(x => x.Codigo.ToString() == node.Value);
                if (!object.ReferenceEquals(servico, null))
                    node.Checked = true;

                // buscar nós filhos
                if (node.ChildNodes.Count > 0)
                {
                    this.MarcarPermissoes(node.ChildNodes, _servicos);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LimparArvore()
        {
            if (_trvServicos.CheckedNodes.Count > 0)
            {
                for (int i = 0; i < _trvServicos.CheckedNodes.Count; i++)
                {
                    TreeNode node = _trvServicos.CheckedNodes[0];
                    node.Checked = false;
                    i--;
                }
            }
        }

        /// <summary>
        /// Carregar a lista de grupos de entidade
        /// </summary>
        private void CarregarGruposEntidade()
        {
            Int32 codigoRetorno;

            EntidadeServico.EntidadeServicoClient client = null;
            try
            {
                client = new EntidadeServico.EntidadeServicoClient();
                EntidadeServico.GrupoEntidade[] grupos = client.ConsultarGrupoCache(out codigoRetorno, -1, false);

                if (codigoRetorno == 0)
                {
                    ddlGrupoEntidade.DataSource = grupos;
                    ddlGrupoEntidade.DataTextField = "Descricao";
                    ddlGrupoEntidade.DataValueField = "Codigo";
                    ddlGrupoEntidade.DataBind();
                }
                client.Close();
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
                if (!object.ReferenceEquals(client, null))
                    client.Abort();
            }
            catch (Exception exception)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(exception.Message));
                if (!object.ReferenceEquals(client, null))
                    client.Abort();
            }
        }
    }
}
