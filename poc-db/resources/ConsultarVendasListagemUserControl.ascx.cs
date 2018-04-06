using Redecard.PN.Comum;
using Resumo = Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ResumoVendas;
using Transacao = Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ConsultaTransacao;
using System;
using System.ComponentModel;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using System.Text.RegularExpressions;

namespace Redecard.PN.Extrato.SharePoint.WebParts.ConsultarVendas.ConsultarVendasListagem
{
    /// <summary>
    /// Controle para listagem do conteúdo das vendas
    /// </summary>
    public partial class ConsultarVendasListagemUserControl : BaseUserControl
    {
        #region Controles disponíveis na página

        /// <summary>
        /// ResumoCreditoControl
        /// </summary>
        protected Resumo.Credito ResumoCreditoControl
        {
            get
            {
                return (Resumo.Credito)this.resumoCreditoControl;
            }
        }

        /// <summary>
        /// ResumoDebitoControl
        /// </summary>
        protected Resumo.Debito ResumoDebitoControl
        {
            get
            {
                return (Resumo.Debito)this.resumoDebitoControl;
            }
        }

        /// <summary>
        /// ResumoConstrucardControl
        /// </summary>
        protected Resumo.Construcard ResumoConstrucardControl
        {
            get
            {
                return (Resumo.Construcard)this.resumoConstrucardControl;
            }
        }

        /// <summary>
        /// ResumoRecargaCelularControl
        /// </summary>
        protected Resumo.RecargaCelular ResumoRecargaCelularControl
        {
            get
            {
                return (Resumo.RecargaCelular)this.resumoRecargaCelularControl;
            }
        }

        /// <summary>
        /// TransacaoCreditoControl
        /// </summary>
        protected Transacao.Credito TransacaoCreditoControl
        {
            get
            {
                return (Transacao.Credito)this.transacaoCreditoControl;
            }
        }

        /// <summary>
        /// TransacaoDebitoControl
        /// </summary>
        protected Transacao.Debito TransacaoDebitoControl
        {
            get
            {
                return (Transacao.Debito)this.transacaoDebitoControl;
            }
        }

        #endregion

        /// <summary>
        /// Sobrescrita do método OnLoad do BaseUserControl
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // impede a validação da permissão no controle
            this.ValidarPermissao = false;

            base.OnLoad(e);
        }

