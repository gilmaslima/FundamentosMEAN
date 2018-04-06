using Redecard.PN.Comum;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico;
using System.ServiceModel;
using Rede.PN.Eadquirencia.Sharepoint.CONTROLTEMPLATES.Rede.PN.Eadquirencia.Sharepoint;
using Rede.PN.Eadquirencia.Sharepoint.Helper;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.RelatorioVendas
{
    public partial class RelatorioVendasUserControl : WebpartBase
    {
        #region Métodos
        /// <summary>
        /// Retornar os filtros da tela
        /// </summary>
        /// <returns></returns>
        private FiltroRelatorioTransacoes RetornarFiltro()
        {
            FiltroRelatorioTransacoes filtro = ViewState["FiltroRelatorio"] as FiltroRelatorioTransacoes;

            if (filtro != null)
                return filtro;

            filtro = new FiltroRelatorioTransacoes();
#if !DEBUG
            if (!Sessao.Contem())
                throw new Exception("Falha ao obter sessão.");

            filtro.NumeroPv = SessaoAtual.CodigoEntidade;
#else
            filtro.NumeroPv = Convert.ToInt32(txtNumeroPv.Text);
#endif
            GridPaginacao paginacao = this.paginacao as GridPaginacao;
            if (paginacao == null)
                return filtro;

            filtro.FormasPagamento = new Dictionary<int, string>();
            filtro.Status = new Dictionary<int, string>();

            filtro.NumeroPagina = paginacao.PaginaAtual == 0 ? 1 : paginacao.PaginaAtual; //na primeira vez a página é zero
            filtro.ItensPorPagina = Convert.ToInt32(ddlItemsPorPagina.SelectedValue);

            if (rdbBuscaEspecifica.Checked)
            {
                filtro.Tid = txtNumeroTID.Text;
            }
            else
            {
                DateTime dataTemp;
                DateTime.TryParseExact(txtPeriodoInicio.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dataTemp);

                filtro.DataInicio = dataTemp;

                DateTime.TryParseExact(txtPeriodoFinal.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dataTemp);

                filtro.DataFim = dataTemp;

                foreach (ListItem listItem in msdFormaPagamento.SelectedItems)
                {
                    filtro.FormasPagamento.Add(Convert.ToInt32(listItem.Value), listItem.Text);
                }

                decimal valorTemp;
                Decimal.TryParse(txtNumeroNSU.Text, out valorTemp);

                filtro.Nsu = valorTemp;
                filtro.NumeroPedido = txtNumeroPedido.Text;
                filtro.Status = new Dictionary<int, string>();

                foreach (ListItem listItem in msdStatus.SelectedItems)
                {
                    filtro.Status.Add(Convert.ToInt32(listItem.Value), listItem.Text);
                }

                if (!msdTipoTransacao.TodosItensSelecionados && msdTipoTransacao.SelectedItems.Count() > 0)
                {
                    filtro.TiposTransacao = Convert.ToInt32(msdTipoTransacao.SelectedItems.First().Value);
                }

            }

            ViewState["FiltroRelatorio"] = filtro;

            return filtro;
        }

        /// <summary>
        /// Método para tratar data nula
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected String TratarDataNula(object data)
        {
            if (data == null)
                return " - ";

            DateTime valor = Convert.ToDateTime(data);

            return valor.ToString("dd/MM/yyyy");

        }
		
        /// <summary>
        /// Método para tratar campo vazio.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected String TratarCampoVazio(object data)
        {
            if (data == null || String.IsNullOrEmpty(data.ToString()))
                return " - ";

            return data.ToString();
        }

        /// <summary>
        /// Método para tratar campo vazio.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected String TratarCampoVazioDecimal(object valor)
        {
            if (valor == null || String.IsNullOrEmpty(valor.ToString()))
                return " - ";

            return String.Format("{0:0.00}", valor);
        }

        /// <summary>
        /// Monstar a string dos filtros
        /// </summary>
        /// <returns></returns>
        private String MontarStringFiltros()
        {
            StringBuilder sb = new StringBuilder();
            if (rdbBuscaEspecifica.Checked)
            {
                sb.Append("[TID]");
            }
            else
            {

                if (msdStatus.TodosItensSelecionados)
                {
                    sb.Append("[Todos os Status] ");
                }
                else
                {
                    foreach (ListItem item in msdStatus.SelectedItems)
                    {
                        sb.Append("[");
                        sb.Append(item.Text);
                        sb.Append("] ");
                    }
                }

                if (msdTipoTransacao.TodosItensSelecionados || msdTipoTransacao.SelectedItems.Count() == 0)
                    sb.Append("[Crédito] [Débito] ");
                else
                {
                    sb.Append("[");
                    sb.Append(msdTipoTransacao.SelectedItems.First().Text);
                    sb.Append("] ");
                }

                if (msdFormaPagamento.TodosItensSelecionados)
                    sb.Append("[Todas as Formas de Pagamento]");
                else
                {
                    foreach (ListItem item in msdFormaPagamento.SelectedItems)
                    {
                        sb.Append("[");
                        sb.Append(item.Text);
                        sb.Append("] ");
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Atualiza campos MultiSelectDropDown
        /// </summary>
        protected void CarregarMultiSelectDropDown()
        {
            Dictionary<Int32, String> listaStatus = new Dictionary<Int32, String>();
            Dictionary<Int32, String> listaFormaPgto = new Dictionary<Int32, String>();

            using (ContextoWCF<ServicoEAdquirenciaClient> adquirencia = new ContextoWCF<ServicoEAdquirenciaClient>())
            {
                //#if !DEBUG
                listaStatus = adquirencia.Cliente.ListarStatusTransacao();
                listaFormaPgto = adquirencia.Cliente.ListarFormasPagamento();
                //#else
                //                        listaStatus = new Dictionary<Int32, String>() { { 1, "Aprovado" }, { 2, "Pendente" }, { 3, "Não Aprovado" }, { 4, "Cancelado" }, { 5, "Desfeita" } };
                //                        listaFormaPgto = new Dictionary<Int32, String>() { { 1, "À vista" }, { 2, "Parcelado sem juros" }, { 3, "Parcelado com juros" } };
                //#endif
            }

            msdFormaPagamento.DataTextField = "Value";
            msdFormaPagamento.DataValueField = "Key";
            msdFormaPagamento.DataSource = listaFormaPgto;
            msdFormaPagamento.DataBind();

            msdStatus.DataTextField = "Value";
            msdStatus.DataValueField = "Key";
            msdStatus.DataSource = listaStatus;
            msdStatus.DataBind();
        }

        /// <summary>
        /// Buscar as transações e montar o relatório
        /// </summary>
        private void BuscarTransacoesEMontarRelatorio()
        {
            FiltroRelatorioTransacoes filtro = RetornarFiltro();
            RetornoRelatorioTransacoes relatorio = null;

            using (ContextoWCF<ServicoEAdquirenciaClient> adquirencia = new ContextoWCF<ServicoEAdquirenciaClient>())
            {
                relatorio = adquirencia.Cliente.BuscarRelatorioTransacoesAdquirencia(filtro);
            }

            if (relatorio != null && relatorio.Transacoes.Count > 0)
            {
                lblFiltros.Text = MontarStringFiltros();

                lblTotalTransacoes.Text = relatorio.QuantidadeTotalTransacoes.ToString();
                lblValorTotalAprovadas.Text = relatorio.ValorTotalTransacoesAprovadas.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
                lblValorTotalNaoAprovadas.Text = relatorio.ValorTotalTransacoesNaoAprovadas.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));

                GridPaginacao paginacao = this.paginacao as GridPaginacao;
                paginacao.NumeroPaginas = relatorio.QuantidadePaginas;
                paginacao.PaginaAtual = filtro.NumeroPagina;


                lstvGridResultados.DataSource = relatorio.Transacoes;
                lstvGridResultados.DataBind();

                divResultados.Visible = true;
            }
            else
            {
                divResultados.Visible = false; ;
                ExibirMensagemErro("Não foram encontradas transações com os parâmetros selecionados");
            }
        }

        /// <summary>
        /// Gerar o PDF
        /// </summary>
        /// <param name="exibirDetalhesTransacao"></param>
        private void GerarPDF(Boolean exibirDetalhesTransacao)
        {
            Byte[] pdf = null;
            FiltroRelatorioTransacoes filtro = RetornarFiltro();
            filtro.NumeroPagina = 1;
            filtro.ItensPorPagina = null;

            using (ContextoWCF<ServicoEAdquirenciaClient> adquirencia = new ContextoWCF<ServicoEAdquirenciaClient>())
            {
                pdf = adquirencia.Cliente.GerarPDFRelatorioVendas(filtro, exibirDetalhesTransacao);
            }

            String nomeArquivo = String.Format("attachment; filename=Relatorio_{0}.pdf", DateTime.Now.ToString("ddMMyyyy_HHmmss"));

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", nomeArquivo);
            Response.BinaryWrite(pdf);

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Exibir mensagem de erro padrão
        /// </summary>
        protected void ExibirMensagemErro(String msg)
        {
            base.ExibirPainelMensagem(msg);
        }

        /// <summary>
        /// Script para reposicionar modal
        /// </summary>
        protected void ReposicionarModal()
        {
            String script = "ReposicionarModal();";
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
        }
        #endregion

        #region Eventos
        /// <summary>
        /// Método do PAGE_LOAD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecucaoTratada("Relatorio de vendas - Page_Load", () =>
                {
					((GridPaginacao)paginacao).PaginacaoChange += paginacao_PaginacaoChange;
					
                    if (!IsPostBack)
                    {
                        CarregarMultiSelectDropDown();
                    }
                });
        }

        /// <summary>
        /// Exportar PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportarPdf_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Relatorio de vendas - btnExportarPdf_Click", () =>
            {
                divConfirmacao.Visible = true;
                StringBuilder sb = new StringBuilder();
                this.ExibirPainelHtml(sb.ToString());
            });
        }

        /// <summary>
        /// Exportar para excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Relatorio de vendas - btnExportarExcel_Click", () =>
            {
                Byte[] csv = null;

                FiltroRelatorioTransacoes filtro = RetornarFiltro();
                filtro.NumeroPagina = 1;
                filtro.ItensPorPagina = null;

                using (ContextoWCF<ServicoEAdquirenciaClient> adquirencia = new ContextoWCF<ServicoEAdquirenciaClient>())
                {
                    csv = adquirencia.Cliente.GerarCsvRelatorioVendas(filtro);
                }

                String nomeArquivo = String.Format("attachment; filename=Relatorio_{0}.csv", DateTime.Now.ToString("ddMMyyyy_HHmmss"));

                Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", nomeArquivo);
                Response.BinaryWrite(csv);

                Response.End();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            });
        }

        /// <summary>
        /// Imprimir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImprimir_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Evento do botão buscar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Relatorio de vendas - btnBuscar_Click", () =>
            {
                GridPaginacao paginacao = this.paginacao as GridPaginacao;
                paginacao.PaginaAtual = 1;
                ViewState["FiltroRelatorio"] = null; //reseta o filtro, para ler de todos os campos novamente
                divConfirmacao.Visible = false;
                BuscarTransacoesEMontarRelatorio();
            });
        }

        /// <summary>
        /// Evento do selecionar itens por página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlItemsPorPagina_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecucaoTratada("Relatorio de vendas - ddlItemsPorPagina_SelectedIndexChanged", () =>
                {
                    GridPaginacao paginacao = this.paginacao as GridPaginacao;
                    paginacao.PaginaAtual = 1;
                    FiltroRelatorioTransacoes filtro = ViewState["FiltroRelatorio"] as FiltroRelatorioTransacoes;

                    if (filtro != null)
                    {
                        filtro.NumeroPagina = 1;
                        filtro.ItensPorPagina = Convert.ToInt32(ddlItemsPorPagina.SelectedValue);
                    }

                    BuscarTransacoesEMontarRelatorio();
                });
        }

        /// <summary>
        /// Evento utilizado quando o usuário troca de página.
        /// </summary>
        protected void paginacao_PaginacaoChange(object sender, EventArgs e)
        {
            ExecucaoTratada("Relatorio de vendas - paginacao_PaginacaoChange", () =>
                {
                    FiltroRelatorioTransacoes filtro = ViewState["FiltroRelatorio"] as FiltroRelatorioTransacoes;
                    GridPaginacao paginacao = this.paginacao as GridPaginacao;

                    if (filtro != null)
                    {
                        filtro.NumeroPagina = paginacao.PaginaAtual;
                    }

                    BuscarTransacoesEMontarRelatorio();
                });
        }

        /// <summary>
        /// Em caso de postback da página e pelo menos um item na tabela, monta a paginação.
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            ExecucaoTratada("Relatorio de vendas - paginacao_PaginacaoChange", () =>
                {
                    if (lstvGridResultados.Items.Count > 0)
                        ((GridPaginacao)paginacao).CarregaPaginacao();
                });
        }

        /// <summary>
        /// Gera o PDF com os detalhes
        /// </summary>
        protected void btnExportarPDFComDetalhes_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Relatorio de vendas - btnExportarPDFComDetalhes_Click", () =>
                {
                    divConfirmacao.Visible = false;
                    GerarPDF(true);
                });
        }

        /// <summary>
        /// Gera o PDF sem os detalhes.
        /// </summary>
        protected void btnExportarDFSimples_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Relatorio de vendas - btnExportarDFSimples_Click", () =>
                {
                    divConfirmacao.Visible = false;
                    GerarPDF(false);
                });
        }
        #endregion
    }
}
