using Rede.PN.CondicaoComercial.Core.Web.Controles.Portal;
using Rede.PN.CondicaoComercial.SharePoint.Business;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Rede.PN.CondicaoComercial.SharePoint.EntidadeServico;

namespace Rede.PN.CondicaoComercial.SharePoint.ControlTemplates.DadosBancarios
{
    public partial class DadosBancariosDebitoUserControl : UserControlBase
    {
        /// <summary>
        /// Relação das bandeiras que devem ser removidas da listagem
        /// </summary>
        private readonly Int32[] listBandeirasRemover = new Int32[] {

            // bandeiras Amex,
            69,

            // bandeiras Redeshop e Avisa
            20, 22, 67,

            // bandeiras internacionais
            2, 4
        };

        /// <summary>
        /// Imagens das bandeiras
        /// </summary>
        private List<BandeiraImagem> BandeirasImagens
        {
            get
            {
                if (ViewState["BandeirasImagens"] == null)
                    ViewState["BandeirasImagens"] = ListaImagensBandeira.ObterImagensBandeiras();
                return (List<BandeiraImagem>)ViewState["BandeirasImagens"];
            }
            set { ViewState["BandeirasImagens"] = value; }
        }

        /// <summary>
        /// Inicialização da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CarregarDados();
            }
        }

        /// <summary>
        /// Carrega os Dados Bancários de Débito na tela
        /// </summary>
        private void CarregarDados()
        {
            using (Logger Log = Logger.IniciarLog("Dados bancários - Débito - Carregar página"))
            {
                try
                {
                    using (var entidadeServico = new EntidadeServico.EntidadeServicoClient())
                    {
                        //Código de retorno de erro de negócio na proc
                        Int32 codigoRetorno;

                        //Consulta os dados bancários de Crédito da Entidade. 
                        //Tipo dados "C" representa Crédito. "D" para buscar somente débito. "V" para buscar somente voucher
                        var dadosBancarios = entidadeServico.ConsultarDadosBancarios(out codigoRetorno, SessaoAtual.CodigoEntidade, "D");

                        if (dadosBancarios.Length != 0)
                        {
                            // Caso o código de retorno seja <> de 0 ocorreu um erro
                            if (codigoRetorno > 0)
                                base.ExibirPainelExcecao("EntidadeServico.ConsultarDadosBancarios", codigoRetorno);
                            else
                            {
                                // Bind no Repeater
                                rptTabelaDebito.DataSource = MontarItensDebitoOld(dadosBancarios);
                                rptTabelaDebito.DataBind();

                                grvTabelaDebito.DataSource = MontarItensDebito(dadosBancarios);
                                grvTabelaDebito.DataBind();

                                hdnContemRegistrosDebito.Value = "1";

                                if (grvTabelaDebito.Rows.Count > 0)
                                    grvTabelaDebito.HeaderRow.TableSection = TableRowSection.TableHeader;
                            }
                        }
                        else
                        {
                            pnlDebito.Visible = false;
                            qdAvisoDebito.Visible = true;
                            qdAvisoDebito.TipoQuadro = TipoQuadroAviso.Aviso;
                            qdAvisoDebito.Mensagem = "Não há bandeiras contratadas para este estabelecimento";
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
                }
            }
        }

        /// <summary>
        /// Efetua o preenchimento dos dados no UserControl na renderização de Contas encontradas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptTabelaDebito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    TabelaDebito tblDebito = (TabelaDebito)e.Item.FindControl("tblDebito");

                    if (tblDebito != null)
                    {
                        DataSet tabelas = new DataSet();
                        DataSet dadosBancarios = (DataSet)e.Item.DataItem;
                        DataTable tblBanco = dadosBancarios.Tables["Banco"];
                        DataTable tblPagamento = dadosBancarios.Tables["ProdutoPagamento"];
                        DataTable tblParcelamento = dadosBancarios.Tables["ProdutoParcelamento"];

                        tabelas.Tables.Add(tblPagamento.Copy());
                        tabelas.Tables.Add(tblParcelamento.Copy());

                        tblDebito.DescricaoCartao = tblBanco.Rows[0]["DescricaoCartao"].ToString();
                        tblDebito.Banco = tblBanco.Rows[0]["NomeBanco"].ToString();
                        tblDebito.Agencia = tblBanco.Rows[0]["NomeAgencia"].ToString();
                        tblDebito.Conta = tblBanco.Rows[0]["ContaAtualizada"].ToString();

                        tblDebito.Tabelas = tabelas;
                        tblDebito.Visible = true;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Logger.GravarErro("Erro durante bind de dados na tabela", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante bind de dados na tabela", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
            }
        }

        /// <summary>
        /// Organiza os itens de Crédito em um novo DataSet para preenchimento das Tabelas Parcelamento e Pagamento do UserControl de Crédito
        /// </summary>
        /// <param name="listaDados">Lista de Dados Bancários Débito</param>
        /// <returns>Novo DataSet para preenchimento das Tabelas Parcelamento e Pagamento</returns>
        private List<DataSet> MontarItensDebitoOld(EntidadeServico.DadosBancarios[] listaDados)
        {
            var dadosBancariosLista = listaDados.OrderBy(x =>
            {
                /* 
                 * JFR: define a ordenação das bandeiras, sendo:
                 * - 1º master
                 * - 2º visa
                 * - 3º demais */

                Int32 order = 2;

                if (x.DescricaoCartao.ToLower().Contains("master")|| x.DescricaoCartao.ToLower().Contains("maestro"))
                    order = 0;

                else if (x.DescricaoCartao.ToLower().Contains("visa"))
                    order = 1;

                return order;

            }).ToList();

            // SKU: Nao deve conter a bandeira AMEX(69)
            // JFR: não deve conter bandeiras internacionais e outras
            dadosBancariosLista.RemoveAll(x => listBandeirasRemover.Any(b => String.Compare(x.CodigoCartao, b.ToString()) == 0));

            listaDados = dadosBancariosLista.ToArray();

            Int32 codigoCartao = 0;

            String descricaoCartao = String.Empty;
            String nomeBanco = String.Empty;
            String codigoAgencia = String.Empty;
            String contaAtualizada = String.Empty;

            List<DataSet> listaTabelasDebito = new List<DataSet>();

            DataSet tabelas = new DataSet();
            DataRow linhaTabela;
            DataTable tblPagamento = new DataTable("ProdutoPagamento");
            DataTable tblParcelamento = new DataTable("ProdutoParcelamento");
            DataTable tblBanco = new DataTable("Banco");

            if (listaDados != null)
                codigoCartao = listaDados[0].CodigoCartao.ToString().ToInt32();

            tblPagamento.Columns.Add("Descricao");
            tblPagamento.Columns.Add("Prazo");
            tblPagamento.Columns.Add("Taxa");
            tblPagamento.Columns.Add("Tarifa");

            tblParcelamento.Columns.Add("Descricao");
            tblParcelamento.Columns.Add("Prazo");
            tblParcelamento.Columns.Add("Taxa");
            tblParcelamento.Columns.Add("TaxaEmissor");

            tblBanco.Columns.Add("DescricaoCartao");
            tblBanco.Columns.Add("NomeBanco");
            tblBanco.Columns.Add("NomeAgencia");
            tblBanco.Columns.Add("ContaAtualizada");

            foreach (EntidadeServico.DadosBancarios dado in listaDados)
            {
                if (!codigoCartao.Equals(dado.CodigoCartao.ToString().ToInt32()))
                {
                    tabelas.Tables.Clear();
                    tabelas.Tables.Add(tblPagamento);
                    tabelas.Tables.Add(tblParcelamento);

                    tblBanco.Rows.Clear();
                    linhaTabela = tblBanco.NewRow();
                    linhaTabela["DescricaoCartao"] = descricaoCartao;
                    linhaTabela["NomeBanco"] = nomeBanco;
                    linhaTabela["NomeAgencia"] = codigoAgencia;
                    linhaTabela["ContaAtualizada"] = contaAtualizada;
                    tblBanco.Rows.Add(linhaTabela);
                    tabelas.Tables.Add(tblBanco);

                    listaTabelasDebito.Add(tabelas);

                    tabelas = new DataSet();
                    tblPagamento = new DataTable("ProdutoPagamento");
                    tblParcelamento = new DataTable("ProdutoParcelamento");
                    tblBanco = new DataTable("Banco");

                    tblPagamento.Columns.Add("Descricao");
                    tblPagamento.Columns.Add("Prazo");
                    tblPagamento.Columns.Add("Taxa");
                    tblPagamento.Columns.Add("Tarifa");

                    tblParcelamento.Columns.Add("Descricao");
                    tblParcelamento.Columns.Add("Prazo");
                    tblParcelamento.Columns.Add("Taxa");
                    tblParcelamento.Columns.Add("TaxaEmissor");

                    tblBanco.Columns.Add("DescricaoCartao");
                    tblBanco.Columns.Add("NomeBanco");
                    tblBanco.Columns.Add("NomeAgencia");
                    tblBanco.Columns.Add("ContaAtualizada");
                }

                descricaoCartao = dado.DescricaoCartao;
                nomeBanco = dado.NomeBanco;
                codigoAgencia = dado.CodigoAgencia;
                contaAtualizada = dado.ContaAtualizada;

                if (!dado.CodigoFEAT.Equals("5") && !dado.TemTarifa)
                {
                    linhaTabela = tblPagamento.NewRow();
                    if (!dado.TemTarifa)
                        linhaTabela["Prazo"] = String.Format("{0} dias", dado.PercentualTaxa.ToString());
                    else
                        linhaTabela["Prazo"] = String.Format("{0} dias", dado.PercentualTarifa.ToString());

                    linhaTabela["Taxa"] = dado.Taxa.ToString();
                    linhaTabela["Descricao"] = dado.DescricaoFEAT.ToUpper();
                    linhaTabela["Tarifa"] = dado.Tarifa.ToString();
                    tblPagamento.Rows.Add(linhaTabela);
                }
                else
                {
                    linhaTabela = tblParcelamento.NewRow();
                    linhaTabela["Descricao"] = dado.DescricaoFEAT.ToUpper();

                    linhaTabela["Prazo"] = "De 2 a 6 Parcelas";

                    linhaTabela["Taxa"] = dado.Taxa;
                    linhaTabela["TaxaEmissor"] = dado.Tarifa.ToString();
                    tblParcelamento.Rows.Add(linhaTabela);
                }

                codigoCartao = dado.CodigoCartao.ToString().ToInt32();
            }

            tabelas.Tables.Clear();
            tabelas.Tables.Add(tblPagamento);
            tabelas.Tables.Add(tblParcelamento);

            tblBanco.Rows.Clear();
            linhaTabela = tblBanco.NewRow();
            linhaTabela["DescricaoCartao"] = descricaoCartao;
            linhaTabela["NomeBanco"] = nomeBanco;
            linhaTabela["NomeAgencia"] = codigoAgencia;
            linhaTabela["ContaAtualizada"] = contaAtualizada;
            tblBanco.Rows.Add(linhaTabela);
            tabelas.Tables.Add(tblBanco);

            listaTabelasDebito.Add(tabelas);

            return listaTabelasDebito;
        }

        private List<Business.Bandeira> MontarItensDebito(EntidadeServico.DadosBancarios[] listaDados)
        {
            var dadosBancariosLista = listaDados.OrderBy(x =>
            {
                /* 
                 * JFR: define a ordenação das bandeiras, sendo:
                 * - 1º master
                 * - 2º visa
                 * - 3º demais */

                Int32 order = 2;

                if (x.DescricaoCartao.ToLower().Contains("master") || x.DescricaoCartao.ToLower().Contains("maestro"))
                    order = 0;

                else if (x.DescricaoCartao.ToLower().Contains("visa"))
                    order = 1;

                return order;

            }).ToList();

            // SKU: Nao deve conter a bandeira AMEX(69), ELO(70) e ELO(71)
            // JFR: não deve conter bandeiras internacionais e outras
            dadosBancariosLista.RemoveAll(x => listBandeirasRemover.Any(b => String.Compare(x.CodigoCartao, b.ToString()) == 0));

            listaDados = dadosBancariosLista.ToArray();

            List<Business.Bandeira> bandeiras = new List<Business.Bandeira>();

            Business.Bandeira bandeira = null;
            foreach (EntidadeServico.DadosBancarios dado in listaDados)
            {
                // se é a primeira vez ou se ja nao é a sequencia da mesma bandeira
                if ((bandeira == null) || (bandeira.Codigo != dado.CodigoCartao.ToInt32()))
                {
                    if (bandeira != null)
                        bandeiras.Add(bandeira);

                    bandeira = new Business.Bandeira();
                    bandeira.Taxas = new List<Business.Taxa>();
                    bandeira.Flex = new List<Flex>();
                    bandeira.Codigo = dado.CodigoCartao.ToInt32();
                }

                bandeira.Nome = dado.DescricaoCartao;
                bandeira.NomeBanco = dado.NomeBanco;
                bandeira.CodigoAgencia = dado.CodigoAgencia;
                bandeira.ContaAtualizada = dado.ContaAtualizada;

                Business.Taxa taxa = new Business.Taxa();
                taxa.ModalidadeVenda = dado.DescricaoFEAT.ToLower();
                taxa.ValorTaxa = dado.Taxa;

                if (!dado.CodigoFEAT.Equals("5") && !dado.TemTarifa)
                {
                    if (!dado.TemTarifa)
                        taxa.Prazo = String.Format("{0} dias", dado.PercentualTaxa);
                    else
                        taxa.Prazo = String.Format("{0} dias", dado.PercentualTarifa);

                    taxa.Tarifa = dado.Tarifa;
                }
                else
                {
                    taxa.Prazo = "De 2 a 6 Parcelas";
                    taxa.ValorTaxaEmissor = dado.Tarifa;
                    taxa.Predatado = true;
                }

                bandeira.Taxas.Add(taxa);
            }

            // adiciona a ultima bandeira
            if (bandeira != null)
                bandeiras.Add(bandeira);

            return bandeiras;
        }

        protected void grvTabelaDebito_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Business.Bandeira bandeira = (Business.Bandeira)e.Row.DataItem;

                Image imgBandeira = (Image)e.Row.FindControl("imgBandeira");
                Label lblBandeira = (Label)e.Row.FindControl("lblBandeira");

                Repeater rptPrazo = (Repeater)e.Row.FindControl("rptPrazo");
                Repeater rptTipoVenda = (Repeater)e.Row.FindControl("rptTipoVenda");
                Repeater rptTaxas = (Repeater)e.Row.FindControl("rptTaxas");
                Repeater rptTarifas = (Repeater)e.Row.FindControl("rptTarifas");

                BandeiraImagem imagem = this.BandeirasImagens.FirstOrDefault(b => b.Codigo == bandeira.Codigo);

                if (imagem != null)
                {
                    imgBandeira.ImageUrl = imagem.Url;
                    lblBandeira.Text = imagem.Descricao;
                }
                else
                {
                    lblBandeira.Text = bandeira.Nome;
                }

                rptPrazo.DataSource = bandeira.Taxas.Select(t => t.Prazo);
                rptPrazo.DataBind();

                rptTipoVenda.DataSource = bandeira.Taxas.Select(t => t.ModalidadeVenda);
                rptTipoVenda.DataBind();

                rptTaxas.DataSource = bandeira.Taxas.Select(t => new
                {
                    Taxa = String.Format("{0:N2}", t.ValorTaxa)
                });
                rptTaxas.DataBind();

                rptTarifas.DataSource = bandeira.Taxas.Select(t => {
                    Decimal valorTarifa = t.Predatado ? t.ValorTaxaEmissor : t.Tarifa;
                    return new
                    {
                        Tarifa = valorTarifa == 0 ? "-" : String.Format("{0:N2}", valorTarifa)
                    };
                });
                rptTarifas.DataBind();
            }
        }
    }
}
