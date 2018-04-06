using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using Redecard.PN.Comum;

namespace Rede.PN.CondicaoComercial.SharePoint.ControlTemplates.DadosBancarios
{
    public partial class TabelaDebito : UserControlBase
    {
        /// <summary>
        /// Código do Cartão
        /// </summary>
        public int CodigoCartao { get; set; }
        
        /// <summary>
        /// Descrição do Cartão
        /// </summary>
        public String DescricaoCartao { get; set; }

        /// <summary>
        /// Nome do Banco
        /// </summary>
        public String Banco { get; set; }

        /// <summary>
        /// Agência do Banco
        /// </summary>
        public String Agencia { get; set; }

        /// <summary>
        /// Número da Conta Bancária
        /// </summary>
        public String Conta { get; set; }

        /// <summary>
        /// Tabelas de Débito para bind no DataGrid
        /// </summary>
        public DataSet Tabelas
        { get; set; }

        /// <summary>
        /// Inicialização da Telas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Carregando página"))
            {
                try
                {
                    if (Tabelas != null)
                    {
                        lblTitulo.Text = String.Format("PRODUTO {0}", DescricaoCartao.ToUpper());
                        lblBanco.Text = Banco.ToUpper();
                        lblAgencia.Text = Agencia;
                        lblCC.Text = Conta;

                        dtgPagamento.DataSource = Tabelas;
                        dtgPagamento.DataMember = "ProdutoPagamento";

                        dtgParcelamento.DataSource = Tabelas;
                        dtgParcelamento.DataMember = "ProdutoParcelamento";

                        dtgPagamento.DataBind();
                        dtgParcelamento.DataBind();

                        if (dtgPagamento.Items.Count == 0)
                            dtgPagamento.Visible = false;

                        if (dtgParcelamento.Items.Count == 0)
                        {
                            dtgParcelamento.Visible = false;
                            pnlCalculoProrata.Visible = false;
                        }
                    }
                    else
                        this.Visible = false;
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
