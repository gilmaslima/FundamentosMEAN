using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.OutrasEntidades.SharePoint.HISPBServico;

namespace Redecard.PN.OutrasEntidades.SharePoint.WebParts.BancosSPB.ISSPB
{
    public partial class ISSPBUserControl : UserControlBase
    {
        #region constantes

        const string MSG_CREDITO = "<B>Valores a <font color='green'>Receber</font> &nbsp;(Aguardar Lançamento do Crédito da Origem)</B>";
        const string MSG_DEBITO = "<B>Valores a <font color='red'>Pagar</font> &nbsp;(Executar Lançamento a Crédito ao(s) Banco(s) Destino - via STR)</B>";
        #endregion

        /// <summary>
        /// Guarda o tipo de movimentação da gradeSPB consultada. Utilizado para mudar á informação de acordo com o tipo de movimentação.
        /// </summary>
        public string TipoMovimentacao
        {
            get { return ViewState["TipoMovimentacao"] != null ? ViewState["TipoMovimentacao"].ToString() : string.Empty; }
            set { ViewState["TipoMovimentacao"] = value; }
        }

        /// <summary>
        /// Inicialização da webpart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (DateTime.Now.Hour >= 15)
                {
                    pnlQuadro.Visible = true;
                    Panel[] paineis = new Panel[1] { pnlSPB };
                    base.ExibirPainelConfirmacaoAcao("", "Horário limite para consulta expirado (Horário Limite: 15:00 Horas)", SPContext.Current.Web.Url, paineis, "icone-aviso");
                }
                else
                    //Carrega os dados da tela
                    CarregarISSPB();
            }
        }

        /// <summary>
        /// CarregarISSPB
        /// </summary>
        void CarregarISSPB()
        {
            pnlSPB.Visible = true;
            Int32 codRetorno;
            String mensagem, retorno, ispb, usuario, dataContabil;

#if DEBUG
            if (SessaoAtual == null)
            {
                ispb = "33700394";
                usuario = "usuarioTeste";
            }
            else
            {
                ispb = SessaoAtual.CodigoEntidade.ToString();
                usuario = SessaoAtual.NomeUsuario;
            }
#else
            // Recupera o Código do ISPB da sessão
            ispb = SessaoAtual.CodigoEntidade.ToString();
            // Recupera o usuario da sessão
            usuario = SessaoAtual.NomeUsuario;
#endif

            using (Logger Log = Logger.IniciarLog("Carregando ISSPB"))
            {
                try
                {
                    using (HISServicoPBClient client = new HISServicoPBClient())
                    {
                        //Consulta os Dados SPB
                        GradeLiquidacao[] itens = client.ExtrairDadosSPB(out codRetorno, out mensagem, out retorno, out dataContabil, ispb, usuario);

                        dataContabil = dataContabil.PadLeft(6, '0');

                        String diaContabil = dataContabil.Substring(4, 2);
                        String mesContabil = dataContabil.Substring(2, 2);
                        String anoContabil = dataContabil.Substring(0, 2);

                        String dataContabilSPB = String.Concat(diaContabil, "/", mesContabil, "/", anoContabil);

                        if (codRetorno != 0)
                        {
                            rptBancos.Visible = true;

                            base.ExibirPainelExcecao("HISPBServico.ExtrairDadosSPB", codRetorno);
                        }
                        else
                        {

                            rptBancos.DataSource = ProcessaDados(itens);
                            rptBancos.DataBind();

                            // Consulta os Detalhes do SPB
                            GradeLiquidacaoBandeira gradeLiquidacaoBandeira = client.ExtrairDetalhesSPB(out codRetorno, out mensagem, out retorno, out dataContabil, ispb, usuario);
                            lblDtEmissao.Text = DateTime.Now.ToString("dd/MM/yyyy");
                            lblDtContabil.Text = Convert.ToDateTime(dataContabilSPB).ToString("dd/MM/yyyy");

                            if (codRetorno != 0)
                            {
                                base.ExibirPainelExcecao("HISPBServico.ExtrairDetalhesSPB", codRetorno);
                            }
                            else
                            {
                                // Preenhe os detalhes
                                PreencherDetalhe(gradeLiquidacaoBandeira);
                            }
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    rptBancos.Visible = false;
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega a lista de GradeLiquidacaoSPB agrupando por tipo de registro.
        /// </summary>
        /// <param name="itens">Array com os dados retornados pela consulta ExtrairDadosSPB</param>
        /// <returns>lista de GradeLiquidacaoSPB agrupando por tipo de registro.</returns>
        List<GradeLiquidacaoSPB> ProcessaDados(GradeLiquidacao[] itens)
        {
            List<GradeLiquidacaoSPB> lista = new List<GradeLiquidacaoSPB>();
            GradeLiquidacaoSPB grade;
            GradeLiquidacao item;
            int i = 0;
            int cnt = itens.Length;

            while (i < cnt)
            {
                grade = new GradeLiquidacaoSPB();

                if (i < cnt && itens[i].Tipo == 1)
                {
                    grade.Ispb = itens[i].Ispb;
                    grade.Descricao = itens[i].Descricao;
                    grade.Banco = itens[i].Banco;
                    grade.Agencia = itens[i].Agencia;
                    grade.ContaCorrente = itens[i].ContaCorrente;
                    grade.Tipo = itens[i].Tipo;
                    grade.TipoMovimentacao = itens[i].TipoMovimentacao;
                    grade.TipoSolicitacao = itens[i].TipoSolicitacao;
                    grade.ValorSaldoLiquidacao = itens[i].ValorSaldoLiquidacao;
                    i++;
                    grade.Itens = new List<GradeLiquidacao>();
                    while (i < cnt && itens[i].Tipo != 1)
                    {
                        item = new GradeLiquidacao();
                        item.Ispb = itens[i].Ispb;
                        item.Descricao = itens[i].Descricao;
                        item.Banco = itens[i].Banco;
                        item.Agencia = itens[i].Agencia;
                        item.ContaCorrente = itens[i].ContaCorrente;
                        item.Tipo = itens[i].Tipo;
                        item.TipoMovimentacao = itens[i].TipoMovimentacao;
                        item.TipoSolicitacao = itens[i].TipoSolicitacao;
                        item.ValorSaldoLiquidacao = itens[i].ValorSaldoLiquidacao;
                        grade.Itens.Add(item);
                        i++;
                    }
                }
                lista.Add(grade);
            }
            return lista;
        }

        /// <summary>
        /// Preenche os campos na tela com as informações da GradeLiquidacaoBandeira passada.
        /// Caso o valor da bandeira seja 0 não será exibido na tela.
        /// </summary>
        /// <param name="gradeLiquidacaoBandeira">Objeto do tipo GradeLiquidacaoBandeira </param>
        void PreencherDetalhe(GradeLiquidacaoBandeira gradeLiquidacaoBandeira)
        {
            string sinal = string.Empty;

            #region [Crédito]

            //Crédito Master
            if (gradeLiquidacaoBandeira.ValorCreditoMaster != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalCreditoMaster.Equals("D") ? "-" : string.Empty;
                lblValorMaster.Text = sinal + gradeLiquidacaoBandeira.ValorCreditoMaster.ToString("N2");
            }
            else
            {
                trMaster.Visible = false;
            }

            //Crédito Cabal
            if (gradeLiquidacaoBandeira.ValorCreditoCabal != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalCreditoCabal.Equals("D") ? "-" : string.Empty;
                lblValorCabal.Text = sinal + gradeLiquidacaoBandeira.ValorCreditoCabal.ToString("N2");
            }
            else
            {
                trCabal.Visible = false;
            }

            //Crédito Construcard
            if (gradeLiquidacaoBandeira.ValorCreditoConstrucard != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalCreditoConstrucard.Equals("D") ? "-" : string.Empty;
                lblValorConstrucard.Text = sinal + gradeLiquidacaoBandeira.ValorCreditoConstrucard.ToString("N2");
            }
            else
            {
                trConstrucard.Visible = false;
            }

            //Crédito VISA  
            if (gradeLiquidacaoBandeira.ValorCreditoVisa != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalCreditoVisa.Equals("D") ? "-" : string.Empty;
                lblValorVisa.Text = sinal + gradeLiquidacaoBandeira.ValorCreditoVisa.ToString("N2");
            }
            else
            {
                trVisa.Visible = false;
            }

            //Crédito Sicredi
            if (gradeLiquidacaoBandeira.ValorCreditoSicredi != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalCreditoSicredi.Equals("D") ? "-" : string.Empty;
                lblValorSicredi.Text = sinal + gradeLiquidacaoBandeira.ValorCreditoSicredi.ToString("N2");
            }
            else
            {
                trSicredi.Visible = false;
            }

            //Crédito Hiper
            if (gradeLiquidacaoBandeira.ValorCreditoHipercard != 0)
            {
                sinal = gradeLiquidacaoBandeira.ValorCreditoHipercard.Equals("D") ? "-" : string.Empty; // TODO [DOL]: Esta errada a comparação de SINAL, corrigir?
                lblValorHiper.Text = sinal + gradeLiquidacaoBandeira.ValorCreditoHipercard.ToString("N2");
            }
            else
            {
                trHiper.Visible = false;
            }

            //Crédito Banescard
            if (gradeLiquidacaoBandeira.ValorCreditoBanescard != 0)
            {
                lblValorBanescard.Text = String.Format("{0}{1}",
                                                        String.Compare(gradeLiquidacaoBandeira.SinalCreditoBanescard, "D", true) == 0 ? "-" : String.Empty, 
                                                        gradeLiquidacaoBandeira.ValorCreditoBanescard.ToString("N2"));
            }
            else
            {
                trBanescard.Visible = false;
            }


            //Crédito Elo
            if (gradeLiquidacaoBandeira.ValorCreditoElo != 0)
            {
                lblValorElo.Text = String.Format("{0}{1}",
                                                        String.Compare(gradeLiquidacaoBandeira.SinalCreditoElo, "D", true) == 0 ? "-" : String.Empty,
                                                        gradeLiquidacaoBandeira.ValorCreditoElo.ToString("N2"));
            }
            else
            {
                trElo.Visible = false;
            }

            //Valor Crédito Total
            if (gradeLiquidacaoBandeira.ValorCreditoSaldo != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalCreditoSaldo.Equals("D") ? "-" : string.Empty;
                lblValTotal1.Text = sinal + gradeLiquidacaoBandeira.ValorCreditoSaldo.ToString("N2");
            }
            else
            {
                trTotal1.Visible = false;
            }

            #endregion

            #region [Débito]

            //Débito Master
            if (gradeLiquidacaoBandeira.ValorDebitoMaster != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalDebitoMaster.Equals("D") ? "-" : string.Empty;
                lblValorMasterDeb.Text = sinal + gradeLiquidacaoBandeira.ValorDebitoMaster.ToString("N2");
            }
            else
            {
                trMasterDeb.Visible = false;
            }

            //Débito Cabal
            if (gradeLiquidacaoBandeira.ValorDebitoCabal != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalDebitoCabal.Equals("D") ? "-" : string.Empty;
                lblValorCabalDeb.Text = sinal + gradeLiquidacaoBandeira.ValorDebitoCabal.ToString("N2");
            }
            else
            {
                trCabalDeb.Visible = false;
            }

            //Débito Construcard
            if (gradeLiquidacaoBandeira.ValorDebitoConstrucard != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalDebitoConstrucard.Equals("D") ? "-" : string.Empty;
                lblValorConstrucardDeb.Text = sinal + gradeLiquidacaoBandeira.ValorDebitoConstrucard.ToString("N2");
            }
            else
            {
                trConstrucardDeb.Visible = false;
            }

            //Débito VISA
            if (gradeLiquidacaoBandeira.ValorDebitoVisa != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalDebitoVisa.Equals("D") ? "-" : string.Empty;
                lblValorVisaDeb.Text = sinal + gradeLiquidacaoBandeira.ValorDebitoVisa.ToString("N2");
            }
            else
            {
                trVisaDeb.Visible = false;
            }

            //Débito Sicredi
            if (gradeLiquidacaoBandeira.ValorDebitoSicredi != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalDebitoSicredi.Equals("D") ? "-" : string.Empty;
                lblValorSicredDeb.Text = sinal + gradeLiquidacaoBandeira.ValorDebitoSicredi.ToString("N2");
            }
            else
            {
                trSicrediDeb.Visible = false;
            }

            //Débito Hiper
            if (gradeLiquidacaoBandeira.ValorDebitoHipercard != 0)
            {
                sinal = gradeLiquidacaoBandeira.ValorDebitoHipercard.Equals("D") ? "-" : string.Empty;
                lblValorHiperDebito.Text = sinal + gradeLiquidacaoBandeira.ValorDebitoHipercard.ToString("N2");
            }
            else
            {
                trHiperDebito.Visible = false;
            }

            //Débito Banescard
            if (gradeLiquidacaoBandeira.ValorDebitoBanescard != 0)
            {
                lblValorBanescardDebito.Text = String.Format("{0}{1}",
                                                              String.Compare(gradeLiquidacaoBandeira.SinalDebitoBanescard, "D", true) == 0 ? "-" : String.Empty,
                                                              gradeLiquidacaoBandeira.ValorDebitoBanescard.ToString("N2"));
            }
            else
            {
                trBanescardDebito.Visible = false;
            }


            //Débito Elo
            if (gradeLiquidacaoBandeira.ValorDebitoElo != 0)
            {
                lblValorEloDebito.Text = String.Format("{0}{1}",
                                                              String.Compare(gradeLiquidacaoBandeira.SinalDebitoElo, "D", true) == 0 ? "-" : String.Empty,
                                                              gradeLiquidacaoBandeira.ValorDebitoElo.ToString("N2"));
            }
            else
            {
                trEloDebito.Visible = false;
            }

            //Débito Total
            if (gradeLiquidacaoBandeira.ValorDebitoSaldo != 0)
            {
                sinal = gradeLiquidacaoBandeira.SinalDebitoSaldo.Equals("D") ? "-" : string.Empty;
                lblValorTotal2.Text = sinal + gradeLiquidacaoBandeira.ValorDebitoSaldo.ToString("N2");
            }
            else
            {
                trTotal2.Visible = false;
            }

            #endregion

            decimal valorNet = (gradeLiquidacaoBandeira.ValorCreditoSaldo - gradeLiquidacaoBandeira.ValorDebitoSaldo);
            lblValorNet.Text = valorNet.ToString("N2");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptBancos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                GradeLiquidacaoSPB item = (GradeLiquidacaoSPB)e.Item.DataItem;


                Label lblDestBanco = (Label)e.Item.FindControl("lblDestBanco");
                Label lblDescricaoBanco = (Label)e.Item.FindControl("lblDescricaoBanco");
                Label lblIspbBanco = (Label)e.Item.FindControl("lblIspbBanco");
                Label lblAgenciaBanco = (Label)e.Item.FindControl("lblAgenciaBanco");
                Label lblContaBanco = (Label)e.Item.FindControl("lblContaBanco");
                Label lblMsgDebCred = (Label)e.Item.FindControl("lblMsgDebCred");
                Repeater rbtBancoItens = (Repeater)e.Item.FindControl("rbtBancoItens");
                Literal ltlItens = (Literal)e.Item.FindControl("ltlItens");
                Label lblTotalReceb = (Label)e.Item.FindControl("lblTotalReceb");
                Label lvlValorTotal = (Label)e.Item.FindControl("lvlValorTotal");
                Label lblMensagemCredito = (Label)e.Item.FindControl("lblMensagemCredito");

                TipoMovimentacao = item.TipoMovimentacao;
                // Muda o label conforme tipo de movimentação.
                if (TipoMovimentacao.Equals("C"))
                {
                    lblDestBanco.Text = "Bco. Destino";
                    //                    lblDestBanco.ForeColor = System.Drawing.Color.Green;

                    lblMsgDebCred.Text = MSG_CREDITO;

                    lblTotalReceb.Text = "Total a Receber :";

                }
                else
                {
                    lblDestBanco.Text = "Bco. Origem";
                    //                    lblDestBanco.ForeColor = System.Drawing.Color.Red;
                    lblMsgDebCred.Text = MSG_DEBITO;
                    lblTotalReceb.Text = "Total a Pagar :";
                }
                lblDescricaoBanco.Text = item.Descricao;
                lblIspbBanco.Text = item.Ispb.ToString();
                lblAgenciaBanco.Text = item.Agencia;
                lblContaBanco.Text = item.ContaCorrente;
                lvlValorTotal.Text = item.ValorSaldoLiquidacao.ToString("N2");
                // se a descrição é igual a UNIBANCO (*), mostra a mensagem.
                if (item.Descricao == "UNIBANCO (*)")
                {
                    if (TipoMovimentacao.Equals("C"))
                        lblMensagemCredito.Text = "(*) Creditar Conta Corrente Redecard";
                    else
                        lblMensagemCredito.Text = "(*) Sacar Conta Corrente Redecard";
                }
                rbtBancoItens.DataSource = item.Itens;
                rbtBancoItens.DataBind();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbtBancoItens_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                Label lblBancoOrigDestItem = (Label)e.Item.FindControl("lblBancoOrigDestItem");
                if (TipoMovimentacao == "C")
                {
                    lblBancoOrigDestItem.Text = "Bco. Origem";
                    //lblBancoOrigDestItem.ForeColor = System.Drawing.Color.Red;

                }
                else
                {
                    lblBancoOrigDestItem.Text = "Bco. Destino";
                    //lblBancoOrigDestItem.ForeColor = System.Drawing.Color.Green;

                }
            }
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblBancoItem = (Label)e.Item.FindControl("lblBancoItem");
                Label lblISPBItem = (Label)e.Item.FindControl("lblISPBItem");
                Label lblAgenciaItem = (Label)e.Item.FindControl("lblAgenciaItem");
                Label lblContaItem = (Label)e.Item.FindControl("lblContaItem");
                Label lblValorItem = (Label)e.Item.FindControl("lblValorItem");

                GradeLiquidacao item = (GradeLiquidacao)e.Item.DataItem;
                lblBancoItem.Text = item.Descricao;
                lblISPBItem.Text = item.Ispb.ToString();
                lblAgenciaItem.Text = item.Agencia;
                lblContaItem.Text = item.ContaCorrente;
                lblValorItem.Text = item.ValorSaldoLiquidacao.ToString("N2");
            }

        }

        /// <summary>
        /// Imprimir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            string html = HdnValue.Value;
            ExportToExcel(ref html);
        }

        /// <summary>
        /// Exporta o contéudo da variável passada para Excel.
        /// </summary>
        /// <param name="html">Valor com o HTML que se deseja exportar.</param>
        public void ExportToExcel(ref string html)
        {
            try
            {
                html = html.Replace("&gt;", ">");
                html = html.Replace("&lt;", "<");
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename=GradeSPB.xls"));
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                //Abaixo codifica os caracteres para o alfabeto latino
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("Windows-1252");
                HttpContext.Current.Response.Charset = "ISO-8859-1";
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                    {
                        HttpContext.Current.Response.Write(html);
                        HttpContext.Current.Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante Exportação para Excel", ex);
                rptBancos.Visible = false;
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);

            }
        }
    }

    /// <summary>
    /// Classe para organizar as informações retornadas pelo método ExtrairDadosSPB
    /// </summary>
    public class GradeLiquidacaoSPB : GradeLiquidacao
    {
        public List<GradeLiquidacao> Itens { get; set; }
    }
}
