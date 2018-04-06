using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Linq;
using System.Web.UI.WebControls;
using CacheHelper = Redecard.PN.Comum.CacheAdmin;
using CacheEnum = Redecard.PN.Comum.Cache;
using System.Collections.Generic;
using System.Web.UI;

namespace Redecard.PN.Comum.SharePoint.LAYOUTS.Redecard.Comum
{
    public partial class CacheAdmin : ApplicationPageBaseAutenticadaWindows
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ChecarUsuarioAdministrador())
                {
                    CarregarCaches();
                    CarregarObjetosGeral();
                }
            }
        }

        private void CarregarCaches()
        {
            var caches = CacheHelper.ObterCaches()
                .Where(cache => cache.Key != CacheEnum.Geral)
                .Select(cache => new
                {
                    Chave = cache.Key.ToString(),
                    Descricao = cache.Value
                }).ToList();     
                   
            rptCaches.DataSource = caches;
            rptCaches.DataBind();
        }

        private void CarregarObjetosGeral()
        {
            List<String> objetosGeral = CacheHelper.ObterObjetos(CacheEnum.Geral);

            if (objetosGeral != null && objetosGeral.Count > 0)
            {
                lblGeralObjetos.Visible = false;
                rptGeralObjetos.Visible = true;

                rptGeralObjetos.DataSource = objetosGeral;
                rptGeralObjetos.DataBind();
            }
            else
            {
                lblGeralObjetos.Visible = true;
                rptGeralObjetos.Visible = false;                
            }
        }

        protected void rptGeralObjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                String chaveObjeto = e.Item.DataItem as String;
                Label lblChaveObjetoGeral = e.Item.FindControl("lblChaveObjetoGeral") as Label;
                Button btnRemoverObjetoGeral = e.Item.FindControl("btnRemoverObjetoGeral") as Button;

                lblChaveObjetoGeral.Text = chaveObjeto;
                btnRemoverObjetoGeral.OnClientClick = String.Format(btnRemoverObjetoGeral.OnClientClick, chaveObjeto);
                btnRemoverObjetoGeral.CommandArgument = chaveObjeto;
            }
        }

        protected void rptCaches_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = e.Item.DataItem;
                String cache = (String)DataBinder.Eval(dataItem, "Descricao");
                InputFormSection ifsCache = e.Item.FindControl("ifsCache") as InputFormSection;
                InputFormControl ifcCache = ifsCache.FindControl("ifcCache") as InputFormControl;

                Button btnLimpar = ifcCache.FindControl("btnLimpar") as Button;
                
                ifsCache.Title = String.Format(ifsCache.Title, cache);
                ifsCache.Description = String.Format(ifsCache.Description, cache);
                btnLimpar.OnClientClick = String.Format(btnLimpar.OnClientClick, cache);
                btnLimpar.CommandArgument = cache;
            }            
        }

        protected void rptGeralObjetos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                String chaveObjeto = Convert.ToString(e.CommandArgument);
                CacheHelper.Remover(CacheEnum.Geral, chaveObjeto);

                CarregarObjetosGeral();

                String mensagemAlerta = "Remoção do objeto " + chaveObjeto + " realizada com sucesso!";

                System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, typeof(string),
                    "alertaRemocaoObjetoGeral", String.Format("alert('{0}');", mensagemAlerta), true);
            }
            catch(Exception ex)
            {
                String mensagem = "Erro durante carregamento de objetos do Geral.";
                System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, typeof(string),
                    "erroCarregandoObjetosGeral", String.Format("alert('{0}');", mensagem), true);
                SharePointUlsLog.LogErro(ex);
            }
        }

        protected void rptCaches_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            String nomeCache = Convert.ToString(e.CommandArgument);

            try
            {                
                Boolean sucesso = CacheHelper.LimparCache(nomeCache.GetEnumByDescription<CacheEnum>());

                CarregarCaches();

                String mensagemAlerta = sucesso ?
                    "Limpeza do cache " + nomeCache + " realizada com sucesso!" :
                    "Não foi possível executar a limpeza do cache " + nomeCache + ".";

                System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, typeof(string),
                    "alertaLimpezaCache", String.Format("alert('{0}');", mensagemAlerta), true);
            }
            catch (Exception ex)
            {
                String mensagem = "Erro durante limpeza do cache " + nomeCache + ".";
                System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, typeof(string),
                    "erroLimpezaCache", String.Format("alert('{0}');", mensagem), true);
                SharePointUlsLog.LogErro(ex);
            }
        }

        private Boolean ChecarUsuarioAdministrador()
        {
#if DEBUG
            return true;
#else
            try
            {
                Boolean permissao = SPContext.Current.Web.UserIsWebAdmin;
                this.ExibeAvisoAcessoNegado(!permissao, 
                    base.RetornarPainelExcecao("Somente usuários administradores podem acessar a página de Gerenciamento de Cache."));
                return permissao;
            }
            catch
            {
                this.ExibeAvisoAcessoNegado(true, base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));                
                return false;
            }
#endif
        }

        private void ExibeAvisoAcessoNegado(Boolean exibir, Panel painelExcecao)
        {
            pnlAcessoNegado.Controls.Clear();
            if (painelExcecao != null && exibir)
                pnlAcessoNegado.Controls.Add(painelExcecao);           
            pnlAcessoNegado.Visible = exibir;
            ifsCacheGeral.Visible = !exibir;
            rptCaches.Visible = !exibir;
        }
    }
}
