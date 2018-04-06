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
    public partial class Retidos : BaseUserControl, IControlesParaDownload
    {
        #region Attribute
        public Redecard.PN.Extrato.SharePoint.ControlTemplates.Paginacao objPaginacao;
        // Total de Valores a Reter
        public Decimal TotalValoresReter { get; set; }
        // Total de Valores Retidos  
        public Decimal TotalValoresRetidos { get; set; }
        #endregion

        #region Event
        protected void Page_Load(object sender, EventArgs e)
        {
            objPaginacao.onPaginacaoChanged += new Paginacao.PaginacaoChanged(objPaginacao_onPaginacaoChanged);
            objPaginacao.RegistrosPorPagina = ObterQuantidadeRegistrosPorPagina();
            this.TotalValoresReter = 0;
            this.TotalValoresRetidos = 0;
        }

        void objPaginacao_onPaginacaoChanged(int pagina, EventArgs e)
        {
        }

        protected void ddlRegistroPorPagina_SelectedIndexChanged(Object sender, Int32 selectedSize)
        {
            objPaginacao.PaginaAtual = 1;
        }

        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Servico.CSP.BasicContract di = (Servico.CSP.BasicContract)e.Item.DataItem;
            if ((e.Item.ItemType == ListItemType.Item) ||
                (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                String tipoLinha = di.TipoRegistro.Split('_')[0];

                if (tipoLinha == "D1")
                {
                    tipoLinha = "DCT";
                }
                else if (tipoLinha == "D2")
                {
                    tipoLinha = "DDT";
                }

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

                switch (di.TipoRegistro)
                {
                    case "PR":
                        {
                            if (e.Item.ItemIndex > 0)
                            {
                                ((HtmlTableRow)e.Item.FindControl("SPACE")).Visible = true;
                            }
                            Servico.CSP.ConsultarRetencaoNumeroProcessoRetorno obj = (Servico.CSP.ConsultarRetencaoNumeroProcessoRetorno)di;
                            (e.Item.FindControl("spnTituloNumeroProcesso") as HtmlGenericControl).InnerText = obj.NumeroProcesso;
                            (e.Item.FindControl("spnTituloNumeroProcessoValorTotal") as HtmlGenericControl).InnerText = obj.ValorTotalProcesso.ToString("N2");
                        }
                        break;
                    case "DC"://OK
                        {
                            Servico.CSP.ConsultarRetencaoDetalheProcessoCreditoRetorno obj = (Servico.CSP.ConsultarRetencaoDetalheProcessoCreditoRetorno)di;
                            tr.Cells[0].InnerText = obj.NumeroEstabelecimento.ToString();
                            tr.Cells[1].InnerText = obj.DataProcesso.ToString("dd/MM/yy");
                            tr.Cells[2].InnerText = obj.DataApresentacao.ToString("dd/MM/yy");
                            tr.Cells[3].InnerText = obj.DataVencimento.ToString("dd/MM/yy");
                            tr.Cells[4].InnerText = obj.DescricaoResumo;
                            tr.Cells[5].InnerText = obj.TipoBandeira;
                            tr.Cells[6].InnerText = obj.NumeroRV.ToString();
                            tr.Cells[7].InnerText = obj.QuantidadeTransacoes.ToString();
                            tr.Cells[8].InnerText = obj.ValorRetencao.ToString("N2");
                        }
                        break;
                    case "DCT_D1":
                    case "DDT_D1":
                    case "D1":
                        {
                            Servico.CSP.ConsultarRetencaoDescricaoComValorRetorno obj = (Servico.CSP.ConsultarRetencaoDescricaoComValorRetorno)di;
                            tr.Cells[0].InnerText = obj.DescricaoResumo.ToLowerInvariant();
                            tr.Cells[1].InnerText = obj.ValorRetencao.ToString("N2");
                        }
                        break;
                    case "DCT_D2":
                    case "DDT_D2":
                    case "D2":
                        {
                            Servico.CSP.ConsultarRetencaoDescricaoSemValorRetorno obj = (Servico.CSP.ConsultarRetencaoDescricaoSemValorRetorno)di;
                            tr.Cells[0].InnerText = obj.DescricaoResumo.ToLowerInvariant();
                            tr.Cells[1].InnerText = obj.ValorRetencao.ToString("N2");
                        }
                        break;
                    case "DD"://OK
                        {
                            Servico.CSP.ConsultarRetencaoDetalheProcessoDebitoRetorno obj = (Servico.CSP.ConsultarRetencaoDetalheProcessoDebitoRetorno)di;
                            tr.Cells[0].InnerText = obj.NumeroEstabelecimento.ToString();
                            tr.Cells[1].InnerText = obj.NumeroRV.ToString();
                            tr.Cells[2].InnerText = obj.DescricaoResumo;
                            tr.Cells[3].InnerText = obj.ValorRetencao.ToString("N2");
                        }
                        break;
                }

            }

        }
        #endregion

        #region Method

        /// <summary>
        /// Altera primeira letra da String pata maiusculo
        /// </summary>
        /// <param name="s">String</param>
        /// <returns>String com a primeira letra maiscula e o resto miniscula</returns>
        static string UppercaseFirst(String s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public List<Control> ObterControlesParaDownload()
        {
            List<Control> result = new List<Control>();
            if (divTituloComDados.Visible)
            {
                result.Add(divTituloComDados);
            }
            if (rptDados.Visible)
            {
                result.Add(this.rptDados);
            }
            return result;
        }

        private Servico.CSP.ConsultarRetencaoEnvio TradutorEnvioSPParaServico(BuscarDados buscarDados)
        {
            Servico.CSP.ConsultarRetencaoEnvio objEnvio = new Servico.CSP.ConsultarRetencaoEnvio();
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
            using (Logger.IniciarLog("Retidos"))
            {
                Servico.CSP.ConsultarRetencaoRetorno objRetorno;
                Servico.CSP.ConsultarRetencaoEnvio objEnvio = TradutorEnvioSPParaServico(buscarDados);

                using (var contexto = new ContextoWCF<Servico.CSP.RelatorioCreditoSuspensosRetidosPenhoradosClient>())
                {
                    GravarBuscarDados(buscarDados);

                    Servico.CSP.StatusRetorno objStatusRetorno;
                    objRetorno = contexto.Cliente.ConsultarRetencaoPesquisa(
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
                            this.TotalValoresReter = 0;
                            this.TotalValoresRetidos = 0;
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

                    objPaginacao.QuantidadeTotalRegistros = objRetorno.QuantidadeTotalRegistros;
                    objPaginacao.PaginaAtual = pagina;

                    this.TotalValoresReter = objRetorno.Totais.TotalValorProcesso;
                    this.TotalValoresRetidos = objRetorno.Totais.TotalValorRetencao;

                    // Adiciona dados
                    rptDados.DataSource = objRetorno.Registros;
                    rptDados.DataBind();
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
