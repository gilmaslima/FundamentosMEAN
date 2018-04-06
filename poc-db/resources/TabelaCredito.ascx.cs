using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using Redecard.PN.Comum;

namespace Rede.PN.CondicaoComercial.SharePoint.ControlTemplates.DadosBancarios
{
    /// <summary>
    /// Controle para preenchimento de Dados Bancários de Crédito
    /// </summary>
    public partial class TabelaCredito : UserControlBase
    {
        /// <summary>
        /// Carregamento do controle
        /// </summary>        
        /// <param name="agencia">Agência do banco</param>
        /// <param name="banco">Nome do banco</param>
        /// <param name="conta">Número da conta bancária</param>
        /// <param name="descricaoCartao">Descrição do cartão</param>
        /// <param name="tabelas">Tabelas com informações bancárias para preenchimento dos DataGrids</param>
        public void CarregarControle(String descricaoCartao, String banco, String agencia, String conta, DataSet tabelas)
        {            
            using (Logger Log = Logger.IniciarLog("Carregando Controle"))
            {
                try
                {
                    if (tabelas != null)
                    {
                        lblTitulo.Text = String.Format("PRODUTO {0}", descricaoCartao.Trim().ToUpper());
                        lblBanco.Text = banco.Trim().ToUpper();
                        lblAgencia.Text = agencia;
                        lblConta.Text = conta;
                        
                        //Carrega no DataGrid as informações preenchidas na tabela "ProdutosPagamentos" do DataSet o UserControl
                        dtgPagamentos.DataSource = tabelas;
                        dtgPagamentos.DataMember = "ProdutosPagamentos";

                        //Carrega no DataGrid as informações preenchidas na tabela "ProdutosFlex" do DataSet o UserControl
                        dtgProdutosFlex.DataSource = tabelas;
                        dtgProdutosFlex.DataMember = "ProdutosFlex";
                                                
                        dtgPagamentos.DataBind();
                        dtgProdutosFlex.DataBind();

                        if (dtgPagamentos.Items.Count == 0)
                        {
                            pnlPagamentos.Visible = false;
                            pnlDisclaimerPagamentos.Visible = false;
                        }

                        if (dtgProdutosFlex.Items.Count == 0)
                        {
                            pnlProdutosFlex.Visible = false;
                            pnlDisclaimerFlex.Visible = false;
                            pnlDisclaimerFlexDiasUteis.Visible = false;
                        }
                    }
                    else
                        this.Visible = false;
                }
                catch (ArgumentException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, 300);
                }
                catch (FormatException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, 300);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, 300);
                }
            }
        }     
    }
}
