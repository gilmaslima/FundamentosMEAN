using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Sharepoint.GEServicos;
using Redecard.PN.Credenciamento.Sharepoint.GEProdutos;
using Redecard.PN.Credenciamento.Sharepoint.WFProdutos;
using Redecard.PN.Credenciamento.Sharepoint.WFServicos;
using System.ServiceModel;
using System.Linq;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using Redecard.PN.Credenciamento.Sharepoint.Modelo;
using Redecard.PN.Credenciamento.Sharepoint.PNTransicoesServico;
using Redecard.PN.Credenciamento.Sharepoint.WFCampanhas;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.ContratacaoServicos
{
    public partial class ContratacaoServicosUserControl : UserControlCredenciamentoBase
    {
        #region [ Propriedades ]

        public List<ListaServicosLiberadosComRegime> ListaServicos
        {
            get
            {
                if (ViewState["ListaServicos"] == null)
                    ViewState["ListaServicos"] = new List<ListaServicosLiberadosComRegime>();

                return (List<ListaServicosLiberadosComRegime>)ViewState["ListaServicos"];
            }
            set
            {
                ViewState["ListaServicos"] = value;
            }
        }

        #endregion

        #region [ Eventos da Página ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Credenciamento.Fase < 7)
                    Credenciamento.Fase = 7;

                Page.MaintainScrollPositionOnPostBack = true;
                Page.Title = "Contratação de Serviços";

                if (!IsPostBack)
                {
                    //if (String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                    //    String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                    //    String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0)
                    //{
                    //    ltlHeaderValor.Text = "Valor (mensal)";
                    //    ltlHeaderQtdeMinima.Text = "Qtde. Máxima";
                    //    CarregarServicosDatacash();
                    //}
                    //else
                    //{
                    //    CarregarServicos();
                    //}

                    CarregarServicos();
                    CarregarServicosPorCampanha();
                    CarregaTotalValoresServico();
                    CarregarProdutosVan();
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// Evento do botão continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            if (Credenciamento.Servicos != null && Credenciamento.Servicos.Count > 0)
                RemoveListaServicos(Credenciamento.Servicos);

            if (Credenciamento.ProdutosVan != null && Credenciamento.ProdutosVan.Count > 0)
                RemoveListaProdutosVan(Credenciamento.ProdutosVan.Select(p => p.CodCCA).ToList());

            Credenciamento.Servicos = GetSelectedRowsServicos();
            Credenciamento.ProdutosVan = GetSelectedRowsProdutosVan();

            Int32 codRetorno = SalvarDados();
            if (codRetorno == 0)
                Response.Redirect("pn_confirmacaodados.aspx", false);
            else
                base.ExibirPainelExcecao("Redecard.PN.Credenciamento.Servicos", codRetorno);
        }

        /// <summary>
        /// Evento do botão Parar e Salvar Proposta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            SalvarDados();

            Credenciamento = new Modelo.Credenciamento();
            Response.Redirect("pn_dadosiniciais.aspx", false);
        }

        /// <summary>
        /// Evento do botão voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_escolhatecnologia.aspx", false);
        }

        /// <summary>
        /// Data Bound da tabela de serviços
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptServicos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                {
                    Int32 codServico = (Int32)((ListaServicosLiberadosComRegime)e.Item.DataItem).CodServico;
                    ((Literal)e.Item.FindControl("ltlCodigo")).Text = codServico.ToString();
                    ((Literal)e.Item.FindControl("ltlServico")).Text = ((ListaServicosLiberadosComRegime)e.Item.DataItem).DescricaoServico;

                    DropDownList ddlRegime = (DropDownList)e.Item.FindControl("ddlRegime");
                    ListaRegimes[] regimes = ((ListaServicosLiberadosComRegime)e.Item.DataItem).ListaRegimes;

                    if (regimes.Length > 0)
                    {
                        ddlRegime.Items.Clear();
                        if ((String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0) && !Credenciamento.TipoComercializacao.Equals("80105"))
                        {
                            foreach (ListaRegimes regime in regimes)
                            {
                                ListItem item = new ListItem(String.Format("{0}", regime.CodRegimeServico), String.Format("{0}", regime.CodRegimeServico));
                                ddlRegime.Items.Add(item);
                            }                            
                        }
                        else
                        {
                            foreach (ListaRegimes regime in regimes)
                            {
                                ListItem item = new ListItem(String.Format("{0} - {1}", regime.NumSequencia, regime.PatamarFim), String.Format("{0} - {1}", regime.CodRegimeServico, regime.NumSequencia));
                                ddlRegime.Items.Add(item);
                            }
                        }

                        ((Literal)e.Item.FindControl("ltlQtdMinima")).Text = regimes[0].PatamarInicio != null ? regimes[0].PatamarInicio.ToString() : String.Empty;
                        ((Literal)e.Item.FindControl("ltlValor")).Text = regimes[0].ValorCobranca != null ? String.Format(@"{0:C}", regimes[0].ValorCobranca) : String.Empty;
                        ((Literal)e.Item.FindControl("ltlTarifaExcedente")).Text = regimes[0].ValorAdicional != null ? String.Format(@"{0:C}", regimes[0].ValorAdicional) : String.Empty;
                    }

                    Modelo.Servico servico = Credenciamento.Servicos.FirstOrDefault(s => s.CodServico == codServico);
                    if (servico != null)
                    {
                        ListaRegimes regime = regimes.Where(r => r.CodRegimeServico == servico.CodRegimeServico).FirstOrDefault();
                        ((CheckBox)e.Item.FindControl("ckbSeleciona")).Checked = true;

                        if ((String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0) && !Credenciamento.TipoComercializacao.Equals("80105"))
                            ddlRegime.SelectedValue = String.Format("{0}", servico.CodRegimeServico);
                        else
                            ddlRegime.SelectedValue = String.Format("{0} - {1}", servico.CodRegimeServico, servico.NumSeq);

                        ((Literal)e.Item.FindControl("ltlValor")).Text = String.Format(@"{0:C}", regime.ValorCobranca);
                        ((Literal)e.Item.FindControl("ltlTarifaExcedente")).Text = String.Format(@"{0:C}", regime.ValorAdicional);
                    }

                    CheckBox chk = (CheckBox)e.Item.FindControl("ckbSeleciona");
                    if (((ListaServicosLiberadosComRegime)e.Item.DataItem).Obrigatorio == 'S')
                    {
                        chk.Checked = true;
                        chk.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Data Bound da tabela de serviços
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptProdutosVan_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                {
                    ((Label)e.Item.FindControl("lblCodigo")).Text = ((ProdutosListaDadosProdutosVanPorRamo)e.Item.DataItem).CodCCA.ToString();
                    ((Label)e.Item.FindControl("lblDescricao")).Text = ((ProdutosListaDadosProdutosVanPorRamo)e.Item.DataItem).NomeCCA;

                    //if (Credenciamento.ProdutosVan == null ||
                    //    Credenciamento.ProdutosVan.Contains(((ProdutosListaDadosProdutosVanPorRamo)e.Item.DataItem).CodCCA))
                        ((CheckBox)e.Item.FindControl("chkProdutoVan")).Checked = true;
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Muda informação do regime selecionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRegime_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                RepeaterItem row = (RepeaterItem)((DropDownList)sender).Parent;

                Int32 codServico = ((Literal)row.FindControl("ltlCodigo")).Text.ToInt32();
                ListaServicosLiberadosComRegime servico = ListaServicos.FirstOrDefault(s => s.CodServico == codServico);

                if ((String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0) && !Credenciamento.TipoComercializacao.Equals("80105"))
                {
                    Int32 codRegime = ((DropDownList)sender).SelectedValue.ToInt32();
                    ListaRegimes regime = servico.ListaRegimes.FirstOrDefault(r => r.CodRegimeServico == codRegime);

                    ((Literal)row.FindControl("ltlQtdMinima")).Text = regime.PatamarInicio.ToString();
                    ((Literal)row.FindControl("ltlValor")).Text = String.Format(@"{0:C}", regime.ValorCobranca);
                    ((Literal)row.FindControl("ltlTarifaExcedente")).Text = String.Format(@"{0:C}", regime.ValorAdicional);
                }
                else
                {
                    Int32 codRegime = ((DropDownList)sender).SelectedValue.Split('-')[0].ToInt32();
                    Int32 numSeq = ((DropDownList)sender).SelectedValue.Split('-')[1].ToInt32();
                    ListaRegimes regime = servico.ListaRegimes.FirstOrDefault(r => r.CodRegimeServico == codRegime && r.NumSequencia == numSeq);

                    ((Literal)row.FindControl("ltlQtdMinima")).Text = regime.PatamarInicio.ToString();
                    //((Literal)row.FindControl("ltlValor")).Text = String.Format(@"{0:C}", regime.ValorCobranca);

                    ((Literal)row.FindControl("ltlTarifaExcedente")).Text = numSeq != 0 ? String.Format(@"{0:C}", regime.ValorCobranca) : String.Format(@"R$ {0}", 0);
                }

                CarregaTotalValoresServico();
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega os valores totais da grid de serviços
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ckbSeleciona_CheckedChanged(object sender, EventArgs e)
        {
            CarregaTotalValoresServico();
        }

        #endregion

        #region [ Eventos Auxiliares ]

        /// <summary>
        /// Carrega lista de serviços e carrega o repeater
        /// </summary>
        private void CarregarServicos()
        {
            ServicoPortalGEServicosClient client = new ServicoPortalGEServicosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Serviços"))
                {
                    Int32 codGrupoRamo = Credenciamento.GrupoRamo;
                    Int32 codRamoAtivididade = Credenciamento.RamoAtividade;
                    Char indOrigemTelemarketing = 'N';
                    Int32 codCanalOrigem = Credenciamento.Canal;
                    String tipoEquipamento = Credenciamento.TipoEquipamento;

                    //var servicos = client.ListaServicosLiberadosComRegime(codGrupoRamo, codRamoAtivididade, indOrigemTelemarketing, codCanalOrigem);
                    var servicos = client.ListaServicosComRegime(codGrupoRamo, codRamoAtivididade, indOrigemTelemarketing, codCanalOrigem, tipoEquipamento);
                    client.Close();

                    if (servicos != null && servicos.Length > 0)
                    {
                        if (String.Compare(Credenciamento.TipoPessoa, "F") == 0)
                            servicos = servicos.Where(s => s.CodServico != 1).ToArray();

                        ListaServicos = servicos.ToList();

                        rptServicos.DataSource = servicos;
                        rptServicos.DataBind();
                    }
                    else
                        pnlServicos.Visible = false;
                }
            }
            catch (FaultException<GEServicos.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega lista de serviços para o datacash e carrega o repeater
        /// </summary>
        private void CarregarServicosDatacash()
        {
            ServicoPortalGEServicosClient client = new ServicoPortalGEServicosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Serviços"))
                {
                    Int32 codGrupoRamo = Credenciamento.GrupoRamo;
                    Int32 codRamoAtivididade = Credenciamento.RamoAtividade;
                    Char indOrigemTelemarketing = 'N';
                    Int32 codCanalOrigem = Credenciamento.Canal;

                    ListaServicosLiberadosComRegime[] servicos = client.ListaServicosDataCashComRegime(codGrupoRamo, codRamoAtivididade, indOrigemTelemarketing, codCanalOrigem);
                    client.Close();

                    ListaServicos = servicos.ToList();

                    rptServicos.DataSource = servicos;
                    rptServicos.DataBind();
                }
            }
            catch (FaultException<GEServicos.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de produtos Van e carrega o dropdown
        /// </summary>
        private void CarregarProdutosVan()
        {
            ServicoPortalGEProdutosClient client = new ServicoPortalGEProdutosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Produtos Van"))
                {
                    Int32 codGrupoRamo = Credenciamento.GrupoRamo;
                    Int32 codRamoAtividade = Credenciamento.RamoAtividade;

                    ProdutosListaDadosProdutosVanPorRamo[] produtosVan = client.ListaDadosProdutosVanPorRamo(codGrupoRamo, codRamoAtividade);
                    client.Close();

                    if (produtosVan.Count() > 0)
                    {
                        rptProdutosVan.DataSource = produtosVan;
                        rptProdutosVan.DataBind();
                    }
                    else
                        pnlProdutosVan.Visible = false;
                }
            }
            catch (FaultException<GEProdutos.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega total de valores dos serviços
        /// </summary>
        private void CarregaTotalValoresServico()
        {
            try
            {
                Decimal valorTotal = 0;
                Decimal valorExcedenteTotal = 0;
                foreach (RepeaterItem item in rptServicos.Items)
                {
                    if (((CheckBox)item.FindControl("ckbSeleciona")).Checked)
                    {
                        valorTotal += ((Literal)item.FindControl("ltlValor")).Text.Replace("R$", "").ToDecimal();
                        valorExcedenteTotal += ((Literal)item.FindControl("ltlTarifaExcedente")).Text.Replace("R$", "").ToDecimal();
                    }
                }

                ltlTotalValor.Text = String.Format("{0:C}", valorTotal);
                ltlTotalExcedente.Text = String.Format("{0:C}", valorExcedenteTotal);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Grava ou atualiza dados da primeira tela
        /// </summary>
        /// <returns></returns>
        private Int32 GravarAtualizarPasso8()
        {
            TransicoesClient client = new TransicoesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Gravar Dados - Contratação de Serviços"))
                {
                    PNTransicoesServico.Proposta proposta = PreencheProposta();
                    List<PNTransicoesServico.Servico> servicos = PreencheListaServicos(Credenciamento.Servicos);
                    List<PNTransicoesServico.ProdutoVan> produtosVan = PreencheListaProdutosVan(GetSelectedRowsProdutosVan());

                    Int32 retorno = client.GravarAtualizarPasso8(proposta, servicos, produtosVan);
                    client.Close();

                    return retorno;
                }
            }
            catch (FaultException<PNTransicoesServico.GeneralFault> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Retorna lista de serviços selecionados
        /// </summary>
        /// <returns></returns>
        private List<Modelo.Servico> GetSelectedRowsServicos()
        {
            List<Modelo.Servico> retorno = new List<Modelo.Servico>();

            foreach (RepeaterItem servico in rptServicos.Items)
            {
                CheckBox servicoSelecionado = (CheckBox)servico.FindControl("ckbSeleciona");

                if (servicoSelecionado.Checked)
                {
                    Int32? codServico = ((Literal)servico.FindControl("ltlCodigo")).Text.ToInt32Null();
                    Int32? codRegimeServico;
                    Int32? numSeq;

                    if ((String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0) && !Credenciamento.TipoComercializacao.Equals("80105"))
                    {
                        codRegimeServico = ((DropDownList)servico.FindControl("ddlRegime")).SelectedValue.ToInt32Null();
                        numSeq = 0;
                    }
                    else
                    {
                        codRegimeServico = ((DropDownList)servico.FindControl("ddlRegime")).SelectedValue.Split('-')[0].ToInt32Null();
                        numSeq = ((DropDownList)servico.FindControl("ddlRegime")).SelectedValue.Split('-')[1].ToInt32Null();
                    }
                    Int32 qtdeMinima = ((Literal)servico.FindControl("ltlQtdMinima")).Text.ToInt32();
                    Double? valorFranquia = ((Literal)servico.FindControl("ltlValor")).Text.Replace("R$", "").ToDouble();
                    String descServico = ((Literal)servico.FindControl("ltlServico")).Text;

                    retorno.Add(new Modelo.Servico
                    {
                        CodServico = codServico,
                        CodRegimeServico = codRegimeServico,
                        QtdeMinima = qtdeMinima,
                        ValorFranquia = valorFranquia,
                        NumSeq = numSeq,
                        DescServico = descServico
                    });
                }
            }

            return retorno;
        }

        /// <summary>
        /// Retorna lista de produtos vans selecionados
        /// </summary>
        private List<ProdutosListaDadosProdutosVanPorRamo> GetSelectedRowsProdutosVan()
        {
            List<ProdutosListaDadosProdutosVanPorRamo> retorno = new List<ProdutosListaDadosProdutosVanPorRamo>();

            foreach (RepeaterItem produto in rptProdutosVan.Items)
            {
                CheckBox prodSelecionado = (CheckBox)produto.FindControl("chkProdutoVan");

                if (prodSelecionado.Checked)
                {
                    Int32? codCCA = ((Label)produto.FindControl("lblCodigo")).Text.ToInt32Null();
                    String nomeCCA = ((Label)produto.FindControl("lblDescricao")).Text;

                    retorno.Add(new ProdutosListaDadosProdutosVanPorRamo
                    {
                        CodCCA = codCCA,
                        NomeCCA = nomeCCA
                    });
                }
            }

            return retorno;
        }

        /// <summary>
        /// Salva dados da tela
        /// </summary>
        /// <returns></returns>
        private int SalvarDados()
        {
            try
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    return GravarAtualizarPasso8();
                }
                return 300;
            }
            catch (FaultException<PNTransicoesServico.GeneralFault> fe)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                return fe.Detail.Codigo;
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
                return 300;
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
                return 300;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return CODIGO_ERRO;
            }
        }

        /// <summary>
        /// Remove um serviço da base wf
        /// </summary>
        /// <param name="servico"></param>
        /// <returns></returns>
        private WFServicos.RetornoErro RemoveServico(Modelo.Servico servico)
        {
            ServicoPortalWFServicosClient client = new ServicoPortalWFServicosClient();

            try
            {
                Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                Int64 numCNPJ = 0;
                if (Credenciamento.TipoPessoa == "J")
                    Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                else
                    Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                Int32 numSeqProp = (Int32)Credenciamento.NumSequencia;
                Int32? codServico = servico.CodServico;
                Int32? codRegimeServico = null;
                Char? indAceitaServico = null;
                Int32? qtdeMinimaConsulta = null;
                Double? valorFranquia = null;
                Char? indHabilitaCargaPre = null;
                DateTime? dataHoraUltimaAtualizacao = null;
                String usuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;

                WFServicos.RetornoErro[] retorno = client.ExclusaoServicos(
                    codTipoPessoa,
                    numCNPJ,
                    numSeqProp,
                    codServico,
                    codRegimeServico,
                    indAceitaServico,
                    qtdeMinimaConsulta,
                    valorFranquia,
                    indHabilitaCargaPre,
                    dataHoraUltimaAtualizacao,
                    usuarioUltimaAtualizacao);
                client.Close();

                return retorno[0];
            }
            catch (FaultException<WFServicos.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Remove uma lista de serviços da base wf
        /// </summary>
        /// <param name="servicos"></param>
        /// <returns></returns>
        private WFServicos.RetornoErro RemoveListaServicos(List<Modelo.Servico> servicos)
        {
            Int32 i = 0;
            WFServicos.RetornoErro erro = new WFServicos.RetornoErro { CodigoErro = 0 };

            while (servicos.Count > i && erro.CodigoErro == 0)
            {
                erro = RemoveServico(servicos[i]);
                i++;
            }

            return erro;
        }

        /// <summary>
        /// Remove um produto do tipo Van da base WF
        /// </summary>
        /// <returns></returns>
        private WFProdutos.RetornoErro RemoveProdutoVan(Int32? _codCca)
        {
            ServicoPortalWFProdutosClient client = new ServicoPortalWFProdutosClient();

            try
            {
                Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                Int64 numCNPJ = 0;
                if (Credenciamento.TipoPessoa == "J")
                    Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                else
                    Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                Int32 numSeqProp = (Int32)Credenciamento.NumSequencia;
                Int32 codCca = (Int32)_codCca;
                Char? indTipoOperacaoProd = null;
                String usuario = SessaoAtual.LoginUsuario;

                WFProdutos.RetornoErro[] retorno = client.ExclusaoProdutoVan(
                    codTipoPessoa,
                    numCNPJ,
                    numSeqProp,
                    codCca,
                    indTipoOperacaoProd,
                    usuario);
                client.Close();

                return retorno[0];
            }
            catch (FaultException<WFProdutos.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Remove uma lista de produtos Van da base WF
        /// </summary>
        /// <param name="produtos"></param>
        /// <returns></returns>
        private WFProdutos.RetornoErro RemoveListaProdutosVan(List<Int32?> produtos)
        {
            Int32 i = 0;
            WFProdutos.RetornoErro erro = new WFProdutos.RetornoErro { CodigoErro = 0 };

            while (produtos.Count > i && erro.CodigoErro == 0)
            {
                erro = RemoveProdutoVan(produtos[i]);
                i++;
            }

            return erro;
        }

        /// <summary>
        /// Carrega os serviços de acordo com a campanha selecionada
        /// </summary>
        private void CarregarServicosPorCampanha()
        {
            ServicoPortalWFCampanhasClient client = new ServicoPortalWFCampanhasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Taxa Adesão por Campanha"))
                {
                    if (!String.IsNullOrEmpty(Credenciamento.CodCampanha))
                    {
                        String codigoCampanha = Credenciamento.CodCampanha;
                        ListaParametrosCampanha[] parametros = client.ListaParametrosCampanha(codigoCampanha, 'S');

                        foreach (RepeaterItem item in rptServicos.Items)
                        {
                            CheckBox chkSeleciona = (CheckBox)item.FindControl("ckbSeleciona");
                            DropDownList ddlRegime = (DropDownList)item.FindControl("ddlRegime");
                            Int32 codServico = ((Literal)item.FindControl("ltlCodigo")).Text.ToInt32();
                            var parametro = parametros.FirstOrDefault(p => p.CodServico == codServico);

                            if(parametros.Count() > 0)
                                chkSeleciona.Enabled = false;

                            if (parametro != null)
                                chkSeleciona.Checked = true;

                        }
                    }
                    client.Close();
                }
            }
            catch (FaultException<WFCampanhas.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion
    }
}