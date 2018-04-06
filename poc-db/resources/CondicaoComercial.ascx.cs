using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Rede.PN.Credenciamento.Sharepoint.Servicos;
using System.Linq;
using System.Data;
using System.Text;
using System.Globalization;

namespace Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento
{
    public partial class CondicaoComercial : UserControlCredenciamentoBase
    {
        /// <summary>
        /// Valor equipamento que aparece na tela
        /// </summary>
        public Double ValorEquipamentoTela
        {
            get
            {
                if (Session["ValorEquipamentoTela"] == null)
                    Session["ValorEquipamentoTela"] = new Double();

                return (Double)Session["ValorEquipamentoTela"];
            }
            set
            {
                Session["ValorEquipamentoTela"] = value;
            }
        }

        /// <summary>
        /// Listagem de pacote de serviços
        /// </summary>
        public List<Modelo.Servico> PacoteServicos
        {
            get
            {
                if (ViewState["PacoteServicos"] == null)
                    ViewState["PacoteServicos"] = new List<Modelo.Servico>();

                return (List<Modelo.Servico>)ViewState["PacoteServicos"];
            }
            set
            {
                ViewState["PacoteServicos"] = value;
            }
        }

        /// <summary>
        /// Listagem de Produtos
        /// </summary>
        public List<Modelo.Produto> Produtos
        {
            get
            {
                if (ViewState["Produtos"] == null)
                    ViewState["Produtos"] = new List<Modelo.Produto>();

                return (List<Modelo.Produto>)ViewState["Produtos"];
            }
            set
            {
                ViewState["Produtos"] = value;
            }
        }

