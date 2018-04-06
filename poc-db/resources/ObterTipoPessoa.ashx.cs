using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing.Navigation;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace Redecard.PN.DadosCadastrais.SharePoint.Handlers
{
    public partial class ObterTipoPessoa : IHttpHandler, IRequiresSessionState
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

        #endregion

        /// <summary>
        /// Recebe a chamada do front-end e faz a chamada do serviço.
        /// </summary>
        /// <param name="context">Contexto Http</param>
        public void ProcessRequest(HttpContext context)
        {
            /*  Códigos de Retorno:
             *  0  - OK
             *  1  - Retorno do serviço vazio
             *  2  - TipoPessoa nulo.
             *  3  - Sessão não existente.
             *  4  - TipoPessoa já enviado para o PV informado.
             *  5  - Parâmetro ArrayPvs nulo.
             *  99 - Exceção genérica
            */
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            using (var log = Logger.IniciarLog("TipoPessoa - Handler - Obtendo tipo pessoa do GE"))
            {
                Int32 codigoRetorno = 0;

                try
                {
                    Int32 pv = SessaoAtual.CodigoEntidade;

                    if (context.Request.Params["datasIguais"].ToBoolNull() == true)
                    {
                        if (String.IsNullOrEmpty(context.Request.Params["arrayPvs"]))
                        {
                            codigoRetorno = 5;
                            context.Response.Write(jsSerializer.Serialize(new
                            {
                                CodigoRetorno = codigoRetorno,
                                MensagemRetorno = RetornarMensagem(codigoRetorno)
                            }));
                            return;
                        }

                        List<String> arrayPvs = new List<String>();
                        arrayPvs = context.Request.Params["arrayPvs"].Split(',').ToList();

                        var parseResult = 0;
                        List<Int32> listPvs = arrayPvs.Where(str => int.TryParse(str, out parseResult)).Select(str => parseResult).ToList();

                        if (listPvs.Contains(pv))
                        {
                            codigoRetorno = 4;
                            context.Response.Write(jsSerializer.Serialize(new
                            {
                                CodigoRetorno = codigoRetorno,
                                MensagemRetorno = RetornarMensagem(codigoRetorno)
                            }));
                            return;
                        }
                    }


                    Char tipoPessoa;

                    //Só faz a consulta caso a sessão exista.
                    if (SessaoAtual == null)
                    {
                        codigoRetorno = 3;
                        context.Response.Write(jsSerializer.Serialize(new
                        {
                            CodigoRetorno = codigoRetorno,
                            MensagemRetorno = RetornarMensagem(codigoRetorno)
                        }));
                    }
                    else
                    {
                        //Parâmetros de consulta.
                        using (var entidadeServico = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            tipoPessoa = entidadeServico.Cliente.ConsultarTipoPessoaPV(out codigoRetorno, pv);
                        }


                        //Caso o serviço retorne OK.
                        if (codigoRetorno == 0)
                        {
                            context.Response.Write(jsSerializer.Serialize(new
                            {
                                NumeroPv = pv,
                                CodigoRetorno = codigoRetorno,
                                TipoPessoa = tipoPessoa
                            }));

                        }

                        //Retorno com erro
                        else
                        {
                            context.Response.Write(jsSerializer.Serialize(new
                            {
                                CodigoRetorno = codigoRetorno,
                                MensagemRetorno = RetornarMensagem(codigoRetorno)
                            }));
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    codigoRetorno = codigoRetorno == 0 ? 99 : codigoRetorno;

                    context.Response.Write(jsSerializer.Serialize(new
                    {
                        CodigoRetorno = codigoRetorno,
                        MensagemRetorno = ex.Message
                    }));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    codigoRetorno = codigoRetorno == 0 ? 99 : codigoRetorno;

                    context.Response.Write(jsSerializer.Serialize(new
                    {
                        CodigoRetorno = codigoRetorno,
                        MensagemRetorno = ex.Message
                    }));
                }
            }
        }

        /// <summary>
        /// Retorna a mensagem de erro de acordo com o código de retorno.
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno.</param>
        /// <returns></returns>
        private String RetornarMensagem(Int32 codigoRetorno)
        {
            String mensagemRetorno;
            switch (codigoRetorno)
            {
                case 1:
                    mensagemRetorno = "Retorno do serviço vazio";
                    break;

                case 2:
                    mensagemRetorno = "TipoPessoa nulo";
                    break;

                case 3:
                    mensagemRetorno = "Sessão não existente para o usuário atual";
                    break;

                case 4:
                    mensagemRetorno = "TipoPessoa já enviado para o PV informado";
                    break;

                case 5:
                    mensagemRetorno = "Parâmetro ArrayPvs nulo";
                    break;

                default:
                    mensagemRetorno = "Exceção genérica";
                    break;
            }
            return mensagemRetorno;
        }
    }
}
