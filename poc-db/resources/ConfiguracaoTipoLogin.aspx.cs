using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Login;
using System.Collections.Generic;
using Redecard.PN.DadosCadastrais.SharePoint.Login.Modelo;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{
    public partial class ConfiguracaoTipoLogin : ApplicationPageBaseAutenticadaWindows
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    this.ChecarUsuarioAdministrador();
                    this.CarregarDadosConfiguracao();
                }
            }
            catch (NullReferenceException ex)
            {
                CriarMensagem(ex.Message, true);
            }
            catch (Exception ex)
            {
                CriarMensagem(ex.Message, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                var configs = ConfiguracaoLogin.ConsultarConfiguracaoLogin(TipoOrigem.Lista);

                if (configs != null && configs.Count > 0)
                    ConfiguracaoLogin.AdicionaConfiguracaoCache(configs);

                this.CarregarDadosConfiguracao();
            }
            catch (NullReferenceException ex)
            {
                CriarMensagem(ex.Message, true);
            }
            catch (Exception ex)
            {
                CriarMensagem(ex.Message, true);
            }
        }

        /// <summary>
        /// Atualizar o cache de PVs Piloto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAtualizarPvsPiloto_Click(object sender, EventArgs e)
        {
            try
            {
                CacheAdmin.Remover(Comum.Cache.RedeTokenLogin, "__listaPVsPiloto");
            }
            catch (NullReferenceException ex)
            {
                CriarMensagem(ex.Message, true);
            }
            catch (Exception ex)
            {
                CriarMensagem(ex.Message, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ChecarUsuarioAdministrador()
        {
            if (!SPContext.Current.Web.UserIsWebAdmin)
                throw new UnauthorizedAccessException("Somente usuários administradores podem configurar o tipo de login do Portal.");
        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarDadosConfiguracao()
        {
            //Consulta as configurações do tipo de login na lista Sharepoint
            var configsLista = ConfiguracaoLogin.ConsultarConfiguracaoLogin(TipoOrigem.Lista);

            if (configsLista != null && configsLista.Count > 0)
            {
                repConfiguracao.DataSource = configsLista;
                repConfiguracao.DataBind();
            }

            //Consulta as configurações do tipo de login disponiveis em Cache
            var configsCache = ConfiguracaoLogin.ConsultarConfiguracaoLogin(TipoOrigem.Cache);

            if (configsCache != null && configsCache.Count > 0)
            {
                repDadosCache.DataSource = configsCache;
                repDadosCache.DataBind();

                if (!CompararListas(configsLista, configsCache))
                {
                    lblMensagemCache.Text = "Os dados em Cache não estão sincronizados com as configurações na lista.";
                }
                else
                {
                    lblMensagemCache.Text = "Os dados de configuração em cache estão sincronizados.";
                    lblMensagemCache.ForeColor = System.Drawing.Color.Green;
                }
            }
            else
            {
                lblMensagemCache.Text = "Não há dados em cache.";
                lblMensagemCache.ForeColor = System.Drawing.Color.Red;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listaPrimaria"></param>
        /// <param name="listaSecundaria"></param>
        private bool CompararListas(List<ConfiguracaoLoginRetorno> listaPrimaria, List<ConfiguracaoLoginRetorno> listaSecundaria)
        {
            if (listaPrimaria == null || listaSecundaria == null)
                return false;

            if (listaPrimaria.Count != listaSecundaria.Count)
                return false;

            foreach (var item in listaPrimaria)
            {
                if (!listaSecundaria.Any(sec => sec.Servidor == item.Servidor &&
                    sec.UtilizaApiLogin == item.UtilizaApiLogin))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Emite um alerta para o usuário
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="erro"></param>
        private void CriarMensagem(string mensagem, bool erro)
        {
            string script = String.Empty;

            if (erro)
                script = String.Format("window.setTimeout('emiteAlertaErro(\"{0}\");', 500);", mensagem);
            else
                script = String.Format("window.setTimeout('emiteAlerta(\"{0}\");', 500);", mensagem);

            //Registra o script na página
            System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, typeof(string), "alerta", script, true);
        }
    }
}
