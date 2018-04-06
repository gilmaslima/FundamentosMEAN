using Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico;
using Rede.PN.Eadquirencia.Sharepoint.Helper;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.FacaSuaVenda
{
    /// <summary>
    /// Enum para saber o tipo de transação.
    /// </summary>
    public enum TiposTransacaoAdquirencia
    {
        CreditoAVista = 04,
        CreditoParceladoEmissor = 06,
        CreditoParceladoEstabelecimento = 08,
        IataAVista = 39,
        IataParceladoEstabelecimento = 40,
        Autorizacao = 74
    }

    /// <summary>
    /// Webpart de faça sua venda.
    /// </summary>
    public partial class FacaSuaVendaUserControl : WebpartBase
    {
        /// <summary>
        /// Viewstate com as informações do método GetAuthorizedCredit.
        /// </summary>
        public EadquirenciaServico.GetAuthorizedCredit RequestAutorizacaoViewState
        {
            get
            {
                return ViewState["requestAutorizacao"] as EadquirenciaServico.GetAuthorizedCredit;
            }
            set
            {
                ViewState["requestAutorizacao"] = value;
            }
        }

        /// <summary>
        /// Viewstate com as informações da tela.
        /// </summary>
        public DadosComprovanteFacaSuaVenda ComprovanteFacaSuaVenda
        {
            get
            {
                return ViewState["comprovanteFacaSuaVenda"] as DadosComprovanteFacaSuaVenda;
            }
            set
            {
                ViewState["comprovanteFacaSuaVenda"] = value;
            }
        }

        #region Eventos
        /// <summary>
        /// Page Load.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecucaoTratada("Faça sua venda - Page_Load", () =>
                {
                    if (!IsPostBack)
                    {
                        int codigoGrupoRamo;
                        int codigoRamoAtividade;
                        int quantidadeParcelas;

                        if (!Sessao.Contem())
                            throw new Exception("Falha ao obter sessão.");

                        codigoGrupoRamo = SessaoAtual.CodigoGrupoRamo;
                        codigoRamoAtividade = SessaoAtual.CodigoRamoAtividade;
                        
                        using (ContextoWCF<EadquirenciaServico.ServicoEAdquirenciaClient> contexto = new ContextoWCF<EadquirenciaServico.ServicoEAdquirenciaClient>())
                        {
                            quantidadeParcelas = contexto.Cliente.ConsultarNumeroParcelas(codigoGrupoRamo, codigoRamoAtividade);
                        }
                        for (int i = 2; i <= quantidadeParcelas; i++)
                        {
                         ddlNumeroParcelas.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        }

                        CarregarComboAno();
                        CarregarTelaInicial();
                    }
                });
        }

        /// <summary>
        /// Evento de Click Voltar do multiview.
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Faça sua venda - btnVoltar_Click", () =>
                {
                    switch (mvFacaSuaVenda.ActiveViewIndex)
                    {
                        case 0:
                            LimparCampos(true);
                            btnImprimir.Visible = false;
                            btnAvancar.Visible = true;
                            break;
                        case 1:
                            ContinuarVenda();
                            break;
                        case 2:
                            ContinuarVenda();
                            break;
                        default:
                            throw new Exception("Passo não implementado.");
                            break;
                    }
                });
        }

        /// <summary>
        /// Evento do botão Exportar PDF.
        /// </summary>
        protected void btnExportarPdf_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Faça sua venda - btnExportarPdf_Click", () =>
                {
                    Byte[] pdf = null;

                    using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                    {
                        pdf = ctx.Cliente.GerarPDFComprovanteFacaSuaVenda(ComprovanteFacaSuaVenda);
                    }

                    String nomeArquivo = String.Format("attachment; filename=ComprovanteVenda_{0}.pdf", DateTime.Now.ToString("ddMMyyyy_HHmmss"));

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", nomeArquivo);
                    Response.BinaryWrite(pdf);

                    Response.End();
                });
        }

        /// <summary>
        /// Evento do Botão Exportar Excel.
        /// </summary>
        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Faça sua venda - btnExportarExcel_Click", () =>
                {
                    Byte[] csv = null;

                    using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                    {
                        csv = ctx.Cliente.GerarCSVComprovanteFacaSuaVenda(ComprovanteFacaSuaVenda);
                    }

                    String nomeArquivo = String.Format("attachment; filename=ComprovanteVenda_{0}.csv", DateTime.Now.ToString("ddMMyyyy_HHmmss"));

                    Response.ContentType = "text/csv";
                    Response.AddHeader("content-disposition", nomeArquivo);
                    Response.BinaryWrite(csv);

                    Response.End();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                });
        }

        /// <summary>
        /// Evento do botão enviar comprovante (SMS ou Email)
        /// </summary>
        protected void btnEnviarComprovante_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Faça sua venda - btnEnviarComprovante_Click", () =>
                {
                    String msgErroEmail = String.Empty;
                    String msgErroSms = String.Empty;
                    String msgEmail = String.Empty;
                    String msgSms = String.Empty;
                    bool emailChecado = chkEmail.Checked;
                    bool smsChecado = chkSms.Checked;
                    StringBuilder msg = new StringBuilder();

                    using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                    {
                        if (emailChecado)
                        {
                            msgEmail = ctx.Cliente.GerarMensagemEmail(ComprovanteFacaSuaVenda);

                            String[] destinatario = new String[1];
                            destinatario[0] = txtEmailComprovante.Text;

                            if (EmailHelper.EnviarEmail(destinatario, "Comprovante de Venda", msgEmail, out msgErroEmail))
                                msg.Append("E-mail do comprovante enviado com sucesso. ");
                            else
                                msg.Append(msgErroEmail);
                        }

                        if (smsChecado)
                        {
                            msgSms = ctx.Cliente.GerarMensagemSms(ComprovanteFacaSuaVenda);
                            Regex rgx = new Regex(@"[^\d]");
                            String celular = rgx.Replace(txtCelularComprovante.Text, "");
                            celular = String.Concat("55", celular);

                            if (SMSHelper.EnviarSMS(msgSms, celular, out msgErroSms))
                                msg.Append("Sms do comprovante enviado com sucesso. ");
                            else
                                msg.Append(msgErroSms);
                        }
                    }

                    if (msg.Length > 0)
                        ExibirMensagem(msg.ToString());
                });
        }

        /// <summary>
        /// Evento de Click Próximo do multiview
        /// </summary>
        protected void btnAvancar_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Faça sua venda - btnAvancar_Click", () =>
                {
                    switch (mvFacaSuaVenda.ActiveViewIndex)
                    {
                        case 0:
                            TratarPassoInicialParaConfirmacao();
                            break;
                        case 1:
                            TratarPassoConfirmacaoParaComprovante();
                            break;
                        case 2:
                            TratarPassoComprovanteParaInicial();
                            break;
                        default:
                            throw new Exception("Passo não implementado.");
                    }
                });
        }
        #endregion

        #region Métodos

        /// <summary>
        /// Método com as configurações da view para continuar venda
        /// </summary>
        private void ContinuarVenda()
        {
            LimparCampos(false);
            btnAvancar.ValidationGroup = "ContinuarVenda";
            btnAvancar.Text = "Continuar";
            btnAvancar.Visible = true;
            btnVoltar.Text = " Limpar";
            pnlCamposObrigatorios.Visible = true;
            btnImprimir.Visible = false;
            mvFacaSuaVenda.SetActiveView(vwDadosVenda);
            ucAssistente.Voltar();
        }

        private void CarregarTelaInicial()
        {
            ddlTipoTransacao.SelectedValue = "1";
            ddlMesValidade.SelectedValue = "";
            ddlAnoValidade.SelectedValue = "";
            divCodigoIata.Visible = false;
            divTaxaEmbarque.Visible = false;
            divComprovanteCodigoIata.Visible = false;
            divComprovanteTaxaEmbarque.Visible = false;
            chkEmail.Checked = false;
            chkSms.Checked = false;
            txtEmailComprovante.Text = String.Empty;
            txtConfirmacaoEmail.Text = String.Empty;
            txtCelularComprovante.Text = String.Empty;
            txtConfirmacaoCelular.Text = String.Empty;
            txtCodigoIata.Text = String.Empty;
            txtTaxaEmbarque.Text = String.Empty;
            btnAvancar.Visible = true;
        }

        /// <summary>
        /// Método do passo 1.
        /// </summary>
        private void TratarPassoConfirmacaoParaComprovante()
        {

            String nomeEntidade = String.Empty;
#if !DEBUG
            nomeEntidade = SessaoAtual.NomeEntidade;

            EadquirenciaServico.GetAuthorizedCredit requestCredit = RequestAutorizacaoViewState;

            if (requestCredit == null)
                throw new Exception("Falha ao obter view state");

            EadquirenciaServico.CreditAuthorizationResponse respostaServico = null;

            using (ContextoWCF<EadquirenciaServico.ServicoEAdquirenciaClient> contexto = new ContextoWCF<EadquirenciaServico.ServicoEAdquirenciaClient>())
            {
                respostaServico = contexto.Cliente.GetAuthorizedCredit(requestCredit);
            }
#else
            EadquirenciaServico.CreditAuthorizationResponse respostaServico = new EadquirenciaServico.CreditAuthorizationResponse() { CodRet = "00", Msgret = "sucesso", Tid = "4732984", NumAutor = "666666", NumPedido = "6768768", NumSqn = "753745", Data = "20161230", Hora = "09:09", ValParcelas = "10000", ValTotalJuros = "1", Cet = "11000" };
            nomeEntidade = "TAM Linhas Areas";
#endif

            btnAvancar.ValidationGroup = String.Empty;
            btnAvancar.Text = "Concluir";
            btnVoltar.Text = "Voltar";
            pnlCamposObrigatorios.Visible = false;
            mvFacaSuaVenda.SetActiveView(vwComprovante);
            ucAssistente.Avancar();

            if (String.Compare(respostaServico.CodRet, "00") != 0)
            {
                pnlNaoAprovada.Visible = true;
                btnVoltar.Visible = true;

                pnlAprovada.Visible = false;
                btnAvancar.Visible = false;
                btnImprimir.Visible = false;
                btnExportarExcel.Visible = false;
                btnExportarPdf.Visible = false;

                litCodRet.Text = respostaServico.CodRet;
                litMsgRet.Text = respostaServico.Msgret;

                this.ExibirMensagem("Venda não realizada. Por favor, verifique junto ao emissor do cartão.");
            }
            else
            {
                DateTime dataTransacao;
                DateTime.TryParseExact(respostaServico.Data, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataTransacao);

                litComprovanteTid.Text                  = respostaServico.Tid;
                litComprovanteNumeroAutorizacao.Text    = respostaServico.NumAutor;
                litComprovanteNumeroPedido.Text         = litNumeroPedido.Text;
                litComprovanteNSU.Text                  = respostaServico.NumSqn;
                litComprovanteDataTransacao.Text        = dataTransacao.ToString("dd/MM/yyyy");
                litValorTotal.Text                      = litValorVenda.Text;
                litValorParcelas.Text                   = String.Format("{0}x  R$ {1}", RequestAutorizacaoViewState.Parcelas, TextHelper.ConverterValorMonetario(respostaServico.ValParcelas));
                litValorTotalJuros.Text                 = String.Format("R$ {0}", TextHelper.ConverterValorMonetario(respostaServico.Cet));

                litComprovanteTipoTransacao.Text        = litTipoTransacao.Text;
                litComprovanteFormaPagamento.Text       = litFormaPagamento.Text;
                litComprovanteBandeira.Text             = litBandeira.Text;
                litComprovanteNomePortador.Text         = litNomePortador.Text;
                litComprovanteCartao.Text               = litNumCartao.Text;

                if (ddlTipoTransacao.SelectedItem.Text == "IATA")
                {
                    divComprovanteCodigoIata.Visible = true;
                    divComprovanteTaxaEmbarque.Visible = true;
                    litComprovanteCodigoIata.Text = txtCodigoIata.Text;
                    litComprovanteTaxaEmbarque.Text = txtTaxaEmbarque.Text;
                }
                else
                {
                    divComprovanteCodigoIata.Visible = false;
                    divComprovanteTaxaEmbarque.Visible = false;
                    litComprovanteCodigoIata.Text = String.Empty;
                    litComprovanteTaxaEmbarque.Text = String.Empty;
                }

                DadosComprovanteFacaSuaVenda dados = new DadosComprovanteFacaSuaVenda()
                {
                    NumeroCartao            = txtNumCartaoCredito.Text.Substring(txtNumCartaoCredito.Text.Length - 4, 4),
                    Bandeira                = RetornarBandeiraCartao(txtNumCartaoCredito.Text.Replace(" ", String.Empty)),
                    NumeroAutorizacao       = respostaServico.NumAutor,
                    Tid                     = respostaServico.Tid,
                    NomeEstabelecimento     = nomeEntidade,
                    TipoVenda               = ddlTipoTransacao.SelectedItem.Text,
                    ValorVenda              = litValorVenda.Text,
                    Cet                     = TextHelper.ConverterValorMonetario(respostaServico.Cet),
                    Parcelas                = RequestAutorizacaoViewState.Parcelas,
                    ValorParcelas           = TextHelper.ConverterValorMonetario(respostaServico.ValParcelas),
                    Data                    = dataTransacao.ToString("dd/MM/yyyy"),
                    Hora                    = respostaServico.Hora,
                    NumeroPedido            = litNumeroPedido.Text,
                    NumeroComprovante       = litComprovanteNSU.Text,
                    FormaPagamento          = litFormaPagamento.Text,
                    NomePortador            = litNomePortador.Text,
                    TaxaEmbarque            = litTaxaEmbarque.Text
                };

                ComprovanteFacaSuaVenda = dados;

                pnlNaoAprovada.Visible = false;
                btnVoltar.Visible = false;

                pnlAprovada.Visible = true;
                btnAvancar.Visible = true;
                btnImprimir.Visible = true;
                btnExportarExcel.Visible = true;
                btnExportarPdf.Visible = true;

                this.ExibirMensagem("Venda realizada com sucesso.");

            }
        }

        /// <summary>
        /// Método do passo 2.
        /// </summary>
        private void TratarPassoInicialParaConfirmacao()
        {
            EadquirenciaServico.GetAuthorizedCredit requestCredit = ValidarInputsERetornarRequest();
            if (requestCredit == null)
                return;

            RequestAutorizacaoViewState = requestCredit;

            PreencherConfirmacao();

            btnAvancar.Text = "Confirmar";
            btnVoltar.Text = "Voltar";
            pnlCamposObrigatorios.Visible = false;
            btnImprimir.Visible = false;
            mvFacaSuaVenda.SetActiveView(vwConfirmacao);
            ucAssistente.Avancar();

            btnAvancar.ValidationGroup = String.Empty;
        }

        /// <summary>
        /// Método do passo 3.
        /// </summary>
        private void TratarPassoComprovanteParaInicial()
        {
            mvFacaSuaVenda.SetActiveView(vwDadosVenda);
            btnVoltar.Text = " Limpar";
            btnAvancar.Text = "Continuar";
            btnAvancar.ValidationGroup = "ContinuarVenda";
            LimparCampos(true);
            btnExportarPdf.Visible = false;
            btnExportarExcel.Visible = false;
            btnImprimir.Visible = false;
            btnVoltar.Visible = true;
            pnlCamposObrigatorios.Visible = true;
            ucAssistente.AtivarPasso(0);
            CarregarTelaInicial();
        }

        /// <summary>
        /// Valida as informações da tela e faz a chamada do GetAuthorizedCredit.
        /// </summary>
        /// <returns>Objeto de retorno do GetAuthorizedCredit.</returns>
        private EadquirenciaServico.GetAuthorizedCredit ValidarInputsERetornarRequest()
        {

            if (String.IsNullOrWhiteSpace(txtValorVenda.Text)
                || String.IsNullOrWhiteSpace(ddlTipoTransacao.SelectedValue)
                || String.IsNullOrWhiteSpace(txtNomePortador.Text)
                || String.IsNullOrWhiteSpace(txtNumCartaoCredito.Text)
                || String.IsNullOrWhiteSpace(txtCodigoSeguranca.Text)
                || String.IsNullOrWhiteSpace(ddlMesValidade.SelectedValue)
                || String.IsNullOrWhiteSpace(ddlAnoValidade.SelectedValue))
            {
                ExibirMensagem("Campos obrigatórios ausentes.");
                return null;
            }

            IFormatProvider culturaBr = CultureInfo.GetCultureInfo("pt-BR");
            Decimal checkValorVenda;
            String valorVenda = txtValorVenda.Text.Replace("R$ ", String.Empty);
            if (!Decimal.TryParse(valorVenda, NumberStyles.Number, culturaBr, out checkValorVenda)
                || checkValorVenda > 99999999.99M)
            {
                ExibirMensagem("Campo Valor da Venda incorreto.");
                return null;
            }


            int checkInt;

            if (txtCodigoSeguranca.Text.Length > 3
                || !int.TryParse(txtCodigoSeguranca.Text, out checkInt))
            {
                ExibirMensagem("Código de Segurança incorreto.");
                return null;
            }

            if (txtNomePortador.Text.Length > 40)
            {
                ExibirMensagem("Nome do Portador incorreto.");
                return null;
            }

            long checkNumero;
            String numeroCartaoCredito = txtNumCartaoCredito.Text.Replace(" ", String.Empty);
            if (numeroCartaoCredito.Length > 16
                || !long.TryParse(numeroCartaoCredito, out checkNumero))
            {
                ExibirMensagem("Número do cartão incorreto.");
                return null;
            }

            if (!int.TryParse(ddlMesValidade.SelectedValue, out checkInt)
                || checkInt > 12
                || !int.TryParse(ddlAnoValidade.SelectedValue, out checkInt))
            {
                ExibirMensagem("Validade do cartão incorreto.");
                return null;
            }

            if (!String.IsNullOrEmpty(txtCodigoIata.Text)
                && (!int.TryParse(txtCodigoIata.Text, out checkInt)
                || txtCodigoIata.Text.Length > 9))
            {
                ExibirMensagem("Valor do Código Iata incorreto.");
                return null;
            }

            Decimal checkTaxaEmbarque = 0;
            if (!String.IsNullOrEmpty(txtTaxaEmbarque.Text)
            && !Decimal.TryParse(txtTaxaEmbarque.Text.Replace("R$ ", String.Empty), NumberStyles.Number, culturaBr, out checkTaxaEmbarque))
            {
                ExibirMensagem("Valor da Taxa de Embarque incorreto.");
                return null;
            }

            String numeroPv = String.Empty;
            if (!Sessao.Contem())
                throw new Exception("Falha ao obter sessão.");

             numeroPv = SessaoAtual.CodigoEntidade.ToString();

            TiposTransacaoAdquirencia tipoTransacao;


            int valorTipoTransacaoSelecionado = int.Parse(ddlTipoTransacao.SelectedValue);
            int valorFormaPagamentoSelecionado = int.Parse(ddlFormaPagamento.SelectedValue);
            string numeroParcelasSelecionado = String.Empty;

            //coloca parcelas dependendo do tipo da forma de pagamento
            if (valorFormaPagamentoSelecionado == 1) //a vista
            {
                numeroParcelasSelecionado = "0";
            }
            else
            {
                numeroParcelasSelecionado = ddlNumeroParcelas.SelectedValue;
            }


            //~tipo de transação:
            if (valorTipoTransacaoSelecionado == 1) //crédito
            {
                switch (valorFormaPagamentoSelecionado)
                {
                    case 1: // a vista
                        tipoTransacao = TiposTransacaoAdquirencia.CreditoAVista;
                        break;
                    case 2: //parcelado estab
                        tipoTransacao = TiposTransacaoAdquirencia.CreditoParceladoEstabelecimento;
                        break;
                    case 3: //parcelado emissor
                        tipoTransacao = TiposTransacaoAdquirencia.CreditoParceladoEmissor;
                        break;
                    default:
                        tipoTransacao = 00;
                        break;
                }
            }
            else if (valorTipoTransacaoSelecionado == 2) //autorização (pré)
            {
                tipoTransacao = TiposTransacaoAdquirencia.Autorizacao;
            }
            else //IATA
            {
                if (valorFormaPagamentoSelecionado == 1)
                    tipoTransacao = TiposTransacaoAdquirencia.IataAVista;
                else
                    tipoTransacao = TiposTransacaoAdquirencia.IataParceladoEstabelecimento;
            }


            EadquirenciaServico.GetAuthorizedCredit requestCredit = new EadquirenciaServico.GetAuthorizedCredit
            {
                Ano                     = ddlAnoValidade.SelectedValue,
                Cvc2                    = txtCodigoSeguranca.Text,
                Filiacao                = numeroPv,
                Iata                    = txtCodigoIata.Text,
                Mes                     = ddlMesValidade.SelectedValue,
                Nrcartao                = txtNumCartaoCredito.Text.Replace(" ", String.Empty),
                NumPedido               = txtNumeroPedido.Text,
                Parcelas                = numeroParcelasSelecionado,
                Portador                = txtNomePortador.Text,
                TaxaEmbarque            = String.IsNullOrEmpty(txtTaxaEmbarque.Text) ? null : checkTaxaEmbarque.ToString("0.00", CultureInfo.InvariantCulture),
                Total                   = checkValorVenda.ToString("0.00", CultureInfo.InvariantCulture),
                Transacao               = (int)tipoTransacao,
                Origem                  = "01", //VALOR fixo segundo a spec
                Recorrente              = "0", //VALOR fixo segundo a spec
            };


            return requestCredit;
        }

        /// <summary>
        /// Método de apoio que preenche as informações do passo de Confirmação.
        /// </summary>
        private void PreencherConfirmacao()
        {
            litTipoTransacao.Text = ddlTipoTransacao.SelectedItem.Text;
            litFormaPagamento.Text = ddlFormaPagamento.SelectedItem.Text;
            litBandeira.Text = RetornarBandeiraCartao(txtNumCartaoCredito.Text.Replace(" ", String.Empty));
            litNomePortador.Text = txtNomePortador.Text.ToUpper();
            litNumCartao.Text = txtNumCartaoCredito.Text.Substring(txtNumCartaoCredito.Text.Length - 4, 4);
            litValorVenda.Text = txtValorVenda.Text;
            litNumeroPedido.Text = txtNumeroPedido.Text;

            if (ddlTipoTransacao.SelectedItem.Text == "IATA")
            {
                divCodigoIata.Visible = true;
                divTaxaEmbarque.Visible = true;
                litCodigoIata.Text = txtCodigoIata.Text;
                litTaxaEmbarque.Text = txtTaxaEmbarque.Text;
            }
            else
            {
                divCodigoIata.Visible = false;
                divTaxaEmbarque.Visible = false;
                litCodigoIata.Text = String.Empty;
                litTaxaEmbarque.Text = String.Empty;
            }
        }

        /// <summary>
        /// Método de apoio que limpa os campos da tela.
        /// </summary>
        /// <param name="limparTodos">True se é pra limpar todos os campos (inclusive o IATA).</param>
        private void LimparCampos(bool limparTodos)
        {
            if (limparTodos)
            {
                txtCodigoIata.Text = String.Empty;
                txtNomePortador.Text = String.Empty;
                txtNumeroPedido.Text = String.Empty;
                txtTaxaEmbarque.Text = String.Empty;
                txtValorVenda.Text = String.Empty;
                ddlMesValidade.SelectedValue = "";
                ddlAnoValidade.SelectedValue = "";
            }

            txtNumCartaoCredito.Text = String.Empty;
            txtCodigoSeguranca.Text = String.Empty;
        }

        /// <summary>
        /// Método de apoio que retorna a descrição da bandeira baseado no número do cartão.
        /// </summary>
        /// <param name="numeroCartao">Número do cartão.</param>
        /// <returns>Descrição da bandeira.</returns>
        private String RetornarBandeiraCartao(String numeroCartao)
        {
            Dictionary<Regex, String> dic = new Dictionary<Regex, String>();
            dic.Add(new Regex(@"^3(6|8|0[0-5])"), "Diners Club");
            dic.Add(new Regex(@"^5[1-5]"), "Mastercard");
            dic.Add(new Regex(@"^(637(095|612|599|609))"), "Hiper");
            dic.Add(new Regex(@"^606282"), "Hipercard");
            dic.Add(new Regex(@"^4"), "Visa");

            foreach (var patt in dic)
            {
                if (patt.Key.IsMatch(numeroCartao))
                    return patt.Value;
            }

            return String.Empty;
        }

        /// <summary>
        /// Efetuar tratamento dos encoding das mensagens
        /// </summary>
        /// <param name="msg"></param>
        protected void ExibirMensagem(String msg)
        {
            base.ExibirPainelMensagem(TextHelper.FormatarMensagemJavaScript(msg));
        }

        /// <summary>
        /// Carregar Combo ano
        /// </summary>
        protected void CarregarComboAno()
        {
            List<Int32> years = Enumerable.Range((DateTime.Today.Year - 2000), 15).ToList();
            ddlAnoValidade.DataSource = years;
            ddlAnoValidade.DataBind();
            ddlAnoValidade.Items.Insert(0, new ListItem("", ""));
        }

        #endregion
    }
}