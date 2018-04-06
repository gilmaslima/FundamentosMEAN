using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using System.Collections.Generic;
using System.Linq;
using Redecard.PN.Comum;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.VendaDuplicadaConfirmar
{
    public partial class VendaDuplicadaConfirmarUserControl : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)

                try
                {
                    if ((bool)Session["CancelamentoLote"] == true)
                    {
                        TituloPaginaNaoRealizado.Text = " CANCELAMENTOS DE VENDAS POR LOTE";
                    }
                }
            catch(Exception ex){
                this.ExibirPainelExcecao("VendaDuplicadaConfirmarUserControl", 300);
            }

           
                CarregaTabelaLista();
            
        }

        private void CarregaTabelaLista()
        {
            try
            {

                if (Session["DuplicadosSaida"] != null)
                {
                    List<ModConsultaDuplicado> ObjDuplicados = (List<ModConsultaDuplicado>)Session["DuplicadosSaida"];
                    List<ItemCancelamentoEntrada> lstEntrada = (List<ItemCancelamentoEntrada>)Session["ItensEntrada"];

                    lstEntrada = lstEntrada.Where(x => ObjDuplicados.Where(y => y.NumNSUCartao == x.NSU).FirstOrDefault() != null).ToList();
                    List<ItemCancelamentoEntrada> listaEntrada = new List<ItemCancelamentoEntrada>();

                    foreach (ItemCancelamentoEntrada entrada in lstEntrada)
                    {
                        var list = ObjDuplicados.Where(x => x.NumNSUCartao == entrada.NSU).ToList();
                        foreach (ModConsultaDuplicado x in list)
                        {
                            listaEntrada.Add(new ItemCancelamentoEntrada()
                            {
                                CodUserCanc = entrada.CodUserCanc,
                                DtTransf = entrada.DtTransf,
                                IPCanc = entrada.IPCanc,
                                NSU = entrada.NSU,
                                NumEstabelecimento = entrada.NumEstabelecimento,
                                NumPDVCanc = entrada.NumPDVCanc,
                                VlCanc = entrada.VlCanc,
                                VlTrans = entrada.VlTrans,
                                NumCartao = string.Empty,//item.NSU,
                                TpVenda = entrada.TpVenda,
                                NumeroAutorizacao = x.Cd_aut_bnd,
                                DataHoraTransacao = x.Nu_trx_tcc
                            });
                        }
                    }

                    txtTotal.Text = listaEntrada.Select(x => x.VlCanc).Sum().ToString("0,0.00");

                    rptProtocoloVendas.DataSource = listaEntrada;
                    rptProtocoloVendas.DataBind();
                }
            }
            catch(Exception ex){
            
            }
        }

        protected void BotaoVoltar(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("pn_CancelamentoVendas.aspx");
            }
            catch (Exception ex)
            {
                this.ExibirPainelExcecao("VendaDuplicadaConfirmarUserControl", 300);
            }
        }

        protected void BotaoCancelar(object sender, EventArgs e)
        {
            int contador = 0;

            try
            {
                List<ModConsultaDuplicado> ObjDuplicados = (List<ModConsultaDuplicado>)Session["DuplicadosSaida"];
                List<ModConsultaDuplicado> listDuplicadosSaida = new List<ModConsultaDuplicado>();

                List<ModComprovante> listAnular = (List<ModComprovante>)Session["ItensAnulamento"];

                foreach (RepeaterItem itemrptDados in rptProtocoloVendas.Items)
                {
                    Label NumeroAutorizacao = (Label)(rptProtocoloVendas.Items[contador].FindControl("NumeroAutorizacao"));
                    Label NumNSU = (Label)(rptProtocoloVendas.Items[contador].FindControl("NumNSU"));
                    Label DataVenda = (Label)(rptProtocoloVendas.Items[contador].FindControl("DataVenda"));
                    Label ValorVenda = (Label)(rptProtocoloVendas.Items[contador].FindControl("ValorVenda"));
                    Label ValorCancelamento = (Label)(rptProtocoloVendas.Items[contador].FindControl("ValorCancelamento"));
                    Label TipoCancel = (Label)(rptProtocoloVendas.Items[contador].FindControl("TipoCancel"));
                    Label DataHora = (Label)(rptProtocoloVendas.Items[contador].FindControl("DataHora"));

                    CheckBox CheckExcluir = (CheckBox)(rptProtocoloVendas.Items[contador].FindControl("chkSel"));

                    if (CheckExcluir.Checked) // Apaga registro selecionado
                    {
                        ModConsultaDuplicado ListaCanDuplicados = ObjDuplicados.Where(x => x.NumNSUCartao.CompareTo(NumNSU.Text) == 0 && x.Nu_trx_tcc.CompareTo(DataHora.Text) == 0 && x.Cd_aut_bnd.CompareTo(NumeroAutorizacao.Text) == 0).FirstOrDefault();

                        if (ListaCanDuplicados != null)
                        {
                            listDuplicadosSaida.Add(ListaCanDuplicados);
                        }
                    }
                    contador++;
                }

                if (listDuplicadosSaida.Count > 0)
                {
                    Session["DuplicadosSaida"] = listDuplicadosSaida;

                    try
                    {
                        Response.Redirect("pn_ConfirmacaoCancelamento.aspx");
                    }
                    catch (Exception ex) {
                        this.ExibirPainelExcecao("VendaDuplicadaConfirmarUserControl", 300);
                    }
                }

            }
            catch (Exception ex)
            {
                this.ExibirPainelExcecao("VendaDuplicadaConfirmarUserControl", 300);
            }
        }
    }
}
