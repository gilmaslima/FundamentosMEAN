/*
© Copyright 2014 Rede S.A.
Autor : Seygi Kutani
Empresa : Iteris Consultoria e Software
*/

using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.Comum.TrataErroServico;
using System;
using System.Globalization;
using System.ServiceModel;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace Redecard.PN.Extrato.SharePoint.Handlers
{
    /// <summary>
    /// Handler utilizado nas requisições assíncronas da HomePage Segmentada (Varejo e EMP/IBBA)
    /// </summary>
    public partial class DirfAceite : UserControlBase, IHttpHandler, IReadOnlySessionState
    {
        /// <summary>
        /// IsReusable
        /// </summary>
        public Boolean IsReusable { get { return false; } }

        /// <summary>
        /// Culture pt-BR
        /// </summary>
        private static CultureInfo PtBr { get { return new CultureInfo("pt-BR"); } }

        /// <summary>
        /// Random
        /// </summary>
        private static Random Random { get { return new Random(); } }

        /// <summary>
        /// ProcessRequest
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                //Se não está autenticado, gera mensagem de erro
                if (!Sessao.Contem())
                {
                    ProcessarErro(context, String.Empty, null);
                    return;
                }

                String nomeLista = "Dirf Usuario Aceite";

                Boolean retorno = false;
                Object StrRetornoErro = String.Format("A lista {0} não foi encontrada.",nomeLista);

                Int32 pv = SessaoAtual.CodigoEntidade;
                Int32 codigoUsuario = SessaoAtual.CodigoIdUsuario;
                Int32 anoCorrente = context.Request.Params["anocorrente"].ToInt32();
                String ano = String.Format("AnoCalendario{0}",anoCorrente);

                if (SPContext.Current.Web.Lists.TryGetList(nomeLista) != null)
                {
                    SPList listaDirf = SPContext.Current.Web.Lists[nomeLista];

                    if (listaDirf.Fields.ContainsField(ano))
                    {
                        SPQuery query = new SPQuery();
                        query.Query = String.Format(@"
                        <Where>
                            <And>
                                <Eq>
                                    <FieldRef Name='Title' />
                                    <Value Type='Text'>{0}</Value>
                                </Eq>
                                <And>
                                    <Eq>    
                                        <FieldRef Name='NumeroDoPV' />
                                        <Value Type='Text'>{1}</Value>
                                    </Eq>
                                    <Eq>
                                        <FieldRef Name='{2}' />
                                        <Value Type='Boolean'>1</Value>
                                    </Eq>
                                </And>
                            </And>
                        </Where>",codigoUsuario,pv,ano);

                        SPListItemCollection itens = listaDirf.GetItems(query);
                        retorno = itens.Count > 0;
                    }
                    context.Response.Write(Serializar(retorno));
                }
                else
                {
                    context.Response.Write(Serializar(StrRetornoErro));
                }
                
            }
            catch (PortalRedecardException exc)
            {
                SharePointUlsLog.LogErro(exc);
                ProcessarErro(context, exc.Fonte, exc.Codigo);
            }
            catch (Exception exc)
            {
                SharePointUlsLog.LogErro(exc);
                ProcessarErro(context, FONTE, CODIGO_ERRO);
            }
            finally
            {
                context.Response.ContentType = "application/json";
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
        }

        /// <summary>
        /// Serialização Javascript
        /// </summary>
        private static String Serializar(Object obj)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        #region [ Processamento Comum ]
        /// <summary>
        /// Processamento genérico de Erro
        /// </summary>
        private static void ProcessarErro(HttpContext context, String fonte, Int32? codigo)
        {
            String mensagemRetorno = "Sistema indisponível";
            Int32 codigoRetorno = -1;
            if (!String.IsNullOrEmpty(fonte) && codigo.HasValue)
            {
                try
                {
                    using (var ctx = new ContextoWCF<TrataErroServicoClient>())
                    {
                        TrataErro erro = ctx.Cliente.Consultar(fonte, codigo.Value);
                        if (erro != null && erro.Codigo != 0)
                        {
                            mensagemRetorno = erro.Fonte;
                            codigoRetorno = erro.Codigo;
                        }
                    }
                }
                catch (FaultException ex)
                {
                    Logger.GravarErro("Erro durante exibição de erro", ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro durante exibição de erro", ex);
                }
            }

            var retornoErro = new { Mensagem = mensagemRetorno, Codigo = codigoRetorno };

            //Response contendo o erro
            context.Response.Write(Serializar(retornoErro));
        }
        #endregion
    }
}