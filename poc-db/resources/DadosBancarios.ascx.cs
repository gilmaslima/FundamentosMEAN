using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;


namespace Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento
{
    public partial class DadosBancarios : UserControlCredenciamentoBase
    {
        #region Propriedades

        /// <summary>
        /// Produtos armazenados na Session de credenciamento
        /// </summary>
        private List<Modelo.Produto> Produtos
        {
            get
            {
                if (Credenciamento.Produtos == null)
                    Credenciamento.Produtos = new List<Modelo.Produto>();

                return (List<Modelo.Produto>)Credenciamento.Produtos;
            }
        }

        /// <summary>
        /// Domicilios bancarios armazenados na Session de credenciamento
        /// </summary>
        private List<Modelo.DomicilioBancario> DomiciliosBancarios
        {
            get
            {
                if (Credenciamento.DomiciliosBancarios == null)
                    Credenciamento.DomiciliosBancarios = new List<Modelo.DomicilioBancario>();

                return (List<Modelo.DomicilioBancario>)Credenciamento.DomiciliosBancarios;
            }
        }

        /// <summary>
        /// Produtos Parceiro armazenados na Session de credenciamento
        /// </summary>
        private List<Modelo.ProdutoParceiro> ProdutoParceiro
        {
            get
            {
                if (Credenciamento.ProdutoParceiro == null)
                    Credenciamento.ProdutoParceiro = new List<Modelo.ProdutoParceiro>();

                return (List<Modelo.ProdutoParceiro>)Credenciamento.ProdutoParceiro;
            }
        }

        /// <summary>
        /// Propostas armazenadas na Session de credenciamento
        /// </summary>
        private Modelo.Proposta Proposta
        {
            get
            {
                if (Credenciamento.Proposta == null)
                    Credenciamento.Proposta = new Modelo.Proposta();

                return Credenciamento.Proposta;
            }
        }

        /// <summary>
        /// Contas Correntes armazenadas na viewstate.
        /// </summary>
        private List<Modelo.ContaCorrente> ContasCorrentes
        {
            get
            {
                if (ViewState["ContasCorrentes"] == null)
                    ViewState["ContasCorrentes"] = new List<Modelo.ContaCorrente>();
                return (List<Modelo.ContaCorrente>)ViewState["ContasCorrentes"];
            }
        }

        /// <summary>
        /// Retorna valor do CNPJ/CPF do objeto Proposta
        /// </summary>
        public Int64 NumeroCpfCnpj
        {
            get
            {
                if (Credenciamento.Proposta != null)
                    return Credenciamento.Proposta.NumeroCnpjCpf;
                return 0;
            }
        }

        private StringBuilder stringBuilder;

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Controla visibilidade do place holder de domicílio bancário de débito. 
        /// </summary>
        /// <param name="visible">Controla visibilidade do placeholder de domicílio bancário de débito</param>
        private void ExibirDomicilioDebito(bool visible)
        {
            phDomicilioDebito.Visible = visible;
        }

        /// <summary>
        /// Controla visibilidade do place holder de domicílio bancário de Alelo Alimentacao. 
        /// </summary>
        /// <param name="visible">Controla visibilidade do placeholder de domicílio bancário de Alelo Alimentacao</param>
        private void ExibirDomicilioAleloAlimentacao(bool visible)
        {
            phDomicilioAleloAlimentacao.Visible = visible;
        }

        /// <summary>
        /// Valida agêcia através do serviço GE
        /// </summary>
        /// <param name="txtAgencia">TextBox Agência</param>
        /// <param name="ddlBanco">DropDownList Banco</param>
        /// <returns>Retorna boolean de validação do campo Agência</returns>
        private Boolean ValidaAgencia(TextBox txtAgencia, DropDownList ddlBanco)
        {
            if (ddlBanco.SelectedIndex <= 0)
                return false;

            Int32 codigoBanco = ddlBanco.SelectedValue.ToInt32();
            Int32 codigoAgencia = txtAgencia.Text.ToInt32();

            return Servicos.ServicosGE.ValidaAgencia(codigoBanco, codigoAgencia);
        }