        #region [Page Events]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
            }
        }

        #endregion

        #region [Metodos Auxiliares]

        /// <summary>
        /// Verifica se o grupo ramo e ramo atividade podem possuir produtos parceiro
        /// </summary>
        /// <returns></returns>
        private List<Modelo.RamoAtividadeParceiro> VerificarProdutosParceirosPorRamo()
        {
            List<Modelo.RamoAtividadeParceiro> produtos = new List<Modelo.RamoAtividadeParceiro>();

            try
            {
                produtos = ServicosGE.ValidaRamoAtividadeParceiro(
                    Credenciamento.Proposta.CodigoGrupoRamo.Value,
                    Credenciamento.Proposta.CodigoRamoAtividade.Value,
                    15).ConvertAll(p => new Modelo.RamoAtividadeParceiro
                        {
                            CodigoCca = p.CodigoCca,
                            CodigoFeature = p.CodigoFeature,
                            CodigoGrupoRamo = p.CodigoGrupoRamo,
                            CodigoRamoAtividade = p.CodigoRamoAtividade,
                            NomeRamoAtividadeProdutoParceiro = p.NomeRamoAtividadeProdutoParceiro
                        });
            }

            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecaoAsync(FONTE, CODIGO_ERRO, upCondicaoComercial);
            }

            return produtos;
        }

        /// <summary>
        /// Carrega os itens da Alelo na listagem de produtos VAN
        /// </summary>
        /// <param name="tipoEquipamento"></paramTipoEquipamentoSelecionado
        private void CarregarProdutosVendasAlelo(String tipoEquipamento)
        {
            if (String.Compare(tipoEquipamento.ToUpper(), "POO") == 0 || String.Compare(tipoEquipamento.ToUpper(), "POS") == 0)
            {
                List<Modelo.RamoAtividadeParceiro> produtos = VerificarProdutosParceirosPorRamo();

                if (produtos.Count > 0)
                {
                    Modelo.RamoAtividadeParceiro aleloRefeicao = produtos.FirstOrDefault(p => p.CodigoFeature == 128);

                    if (!ReferenceEquals(aleloRefeicao, null))
                    {
                        cbHabilitarVendasAleloRefeicaoNaoRede.Visible = true;
                        hdnCodFeatureAleloRefeicao.Value = aleloRefeicao.CodigoFeature.ToString();
                    }

                    Modelo.RamoAtividadeParceiro aleloAlimentacao = produtos.FirstOrDefault(p => p.CodigoFeature == 129);

                    if (!ReferenceEquals(aleloAlimentacao, null))
                    {
                        cbHabilitarVendasAleloAlimentacaoNaoRede.Visible = true;
                        hdnCodFeatureAleloAlimentacao.Value = aleloAlimentacao.CodigoFeature.ToString();
                    }

                    Credenciamento.Proposta.ListaRamoAtividadeParceiro = produtos;
                }
            }
            else
                ResetarPaineisAlelo();
        }

        /// <summary>
        /// Reicinia o Formulário PAT
        /// </summary>
        private void ResetFormularioAleloAlimentacao()
        {
            txtAreaLoja.Text =
            txtQtdCheckout.Text = "0";
        }

        /// <summary>
        /// Reicinia o Formulário PAT
        /// </summary>
        private void ResetFormularioAleloRefeicao()
        {
            txtNumMaximoRefeicoes.Text =
            txtAreaAtendimento.Text =
            txtNumMesas.Text =
            txtNumAssentos.Text = "0";
        }

        /// <summary>
        /// Carrega componentes iniciais
        /// </summary>
        public void CarregarCamposIniciais()
        {
            try
            {
                ((ResumoProposta)resumoProposta).CarregaResumoProposta();
                CarregarListaEquipamentos();
                CarregarListaAcaoComercial();
                CarregarListaEventos();
                CarregarTaxaFiliacao();
                CarregarSoftwareTEF();
                CarregarFabricanteHardware();
                CarregarProdutosVAN();
                CarregarDadosBancarios();
                CarregarCondicoesComerciais();
                this.VisibilidadePaineisCondicoesComerciais(false, false, false, false);
                this.VisibilidadeCamposNaoCondicoesComerciais(true);
                this.ResetarPaineisAlelo();
                CarregarEscalonamentoValorDefault();

            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Recarrega componentes iniciais
        /// </summary>
        private void ReiniciaCamposIniciais()
        {
            try
            {
                CarregarListaEquipamentos();
                CarregarListaAcaoComercial();
                CarregarListaEventos();
                CarregarTaxaFiliacao();
                CarregarSoftwareTEF();
                CarregarFabricanteHardware();
                CarregarProdutosVAN();
                CarregarDadosBancarios();
                this.VisibilidadePaineisCondicoesComerciais(false, false, false, false);
                this.VisibilidadeCamposNaoCondicoesComerciais(true);
                this.ResetarPaineisAlelo();
                CarregarEscalonamentoValorDefault();
                mvTiposFormularios.SetActiveView(vNaoRede);

            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Carrega valores default na tabela de escalonamento
        /// </summary>
        private void CarregarEscalonamentoValorDefault()
        {
            lblDescontoJaneiroNaoRede.Text = "0%";
            lblPrecoJaneiroNaoRede.Text = "0,00";

            lblDescontoFevereiroNaoRede.Text = "0%";
            lblPrecoFevereiroNaoRede.Text = "0,00";

            lblDescontoMarcoNaoRede.Text = "0%";
            lblPrecoMarcoNaoRede.Text = "0,00";

            lblDescontoAbrilNaoRede.Text = "0%";
            lblPrecoAbrilNaoRede.Text = "0,00";

            lblDescontoMaioNaoRede.Text = "0%";
            lblPrecoMaioNaoRede.Text = "0,00";

            lblDescontoJunhoNaoRede.Text = "0%";
            lblPrecoJunhoNaoRede.Text = "0,00";

            lblDescontoJulhoNaoRede.Text = "0%";
            lblPrecoJulhoNaoRede.Text = "0,00";

            lblDescontoAgostoNaoRede.Text = "0%";
            lblPrecoAgostoNaoRede.Text = "0,00";

            lblDescontoSetembroNaoRede.Text = "0%";
            lblPrecoSetembroNaoRede.Text = "0,00";

            lblDescontoOutubroNaoRede.Text = "0%";
            lblPrecoOutubroNaoRede.Text = "0,00";

            lblDescontoNovembroNaoRede.Text = "0%";
            lblPrecoNovembroNaoRede.Text = "0,00";

            lblDescontoDezembroNaoRede.Text = "0%";
            lblPrecoDezembroNaoRede.Text = "0,00";

            lblDescontoJaneiroRede.Text = "0%";
            lblPrecoJaneiroRede.Text = "0,00";

            lblDescontoFevereiroRede.Text = "0%";
            lblPrecoFevereiroRede.Text = "0,00";

            lblDescontoMarcoRede.Text = "0%";
            lblPrecoMarcoRede.Text = "0,00";

            lblDescontoAbrilRede.Text = "0%";
            lblPrecoAbrilRede.Text = "0,00";

            lblDescontoMaioRede.Text = "0%";
            lblPrecoMaioRede.Text = "0,00";

            lblDescontoJunhoRede.Text = "0%";
            lblPrecoJunhoRede.Text = "0,00";

            lblDescontoJulhoRede.Text = "0%";
            lblPrecoJulhoRede.Text = "0,00";

            lblDescontoAgostoRede.Text = "0%";
            lblPrecoAgostoRede.Text = "0,00";

            lblDescontoSetembroRede.Text = "0%";
            lblPrecoSetembroRede.Text = "0,00";

            lblDescontoOutubroRede.Text = "0%";
            lblPrecoOutubroRede.Text = "0,00";

            lblDescontoNovembroRede.Text = "0%";
            lblPrecoNovembroRede.Text = "0,00";

            lblDescontoDezembroRede.Text = "0%";
            lblPrecoDezembroRede.Text = "0,00";
        }

        /// <summary>
        /// Remove caracteres especiais de campos textos
        /// </summary>
        /// <param name="valor">valor da string a ser removida os caracteres especiais</param>
        /// <returns>retorna string sem os caracteres especiais de exibição de tela</returns>
        private string RemoverCaracteresEspeciais(string valor)
        {
            valor = valor.Replace("dia(s)", String.Empty);
            valor = valor.Replace("%", String.Empty);
            valor = valor.Replace("R$", String.Empty);

            return valor;
        }

        /// <summary>
        /// Carrega o formulario coerente ao tipo de equipamento registrado na sessão
        /// </summary>
        private void CarregarTipoFormulario(String tipoEquipamento)
        {
            mvTiposFormularios.SetActiveView(vNaoRede);
            if (String.Compare(tipoEquipamento, "TOL") == 0 || String.Compare(tipoEquipamento, "SNT") == 0 || String.Compare(tipoEquipamento, "TOF") == 0)
            {
                mvTiposFormularios.SetActiveView(vRede);
                ddlTipoEquipamentoRede.SelectedValue = tipoEquipamento;
            }
            else if (String.Compare(tipoEquipamento, "TOL") != 0 && String.Compare(tipoEquipamento, "SNT") != 0 && String.Compare(tipoEquipamento, "TOF") != 0)
            {
                mvTiposFormularios.SetActiveView(vNaoRede);
                ddlTipoEquipamentoNaoRede.SelectedValue = tipoEquipamento;
            }
        }

        /// <summary>
        /// Carrega lista de Condições Comerciais (Ofertas Padrão)
        /// </summary>
        private void CarregarCondicoesComerciais()
        {
            try
            {
                ddlCondicoesComerciais.Items.Clear();
                ddlCondicoesComerciaisNaoRede.Items.Clear();

                Int32 quantidadeRegistros = 0;
                //Consulta no banco do WF
                var listaParametrizacaoOfertas = ServicosWF.ConsultaParametrizacaoOfertas(null,
                                                                                          null,
                                                                                          Credenciamento.Proposta.CodigoTipoPessoa,
                                                                                          Credenciamento.Proposta.NumeroCnpjCpf,
                                                                                          Credenciamento.Proposta.IndicadorSequenciaProposta,
                                                                                          Credenciamento.Proposta.CodigoTipoEstabelecimento,
                                                                                          Convert.ToInt16(Credenciamento.Proposta.CodigoCanal ?? 0),
                                                                                          Credenciamento.Proposta.CodigoCelula,
                                                                                          Credenciamento.Proposta.CodigoGrupoRamo,
                                                                                          Credenciamento.Proposta.CodigoRamoAtividade,
                                                                                          Credenciamento.Enderecos.FirstOrDefault().Estado,
                                                                                          'A',
                                                                                          null,
                                                                                          SessaoAtual.Funcional,
                                                                                          null,
                                                                                          null,
                                                                                          0,
                                                                                          Int32.MaxValue,
                                                                                          out quantidadeRegistros);

                //Bind da combo de Condições Comerciais
                listaParametrizacaoOfertas.ForEach(c =>
                {
                    ddlCondicoesComerciais.Items.Add(new ListItem(String.Format("{0} - {1}", c.CodigoOfertaPadrao.ToString(), c.DescricaoOferta), c.CodigoOfertaPadrao.ToString()));
                    ddlCondicoesComerciaisNaoRede.Items.Add(new ListItem(String.Format("{0} - {1}", c.CodigoOfertaPadrao.ToString(), c.DescricaoOferta), c.CodigoOfertaPadrao.ToString()));
                });

                ddlCondicoesComerciais.Items.Insert(0, new ListItem("Condição Padrão", "0"));
                ddlCondicoesComerciaisNaoRede.Items.Insert(0, new ListItem("Condição Padrão", "0"));
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Carrega controle de lista equipamentos
        /// </summary>
        private void CarregarListaEquipamentos()
        {
            try
            {
                Int32 codGrupoRamo = 0;
                Int32 codRamoAtividade = 0;

                if (Credenciamento.Proposta != null)
                    if (Credenciamento.Proposta.CodigoGrupoRamo != null)
                        codGrupoRamo = Credenciamento.Proposta.CodigoGrupoRamo.Value;

                if (Credenciamento.Proposta != null)
                    if (Credenciamento.Proposta.CodigoRamoAtividade != null)
                        codRamoAtividade = Credenciamento.Proposta.CodigoRamoAtividade.Value;

                ddlTipoEquipamentoNaoRede.Items.Clear();
                ddlTipoEquipamentoRede.Items.Clear();

                ServicosGE.ConsultaEquipamentosPorRamoAtividade(codGrupoRamo, codRamoAtividade).ForEach(c =>
                {
                    ddlTipoEquipamentoNaoRede.Items.Add(new ListItem(c.CodTipoEquipamento, c.CodTipoEquipamento.ToString()));
                    ddlTipoEquipamentoRede.Items.Add(new ListItem(c.CodTipoEquipamento, c.CodTipoEquipamento.ToString()));
                });

                ddlTipoEquipamentoNaoRede.Items.Insert(0, new ListItem("", ""));
                ddlTipoEquipamentoRede.Items.Insert(0, new ListItem("", ""));
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }

        }

        /// <summary>
        /// Carrega controle de lista Comercial
        /// </summary>
        private void CarregarListaAcaoComercial()
        {
            try
            {
                Int32? codAcao = null;
                Char statusAcao = 'A';

                ddlAcaoComercialRede.Items.Clear();
                ddlAcaoComercialNaoRede.Items.Clear();
                ServicosTG.ConsultaAcaoComercial(codAcao, statusAcao).ForEach(c =>
                {
                    ddlAcaoComercialNaoRede.Items.Add(new ListItem((String.Format(@"{0} - {1}", c.CodAcaoComercial.ToString(), c.DescAcaoComercial.ToString())), c.CodAcaoComercial.ToString()));
                    ddlAcaoComercialRede.Items.Add(new ListItem((String.Format(@"{0} - {1}", c.CodAcaoComercial.ToString(), c.DescAcaoComercial.ToString())), c.CodAcaoComercial.ToString()));
                });

                ddlAcaoComercialRede.Items.Insert(0, new ListItem("", ""));
                ddlAcaoComercialNaoRede.Items.Insert(0, new ListItem("", ""));
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }

        }

        /// <summary>
        /// Carrega controle de lista Eventos Especiais
        /// </summary>
        private void CarregarListaEventos()
        {
            try
            {
                string codEvEspecial = null;
                Char? statusEvento = 'A';
                Char? codIndicadorAcaoFCT = null;

                ddlEventoNaoRede.Items.Clear();
                ddlEventoRede.Items.Clear();
                ServicosTG.ConsultaEventosEspeciais(codEvEspecial, statusEvento, codIndicadorAcaoFCT).ForEach(c =>
                {
                    ddlEventoNaoRede.Items.Add(new ListItem(String.Format(@"{0} - {1}", c.CodEventoEspecial.ToString(), c.NomeEventoEspecial.ToString()), c.CodEventoEspecial.ToString()));
                    ddlEventoRede.Items.Add(new ListItem(String.Format(@"{0} - {1}", c.CodEventoEspecial.ToString(), c.NomeEventoEspecial.ToString()), c.CodEventoEspecial.ToString()));
                });

                ddlEventoNaoRede.Items.Insert(0, new ListItem("", ""));
                ddlEventoRede.Items.Insert(0, new ListItem("", ""));
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }

        }

        /// <summary>
        /// Carrega controle de lista Campanhas
        /// </summary>
        private void CarregarListaCampanhas(String tipoEquipamento)
        {
            try
            {
                List<WFCampanhas.ListaCampanhaPorCanalCelulaRamoCep> campanhas = new List<WFCampanhas.ListaCampanhaPorCanalCelulaRamoCep>();
                ddlCampanhaNaoRede.Items.Clear();
                ddlCampanhaRede.Items.Clear();

                if (!String.IsNullOrEmpty(tipoEquipamento))
                {
                    Char? codigoTipoCampanha = 'C';
                    Modelo.Endereco endereco = Credenciamento.Enderecos.FirstOrDefault();
                    String cep = String.Format(@"{0}{1}", endereco.CodigoCep, endereco.CodigoComplementoCep);


                    campanhas = ServicosWF.ConsultaCampanhaPorCanalCelulaRamoCep(Credenciamento.Proposta.CodigoCanal.Value,
                                                                     Credenciamento.Proposta.CodigoCelula.Value,
                                                                     Credenciamento.Proposta.CodigoGrupoRamo.Value,
                                                                     Credenciamento.Proposta.CodigoRamoAtividade.Value,
                                                                     cep,
                                                                     codigoTipoCampanha, null, tipoEquipamento);
                    campanhas.ForEach(c =>
                    {
                        ddlCampanhaNaoRede.Items.Add(new ListItem(String.Format(@"{0} - {1}", c.CodigoCampanha.ToString(), c.NomeCampanha.ToString()), c.CodigoCampanha.ToString()));
                        ddlCampanhaRede.Items.Add(new ListItem(String.Format(@"{0} - {1}", c.CodigoCampanha.ToString(), c.NomeCampanha.ToString()), c.CodigoCampanha.ToString()));
                    });
                }

                spExistemCampanhasNaoRede.Visible = campanhas.Count > 0;
                spExistemCampanhas.Visible = campanhas.Count > 0;

                ddlCampanhaNaoRede.Items.Insert(0, new ListItem("", ""));
                ddlCampanhaRede.Items.Insert(0, new ListItem("", ""));
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }

        }

        /// <summary>
        /// Carrega controle de lista Campanhas
        /// </summary>
        private void CarregarListaCenarios(String tipoEquipamento)
        {
            try
            {
                DropDownList ddlCampanha = mvTiposFormularios.ActiveViewIndex == 0 ? ddlCampanhaRede : ddlCampanhaNaoRede;

                ddlCenarioRede.Items.Clear();
                ddlCenarioNaoRede.Items.Clear();
                ServicosTG.ConsultaCenario(Credenciamento.Proposta.CodigoCanal.Value, tipoEquipamento, 'A', ddlCampanha.SelectedValue, null).ForEach(c =>
                {
                    String codigoCenario = c.CodigoCenario.ToString();
                    String descricaoCenario = c.DescricaoCenario.ToString();

                    if (c.CodigoCenario.HasValue)
                        codigoCenario = c.CodigoCenario.Value.ToString();

                    if (!String.IsNullOrEmpty(c.DescricaoCenario))
                        descricaoCenario = c.DescricaoCenario;

                    ddlCenarioRede.Items.Add(new ListItem(String.Format("{0} - {1}", codigoCenario, descricaoCenario), codigoCenario));
                    ddlCenarioNaoRede.Items.Add(new ListItem(String.Format("{0} - {1}", codigoCenario, descricaoCenario), codigoCenario));
                });

                ddlCenarioRede.Items.Insert(0, new ListItem("", ""));
                ddlCenarioNaoRede.Items.Insert(0, new ListItem("", ""));

            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Carrega taxa de filiação
        /// </summary>
        private void CarregarTaxaFiliacao()
        {
            try
            {
                GETaxaFiliacao.TaxaFiliacaoConsultaValorTaxaFiliacao taxaFiliacao = ServicosGE.ConsultaTaxaFiliacao();

                if (taxaFiliacao != null)
                {
                    txtTaxaAdesaoNaoRede.Text = String.Format("{0:f2}", taxaFiliacao.ValorParametro);
                    txtTaxaAdesaoRede.Text = String.Format("{0:f2}", taxaFiliacao.ValorParametro);
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        private bool TaxaFiliacaoIsenta()
        {
            try
            {
                Int32 quantidadeRegistros = 0;

                Int32 codigoOferta =
                    mvTiposFormularios.GetActiveView() == vNaoRede ?
                        Convert.ToInt32(ddlCondicoesComerciaisNaoRede.SelectedValue) :
                        Convert.ToInt32(ddlCondicoesComerciais.SelectedValue);

                //Consulta no banco do WF
                var listaParametrizacaoOfertas = ServicosWF.ConsultaParametrizacaoOfertas(null,
                                                                                          codigoOferta,
                                                                                          Credenciamento.Proposta.CodigoTipoPessoa,
                                                                                          Credenciamento.Proposta.NumeroCnpjCpf,
                                                                                          Credenciamento.Proposta.IndicadorSequenciaProposta,
                                                                                          Credenciamento.Proposta.CodigoTipoEstabelecimento,
                                                                                          Convert.ToInt16(Credenciamento.Proposta.CodigoCanal ?? 0),
                                                                                          Credenciamento.Proposta.CodigoCelula,
                                                                                          Credenciamento.Proposta.CodigoGrupoRamo,
                                                                                          Credenciamento.Proposta.CodigoRamoAtividade,
                                                                                          Credenciamento.Enderecos.FirstOrDefault().Estado,
                                                                                          'A',
                                                                                          null,
                                                                                          SessaoAtual.Funcional,
                                                                                          null,
                                                                                          null,
                                                                                          0,
                                                                                          Int32.MaxValue,
                                                                                          out quantidadeRegistros);

                if (listaParametrizacaoOfertas.Count > 0)
                    return listaParametrizacaoOfertas[0].IndicadorIsencaoTaxaAdesao.Equals('S');

            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
            return false;
        }

        /// <summary>
        /// Carrega taxa de filiação
        /// </summary>
        private void CarregarSoftwareTEF()
        {
            try
            {
                ddlSoftwareTEFNaoRede.Items.Clear();

                ServicosTG.ConsultaSoftwaresTEF(null, 'A').ForEach(c =>
                {
                    ddlSoftwareTEFNaoRede.Items.Add(new ListItem(String.Format(@"{0} - {1}", c.CodFornecedorSoftware.ToString(), c.NomeFornecedorSoftware.ToString()), c.CodFornecedorSoftware.ToString()));
                });

                ddlSoftwareTEFNaoRede.Items.Insert(0, new ListItem("", ""));
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Carregar Fabricante Hardware
        /// </summary>
        private void CarregarFabricanteHardware()
        {
            try
            {
                ddlMarcaPDVNaoRede.Items.Clear();

                TGFabHardware.FabricanteHardware[] fabricantes = ServicosTG.ConsultaFabricanteHardware(null, 'A');

                foreach (TGFabHardware.FabricanteHardware fabricante in fabricantes)
                    ddlMarcaPDVNaoRede.Items.Add(new ListItem(String.Format(@"{0} - {1}", fabricante.CodFabricanteHardware.ToString(), fabricante.NomeFabricanteHardware.ToString()), fabricante.CodFabricanteHardware.ToString()));

                ddlMarcaPDVNaoRede.Items.Insert(0, new ListItem("", ""));
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Carregar Serviços 
        /// </summary>
        private void CarregarServicos(String tipoEquipamento, List<Modelo.ParametrosCampanha> servicosCampanha)
        {
            try
            {
                Repeater rServico = mvTiposFormularios.GetActiveView() == vNaoRede ? rServicosNaoRede : rServicosRede;

                List<Modelo.Servico> servicos = ServicosWF.ConsultaServicosPacotes(Modelo.TipoServico.Servico, (Modelo.TipoPessoa)Credenciamento.Proposta.CodigoTipoPessoa,
                                                    Credenciamento.Proposta.CodigoGrupoRamo.Value, Credenciamento.Proposta.CodigoRamoAtividade.Value,
                                                    'N', Credenciamento.Proposta.CodigoCanal.Value, tipoEquipamento);


                if (servicos.Count() > 0)
                {
                    if (mvTiposFormularios.GetActiveView() == vRede)
                        phTodosServicosRede.Visible = true;
                    else
                        phTodosServicosNaoRede.Visible = true;

                    rServico.DataSource = servicos;
                    rServico.DataBind();
                }
                else
                {
                    if (mvTiposFormularios.GetActiveView() == vRede)
                        phTodosServicosRede.Visible = false;
                    else
                        phTodosServicosNaoRede.Visible = false;
                }

                CalcularTotalPacoteServico();
                //CarregarFooterServicos();
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Carregar Pacotes 
        /// </summary>
        private void CarregarPacotes(String tipoEquipamento)
        {
            try
            {
                if (mvTiposFormularios.GetActiveView() == vRede)
                {
                    List<Modelo.Servico> servicos = ServicosWF.ConsultaServicosPacotes(Modelo.TipoServico.Pacote, (Modelo.TipoPessoa)Credenciamento.Proposta.CodigoTipoPessoa,
                                                        Credenciamento.Proposta.CodigoGrupoRamo.Value, Credenciamento.Proposta.CodigoRamoAtividade.Value,
                                                        'N', Credenciamento.Proposta.CodigoCanal.Value, tipoEquipamento);

                    this.PacoteServicos = servicos;

                    ddlPacoteServicosRede.Items.Clear();

                    PacoteServicos.ForEach(c =>
                    {
                        ddlPacoteServicosRede.Items.Add(new ListItem(String.Format(@"{0} - {1}", c.CodigoServico, c.DescricaoServico), c.CodigoServico.ToString()));
                    });

                    ddlPacoteServicosRede.Items.Insert(0, new ListItem("", ""));
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Carrega GRID com informaões de produtos VAN
        /// </summary>
        private void CarregarProdutosVAN()
        {
            try
            {
                Int32 codigoGrupoRamoAtuacao = Credenciamento.Proposta.CodigoGrupoRamo.Value;
                Int32 codigoRamoAtividade = Credenciamento.Proposta.CodigoRamoAtividade.Value;

                List<GEProdutos.ProdutosListaDadosProdutosVanPorRamo> listaProdutosVan = ServicosGE.CarregarProdutosVAN(codigoGrupoRamoAtuacao, codigoRamoAtividade);

                if (listaProdutosVan.Count() > 0)
                {
                    phProdutosVANNaorede.Visible = true;
                    rProdutosVanNaoRede.DataSource = listaProdutosVan;
                    rProdutosVanNaoRede.DataBind();
                }
                else
                {
                    phProdutosVANNaorede.Visible = false;
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }

        }

        /// <summary>
        /// Carregar Pacotes Dados Bancarios 
        /// </summary>
        private void CarregarDadosBancarios()
        {
            try
            {
                Produtos.Clear();
                CarregarDadosCartaoCredito();
                CarregarDadosCartaoDebito();
                CarregarDadosCartaoConstrucard();
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Carregar Pacotes Cartao Credito 
        /// </summary>
        private void CarregarDadosCartaoCredito()
        {
            //Numero do PV, é o da Matriz, quando o tipo de estabelecimento for uma Filial.
            Int32 codigoMatriz = Credenciamento.Proposta.NumeroMatriz.GetValueOrDefault();

            List<Modelo.Produto> produtosCredito = ServicosWF.ConsultaDadosBancarios(Modelo.TipoProduto.Credito,
                                                                                     Credenciamento.Proposta.CodigoGrupoRamo.GetValueOrDefault(),
                                                                                     Credenciamento.Proposta.CodigoRamoAtividade.GetValueOrDefault().ToString(),
                                                                                     Credenciamento.Proposta.CodigoCanal.GetValueOrDefault(),
                                                                                     codigoMatriz);

            foreach (var produtoCredito in produtosCredito)
            {
                produtoCredito.IndicadorTipoProduto = Modelo.TipoProduto.Credito;
                Produtos.Add(produtoCredito);
            }

            Repeater rCredito = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoCreditoNaoRede : rDadosCartaoCreditoRede;

            var produtosCreditoFiltrado = produtosCredito.Where(p => p.CodigoCCA != 69 && p.CodigoCCA != 70); // Diferente de AMEX e ELO (Crédito)

            rCredito.DataSource = produtosCreditoFiltrado;
            rCredito.DataBind();

            if (produtosCreditoFiltrado.Count() > 0)
            {
                if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    phCreditoNaoRede.Visible = true;
                else
                    phCreditoRede.Visible = true;
            }
            else
            {
                if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    phCreditoNaoRede.Visible = false;
                else
                    phCreditoRede.Visible = false;
            }

            Repeater rAmexCredito = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoCreditoAmexNaoRede : rDadosCartaoCreditoAmexRede;

            var produtosAmexCreditoFiltrado = produtosCredito.Where(p => p.CodigoCCA == 69); // AMEX Crédito
            rAmexCredito.DataSource = produtosAmexCreditoFiltrado;
            rAmexCredito.DataBind();

            if (produtosAmexCreditoFiltrado.Count() > 0)
            {
                if (mvTiposFormularios.GetActiveView() == vNaoRede)
                {
                    phAmexCreditoNaoRede.Visible = true;
                }
                else
                {
                    phAmexCreditoRede.Visible = true;
                }
            }
            else
            {
                if (mvTiposFormularios.GetActiveView() == vNaoRede)
                {
                    phAmexCreditoNaoRede.Visible = false;
                }
                else
                {
                    phAmexCreditoRede.Visible = false;
                }
            }

            Repeater rEloCredito = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoCreditoEloNaoRede : rDadosCartaoCreditoEloRede;

            var produtosEloCreditoFiltrado = produtosCredito.Where(p => p.CodigoCCA == 70); // ELO Crédito
            rEloCredito.DataSource = produtosEloCreditoFiltrado;
            rEloCredito.DataBind();

            if (produtosEloCreditoFiltrado.Count() > 0)
            {
                if (mvTiposFormularios.GetActiveView() == vNaoRede)
                {
                    phEloCreditoNaoRede.Visible = true;
                }
                else
                {
                    phEloCreditoRede.Visible = true;
                }
        }
            else
            {
                if (mvTiposFormularios.GetActiveView() == vNaoRede)
                {
                    phEloCreditoNaoRede.Visible = false;
                }
                else
                {
                    phEloCreditoRede.Visible = false;
                }
            }
        }

        /// <summary>
        /// Carregar Pacotes Cartao Credito 
        /// </summary>
        private void CarregarDadosCartaoDebito()
        {
            //Numero do PV, é o da Matriz, quando o tipo de estabelecimento for uma Filial.
            Int32 codigoMatriz = Credenciamento.Proposta.NumeroMatriz.GetValueOrDefault();

            List<Modelo.Produto> produtosDebito = ServicosWF.ConsultaDadosBancarios(Modelo.TipoProduto.Debito,
                                                                                    Credenciamento.Proposta.CodigoGrupoRamo.GetValueOrDefault(),
                                                                                    Credenciamento.Proposta.CodigoRamoAtividade.GetValueOrDefault().ToString(),
                                                                                    Credenciamento.Proposta.CodigoCanal.GetValueOrDefault(),
                                                                                    codigoMatriz);

            foreach (var produtoDebito in produtosDebito)
            {
                produtoDebito.IndicadorTipoProduto = Modelo.TipoProduto.Debito;
                Produtos.Add(produtoDebito);
            }
            Repeater rDebito = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoDebitoNaoRede : rDadosCartaoDebitoRede;

            var produtosDebitoFiltrado = produtosDebito.Where(p => p.CodigoCCA != 71); // Diferente de ELO (Débito)

            rDebito.DataSource = produtosDebitoFiltrado;
            rDebito.DataBind();

            if (produtosDebitoFiltrado.Count() > 0)
            {
                if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    phDebitoRede.Visible = true;
                else
                    phDebitoNaoRede.Visible = true;
            }
            else
            {
                if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    phDebitoRede.Visible = false;
                else
                    phDebitoNaoRede.Visible = false;
            }

            Repeater rEloDebito = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoDebitoEloNaoRede : rDadosCartaoDebitoEloRede;

            var produtosEloDebitoFiltrado = produtosDebito.Where(p => p.CodigoCCA == 71); // ELO Débito
            rEloDebito.DataSource = produtosEloDebitoFiltrado;
            rEloDebito.DataBind();

            if (produtosEloDebitoFiltrado.Count() > 0)
            {
                if (mvTiposFormularios.GetActiveView() == vNaoRede)
                {
                    phEloDebitoNaoRede.Visible = true;
        }
                else
                {
                    phEloDebitoRede.Visible = true;
                }
            }
            else
            {
                if (mvTiposFormularios.GetActiveView() == vNaoRede)
                {
                    phEloDebitoNaoRede.Visible = false;
                }
                else
                {
                    phEloDebitoRede.Visible = false;
                }
            }
        }

        /// <summary>
        /// Carregar Pacotes Cartao Construcard 
        /// </summary>
        private void CarregarDadosCartaoConstrucard()
        {
            //Numero do PV, é o da Matriz, quando o tipo de estabelecimento for uma Filial.
            Int32 codigoMatriz = Credenciamento.Proposta.NumeroMatriz.GetValueOrDefault();

            List<Modelo.Produto> produtosConstrucard = ServicosWF.ConsultaDadosBancarios(Modelo.TipoProduto.Construcard,
                                                                                         Credenciamento.Proposta.CodigoGrupoRamo.GetValueOrDefault(),
                                                                                         Credenciamento.Proposta.CodigoRamoAtividade.GetValueOrDefault().ToString(),
                                                                                         Credenciamento.Proposta.CodigoCanal.GetValueOrDefault(),
                                                                                         codigoMatriz);

            foreach (var produtoConstrucard in produtosConstrucard)
            {
                produtoConstrucard.IndicadorTipoProduto = Modelo.TipoProduto.Construcard;
                Produtos.Add(produtoConstrucard);
            }

            PlaceHolder phProdutosVendasConstrucard = mvTiposFormularios.GetActiveView() == vNaoRede ? phProdutosVendasConstrucardNaoRede : phProdutosVendasConstrucardRede;
            PlaceHolder phVendasConstrucard = mvTiposFormularios.GetActiveView() == vNaoRede ? phVendasConstrucardNaoRede : phVendasConstrucardRede;
            CheckBox cbHabilitarVendasConstrucard = mvTiposFormularios.GetActiveView() == vNaoRede ? cbHabilitarVendasConstrucardNaoRede : cbHabilitarVendasConstrucardRede;

            Repeater rConstrucard = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoConstrucardNaoRede : rDadosCartaoConstrucardRede;
            rConstrucard.DataSource = produtosConstrucard;
            rConstrucard.DataBind();

            if (produtosConstrucard.Count() > 0)
            {
                phProdutosVendasConstrucard.Visible = true;
                phVendasConstrucard.Visible = false;
                cbHabilitarVendasConstrucard.Checked = false;
            }
            else
            {
                phProdutosVendasConstrucard.Visible = false;
                phVendasConstrucard.Visible = false;
                cbHabilitarVendasConstrucard.Checked = false;
            }
        }

        /// <summary>
        /// Carregar Valor Aluguel, e quantidade caso tipo do equipamento seja do tipo TOL, ou SNT, TOF
        /// </summary>
        private void CarregarValorAluguel(String tipoEquipamento)
        {
            if (String.Compare(tipoEquipamento, "TOL") == 0 || String.Compare(tipoEquipamento, "SNT") == 0 || String.Compare(tipoEquipamento, "TOF") == 0)
            {
                txtValorMensalAlguelRede.Enabled = false;
                txtValorMensalAlguelRede.Text = "0,01";

                txtValorMensalAluguelNaoRede.Text = "0,01";
                txtValorMensalAluguelNaoRede.Enabled = false;

                ValorEquipamentoTela = 0.01;

                txtQuantidadeRede.Text = "1";
                txtQuantidadeRede.Enabled = false;

                txtQuantidadeNaoRede.Text = "1";
                txtQuantidadeNaoRede.Enabled = false;
            }
            else
            {
                Double? valorMensalAluguel = ServicosTG.ConsultaValorMensalAluguel(tipoEquipamento, 'A', 'N');

                if (valorMensalAluguel.HasValue)
                {
                    txtValorMensalAlguelRede.Text = String.Format(@"{0:f2}", valorMensalAluguel.GetValueOrDefault());
                    txtValorMensalAluguelNaoRede.Text = String.Format(@"{0:f2}", valorMensalAluguel.GetValueOrDefault());
                }

                if (Credenciamento.Proposta.CodigoCanal.GetValueOrDefault() == 2)
                {
                    txtValorMensalAlguelRede.Enabled = true;
                    txtValorMensalAluguelNaoRede.Enabled = true;
                }
                else
                {
                    txtValorMensalAlguelRede.Enabled = false;
                    txtValorMensalAluguelNaoRede.Enabled = false;
                }

                txtQuantidadeRede.Enabled = true;
                txtQuantidadeNaoRede.Enabled = true;
            }
        }

        /// <summary>
        /// Carrega o grid final de totalizadores
        /// </summary>
        private void CarregarTotalizadores(Double valorMensalidade, Double tarifaExcedente)
        {
            if (String.IsNullOrEmpty(lblDescricaoPagamentoTotalRede.Text))
            {
                lblDescricaoPagamentoTotalRede.Text = "Valor total do pacote + Serviços";
                lblMensalidadePagamentoTotalRede.Text = String.Format("R$ {0}", valorMensalidade.ToString());
                lblTarifaExcedentePagamentoTotalRede.Text = String.Format("R$ {0}", tarifaExcedente.ToString());
            }
            else
            {
                lblMensalidadePagamentoTotalRede.Text = String.Format("R$ {0}", (RemoverCaracteresEspeciais(lblMensalidadePagamentoTotalRede.Text).ToDouble() + valorMensalidade).ToString());
                lblTarifaExcedentePagamentoTotalRede.Text = String.Format("R$ {0}", (RemoverCaracteresEspeciais(lblTarifaExcedentePagamentoTotalRede.Text).ToDouble() + tarifaExcedente).ToString());
            }
        }

        /// <summary>
        /// Método que aplica algumas regras de visibilidade de campos, ao carregar da pagina, segundo a documentação.
        /// </summary>
        private void AplicarRegras()
        {
            //verifica se o canal selecionado é o 2, e hailita o campo de valor mensal de aluguel
            if (Credenciamento.Proposta.CodigoCanal == 2)
            {
                txtValorMensalAlguelRede.Enabled = true;
                txtValorMensalAluguelNaoRede.Enabled = true;
            }
            else
            {
                txtValorMensalAlguelRede.Enabled = false;
                txtValorMensalAluguelNaoRede.Enabled = false;
            }
        }

        /// <summary>
        /// Habilita/Desabilita campos do PDV da tela, quando o tipo de estabelecimento for PDV
        /// </summary>
        private void HabilitarCamposPDV(Boolean habilitado)
        {
            phDadosPV.Visible = habilitado;
        }

        /// <summary>
        /// Preenche objeto credenciamento com dados da tela e chama método que salva
        /// </summary>
        private void SalvarDadosFormulario()
        {
            if (mvTiposFormularios.GetActiveView() == vRede)
            {
                if (Credenciamento.Proposta.CodigoFaseFiliacao == null || Credenciamento.Proposta.CodigoFaseFiliacao < 2)
                    Credenciamento.Proposta.CodigoFaseFiliacao = 2;

                Credenciamento.Tecnologia.CodigoTipoPessoa = Credenciamento.Proposta.CodigoTipoPessoa;
                Credenciamento.Tecnologia.NumeroCNPJ = Credenciamento.Proposta.NumeroCnpjCpf;
                Credenciamento.Tecnologia.NumeroSequenciaProposta = Credenciamento.Proposta.IndicadorSequenciaProposta;

                if (ddlTipoEquipamentoRede.SelectedValue != String.Empty)
                    Credenciamento.Tecnologia.CodigoTipoEquipamento = ddlTipoEquipamentoRede.SelectedValue;
                else
                    Credenciamento.Tecnologia.CodigoTipoEquipamento = null;

                Credenciamento.Tecnologia.QuantidadeTerminalSolicitado = txtQuantidadeRede.Text.ToInt32();
                Credenciamento.Tecnologia.CodigoPropiedadeEquipamento = 1;
                Credenciamento.Tecnologia.CodigoTipoLigacao = 2;

                if (String.Compare(ddlTipoEquipamentoRede.SelectedValue, "TOL") == 0 || String.Compare(ddlTipoEquipamentoRede.SelectedValue, "SNT") == 0 || String.Compare(ddlTipoEquipamentoRede.SelectedValue, "TOF") == 0)
                    Credenciamento.Tecnologia.IndicadorHabilitaVendaDigitada = 'S';
                else
                    Credenciamento.Tecnologia.IndicadorHabilitaVendaDigitada = ' ';

                if (Credenciamento.Tecnologia.IndicadorEnderecoIgualComercial != 'N' && Credenciamento.Tecnologia.IndicadorEnderecoIgualComercial != 'S')
                    Credenciamento.Tecnologia.IndicadorEnderecoIgualComercial = 'S';

                if (Credenciamento.Enderecos.Where(c => c.IndicadorTipoEndereco == (char)Modelo.TipoEndereco.Instalação).Count() > 0)
                {
                    Modelo.Endereco endereco = Credenciamento.Enderecos.Where(c => c.IndicadorTipoEndereco == (char)Modelo.TipoEndereco.Instalação).FirstOrDefault();

                    Credenciamento.Tecnologia.LogradouroTecnologia = endereco.Logradouro;
                    Credenciamento.Tecnologia.ComplementoEnderecoTecnologia = endereco.ComplementoEndereco;
                    Credenciamento.Tecnologia.NumeroEnderecoTecnologia = endereco.NumeroEndereco;
                    Credenciamento.Tecnologia.BairroTecnologia = endereco.Bairro;
                    Credenciamento.Tecnologia.CidadeTecnologia = endereco.Cidade;
                    Credenciamento.Tecnologia.EstadoTecnologia = endereco.Estado;
                    Credenciamento.Tecnologia.CodigoCepTecnologia = endereco.CodigoCep;
                    Credenciamento.Tecnologia.CodigoComplementoCepTecnologia = endereco.CodigoComplementoCep;
                }
                else
                {
                    Credenciamento.Tecnologia.LogradouroTecnologia = "";
                    Credenciamento.Tecnologia.ComplementoEnderecoTecnologia = "";
                    Credenciamento.Tecnologia.NumeroEnderecoTecnologia = "";
                    Credenciamento.Tecnologia.BairroTecnologia = "";
                    Credenciamento.Tecnologia.CidadeTecnologia = "";
                    Credenciamento.Tecnologia.EstadoTecnologia = "";
                    Credenciamento.Tecnologia.CodigoCepTecnologia = "";
                    Credenciamento.Tecnologia.CodigoComplementoCepTecnologia = "";
                }

                //Dados comunicação tecnologia
                if (String.IsNullOrEmpty(Credenciamento.Tecnologia.NomeContato))
                    Credenciamento.Tecnologia.NomeContato = Credenciamento.Proposta.PessoaContato;

                if (Credenciamento.Tecnologia.NumeroTelefone == 0)
                {
                    Credenciamento.Tecnologia.NumeroTelefone = Credenciamento.Proposta.NumeroTelefone1.Value;
                    Credenciamento.Tecnologia.NumeroDDD = Credenciamento.Proposta.NumeroDDD1;
                    Credenciamento.Tecnologia.NumeroRamal = Credenciamento.Proposta.Ramal1.Value;
                }

                //Nao rede preencher
                Credenciamento.Tecnologia.CodigoFabricanteHardware = "";
                Credenciamento.Tecnologia.NomeFabricanteHardware = "";
                Credenciamento.Tecnologia.CodigoFornecedorSoftware = "";
                Credenciamento.Tecnologia.NomeFornecedorSoftware = "";
                //

                if (String.IsNullOrEmpty(Credenciamento.Tecnologia.NumeroDDD))
                    Credenciamento.Tecnologia.NumeroDDD = "";

                Credenciamento.Tecnologia.CodRegimeTecnologia = 0;
                Credenciamento.Tecnologia.CodCentroCustoTecnologia = 0;
                Credenciamento.Tecnologia.CodigoFilialTecnologia = 0;
                //Nao rede preencher
                if (String.IsNullOrEmpty(Credenciamento.Tecnologia.Observacao))
                    Credenciamento.Tecnologia.Observacao = "";

                Credenciamento.Tecnologia.QuantidadeCheckOut = 0;
                Credenciamento.Tecnologia.NumeroPontoVenda = Credenciamento.Proposta.NumeroPontoDeVenda.HasValue ? Credenciamento.Proposta.NumeroPontoDeVenda.Value : 0;
                Credenciamento.Tecnologia.IndicadorPinPad = 'N';
                Credenciamento.Tecnologia.IndicadorFct = ' ';
                Credenciamento.Tecnologia.UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;
                Credenciamento.Tecnologia.CodigoCenario = ddlCenarioRede.SelectedValue.ToInt32();

                Credenciamento.Tecnologia.CodigoEventoEspecial = ddlEventoRede.SelectedIndex != 0 ? ddlEventoRede.SelectedValue : null;
                Credenciamento.Tecnologia.NumeroCpfTecnologia = 0;
                Credenciamento.Tecnologia.AcaoComercial = ddlAcaoComercialRede.SelectedValue.ToInt32();
                Credenciamento.Tecnologia.TerminalFatrExps = 'N';

                Credenciamento.Proposta.CodigoCampanha = ddlCampanhaRede.SelectedValue;
                Credenciamento.Tecnologia.CodigoCenario = ddlCenarioRede.SelectedValue.ToInt32();
                Credenciamento.Tecnologia.ValorEquipamento = ddlCenarioRede.SelectedIndex == 0 ? txtValorMensalAlguelRede.Text.ToDouble() : 0;
                ValorEquipamentoTela = txtValorMensalAlguelRede.Text.ToDouble();
                Credenciamento.Proposta.ValorTaxaAdesao = txtTaxaAdesaoRede.Text.ToDouble();

                if (Credenciamento.Servicos == null)
                    Credenciamento.Servicos = new List<Modelo.Servico>();

                //SERVIÇOS REDE

                Credenciamento.Servicos.Clear();

                //adiciona Serviço Pacote
                if (ddlPacoteServicosRede.SelectedIndex > 0)
                {
                    List<Modelo.Servico> objPacoteservicos = this.PacoteServicos;

                    Modelo.Servico pacoteSelecionado = objPacoteservicos.Where(cod => cod.CodigoServico == ddlPacoteServicosRede.SelectedItem.Value.ToInt32()).FirstOrDefault();
                    Modelo.Regime regimeSelecionado = pacoteSelecionado.Regimes.Where(cod => cod.codigoRegimeServico == ddlQuantidadeTransacoesRede.SelectedItem.Value.ToInt32()).FirstOrDefault();

                    //Atualiza dados Serviço Pacote
                    pacoteSelecionado.CodigoRegimeServico = regimeSelecionado.codigoRegimeServico;
                    pacoteSelecionado.IndicadorAceitaServico = 'S';
                    pacoteSelecionado.IndicadorHabilitaCargaPre = 'N';
                    pacoteSelecionado.UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;
                    pacoteSelecionado.ValorFranquia = regimeSelecionado.PatamarInicio;
                    String quantidadeMinimaTransacoes = ddlQuantidadeTransacoesRede.SelectedItem.Text.Split('-')[1].Trim();
                    if (quantidadeMinimaTransacoes.Length > 4)
                        quantidadeMinimaTransacoes = new String(quantidadeMinimaTransacoes.Take(4).ToArray());
                    pacoteSelecionado.QuantidadeMinimaConsulta = quantidadeMinimaTransacoes.ToInt32();

                    //((DropDownList)item.FindControl("ddlFranquiaRede")).SelectedValue.Split(';')[0].ToDouble();

                    //Adiciona apenas o Patamar Selecionado
                    pacoteSelecionado.Regimes.Clear();
                    pacoteSelecionado.Regimes.Add(regimeSelecionado);

                    Credenciamento.Servicos.Add(pacoteSelecionado);
                }

                foreach (RepeaterItem item in rServicosRede.Items)
                {
                    if (((CheckBox)item.FindControl("cbSelecionarServicoRede")).Checked)
                    {
                        Modelo.Servico servico = new Modelo.Servico();

                        servico.CodigoServico = ((Label)item.FindControl("lblCodigoServicoRede")).Text.ToInt32();
                        servico.TipoServico = (char)Modelo.TipoServico.Servico;
                        servico.CodigoRegimeServico = ((Label)item.FindControl("lblCodigoRegimeRede")).Text.ToInt32();
                        servico.IndicadorAceitaServico = 'S';
                        servico.DescricaoServico = ((Label)item.FindControl("lblServicoRede")).Text;
                        servico.ValorFranquia = RemoverCaracteresEspeciais(((Label)item.FindControl("lblMensalidadeRede")).Text).ToDouble();
                        servico.IndicadorHabilitaCargaPre = 'N';
                        servico.UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;
                        String quantidadeMinimaTransacoes = ((DropDownList)item.FindControl("ddlFranquiaRede")).SelectedItem.Text.Split('-')[1].Trim();
                        if (quantidadeMinimaTransacoes.Length > 4)
                            quantidadeMinimaTransacoes = new String(quantidadeMinimaTransacoes.Take(4).ToArray());

                        servico.QuantidadeMinimaConsulta = quantidadeMinimaTransacoes.ToInt32();

                        if (servico.Regimes == null)
                            servico.Regimes = new List<Modelo.Regime>();

                        servico.Regimes.Add(new Modelo.Regime()
                        {

                            codigoRegimeServico = ((Label)item.FindControl("lblCodigoRegimeRede")).Text.ToInt32(),
                            NumeroSequencia = 0,
                            PatamarInicio = ((DropDownList)item.FindControl("ddlFranquiaRede")).SelectedItem.Text.Split('-')[0].Trim().ToInt32(),
                            PatamarFim = ((DropDownList)item.FindControl("ddlFranquiaRede")).SelectedItem.Text.Split('-')[1].Trim().ToInt32(),
                            ValorCobranca = RemoverCaracteresEspeciais(((Label)item.FindControl("lblMensalidadeRede")).Text).ToDouble(),
                            ValorAdicional = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTarifaExcedenteRede")).Text).ToDouble()

                        });

                        Credenciamento.Servicos.Add(servico);
                    }
                }

                Credenciamento.Produtos.Clear();

                // Verificar se não é Oferta
                if (String.Compare(ddlCondicoesComerciais.SelectedValue, "0") == 0)
                {
                    #region Crédito

                        #region Crédito Rede

                        foreach (RepeaterItem item in rDadosCartaoCreditoRede.Items)
                        {
                            Double taxaPatamar1;
                            Double taxaPatamar2;
                            Double taxa;

                            Modelo.Produto produtoCredito = ObterObjetoProduto();

                            produtoCredito.IndicadorTipoProduto = Modelo.TipoProduto.Credito;
                            produtoCredito.NomeFeature = ((Label)item.FindControl("lblFormaPagamentoCredito")).Text;

                            produtoCredito.CodigoCCA = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32();
                            produtoCredito.CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32();

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text))
                                produtoCredito.ValorPrazoDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32();
                            else
                                produtoCredito.ValorPrazoDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblTaxaCredito")).Text))
                                produtoCredito.ValorTaxaDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            else
                                produtoCredito.ValorTaxaDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoInicialCredito")).Text))
                                produtoCredito.QtdeDefaultParcela = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeDefaultParcela = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoFinalCredito")).Text))
                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeMaximaParcela = 0;

                            taxa = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            if (produtoCredito.CodigoFeature == 3)
                            {
                                if (produtoCredito.Patamares == null)
                                    produtoCredito.Patamares = new List<Modelo.Patamar>();

                                produtoCredito.Patamares.Add(new Modelo.Patamar
                                {
                                    SequenciaPatamar = 1,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32(),
                                    PatamarFinal = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                });
                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            }

                            if (((Panel)item.FindControl("pnlPatamar1")).Visible)
                            {
                                if (produtoCredito.Patamares == null)
                                    produtoCredito.Patamares = new List<Modelo.Patamar>();

                                produtoCredito.Patamares.Add(new Modelo.Patamar()
                                {
                                    SequenciaPatamar = 2,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                }
                                );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32();

                                taxaPatamar1 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble();
                                if (taxaPatamar1 < taxa)
                                    throw new PortalRedecardException(310, FONTE);
                            }

                            if (((Panel)item.FindControl("pnlPatamar2")).Visible)
                            {
                                if (produtoCredito.Patamares == null)
                                    produtoCredito.Patamares = new List<Modelo.Patamar>();

                                produtoCredito.Patamares.Add(new Modelo.Patamar()
                                {
                                    SequenciaPatamar = 3,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                }
                               );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32();

                                taxaPatamar2 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble();
                                if (taxaPatamar2 < taxa)
                                    throw new PortalRedecardException(311, FONTE);
                            }

                            Credenciamento.Produtos.Add(produtoCredito);
                        }

                        #endregion

                        #region Crédito AMEX Rede

                        foreach (RepeaterItem item in rDadosCartaoCreditoAmexRede.Items)
                            {
                            Double taxaPatamar1;
                            Double taxaPatamar2;
                            Double taxa;

                            Modelo.Produto produtoCredito = ObterObjetoProduto();

                            produtoCredito.IndicadorTipoProduto = Modelo.TipoProduto.Credito;
                            produtoCredito.NomeFeature = ((Label)item.FindControl("lblFormaPagamentoCredito")).Text;

                            produtoCredito.CodigoCCA = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32();
                            produtoCredito.CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32();

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text))
                                produtoCredito.ValorPrazoDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32();
                            else
                                produtoCredito.ValorPrazoDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblTaxaCredito")).Text))
                                produtoCredito.ValorTaxaDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            else
                                produtoCredito.ValorTaxaDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoInicialCredito")).Text))
                                produtoCredito.QtdeDefaultParcela = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeDefaultParcela = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoFinalCredito")).Text))
                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeMaximaParcela = 0;

                            taxa = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            if (produtoCredito.CodigoFeature == 3)
                            {
                                if (produtoCredito.Patamares == null)
                                    produtoCredito.Patamares = new List<Modelo.Patamar>();

                                produtoCredito.Patamares.Add(new Modelo.Patamar
                                {
                                    SequenciaPatamar = 1,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32(),
                                    PatamarFinal = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                });
                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            }

                            if (((Panel)item.FindControl("pnlPatamar1")).Visible)
                            {
                                if (produtoCredito.Patamares == null)
                                    produtoCredito.Patamares = new List<Modelo.Patamar>();

                                produtoCredito.Patamares.Add(new Modelo.Patamar()
                                {
                                    SequenciaPatamar = 2,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                }
                                );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32();

                                taxaPatamar1 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble();
                                if (taxaPatamar1 < taxa)
                                    throw new PortalRedecardException(310, FONTE);
                            }

                            if (((Panel)item.FindControl("pnlPatamar2")).Visible)
                            {
                                if (produtoCredito.Patamares == null)
                                    produtoCredito.Patamares = new List<Modelo.Patamar>();

                                produtoCredito.Patamares.Add(new Modelo.Patamar()
                                {
                                    SequenciaPatamar = 3,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                }
                               );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32();

                                taxaPatamar2 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble();
                                if (taxaPatamar2 < taxa)
                                    throw new PortalRedecardException(311, FONTE);
                        }

                            Credenciamento.Produtos.Add(produtoCredito);
                        }

                        #endregion

                        #region Crédito ELO Rede

                        foreach (RepeaterItem item in rDadosCartaoCreditoEloRede.Items)
                        {
                            Double taxaPatamar1;
                            Double taxaPatamar2;
                            Double taxa;

                            Modelo.Produto produtoCredito = ObterObjetoProduto();

                            produtoCredito.IndicadorTipoProduto = Modelo.TipoProduto.Credito;
                            produtoCredito.NomeFeature = ((Label)item.FindControl("lblFormaPagamentoCredito")).Text;

                            produtoCredito.CodigoCCA = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32();
                            produtoCredito.CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32();

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text))
                                produtoCredito.ValorPrazoDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32();
                            else
                                produtoCredito.ValorPrazoDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblTaxaCredito")).Text))
                                produtoCredito.ValorTaxaDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            else
                                produtoCredito.ValorTaxaDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoInicialCredito")).Text))
                                produtoCredito.QtdeDefaultParcela = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeDefaultParcela = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoFinalCredito")).Text))
                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeMaximaParcela = 0;

                            taxa = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            if (produtoCredito.CodigoFeature == 3)
                            {
                                if (produtoCredito.Patamares == null)
                                    produtoCredito.Patamares = new List<Modelo.Patamar>();

                                produtoCredito.Patamares.Add(new Modelo.Patamar
                                {
                                    SequenciaPatamar = 1,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32(),
                                    PatamarFinal = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                });
                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            }

                            if (((Panel)item.FindControl("pnlPatamar1")).Visible)
                            {
                                if (produtoCredito.Patamares == null)
                                    produtoCredito.Patamares = new List<Modelo.Patamar>();

                                produtoCredito.Patamares.Add(new Modelo.Patamar()
                                {
                                    SequenciaPatamar = 2,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                }
                                );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32();

                                taxaPatamar1 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble();
                                if (taxaPatamar1 < taxa)
                                    throw new PortalRedecardException(310, FONTE);
                        }

                        if (((Panel)item.FindControl("pnlPatamar2")).Visible)
                        {
                            if (produtoCredito.Patamares == null)
                                produtoCredito.Patamares = new List<Modelo.Patamar>();

                            produtoCredito.Patamares.Add(new Modelo.Patamar()
                            {
                                SequenciaPatamar = 3,
                                CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2De")).Text.ToInt32(),
                                PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32(),
                                TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble(),
                                Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                            }
                           );

                            produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32();

                            taxaPatamar2 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble();
                            if (taxaPatamar2 < taxa)
                                throw new PortalRedecardException(311, FONTE);
                        }

                        Credenciamento.Produtos.Add(produtoCredito);
                        }

                        #endregion

                    #endregion

                    #region Débito

                        #region Débito Rede

                        foreach (RepeaterItem item in rDadosCartaoDebitoRede.Items)
                        {
                            Modelo.Produto produtoDebito = ObterObjetoProduto();

                            produtoDebito.IndicadorTipoProduto = Modelo.TipoProduto.Debito;
                            produtoDebito.NomeFeature = ((Label)item.FindControl("lblFormaPagamentoDebito")).Text;

                            produtoDebito.CodigoCCA = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32();
                            produtoDebito.CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32();

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblTaxaDebito")).Text))
                                produtoDebito.ValorTaxaDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaDebito")).Text).ToDouble();
                            else
                                produtoDebito.ValorTaxaDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoDebito")).Text))
                                produtoDebito.ValorPrazoDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoDebito")).Text).ToInt32();
                            else
                                produtoDebito.ValorPrazoDefault = 0;

                            Credenciamento.Produtos.Add(produtoDebito);
                        }

                        #endregion

                        #region Débito ELO Rede
                        
                        foreach (RepeaterItem item in rDadosCartaoDebitoEloRede.Items)
                        {
                            Modelo.Produto produtoDebito = ObterObjetoProduto();

                            produtoDebito.IndicadorTipoProduto = Modelo.TipoProduto.Debito;
                            produtoDebito.NomeFeature = ((Label)item.FindControl("lblFormaPagamentoDebito")).Text;

                            produtoDebito.CodigoCCA = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32();
                            produtoDebito.CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32();

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblTaxaDebito")).Text))
                                produtoDebito.ValorTaxaDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaDebito")).Text).ToDouble();
                            else
                                produtoDebito.ValorTaxaDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoDebito")).Text))
                                produtoDebito.ValorPrazoDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoDebito")).Text).ToInt32();
                            else
                                produtoDebito.ValorPrazoDefault = 0;

                            Credenciamento.Produtos.Add(produtoDebito);
                        }

                        #endregion

                    #endregion

                }
                else
                {
                    // É oferta
                    foreach (var produto in Produtos.Where(p => p.IndicadorTipoProduto != Modelo.TipoProduto.Construcard))
                    {
                        Modelo.Produto produtoModificado = ObterObjetoProduto();

                        produtoModificado.IndicadorTipoProduto = produto.IndicadorTipoProduto;
                        produtoModificado.NomeFeature = produto.NomeFeature;

                        produtoModificado.CodigoCCA = produto.CodigoCCA;
                        produtoModificado.CodigoFeature = produto.CodigoFeature;

                        switch (produtoModificado.IndicadorTipoProduto)
                        {
                            case Modelo.TipoProduto.Credito:
                                produtoModificado.ValorPrazoDefault = produto.ValorPrazoDefault.GetValueOrDefault();
                                produtoModificado.ValorTaxaDefault = produto.ValorTaxaDefault.GetValueOrDefault();

                                if (produto.CodigoFeature == 3)
                                {
                                    produtoModificado.QtdeDefaultParcela = 2;
                                    produtoModificado.QtdeMaximaParcela = produto.QtdeDefaultParcela.GetValueOrDefault();

                                    if (produtoModificado.Patamares == null)
                                        produtoModificado.Patamares = new List<Modelo.Patamar>();

                                    produtoModificado.Patamares.Add(new Modelo.Patamar
                                    {
                                        SequenciaPatamar = 1,
                                        CodigoFeature = produto.CodigoFeature.GetValueOrDefault(),
                                        CodigoCca = produto.CodigoCCA.GetValueOrDefault(),
                                        PatamarInicial = 2,
                                        PatamarFinal = produto.QtdeDefaultParcela.GetValueOrDefault(),
                                        TaxaPatamar = produto.ValorTaxaDefault.GetValueOrDefault(),
                                        Prazo = produto.ValorPrazoDefault.GetValueOrDefault()
                                    });
                                }
                                break;
                            case Modelo.TipoProduto.Debito:
                                produtoModificado.ValorTaxaDefault = produto.ValorTaxaDefault.GetValueOrDefault();
                                produtoModificado.ValorPrazoDefault = produto.ValorPrazoDefault.GetValueOrDefault();
                                break;
                            default:
                                break;
                        }

                        Credenciamento.Produtos.Add(produto);
                    }

                    Credenciamento.OfertasPrecoUnico.Clear();
                    Credenciamento.OfertasPrecoUnico.Add(new Modelo.OfertaPrecoUnico()
                    {
                        CodigosOfertaPrecoUnico = ddlCondicoesComerciais.SelectedValue.ToInt32(),
                        CodigoTipoPessoa = Credenciamento.Proposta.CodigoTipoPessoa,
                        CodigoUsuario = Credenciamento.Proposta.UsuarioUltimaAtualizacao,
                        DataHoraUltimaAtualizacao = DateTime.Now,
                        NumeroCNPJ = Credenciamento.Proposta.NumeroCnpjCpf,
                        NumeroSequenciaProposta = Credenciamento.Proposta.IndicadorSequenciaProposta
                    });
                }

                if (cbHabilitarVendasConstrucardRede.Checked)
                {
                    foreach (Modelo.Produto produto in Produtos.Where(p => p.IndicadorTipoProduto == Modelo.TipoProduto.Construcard))
                    {
                        Modelo.Produto produtoConstrucard = new Modelo.Produto();

                        produtoConstrucard.IndicadorTipoProduto = produto.IndicadorTipoProduto;
                        produtoConstrucard.CodigoCCA = produto.CodigoCCA;
                        produtoConstrucard.CodigoFeature = produto.CodigoFeature;
                        produtoConstrucard.ValorPrazoDefault = produto.ValorPrazoDefault;
                        produtoConstrucard.ValorTaxaDefault = produto.ValorTaxaDefault;
                        produtoConstrucard.ProdutoVAN = false;
                        produtoConstrucard.NomeCCA = "";
                        produtoConstrucard.IndicadorPertenceRamo = ' ';
                        produtoConstrucard.CodigoTipoNegocio = ' ';
                        produtoConstrucard.IndicadorFormaPagamento = ' ';
                        produtoConstrucard.IndicadorPatamarUnico = ' ';
                        produtoConstrucard.QtdeMaximaPatamar = 0;
                        produtoConstrucard.QtdeDefaultParcela = 0;
                        produtoConstrucard.QtdeMaximaParcela = 0;
                        produtoConstrucard.IndicadorPermmissaoCancelamento = ' ';
                        produtoConstrucard.Patamares = new List<Modelo.Patamar>();

                        Credenciamento.Produtos.Add(produtoConstrucard);
                    }
                }
            }
            else
            {
                if (Credenciamento.Proposta.CodigoFaseFiliacao == null || Credenciamento.Proposta.CodigoFaseFiliacao < 2)
                    Credenciamento.Proposta.CodigoFaseFiliacao = 2;

                Credenciamento.Tecnologia.CodigoTipoPessoa = Credenciamento.Proposta.CodigoTipoPessoa;
                Credenciamento.Tecnologia.NumeroCNPJ = Credenciamento.Proposta.NumeroCnpjCpf;
                Credenciamento.Tecnologia.NumeroSequenciaProposta = Credenciamento.Proposta.IndicadorSequenciaProposta;

                if (ddlTipoEquipamentoRede.SelectedValue != String.Empty)
                    Credenciamento.Tecnologia.CodigoTipoEquipamento = ddlTipoEquipamentoNaoRede.SelectedValue;
                else
                    Credenciamento.Tecnologia.CodigoTipoEquipamento = null;

                Credenciamento.Tecnologia.QuantidadeTerminalSolicitado = txtQuantidadeNaoRede.Text.ToInt32();
                Credenciamento.Tecnologia.CodigoPropiedadeEquipamento = 1;
                Credenciamento.Tecnologia.CodigoTipoLigacao = 2;

                if (String.Compare(ddlTipoEquipamentoNaoRede.SelectedValue, "TOL") == 0 || String.Compare(ddlTipoEquipamentoNaoRede.SelectedValue, "SNT") == 0 || String.Compare(ddlTipoEquipamentoNaoRede.SelectedValue, "TOF") == 0)
                    Credenciamento.Tecnologia.IndicadorHabilitaVendaDigitada = 'S';
                else
                    Credenciamento.Tecnologia.IndicadorHabilitaVendaDigitada = ' ';

                if (Credenciamento.Tecnologia.IndicadorEnderecoIgualComercial != 'N' && Credenciamento.Tecnologia.IndicadorEnderecoIgualComercial != 'S')
                    Credenciamento.Tecnologia.IndicadorEnderecoIgualComercial = 'S';

                if (Credenciamento.Enderecos.Where(c => c.IndicadorTipoEndereco == (char)Modelo.TipoEndereco.Instalação).Count() > 0)
                {
                    Modelo.Endereco endereco = Credenciamento.Enderecos.Where(c => c.IndicadorTipoEndereco == (char)Modelo.TipoEndereco.Instalação).FirstOrDefault();

                    Credenciamento.Tecnologia.LogradouroTecnologia = endereco.Logradouro;
                    Credenciamento.Tecnologia.ComplementoEnderecoTecnologia = endereco.ComplementoEndereco;
                    Credenciamento.Tecnologia.NumeroEnderecoTecnologia = endereco.NumeroEndereco;
                    Credenciamento.Tecnologia.BairroTecnologia = endereco.Bairro;
                    Credenciamento.Tecnologia.CidadeTecnologia = endereco.Cidade;
                    Credenciamento.Tecnologia.EstadoTecnologia = endereco.Estado;
                    Credenciamento.Tecnologia.CodigoCepTecnologia = endereco.CodigoCep;
                    Credenciamento.Tecnologia.CodigoComplementoCepTecnologia = endereco.CodigoComplementoCep;
                }
                else
                {
                    Credenciamento.Tecnologia.LogradouroTecnologia = "";
                    Credenciamento.Tecnologia.ComplementoEnderecoTecnologia = "";
                    Credenciamento.Tecnologia.NumeroEnderecoTecnologia = "";
                    Credenciamento.Tecnologia.BairroTecnologia = "";
                    Credenciamento.Tecnologia.CidadeTecnologia = "";
                    Credenciamento.Tecnologia.EstadoTecnologia = "";
                    Credenciamento.Tecnologia.CodigoCepTecnologia = "";
                    Credenciamento.Tecnologia.CodigoComplementoCepTecnologia = "";
                }

                //Dados comunicação tecnologia não erede
                if (String.IsNullOrEmpty(Credenciamento.Tecnologia.NomeContato))
                    Credenciamento.Tecnologia.NomeContato = Credenciamento.Proposta.PessoaContato;

                if (Credenciamento.Tecnologia.NumeroTelefone == 0)
                {
                    Credenciamento.Tecnologia.NumeroTelefone = Credenciamento.Proposta.NumeroTelefone1.Value;
                    Credenciamento.Tecnologia.NumeroDDD = Credenciamento.Proposta.NumeroDDD1;
                    Credenciamento.Tecnologia.NumeroRamal = Credenciamento.Proposta.Ramal1.Value;
                }

                //Nao rede preencher
                if (phDadosPV.Visible)
                {
                    Credenciamento.Tecnologia.NumeroRenpac = txtNumRenapacNaoRede.Text.ToInt32();
                    Credenciamento.Tecnologia.CodigoFabricanteHardware = ddlMarcaPDVNaoRede.SelectedValue;
                    Credenciamento.Tecnologia.NomeFabricanteHardware = ddlMarcaPDVNaoRede.SelectedItem.Text;
                    Credenciamento.Tecnologia.CodigoFornecedorSoftware = ddlSoftwareTEFNaoRede.SelectedValue;
                    Credenciamento.Tecnologia.NomeFornecedorSoftware = ddlSoftwareTEFNaoRede.SelectedItem.Text;
                    Credenciamento.Tecnologia.Observacao = txtObservacaoNaoRede.Text.ToString();
                }
                else
                {
                    Credenciamento.Tecnologia.NumeroRenpac = 0;
                    Credenciamento.Tecnologia.CodigoFabricanteHardware = String.Empty;
                    Credenciamento.Tecnologia.NomeFabricanteHardware = String.Empty;
                    Credenciamento.Tecnologia.CodigoFornecedorSoftware = String.Empty;
                    Credenciamento.Tecnologia.NomeFornecedorSoftware = String.Empty;
                    Credenciamento.Tecnologia.Observacao = String.Empty;
                }

                Credenciamento.Tecnologia.CodRegimeTecnologia = 0;
                Credenciamento.Tecnologia.CodCentroCustoTecnologia = 0;
                Credenciamento.Tecnologia.CodigoFilialTecnologia = 0;
                //Nao rede preencher
                if (String.IsNullOrEmpty(Credenciamento.Tecnologia.Observacao))
                    Credenciamento.Tecnologia.Observacao = "";
                if (String.IsNullOrEmpty(Credenciamento.Tecnologia.NumeroDDD))
                    Credenciamento.Tecnologia.NumeroDDD = "";


                Credenciamento.Tecnologia.QuantidadeCheckOut = 0;
                Credenciamento.Tecnologia.NumeroPontoVenda = Credenciamento.Proposta.NumeroPontoDeVenda.HasValue ? Credenciamento.Proposta.NumeroPontoDeVenda.Value : 0;
                Credenciamento.Tecnologia.IndicadorPinPad = 'N';
                Credenciamento.Tecnologia.IndicadorFct = ' ';
                Credenciamento.Tecnologia.UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;
                Credenciamento.Tecnologia.CodigoCenario = ddlCenarioNaoRede.SelectedValue.ToInt32();

                Credenciamento.Tecnologia.CodigoEventoEspecial = ddlEventoNaoRede.SelectedIndex != 0 ? ddlEventoNaoRede.SelectedValue : null;
                Credenciamento.Tecnologia.NumeroCpfTecnologia = 0;
                Credenciamento.Tecnologia.AcaoComercial = ddlAcaoComercialNaoRede.SelectedValue.ToInt32();
                Credenciamento.Tecnologia.TerminalFatrExps = 'N';

                Credenciamento.Proposta.CodigoCampanha = ddlCampanhaNaoRede.SelectedValue;
                Credenciamento.Tecnologia.CodigoCenario = ddlCenarioNaoRede.SelectedValue.ToInt32();
                Credenciamento.Tecnologia.ValorEquipamento = ddlCenarioNaoRede.SelectedIndex == 0 ? txtValorMensalAluguelNaoRede.Text.ToDouble() : 0;
                ValorEquipamentoTela = txtValorMensalAluguelNaoRede.Text.ToDouble();
                Credenciamento.Proposta.ValorTaxaAdesao = txtTaxaAdesaoNaoRede.Text.ToDouble();

                if (Credenciamento.Servicos == null)
                    Credenciamento.Servicos = new List<Modelo.Servico>();

                Credenciamento.Tecnologia.CodigoTipoEquipamento = ddlTipoEquipamentoNaoRede.SelectedValue;
                Credenciamento.Tecnologia.QuantidadeTerminalSolicitado = txtQuantidadeNaoRede.Text.ToInt32();
                Credenciamento.Proposta.CodigoCampanha = ddlCampanhaNaoRede.SelectedValue;
                Credenciamento.Tecnologia.CodigoCenario = ddlCenarioNaoRede.SelectedValue.ToInt32();
                Credenciamento.Proposta.ValorTaxaAdesao = txtTaxaAdesaoNaoRede.Text.ToDouble();

                Credenciamento.Servicos.Clear();

                foreach (RepeaterItem item in rServicosNaoRede.Items)
                {
                    if (((CheckBox)item.FindControl("cbSelecionarServicoNaoRede")).Checked)
                    {
                        Modelo.Servico servico = new Modelo.Servico();

                        servico.CodigoServico = ((Label)item.FindControl("lblCodigoNaoRede")).Text.ToInt32();
                        servico.TipoServico = (char)Modelo.TipoServico.Servico;
                        servico.CodigoRegimeServico = ((Label)item.FindControl("lblCodigoRegimeNaoRede")).Text.ToInt32();
                        servico.IndicadorAceitaServico = 'S';
                        servico.DescricaoServico = ((Label)item.FindControl("lblServicoNaoRede")).Text;
                        servico.ValorFranquia = RemoverCaracteresEspeciais(((Label)item.FindControl("lblMensalidadeNaoRede")).Text).ToDouble();
                        servico.IndicadorHabilitaCargaPre = 'N';
                        servico.UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;
                        String quantidadeMinimaTransacoes = ((DropDownList)item.FindControl("ddlFranquiaNaoRede")).SelectedItem.Text.Split('-')[1].Trim();
                        if (quantidadeMinimaTransacoes.Length > 4)
                            quantidadeMinimaTransacoes = new String(quantidadeMinimaTransacoes.Take(4).ToArray());

                        servico.QuantidadeMinimaConsulta = quantidadeMinimaTransacoes.ToInt32();

                        if (servico.Regimes == null)
                            servico.Regimes = new List<Modelo.Regime>();

                        servico.Regimes.Add(new Modelo.Regime()
                        {

                            codigoRegimeServico = ((Label)item.FindControl("lblCodigoRegimeNaoRede")).Text.ToInt32(),
                            NumeroSequencia = 0,
                            PatamarInicio = ((DropDownList)item.FindControl("ddlFranquiaNaoRede")).SelectedItem.Text.Split('-')[0].Trim().ToInt32(),
                            PatamarFim = ((DropDownList)item.FindControl("ddlFranquiaNaoRede")).SelectedItem.Text.Split('-')[1].Trim().ToInt32(),
                            ValorCobranca = RemoverCaracteresEspeciais(((Label)item.FindControl("lblMensalidadeNaoRede")).Text).ToDouble(),
                            ValorAdicional = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTarifaExcedenteNaoRede")).Text).ToDouble()

                        });

                        Credenciamento.Servicos.Add(servico);
                    }
                }

                Credenciamento.Produtos.Clear();

                foreach (RepeaterItem item in rProdutosVanNaoRede.Items)
                {
                    if (((CheckBox)item.FindControl("cSelecionarProdutoVanNaoRede")).Checked)
                    {
                        Modelo.Produto produto = new Modelo.Produto();

                        produto.IndicadorTipoProduto = Modelo.TipoProduto.Desconhecido;
                        produto.ProdutoVAN = true;
                        produto.CodigoCCA = ((Label)item.FindControl("lblCodigoProdutoVanNaoRede")).Text.ToInt32();
                        produto.NomeCCA = "";
                        produto.CodigoFeature = 0;
                        produto.NomeFeature = ((Label)item.FindControl("lblDescricaoProdutoVanNaoRede")).Text;
                        produto.IndicadorPertenceRamo = ' ';
                        produto.CodigoTipoNegocio = ' ';
                        produto.IndicadorFormaPagamento = ' ';
                        produto.IndicadorPatamarUnico = ' ';
                        produto.QtdeMaximaPatamar = 0;
                        produto.QtdeDefaultParcela = 0;
                        produto.QtdeMaximaParcela = 0;
                        produto.IndicadorPermmissaoCancelamento = ' ';
                        produto.ValorPrazoDefault = 0;
                        produto.ValorTaxaDefault = 0;
                        produto.Patamares = new List<Modelo.Patamar>();

                        Credenciamento.Produtos.Add(produto);
                    }
                }

                // Verificar se não é Oferta
                if (String.Compare(ddlCondicoesComerciaisNaoRede.SelectedValue, "0") == 0)
                {
                    #region Crédito

                        #region Crédito NãoRede

                        foreach (RepeaterItem item in rDadosCartaoCreditoNaoRede.Items)
                        {
                            Double taxaPatamar1;
                            Double taxaPatamar2;
                            Double taxa;

                            Modelo.Produto produtoCredito = ObterObjetoProduto();

                            produtoCredito.IndicadorTipoProduto = Modelo.TipoProduto.Credito;
                            produtoCredito.NomeFeature = ((Label)item.FindControl("lblFormaPagamentoCredito")).Text;

                            produtoCredito.CodigoCCA = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32();
                            produtoCredito.CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32();

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text))
                                produtoCredito.ValorPrazoDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32();
                            else
                                produtoCredito.ValorPrazoDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblTaxaCredito")).Text))
                                produtoCredito.ValorTaxaDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            else
                                produtoCredito.ValorTaxaDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoInicialCredito")).Text))
                                produtoCredito.QtdeDefaultParcela = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeDefaultParcela = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoFinalCredito")).Text))
                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeMaximaParcela = 0;

                            taxa = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            if (produtoCredito.CodigoFeature == 3)
                            {
                                produtoCredito.Patamares.Add(new Modelo.Patamar
                                {
                                    SequenciaPatamar = 1,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32(),
                                    PatamarFinal = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                });

                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            }

                            if (((Panel)item.FindControl("pnlPatamar1")).Visible)
                            {
                                produtoCredito.Patamares.Add(new Modelo.Patamar()
                            {
                                    SequenciaPatamar = 2,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                }
                                );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32();

                                taxaPatamar1 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble();
                                if (taxaPatamar1 < taxa)
                                    throw new PortalRedecardException(310, FONTE);
                            }

                            if (((Panel)item.FindControl("pnlPatamar2")).Visible)
                            {
                                produtoCredito.Patamares.Add(new Modelo.Patamar()
                                {
                                    SequenciaPatamar = 3,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                }
                               );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32();

                                taxaPatamar2 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble();
                                if (taxaPatamar2 < taxa)
                                    throw new PortalRedecardException(311, FONTE);
                            }

                            Credenciamento.Produtos.Add(produtoCredito);
                        }
                        #endregion

                        #region Crédito AMEX NãoRede

                        foreach (RepeaterItem item in rDadosCartaoCreditoAmexNaoRede.Items)
                        {
                            Double taxaPatamar1;
                            Double taxaPatamar2;
                            Double taxa;

                            Modelo.Produto produtoCredito = ObterObjetoProduto();

                            produtoCredito.IndicadorTipoProduto = Modelo.TipoProduto.Credito;
                            produtoCredito.NomeFeature = ((Label)item.FindControl("lblFormaPagamentoCredito")).Text;

                            produtoCredito.CodigoCCA = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32();
                            produtoCredito.CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32();

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text))
                                produtoCredito.ValorPrazoDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32();
                            else
                                produtoCredito.ValorPrazoDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblTaxaCredito")).Text))
                                produtoCredito.ValorTaxaDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            else
                                produtoCredito.ValorTaxaDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoInicialCredito")).Text))
                                produtoCredito.QtdeDefaultParcela = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeDefaultParcela = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoFinalCredito")).Text))
                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeMaximaParcela = 0;

                            taxa = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            if (produtoCredito.CodigoFeature == 3)
                            {
                                produtoCredito.Patamares.Add(new Modelo.Patamar
                                {
                                    SequenciaPatamar = 1,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32(),
                                    PatamarFinal = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                });

                            produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                        }

                            if (((Panel)item.FindControl("pnlPatamar1")).Visible)
                            {
                                produtoCredito.Patamares.Add(new Modelo.Patamar()
                                {
                                    SequenciaPatamar = 2,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                }
                                );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32();

                                taxaPatamar1 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble();
                                if (taxaPatamar1 < taxa)
                                    throw new PortalRedecardException(310, FONTE);
                            }

                            if (((Panel)item.FindControl("pnlPatamar2")).Visible)
                        {
                            produtoCredito.Patamares.Add(new Modelo.Patamar()
                            {
                                    SequenciaPatamar = 3,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                            }
                               );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32();

                                taxaPatamar2 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble();
                                if (taxaPatamar2 < taxa)
                                    throw new PortalRedecardException(311, FONTE);
                            }

                            Credenciamento.Produtos.Add(produtoCredito);
                        }
                        #endregion

                        #region Crédito ELO NãoRede

                        foreach (RepeaterItem item in rDadosCartaoCreditoEloNaoRede.Items)
                        {
                            Double taxaPatamar1;
                            Double taxaPatamar2;
                            Double taxa;

                            Modelo.Produto produtoCredito = ObterObjetoProduto();

                            produtoCredito.IndicadorTipoProduto = Modelo.TipoProduto.Credito;
                            produtoCredito.NomeFeature = ((Label)item.FindControl("lblFormaPagamentoCredito")).Text;

                            produtoCredito.CodigoCCA = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32();
                            produtoCredito.CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32();

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text))
                                produtoCredito.ValorPrazoDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32();
                            else
                                produtoCredito.ValorPrazoDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblTaxaCredito")).Text))
                                produtoCredito.ValorTaxaDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            else
                                produtoCredito.ValorTaxaDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoInicialCredito")).Text))
                                produtoCredito.QtdeDefaultParcela = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeDefaultParcela = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoFinalCredito")).Text))
                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            else
                                produtoCredito.QtdeMaximaParcela = 0;

                            taxa = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble();
                            if (produtoCredito.CodigoFeature == 3)
                            {
                                produtoCredito.Patamares.Add(new Modelo.Patamar
                                {
                                    SequenciaPatamar = 1,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((Label)item.FindControl("lblPrazoInicialCredito")).Text.ToInt32(),
                                    PatamarFinal = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaCredito")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                });

                                produtoCredito.QtdeMaximaParcela = ((Label)item.FindControl("lblPrazoFinalCredito")).Text.ToInt32();
                            }

                            if (((Panel)item.FindControl("pnlPatamar1")).Visible)
                            {
                                produtoCredito.Patamares.Add(new Modelo.Patamar()
                                {
                                    SequenciaPatamar = 2,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                }
                                );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Ate")).Text.ToInt32();

                                taxaPatamar1 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar1")).FindControl("txtPatamar1Taxa")).Text).ToDouble();
                                if (taxaPatamar1 < taxa)
                                    throw new PortalRedecardException(310, FONTE);
                            }

                            if (((Panel)item.FindControl("pnlPatamar2")).Visible)
                            {
                                produtoCredito.Patamares.Add(new Modelo.Patamar()
                                {
                                    SequenciaPatamar = 3,
                                    CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32(),
                                    CodigoCca = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32(),
                                    PatamarInicial = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2De")).Text.ToInt32(),
                                    PatamarFinal = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32(),
                                    TaxaPatamar = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble(),
                                    Prazo = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoRecebimentoCredito")).Text).ToInt32()
                                }
                               );

                                produtoCredito.QtdeMaximaParcela = ((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Ate")).Text.ToInt32();

                                taxaPatamar2 = RemoverCaracteresEspeciais(((TextBox)((Panel)item.FindControl("pnlPatamar2")).FindControl("txtPatamar2Taxa")).Text).ToDouble();
                                if (taxaPatamar2 < taxa)
                                    throw new PortalRedecardException(311, FONTE);
                            }

                            Credenciamento.Produtos.Add(produtoCredito);
                        }
                        #endregion

                    #endregion

                    #region Débito

                        #region Débito NãoRede
                        
                        foreach (RepeaterItem item in rDadosCartaoDebitoNaoRede.Items)
                        {
                            Modelo.Produto produtoDebito = ObterObjetoProduto();

                            produtoDebito.IndicadorTipoProduto = Modelo.TipoProduto.Debito;
                            produtoDebito.NomeFeature = ((Label)item.FindControl("lblFormaPagamentoDebito")).Text;

                            produtoDebito.CodigoCCA = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32();
                            produtoDebito.CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32();

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblTaxaDebito")).Text))
                                produtoDebito.ValorTaxaDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaDebito")).Text).ToDouble();
                            else
                                produtoDebito.ValorTaxaDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoDebito")).Text))
                                produtoDebito.ValorPrazoDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoDebito")).Text).ToInt32();
                            else
                                produtoDebito.ValorPrazoDefault = 0;

                            Credenciamento.Produtos.Add(produtoDebito);
                        }

                        #endregion

                        #region Débito ELO NãoRede

                        foreach (RepeaterItem item in rDadosCartaoDebitoEloNaoRede.Items)
                        {
                            Modelo.Produto produtoDebito = ObterObjetoProduto();

                            produtoDebito.IndicadorTipoProduto = Modelo.TipoProduto.Debito;
                            produtoDebito.NomeFeature = ((Label)item.FindControl("lblFormaPagamentoDebito")).Text;

                            produtoDebito.CodigoCCA = ((HiddenField)item.FindControl("hfCodigoCca")).Value.ToInt32();
                            produtoDebito.CodigoFeature = ((HiddenField)item.FindControl("hfCodigoFeature")).Value.ToInt32();

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblTaxaDebito")).Text))
                                produtoDebito.ValorTaxaDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblTaxaDebito")).Text).ToDouble();
                            else
                                produtoDebito.ValorTaxaDefault = 0;

                            if (!String.IsNullOrEmpty(((Label)item.FindControl("lblPrazoDebito")).Text))
                                produtoDebito.ValorPrazoDefault = RemoverCaracteresEspeciais(((Label)item.FindControl("lblPrazoDebito")).Text).ToInt32();
                            else
                                produtoDebito.ValorPrazoDefault = 0;

                            Credenciamento.Produtos.Add(produtoDebito);
                        }

                        #endregion

                    #endregion

                }
                else
                {
                    // É oferta
                    foreach (var produto in Produtos.Where(p => p.IndicadorTipoProduto != Modelo.TipoProduto.Construcard))
                    {

                        Modelo.Produto produtoModificado = ObterObjetoProduto();

                        produtoModificado.IndicadorTipoProduto = produto.IndicadorTipoProduto;
                        produtoModificado.NomeFeature = produto.NomeFeature;

                        produtoModificado.CodigoCCA = produto.CodigoCCA;
                        produtoModificado.CodigoFeature = produto.CodigoFeature;

                        switch (produtoModificado.IndicadorTipoProduto)
                        {
                            case Modelo.TipoProduto.Credito:
                                produtoModificado.ValorPrazoDefault = produto.ValorPrazoDefault.GetValueOrDefault();
                                produtoModificado.ValorTaxaDefault = produto.ValorTaxaDefault.GetValueOrDefault();

                                if (produto.CodigoFeature == 3)
                                {
                                    produtoModificado.QtdeDefaultParcela = 2;
                                    produtoModificado.QtdeMaximaParcela = produto.QtdeDefaultParcela.GetValueOrDefault();

                                    if (produtoModificado.Patamares == null)
                                        produtoModificado.Patamares = new List<Modelo.Patamar>();

                                    produtoModificado.Patamares.Add(new Modelo.Patamar
                                    {
                                        SequenciaPatamar = 1,
                                        CodigoFeature = produto.CodigoFeature.GetValueOrDefault(),
                                        CodigoCca = produto.CodigoCCA.GetValueOrDefault(),
                                        PatamarInicial = 2,
                                        PatamarFinal = produto.QtdeDefaultParcela.GetValueOrDefault(),
                                        TaxaPatamar = produto.ValorTaxaDefault.GetValueOrDefault(),
                                        Prazo = produto.ValorPrazoDefault.GetValueOrDefault()
                                    });
                                }
                                break;
                            case Modelo.TipoProduto.Debito:
                                produtoModificado.ValorTaxaDefault = produto.ValorTaxaDefault.GetValueOrDefault();
                                produtoModificado.ValorPrazoDefault = produto.ValorPrazoDefault.GetValueOrDefault();
                                break;
                            default:
                                break;
                        }
                        Credenciamento.Produtos.Add(produtoModificado);
                    }

                    Credenciamento.OfertasPrecoUnico.Clear();
                    Credenciamento.OfertasPrecoUnico.Add(new Modelo.OfertaPrecoUnico()
                    {
                        CodigosOfertaPrecoUnico = ddlCondicoesComerciaisNaoRede.SelectedValue.ToInt32(),
                        CodigoTipoPessoa = Credenciamento.Proposta.CodigoTipoPessoa,
                        CodigoUsuario = Credenciamento.Proposta.UsuarioUltimaAtualizacao,
                        DataHoraUltimaAtualizacao = DateTime.Now,
                        NumeroCNPJ = Credenciamento.Proposta.NumeroCnpjCpf,
                        NumeroSequenciaProposta = Credenciamento.Proposta.IndicadorSequenciaProposta
                    });
                }

                if (cbHabilitarVendasConstrucardNaoRede.Checked)
                {
                    foreach (Modelo.Produto produto in Produtos.Where(p => p.IndicadorTipoProduto == Modelo.TipoProduto.Construcard))
                    {
                        Modelo.Produto produtoConstrucard = new Modelo.Produto();

                        produtoConstrucard.IndicadorTipoProduto = produto.IndicadorTipoProduto;
                        produtoConstrucard.CodigoCCA = produto.CodigoCCA;
                        produtoConstrucard.CodigoFeature = produto.CodigoFeature;
                        produtoConstrucard.ValorPrazoDefault = produto.ValorPrazoDefault;
                        produtoConstrucard.ValorTaxaDefault = produto.ValorTaxaDefault;
                        produtoConstrucard.ProdutoVAN = false;
                        produtoConstrucard.NomeCCA = "";
                        produtoConstrucard.IndicadorPertenceRamo = ' ';
                        produtoConstrucard.CodigoTipoNegocio = ' ';
                        produtoConstrucard.IndicadorFormaPagamento = ' ';
                        produtoConstrucard.IndicadorPatamarUnico = ' ';
                        produtoConstrucard.QtdeMaximaPatamar = 0;
                        produtoConstrucard.QtdeDefaultParcela = 0;
                        produtoConstrucard.QtdeMaximaParcela = 0;
                        produtoConstrucard.IndicadorPermmissaoCancelamento = ' ';
                        produtoConstrucard.Patamares = new List<Modelo.Patamar>();

                        Credenciamento.Produtos.Add(produtoConstrucard);
                    }
                }

                #region Alelo

                Credenciamento.ProdutoParceiro.Clear();

                if (cbHabilitarVendasAleloRefeicaoNaoRede.Checked)
                {
                    Modelo.ProdutoParceiro produtoAleloRefeicao = new Modelo.ProdutoParceiro();

                    produtoAleloRefeicao.DataSolicitacao =
                    produtoAleloRefeicao.DataUltimaAtualizacao = DateTime.Now;
                    produtoAleloRefeicao.NumAssentos = txtNumAssentos.Text.ToInt32();
                    produtoAleloRefeicao.NumMesas = txtNumMesas.Text.ToInt32();
                    produtoAleloRefeicao.NumMaximoRefeicoes = txtNumMaximoRefeicoes.Text.ToInt32();
                    produtoAleloRefeicao.AreaAtendimento = txtAreaAtendimento.Text.ToInt32();

                    produtoAleloRefeicao.NumeroCnpj = Credenciamento.Proposta.NumeroCnpjCpf;
                    produtoAleloRefeicao.NumeroPontoDeVenda = Credenciamento.Proposta.NumeroPontoDeVenda;

                    produtoAleloRefeicao.CodigoCanal = Credenciamento.Proposta.CodigoCanal.GetValueOrDefault();
                    produtoAleloRefeicao.CodigoCelula = Credenciamento.Proposta.CodigoCelula.GetValueOrDefault();

                    produtoAleloRefeicao.Produto = new Modelo.Produto() { CodigoCCA = 15, CodigoFeature = hdnCodFeatureAleloRefeicao.Value.ToInt32Null(), CodigoTipoNegocio = 'V' };

                    Credenciamento.ProdutoParceiro.Add(produtoAleloRefeicao);

                    Modelo.Produto produto = new Modelo.Produto();

                    produto.IndicadorTipoProduto = Modelo.TipoProduto.Voucher;
                    produto.ProdutoVAN = false;
                    produto.CodigoCCA = produtoAleloRefeicao.Produto.CodigoCCA;
                    produto.NomeCCA = "";
                    produto.CodigoFeature = produtoAleloRefeicao.Produto.CodigoFeature;
                    produto.NomeFeature = "Alelo Refeição";
                    produto.IndicadorPertenceRamo = ' ';
                    produto.CodigoTipoNegocio = ' ';
                    produto.IndicadorFormaPagamento = ' ';
                    produto.IndicadorPatamarUnico = ' ';
                    produto.QtdeMaximaPatamar = 0;
                    produto.QtdeDefaultParcela = 0;
                    produto.QtdeMaximaParcela = 0;
                    produto.IndicadorPermmissaoCancelamento = ' ';
                    produto.ValorPrazoDefault = 0;
                    produto.ValorTaxaDefault = 0;
                    produto.Patamares = new List<Modelo.Patamar>();

                    Credenciamento.Produtos.Add(produto);
                }
                else
                {
                    if (Credenciamento.ProdutoParceiro.Any(p => p.Produto.CodigoFeature == hdnCodFeatureAleloRefeicao.Value.ToInt32Null()))
                        Credenciamento.ProdutoParceiro.RemoveAll(p => p.Produto.CodigoFeature == hdnCodFeatureAleloRefeicao.Value.ToInt32Null());
                }

                if (cbHabilitarVendasAleloAlimentacaoNaoRede.Checked)
                {
                    Modelo.ProdutoParceiro produtoAleloAlimentacao = new Modelo.ProdutoParceiro();

                    produtoAleloAlimentacao.DataSolicitacao =
                    produtoAleloAlimentacao.DataUltimaAtualizacao = DateTime.Now;
                    produtoAleloAlimentacao.AreaLoja = txtAreaLoja.Text.ToInt32();
                    produtoAleloAlimentacao.QtdCheckOutLoja = txtQtdCheckout.Text.ToInt32();

                    produtoAleloAlimentacao.NumeroCnpj = Credenciamento.Proposta.NumeroCnpjCpf;
                    produtoAleloAlimentacao.NumeroPontoDeVenda = Credenciamento.Proposta.NumeroPontoDeVenda;

                    produtoAleloAlimentacao.CodigoCanal = Credenciamento.Proposta.CodigoCanal.GetValueOrDefault();
                    produtoAleloAlimentacao.CodigoCelula = Credenciamento.Proposta.CodigoCelula.GetValueOrDefault();

                    produtoAleloAlimentacao.Produto = new Modelo.Produto() { CodigoCCA = 15, CodigoFeature = hdnCodFeatureAleloAlimentacao.Value.ToInt32Null(), CodigoTipoNegocio = 'V' };

                    Credenciamento.ProdutoParceiro.Add(produtoAleloAlimentacao);

                    Modelo.Produto produto = new Modelo.Produto();

                    produto.IndicadorTipoProduto = Modelo.TipoProduto.Voucher;
                    produto.ProdutoVAN = false;
                    produto.CodigoCCA = produtoAleloAlimentacao.Produto.CodigoCCA;
                    produto.NomeCCA = "";
                    produto.CodigoFeature = produtoAleloAlimentacao.Produto.CodigoFeature;
                    produto.NomeFeature = "Alelo Alimentação";
                    produto.IndicadorPertenceRamo = ' ';
                    produto.CodigoTipoNegocio = ' ';
                    produto.IndicadorFormaPagamento = ' ';
                    produto.IndicadorPatamarUnico = ' ';
                    produto.QtdeMaximaPatamar = 0;
                    produto.QtdeDefaultParcela = 0;
                    produto.QtdeMaximaParcela = 0;
                    produto.IndicadorPermmissaoCancelamento = ' ';
                    produto.ValorPrazoDefault = 0;
                    produto.ValorTaxaDefault = 0;
                    produto.Patamares = new List<Modelo.Patamar>();

                    Credenciamento.Produtos.Add(produto);
                }
                else
                {
                    if (Credenciamento.ProdutoParceiro.Any(p => p.Produto.CodigoFeature == hdnCodFeatureAleloAlimentacao.Value.ToInt32Null()))
                        Credenciamento.ProdutoParceiro.RemoveAll(p => p.Produto.CodigoFeature == hdnCodFeatureAleloAlimentacao.Value.ToInt32Null());
                }
                #endregion
            }

            ServicosWF.SalvarDadosComerciais(Credenciamento);
        }

        /// <summary>
        /// Cria objeto com propriedades padrão para produto
        /// </summary>
        /// <returns>retorna objeto da classe Produto</returns>
        private Modelo.Produto ObterObjetoProduto()
        {
            Modelo.Produto produto = new Modelo.Produto();

            produto.IndicadorTipoProduto = Modelo.TipoProduto.Desconhecido;
            produto.ProdutoVAN = false;
            produto.CodigoCCA = 0;
            produto.NomeCCA = "";
            produto.IndicadorPertenceRamo = ' ';
            produto.CodigoTipoNegocio = ' ';
            produto.IndicadorFormaPagamento = ' ';
            produto.IndicadorPatamarUnico = ' ';
            produto.QtdeMaximaPatamar = 0;
            produto.QtdeDefaultParcela = 0;
            produto.QtdeMaximaParcela = 0;
            produto.IndicadorPermmissaoCancelamento = ' ';
            produto.ValorPrazoDefault = 0;
            produto.ValorTaxaDefault = 0;
            produto.Patamares = new List<Modelo.Patamar>();

            return produto;
        }

        #endregion

        #region [Control Events]

        /// <summary>
        /// Evento ao marcar, ou desmarcar o checkbox Habilitar Alelo Alimentacao
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbHabilitarVendasAleloAlimentacaoNaoRede_CheckedChanged(object sender, EventArgs e)
        {
            phProdutosVendasAleloAlimentacao.Visible = ((CheckBox)sender).Checked;

            if (!phProdutosVendasAleloAlimentacao.Visible)
                ResetFormularioAleloAlimentacao();

        }

        /// <summary>
        /// Evento ao marcar, ou desmarcar o checkbox Habilitar Alelo Refeicao
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbHabilitarVendasAleloRefeicaoNaoRede_CheckedChanged(object sender, EventArgs e)
        {
            phProdutosVendasAleloRefeicao.Visible = ((CheckBox)sender).Checked;

            if (!phProdutosVendasAleloRefeicao.Visible)
                ResetFormularioAleloRefeicao();
        }

        /// <summary>
        /// Evento disparado ao clicar no botão Salvar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    SalvarDadosFormulario();
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            finally
            {
                Credenciamento = null;
                NovaProposta(sender, e);
            }
        }

        public event EventHandler NovaProposta;

        /// <summary>
        /// Evento do botão continuar do passo de condição comercial
        /// </summary>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Condição Comercial - Continuar"))
            {
                try
                {
                    Page.Validate();
                    if (Page.IsValid)
                    {
                        SalvarDadosFormulario();
                        AtualizarValoresIndicadores();
                        Continuar(sender, e);
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    ex.HandleException(this);
                }
            }
        }

        /// <summary>
        /// Atualiza Valores dos indicadores com base na regra de negócio
        /// </summary>
        protected void AtualizarValoresIndicadores()
        {
            Credenciamento.Proposta.IndicadorComercializacaoNormal = (Credenciamento.Proposta.IndicadorComercializacaoNormal != null && Credenciamento.Proposta.IndicadorComercializacaoNormal == 'S') ? 'N' : 'S';
            Credenciamento.Proposta.IndicadorComercializacaoCatalogo = (Credenciamento.Proposta.IndicadorComercializacaoCatalogo != null && Credenciamento.Proposta.IndicadorComercializacaoCatalogo == 'S') ? 'S' : 'N';
            Credenciamento.Proposta.IndicadorComercializacaoTelefone = (Credenciamento.Proposta.IndicadorComercializacaoTelefone != null && Credenciamento.Proposta.IndicadorComercializacaoTelefone == 'S') ? 'S' : 'N';
            Credenciamento.Proposta.IndicadorComercializacaoEletronico = (Credenciamento.Proposta.IndicadorComercializacaoEletronico != null && Credenciamento.Proposta.IndicadorComercializacaoEletronico == 'S') ? 'S' : 'N';
        }

        public event EventHandler Continuar;

        /// <summary>
        /// Voltar do botão voltar do passo de condição comercial
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Condição Comercial - Voltar"))
            {
                try
                {
                    Voltar(sender, e);
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    ex.HandleException(this);
                }
            }
        }

        /// <summary>
        /// Preenche tabela com informações do Serviço com Base no Pacote selecionado
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ddlQuantidadeTransacoesRede_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Modelo.Servico> servicos = this.PacoteServicos;

            //PACOTE
            Modelo.Servico servicoSelecionado = servicos.Where(cod => cod.CodigoServico == ddlPacoteServicosRede.SelectedItem.Value.ToInt32()).FirstOrDefault();

            //LISTA SERVICOS INCLUSOS NO PACOTE
            List<Modelo.Servico> servicoInclusos = ServicosGE.RecuperarServicosDisponiveisParaPacote(servicoSelecionado.CodigoServico.Value);

            StringBuilder sbServicosInclusos = new StringBuilder();
            foreach (Modelo.Servico objServicoIncluso in servicoInclusos)
            {
                sbServicosInclusos.Append(String.Format("{0}; ", objServicoIncluso.DescricaoServico));
            }

            //Controle de campos de Serviço com base nos Serviços que já existem no Pacote
            ControleServicosNoPacote(servicoInclusos);

            //REGIME
            Modelo.Regime regimeSelecionado = servicoSelecionado.Regimes.Where(cod => cod.codigoRegimeServico ==
                ddlQuantidadeTransacoesRede.SelectedItem.Value.ToInt32()).FirstOrDefault();

            lblMensalidadeRede.Text = regimeSelecionado.ValorCobranca.HasValue ? String.Format("R$ {0}", regimeSelecionado.ValorCobranca.ToString()) : "0";
            lblTarifaExcedenteRede.Text = regimeSelecionado.ValorAdicional.HasValue ? String.Format("R$ {0}", regimeSelecionado.ValorAdicional.ToString()) : "0";
            lblServicosInclusosRede.Text = sbServicosInclusos.ToString();

            //Calucular total tela
            CalcularTotalPacoteServico();
        }

        /// <summary>
        /// Verifica se todos os produtos existentes no Pacote também existem no Repeater de Serviços, "Se sim", bloqueia a seleção destes na tela.
        /// </summary>
        protected void ControleServicosNoPacote(List<Modelo.Servico> lstServicosPacote)
        {
            if (lstServicosPacote == null)
                lstServicosPacote = new List<Modelo.Servico>();

            if (lstServicosPacote.Count() > 0)
                tblServicosPacote.Visible = true;
            else
                tblServicosPacote.Visible = false;

            //Pacote é sempre REDE
            foreach (RepeaterItem item in rServicosRede.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {

                    //Possuem serviços no Pacote, ou seja, pacote selecionado
                    Int32 idServico = ((Label)item.FindControl("lblCodigoServicoRede")).Text.ToInt32();
                    Modelo.Servico objServico = lstServicosPacote.Where(serv => serv.CodigoServico.GetValueOrDefault() == idServico).FirstOrDefault();

                    CheckBox chkServicoRede = ((CheckBox)item.FindControl("cbSelecionarServicoRede"));

                    //Se Serviço está presente no pacote setar a regra de bloqueio
                    if (objServico != null)
                    {
                        //Não existe regra no documento, mas esse if previne do pacote interferir no Serviço da campanha
                        //Se Seleção editável na tela setar regra de pacote
                        if (chkServicoRede.Enabled == true)
                        {
                            ((CheckBox)item.FindControl("cbSelecionarServicoRede")).Checked = false;
                            ((CheckBox)item.FindControl("cbSelecionarServicoRede")).Enabled = false;
                            ((DropDownList)item.FindControl("ddlFranquiaRede")).Enabled = false;
                        }
                    }
                    else
                    {
                        //Se serviço não está na regra de pacote, remover a regra de bloqueio do pacote
                        if (chkServicoRede.Checked == false && chkServicoRede.Enabled == false)
                        {
                            ((CheckBox)item.FindControl("cbSelecionarServicoRede")).Enabled = true;
                            ((DropDownList)item.FindControl("ddlFranquiaRede")).Enabled = true;
                        }
                    }
                }
            }
        }

        public event EventHandler Voltar;

        /// <summary>
        /// Evento ao mudar seleção do controle tipo equipamento
        /// </summary>
        protected void ddlTipoEquipamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlTipoEquipamento = ((DropDownList)sender);
            this.VisibilidadePaineisCondicoesComerciais(false, false, false, false);
            TipoEquipamentoSelecionado(ddlTipoEquipamento.SelectedValue);
        }

        /// <summary>
        /// Ação que ocorre ao preencher ou mudar a seleção de um Equipamento na DropDownList
        /// </summary>
        private void TipoEquipamentoSelecionado(string tipoEquipamento)
        {

            CarregarTipoFormulario(tipoEquipamento);
            CarregarListaCenarios(tipoEquipamento);
            CarregarListaCampanhas(tipoEquipamento);
            CarregarServicos(tipoEquipamento, null);
            CarregarDadosBancarios();
            CarregarPacotes(tipoEquipamento);
            CarregarValorAluguel(tipoEquipamento);

            CarregarProdutosVendasAlelo(tipoEquipamento);

            //Regra da documentação
            //Depois que o usuario seleciona um tipo de equipamento, não há mais possibilidade de selecionar opção em branco.
            if (ddlTipoEquipamentoRede.Items.IndexOf(ddlTipoEquipamentoRede.Items.FindByValue(String.Empty)) >= 0)
                ddlTipoEquipamentoRede.Items.Remove(ddlTipoEquipamentoRede.Items.FindByValue(String.Empty));
            if (ddlTipoEquipamentoNaoRede.Items.IndexOf(ddlTipoEquipamentoNaoRede.Items.FindByValue(String.Empty)) >= 0)
                ddlTipoEquipamentoNaoRede.Items.Remove(ddlTipoEquipamentoNaoRede.Items.FindByValue(String.Empty));

            HabilitarCamposPDV(String.Compare(tipoEquipamento, "PDV") == 0);

            //deixa campo aluguel habilitado/desabilitado de acordo com tipo de equipamento.
            CarregarValorAluguel(tipoEquipamento);
        }

        /// <summary>
        /// Evento disparado ao mudar seleção do dropdownlist cenario
        /// </summary>
        protected void ddlPacoteServicoRede_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlQuantidadeTransacoesRede.Items.Clear();

            List<Modelo.Servico> servicos = this.PacoteServicos;

            Modelo.Servico servicoSelecionado = servicos.Where(cod => cod.CodigoServico.GetValueOrDefault() == ddlPacoteServicosRede.SelectedItem.Value.ToInt32()).FirstOrDefault();

            // Preencher quantidade de transações
            ddlQuantidadeTransacoesRede.Items.Clear();

            if (servicoSelecionado != null)
                foreach (Modelo.Regime regime in servicoSelecionado.Regimes)
                {
                    ddlQuantidadeTransacoesRede.Items.Add(new ListItem(
                        String.Format("{0} - {1}", regime.PatamarInicio.GetValueOrDefault().ToString(), regime.PatamarFim.GetValueOrDefault().ToString()), regime.codigoRegimeServico.ToString())
                    );
                }

            if (ddlQuantidadeTransacoesRede.Items.Count > 0)
                this.ddlQuantidadeTransacoesRede_SelectedIndexChanged(this, null);
            else
                ControleServicosNoPacote(null);

        }

        /// <summary>
        /// Evento disparado ao mudar seleção do dropdownlist cenario
        /// </summary>
        protected void ddlCenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            Boolean formularioNaoRede = mvTiposFormularios.GetActiveView() == vNaoRede;

            DropDownList ddlCenario = (DropDownList)sender;
            DropDownList ddlTipoEquipamento = formularioNaoRede ? ddlTipoEquipamentoNaoRede : ddlTipoEquipamentoRede;
            DropDownList ddlCampanha = formularioNaoRede ? ddlCampanhaNaoRede : ddlCampanhaRede;
            TextBox txtValorAluguel = formularioNaoRede ? txtValorMensalAluguelNaoRede : txtValorMensalAlguelRede;

            if (ddlCenario.SelectedIndex > 0)
            {
                txtValorMensalAlguelRede.Enabled = false;
                txtValorMensalAluguelNaoRede.Enabled = false;
            }
            else if (ddlCenario.SelectedIndex == 0)
            {
                CarregarValorAluguel(ddlTipoEquipamento.SelectedValue);
            }

            if (((DropDownList)sender).SelectedIndex == 0)
            {
                //Conforme regra na documentacao, caso seja retirado o campo campanha, deverá ser carregado os grids de produtos 
                //novamente, conforme load da pagina

                if (ddlCampanha.SelectedIndex > 0)
                    ddlCampanha_SelectedIndexChanged(this, null);
                else
                    CarregarDadosBancarios();
            }

            CarregarValoresCenario(ddlCenario.SelectedValue.ToInt32(), ddlTipoEquipamento.SelectedValue, ddlCampanha.SelectedValue, txtValorAluguel.Text.ToDouble());
        }

        /// <summary>
        /// Carregar valores do cenário
        /// </summary>
        /// <param name="cenario">Cenário</param>
        /// <param name="tipoEquipamento">Tipo de Equipamento</param>
        /// <param name="codigoCampanha">Código da Campanha</param>
        /// <param name="aluguel">Aluguel</param>
        private void CarregarValoresCenario(Int32 cenario, String tipoEquipamento, String codigoCampanha, Double aluguel)
        {
            Boolean formularioNaoRede = mvTiposFormularios.GetActiveView() == vNaoRede;
            //TextBox txtValorMensalAluguel = formularioNaoRede ? txtValorMensalAluguelNaoRede : txtValorMensalAlguelRede;

            TextBox txtValorAluguel = formularioNaoRede ? txtValorMensalAluguelNaoRede : txtValorMensalAlguelRede;

            //marcos.deus
            //Definir valors para os parametros codigosituacao Cenario Canal, e codigoOrigem corretamente.
            List<Modelo.Cenario> listaCenario = ServicosWF.RecuperarDadosCenario(cenario, Credenciamento.Proposta.CodigoCanal.Value,
                                                                                 tipoEquipamento, 'A', codigoCampanha, "Portal",
                                                                                 0, SessaoAtual.LoginUsuario);

            if (listaCenario == null)
                listaCenario = new List<Modelo.Cenario>();

            if (listaCenario.Count > 0)
            {
                txtValorAluguel.Text = String.Format(@"{0:f2}", listaCenario[0].ValorCenario);
                ValorEquipamentoTela = String.Format(@"{0:f2}", listaCenario[0].ValorCenario).ToDouble();
                aluguel = listaCenario[0].ValorCenario;
            }

            if (formularioNaoRede)
            {
                lblDescontoJaneiroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes1) : "0%";
                lblPrecoJaneiroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes1) : "0,00";

                lblDescontoFevereiroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes2) : "0%";
                lblPrecoFevereiroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes2) : "0,00";

                lblDescontoMarcoNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes3) : "0%";
                lblPrecoMarcoNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes3) : "0,00";

                lblDescontoAbrilNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes4) : "0%";
                lblPrecoAbrilNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes4) : "0,00";

                lblDescontoMaioNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes5) : "0%";
                lblPrecoMaioNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes5) : "0,00";

                lblDescontoJunhoNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes6) : "0%";
                lblPrecoJunhoNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes6) : "0,00";

                lblDescontoJulhoNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes7) : "0%";
                lblPrecoJulhoNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes7) : "0,00";

                lblDescontoAgostoNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes8) : "0%";
                lblPrecoAgostoNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes8) : "0,00";

                lblDescontoSetembroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes9) : "0%";
                lblPrecoSetembroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes9) : "0,00";

                lblDescontoOutubroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes10) : "0%";
                lblPrecoOutubroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes10) : "0,00";

                lblDescontoNovembroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes11) : "0%";
                lblPrecoNovembroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes11) : "0,00";

                lblDescontoDezembroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes12) : "0%";
                lblPrecoDezembroNaoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes12) : "0,00";
            }
            else
            {
                lblDescontoJaneiroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes1) : "0%";
                lblPrecoJaneiroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes1) : "0,00";

                lblDescontoFevereiroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes2) : "0%";
                lblPrecoFevereiroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes2) : "0,00";

                lblDescontoMarcoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes3) : "0%";
                lblPrecoMarcoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes3) : "0,00";

                lblDescontoAbrilRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes4) : "0%";
                lblPrecoAbrilRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes4) : "0,00";

                lblDescontoMaioRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes5) : "0%";
                lblPrecoMaioRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes5) : "0,00";

                lblDescontoJunhoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes6) : "0%";
                lblPrecoJunhoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes6) : "0,00";

                lblDescontoJulhoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes7) : "0%";
                lblPrecoJulhoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes7) : "0,00";

                lblDescontoAgostoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes8) : "0%";
                lblPrecoAgostoRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes8) : "0,00";

                lblDescontoSetembroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes9) : "0%";
                lblPrecoSetembroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes9) : "0,00";

                lblDescontoOutubroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes10) : "0%";
                lblPrecoOutubroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes10) : "0,00";

                lblDescontoNovembroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes11) : "0%";
                lblPrecoNovembroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes11) : "0,00";

                lblDescontoDezembroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}%", listaCenario[0].ValorEscalonamentoMes12) : "0%";
                lblPrecoDezembroRede.Text = listaCenario.Count > 0 ? String.Format(@"{0:f2}", listaCenario[0].ValorDescontoMes12) : "0,00";
            }
        }

        /// <summary>
        /// Evento disparado ao mudar seleção do dropdownlist campanha
        /// </summary>
        protected void ddlCampanha_SelectedIndexChanged(object sender, EventArgs e)
        {
            Boolean tipoFormularioNaoRede = mvTiposFormularios.GetActiveView() == vNaoRede;

            DropDownList ddlCampanha = tipoFormularioNaoRede ? ddlCampanhaNaoRede : ddlCampanhaRede;
            String codigoCampanha = ddlCampanha.SelectedValue;


            DropDownList ddlTipoEquipamento = tipoFormularioNaoRede ? ddlTipoEquipamentoNaoRede : ddlTipoEquipamentoRede;
            String tipoEquipamento = ddlTipoEquipamento.SelectedValue;

            TextBox txtValorMensalAluguel = tipoFormularioNaoRede ? txtValorMensalAluguelNaoRede : txtValorMensalAlguelRede;

            CarregarListaCenarios(tipoEquipamento);
            DropDownList ddlCenario = tipoFormularioNaoRede ? ddlCenarioNaoRede : ddlCenarioRede;
            CarregarValoresCenario(ddlCenario.SelectedValue.ToInt32(), tipoEquipamento, codigoCampanha, txtValorMensalAluguel.Text.ToDouble());
            if (ddlCampanha.SelectedIndex == 0)
            {
                //Conforme regra na documentacao, caso seja retirado o campo campanha, deverá ser carregado os grids de produtos e serviços
                CarregarDadosBancarios();
                CarregarServicos(tipoEquipamento, null);
                CarregarTaxaFiliacao();
            }

            Modelo.DadosCampanha dadosCampanha = ServicosWF.ConsultaDadosCampanha(codigoCampanha, tipoEquipamento, SessaoAtual.LoginUsuario);

            if (dadosCampanha.Produtos.Count() > 0)
            {
                foreach (Modelo.Produto produto in Produtos)
                {
                    foreach (Modelo.ParametrosCampanha produtoCampanha in dadosCampanha.Produtos)
                    {
                        if (produto.CodigoFeature.HasValue && produtoCampanha.CodigoFeature.HasValue)
                        {
                            if (produto.CodigoFeature.Value == produtoCampanha.CodigoFeature.Value &&
                                produto.CodigoCCA == produtoCampanha.CodigoCca)
                            {
                                produto.ValorTaxaDefault = produtoCampanha.ValorTaxaParametro.Value;
                                produto.ValorPrazoDefault = produtoCampanha.PrazoParametro.Value;
                            }
                        }
                    }
                }

                Repeater rCredito = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoCreditoNaoRede : rDadosCartaoCreditoRede;


                rCredito.DataSource = Produtos.Where(p => p.IndicadorTipoProduto == Modelo.TipoProduto.Credito && p.CodigoCCA != 69 && p.CodigoCCA != 70); // Diferente de AMEX e ELO (Crédito)
                rCredito.DataBind();

                Repeater rDebito = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoDebitoNaoRede : rDadosCartaoDebitoRede;

                rDebito.DataSource = Produtos.Where(p => p.IndicadorTipoProduto == Modelo.TipoProduto.Debito && p.CodigoCCA != 71); // Diferente de ELO (Débito)
                rDebito.DataBind();

                Repeater rAmexCredito = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoCreditoAmexNaoRede : rDadosCartaoCreditoAmexRede;

                rAmexCredito.DataSource = Produtos.Where(p => p.IndicadorTipoProduto == Modelo.TipoProduto.Credito && p.CodigoCCA == 69); // AMEX (Crédito)
                rAmexCredito.DataBind();

                Repeater rEloCredito = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoCreditoEloNaoRede : rDadosCartaoCreditoEloRede;

                rEloCredito.DataSource = Produtos.Where(p => p.IndicadorTipoProduto == Modelo.TipoProduto.Credito && p.CodigoCCA == 70); // ELO (Crédito)
                rEloCredito.DataBind();

                Repeater rEloDebito = mvTiposFormularios.GetActiveView() == vNaoRede ? rDadosCartaoDebitoEloNaoRede : rDadosCartaoDebitoEloRede;

                rEloDebito.DataSource = Produtos.Where(p => p.IndicadorTipoProduto == Modelo.TipoProduto.Debito && p.CodigoCCA == 71); // ELO (Débito)
                rEloDebito.DataBind();
            }

            Credenciamento.DadosCampanhas.Add(dadosCampanha);


            //SERVIÇOS
            if (dadosCampanha.Servicos.Count() > 0)
            {
                Repeater rServico = mvTiposFormularios.GetActiveView() == vNaoRede ? rServicosNaoRede : rServicosRede;

                foreach (RepeaterItem item in rServico.Items)
                {
                    CheckBox chkSeleciona = new CheckBox();
                    Int32 codServico = 0;
                    if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    {
                        codServico = ((Label)item.FindControl("lblCodigoNaoRede")).Text.ToInt32();
                        chkSeleciona = (CheckBox)item.FindControl("cbSelecionarServicoNaoRede");
                    }
                    else
                    {
                        codServico = ((Label)item.FindControl("lblCodigoServicoRede")).Text.ToInt32();
                        chkSeleciona = (CheckBox)item.FindControl("cbSelecionarServicoRede");
                    }

                    var parametro = dadosCampanha.Servicos.FirstOrDefault(p => p.CodigoServico == codServico);

                    //if (dadosCampanha.Servicos.Count() > 0)
                    //    chkSeleciona.Enabled = false;

                    if (parametro != null)
                    {
                        chkSeleciona.Enabled = false;
                        chkSeleciona.Checked = true;
                    }

                }
                CalcularTotalPacoteServico();

            }
            //Carregar controles do Pacote [Campanha tem prioridade em cima de pacote, porém quando campanha zera ou muda, carrega os serviços do pacote
            if (ddlQuantidadeTransacoesRede.Items.Count > 0)
                this.ddlQuantidadeTransacoesRede_SelectedIndexChanged(this, null);
            else
                ControleServicosNoPacote(null);


            if (dadosCampanha.CenarioAluguel.Count() > 0)
            {
                txtValorMensalAluguel.Enabled = false;
                txtValorMensalAluguel.Text = String.Format("{0:f2}", dadosCampanha.CenarioAluguel[0].ValorTaxaParametro.Value);
            }
            else
            {
                CarregarValorAluguel(tipoEquipamento);
            }

            TextBox txtTaxaAdesao = tipoFormularioNaoRede ? txtTaxaAdesaoNaoRede : txtTaxaAdesaoRede;
            if (dadosCampanha.TaxaAdesao.Count() > 0)
            {
                txtTaxaAdesao.Text = String.Format("{0:f2}", dadosCampanha.TaxaAdesao[0].ValorTaxaParametro);
            }

            if (dadosCampanha.CenarioAluguel.Count() > 0 && dadosCampanha.CenarioAluguel[0].CodigoCenario != null)
            {
                ddlCenario.SelectedValue = dadosCampanha.CenarioAluguel[0].CodigoCenario.ToString();
                CarregarValoresCenario((Int32)dadosCampanha.CenarioAluguel[0].CodigoCenario, tipoEquipamento, codigoCampanha, txtValorMensalAluguel.Text.ToDouble());
                ddlCenario.Enabled = false;
            }
            else
                ddlCenario.Enabled = true;
        }

        /// <summary>
        /// Evento disparado quando o repeater rPRodutosVanNaoRede é alimentado com um novo datasource
        /// </summary>
        protected void rProdutosVanNaoRede_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                GEProdutos.ProdutosListaDadosProdutosVanPorRamo produtoVan = ((GEProdutos.ProdutosListaDadosProdutosVanPorRamo)e.Item.DataItem);

                ((Label)e.Item.FindControl("lblCodigoProdutoVanNaoRede")).Text = produtoVan.CodCCA.ToString();
                ((Label)e.Item.FindControl("lblDescricaoProdutoVanNaoRede")).Text = produtoVan.NomeCCA;
            }
        }

        /// <summary>
        /// Evento disparado quando o repeater rServicosNaoRede é alimentado com um novo datasource
        /// </summary>
        protected void rServicosNaoRede_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Servico servico = ((Modelo.Servico)e.Item.DataItem);

                ((Label)e.Item.FindControl("lblCodigoNaoRede")).Text = servico.CodigoServico.GetValueOrDefault().ToString();
                ((Label)e.Item.FindControl("lblServicoNaoRede")).Text = servico.DescricaoServico;

                // Preencher quantidade de transações
                if (servico.Regimes != null && servico.Regimes.Count > 0)
                    foreach (Modelo.Regime regime in servico.Regimes)
                    {
                        ((DropDownList)e.Item.FindControl("ddlFranquiaNaoRede")).Items.Add(new ListItem(
                            String.Format("{0} - {1}", regime.PatamarInicio.GetValueOrDefault().ToString(), regime.PatamarFim.GetValueOrDefault().ToString()), String.Format("{0};{1};{2};{3}", regime.codigoRegimeServico.ToString(), regime.ValorCobranca.GetValueOrDefault().ToString(), regime.ValorAdicional.GetValueOrDefault().ToString(), regime.codigoRegimeServico.ToString()))
                        );
                    }

                //Carrega dados da linha da tabea utilizando o Regime selecionado (Se existir)
                if (((DropDownList)e.Item.FindControl("ddlFranquiaNaoRede")).Items.Count > 0)
                    this.ddlFranquiaNaoRede_SelectedIndexChanged(e.Item.FindControl("ddlFranquiaNaoRede"), null);

                //SERVIÇO CAMPANHA: Se serviço retornado de campanha bloquear eles na tela e checa-los
                ((CheckBox)e.Item.FindControl("cbSelecionarServicoNaoRede")).Checked = servico.ServicoCampanha;
                ((CheckBox)e.Item.FindControl("cbSelecionarServicoNaoRede")).Enabled = !servico.ServicoCampanha;
            }
        }

        /// <summary>
        /// Evento disparado quando o repeater rServicosRede é alimentado com um novo datasource
        /// </summary>
        protected void rServicosRede_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Servico servico = ((Modelo.Servico)e.Item.DataItem);

                ((Label)e.Item.FindControl("lblCodigoServicoRede")).Text = servico.CodigoServico.GetValueOrDefault().ToString();
                ((Label)e.Item.FindControl("lblServicoRede")).Text = servico.DescricaoServico;

                // Preencher quantidade de Franquias
                foreach (Modelo.Regime regime in servico.Regimes)
                {
                    ((DropDownList)e.Item.FindControl("ddlFranquiaRede")).Items.Add(new ListItem(
                        String.Format("{0} - {1}", regime.PatamarInicio.GetValueOrDefault().ToString(), regime.PatamarFim.GetValueOrDefault().ToString()), String.Format("{0};{1};{2};{3}", regime.codigoRegimeServico.ToString(), regime.ValorCobranca.GetValueOrDefault().ToString(), regime.ValorAdicional.GetValueOrDefault().ToString(), regime.codigoRegimeServico.ToString()))
                    );
                }

                //Carrega dados da linha da tabea utilizando o Regime selecionado (Se existir)
                if (((DropDownList)e.Item.FindControl("ddlFranquiaRede")).Items.Count > 0)
                    this.ddlFranquiaRede_SelectedIndexChanged(e.Item.FindControl("ddlFranquiaRede"), null);

                //SERVIÇO CAMPANHA: Se serviço retornado de campanha bloquear eles na tela e checa-los
                ((CheckBox)e.Item.FindControl("cbSelecionarServicoRede")).Checked = servico.ServicoCampanha;
                ((CheckBox)e.Item.FindControl("cbSelecionarServicoRede")).Enabled = !servico.ServicoCampanha;
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Double totalMensalidade = 0;
                Double totalExcedente = 0;

                foreach (RepeaterItem item in rServicosNaoRede.Items)
                {
                    Modelo.Servico servico = ((Modelo.Servico)item.DataItem);

                    if (servico.Regimes[0].ValorCobranca.HasValue)
                        totalMensalidade += servico.Regimes[0].ValorCobranca.Value;
                    if (servico.Regimes[0].ValorAdicional.HasValue)
                        totalExcedente += servico.Regimes[0].ValorAdicional.Value;
                }

                ((Label)e.Item.FindControl("lblTotalMensalidadeRede")).Text = totalMensalidade.ToString();
                ((Label)e.Item.FindControl("lblTotalExcedenteRede")).Text = totalExcedente.ToString();
            }
        }

        /// <summary>
        /// Alimenta a linha da tabela em que a franquia modificada se encontra - REDE
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ddlFranquiaRede_SelectedIndexChanged(object sender, EventArgs e)
        {


            String chave = ((DropDownList)sender).SelectedItem.Value;
            String[] dados = chave.Split(';');

            Label lblMensalidadeRede = ((Label)((RepeaterItem)((DropDownList)sender).Parent).FindControl("lblMensalidadeRede"));
            Label lblTarifaExcedente = ((Label)((RepeaterItem)((DropDownList)sender).Parent).FindControl("lblTarifaExcedenteRede"));
            Label lblCodigoRegime = ((Label)((RepeaterItem)((DropDownList)sender).Parent).FindControl("lblCodigoRegimeRede"));

            lblMensalidadeRede.Text = String.Format("R$ {0}", dados[1].ToString());
            lblTarifaExcedente.Text = String.Format("R$ {0}", dados[2].ToString());
            lblCodigoRegime.Text = dados[3].ToString();

            CalcularTotalPacoteServico();
        }

        /// <summary>
        /// Alimenta a linha da tabela em que a franquia modificada se encontra - REDE
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ddlFranquiaNaoRede_SelectedIndexChanged(object sender, EventArgs e)
        {


            String chave = ((DropDownList)sender).SelectedItem.Value;
            String[] dados = chave.Split(';');

            Label lblMensalidadeNaoRede = ((Label)((RepeaterItem)((DropDownList)sender).Parent).FindControl("lblMensalidadeNaoRede"));
            Label lblTarifaExcedenteNaoRede = ((Label)((RepeaterItem)((DropDownList)sender).Parent).FindControl("lblTarifaExcedenteNaoRede"));
            Label lblCodigoRegimeNaoRede = ((Label)((RepeaterItem)((DropDownList)sender).Parent).FindControl("lblCodigoRegimeNaoRede"));

            lblMensalidadeNaoRede.Text = String.Format("R$ {0}", dados[1].ToString());
            lblTarifaExcedenteNaoRede.Text = String.Format("R$ {0}", dados[2].ToString());
            lblCodigoRegimeNaoRede.Text = dados[3].ToString();

            CalcularTotalPacoteServico();
        }

        /// <summary>
        /// Evento disparado quando o repeater rDadosCartaoCredito é alimentado com um novo datasource
        /// </summary>
        protected void rDadosCartaoCredito_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = ((Modelo.Produto)e.Item.DataItem);

                ((HiddenField)e.Item.FindControl("hfCodigoFeature")).Value = produto.CodigoFeature.ToString();
                ((HiddenField)e.Item.FindControl("hfCodigoCca")).Value = produto.CodigoCCA.ToString();
                ((Label)e.Item.FindControl("lblFormaPagamentoCredito")).Text = produto.NomeFeature;
                ((Label)e.Item.FindControl("lblPrazoRecebimentoCredito")).Text = String.Format("{0} dia(s)", produto.ValorPrazoDefault.ToString());
                ((Label)e.Item.FindControl("lblTaxaCredito")).Text = String.Format("{0}%", produto.ValorTaxaDefault.ToString());
                ((Label)e.Item.FindControl("lblPrazoInicialCredito")).Text = produto.CodigoFeature == 3 ? "2" : "-";
                ((Label)e.Item.FindControl("lblPrazoFinalCredito")).Text = produto.CodigoFeature == 3 ? produto.QtdeDefaultParcela.ToString() : "-";

                if (produto.QtdeMaximaParcela != null)
                    ((TextBox)e.Item.FindControl("txtQtdeMaximaParcelas")).Text = produto.QtdeMaximaParcela.ToString();

                if (produto.Patamares != null && produto.Patamares.Count > 0)
                {

                    if (produto.Patamares.Count > 1)
                    {
                        Panel pnlPatamar1 = (Panel)e.Item.FindControl("pnlPatamar1");
                        pnlPatamar1.Visible = true;

                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1PrazoRecebimento")).Text = String.Format("{0} dia(s)", produto.Patamares[1].Prazo);
                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1QtdMinParcelas")).Text = produto.Patamares[1].PatamarInicial.ToString();
                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1QtdMaxParcelas")).Text = produto.Patamares[1].PatamarFinal.ToString();
                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1Taxa")).Text = String.Format(@"{0:f2}", produto.Patamares[1].TaxaPatamar);
                    }

                    if (produto.Patamares.Count > 2)
                    {
                        Panel pnlPatamar2 = (Panel)e.Item.FindControl("pnlPatamar2");
                        pnlPatamar2.Visible = true;

                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2PrazoRecebimento")).Text = String.Format("{0} dia(s)", produto.Patamares[2].Prazo);
                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2QtdMinParcelas")).Text = produto.Patamares[2].PatamarInicial.ToString();
                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2QtdMaxParcelas")).Text = produto.Patamares[2].PatamarFinal.ToString();
                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2Taxa")).Text = String.Format(@"{0:f2}", produto.Patamares[2].TaxaPatamar);
                    }
                }

                if (produto.CodigoFeature == 3 && produto.IndicadorPatamarUnico != 'S')
                    ((ImageButton)e.Item.FindControl("ibtnMais")).Visible = true;
            }
        }

        /// <summary>
        /// Evento disparado quando o repeater rDadosCartaoCreditoAmex é alimentado com um novo datasource
        /// </summary>
        protected void rDadosCartaoCreditoAmex_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = ((Modelo.Produto)e.Item.DataItem);

                ((HiddenField)e.Item.FindControl("hfCodigoFeature")).Value = produto.CodigoFeature.ToString();
                ((HiddenField)e.Item.FindControl("hfCodigoCca")).Value = produto.CodigoCCA.ToString();
                ((Label)e.Item.FindControl("lblFormaPagamentoCredito")).Text = produto.NomeFeature;
                ((Label)e.Item.FindControl("lblPrazoRecebimentoCredito")).Text = String.Format("{0} dia(s)", produto.ValorPrazoDefault.ToString());
                ((Label)e.Item.FindControl("lblTaxaCredito")).Text = String.Format("{0}%", produto.ValorTaxaDefault.ToString());
                ((Label)e.Item.FindControl("lblPrazoInicialCredito")).Text = produto.CodigoFeature == 3 ? "2" : "-";
                ((Label)e.Item.FindControl("lblPrazoFinalCredito")).Text = produto.CodigoFeature == 3 ? produto.QtdeDefaultParcela.ToString() : "-";

                if (produto.QtdeMaximaParcela != null)
                    ((TextBox)e.Item.FindControl("txtQtdeMaximaParcelas")).Text = produto.QtdeMaximaParcela.ToString();

                if (produto.Patamares != null && produto.Patamares.Count > 0)
                {

                    if (produto.Patamares.Count > 1)
                    {
                        Panel pnlPatamar1 = (Panel)e.Item.FindControl("pnlPatamar1");
                        pnlPatamar1.Visible = true;

                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1PrazoRecebimento")).Text = String.Format("{0} dia(s)", produto.Patamares[1].Prazo);
                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1QtdMinParcelas")).Text = produto.Patamares[1].PatamarInicial.ToString();
                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1QtdMaxParcelas")).Text = produto.Patamares[1].PatamarFinal.ToString();
                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1Taxa")).Text = String.Format(@"{0:f2}", produto.Patamares[1].TaxaPatamar);
                    }

                    if (produto.Patamares.Count > 2)
                    {
                        Panel pnlPatamar2 = (Panel)e.Item.FindControl("pnlPatamar2");
                        pnlPatamar2.Visible = true;

                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2PrazoRecebimento")).Text = String.Format("{0} dia(s)", produto.Patamares[2].Prazo);
                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2QtdMinParcelas")).Text = produto.Patamares[2].PatamarInicial.ToString();
                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2QtdMaxParcelas")).Text = produto.Patamares[2].PatamarFinal.ToString();
                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2Taxa")).Text = String.Format(@"{0:f2}", produto.Patamares[2].TaxaPatamar);
                    }
                }

                if (produto.CodigoFeature == 3 && produto.IndicadorPatamarUnico != 'S')
                    ((ImageButton)e.Item.FindControl("ibtnMais")).Visible = true;
            }
        }

        /// <summary>
        /// Evento disparado quando o repeater rDadosCartaoCreditoElo é alimentado com um novo datasource
        /// </summary>
        protected void rDadosCartaoCreditoElo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = ((Modelo.Produto)e.Item.DataItem);

                ((HiddenField)e.Item.FindControl("hfCodigoFeature")).Value = produto.CodigoFeature.ToString();
                ((HiddenField)e.Item.FindControl("hfCodigoCca")).Value = produto.CodigoCCA.ToString();
                ((Label)e.Item.FindControl("lblFormaPagamentoCredito")).Text = produto.NomeFeature;
                ((Label)e.Item.FindControl("lblPrazoRecebimentoCredito")).Text = String.Format("{0} dia(s)", produto.ValorPrazoDefault.ToString());
                ((Label)e.Item.FindControl("lblTaxaCredito")).Text = String.Format("{0}%", produto.ValorTaxaDefault.ToString());
                ((Label)e.Item.FindControl("lblPrazoInicialCredito")).Text = produto.CodigoFeature == 3 ? "2" : "-";
                ((Label)e.Item.FindControl("lblPrazoFinalCredito")).Text = produto.CodigoFeature == 3 ? produto.QtdeDefaultParcela.ToString() : "-";

                if (produto.QtdeMaximaParcela != null)
                    ((TextBox)e.Item.FindControl("txtQtdeMaximaParcelas")).Text = produto.QtdeMaximaParcela.ToString();

                if (produto.Patamares != null && produto.Patamares.Count > 0)
                {

                    if (produto.Patamares.Count > 1)
                    {
                        Panel pnlPatamar1 = (Panel)e.Item.FindControl("pnlPatamar1");
                        pnlPatamar1.Visible = true;

                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1PrazoRecebimento")).Text = String.Format("{0} dia(s)", produto.Patamares[1].Prazo);
                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1QtdMinParcelas")).Text = produto.Patamares[1].PatamarInicial.ToString();
                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1QtdMaxParcelas")).Text = produto.Patamares[1].PatamarFinal.ToString();
                        ((Literal)pnlPatamar1.FindControl("ltlPatamar1Taxa")).Text = String.Format(@"{0:f2}", produto.Patamares[1].TaxaPatamar);
                    }

                    if (produto.Patamares.Count > 2)
                    {
                        Panel pnlPatamar2 = (Panel)e.Item.FindControl("pnlPatamar2");
                        pnlPatamar2.Visible = true;

                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2PrazoRecebimento")).Text = String.Format("{0} dia(s)", produto.Patamares[2].Prazo);
                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2QtdMinParcelas")).Text = produto.Patamares[2].PatamarInicial.ToString();
                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2QtdMaxParcelas")).Text = produto.Patamares[2].PatamarFinal.ToString();
                        ((Literal)pnlPatamar2.FindControl("ltlPatamar2Taxa")).Text = String.Format(@"{0:f2}", produto.Patamares[2].TaxaPatamar);
                    }
                }

                if (produto.CodigoFeature == 3 && produto.IndicadorPatamarUnico != 'S')
                    ((ImageButton)e.Item.FindControl("ibtnMais")).Visible = true;
            }
        }

        /// <summary>
        /// Evento disparado quando o repeater rDadosCartaoDebito é alimentado com um novo datasource
        /// </summary>
        protected void rDadosCartaoDebito_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = ((Modelo.Produto)e.Item.DataItem);

                ((HiddenField)e.Item.FindControl("hfCodigoFeature")).Value = produto.CodigoFeature.ToString();
                ((HiddenField)e.Item.FindControl("hfCodigoCca")).Value = produto.CodigoCCA.ToString();
                ((Label)e.Item.FindControl("lblFormaPagamentoDebito")).Text = produto.NomeFeature;
                ((Label)e.Item.FindControl("lblPrazoDebito")).Text = String.Format("{0} dia(s)", produto.ValorPrazoDefault.ToString());
                ((Label)e.Item.FindControl("lblTaxaDebito")).Text = String.Format("{0}%", produto.ValorTaxaDefault.ToString());
            }
        }

        /// <summary>
        /// Evento disparado quando o repeater rDadosCartaoDebitoNaoRedeElo_ItemDataBound é alimentado com um novo datasource
        /// </summary>
        protected void rDadosCartaoDebitoElo_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = ((Modelo.Produto)e.Item.DataItem);

                ((HiddenField)e.Item.FindControl("hfCodigoFeature")).Value = produto.CodigoFeature.ToString();
                ((HiddenField)e.Item.FindControl("hfCodigoCca")).Value = produto.CodigoCCA.ToString();
                ((Label)e.Item.FindControl("lblFormaPagamentoDebito")).Text = produto.NomeFeature;
                ((Label)e.Item.FindControl("lblPrazoDebito")).Text = String.Format("{0} dia(s)", produto.ValorPrazoDefault.ToString());
                ((Label)e.Item.FindControl("lblTaxaDebito")).Text = String.Format("{0}%", produto.ValorTaxaDefault.ToString());
            }
        }

        /// <summary>
        /// Evento disparado quando o repeater rDadosCartaoConstrucard é alimentado com um novo datasource
        /// </summary>
        protected void rDadosCartaoConstrucard_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = ((Modelo.Produto)e.Item.DataItem);

                ((HiddenField)e.Item.FindControl("hfCodigoFeature")).Value = produto.CodigoFeature.ToString();
                ((HiddenField)e.Item.FindControl("hfCodigoCca")).Value = produto.CodigoCCA.ToString();
                ((Label)e.Item.FindControl("lblFormaPagamentoConstrucard")).Text = produto.NomeFeature;
                ((Label)e.Item.FindControl("lblPrazoRecebimentoConstrucard")).Text = String.Format("{0} dia(s)", produto.ValorPrazoDefault.ToString());
                ((Label)e.Item.FindControl("lblTaxaConstrucard")).Text = String.Format("{0}%", produto.ValorTaxaDefault.ToString());
            }
        }

        /// <summary>
        /// Evento disparado ao marcar, ou desmarcar o checkbox cbSelecionarServicoNaoRede, dentro do repeater de serviço e-rede
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbSelecionarServicoRede_CheckedChanged(object sender, EventArgs e)
        {

            CalcularTotalPacoteServico();

            CarregaTotalMensal();
        }

        /// <summary>
        /// Evento disparado ao marcar, ou desmarcar o checkbox cbSelecionarServicoNaoRede, dentro do repeater de serviço e-rede
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbSelecionarServicoNaoRede_CheckedChanged(object sender, EventArgs e)
        {

            CalcularTotalPacoteServico();

            CarregaTotalMensal();
        }

        protected void CalcularTotalPacoteServico()
        {
            Double valorMensalidade = 0;
            Double tarifaExcedente = 0;

            if (mvTiposFormularios.GetActiveView() == vRede)
            {
                CalcularTotalServicoRede(ref valorMensalidade, ref tarifaExcedente);
                CalcularTotalPacoteRede(ref valorMensalidade, ref tarifaExcedente);

                //Carregar dados totais na tabela de exibição
                lblDescricaoPagamentoTotalRede.Text = "Valor total do pacote + Serviços";
                lblMensalidadePagamentoTotalRede.Text = String.Format("R$ {0}", valorMensalidade.ToString());
                lblTarifaExcedentePagamentoTotalRede.Text = String.Format("R$ {0}", tarifaExcedente.ToString());
            }
            else
                CalcularTotalServicoNaoRede(ref valorMensalidade, ref tarifaExcedente);


        }

        protected void CalcularTotalPacoteRede(ref Double valorMensalidade, ref Double tarifaExcedente)
        {
            valorMensalidade += RemoverCaracteresEspeciais(lblMensalidadeRede.Text).ToDouble();
            tarifaExcedente += RemoverCaracteresEspeciais(lblTarifaExcedenteRede.Text).ToDouble();
        }

        protected void CalcularTotalServicoRede(ref Double valorMensalidade, ref Double tarifaExcedente)
        {
            //Verifica todos os serviços Rede e soma seu valor total
            foreach (RepeaterItem item in rServicosRede.Items)
            {
                //Se valor checked somar na conta total de exibição
                if (((CheckBox)item.FindControl("cbSelecionarServicoRede")).Checked)
                {
                    valorMensalidade += RemoverCaracteresEspeciais(((Label)(item.FindControl("lblMensalidadeRede"))).Text).ToDouble();
                    tarifaExcedente += RemoverCaracteresEspeciais(((Label)(item.FindControl("lblTarifaExcedenteRede"))).Text).ToDouble();
                }
            }

            //Setar footer Serviço Rede
            lblTotalMensalidadeRede.Text = String.Format("R$ {0}", valorMensalidade.ToString());
            lblTotalExcedenteRede.Text = String.Format("R$ {0}", tarifaExcedente.ToString());


        }

        protected void CalcularTotalServicoNaoRede(ref Double valorMensalidade, ref Double tarifaExcedente)
        {
            //Verifica todos os serviços Rede e soma seu valor total
            foreach (RepeaterItem item in rServicosNaoRede.Items)
            {
                if (((CheckBox)item.FindControl("cbSelecionarServicoNaoRede")).Checked)
                {
                    valorMensalidade += RemoverCaracteresEspeciais(((Label)(item.FindControl("lblMensalidadeNaoRede"))).Text).ToDouble();
                    tarifaExcedente += RemoverCaracteresEspeciais(((Label)(item.FindControl("lblTarifaExcedenteNaoRede"))).Text).ToDouble();
                }
            }

            //Setar footer Serviço Rede
            lblTotalMensalidadeNaoRede.Text = String.Format("R$ {0}", valorMensalidade.ToString());
            lblTotalExcedenteNaoRede.Text = String.Format("R$ {0}", tarifaExcedente.ToString());
        }

        /// <summary>
        /// Evento ao marcar, ou desmarcar o checkbox selecionar todos
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbSelecionarTodosServicosRede_CheckedChanged(object sender, EventArgs e)
        {
            bool check = ((CheckBox)sender).Checked;

            foreach (RepeaterItem item in rServicosRede.Items)
            {
                CheckBox chkSelecionarServicoRede = ((CheckBox)item.FindControl("cbSelecionarServicoRede"));

                //Apenas surte efeito nos Serviços não bloqueados (Serviços bloqueados para seleção são de Campanha ou de Pacote e possuem regra prória de seleção)
                if (chkSelecionarServicoRede.Enabled)
                    ((CheckBox)item.FindControl("cbSelecionarServicoRede")).Checked = check;

            }

            CalcularTotalPacoteServico();
        }

        /// <summary>
        /// Evento ao marcar, ou desmarcar o checkbox selecionar todos
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbSelecionarTodosServicosNaoRede_CheckedChanged(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rServicosNaoRede.Items)
            {
                ((CheckBox)item.FindControl("cbSelecionarServicoNaoRede")).Checked = ((CheckBox)sender).Checked;
            }
        }

        /// <summary>
        /// Evento ao marcar, ou desmarcar o checkbox habilitar/desabilitar vendas cartao construcard
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbHabilitarVendasConstrucard_CheckedChanged(object sender, EventArgs e)
        {
            DropDownList condicaoComercial = mvTiposFormularios.GetActiveView() == vNaoRede ? ddlCondicoesComerciaisNaoRede : ddlCondicoesComerciais;
            if (condicaoComercial.SelectedIndex == 0)
            {
                phVendasConstrucardNaoRede.Visible = ((CheckBox)sender).Checked;
                phVendasConstrucardRede.Visible = ((CheckBox)sender).Checked;
            }
            else
            {
                phVendasConstrucardNaoRede.Visible = false;
                phVendasConstrucardRede.Visible = false;
            }

            //upCondicaoComercial.Update();
        }

        /// <summary>
        /// Evento ao marcar, ou desmarcar o checkbox selecionar todos
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbSelecionarTodosProdutosVanNaoRede_CheckedChanged(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rProdutosVanNaoRede.Items)
            {
                ((CheckBox)item.FindControl("cSelecionarProdutoVanNaoRede")).Checked = ((CheckBox)sender).Checked;
            }
        }

        #endregion

        /// <summary>
        /// Muda o valor do limite de parcelas
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtPatamar1Ate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Int32 patamar1Ate = ((TextBox)sender).Text.ToInt32();

                Panel patamar2 = (Panel)((TextBox)sender).Parent.FindControl("pnlPatamar2");
                if (patamar2.Visible)
                {
                    Int32 patamar2Ate = ((TextBox)patamar2.FindControl("txtPatamar2Ate")).Text.ToInt32();
                    ((TextBox)patamar2.FindControl("txtPatamar2De")).Text = (patamar1Ate + 1).ToString();

                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecaoAsync(FONTE, CODIGO_ERRO, upCondicaoComercial);
            }
        }

        /// <summary>
        /// Muda o valor do limite de parcelas
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtPatamar2Ate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Int32 patamar2Ate = ((TextBox)sender).Text.ToInt32();

                if (patamar2Ate == 0)
                {
                    Panel patamar1 = (Panel)((TextBox)sender).Parent.FindControl("pnlPatamar1");
                    Int32 patamar1Ate = ((TextBox)patamar1.FindControl("txtPatamar1Ate")).Text.ToInt32();
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecaoAsync(FONTE, CODIGO_ERRO, upCondicaoComercial);
            }
        }

        /// <summary>
        /// Adiciona Patamares
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ibtnMais_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Panel patamar1 = (Panel)((ImageButton)sender).Parent.FindControl("pnlPatamar1");

                if (patamar1.Visible == false)
                {
                    patamar1.Visible = !patamar1.Visible;
                    ((TextBox)patamar1.FindControl("txtPatamar1De")).Text = (((Label)((ImageButton)sender).Parent.FindControl("lblPrazoFinalCredito")).Text.ToInt32() + 1).ToString();
                }
                else
                {
                    Panel patamar2 = (Panel)((ImageButton)sender).Parent.FindControl("pnlPatamar2");
                    if (patamar2.Visible == false)
                    {
                        patamar2.Visible = !patamar2.Visible;
                        ((TextBox)patamar2.FindControl("txtPatamar2De")).Text = (((TextBox)patamar1.FindControl("txtPatamar1Ate")).Text.ToInt32() + 1).ToString();

                        patamar1.FindControl("ibtnMenosPatamar1").Visible = false;
                    }
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados de Negócio", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecaoAsync(FONTE, CODIGO_ERRO, upCondicaoComercial);
            }
        }

        /// <summary>
        /// Remove Patamar 1
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ibtnMenosPatamar1_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Panel patamar1 = (Panel)((ImageButton)sender).Parent;
                patamar1.Visible = false;

                ((TextBox)patamar1.FindControl("txtPatamar1Taxa")).Text = "0,00";
                ((TextBox)patamar1.FindControl("txtPatamar1Ate")).Text = String.Empty;
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados de Negócio", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecaoAsync(FONTE, CODIGO_ERRO, upCondicaoComercial);
            }
        }

        /// <summary>
        /// Remove Patamar 2
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ibtnMenosPatamar2_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Panel patamar2 = (Panel)((ImageButton)sender).Parent;
                patamar2.Visible = false;

                ((TextBox)patamar2.FindControl("txtPatamar2Taxa")).Text = "0,00";
                ((TextBox)patamar2.FindControl("txtPatamar2Ate")).Text = String.Empty;

                Panel patamar1 = (Panel)((ImageButton)sender).Parent.Parent.FindControl("pnlPatamar1");
                patamar1.FindControl("ibtnMenosPatamar1").Visible = true;
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados de Negócio", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecaoAsync(FONTE, CODIGO_ERRO, upCondicaoComercial);
            }
        }

        /// <summary>
        /// Valida taxa patamar 1
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvPatamar1Taxa_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                var lblTaxaCredito = (Label)((Control)source).Parent.Parent.FindControl("lblTaxaCredito");

                if (args.Value.ToDouble() >= lblTaxaCredito.Text.Replace("%", "").ToDouble())
                    args.IsValid = true;
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Obtem valores de condições comerciais 
        /// e apresenta parâmetros de acordo com a condição comerciais (oferta) selecionada
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ddlCondicoesComerciais_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlCondicaoComercial = sender as DropDownList;
            Int32 codigoOferta = ddlCondicaoComercial.SelectedValue.ToInt32();

            bool taxaFiliacaoIsenta = false;

            if (String.Compare(ddlCondicaoComercial.SelectedValue, "0", true) == 0) // retornando 0 indica que é igual
            {
                mvTiposFormularios.SetActiveView(vNaoRede);
                CarregarCamposIniciais();
            }
            else
            {
                ReiniciaCamposIniciais();

                ddlCondicoesComerciaisNaoRede.SelectedValue = codigoOferta.ToString();

                this.VisibilidadePaineisCondicoesComerciais(true, true, true, true);
                this.VisibilidadeCamposNaoCondicoesComerciais(true);

                Repeater tecnologia = mvTiposFormularios.GetActiveView() == vNaoRede ? repCondicaoComercialTecnologiaNaoRede : repCondicaoComercialTecnologia;
                Repeater tecnologiaFlex = mvTiposFormularios.GetActiveView() == vNaoRede ? repCondicaoComercialTecnologiaFlexNaoRede : repCondicaoComercialTecnologiaFlex;
                Repeater tipoVenda = mvTiposFormularios.GetActiveView() == vNaoRede ? repTipoVendaNaoRede : repTipoVenda;
                Repeater tipoVendaAmex = mvTiposFormularios.GetActiveView() == vNaoRede ? repTipoVendaAmexNaoRede : repTipoVendaAmex;
                Repeater tipoVendaElo = mvTiposFormularios.GetActiveView() == vNaoRede ? repTipoVendaEloNaoRede : repTipoVendaElo;

                //DropDownList tipoEquipamento = mvTiposFormularios.GetActiveView() == vNaoRede ? ddlTipoEquipamentoNaoRede : ddlTipoEquipamentoRede;

                var oferta = ServicosWF.RecuperarOfertaPadrao(codigoOferta);

                taxaFiliacaoIsenta = TaxaFiliacaoIsenta();

                if (taxaFiliacaoIsenta)
                {
                    txtTaxaAdesaoNaoRede.Text = String.Format("{0:f2}", 0);
                    txtTaxaAdesaoRede.Text = String.Format("{0:f2}", 0);
                }

                if (String.Compare(oferta.Bandeiras.FirstOrDefault().IndicadorProdutoFlex, "S", true) == 0)
                {
                    this.VisibilidadePaineisCondicoesComerciais(true, false, true, true);

                    // Preenche Faturamento
                    Literal valorFaturamentoFlex = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrValorFaturamentoFlexNaoRede : ltrValorFaturamentoFlex;
                    Literal mensalidadeCarencia = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrMensalidadeCarenciaNaoRede : ltrMensalidadeCarencia;
                    Literal mensalidadePosCarencia = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrMensalidadePosCarenciaNaoRede : ltrMensalidadePosCarencia;
                    valorFaturamentoFlex.Text = oferta.ValorFaturamento.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
                    mensalidadeCarencia.Text = oferta.ValorPrecoUnicoSemFlex.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
                    mensalidadePosCarencia.Text = oferta.ValorPrecoUnicoComFlex.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));

                    if (oferta.Tecnologias != null && oferta.Tecnologias.Count > 0)
                    {
                        tecnologiaFlex.DataSource = oferta.Tecnologias;
                        tecnologiaFlex.DataBind();
                    }

                    if (oferta.Bandeiras.Count > 0)
                    {
                        // Preenche FLEX
                        this.PreencheCamposFlex(oferta.Bandeiras.FirstOrDefault().PercentualTaxaFlex1,
                                                oferta.Bandeiras.FirstOrDefault().PercentualTaxaFlex1,
                                                oferta.Bandeiras.FirstOrDefault().PercentualTaxaFlex2);
                    }
                    else
                        this.PreencheCamposFlex();
                }
                else
                {
                    this.VisibilidadePaineisCondicoesComerciais(true, true, false, true);

                    // Preenche Faturamento
                    Literal valorFaturamento = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrValorFaturamentoNaoRede : ltrValorFaturamento;
                    Literal mensalidade = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrMensalidadeNaoRede : ltrMensalidade;
                    valorFaturamento.Text = oferta.ValorFaturamento.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
                    mensalidade.Text = oferta.ValorPrecoUnicoSemFlex.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
                    if (oferta.Tecnologias != null && oferta.Tecnologias.Count > 0)
                    {
                        tecnologia.DataSource = oferta.Tecnologias;
                        tecnologia.DataBind();
                    }
                    // Limpa FLEX
                    this.PreencheCamposFlex();
                }
                Literal valorTaxaAdesao = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrValorTaxaAdesaoNaoRede : ltrValorTaxaAdesao;
                Literal valorMensalOferta = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrValorMensalOfertaNaoRede : ltrValorMensalOferta;

                if (taxaFiliacaoIsenta)
                    valorTaxaAdesao.Text = 0.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
                else
                    valorTaxaAdesao.Text = ServicosGE.ConsultaTaxaFiliacao().ValorParametro.GetValueOrDefault().ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));

                valorMensalOferta.Text = String.Compare(oferta.Bandeiras.FirstOrDefault().IndicadorProdutoFlex, "S", true) == 0 ?
                                         oferta.ValorPrecoUnicoComFlex.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR")) :
                                         oferta.ValorPrecoUnicoSemFlex.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));

                CarregaTotalMensal();

                // Diferente de AMEX e ELO
                var produtos = from p in Produtos
                               where p.CodigoCCA != 69 && 
                                     p.CodigoCCA != 70 && 
                                     p.CodigoCCA != 71
                               group p by p.IndicadorTipoProduto into h
                               select h.First();

                tipoVenda.DataSource = produtos;
                tipoVenda.DataBind();

                if (produtos.Count() > 0)
                {
                    if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    {
                        pnlTaxaExcedenteAgrupadoNaoRede.Visible = true;
                    }
                    else
                    {
                        pnlTaxaExcedenteAgrupado.Visible = true;
                    }
                }
                else
                {
                    if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    {
                        pnlTaxaExcedenteAgrupadoNaoRede.Visible = false;
                    }
                    else
                    {
                        pnlTaxaExcedenteAgrupado.Visible = false;
                    }
                }

                // AMEX
                var produtosAmex = from p in Produtos
                                   where p.CodigoCCA == 69
                                   group p by p.IndicadorTipoProduto into h
                                   select h.First();

                tipoVendaAmex.DataSource = produtosAmex;
                tipoVendaAmex.DataBind();

                if (produtosAmex.Count() > 0)
                {
                    if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    {
                        pnlTaxaExcedenteAgrupadoAmexNaoRede.Visible = true;
                    }
                    else
                    {
                        pnlTaxaExcedenteAgrupadoAmex.Visible = true;
                    }
                }
                else
                {
                    if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    {
                        pnlTaxaExcedenteAgrupadoAmexNaoRede.Visible = false;
                    }
                    else
                    {
                        pnlTaxaExcedenteAgrupadoAmex.Visible = false;
                    }
                }

                // ELO
                var produtosElo = from p in Produtos
                                  where p.CodigoCCA == 70 || p.CodigoCCA == 71
                                  group p by p.IndicadorTipoProduto into h
                                  select h.First();

                tipoVendaElo.DataSource = produtosElo;
                tipoVendaElo.DataBind();

                if (produtosElo.Count() > 0)
                {
                    if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    {
                        pnlTaxaExcedenteAgrupadoEloNaoRede.Visible = true;
                    }
                    else
                    {
                        pnlTaxaExcedenteAgrupadoElo.Visible = true;
                    }
                }
                else
                {
                    if (mvTiposFormularios.GetActiveView() == vNaoRede)
                    {
                        pnlTaxaExcedenteAgrupadoEloNaoRede.Visible = false;
                    }
                    else
                    {
                        pnlTaxaExcedenteAgrupadoElo.Visible = false;
                    }
                }

                this.VisibilidadeCamposNaoCondicoesComerciais(false);
            }
        }

        /// <summary>
        /// Apresentação de dados de Tecnologia da Oferta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repCondicaoComercialTecnologia_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                WFOfertas.TecnologiaPadrao tecnologia = e.Item.DataItem as WFOfertas.TecnologiaPadrao;

                var quantidade = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrQuantidadeNaoRede") : (Literal)e.Item.FindControl("ltrQuantidade");
                var tipo = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrTipoNaoRede") : (Literal)e.Item.FindControl("ltrTipo");
                quantidade.Text = tecnologia.QuantidadeEquipamento.ToString();
                tipo.Text = tecnologia.CodigoEquipamento;
            }
        }

        /// <summary>
        /// Apresentação de dados de Tipo de Venda da Oferta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repTipoVenda_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = e.Item.DataItem as Modelo.Produto;
                var tipoVenda = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrTipoVendaNaoRede") : (Literal)e.Item.FindControl("ltrTipoVenda");
                tipoVenda.Text = produto.IndicadorTipoProduto.GetDescription();

                var modalidadeParcela = mvTiposFormularios.GetActiveView() == vNaoRede ? (Repeater)e.Item.FindControl("repModalidadeParcelaNaoRede") : (Repeater)e.Item.FindControl("repModalidadeParcela");

                modalidadeParcela.DataSource = from p in Produtos
                                               where p.CodigoCCA != 69 && 
                                                     p.CodigoCCA != 70 && 
                                                     p.CodigoCCA != 71 &&
                                                     p.IndicadorTipoProduto == produto.IndicadorTipoProduto
                                               group p by p.NomeFeature into h
                                               select h.First();
                modalidadeParcela.DataBind();
            }
        }

        /// <summary>
        /// Apresentação de dados de Modalidade e Limites de Parcelas da Oferta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repModalidadeParcela_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = e.Item.DataItem as Modelo.Produto;

                var modalidade = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrModalidadeNaoRede") : (Literal)e.Item.FindControl("ltrModalidade");
                var limiteParcela = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrLimiteParcelaNaoRede") : (Literal)e.Item.FindControl("ltrLimiteParcela");
                var prazoTaxa = mvTiposFormularios.GetActiveView() == vNaoRede ? (Repeater)e.Item.FindControl("repPrazoTaxaNaoRede") : (Repeater)e.Item.FindControl("repPrazoTaxa");

                modalidade.Text = produto.NomeFeature;

                if (produto.IndicadorTipoProduto == Modelo.TipoProduto.Credito && produto.CodigoFeature == 3)
                    limiteParcela.Text = String.Format("2 - {0}", produto.QtdeDefaultParcela.GetValueOrDefault());
                else
                    limiteParcela.Text = "-";

                prazoTaxa.DataSource = from p in Produtos
                                       where p.CodigoCCA != 69 &&
                                             p.CodigoCCA != 70 &&
                                             p.CodigoCCA != 71 && 
                                             p.IndicadorTipoProduto == produto.IndicadorTipoProduto &&
                                             p.NomeFeature == produto.NomeFeature
                                       select p;
                prazoTaxa.DataBind();
            }

        }

        /// <summary>
        /// Apresentação de dados de Prazo e Taxas da Oferta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repPrazoTaxa_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = e.Item.DataItem as Modelo.Produto;

                var prazoRecebimento = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrPrazoRecebimentoNaoRede") : (Literal)e.Item.FindControl("ltrPrazoRecebimento");
                var taxa = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrTaxaNaoRede") : (Literal)e.Item.FindControl("ltrTaxa");

                prazoRecebimento.Text = produto.ValorPrazoDefault.HasValue ? String.Format("{0} dia(s)", produto.ValorPrazoDefault.Value) : String.Empty;
                taxa.Text = produto.ValorTaxaDefault.GetValueOrDefault() > 0 ? (produto.ValorTaxaDefault.GetValueOrDefault() / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : produto.ValorTaxaDefault.GetValueOrDefault().ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        /// <summary>
        /// Apresentação de dados de Tipo de Venda da Oferta Amex
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repTipoVendaAmex_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = e.Item.DataItem as Modelo.Produto;
                var tipoVenda = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrTipoVendaNaoRede") : (Literal)e.Item.FindControl("ltrTipoVenda");
                tipoVenda.Text = produto.IndicadorTipoProduto.GetDescription();

                var modalidadeParcela = mvTiposFormularios.GetActiveView() == vNaoRede ? (Repeater)e.Item.FindControl("repModalidadeParcelaAmexNaoRede") : (Repeater)e.Item.FindControl("repModalidadeParcelaAmex");

                modalidadeParcela.DataSource = from p in Produtos
                                               where p.CodigoCCA == 69 && 
                                                     p.IndicadorTipoProduto == produto.IndicadorTipoProduto
                                               group p by p.NomeFeature into h
                                               select h.First();
                modalidadeParcela.DataBind();
            }

        }

        /// <summary>
        /// Apresentação de dados de Modalidade e Limites de Parcelas da Oferta Amex
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repModalidadeParcelaAmex_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = e.Item.DataItem as Modelo.Produto;

                var modalidade = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrModalidadeNaoRede") : (Literal)e.Item.FindControl("ltrModalidade");
                var limiteParcela = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrLimiteParcelaNaoRede") : (Literal)e.Item.FindControl("ltrLimiteParcela");
                var prazoTaxa = mvTiposFormularios.GetActiveView() == vNaoRede ? (Repeater)e.Item.FindControl("repPrazoTaxaAmexNaoRede") : (Repeater)e.Item.FindControl("repPrazoTaxaAmex");

                modalidade.Text = produto.NomeFeature;

                if (produto.IndicadorTipoProduto == Modelo.TipoProduto.Credito && produto.CodigoFeature == 3)
                    limiteParcela.Text = String.Format("2 - {0}", produto.QtdeDefaultParcela.GetValueOrDefault());
                else
                    limiteParcela.Text = "-";

                prazoTaxa.DataSource = from p in Produtos
                                       where p.CodigoCCA == 69 && 
                                             p.IndicadorTipoProduto == produto.IndicadorTipoProduto &&
                                             p.NomeFeature == produto.NomeFeature
                                       select p;
                prazoTaxa.DataBind();
            }
        }

        /// <summary>
        /// Apresentação de dados de Prazo e Taxas da Oferta AMEX
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repPrazoTaxaAmex_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = e.Item.DataItem as Modelo.Produto;

                var prazoRecebimento = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrPrazoRecebimentoNaoRede") : (Literal)e.Item.FindControl("ltrPrazoRecebimento");
                var taxa = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrTaxaNaoRede") : (Literal)e.Item.FindControl("ltrTaxa");

                prazoRecebimento.Text = produto.ValorPrazoDefault.HasValue ? String.Format("{0} dia(s)", produto.ValorPrazoDefault.Value) : String.Empty;
                taxa.Text = produto.ValorTaxaDefault.GetValueOrDefault() > 0 ? (produto.ValorTaxaDefault.GetValueOrDefault() / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : produto.ValorTaxaDefault.GetValueOrDefault().ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        /// <summary>
        /// Apresentação de dados de Tipo de Venda da Oferta ELO
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repTipoVendaElo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = e.Item.DataItem as Modelo.Produto;
                var tipoVenda = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrTipoVendaNaoRede") : (Literal)e.Item.FindControl("ltrTipoVenda");
                tipoVenda.Text = produto.IndicadorTipoProduto.GetDescription();

                var modalidadeParcela = mvTiposFormularios.GetActiveView() == vNaoRede ? (Repeater)e.Item.FindControl("repModalidadeParcelaEloNaoRede") : (Repeater)e.Item.FindControl("repModalidadeParcelaElo");

                modalidadeParcela.DataSource = from p in Produtos
                                               where (p.CodigoCCA == 70 ||
                                                      p.CodigoCCA == 71) && 
                                                      p.IndicadorTipoProduto == produto.IndicadorTipoProduto
                                               group p by p.NomeFeature into h
                                               select h.First();
                modalidadeParcela.DataBind();
            }
        }

        /// <summary>
        /// Apresentação de dados de Modalidade e Limites de Parcelas da Oferta ELO
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repModalidadeParcelaElo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = e.Item.DataItem as Modelo.Produto;

                var modalidade = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrModalidadeNaoRede") : (Literal)e.Item.FindControl("ltrModalidade");
                var limiteParcela = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrLimiteParcelaNaoRede") : (Literal)e.Item.FindControl("ltrLimiteParcela");
                var prazoTaxa = mvTiposFormularios.GetActiveView() == vNaoRede ? (Repeater)e.Item.FindControl("repPrazoTaxaEloNaoRede") : (Repeater)e.Item.FindControl("repPrazoTaxaElo");

                modalidade.Text = produto.NomeFeature;

                if (produto.IndicadorTipoProduto == Modelo.TipoProduto.Credito && produto.CodigoFeature == 3)
                    limiteParcela.Text = String.Format("2 - {0}", produto.QtdeDefaultParcela.GetValueOrDefault());
                else
                    limiteParcela.Text = "-";

                prazoTaxa.DataSource = from p in Produtos
                                       where (p.CodigoCCA == 70 ||
                                              p.CodigoCCA == 71) &&
                                              p.IndicadorTipoProduto == produto.IndicadorTipoProduto &&
                                              p.NomeFeature == produto.NomeFeature
                                       select p;
                prazoTaxa.DataBind();
            }

        }

        /// <summary>
        /// Apresentação de dados de Prazo e Taxas da Oferta ELO
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repPrazoTaxaElo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Produto produto = e.Item.DataItem as Modelo.Produto;

                var prazoRecebimento = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrPrazoRecebimentoNaoRede") : (Literal)e.Item.FindControl("ltrPrazoRecebimento");
                var taxa = mvTiposFormularios.GetActiveView() == vNaoRede ? (Literal)e.Item.FindControl("ltrTaxaNaoRede") : (Literal)e.Item.FindControl("ltrTaxa");

                prazoRecebimento.Text = produto.ValorPrazoDefault.HasValue ? String.Format("{0} dia(s)", produto.ValorPrazoDefault.Value) : String.Empty;
                taxa.Text = produto.ValorTaxaDefault.GetValueOrDefault() > 0 ? (produto.ValorTaxaDefault.GetValueOrDefault() / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : produto.ValorTaxaDefault.GetValueOrDefault().ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        #region     [ PRIVATE METHODS ]

        /// <summary>
        /// Controle de visibilidadde de campos que não pertencem a Condições Comerciais da Oferta
        /// </summary>
        /// <param name="visivel">Indicador do visibilidade dos campos não pertencentes a Oferta</param>
        private void VisibilidadeCamposNaoCondicoesComerciais(Boolean visivel)
        {
            pnlTipoEquipamentoNaoRede.Visible = visivel;
            pnlTipoEquipamento.Visible = visivel;
            pnlQuantidadeNaoRede.Visible = visivel;
            pnlQuantidade.Visible = visivel;
            pnlCampanhaNaoRede.Visible = visivel;
            pnlCampanha.Visible = visivel;
            pnlCenarioNaoRede.Visible = visivel;
            pnlCenario.Visible = visivel;
            pnlValorMensalAluguelNaoRede.Visible = visivel;
            pnlValorMensalAluguel.Visible = visivel;
            pnlCenarioEscalonamentoNaoRede.Visible = visivel;
            pnlCenarioEscalonamento.Visible = visivel;
        }

        /// <summary>
        /// Controle de visibilidade dos painéis de Condições Comerciais da Oferta
        /// </summary>
        /// <param name="agrupamento">Indicador de tipo de painel de taxa excedente</param>
        /// <param name="nonFlex">Indicador de controle sobre painel não Flex</param>
        /// <param name="flex">Indicador de controle sobre paineis Flex</param>
        private void VisibilidadePaineisCondicoesComerciais(Boolean agrupamento, Boolean nonFlex, Boolean flex, Boolean totais)
        {
            pnlTaxaExcedenteNaoAgrupadoNaoRede.Visible = !agrupamento;
            pnlTaxaExcedenteNaoAgrupadoAmexNaoRede.Visible = !agrupamento;
            pnlTaxaExcedenteNaoAgrupadoEloNaoRede.Visible = !agrupamento;

            pnlTaxaExcedenteNaoAgrupado.Visible = !agrupamento;
            pnlTaxaExcedenteNaoAgrupadoAmex.Visible = !agrupamento;
            pnlTaxaExcedenteNaoAgrupadoElo.Visible = !agrupamento;

            pnlTaxaExcedenteAgrupadoNaoRede.Visible = agrupamento;
            pnlTaxaExcedenteAgrupadoAmexNaoRede.Visible = agrupamento;
            pnlTaxaExcedenteAgrupadoEloNaoRede.Visible = agrupamento;

            pnlTaxaExcedenteAgrupado.Visible = agrupamento;
            pnlTaxaExcedenteAgrupadoAmex.Visible = agrupamento;
            pnlTaxaExcedenteAgrupadoElo.Visible = agrupamento;

            pnlCondicaoComercialFaturamentoNaoRede.Visible = nonFlex;
            pnlCondicaoComercialFaturamento.Visible = nonFlex;
            pnlCondicaoComercialFaturamentoFlexNaoRede.Visible = flex;
            pnlCondicaoComercialFaturamentoFlex.Visible = flex;
            pnlCondicaoComercialFlexNaoRede.Visible = flex;
            pnlCondicaoComercialFlex.Visible = flex;

            pnlTotaisOfertaNaoRede.Visible = totais;
            pnlTotaisOferta.Visible = totais;
        }

        /// <summary>
        /// Realiza a limpeza de campos de dados dos Formulários Alelo e esconde os checkboxes.
        /// </summary>
        private void ResetarPaineisAlelo()
        {
            cbHabilitarVendasAleloAlimentacaoNaoRede.Visible =
                cbHabilitarVendasAleloRefeicaoNaoRede.Visible =
                cbHabilitarVendasAleloRefeicaoNaoRede.Checked =
                phProdutosVendasAleloAlimentacao.Visible =
                phProdutosVendasAleloRefeicao.Visible =
                cbHabilitarVendasAleloAlimentacaoNaoRede.Checked = false;

            ResetFormularioAleloRefeicao();
            ResetFormularioAleloAlimentacao();

            Credenciamento.ProdutoParceiro.Clear();
        }

        /// <summary>
        /// Limpa os campos de dados de Faturamento
        /// </summary>
        private void LimpaCamposFaturamento()
        {
            ltrValorFaturamentoFlexNaoRede.Text = String.Empty;
            ltrValorFaturamentoFlex.Text = String.Empty;
            ltrValorFaturamentoNaoRede.Text = String.Empty;
            ltrValorFaturamento.Text = String.Empty;

            ltrMensalidadeNaoRede.Text = String.Empty;
            ltrMensalidade.Text = String.Empty;
            ltrMensalidadeCarenciaNaoRede.Text = String.Empty;
            ltrMensalidadeCarencia.Text = String.Empty;
            ltrMensalidadePosCarenciaNaoRede.Text = String.Empty;
            ltrMensalidadePosCarencia.Text = String.Empty;
        }

        /// <summary>
        /// Preenche os campos de dados Flex
        /// </summary>
        /// <param name="vendaVista">Descrição de venda a vista</param>
        /// <param name="parcelaPrimeira">Descrição da primeira parcela</param>
        /// <param name="parcelaAdicional">Descrição das parcelas adicionais</param>
        private void PreencheCamposFlex(Decimal vendaVista, Decimal parcelaPrimeira, Decimal parcelaAdicional)
        {
            Literal porcentagemVendaVista = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrVendaVistaNaoRede : ltrVendaVista;
            Literal porcentagemParcelaPrimeira = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrParcelaPrimeiraNaoRede : ltrParcelaPrimeira;
            Literal porcentagemParcelaAdicional = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrParcelaAdicionalNaoRede : ltrParcelaAdicional;

            porcentagemVendaVista.Text = vendaVista > 0 ? (vendaVista / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : vendaVista.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            porcentagemParcelaPrimeira.Text = parcelaPrimeira > 0 ? (parcelaPrimeira / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : parcelaPrimeira.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            porcentagemParcelaAdicional.Text = parcelaAdicional > 0 ? (parcelaAdicional / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : parcelaAdicional.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
        }

        /// <summary>
        /// Preenche os campos de dados Flex sem valores
        /// </summary>
        private void PreencheCamposFlex()
        {
            Literal porcentagemVendaVista = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrVendaVistaNaoRede : ltrVendaVista;
            Literal porcentagemParcelaPrimeira = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrParcelaPrimeiraNaoRede : ltrParcelaPrimeira;
            Literal porcentagemParcelaAdicional = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrParcelaAdicionalNaoRede : ltrParcelaAdicional;

            porcentagemVendaVista.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            porcentagemParcelaPrimeira.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            porcentagemParcelaAdicional.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
        }

        /// <summary>
        /// Carrega o valor total mensal de acordo com a oferta e os serviços selecionados.
        /// </summary>
        private void CarregaTotalMensal()
        {
            Literal valorTotalMensal = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrValorTotalMensalNaoRede : ltrValorTotalMensal;
            Literal valorMensalOferta = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrValorMensalOfertaNaoRede : ltrValorMensalOferta;
            Label totalMensalidade = mvTiposFormularios.GetActiveView() == vNaoRede ? lblTotalMensalidadeNaoRede : lblTotalMensalidadeRede;
            Literal valorTotalServicos = mvTiposFormularios.GetActiveView() == vNaoRede ? ltrValorTotalServicosNaoRede : ltrValorTotalServicos;

            valorTotalServicos.Text = RemoverCaracteresEspeciais(totalMensalidade.Text).ToDouble().ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
            valorTotalMensal.Text = (RemoverCaracteresEspeciais(valorMensalOferta.Text).ToDouble() + RemoverCaracteresEspeciais(totalMensalidade.Text).ToDouble())
                                    .ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
        }
        #endregion  [ PRIVATE METHODS ]
    }
}
