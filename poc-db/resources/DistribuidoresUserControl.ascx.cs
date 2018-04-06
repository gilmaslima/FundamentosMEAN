using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.DataCash.SharePoint.DataCashService;
using System.Collections.Generic;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.Text;

namespace Redecard.PN.DataCash.SharePoint.WebParts.Distribuidores
{
    public partial class DistribuidoresUserControl : UserControlBaseDataCash
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Início Página Distribuídores"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        using (DataCashService.DataCashServiceClient clienteServico = new DataCashService.DataCashServiceClient())
                        {
                            MensagemErro mensagemErro;
                            Boolean pvFornecedor = clienteServico.PerfilPVFornecedor(out mensagemErro, base.SessaoAtual.CodigoEntidade);

                            if (!pvFornecedor || mensagemErro.CodigoRetorno != 0)
                            {
                                pnlSemDistribuidores.Visible = true;
                                ((QuadroAviso)qdAviso).CarregarMensagem("Aviso", "Não há distribuidores cadastrados.");
                            }
                            else
                                this.paginacao.Carregar();
                        }

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

        private void CarregarDistribuidores()
        {
            using (Logger log = Logger.IniciarLog("Carregando a tela com Distribuidores"))
            {
                try
                {
                    Int32 codigoRetorno = 0;
                    //TODO: Chamar o Serviço
                    using (DataCashService.DataCashServiceClient clienteServico = new DataCashService.DataCashServiceClient())
                    {
                        DataCashService.RetornoDistribuidor retornoDistribuidores = clienteServico.ConsultarDistribuidores(base.SessaoAtual.CodigoEntidade, 1);

                        if (codigoRetorno != 0)
                            base.ExibirPainelExcecao("DataCashServico.ConsultarDistribuidores", codigoRetorno);
                        else
                        {
                            if (retornoDistribuidores.Distribuidores.Count > 0)
                            {
                                rptDistribuidores.DataSource = retornoDistribuidores.Distribuidores;
                                rptDistribuidores.DataBind();

                                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tabelaDistribuidores', 1, 10, 5);", true);
                            }
                            else
                            {
                                pnlSemDistribuidores.Visible = !(rptDistribuidores.Visible = false);
                                ((QuadroAviso)qdAviso).CarregarMensagem("Aviso", "Não há distribuidores cadastrados.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        protected void rptDistribuidores_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Renderização dos itens de Distribuidores"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        var distribuidor = (DataCashService.RegistroDistribuidor)e.Item.DataItem;
                        Label lblCodigo = (Label)e.Item.FindControl("lblCodigo");
                        Label lblDescricao = (Label)e.Item.FindControl("lblDescricao");

                        lblCodigo.Text = distribuidor.NumPdvDistribuidor.ToString().PadLeft(6, '0');
                        lblDescricao.Text = distribuidor.NomeDistribuidor.Trim();
                    }
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
        protected IEnumerable<Object> Paginacao_ObterDados(Guid IdPesquisa, int registroInicial, int quantidadeRegistros, int quantidadeRegistroBuffer, out int quantidadeTotalRegistrosEmCache, params object[] parametros)
        {
            IEnumerable<Object> retorno = new List<Object>();
            quantidadeTotalRegistrosEmCache = 0;
            using (Logger log = Logger.IniciarLog("Carregando a tela com Distribuidores"))
            {
                try
                {
                    // para contornar a mensagem de erro quando clicado em >>
                    if (quantidadeRegistros == 0)
                    {
                        quantidadeRegistros = ((Paginacao)this.paginacao).Configuracao.TamanhoPagina;
                    }
                    Int32 codigoRetorno = 0;
                                        
                    //TODO: Chamar o Serviço
                    using (DataCashService.DataCashServiceClient clienteServico = new DataCashService.DataCashServiceClient())
                    {
                        Int32 paginaAtual = (registroInicial / quantidadeRegistros) + 1;

                        DataCashService.RetornoDistribuidor retornoDistribuidores = clienteServico.ConsultarDistribuidores(base.SessaoAtual.CodigoEntidade, paginaAtual);

                        if (codigoRetorno != 0)
                            base.ExibirPainelExcecao("DataCashServico.ConsultarDistribuidores", codigoRetorno);
                        else
                        {
                            if (retornoDistribuidores.Distribuidores != null && retornoDistribuidores.Distribuidores.Count > 0)
                            {
                                quantidadeTotalRegistrosEmCache = retornoDistribuidores.QuantidadePagina * quantidadeRegistros;

                                retorno = retornoDistribuidores.Distribuidores.ToArray();
                                //rptDistribuidores.DataBind();

                                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tabelaDistribuidores', 1, 10, 5);", true);
                            }
                            else
                            {
                                pnlSemDistribuidores.Visible = !(rptDistribuidores.Visible = false);
                                ((QuadroAviso)qdAviso).CarregarMensagem("Aviso", "Não há distribuidores cadastrados.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                return retorno;
            }
        }
    }
}
