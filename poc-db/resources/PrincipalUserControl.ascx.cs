/*
(c) Copyright [ANO] Redecard S.A.
Autor : [Daniel]
Empresa : [BRQ IT Services]
Histórico:
- [01/08/2012] – [Daniel] – [Etapa inicial]
*/
using Redecard.PN.Comum;
using Redecard.PN.RAV.Sharepoint.ModuloRAV;
using Redecard.PN.RAV.Sharepoint.WAExtratoAntecipacaoRAVServico;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Sharepoint.WebParts.Principal
{
    public partial class PrincipalUserControl : UserControlBase
    {
        #region Constantes
        private const string FONTE = "PrincipalUserControl.ascx";
        private const int COGIDO_ERRO_RAVAVULSO = 3004;
        private const int COGIDO_ERRO_RAVAUTO = 3005;
        private const int COGIDO_ERRO_RAVEMAIL = 3006;
        private const int COGIDO_ERRO_RAVAUTO_CONTINUAR = 3007;
        private const int COGIDO_ERRO_RAVAUTO_PERSONALIZAR = 3008;
        private const int CODIGO_ERRO_CONFIRMAE_EMAIL = 3009;
        private int CodigoErroCadastrar = 3003;
        #endregion

        #region Atributos
        private string valorAntecipacaoAvulso = String.Empty;
        private string tipoVendaAvulso = String.Empty;
        private string valorMinimoAutomatico = String.Empty;
        private string tipoVendaAutomatico = String.Empty;
        private Boolean excessaoLancada = false;
        private readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("pt-BR");
        #endregion

        /// <summary>
        /// ultimo email cadastrado
        /// </summary>
        private String UltimoEmail
        {
            get
            {
                if (!Object.ReferenceEquals(ViewState["UltimoEmail"], null))
                    return (String)ViewState["UltimoEmail"];
                else
                    return String.Empty;
            }
            set
            {
                ViewState["UltimoEmail"] = value;
            }
        }

        /// <summary>
        /// Inicialização da Página de RAV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("RAV Principal - Page Load BBF"))
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(Request.QueryString["dados"]))
                    {
                        QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                        SharePointUlsLog.LogMensagem(queryString["AcessoSenha"]);
                        log.GravarMensagem(queryString["AcessoSenha"]);

                        if (queryString["ValorRavAvulsoSolicitado"] != null)
                        {
                            valorAntecipacaoAvulso = queryString["ValorRavAvulsoSolicitado"].ToString();
                        }
                        if (queryString["TipoRavAvulsoAlteracao"] != null)
                        {
                            tipoVendaAvulso = queryString["TipoRavAvulsoAlteracao"].ToString();
                        }
                        if (queryString["RavAutomaticoValorMinimo"] != null)
                        {
                            valorMinimoAutomatico = queryString["RavAutomaticoValorMinimo"].ToString();
                        }
                        if (queryString["RavAutomaticoTipoVenda"] != null)
                        {
                            tipoVendaAutomatico = queryString["RavAutomaticoTipoVenda"].ToString();
                        }
                    }

                    if (!Page.IsPostBack)
                    {
                        //RAV Avulso
                        VerificarRAVAvulso();

                        //RAV Automático
                        VerificarRAVAutomatico();

                        //RAV E-mail
                        VerificarEmail();

                        //ultimas antecipacoes
                        VerificarUltimasAntecipacoes();
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

        /// <summary>
        /// Verifica o RAV Avulso e preenche na tela
        /// </summary>
        private void VerificarRAVAvulso()
        {
            #region RAV Avulso
            using (Logger log = Logger.IniciarLog("Verificando RAV Avulso - Front"))
            {
                using (ServicoPortalRAVClient servicoPortal = new ServicoPortalRAVClient())
                {
                    ModRAVAvulsoSaida ravAvulso = servicoPortal.VerificarRAVDisponivel(SessaoAtual.CodigoEntidade);

                    if (ravAvulso != null && ravAvulso.Retorno == -1)
                    {
                        ExibirMensagem("Acesso não permitido.", ravAvulso.MsgErro);
                        return;
                    }

                    if (ravAvulso != null & ravAvulso.Retorno > 70000)
                    {
                        this.ExibirPainelExcecao(FONTE, ravAvulso.Retorno);
                        return;
                    }

                    Session["DadosRAVAvulso"] = ravAvulso;

                    if (ravAvulso.ValorDisponivel >= 50)
                    {
                        pnlRavAvulsoDisponivel.Visible = true;
                        //pnlRavAvulsoIndisponivel.Visible = false;

                        string valorRavAvulso = ravAvulso.ValorDisponivel.ToString("0,0.00");
                        //lblDataRavAvulso.Text = ObterDataRavAvulso();

                        //if (ravAvulso.DadosParaCredito.Count == 2 && (ravAvulso.DadosParaCredito[0].ValorRotativo > 0 || ravAvulso.DadosParaCredito[1].ValorRotativo > 0))
                        //{
                        //    if (ravAvulso.DadosParaCredito[0].ValorRotativo > 0)
                        //    {
                        //        valorRavAvulso = ravAvulso.ValorDisponivel.ToString("0,0.00");
                        //        //valorRavAvulso = ravAvulso.DadosParaCredito[0].ValorRotativo.ToString("0,0.00");
                        //        //lblDataRavAvulso.Text = ravAvulso.DadosParaCredito[0].DataCredito.ToShortDateString();
                        //        lblDataRavAvulso.Text = ReturnDate();
                        //    }
                        //    else if (ravAvulso.DadosParaCredito[1].ValorRotativo > 0)
                        //    {
                        //        valorRavAvulso = ravAvulso.ValorDisponivel.ToString("0,0.00");
                        //        //valorRavAvulso = ravAvulso.DadosParaCredito[1].ValorRotativo.ToString("0,0.00");
                        //        //lblDataRavAvulso.Text = ravAvulso.DadosParaCredito[1].DataCredito.ToShortDateString();
                        //        lblDataRavAvulso.Text = ReturnDate();
                        //    }
                        //}

                        //if (valorRavAvulso.Length < 11)
                        //{
                        //    lblValorRavAvulso.Style["font-size"] = "25px";
                        //    //lblAsterisco.Style["font-size"] = "35px";
                        //}
                        //else if (valorRavAvulso.Length > 10 && valorRavAvulso.Length < 13)
                        //{
                        //    lblValorRavAvulso.Style["font-size"] = "25px";
                        //    //lblAsterisco.Style["font-size"] = "29px";
                        //}
                        //else if (valorRavAvulso.Length > 12 && valorRavAvulso.Length < 15)
                        //{
                        //    lblValorRavAvulso.Style["font-size"] = "25px";
                        //    //lblAsterisco.Style["font-size"] = "25px";
                        //}
                        //else if (valorRavAvulso.Length > 14 && valorRavAvulso.Length < 17)
                        //{
                        //    lblValorRavAvulso.Style["font-size"] = "25px";
                        //    //lblAsterisco.Style["font-size"] = "21px";
                        //}
                        //else if (valorRavAvulso.Length > 16 && valorRavAvulso.Length < 19)
                        //{
                        //    lblValorRavAvulso.Style["font-size"] = "22px";
                        //    //lblAsterisco.Style["font-size"] = "17px";
                        //}
                        //else if (valorRavAvulso.Length > 18 && valorRavAvulso.Length <= 20)
                        //{
                        //    lblValorRavAvulso.Style["font-size"] = "20px";
                        //    //lblAsterisco.Style["font-size"] = "15px";
                        //}

                        ltrValorRavAvulso.Text = valorRavAvulso;
                        Session["ValorRavAvulsoSolicitado"] = valorRavAvulso;
                        //lblValorMaximoAntecipacao.Text = ravAvulso.ValorDisponivel.ToString("0,0.00");
                    }
                    else
                    {
                        //lblDataAntecipacaoSemValor.Text = ObterDataRavAvulso();
                        btnSelecionarAvulso.Enabled = false;
                        pnlRavAvulsoDisponivel.Visible = false;
                        //pnlRavAvulsoIndisponivel.Visible = true;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Verifica o RAV Automático e preenche na tela
        /// </summary>
        private void VerificarRAVAutomatico()
        {
            #region RAV Automático
            using (Logger log = Logger.IniciarLog("Verificando RAV Automático - Front"))
            {
                using (ServicoPortalRAVClient servicoPortal = new ServicoPortalRAVClient())
                {
                    if (servicoPortal.VerificarRAVAutomatico(SessaoAtual.CodigoEntidade, 'R', 'D'))
                    {
                        ModRAVAutomatico ravAutomatico = servicoPortal.ConsultarRAVAutomatico(SessaoAtual.CodigoEntidade, 'R', 'D');
                        if (ravAutomatico != null)
                        {
                            if (ravAutomatico.DadosRetorno.CodRetorno == 0)
                            {
                                btnSelecionarAutomatico.Enabled = false;
                                pnlRavAutomaticoContratado.Visible = true;

                                // 06/04/2017 - BBF - Nao ira mais exibir as bandeiras
                                //if (ravAutomatico.Bandeiras.Count > 0)
                                //{
                                //    //obtem apenas as bandeiras que devem ser exibidas
                                //    rptBandeiras.DataSource = ravAutomatico.Bandeiras.Where(b => b.IndSel == "S" || ravAutomatico.IndFull.Equals("S"));
                                //    rptBandeiras.DataBind();
                                //}

                                switch (ravAutomatico.TipoRAV)
                                {
                                    case ElndProdutoAntecipa.Ambos:
                                        { ltrTipoVendaAutomatico.Text = "à vista e parcelado"; break; }
                                    case ElndProdutoAntecipa.Parcelado:
                                        { ltrTipoVendaAutomatico.Text = "parcelado"; break; }
                                    case ElndProdutoAntecipa.Rotativo:
                                        { ltrTipoVendaAutomatico.Text = "à vista"; break; }
                                }

                                if (ravAutomatico.TipoRAV == ElndProdutoAntecipa.Ambos || ravAutomatico.TipoRAV == ElndProdutoAntecipa.Parcelado)
                                {
                                    ltrParcelas.Text = String.Format(" ({0} - {1})", ravAutomatico.NumParcelaIni, ravAutomatico.NumParcelaFim);
                                }

                                ltrValorMinimo.Text = ravAutomatico.ValorMinimo.ToString("C", culture);
                                switch (ravAutomatico.Periodicidade)
                                {
                                    case EPeriodicidade.Diario:
                                        { ltrFrequencia.Text = "diário"; break; }
                                    case EPeriodicidade.Semanal:
                                        { ltrFrequencia.Text = "semanal" + " (" + ObterDescricaoDiaSemana(ravAutomatico.DiaSemana) + ")"; break; }
                                    case EPeriodicidade.Quinzenal:
                                        { ltrFrequencia.Text = "quinzenal"; break; }
                                    case EPeriodicidade.Mensal:
                                        { ltrFrequencia.Text = "mensal"; break; }
                                }
                                ltrVigenciaInicio.Text = ravAutomatico.DataVigenciaIni.ToString("dd/MM/yyyy");
                                ltrVigenciaFim.Text = ravAutomatico.DataVigenciaFim.ToString("dd/MM/yyyy");

                                // Monta URL para exibir os dados do Contrato
                                var queryString = new QueryStringSegura();

                                queryString["AcessoSenha"] = bool.TrueString;
                                queryString["RavAutomaticoValorMinimo"] = ravAutomatico.ValorMinimo.ToString("0.0,0");
                                queryString["RavAutomaticoTipoVenda"] = ltrTipoVendaAutomatico.Text;
                                queryString["RavAutomaticoPeriodoRecebimento"] = ltrFrequencia.Text;
                                queryString["RavAutomaticoTaxa"] = ravAutomatico.DadosRetorno.TaxaCategoria.ToString("0.0,0");
                                queryString["RavAutomaticoDataIni"] = ltrVigenciaInicio.Text;
                                queryString["RavAutomaticoDataFim"] = ltrVigenciaFim.Text;
                                queryString["RavAutomaticoTipoAntecipacao"] = ravAutomatico.DadosRetorno.NomeProdutoAntecipacao;
                                queryString["RavAutomaticoBandeiras"] = ravAutomatico.Bandeiras.Where(b => b.IndSel == "S" || ravAutomatico.IndFull.Equals("S")).Select(b => b.DscBandeira).Join(";", ";");

                                hlkDetalharContrato.NavigateUrl = String.Format("/sites/fechado/servicos/Paginas/pn_ComprovanteRavAutomatico.aspx?dados={0}", queryString);
                            }
                            else
                            {
                                log.GravarMensagem(String.Format("Retorno do ConsultarRAVAutomatico: cod {0}", ravAutomatico.DadosRetorno.CodRetorno));
                                this.ExibirPainelExcecao(FONTE, ravAutomatico.DadosRetorno.CodRetorno);
                                pnlRavAutomaticoContratado.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        pnlRavAutomaticoContratado.Visible = false;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Verifica o e-mail de RAV e preenche a tela
        /// </summary>
        private void VerificarEmail()
        {
            #region RAV Email
            using (Logger Log = Logger.IniciarLog("Consulta de e-mails - Front"))
            {
                try
                {
                    using (ServicoPortalRAVClient servicoPortal = new ServicoPortalRAVClient())
                    {
                        ModRAVEmailEntradaSaida ravEmail = servicoPortal.ConsultarEmails(SessaoAtual.CodigoEntidade);

                        if (ravEmail.ListaEmails.Count > 0)
                        {
                            pnlEmailAlterar.Visible = true;
                            pnlEmailCadastrar.Visible = false;

                            rptEmails.DataSource = ravEmail.ListaEmails;
                            rptEmails.DataBind();

                            if (ravEmail.ListaEmails.Count > 2)
                                lnkBtnCadastrarEmail.Visible = false;
                            else
                                lnkBtnCadastrarEmail.Visible = true;
                        }
                        else
                        {
                            pnlEmailAlterar.Visible = false;
                            pnlEmailCadastrar.Visible = true;
                        }
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirPainelExcecao(FONTE, COGIDO_ERRO_RAVEMAIL);
                    SharePointUlsLog.LogErro(ex);
                }
            }
            #endregion
        }

        /// <summary>
        /// Obtem as ultimas antecipacoes do PV consumindo servicos usado no Extrato - Relatorios - antecipações
        /// </summary>
        private void VerificarUltimasAntecipacoes()
        {
            using (Logger log = Logger.IniciarLog("Verificando Ultimas antecipacoes - Front"))
            {
                // campos padroes para chamada dos metodos
                var guidPesquisaTotalizador = Guid.NewGuid();
                var guidPesquisa = Guid.NewGuid();
                Int32 tamanhoPagina = 99;
                Int32 registroInicial = 0;
                Int32 qtdRegistrosVirtuais = 480;

                // campos do filtro
                Int32 codigoBandeira = 0;
                DateTime dataInicial = DateTime.Now.AddDays(-30);
                DateTime dataFinal = DateTime.Now;
                Int32[] pv = new int[] { this.SessaoAtual.CodigoEntidade };

                //campos de retorno
                StatusRetorno statusRelatorio;
                StatusRetorno statusTotalizador;
                WAExtratoAntecipacaoRAVServico.RAV[] registros;
                RAVTotalizador totalizador;

                //StatusRetorno statusRelatorioDetalhe = null;
                //StatusRetorno statusTotalizadorDetalhe = null;
                //RAVDetalhe[] registrosDetalhe = null;
                //RAVDetalheTotalizador totalizadorDetalhe = null;

                using (var contexto = new ContextoWCF<HISServicoWA_Extrato_AntecipacaoRAVClient>())
                {
                    log.GravarLog(EventoLog.ChamadaServico, new
                    {
                        CodigoBandeira = codigoBandeira,
                        DataInicial = dataInicial,
                        DataFinal = dataFinal,
                        PV = pv,
                        GuidPesquisa = guidPesquisa,
                        GuidTotalizador = guidPesquisaTotalizador
                    });

                    totalizador = contexto.Cliente.ConsultarRelatorio(
                        guidPesquisaTotalizador,
                        guidPesquisa,
                        codigoBandeira,
                        dataInicial,
                        dataFinal,
                        pv,
                        registroInicial,
                        tamanhoPagina,
                        ref qtdRegistrosVirtuais,
                        out registros,
                        out statusTotalizador,
                        out statusRelatorio);

                    log.GravarLog(EventoLog.RetornoServico, new { totalizador, registros, statusTotalizador, statusRelatorio, qtdRegistrosVirtuais });

                    //var codigoBandeiras = totalizador.Valores.Select(totalizadorBandeira => totalizadorBandeira.CodigoBandeira).ToArray();

                    //log.GravarLog(EventoLog.ChamadaServico, new
                    //{
                    //    CodigoBandeiras = codigoBandeiras,
                    //    Detalhe = "Obtendo detalhamento"
                    //});

                    //totalizadorDetalhe = contexto.Cliente.ConsultarRelatorioDetalheTodos(
                    //            guidPesquisaTotalizador,
                    //            guidPesquisa,
                    //            codigoBandeiras,
                    //            dataInicial,
                    //            dataFinal,
                    //            pv,
                    //            registroInicial,
                    //            tamanhoPagina,
                    //            RAVDetalheTipoRegistro.Todos,
                    //            ref qtdRegistrosVirtuais,
                    //            out registrosDetalhe,
                    //            out statusTotalizadorDetalhe,
                    //            out statusRelatorioDetalhe);

                    //log.GravarLog(EventoLog.RetornoServico, new { totalizadorDetalhe, registrosDetalhe, statusTotalizadorDetalhe, statusRelatorioDetalhe, qtdRegistrosVirtuais });
                }

                if (statusRelatorio.CodigoRetorno != 0 && statusRelatorio.CodigoRetorno != 10) // codigo 10 é registro nao encontrado
                {
                    log.GravarErro(new Exception(String.Format("Erro ao consumir servico HISServicoWA_Extrato_AntecipacaoRAVClient.ConsultarRelatorio, StatusRelatorio.CodRetorno: {0}", statusRelatorio.CodigoRetorno)));
                    base.ExibirPainelExcecao(statusRelatorio.Fonte, statusRelatorio.CodigoRetorno);
                    return;
                }

                if (statusTotalizador.CodigoRetorno != 0 && statusRelatorio.CodigoRetorno != 10) // codigo 10 é registro nao encontrado
                {
                    log.GravarErro(new Exception(String.Format("Erro ao consumir servico HISServicoWA_Extrato_AntecipacaoRAVClient.ConsultarRelatorio, statusTotalizador.CodRetorno: {0}", statusTotalizador.CodigoRetorno)));
                    base.ExibirPainelExcecao(statusTotalizador.Fonte, statusTotalizador.CodigoRetorno);
                    return;
                }

                if (registros != null)
                {
                    rptUltimasAntecipacoes.DataSource = registros.OrderByDescending("DataAntecipacao").Take(4);
                    rptUltimasAntecipacoes.DataBind();

                    Int32 totalRegistros = registros.Count();

                    if (totalRegistros > 0)
                    {
                        pnlUltimasAntecipacoes.Visible = true;
                        pnlTodasAntecipacoes.Visible = true;

                        //monta o link para enviar para o extrato relatorios de antecipações
                        QueryStringSegura qs = new QueryStringSegura();
                        qs["TipoRelatorio"] = "38"; //Antecipações
                        qs["TipoVenda"] = "0"; // Crédito
                        qs["DataInicial"] = dataInicial.ToString("dd/MM/yyyy");
                        qs["DataFinal"] = dataFinal.ToString("dd/MM/yyyy");

                        hlkTodasAntecipacoes.NavigateUrl = String.Format("/sites/fechado/extrato/Paginas/pn_relatorios.aspx?dados={0}", qs);
                    }
                }
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //private string ObterDataRavAvulso()
        //{
        //    DateTime date = DateTime.Now;
        //    string correctDate = "";
        //    if ((date.Hour > 14 && date.Minute >= 30) || date.Hour > 14)
        //    {
        //        //if (date.DayOfWeek == DayOfWeek.Friday)
        //        //    correctDate = date.AddDays(3).ToShortDateString();
        //        //else
        //        correctDate = date.AddDays(1).ToShortDateString();
        //    }
        //    else
        //    {
        //        correctDate = date.ToShortDateString();
        //    }

        //    return correctDate;
        //}

        /// <summary>
        /// Redireciona o usuário para a página de RAV Avulso (máximo valor disponível).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void RAVAvulso_Continuar(object sender, EventArgs e)
        //{
        //    using (Logger Log = Logger.IniciarLog("Principal - Continuar RAV Avulso"))
        //    {
        //        try
        //        {
        //            if (Session["DadosRAVAvulso"] != null && (Session["DadosRAVAvulso"] as ModRAVAvulsoSaida).ValorDisponivel < 50)
        //            {
        //                base.Alert("Não existe saldo suficiente.", false);
        //            }
        //            else
        //            {
        //                QueryStringSegura queryString = new QueryStringSegura();
        //                ModRAVAvulsoSaida saida = (ModRAVAvulsoSaida)Session["DadosRAVAvulso"];
        //                if (saida.DadosParaCredito.Count > 0)
        //                {
        //                    if (saida.DadosParaCredito[0].ValorRotativo > 0)
        //                    {
        //                        queryString["RavAvulsoTipoCred"] = "0";
        //                    }
        //                    else
        //                    {
        //                        queryString["RavAvulsoTipoCred"] = "1";
        //                    }
        //                }
        //                else
        //                {
        //                    queryString["RavAvulsoTipoCred"] = "1";
        //                }

        //                queryString["RavAvulsoTipoVenda"] = "Ambos (À Vista e Parcelado)";
        //                queryString["RavAvulsoTipoAntecipacao"] = saida.DadosAntecipacao.NomeProdutoAntecipacao;
        //                queryString["AcessoSenha"] = _validaSenha;
        //                Response.Redirect(string.Format("pn_ConfirmacaoRavAvulso.aspx?dados={0}", queryString.ToString()));
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.GravarErro(ex);
        //            this.ExibirPainelExcecao("Redecard.PN.SharePoint", CODIGO_ERRO);
        //            SharePointUlsLog.LogErro(ex);
        //        }
        //    }
        //}

        /// <summary>
        /// Redireciona o usuário para a página de alterar o RAV Avulso.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RAVAvulso_OutrosValores(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Principal - RAV Avulso Personalizar"))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura();
                    if (!String.IsNullOrEmpty(valorAntecipacaoAvulso))
                    {
                        queryString["ValorRavAvulsoSolicitado"] = valorAntecipacaoAvulso;
                    }
                    else
                    {
                        queryString["ValorRavAvulsoSolicitado"] = !string.IsNullOrEmpty(ltrValorRavAvulso.Text) ? ltrValorRavAvulso.Text : "0";
                    }
                    if (!String.IsNullOrEmpty(tipoVendaAvulso))
                    {
                        queryString["TipoRavAvulsoAlteracao"] = tipoVendaAvulso;
                    }
                    
                    queryString["TipoRavSelecionado"] = "Avulso";

                    Response.Redirect(string.Format("pn_Rav.aspx?dados={0}", queryString.ToString()), false);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

        /// <summary>
        /// Redireciona o usuário para a página de confirmação do RAV Automático.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void RAVAuto_Continuar(object sender, EventArgs e)
        //{
        //    using (Logger Log = Logger.IniciarLog("Principal - Continuar"))
        //    {
        //        // AGA: Caso não seja diário, o sistema deve redirecionar para o 
        //        // o RAV Personalizado conforme solicitação de Canais
        //        if (!rbtDiario.Checked)
        //        {
        //            RAVAuto_Personalizar(sender, e);
        //        }
        //        else
        //        {
        //            try
        //            {
        //                QueryStringSegura queryString = new QueryStringSegura();
        //                queryString["AcessoSenha"] = _validaSenha;

        //                char tipoVenda = ' ';
        //                if (rbtTipoVenda_Ambos.Checked == true) { tipoVenda = 'A'; }
        //                else if (rbtTipoVenda_Avista.Checked == true) { tipoVenda = 'V'; }
        //                else if (rbtTipoVenda_Parcelado.Checked == true) { tipoVenda = 'P'; }


        //                char periodoRecebimento = ' ';
        //                if (rbtDiario.Checked == true) { periodoRecebimento = 'D'; }
        //                else if (rbtSemanal.Checked == true) { periodoRecebimento = 'S'; }
        //                else if (rbtQuinzenal.Checked == true) { periodoRecebimento = 'Q'; }
        //                else if (rbtMensal.Checked == true) { periodoRecebimento = 'M'; }


        //                ServicoPortalRAVClient cliente = new ServicoPortalRAVClient();
        //                ModRAVAutomatico automatico = null;
        //                automatico = cliente.ConsultarRAVAutomatico(SessaoAtual.CodigoEntidade, Convert.ToChar(tipoVenda), Convert.ToChar(periodoRecebimento));

        //                //if (automatico.ValorMinimo < 30)
        //                //{
        //                //    base.Alert("Não existe saldo suficiente.", false);
        //                //}

        //                //else
        //                //{
        //                queryString["RavAutomaticoTipoVenda"] = tipoVenda.ToString();
        //                queryString["RavAutomaticoPeriodoRecebimento"] = periodoRecebimento.ToString();
        //                queryString["RavAutomaticoTipoAntecipacao"] = automatico.DadosRetorno.NomeProdutoAntecipacao;

        //                try
        //                {
        //                    Response.Redirect(string.Format("pn_ConfirmacaoRavAutomatico.aspx?dados={0}", queryString.ToString()));
        //                }
        //                catch (Exception ex)
        //                {
        //                    Log.GravarErro(ex);
        //                    SharePointUlsLog.LogErro(ex);
        //                }
        //            }
        //            catch (FaultException<ServicoRAVException> ex)
        //            {
        //                Log.GravarErro(ex);
        //                this.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
        //                SharePointUlsLog.LogErro(ex);
        //            }
        //            catch (Exception ex)
        //            {
        //                Log.GravarErro(ex);
        //                this.ExibirPainelExcecao(FONTE, COGIDO_ERRO_RAVAUTO_CONTINUAR);
        //                SharePointUlsLog.LogErro(ex);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Redireciona o usuário para a página de personalização do RAV Automático.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RAVAuto_Personalizar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Principal - Personalizar RAV Automático"))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura();

                    char tipoVenda = 'A';
                    char periodoRecebimento = 'D';

                    queryString["RavAutomaticoTipoVenda"] = tipoVenda.ToString();
                    queryString["RavAutomaticoPeriodoRecebimento"] = periodoRecebimento.ToString();
                    queryString["RavAutomaticoValorMinimo"] = valorMinimoAutomatico;
                    queryString["RavAutomaticoTipoVenda"] = !string.IsNullOrEmpty(tipoVendaAutomatico) ? tipoVendaAutomatico : tipoVenda.ToString();
                    queryString["TipoRavSelecionado"] = "Auto";

                    Response.Redirect(string.Format("pn_Rav.aspx?dados={0}", queryString.ToString()), false);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirPainelExcecao(FONTE, COGIDO_ERRO_RAVAUTO_PERSONALIZAR);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

        /// <summary>
        /// Método chamado quando o usuário clica em Confirmar Email, quando ele ainda não possui um email cadastrado no RAV.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCadastrarEmail_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("PrincipalUserControl.ascx - btnCadastrarEmail_Click"))
            {
                try
                {
                    if (ckdEmailInformacoes.Checked)
                    {
                        ModRAVEmailEntradaSaida ravEmailsAtuais = null;

                        // obtem os emails atuais
                        using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                        {
                            ravEmailsAtuais = cliente.ConsultarEmails(this.SessaoAtual.CodigoEntidade);
                        }

                        if (ravEmailsAtuais == null)
                            throw new Exception(String.Format("Serviço do RAV ConsultarEmails retornou nulo para o PV: {0}", this.SessaoAtual.CodigoEntidade));

                        //verifica se o PV ja possui 3 emails cadastrados
                        if (ravEmailsAtuais.ListaEmails.Count == 3)
                        {
                            String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { RavOpenFeedbackUser('O pv já possui 3 e-mails cadastrado.', false); }, 'SP.UI.Dialog.js');";
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenFeedback", javaScript, true);
                            return;
                        }

                        EPeriodicidadeEmail periodicidadeEmail = EPeriodicidadeEmail.Diario;
                        if (rbtPeriodoDiario.Checked)
                        { periodicidadeEmail = EPeriodicidadeEmail.Diario; }
                        else if (rbtPeriodoSemanal.Checked)
                        { periodicidadeEmail = EPeriodicidadeEmail.Semanal; }
                        else if (rbtPeriodoQuinzenal.Checked)
                        { periodicidadeEmail = EPeriodicidadeEmail.Quinzenal; }
                        else if (rbtPeriodoMensal.Checked)
                        { periodicidadeEmail = EPeriodicidadeEmail.Mensal; }

                        ravEmailsAtuais.NumeroPDV = SessaoAtual.CodigoEntidade;

                        ravEmailsAtuais.IndEnviaEmail = 'S';
                        ravEmailsAtuais.IndEnviaFluxoCaixa = ' ';
                        ravEmailsAtuais.IndEnviaValoresPV = ' ';
                        ravEmailsAtuais.IndEnviaResumoOperacao = ' ';

                        Int32 sequencia = ObterNumeroSequencial(ravEmailsAtuais.ListaEmails.Select(x => x.Sequencia));

                        if (sequencia == 0)
                            throw new Exception(String.Format("Proxima sequencia nao localizada para os emails do PV: {0}", this.SessaoAtual.CodigoEntidade));

                        ravEmailsAtuais.ListaEmails.Add(new ModRAVEmail()
                        {
                            Sequencia = sequencia,
                            Email = txtEmailCadastro.Value,
                            Status = EStatusEmail.Incluso,
                            Periodicidade = periodicidadeEmail
                        });


                        using (ServicoPortalRAVClient servicoPortal = new ServicoPortalRAVClient())
                        {
                            if (servicoPortal.SalvarEmails(ravEmailsAtuais))
                            {
                                String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { RavOpenFeedbackUser('cadastro do e-mail efetuado com sucesso.', true); }, 'SP.UI.Dialog.js');";
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenFeedback", javaScript, true);

                                //Registro no histórico/log de atividades
                                Historico.RealizacaoServico(SessaoAtual, "Cadastro de E-mail Antecipação");

                                //Salva no view state o email cadastrado
                                this.UltimoEmail = txtEmailCadastro.Value;
                            }
                        }

                        txtEmailCadastro.Value = String.Empty;

                        VerificarEmail();
                    }
                    else
                    {
                        pnlRetornoCheckBox.Visible = true;
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex.Message);
                    this.ExibirPainelExcecao(FONTE, CODIGO_ERRO_CONFIRMAE_EMAIL);
                }
            }
        }

        /// <summary>
        /// Link para trocar o quadro de listagem de emails para cadastro de email novo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkBtnCadastrarEmail_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("PrincipalUserControl.ascx - lnkBtnCadastrarEmail_Click"))
            {
                try
                {
                    pnlEmailAlterar.Visible = false;
                    pnlEmailCadastrar.Visible = true;
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex.Message);
                    this.ExibirPainelExcecao(FONTE, COGIDO_ERRO_RAVEMAIL);
                }
            }
        }

        /// <summary>
        /// Databound do repeater de Emails
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptEmails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ModRAVEmail item = (ModRAVEmail)e.Item.DataItem;

                Literal ltrEmail = (Literal)e.Item.FindControl("ltrEmail");
                Literal ltrPeriodo = (Literal)e.Item.FindControl("ltrPeriodo");
                LinkButton lnkExcluir = (LinkButton)e.Item.FindControl("lnkExcluir");

                ltrEmail.Text = item.Email;
                ltrPeriodo.Text = ObterDescricaoPeriodicidadeEmail(item.Periodicidade);
                lnkExcluir.CommandArgument = e.Item.ItemIndex.ToString();
            }
        }

        /// <summary>
        /// Exclusao de um email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkExcluir_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("PrincipalUserControl.ascx - lnkExcluir_Click"))
            {
                try
                {
                    LinkButton lnkExcluir = (LinkButton)sender;
                    Int32 indexItem = lnkExcluir.CommandArgument.ToInt32();

                    ServicoPortalRAVClient cliente = new ServicoPortalRAVClient();

                    var ravEmailEntrada = cliente.ConsultarEmails(SessaoAtual.CodigoEntidade);
                    List<ModRAVEmail> ravEmailList = ravEmailEntrada.ListaEmails;

                    ravEmailEntrada.NumeroPDV = this.SessaoAtual.CodigoEntidade;
                    ravEmailEntrada.IndEnviaEmail = 'S';
                    ravEmailEntrada.IndEnviaFluxoCaixa = ' ';
                    ravEmailEntrada.IndEnviaResumoOperacao = ' ';
                    ravEmailEntrada.IndEnviaValoresPV = ' ';

                    //verifica se o indice existe na lista
                    ModRAVEmail emailExcluir = ravEmailEntrada.ListaEmails.ElementAtOrDefault(indexItem);
                    if (emailExcluir != null)
                    {
                        emailExcluir.Status = EStatusEmail.Excluido;
                        log.GravarMensagem(String.Format("PV: {0} Email: {1} - Status: {2}", this.SessaoAtual.CodigoEntidade, emailExcluir.Email, emailExcluir.Status));

                        Boolean status = cliente.SalvarEmails(ravEmailEntrada);
                        if (status)
                        {
                            //Registro no histórico/log de atividades
                            Historico.RealizacaoServico(SessaoAtual, "Exclusão de E-mail Antecipação");

                            String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { RavOpenFeedbackUser('e-mail excluído com sucesso.', false); }, 'SP.UI.Dialog.js');";
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenFeedback", javaScript, true);
                        }
                        else
                        {
                            String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { RavOpenFeedbackUser('erro durante a exclusão de e-mail.', false); }, 'SP.UI.Dialog.js');";
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenFeedback", javaScript, true);
                        }
                        VerificarEmail();
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, this.CodigoErroCadastrar);
                }
            }
        }

        /// <summary>
        /// Desfazimento do cadastro de um novo email, ou seja, exclusao
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkBtnDesfazer_Click(object sender, EventArgs e)
        {
            // exclui o email que acabou de ser criado
            using (Logger log = Logger.IniciarLog("PrincipalUserControl.ascx - lnkBtnDesfazer_Click"))
            {
                try
                {
                    ServicoPortalRAVClient cliente = new ServicoPortalRAVClient();

                    var ravEmailEntrada = cliente.ConsultarEmails(SessaoAtual.CodigoEntidade);
                    List<ModRAVEmail> ravEmailList = ravEmailEntrada.ListaEmails;

                    ravEmailEntrada.NumeroPDV = this.SessaoAtual.CodigoEntidade;
                    ravEmailEntrada.IndEnviaEmail = 'S';
                    ravEmailEntrada.IndEnviaFluxoCaixa = ' ';
                    ravEmailEntrada.IndEnviaResumoOperacao = ' ';
                    ravEmailEntrada.IndEnviaValoresPV = ' ';

                    //verifica se o indice existe na lista
                    ModRAVEmail emailExcluir = ravEmailEntrada.ListaEmails.FirstOrDefault(em => em.Email == this.UltimoEmail);
                    if (emailExcluir != null)
                    {
                        emailExcluir.Status = EStatusEmail.Excluido;
                        log.GravarMensagem(String.Format("PV: {0} Email: {1} - Status: {2}", this.SessaoAtual.CodigoEntidade, emailExcluir.Email, emailExcluir.Status));

                        Boolean status = cliente.SalvarEmails(ravEmailEntrada);
                        if (status)
                        {
                            //Registro no histórico/log de atividades
                            Historico.RealizacaoServico(SessaoAtual, "Exclusão de E-mail Antecipação");
                        }
                        else
                        {
                            String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { RavOpenFeedbackUser('erro durante a exclusão de e-mail.', false); }, 'SP.UI.Dialog.js');";
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenFeedback", javaScript, true);
                        }
                        VerificarEmail();
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, this.CodigoErroCadastrar);
                }
            }
        }

        /// <summary>
        /// Databound do repeater de bandeiras
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void rptBandeiras_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        ModRAVAutomaticoBandeira item = (ModRAVAutomaticoBandeira)e.Item.DataItem;

        //        Image imgBandeira = (Image)e.Item.FindControl("imgBandeira");
        //        Literal ltrNomeBandeiras = (Literal)e.Item.FindControl("ltrNomeBandeiras");

        //        ltrNomeBandeiras.Text = item.DscBandeira;

        //    }
        //}

        /// <summary>
        /// Databound do repeater das ultimas antecipacoes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptUltimasAntecipacoes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                WAExtratoAntecipacaoRAVServico.RAV item = (WAExtratoAntecipacaoRAVServico.RAV)e.Item.DataItem;

                Literal ltrDataCredito = (Literal)e.Item.FindControl("ltrDataCredito");
                Literal ltrBandeira = (Literal)e.Item.FindControl("ltrBandeira");
                Literal ltrValorLiquido = (Literal)e.Item.FindControl("ltrValorLiquido");

                ltrDataCredito.Text = item.DataAntecipacao.ToString("dd/MM/yy");
                ltrBandeira.Text = item.TipoBandeira;
                ltrValorLiquido.Text = item.ValorAntecipacao.ToString("N2");
            }
        }

        /// <summary>
        /// Obtem o proximo numero sequencial disponivel para o cadastro de email
        /// </summary>
        /// <param name="ints"></param>
        /// <returns></returns>
        public int ObterNumeroSequencial(IEnumerable<int> ints)
        {
            int counter = 1;

            while (counter < 4)
            {
                if (!ints.Contains(counter)) return counter;
                counter++;
            }

            return 0;
        }

        /// <summary>
        /// De-para do enum de dias da semana
        /// </summary>
        /// <param name="diaSemana"></param>
        /// <returns></returns>
        private string ObterDescricaoDiaSemana(EDiaSemana diaSemana)
        {
            String retorno = String.Empty;
            switch (diaSemana)
            {
                case EDiaSemana.Segunda:
                    retorno = "segunda";
                    break;
                case EDiaSemana.Terca:
                    retorno = "terça";
                    break;
                case EDiaSemana.Quarta:
                    retorno = "quarta";
                    break;
                case EDiaSemana.Quinta:
                    retorno = "quinta";
                    break;
                case EDiaSemana.Sexta:
                    retorno = "sexta";
                    break;
                default:
                    retorno = diaSemana.ToString();
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Método para exibir painel de informações
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensagem"></param>
        private void ExibirMensagem(string titulo, string mensagem)
        {
            Panel[] paineisDados = new Panel[1]{
                            pnlDadosGerais
                    };

            QueryStringSegura queryString = new QueryStringSegura();
            pnlDadosGerais.Visible = false;
            base.ExibirPainelConfirmacaoAcao(titulo, mensagem, Request.UrlReferrer.AbsoluteUri, paineisDados);
        }

        /// <summary>
        /// Exibe o painel de excecao
        /// </summary>
        /// <param name="fonteErro"></param>
        /// <param name="codigoErro"></param>
        private void ExibirPainelExcecao(String fonteErro, Int32 codigoErro)
        {
            if (!excessaoLancada)
            {
                excessaoLancada = true;
                base.ExibirPainelExcecao(fonteErro, codigoErro);
            }
        }

        /// <summary>
        /// De-para do enum de periodicidade do email
        /// </summary>
        /// <param name="periodicidade"></param>
        /// <returns></returns>
        private String ObterDescricaoPeriodicidadeEmail(EPeriodicidadeEmail periodicidade)
        {
            String retorno = String.Empty;
            switch (periodicidade)
            {
                case EPeriodicidadeEmail.Diario:
                    retorno = "diário";
                    break;
                case EPeriodicidadeEmail.Semanal:
                    retorno = "semanal";
                    break;
                case EPeriodicidadeEmail.Quinzenal:
                    retorno = "quinzenal";
                    break;
                case EPeriodicidadeEmail.Mensal:
                    retorno = "mensal";
                    break;
                default:
                    retorno = periodicidade.ToString();
                    break;
            }

            return retorno;
        }
    }
}
