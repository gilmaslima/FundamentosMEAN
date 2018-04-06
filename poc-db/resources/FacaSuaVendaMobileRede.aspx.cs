/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.DataCash.BasePage;
using Redecard.PN.DataCash.Modelo;
using Redecard.PN.DataCash.Modelo.Util;

namespace Redecard.PN.DataCash
{
    /// <summary>
    /// Esta página estará dentro de um iframe, da tela Mobile 2.0 "NovoLeitorCartao".
    /// Esta página recebe por QueryString um Guid, que é a chave do objeto no AppFabric que contém
    /// os dados dos valores dos leitores e endereço de entrega (Cache.DataCashIntegracao).
    /// No retorno, o iframe adiciona o objeto do resultado da transação no AppFabric, e envia para a página
    /// pai o GUID deste objeto.
    /// </summary>
    public partial class FacaSuaVendaMobileRede : PageBaseDataCash
    {
        #region [ Propriedades/Variáveis ]

        /// <summary>
        /// Chave da QueryString que contém o GUID do objeto de integração com o Portal
        /// para a página NovoLeitorCartao.aspx do projeto Redecard.PN.Boston
        /// </summary>
        private static String ChaveIntegracao { get { return "NovoLeitorCartao"; } }

        /// <summary>
        /// PT-BR
        /// </summary>
        private static CultureInfo PtBr { get { return new CultureInfo("pt-BR"); } }

        /// <summary>JS Serializer</summary>
        private static JavaScriptSerializer jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get { return jsSerializer ?? (jsSerializer = new JavaScriptSerializer()); }
        }

