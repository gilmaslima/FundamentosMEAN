using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.DataCash.Modelo;
using System.Web.UI.HtmlControls;
using Redecard.PN.Comum;
using System.Globalization;

namespace Redecard.PN.DataCash.controles
{
    public partial class ControleAnalisedeRisco : System.Web.UI.UserControl
    {

        public List<Produto> ListaProdutos
        {
            get
            {
                if (object.ReferenceEquals(ViewState["ListaProdutos"], null))
                    ViewState["ListaProdutos"] = new List<Produto>();

                return (ViewState["ListaProdutos"] as List<Produto>);
            }
            set { ViewState["ListaProdutos"] = value; }
        }
        public List<DadosPassageiro> ListaPassageiros
        {
            get
            {
                if (object.ReferenceEquals(ViewState["ListaPassageiros"], null))
                    ViewState["ListaPassageiros"] = new List<DadosPassageiro>();

                return (ViewState["ListaPassageiros"] as List<DadosPassageiro>);
            }
            set { ViewState["ListaPassageiros"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarDropDown();
            }
        }

        protected void btnAdicionarPassageiro_Click(object sender, EventArgs e)
        {
            Page.Validate("vlgDadosIATA");
            if (Page.IsValid)
            {
                DadosPassageiro item = new DadosPassageiro();

                //Dados dos Passageiros
                item.TipoDocumento = ddlTipoDocumento.SelectedValue;
                item.TipoDocumentoDescricao = ddlTipoDocumento.SelectedItem.Text;
                item.NumeroDocumento = txtNumeroDocumento.Text;
                item.TipoPassageiro = ddlTipoPassageiro.SelectedValue;
                item.TipoPassageiroDescricao = ddlTipoPassageiro.SelectedItem.Text;
                item.TipoProgramaFidelidade = ddlTipoProgramaFidelidade.SelectedValue;
                item.TipoProgramaFidelidadeDescricao = ddlTipoProgramaFidelidade.SelectedItem.Text;
                item.PaisOrigem = ddlNacionalidade.SelectedValue;
                item.PaisOrigemDescricao = ddlNacionalidade.SelectedItem.Text;
                item.CodigoProgramaFidelidade = txtCodigoProgramaFidelidade.Text;

                ListaPassageiros.Add(item);

                gvPassageiros.DataSource = ListaPassageiros;
                gvPassageiros.DataBind();

                LimparDadosPassageiro();
            }
        }

        void LimparDadosPassageiro()
        {
            ddlTipoDocumento.SelectedIndex = 0;
            txtNumeroDocumento.Text = string.Empty;
            ddlTipoPassageiro.SelectedIndex = 0;
            ddlNacionalidade.SelectedValue = "BR";
            ddlTipoProgramaFidelidade.SelectedIndex = 0;
            txtCodigoProgramaFidelidade.Text = string.Empty;
        }

        protected void btnAdicionarProduto_Click(object sender, EventArgs e)
        {
            Page.Validate("vlgDadosProduto");

            if (Page.IsValid)
            {
                Produto item = new Produto();
                item.Codigo = txtcodigoProduto.Text;
                item.Categoria = ddlCategoriaProduto.SelectedValue;
                item.Nome = txtNomeProduto.Text;
                item.Quantidade = Int16.Parse(txtQuantidadeProduto.Text);
                item.PrecoUnitario = (Double)txtPrecoUnitarioProduto.Text.ToDecimalNull(0).Value;
                item.GrauRisco = ddlRisco.SelectedValue;
                item.GrauRiscoDescricao = ddlRisco.SelectedItem.Text;
                ListaProdutos.Add(item);

                gvProdutos.DataSource = ListaProdutos;
                gvProdutos.DataBind();
                
                LimparDadosProduto();
            }
        }
        void LimparDadosProduto()
        {
            txtcodigoProduto.Text = string.Empty;
            ddlCategoriaProduto.SelectedIndex = 0;
            txtNomeProduto.Text = string.Empty;
            txtQuantidadeProduto.Text = string.Empty;
            txtPrecoUnitarioProduto.Text = string.Empty;
            ddlRisco.SelectedIndex = 0;

        }
        public Modelo.AnalisedeRisco ObterAnalisedeRisco()
        {
            Modelo.AnalisedeRisco analise = new Modelo.AnalisedeRisco();
            analise.DadosGerais = ObterDadosGerais();

            analise.DadosEntrega = ObterDadosEntrega(rboInstalacaoSim.Checked);
            analise.DadosCobranca = ObterDadosCobranca();
            analise.DetalhesPedido = ObterDetalhePedido();
            analise.DadosIATA = ObterDadosIata();

            return analise;
        }

        Modelo.DadosGerais ObterDadosGerais()
        {
            Modelo.DadosGerais dados = new Modelo.DadosGerais();
            dados.CodigoCliente = txtCodigoCliente.Text;
            dados.DataNascimento = txtDataNascimento.Text;
            dados.DDDTelefone = txtDDDFone.Text;
            dados.Telefone = txtFone.Text;
            dados.DDDCelular = txtDDDCelular.Text;
            dados.Celular = txtCelular.Text;
            dados.Documento = txtDocumento.Text;
            dados.Email = txtEmailDadosGerais.Text;
            return dados;
        }
        Modelo.DadosEntrega ObterDadosEntrega(Boolean requerInstalacao)
        {
            Modelo.DadosEntrega entrega = new DadosEntrega();
            entrega.NomeDestinatario = txtNomeDestinatario.Text;
            entrega.SobrenomeDestinatario = txtSobrenomeDestinatario.Text;
            entrega.DataColeta = txtDataColetaEntrega.Text;
            entrega.RequerInstalacao = requerInstalacao;
            entrega.Endereco = ucEnderecoEntrega.ObterEndereco(requerInstalacao);
            entrega.BoxExpandido = hddBoxDadosEntregaExpandido.Value.CompareTo("true") == 0;

            return entrega;
        }
        Modelo.DadosCobranca ObterDadosCobranca()
        {
            Modelo.DadosCobranca cobranca = new DadosCobranca();

            cobranca.Endereco = ucEnderecoDadosCobranca.ObterEndereco();
            cobranca.BoxExpandido = hddBoxDadosCobrancaExpandido.Value.CompareTo("true") == 0;

            return cobranca;
        }

        Modelo.DetalhesPedido ObterDetalhePedido()
        {
            Modelo.DetalhesPedido pedido = new DetalhesPedido();
            pedido.Produtos = ListaProdutos;
            pedido.BoxExpandido = hddBoxDetalhesPedidoExpandido.Value.CompareTo("true") == 0;
            return pedido;
        }

        Modelo.DadosIATA ObterDadosIata()
        {
            Modelo.DadosIATA iata = new Modelo.DadosIATA();
            iata.CodigoCompanhia = txtCodigoCompanhia.Text;
            iata.NumeroVoo = txtNumeroVoo.Text;
            
            Int32 baseTarifaria = 0;
            iata.BaseTarifaria = Redecard.PN.DataCash.Modelo.Util.ExtensionMethods.TryParseToInt32(txtBaseTarifaria.Text, baseTarifaria);
            
            iata.CPF = txtCPFPortadorCartao.Text;
            iata.Nome = txtNome.Text;
            iata.Sobrenome = txtSobrenome.Text;

            iata.Passageiros = ListaPassageiros;

            iata.BoxExpandido = hddBoxDadosIATAExpandido.Value.CompareTo("true") == 0;

            return iata;
        }
        #region Grid Produtos
        protected void gvProdutos_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvProdutos.EditIndex = e.NewEditIndex;
            gvProdutos.DataSource = ListaProdutos;
            gvProdutos.DataBind();

        }
        protected void gvProdutos_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvProdutos.EditIndex = -1;
            gvProdutos.DataSource = ListaProdutos;
            gvProdutos.DataBind();

        }
        protected void gvProdutos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var cadastrado = ListaProdutos.Find(item => item.Codigo == e.Keys[0].ToString());
            ListaProdutos.Remove(cadastrado);
            gvProdutos.DataSource = ListaProdutos;
            gvProdutos.DataBind();

        }
        protected void gvProdutos_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (gvProdutos.EditIndex >= 0)
            {
                using (Logger Log = Logger.IniciarLog("gvProdutos_RowUpdating - Faça sua Venda"))
                {
                    try
                    {
                        Page.Validate("vlgDadosProdutoGV");
                        if (Page.IsValid)
                        {
                            TextBox txtCodigoProduto = gvProdutos.Rows[e.RowIndex].FindControl("txtCodigoProduto") as TextBox;
                            TextBox txtNomeProduto = gvProdutos.Rows[e.RowIndex].FindControl("txtNomeProduto") as TextBox;
                            TextBox txtQtdeProduto = gvProdutos.Rows[e.RowIndex].FindControl("txtQtdeProduto") as TextBox;
                            DropDownList ddlCategoriaProdutoGv = gvProdutos.Rows[e.RowIndex].FindControl("ddlCategoriaProdutoGv") as DropDownList;
                            TextBox txtPrecoProduto = gvProdutos.Rows[e.RowIndex].FindControl("txtPrecoProduto") as TextBox;
                            DropDownList ddlRiscoGv = gvProdutos.Rows[e.RowIndex].FindControl("ddlRiscoGv") as DropDownList;


                            ListaProdutos[e.RowIndex].Categoria = ddlCategoriaProduto.SelectedValue;
                            ListaProdutos[e.RowIndex].Nome = txtNomeProduto.Text;
                            ListaProdutos[e.RowIndex].Quantidade = Int16.Parse(txtQtdeProduto.Text);
                            ListaProdutos[e.RowIndex].Categoria = ddlCategoriaProdutoGv.SelectedValue;
                            ListaProdutos[e.RowIndex].PrecoUnitario = Convert.ToDouble(txtPrecoProduto.Text);
                            ListaProdutos[e.RowIndex].GrauRisco = ddlRiscoGv.SelectedValue;
                            ListaProdutos[e.RowIndex].GrauRiscoDescricao = ddlRiscoGv.SelectedItem.Text;

                            gvProdutos.EditIndex = -1;
                            gvProdutos.DataSource = ListaProdutos;
                            gvProdutos.DataBind();
                        }
                    }
                    catch (PortalRedecardException ex)
                    {
                        Log.GravarErro(ex);
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                    }
                }
            }
        }
        protected void gvProdutos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState == DataControlRowState.Edit) || e.Row.RowState == (DataControlRowState.Edit | DataControlRowState.Alternate))
                {
                    DropDownList ddlCategoriaProdutoGv = e.Row.FindControl("ddlCategoriaProdutoGv") as DropDownList;
                    if (ddlCategoriaProdutoGv != null)
                    {
                        ddlCategoriaProdutoGv.Items.Clear();
                        ddlCategoriaProdutoGv.DataSource = ProcessaListas.ObterCategorias();
                        ddlCategoriaProdutoGv.DataTextField = "Text";
                        ddlCategoriaProdutoGv.DataValueField = "Value";
                        ddlCategoriaProdutoGv.DataBind();
                        ddlCategoriaProdutoGv.SelectedValue = (e.Row.DataItem as Produto).Categoria;
                    }
                }
            }
        }
        protected void gvProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Update")
                gvProdutos.UpdateRow(gvProdutos.EditIndex, false);
        }

        protected void gvPassageiros_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState == DataControlRowState.Edit) || e.Row.RowState == (DataControlRowState.Edit | DataControlRowState.Alternate))
                {
                    DropDownList ddlPaisOrigem = e.Row.FindControl("ddlPaisOrigem") as DropDownList;
                    if (ddlPaisOrigem != null)
                    {
                        ddlPaisOrigem.Items.Clear();
                        ddlPaisOrigem.DataSource = ProcessaListas.ObterPaises();
                        ddlPaisOrigem.DataTextField = "Text";
                        ddlPaisOrigem.DataValueField = "Value";
                        ddlPaisOrigem.DataBind();
                        ddlPaisOrigem.SelectedValue = (e.Row.DataItem as DadosPassageiro).PaisOrigem;
                    }
                    DropDownList ddlTipoPassageiro = e.Row.FindControl("ddlTipoPassageiro") as DropDownList;
                    if (ddlTipoPassageiro != null)
                    {
                        ddlTipoPassageiro.SelectedValue = (e.Row.DataItem as DadosPassageiro).TipoPassageiro;
                    }
                    DropDownList ddlTipoDocumento = e.Row.FindControl("ddlTipoDocumento") as DropDownList;
                    if (ddlTipoDocumento != null)
                    {
                        ddlTipoDocumento.SelectedValue = (e.Row.DataItem as DadosPassageiro).TipoDocumento;
                    }
                    DropDownList ddlTipoProgramaFidelidade = e.Row.FindControl("ddlTipoProgramaFidelidade") as DropDownList;
                    if (ddlTipoProgramaFidelidade != null)
                    {
                        ddlTipoProgramaFidelidade.SelectedValue = (e.Row.DataItem as DadosPassageiro).TipoProgramaFidelidade;
                    }
                }
            }
        }

        #endregion
        #region Grid Passageiros
        protected void gvPassageiros_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvPassageiros.EditIndex = e.NewEditIndex;
            gvPassageiros.DataSource = ListaPassageiros;
            gvPassageiros.DataBind();

        }
        protected void gvPassageiros_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPassageiros.EditIndex = -1;
            gvPassageiros.DataSource = ListaPassageiros;
            gvPassageiros.DataBind();

        }
        protected void gvPassageiros_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var cadastrado = ListaPassageiros.Find(item => item.NumeroDocumento == e.Keys[0].ToString());
            ListaPassageiros.Remove(cadastrado);
            gvPassageiros.DataSource = ListaPassageiros;
            gvPassageiros.DataBind();

        }
        protected void gvPassageiros_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (gvPassageiros.EditIndex >= 0)
            {
                using (Logger Log = Logger.IniciarLog("gvPassageiros_RowUpdating - Faça sua Venda"))
                {
                    try
                    {
                        DropDownList ddlTipoDocumento = gvPassageiros.Rows[e.RowIndex].FindControl("ddlTipoDocumento") as DropDownList;
                        TextBox txtNumeroDocumento = gvPassageiros.Rows[e.RowIndex].FindControl("txtNumeroDocumento") as TextBox;
                        DropDownList ddlTipoPassageiro = gvPassageiros.Rows[e.RowIndex].FindControl("ddlTipoPassageiro") as DropDownList;
                        DropDownList ddlPaisOrigem = gvPassageiros.Rows[e.RowIndex].FindControl("ddlPaisOrigem") as DropDownList;
                        DropDownList ddlTipoProgramaFidelidade = gvPassageiros.Rows[e.RowIndex].FindControl("ddlTipoProgramaFidelidade") as DropDownList;
                        TextBox txtCodigoProgramaFidelidade = gvPassageiros.Rows[e.RowIndex].FindControl("txtCodigoProgramaFidelidade") as TextBox;

                        ListaPassageiros[e.RowIndex].TipoDocumento = ddlTipoDocumento.SelectedValue;
                        ListaPassageiros[e.RowIndex].TipoDocumentoDescricao = ddlTipoDocumento.SelectedItem.Text;
                        ListaPassageiros[e.RowIndex].NumeroDocumento = txtNumeroDocumento.Text;
                        ListaPassageiros[e.RowIndex].TipoPassageiro = ddlTipoPassageiro.SelectedValue;
                        ListaPassageiros[e.RowIndex].TipoPassageiroDescricao = ddlTipoPassageiro.SelectedItem.Text;
                        ListaPassageiros[e.RowIndex].PaisOrigem = ddlPaisOrigem.SelectedValue;
                        ListaPassageiros[e.RowIndex].PaisOrigemDescricao = ddlPaisOrigem.SelectedItem.Text;
                        ListaPassageiros[e.RowIndex].TipoProgramaFidelidade = ddlTipoProgramaFidelidade.SelectedValue;
                        ListaPassageiros[e.RowIndex].TipoProgramaFidelidadeDescricao = ddlTipoProgramaFidelidade.SelectedItem.Text;
                        ListaPassageiros[e.RowIndex].CodigoProgramaFidelidade = txtCodigoProgramaFidelidade.Text;

                        gvPassageiros.EditIndex = -1;
                        gvPassageiros.DataSource = ListaPassageiros;
                        gvPassageiros.DataBind();
                    }
                    catch (PortalRedecardException ex)
                    {
                        Log.GravarErro(ex);
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                    }
                }
            }
        }
        protected void gvPassageiros_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Update")
                gvPassageiros.UpdateRow(gvPassageiros.EditIndex, false);
        }
        #endregion

        void CarregarDropDown()
        {
            ddlCategoriaProduto.Items.Clear();
            ddlCategoriaProduto.DataSource = ProcessaListas.ObterCategorias();
            ddlCategoriaProduto.DataTextField = "Text";
            ddlCategoriaProduto.DataValueField = "Value";
            ddlCategoriaProduto.DataBind();

            ddlNacionalidade.Items.Clear();
            ddlNacionalidade.DataSource = ProcessaListas.ObterPaises();
            ddlNacionalidade.DataTextField = "Text";
            ddlNacionalidade.DataValueField = "Value";
            ddlNacionalidade.DataBind();

            ddlNacionalidade.SelectedValue = "BR";
        }


        internal void CarregaAnaliseRisco(Venda venda)
        {
            switch (venda.TipoTransacao)
            {
                case enTipoTransacao.IATA:
                    CarregaDadosGerais(venda.AnalisedeRisco.DadosGerais);
                    CarregaDadosCobranca(venda.AnalisedeRisco.DadosCobranca);
                    CarregaDadosIATA(venda.AnalisedeRisco.DadosIATA);
                    break;
                default:
                    CarregaDadosEntrega(venda.AnalisedeRisco.DadosEntrega);
                    CarregaDadosGerais(venda.AnalisedeRisco.DadosGerais);
                    CarregaDadosCobranca(venda.AnalisedeRisco.DadosCobranca);
                    CarregaDadosDetalhesPedido(venda.AnalisedeRisco.DetalhesPedido);
                    break;
            }
        }

        private void CarregaDadosIATA(Modelo.DadosIATA dadosIATA)
        {
            if (dadosIATA != null)
            {
                //Carrega grid de passageiros
                gvPassageiros.DataSource = dadosIATA.Passageiros;
                gvPassageiros.DataBind();

                this.ConfigurarExibicaoBox(dadosIATA.BoxExpandido, dvDadosIATA, imgDadosIATA, hddBoxDadosIATAExpandido);
                this.ListaPassageiros = dadosIATA.Passageiros;
            }
        }

        private void CarregaDadosDetalhesPedido(DetalhesPedido detalhesPedido)
        {
            if (detalhesPedido != null)
            {
                //Carrega itens de pedido
                gvProdutos.DataSource = detalhesPedido.Produtos;
                gvProdutos.DataBind();

                this.ConfigurarExibicaoBox(detalhesPedido.BoxExpandido, dvDetalhesPedido, imgDetalhesPedido, hddBoxDetalhesPedidoExpandido);
                this.ListaProdutos = detalhesPedido.Produtos;
            }
        }

        private void CarregaDadosCobranca(DadosCobranca dadosCobranca)
        {
            if (dadosCobranca != null)
            {
                //Carrega dados de cobrança
                ucEnderecoDadosCobranca.CarregarEndereco(dadosCobranca.Endereco);

                this.ConfigurarExibicaoBox(dadosCobranca.BoxExpandido, dvDadosCobranca, imgDadosCobranca, hddBoxDadosCobrancaExpandido);
            }
        }

        private void CarregaDadosEntrega(DadosEntrega dadosEntrega)
        {
            if (dadosEntrega != null)
            {
                //Carrega dados de entrega
                txtNomeDestinatario.Text = dadosEntrega.NomeDestinatario;
                txtSobrenomeDestinatario.Text = dadosEntrega.SobrenomeDestinatario;
                txtDataColetaEntrega.Text = dadosEntrega.DataColeta;
                if (dadosEntrega.RequerInstalacao)
                {
                    rboInstalacaoSim.Checked = true; //Sim
                    ucEnderecoEntrega.CarregarEndereco(dadosEntrega.Endereco);
                }
                else
                    rboInstalacaoNao.Checked = true; //Não

                this.ConfigurarExibicaoBox(dadosEntrega.BoxExpandido, dvDadosEntrega, imgDadosEntrega, hddBoxDadosEntregaExpandido);
            }
        }

        private void CarregaDadosGerais(DadosGerais dadosGerais)
        {
            if (dadosGerais != null)
            {
                //carrega dados gerais
                txtCodigoCliente.Text = dadosGerais.CodigoCliente;
                txtEmailDadosGerais.Text = dadosGerais.Email;
                txtDDDFone.Text = dadosGerais.DDDTelefone;
                txtFone.Text = dadosGerais.Telefone;
                txtDDDCelular.Text = dadosGerais.DDDCelular;
                txtCelular.Text = dadosGerais.Celular;
                txtDataNascimento.Text = dadosGerais.DataNascimento;
                txtDocumento.Text = dadosGerais.Documento;
            }
        }

        /// <summary>
        /// Configura exibição dos boxes de acordo com o estado prévio
        /// </summary>
        /// <param name="boxExpandido">Box Expandido?</param>
        /// <param name="divBox">Div do Box</param>
        /// <param name="imagemBox">Imagem do Box</param>
        private void ConfigurarExibicaoBox(Boolean boxExpandido, HtmlGenericControl divBox, Image imagemBox, HiddenField hiddenBoxExpandido)
        {
            imagemBox.ImageUrl = boxExpandido ? "../images/btnOcultar.png" : "../images/btnExpandir.png";
            divBox.Attributes["style"] = boxExpandido ? String.Empty : "display:none";
            hiddenBoxExpandido.Value = boxExpandido ? "true" : "false";
        }
    }
}
