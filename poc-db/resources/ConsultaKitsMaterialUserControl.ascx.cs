#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [16/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.ConsultaKitsMaterial
{
    /// <summary>
    /// Web part de consulta kit de materiais de venda e sinalização
    /// </summary>
    public partial class ConsultaKitsMaterialUserControl : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        private const String NomeServico = "Redecard.PN.OutrosServicos.Servicos.MaterialVendaServico";

        /// <summary>
        /// Carregamento da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && Util.UsuarioLogadoFBA())
            {
                this.CarregarKitsSinalizacao();
                this.CarregarMateriaisVenda();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void KitDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                HyperLink hlLink = e.Item.FindControl("hlLink") as HyperLink;
                if (!object.ReferenceEquals(hlLink, null))
                {
                    MaterialVendaServico.Kit kit = e.Item.DataItem as MaterialVendaServico.Kit;
                    QueryStringSegura query = new QueryStringSegura();
                    query.Add("Codigo", kit.CodigoKit.ToString());
                    hlLink.NavigateUrl = "/sites/fechado/servicos/Paginas/pn_ConsultarKitsMaterialComposicao.aspx?dados=" + query.EncryptedString;
                }
            }
        }

        /// <summary>
        /// Carrega os materiais de sinalização disponíveis para o estabelecimento
        /// </summary>
        private void CarregarKitsSinalizacao()
        {
            using (Logger Log = Logger.IniciarLog("Carregando materiais de sinalização disponíveis para o estabelecimento"))
            {
                try
                {
                    using (var client = new MaterialVendaServico.MaterialVendaServicoClient())
                    {
                        MaterialVendaServico.Kit[] remessas = client.ConsultarKitsSinalizacao(this.SessaoAtual.CodigoEntidade);

                        if (remessas.Length > 0)
                        {
                            rptKitsSinalizacao.DataSource = remessas;
                            rptKitsSinalizacao.DataBind();
                        }
                        else
                        {
                            pnlKitsSinalizacao.Visible = false;
                            pnlVazioKitsSinalizacao.Visible = true;
                            ((QuadroAviso)qdKitsSinalizacao).CarregarMensagem("Aviso", "Não Existem Kits de Sinalização Cadastrados.");
                            ((QuadroAviso)qdKitsSinalizacao).ClasseImagem = "icone-aviso";
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
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
        /// Carrega os materiais disponíveis para o estabelecimento
        /// </summary>
        private void CarregarMateriaisVenda()
        {
            using (Logger Log = Logger.IniciarLog("Carregando materias de venda"))
            {
                try
                {
                    using (var client = new MaterialVendaServico.MaterialVendaServicoClient())
                    {
                        MaterialVendaServico.Kit[] remessas = client.ConsultarKitsVendas(this.SessaoAtual.CodigoEntidade);

                        if (remessas.Length > 0)
                        {
                            rptKitsVendas.DataSource = remessas;
                            rptKitsVendas.DataBind();
                        }
                        else
                        {
                            pnlKitsVenda.Visible = false;
                            pnlVazioKitsVendas.Visible = true;
                            ((QuadroAviso)qdKitsVenda).CarregarMensagem("Aviso", "Não Existem Kits de Vendas Cadastrados");
                            ((QuadroAviso)qdKitsVenda).ClasseImagem = "icone-aviso";
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
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
    }
}