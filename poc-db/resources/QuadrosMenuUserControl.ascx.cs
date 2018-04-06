/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [19/05/2012] - [André Garcia] - [Criação]
*/

using System;
using System.Linq;
using Redecard.PN.Comum;
using System.Collections.Generic;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Microsoft.SharePoint.Utilities;
using System.Web;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.QuadrosMenu
{
    public partial class QuadrosMenuUserControl : UserControlBase
    {


        /// <summary>
        /// Carregamento da página, pesquisar pelo item de menu atual e exibir os boxes
        /// das demais funcionalidades
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !object.ReferenceEquals(this.SessaoAtual, null))
            {
                if (base.VerificarConfirmacaoPositia() && Request.Url.AbsolutePath.Contains("pn_DadosCadastrais.aspx"))
                    RedirecionarConfirmacaoPositiva();
                else
                {
                    this.pnlDados.Visible = true;
                    this.CarregarBoxes();
                }
            }
        }


        /// <summary>
        /// Carrega a relação de boxes (items de menu) ao qual o usuário possui acesso.
        /// </summary>
        private void CarregarBoxes()
        {
            Menu itemAtual = base.ObterMenuItemAtual();
            if (!object.ReferenceEquals(itemAtual, null))
            {
                if (itemAtual.Items.Count > 0)
                {
                    pnlMensagem.Visible = false;
                    pnlDados.Visible = true;
                    // carregar boxes filhos
                    rptBoxes.DataSource = itemAtual.Items;
                    rptBoxes.DataBind();
                }
                else
                {

                    pnlMensagem.Visible = true;
                    QuadroAcessoNegado quadro = (QuadroAcessoNegado)qdAcessoNegado;
                    pnlDados.Visible = false;

                }
            }
        }

        /// <summary>
        /// Carregamento dos boxes (Serviços)
        /// </summary>
        protected void ServicoDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item ||
                e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                System.Web.UI.WebControls.Repeater rptPaginas = e.Item.FindControl("rptPaginas") as System.Web.UI.WebControls.Repeater;
                if (!object.ReferenceEquals(rptPaginas, null))
                {
                    Menu itemAtual = e.Item.DataItem as Menu;

                    // carregar páginas
                    rptPaginas.DataSource = itemAtual.Paginas;
                    rptPaginas.DataBind();
                }
            }
        }
    }
}