using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace Redecard.PN.DadosCadastrais.SharePoint.Handlers
{
    public partial class DomicilioBancarioValidacoes : IHttpHandler, IRequiresSessionState
    {
        #region [ Propriedades ]
        /// <summary>Valor padrão de propriedades</summary>
        public bool IsReusable { get { return false; } }

        private Sessao sessao = null;
        private Sessao SessaoAtual
        {
            get
            {
                if (sessao != null && Sessao.Contem())
                    return sessao;
                else
                {
                    if (Sessao.Contem())
                    {
                        sessao = Sessao.Obtem();
                    }
                    return sessao;
                }
            }
        }

        private JavaScriptSerializer jsSerializer;
        #endregion

        /// <summary>
        /// Executa o serviço que valida as transações
        /// </summary>
        /// <param name="context">Contexto Http</param>
        public void ProcessRequest(HttpContext context)
        {
            /*
            Retornos 
            0 = ok
            2 = erro na validacao dos parametros
            3 = codigo do retorno do servico diferente de 0
            99 = qlqr exepction nao tratada
            */

            jsSerializer = new JavaScriptSerializer();
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            using (var log = Logger.IniciarLog("Domicilio Bancario validações - Handler - Validando alterações efetuadas nos dados de domicílio bancário"))
            {
                try
                {
                    Int32 codigoBanco = context.Request.Params["codigobanco"].ToInt32();
                    String codigoAgencia = context.Request.Params["codigoagencia"];
                    String numeroConta = context.Request.Params["numeroconta"];
                    Boolean precisaConfirmacaoEletronica = Convert.ToBoolean(context.Request.Params["confirmacaoeletronica"]);
                    String tipoTransacao = context.Request.Params["tipotransacao"];
                    String descricaoBandeira = context.Request.Params["descricaobandeira"];
                    String siglaBandeira = context.Request.Params["siglabandeira"];

                    if (codigoBanco == 0)
                        throw new PortalRedecardException(2, String.Empty, "Código do banco inválido", null);
                    if (String.IsNullOrWhiteSpace(codigoAgencia))
                        throw new PortalRedecardException(2, String.Empty, "Código da agencia inválido", null);
                    if (String.IsNullOrWhiteSpace(numeroConta))
                        throw new PortalRedecardException(2, String.Empty, "Número da conta inválido", null);
                    if (String.IsNullOrWhiteSpace(tipoTransacao))
                        throw new PortalRedecardException(2, String.Empty, "Tipo da transação da bandeira inválida", null);
                    if (String.IsNullOrWhiteSpace(descricaoBandeira))
                        throw new PortalRedecardException(2, String.Empty, "Descrição da bandeira inválida", null);
                    if (String.IsNullOrWhiteSpace(siglaBandeira))
                        throw new PortalRedecardException(2, String.Empty, "Sigla da bandeira inválida", null);

                    using (var entidadeCliente = new EntidadeServico.EntidadeServicoClient())
                    {
                        String aguardaDocumento = "S";
                        String confEletronica = "N";
                        Int32 codigoRetorno = 0;

                        if (precisaConfirmacaoEletronica)
                        {
                            confEletronica = "S";
                            aguardaDocumento = "N";
                        }

                        //BBF19 - substitui qlqr caracter alfa por 0 no numero da conta
                        numeroConta = Regex.Replace(numeroConta, @"[a-zA-Z]+", "0").Replace("-", String.Empty);

                        DadosDomiciolioBancario domicilio = new DadosDomiciolioBancario();

                        domicilio.CodigoAgencia = codigoAgencia;
                        domicilio.NumeroConta = numeroConta.PadLeft(10, '0');
                        domicilio.CodigoBanco = codigoBanco;

                        Bandeira bandeira = new Bandeira
                        {
                            DescricaoProduto = descricaoBandeira,
                            SiglaProduto = siglaBandeira,
                            TipoTransacao = Char.Parse(tipoTransacao)
                        };

                        if (bandeira.TipoTransacao == 'C')
                        {
                            domicilio.BandeirasCredito = new Bandeira[1]{
                                bandeira
                            };
                        }
                        else if (bandeira.TipoTransacao == 'D')
                        {
                            domicilio.BandeirasDebito = new Bandeira[1]{
                                bandeira
                            };
                        }
                        else
                            throw new PortalRedecardException(2, String.Empty, String.Format("Tipo da transação da bandeira inválida. {0}", bandeira.TipoTransacao), null);

                        // Verifica se a bandeira de débito é Construcard. Se for, permite apenas domicílios CEF - Código 104
                        if (domicilio.BandeirasDebito != null)
                        {
                            var bndConstucard = domicilio.BandeirasDebito.FirstOrDefault(o => o.SiglaProduto == "CC");
                            if (bndConstucard != null && domicilio.CodigoBanco != 104)
                                throw new PortalRedecardException(2, String.Empty, "Domicílio bancário não permitido", null);
                        }

                        entidadeCliente.ValidarAlteracaoDomicilioBancario(out codigoRetorno, SessaoAtual.CodigoEntidade, domicilio, confEletronica, aguardaDocumento);
                        if (codigoRetorno != 0)
                        {
                            String mensagem = String.Format("Erro ao validar alteração do Domicílio bancário. Cod. {0}", codigoRetorno);
                            context.Response.Write(jsSerializer.Serialize(new
                            {
                                Retorno = 3,
                                MensagemErro = mensagem
                            }));
                            return;
                        }
                    }

                    context.Response.Write(jsSerializer.Serialize(new { Retorno = 0 }));
                    return;
                }
                catch (PortalRedecardException ex)
                {
                    if (ex.Codigo == 2) // erro amigavel
                    {
                        context.Response.Write(jsSerializer.Serialize(new
                        {
                            Retorno = 2,
                            MensagemErro = ex.Message
                        }));
                    }
                    else
                    {
                        log.GravarErro(ex);
                        context.Response.Write(jsSerializer.Serialize(new
                        {
                            Retorno = 99,
                            Fonte = ex.Fonte,
                            MensagemErro = ex.Message
                        }));
                    }
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    context.Response.Write(jsSerializer.Serialize(new
                    {
                        Retorno = 99,
                        MensagemErro = ex.Message
                    }));
                }
            }
        }
    }
}