        /// <summary>
        /// Caso o banco selecionado seja 104 - Caixa, o campo conta corrente deverá ser preenchido com 0, até completar 10 caracteres.
        /// </summary>
        /// <param name="txtContaCorrente">TextBox Conta Corrente</param>
        /// <param name="ddlBanco">DropDownList Banco</param>
        private void AutoCompletarContaCorrenteCaixa(TextBox txtContaCorrente, DropDownList ddlBanco)
        {
            try
            {
                String bancoSelecionado = ddlBanco.SelectedValue;
                String codigoBancoCaixa = "104";

                if (String.Compare(bancoSelecionado, codigoBancoCaixa) == 0)
                {
                    while (txtContaCorrente.Text.Length < 10)
                    {
                        if (txtContaCorrente.Text.Length == 9)
                        {
                            if (Credenciamento.Proposta.CodigoTipoPessoa.Equals('J'))
                                txtContaCorrente.Text = String.Format(@"3{0}", txtContaCorrente.Text);
                            else if (Credenciamento.Proposta.CodigoTipoPessoa.Equals('F'))
                                txtContaCorrente.Text = String.Format(@"1{0}", txtContaCorrente.Text);
                        }
                        else
                            txtContaCorrente.Text = String.Format(@"0{0}", txtContaCorrente.Text);
                    }
                }

                upDadosBancarios.Update();
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
        /// Recupera o indice da conta corrente na viewstate
        /// </summary>
        /// <param name="tipoDomicilioBancario">tipoDomicilioBancario</param>
        /// <returns>Retorna o indice da conta corrente na viewState</returns>
        private int IndexContaCorrente(Modelo.TipoDomicilioBancario tipoDomicilioBancario)
        {
            int index = -1;

            for (int i = 0; i < ContasCorrentes.Count; i++)
            {
                if (ContasCorrentes[i].TipoDomicilioBancario == tipoDomicilioBancario)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Limpar dados do domicílio bancário de crédito
        /// </summary>
        private void LimparDomicilioBancarioCredito()
        {
            ddlBancoReceptorVendasCredito.Enabled = true;
            if (ddlBancoReceptorVendasCredito.Items.Count > 0)
                ddlBancoReceptorVendasCredito.SelectedIndex = 0;
            txtAgenciaRecepotoraVendasCredito.Enabled = true;
            txtAgenciaRecepotoraVendasCredito.Text = String.Empty;
            txtContaCorrenteReceptoraVendasCredito.Enabled = true;
            txtContaCorrenteReceptoraVendasCredito.Text = String.Empty;
            lblAgenciaReceptoraVendasCredito.Text = String.Empty;
        }

        /// <summary>
        /// Controle de tela executado pelo campo DropDown Local Pagamento
        /// </summary>
        protected void ControleTelaLocalPagamento()
        {
            bool enabled = String.Compare(ddlLocalPagamento.SelectedValue, ((int)Modelo.LocalPagamento.CENTRALIZADO).ToString()) == 0;

            txtPVCentralizador.Visible = enabled;
            lblTextoPVCentralizador.Visible = enabled;
            txtPVCentralizador.Text = String.Empty;
            rfvPVCentralizador.Enabled = enabled;
            rfvPVCentralizador.Visible = enabled;

            if (!ddlLocalPagamento.SelectedValue.Equals("2"))
                LimparDomicilioBancarioCredito();
            upDadosBancarios.Update();
        }

        /// <summary>
        /// Carrega componentes iniciais
        /// </summary>
        public void CarregarCamposIniciais()
        {
            CarregarControles();
            CarregarPropostaAndamento();
        }

        /// <summary>
        /// Carrega os Controles da pagina, que precisam de informações previamente pesquisadas.
        /// </summary>
        private void CarregarControles()
        {
            ((ResumoProposta)resumoProposta).CarregaResumoProposta();
            CarregarLocalPagamento();

            VisibilidadeBlocoCredito();

            VisibilidadeBlocoConstrucard();

            VisibilidadeBlocoAleloRefeicao();

            VisibilidadeBlocoDebito();


            CarregarBancos();

        }

        /// <summary>
        /// Preenche dados na tela caso proposta em andamento
        /// </summary>
        private void CarregarPropostaAndamento()
        {
            if (Proposta.CodigoLocalPagamento != null && Proposta.CodigoLocalPagamento > 0)
                ddlLocalPagamento.SelectedValue = Proposta.CodigoLocalPagamento.ToString();

            if (Proposta.NumeroCentralizadora != null && Proposta.NumeroCentralizadora > 0)
                txtPVCentralizador.Text = Proposta.NumeroCentralizadora.ToString();

            foreach (Modelo.DomicilioBancario objDomicilio in Credenciamento.DomiciliosBancarios)
            {

                if (objDomicilio.TipoDomicilioBancario == Modelo.TipoDomicilioBancario.Credito)
                {
                    ddlBancoReceptorVendasCredito.SelectedValue = objDomicilio.CodigoBanco.ToString();
                    txtAgenciaRecepotoraVendasCredito.Text = objDomicilio.CodigoAgencia.ToString();
                    txtContaCorrenteReceptoraVendasCredito.Text = objDomicilio.NumeroContaCorrente.ToString();
                }

                if (objDomicilio.TipoDomicilioBancario == Modelo.TipoDomicilioBancario.Debito)
                {
                    ddlBancoReceptorVendasDebito.SelectedValue = objDomicilio.CodigoBanco.ToString();
                    txtAgenciaReceptoraVendasDebito.Text = objDomicilio.CodigoAgencia.ToString();
                    txtContaCorrenteReceptoraVendasDebito.Text = objDomicilio.NumeroContaCorrente.ToString();
                }

                if (objDomicilio.TipoDomicilioBancario == Modelo.TipoDomicilioBancario.Construcard)
                {
                    ddlBancoReceptorVendasConstrucard.SelectedValue = objDomicilio.CodigoBanco.ToString();
                    txtAgenciaReceptoraVendasConstrucard.Text = objDomicilio.CodigoAgencia.ToString();
                    txtContaCorrenteReceptoraVendasConstrucard.Text = objDomicilio.NumeroContaCorrente.ToString();
                }
            }
            // Limpa agencias e contas corrente Alelo
            txtAgenciaReceptoraVendasAleloAlimentacao.Text = String.Empty;
            txtAgenciaReceptoraVendasAleloRefeicao.Text = String.Empty;
            txtContaCorrenteReceptoraVendasAleloAlimentacao.Text = String.Empty;
            txtContaCorrenteReceptoraVendasAleloRefeicao.Text = String.Empty;
            lblAgenciaReceptoraVendasAleloAlimentacao.Text = String.Empty;
            lblAgenciaReceptoraVendasAleloRefeicao.Text = String.Empty;
        }


        /// <summary>
        /// Salva as informaçoes preenchidas no formulario na viewstate de credenciamento
        /// </summary>
        private void CapturarInformacoesPreenchidas()
        {
            DropDownList ddlBancoDebito = cbConsiderarDomicilioBancario.Checked ? ddlBancoReceptorVendasCredito : ddlBancoReceptorVendasDebito;
            TextBox txtAgenciaDebito = cbConsiderarDomicilioBancario.Checked ? txtAgenciaRecepotoraVendasCredito : txtAgenciaReceptoraVendasDebito;
            TextBox txtContaCorrenteDebito = cbConsiderarDomicilioBancario.Checked ? txtContaCorrenteReceptoraVendasCredito : txtContaCorrenteReceptoraVendasDebito;

            DropDownList ddlBancoAleloAlimentacao = cbConsiderarDomicilioBancarioAlelo.Checked ? ddlBancoReceptorVendasAleloRefeicao : ddlBancoReceptorVendasAleloAlimentacao;
            TextBox txtAgenciaAleloAlimentacao = cbConsiderarDomicilioBancarioAlelo.Checked ? txtAgenciaReceptoraVendasAleloRefeicao : txtAgenciaReceptoraVendasAleloAlimentacao;
            TextBox txtContaCorrenteAleloAlimentacao = cbConsiderarDomicilioBancarioAlelo.Checked ? txtContaCorrenteReceptoraVendasAleloRefeicao : txtContaCorrenteReceptoraVendasAleloAlimentacao;

            Credenciamento.DomiciliosBancarios.Clear();

            if (phDomicilioCredito.Visible)
                AdicionarNovoDomicilio(ddlBancoReceptorVendasCredito, txtAgenciaRecepotoraVendasCredito, txtContaCorrenteReceptoraVendasCredito, Modelo.TipoDomicilioBancario.Credito, (int)Modelo.TipoOperacao.Credito);

            if (phDomicilioDebito.Visible || cbConsiderarDomicilioBancario.Checked)
                AdicionarNovoDomicilio(ddlBancoDebito, txtAgenciaDebito, txtContaCorrenteDebito, Modelo.TipoDomicilioBancario.Debito, (int)Modelo.TipoOperacao.Debito);

            if (phDomicilioConstrucard.Visible)
                AdicionarNovoDomicilio(ddlBancoReceptorVendasConstrucard, txtAgenciaReceptoraVendasConstrucard, txtContaCorrenteReceptoraVendasConstrucard, Modelo.TipoDomicilioBancario.Construcard, (int)Modelo.TipoOperacao.Construcard);

            if (phDomicilioAleloRefeicao.Visible)
            {
                AdicionarNovoDomicilio(ddlBancoReceptorVendasAleloRefeicao, txtAgenciaReceptoraVendasAleloRefeicao, txtContaCorrenteReceptoraVendasAleloRefeicao, Modelo.TipoDomicilioBancario.Voucher, (int)Modelo.TipoOperacao.Voucher, true);
                if (ProdutoParceiro.Any(p => p.Produto != null && p.Produto.CodigoFeature == 128))
                {
                    Modelo.ProdutoParceiro produtoRefeicao = ProdutoParceiro.FirstOrDefault(p => p.Produto.CodigoFeature == 128);
                    produtoRefeicao.Banco = ddlBancoReceptorVendasAleloRefeicao.SelectedValue.ToInt32();
                    produtoRefeicao.Agencia = txtAgenciaReceptoraVendasAleloRefeicao.Text.ToInt32();
                    produtoRefeicao.Conta = txtContaCorrenteReceptoraVendasAleloRefeicao.Text;
                }
            }

            if (phDomicilioAleloAlimentacao.Visible || cbConsiderarDomicilioBancarioAlelo.Checked)
            {
                if (cbConsiderarDomicilioBancarioAlelo.Checked && !Servicos.ServicosGE.ConsultaBancosConveniadosVoucher('L', 15, 129, null, SessaoAtual.LoginUsuario).Exists(b => b.CodigoBanco == ddlBancoAleloAlimentacao.SelectedValue.ToInt32()))
                    throw new PortalRedecardException(90401, FONTE); // Domicílio Bancário de Alelo Alimentação não pode ser igual ao de Refeição, favor realizar o preenchimento manual.

                AdicionarNovoDomicilio(ddlBancoAleloAlimentacao, txtAgenciaAleloAlimentacao, txtContaCorrenteAleloAlimentacao, Modelo.TipoDomicilioBancario.Voucher, (int)Modelo.TipoOperacao.Voucher);
                if (ProdutoParceiro.Any(p => p.Produto != null && p.Produto.CodigoFeature == 129))
                {
                    Modelo.ProdutoParceiro produtoAlimentacao = ProdutoParceiro.FirstOrDefault(p => p.Produto.CodigoFeature == 129);
                    produtoAlimentacao.Banco = ddlBancoAleloAlimentacao.SelectedValue.ToInt32();
                    produtoAlimentacao.Agencia = txtAgenciaAleloAlimentacao.Text.ToInt32();
                    produtoAlimentacao.Conta = txtContaCorrenteAleloAlimentacao.Text;
                }
            }

            Proposta.CodigoLocalPagamento = ddlLocalPagamento.SelectedValue.ToInt32();
            Proposta.NumeroCentralizadora = txtPVCentralizador.Text.ToInt32();

            if (Credenciamento.Proposta.CodigoFaseFiliacao == null || Credenciamento.Proposta.CodigoFaseFiliacao < 3)
                Credenciamento.Proposta.CodigoFaseFiliacao = 3;

            Int32? codigoErro = null;
            string mensagemErro = null;

            Servicos.ServicosWF.SalvarDadosBancarios(Credenciamento.Proposta, Credenciamento.DomiciliosBancarios.ToList(), Credenciamento.ProdutoParceiro, ref codigoErro, ref mensagemErro);
        }


        /// <summary>
        /// Adiciona novo domicilio bancario na session de credenciamento
        /// </summary>
        /// <param name="ddlBanco">DropDownList Banco</param>
        /// <param name="txtAgencia">TextBox Agência</param>
        /// <param name="txtContaCorrente">TextBox Conta Corrente</param>
        /// <param name="tipoDomicilioBancario">Tipo de Domicílio Bancário</param>
        /// <param name="indicadorTipoOperacao">Indicador Tipo de Operação</param>
        private void AdicionarNovoDomicilio(DropDownList ddlBanco, TextBox txtAgencia, TextBox txtContaCorrente, Modelo.TipoDomicilioBancario tipoDomicilioBancario, Int32 indicadorTipoOperacao, bool refeicao = false)
        {
            Modelo.DomicilioBancario domicilio = new Modelo.DomicilioBancario
            {
                NumeroCNPJ = NumeroCpfCnpj,
                NumeroSeqProp = Credenciamento.Proposta.IndicadorSequenciaProposta,
                CodigoBanco = ddlBanco.SelectedValue.ToInt32(),
                NomeBanco = ddlBanco.SelectedItem.Text,
                CodigoAgencia = txtAgencia.Text.ToInt32(),
                NumeroContaCorrente = txtContaCorrente.Text,
                IndConfirmacaoDomicilio = ' ',
                IndicadorDomicilioDuplicado = ' ',
                IndicadorTipoOperacaoProd = indicadorTipoOperacao,
                TipoDomicilioBancario = tipoDomicilioBancario,
                DataHoraUltimaAtualizacao = DateTime.Now,
                UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario,
                CodigoTipoPessoa = Credenciamento.Proposta.CodigoTipoPessoa,
                IndicadorTipoAcaoBanco = Modelo.TipoAcaoBanco.Inclusao

            };

            if (tipoDomicilioBancario == Modelo.TipoDomicilioBancario.Voucher)
                domicilio.CodigoTipoPessoa = refeicao ? 'R' : 'A';

            DomiciliosBancarios.Add(domicilio);
        }
        #region Visibilidade Blocos Domicilio Bancário

        /// <summary>
        /// Esconde/Exibe o bloco de Credito, de acordo com a utilização 
        /// </summary>
        private void VisibilidadeBlocoCredito()
        {
            if (Credenciamento.Produtos.Where(obj => obj.IndicadorTipoProduto == Modelo.TipoProduto.Credito).Count() == 0)
            {
                phDomicilioCredito.Visible = false;
                cbConsiderarDomicilioBancario.Checked = false;
                phDomicilioDebito.Visible = true;
            }
            else
            {
                phDomicilioCredito.Visible = true;
                phDomicilioDebito.Visible = !cbConsiderarDomicilioBancario.Checked;
            }
        }

        /// <summary>
        /// Esconde/Exibe o bloco de Débito, de acordo com a utilização 
        /// </summary>
        private void VisibilidadeBlocoDebito()
        {
            if (Credenciamento.Produtos.Where(obj => obj.IndicadorTipoProduto == Modelo.TipoProduto.Debito).Count() == 0)
            {
                ExibirDomicilioDebito(false);
                cbConsiderarDomicilioBancario.Checked = false;
                cbConsiderarDomicilioBancario.Enabled = false;
            }
            else
            {
                cbConsiderarDomicilioBancario.Enabled = true;
            }
        }

        /// <summary>
        /// Esconde/Exibe o bloco de construcard, de acordo com a viewstate de credenciamento
        /// </summary>
        private void VisibilidadeBlocoConstrucard()
        {
            phDomicilioConstrucard.Visible = Produtos.Where(p => p.CodigoCCA == 22 && p.CodigoFeature == 21).Count() > 0;

            upDadosBancarios.Update();
        }



        /// <summary>
        /// Esconde/Exibe o bloco de Alelo Refeição, de acordo com a viewstate de credenciamento
        /// </summary>
        private void VisibilidadeBlocoAleloRefeicao()
        {
            phDomicilioAleloRefeicao.Visible = Credenciamento.ProdutoParceiro.Any(p => p.Produto.CodigoFeature == 128);
            phDomicilioAleloAlimentacao.Visible = Credenciamento.ProdutoParceiro.Any(p => p.Produto.CodigoFeature == 129);

            if (phDomicilioAleloRefeicao.Visible && phDomicilioAleloAlimentacao.Visible)
            {
                cbConsiderarDomicilioBancarioAlelo.Enabled = cbConsiderarDomicilioBancarioAlelo.Checked = true;

                phDomicilioAleloAlimentacao.Visible = !cbConsiderarDomicilioBancarioAlelo.Checked;

            }

            else
                cbConsiderarDomicilioBancarioAlelo.Enabled = cbConsiderarDomicilioBancarioAlelo.Checked = false;

        }

        #endregion


        /// <summary>
        /// Carrega os DropDownList de Local de Pagamento.
        /// </summary>
        private void CarregarLocalPagamento()
        {
            stringBuilder = new StringBuilder();
            String localPagamentoEstabelecimento = stringBuilder.Append(((int)Modelo.LocalPagamento.ESTABELECIMENTO).ToString()).Append(" - ").Append(Modelo.LocalPagamento.ESTABELECIMENTO.ToString()).ToString();

            stringBuilder = new StringBuilder();
            String localPagamentoCentralizado = stringBuilder.Append(((int)Modelo.LocalPagamento.CENTRALIZADO).ToString()).Append(" - ").Append(Modelo.LocalPagamento.CENTRALIZADO.ToString()).ToString();

            ddlLocalPagamento.Items.Clear();
            ddlLocalPagamento.Items.Add(new ListItem(localPagamentoEstabelecimento, ((int)Modelo.LocalPagamento.ESTABELECIMENTO).ToString()));
            ddlLocalPagamento.Items.Add(new ListItem(localPagamentoCentralizado, ((int)Modelo.LocalPagamento.CENTRALIZADO).ToString()));

            ddlLocalPagamento.SelectedValue = ((int)Modelo.LocalPagamento.ESTABELECIMENTO).ToString();
            ControleTelaLocalPagamento();
            //ddlLocalPagamento.Items.Insert(0, new ListItem("", ""));

            if (Credenciamento.Proposta.CodigoTipoEstabelecimento != 1)
            {
                ddlLocalPagamento.Enabled = false;
                txtPVCentralizador.Visible = false;
                lblTextoPVCentralizador.Visible = false;
            }
            else
            {
                ddlLocalPagamento.Enabled = true;
                txtPVCentralizador.Visible = false;
                lblTextoPVCentralizador.Visible = false;
            }
        }

        /// <summary>
        /// Carrega os DropDownList de Bancos.
        /// </summary>
        private void CarregarBancos()
        {
            stringBuilder = new StringBuilder();

            ddlBancoReceptorVendasCredito.Items.Clear();
            Servicos.ServicosGE.ConsultaBancos('C').ForEach(c =>
            {
                stringBuilder = new StringBuilder();
                stringBuilder.Append(c.CodBancoCompensacao).Append(" - ").Append(c.NomeBanco);

                ddlBancoReceptorVendasCredito.Items.Add(new ListItem(stringBuilder.ToString(), c.CodBancoCompensacao.ToString()));
            });

            ddlBancoReceptorVendasDebito.Items.Clear();
            Servicos.ServicosGE.ConsultaBancos('D').ForEach(c =>
            {
                stringBuilder = new StringBuilder();
                stringBuilder.Append(c.CodBancoCompensacao).Append(" - ").Append(c.NomeBanco);

                ddlBancoReceptorVendasDebito.Items.Add(new ListItem(stringBuilder.ToString(), c.CodBancoCompensacao.ToString()));
            });

            ddlBancoReceptorVendasAleloAlimentacao.Items.Clear();
            Servicos.ServicosGE.ConsultaBancosConveniadosVoucher('L', 15, 129, null, SessaoAtual.LoginUsuario).
                Where(p => p.SituacaoRegistro == GEBancos.SituacaoRegistro.Ativo).ToList().ForEach(c =>
            {
                ddlBancoReceptorVendasAleloAlimentacao.Items.Add(new ListItem(c.Banco.NomeBanco, c.CodigoBanco.ToString()));
            });

            ddlBancoReceptorVendasAleloRefeicao.Items.Clear();
            Servicos.ServicosGE.ConsultaBancosConveniadosVoucher('L', 15, 128, null, SessaoAtual.LoginUsuario).
                Where(p => p.SituacaoRegistro == GEBancos.SituacaoRegistro.Ativo).ToList().ForEach(c =>
            {
                ddlBancoReceptorVendasAleloRefeicao.Items.Add(new ListItem(c.Banco.NomeBanco, c.CodigoBanco.ToString()));
            });
            
            ddlBancoReceptorVendasCredito.Items.Insert(0, new ListItem("", ""));
            ddlBancoReceptorVendasAleloAlimentacao.Items.Insert(0, new ListItem("", ""));
            ddlBancoReceptorVendasAleloRefeicao.Items.Insert(0, new ListItem("", ""));
            ddlBancoReceptorVendasDebito.Items.Insert(0, new ListItem("", ""));

			//Somente CAIXA ECONOMICA FEDERAL
            ddlBancoReceptorVendasConstrucard.Items.Clear();
			ddlBancoReceptorVendasConstrucard.Items.Add(new ListItem("104 - CAIXA ECONOMICA FEDERAL", "104"));
            ddlBancoReceptorVendasConstrucard.Items.Insert(0, new ListItem("", ""));
            ddlBancoReceptorVendasConstrucard.SelectedValue = "104";
            ddlBancoReceptorVendasConstrucard.Enabled = false;
        }

        /// <summary>
        /// Método genérico que busca o detelhe da agencia, de acordo com os valores dos 
        /// controles passados como parametro 
        /// </summary>
        /// <param name="txtAgencia">TextBox Agência</param>
        /// <param name="lblAgencia">Label Agência</param>
        /// <param name="ddlBanco">DropDownList Banco</param>
        private void PesquisarDetalheAgencia(TextBox txtAgencia, Label lblAgencia, DropDownList ddlBanco)
        {
            if (ddlBanco.SelectedIndex <= 0)
                return;

            stringBuilder = new StringBuilder();

            String mensagemErro = String.Empty,
                   iCodigoErro = String.Empty;

            Int32 codigoBanco = ddlBanco.SelectedValue.ToInt32();
            Int32 codigoAgencia = txtAgencia.Text.ToInt32();

            GEAgencias.ConsultaDetalheAgencia agencia = Servicos.ServicosGE.ConsultaDetalheAgencia(codigoBanco, codigoAgencia, ref iCodigoErro, ref mensagemErro);

            if (agencia != null)
            {
                if (String.Compare(iCodigoErro, "0") != 0 && !String.IsNullOrEmpty(iCodigoErro))
                    this.RetornarMensagemErro(mensagemErro, iCodigoErro.ToInt32());
                else
                    lblAgencia.Text = agencia.NomeAgencia.Trim();

                upDadosBancarios.Update();
            }
        }


        #endregion

        #region Eventos - Página

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        #endregion

        #region Eventos - Botões

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
                    CapturarInformacoesPreenchidas();
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
            finally
            {
                Credenciamento = null;
                NovaProposta(sender, e);
            }
        }

        /// <summary>
        /// Chama o serviço que verifica se a conta corrente é valida
        /// </summary>
        /// <param name="ddlBanco">DropDownList Banco</param>
        /// <param name="txtAgencia">TextBox Agência</param>
        /// <param name="txtContaCorrente">TextBox Conta Corrente</param>
        /// <param name="codigoTipoPessoa">Tipo Pessoa do Credenciamento</param>
        /// <param name="tipoDomicilioBancario">Tipo de Domicílio Bancario</param>
        /// <returns>retorna boolean de verificação de Dígito na Conta Corrente</returns>
        private Boolean VerficarDigitoContaCorrente(DropDownList ddlBanco, TextBox txtAgencia, TextBox txtContaCorrente, Char codigoTipoPessoa, Modelo.TipoDomicilioBancario tipoDomicilioBancario)
        {
            Int32 codigoBanco = ddlBanco.SelectedValue.ToInt32();
            Int64 codigoAgencia = txtAgencia.Text.ToInt64();
            Int64 codigoContaCorrente = txtContaCorrente.Text.ToInt64();
            String mensagemErro = string.Empty;

            return Servicos.ServicosGE.ChecaDigito(codigoBanco, codigoAgencia, codigoContaCorrente, codigoTipoPessoa, ref mensagemErro);
        }

        public event EventHandler NovaProposta;

        /// <summary>
        /// Evento disparado ao clicar no continuar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Bancários - Continuar"))
            {
                try
                {
                    Page.Validate();
                    if (Page.IsValid)
                    {
                        CapturarInformacoesPreenchidas();
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

        public event EventHandler Continuar;

        /// <summary>
        /// Evento disparado ao clicar no botão Voltar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Bancários - Voltar"))
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

        public event EventHandler Voltar;

        #endregion

        #region Eventos - Validação

        #region Débito

        /// <summary>
        /// Validação Server Side do número de Conta Corrente Débito
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvContaCorrenteDebito_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = VerficarDigitoContaCorrente(ddlBancoReceptorVendasDebito,
                                                           txtAgenciaReceptoraVendasDebito,
                                                           txtContaCorrenteReceptoraVendasDebito,
                                                           Credenciamento.Proposta.CodigoTipoPessoa,
                                                           Modelo.TipoDomicilioBancario.Debito);
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
        /// Validação do número da Agência Débito
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvAgenciaDebito_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = ValidaAgencia(txtAgenciaReceptoraVendasDebito, ddlBancoReceptorVendasDebito);
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
        #endregion

        #region Crédito

        /// <summary>
        /// Validação Server Side do número de Conta Corrente Crédito
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvContaCorrenteCredito_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = VerficarDigitoContaCorrente(ddlBancoReceptorVendasCredito,
                                                           txtAgenciaRecepotoraVendasCredito,
                                                           txtContaCorrenteReceptoraVendasCredito,
                                                           Credenciamento.Proposta.CodigoTipoPessoa,
                                                           Modelo.TipoDomicilioBancario.Credito);
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
        /// Validação do número da Agência Crédito
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvAgenciaCredito_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = ValidaAgencia(txtAgenciaRecepotoraVendasCredito, ddlBancoReceptorVendasCredito);
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

        #endregion

        #region Construcard

        /// <summary>
        /// Validação Server Side do número de Conta Corrente Construcard
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvContaCorrenteConstrucard_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = VerficarDigitoContaCorrente(ddlBancoReceptorVendasConstrucard,
                                                           txtAgenciaReceptoraVendasConstrucard,
                                                           txtContaCorrenteReceptoraVendasConstrucard,
                                                           Credenciamento.Proposta.CodigoTipoPessoa,
                                                           Modelo.TipoDomicilioBancario.Construcard);
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
        /// Validação do número da Agência Construcard
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvAgenciaConstrucard_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = ValidaAgencia(txtAgenciaReceptoraVendasConstrucard, ddlBancoReceptorVendasConstrucard);
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

        #endregion

        #region Alelo - Alimentação

        /// <summary>
        /// Validação Server Side do número de Agencia Alelo Alimentacao
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvAgenciaReceptoraVendasAleloAlimentacao_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = ValidaAgencia(txtAgenciaReceptoraVendasAleloAlimentacao, ddlBancoReceptorVendasAleloAlimentacao);

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
        /// Validação Server Side do número de Conta Corrente Alelo Alimentação
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvContaCorrenteAleloAlimentacao_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = VerficarDigitoContaCorrente(ddlBancoReceptorVendasAleloAlimentacao,
                                                           txtAgenciaReceptoraVendasAleloAlimentacao,
                                                           txtContaCorrenteReceptoraVendasAleloAlimentacao,
                                                           Credenciamento.Proposta.CodigoTipoPessoa,
                                                           Modelo.TipoDomicilioBancario.Voucher);
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

        #endregion

        #region Alelo - Refeição

        /// <summary>
        /// Validação Server Side do número de Conta Corrente Alelo Refeicao
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvContaCorrenteAleloRefeicao_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = VerficarDigitoContaCorrente(ddlBancoReceptorVendasAleloRefeicao,
                                                           txtAgenciaReceptoraVendasAleloRefeicao,
                                                           txtContaCorrenteReceptoraVendasAleloRefeicao,
                                                           Credenciamento.Proposta.CodigoTipoPessoa,
                                                           Modelo.TipoDomicilioBancario.Voucher);
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
        /// Validação do número da Agência Alelo Refeição
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvAgenciaAleloRefeicao_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = ValidaAgencia(txtAgenciaReceptoraVendasAleloRefeicao, ddlBancoReceptorVendasAleloRefeicao);
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

        #endregion

        #endregion

        #region Eventos - Controles

        /// <summary>
        /// Evento disparado ao mudar a seleção do campo Local de Pagamento.
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ddlLocalPagamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            ControleTelaLocalPagamento();
        }

        /// <summary>
        /// Busca informações do domicilio bancário de crédito para o pv centralizador
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtPVCentralizador_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtPVCentralizador.Text))
            {
                LimparDomicilioBancarioCredito();
            }
            else
            {
                Int32 numeroPvCentralizador = txtPVCentralizador.Text.ToInt32();
                var domicilioBancario = Servicos.ServicosGE.CarregarDadosDomicilioBancarioPorPontoVenda(numeroPvCentralizador, 'C', "CR").FirstOrDefault(d => d.CodigoErro == 0);

                if (domicilioBancario != null)
                {
                    ddlBancoReceptorVendasCredito.Enabled = false;
                    ddlBancoReceptorVendasCredito.SelectedValue = domicilioBancario.CodBancoAtual.Value.ToString();
                    txtAgenciaRecepotoraVendasCredito.Enabled = false;
                    txtAgenciaRecepotoraVendasCredito.Text = domicilioBancario.CodAgenciaAtual.Value.ToString();
                    txtContaCorrenteReceptoraVendasCredito.Enabled = false;
                    txtContaCorrenteReceptoraVendasCredito.Text = domicilioBancario.NumeroContaAtual;
                    lblAgenciaReceptoraVendasCredito.Text = domicilioBancario.NomeAgenciaAtual;
                }
                else
                {
                    LimparDomicilioBancarioCredito();
                }
            }
        }

        #region Alelo - Refeição

        /// <summary>
        /// Evento ao marcar, ou desmarcar o checkbox cbConsiderarDomicilioBancarioAlelo
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbConsiderarDomicilioBancarioAlelo_CheckedChanged(object sender, EventArgs e)
        {
            ExibirDomicilioAleloAlimentacao(!cbConsiderarDomicilioBancarioAlelo.Checked);

            upDadosBancarios.Update();
        }


        /// <summary>
        /// Evento disparado ao mudar o numero da agencia, do domicilio bancario Alelo Refeição
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtAgenciaReceptoraVendasAleloRefeicao_TextChanged(object sender, EventArgs e)
        {
            PesquisarDetalheAgencia(txtAgenciaReceptoraVendasAleloRefeicao, lblAgenciaReceptoraVendasAleloRefeicao, ddlBancoReceptorVendasAleloRefeicao);
        }

        /// <summary>
        /// Evento disparado ao mudar o numero da conta corrente, do domicilio bancario Alelo - Refeição
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtContaCorrenteReceptoraVendasAleloRefeicao_TextChanged(object sender, EventArgs e)
        {
            AutoCompletarContaCorrenteCaixa(txtContaCorrenteReceptoraVendasAleloRefeicao, ddlBancoReceptorVendasAleloRefeicao);
        }

        #endregion

        #region Alelo - Alimentacao

        /// <summary>
        /// Evento disparado ao mudar o numero da agencia, do domicilio bancario Alelo Alimentação
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtAgenciaReceptoraVendasAleloAlimentacao_TextChanged(object sender, EventArgs e)
        {
            PesquisarDetalheAgencia(txtAgenciaReceptoraVendasAleloAlimentacao, lblAgenciaReceptoraVendasAleloAlimentacao, ddlBancoReceptorVendasAleloAlimentacao);
        }

        /// <summary>
        /// Evento disparado ao mudar o numero da conta corrente, do domicilio bancario Alelo - Alimentacao
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtContaCorrenteReceptoraVendasAleloAlimentacao_TextChanged(object sender, EventArgs e)
        {
            AutoCompletarContaCorrenteCaixa(txtContaCorrenteReceptoraVendasAleloAlimentacao, ddlBancoReceptorVendasAleloAlimentacao);
        }

        #endregion

        #region Débito

        /// <summary>
        /// Evento ao marcar, ou desmarcar o checkbox cbConsiderarDomicilioBancario
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbConsiderarDomicilioBancario_CheckedChanged(object sender, EventArgs e)
        {
            ExibirDomicilioDebito(!cbConsiderarDomicilioBancario.Checked);

            upDadosBancarios.Update();
        }


        /// <summary>
        /// Evento Evento disparado ao mudar o numero da agencia, do domicilio bancario Debito
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtAgenciaReceptoraVendasDebito_TextChanged(object sender, EventArgs e)
        {
            PesquisarDetalheAgencia(txtAgenciaReceptoraVendasDebito, lblAgenciaReceptoraVendasDebito, ddlBancoReceptorVendasDebito);
        }

        /// <summary>
        /// Evento disparado ao mudar o numero da conta corrente, do domicilio bancario Débito
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtContaCorrenteReceptoraVendasDebito_TextChanged(object sender, EventArgs e)
        {
            AutoCompletarContaCorrenteCaixa(txtContaCorrenteReceptoraVendasDebito, ddlBancoReceptorVendasDebito);
        }

        #endregion

        #region Crédito

        /// <summary>
        /// Evento Evento disparado ao mudar o numero da agencia, do domicilio bancario Credito
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtAgenciaReceptoraVendasCredito_TextChanged(object sender, EventArgs e)
        {
            PesquisarDetalheAgencia(txtAgenciaRecepotoraVendasCredito, lblAgenciaReceptoraVendasCredito, ddlBancoReceptorVendasCredito);
        }

        /// <summary>
        /// Evento disparado ao mudar o numero da conta corrente, do domicilio bancario Crédito
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtContaCorrenteReceptoraVendasCredito_TextChanged(object sender, EventArgs e)
        {
            AutoCompletarContaCorrenteCaixa(txtContaCorrenteReceptoraVendasCredito, ddlBancoReceptorVendasCredito);
        }

        #endregion

        #region Construcard

        /// <summary>
        /// Evento disparado ao mudar o numero da agencia, do domicilio bancario Construcard
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtAgenciaReceptoraVendasConstrucard_TextChanged(object sender, EventArgs e)
        {
            PesquisarDetalheAgencia(txtAgenciaReceptoraVendasConstrucard, lblAgenciaReceptoraVendasConstrucard, ddlBancoReceptorVendasConstrucard);
        }

        /// <summary>
        /// Evento disparado ao mudar o numero da conta corrente, do domicilio bancario Construcard
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtContaCorrenteReceptoraVendasConstrucard_TextChanged(object sender, EventArgs e)
        {
            AutoCompletarContaCorrenteCaixa(txtContaCorrenteReceptoraVendasConstrucard, ddlBancoReceptorVendasConstrucard);
        }

        #endregion

        #endregion

    }
}
