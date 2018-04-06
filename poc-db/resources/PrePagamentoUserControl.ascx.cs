#region Histórico do Arquivo
/*
(c) Copyright [2012] BRQ IT Solutions.
Autor       : [- 2012/10/01 - Lucas Nicoletto da Cunha]
Empresa     : [BRQ IT Solutions]
Histórico   : Criação da Classe
- [2012/10/01] – [Lucas Nicoletto da Cunha] – [Criação]
 -[2012/11/29] – [Tiago Barbosa dos Santos] – [Ajustes de layout]
*/
#endregion

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Emissores.Sharepoint.ServicoEmissores;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Globalization;

namespace Redecard.PN.Emissores.Sharepoint.WebParts.PrePagamento
{
    public partial class PrePagamentoUserControl : UserControlBase
    {

        #region [Propriedades]
        /// <summary>
        /// Listagem de bandeiras a serem guardadas para obter o nome das Bandeiras ao carregar uma Consulta
        /// </summary>
        private List<Bandeira> Bandeiras
        {
            get
            {
                if (!Object.ReferenceEquals(Session["Bandeiras"], null))
                    return (List<Bandeira>)Session["Bandeiras"];
                else
                    return new List<Bandeira>();
            }
            set
            {
                Session["Bandeiras"] = value;
            }
        }

        /// <summary>
        /// Valor da sumarização dos Pré-Pagamentos
        /// </summary>
        private ServicoEmissores.PrePagamento SumarizacaoPrePagamento
        {
            get
            {
                if (!Object.ReferenceEquals(Session["SumarizacaoPrePagamento"], null))
                    return (ServicoEmissores.PrePagamento)Session["SumarizacaoPrePagamento"];
                else
                    return new ServicoEmissores.PrePagamento();
            }
            set
            {
                Session["SumarizacaoPrePagamento"] = value;
            }
        }

        /// <summary>
        /// Período Inicial da Consulta
        /// </summary>
        private DateTime PeriodoIncio
        {
            get
            {
                if (!Object.ReferenceEquals(Session["PeriodoIncio"], null))
                    return (DateTime)Session["PeriodoIncio"];
                else
                    return DateTime.Today;
            }
            set
            {
                Session["PeriodoIncio"] = value;
            }
        }

        /// <summary>
        /// Período Final da Consulta
        /// </summary>
        private DateTime PeriodoFim
        {
            get
            {
                if (!Object.ReferenceEquals(Session["PeriodoFim"], null))
                    return (DateTime)Session["PeriodoFim"];
                else
                    return DateTime.Today;
            }
            set
            {
                Session["PeriodoFim"] = value;
            }
        }

        private readonly String ARQUIVOEXPORTAR = "PrePagamento";

        #endregion

        /// <summary>
        /// Carregamento dos filtros da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Início Carregamento dos filtros da página"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        TextInfo txtInfo = new CultureInfo("pt-BR", false).TextInfo;
                        String nomeEntidade = txtInfo.ToTitleCase(SessaoAtual.NomeEntidade.ToLower());
                        NomeEntidade.Text = nomeEntidade;

