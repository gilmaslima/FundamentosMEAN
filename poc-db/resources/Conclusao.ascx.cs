using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Comum;
using Rede.PN.Credenciamento.Modelo;
using Rede.PN.Credenciamento.Sharepoint.Servicos;
using System.Globalization;

namespace Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento
{

    public partial class Conclusao : UserControlCredenciamentoBase
    {

        /// <summary>
        /// Valor do equipamento que aparece na tela
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
        /// Page Load
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Carrega dados dos controles na tela
        /// </summary>
        public void CarregarControles()
        {
            try
            {
                Int32 numPdv = default(Int32);
                Char situacaoProposta = default(Char);
                String descSituacaoProposta = String.Empty;
                String listaFct = String.Empty;

                Servicos.ServicosWF.RecuperarDadosPveFct(Credenciamento.Proposta, out numPdv, out listaFct, out situacaoProposta, out descSituacaoProposta);
                CarregarDadosConclusao(numPdv, situacaoProposta, descSituacaoProposta, listaFct);

                //Projeto Paperless
                if (situacaoProposta == 'A')
                {
                    // Monta lista de cpfs e cnpjs
                    List<Int64> listaCpfCnpj = new List<Int64>();
                    listaCpfCnpj.Add(Credenciamento.Proposta.NumeroCnpjCpf);
                    listaCpfCnpj.AddRange(Credenciamento.Proprietarios.Select(p => p.NumeroCNPJCPFProprietario).ToList());

                    var listaPvs = Servicos.ServicosGE.ListaPvsPendentes(listaCpfCnpj.ToArray());
                    var debitosPendentes = Servicos.ServicosWF.ConsultaDebitosPendentes(listaPvs.Select(p => (Int32)p.NumeroPV).ToArray());
                    if (debitosPendentes.QuantidadeTotalRegistros > 0)
                    {
                        CarregaTabelaDebitosPendentes(debitosPendentes, numPdv, listaPvs);
                        ExibiPainelDebitosPendentes();
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
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Carrega dados na tela sobre confirmação da proposta
        /// </summary>
        /// <param name="numPdv">Número do PDV - Ponto de Venda </param>
        /// <param name="situacaoProposta">Situação da proposta</param>
        /// <param name="descSituacaoProposta">Descrição da Situação da proposta</param>
        /// <param name="listaFct">Valor texto do número FCT</param>
        private void CarregarDadosConclusao(Int32 numPdv, Char situacaoProposta, String descSituacaoProposta, String listaFct)
        {
            ltlPv.Text = numPdv.ToString();
            ltlNrProposta.Text = Credenciamento.Proposta.NumeroSolicitacao.ToString();
            ltlSituacaoProposta.Text = String.Format("{0} - {1}", situacaoProposta, descSituacaoProposta);
            ltlNumeroFCT.Text = listaFct;

            ltlMensagemConfirmacao.Text = "Proposta recebida com sucesso.";
        }

        /// <summary>
        /// Carrega tabela de Débitos pendentes na tela caso exista
        /// </summary>
        /// <param name="debitosPendentes">Objeto com registros de Débitos Pendentes</param>
        /// <param name="numPdv">Número do PDV - Ponto de Venda </param>
        /// <param name="listaPvs">Lista de PVs Pendentes</param>
        private void CarregaTabelaDebitosPendentes(Servico.DD.ConsultarDetalhamentoDebitosRetorno debitosPendentes, Int32 numPdv, GEPendencias.PvsPendentes[] listaPvs)
        {
            var debitosPendentesGrouped = debitosPendentes.Registros.GroupBy(d => d.EstabelecimentoOrigem);

            if (debitosPendentesGrouped.Count() > 7)
            {
                pnlMais8DebitosPendentes.Visible = true;
                ltlNroEstabelecimentoResumido.Text = numPdv.ToString();
                ltlNomeEstabelecimentoResumido.Text = "-";
                ltlValorResumido.Text = String.Format(@"{0:C}", debitosPendentes.Registros.Sum(d => d.ValorDebito)).Replace("R$", "");
            }
            else
            {
                List<Modelo.DebitosPendentes> debitos = new List<Modelo.DebitosPendentes>();

                foreach (var group in debitosPendentesGrouped)
                {
                    Modelo.DebitosPendentes debito = new Modelo.DebitosPendentes
                    {
                        NumeroEstabelecimento = group.First().EstabelecimentoOrigem,
                        Data = group.First().DataInclusao,
                        Saldo = group.Sum(g => g.ValorDebito)
                    };
                    var pv = listaPvs.Where(e => e.NumeroPV == group.First().EstabelecimentoOrigem).FirstOrDefault();

                    if (pv != null)
                        debito.NomeEstabelecimento = pv.NomeRazaoEstabelecimento;

                    debitos.Add(debito);
                }

                pnlMenos8DebitosPendentes.Visible = true;
                rptDebitosPendentes.DataSource = debitos;
                rptDebitosPendentes.DataBind();
                ltlValorTotal.Text = String.Format(@"{0:C}", debitosPendentes.Registros.Sum(d => d.ValorDebito)).Replace("R$", "");
            }
        }

        /// <summary>
        /// Repeater de Débitos pendentes
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptDebitosPendentes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                ((Literal)e.Item.FindControl("ltlNroEstabelecimento")).Text = ((Modelo.DebitosPendentes)e.Item.DataItem).NumeroEstabelecimento.ToString();
                ((Literal)e.Item.FindControl("ltlNomeEstabelecimento")).Text = ((Modelo.DebitosPendentes)e.Item.DataItem).NomeEstabelecimento;
                ((Literal)e.Item.FindControl("ltlData")).Text = ((Modelo.DebitosPendentes)e.Item.DataItem).Data.ToString("dd/MM/yyyy");
                ((Literal)e.Item.FindControl("ltlSaldo")).Text = String.Format(@"{0:C}", ((Modelo.DebitosPendentes)e.Item.DataItem).Saldo).Replace("R$", "");
            }
        }

        /// <summary>
        /// Exibe painel de Débitos pendentes
        /// </summary>
        private void ExibiPainelDebitosPendentes()
        {
            String script = HttpUtility.HtmlEncode("ExibirPainelDebitosPendentes()");
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
        }

        /// <summary>
        /// Evento que exibe/esconde detalhes da proposta preenchida
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnPropostas_Click(object sender, EventArgs e)
        {
            try
            {
                pnlDadosCompletos.Visible = !pnlDadosCompletos.Visible;
                divBotoes.Attributes.Remove("style");
                if (pnlDadosCompletos.Visible)
                    divBotoes.Attributes.CssStyle.Add(HtmlTextWriterStyle.Width, "749px");
                else
                    divBotoes.Attributes.CssStyle.Add(HtmlTextWriterStyle.Width, "563px");

                if (pnlDadosCompletos.Visible == true)
                {
                    // Dados Cadastrais
                    if (Credenciamento.Proposta.CodigoTipoPessoa.Equals('J'))
                    {
                        pnlCompletoPF.Visible = false;
                        pnlCompletoPJ.Visible = true;

                        lblCPFCNPJ.Text = "CNPJ:";
                        ltlCompletoCPFCNPJ.Text = Credenciamento.Proposta.NumeroCnpjCpf.FormatToCnpj();
                        ltlCompletoCNAE.Text = Credenciamento.Proposta.CodigoCNAE;
                        ltlCompletoRazaoSocial.Text = Credenciamento.Proposta.RazaoSocial;
                        ltlCompletoDataFundacao.Text = Credenciamento.Proposta.DataFundacao.GetValueOrDefault().ToString("dd/MM/yyyy");
                        rptCompletoProprietarios.DataSource = Credenciamento.Proprietarios;
                        rptCompletoProprietarios.DataBind();
                    }
                    else
                    {
                        pnlCompletoPF.Visible = true;
                        pnlCompletoPJ.Visible = false;

                        lblCPFCNPJ.Text = "CPF:";
                        ltlCompletoCPFCNPJ.Text = Credenciamento.Proposta.NumeroCnpjCpf.FormatToCpf();
                        ltlCompletoNomeCompleto.Text = Credenciamento.Proposta.RazaoSocial;
                        ltlCompletoDataNascimento.Text = Credenciamento.Proposta.DataFundacao.GetValueOrDefault().ToString("dd/MM/yyyy");
                    }

                    ltlCompletoRamoAtuacao.Text = Servicos.ServicosGE.GetDescricaoGrupoRamo(Credenciamento.Proposta.CodigoGrupoRamo.GetValueOrDefault());
                    ltlCompletoRamoAtividade.Text = Servicos.ServicosGE.GetDescricaoRamoAtividade(Credenciamento.Proposta.CodigoGrupoRamo.GetValueOrDefault(), Credenciamento.Proposta.CodigoRamoAtividade.GetValueOrDefault());
                    ltlCompletoContato.Text = Credenciamento.Proposta.PessoaContato;
                    ltlCompletoTelefones.Text = String.Format("({0}) {1}", Credenciamento.Proposta.NumeroDDD1.Trim(), Credenciamento.Proposta.NumeroTelefone1.GetValueOrDefault());

                    if (!String.IsNullOrEmpty(Credenciamento.Proposta.NumeroDDD2) && Credenciamento.Proposta.NumeroTelefone2.GetValueOrDefault() > 0)
                        ltlCompletoTelefones.Text = String.Format("{0}<br/><br/>({1}) {2}", ltlCompletoTelefones.Text, Credenciamento.Proposta.NumeroDDD2.Trim(), Credenciamento.Proposta.NumeroTelefone2.GetValueOrDefault());

                    if (!String.IsNullOrEmpty(Credenciamento.Proposta.NumeroDDDFax) && Credenciamento.Proposta.NumeroTelefoneFax.GetValueOrDefault() > 0)
                        ltlCompletoTelefones.Text = String.Format("{0}<br/><br/>({1}) {2}", ltlCompletoTelefones.Text, Credenciamento.Proposta.NumeroDDDFax.Trim(), Credenciamento.Proposta.NumeroTelefoneFax.GetValueOrDefault());
                    ltlCompletoEmail.Text = Credenciamento.Proposta.NomeEmail;
                    ltlCompletoSite.Text = Credenciamento.Proposta.NomeHomePage;

                    // Dados do Cenário
                    Int32 codCenario = Credenciamento.Tecnologia.CodigoCenario.GetValueOrDefault();
                    Int32 codCanal = Credenciamento.Proposta.CodigoCanal.GetValueOrDefault();
                    String codTipoEquipamento = Credenciamento.Tecnologia.CodigoTipoEquipamento;
                    Char codSituacaoCenarioCanal = 'A';
                    String codCampanha = !String.IsNullOrEmpty(Credenciamento.Proposta.CodigoCampanha) ? Credenciamento.Proposta.CodigoCampanha : null;
                    String codOrigemChamada = "Portal";

                    List<Modelo.Cenario> listaCenario = ServicosWF.RecuperarDadosCenario(codCenario, codCanal,
                                                                                         codTipoEquipamento, codSituacaoCenarioCanal, codCampanha, codOrigemChamada,
                                                                                         0, SessaoAtual.LoginUsuario);

                    Modelo.Cenario cenario = null;

                    if (listaCenario != null)
                        cenario = listaCenario.FirstOrDefault();

                    if (cenario != null)
                    {
                        //ltlCompletoAcaoComercial.Text = cenario.DescricaoCenario;

                        ltlCompletoDesconto1.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes1);
                        ltlCompletoDesconto2.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes2);
                        ltlCompletoDesconto3.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes3);
                        ltlCompletoDesconto4.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes4);
                        ltlCompletoDesconto5.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes5);
                        ltlCompletoDesconto6.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes6);
                        ltlCompletoDesconto7.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes7);
                        ltlCompletoDesconto8.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes8);
                        ltlCompletoDesconto9.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes9);
                        ltlCompletoDesconto10.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes10);
                        ltlCompletoDesconto11.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes11);
                        ltlCompletoDesconto12.Text = String.Format("{0}%", cenario.ValorEscalonamentoMes12);

                        Double aluguel = ValorEquipamentoTela;
                        if (aluguel == 0)
                            aluguel = cenario.ValorCenario;

                        ltlCompletoValor1.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes1);
                        ltlCompletoValor2.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes2);
                        ltlCompletoValor3.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes3);
                        ltlCompletoValor4.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes4);
                        ltlCompletoValor5.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes5);
                        ltlCompletoValor6.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes6);
                        ltlCompletoValor7.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes7);
                        ltlCompletoValor8.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes8);
                        ltlCompletoValor9.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes9);
                        ltlCompletoValor10.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes10);
                        ltlCompletoValor11.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes11);
                        ltlCompletoValor12.Text = String.Format("{0:0.00}", cenario.ValorDescontoMes12);
                    }
                    else
                    {
                        ltlCompletoDesconto1.Text = "0%";
                        ltlCompletoDesconto2.Text = "0%";
                        ltlCompletoDesconto3.Text = "0%";
                        ltlCompletoDesconto4.Text = "0%";
                        ltlCompletoDesconto5.Text = "0%";
                        ltlCompletoDesconto6.Text = "0%";
                        ltlCompletoDesconto7.Text = "0%";
                        ltlCompletoDesconto8.Text = "0%";
                        ltlCompletoDesconto9.Text = "0%";
                        ltlCompletoDesconto10.Text = "0%";
                        ltlCompletoDesconto11.Text = "0%";
                        ltlCompletoDesconto12.Text = "0%";

                        ltlCompletoValor1.Text = "0,00";
                        ltlCompletoValor2.Text = "0,00";
                        ltlCompletoValor3.Text = "0,00";
                        ltlCompletoValor4.Text = "0,00";
                        ltlCompletoValor5.Text = "0,00";
                        ltlCompletoValor6.Text = "0,00";
                        ltlCompletoValor7.Text = "0,00";
                        ltlCompletoValor8.Text = "0,00";
                        ltlCompletoValor9.Text = "0,00";
                        ltlCompletoValor10.Text = "0,00";
                        ltlCompletoValor11.Text = "0,00";
                        ltlCompletoValor12.Text = "0,00";
                    }

                    // Dados do PDV
                    ltlCompletoRENPAC.Text = Credenciamento.Tecnologia.NumeroRenpac != 0 ? Credenciamento.Tecnologia.NumeroRenpac.ToString() : String.Empty;
                    ltlCompletoSoftwareTEF.Text = Credenciamento.Tecnologia.NomeFornecedorSoftware;
                    ltlCompletoMarcaPDV.Text = Credenciamento.Tecnologia.NomeFabricanteHardware;

                    var enderecoComercial = Credenciamento.Enderecos.Where(endereco => endereco.IndicadorTipoEndereco == '1').FirstOrDefault();
                    var enderecoCorrespondencia = Credenciamento.Enderecos.Where(endereco => endereco.IndicadorTipoEndereco == '2').FirstOrDefault();
                    var enderecoInstalacao = Credenciamento.Enderecos.Where(endereco => endereco.IndicadorTipoEndereco == '4').FirstOrDefault();

                    // Dados de Endereço
                    ltlCompletoEnderecoComercial.Text = String.Format("{0}, {1}, {2} - CEP {3}, {4}, {5}",
                         enderecoComercial.Logradouro, enderecoComercial.NumeroEndereco,
                        enderecoComercial.ComplementoEndereco, enderecoComercial.CodigoCep,
                        enderecoComercial.Cidade, enderecoComercial.Estado);
                    ltlCompletoEnderecoCorrespondencia.Text = String.Format("{0}, {1}, {2} - CEP {3}, {4}, {5}",
                        enderecoCorrespondencia.Logradouro, enderecoCorrespondencia.NumeroEndereco,
                        enderecoCorrespondencia.ComplementoEndereco, enderecoCorrespondencia.CodigoCep,
                        enderecoCorrespondencia.Cidade, enderecoCorrespondencia.Estado);
                    ltlCompletoEnderecoInstalacao.Text = String.Format("{0}, {1}, {2} - CEP {3}, {4}, {5}",
                        enderecoInstalacao.Logradouro, enderecoInstalacao.NumeroEndereco,
                        enderecoInstalacao.ComplementoEndereco, enderecoInstalacao.CodigoCep,
                        enderecoInstalacao.Cidade, enderecoInstalacao.Estado);
                    ltlCompletoHorarioFuncionamento.Text = String.Format("{0} à {1} - {2} às {3}",
                        Credenciamento.Tecnologia.DiaInicioFuncionamento, Credenciamento.Tecnologia.DiaFimFuncionamento,
                        Credenciamento.Tecnologia.HoraInicioFuncionamento, Credenciamento.Tecnologia.HoraFimFuncionamento);
                    ltlCompletoContatoInstalacao.Text = Credenciamento.Tecnologia.NomeContato;
                    ltlCompletoTelefoneInstalacao.Text = String.Format("({0}) {1}", Credenciamento.Tecnologia.NumeroDDD, Credenciamento.Tecnologia.NumeroTelefone);
                    ltlCompletoObs.Text = Credenciamento.Tecnologia.Observacao;

                    // Dados Operacionais
                    ltlCompletoNomeFatura.Text = Credenciamento.Proposta.NomeFatura;
                    ltlCompletoFuncionamento.Text = Credenciamento.Proposta.CodigoHoraFuncionamentoPV == 0 ? "Comercial" : "Noturno";

                    String tipoEstabelecimento = "Autônomo";
                    if (Credenciamento.Proposta.CodigoTipoEstabelecimento == 2)
                        tipoEstabelecimento = "Matriz";
                    else if (Credenciamento.Proposta.CodigoTipoEstabelecimento == 1)
                        tipoEstabelecimento = "Filial";

                    ltlCompletoTipoEstabelecimento.Text = tipoEstabelecimento;
                    ltlCompletoDataAssinatura.Text = Credenciamento.Proposta.DataCadastroProposta.GetValueOrDefault().ToString("dd/MM/yyyy");
                    ltlCompletoLocalPagamento.Text = Credenciamento.Proposta.CodigoLocalPagamento.GetValueOrDefault() == 1 ? "Estabelecimento" : "Centralizadora - PV Centralizador: " + Credenciamento.Proposta.NumeroCentralizadora.GetValueOrDefault();

                    //Domicílio Bancários
                    DomicilioBancario domBancarioCredito = Credenciamento.DomiciliosBancarios.Where(d => d.TipoDomicilioBancario == TipoDomicilioBancario.Credito).FirstOrDefault();
                    if (domBancarioCredito != null)
                    {
                        ltlCompletoBancoCredito.Text = domBancarioCredito.NomeBanco;
                        ltlCompletoAgenciaCredito.Text = domBancarioCredito.CodigoAgencia.ToString();
                        ltlCompletoContaCorrenteCredito.Text = domBancarioCredito.NumeroContaCorrente;
                    }
                    else
                        pnlDomicilioCredito.Visible = false;

                    DomicilioBancario domBancarioDebito = Credenciamento.DomiciliosBancarios.Where(d => d.TipoDomicilioBancario == TipoDomicilioBancario.Debito).FirstOrDefault();
                    if (domBancarioDebito != null)
                    {
                        ltlCompletoBancoDebito.Text = domBancarioCredito.NomeBanco;
                        ltlCompletoAgenciaDebito.Text = domBancarioCredito.CodigoAgencia.ToString();
                        ltlCompletoContaCorrenteDebito.Text = domBancarioCredito.NumeroContaCorrente;
                    }
                    else
                        pnlDomicilioDebito.Visible = false;

                    DomicilioBancario domBancarioConstrucard = Credenciamento.DomiciliosBancarios.Where(d => d.TipoDomicilioBancario == TipoDomicilioBancario.Construcard).FirstOrDefault();
                    if (domBancarioConstrucard != null)
                    {
                        pnlConstrucard.Visible = true;
                        ltlCompletoBancoConstrucard.Text = domBancarioCredito.NomeBanco;
                        ltlCompletoAgenciaConstrucard.Text = domBancarioCredito.CodigoAgencia.ToString();
                        ltlCompletoContaCorrenteConstrucard.Text = domBancarioCredito.NumeroContaCorrente;
                    }
                    else
                        pnlConstrucard.Visible = false;

                    List<DomicilioBancario> domiciliosBancarioVoucher = Credenciamento.DomiciliosBancarios.Where(d => d.TipoDomicilioBancario == TipoDomicilioBancario.Voucher).ToList();
                    if (domiciliosBancarioVoucher != null && domiciliosBancarioVoucher.Count > 0)
                    {
                        pnlAleloAlimentacao.Visible = domiciliosBancarioVoucher.Any(p => p.CodigoTipoPessoa.Equals('A'));
                        pnlAleloRefeicao.Visible = domiciliosBancarioVoucher.Any(p => p.CodigoTipoPessoa.Equals('R'));

                        domiciliosBancarioVoucher.ForEach(d =>
                        {
                            if (d.CodigoTipoPessoa.Equals('A'))
                            {
                                ltlCompletoBancoAleloAlimentacao.Text = d.NomeBanco;
                                ltlCompletoAgenciaAleloAlimentacao.Text = d.CodigoAgencia.ToString();
                                ltlCompletoContaCorrenteAleloAlimentacao.Text = d.NumeroContaCorrente;
                            }

                            if (d.CodigoTipoPessoa.Equals('R'))
                            {
                                ltlCompletoBancoAleloRefeicao.Text = d.NomeBanco;
                                ltlCompletoAgenciaAleloRefeicao.Text = d.CodigoAgencia.ToString();
                                ltlCompletoContaCorrenteAleloRefeicao.Text = d.NumeroContaCorrente;
                            }
                        });
                    }
                    else
                    {
                        pnlAleloAlimentacao.Visible = false;
                        pnlAleloRefeicao.Visible = false;
                    }

                    // Carrega oferta de preço único da proposta
                    var ofertasCadastradas = ServicosWF.ConsultaOfertaPrecoUnico('C', Credenciamento.Proposta.CodigoTipoPessoa, Credenciamento.Proposta.NumeroCnpjCpf, Credenciamento.Proposta.IndicadorSequenciaProposta, null);

                    this.VisibilidadePaineisCondicoesComerciaisCompleto(true, true, true, true);
                    if (ofertasCadastradas.Count > 0)
                    {
                        // Carrega detalhes da oferta de preco único
                        var oferta = ServicosWF.RecuperarOfertaPadrao(ofertasCadastradas.FirstOrDefault().CodigosOfertaPrecoUnico);

                        if (String.Compare(oferta.Bandeiras.FirstOrDefault().IndicadorProdutoFlex, "S", true) == 0)
                        {
                            this.VisibilidadePaineisCondicoesComerciaisCompleto(true, false, true, false);

                            // Preenche Faturamento
                            ltrValorFaturamentoFlexCompleto.Text = oferta.ValorFaturamento.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
                            ltrMensalidadeCarenciaCompleto.Text = oferta.ValorPrecoUnicoSemFlex.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
                            ltrMensalidadePosCarenciaCompleto.Text = oferta.ValorPrecoUnicoComFlex.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));

                            repCondicaoComercialTecnologiaFlexCompleto.DataSource = oferta.Tecnologias;
                            repCondicaoComercialTecnologiaFlexCompleto.DataBind();

                            if (oferta.Bandeiras.Count > 0)
                            {
                                // Preenche FLEX
                                this.PreencheCamposFlexCompleto(oferta.Bandeiras.FirstOrDefault().PercentualTaxaFlex1,
                                                                oferta.Bandeiras.FirstOrDefault().PercentualTaxaFlex1,
                                                                oferta.Bandeiras.FirstOrDefault().PercentualTaxaFlex2,
                                                                oferta.Bandeiras.FirstOrDefault().QuantidadeDiasPrazo);
                            }
                            else
                                this.PreencheCamposFlexCompleto();
                        }
                        else
                        {
                            this.VisibilidadePaineisCondicoesComerciaisCompleto(true, true, false, false);

                            // Preenche Faturamento
                            ltrValorFaturamentoCompleto.Text = oferta.ValorFaturamento.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
                            ltrMensalidadeCompleto.Text = oferta.ValorPrecoUnicoSemFlex.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));

                            repCondicaoComercialTecnologiaCompleto.DataSource = oferta.Tecnologias;
                            repCondicaoComercialTecnologiaCompleto.DataBind();

                            this.PreencheCamposFlexCompleto();
                        }

                        // Diferente de AMEX e ELO
                        var produtos = from p in Credenciamento.Produtos
                                       where p.CodigoCCA != 69 &&
                                             p.CodigoCCA != 70 &&
                                             p.CodigoCCA != 71 &&
                                             p.IndicadorTipoProduto != TipoProduto.Desconhecido
                                       group p by p.IndicadorTipoProduto into h
                                       select h.First();

                        repTipoVendaCompleto.DataSource = produtos;
                        repTipoVendaCompleto.DataBind();

                        if (produtos.Count() > 0)
                            pnlTaxaExcedenteAgrupadoCompleto.Visible = true;
                        else
                            pnlTaxaExcedenteAgrupadoCompleto.Visible = false;

                        // AMEX
                        var produtosAmex = from p in Credenciamento.Produtos
                                           where p.CodigoCCA == 69 &&
                                                 p.IndicadorTipoProduto != TipoProduto.Desconhecido
                                           group p by p.IndicadorTipoProduto into h
                                           select h.First();

                        repTipoVendaCompletoAmex.DataSource = produtosAmex;
                        repTipoVendaCompletoAmex.DataBind();

                        if (produtosAmex.Count() > 0)
                            pnlTaxaExcedenteAgrupadoCompletoAmex.Visible = true;
                        else
                            pnlTaxaExcedenteAgrupadoCompletoAmex.Visible = false;

                        // ELO
                        var produtosElo = from p in Credenciamento.Produtos
                                          where (p.CodigoCCA == 70 ||
                                                 p.CodigoCCA == 71) &&
                                                 p.IndicadorTipoProduto != TipoProduto.Desconhecido
                                          group p by p.IndicadorTipoProduto into h
                                          select h.First();

                        repTipoVendaCompletoElo.DataSource = produtosElo;
                        repTipoVendaCompletoElo.DataBind();

                        if (produtosElo.Count() > 0)
                            pnlTaxaExcedenteAgrupadoCompletoElo.Visible = true;
                        else
                            pnlTaxaExcedenteAgrupadoCompletoElo.Visible = false;
                    }
                    else
                    {
                        this.VisibilidadePaineisCondicoesComerciaisCompleto(false, false, false, true);

                        this.PreencheCamposFlexCompleto();

                        // Dados Equipamento
                        ltlCompletoEquipamento.Text = Credenciamento.Tecnologia.CodigoTipoEquipamento;
                        ltlCompletoQtde.Text = Credenciamento.Tecnologia.QuantidadeTerminalSolicitado.ToString();
                        ltlCompletoValorAluguel.Text = String.Format(@"{0:c}", ValorEquipamentoTela);
                        ltlCompletoTaxaAdesao.Text = String.Format(@"{0:c}", Credenciamento.Proposta.ValorTaxaAdesao);
                        ltlCompletoAcaoComercial.Text = Credenciamento.Tecnologia.AcaoComercial.ToString();
                        ltlCompletoEvento.Text = Credenciamento.Tecnologia.CodigoEventoEspecial;

                        #region Crédito

                        //Dados Produtos Crédito
                        List<Produto> lstProdutosCredito = Credenciamento.Produtos.Where(c => c.IndicadorTipoProduto == TipoProduto.Credito).ToList();

                        //Produtos Crédito
                        var lstProdutosCreditoFiltrado = lstProdutosCredito.Where(p => p.CodigoCCA != 69 && p.CodigoCCA != 70); // Diferente de AMEX e ELO (Crédito)

                        if (lstProdutosCreditoFiltrado.Count() > 0)
                        {
                            pnlCredito.Visible = true;
                            rptCompletoVendasCredito.DataSource = lstProdutosCreditoFiltrado;
                            rptCompletoVendasCredito.DataBind();
                        }
                        else
                        {
                            pnlCredito.Visible = false;
                        }

                        //Produtos Crédito Amex
                        var lstProdutosCreditoAmexFiltrado = lstProdutosCredito.Where(p => p.CodigoCCA == 69); // AMEX (Crédito)

                        if (lstProdutosCreditoAmexFiltrado.Count() > 0)
                        {
                            pnlTaxaExcedenteNaoAgrupadoCompletoAmex.Visible = true;
                            pnlCreditoAmex.Visible = true;
                            rptCompletoVendasCreditoAmex.DataSource = lstProdutosCreditoAmexFiltrado;
                            rptCompletoVendasCreditoAmex.DataBind();
                        }
                        else
                        {
                            pnlTaxaExcedenteNaoAgrupadoCompletoAmex.Visible = false;
                            pnlCreditoAmex.Visible = false;
                        }

                        //Produtos Crédito Elo
                        var lstProdutosCreditoEloFiltrado = lstProdutosCredito.Where(p => p.CodigoCCA == 70); // ELO (Crédito)

                        if (lstProdutosCreditoEloFiltrado.Count() > 0)
                        {
                            pnlTaxaExcedenteNaoAgrupadoCompletoElo.Visible = true;
                            pnlCreditoElo.Visible = true;
                            rptCompletoVendasCreditoElo.DataSource = lstProdutosCreditoEloFiltrado;
                            rptCompletoVendasCreditoElo.DataBind();
                        }
                        else
                        {
                            pnlTaxaExcedenteNaoAgrupadoCompletoElo.Visible = false;
                            pnlCreditoElo.Visible = false;
                        }

                        #endregion

                        #region Débito

                        //Produtos Débito
                        List<Produto> lstProdutosDebito = Credenciamento.Produtos.Where(c => c.IndicadorTipoProduto == TipoProduto.Debito).ToList();

                        var lstProdutosDebitoFiltrado = lstProdutosDebito.Where(p => p.CodigoCCA != 71); // Diferente de ELO (Débito)

                        if (lstProdutosDebitoFiltrado.Count() > 0)
                        {
                            pnlDebito.Visible = true;
                            rptCompletoVendasDebito.DataSource = lstProdutosDebitoFiltrado;
                            rptCompletoVendasDebito.DataBind();
                        }
                        else
                        {
                            pnlDebito.Visible = false;
                        }

                        //Produtos Débito Elo

                        var lstProdutosDebitoEloFiltrado = lstProdutosDebito.Where(p => p.CodigoCCA == 71); // ELO (Débito)

                        if (lstProdutosDebitoEloFiltrado.Count() > 0)
                        {
                            pnlDebitoElo.Visible = true;
                            rptCompletoVendasDebitoElo.DataSource = lstProdutosDebitoEloFiltrado;
                            rptCompletoVendasDebitoElo.DataBind();
                        }
                        else
                        {
                            pnlDebitoElo.Visible = false;
                        }

                        #endregion

                        //Produtos Construcard 
                        List<Produto> lstProdutosConstrucard = Credenciamento.Produtos.Where(c => c.IndicadorTipoProduto == TipoProduto.Construcard).ToList();
                        if (lstProdutosConstrucard != null && lstProdutosConstrucard.Count > 0)
                        {
                            rptCompletoVendasConstrucard.DataSource = lstProdutosConstrucard;
                            rptCompletoVendasConstrucard.DataBind();
                        }

                    }
                    //Dados Serviços
                    if (String.Compare(Credenciamento.Tecnologia.CodigoTipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.Tecnologia.CodigoTipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.Tecnologia.CodigoTipoEquipamento, "TOF") == 0)
                    {
                        ltlHeaderValor.Text = "Valor (mensal)";
                    }

                    if (Credenciamento.Servicos.Count > 0)
                    {
                        pnlServicos.Visible = true;
                        rptServicos.DataSource = Credenciamento.Servicos;
                        rptServicos.DataBind();
                    }
                    else
                        pnlServicos.Visible = false;

                    //Dados Produtos Van
                    List<Produto> lstProdutosVan = Credenciamento.Produtos.Where(c => c.ProdutoVAN == true).ToList();
                    if (lstProdutosVan != null && lstProdutosVan.Count > 0)
                    {
                        pnlProdutosVan.Visible = true;
                        rptProdutosVan.DataSource = lstProdutosVan;
                        rptProdutosVan.DataBind();
                    }
                    else
                        pnlProdutosVan.Visible = false;

                    List<Produto> prodVoucher = Credenciamento.Produtos.Where(p => p.IndicadorTipoProduto == TipoProduto.Voucher).ToList();

                    if (prodVoucher != null && prodVoucher.Count > 0)
                    {
                        pnlProdutosVoucher.Visible = true;
                        rptProdutosVoucher.DataSource = prodVoucher;
                        rptProdutosVoucher.DataBind();
                    }
                    else
                        pnlProdutosVoucher.Visible = false;
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
        /// Repeater de dados sobre vendas crédito - detalhes da proposta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptCompletoVendasCredito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                Literal ltlDe = (Literal)e.Item.FindControl("ltlDe");
                Literal ltlAte = (Literal)e.Item.FindControl("ltlAte");
                //Literal ltlLimiteParcela = (Literal)e.Item.FindControl("ltlLimiteParcelas");
                //Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                Produto item = (Produto)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                ltlDe.Text = "-";
                ltlAte.Text = "-";
                //ltlLimiteParcela.Text = String.Empty;
                //ltlFormaPagto.Text = item.IndicadorFormaPagamento == 'T' ? "Tarifa" : "Taxa";

                Int32 codCca = (Int32)item.CodigoCCA;
                Int32 codFeature = (Int32)item.CodigoFeature;

                if (item.Patamares != null && item.Patamares.Count() > 0)
                {
                    if (item.Patamares.Count > 0)
                    {
                        ((Literal)e.Item.FindControl("ltlDe")).Text = item.Patamares[0].PatamarInicial.ToString();
                        ((Literal)e.Item.FindControl("ltlAte")).Text = item.Patamares[0].PatamarFinal.ToString();
                        //ltlLimiteParcela.Text = item.QtdeMaximaParcela.ToString();

                        if (item.Patamares.Count > 1)
                        {
                            Panel pnlPatamar1 = (Panel)e.Item.FindControl("pnlPatamar1");
                            pnlPatamar1.Visible = true;

                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1PrazoRecebimento")).Text = String.Format("{0} dia(s)", item.Patamares[1].Prazo.ToString());
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Taxa")).Text = String.Format(@"{0:f2}", item.Patamares[1].TaxaPatamar.ToString());
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1De")).Text = item.Patamares[1].PatamarInicial.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Ate")).Text = item.Patamares[1].PatamarFinal.ToString();
                            //((Literal)pnlPatamar1.FindControl("ltlPatamar1LimiteParcela")).Text = item.QtdeMaximaParcela.ToString();
                            //((Literal)pnlPatamar1.FindControl("ltlPatamar1FormaPagamento")).Text = item.IndicadorFormaPagamento == 'T' ? "Tarifa" : "Taxa";
                        }

                        if (item.Patamares.Count > 2)
                        {
                            Panel pnlPatamar2 = (Panel)e.Item.FindControl("pnlPatamar2");
                            pnlPatamar2.Visible = true;

                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2PrazoRecebimento")).Text = String.Format("{0} dia(s)", item.Patamares[2].Prazo.ToString());
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Taxa")).Text = String.Format(@"{0:f2}", item.Patamares[2].TaxaPatamar.ToString());
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2De")).Text = item.Patamares[2].PatamarInicial.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Ate")).Text = item.Patamares[2].PatamarFinal.ToString();
                            //((Literal)pnlPatamar2.FindControl("ltlPatamar2LimiteParcela")).Text = item.QtdeMaximaParcela.ToString();
                            //((Literal)pnlPatamar2.FindControl("ltlPatamar2FormaPagamento")).Text = item.IndicadorFormaPagamento == 'T' ? "Tarifa" : "Taxa";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Repeater de dados sobre vendas crédito - detalhes da proposta Amex
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptCompletoVendasCreditoAmex_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                Literal ltlDe = (Literal)e.Item.FindControl("ltlDe");
                Literal ltlAte = (Literal)e.Item.FindControl("ltlAte");
                //Literal ltlLimiteParcela = (Literal)e.Item.FindControl("ltlLimiteParcelas");
                //Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                Produto item = (Produto)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                ltlDe.Text = "-";
                ltlAte.Text = "-";
                //ltlLimiteParcela.Text = String.Empty;
                //ltlFormaPagto.Text = item.IndicadorFormaPagamento == 'T' ? "Tarifa" : "Taxa";

                Int32 codCca = (Int32)item.CodigoCCA;
                Int32 codFeature = (Int32)item.CodigoFeature;

                if (item.Patamares != null && item.Patamares.Count() > 0)
                {
                    if (item.Patamares.Count > 0)
                    {
                        ((Literal)e.Item.FindControl("ltlDe")).Text = item.Patamares[0].PatamarInicial.ToString();
                        ((Literal)e.Item.FindControl("ltlAte")).Text = item.Patamares[0].PatamarFinal.ToString();
                        //ltlLimiteParcela.Text = item.QtdeMaximaParcela.ToString();

                        if (item.Patamares.Count > 1)
                        {
                            Panel pnlPatamar1 = (Panel)e.Item.FindControl("pnlPatamar1");
                            pnlPatamar1.Visible = true;

                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1PrazoRecebimento")).Text = String.Format("{0} dia(s)", item.Patamares[1].Prazo.ToString());
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Taxa")).Text = String.Format(@"{0:f2}", item.Patamares[1].TaxaPatamar.ToString());
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1De")).Text = item.Patamares[1].PatamarInicial.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Ate")).Text = item.Patamares[1].PatamarFinal.ToString();
                            //((Literal)pnlPatamar1.FindControl("ltlPatamar1LimiteParcela")).Text = item.QtdeMaximaParcela.ToString();
                            //((Literal)pnlPatamar1.FindControl("ltlPatamar1FormaPagamento")).Text = item.IndicadorFormaPagamento == 'T' ? "Tarifa" : "Taxa";
                        }

                        if (item.Patamares.Count > 2)
                        {
                            Panel pnlPatamar2 = (Panel)e.Item.FindControl("pnlPatamar2");
                            pnlPatamar2.Visible = true;

                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2PrazoRecebimento")).Text = String.Format("{0} dia(s)", item.Patamares[2].Prazo.ToString());
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Taxa")).Text = String.Format(@"{0:f2}", item.Patamares[2].TaxaPatamar.ToString());
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2De")).Text = item.Patamares[2].PatamarInicial.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Ate")).Text = item.Patamares[2].PatamarFinal.ToString();
                            //((Literal)pnlPatamar2.FindControl("ltlPatamar2LimiteParcela")).Text = item.QtdeMaximaParcela.ToString();
                            //((Literal)pnlPatamar2.FindControl("ltlPatamar2FormaPagamento")).Text = item.IndicadorFormaPagamento == 'T' ? "Tarifa" : "Taxa";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Repeater de dados sobre vendas crédito - detalhes da proposta Elo
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptCompletoVendasCreditoElo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                Literal ltlDe = (Literal)e.Item.FindControl("ltlDe");
                Literal ltlAte = (Literal)e.Item.FindControl("ltlAte");
                //Literal ltlLimiteParcela = (Literal)e.Item.FindControl("ltlLimiteParcelas");
                //Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                Produto item = (Produto)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                ltlDe.Text = "-";
                ltlAte.Text = "-";
                //ltlLimiteParcela.Text = String.Empty;
                //ltlFormaPagto.Text = item.IndicadorFormaPagamento == 'T' ? "Tarifa" : "Taxa";

                Int32 codCca = (Int32)item.CodigoCCA;
                Int32 codFeature = (Int32)item.CodigoFeature;

                if (item.Patamares != null && item.Patamares.Count() > 0)
                {
                    if (item.Patamares.Count > 0)
                    {
                        ((Literal)e.Item.FindControl("ltlDe")).Text = item.Patamares[0].PatamarInicial.ToString();
                        ((Literal)e.Item.FindControl("ltlAte")).Text = item.Patamares[0].PatamarFinal.ToString();
                        //ltlLimiteParcela.Text = item.QtdeMaximaParcela.ToString();

                        if (item.Patamares.Count > 1)
                        {
                            Panel pnlPatamar1 = (Panel)e.Item.FindControl("pnlPatamar1");
                            pnlPatamar1.Visible = true;

                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1PrazoRecebimento")).Text = String.Format("{0} dia(s)", item.Patamares[1].Prazo.ToString());
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Taxa")).Text = String.Format(@"{0:f2}", item.Patamares[1].TaxaPatamar.ToString());
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1De")).Text = item.Patamares[1].PatamarInicial.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Ate")).Text = item.Patamares[1].PatamarFinal.ToString();
                            //((Literal)pnlPatamar1.FindControl("ltlPatamar1LimiteParcela")).Text = item.QtdeMaximaParcela.ToString();
                            //((Literal)pnlPatamar1.FindControl("ltlPatamar1FormaPagamento")).Text = item.IndicadorFormaPagamento == 'T' ? "Tarifa" : "Taxa";
                        }

                        if (item.Patamares.Count > 2)
                        {
                            Panel pnlPatamar2 = (Panel)e.Item.FindControl("pnlPatamar2");
                            pnlPatamar2.Visible = true;

                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2PrazoRecebimento")).Text = String.Format("{0} dia(s)", item.Patamares[2].Prazo.ToString());
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Taxa")).Text = String.Format(@"{0:f2}", item.Patamares[2].TaxaPatamar.ToString());
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2De")).Text = item.Patamares[2].PatamarInicial.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Ate")).Text = item.Patamares[2].PatamarFinal.ToString();
                            //((Literal)pnlPatamar2.FindControl("ltlPatamar2LimiteParcela")).Text = item.QtdeMaximaParcela.ToString();
                            //((Literal)pnlPatamar2.FindControl("ltlPatamar2FormaPagamento")).Text = item.IndicadorFormaPagamento == 'T' ? "Tarifa" : "Taxa";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Repeater de dados sobre vendas Débito - detalhes da proposta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptCompletoVendasDebito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                //Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                Produto item = (Produto)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                //ltlFormaPagto.Text = item.IndicadorFormaPagamento == 'X' ? "Taxa" : "Tarifa";
            }
        }

        /// <summary>
        /// Repeater de dados sobre vendas Débito - detalhes da proposta Elo
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptCompletoVendasDebitoElo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                //Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                Produto item = (Produto)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                //ltlFormaPagto.Text = item.IndicadorFormaPagamento == 'X' ? "Taxa" : "Tarifa";
            }
        }

        /// <summary>
        /// Repeater de dados sobre vendas Construcard - detalhes da proposta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptCompletoVendasConstrucard_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                //Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                Produto item = (Produto)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                //ltlFormaPagto.Text = item.IndicadorFormaPagamento == 'X' ? "Taxa" : "Tarifa";
            }
        }

        /// <summary>
        /// Repeater de dados sobre Serviços - detalhes da proposta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptServicos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCodigoServico = (Literal)e.Item.FindControl("ltlCodigoServico");
                Literal ltlNomeServico = (Literal)e.Item.FindControl("ltlNomeServico");
                Literal ltlCodigoRegime = (Literal)e.Item.FindControl("ltlCodigoRegime");
                Literal ltlQtde = (Literal)e.Item.FindControl("ltlQtde");
                Literal ltlValor = (Literal)e.Item.FindControl("ltlValor");
                Literal ltlExcedente = (Literal)e.Item.FindControl("ltlExcedente");

                Modelo.Servico item = (Modelo.Servico)e.Item.DataItem;

                ltlCodigoServico.Text = item.CodigoServico.ToString();
                ltlNomeServico.Text = item.DescricaoServico;
                ltlCodigoRegime.Text = item.CodigoRegimeServico.ToString();
                ltlQtde.Text = item.QuantidadeMinimaConsulta.ToString();
                ltlValor.Text = String.Format("{0:C}", item.ValorFranquia);
                ltlExcedente.Text = String.Format("{0:C}", 0);
            }
        }

        /// <summary>
        /// Repeater de dados dos proprietários
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptProprietarios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlTituloProprietario = (Literal)e.Item.FindControl("ltlTituloProprietario");
                Literal ltlProprietario = (Literal)e.Item.FindControl("ltlProprietario");

                ltlTituloProprietario.Visible = (e.Item.ItemIndex == 0);

                Modelo.Proprietario objProprietario = ((Modelo.Proprietario)e.Item.DataItem);

                //Formatar dados CNPJ CPF
                string strCnpjCpf = String.Empty;
                if (objProprietario.CodigoTipoPesssoaProprietario == TipoPessoa.Juridica)
                    strCnpjCpf = objProprietario.NumeroCNPJCPFProprietario.FormatToCnpj();
                else
                    strCnpjCpf = objProprietario.NumeroCNPJCPFProprietario.FormatToCpf();

                String proprietario = String.Format("{0} - {1} - {2}% de participação", strCnpjCpf, objProprietario.NomeProprietario, objProprietario.ParticipacaoAcionaria, "%");
                ltlProprietario.Text = proprietario;
            }
        }

        /// <summary>
        /// Data Bound da tabela de serviços
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptProdutosVan_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                {
                    Modelo.Produto item = (Modelo.Produto)e.Item.DataItem;

                    ((Literal)e.Item.FindControl("ltlCodigo")).Text = item.CodigoCCA.ToString();
                    ((Literal)e.Item.FindControl("ltlDescricao")).Text = item.NomeFeature;
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
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Data Bound da tabela de Produtos Voucher
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptProdutosVoucher_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                {
                    Produto item = (Produto)e.Item.DataItem;

                    ((Literal)e.Item.FindControl("ltlCodigo")).Text = item.CodigoFeature.ToString();
                    ((Literal)e.Item.FindControl("ltlDescricao")).Text = item.NomeFeature;
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
                Logger.GravarErro("Credenciamento - Contratação de Produtos Voucher", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        public event EventHandler NovaProposta;

        /// <summary>
        /// Evento para iniciar o preenchimento de uma nova proposta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnNovaProposta_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Iniciais - Continuar"))
            {
                try
                {
                    Credenciamento = null;
                    NovaProposta(sender, e);
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
        /// Apresentação de dados de Tecnologia da Oferta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repCondicaoComercialTecnologiaCompleto_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var tecnologia = (WFOfertas.TecnologiaPadrao)e.Item.DataItem;

                var ltrQuantidade = (Literal)e.Item.FindControl("ltrQuantidadeCompleto");
                var ltrTipo = (Literal)e.Item.FindControl("ltrTipoCompleto");

                ltrQuantidade.Text = tecnologia.QuantidadeEquipamento.ToString();
                ltrTipo.Text = tecnologia.CodigoEquipamento;
            }
        }

        private void VisibilidadePaineisCondicoesComerciaisCompleto(Boolean agrupamento, Boolean nonFlex, Boolean flex, Boolean naoCondicaoComercial)
        {
            pnlTaxaExcedenteNaoAgrupadoCompleto.Visible = !agrupamento;
            pnlTaxaExcedenteNaoAgrupadoCompletoAmex.Visible = !agrupamento;
            pnlTaxaExcedenteNaoAgrupadoCompletoElo.Visible = !agrupamento;

            pnlTaxaExcedenteAgrupadoCompleto.Visible = agrupamento;
            pnlTaxaExcedenteAgrupadoCompletoAmex.Visible = agrupamento;
            pnlTaxaExcedenteAgrupadoCompletoElo.Visible = agrupamento;

            pnlCondicaoComercialFaturamentoCompleto.Visible = nonFlex;

            pnlNaoCondicaoComercialFaturamentoCompleto.Visible = naoCondicaoComercial;
            pnlCondicaoComercialFaturamentoFlexCompleto.Visible = flex;
            pnlCondicaoComercialFlexCompleto.Visible = flex;
        }

        /// <summary>
        /// Apresentação de dados de Tipo de Venda da Oferta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repTipoVendaCompleto_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = (Modelo.Produto)e.Item.DataItem;

                var tipoVenda = (Literal)e.Item.FindControl("ltrTipoVendaCompleto");
                tipoVenda.Text = produto.IndicadorTipoProduto.GetDescription();

                var modalidadeParcela = (Repeater)e.Item.FindControl("repModalidadeParcelaCompleto");
                modalidadeParcela.DataSource = from p in Credenciamento.Produtos
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
        protected void repModalidadeParcelaCompleto_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = (Modelo.Produto)e.Item.DataItem;

                var modalidade = (Literal)e.Item.FindControl("ltrModalidadeCompleto");
                var limiteParcela = (Literal)e.Item.FindControl("ltrLimiteParcelaCompleto");
                var prazoTaxa = (Repeater)e.Item.FindControl("repPrazoTaxaCompleto");

                modalidade.Text = produto.NomeFeature;
                if (produto.IndicadorTipoProduto == Modelo.TipoProduto.Credito && produto.CodigoFeature == 3)
                    limiteParcela.Text = String.Format("2 - {0}", produto.QtdeMaximaParcela.GetValueOrDefault());
                else
                    limiteParcela.Text = "-";

                prazoTaxa.DataSource = from p in Credenciamento.Produtos
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
        protected void repPrazoTaxaCompleto_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = (Modelo.Produto)e.Item.DataItem;

                var prazoRecebimento = (Literal)e.Item.FindControl("ltrPrazoRecebimentoCompleto");
                var taxa = (Literal)e.Item.FindControl("ltrTaxaCompleto");

                prazoRecebimento.Text = produto.ValorPrazoDefault.HasValue ? String.Format("{0} dia(s)", produto.ValorPrazoDefault.Value) : String.Empty;
                taxa.Text = produto.ValorTaxaDefault.GetValueOrDefault() > 0 ? (produto.ValorTaxaDefault.GetValueOrDefault() / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : produto.ValorTaxaDefault.GetValueOrDefault().ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        /// <summary>
        /// Apresentação de dados de Tipo de Venda da Oferta Amex
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repTipoVendaCompletoAmex_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = (Modelo.Produto)e.Item.DataItem;

                var tipoVenda = (Literal)e.Item.FindControl("ltrTipoVendaCompleto");
                tipoVenda.Text = produto.IndicadorTipoProduto.GetDescription();

                var modalidadeParcela = (Repeater)e.Item.FindControl("repModalidadeParcelaCompletoAmex");

                modalidadeParcela.DataSource = from p in Credenciamento.Produtos
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
        protected void repModalidadeParcelaCompletoAmex_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = (Modelo.Produto)e.Item.DataItem;

                var modalidade = (Literal)e.Item.FindControl("ltrModalidadeCompleto");
                var limiteParcela = (Literal)e.Item.FindControl("ltrLimiteParcelaCompleto");
                var prazoTaxa = (Repeater)e.Item.FindControl("repPrazoTaxaCompletoAmex");

                modalidade.Text = produto.NomeFeature;
                if (produto.IndicadorTipoProduto == Modelo.TipoProduto.Credito && produto.CodigoFeature == 3)
                    limiteParcela.Text = String.Format("2 - {0}", produto.QtdeMaximaParcela.GetValueOrDefault());
                else
                    limiteParcela.Text = "-";

                prazoTaxa.DataSource = from p in Credenciamento.Produtos
                                       where p.CodigoCCA == 69 &&
                                             p.IndicadorTipoProduto == produto.IndicadorTipoProduto &&
                                             p.NomeFeature == produto.NomeFeature
                                       select p;
                prazoTaxa.DataBind();
            }
        }

        /// <summary>
        /// Apresentação de dados de Prazo e Taxas da Oferta Amex
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repPrazoTaxaCompletoAmex_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = (Modelo.Produto)e.Item.DataItem;

                var prazoRecebimento = (Literal)e.Item.FindControl("ltrPrazoRecebimentoCompleto");
                var taxa = (Literal)e.Item.FindControl("ltrTaxaCompleto");

                prazoRecebimento.Text = produto.ValorPrazoDefault.HasValue ? String.Format("{0} dia(s)", produto.ValorPrazoDefault.Value) : String.Empty;
                taxa.Text = produto.ValorTaxaDefault.GetValueOrDefault() > 0 ? (produto.ValorTaxaDefault.GetValueOrDefault() / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : produto.ValorTaxaDefault.GetValueOrDefault().ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        /// <summary>
        /// Apresentação de dados de Tipo de Venda da Oferta Elo
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repTipoVendaCompletoElo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = (Modelo.Produto)e.Item.DataItem;

                var tipoVenda = (Literal)e.Item.FindControl("ltrTipoVendaCompleto");
                tipoVenda.Text = produto.IndicadorTipoProduto.GetDescription();

                var modalidadeParcela = (Repeater)e.Item.FindControl("repModalidadeParcelaCompletoElo");
                modalidadeParcela.DataSource = from p in Credenciamento.Produtos
                                               where (p.CodigoCCA == 70 ||
                                                      p.CodigoCCA == 71) && 
                                                      p.IndicadorTipoProduto == produto.IndicadorTipoProduto
                                               group p by p.NomeFeature into h
                                               select h.First();
                modalidadeParcela.DataBind();
            }
        }

        /// <summary>
        /// Apresentação de dados de Modalidade e Limites de Parcelas da Oferta Elo
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repModalidadeParcelaCompletoElo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = (Modelo.Produto)e.Item.DataItem;

                var modalidade = (Literal)e.Item.FindControl("ltrModalidadeCompleto");
                var limiteParcela = (Literal)e.Item.FindControl("ltrLimiteParcelaCompleto");
                var prazoTaxa = (Repeater)e.Item.FindControl("repPrazoTaxaCompletoElo");

                modalidade.Text = produto.NomeFeature;
                if (produto.IndicadorTipoProduto == Modelo.TipoProduto.Credito && produto.CodigoFeature == 3)
                    limiteParcela.Text = String.Format("2 - {0}", produto.QtdeMaximaParcela.GetValueOrDefault());
                else
                    limiteParcela.Text = "-";

                prazoTaxa.DataSource = from p in Credenciamento.Produtos
                                       where (p.CodigoCCA == 70 ||
                                              p.CodigoCCA == 71) && 
                                              p.IndicadorTipoProduto == produto.IndicadorTipoProduto &&
                                              p.NomeFeature == produto.NomeFeature
                                       select p;
                prazoTaxa.DataBind();
            }
        }

        /// <summary>
        /// Apresentação de dados de Prazo e Taxas da Oferta Elo
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void repPrazoTaxaCompletoElo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = (Modelo.Produto)e.Item.DataItem;

                var prazoRecebimento = (Literal)e.Item.FindControl("ltrPrazoRecebimentoCompleto");
                var taxa = (Literal)e.Item.FindControl("ltrTaxaCompleto");

                prazoRecebimento.Text = produto.ValorPrazoDefault.HasValue ? String.Format("{0} dia(s)", produto.ValorPrazoDefault.Value) : String.Empty;
                taxa.Text = produto.ValorTaxaDefault.GetValueOrDefault() > 0 ? (produto.ValorTaxaDefault.GetValueOrDefault() / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : produto.ValorTaxaDefault.GetValueOrDefault().ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        /// <summary>
        /// Preenche os campos de dados Flex
        /// </summary>
        /// <param name="vendaVista">Descrição de venda a vista</param>
        /// <param name="parcelaPrimeira">Descrição da primeira parcela</param>
        /// <param name="parcelaAdicional">Descrição das parcelas adicionais</param>
        private void PreencheCamposFlexCompleto(Decimal vendaVista, Decimal parcelaPrimeira, Decimal parcelaAdicional, Decimal diasUteis)
        {
            ltrVendaVistaCompleto.Text = vendaVista > 0 ? (vendaVista / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : vendaVista.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaPrimeiraCompleto.Text = parcelaPrimeira > 0 ? (parcelaPrimeira / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : parcelaPrimeira.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaAdicionalCompleto.Text = parcelaAdicional > 0 ? (parcelaAdicional / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : parcelaAdicional.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrDiasUteisCompleto.Text = diasUteis.ToString();
        }

        /// <summary>
        /// Preenche os campos de dados Flex sem valores
        /// </summary>
        private void PreencheCamposFlexCompleto()
        {
            ltrVendaVistaCompleto.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaPrimeiraCompleto.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaAdicionalCompleto.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrDiasUteisCompleto.Text = default(Decimal).ToString();
        }
    }
}
