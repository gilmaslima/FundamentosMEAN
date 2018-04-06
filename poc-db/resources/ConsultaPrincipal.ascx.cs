using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.PN.Comum;
using Redecard.PN.Emissores.Sharepoint.ServicoEmissores;
using System.ServiceModel;
using System.Linq;
using System.ComponentModel;
using Redecard.PN.Emissores.Sharepoint.WebParts.ConsultaTravaDomicilio;
using Microsoft.SharePoint;

namespace Redecard.PN.Emissores.Sharepoint.ControlTemplates.TravaDomicilio
{
    public partial class ConsultaPrincipal : UserControlBase
    {
        static readonly DateTime inicioVigencia = new DateTime(2008, 08, 01);
        Int16 anoSelecionado = 0;
        public Int32 QuantidadeRegistros
        {
            get
            {
                return object.Equals(ViewState["QuantidadeRegistros"], null) ? 0 : ViewState["QuantidadeRegistros"].ToString().ToInt32();

            }
            set { ViewState["QuantidadeRegistros"] = value; }
        }

        public HisServicoZpEmissores.EntidadeConsultaTrava EntidadeConsulta
        {
            get
            {
                return object.Equals(ViewState["EntidadeConsulta"], null) ? null : (HisServicoZpEmissores.EntidadeConsultaTrava)ViewState["EntidadeConsulta"];

            }
            set { ViewState["EntidadeConsulta"] = value; }
        }

        public delegate void Parametros(object[] parametros, EventArgs e);

