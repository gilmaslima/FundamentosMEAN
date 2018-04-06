/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using Redecard.PN.Boston.Sharepoint.ControlTemplates.Redecard.PN.Boston.Sharepoint;
using Redecard.PN.Boston.Sharepoint.DRParametrizacao;
using Redecard.PN.Boston.Sharepoint.Modelo;
using Redecard.PN.Boston.Sharepoint.Negocio;
using Redecard.PN.Boston.Sharepoint.WFProposta;
using Redecard.PN.Comum;
using System.Web.Configuration;

namespace Redecard.PN.Boston.Sharepoint.WebParts.NovoLeitorCartao
{
    /// <summary>
    /// Solicitação de Novo Leitor de Cartão.<br/>
    /// Esta página está acoplada ao projeto DataCash (FacaSuaVendaMobileRede.aspx).<br/>
    /// Tipos de equipamento:<br/>
    /// CCM – Leitor de Tarja Magnética<br/>
    /// CPA - Leitor de Chip e Tarja<br/>
    /// CPC - Leitor de Chip e Tarja (Aluguel)
    /// </summary>
    public partial class NovoLeitorCartaoUserControl : UserControlBase, IPostBackEventHandler
    {
        #region [ Propriedades / Variáveis ]

        /// <summary>
        /// Chave da QueryString que contém o GUID do objeto de integração com o Portal
        /// para a página FacaSuaVendaMobileRede.aspx do projeto DataCash
        /// </summary>
        private static String ChaveIntegracao { get { return "NovoLeitorCartao"; } }

        /// <summary>
        /// Chave da QueryString que contém o GUID do objeto de integração com o Portal
        /// para a página FacaSuaVendaMobileRedeComprovanteCredito.aspx do projeto DataCash
        /// </summary>
        private static String ChaveIntegracaoComprovante { get { return "NovoLeitorCartaoComprovanteCredito"; } }

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
        /// ucRedirecionaDataCash
        /// </summary>
        private RedirecionaDataCash UcRedirecionaDataCash { get { return (RedirecionaDataCash)ucRedirecionaDataCash; } }

