using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using System.ServiceModel;
using Redecard.PN.Credenciamento.Sharepoint.TGCenarios;
using System.Linq;
using Redecard.PN.Credenciamento.Sharepoint.GEProdutos;
using System.Collections.Generic;
using Redecard.PN.Credenciamento.Sharepoint.Modelo;
using System.Web;
using Redecard.PN.Credenciamento.Sharepoint.Servico.DD;
using Redecard.PN.Credenciamento.Sharepoint.GERamosAtd;
using Redecard.PN.Credenciamento.Sharepoint.GEPendencias;
using Redecard.PN.Credenciamento.Sharepoint.TGFichaTec;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.Conclusao
{
    public partial class ConclusaoUserControl : UserControlCredenciamentoBase
    {
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    Int32 numPdv;
                    Char situacaoProposta;
                    ConsPropCredenciamentoPendente(out numPdv, out situacaoProposta);
                    ltlNumeroFCT.Text = ListaFCTInstalacaoAtivaPorPV(numPdv, null);

                    //Projeto Paperless
                    if (situacaoProposta == 'A')
                    {
                        // Monta lista de cpfs e cnpjs
                        List<Int64> listaCpfCnpj = new List<Int64>();
                        if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                            listaCpfCnpj.Add(Credenciamento.CNPJ.CpfCnpjToLong());
                        else
                            listaCpfCnpj.Add(Credenciamento.CPF.CpfCnpjToLong());
                        listaCpfCnpj.AddRange(Credenciamento.Proprietarios.Select(p => p.CPF_CNPJ.CpfCnpjToLong()).ToList());

                        var listaPvs = ListaPvsPendentes(listaCpfCnpj.ToArray());
                        var debitosPendentes = ConsultaDebitosPendentes(listaPvs.Select(p => (Int32)p.NumeroPV).ToArray());
                        if (debitosPendentes.QuantidadeTotalRegistros > 0)
                        {
                            CarregaTabelaDebitosPendentes(debitosPendentes, numPdv, listaPvs);
                            ExibiPainelDebitosPendentes();
                        }
                    }
                }
            }
            catch (FaultException<Servico.DD.GeneralFault> fe)
            {
                Logger.GravarErro("Credenciamento - Conclusão", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Conclusão", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Conclusão", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Conclusão", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Conclusão", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        private void CarregaTabelaDebitosPendentes(ConsultarDetalhamentoDebitosRetorno debitosPendentes, Int32 numPdv, PvsPendentes[] listaPvs)
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
                List<DebitosPendentes> debitos = new List<DebitosPendentes>();

                foreach (var group in debitosPendentesGrouped)
                {
                    DebitosPendentes debito = new DebitosPendentes
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

        protected void rptDebitosPendentes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                ((Literal)e.Item.FindControl("ltlNroEstabelecimento")).Text = ((DebitosPendentes)e.Item.DataItem).NumeroEstabelecimento.ToString();
                ((Literal)e.Item.FindControl("ltlNomeEstabelecimento")).Text = ((DebitosPendentes)e.Item.DataItem).NomeEstabelecimento;
                ((Literal)e.Item.FindControl("ltlData")).Text = ((DebitosPendentes)e.Item.DataItem).Data.ToString("dd/MM/yyyy");
                ((Literal)e.Item.FindControl("ltlSaldo")).Text = String.Format(@"{0:C}", ((DebitosPendentes)e.Item.DataItem).Saldo).Replace("R$", "");
                //((Literal)e.Item.FindControl("ltlMotivoDescricao")).Text = ((ConsultarDetalhamentoDebitosDetalheRetorno)e.Item.DataItem).MotivoDebito;
            }
        }

        private ConsultarDetalhamentoDebitosRetorno ConsultaDebitosPendentes(Int32[] numPdvs)
        {
            ConsultarDetalhamentoDebitosRetorno retorno = new ConsultarDetalhamentoDebitosRetorno();
            StatusRetorno statusRetorno = new StatusRetorno();
            ConsultarDetalhamentoDebitosEnvio envio = new ConsultarDetalhamentoDebitosEnvio
            {
                Estabelecimentos = numPdvs,
                Versao = Servico.DD.VersaoDebitoDesagendamento.ISF,
                DataFinal = DateTime.Now,
                DataInicial = DateTime.Now,
                ChavePesquisa = null,
                CodigoBandeira = 0,
                TipoPesquisa = "P",
            };

            using (Logger log = Logger.IniciarLog("Consulta Débitos Pendentes por PV"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPdvs,
                    envio
                });

                using (var contexto = new ContextoWCF<RelatorioDebitosDesagendamentosClient>())
                {
                    retorno = contexto.Cliente.ConsultarDetalhamentoDebitosPesquisa(out statusRetorno, envio, 1, 500, Guid.NewGuid(), Guid.NewGuid());
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno,
                    statusRetorno
                });
            }

            return retorno;
        }

        private PvsPendentes[] ListaPvsPendentes(Int64[] listaCpfCnpj)
        {
            PvsPendentes[] listaPvs;

            using (Logger log = Logger.IniciarLog("Lista PVs Pendentes"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    listaCpfCnpj
                });

                using (var contexto = new ContextoWCF<ServicoPortalGEConsultaPendenciasPVPorCpfCnpjClient>())
                {
                    listaPvs = contexto.Cliente.ListaPVsPendentes(listaCpfCnpj);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    listaPvs
                });
            }

            return listaPvs;
        }

        private void ExibiPainelDebitosPendentes()
        {
            String script = HttpUtility.HtmlEncode("exibirPainelDebitosPendentes()");
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
        }

        protected void btnNovaProposta_Click(object sender, EventArgs e)
        {
            Credenciamento = new Modelo.Credenciamento();
            Response.Redirect("pn_dadosiniciais.aspx");
        }

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
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                    {
                        pnlCompletoPF.Visible = false;
                        pnlCompletoPJ.Visible = true;

                        lblCPFCNPJ.Text = "CNPJ:";
                        ltlCompletoCPFCNPJ.Text = Credenciamento.CNPJ;
                        ltlCompletoCNAE.Text = Credenciamento.CNAE;
                        ltlCompletoRazaoSocial.Text = Credenciamento.RazaoSocial;
                        ltlCompletoDataFundacao.Text = Credenciamento.DataFundacao.ToString("dd/MM/yyyy");
                        rptCompletoProprietarios.DataSource = Credenciamento.Proprietarios;
                        rptCompletoProprietarios.DataBind();
                    }
                    else
                    {
                        pnlCompletoPF.Visible = true;
                        pnlCompletoPJ.Visible = false;

                        lblCPFCNPJ.Text = "CPF:";
                        ltlCompletoCPFCNPJ.Text = Credenciamento.CPF;
                        ltlCompletoNomeCompleto.Text = Credenciamento.NomeCompleto;
                        ltlCompletoDataNascimento.Text = Credenciamento.DataNascimento.ToString("dd/MM/yyyy");
                    }

                    ltlCompletoRamoAtuacao.Text = GetDescricaoGrupoRamo(Credenciamento.GrupoRamo);
                    ltlCompletoRamoAtividade.Text = GetDescricaoRamoAtividade();
                    ltlCompletoContato.Text = Credenciamento.PessoaContato;
                    ltlCompletoTelefones.Text = String.Format("({0}) {1}", Credenciamento.NumDDD1.Trim(), Credenciamento.NumTelefone1);
                    if (!String.IsNullOrEmpty(Credenciamento.NumDDDFax.Trim()) && Credenciamento.NumTelefoneFax != null && Credenciamento.NumTelefoneFax != 0)
                        ltlCompletoFax.Text = String.Format("({0}) {1}", Credenciamento.NumDDDFax.Trim(), Credenciamento.NumTelefoneFax);
                    ltlCompletoEmail.Text = Credenciamento.NomeEmail;
                    ltlCompletoSite.Text = Credenciamento.NomeHomePage;

                    // Dados Equipamento
                    ltlCompletoEquipamento.Text = Credenciamento.TipoEquipamento;
                    ltlCompletoQtde.Text = Credenciamento.QtdeTerminaisSolicitados.ToString();
                    ltlCompletoValorAluguel.Text = String.Format(@"{0:c}", Credenciamento.ValorAluguel);
                    ltlCompletoTaxaAdesao.Text = String.Format(@"{0:c}", Credenciamento.TaxaAdesao);
                    ltlCompletoEvento.Text = Credenciamento.CodEvento;

                    // Dados do Cenário
                    Cenarios cenario = BuscarDadosCenario();
                    if (cenario != null)
                    {
                        //ltlCompletoAcaoComercial.Text = cenario.DescricaoCenario;

                        ltlCompletoDesconto1.Text = cenario.ValorEscalonamentoMes1 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes1) : "-";
                        ltlCompletoDesconto2.Text = cenario.ValorEscalonamentoMes2 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes2) : "-";
                        ltlCompletoDesconto3.Text = cenario.ValorEscalonamentoMes3 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes3) : "-";
                        ltlCompletoDesconto4.Text = cenario.ValorEscalonamentoMes4 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes4) : "-";
                        ltlCompletoDesconto5.Text = cenario.ValorEscalonamentoMes5 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes5) : "-";
                        ltlCompletoDesconto6.Text = cenario.ValorEscalonamentoMes6 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes6) : "-";
                        ltlCompletoDesconto7.Text = cenario.ValorEscalonamentoMes7 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes7) : "-";
                        ltlCompletoDesconto8.Text = cenario.ValorEscalonamentoMes8 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes8) : "-";
                        ltlCompletoDesconto9.Text = cenario.ValorEscalonamentoMes9 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes9) : "-";
                        ltlCompletoDesconto10.Text = cenario.ValorEscalonamentoMes10 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes10) : "-";
                        ltlCompletoDesconto11.Text = cenario.ValorEscalonamentoMes11 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes11) : "-";
                        ltlCompletoDesconto12.Text = cenario.ValorEscalonamentoMes12 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes12) : "-";

                        Double aluguel = Credenciamento.ValorAluguel;
                        ltlCompletoValor1.Text = cenario.ValorEscalonamentoMes1 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes1 / 100)) : "-";
                        ltlCompletoValor2.Text = cenario.ValorEscalonamentoMes2 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes2 / 100)) : "-";
                        ltlCompletoValor3.Text = cenario.ValorEscalonamentoMes3 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes3 / 100)) : "-";
                        ltlCompletoValor4.Text = cenario.ValorEscalonamentoMes4 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes4 / 100)) : "-";
                        ltlCompletoValor5.Text = cenario.ValorEscalonamentoMes5 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes5 / 100)) : "-";
                        ltlCompletoValor6.Text = cenario.ValorEscalonamentoMes6 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes6 / 100)) : "-";
                        ltlCompletoValor7.Text = cenario.ValorEscalonamentoMes7 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes7 / 100)) : "-";
                        ltlCompletoValor8.Text = cenario.ValorEscalonamentoMes8 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes8 / 100)) : "-";
                        ltlCompletoValor9.Text = cenario.ValorEscalonamentoMes9 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes9 / 100)) : "-";
                        ltlCompletoValor10.Text = cenario.ValorEscalonamentoMes10 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes10 / 100)) : "-";
                        ltlCompletoValor11.Text = cenario.ValorEscalonamentoMes11 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes11 / 100)) : "-";
                        ltlCompletoValor12.Text = cenario.ValorEscalonamentoMes12 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes12 / 100)) : "-";

                        ltlCompletoSaz1.Text = cenario.PercentualSazonalidadeJan != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeJan) : "-";
                        ltlCompletoSaz2.Text = cenario.PercentualSazonalidadeFev != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeFev) : "-";
                        ltlCompletoSaz3.Text = cenario.PercentualSazonalidadeMar != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeMar) : "-";
                        ltlCompletoSaz4.Text = cenario.PercentualSazonalidadeAbr != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeAbr) : "-";
                        ltlCompletoSaz5.Text = cenario.PercentualSazonalidadeMai != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeMai) : "-";
                        ltlCompletoSaz6.Text = cenario.PercentualSazonalidadeJun != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeJun) : "-";
                        ltlCompletoSaz7.Text = cenario.PercentualSazonalidadeJul != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeJul) : "-";
                        ltlCompletoSaz8.Text = cenario.PercentualSazonalidadeAgo != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeAgo) : "-";
                        ltlCompletoSaz9.Text = cenario.PercentualSazonalidadeSet != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeSet) : "-";
                        ltlCompletoSaz10.Text = cenario.PercentualSazonalidadeOut != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeOut) : "-";
                        ltlCompletoSaz11.Text = cenario.PercentualSazonalidadeNov != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeNov) : "-";
                        ltlCompletoSaz12.Text = cenario.PercentualSazonalidadeDez != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeDez) : "-";
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

                        ltlCompletoSaz1.Text = "0,00";
                        ltlCompletoSaz2.Text = "0,00";
                        ltlCompletoSaz3.Text = "0,00";
                        ltlCompletoSaz4.Text = "0,00";
                        ltlCompletoSaz5.Text = "0,00";
                        ltlCompletoSaz6.Text = "0,00";
                        ltlCompletoSaz7.Text = "0,00";
                        ltlCompletoSaz8.Text = "0,00";
                        ltlCompletoSaz9.Text = "0,00";
                        ltlCompletoSaz10.Text = "0,00";
                        ltlCompletoSaz11.Text = "0,00";
                        ltlCompletoSaz12.Text = "0,00";
                    }

                    // Dados do PDV
                    ltlCompletoRENPAC.Text = Credenciamento.NroRenpac != 0 ? Credenciamento.NroRenpac.ToString() : String.Empty;
                    ltlCompletoSoftwareTEF.Text = Credenciamento.NomeSoftwareTEF;
                    ltlCompletoMarcaPDV.Text = Credenciamento.NomeMarcaPDV;

                    // Dados de Endereço
                    ltlCompletoEnderecoComercial.Text = String.Format("{0}, {1}, {2} - CEP {3}, {4}, {5}",
                        Credenciamento.EnderecoComercial.Logradouro, Credenciamento.EnderecoComercial.Numero,
                        Credenciamento.EnderecoComercial.Complemento, Credenciamento.EnderecoComercial.CEP,
                        Credenciamento.EnderecoComercial.Cidade, Credenciamento.EnderecoComercial.Estado);
                    ltlCompletoEnderecoCorrespondencia.Text = String.Format("{0}, {1}, {2} - CEP {3}, {4}, {5}",
                        Credenciamento.EnderecoCorrespondencia.Logradouro, Credenciamento.EnderecoCorrespondencia.Numero,
                        Credenciamento.EnderecoCorrespondencia.Complemento, Credenciamento.EnderecoCorrespondencia.CEP,
                        Credenciamento.EnderecoCorrespondencia.Cidade, Credenciamento.EnderecoCorrespondencia.Estado);
                    ltlCompletoEnderecoInstalacao.Text = String.Format("{0}, {1}, {2} - CEP {3}, {4}, {5}",
                        Credenciamento.EnderecoInstalacao.Logradouro, Credenciamento.EnderecoInstalacao.Numero,
                        Credenciamento.EnderecoInstalacao.Complemento, Credenciamento.EnderecoInstalacao.CEP,
                        Credenciamento.EnderecoInstalacao.Cidade, Credenciamento.EnderecoInstalacao.Estado);
                    ltlCompletoHorarioFuncionamento.Text = String.Format("{0} à {1} - {2} às {3}",
                        Credenciamento.DiaInicioFuncionamento, Credenciamento.DiaFimFuncionamento,
                        Credenciamento.HoraInicioFuncionamento, Credenciamento.HoraFimFuncionamento);
                    ltlCompletoContatoInstalacao.Text = Credenciamento.NomeContatoInstalacao;
                    ltlCompletoTelefoneInstalacao.Text = String.Format("({0}) {1}", Credenciamento.NumDDDInstalacao, Credenciamento.NumTelefoneInstalacao);
                    //ltlCompletoDataHorarioInstalacao.Text = String.Format("{0} à {1} - {2} às {3}",
                    //    Credenciamento.DiaInicioInstalacao, Credenciamento.DiaFimInstalacao,
                    //    Credenciamento.HoraInicioInstalacao, Credenciamento.HoraFimInstalacao);

                    ltlCompletoObs.Text = Credenciamento.Observacao;
                    //if (!String.IsNullOrEmpty(Credenciamento.Observacao))
                    //{
                    //    int obsIndex = Credenciamento.Observacao.LastIndexOf("#OBS:");
                    //    if (obsIndex != -1)
                    //    {
                    //        ltlCompletoPontoReferencia.Text = Credenciamento.Observacao.Substring(0, obsIndex).Trim().Replace("#PTREF:", "");
                    //    }
                    //}

                    // Dados Operacionais
                    ltlCompletoNomeFatura.Text = Credenciamento.NomeFatura;
                    ltlCompletoFuncionamento.Text = Credenciamento.HorarioFuncionamento == 0 ? "Comercial" : "Noturno";

                    String tipoEstabelecimento = "Autônomo";
                    if (Credenciamento.CodTipoEstabelecimento == 2)
                        tipoEstabelecimento = "Matriz";
                    else if (Credenciamento.CodTipoEstabelecimento == 1)
                        tipoEstabelecimento = "Filial";

                    ltlCompletoTipoEstabelecimento.Text = tipoEstabelecimento;
                    ltlCompletoDataAssinatura.Text = Credenciamento.DataCadastroProposta.ToString("dd/MM/yyyy");
                    ltlCompletoLocalPagamento.Text = Credenciamento.LocalPagamento == 1 ? "Estabelecimento" : "Centralizadora - PV Centralizador: " + Credenciamento.Centralizadora;

                    // Dados Bancários
                    if (Credenciamento.ProdutosCredito != null && Credenciamento.ProdutosCredito.Count > 0)
                    {
                        ltlCompletoBancoCredito.Text = Credenciamento.NomeBancoCredito;
                        ltlCompletoAgenciaCredito.Text = Credenciamento.AgenciaCredito.ToString(); //TODO
                        ltlCompletoContaCorrenteCredito.Text = Credenciamento.ContaCredito;
                    }
                    else
                        pnlDomicilioCredito.Visible = false;

                    if (Credenciamento.ProdutosDebito != null && Credenciamento.ProdutosDebito.Count > 0)
                    {
                        ltlCompletoBancoDebito.Text = Credenciamento.NomeBancoDebito;
                        ltlCompletoAgenciaDebito.Text = Credenciamento.AgenciaDebito.ToString(); //TODO
                        ltlCompletoContaCorrenteDebito.Text = Credenciamento.ContaDebito;
                    }
                    else
                        pnlDomicilioDebito.Visible = false;

                    if (Credenciamento.ProdutosConstrucard != null && Credenciamento.ProdutosConstrucard.Count > 0)
                    {
                        pnlConstrucard.Visible = true;
                        ltlCompletoBancoConstrucard.Text = Credenciamento.NomeBancoConstrucard;
                        ltlCompletoAgenciaConstrucard.Text = Credenciamento.AgenciaConstrucard.ToString(); //TODO
                        ltlCompletoContaCorrenteConstrucard.Text = Credenciamento.ContaConstrucard;
                    }

                    //Dados Produtos
                    var produtos = (from p in Credenciamento.ProdutosCredito
                                    group p by p.CodFeature
                                        into grp
                                        select grp.First()).ToArray();

                    if (produtos.Count() > 0)
                    {
                        rptCompletoVendasCredito.DataSource = produtos;
                        rptCompletoVendasCredito.DataBind();
                    }
                    else
                        pnlCredito.Visible = false;

                    var produtosDebitos = (from p in Credenciamento.ProdutosDebito
                                           group p by p.CodFeature
                                               into grp
                                               select grp.First()).ToArray();

                    if (produtosDebitos.Count() > 0)
                    {
                        rptCompletoVendasDebito.DataSource = produtosDebitos;
                        rptCompletoVendasDebito.DataBind();
                    }
                    else
                        pnlDebito.Visible = false;

                    if (Credenciamento.ProdutosConstrucard != null && Credenciamento.ProdutosConstrucard.Count > 0)
                    {
                        rptCompletoVendasConstrucard.DataSource = Credenciamento.ProdutosConstrucard;
                        rptCompletoVendasConstrucard.DataBind();
                    }

                    //Dados Serviços
                    if (String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0)
                    {
                        ltlHeaderValor.Text = "Valor (mensal)";
                    }

                    if (Credenciamento.Servicos.Count > 0)
                    {
                        pnlServicos.Visible = true;
                        rptServicos.DataSource = Credenciamento.Servicos;
                        rptServicos.DataBind();
                    }

                    //Dados Produtos Van
                    if (Credenciamento.ProdutosVan.Count > 0)
                    {
                        pnlProdutosVan.Visible = true;
                        rptProdutosVan.DataSource = Credenciamento.ProdutosVan;
                        rptProdutosVan.DataBind();
                    }
                }
            }
            catch (FaultException<TGCenarios.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Conclusão", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Conclusão", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Conclusão", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void rptCompletoVendasCredito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                Literal ltlDe = (Literal)e.Item.FindControl("ltlDe");
                Literal ltlAte = (Literal)e.Item.FindControl("ltlAte");
                Literal ltlLimiteParcela = (Literal)e.Item.FindControl("ltlLimiteParcelas");
                Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                ProdutosListaDadosProdutosPorRamoCanal item = (ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                ltlDe.Text = "-";
                ltlAte.Text = "-";
                ltlLimiteParcela.Text = String.Empty;
                ltlFormaPagto.Text = item.IndFormaPagamento == 'T' ? "Tarifa" : "Taxa";

                Int32 codCca = (Int32)item.CodCCA;
                Int32 codFeature = (Int32)item.CodFeature;

                if (Credenciamento.Patamares != null)
                {
                    List<Modelo.Patamar> patamares = Credenciamento.Patamares.FindAll(p => p.CodCca == codCca && p.CodFeature == codFeature);
                    if (patamares.Count > 0)
                    {
                        ((Literal)e.Item.FindControl("ltlDe")).Text = patamares[0].PatamarInicial.ToString();
                        ((Literal)e.Item.FindControl("ltlAte")).Text = patamares[0].PatamarFinal.ToString();
                        ltlLimiteParcela.Text = item.QtdeMaximaParcela.ToString();

                        if (patamares.Count > 1)
                        {
                            Panel pnlPatamar1 = (Panel)e.Item.FindControl("pnlPatamar1");
                            pnlPatamar1.Visible = true;

                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1PrazoRecebimento")).Text = String.Format("{0} dia(s)", patamares[1].Prazo.ToString());
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Taxa")).Text = String.Format(@"{0:f2}", patamares[1].TaxaPatamar.ToString());
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1De")).Text = patamares[1].PatamarInicial.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Ate")).Text = patamares[1].PatamarFinal.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1LimiteParcela")).Text = item.QtdeMaximaParcela.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1FormaPagamento")).Text = item.IndFormaPagamento == 'T' ? "Tarifa" : "Taxa";
                        }

                        if (patamares.Count > 2)
                        {
                            Panel pnlPatamar2 = (Panel)e.Item.FindControl("pnlPatamar2");
                            pnlPatamar2.Visible = true;

                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2PrazoRecebimento")).Text = String.Format("{0} dia(s)", patamares[2].Prazo.ToString());
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Taxa")).Text = String.Format(@"{0:f2}", patamares[2].TaxaPatamar.ToString());
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2De")).Text = patamares[2].PatamarInicial.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Ate")).Text = patamares[2].PatamarFinal.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2LimiteParcela")).Text = item.QtdeMaximaParcela.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2FormaPagamento")).Text = item.IndFormaPagamento == 'T' ? "Tarifa" : "Taxa";
                        }
                    }
                }
            }
        }

        protected void rptCompletoVendasDebito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                ProdutosListaDadosProdutosPorRamoCanal item = (ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                ltlFormaPagto.Text = item.IndFormaPagamento == 'X' ? "Taxa" : "Tarifa";
            }
        }

        protected void rptCompletoVendasConstrucard_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                ProdutosListaDadosProdutosPorRamoCanal item = (ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                ltlFormaPagto.Text = item.IndFormaPagamento == 'X' ? "Taxa" : "Tarifa";
            }
        }

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

                ltlCodigoServico.Text = item.CodServico.ToString();
                ltlNomeServico.Text = item.DescServico;
                ltlCodigoRegime.Text = item.CodRegimeServico.ToString();
                ltlQtde.Text = item.QtdeMinima.ToString();
                ltlValor.Text = String.Format("{0:C}", item.ValorFranquia);
                ltlExcedente.Text = String.Format("{0:C}", 0);
            }
        }

        protected void rptProprietarios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlTituloProprietario = (Literal)e.Item.FindControl("ltlTituloProprietario");
                Literal ltlProprietario = (Literal)e.Item.FindControl("ltlProprietario");

                ltlTituloProprietario.Visible = (e.Item.ItemIndex == 0);

                String proprietario = String.Format("{0} - {1}%", ((Proprietario)e.Item.DataItem).Nome, ((Proprietario)e.Item.DataItem).Participacao, "%");
                ltlProprietario.Text = proprietario;
            }
        }

        /// <summary>
        /// Data Bound da tabela de serviços
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptProdutosVan_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                {
                    ((Literal)e.Item.FindControl("ltlCodigo")).Text = ((ProdutosListaDadosProdutosVanPorRamo)e.Item.DataItem).CodCCA.ToString();
                    ((Literal)e.Item.FindControl("ltlDescricao")).Text = ((ProdutosListaDadosProdutosVanPorRamo)e.Item.DataItem).NomeCCA;
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca informações detalhadas do cenário escolhido
        /// </summary>
        private Cenarios BuscarDadosCenario()
        {
            ServicoPortalTGCenariosClient client = new ServicoPortalTGCenariosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados do cenário"))
                {
                    if (Credenciamento.CodCenario != null)
                    {
                        Int32 codCenario = (Int32)Credenciamento.CodCenario;
                        Int32 codCanal = Credenciamento.Canal;
                        String codTipoEquipamento = Credenciamento.CodTipoEquipamento;
                        Char codSituacaoCenarioCanal = 'A';
                        String codCampanha = !String.IsNullOrEmpty(Credenciamento.CodCampanha) ? Credenciamento.CodCampanha : null;
                        String codOrigemChamada = null;

                        Cenarios[] cenarios = client.ListaDadosCadastrais(codCenario, codCanal, codTipoEquipamento, codSituacaoCenarioCanal, codCampanha, codOrigemChamada);
                        client.Close();

                        if (cenarios.Length == 1)
                            return cenarios[0];
                    }
                    return null;
                }
            }
            catch (FaultException<TGCenarios.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Consulta dados do Credenciamento
        /// </summary>
        private void ConsPropCredenciamentoPendente(out Int32 numPdv, out Char situacaoProposta)
        {
            ServicoPortalWFPropostaClient client = new ServicoPortalWFPropostaClient();
            numPdv = 0;
            situacaoProposta = default(Char);

            try
            {
                using (Logger log = Logger.IniciarLog("Consulta Proposta Credenciamento Pendente"))
                {
                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJ = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    Int32 numSequencia = (Int32)Credenciamento.NumSequencia;

                    PropostasCNPJ[] propostas = client.ConsPropCredenciamentoPendente(codTipoPessoa, numCNPJ, numSequencia);
                    client.Close();

                    if (propostas.Length > 0)
                    {
                        ltlPv.Text = propostas[0].NumPontoVenda != null ? propostas[0].NumPontoVenda.ToString() : String.Empty;
                        ltlNrProposta.Text = propostas[0].NumSolicitacao != null ? propostas[0].NumSolicitacao.ToString() : String.Empty;

                        if (propostas[0].IndSituacaoProposta != null)
                            ltlSituacaoProposta.Text = String.Format("{0} - {1}", propostas[0].IndSituacaoProposta, propostas[0].DescSituacaoProposta);

                        ltlMensagemConfirmacao.Text = "Proposta recebida com sucesso.";

                        numPdv = propostas[0].NumPontoVenda ?? default(Int32);
                        situacaoProposta = propostas[0].IndSituacaoProposta ?? default(Char);
                    }
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Retorna a descrição do ramo atividade
        /// </summary>
        /// <returns></returns>
        private String GetDescricaoRamoAtividade()
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Atualiza situação da proposta"))
                {
                    RamosAtividadesListaDadosCadastraisRamosAtividades[] retorno = client.ListaDadosCadastraisRamosAtividades(Credenciamento.GrupoRamo, Credenciamento.RamoAtividade);
                    client.Close();

                    return retorno[0].DescrRamoAtividade;
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Retorna a descrição do Grupo de Atividade
        /// </summary>
        /// <returns></returns>
        private String GetDescricaoGrupoRamo(Int32 codGrupoRamo)
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Atualiza situação da proposta"))
                {
                    var retorno = client.ListaDadosCadastraisGruposRamosAtividades();
                    client.Close();

                    var grupoRamo = retorno.FirstOrDefault(g => g.CodGrupoRamoAtividade == codGrupoRamo);

                    if (grupoRamo != null)
                        return grupoRamo.DescrRamoAtividade;

                    return codGrupoRamo.ToString();
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Busca Lista de FCTs
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <param name="numeroFct"></param>
        /// <returns></returns>
        private String ListaFCTInstalacaoAtivaPorPV(Int32 numeroPDV, Int32? numeroFct)
        {
            String retorno = String.Empty;
            FCTInstalacaoAtivaPorPV[] listaFCTs;

            using (var log = Logger.IniciarLog("Lista FCT Instalção Ativa Por PV"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroPDV,
                    numeroFct
                });

                using (var contexto = new ContextoWCF<ServicoPortalTGFichaTecnologiaClient>())
                {
                    listaFCTs = contexto.Cliente.ListaFCTInstalacaoAtivaPorPV(numeroPDV, numeroFct);

                    foreach (var fct in listaFCTs)
                    {
                        retorno = String.Format(@"{0}|{1}", retorno, fct.NumeroFCT);
                    }

                    retorno = retorno.Remove(0, 1);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    listaFCTs,
                    retorno
                });
            }

            return retorno;
        }

    }
}
