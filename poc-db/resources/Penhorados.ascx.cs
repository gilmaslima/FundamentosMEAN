using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using System.Web.UI.HtmlControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.RelatorioCreditoSuspensosRetidosPenhorados
{
    public partial class Penhorados : BaseUserControl, IControlesParaDownload
    {
        #region Attribute
        public Redecard.PN.Extrato.SharePoint.ControlTemplates.Paginacao objPaginacao;
        // Total de Valores a Penhorar
        public Decimal TotalValoresPenhorar { get; set; }
        // Total de Valores Penhorados
        public Decimal TotalValoresPenhorados { get; set; }
        #endregion

        #region Event
        protected void Page_Load(object sender, EventArgs e)
        {
            objPaginacao.onPaginacaoChanged += new Paginacao.PaginacaoChanged(objPaginacao_onPaginacaoChanged);
            objPaginacao.RegistrosPorPagina = ObterQuantidadeRegistrosPorPagina();
            this.TotalValoresPenhorados = 0;
            this.TotalValoresPenhorar = 0;
        }

        protected void ddlRegistroPorPagina_SelectedIndexChanged(Object sender, Int32 selectedSize)
        {
            objPaginacao.PaginaAtual = 1;
        }

        void objPaginacao_onPaginacaoChanged(int pagina, EventArgs e)
        {

        }

        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Servico.CSP.BasicContract di = (Servico.CSP.BasicContract)e.Item.DataItem;
            if ((e.Item.ItemType == ListItemType.Item) ||
                (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                String tipoLinha = di.TipoRegistro.Split('_')[0];

                HtmlTableRow tr = (HtmlTableRow)e.Item.FindControl(tipoLinha);

                // Se não encontrar o tipo da linha, coloca na tela para visualizarmos o tipo com erro
                if (tr == null)
                {
                    tr = (HtmlTableRow)e.Item.FindControl("SPACE");
                    tr.Visible = true;
                    tr.Cells[0].InnerText = di.TipoRegistro + " - " + di.GetType().FullName;
                    return;
                }

                tr.Visible = true;

                // PR - ConsultarPenhoraNumeroProcessoRetorno
                // HDT (Header)
                // DT - ConsultarPenhoraDetalheProcessoCreditoRetorno
                // T1 - ConsultarPenhoraTotalBandeiraRetorno
                // TP - ConsultarPenhoraTotalSemBandeiraRetorno
                switch (di.TipoRegistro)
                {
                    case "PR":
                        {
                            if (e.Item.ItemIndex > 0)
                            {
                                ((HtmlTableRow)e.Item.FindControl("SPACE")).Visible = true;
                            }

                            Servico.CSP.ConsultarPenhoraNumeroProcessoRetorno obj = (Servico.CSP.ConsultarPenhoraNumeroProcessoRetorno)di;
                            (e.Item.FindControl("spnTituloNumeroProcesso") as HtmlGenericControl).InnerText = obj.NumeroProcesso;
                            (e.Item.FindControl("spnTituloNumeroProcessoValorTotal") as HtmlGenericControl).InnerText = obj.ValorTotalProcesso.ToString("N2");
                        }
                        break;
                    case "HDT":
                        break;
                    case "DT":
                        {
                            Servico.CSP.ConsultarPenhoraDetalheProcessoCreditoRetorno obj = (Servico.CSP.ConsultarPenhoraDetalheProcessoCreditoRetorno)di;
                            tr.Cells[0].InnerText = obj.NumeroEstabelecimento.ToString();
                            tr.Cells[1].InnerText = obj.DataProcesso.ToString("dd/MM/yy");
                            tr.Cells[2].InnerText = obj.DataApresentacao.ToString("dd/MM/yy");
                            tr.Cells[3].InnerText = obj.DataVencimento.ToString("dd/MM/yy");
                            tr.Cells[4].InnerText = obj.DescricaoResumo;
                            tr.Cells[5].InnerText = obj.TipoBandeira;
                            tr.Cells[6].InnerText = obj.NumeroRV.ToString();
                            tr.Cells[7].InnerText = obj.QuantidadeTransacoes.ToString();
                            tr.Cells[8].InnerText = obj.ValorPenhorado.ToString("N2");
                            //tr.Cells[8].Attributes.Add("style", "text-align:right;");
                        }
                        break;
                    case "T1":
                        {
                            Servico.CSP.ConsultarPenhoraTotalBandeiraRetorno obj = (Servico.CSP.ConsultarPenhoraTotalBandeiraRetorno)di;
                            tr.Cells[0].InnerText = obj.DescricaoResumo;
                            tr.Cells[1].InnerText = obj.ValorPenhorado.ToString("N2");
                            //tr.Cells[1].Attributes.Add("style", "text-align:right;");
                        }
                        break;
                    case "TP":
                        {
                            Servico.CSP.ConsultarPenhoraTotalSemBandeiraRetorno obj = (Servico.CSP.ConsultarPenhoraTotalSemBandeiraRetorno)di;
                            tr.Cells[0].InnerText = obj.DescricaoResumo;
                            tr.Cells[1].InnerText = obj.ValorPenhorado.ToString("N2");
                            //tr.Cells[1].Attributes.Add("style", "text-align:right;");
                        }
                        break;
                }
            }

        }
        #endregion

        #region Method
        public List<Control> ObterControlesParaDownload()
        {
            List<Control> result = new List<Control>();
            if (divTituloComDados.Visible)
            {
                result.Add(divTituloComDados);
            }
            if (rptDados.Visible)
            {
                result.Add(rptDados);
            }
            return result;
        }

        private Servico.CSP.ConsultarPenhoraEnvio TradutorEnvioSPParaServico(BuscarDados buscarDados)
        {
            Servico.CSP.ConsultarPenhoraEnvio objEnvio = new Servico.CSP.ConsultarPenhoraEnvio();
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


        private void Consultar(BuscarDados buscarDados, int pagina, int qtdRegistrosPorPagina)
        {
            using (Logger Log = Logger.IniciarLog("Penhorados"))
            {
                Servico.CSP.ConsultarPenhoraRetorno objRetorno;
                List<Servico.CSP.BasicContract> registros;
                Servico.CSP.ConsultarPenhoraEnvio objEnvio = TradutorEnvioSPParaServico(buscarDados);

                using (var contexto = new ContextoWCF<Servico.CSP.RelatorioCreditoSuspensosRetidosPenhoradosClient>())
                {
                    GravarBuscarDados(buscarDados);

                    Servico.CSP.StatusRetorno objStatusRetorno;
                    objRetorno = contexto.Cliente.ConsultarPenhoraPesquisa(
                        out objStatusRetorno,
                        objEnvio,
                        pagina,
                        qtdRegistrosPorPagina,
                        GuidPesquisa(), GuidUsuario()
                    );
                    if (objStatusRetorno.CodigoRetorno != 0) // Se houve algum retorno de erro
                    {
                        if (objStatusRetorno.CodigoRetorno == 10) // erro sem dados só esconde o relatório
                        {
                            MostrarResultadoRelatorio(false);
                            this.TotalValoresPenhorados = 0;
                            this.TotalValoresPenhorar = 0;

                            divTituloComDados.Visible = false;

                            return;
                        }
                        else
                        {
                            throw new PortalRedecardException(objStatusRetorno.CodigoRetorno, objStatusRetorno.Fonte);
                        }
                    }

                    // Se chegou aqui exibe relatório
                    MostrarResultadoRelatorio(true);

                    rptDados.DataSource = objRetorno.Registros;
                    rptDados.DataBind();

                    objPaginacao.QuantidadeTotalRegistros = objRetorno.QuantidadeTotalRegistros;
                    objPaginacao.PaginaAtual = pagina;

                    this.TotalValoresPenhorados = objRetorno.Totais.TotalValorPenhorado;
                    this.TotalValoresPenhorar = objRetorno.Totais.TotalValorProcesso;

                    divTituloComDados.Visible = true;
                }
            }
        }


        private void MostrarResultadoRelatorio(bool FlagMostrar)
        {
            if (objPaginacao != null)
            {
                objPaginacao.Visible = FlagMostrar;
            }
            rptDados.Visible = FlagMostrar;
        }

        private int ObterQuantidadeRegistrosPorPagina()
        {
            return ddlRegistroPorPagina.SelectedSize;
        }
        #endregion
    }
}
