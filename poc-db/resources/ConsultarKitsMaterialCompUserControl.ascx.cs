#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [17/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.ServiceModel;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.ConsultarKitsMaterialComp
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ConsultarKitsMaterialCompUserControl : UserControlBase
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
                QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                Int32 codigoKit = Int32.Parse(queryString["Codigo"]);
                this.CarregarMateriaisComposicao(codigoKit);
            }
        }

        /// <summary>
        /// Carrega os materiais do kit disponíveis para o estabelecimento
        /// </summary>
        private void CarregarMateriaisComposicao(Int32 codigoKit)
        {
            using (Logger Log = Logger.IniciarLog("Carregando materiais do kit disponíveis para o estabelecimento"))
            {
                try
                {
                    using (var client = new MaterialVendaServico.MaterialVendaServicoClient())
                    {
                        MaterialVendaServico.Material[] materiais = client.ConsultarComposicaoKit(codigoKit);

                        if (materiais.Length > 0)
                        {
                            rptMateriais.DataSource = materiais;
                            rptMateriais.DataBind();
                        }
                        else
                        {
                            String urlVoltar = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_ConsultarKitsMaterial.aspx");

                            pnlComposicao.Visible = false;
                            pnlVazioMateriais.Visible = true;
                            ((QuadroAviso)qdMateriais).CarregarMensagem("Aviso", "Não Existem Materiais Cadastrados.", urlVoltar);
                            ((QuadroAviso)qdMateriais).ClasseImagem = "icone-aviso";
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
