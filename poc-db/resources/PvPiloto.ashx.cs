using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login;
using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace Redecard.PN.DadosCadastrais.SharePoint.Handlers
{
    public partial class PvPiloto : IHttpHandler, IRequiresSessionState
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

        private JavaScriptSerializer jsSerializer;

        /// <summary>
        /// Executa o serviço que valida as transações
        /// </summary>
        /// <param name="context">Contexto Http</param>
        public void ProcessRequest(HttpContext context)
        {
            jsSerializer = new JavaScriptSerializer();
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            using (var log = Logger.IniciarLog("PVPiloto - Handler - Verificando se o PV é piloto"))
            {
                try
                {
                    //Parâmetros de consulta.
                    Int32 pv = SessaoAtual.CodigoEntidade;

                    Boolean pvEhPiloto = LoginClass.VerificarPiloto(pv);

                    log.GravarMensagem(String.Format("O PV: {0} é piloto: {1}", pv, pvEhPiloto));

                    //Serializando os dados em JSON e enviando para o front-end.
                    context.Response.Write(jsSerializer.Serialize(pvEhPiloto));
                    return;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    context.Response.Write(jsSerializer.Serialize(false));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    context.Response.Write(jsSerializer.Serialize(false));
                }
            }
        }
    }
}