                        using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                        {
                            Bandeiras = new List<Bandeira>();
                            Bandeiras = context.Cliente.ConsultarBandeiras();

                            this.CarregarBandeiras(SessaoAtual.CodigoEntidade);
                        }
                    }
                }
                catch (FaultException<ServicoEmissores.GeneralFault> ex)
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

        ///// <summary>
        ///// Carregar o dropdownlist de Emissores para filtrar Consulta
        ///// </summary>
        //private void CarregarEmissores()
        //{
        //    using (Logger Log = Logger.IniciarLog("Início Carregar o dropdownlist de Emissores para filtrar Consulta"))
        //    {
        //        try
        //        {
        //            using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
        //            {
        //                List<Emissor> emissores = context.Cliente.ConsultarEmissores();

        //                foreach (Emissor emissor in emissores)
        //                {
        //                    ddlEmissores.Items.Add(new ListItem()
        //                        {
        //                            Text = String.Format("{0} - {1}",
        //                                                emissor.Codigo.ToString().PadLeft(4, '0'),
        //                                                this.NomeBanco(emissor.Codigo)),
        //                            Value = emissor.Codigo.ToString()
        //                        });
        //                }
        //            }
        //        }
        //        catch (FaultException<ServicoEmissores.GeneralFault> ex)
        //        {
        //            Log.GravarErro(ex);
        //            base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.GravarErro(ex);
        //            SharePointUlsLog.LogErro(ex);
        //            base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
        //        }
        //    }
        //}
        
        /// <summary>
        /// Retorna o nome de uma bandeira que esteja na Sessão
        /// </summary>
        /// <param name="codigoBandeira">Código da Bandeira</param>
        /// <returns>String Nome da Bandeira</returns>
        private String NomeBandeira(Int32 codigoBandeira)
        {
            Bandeira bandeira = this.Bandeiras.Find(bndr => bndr.Codigo == codigoBandeira);
            if (bandeira != null)
                return bandeira.Descricao;
            else
                return String.Empty;
        }

        ///// <summary>
        ///// Selecionado um Emissor no combo, busca-se todas suas bandeiras e ICA/BID
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void ddlEmissores_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    using (Logger Log = Logger.IniciarLog("Início Selecionado um Emissor no combo, busca-se todas suas bandeiras e ICA/BID"))
        //    {
        //        try
        //        {
        //            this.CarregarBandeiras(ddlEmissores.SelectedValue.ToInt32());
        //        }
        //        catch (FaultException<ServicoEmissores.GeneralFault> ex)
        //        {
        //            Log.GravarErro(ex);
        //            base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.GravarErro(ex);
        //            SharePointUlsLog.LogErro(ex);
        //            base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
        //        }
        //    }
        //}

        /// <summary>
        /// Busca-se as bandeiras e ICA/BID do Emissor
        /// </summary>
        /// <param name="codigoEmissor">Código do Emissor</param>
        private void CarregarBandeiras(Int32 codigoEmissor)
        {
            using (Logger Log = Logger.IniciarLog("Início Busca das bandeiras e ICA/BID do Emissor"))
            {
                try
                {
                    using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                    {
                        List<Bandeira> bandeirasEmissor = context.Cliente.ConsultarEmissoresBandeiras(codigoEmissor);

                        pnlConsultaPagamento.Attributes["style"] = "visibility:hidden";
                        pnlVazio.Visible = false;

                        if (bandeirasEmissor.Count == 0 || codigoEmissor == 0)
                        {
                            pnlBandeiras.Visible = false;
                        }
                        else
                        {
                            pnlBandeiras.Visible = true;
                            rptBandeiras.DataSource = bandeirasEmissor;
                            rptBandeiras.DataBind();
                        }
                    }
                }
                catch (FaultException<ServicoEmissores.GeneralFault> ex)
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

        /// <summary>
        /// Carregando as bandeiras do Emissor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptBandeiras_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Início Carregando as bandeiras do Emissor"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        Bandeira bandeiraEmissor = (Bandeira)e.Item.DataItem;

                        if (bandeiraEmissor != null)
                        {
                            HiddenField hdnCodigoBandeira = (HiddenField)e.Item.FindControl("hdnCodigoBandeira");
                            if (hdnCodigoBandeira != null)
                                hdnCodigoBandeira.Value = bandeiraEmissor.Codigo.ToString();

                            CheckBox chkBandeira = (CheckBox)e.Item.FindControl("chkBandeira");
                            if (chkBandeira != null)
                                chkBandeira.Text = NomeBandeira(bandeiraEmissor.Codigo);

                            //if (bandeiraEmissor.Codigo.Equals(1) || bandeiraEmissor.Codigo.Equals(2)) //26/02/2014 - habilitado o filtro de ICA/BID para todas as bandeiras
                            //{
                                if (bandeiraEmissor.EmissoresBandeiras.Count > 0)
                                {
                                    Repeater rptICABID = (Repeater)e.Item.FindControl("rptICABID");
                                    rptICABID.Visible = true;
                                    rptICABID.DataSource = bandeiraEmissor.EmissoresBandeiras;
                                    rptICABID.DataBind();
                                }
                            //}
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

        /// <summary>
        /// Carregando os ICA/BID do Emissor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptICABID_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Início Carregando os ICA/BID do Emissor"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        EmissorBandeira bandeiraEmissor = (EmissorBandeira)e.Item.DataItem;

                        if (bandeiraEmissor != null)
                        {
                            HiddenField hdnCodigoICABID = (HiddenField)e.Item.FindControl("hdnCodigoICABID");
                            if (hdnCodigoICABID != null)
                                hdnCodigoICABID.Value = bandeiraEmissor.Codigo.ToString();

                            CheckBox chkICABID = (CheckBox)e.Item.FindControl("chkICABID");
                            if (chkICABID != null)
                                chkICABID.Text = bandeiraEmissor.Codigo.ToString();
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carregando os Pré-Pagamentos consultados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptPrePagamento_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Início Carregando os Pré-Pagamentos consultados"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        ServicoEmissores.PrePagamento prePagamento = (ServicoEmissores.PrePagamento)e.Item.DataItem;

                        if (prePagamento != null)
                        {
                            Label lblDataVencimento = (Label)e.Item.FindControl("lblDataVencimento");
                            Label lblNomeBandeira = (Label)e.Item.FindControl("lblNomeBandeira");
                            Label lblICABID = (Label)e.Item.FindControl("lblICABID");
                            Label lblSaldoPagar = (Label)e.Item.FindControl("lblSaldoPagar");
                            Label lblValorAntecipado = (Label)e.Item.FindControl("lblValorAntecipado");
                            Label lblSaldoEstoque = (Label)e.Item.FindControl("lblSaldoEstoque");

                            if (lblDataVencimento != null)
                                lblDataVencimento.Text = prePagamento.DataVencimento.ToString("dd/MM/yyyy");

                            if (lblNomeBandeira != null)
                                lblNomeBandeira.Text = this.NomeBandeira(prePagamento.Bandeira.Codigo);

                            if (lblICABID != null)
                                lblICABID.Text = prePagamento.Bandeira.EmissoresBandeiras[0].Codigo.ToString();

                            //lblICABID.Text = (prePagamento.Bandeira.Codigo == 1 || //26/02/2014 - habilitado o filtro de ICA/BID para todas as bandeiras
                                //    prePagamento.Bandeira.Codigo == 2) ? prePagamento.Bandeira.EmissoresBandeiras[0].Codigo.ToString() : "-";

                            if (lblSaldoPagar != null)
                            {
                                lblSaldoPagar.Text = prePagamento.ValorPagarReceber.ToString("N2");

                                if (prePagamento.ValorPagarReceber < 0)
                                    lblSaldoPagar.ForeColor = System.Drawing.Color.Red;
                            }

                            if (lblValorAntecipado != null)
                            {
                                lblValorAntecipado.Text = prePagamento.SaldoAntecipado.ToString("N2");

                                if (prePagamento.SaldoAntecipado < 0)
                                    lblValorAntecipado.ForeColor = System.Drawing.Color.Red;
                            }

                            if (lblSaldoEstoque != null)
                            {
                                lblSaldoEstoque.Text = prePagamento.SaldoEstoque.ToString("N2");

                                if (prePagamento.SaldoEstoque < 0)
                                    lblSaldoEstoque.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                    else if (e.Item.ItemType == ListItemType.Footer)
                    {
                        Label lblSaldoPagar = (Label)e.Item.FindControl("lblSaldoPagar");
                        Label lblValorAntecipado = (Label)e.Item.FindControl("lblValorAntecipado");
                        Label lblSaldoEstoque = (Label)e.Item.FindControl("lblSaldoEstoque");

                        if (lblSaldoPagar != null)
                        {
                            lblSaldoPagar.Text = SumarizacaoPrePagamento.ValorPagarReceber.ToString("N2");

                            if (SumarizacaoPrePagamento.ValorPagarReceber < 0)
                                lblSaldoPagar.ForeColor = System.Drawing.Color.Red;
                        }

                        if (lblValorAntecipado != null)
                        {
                            lblValorAntecipado.Text = SumarizacaoPrePagamento.SaldoAntecipado.ToString("N2");

                            if (SumarizacaoPrePagamento.SaldoAntecipado < 0)
                                lblValorAntecipado.ForeColor = System.Drawing.Color.Red;
                        }

                        if (lblSaldoEstoque != null)
                        {
                            lblSaldoEstoque.Text = (SumarizacaoPrePagamento.SaldoEstoque).ToString("N2");

                            if (SumarizacaoPrePagamento.SaldoEstoque < 0)
                                lblSaldoEstoque.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Recupera as bandeiras selecionadas no filtro
        /// </summary>
        /// <returns>List of Bandeira selecionadas</returns>
        private List<Bandeira> RecuperarBandeirasICABID()
        {
            List<Bandeira> bandeiras = new List<Bandeira>();
            Bandeira bndr = new Bandeira();

            foreach (RepeaterItem rptBndr in rptBandeiras.Items)
            {
                if (rptBndr.ItemType == ListItemType.Item || rptBndr.ItemType == ListItemType.AlternatingItem)
                {
                    CheckBox chkBandeira = (CheckBox)rptBndr.FindControl("chkBandeira");
                    HiddenField hdnCodigoBandeira = (HiddenField)rptBndr.FindControl("hdnCodigoBandeira");
                    if (chkBandeira != null)
                    {
                        if (chkBandeira.Checked)
                        {
                            bndr = new Bandeira()
                            {
                                Codigo = hdnCodigoBandeira.Value.ToInt32()
                            };
                            bndr.EmissoresBandeiras = new List<EmissorBandeira>();

                            //if (hdnCodigoBandeira.Value.Equals("1") || hdnCodigoBandeira.Value.Equals("2"))
                            //{
                                Repeater rptICABID = (Repeater)rptBndr.FindControl("rptICABID");

                                if (rptICABID != null)
                                {
                                    foreach (RepeaterItem icaBIDItem in rptICABID.Items)
                                    {
                                        if (icaBIDItem.ItemType == ListItemType.Item || icaBIDItem.ItemType == ListItemType.AlternatingItem)
                                        {
                                            CheckBox chkICABID = (CheckBox)icaBIDItem.FindControl("chkICABID");
                                            HiddenField hdnCodigoICABID = (HiddenField)icaBIDItem.FindControl("hdnCodigoICABID");

                                            if (chkICABID != null)
                                            {
                                                if (chkICABID.Checked)
                                                {
                                                    bndr.EmissoresBandeiras.Add(new EmissorBandeira()
                                                    {
                                                        Codigo = Convert.ToInt64(hdnCodigoICABID.Value)
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            //}

                            bandeiras.Add(bndr);
                        }
                    }
                }
            }

            return bandeiras;
        }

        /// <summary>
        /// Trata os filtros aplicados e efetua a pesquisa dos Pré-Pagamentos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Início Trata os filtros aplicados e efetua a pesquisa dos Pré-Pagamentos"))
            {
                try
                {
                    pnlValidacaoImprimir.Attributes["style"] = "visibility:hidden";

                    List<ServicoEmissores.PrePagamento> prePagamentos = this.ConsultarPrePagamentos();

                    if (prePagamentos.Count == 0)
                    {
                        pnlVazio.Visible = true;
                        pnlConsultaPagamento.Attributes["style"] = "visibility:hidden";
                        ((QuadroAviso)qdAviso).CarregarMensagem();
                    }
                    else
                    {
                        pnlVazio.Visible = false;
                        pnlConsultaPagamento.Attributes["style"] = "visibility:visible";

                        txtPeriodoDe.Text = txtDataInicio.Text;
                        txtPeriodoAte.Text = txtDataFim.Text;

                        rptPrePagamento.DataSource = prePagamentos;
                        rptPrePagamento.DataBind();
                    }
                }
                catch (FaultException<ServicoEmissores.GeneralFault> ex)
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

        /// <summary>
        /// Exportar a consulta para TXT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void downloadTXT_Click(object sender, EventArgs e)
        {
            this.ExportarTXT();
        }

        /// <summary>
        /// Exportar a consulta para TXT
        /// </summary>
        private void ExportarTXT()
        {
            using (Logger Log = Logger.IniciarLog("Início Exportar a consulta para TXT"))
            {
                try
                {
                    if (!pnlConsultaPagamento.Attributes["style"].Contains("hidden"))
                    {
                        pnlValidacaoImprimir.Attributes["style"] = "visibility:hidden";

                        String arquivo = String.Format("{0}_{1}", ARQUIVOEXPORTAR, DateTime.Now.ToString("ddMMyyyyHHmmss"));

                        this.Page.Response.Clear();
                        this.Page.Response.Charset = "iso-8859-1";
                        this.Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                        this.Page.Response.AddHeader("Content-Disposition", "attachment;filename=" + arquivo + ".txt");
                        this.Page.Response.ContentType = "text/txt";

                        String colunasFormatadas = "{0,-13} {1,-20} {2,-12}   {3,18}   {4,18}   {5,18} \r\n";
                        String colunasFooter = "{0,49} {1,18}   {2,18}   {3,18} \r\n";

                        String consultaPrePagamento = String.Format("Dados Emissor: \r\n{0}\r\nPré-Pagamento Emissores:\r\n{1}\r\nTesouraria - Mesa de Operações\r\n{2}\r\nPeríodo: {3} - {4}\r\n\r\n",
                                                                    NomeEntidade.Text, "Os valores informados após 27 dias corridos são parciais e poderão sofrer alterações.",
                                                                    "Tel: (11) 2121-0861/0993", txtPeriodoAte.Text, txtPeriodoAte.Text);

                        consultaPrePagamento += String.Format(colunasFormatadas,
                                                            "Vencimento", "Bandeira", "ICA/BID",
                                                            "Saldo a Pagar", "Valor Antecipado", "Sld Liq/Estq");

                        txtDataInicio.Text = this.PeriodoIncio.ToString("dd/MM/yyyy");
                        txtDataFim.Text = this.PeriodoFim.ToString("dd/MM/yyyy");

                        List<ServicoEmissores.PrePagamento> prePagamentos = this.ConsultarPrePagamentos();

                        //Monta as informações do TXT
                        foreach (ServicoEmissores.PrePagamento prePagamento in prePagamentos)
                        {
                            //String icaBid = prePagamento.Bandeira.Codigo.Equals(1) || prePagamento.Bandeira.Codigo.Equals(2) ?
                            //                prePagamento.Bandeira.EmissoresBandeiras[0].Codigo.ToString() : "-";

                            String icaBid = prePagamento.Bandeira.EmissoresBandeiras[0].Codigo.ToString();

                            consultaPrePagamento += String.Format(colunasFormatadas,
                                                                    prePagamento.DataVencimento.ToString("dd/MM/yyyy"),
                                                                    this.NomeBandeira(prePagamento.Bandeira.Codigo).Trim(), icaBid,
                                                                    prePagamento.ValorPagarReceber.ToString("N2"), prePagamento.SaldoAntecipado.ToString("N2"),
                                                                    prePagamento.SaldoEstoque.ToString("N2"));
                        }

                        consultaPrePagamento += String.Format(colunasFooter, "Total",
                                                                SumarizacaoPrePagamento.ValorPagarReceber.ToString("N2"),
                                                                SumarizacaoPrePagamento.SaldoAntecipado.ToString("N2"),
                                                                SumarizacaoPrePagamento.SaldoEstoque.ToString("N2"));

                        this.Page.Response.Write(consultaPrePagamento);

                        //Finaliza o envio do arquivo para a página
                        this.Page.Response.Flush();
                        this.Page.Response.End();
                    }
                    else
                        pnlValidacaoImprimir.Attributes["style"] = "visibility:visible";
                }
                catch (ArgumentNullException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (System.Web.HttpException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Converte o HTML em CSV
        /// </summary>
        /// <param name="s">String com o HTML para conversão</param>
        /// <returns>CSV formatado</returns>
        private String ConverteHTMLParaCSV(String s)
        {
            using (Logger Log = Logger.IniciarLog("Início Exportar a consulta para TXT"))
            {
                try
                {
                    s = Regex.Replace(s, ",", ".");

                    //s = Regex.Replace(s, "\n", ""); //N

                    s = Regex.Replace(s, "\r|\n|\t", "");

                    //s = Regex.Replace(s, "\\s[^>]+<", ""); //N

                    //s = Regex.Replace(s, "<th[^>]+>", "", RegexOptions.IgnoreCase); //Novo
                    //s = Regex.Replace(s, "</th>", "", RegexOptions.IgnoreCase); //Novo
                    //s = Regex.Replace(s, "</thead>", "", RegexOptions.IgnoreCase); //Novo

                    //s = Regex.Replace(s, "<tbody>", "", RegexOptions.IgnoreCase); //Novo
                    //s = Regex.Replace(s, "</tbody>", "", RegexOptions.IgnoreCase); //Novo

                    s = Regex.Replace(s, "</TR>", "\r\n", RegexOptions.IgnoreCase);
                    s = Regex.Replace(s, "</TD> *<TD", ",<TD", RegexOptions.IgnoreCase);
                    s = Regex.Replace(s, "<[^>]+>", "");
                    s = Regex.Replace(s, " +, *", ",");
                    s = Regex.Replace(s, "&nbsp;", " ");
                    s = Regex.Replace(s, ",", "\t");
                    
                }
                catch (ArgumentNullException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                return s;
            }
        }

        /// <summary>
        /// Exportar consulta para XSL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void downloadXSL_Click(object sender, EventArgs e)
        {
            this.ExportarExcel();
        }

        /// <summary>
        /// Exportar consulta para XSL
        /// </summary>
        private void ExportarExcel()
        {
            using (Logger Log = Logger.IniciarLog("Início Exportar consulta para XSL"))
            {
                try
                {
                    if (!pnlConsultaPagamento.Attributes["style"].Contains("hidden"))
                    {
                        pnlValidacaoImprimir.Attributes["style"] = "visibility:hidden";

                        String arquivo = String.Format("{0}_{1}", ARQUIVOEXPORTAR, DateTime.Now.ToString("ddMMyyyyHHmmss"));
                        Log.GravarMensagem("Arquivo Exportação", new { arquivo });

                        txtDataInicio.Text = this.PeriodoIncio.ToString("ddMMyyyy");
                        txtDataFim.Text = this.PeriodoFim.ToString("ddMMyyyy");

                        this.Page.Response.Clear();
                        this.Page.Response.Buffer = true;
                        this.Page.Response.ContentType = "application/vnd.ms-excel";
                        this.Page.Response.AddHeader("Content-Disposition", "attachment;filename=" + arquivo + ".xls");
                        this.Page.Response.Charset = "UTF-8";

                        using (StringWriter sw = new System.IO.StringWriter())
                        {
                            using (HtmlTextWriter htmlTextWriter = new System.Web.UI.HtmlTextWriter(sw))
                            {
                                tblConsulta.RenderControl(htmlTextWriter);
                                //rptPrePagamento.RenderControl(htmlTextWriter);

                                this.Page.Response.Write("<HTML><HEAD><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"></HEAD>");
                                this.Page.Response.Write(sw.ToString());
                                this.Page.Response.Flush();
                                this.Page.Response.End();
                            }
                        }
                    }
                    else
                        pnlValidacaoImprimir.Attributes["style"] = "visibility:visible";
                }
                catch (ArgumentNullException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (System.Web.HttpException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }                
            }
        }

        /// <summary>
        /// Exportar TXT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void downloadTXT_Click(object sender, ImageClickEventArgs e)
        {
            this.ExportarTXT();
        }

        /// <summary>
        /// Exportar consulta para XSL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void downloadXSL_Click(object sender, ImageClickEventArgs e)
        {
            this.ExportarExcel();
        }

        /// <summary>
        /// Consulta os Pré-Pagamentos
        /// </summary>
        /// <returns>Pré-Pagamentos</returns>
        private List<ServicoEmissores.PrePagamento> ConsultarPrePagamentos()
        {
            List<ServicoEmissores.PrePagamento> prePagamentos = new List<ServicoEmissores.PrePagamento>();

            using (Logger Log = Logger.IniciarLog("Início Trata os filtros aplicados e efetua a pesquisa dos Pré-Pagamentos"))
            {
                try
                {
                    //Recuperando os valores do filtro
                    DateTime dataInicial = new DateTime();
                    if (!DateTime.TryParse(txtDataInicio.Text, out dataInicial))
                        dataInicial = DateTime.Today;

                    DateTime dataFinal = new DateTime();
                    if (!DateTime.TryParse(txtDataFim.Text, out dataFinal))
                        dataFinal = DateTime.Today;

                    Int32 codigoEmissor = SessaoAtual.CodigoEntidade;

                    List<Bandeira> bandeirasICABID = new List<Bandeira>();
                    bandeirasICABID = this.RecuperarBandeirasICABID();

                    using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                    {
                        prePagamentos = context.Cliente.ConsultarPrePagamento(codigoEmissor, dataInicial, dataFinal, bandeirasICABID);

                        Double valorSaldoAntecipado = (from pre in prePagamentos
                                                       select pre.SaldoAntecipado).Sum();

                        Double valorSaldoLiquidoPagar = (from pre in prePagamentos
                                                         select pre.ValorPagarReceber).Sum();

                        Double valorSaldoEstoque = (from pre in prePagamentos
                                                    select pre.SaldoEstoque).Sum();

                        this.SumarizacaoPrePagamento = new ServicoEmissores.PrePagamento();
                        SumarizacaoPrePagamento.SaldoAntecipado = valorSaldoAntecipado;
                        SumarizacaoPrePagamento.ValorPagarReceber = valorSaldoLiquidoPagar;
                        SumarizacaoPrePagamento.SaldoEstoque = valorSaldoEstoque;

                        this.PeriodoIncio = dataInicial;
                        this.PeriodoFim = dataFinal;
                    }

                    return prePagamentos;
                }
                catch (FaultException<ServicoEmissores.GeneralFault> ex)
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

                return prePagamentos;
            }
        }
    }
}