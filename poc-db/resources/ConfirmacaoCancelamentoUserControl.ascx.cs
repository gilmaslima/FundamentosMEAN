using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using Redecard.PN.Comum;
using System.Web;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.ConfirmacaoCancelamento
{
    public partial class ConfirmacaoCancelamentoUserControl : UserControlBase
    {
        List<ItemCancelamentoEntrada> ListaInicial;

        /// <summary>
        /// Método executado em cada carregamento da tela.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(base.SessaoAtual, null))
            {
                using (Logger Log = Logger.IniciarLog("Confirmação Cancelamento - Page Load"))
                {
                    EstabelecimentoCancelamento estabelecimento = null;
                    using (ServicoPortalCancelamentoClient client = new ServicoPortalCancelamentoClient())
                    {
                        try
                        {
                            estabelecimento = client.RetornaDadosEstabelecimentoCancelamento(this.SessaoAtual.CodigoEntidade);
                        }
                        catch (Exception ex)
                        {
                            SharePointUlsLog.LogErro(ex.Message);
                            Log.GravarErro(ex);
                        }
                    }

                    if (estabelecimento == null || !estabelecimento.Centralizadora)
                    {
                        //ScriptManager.RegisterStartupScript(Page, GetType(), "key", "$('.ocultar').hide();", true);
                        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "$('.ocultar').hide();", true);
                        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "document.getElementsByClassName('ocultar').style.display = 'none';", true);
                    }

                    if (!Page.IsPostBack)
                    {

                        if ((bool)Session["CancelamentoLote"] == true)
                        {
                            TituloPaginaConfirmacao.Text = "Cancelamentos de Vendas por Lote";
                        }

                        ListaInicial = new List<ItemCancelamentoEntrada>();
                        List<ItemCancelamentoEntrada> input = new List<ItemCancelamentoEntrada>();

                        if (Session["ItensEntrada"] != null)
                        {
                            input = (List<ItemCancelamentoEntrada>)Session["ItensEntrada"];
                            ListaInicial = input;
                            rptDados.DataSource = ListaInicial;
                            rptDados.DataBind();

                            CarregarDuplicados();
                        }

                        //Adiciona a função de JS de paginação do grid para as funções a serem executadas após renderização
                        //       ScriptManager.RegisterStartupScript(Page, GetType(), "key", "document.getElementById('ocultar').style.display = 'none';", true);
                        //        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "document.getElementById('ocultar2').style.display = 'none';", true);
                    }
                }
            }
        }

        /// <summary>
        /// Método para carregar os dados de itens duplicados.
        /// </summary>
        private void CarregarDuplicados()
        {
            using (Logger Log = Logger.IniciarLog("Confirmação Cancelamento - Carregar Duplicados"))
            {
                try
                {

                    if (Session["DuplicadosSaida"] != null)
                    {
                        List<ModConsultaDuplicado> ObjDuplicados = (List<ModConsultaDuplicado>)Session["DuplicadosSaida"];
                        List<ItemCancelamentoEntrada> lstEntrada = (List<ItemCancelamentoEntrada>)Session["ItensEntrada"];

                        lstEntrada = lstEntrada.Where(x => ObjDuplicados.Where(y => y.NumNSUCartao == x.NSUFormatado).FirstOrDefault() != null).ToList();
                        List<ItemCancelamentoEntrada> listaEntrada = new List<ItemCancelamentoEntrada>();

                        foreach (ItemCancelamentoEntrada entrada in lstEntrada)
                        {
                            ObjDuplicados.Where(x => x.NumNSUCartao == entrada.NSUFormatado).ToList().ForEach(x =>
                            {
                                ItemCancelamentoEntrada novaEntradaDuplicada = new ItemCancelamentoEntrada();
                                novaEntradaDuplicada.CodUserCanc = entrada.CodUserCanc;
                                novaEntradaDuplicada.DataHoraTransacao = entrada.DataHoraTransacao;
                                novaEntradaDuplicada.DtTransf = entrada.DtTransf;
                                novaEntradaDuplicada.FormaVenda = entrada.FormaVenda;
                                novaEntradaDuplicada.IPCanc = entrada.IPCanc;
                                novaEntradaDuplicada.NSU = entrada.NSU;
                                novaEntradaDuplicada.NumCartao = entrada.NumCartao;
                                novaEntradaDuplicada.NumEstabelecimento = entrada.NumEstabelecimento;
                                novaEntradaDuplicada.NumPDVCanc = entrada.NumPDVCanc;
                                novaEntradaDuplicada.TpVenda = entrada.TpVenda;
                                novaEntradaDuplicada.VlCanc = entrada.VlCanc;
                                novaEntradaDuplicada.VlTrans = entrada.VlTrans;
                                novaEntradaDuplicada.NumeroAutorizacao = x.Cd_aut_bnd;
                                novaEntradaDuplicada.DataHoraTransacao = x.Nu_trx_tcc;

                                listaEntrada.Add(novaEntradaDuplicada);
                            });
                        }

                        if (listaEntrada.Count > 0)
                        {
                            pnlDuplicadas.Visible = true;
                            rptCancelaDuplicados.DataSource = listaEntrada;
                            rptCancelaDuplicados.DataBind();
                        }

                        lstEntrada = ((List<ItemCancelamentoEntrada>)Session["ItensEntrada"]).Except(lstEntrada).ToList();

                        if (lstEntrada.Count == 0)
                        {
                            rptDados.Visible = false;
                        }
                        else
                        {
                            ListaInicial = lstEntrada;
                            rptDados.DataSource = ListaInicial;
                            rptDados.DataBind();
                        }

                        lstEntrada.AddRange(listaEntrada);

                        Session["ItensEntrada"] = lstEntrada;

                    }
                }
                catch (Exception Ex)
                {
                    SharePointUlsLog.LogErro(Ex.Message);
                    Log.GravarErro(Ex);
                }
            }
        }

        /// <summary>
        /// Método executado no clique do botão Confirmar Cancelamento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnConfirmarCancelamento_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação Cancelamento - Confirmar Cancelamento"))
            {
                using (ServiceCancelamento.ServicoPortalCancelamentoClient client = new ServiceCancelamento.ServicoPortalCancelamentoClient())
                {

                    List<ItemCancelamentoEntrada> LstItemCancelamentoEntrada = (List<ItemCancelamentoEntrada>)Session["ItensEntrada"];
                    List<ItemCancelamentoEntrada> input = new List<ItemCancelamentoEntrada>();

                    string address = "";

                    #if(!DEBUG)
                    {
                        HttpRequest request = base.Request;
                        address = request.ServerVariables["HTTP_TRUE_CLIENT_IP"];
                        if (String.IsNullOrEmpty(address))
                            address = request.ServerVariables["REMOTE_ADDR"];
                    }
                    #else
                    {
                        address = "172.168.4.86";
                    }
                    #endif

                    SharePointUlsLog.LogMensagem("IP USER:" + address);
                    Log.GravarMensagem("IP USER:" + address);

                    foreach (ItemCancelamentoEntrada item in LstItemCancelamentoEntrada.Where(x => string.IsNullOrEmpty(x.NumeroAutorizacao)))
                    {
                        input.Add(new ItemCancelamentoEntrada()
                        {
                            CodUserCanc = this.SessaoAtual.LoginUsuario,
                            DtTransf = item.DtTransf,
                            IPCanc = address,//"172.168.4.86", // IP HARDCODED DEVE SER DINAMICO
                            NSU = item.NSU,
                            NumEstabelecimento = item.NumEstabelecimento,
                            NumPDVCanc = item.NumPDVCanc,
                            FormaVenda = item.FormaVenda,
                            VlCanc = item.VlCanc,
                            VlTrans = item.VlTrans,
                            NumCartao = string.Empty,//item.NSU,
                            TpVenda = item.TpVenda,
                            NumeroAutorizacao = item.NumeroAutorizacao,
                            DataHoraTransacao = item.DataHoraTransacao
                        });
                    }

                    List<ItemCancelamentoSaida> resultadoSaida = new List<ItemCancelamentoSaida>();

                    if (input.Count > 0)
                    {
                        foreach (ItemCancelamentoEntrada entrada in input)
                        {
                            entrada.DtTransfInt = int.Parse(entrada.DtTransf.ToString("ddMMyyyy"));
                            SharePointUlsLog.LogMensagem("Cancelamento - Data Transacao: " + entrada.DtTransfInt.ToString());
                            SharePointUlsLog.LogMensagem("Cancelamento - Valor Transacao: " + entrada.VlTrans.ToString());
                            SharePointUlsLog.LogMensagem("Cancelamento - Valor Cancelamento: " + entrada.VlCanc.ToString());
                            Log.GravarMensagem("Cancelamento", new { entrada });

                            if (entrada.VlCanc.ToString().IndexOf(".") > 0) entrada.VlCancStr = entrada.VlCanc.ToString().Remove(entrada.VlCanc.ToString().IndexOf("."), 1);
                            if (entrada.VlTrans.ToString().IndexOf(".") > 0) entrada.VlTransStr = entrada.VlTrans.ToString().Remove(entrada.VlTrans.ToString().IndexOf("."), 1);

                            if (entrada.VlCanc.ToString().IndexOf(",") > 0) entrada.VlCancStr = entrada.VlCanc.ToString().Remove(entrada.VlCanc.ToString().IndexOf(","), 1);
                            if (entrada.VlTrans.ToString().IndexOf(",") > 0) entrada.VlTransStr = entrada.VlTrans.ToString().Remove(entrada.VlTrans.ToString().IndexOf(","), 1);

                            if (string.IsNullOrEmpty(entrada.VlCancStr)) entrada.VlCancStr = entrada.VlCanc.ToString();
                            if (string.IsNullOrEmpty(entrada.VlTransStr)) entrada.VlTransStr = entrada.VlTrans.ToString();

                            SharePointUlsLog.LogMensagem("Cancelamento - Valor Transacao: " + entrada.VlTransStr);
                            SharePointUlsLog.LogMensagem("Cancelamento - Valor Cancelamento: " + entrada.VlCancStr);
                            SharePointUlsLog.LogMensagem("Cancelamento - NumEstabelecimento: " + entrada.NumEstabelecimento);
                            SharePointUlsLog.LogMensagem("Cancelamento - NumPDVCanc: " + entrada.NumPDVCanc);
                            SharePointUlsLog.LogMensagem("Cancelamento - TpVenda: " + entrada.TpVenda);
                            SharePointUlsLog.LogMensagem("Cancelamento - CodUserCanc: " + entrada.CodUserCanc);
                            SharePointUlsLog.LogMensagem("Cancelamento - NSU: " + entrada.NSU);
                            Log.GravarMensagem("Cancelamento", new { entrada });

                            // INSERIR REGRA DO PV

                        }

                        // SE O PV FOR MISMACH FAZER ESSA CHAMADA
                        // E MOSTRAR ERRO NA TELA
                        resultadoSaida = client.Cancelamento(input);

                    }

                    //Cancela os duplicados
                    if (Session["DuplicadosSaida"] != null)
                    {
                        foreach (ItemCancelamentoEntrada item in LstItemCancelamentoEntrada.Where(x => !string.IsNullOrEmpty(x.NumeroAutorizacao)))
                        {
                            input.Add(new ItemCancelamentoEntrada()
                            {
                                CodUserCanc = this.SessaoAtual.LoginUsuario,
                                DtTransf = item.DtTransf,
                                IPCanc = address,//"172.168.4.86", // IP HARDCODED DEVE SER DINAMICO
                                NSU = item.NSU,
                                NumEstabelecimento = item.NumEstabelecimento,
                                NumPDVCanc = item.NumPDVCanc,
                                VlCanc = item.VlCanc,
                                VlTrans = item.VlTrans,
                                NumCartao = string.Empty,//item.NSU,
                                TpVenda = item.TpVenda,
                                FormaVenda = item.FormaVenda,
                                NumeroAutorizacao = item.NumeroAutorizacao,
                                DataHoraTransacao = item.DataHoraTransacao
                            });
                        }

                        List<ItemCancelamentoSaida> resultadoDuplicados = client.CancelamentoDuplicadas((List<ModConsultaDuplicado>)Session["DuplicadosSaida"]);

                        if (resultadoSaida != null)
                        {

                            resultadoSaida.AddRange(resultadoDuplicados);

                            Session.Remove("DuplicadosSaida");
                        }
                        else
                        {
                            resultadoSaida = resultadoDuplicados;
                        }
                    }

                    Session["ItensEntrada"] = input;
                    Session["ItensSaida"] = resultadoSaida;

                    try
                    {
                        if (resultadoSaida.Where(x => int.Parse(x.CodRetorno) != 0).Count() == 0)
                        {
                            Response.Redirect("pn_CompCancelamentoVenda.aspx");
                        }
                        else
                        {

                            if (resultadoSaida.Where(x => int.Parse(x.CodRetorno) != 0).Count() == resultadoSaida.Count())
                            {
                                Response.Redirect("pn_CancelamentoVendasNaoRealizado.aspx");
                            }
                            else
                            {
                                Response.Redirect("pn_CompCancelamentoVenda.aspx");
                            }
                        }

                    }
                    catch (Exception ex) { Log.GravarErro(ex); }
                    // this.lblResultadoCancelar.Content = string.Format("{0} - Numero: {1}", resultado[0].MsgErro, resultado[0].NumAvisoCanc);

                }
            }
        }

        /// <summary>
        /// Método para chamar excluir a linha da lista de cancelamento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgConfirmacao_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação Cancelamento - Confirmação"))
            {
                ImageButton btnImage = (ImageButton)sender;

                for (int i = 0; i < rptDados.Items.Count; i++)
                {

                    ImageButton btnImagem = (ImageButton)(rptDados.Items[i].FindControl("imgConfirmacao"));

                    if (btnImage.UniqueID == btnImagem.UniqueID)
                    {
                        ListaInicial = (List<ItemCancelamentoEntrada>)Session["ItensEntrada"];
                        ListaInicial.RemoveAt(i);

                        if (ListaInicial.Count == 0)
                        {
                            try
                            {
                                Response.Redirect("pn_cancelamentovendas.aspx");
                            }
                            catch (Exception ex)
                            {
                                Log.GravarErro(ex);
                            }

                        }

                        rptDados.DataSource = null;
                        rptDados.DataSource = ListaInicial;
                        rptDados.DataBind();
                    }
                }
            }
        }

        /// <summary>
        /// Método para chamar excluir a linha da lista de cancelamento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgConfirmacaoDuplicadas_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação Cancelamento - Confirmação Duplicadas"))
            {
                try
                {
                    List<ModConsultaDuplicado> ObjDuplicados = new List<ModConsultaDuplicado>();
                    List<ItemCancelamentoEntrada> lstEntrada = new List<ItemCancelamentoEntrada>();

                    if (Session["DuplicadosSaida"] != null)
                    {
                        ObjDuplicados = (List<ModConsultaDuplicado>)Session["DuplicadosSaida"];
                    }

                    if (Session["ItensEntrada"] != null)
                    {
                        lstEntrada = (List<ItemCancelamentoEntrada>)Session["ItensEntrada"];
                    }

                    ImageButton btnImage = (ImageButton)sender;

                    for (int i = 0; i < rptCancelaDuplicados.Items.Count; i++)
                    {

                        ImageButton btnImagem = (ImageButton)(rptCancelaDuplicados.Items[i].FindControl("imgConfirmacao"));

                        if (btnImage.UniqueID == btnImagem.UniqueID)
                        {
                            ObjDuplicados.RemoveAt(i);
                            lstEntrada.RemoveAt(i);

                            if (ObjDuplicados.Count == 0 && lstEntrada.Count == 0)
                            {
                                try
                                {
                                    Response.Redirect("pn_cancelamentovendas.aspx");
                                }
                                catch (Exception ex)
                                {
                                    Log.GravarErro(ex);
                                }
                            }

                            Session["ItensEntrada"] = lstEntrada;
                            Session["DuplicadosSaida"] = ObjDuplicados;

                            rptCancelaDuplicados.DataSource = null;
                            rptCancelaDuplicados.DataSource = lstEntrada;
                            rptCancelaDuplicados.DataBind();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExibirPainelExcecao(FONTE, 300);
                    SharePointUlsLog.LogErro(ex);
                    Log.GravarErro(ex);
                }
            }
        }

        /// <summary>
        /// Método executado no clique do Voltar!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Cancelamento_Voltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação Cancelamento - Voltar"))
            {
                Response.Redirect("pn_cancelamentovendas.aspx");
            }
        }
    }
}