        /// <summary>
        /// Evento ao carregar a página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("ConsultarVendasFiltroUserControl - Page_Load()"))
            {
                // mantém scroll da página no post back
                Page.MaintainScrollPositionOnPostBack = true;

                try
                {
                    // valida se a página está em modo edição
                    if (SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                    {
                        this.AlterarViewAtual(ViewConsultaVenda.Default);
                        this.quadroAviso.Mensagem = "Página em modo de edição";
                        this.quadroAviso.TipoQuadro = TipoQuadroAviso.Sucesso;
                        return;
                    }

                    if (this.SessaoAtual != null)
                    {
                        // dados do usuário no header do bloco de impressão
                        this.lblHeaderImpressaoUsuario.Text = string.Concat(SessaoAtual.CodigoEntidade, " / ", SessaoAtual.LoginUsuario);
                    }

                    if (!IsPostBack)
                    {
                        this.AlterarViewAtual(ViewConsultaVenda.Default);

                        String dados = Request.QueryString["dados"];

                        if (!String.IsNullOrEmpty(dados))
                        {
                            QueryStringSegura queryString = new QueryStringSegura(dados);

                            TipoBusca tipoBusca = (TipoBusca)queryString["tipoBusca"].ToInt32Null(0);
                            TipoVenda tipoVenda = (TipoVenda)queryString["tipoVenda"].ToInt32Null(0);
                            String numero = queryString["numero"];
                            Int32 pvSelecionado = queryString["numeroEstabelecimento"].ToInt32Null(0).Value;

                            DateTime? dataInicial = null;
                            if (!string.IsNullOrEmpty(queryString["dataInicial"]))
                                dataInicial = queryString["dataInicial"].ToDateTimeNull();

                            DateTime? dataFinal = null;
                            if (!string.IsNullOrEmpty(queryString["dataFinal"]))
                                dataFinal = queryString["dataFinal"].ToDateTimeNull();

                            // valida o conteúdo passado na URL
                            if (tipoBusca == TipoBusca.Default
                                || tipoVenda == TipoVenda.Default
                                || string.IsNullOrEmpty(numero)
                                || pvSelecionado == 0
                                || !dataInicial.HasValue)
                            {
                                this.quadroAviso.Mensagem = "Não há movimento para o período informado";
                                this.quadroAviso.TipoQuadro = TipoQuadroAviso.Aviso;
                            }
                            else
                            {
                                this.ConsultarVendas(tipoBusca, tipoVenda, numero, dataInicial, dataFinal, pvSelecionado, acessoSemFiltro: true);
                            }
                        }
                        else
                        {
                            this.quadroAviso.Mensagem = "Informe os critérios para busca";
                            this.quadroAviso.TipoQuadro = TipoQuadroAviso.Aviso;
                        }
                    }
                }
                catch (HttpException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);
                    if (Request.QueryString["mostrarErro"] != null)
                    {
                        throw;
                    }
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);
                    if (Request.QueryString["mostrarErro"] != null)
                    {
                        throw;
                    }
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega o conteúdo para listagem na tela
        /// </summary>
        /// <param name="tipoBusca">Identificação do tipo de busca (NSU, TID, nº cartão, nº do resumo de vendas)</param>
        /// <param name="tipoVenda">Identificação do tipo de venda para filtro (crédito, débito, construcard, recarda de celular)</param>
        /// <param name="numero">Valor para filtro correspondente ao tipo de busca</param>
        /// <param name="dataInicial">Data de início do range de filtro</param>
        /// <param name="dataFinal">Data final do range de filtro</param>
        /// <param name="pvSelecionado">Número do PV selecionado</param>
        /// <param name="acessoSemFiltro">Identifica se o acesso foi direto ou vindo da tela com filtro</param>
        public void ConsultarVendas(
            TipoBusca tipoBusca,
            TipoVenda tipoVenda,
            String numero,
            DateTime? dataInicial,
            DateTime? dataFinal,
            Int32 pvSelecionado,
            Boolean acessoSemFiltro = false)
        {
            this.AlterarViewAtual(ViewConsultaVenda.Default);

            // remove caracteres especiais quando nao é TID
            if (tipoBusca != TipoBusca.TID)
                numero = new Regex("[^0-9]").Replace(numero, String.Empty);

            this.DefineHeaderFooter(tipoBusca, tipoVenda, acessoSemFiltro);

            if (tipoBusca == TipoBusca.NSU || tipoBusca == TipoBusca.TID || tipoBusca == TipoBusca.Cartao)
            {
                // define o objeto para a consulta
                TransacaoDadosConsultaDTO transacaoDTO = new TransacaoDadosConsultaDTO()
                {
                    DataInicial = dataInicial.HasValue ? dataInicial.Value : default(DateTime),
                    DataFinal = dataFinal.HasValue ? dataFinal.Value : default(DateTime),
                    NumeroEstabelecimento = pvSelecionado
                };

                switch (tipoBusca)
                {
                    case TipoBusca.Cartao:
                        transacaoDTO.NumeroCartao = numero;
                        break;

                    case TipoBusca.NSU:
                        transacaoDTO.Nsu = numero.ToDecimal();
                        break;

                    case TipoBusca.TID:
                    default:
                        transacaoDTO.TID = numero;
                        break;
                }

                switch (tipoVenda)
                {
                    case TipoVenda.Credito:
                        this.TransacaoCreditoControl.ConsultarTransacao(transacaoDTO);
                        AlterarViewAtual(ViewConsultaVenda.TransacaoCredito);
                        break;

                    case TipoVenda.Debito:
                        this.TransacaoDebitoControl.ConsultarTransacao(transacaoDTO);
                        AlterarViewAtual(ViewConsultaVenda.TransacaoDebito);
                        break;

                    default:
                        base.ExibirPainelMensagem("Tipo de venda não informada");
                        break;
                }
            }
            else if (tipoBusca == TipoBusca.ResumoVendas)
            {
                ResumoVendaDadosConsultaDTO resumoDTO = new ResumoVendaDadosConsultaDTO()
                {
                    DataApresentacao = dataInicial.HasValue ? dataInicial.Value : default(DateTime),
                    NumeroResumoVenda = numero.ToInt32Null(0).Value,
                    NumeroEstabelecimento = pvSelecionado
                };

                switch (tipoVenda)
                {
                    case TipoVenda.Credito:
                        ResumoCreditoControl.ConsultarResumoVenda(resumoDTO);
                        AlterarViewAtual(ViewConsultaVenda.ResumoCredito);
                        break;

                    case TipoVenda.Debito:
                        ResumoDebitoControl.ConsultarResumoVenda(resumoDTO);
                        AlterarViewAtual(ViewConsultaVenda.ResumoDebito);
                        break;

                    case TipoVenda.Construcard:
                        ResumoConstrucardControl.ConsultarResumoVenda(resumoDTO);
                        AlterarViewAtual(ViewConsultaVenda.ResumoConstrucard);
                        break;

                    case TipoVenda.Recarga:
                        ResumoRecargaCelularControl.ConsultarResumoVenda(resumoDTO);
                        AlterarViewAtual(ViewConsultaVenda.ResumoRecargaCelular);
                        break;

                    case TipoVenda.Default:
                    default:
                        base.ExibirPainelMensagem("Tipo de venda não informada");
                        break;
                }
            }
            else
            {
                base.ExibirPainelMensagem("Tipo de busca não informada");
            }
        }

        #region Enumerators

        /// <summary>
        /// Enumerator para identificação do tipo de busca
        /// </summary>
        public enum TipoBusca
        {
            [Description("selecione")]
            Default = 0,

            [Description("NSU")]
            NSU = 1,

            [Description("TID")]
            TID = 2,

            [Description("nº do cartão")]
            Cartao = 3,

            [Description("nº do resumo de vendas")]
            ResumoVendas = 4
        }

        /// <summary>
        /// Enumerator para identificação do tipo de venda
        /// </summary>
        public enum TipoVenda
        {
            [Description("selecione")]
            Default = 0,

            [Description("crédito")]
            Credito = 1,

            [Description("débito")]
            Debito = 2,

            [Description("Construcard")]
            Construcard = 3,

            [Description("recarga")]
            Recarga = 4
        }

        /// <summary>
        /// Enumerador para as Views do Resumo de Vendas
        /// </summary>
        private enum ViewConsultaVenda
        {
            Default = 0,
            ResumoCredito = 1,
            ResumoDebito = 2,
            ResumoConstrucard = 3,
            ResumoRecargaCelular = 4,
            TransacaoCredito = 5,
            TransacaoDebito = 6
        }

        #endregion

        #region Métodos privados

        /// <summary>
        /// Altera a view atual
        /// </summary>
        private void AlterarViewAtual(ViewConsultaVenda view)
        {
            ViewState["ViewAtual"] = view;
            this.multiViewConsultaVenda.ActiveViewIndex = (int)view;
        }

        /// <summary>
        /// Método para definição do título do conteúdo a ser exibido
        /// </summary>
        /// <param name="tipoBusca"></param>
        private void DefineHeaderFooter(TipoBusca tipoBusca, TipoVenda tipoVenda, bool acessoSemFiltro)
        {
            // define o título do conteúdo
            switch (tipoBusca)
            {
                case TipoBusca.NSU:
                    this.lblTitulo.Text = "transação (NSU)";
                    break;
                case TipoBusca.TID:
                    this.lblTitulo.Text = "transação (TID)";
                    break;
                case TipoBusca.Cartao:
                    this.lblTitulo.Text = "transação (nº do cartão)";
                    break;
                case TipoBusca.ResumoVendas:
                    this.lblTitulo.Text = "resumo de vendas";
                    break;
                case TipoBusca.Default:
                default:
                    this.lblTitulo.Text = string.Empty;
                    break;
            }

            // para acesso direto (sem filtro), inclui o tipo da venda ao título do bloco de resultado
            if (acessoSemFiltro)
            {
                pnlTitulo.CssClass = "title";

                switch (tipoVenda)
                {
                    case TipoVenda.Credito:
                        this.lblTitulo.Text = string.Format("{0} - cartões de crédito", this.lblTitulo.Text);
                        break;
                    case TipoVenda.Debito:
                        this.lblTitulo.Text = string.Format("{0} - cartões de débito", this.lblTitulo.Text);
                        break;
                    case TipoVenda.Construcard:
                        this.lblTitulo.Text = string.Format("{0} - Construcard", this.lblTitulo.Text);
                        break;
                    case TipoVenda.Recarga:
                        this.lblTitulo.Text = string.Format("{0} - recargas de celular", this.lblTitulo.Text);
                        break;
                    case TipoVenda.Default:
                    default:
                        break;
                }
            }

            // adiciona ao título de impressão
            lblTituloImpressao.Text = lblTitulo.Text;

            // define a exibição do botão
            this.pnlVoltar.Visible = acessoSemFiltro;
        }

        /// <summary>
        /// Método de voltar para relatórios de resumo de vendas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            base.RetornarPaginaAnterior();
        }

        #endregion
    }
}
