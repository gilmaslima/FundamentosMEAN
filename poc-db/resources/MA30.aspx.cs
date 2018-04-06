using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.RAV.Sharepoint.ModuloRAV;
using System.ServiceModel;

namespace Redecard.PN.RAV.Sharepoint.LAYOUTS
{
    public partial class MA30 : ApplicationPageBaseAnonima
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Executar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Executar MA30"))
            {
                try
                {
                    ModuloRAV.MA30 chamadaMA30 = new ModuloRAV.MA30();
                    //ModuloRAV.MA30 retornoMA30 = new ModuloRAV.MA30();

                    chamadaMA30.MA030_AGENCIA = txt_MA030_AGENCIA.Text.ToInt32();
                    chamadaMA30.MA030_BANCO = txt_MA030_BANCO.Text.ToInt16();
                    chamadaMA30.MA030_CA_IND_ANTEC = txt_MA030_CA_IND_ANTEC.Text;
                    chamadaMA30.MA030_CA_IND_DATA_ANTEC = txt_MA030_CA_IND_DATA_ANTEC.Text;
                    chamadaMA30.MA030_CA_IND_PRODUTO = txt_MA030_CA_IND_PRODUTO.Text;
                    chamadaMA30.MA030_CA_PER_DATA_ATE = txt_MA030_CA_PER_DATA_ATE.Text;
                    chamadaMA30.MA030_CA_PER_DATA_DE = txt_MA030_CA_PER_DATA_DE.Text;
                    chamadaMA30.MA030_CA_VAL_ANTEC = txt_MA030_CA_VAL_ANTEC.Text;
                    chamadaMA30.MA030_CANAL = txt_MA030_CANAL.Text.ToInt16();

                    Decimal MA030_CONTA = 0;
                    Decimal.TryParse(txt_MA030_CONTA.Text, out MA030_CONTA);
                    chamadaMA30.MA030_CONTA = MA030_CONTA;

                    chamadaMA30.MA030_DAT_PERIODO_ATE = txt_MA030_DAT_PERIODO_ATE.Text;
                    chamadaMA30.MA030_DAT_PERIODO_DE = txt_MA030_DAT_PERIODO_DE.Text;
                    chamadaMA30.MA030_DAT_PROCESSAMENTO = txt_MA030_DAT_PROCESSAMENTO.Text;
                    chamadaMA30.MA030_DAT_RESTRICAO = txt_MA030_DAT_RESTRICAO.Text;
                    chamadaMA30.MA030_DATA_FIM_CARENCIA = txt_MA030_DATA_FIM_CARENCIA.Text;
                    chamadaMA30.MA030_FUNCAO = txt_MA030_FUNCAO.Text.ToInt16();
                    chamadaMA30.MA030_HOR_PROCESSAMENTO = txt_MA030_HOR_PROCESSAMENTO.Text;
                    chamadaMA30.MA030_HORA_FIM_D0 = txt_MA030_HORA_FIM_D0.Text.ToInt16();
                    chamadaMA30.MA030_HORA_FIM_DN = txt_MA030_HORA_FIM_DN.Text.ToInt16();
                    chamadaMA30.MA030_HORA_INI_D0 = txt_MA030_HORA_INI_D0.Text.ToInt16();
                    chamadaMA30.MA030_HORA_INI_DN = txt_MA030_HORA_INI_DN.Text.ToInt16();
                    chamadaMA30.MA030_MSGERRO = txt_MA030_MSGERRO.Text;
                    chamadaMA30.MA030_NUM_PDV = txt_MA030_NUM_PDV.Text.ToInt32();
                    
                    Decimal MA030_PCT_DESCONTO = 0;
                    Decimal.TryParse(txt_MA030_PCT_DESCONTO.Text, out MA030_PCT_DESCONTO);
                    chamadaMA30.MA030_PCT_DESCONTO = MA030_PCT_DESCONTO;

                    chamadaMA30.MA030_RV_QTD_RV = txt_MA030_RV_QTD_RV.Text.ToInt32();
                    chamadaMA30.MA030_TIP_CREDITO = txt_MA030_TIP_CREDITO.Text.ToInt16();
                    
                    Decimal MA030_VALOR_A_ANTECIPAR = 0;
                    Decimal.TryParse(txt_MA030_VALOR_A_ANTECIPAR.Text, out MA030_VALOR_A_ANTECIPAR);
                    chamadaMA30.MA030_VALOR_A_ANTECIPAR = MA030_VALOR_A_ANTECIPAR;
                    
                    Decimal MA030_VALOR_ANTEC_D0 = 0;
                    Decimal.TryParse(txt_MA030_VALOR_ANTEC_D0.Text, out MA030_VALOR_ANTEC_D0);
                    chamadaMA30.MA030_VALOR_ANTEC_D0 = MA030_VALOR_ANTEC_D0;

                    Decimal MA030_VALOR_ANTEC_D1 = 0;
                    Decimal.TryParse(txt_MA030_VALOR_ANTEC_D1.Text, out MA030_VALOR_ANTEC_D1);
                    chamadaMA30.MA030_VALOR_ANTEC_D1 = MA030_VALOR_ANTEC_D1;

                    Decimal MA030_VALOR_BRUTO = 0;
                    Decimal.TryParse(txt_MA030_VALOR_BRUTO.Text, out MA030_VALOR_BRUTO);
                    chamadaMA30.MA030_VALOR_BRUTO = MA030_VALOR_BRUTO;

                    Decimal MA030_VALOR_DISP_ANTEC = 0;
                    Decimal.TryParse(txt_MA030_VALOR_DISP_ANTEC.Text, out MA030_VALOR_DISP_ANTEC);
                    chamadaMA30.MA030_VALOR_DISP_ANTEC = MA030_VALOR_DISP_ANTEC;

                    Decimal MA030_VALOR_MINIMO = 0;
                    Decimal.TryParse(txt_MA030_VALOR_MINIMO.Text, out MA030_VALOR_MINIMO);
                    chamadaMA30.MA030_VALOR_MINIMO = MA030_VALOR_MINIMO;

                    Decimal MA030_VALOR_ORIG = 0;
                    Decimal.TryParse(txt_MA030_VALOR_ORIG.Text, out MA030_VALOR_ORIG);
                    chamadaMA30.MA030_VALOR_ORIG = MA030_VALOR_ORIG;
                    
                    using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                    {
                        var retornoMA30 = cliente.ExecutarMA30(chamadaMA30);

                        rptRetornoMA30.DataSource = PreecherRetorno(retornoMA30);
                        rptRetornoMA30.DataBind();

                        rptFiller.DataSource = PreecherRetorno(retornoMA30.filler);
                        rptFiller.DataBind();

                        rptFiller1.DataSource = PreecherRetorno(retornoMA30.filler1);
                        rptFiller1.DataBind();
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    throw new Exception("Erro ao excutar", ex);
                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    throw new Exception("Erro ao excutar", ex);
                }
            }
        }

