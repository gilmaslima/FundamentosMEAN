/*
(c) Copyright [ANO] Redecard S.A.
Autor : [Tiago]
Empresa : [BRQ IT Services]
Histórico:
- [01/08/2012] – [Tiago] – [Etapa inicial]
*/

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Redecard.PN.Comum;
using Redecard.PN.RAV.Core.Web.Controles.Portal;
using Redecard.PN.RAV.Sharepoint.ModuloRAV;
using System;
using System.ServiceModel;
using System.Web.UI;

namespace Redecard.PN.RAV.Sharepoint.WebParts.AcessoSenha
{
    public partial class AcessoSenhaUserControl : UserControlBase
    {
        #region Constantes
        public const string FONTE = "AcessoSenhaUserControl.ascx";
        public const int CODIGO_ERRO = 3001;
       
        #endregion
           
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // ignora validações se a página estiver em modo de edição
            if (SPContext.Current.FormContext.FormMode == SPControlMode.Edit ||
                SPContext.Current.Site.RootWeb.CurrentUser != null && SPContext.Current.Site.RootWeb.CurrentUser.IsSiteAdmin)
                return;

            using (Logger Log = Logger.IniciarLog("Acesso Senha - Page Load"))
            {
                // exige o parâmetro "dados" na querystring
                if (String.IsNullOrWhiteSpace(Request.QueryString["dados"]))
                {
                    Log.GravarMensagem("Nenhum parâmetro fornecido via QueryString");
                    Response.Redirect("pn_Principal.aspx", false);
                    return;
                }

                if (((AcessoSenha)this.Parent).RedirecionaAutomatico)
                {
                    Log.GravarMensagem("Redirecionando automaticamente (ignorando input de senha RAV)");
                    Redireciona();
                    return;
                }
            }
        }

        /// <summary>
        /// Evento que verifica senha do usuário e envia para o meu Principal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Continuar(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            using (Logger Log = Logger.IniciarLog("Acesso Senha - Continuar"))
            {
                try
                {
                    Redireciona();
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex.Message);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Redireciona o usuário para a página final
        /// </summary>
        private void Redireciona()
        {
            QueryStringSegura qs = new QueryStringSegura(Request.QueryString["dados"]);
            qs["AcessoSenha"] = bool.TrueString;

            String tipoRav = qs["TipoRavSelecionado"].ToString();
            String redirect = String.Compare(tipoRav, "avulso", true) == 0
                ? "pn_AlteracaoRavAvulso.aspx?dados={0}"
                : "pn_PersonalizacaoRavAutomatico.aspx?dados={0}";

            Response.Redirect(string.Format(redirect, qs.ToString()), false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static ServicoPortalRAVClient GetWebServiceInstance()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.SendTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.AllowCookies = false;
            binding.BypassProxyOnLocal = false;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.TextEncoding = System.Text.Encoding.UTF8;
            binding.TransferMode = TransferMode.Buffered;
            binding.UseDefaultWebProxy = true;
            return new ServicoPortalRAVClient(binding, new EndpointAddress("http://localhost:36651/HIServiceMA_RAV.svc"));
        }

        /// <summary>
        /// Validação da senha em server-side
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidarSenha(object source, CustomServerValidateEventArgs args)
        {
            using (Logger Log = Logger.IniciarLog("Validação da senha RAV - Continuar"))
            {
                if (((AcessoSenha)this.Parent).IgnoraSenha)
                {
                    Log.GravarMensagem("Ignorando senha informada na tela");
                    args.IsValid = true;
                    return;
                }

                try
                {
                    using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                    {
                        bool retornoSenha = cliente.ValidarSenha(txtSenhaEspecial.Text, SessaoAtual.CodigoEntidade);
                        if (retornoSenha)
                        {
                            args.IsValid = true;
                            return;
                        }
   
                        args.ErrorMessage = base.RetornarMensagemErro(FONTE, CODIGO_ERRO);
                        args.IsValid = false;
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    args.ErrorMessage = base.RetornarMensagemErro(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex.Message);

                    args.ErrorMessage = base.RetornarMensagemErro(FONTE, CODIGO_ERRO);
                }

                args.IsValid = false;
            }
        }
    }
}
