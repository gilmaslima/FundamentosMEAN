#region Histórico do Arquivo
/*
(c) Copyright [2015] Rede S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [09/02/2015] – [Agnaldo Costa] – [Criação]
*/
#endregion 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.OutrosServicos.SharePoint.ControlTemplates.OutrosServicos.Corban;
using Redecard.PN.OutrosServicos.SharePoint.Modelos;
using Redecard.PN.OutrosServicos.SharePoint.WaOutrosServicos;
using Redecard.PN.Outro.Core.Web.Controles.Portal;

namespace Redecard.PN.OutrosServicos.SharePoint.WebPartsCorban.HistoricoTransacoesCorban
{
    /// <summary>
    /// Webpart de Histórico de Transações Corban
    /// </summary>
    public partial class HistoricoTransacoesCorbanUserControl : UserControlBase
    {
        #region [Controles WebPart]

        /// <summary>
        /// UserControl PgnTrasacoes
        /// </summary>
        protected Paginador PgnTrasacoes { get { return (Paginador)pgnTrasacoes; } }
        
        #endregion

        #region [Eventos da WebPart]

        /// <summary>
        /// Inicialiazação da WebPart
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            PgnTrasacoes.RegistrosPorPagina = ddlRegistroPorPagina.SelectedValue.ToInt32(30);
        }

        /// <summary>
        /// Ação de busca de transações Corban
        /// </summary>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Ação de busca de transações Corban"))
            {
                try
                {
                    var dados = new DadosBuscaCorban();

                    DateTime dataInicio = default(DateTime);
                    DateTime dataFim = default(DateTime);

                    if (!DateTime.TryParse(txtDataInicial.Text, out dataInicio))
                    {
                        base.ExibirPainelExcecao("Data inicial inválida.", "312");
                        return;
                    }

                    if (!DateTime.TryParse(txtDataFinal.Text, out dataFim))
                    {
                        base.ExibirPainelExcecao("Data final inválida.", "312");
                        return;
                    }

                    if (dataInicio > dataFim)
                    {
                        base.ExibirPainelExcecao("O período informado está inválido. O período inicial não pode ser superior ao período final.", "313");
                        return;
                    }

                    if ((dataFim - dataInicio).TotalDays > 30)
                    {
                        base.ExibirPainelExcecao("O período informado está inválido. O período não pode ser superior a 30 dias. Por favor, selecione outro período.", "311");
                        return;
                    }

                    dados.DataInicial = dataInicio;
                    dados.DataFinal = dataFim;
                    dados.CodigoTipoConta = ddlTipoConta.SelectedValue.ToInt16(default(Int16));
                    dados.CodigoFormaPagamento = ddlFormaPagamento.SelectedValue.ToInt16(default(Int16));
                    dados.CodigoStatusTransacao = ddlStatus.SelectedValue.FirstOrDefault();
                    dados.Estabelecimentos = consultaPV.PVsSelecionados.ToArray();
                    dados.CodigoServico = txtCodigoServico.Text.ToDecimalNull(0).Value;

                    //Gera uma nova chave de cache
                    GuidPesquisa = Guid.NewGuid();

                    //Após fazer a validações, executa a consulta e carrega dados na tela
                    this.Consultar(dados, 1, ddlRegistroPorPagina.SelectedValue.ToInt32Null(30).Value, PgnTrasacoes.PaginasVirtuais);

                }
                catch (FaultException<WaOutrosServicos.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                   
            }
        }

