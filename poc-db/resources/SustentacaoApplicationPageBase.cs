using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    public class SustentacaoApplicationPageBase : ApplicationPageBaseAutenticadaWindows
    {
        private string _nomeGrupoSustentacao = "Ferramentas Portal PN";

        private bool _exibeDadosExecucao;
        public bool ExibeDadosExecucao
        {
            get { return _exibeDadosExecucao; }
            set { _exibeDadosExecucao = value; }
        }

        public String UsuarioLogado
        {
            get 
            {
                return ViewState["UsuarioLogado"] == null ? null : (String)ViewState["UsuarioLogado"]; 
            }
            set 
            {
                ViewState["UsuarioLogado"] = value; 
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.IsPostBack)
            {
                string mensagemLog = "Sustentação - Acesso à página de administração do PN";
                using (Logger Log = Logger.IniciarLog(mensagemLog))
                {
                    SharePointUlsLog.LogMensagem(mensagemLog);

                    try
                    {
                        using (SPSite site = new SPSite(SPContext.Current.Site.Url))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                if (web.CurrentUser == null)
                                {
                                    return;
                                }

                                UsuarioLogado = web.CurrentUser.Name;

                                mensagemLog = string.Format("Sustentação - Validar se usuário '{0}' logado possui permissão", web.CurrentUser.Name);
                                SharePointUlsLog.LogMensagem(mensagemLog);
                                Log.GravarMensagem(mensagemLog);

                                SPGroup grupo = web.Groups.GetByName(_nomeGrupoSustentacao);
                                if (grupo == null)
                                {
                                    mensagemLog = string.Format("Sustentação - Não foi possível obter as permissões do usuário '{0}'", web.CurrentUser.Name);
                                    SharePointUlsLog.LogMensagem(mensagemLog);
                                    Log.GravarMensagem(mensagemLog);
                                    throw new Exception(mensagemLog);
                                }
                                if (!web.IsCurrentUserMemberOfGroup(grupo.ID))
                                {
                                    mensagemLog = string.Format("Sustentação - O usuário '{0}' NÃO possui permissão", web.CurrentUser.Name);
                                    SharePointUlsLog.LogMensagem(mensagemLog);
                                    Log.GravarMensagem(mensagemLog);
                                    throw new SecurityException(mensagemLog);
                                }
                                this.ExibeDadosExecucao = ValidarChaveAcesso();
                            }
                        }
                    }
                    catch (SecurityException ex)
                    {
                        Log.GravarErro(ex);
                        SPUtility.HandleAccessDenied(ex);
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        SPUtility.TransferToErrorPage(ex.Message);
                    }
                }

                CriarMenus();
            }
            base.OnLoad(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ValidarChaveAcesso()
        {
            bool possuiChaveAcesso = false;
            try
            {
                if (Request.QueryString["n"] != null)
                {
                    possuiChaveAcesso = Request.QueryString["n"].ToString() ==
                        _nomeGrupoSustentacao + "-" + DateTime.Today.Year.ToString("0000") + DateTime.Today.Month.ToString("00");
                }

            }
            catch (Exception ex)
            {
                SPUtility.TransferToErrorPage(ex.Message);
            }
            return possuiChaveAcesso;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CriarMenus()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite oSiteCollection = new SPSite(Request.Url.AbsoluteUri))
                {
                    using (SPWeb oWeb = oSiteCollection.OpenWeb())
                    {
                        bool correntAllowUnsafeUpdates = oWeb.AllowUnsafeUpdates;
                        oWeb.AllowUnsafeUpdates = true;

                        // Create the node.
                        SPNavigationNodeCollection _quickLaunchNav = oWeb.Navigation.QuickLaunch;
                        // Delete all existing items in Quick Launch
                        int _counter = 0;
                        while (_quickLaunchNav.Count != _counter && _quickLaunchNav.Count > 0)
                        {
                            _counter++;
                            SPNavigationNode _item = _quickLaunchNav[0];
                            _item.Delete();
                        }

                        SPNavigationNode _SPNode = new SPNavigationNode("PV e Usuários", "/_layouts/Sustentacao/pv.aspx");
                        _quickLaunchNav.AddAsLast(_SPNode);
                        _SPNode = new SPNavigationNode("Consulta de Log", "/_layouts/Sustentacao/LogConsulta.aspx");
                        _quickLaunchNav.AddAsLast(_SPNode);
                        _SPNode = new SPNavigationNode("Consulta de Serviços", "/_layouts/Sustentacao/ServicosConsulta.aspx");
                        _quickLaunchNav.AddAsLast(_SPNode);
                        _SPNode = new SPNavigationNode("Consulta de Usuários", "/_layouts/Sustentacao/UsuarioPorEmail.aspx");
                        _quickLaunchNav.AddAsLast(_SPNode);
                        _SPNode = new SPNavigationNode("Consulta de Cancelamentos", "/_layouts/Sustentacao/ConsultaCancelamento.aspx");
                        _quickLaunchNav.AddAsLast(_SPNode);
                        //_SPNode = new SPNavigationNode("Consulta de ULS", "/_layouts/Sustentacao/consultauls.aspx");
                        //_quickLaunchNav.AddAsLast(_SPNode);

                        oWeb.AllowUnsafeUpdates = correntAllowUnsafeUpdates;
                        oWeb.Update();
                    }
                }
            });
        }

        public void Alerta(String msg)
        {
            Alerta(msg, false);
        }

        public void Alerta(String msg, Boolean fechaModal)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("alert('");
            sb.Append(msg);
            sb.Append("'); ");
            if (Request.Url.PathAndQuery.ToLower().Contains("isdlg=1"))
            {
                if (fechaModal)
                {
                    sb.Append("SP.UI.ModalDialog.commonModalDialogClose(1, '')");                
                }
            }
            else
            {
                sb.Append("window.parent.location.href='");
                sb.Append(Request.Url.PathAndQuery);
                sb.Append("'");
            }
            sb.Append(";");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AlertRedirect", sb.ToString(), true);
        }


    }
}
