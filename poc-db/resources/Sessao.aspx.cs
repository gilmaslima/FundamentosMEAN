#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [30/08/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Web.UI.WebControls;

namespace Redecard.PN.Comum.SharePoint.LAYOUTS.Redecard.Comum
{
    /// <summary>
    /// Página para verificação dos dados armazenados na sessão
    /// </summary>
    public class SessaoPage : ApplicationPageBaseAutenticada
    {
        /// <summary>
        /// ltrPropriedades control.
        /// </summary>
        protected Literal ltrPropriedades;

        /// <summary>
        /// treeViewSessao control.
        /// </summary>
        protected TreeView treeViewSessao;

        /// <summary>
        /// Page_Load.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Sessao.Contem() && String.Compare(Request["debug"], "enabled") == 0)
                {
                    TreeNode treeNode = new Node("Sessão", this.SessaoAtual).ToTreeNode();
                    treeViewSessao.Nodes.Add(treeNode);
                    treeViewSessao.CollapseAll();
                    treeNode.Expand();
                }
            }
            catch (PortalRedecardException ex)
            {
                ltrPropriedades.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ltrPropriedades.Text = ex.Message;
            }
        }
    }
}