        #endregion

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Novo Leitor Cartão - PageLoad"))
            {
                try
                {
                    if (Sessao.Contem())
                    {
                        if (!IsPostBack)
                        {
                            //Preparação do objeto que será repassado por AppFabric para o iframe DataCash
                            PedidoNovoLeitor parametros = 
                                ObterParametros(SessaoAtual.CodigoEntidade, SessaoAtual.CodigoCanal, SessaoAtual.CodigoCelula);

                            //Adiciona os dados do pedido em Cache, para recuperação pela página do DataCash/E-Rede
                            String guidResumoPedido = Guid.NewGuid().ToString("N");
                            CacheAdmin.Adicionar(Comum.Cache.DataCashIntegracao, guidResumoPedido, JsSerializer.Serialize(parametros));

                            log.GravarMensagem("GuidResumoPedido", guidResumoPedido);

                            //Atualiza URL do iframe, repassando o ID do objeto em cache que contém os dados consultados acima
                            //A página DataCash irá recuperar este objeto para montagem da tela, contendo os valores dos leitores
                            //e endereço de entrega
                            var queryString = new QueryStringSegura();
                            queryString[ChaveIntegracao] = guidResumoPedido;
                            UcRedirecionaDataCash.AtualizarRedirecionamento(queryString);

                            //Registra a função de postback customizado do controle
                            //Esta função é chamada quando o iframe sinaliza que a transação foi executada (com sucesso ou não)
                            StringBuilder script = new StringBuilder()
                                .AppendLine("function RetornoDataCashPostBack(params) {")
                                .AppendLine(Page.ClientScript.GetPostBackEventReference(this, "params").Replace("'params'", "params"))
                                .Append("}");
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "RetornoDataCashPostBack", script.ToString(), true);

                            UcRedirecionaDataCash.Visible = true;
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    UcRedirecionaDataCash.Visible = false;
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    UcRedirecionaDataCash.Visible = false;
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Consulta e monta objeto dos parâmetros necessários para a montagem da tela do iframe.
        /// Consulta os dados dos leitores (CCM, CPC e CPA), além do Endereço de Entrega.
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoCanal">Código do Canal</param>
        /// <param name="codigoCelula">Código da Célula</param>
        /// <returns>Parâmetros para montagem do iframe</returns>
        private static PedidoNovoLeitor ObterParametros(Int32 codigoEntidade, Int32 codigoCanal, Int32 codigoCelula)
        {
            //Preparação do objeto que será repassado por AppFabric para o iframe DataCash
            PedidoNovoLeitor parametros = new PedidoNovoLeitor();

            using (Logger log = Logger.IniciarLog("Obtendo parâmetros da página"))
            {
                log.GravarMensagem("Parâmetros Entrada", new { codigoEntidade, codigoCanal, codigoCelula });

                //Consulta dos dados CCM
                ParametroMobile parametrosCCM = Servicos.ConsultarParametroMobile(codigoCanal, codigoCelula, "CCM");
                if (parametrosCCM != null)
                {
                    parametros.QtdeMaximaParcelasCCM = parametrosCCM.QtdeMaximaParcelas;
                    parametros.TaxaAtivacaoCCM = parametrosCCM.ValorTaxaAtivacao;
                }

                //Consulta dos dados CPA
                ParametroMobile parametrosCPA = Servicos.ConsultarParametroMobile(codigoCanal, codigoCelula, "CPA");
                if (parametrosCPA != null)
                {
                    parametros.QtdeMaximaParcelasCPA = parametrosCPA.QtdeMaximaParcelas;
                    parametros.TaxaAtivacaoCPA = parametrosCPA.ValorTaxaAtivacao;
                }

                //Consulta dos dados CPC
                ParametroMobile parametrosCPC = Servicos.ConsultarParametroMobile(codigoCanal, codigoCelula, "CPC");
                if (parametrosCPC != null)
                {
                    parametros.QtdeMaximaParcelasCPC = parametrosCPC.QtdeMaximaParcelas;
                    parametros.TaxaAtivacaoCPC = parametrosCPC.ValorTaxaAtivacao;
                }

                //Consulta dos dados Endereço de Entrega
                Endereco endereco = Servicos.GetEnderecoInstalacaoPorPV(codigoEntidade);
                if (endereco != null)
                {
                    parametros.EnderecoBairro = endereco.Bairro;
                    parametros.EnderecoCEP = endereco.CEP;
                    parametros.EnderecoCidade = endereco.Cidade;
                    parametros.EnderecoComplemento = endereco.Complemento;
                    parametros.EnderecoEstado = endereco.Estado;
                    parametros.EnderecoLogradouro = endereco.Logradouro;
                    parametros.EnderecoNumero = endereco.Numero;
                    parametros.EnderecoTipoEndereco = endereco.TipoEndereco;
                }

                //Número do PV Mobile Rede que será transacionado no DataCash
                parametros.CodigoEntidadeDataCash =
                    Convert.ToString(WebConfigurationManager.AppSettings["numPdvEstabelecimento"]).ToInt32(0);

                log.GravarMensagem("Parâmetros obtidos", parametros);
            }

            return parametros;
        }

        /// <summary>
        /// Recupera do Cache o objeto contendo os dados de retorno do DataCash.
        /// </summary>
        /// <param name="guidCache">Guid do objeto no Cache</param>
        /// <returns>Objeto de retorno do DataCash</returns>
        private static PedidoNovoLeitorRetorno RecuperarCacheRetornoDataCash(String guidCache)
        {
            var retornoDataCash = default(PedidoNovoLeitorRetorno);
            var valorSerializado = default(String);

            using (Logger log = Logger.IniciarLog("Recuperar Cache Retorno DataCash"))
            {
                try
                {
                    //Recupera objeto serializado contendo o resultado da transação, além de informações do pedido
                    valorSerializado = CacheAdmin.Recuperar<String>(Comum.Cache.DataCashIntegracao, guidCache);

                    log.GravarMensagem("Recuperação no Cache do retorno DataCash", new { guidCache, valorSerializado });

                    if (valorSerializado.EmptyToNull() != null)
                    {
                        //Remove os dados do retorno da transação DataCash do Cache (Será futuramente reincluído no Cache)
                        CacheAdmin.Remover(Comum.Cache.DataCashIntegracao, guidCache);

                        //Recupera os dados da transação realizada
                        retornoDataCash = JsSerializer.Deserialize<PedidoNovoLeitorRetorno>(valorSerializado);
                    }

                    log.GravarMensagem("Valor retorno DataCash recuperado", retornoDataCash);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }

            return retornoDataCash;
        }

        /// <summary>
        /// Adiciona em cache o objeto que contém dados do retorno do DataCash
        /// </summary>
        /// <param name="guidCache">Guid do cache</param>
        /// <param name="dadosCache">Objeto que contém dados do retorno do DataCash</param>
        private static void AdicionarCacheRetornoDataCash(String guidCache, String dadosCache)
        {
            //Reinclui os dados do retorno da transação em Cache, para
            //que a nova tela de iframe do DataCash
            if (CacheAdmin.Recuperar<String>(Comum.Cache.DataCashIntegracao, guidCache) != null)
                CacheAdmin.Remover(Comum.Cache.DataCashIntegracao, guidCache);
            CacheAdmin.Adicionar(Comum.Cache.DataCashIntegracao, guidCache, dadosCache);
        }

        /// <summary>
        /// PostBack, chamado após a execução da transação realizada pelo iframe (resposta DataCash)
        /// </summary>
        /// <param name="guidCache">Guid do objeto em cache contendo os dados de retorno DataCash</param>
        public void RaisePostBackEvent(String guidCache)
        {
            using (Logger log = Logger.IniciarLog(String.Format("PostBackEventReference (guid={0})", guidCache)))
            {
                //Recupera os dados da transação realizada
                PedidoNovoLeitorRetorno retorno = RecuperarCacheRetornoDataCash(guidCache);

                if (retorno != null)
                {
                    try
                    {
                        //Inclusão no WF, em caso de transação aprovada
                        if (retorno.CodigoRetorno == 1)
                        {
                            Int32 numeroPv = this.SessaoAtual.CodigoEntidade;
                            Int32 codigoCelula = this.SessaoAtual.CodigoCelula;
                            Int32 codigoCanal = this.SessaoAtual.CodigoCanal;

                            //Prepara lista de equipamentos solicitados/comprados para inclusão no WF
                            var equipamentos = new List<ListaPropostaVendaTecnologia>(
                                new[] {
                                    new ListaPropostaVendaTecnologia {
                                        codTipoEquipamento = "CCM",
                                        QuantidadeTipoEquipamento = retorno.QuantidadeCCM,
                                        valorDoEquipamento = retorno.Parametros.TaxaAtivacaoCCM
                                    },
                                    new ListaPropostaVendaTecnologia {
                                        codTipoEquipamento = "CPA",
                                        QuantidadeTipoEquipamento = retorno.QuantidadeCPA,
                                        valorDoEquipamento = retorno.Parametros.TaxaAtivacaoCPA
                                    },
                                    new ListaPropostaVendaTecnologia {
                                        codTipoEquipamento = "CPC",
                                        QuantidadeTipoEquipamento = retorno.QuantidadeCPC,
                                        valorDoEquipamento = retorno.Parametros.TaxaAtivacaoCPC
                                    }
                            //Apenas equipamentos com quantidade > 0 devem ser enviados na lista
                            }).Where(equipamento => equipamento.QuantidadeTipoEquipamento > 0).ToList();

                            //Inclusão no WF
                            WFProposta.RetornoErro retornoWF =
                                Servicos.IncluirPropostaVendaTecnologia(numeroPv, codigoCanal, codigoCelula, equipamentos.ToArray());

                            //Armazena resultado da inclusão no WF no objeto de retorno que é enviado para o comprovante
                            if (retornoWF != null)
                            {
                                if(retornoWF.CodigoErro.HasValue)
                                    retorno.CodigoRetornoWf = retornoWF.CodigoErro.Value;
                                retorno.MensagemWf = retornoWF.DescricaoErro;
                            }
                        }
                    }
                    catch (PortalRedecardException ex)
                    {
                        log.GravarErro(ex);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                    }
                    //Sempre executado, mesmo se houver erro na inclusão no WF
                    finally
                    {
                        //Reinclui os dados do retorno da transação em Cache, para
                        //que a nova tela de iframe do DataCash, renovando o tempo de expiração
                        String valorSerializado = JsSerializer.Serialize(retorno);
                        AdicionarCacheRetornoDataCash(guidCache, valorSerializado);

                        //Redireciona para a página de Comprovante, repassando o guid do objeto em cache
                        //que contém os dados de retorno do DataCash
                        var qs = new QueryStringSegura();
                        qs[ChaveIntegracaoComprovante] = guidCache;
                        String url = String.Format("NovoLeitorCartaoComprovanteCredito.aspx?dados={0}", qs.ToString());
                        this.Response.Redirect(url, false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
                else
                {
                    //Em caso de dados inválidos no PostBack, redireciona para estado inicial da tela
                    Response.Redirect(this.Page.Request.Url.AbsolutePath, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
        }
    }
}