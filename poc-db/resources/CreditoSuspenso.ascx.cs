using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.RelatorioCreditoSuspensosRetidosPenhorados
{
    public partial class CreditoSuspenso : BaseUserControl, IControlesParaDownload
    {
        #region Attribute
        public Redecard.PN.Extrato.SharePoint.ControlTemplates.Paginacao objPaginacao;
        private List<SPListaPadrao> objListaBanco;
        public Servico.CSP.ConsultarSuspensaoTotaisRetorno Totais { get; private set; }
        #endregion

        #region Event
        protected void Page_Load(object sender, EventArgs e)
        {
            objPaginacao.onPaginacaoChanged += new Paginacao.PaginacaoChanged(objPaginacao_onPaginacaoChanged);
            objPaginacao.RegistrosPorPagina = ObterQuantidadeRegistrosPorPagina();
            this.Totais = new Servico.CSP.ConsultarSuspensaoTotaisRetorno()
            {
                TotalTransacoes = 0,
                TotalValorSuspencao = 0
            };
        }

        protected void ddlRegistroPorPagina_SelectedIndexChanged(Object sender, Int32 selectedSize)
        {
            objPaginacao.PaginaAtual = 1;
        }

        void objPaginacao_onPaginacaoChanged(int pagina, EventArgs e)
        {

        }

        protected void gridViewDados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Servico.CSP.ConsultarSuspensaoDetalheRetorno objItem = e.Row.DataItem as Servico.CSP.ConsultarSuspensaoDetalheRetorno;

                SPListaPadrao objBanco = objListaBanco.Find(base.SPListaPadraoPorValor(objItem.CodigoBanco.ToString()));

                e.Row.Cells[8].Text = (objBanco == null ? objItem.CodigoBanco.ToString() : objBanco.Titulo.PadLeft(10, ' ').Substring(0, 10).TrimStart(' ')) + "/" + objItem.CodigoAgencia.ToString() + "/" + objItem.NumeroConta;
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Text = "total no período";
                e.Row.Cells[7].Text = this.Totais.TotalValorSuspencao.ToString("N2");
                e.Row.Cells[0].ColumnSpan = 7;
                e.Row.Cells.RemoveAt(6);
                e.Row.Cells.RemoveAt(5);
                e.Row.Cells.RemoveAt(4);
                e.Row.Cells.RemoveAt(3);
                e.Row.Cells.RemoveAt(2);
                e.Row.Cells.RemoveAt(1);
            }
        }
        #endregion

        #region Method
        public List<Control> ObterControlesParaDownload()
        {
            List<Control> result = new List<Control>();
            if (divTituloComDados.Visible && this.gridViewDados.Visible)
            {
                result.Add(this.divTituloComDados);
                if (this.gridViewDados.Controls.Count > 0)
                {
                    result.Add(this.gridViewDados.Controls[0]);
                }
            }
            else if (divTituloComDados.Visible)
            {
                result.Add(this.divTituloComDados);
            }
            return result;
        }

        private Servico.CSP.ConsultarSuspensaoEnvio TradudorEnvioSPParaServico(BuscarDados buscarDados)
        {
            Servico.CSP.ConsultarSuspensaoEnvio objEnvio = new Servico.CSP.ConsultarSuspensaoEnvio();
            objEnvio.DataInicial = buscarDados.DataInicial;
            objEnvio.DataFinal = buscarDados.DataFinal;
            objEnvio.CodigoBandeira = buscarDados.CodigoBandeira;
            objEnvio.Estabelecimentos = buscarDados.Estabelecimentos;

            return objEnvio;
        }

        /// <summary>
        /// Faz a consulta dos dados
        /// </summary>
        /// <param name="buscarDados">Objeto BuscarDados</param>
        /// <param name="pagina">Pagina para recuperar. Se for -1, pega a página atual do objeto paginador</param>
        /// <param name="qtdLinhasPorPagina">Quantidade de linhas por página. Se for -1 pega do combo de linhas por página</param>
        /// <param name="savePaginaAtual">Devolve página atual marcada no paginador antes de realizar a consulta</param>
        /// <param name="saveQtdRegistrosPorPagina">Devolve a quantidade de registros por pagina atual antes de realizar a consulta</param>
        public void Consultar(BuscarDados buscarDados, int pagina, int qtdLinhasPorPagina, out Int32 savePaginaAtual, out Int32 saveQtdRegistrosPorPagina)
        {
            savePaginaAtual = objPaginacao.PaginaAtual;
            saveQtdRegistrosPorPagina = ObterQuantidadeRegistrosPorPagina();

            if (pagina == -1)
            {
                pagina = objPaginacao.PaginaAtual;
            }
            if (qtdLinhasPorPagina == -1)
            {
                qtdLinhasPorPagina = ObterQuantidadeRegistrosPorPagina();
            }

            Consultar(buscarDados, pagina, qtdLinhasPorPagina);
        }

        private void Consultar(BuscarDados buscarDados, int pagina, int qtdLinhasPagina)
        {
            using (Logger Log = Logger.IniciarLog("Créditos Suspensos"))
            {
                Servico.CSP.StatusRetorno objStatusRetorno;
                Servico.CSP.ConsultarSuspensaoRetorno objRetorno;

                Servico.CSP.BasicContract[] objDetalhe;

                using (var contexto = new ContextoWCF<Servico.CSP.RelatorioCreditoSuspensosRetidosPenhoradosClient>())
                {
                    //DadosPesquisa = buscarDados;

                    objRetorno = contexto.Cliente.ConsultarSuspensaoPesquisaCredito(out objStatusRetorno, TradudorEnvioSPParaServico(buscarDados), pagina, qtdLinhasPagina, GuidPesquisa(), GuidUsuario());

                    //trata retorno do serviço

                    bool retornoSemDados = objStatusRetorno.CodigoRetorno == 10;
                    bool retornoComDados = objStatusRetorno.CodigoRetorno == 0;

                    divTituloComDados.Visible = retornoComDados;

                    if (objStatusRetorno.CodigoRetorno != 0)
                    {
                        if (!retornoSemDados)
                        {
                            throw new PortalRedecardException(objStatusRetorno.CodigoRetorno, objStatusRetorno.Fonte);
                        }
                        return;
                    }

                    objDetalhe = objRetorno.Registros;
                }

                this.Totais = objRetorno.Totais;

                //preenche a lista de bancos
                objListaBanco = base.GetListaSP(Constantes.Extrato_Lista_Banco);

                objPaginacao.QuantidadeTotalRegistros = objRetorno.QuantidadeTotalRegistros;
                objPaginacao.PaginaAtual = pagina;

                gridViewDados.DataSource = objDetalhe;
                gridViewDados.DataBind();
            }
        }

        private int ObterQuantidadeRegistrosPorPagina()
        {
            return ddlRegistroPorPagina.SelectedSize;
        }
        #endregion
    }
}