        [Browsable(true)]
        public event Parametros OnItemSelecionado;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlTotais.Visible = false;
            }
        }

        #region Busca Por Periodo
        public Boolean BuscaPorPeriodo(Guid IdPesquisa, Int16 funcao, Int32 numeroPV, decimal cnpj, Int16 ano, Int16 mes, Int32 codigoProduto)
        {
            Boolean retorno = true;
            pnlTotais.Visible = false;
            //pgnPeriodoEstabelecimento.Carregar();        
            Logger.IniciarLog("Busca por período - BuscaPorPeriodo");
            try
            {
                anoSelecionado = ano;
                Int16 codRetorno = 0;
                string mensagemRetorno = string.Empty;
                using (var context = new ContextoWCF<HisServicoZpEmissores.HisServicoZpEmissoresClient>())
                {
                    tblTotalizador.Visible = true;

                    Logger.GravarLog("HisServicoZpEmissores.ObtemInformacaoCobranca", new { mes, ano, SessaoAtual.CodigoEntidade });
                    List<HisServicoZpEmissores.InformacaoCobranca> lstInformacaoCobranca = ObtemInformacaoCobranca(mes, ano, (short)SessaoAtual.CodigoEntidade, 1);
                    if (!object.Equals(lstInformacaoCobranca, null))
                    {
                        if (inicioVigencia.CompareTo(new DateTime(ano, mes, 1)) > 0)
                        {

                            decimal precoMedioReferencia = lstInformacaoCobranca[0].PrecoMedioReferencia;


                            Logger.GravarLog("HisServicoZpEmissores.ConsultarTotaisCobranca", new { funcao, numeroPV, cnpj, SessaoAtual.CodigoEntidade, ano, mes, precoMedioReferencia });
                            EntidadeConsulta = context.Cliente.ConsultarTotaisCobranca(out codRetorno, out mensagemRetorno,
                                  funcao, numeroPV, cnpj, String.Empty, String.Empty, (Int16)SessaoAtual.CodigoEntidade, 1, ano, mes, precoMedioReferencia);

                            if (codRetorno != 0)
                            {
                                retorno = false;
                                //; base.ExibirPainelExcecao("HisServicoZpEmissores.ConsultarTotaisCobranca", codRetorno);
                            }
                            trTotalPagar.Visible = true;
                            ltlDescricaoTotalAPagar.Text = "<strong>Total a Pagar</strong>:";
                            ltlTotalPagar.Text = String.Format("{0} x {1}% = {2}", EntidadeConsulta.ValorTotalCobradoFaixas.ToString("N2"), precoMedioReferencia.ToString("N2"), EntidadeConsulta.ValorTotalCobranca.ToString("N2"));

                            ltlTotalCobrancaFooter.Text = EntidadeConsulta.ValorTotalCobradoFaixas.ToString("N2");
                        }
                        else
                        {
                            Logger.GravarLog("HisServicoZpEmissores.ConsultaPeriodo", new { SessaoAtual.CodigoEntidade, codigoProduto, mes, ano });
                            EntidadeConsulta = context.Cliente.ConsultaPeriodo(out codRetorno, (Int16)SessaoAtual.CodigoEntidade, codigoProduto, mes, ano);
                            trTotalPagar.Visible = false;

                            if (codRetorno != 0)
                            {
                                retorno = false;
                                //  base.ExibirPainelExcecao("HisServicoZpEmissores.ConsultaPeriodo", codRetorno);
                            }
                            ltlTotalCobrancaFooter.Text = EntidadeConsulta.ValorTotalCobranca.ToString("N2");

                        }
                        QuantidadeRegistros = object.Equals(EntidadeConsulta, null) ? 0 : EntidadeConsulta.DadosConsultaTravas.Count;


                        rptPeriodoEstabelecimento.Visible = QuantidadeRegistros > 0;
                        rptDados.Visible = false;
                        if (QuantidadeRegistros > 0)
                        {

                            rptPeriodoEstabelecimento.DataSource = EntidadeConsulta.DadosConsultaTravas;
                            rptPeriodoEstabelecimento.DataBind();
                            pnlDados.Visible = true;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao" + new Guid().ToString(), "pageResultTable('tblDados', 1, 30, 5);", true);
                        }
                        else
                        {
                            retorno = false;
                            pnlDados.Visible = false;
                        }
                        //txtTotal.Text = string.Format("{0:c}", entidade.TotalCobranca);
                        //VerificaControlesVisiveis(qtdRegistro, "Aviso", "Consulta Trava de Domicílio <br>Não há estabelecimentos com trava de domicílio para o período informado");
                    }
                    else
                    {
                        retorno = false;
                        pnlDados.Visible = false;
                    }
                }
            }
            catch (FaultException<PortalRedecardException> ex)
            {
                retorno = false;
                Logger.GravarErro("BuscaPorPeriodo - ", ex);
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);
            }
            catch (Exception ex)
            {
                retorno = false;
                Logger.GravarErro("BuscaPorPeriodo - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }

            return retorno;
        }
        #endregion


        # region Busca PV Travado
        public Boolean BuscaPvTravado(string NunCNPJCPF)
        {
            Logger.IniciarLog("Início método BuscaPvTravado");
            Boolean retorno = true;
            pnlTotais.Visible = false;
            try
            {
                using (var contexto = new ContextoWCF<ServicoPortalEmissoresClient>())
                {
                    List<DadosPV> lstDados = new List<DadosPV>();
                    Logger.GravarLog("Chamada ao método ConsultaPVTravado ", new { SessaoAtual.CodigoEntidade, NunCNPJCPF });
                    lstDados = contexto.Cliente.ConsultaPVTravado(SessaoAtual.CodigoEntidade, Convert.ToInt32(NunCNPJCPF));

                    Logger.GravarLog("Retorno chamada ao método ConsultaPVTravado ", new { lstDados });

                    QuantidadeRegistros = lstDados == null ? 0 : lstDados.Count;

                    rptPeriodoEstabelecimento.Visible = false;
                    rptDados.Visible = QuantidadeRegistros > 0;

                    rptDados.DataSource = lstDados;
                    rptDados.DataBind();

                    pnlDados.Visible = QuantidadeRegistros > 0;
                    tblTotalizador.Visible = false;
                    Logger.GravarLog("Retorno do método BuscaPvTravado", new { lstDados, QuantidadeRegistros });

                    //VerificaControlesVisiveis(qtdRegistro, "Aviso", "Consulta Trava de Domicílio <br>Não há trava de domicílio para o CNPJ / CPF informado");
                }
            }
            catch (FaultException<PortalRedecardException> ex)
            {
                retorno = false;
                Logger.GravarErro("BuscaPvTravado - ", ex);
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);

            }
            catch (Exception ex)
            {
                retorno = false;
                Logger.GravarErro("BuscaPvTravado - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);

            }
            return retorno;

        }
        #endregion

        #region Busca PV Não Travado
        public Boolean BuscaPvNaoTravado(string NunCNPJCPF)
        {
            Logger.IniciarLog("Início método BuscaPvNaoTravado");
            Boolean retorno = true;
            try
            {
                using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                {
                    List<DadosPV> entidade = new List<DadosPV>();
                    Logger.GravarLog("Chamada ao método ConsultaPVNaoTravado ", new { NunCNPJCPF });
                    entidade = context.Cliente.ConsultaPVNaoTravado(SessaoAtual.CodigoEntidade,
                                                           Convert.ToInt32(NunCNPJCPF));
                    Logger.GravarLog("Retorno chamada ao método ConsultaPVNaoTravado ", new { entidade });

                    QuantidadeRegistros = entidade == null ? 0 : entidade.Count;

                    rptPeriodoEstabelecimento.Visible = false;
                    rptDados.Visible = QuantidadeRegistros > 0;

                    rptDados.DataSource = entidade;
                    rptDados.DataBind();
                    pnlDados.Visible = QuantidadeRegistros > 0;
                    tblTotalizador.Visible = false;
                    Logger.GravarLog("Retorno do método BuscaPvNaoTravado", new { QuantidadeRegistros, entidade });

                    //VerificaControlesVisiveis(qtdRegistro, "Aviso", "Consulta Trava de Domicílio <br>Não há trava de domicílio para o CNPJ / CPF informado");
                }
            }
            catch (FaultException<PortalRedecardException> ex)
            {
                retorno = false;
                Logger.GravarErro("BuscaPvNaoTravado - ", ex);
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);

            }
            catch (Exception ex)
            {
                retorno = false;
                Logger.GravarErro("BuscaPvNaoTravado - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);

            }

            return retorno;
        }
        public void CarregarTotais(string NunCNPJCPF)
        {
            Logger.IniciarLog("Início método CarregarTotais");
            try
            {
                using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                {
                    
                    Logger.GravarLog("Chamada ao método ConsultaTotaisPV ", new { NunCNPJCPF });
                    TotaisPV totais = context.Cliente.ConsultaTotaisPV(SessaoAtual.CodigoEntidade,
                                                           Convert.ToInt32(NunCNPJCPF));
                    Logger.GravarLog("Retorno chamada ao método ConsultaTotaisPV ", new { totais });

                    pnlTotais.Visible = true;

                    if (!object.Equals(totais, null))
                    {

                        lblTotalDomiciliado.Text = totais.TotalDomiciliados.ToString();
                        lblTotalNaoDomiciliado.Text = totais.TotalNaoDomiciliados.ToString();
                        lblTotalCancelados.Text = totais.TotalCancelados.ToString();

                    }
                    else
                    {
                        lblTotalDomiciliado.Text = "0";
                        lblTotalNaoDomiciliado.Text = "0";
                        lblTotalCancelados.Text = "0";

                    }

                }
            }
            catch (FaultException<PortalRedecardException> ex)
            {
                Logger.GravarErro("CarregarTotais - ", ex);
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);

            }
            catch (Exception ex)
            {
                Logger.GravarErro("CarregarTotais - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);

            }

        }
        #endregion



        private List<HisServicoZpEmissores.InformacaoCobranca> ObtemInformacaoCobranca(Int16 mes, Int16 ano, Int16 codigoEmissor, Int16 codigoProduto)
        {
            List<HisServicoZpEmissores.InformacaoCobranca> lstInformacaoCobranca = null;
            Logger.GravarLog("Início da chamada ao método ObtemInformacaoCobranca");
            try
            {
                Int16 codRetorno = 0;
                using (var contexto = new ContextoWCF<HisServicoZpEmissores.HisServicoZpEmissoresClient>())
                {

                    Logger.GravarLog("Chamda serviço HisServicoZpEmissoresClient.ConsultaInformacaoCobranca", new { mes, ano, codigoProduto });
                    HisServicoZpEmissores.EntidadeConsultaTrava entidade = new HisServicoZpEmissores.EntidadeConsultaTrava();
                    String mensgemRetorno = string.Empty;

                    string dataDe = string.Empty;
                    string dataAte = string.Empty;

                    lstInformacaoCobranca = contexto.Cliente.ConsultaInformacaoCobranca(out codRetorno, codigoEmissor, codigoProduto, mes, ano);

                    Logger.GravarLog("Retorno chamda serviço HisServicoZpEmissoresClient.ConsultaInformacaoCobranca", new { lstInformacaoCobranca, codRetorno });

                    if (codRetorno != 0)
                    {
                        lstInformacaoCobranca = null;
                        //base.ExibirPainelExcecao("HisServicoZpEmissores.ConsultaInformacaoCobranca", codRetorno);
                    }
                }

            }
            catch (FaultException<PortalRedecardException> ex)
            {
                Logger.GravarErro("ObtemInformacaoCobranca - ", ex);
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("ObtemInformacaoCobranca - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            return lstInformacaoCobranca;
        }


        protected void rptPeriodoEstabelecimento_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {

                Literal ltlQuantidadePVs = e.Item.FindControl("ltlQuantidadePVs") as Literal;
                Literal ltlTotalCobrancaMasterCard = e.Item.FindControl("ltlTotalCobrancaMasterCard") as Literal;
                Literal ltlTotalCobrancaVisa = e.Item.FindControl("ltlTotalCobrancaVisa") as Literal;
                Literal ltlTotalCobranca = e.Item.FindControl("ltlTotalCobranca") as Literal;


                LinkButton lkbDetalhe = e.Item.FindControl("lkbDownload") as LinkButton;

                HisServicoZpEmissores.DadosConsultaTrava item = e.Item.DataItem as HisServicoZpEmissores.DadosConsultaTrava;

                ltlQuantidadePVs.Text = item.QuantidadePVs.ToString();
                ltlTotalCobrancaMasterCard.Text = item.TotalCobrancaMasterCard.ToString("N2");
                ltlTotalCobrancaVisa.Text = item.TotalCobrancaVisa.ToString("N2");
                ltlTotalCobranca.Text = item.TotalCobranca.ToString("N2");

                //funcao, numeroPV, cnpj, ano, mes,
                //lkbDetalhe.CommandArgument = String.Format("{0}|{1}|{2}",
                //    item.FaixaInicialFaturamento.ToString("N2"), item.FaixaFinalFaturamento.ToString("N2"), item.FatorMultiplicado.ToString("N2"));

                lkbDetalhe.CommandArgument = String.Format("{0}", anoSelecionado);
            }
        }

        protected void lkbDownload_Click(object sender, EventArgs e)
        {
            LinkButton lkbDownload = sender as LinkButton;

            String ano = lkbDownload.CommandArgument;


            QueryStringSegura qrySegura = new QueryStringSegura();
            qrySegura["Ano"] = ano;
            Response.Redirect(String.Concat(SPContext.Current.Web.Url, "/Paginas/DownloadExtrato.aspx?dados=", qrySegura.ToString()));


            /*
                        if (!object.Equals(lkbDetalhe, null))
                        {
                            if (!String.IsNullOrEmpty(lkbDetalhe.CommandArgument))
                            {
                                String[] arqumentos = lkbDetalhe.CommandArgument.Split('|');

                                if (arqumentos.Length == 3)
                                {
                                    decimal faixaInicial = arqumentos[0].ToDecimalNull(0).Value;
                                    decimal faixaFinal = arqumentos[1].ToDecimalNull(0).Value;
                                    decimal fator = arqumentos[2].ToDecimalNull(0).Value;

                                    object[] parametros = new object[] { faixaInicial, faixaFinal, fator };
                                    //CarregarDetalheFilho(rptFilho, faixaInicial, faixaFinal, fator);

                                    if (!object.ReferenceEquals(this.onItemSelecionado, null))
                                        this.onItemSelecionado(parametros, e);

                                }
                            }
                        }
                        */
        }



    }
}