        /// <summary>
        /// Voltar para a seleção de menu
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Redireciona o usuário para a seleção de menu"))
            {
                try
                {
                    String urlVoltar = String.Format("{0}/Paginas/pn_ConsultaSolicitacoes.aspx", base.web.ServerRelativeUrl);

                    this.Response.Redirect(urlVoltar, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Troca de tamanho da quantidade de registros por página
        /// </summary>
        protected void ddlRegistroPorPagina_SelectedIndexChanged(object sender, Int32 selectedValue)
        {
            Consultar(this.ObterDadosBusca(),
                      1,
                      selectedValue,
                      PgnTrasacoes.PaginasVirtuais);
        }

        /// <summary>
        /// Mudança de página da transação
        /// </summary>
        protected void pgnTrasacoes_onPaginacaoChanged(Int32 pagina, EventArgs e)
        {
            Consultar(this.ObterDadosBusca(),
                      pagina,
                      ddlRegistroPorPagina.SelectedValue.ToInt32(10),
                      PgnTrasacoes.PaginasVirtuais);
        }

        /// <summary>
        /// Cache de todas as páginas
        /// </summary>
        protected void pgnTransacoes_CacheTodosRegistros()
        {
            Consultar(this.ObterDadosBusca(),
                      1,
                      0,
                      Int32.MaxValue);
        }

        /// <summary>
        /// Monta os itens do repeater de bandeiras
        /// </summary>
        protected void repBandeira_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Monta os itens do repeater de bandeiras"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        var imgBandeira = e.Item.FindControl("imgBandeira") as Image;
                        var lblNomeBandeira = e.Item.FindControl("lblNomeBandeira") as Literal;
                        var lblValorBandeira = e.Item.FindControl("lblValorBandeira") as Literal;

                        var bandeira = e.Item.DataItem as BandeiraTransacao;

                        if (bandeira != null)
                        {
                            log.GravarMensagem("Total por bandeira", bandeira);

                            //Verifica se a bandeira corresponde ao tipo "dinheiro" (mainframe retorna hífen nesta situação)
                            Boolean bandeiraDinheiro = String.Compare("-", bandeira.Descricao.Trim(), true) == 0;

                            //Monta endereço da imagem, nome e valor da bandeira
                            //Se for dinheiro, exibe imagem customizada
                            if (bandeiraDinheiro)
                                imgBandeira.ImageUrl = "/_layouts/OutrosServicos/Corban/ico_dinheiro.png";
                            else
                                imgBandeira.ImageUrl = String.Format("/_layouts/Redecard.PN.Extrato.SharePoint/Styles/ico_{0}.jpg", 
                                    bandeira.Descricao.Trim());

                            lblNomeBandeira.Text = bandeiraDinheiro ? "Dinheiro" : bandeira.Descricao;
                            lblValorBandeira.Text = bandeira.Valor.ToString("C", new CultureInfo("pt-BR"));
                        }
                    }
                }
                catch (NullReferenceException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Preenchimento das transações na tabela
        /// </summary>
        protected void repTransacoes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                {
                    var dadosTransacao = e.Item.DataItem as TransacaoCorban;

                    if (dadosTransacao != null)
                    {
                        var ltrEstabelecimento = e.Item.FindControl("ltrEstabelecimento") as Literal;
                        var ltrDataVenda = e.Item.FindControl("ltrDataVenda") as Literal;
                        var ltrHoraVenda = e.Item.FindControl("ltrHoraVenda") as Literal;
                        var ltrCodigoServico = e.Item.FindControl("ltrCodigoServico") as Literal;
                        var ltrTipoConta = e.Item.FindControl("ltrTipoConta") as Literal;
                        var ltrFormaPagamento = e.Item.FindControl("ltrFormaPagamento") as Literal;
                        var ltrBandeira = e.Item.FindControl("ltrBandeira") as Literal;
                        var ltrCodigoBarras = e.Item.FindControl("ltrCodigoBarras") as Literal;
                        var ltrValorConta = e.Item.FindControl("ltrValorConta") as Literal;
                        var ltrStatus = e.Item.FindControl("ltrStatus") as Literal;

                        ltrEstabelecimento.Text = dadosTransacao.NumeroEstabelecimento.ToString();
                        ltrDataVenda.Text = dadosTransacao.DataPagamento.HasValue ? 
                            dadosTransacao.DataPagamento.Value.ToString("dd/MM/yy") : "-";

                        ltrHoraVenda.Text = dadosTransacao.HoraPagamento.HasValue ?
                            dadosTransacao.HoraPagamento.Value.ToString("HH'h'mm'min'ss's'") : "-";
                        ltrCodigoServico.Text = dadosTransacao.CodigoServico.ToString();
                        ltrTipoConta.Text = dadosTransacao.DescricaoTipoConta;
                        ltrFormaPagamento.Text = dadosTransacao.DescricaoFormaPagamento;

                        //Verifica se a bandeira corresponde ao tipo "dinheiro" (mainframe retorna hífen nesta situação)
                        Boolean bandeiraDinheiro = String.Compare("-", dadosTransacao.DescricaoBandeira.Trim(), true) == 0;
                        ltrBandeira.Text = bandeiraDinheiro ? "Dinheiro" : dadosTransacao.DescricaoBandeira;

                        ltrCodigoBarras.Text = dadosTransacao.CodigoBarras;
                        ltrValorConta.Text = dadosTransacao.ValorBrutoPagamento.ToString("N2");
                        ltrStatus.Text = dadosTransacao.StatusConta;
                    }
                }
                else if (e.Item.ItemType == ListItemType.Footer)
                {
                    var ltrTotalValorConta = e.Item.FindControl("ltrTotalValorConta") as Literal;
                    ltrTotalValorConta.Text = TotalTransacoes.ToString("N2");
                }
            }
            catch (NullReferenceException ex)
            {
                Logger.GravarErro("Erro durante bind de item no repeater", ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante bind de item no repeater", ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Download da consulta como Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void linkExcel_Click(object sender, EventArgs e)
        {
            DadosBuscaCorban dadosBusca = this.ObterDadosBusca();

            using (Logger log = Logger.IniciarLog("Download Excel do Relatório de Histórico de Transações Corban"))
            {
                try
                {
                    Consultar(dadosBusca, 1, Int32.MaxValue, Int32.MaxValue);
                    
                    String nomeArquivo = String.Format("HistoricoTransacoesPagueaquiItau_{0}.xls", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                    String html = RenderizarControles(true, this.repTransacoes);
                    String csv = CSVExporter.GerarCSV(html, "\t");

                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = true;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + nomeArquivo);
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
                    Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                    Response.ContentType = "application/ms-excel";
                    Response.AppendHeader("Content-Length", csv.Length.ToString());
                    Response.Write(csv);
                    Response.Flush();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [Métodos Auxiliares]

        /// <summary>
        /// Consultar Dados das Transações e Totalizadores Corban 
        /// </summary>
        /// <param name="dadosBusca">Dados de busca</param>
        /// <param name="pagina">Número da página</param>
        /// <param name="tamanhoPagina">Tamanho da página</param>
        /// <param name="paginasVirtuais">Páginas virtuais</param>
        private void Consultar(DadosBuscaCorban dadosBusca, Int32 pagina, Int32 tamanhoPagina, Int32 paginasVirtuais)
        {
            using (Logger log = Logger.IniciarLog("Consultar Dados das Transações e Totalizadores Corban "))
            {
                try
                {                    
                    PgnTrasacoes.PaginaAtual = pagina;
                    GravarDadosBusca(dadosBusca);

                    Int32 registroInicial = (pagina - 1) * tamanhoPagina;
					//registroInicial = registroInicial == 0 ? 1 : registroInicial;
					
                    Int32 qtdRegistrosVirtuais = (paginasVirtuais == Int32.MaxValue) ? 
                        Int32.MaxValue : paginasVirtuais * tamanhoPagina;
                    Int16 codigoRetorno = default(Int16);

                    using (var contexto = new ContextoWCF<HisServicoWaOutrosServicosClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaServico, new
                        {
                            dadosBusca,
                            pagina,
                            tamanhoPagina,
                            paginasVirtuais,
                            registroInicial,
                            qtdRegistrosVirtuais,
                            GuidPesquisa
                        });


                        var codigoStatus = (StatusCorban)dadosBusca.CodigoStatusTransacao;

                        List<TransacaoCorban> transacoesCorban = contexto.Cliente.ConsultarTransacoes(
                            out codigoRetorno,
                            GuidPesquisa,
                            registroInicial,
                            tamanhoPagina,
                            ref qtdRegistrosVirtuais,
                            dadosBusca.DataInicial,
                            dadosBusca.DataFinal,
                            (TipoConta)dadosBusca.CodigoTipoConta,
                            (FormaPagemento)dadosBusca.CodigoFormaPagamento,
                            codigoStatus,
                            dadosBusca.CodigoServico,
                            dadosBusca.Estabelecimentos.ToList());

                        log.GravarLog(EventoLog.RetornoServico, new { transacoesCorban, qtdRegistrosVirtuais, codigoRetorno });

                        PgnTrasacoes.QuantidadeTotalRegistros = qtdRegistrosVirtuais;

                        if (codigoRetorno == 0)
                        {
                            if (transacoesCorban != null && transacoesCorban.Count > 0)
                            {
                                CarregarTotalizadores(dadosBusca);

                                repTransacoes.DataSource = transacoesCorban;
                                repTransacoes.DataBind();

                                pnlQuadroAviso.Visible = false;
                                pnlRelatorio.Visible = true;
                                mnuAcoes.Visible = true;
                            }
                            else
                                this.ExibirAvisoNenhumResultado();
                        }
                        else
                        {
                            if (codigoRetorno == 60)
                                this.ExibirAvisoNenhumResultado();
                            else
                                base.ExibirPainelExcecao("HisServicoWaOutrosServicos.ConsultarTransacoes", codigoRetorno);
                        }
                    }
                }
                catch (FaultException<WaOutrosServicos.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carregar os totalizadores da consulta
        /// </summary>
        private void CarregarTotalizadores(DadosBuscaCorban dadosBusca)
        {
            using (Logger log = Logger.IniciarLog("Carregar os totalizadores da consulta"))
            {
                try
                {
                    using (var contexto = new ContextoWCF<HisServicoWaOutrosServicosClient>())
                    {
                        var codigoRetorno = default(Int16);
                        var quantidadeTotal = default(Int32);
                        var bandeirasTransacao = default(List<BandeiraTransacao>);
                        List<Int32> pvs = consultaPV.PVsSelecionados;

                        var codigoStatus = (StatusCorban)dadosBusca.CodigoStatusTransacao;

                        TransacaoCorban totalizador = contexto.Cliente.ConsultarTotalizadorTransacoes(
                            out codigoRetorno,
                            out quantidadeTotal,
                            out bandeirasTransacao,
                            dadosBusca.DataInicial,
                            dadosBusca.DataFinal,
                            (TipoConta)dadosBusca.CodigoTipoConta,
                            (FormaPagemento)dadosBusca.CodigoFormaPagamento,
                            codigoStatus,
                            dadosBusca.CodigoServico,
                            pvs);

                        if (codigoRetorno == 0)
                        {
                            ltrQuantidadeContas.Text = quantidadeTotal.ToString();
                            ltrTotalContas.Text = totalizador.ValorBrutoPagamento.ToString("C", new CultureInfo("pt-BR"));
                            TotalTransacoes = totalizador.ValorBrutoPagamento;

                            if (bandeirasTransacao != null && bandeirasTransacao.Count > 0)
                            {
                                var bandeiras = new List<BandeiraTransacao>(bandeirasTransacao);
                                bandeiras = bandeiras
                                            .GroupBy(bandeira => bandeira.Descricao)
                                            .Select(grupoBandeira => new BandeiraTransacao()
                                            {
                                                Descricao = grupoBandeira.Key,
                                                Valor = grupoBandeira.Sum(bandeira => bandeira.Valor)
                                            })
                                            .ToList();

                                //Bandeiras que serão renderizadas na primeira linha
                                repBandeira1.DataSource = bandeiras.Where((bandeira, index) => index % 2 == 0).ToList();
                                repBandeira1.DataBind();

                                //Bandeiras que serão renderizadas na segunda linha
                                repBandeira2.DataSource = bandeiras.Where((bandeira, index) => index % 2 != 0).ToList();
                                repBandeira2.DataBind();
                            }
                        }
                        else 
                        {

                            if (codigoRetorno != 60)
                                base.ExibirPainelExcecao("HisServicoWaOutrosServicos.ConsultarTotalizadorTransacoes", codigoRetorno);
                            else
                                this.ExibirAvisoNenhumResultado();
                        }   
                    }
                }
                catch (FaultException<WaOutrosServicos.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibir o quadro aviso para quando nenhum registro for encontrado
        /// </summary>
        private void ExibirAvisoNenhumResultado()
        {
            pnlRelatorio.Visible = false;
            pnlQuadroAviso.Visible = true;

            qdAvisoSemRegistros.TipoQuadro = TipoQuadroAviso.Aviso;
            qdAvisoSemRegistros.Titulo = "Aviso";
            qdAvisoSemRegistros.Mensagem = "Não há movimento para o período informado!";
        }

        /// <summary>
        /// Gravar dados de busca
        /// </summary>
        /// <param name="dadosBusca"></param>
        private void GravarDadosBusca(DadosBuscaCorban dadosBusca)
        {
            ViewState["DadosBuscaCorban"] = dadosBusca;
        }

        /// <summary>
        /// Recupera o Guid de pesquisa
        /// </summary>
        /// <returns></returns>
        public Guid GuidPesquisa
        {
            get
            {
                Guid? guid = ViewState["guidPesquisa"] as Guid?;

                //cria ou recupera os guids de consulta
                if (guid == null)
                {
                    guid = Guid.NewGuid();
                    ViewState["guidPesquisa"] = guid;
                }
                return (Guid)ViewState["guidPesquisa"];
            }
            set
            {
                ViewState["guidPesquisa"] = value;
            }
        }

        /// <summary>
        /// Guarda o total das transações
        /// </summary>
        public Decimal TotalTransacoes
        {
            get 
            {
                Decimal valor = default(Decimal);
                if(ViewState["TotalTransacoes"] != null)
                    valor = (Decimal)ViewState["TotalTransacoes"];
                return valor;
            }
            set
            {
                ViewState["TotalTransacoes"] = value;
            }
        }

        /// <summary>
        /// Obter Dados de Busca
        /// </summary>
        /// <returns>Dados Busca do Corban</returns>
        private DadosBuscaCorban ObterDadosBusca()
        {
            return (DadosBuscaCorban)ViewState["DadosBuscaCorban"] ?? new DadosBuscaCorban();
        }

        #endregion

        #region [ Renderização de Controles ]

        /// <summary>Renderização de controles para obtenção do HTML renderizado</summary>
        /// <param name="controle">Controle a ser renderizado</param>
        /// <param name="controles">Lista de controles a serem renderizados</param>
        /// <param name="desativarControles">Se TRUE, renderiza todos os controles como desativados/desabilitados</param>        
        /// <returns>HTML representando o controle renderizado</returns>
        private static String RenderizarControles(Boolean desativarControles, Control controle, params Control[] controles)
        {
            var sb = new StringBuilder();
            var listaControles = new List<Control>();

            if (controle != null)
                listaControles.Add(controle);
            if (controles != null && controles.Length > 0)
                listaControles.AddRange(controles);

            using (StringWriter writer = new StringWriter(sb))
            {
                using (HtmlTextWriter hwriter = new HtmlTextWriter(writer))
                {
                    Control controleAtual;
                    for (Int32 iControle = 0, total = listaControles.Count; iControle < total; iControle++)
                    {
                        controleAtual = listaControles[iControle];
                        if (desativarControles)
                            DesativarControles(controleAtual);
                        controleAtual.RenderControl(hwriter);
                    }
                    return HttpUtility.HtmlDecode(sb.ToString());
                }
            }
        }

        /// <summary>Desativa o controle e todos os seus controles filhos recursivamente</summary>
        /// <param name="controle">Controle a ser desativado</param>
        private static void DesativarControles(Control controle)
        {
            //Desativa o controle atual, de acordo com seu tipo
            if (controle is HtmlAnchor)
            {
                (controle as HtmlAnchor).Disabled = true;
                (controle as HtmlAnchor).HRef = String.Empty;
            }
            else if (controle is HtmlLink)
                (controle as HtmlLink).Disabled = true;
            else if (controle is HyperLink)
            {
                (controle as HyperLink).Enabled = false;
                (controle as HyperLink).Attributes.Remove("href");
            }
            else if (controle is LinkButton)
                (controle as LinkButton).Enabled = false;
            else if (controle is HtmlButton)
                (controle as HtmlButton).Disabled = true;
            else if (controle is HtmlInputControl)
                (controle as HtmlInputControl).Disabled = true;
            else if (controle is HtmlSelect)
                (controle as HtmlSelect).Disabled = true;
            else if (controle is TextBox)
                (controle as TextBox).Enabled = false;
            else if (controle is CheckBox)
                (controle as CheckBox).Enabled = false;

            //Aplica desativação de controles recursivamente para os controles filhos
            Int32 totalFilhos = controle.Controls.Count;
            for (Int32 iControleFilho = 0; iControleFilho < totalFilhos; iControleFilho++)
                DesativarControles(controle.Controls[iControleFilho]);
        }

        #endregion
    }
}