        /// <summary>
        /// Query String
        /// </summary>
        private QueryStringSegura QueryString
        {
            get
            {
                var qs = default(QueryStringSegura);

                try
                {
                    String dados = this.Request.QueryString["dados"];
                    if (!String.IsNullOrEmpty(dados))
                        qs = new QueryStringSegura(dados);
                }
                catch (QueryStringExpiradaException ex)
                {
                    Logger.GravarErro("QueryStringSegura expirada", ex);
                }
                catch (QueryStringInvalidaException ex)
                {
                    Logger.GravarErro("QueryStringSegura inválida", ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro durante leitura de QueryStringSegura", ex);
                }

                return qs;
            }
        }

        /// <summary>
        /// Parâmetros Mobile, ficam armazenados em Sessão, pois o valor não varia por PV.
        /// Contém os valores de ativação/contratação dos leitores CCM, CPA, CPC, e dados
        /// do endereço de entrega
        /// </summary>
        private static PedidoNovoLeitor Parametros
        {
            get { return (PedidoNovoLeitor)HttpContext.Current.Session["PedidoNovoLeitor"]; }
            set { HttpContext.Current.Session["PedidoNovoLeitor"] = value; }
        }

        #endregion

        #region [ Eventos (Páginas/Controles) ]

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Faça sua venda Mobile Rede - Page Load"))
            {
                if (!IsPostBack)
                {
                    //Carrega dados da integração com Portal para carregamento dos controles
                    Parametros = CarregarInformacoesIntegracao();
                    if (Parametros != null)
                    {
                        CarregarDadosEnderecoEntrega();
                        CarregarDadosLeitores();
                        CarregarValidadeCartao();
                    }
                    else
                    {
                        //Erro: dados de integração não foram encontrados
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }

        /// <summary>
        /// Continuação (cobrança E-Rede e inclusão da solicitação).
        /// </summary>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Efetua pagamento"))
            {
                var codRetorno = default(Int32);
                var fonte = default(String);
                var mensagem = String.Empty;
                var venda = default(Venda);
                var retorno = default(TransacaoVenda);

                //Prepara objeto contendo os dados da transação
                venda = MontarTransacao();

                //Por segurança, verifica se todos os dados obrigatórios foram informados
                //e só continua, em caso positivo
                Boolean dadosValidos = ValidarCamposObrigatorios();
                if (dadosValidos)
                {
                    try
                    {
                        //Se há valor a ser transacionado (contratação de CPA ou CCM), 
                        //executa transação DataCash
                        if (venda != null)
                        {
#if DEBUG
                            retorno = new TransacaoVenda
                            {
                                DadosCartao = new Cartao
                                {
                                    AnoValidade = venda.DadosCartao.AnoValidade,
                                    Bandeira = enBandeiras.Master,
                                    NomePortador = "ANTONIO COUTINHO",
                                    Parcelas = venda.DadosCartao.Parcelas,
                                    Numero = venda.DadosCartao.Numero,
                                    CodigoSeguranca = venda.DadosCartao.CodigoSeguranca,
                                },
                                TipoTransacao = venda.TipoTransacao,
                                FormaPagamento = venda.FormaPagamento,
                                NSU = "123456789",
                                TID = "123451234512345",
                                DataConfirmacao = DateTime.Now,
                                DataPreAutorizacao = DateTime.Now,
                                DataTransacao = DateTime.Now,
                                HoraConfirmacao = "12:34",
                                HoraPreAutorizacao = "21:21",
                                HoraTransacao = new DateTime(2014, 01, 01, 16, 25, 0),
                                NumeroAutorizacao = "54542124",
                                NumeroPedido = "12548",
                                ValidadePreAutorizacao = DateTime.Now,
                                Valor = venda.Valor,
                                ValorPreAutorizacao = 919.23m,
                            };
                            codRetorno = 1;
#else
                            log.GravarMensagem("Inicia execução da transação", venda);
                            retorno = new Negocio.Vendas()
                                .ExecutaTransacaoVenda(venda, Request.UserHostAddress, out codRetorno, out mensagem);
                            log.GravarMensagem("Retorno da execução da transação", new { retorno, codRetorno, mensagem });
#endif
                        }
                        else
                        {
                            //Retorno fake, sinaliza como sucesso na transação (apesar de não ter sido realizada)
                            codRetorno = 1;
                            mensagem = "Transação não realizada pois não há valor de cobrança";
                            log.GravarMensagem(mensagem, codRetorno);
                        }
                    }
                    catch (PortalRedecardException ex)
                    {
                        fonte = FONTE;
                        log.GravarErro(ex);
                    }
                    catch (Exception ex)
                    {
                        fonte = FONTE;
                        log.GravarErro(ex);
                    }

                    if (codRetorno != 1)
                        fonte = "DataCashService.TransactionXMLPortal";

                    String guidRetorno = Guid.NewGuid().ToString("N");
                    PedidoNovoLeitorRetorno dadosCache = MontarDadosRetorno(retorno, fonte, mensagem, codRetorno);

                    //Adiciona no Cache, porém, serializando o objeto, para permitir leitura e recuperação na tela do Portal (tela Pai)
                    CacheAdmin.Adicionar(Comum.Cache.DataCashIntegracao, guidRetorno, JsSerializer.Serialize(dadosCache));

                    log.GravarMensagem("Dados inserido em cache", new { guidRetorno, dadosCache });

                    //Registra script para execução, informando Página Portal do resultado da execução da transação
                    String script = String.Format("RetornoTransacao('{0}');", JsSerializer.Serialize(new { Guid = guidRetorno }));
                    ClientScript.RegisterStartupScript(this.GetType(), "RetornoDataCash", script, true);
                }
            }
        }

        /// <summary>
        /// Valida dados obrigatórios
        /// </summary>
        /// <param name="venda">Dados da venda</param>
        /// <returns>TRUE: dados validados. FALSE: caso contrário</returns>
        private Boolean ValidarCamposObrigatorios()
        {
            //Recupera os dados da tela para montagem da transação
            Int32 qtdParcelas = hddParcelas.Value.ToInt32(0);
            String formaPagamento = ddlFormaPagamento.SelectedValue;
            String numeroCartao = txtNumeroCartao.Text;
            String validadeCartaoMes = ddlValidadeCartaoMes.SelectedValue;
            String validadeCartaoAno = ddlValidadeCartaoAno.SelectedValue;
            String codigoSeguranca = txtCodigoSeguranca.Text;
            Int32 quantidadeCCM = txtQuantidadeCCM.Text.ToInt32(0);
            Int32 quantidadeCPA = txtQuantidadeCPA.Text.ToInt32(0);
            Int32 quantidadeCPC = txtQuantidadeCPC.Text.ToInt32(0);
            Decimal valorTotal = CalcularValorTotal(quantidadeCCM, quantidadeCPA, quantidadeCPC);

            //Se há contratação de CPA ou CCM, os dados de pagamento devem ser informados
            Boolean validarDadosPagamento = (quantidadeCCM + quantidadeCPA) > 0;
            if (validarDadosPagamento)
            {
                Boolean dadosPagamentosInformados = 
                    valorTotal > 0
                    && formaPagamento.EmptyToNull() != null
                    && numeroCartao.EmptyToNull() != null
                    && validadeCartaoAno.EmptyToNull() != null
                    && validadeCartaoMes.EmptyToNull() != null
                    && codigoSeguranca.EmptyToNull() != null;
                return dadosPagamentosInformados;
            }
            //Se não houve contratação, deve ter aluguel
            else
            {
                Boolean solicitouAluguel = quantidadeCPC > 0;
                return solicitouAluguel;
            }
        }

        /// <summary>
        /// Monta os dados de retorno
        /// </summary>
        /// <param name="retornoTransacao">Dados do retorno da transação</param>
        /// <param name="codRetorno">Código retorno</param>
        /// <param name="fonte">Fonte</param>
        /// <param name="mensagem">Mensagem</param>
        /// <returns>Dicionário contendo os dados de retorno</returns>
        private PedidoNovoLeitorRetorno MontarDadosRetorno(
            TransacaoVenda retornoTransacao, String fonte, String mensagem, Int32 codRetorno)
        {
            PedidoNovoLeitorRetorno retorno = new PedidoNovoLeitorRetorno();
            retorno.CodigoRetorno = codRetorno;
            retorno.Fonte = fonte;
            retorno.Mensagem = mensagem;

            if (retornoTransacao != null)
            {
                if (retornoTransacao.DadosCartao != null)
                {
                    retorno.CartaoAnoValidade = retornoTransacao.DadosCartao.AnoValidade;
                    retorno.CartaoBandeira = retornoTransacao.DadosCartao.Bandeira.GetTitle();
                    retorno.CartaoCodigoSeguranca = retornoTransacao.DadosCartao.CodigoSeguranca;
                    retorno.CartaoMesValidade = retornoTransacao.DadosCartao.MesValidade;
                    retorno.CartaoNomePortador = retornoTransacao.DadosCartao.NomePortador;
                    retorno.CartaoNumero = retornoTransacao.DadosCartao.Numero;
                    retorno.CartaoNumeroCriptografado = retornoTransacao.DadosCartao.NumeroCriptografado;
                    retorno.CartaoNumeroFormatado = retornoTransacao.DadosCartao.NumeroFormatado;
                    retorno.CartaoParcelas = retornoTransacao.DadosCartao.Parcelas;
                    retorno.CartaoValidade = retornoTransacao.DadosCartao.Validade;
                }

                retorno.DataConfirmacao = retornoTransacao.DataConfirmacao;
                retorno.DataPreAutorizacao = retornoTransacao.DataPreAutorizacao;
                retorno.DataTransacao = retornoTransacao.DataTransacao;
                retorno.FormaPagamento = retornoTransacao.FormaPagamento.GetTitle();
                retorno.HoraConfirmacao = retornoTransacao.HoraConfirmacao;
                retorno.HoraPreAutorizacao = retornoTransacao.HoraPreAutorizacao;
                retorno.HoraTransacao = retornoTransacao.HoraTransacao;
                retorno.NSU = retornoTransacao.NSU;
                retorno.NumeroAutorizacao = retornoTransacao.NumeroAutorizacao;
                retorno.NumeroPedido = retornoTransacao.NumeroPedido;
                retorno.TID = retornoTransacao.TID;
                retorno.TipoTransacao = retornoTransacao.TipoTransacao.GetTitle();
                retorno.ValidadePreAutorizacao = retornoTransacao.ValidadePreAutorizacao;
                retorno.Valor = retornoTransacao.Valor;
                retorno.ValorFormatado = retornoTransacao.ValorFormatado;
                retorno.ValorPreAutorizacao = retornoTransacao.ValorPreAutorizacao;
            }

            retorno.QuantidadeCCM = txtQuantidadeCCM.Text.ToInt32(0);
            retorno.QuantidadeCPA = txtQuantidadeCPA.Text.ToInt32(0);
            retorno.QuantidadeCPC = txtQuantidadeCPC.Text.ToInt32(0);
            retorno.Parametros = Parametros;

            return retorno;
        }

        /// <summary>
        /// Prepara o modelo de transação de vendas.
        /// Retorna <code>null</code> caso não seja necessária transação (sem valor de cobrança)
        /// </summary>
        /// <returns>Modelo Venda</returns>
        private Venda MontarTransacao()
        {
            //Recupera os dados da tela para montagem da transação
            Int32 qtdParcelas = hddParcelas.Value.ToInt32(0);
            String formaPagamento = ddlFormaPagamento.SelectedValue;
            String numeroCartao = txtNumeroCartao.Text;
            String validadeCartaoMes = ddlValidadeCartaoMes.SelectedValue;
            String validadeCartaoAno = ddlValidadeCartaoAno.SelectedValue;
            String codigoSeguranca = txtCodigoSeguranca.Text;
            Int32 quantidadeCCM = txtQuantidadeCCM.Text.ToInt32(0);
            Int32 quantidadeCPA = txtQuantidadeCPA.Text.ToInt32(0);
            Int32 quantidadeCPC = txtQuantidadeCPC.Text.ToInt32(0);
            Decimal valorTotal = CalcularValorTotal(quantidadeCCM, quantidadeCPA, quantidadeCPC);

            //Só realiza transação DataCash se houver valor a ser cobrado
            //Há valor de cobrança quando pelo menos 1 equipamento CCM ou CPA for contratado,
            //pois CPC é aluguel
            Boolean realizarTransacao = valorTotal > 0;

            //Modelo contendo os dados da transação
            Venda venda = null;

            if (realizarTransacao)
            {
                switch (formaPagamento)
                {
                    case "Crédito":
                        switch (qtdParcelas)
                        {
                            case 0:
                                throw new PortalRedecardException(CODIGO_ERRO, FONTE,
                                    "Quantidade de Parcelas Inválida: 0", new NotImplementedException());
                            case 1: //À Vista
                                venda = new Modelo.VendaCreditoAVista();
                                break;
                            default: //2 ou mais parcelas
                                venda = new Modelo.VendaCreditoParceladoEstabelecimento();
                                break;
                        }
                        break;
                    default:
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE,
                            String.Format("Forma de Pagamento inválida: {0}", formaPagamento),
                            new NotImplementedException());
                }

                venda.DadosCartao = new Modelo.Cartao();
                venda.DadosCartao.Numero = numeroCartao;
                venda.DadosCartao.MesValidade = validadeCartaoMes;
                venda.DadosCartao.AnoValidade = validadeCartaoAno;
                venda.DadosCartao.CodigoSeguranca = codigoSeguranca;
                venda.DadosCartao.Parcelas = qtdParcelas.ToString("D2");
                venda.Valor = valorTotal;

                //Número do PV do Mobile Rede
                venda.CodigoEntidade = Parametros.CodigoEntidadeDataCash;

                //Número do pedido deve ser único, por dia, corresponde ao número do PV
                venda.NumeroPedido = GerarNumeroPedido(this.SessaoAtual.CodigoEntidade);                
            }

            return venda;
        }

        #endregion

        #region [ Carregamento de Informações ]

        /// <summary>
        /// Carrega os dados presentes no Cache de Integração e transfere para a sessão
        /// atual do usuário
        /// </summary>
        private PedidoNovoLeitor CarregarInformacoesIntegracao()
        {
            var parametrosMobile = default(PedidoNovoLeitor);

            if (QueryString != null)
            {
                String guidDados = QueryString[ChaveIntegracao];
                String parametros = CacheAdmin.Recuperar<String>(Comum.Cache.DataCashIntegracao, guidDados);

                if (parametros.EmptyToNull() != null)
                {
                    //Remove do Cache de integração, pois já foi recuperado e transferido 
                    //para a sessão do usuário
                    CacheAdmin.Remover(Comum.Cache.DataCashIntegracao, guidDados);

                    parametrosMobile = JsSerializer.Deserialize<PedidoNovoLeitor>(parametros);
                }
            }

            return parametrosMobile;
        }

        /// <summary>
        /// Carrega os dados do Endereço de Entrega
        /// </summary>
        private void CarregarDadosEnderecoEntrega()
        {
            StringBuilder linha1 = new StringBuilder(Parametros.EnderecoLogradouro);
            if(Parametros.EnderecoNumero.EmptyToNull() != null)
                linha1.Append(", ").Append(Parametros.EnderecoNumero);
            if(Parametros.EnderecoComplemento.EmptyToNull() != null)
                linha1.Append(" - ").Append(Parametros.EnderecoComplemento);

            ltrEnderecoLinha1.Text = linha1.ToString();
            ltrEnderecoLinha2.Text = String.Format("{0}, {1}, {2} - {3}",
                Parametros.EnderecoBairro, Parametros.EnderecoCidade, Parametros.EnderecoEstado, Parametros.EnderecoCEP);
            ltrEnderecoLinha3.Text = "Brasil";
        }

        /// <summary>
        /// Carrega os dados dos leitores CPC, CCM e CPA
        /// </summary>
        private void CarregarDadosLeitores()
        {
            using (Logger log = Logger.IniciarLog("Preenchendo controles dos Leitores"))
            {
                try
                {
                    if (Parametros != null)
                    {
                        //Leitor de Tarja Magnética
                        {
                            Decimal valorTotal = Parametros.TaxaAtivacaoCCM;
                            Int32 qtdParcelas = Parametros.QtdeMaximaParcelasCCM;
                            Decimal valorParcela = CalcularValorParcela(valorTotal, qtdParcelas);

                            String valor = String.Format("{0}x {1}", 
                                qtdParcelas.ToString("D0", PtBr), valorParcela.ToString("C2", PtBr));

                            ltrCCMValor.Text = valor;
                            ltrCCMValorTabela.Text = valor;
                        }

                        //Leitor de Chip e Tarja (Pagamento Único)
                        {
                            Decimal valorTotal = Parametros.TaxaAtivacaoCPA;
                            Int32 qtdParcelas = Parametros.QtdeMaximaParcelasCPA;
                            Decimal valorParcela = CalcularValorParcela(valorTotal, qtdParcelas);

                            String valor = String.Format("{0}x {1}", 
                                qtdParcelas.ToString("D0", PtBr), valorParcela.ToString("C2", PtBr));

                            ltrCPAValor.Text = valor;
                            ltrCPAValorTabela.Text = valor;
                        }

                        //Leitor de Chip e Tarja (Taxa de Manutenção / Aluguel)
                        {
                            Decimal valorTotal = Parametros.TaxaAtivacaoCPC;
                            Int32 qtdParcelas = Parametros.QtdeMaximaParcelasCPC;

                            String valor = valorTotal.ToString("C2", PtBr);

                            ltrCPCValor.Text = valor;
                            ltrCPCValorTabela.Text = valor;
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega as combos de Validade do Cartão
        /// </summary>
        private void CarregarValidadeCartao()
        {
            //Preenche combo de Anos
            ddlValidadeCartaoAno.Items.Clear();
            //Preenche com até 10 anos para frente
            Int32 limiteAno = DateTime.Today.Year + 10;
            for (Int32 anoCorrente = DateTime.Today.Year; anoCorrente <= limiteAno; anoCorrente++)
            {
                DateTime ano = new DateTime(anoCorrente, 1, 1);
                ddlValidadeCartaoAno.Items.Add(new ListItem(ano.ToString("yy"), ano.ToString("yy")));
            }

            //Preenche combo de Meses
            ddlValidadeCartaoMes.Items.Clear();
            //Preenche com os 12 meses
            for (Int32 mesCorrente = 1; mesCorrente <= 12; mesCorrente++)
                ddlValidadeCartaoMes.Items.Add(new ListItem(mesCorrente.ToString("D2"), mesCorrente.ToString("D2")));
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Auxiliar para geração do número do pedido DataCash para o PV
        /// </summary>
        /// <param name="numeroPv">Número do PV</param>
        /// <returns>Número do Pedido gerado para o PV</returns>
        private static String GerarNumeroPedido(Int32 numeroPv)
        {
            String numeroPedido = String.Format("{0:D9}{1:ddMMhhmm}", numeroPv, DateTime.Now);
            return numeroPedido;
        }

        /// <summary>
        /// Calcula o valor da parcela, dado o valor total e a quantidade das parcelas.
        /// O valor da parcela é arredondado para baixo, com até 2 casas decimais.
        /// </summary>
        /// <param name="valorTotal">Valor total a ser parcelado</param>
        /// <param name="numeroParcelas">Quantidade de parcelas</param>
        /// <returns>Valor da parcela (arredondado para baixo)</returns>
        private static Decimal CalcularValorParcela(Decimal valorTotal, Int32 numeroParcelas)
        {
            if (numeroParcelas == 1)
                return valorTotal;
            else
                return Decimal.Truncate(100 * valorTotal / numeroParcelas) / 100;
        }

        /// <summary>
        /// Gera as formas de parcelamento
        /// </summary>
        /// <param name="maximoParcelas">Quantidade máxima de parcelas</param>
        /// <param name="valorTotal">Valor total da transação</param>
        private static Object GerarParcelamentosPossiveis(Int32 maximoParcelas, Decimal valorTotal)
        {
            return Enumerable.Range(1, maximoParcelas).ToArray().Select((quantidadeParcelas) =>
            {
                Decimal valorParcela = CalcularValorParcela(valorTotal, quantidadeParcelas);
                String descricao = String.Format("{0:N0}x {1}", quantidadeParcelas, valorParcela.ToString("C2", ptBR));

                if (quantidadeParcelas == 1)
                    descricao = String.Format("À Vista {0}", valorParcela.ToString("C2", ptBR));

                return new 
                { 
                    Descricao = descricao,
                    ValorParcela = valorParcela,
                    ID = quantidadeParcelas.ToString("D2")
                };
            }).ToArray();
        }

        /// <summary>
        /// Calcula o valor total da compra.
        /// </summary>
        /// <param name="quantidadeCCM">Quantidade de leitores CCM</param>
        /// <param name="quantidadeCPA">Quantidade de leitores CPA</param>
        /// <param name="quantidadeCPC">Quantidade de leitors CPC</param>
        /// <returns>Valor total do pedido a ser cobrado no E-Rede</returns>
        private static Decimal CalcularValorTotal(Int32 quantidadeCCM, Int32 quantidadeCPA, Int32 quantidadeCPC)
        {
            //Valor total da compra (apenas CCM e CPA, pois CPC é aluguel)
            return quantidadeCCM * Parametros.TaxaAtivacaoCCM + quantidadeCPA * Parametros.TaxaAtivacaoCPA;
        }

        #endregion

        #region [ WebMethods ]

        /// <summary>
        /// Gera dados do resumo do Pedido (chamado a cada alteração de quantidade de leitores solicitados)
        /// </summary>
        /// <param name="ccm">Quantidade de leitores CCM</param>
        /// <param name="cpa">Quantidade de leitores CPA</param>
        /// <param name="cpc">Quantidade de leitores CPC</param>
        /// <returns>JSON (string) contendo os dados do resumo do pedido</returns>
        [WebMethod(EnableSession=true)]
        public static String AtualizarResumoPedido(String ccm, String cpa, String cpc)
        {
            Sessao sessao = Sessao.Obtem();

            Int32 quantidadeCCM = ccm.ToInt32();
            Int32 quantidadeCPA = cpa.ToInt32();
            Int32 quantidadeCPC = cpc.ToInt32();

            //Calcula a quantidade máxima de parcelas permitidas (considera apenas CCM e CPA)
            Int32 qtdMaximaParcelas = Math.Max(Parametros.QtdeMaximaParcelasCCM, Parametros.QtdeMaximaParcelasCPA);

            //Valor total da compra (apenas CCM e CPA, pois CPC é aluguel)
            Decimal valorTotal = CalcularValorTotal(quantidadeCCM, quantidadeCPA, quantidadeCPC);

            //Prepara objeto de retorno para o Resumo do Pedido - Modelo Taxa de Manutenção
            var resumoManutencao = new
            {
                CPC = new
                {
                    Quantidade = quantidadeCPC.ToString("D3"),
                    Valor = quantidadeCPC > 1 ?
                        String.Format("{0} x {1}", Parametros.TaxaAtivacaoCPC.ToString("N2", PtBr), quantidadeCPC) :
                        (Parametros.TaxaAtivacaoCPC * quantidadeCPC).ToString("N2", PtBr)
                },
                ValorTotal = (quantidadeCPC * Parametros.TaxaAtivacaoCPC).ToString("N2", PtBr)
            };

            //Prepara objeto de retorno para o Resumo do Pedido - Modelo Pagamento Único
            var resumoPagamentoUnico = new
            {
                CCM = new
                {
                    Quantidade = quantidadeCCM.ToString("D3"),
                    Valor = quantidadeCCM > 1 ?
                        String.Format("{0} x {1}", Parametros.TaxaAtivacaoCCM.ToString("N2", PtBr), quantidadeCCM) :
                        (Parametros.TaxaAtivacaoCCM * quantidadeCCM).ToString("N2", PtBr)
                },
                CPA = new
                {
                    Quantidade = quantidadeCPA.ToString("D3"),
                    Valor = quantidadeCPA > 1 ?
                        String.Format("{0} x {1}", Parametros.TaxaAtivacaoCPA.ToString("N2", PtBr), quantidadeCPA) :
                        (Parametros.TaxaAtivacaoCPA * quantidadeCPA).ToString("N2", PtBr)
                },
                ValorTotal = (valorTotal).ToString("N2", PtBr)
            };

            //Objeto de retorno
            var resumo = new
            {
                Manutencao = resumoManutencao,
                PagamentoUnico = resumoPagamentoUnico,
                Parcelas = GerarParcelamentosPossiveis(qtdMaximaParcelas, valorTotal),
            };

            //Resposta da requisição
            return JsSerializer.Serialize(resumo);
        }

        #endregion
    }
}