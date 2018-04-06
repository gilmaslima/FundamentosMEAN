using Rede.PN.CondicaoComercial.Core.Web.Controles.Portal;
using Rede.PN.CondicaoComercial.SharePoint.Business;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rede.PN.CondicaoComercial.SharePoint.ControlTemplates.DadosBancarios
{
    public partial class DadosBancariosCreditoUserControl : UserControlBase
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
        /// Inicialiazação da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Dados Bancários - Crédito - Carregando página"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        CarregarDados();
                        // 10/10/2016 - Carol disse que o quadro tarifa manual nao deve mais aparecer;
                        //PreencherTarifas();
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
                }
            }
        }

        /// <summary>
        /// Preenche as tarifas de Crédito da Entidade
        /// </summary>
        private void PreencherTarifas()
        {
            using (Logger log = Logger.IniciarLog("Preencher tarifas"))
            {
                using (var entidadeServico = new EntidadeServico.EntidadeServicoClient())
                {
                    //Código de retorno de erro de negócio na proc
                    Int32 codigoRetorno;

                    //Consulta as tarifas de Crédito da Entidade.
                    //Tipo dados "CR" representa Crédito. "DB" para buscar somente débito
                    var dadosTarifas = entidadeServico.ConsultarTarifas(out codigoRetorno, SessaoAtual.CodigoEntidade, "CR");

                    if (dadosTarifas != null)
                    {
                        // Caso o código de retorno seja <> de 0 ocorreu um erro
                        if (codigoRetorno > 0)
                            base.ExibirPainelExcecao("EntidadeServico.ConsultarTarifas", codigoRetorno);
                        else
                        {
                            lblTransferenciaEletronica.Text = dadosTarifas.ValorTarifaEletronica.ToString();
                            lblTransferenciaManual.Text = dadosTarifas.ValorTarifaManual.ToString();
                            pnlTarifario.Visible = (dadosTarifas.ValorTarifaManual > 0);
                        }
                    }
                    else
                    {
                        //Não há tarifas para a entidade
                        pnlTarifario.Visible = false;
                        qdAvisoTarifas.Visible = true;
                        qdAvisoTarifas.TipoQuadro = TipoQuadroAviso.Aviso;
                        qdAvisoTarifas.Mensagem = "Não há tarifas para a Entidade";
                    }
                }
            }
        }

        /// <summary>
        /// Carrega os Dados Bancários de Crédito na tela
        /// </summary>
        private void CarregarDados()
        {
            using (Logger log = Logger.IniciarLog("Carregando Dados Bancários Crédito"))
            using (var entidadeServico = new EntidadeServico.EntidadeServicoClient())
            {
                //Código de retorno de erro de negócio na proc
                Int32 codRetornoDadosBancarios;
                Int32 codRetornoProdutosFlex;

                //Consulta os dados bancários de Crédito da Entidade. 
                //Tipo dados "C" representa Crédito. "D" para buscar somente débito. "V" para buscar somente voucher
                var dadosBancariosCredito = entidadeServico.ConsultarDadosBancarios(out codRetornoDadosBancarios, base.SessaoAtual.CodigoEntidade, "C");

                //Consulta todos os produtos Flex para a entidade
                var produtosFlex = entidadeServico.ConsultarProdutosFlex(out codRetornoProdutosFlex, SessaoAtual.CodigoEntidade, null, null);

                if (dadosBancariosCredito.Length != 0)
                {
                    // Caso o código de retorno seja <> de 0 ocorreu um erro
                    if (codRetornoDadosBancarios > 0)
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarDadosBancarios", codRetornoDadosBancarios);
                    else if (codRetornoProdutosFlex > 0 && codRetornoProdutosFlex != 32180) //produtos flex não cadastrados
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarProdutosFlex", codRetornoProdutosFlex);
                    else
                    {
                        // Bind no Repeater
                        //rptTabelaCredito.DataSource = dadosBancarios;
                        rptTabelaCredito.DataSource = MontarItensCreditoOld(dadosBancariosCredito, produtosFlex);
                        rptTabelaCredito.DataBind();

                        var bandeiras = MontarItensCredito(dadosBancariosCredito, produtosFlex);

                        hdnContemRegistrosCredito.Value = "1";

                        grvTabelaCreditoFlex.DataSource = bandeiras.Where(b => b.Flex.Count > 0);
                        grvTabelaCreditoFlex.DataBind();

                        if (grvTabelaCreditoFlex.Rows.Count > 0)
                            grvTabelaCreditoFlex.HeaderRow.TableSection = TableRowSection.TableHeader;
                        else
                            divQuadroFlex.Visible = false;

                        grvTabelaCreditoTaxas.DataSource = bandeiras.Where(b => b.Taxas.Count > 0);
                        grvTabelaCreditoTaxas.DataBind();

                        if (grvTabelaCreditoTaxas.Rows.Count > 0)
                            grvTabelaCreditoTaxas.HeaderRow.TableSection = TableRowSection.TableHeader;
                        else
                            divQuadroCredito.Visible = false;
                    }
                }
                else
                {
                    pnlCredito.Visible = false;
                    qdAvisoDadosBancarios.Visible = true;
                    qdAvisoDadosBancarios.TipoQuadro = TipoQuadroAviso.Aviso;
                    qdAvisoDadosBancarios.Mensagem = "Não há bandeiras contratadas para este estabelecimento";
                }
            }
        }

        /// <summary>
        /// Efetua o preenchimento dos dados na renderização de Conta encontrada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptTabelaCredito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    TabelaCredito tblCredito = (TabelaCredito)e.Item.FindControl("tblCredito");

                    if (tblCredito != null)
                    {
                        DataSet tabelas = new DataSet();
                        DataSet dadosBancarios = (DataSet)e.Item.DataItem;
                        DataTable tblBanco = dadosBancarios.Tables["Banco"];
                        DataTable tblPagamentos = dadosBancarios.Tables["ProdutosPagamentos"];
                        DataTable tblProdutosFlex = dadosBancarios.Tables["ProdutosFlex"];

                        tabelas.Tables.Add(tblPagamentos.Copy());
                        tabelas.Tables.Add(tblProdutosFlex.Copy());

                        String descricaoCartao = tblBanco.Rows[0]["DescricaoCartao"] != DBNull.Value ? tblBanco.Rows[0]["DescricaoCartao"].ToString() : String.Empty;
                        String banco = tblBanco.Rows[0]["NomeBanco"] != DBNull.Value ? tblBanco.Rows[0]["NomeBanco"].ToString() : String.Empty;
                        String agencia = tblBanco.Rows[0]["NomeAgencia"] != DBNull.Value ? tblBanco.Rows[0]["NomeAgencia"].ToString() : String.Empty;
                        String conta = tblBanco.Rows[0]["ContaAtualizada"] != DBNull.Value ? tblBanco.Rows[0]["ContaAtualizada"].ToString() : String.Empty;

                        tblCredito.CarregarControle(descricaoCartao, banco, agencia, conta, tabelas);

                        tblCredito.Visible = true;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Logger.GravarErro("Erro durante bind de dados da conta cadastrada", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante bind de dados da conta cadastrada", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
            }
        }

        /// <summary>
        /// Organiza os itens de Crédito em um novo DataSet para preenchimento das 
        /// Tabelas Pagamento e Produtos Flex do UserControl de Crédito
        /// </summary>
        /// <param name="dadosBancarios">Lista de Dados Bancários Crédito</param>        
        /// <param name="produtosFlex">Lista de Produtos Flex</param>
        /// <returns>Novo DataSet para preenchimento das Tabelas Pagamento e Produtos Flex</returns>
        private List<DataSet> MontarItensCreditoOld(EntidadeServico.DadosBancarios[] dadosBancarios, EntidadeServico.ProdutoFlex[] produtosFlex)
        {
            var dadosBancariosLista = dadosBancarios.OrderBy(x =>
            {
                /* 
                 * JFR: define a ordenação das bandeiras, sendo:
                 * - 1º master
                 * - 2º visa
                 * - 3º demais */

                Int32 order = 2;

                if (x.DescricaoCartao.ToLower().Contains("master"))
                    order = 0;

                else if (x.DescricaoCartao.ToLower().Contains("visa"))
                    order = 1;

                return order;

            }).ToList();
            
            // SKU: Nao deve conter as bandeiras AMEX(69)
            // JFR: não deve conter bandeiras internacionais e outras
            dadosBancariosLista.RemoveAll(x => listBandeirasRemover.Any(b => String.Compare(x.CodigoCartao, b.ToString()) == 0));

            dadosBancarios = dadosBancariosLista.ToArray();

            if (produtosFlex == null)
                produtosFlex = new EntidadeServico.ProdutoFlex[0];
            else if (produtosFlex.Length > 0)
            {
                //Filtra os Produtos Flex, removendo os cancelados (Situação = "C")
                produtosFlex = produtosFlex
                    .Where(produtoFlex => "C".CompareTo(produtoFlex.IndicadorSituacaoRegistro) != 0).ToArray();
            }

            Int32 codigoCartao = 0;

            String descricaoCartao = String.Empty;
            String nomeBanco = String.Empty;
            String codigoAgencia = String.Empty;
            String contaAtualizada = String.Empty;

            List<DataSet> listaTabelasCredito = new List<DataSet>();
            CultureInfo ptBR = new CultureInfo("pt-BR");

            DataSet tabelas = new DataSet();
            DataRow linhaTabela;
            DataTable tblPagamentos = new DataTable("ProdutosPagamentos");
            DataTable tblProdutosFlex = new DataTable("ProdutosFlex");
            DataTable tblBanco = new DataTable("Banco");

            tblBanco.Columns.Add("DescricaoCartao");
            tblBanco.Columns.Add("NomeBanco");
            tblBanco.Columns.Add("NomeAgencia");
            tblBanco.Columns.Add("ContaAtualizada");

            tblPagamentos.Columns.Add("TipoVenda");
            tblPagamentos.Columns.Add("Parcelas");
            tblPagamentos.Columns.Add("Prazo");
            tblPagamentos.Columns.Add("Taxa");
            tblPagamentos.Columns.Add("Tarifa");

            tblProdutosFlex.Columns.Add("RecebimentoAntecipado");
            tblProdutosFlex.Columns.Add("Parcelas");
            tblProdutosFlex.Columns.Add("Fator1");
            tblProdutosFlex.Columns.Add("Fator2");
            tblProdutosFlex.Columns.Add("Prazo");
            tblProdutosFlex.Columns.Add("Taxa");

            if (dadosBancarios != null)
                codigoCartao = dadosBancarios[0].CodigoCartao.ToInt32();

            foreach (EntidadeServico.DadosBancarios dado in dadosBancarios)
            {
                if (!dado.CodigoCartao.ToInt32().Equals(61) && !dado.CodigoFEAT.ToInt32().Equals(88))
                {
                    if (!codigoCartao.Equals(dado.CodigoCartao.ToInt32()))
                    {
                        tabelas.Tables.Clear();
                        tabelas.Tables.Add(tblPagamentos);
                        tabelas.Tables.Add(tblProdutosFlex);

                        tblBanco.Rows.Clear();
                        linhaTabela = tblBanco.NewRow();
                        linhaTabela["DescricaoCartao"] = descricaoCartao;
                        linhaTabela["NomeBanco"] = nomeBanco;
                        linhaTabela["NomeAgencia"] = codigoAgencia;
                        linhaTabela["ContaAtualizada"] = contaAtualizada;
                        tblBanco.Rows.Add(linhaTabela);
                        tabelas.Tables.Add(tblBanco);

                        //Monta registros para tabela de produtos flex, filtrando pelo código do cartão
                        EntidadeServico.ProdutoFlex[] produtosFlexCartaoFeat =
                            produtosFlex.Where(flex => flex.CodigoCCA == codigoCartao).ToArray();

                        foreach (EntidadeServico.ProdutoFlex flex in produtosFlexCartaoFeat)
                        {
                            linhaTabela = tblProdutosFlex.NewRow();
                            linhaTabela["RecebimentoAntecipado"] = flex.DescricaoFeature;
                            linhaTabela["Parcelas"] = String.Format("{0} a {1}", flex.CodigoPatamarInicio, flex.CodigoPatamarFim);
                            linhaTabela["Fator1"] = String.Format(ptBR, "{0:N2}%", flex.ValorPrecoVariante1);
                            linhaTabela["Fator2"] = String.Format(ptBR, "{0:N2}%", flex.ValorPrecoVariante2);
                            linhaTabela["Prazo"] = String.Format("{0} dia{1}", flex.QuantidadePrazoProduto, flex.QuantidadePrazoProduto > 1 ? "s" : String.Empty);

                            //Obtém o objeto DadosBancarios para o produto flex atual
                            EntidadeServico.DadosBancarios dadosFlex = dadosBancarios.FirstOrDefault(d =>
                                d.CodigoFEAT.ToInt32(-1) == flex.CodigoFeature && d.CodigoCartao.ToInt32(-1) == flex.CodigoCCA && d.MaximoParcelas == flex.CodigoPatamarFim);

                            //Cálculo da taxa
                            linhaTabela["Taxa"] = String.Format(ptBR, "{0:N2}%",
                                CalcularFlex(flex.ValorPrecoVariante1, flex.ValorPrecoVariante2, flex.CodigoPatamarFim,
                                dadosFlex != null ? dadosFlex.Taxa : 0));

                            tblProdutosFlex.Rows.Add(linhaTabela);
                        }

                        listaTabelasCredito.Add(tabelas);

                        tabelas = new DataSet();
                        tblPagamentos = new DataTable("ProdutosPagamentos");
                        tblProdutosFlex = new DataTable("ProdutosFlex");
                        tblBanco = new DataTable("Banco");

                        tblBanco.Columns.Add("DescricaoCartao");
                        tblBanco.Columns.Add("NomeBanco");
                        tblBanco.Columns.Add("NomeAgencia");
                        tblBanco.Columns.Add("ContaAtualizada");

                        tblPagamentos.Columns.Add("TipoVenda");
                        tblPagamentos.Columns.Add("Parcelas");
                        tblPagamentos.Columns.Add("Prazo");
                        tblPagamentos.Columns.Add("Taxa");
                        tblPagamentos.Columns.Add("Tarifa");

                        tblProdutosFlex.Columns.Add("RecebimentoAntecipado");
                        tblProdutosFlex.Columns.Add("Parcelas");
                        tblProdutosFlex.Columns.Add("Fator1");
                        tblProdutosFlex.Columns.Add("Fator2");
                        tblProdutosFlex.Columns.Add("Prazo");
                        tblProdutosFlex.Columns.Add("Taxa");
                    }

                    descricaoCartao = dado.DescricaoCartao;
                    nomeBanco = dado.NomeBanco;
                    codigoAgencia = dado.CodigoAgencia;
                    contaAtualizada = dado.ContaAtualizada;

                    //Monta registro para tabela de Pagamentos
                    linhaTabela = tblPagamentos.NewRow();
                    linhaTabela["TipoVenda"] = dado.DescricaoFEAT.ToUpper();
                    linhaTabela["Prazo"] = String.Format("{0} dias", dado.TemTarifa ? dado.PercentualTarifa : dado.PercentualTaxa);
                    linhaTabela["Taxa"] = dado.Taxa;
                    linhaTabela["Tarifa"] = dado.Tarifa;

                    if (String.IsNullOrEmpty(dado.NumeroLimite.Trim()))
                        linhaTabela["Parcelas"] = "-";
                    else if (dado.MinimoParcelas == dado.MaximoParcelas)
                    {
                        if (dado.MinimoParcelas == 1)
                            linhaTabela["Parcelas"] = String.Format("{0} Parcela", dado.MinimoParcelas);
                        else
                            linhaTabela["Parcelas"] = String.Format("{0} Parcelas", dado.MinimoParcelas);
                    }
                    else
                        linhaTabela["Parcelas"] = String.Format("De {0} a {1} Parcelas", dado.MinimoParcelas, dado.MaximoParcelas);

                    tblPagamentos.Rows.Add(linhaTabela);

                    codigoCartao = dado.CodigoCartao.ToInt32();
                }
            }

            tabelas.Tables.Clear();
            tabelas.Tables.Add(tblPagamentos);
            tabelas.Tables.Add(tblProdutosFlex);

            tblBanco.Rows.Clear();
            linhaTabela = tblBanco.NewRow();
            linhaTabela["DescricaoCartao"] = descricaoCartao;
            linhaTabela["NomeBanco"] = nomeBanco;
            linhaTabela["NomeAgencia"] = codigoAgencia;
            linhaTabela["ContaAtualizada"] = contaAtualizada;
            tblBanco.Rows.Add(linhaTabela);
            tabelas.Tables.Add(tblBanco);

            listaTabelasCredito.Add(tabelas);

            return listaTabelasCredito;
        }

        private List<Business.Bandeira> MontarItensCredito(EntidadeServico.DadosBancarios[] dadosBancarios, EntidadeServico.ProdutoFlex[] produtosFlex)
        {
            var dadosBancariosLista = dadosBancarios.OrderBy(x =>
            {
                /* 
                 * JFR: define a ordenação das bandeiras, sendo:
                 * - 1º master
                 * - 2º visa
                 * - 3º demais */

                Int32 order = 2;

                if (x.DescricaoCartao.ToLower().Contains("master"))
                    order = 0;

                else if (x.DescricaoCartao.ToLower().Contains("visa"))
                    order = 1;

                return order;

            }).ToList();

            // SKU: Nao deve conter as bandeiras AMEX(69)
            // JFR: não deve conter bandeiras internacionais e outras
            dadosBancariosLista.RemoveAll(x => listBandeirasRemover.Any(b => String.Compare(x.CodigoCartao, b.ToString()) == 0));

            dadosBancarios = dadosBancariosLista.ToArray();

            if (produtosFlex == null)
                produtosFlex = new EntidadeServico.ProdutoFlex[0];
            else if (produtosFlex.Length > 0)
            {
                //Filtra os Produtos Flex, removendo os cancelados (Situação = "C")
                produtosFlex = produtosFlex
                    .Where(produtoFlex => "C".CompareTo(produtoFlex.IndicadorSituacaoRegistro) != 0).ToArray();
            }

            List<Business.Bandeira> bandeiras = new List<Business.Bandeira>();
            CultureInfo ptBR = new CultureInfo("pt-BR");

            Business.Bandeira bandeira = null;
            foreach (EntidadeServico.DadosBancarios dado in dadosBancarios)
            {
                if (!dado.CodigoCartao.ToInt32().Equals(61) && !dado.CodigoFEAT.ToInt32().Equals(88))
                {
                    // se é a primeira vez ou se ja nao é a sequencia da mesma bandeira
                    if ((bandeira == null) || (bandeira.Codigo != dado.CodigoCartao.ToInt32()))
                    {
                        if (bandeira != null)
                            bandeiras.Add(bandeira);

                        bandeira = new Business.Bandeira();
                        bandeira.Taxas = new List<Taxa>();
                        bandeira.Flex = new List<Flex>();
                        bandeira.Codigo = dado.CodigoCartao.ToInt32();

                        //Monta registros para tabela de produtos flex, filtrando pelo código do cartão
                        foreach (EntidadeServico.ProdutoFlex flex in produtosFlex.Where(flex => flex.CodigoCCA == bandeira.Codigo))
                        {
                            Flex itemFlex = new Flex();
                            itemFlex.RecebimentoAntecipado = flex.DescricaoFeature.ToLower();
                            itemFlex.Parcelas = String.Format("{0} a {1}", flex.CodigoPatamarInicio, flex.CodigoPatamarFim);
                            itemFlex.Fator1 = String.Format(ptBR, "{0:N2}", flex.ValorPrecoVariante1);
                            itemFlex.Fator2 = String.Format(ptBR, "{0:N2}", flex.ValorPrecoVariante2);
                            itemFlex.Prazo = flex.QuantidadePrazoProduto;

                            //Obtém o objeto DadosBancarios para o produto flex atual
                            EntidadeServico.DadosBancarios dadosFlex = dadosBancarios.FirstOrDefault(d =>
                                d.CodigoFEAT.ToInt32(-1) == flex.CodigoFeature && d.CodigoCartao.ToInt32(-1) == flex.CodigoCCA && d.MaximoParcelas == flex.CodigoPatamarFim);

                            //Cálculo da taxa
                            itemFlex.Taxa = String.Format(ptBR, "{0:N2}",
                                CalcularFlex(flex.ValorPrecoVariante1, flex.ValorPrecoVariante2, flex.CodigoPatamarFim,
                                dadosFlex != null ? dadosFlex.Taxa : 0));

                            bandeira.Flex.Add(itemFlex);
                        }
                    }

                    bandeira.Nome = dado.DescricaoCartao;
                    bandeira.NomeBanco = dado.NomeBanco;
                    bandeira.CodigoAgencia = dado.CodigoAgencia;
                    bandeira.ContaAtualizada = dado.ContaAtualizada;

                    //Monta registro para tabela de Pagamentos
                    Taxa taxa = new Taxa();
                    taxa.ModalidadeVenda = dado.DescricaoFEAT.ToLower();
                    taxa.ValorTaxa = dado.Taxa;
                    taxa.Tarifa = dado.Tarifa;
                    taxa.Prazo = String.Format("{0} dias", dado.TemTarifa ? dado.PercentualTarifa : dado.PercentualTaxa);

                    if (String.IsNullOrEmpty(dado.NumeroLimite.Trim()))
                        taxa.Parcelas = "-";
                    else if (dado.MinimoParcelas == dado.MaximoParcelas)
                    {
                        if (dado.MinimoParcelas == 1)
                            taxa.Parcelas = String.Format("{0} Parcela", dado.MinimoParcelas);
                        else
                            taxa.Parcelas = String.Format("{0} Parcelas", dado.MinimoParcelas);
                    }
                    else
                        taxa.Parcelas = String.Format("De {0} a {1} Parcelas", dado.MinimoParcelas, dado.MaximoParcelas);
                    bandeira.Taxas.Add(taxa);
                }
            }

            // adiciona a ultima bandeira
            if (bandeira != null)
                bandeiras.Add(bandeira);

            return bandeiras;
        }

        /// <summary>
        /// Cálculo Flex: Fator1 + Fator2 (N-1), onde N é o número de parcelas,
        /// considerando ainda que a taxa final é %MDR + %Flex.
        /// </summary>
        /// <param name="fator1">Fator 1</param>
        /// <param name="fator2">Fator 2</param>
        /// <param name="numeroParcelas">Número de Parcelas</param>
        /// <param name="taxaMDR">Taxa MDR</param>
        /// <returns>Taxa final (FLEX + MDR)</returns>
        private Decimal CalcularFlex(Decimal fator1, Decimal fator2, Int32 numeroParcelas, Decimal taxaMDR)
        {
            var flex = fator1 + fator2 * (numeroParcelas - 1);
            var taxaFinal = flex + taxaMDR;
            return taxaFinal;
        }

        protected void grvTabelaCreditoTaxas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Business.Bandeira bandeira = (Business.Bandeira)e.Row.DataItem;

                Image imgBandeira = (Image)e.Row.FindControl("imgBandeira");
                Label lblBandeira = (Label)e.Row.FindControl("lblBandeira");

                Repeater rptPrazo = (Repeater)e.Row.FindControl("rptPrazo");
                Repeater rptTipoVenda = (Repeater)e.Row.FindControl("rptTipoVenda");
                Repeater rptParcelas = (Repeater)e.Row.FindControl("rptParcelas");
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

                rptParcelas.DataSource = bandeira.Taxas.Select(t => t.Parcelas);
                rptParcelas.DataBind();

                rptTaxas.DataSource = bandeira.Taxas.Select(t => t.ValorTaxa);
                rptTaxas.DataBind();

                rptTarifas.DataSource = bandeira.Taxas.Select(t => t.Tarifa);
                rptTarifas.DataBind();
            }
        }

        protected void grvTabelaCreditoFlex_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Business.Bandeira bandeira = (Business.Bandeira)e.Row.DataItem;

                Image imgBandeira = (Image)e.Row.FindControl("imgBandeira");
                Label lblBandeira = (Label)e.Row.FindControl("lblBandeira");

                Repeater rptPrazo = (Repeater)e.Row.FindControl("rptPrazo");
                Repeater rptTipoVenda = (Repeater)e.Row.FindControl("rptTipoVenda");
                Repeater rptParcelas = (Repeater)e.Row.FindControl("rptParcelas");
                Repeater rptTaxas = (Repeater)e.Row.FindControl("rptTaxas");
                Repeater rptFator1 = (Repeater)e.Row.FindControl("rptFator1");
                Repeater rptFator2 = (Repeater)e.Row.FindControl("rptFator2");

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

                rptPrazo.DataSource = bandeira.Flex.Select(t => t.Prazo);
                rptPrazo.DataBind();

                rptTipoVenda.DataSource = bandeira.Flex.Select(t => t.RecebimentoAntecipado);
                rptTipoVenda.DataBind();

                rptParcelas.DataSource = bandeira.Flex.Select(t => t.Parcelas);
                rptParcelas.DataBind();

                rptTaxas.DataSource = bandeira.Flex.Select(t => t.Taxa);
                rptTaxas.DataBind();

                rptFator1.DataSource = bandeira.Flex.Select(t => t.Fator1);
                rptFator1.DataBind();

                rptFator2.DataSource = bandeira.Flex.Select(t => t.Fator2);
                rptFator2.DataBind();
            }
        }
    }
}