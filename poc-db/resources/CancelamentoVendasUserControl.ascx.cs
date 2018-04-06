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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using System.Globalization;
using Microsoft.SharePoint;
using Redecard.PN.Cancelamento.Sharepoint.Modelos;
using System.Linq;
using System.Text.RegularExpressions;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.CancelamentoVendas
{

    public partial class CancelamentoVendasUserControl : UserControlBase
    {
        bool lote;

        #region Constantes
        public const string FONTE = "CancelamentoVendasUserControl.ascx";
        public const int CODIGO_ERRO_LOAD = 3070;
        public const int CODIGO_ERRO_CONFIRMAR = 3070;
        public const string ERRO_NUMESTABELECIMENTO = "Número do estabelecimento inválido.";
        public const string ERRO_NUMNSUCARTAO = "Número do NSU ou do Cartão inválido.";
        public const string ERRO_DATA = "Data inválida ou formatação não reconhecida.";
        public const string ERRO_VALORVENDA = "Valor da venda inválido.";
        public const string ERRO_TIPOCANCELAMENTO = "Tipo do Cancelamento inválido.";
        public const string ERRO_VALORCANCELADO = "Valor do cancelamento inválido.";
        public const string ERRO_TIPOVENDA = "Tipo da venda inválido.";
        #endregion

        #region Metodos Tela
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(base.SessaoAtual, null))
            {
                using (Logger Log = Logger.IniciarLog("Cancelamento Vendas - Page Load"))
                {
                    if (Session["ItensEntrada"] != null)
                        Session.Remove("ItensEntrada");

                    if (Session["CancelamentoLote"] != null)
                        Session.Remove("CancelamentoLote");

                    if (Session["ValidaArquivo"] != null)
                        Session.Remove("ValidaArquivo");

                    if (Session["DuplicadosSaida"] != null)
                        Session.Remove("DuplicadosSaida");

                    lblValidacaoArquivo.Text = "";
                    lote = false;

                    this.lblValidacao.Visible = false;
                    this.lblValidacaoArquivo.Visible = false;
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
                        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "$('.ocultar').hide();", true);
                    }
                }
            }
        }

        protected void Cancelamento_Voltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento Vendas - Voltar"))
            {
                Response.Redirect("pn_cancelamento.aspx");
            }
        }
        #endregion

        #region Cancelamento Vendas Digitação
        /// <summary>
        /// Método executado no clique do botão cancelar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btCancel_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento Vendas - Cancelamento Vendas Digitação"))
            {
                lote = false;
                Session["CancelamentoLote"] = lote;

                bool proximatela = ValidaCampos();

                List<ItemCancelamentoEntrada> items = new List<ItemCancelamentoEntrada>();

                //Cancela primeira linha
                if (tipo1.Text == "Total")
                {
                    if (!string.IsNullOrEmpty(nsu1.Text) && !string.IsNullOrEmpty(valor1.Text))
                    {

                        ItemCancelamentoEntrada PrimeiraLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty(numEst1.Text))
                        {
                            PrimeiraLinha.NumEstabelecimento = Int32.Parse(numEst1.Text);
                        }
                        else
                        {
                            PrimeiraLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        PrimeiraLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        PrimeiraLinha.NumCartao = nsu1.Text;
                        PrimeiraLinha.NSU = nsu1.Text;
                        PrimeiraLinha.DtTransf = data1.Text.ToDate();
                        PrimeiraLinha.VlTrans = decimal.Parse(valor1.Text);
                        PrimeiraLinha.FormaVenda = tipo1.Text;
                        PrimeiraLinha.TpVenda = drop_tivpo_venda_1.SelectedValue;
                        PrimeiraLinha.VlCanc = decimal.Parse(valor1.Text); // o valor de cancelamento é o mesmo do total
                        items.Add(PrimeiraLinha);
                    }

                }
                else
                {

                    if (!string.IsNullOrEmpty(nsu1.Text) && !string.IsNullOrEmpty(valor1.Text) && !string.IsNullOrEmpty(valCancel1.Text))
                    {
                        //if (Convert.ToInt32(valCancel1.Text) > Convert.ToInt32(valor1.Text))
                        //{
                        //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Valor de cancelamento maior que valor total, linha 1');", true);
                        //    proximatela = false;
                        //}

                        ItemCancelamentoEntrada PrimeiraLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty(numEst1.Text))
                        {
                            PrimeiraLinha.NumEstabelecimento = Int32.Parse(numEst1.Text);
                        }
                        else
                        {
                            PrimeiraLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        PrimeiraLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        PrimeiraLinha.NumCartao = nsu1.Text;
                        PrimeiraLinha.NSU = nsu1.Text;
                        PrimeiraLinha.DtTransf = data1.Text.ToDate();
                        PrimeiraLinha.VlTrans = decimal.Parse(valor1.Text);
                        PrimeiraLinha.FormaVenda = tipo1.Text;
                        PrimeiraLinha.TpVenda = drop_tivpo_venda_1.SelectedValue;
                        PrimeiraLinha.VlCanc = decimal.Parse(valCancel1.Text);
                        items.Add(PrimeiraLinha);
                    }

                }

                //Cancela a segunda linha
                if (tipo2.Text == "Total")
                {
                    if (!string.IsNullOrEmpty(nsu2.Text) && !string.IsNullOrEmpty(valor2.Text))
                    {
                        //if (Convert.ToInt32(valor1.Text) > Convert.ToInt32(valCancel1.Text))
                        //{
                        //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Valor parcial maior que valor total, linha 1');", true);
                        //    proximatela = false;
                        //}

                        ItemCancelamentoEntrada SegundaLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty(numEst2.Text))
                        {
                            SegundaLinha.NumEstabelecimento = Int32.Parse(numEst2.Text);
                        }
                        else
                        {
                            SegundaLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        SegundaLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        SegundaLinha.NumCartao = nsu2.Text;
                        SegundaLinha.NSU = nsu2.Text;
                        SegundaLinha.DtTransf = data2.Text.ToDate();
                        SegundaLinha.VlTrans = decimal.Parse(valor2.Text);
                        SegundaLinha.FormaVenda = tipo2.Text;
                        SegundaLinha.TpVenda = drop_tivpo_venda2.SelectedValue;
                        SegundaLinha.VlCanc = decimal.Parse(valor2.Text); // o valor de cancelamento é o mesmo do total
                        items.Add(SegundaLinha);
                    }


                }
                else
                {
                    if (!string.IsNullOrEmpty(nsu2.Text) && !string.IsNullOrEmpty(valor2.Text) && !string.IsNullOrEmpty(valCancel2.Text))
                    {

                        //if (Convert.ToInt32(valCancel2.Text) > Convert.ToInt32(valor2.Text))
                        //{
                        //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Valor de cancelamento maior que valor total, linha 2');", true);
                        //    proximatela = false;
                        //}

                        ItemCancelamentoEntrada SegundaLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty(numEst2.Text))
                        {
                            SegundaLinha.NumEstabelecimento = Int32.Parse(numEst2.Text);
                        }
                        else
                        {
                            SegundaLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        SegundaLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        SegundaLinha.NumCartao = nsu2.Text;
                        SegundaLinha.NSU = nsu2.Text;
                        SegundaLinha.DtTransf = data2.Text.ToDate();
                        SegundaLinha.VlTrans = decimal.Parse(valor2.Text);
                        SegundaLinha.FormaVenda = tipo2.Text;
                        SegundaLinha.TpVenda = drop_tivpo_venda2.SelectedValue;
                        SegundaLinha.VlCanc = decimal.Parse(valCancel2.Text);
                        items.Add(SegundaLinha);
                    }

                }

                //Cancela a terceira linha
                if (tipo3.Text == "Total")
                {
                    if (!string.IsNullOrEmpty(nsu3.Text) && !string.IsNullOrEmpty(valor3.Text))
                    {

                        ItemCancelamentoEntrada TerceiraLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty(numEst3.Text))
                        {
                            TerceiraLinha.NumEstabelecimento = Int32.Parse(numEst3.Text);
                        }
                        else
                        {
                            TerceiraLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        TerceiraLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        TerceiraLinha.NumCartao = nsu3.Text;
                        TerceiraLinha.NSU = nsu3.Text;
                        TerceiraLinha.DtTransf = data3.Text.ToDate();
                        TerceiraLinha.VlTrans = decimal.Parse(valor3.Text);
                        TerceiraLinha.FormaVenda = tipo3.Text;
                        TerceiraLinha.TpVenda = DropDownList3.SelectedValue;
                        TerceiraLinha.VlCanc = decimal.Parse(valor3.Text); // o valor de cancelamento é o mesmo do total
                        items.Add(TerceiraLinha);
                    }


                }
                else
                {
                    if (!string.IsNullOrEmpty(nsu3.Text) && !string.IsNullOrEmpty(valor3.Text) && !string.IsNullOrEmpty(valCancel3.Text))
                    {
                        //if (Convert.ToInt32(valCancel3.Text) > Convert.ToInt32(valor3.Text))
                        //{
                        //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Valor de cancelamento maior que valor total, linha 3');", true);
                        //    proximatela = false;
                        //}

                        ItemCancelamentoEntrada TerceiraLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty(numEst3.Text))
                        {
                            TerceiraLinha.NumEstabelecimento = Int32.Parse(numEst3.Text);
                        }
                        else
                        {
                            TerceiraLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        TerceiraLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        TerceiraLinha.NumCartao = nsu3.Text;
                        TerceiraLinha.NSU = nsu3.Text;
                        TerceiraLinha.DtTransf = data3.Text.ToDate();
                        TerceiraLinha.VlTrans = decimal.Parse(valor3.Text);
                        TerceiraLinha.FormaVenda = tipo3.Text;
                        TerceiraLinha.TpVenda = DropDownList3.SelectedValue;
                        TerceiraLinha.VlCanc = decimal.Parse(valCancel3.Text);
                        items.Add(TerceiraLinha);
                    }

                }

                //Cancela a Quarta linha
                if (tipo4.Text == "Total")
                {
                    if (!string.IsNullOrEmpty(nsu4.Text) && !string.IsNullOrEmpty(valor4.Text))
                    {

                        ItemCancelamentoEntrada QuartaLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty(numEst4.Text))
                        {
                            QuartaLinha.NumEstabelecimento = Int32.Parse(numEst4.Text);
                        }
                        else
                        {
                            QuartaLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        QuartaLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        QuartaLinha.NumCartao = nsu4.Text;
                        QuartaLinha.NSU = nsu4.Text;
                        QuartaLinha.DtTransf = data4.Text.ToDate();
                        QuartaLinha.VlTrans = decimal.Parse(valor4.Text);
                        QuartaLinha.FormaVenda = tipo4.Text;
                        QuartaLinha.TpVenda = DropDownList4.SelectedValue;
                        QuartaLinha.VlCanc = decimal.Parse(valor4.Text); // o valor de cancelamento é o mesmo do total
                        items.Add(QuartaLinha);
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(nsu4.Text) && !string.IsNullOrEmpty(valor4.Text) && !string.IsNullOrEmpty(valCancel4.Text))
                    {
                        //if (Convert.ToInt32(valCancel4.Text) > Convert.ToInt32(valor4.Text))
                        //{
                        //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Valor de cancelamento maior que valor total, linha 4');", true);
                        //    proximatela = false;
                        //}

                        ItemCancelamentoEntrada QuartaLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty(numEst4.Text))
                        {
                            QuartaLinha.NumEstabelecimento = Int32.Parse(numEst4.Text);
                        }
                        else
                        {
                            QuartaLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        QuartaLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        QuartaLinha.NumCartao = nsu4.Text;
                        QuartaLinha.NSU = nsu4.Text;
                        QuartaLinha.DtTransf = data4.Text.ToDate();
                        QuartaLinha.VlTrans = decimal.Parse(valor4.Text);
                        QuartaLinha.FormaVenda = tipo4.Text;
                        QuartaLinha.TpVenda = DropDownList4.SelectedValue;
                        QuartaLinha.VlCanc = decimal.Parse(valCancel4.Text);
                        items.Add(QuartaLinha);
                    }

                }

                //Cancela a Quinta linha
                if (tipo5.Text == "Total")
                {
                    if (!string.IsNullOrEmpty(nsu5.Text) && !string.IsNullOrEmpty(valor5.Text))
                    {

                        ItemCancelamentoEntrada QuintaLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty(numEst5.Text))
                        {
                            QuintaLinha.NumEstabelecimento = Int32.Parse(numEst5.Text);
                        }
                        else
                        {
                            QuintaLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        QuintaLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        QuintaLinha.NumCartao = nsu5.Text;
                        QuintaLinha.NSU = nsu5.Text;
                        QuintaLinha.DtTransf = data5.Text.ToDate();
                        QuintaLinha.VlTrans = decimal.Parse(valor5.Text);
                        QuintaLinha.FormaVenda = tipo5.Text;
                        QuintaLinha.TpVenda = DropDownList5.SelectedValue;
                        QuintaLinha.VlCanc = decimal.Parse(valor5.Text); // o valor de cancelamento é o mesmo do total
                        items.Add(QuintaLinha);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(nsu5.Text) && !string.IsNullOrEmpty(valor5.Text) && !string.IsNullOrEmpty(valCancel5.Text))
                    {
                        //if (Convert.ToInt32(valCancel5.Text) > Convert.ToInt32(valor5.Text))
                        //{
                        //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Valor de cancelamento maior que valor total, linha 5');", true);
                        //    proximatela = false;
                        //}

                        ItemCancelamentoEntrada QuintaLinha = new ItemCancelamentoEntrada();
                        if (!string.IsNullOrEmpty(numEst5.Text))
                        {
                            QuintaLinha.NumEstabelecimento = Int32.Parse(numEst5.Text);
                        }
                        else
                        {
                            QuintaLinha.NumEstabelecimento = SessaoAtual.CodigoEntidade;
                        }

                        QuintaLinha.NumPDVCanc = SessaoAtual.CodigoEntidade;
                        QuintaLinha.NumCartao = nsu5.Text;
                        QuintaLinha.NSU = nsu5.Text;
                        QuintaLinha.DtTransf = data5.Text.ToDate();
                        QuintaLinha.VlTrans = decimal.Parse(valor5.Text);
                        QuintaLinha.FormaVenda = tipo5.Text;
                        QuintaLinha.TpVenda = DropDownList5.SelectedValue;
                        QuintaLinha.VlCanc = decimal.Parse(valCancel5.Text);
                        items.Add(QuintaLinha);
                    }

                }

                //if (string.IsNullOrEmpty(numEst2.Text) || string.IsNullOrEmpty(nsu2.Text) || string.IsNullOrEmpty(data2.Text) || string.IsNullOrEmpty(valor2.Text))
                //{
                //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Erro, linha 2 Cancelamento.');", true);
                //    proximatela = false;
                //}
                //if (string.IsNullOrEmpty(numEst3.Text) || string.IsNullOrEmpty(nsu3.Text) || string.IsNullOrEmpty(data3.Text) || string.IsNullOrEmpty(valor3.Text))
                //{
                //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Erro, linha 3 Cancelamento.');", true);
                //    proximatela = false;
                //}
                //if (string.IsNullOrEmpty(numEst4.Text) || string.IsNullOrEmpty(nsu4.Text) || string.IsNullOrEmpty(data4.Text) || string.IsNullOrEmpty(valor4.Text))
                //{
                //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Erro, linha 4 Cancelamento.');", true);
                //    proximatela = false;
                //}

                //if (string.IsNullOrEmpty(numEst1.Text) || string.IsNullOrEmpty(nsu1.Text) || string.IsNullOrEmpty(data1.Text) || string.IsNullOrEmpty(valor1.Text))
                //{
                //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Erro, linha 5 Cancelamento.');", true);
                //    proximatela = false;
                //}

                if (proximatela)
                {
                    Session["ItensEntrada"] = items;
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
        }
        #endregion

        #region Metodo validação
        /// <summary>
        /// Método para validar os dados de entrada da tela.
        /// </summary>
        /// <returns></returns>
        private bool ValidaCampos()
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento Vendas - Valida Campos"))
            {
                try
                {
                    List<int> ListaPV = new List<int>();
                    List<ServiceFiliais.Filial> ListFiliais = VerificarFilial(this.SessaoAtual.CodigoEntidade);

                    ListaPV.Add(SessaoAtual.CodigoEntidade);

                    if (ListFiliais != null && ListFiliais.Count > 0)
                    {
                        ListaPV.AddRange(ListFiliais.Select(x => x.PontoVenda).ToList());
                    }

                    ListFiliais = null;

                    bool todasLinhasNulas = true;

                    //Valida primeira Linha
                    if (!string.IsNullOrEmpty(numEst1.Text) || !string.IsNullOrEmpty(nsu1.Text))
                    {
                        todasLinhasNulas = false;

                        if (string.IsNullOrEmpty(data1.Text) || data1.Text.ToDateTimeNull() == null)
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Data do primeiro registro inválida";
                            return false;
                        }

                        if (string.IsNullOrEmpty(valor1.Text) || valor1.Text.ToDecimalNull() == null)
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Valor do Cancelamento do primeiro registro inválido";
                            return false;
                        }

                        if (this.tipo1.SelectedIndex == 1 && (string.IsNullOrEmpty(valCancel1.Text) || valCancel1.Text.ToDecimalNull() == null))
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Valor Parcial do Cancelamento do primeiro registro inválido";
                            return false;
                        }


                        if (!string.IsNullOrEmpty(numEst1.Text) && !ListaPV.Contains(Convert.ToInt32(numEst1.Text)))
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Número do estabelecimento do primeiro registro inválido.";
                            return false;
                        }
                    }

                    //Valida segunda Linha
                    if (!string.IsNullOrEmpty(numEst2.Text) || !string.IsNullOrEmpty(nsu2.Text))
                    {
                        todasLinhasNulas = false;

                        if (string.IsNullOrEmpty(data2.Text) || data2.Text.ToDateTimeNull() == null)
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Data do segundo registro inválida";
                            return false;
                        }

                        if (string.IsNullOrEmpty(valor2.Text) || valor2.Text.ToDecimalNull() == null)
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Valor do Cancelamento do segundo registro inválido";
                            return false;
                        }

                        if (this.tipo2.SelectedIndex == 1 && (string.IsNullOrEmpty(valCancel2.Text) || valCancel2.Text.ToDecimalNull() == null))
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Valor Parcial do Cancelamento do segundo registro inválido";
                            return false;
                        }
                        if (!string.IsNullOrEmpty(numEst2.Text) && !ListaPV.Contains(Convert.ToInt32(numEst2.Text)))
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Número do estabelecimento do segundo registro inválido..";
                            return false;
                        }

                    }

                    //Valida terceira Linha
                    if (!string.IsNullOrEmpty(numEst3.Text) || !string.IsNullOrEmpty(nsu3.Text))
                    {
                        todasLinhasNulas = false;

                        if (string.IsNullOrEmpty(data3.Text) || data3.Text.ToDateTimeNull() == null)
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Data do terceiro registro inválida";
                            return false;
                        }

                        if (string.IsNullOrEmpty(valor3.Text) || valor3.Text.ToDecimalNull() == null)
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Valor do Cancelamento do terceiro registro inválido";
                            return false;
                        }

                        if (this.tipo3.SelectedIndex == 1 && (string.IsNullOrEmpty(valCancel3.Text) || valCancel3.Text.ToDecimalNull() == null))
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Valor Parcial do Cancelamento do terceiro registro inválido";
                            return false;
                        }
                        if (!string.IsNullOrEmpty(numEst3.Text) && !ListaPV.Contains(Convert.ToInt32(numEst3.Text)))
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Número do estabelecimento do terceiro registro inválido.";
                            return false;
                        }
                    }

                    //Valida quarta Linha
                    if (!string.IsNullOrEmpty(numEst4.Text) || !string.IsNullOrEmpty(nsu4.Text))
                    {
                        todasLinhasNulas = false;

                        if (string.IsNullOrEmpty(data4.Text) || data4.Text.ToDateTimeNull() == null)
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Data do quarto registro inválida";
                            return false;
                        }

                        if (string.IsNullOrEmpty(valor4.Text) || valor4.Text.ToDecimalNull() == null)
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Valor do Cancelamento do quarto registro inválido";
                            return false;
                        }

                        if (this.tipo4.SelectedIndex == 1 && (string.IsNullOrEmpty(valCancel4.Text) || valCancel4.Text.ToDecimalNull() == null))
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Valor Parcial do Cancelamento do quarto registro inválido";
                            return false;
                        }
                        if (!string.IsNullOrEmpty(numEst4.Text) && !ListaPV.Contains(Convert.ToInt32(numEst4.Text)))
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Número do estabelecimento do quarto registro inválido.";
                            return false;
                        }
                    }

                    //Valida quinta Linha
                    if (!string.IsNullOrEmpty(numEst5.Text) || !string.IsNullOrEmpty(nsu5.Text))
                    {
                        todasLinhasNulas = false;

                        if (string.IsNullOrEmpty(data5.Text) || data5.Text.ToDateTimeNull() == null)
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Data do quinto registro inválida";
                            return false;
                        }

                        if (string.IsNullOrEmpty(valor5.Text) || valor5.Text.ToDecimalNull() == null)
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Valor do Cancelamento do quinto registro inválido";
                            return false;
                        }

                        if (this.tipo5.SelectedIndex == 1 && (string.IsNullOrEmpty(valCancel5.Text) || valCancel5.Text.ToDecimalNull() == null))
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Valor Parcial do Cancelamento do quinto registro inválido";
                            return false;
                        }
                        if (!string.IsNullOrEmpty(numEst5.Text) && !ListaPV.Contains(Convert.ToInt32(numEst5.Text)))
                        {
                            this.lblValidacao.Visible = true;
                            this.lblValidacao.Text = "Número do estabelecimento do quinto registro inválido.";
                            return false;
                        }
                    }

                    if (todasLinhasNulas)
                    {
                        this.lblValidacao.Visible = true;
                        this.lblValidacao.Text = "Necessário o Preenchimento do Nº Cartão ou NSU. ";
                        return false;
                    }

                }
                catch (PortalRedecardException ex)
                {
                    this.ExibirPainelExcecao(ex);
                    SharePointUlsLog.LogErro(ex);
                    Log.GravarErro(ex);
                }

                catch (Exception ex)
                {
                    this.ExibirPainelExcecao("CancelamentoVendasUserControl.ascx", 300);
                    SharePointUlsLog.LogErro(ex);
                    Log.GravarErro(ex);
                }
            }
            return true;
        }
        #endregion

        #region Cancelamento de Venda por Lote
        /// <summary>
        /// Processa o arquivo escolhido e cancela o lote.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btEnviar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento Vendas - Enviar"))
            {
                //  Limita entrada de arquivo para extensão 'TXT'
                string[] validExt = { "txt" };
                bool arquivo_valido = true;
                bool arquivo_entrada_valido = true;

                try
                {
                    if (!UploadArquivo.HasFile)
                    {
                        //this.lblValidacaoArquivo.Visible = true;
                        //this.lblValidacaoArquivo.Text = "Informe um arquivo formato TXT para upload.";
                        //arquivo_entrada_valido = false;
                    }
                    else
                    {

                        //if (UploadArquivo.PostedFile.ContentLength < 1024)
                        //{
                        //    this.lblValidacaoArquivo.Visible = true;
                        //    this.lblValidacaoArquivo.Text = "Arquivo informado está em branco.";
                        //    arquivo_entrada_valido = false;
                        //}


                        SharePointUlsLog.LogMensagem("CANCELAMENTO LOTE: " + UploadArquivo.PostedFile.FileName);
                        Log.GravarMensagem("CANCELAMENTO LOTE: " + UploadArquivo.PostedFile.FileName);

                        if (!string.IsNullOrEmpty(UploadArquivo.PostedFile.FileName))
                        {
                            string fileName = UploadArquivo.PostedFile.FileName;

                            string fileExt = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                            for (int i = 0; i < validExt.Length; i++)
                            {
                                if (fileExt.ToLower() != validExt[i])
                                {
                                    // ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Extensão do arquivo não suportada.');", true);
                                    this.lblValidacaoArquivo.Visible = true;
                                    this.lblValidacaoArquivo.Text = "Extensão do arquivo não suportada.";
                                    arquivo_entrada_valido = false;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex.Message);
                    Log.GravarErro(ex);
                    ExibirPainelExcecao(ex.Message, 500);
                    //   arquivo_valido = false;
                }

                try
                {
                    List<string[]> sells = SplitFile();
                    arquivo_valido = ValidarCampos(sells);

                    // Processar o Lote apenas se o formato do arquivo for TXT.
                    if (arquivo_valido)
                    {
                        List<ItemCancelamentoEntrada> items = new List<ItemCancelamentoEntrada>();

                        HttpPostedFile userPostedFile = UploadArquivo.PostedFile;

                        foreach (string[] s in sells)
                        {
                            ItemCancelamentoEntrada item = new ItemCancelamentoEntrada();
                            item.NumEstabelecimento = s[0].ToInt32();
                            item.NumPDVCanc = item.NumEstabelecimento;

                            item.NSU = s[1];
                            item.NumCartao = s[1];

                            item.DtTransf = DateTime.ParseExact(s[2], "ddMMyyyy", null);

                            item.VlTrans = s[3].ToDecimal();

                            if (s[4] == "T")
                            {
                                item.FormaVenda = "Total";
                            }
                            else if (s[4] == "P")
                            {
                                item.FormaVenda = "Parcial";
                            }

                            item.VlCanc = Convert.ToDecimal(s[5]);
                            item.TpVenda = s[6];

                            items.Add(item);
                        }

                        lote = true;
                        Session["ItensEntrada"] = items;
                        Session["CancelamentoLote"] = lote;


                        if (items.Count == 0)
                        {
                            this.lblValidacaoArquivo.Visible = true;
                            this.lblValidacaoArquivo.Text = "Informe um arquivo formato TXT para upload.";
                            arquivo_entrada_valido = false;
                        }

                        if (arquivo_entrada_valido == true)
                        {

                            try
                            {
                                Response.Redirect("pn_ConfirmacaoCancelamento.aspx");
                            }
                            catch (Exception ex)
                            {
                                SharePointUlsLog.LogErro(ex.Message);
                                Log.GravarErro(ex);
                            }
                        }
                    }
                    else
                    {
                        if (arquivo_entrada_valido == true)
                        {
                            try
                            {
                                Response.Redirect("pn_ErroArquivo.aspx");
                            }
                            catch (Exception ex)
                            {
                                SharePointUlsLog.LogErro(ex.Message);
                                Log.GravarErro(ex);
                                arquivo_valido = false;
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex.Message);
                    Log.GravarErro(ex);
                    ExibirPainelExcecao(ex.Message, 500);
                    arquivo_valido = false;
                }
            }
        }

        private bool ValidarCampos(List<string[]> sells)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento Vendas - Validar Campos"))
            {
                bool retorno = true;
                List<ErroLote> retornoErros = new List<ErroLote>();

                List<int> ListaPV = new List<int>();

                try
                {
                    List<ServiceFiliais.Filial> ListFiliais = VerificarFilial(this.SessaoAtual.CodigoEntidade);

                    ListaPV.Add(SessaoAtual.CodigoEntidade);

                    if (ListFiliais != null && ListFiliais.Count > 0)
                    {
                        ListaPV.AddRange(ListFiliais.Select(x => x.PontoVenda).ToList());
                    }

                    ListFiliais = null;

                }
                catch (PortalRedecardException ex)
                {
                    this.ExibirPainelExcecao(ex);
                    SharePointUlsLog.LogErro(ex);
                    Log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    this.ExibirPainelExcecao("CancelamentoVendasUserControl.ascx", 300);
                    SharePointUlsLog.LogErro(ex);
                    Log.GravarErro(ex);
                }

                try
                {
                    SharePointUlsLog.LogMensagem("CANCELAMENTO LOTE: Validar os registros do arquivo");
                    Log.GravarMensagem("CANCELAMENTO LOTE: Validar os registros do arquivo");

                    if (sells != null)
                    {
                        if (sells.Count > 50)
                        {
                            retornoErros.Add(new ErroLote(0, "Número máximo de cancelamentos por arquivo é de 50"));
                            retorno = false;
                        }
                        else
                        {
                            int line = 1;
                            //Regex regex = new Regex(@"(^[0-9]\.?[0-9]{3}\,?[0-9]{2}$)|(^[0-9]+\.[0-9]{3}$)|(^[0-9]*$)|(^[0-9]*\,[0-9]{1}$)|(^[0-9]*\,[0-9]{2}$)");
                            Regex regex = new Regex(@"^[0-9].*,[0-9]{2}$");

                            double value = 0;
                            CultureInfo ptBR = CultureInfo.GetCultureInfo(1046);

                            foreach (string[] s in sells)
                            {
                                if (!string.IsNullOrEmpty(s[0].Trim()) || s[0].ToInt32Null() == null)
                                {

                                    if (s[0].ToInt32Null() == null || !ListaPV.Contains(s[0].ToInt32())) retornoErros.Add(new ErroLote(line, ERRO_NUMESTABELECIMENTO));

                                    if (s[0].ToInt32Null() == null) retornoErros.Add(new ErroLote(line, ERRO_NUMESTABELECIMENTO));

                                    if (s[1].Length > 19) retornoErros.Add(new ErroLote(line, ERRO_NUMNSUCARTAO));

                                    if (s[1].ToDecimalNull() == null) retornoErros.Add(new ErroLote(line, ERRO_NUMNSUCARTAO));

                                    DateTime data;
                                    DateTime.TryParseExact(s[2], "ddMMyyyy", null, DateTimeStyles.None, out data);

                                    if (data == null || data.CompareTo(DateTime.MinValue) == 0) retornoErros.Add(new ErroLote(line, ERRO_DATA));

                                    //if (s[3].Contains(".") || s[3].ToDecimalNull() == null) retornoErros.Add(new ErroLote(line, ERRO_VALORVENDA));

                                    if (s[4] != "T" && s[4] != "P") retornoErros.Add(new ErroLote(line, ERRO_TIPOCANCELAMENTO));

                                    //if (s[5].Contains(".") || s[5].ToDecimalNull() == null) retornoErros.Add(new ErroLote(line, ERRO_VALORCANCELADO));

                                    if (s[6] != "RO" && s[6] != "PC" && s[6] != "PS") retornoErros.Add(new ErroLote(line, ERRO_TIPOVENDA));


                                    if (!(regex.IsMatch(s[3]) && Double.TryParse(s[3], NumberStyles.Any, ptBR, out value)))
                                        retornoErros.Add(new ErroLote(line, ERRO_VALORVENDA));

                                    if (!(regex.IsMatch(s[5]) && Double.TryParse(s[5], NumberStyles.Any, ptBR, out value)))
                                        retornoErros.Add(new ErroLote(line, ERRO_VALORCANCELADO));

                                    line++;
                                }
                            }

                            if (retornoErros.Count > 0) retorno = false;
                        }
                    }
                    else
                    {
                        retorno = false;
                    }
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex.Message);
                    Log.GravarErro(ex);
                    retorno = false;
                }

                if (!retorno && retornoErros.Count > 0)
                {
                    Session["ValidaArquivo"] = retornoErros;
                }
                return retorno;
            }
        }

        /// <summary>
        /// Método para cortar o arquivo da maneira correta
        /// </summary>
        /// <returns></returns>
        private List<string[]> SplitFile()
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento de Vendas - SplitFile"))
            {
                List<string[]> sells = new List<string[]>();

                SharePointUlsLog.LogMensagem("CANCELAMENTO LOTE: Lendo Arquivo");
                Log.GravarMensagem("CANCELAMENTO LOTE: Lendo Arquivo");

                try
                {
                    StreamReader reader = new StreamReader(UploadArquivo.FileContent);

                    string line = "";

                    while (reader.Peek() != -1)
                    {
                        line = reader.ReadLine();
                        SharePointUlsLog.LogMensagem("CANCELAMENTO LOTE: " + string.Format("Linha - {0}", line));
                        Log.GravarMensagem("CANCELAMENTO LOTE: " + string.Format("Linha - {0}", line));

                        string[] aux = new string[7];
                        aux = line.Trim().Split(';');
                        sells.Add(aux);
                    }
                    reader.Close();

                    return sells;
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex.Message);
                    Log.GravarErro(ex);
                    return null;
                }

                return sells;
            }
        }
        #endregion

        private List<ServiceFiliais.Filial> VerificarFilial(int codigoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento de Vendas - Verificar Filial"))
            {
                int codigoRetorno = 0;

                List<ServiceFiliais.Filial> ListFiliais = null;

                using (ServiceFiliais.EntidadeServicoClient client = new ServiceFiliais.EntidadeServicoClient())
                {
                    try
                    {
                        ListFiliais = client.ConsultarFiliais(out codigoRetorno, codigoEntidade, 2);
                    }
                    catch (Exception ex)
                    {
                        SharePointUlsLog.LogErro(ex.Message);
                        Log.GravarErro(ex);
                    }
                }

                return ListFiliais;
            }
        }
    }
}