        private List<String> PreecherRetorno(ModuloRAV.MA30 retorno)
        {
            String valor = "";
            String prop = "";
            List<String> listaAtributos = new List<string>();
            foreach (PropertyInfo propertyInfo in retorno.GetType().GetProperties())
            {
                if (!propertyInfo.Name.Equals("ExtensionData") 
                        && !propertyInfo.Name.Equals("filler") 
                        && !propertyInfo.Name.Equals("filler1"))
                {
                    valor = propertyInfo.GetValue(retorno, null).ToString();
                    prop = propertyInfo.Name;
                    listaAtributos.Add(String.Format("{0}#{1}", prop, valor));
                }
            }

            return listaAtributos;
        }

        private List<String> PreecherRetorno(List<FILLER> retorno)
        {
            String valor = "";
            String prop = "";
            List<String> listaAtributos = new List<string>();

            foreach (FILLER filler in retorno)
            {
                foreach (PropertyInfo propertyInfo in filler.GetType().GetProperties())
                {
                    if (!propertyInfo.Name.Equals("ExtensionData")
                            && !propertyInfo.Name.Equals("filler")
                            && !propertyInfo.Name.Equals("filler1"))
                    {
                        valor = propertyInfo.GetValue(filler, null).ToString();
                        prop = propertyInfo.Name;
                        listaAtributos.Add(String.Format("{0}#{1}", prop, valor));
                    }
                }
            }

            return listaAtributos;
        }

        private List<String> PreecherRetorno(List<FILLER1> retorno)
        {
            String valor = "";
            String prop = "";
            List<String> listaAtributos = new List<string>();
            foreach (FILLER1 filler1 in retorno)
            {
                foreach (PropertyInfo propertyInfo in filler1.GetType().GetProperties())
                {
                    if (!propertyInfo.Name.Equals("ExtensionData")
                            && !propertyInfo.Name.Equals("filler")
                            && !propertyInfo.Name.Equals("filler1"))
                    {
                        valor = propertyInfo.GetValue(filler1, null).ToString();
                        prop = propertyInfo.Name;
                        listaAtributos.Add(String.Format("{0}#{1}", prop, valor));
                    }
                }
            }

            return listaAtributos;
        }

        protected void ItemRetornoMA30(Object Sender, RepeaterItemEventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Items RetornoMA30"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        Label lblAtributo = (Label)e.Item.FindControl("lblAtributo");
                        Label lblValor = (Label)e.Item.FindControl("lblValor");
                        String atributo = (String)e.Item.DataItem;

                        lblAtributo.Text = atributo.Split('#')[0];
                        lblValor.Text = atributo.Split('#')[1];
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    throw new Exception("Erro ao excutar", ex);

                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    throw new Exception("Erro ao excutar", ex);
                }
            }
        }
    }
}
