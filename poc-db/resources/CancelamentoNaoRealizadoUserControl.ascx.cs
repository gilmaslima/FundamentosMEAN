#region Histórico do Arquivo
/*
(c) Copyright [2012] BRQ IT Solutions.
Autor       : [- 2012/08/21 - Lucas Nicoletto da Cunha]
Empresa     : [BRQ IT Solutions]
Histórico   : Criação da Classe
- [08/06/2012] – [Lucas Nicoletto da Cunha] – [Criação]
 *
*/
#endregion

using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using System.Collections.Generic;
using Redecard.PN.Cancelamento.Sharepoint.Modelos;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.CancelamentoNaoRealizado
{
    public partial class CancelamentoNaoRealizadoUserControl : UserControlBase
    {
        #region Metodos Tela
        /// <summary>
        /// Método executado no inicio de cada requisição.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(base.SessaoAtual, null))
            {
                using (Logger Log = Logger.IniciarLog("Cancelamento não realizado - Page Load"))
                {
                    try
                    {
                        EstabelecimentoCancelamento estabelecimento = null;
                        using (ServicoPortalCancelamentoClient client = new ServicoPortalCancelamentoClient())
                        {
                            estabelecimento = client.RetornaDadosEstabelecimentoCancelamento(this.SessaoAtual.CodigoEntidade);
                        }
                        if (estabelecimento == null || !estabelecimento.Centralizadora)
                        {
                            ScriptManager.RegisterStartupScript(Page, GetType(), "key", "$('.ocultar').hide();", true);
                        }

                        if (!IsPostBack)
                        {
                            if ((bool)Session["CancelamentoLote"] == true)
                            {
                                TituloPaginaNaoRealizado.Text = " Cancelamento de Vendas por Lote";
                            }


                            List<ItemCancelamentoSaida> resultadoSaida = Session["ItensSaida"] as List<ItemCancelamentoSaida>;
                            List<ItemCancelamentoEntrada> LstItemCancelamentoEntrada = (List<ItemCancelamentoEntrada>)Session["ItensEntrada"];

                            if (resultadoSaida != null && LstItemCancelamentoEntrada != null)
                            {
                                List<ItemCancelamentoSaida> listaErros = resultadoSaida.Where(x => int.Parse(x.CodRetorno) != 0).ToList();

                                CarregarDados(listaErros);
                            }
                        }
                    }

                    catch (PortalRedecardException ex)
                    {
                        ExibirPainelExcecao(ex);
                        SharePointUlsLog.LogErro(ex.Message);
                        Log.GravarErro(ex);
                    }

                    catch (Exception ex)
                    {
                        SharePointUlsLog.LogErro(ex.Message);
                        Log.GravarErro(ex);
                    }
                }
            }
        }


        private void VerificaDuplicados(List<ItemCancelamentoEntrada> entrada)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento não realizado - Verificação de Duplicados"))
            {
                try
                {
                    List<ItemCancelamentoSaida> resultadoSaida = Session["ItensSaida"] as List<ItemCancelamentoSaida>;
                    resultadoSaida = resultadoSaida.Where(x => x.CodRetorno.CompareTo("60") == 0).ToList();

                    if (resultadoSaida.Count > 0)
                    {
                        List<ItemCancelamentoEntrada> Duplicadas = new List<ItemCancelamentoEntrada>();
                        resultadoSaida.ForEach(x => Duplicadas.Add(entrada[resultadoSaida.IndexOf(x)]));

                        using (ServicoPortalCancelamentoClient client = new ServicoPortalCancelamentoClient())
                        {

                            List<ModConsultaDuplicado> DuplicadosSaida = client.ConsultaDuplicados(Duplicadas);
                            Session["DuplicadosSaida"] = DuplicadosSaida;

                        }

                        if (resultadoSaida.Count == entrada.Count)
                        {
                            try
                            {
                                Response.Redirect("pn_VendaDuplicadaConfirmar.aspx");
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        else
                        {
                            btnVendasDuplicadas.Visible = true;
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    ExibirPainelExcecao(ex);
                    SharePointUlsLog.LogErro(ex);
                    Log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex.Message);
                    Log.GravarErro(ex);
                }
            }
        }

        /// <summary>
        /// Carrega os dados com erro no cancelamento.
        /// </summary>
        /// <param name="listaErros"></param>
        private void CarregarDados(List<ItemCancelamentoSaida> listaErros)
        {
            string mensagensErros = string.Empty;

            //ItemCancelamentoSaida saida = listaErros.FirstOrDefault();
            List<ItemCancelamentoSaida> resultadoSaida = Session["ItensSaida"] as List<ItemCancelamentoSaida>;
            List<ItemCancelamentoEntrada> LstItemCancelamentoEntrada = (List<ItemCancelamentoEntrada>)Session["ItensEntrada"];

            List<ItemCancelamentoEntrada> listaRetorno = new List<ItemCancelamentoEntrada>();

            int index = 1;

            foreach (ItemCancelamentoSaida saida in listaErros)
            {
                ItemCancelamentoEntrada entrada = LstItemCancelamentoEntrada[resultadoSaida.IndexOf(saida)];
                entrada.mensagemErro = string.Format("<span align='left'>{0}</span>", saida.MsgErro);
                listaRetorno.Add(entrada);

                mensagensErros += string.Format("Linha {0}: {1}<br/>", index, saida.MsgErro);
                index++;
            }

            VerificaDuplicados(listaRetorno);

            rptErros.DataSource = listaRetorno;
            rptErros.DataBind();

            this.lblMsgErro.Text = mensagensErros;
        }

        protected void cpRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RadioButtonList radio = e.Item.FindControl("tipo1") as RadioButtonList;

            if (radio != null)
            {
                if (DataBinder.Eval(e.Item.DataItem, "FormaVenda") == null)
                {
                    radio.SelectedIndex = 0;
                }
                else
                {
                    if (DataBinder.Eval(e.Item.DataItem, "FormaVenda").ToString().CompareTo("Total") == 0)
                    {
                        radio.SelectedIndex = 0;
                    }
                    else
                    {
                        radio.SelectedIndex = 1;
                    }
                }
            }

            DropDownList dropDown = e.Item.FindControl("drop_tivpo_venda_1") as DropDownList;

            if (dropDown != null)
            {
                if (DataBinder.Eval(e.Item.DataItem, "TpVenda") == null)
                {
                    dropDown.SelectedIndex = 0;
                }
                else
                {
                    if (DataBinder.Eval(e.Item.DataItem, "TpVenda").ToString().CompareTo("RO") == 0)
                    {
                        dropDown.SelectedIndex = 0;
                    }
                    else if (DataBinder.Eval(e.Item.DataItem, "TpVenda").ToString().CompareTo("PC") == 0)
                    {
                        dropDown.SelectedIndex = 1;
                    }
                    else
                    {
                        dropDown.SelectedIndex = 2;
                    }
                }

            }
        }

        /// <summary>
        /// Método executado no clique do botão cancelar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btCancel_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento Não Realizado - Cancelar"))
            {
                CarregarMudancas();

                if (Session["DuplicadosSaida"] != null)
                {
                    Session.Remove("DuplicadosSaida");
                }

                try
                {
                    Response.Redirect("pn_ConfirmacaoCancelamento.aspx");
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                }
            }
        }

        protected void Cancelamento_Voltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento não Realizado - Voltar"))
            {
                CarregarMudancas();

                if (Session["DuplicadosSaida"] != null)
                {
                    Session.Remove("DuplicadosSaida");
                }

                try
                {
                    Response.Redirect("pn_cancelamentovendas.aspx");
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Método para carregar as mudança realizadas na tela.
        /// </summary>
        private void CarregarMudancas()
        {
            List<ItemCancelamentoEntrada> items = new List<ItemCancelamentoEntrada>();

            foreach (RepeaterItem item in this.rptErros.Items)
            {
                if ((item.FindControl("tipo1") as RadioButtonList).Text == "Total")
                {
                    if (!string.IsNullOrEmpty((item.FindControl("nsu1") as TextBox).Text) && !string.IsNullOrEmpty((item.FindControl("valor1") as TextBox).Text))
                    {

                        ItemCancelamentoEntrada PrimeiraLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty((item.FindControl("numEst1") as TextBox).Text))
                        {
                            PrimeiraLinha.NumEstabelecimento = Int32.Parse((item.FindControl("numEst1") as TextBox).Text);
                        }
                        else
                        {
                            PrimeiraLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        PrimeiraLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        PrimeiraLinha.NumCartao = (item.FindControl("nsu1") as TextBox).Text;
                        PrimeiraLinha.NSU = (item.FindControl("nsu1") as TextBox).Text;
                        PrimeiraLinha.DtTransf = (item.FindControl("data1") as TextBox).Text.ToDate();
                        PrimeiraLinha.DtTransfInt = int.Parse(PrimeiraLinha.DtTransf.ToString("ddMMyyyy"));

                        PrimeiraLinha.VlTrans = decimal.Parse((item.FindControl("valor1") as TextBox).Text);
                        PrimeiraLinha.FormaVenda = (item.FindControl("tipo1") as RadioButtonList).Text;
                        PrimeiraLinha.TpVenda = (item.FindControl("drop_tivpo_venda_1") as DropDownList).SelectedValue;
                        PrimeiraLinha.VlCanc = decimal.Parse((item.FindControl("valor1") as TextBox).Text); // o valor de cancelamento é o mesmo do total

                        if (PrimeiraLinha.VlCanc.ToString().IndexOf(".") > 0) PrimeiraLinha.VlCancStr = PrimeiraLinha.VlCanc.ToString().Remove(PrimeiraLinha.VlCanc.ToString().IndexOf("."), 1);
                        if (PrimeiraLinha.VlTrans.ToString().IndexOf(".") > 0) PrimeiraLinha.VlTransStr = PrimeiraLinha.VlTrans.ToString().Remove(PrimeiraLinha.VlTrans.ToString().IndexOf("."), 1);

                        if (PrimeiraLinha.VlCanc.ToString().IndexOf(",") > 0) PrimeiraLinha.VlCancStr = PrimeiraLinha.VlCanc.ToString().Remove(PrimeiraLinha.VlCanc.ToString().IndexOf(","), 1);
                        if (PrimeiraLinha.VlTrans.ToString().IndexOf(",") > 0) PrimeiraLinha.VlTransStr = PrimeiraLinha.VlTrans.ToString().Remove(PrimeiraLinha.VlTrans.ToString().IndexOf(","), 1);

                        if (string.IsNullOrEmpty(PrimeiraLinha.VlCancStr)) PrimeiraLinha.VlCancStr = PrimeiraLinha.VlCanc.ToString();
                        if (string.IsNullOrEmpty(PrimeiraLinha.VlTransStr)) PrimeiraLinha.VlTransStr = PrimeiraLinha.VlTrans.ToString();

                        if (items.Where(x => x.NSU.CompareTo(PrimeiraLinha.NSU) == 0 && x.VlTrans.CompareTo(PrimeiraLinha.VlTrans) == 0 && x.DtTransf.CompareTo(PrimeiraLinha.DtTransf) == 0).FirstOrDefault() == null)
                        {
                            items.Add(PrimeiraLinha);
                        }
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty((item.FindControl("nsu1") as TextBox).Text) && !string.IsNullOrEmpty((item.FindControl("valor1") as TextBox).Text) && !string.IsNullOrEmpty((item.FindControl("valCancel1") as TextBox).Text))
                    {

                        ItemCancelamentoEntrada PrimeiraLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty((item.FindControl("numEst1") as TextBox).Text))
                        {
                            PrimeiraLinha.NumEstabelecimento = Int32.Parse((item.FindControl("numEst1") as TextBox).Text);
                        }
                        else
                        {
                            PrimeiraLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        PrimeiraLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        PrimeiraLinha.NumCartao = (item.FindControl("nsu1") as TextBox).Text;
                        PrimeiraLinha.NSU = (item.FindControl("nsu1") as TextBox).Text;
                        PrimeiraLinha.DtTransf = (item.FindControl("data1") as TextBox).Text.ToDate();
                        PrimeiraLinha.DtTransfInt = int.Parse(PrimeiraLinha.DtTransf.ToString("ddMMyyyy"));

                        PrimeiraLinha.VlTrans = decimal.Parse((item.FindControl("valor1") as TextBox).Text);
                        PrimeiraLinha.FormaVenda = (item.FindControl("tipo1") as RadioButtonList).Text;
                        PrimeiraLinha.TpVenda = (item.FindControl("drop_tivpo_venda_1") as DropDownList).SelectedValue;
                        PrimeiraLinha.VlCanc = decimal.Parse((item.FindControl("valCancel1") as TextBox).Text);

                        if (PrimeiraLinha.VlCanc.ToString().IndexOf(".") > 0) PrimeiraLinha.VlCancStr = PrimeiraLinha.VlCanc.ToString().Remove(PrimeiraLinha.VlCanc.ToString().IndexOf("."), 1);
                        if (PrimeiraLinha.VlTrans.ToString().IndexOf(".") > 0) PrimeiraLinha.VlTransStr = PrimeiraLinha.VlTrans.ToString().Remove(PrimeiraLinha.VlTrans.ToString().IndexOf("."), 1);

                        if (PrimeiraLinha.VlCanc.ToString().IndexOf(",") > 0) PrimeiraLinha.VlCancStr = PrimeiraLinha.VlCanc.ToString().Remove(PrimeiraLinha.VlCanc.ToString().IndexOf(","), 1);
                        if (PrimeiraLinha.VlTrans.ToString().IndexOf(",") > 0) PrimeiraLinha.VlTransStr = PrimeiraLinha.VlTrans.ToString().Remove(PrimeiraLinha.VlTrans.ToString().IndexOf(","), 1);

                        if (string.IsNullOrEmpty(PrimeiraLinha.VlCancStr)) PrimeiraLinha.VlCancStr = PrimeiraLinha.VlCanc.ToString();
                        if (string.IsNullOrEmpty(PrimeiraLinha.VlTransStr)) PrimeiraLinha.VlTransStr = PrimeiraLinha.VlTrans.ToString();

                        if (items.Where(x => x.NSU.CompareTo(PrimeiraLinha.NSU) == 0 && x.VlTrans.CompareTo(PrimeiraLinha.VlTrans) == 0 && x.DtTransf.CompareTo(PrimeiraLinha.DtTransf) == 0).FirstOrDefault() == null)
                        {
                            items.Add(PrimeiraLinha);
                        }
                    }

                }
            }

            Session["ItensEntrada"] = items;
        }
        #endregion

        protected void btDuplicadas_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento não realizado - Duplicados"))
            {
                try
                {
                    CarregarMudancas();

                    Response.Redirect("pn_VendaDuplicadaConfirmar.aspx");
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                }
            }
        }
    }
}
