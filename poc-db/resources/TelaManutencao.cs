using System;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.SharePoint;
using Redecard.PN.Comum;

namespace Redecard.PN.TelaManutencao
{
    public class TelaManutencao : IHttpModule
    {
        private string Titulo { get; set; }
        private string Mensagem { get; set; }

        public void Init(HttpApplication context)
        {
            //Faço o delegate para o método BeginRequest
            context.PostRequestHandlerExecute += context_BeginRequest;
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                TelaDeManutencao(sender);
            }
            catch (Exception ex)
            {
               //Não fazer nada caso haja erro, para não impactar o portal
            }
        }

        private void TelaDeManutencao(object sender)
        {
            HttpApplication context = sender as HttpApplication;

            //Verifico se é uma página passível de verificação
            if (!context.Request.Url.AbsolutePath.Contains(".aspx") &&
                !context.Request.Url.AbsolutePath.Contains(".html") &&
                !context.Request.Url.AbsolutePath.Contains(".htm")) return;

            //Verifica se a lista existe na sessão e se o tempo de atualização já foi alcançado (baseado no timeout da sessão)
            if (context.Session["ListaTelaManutencao"] == null ||
                DateTime.Now.Subtract(((Tuple<SPListItemCollection, DateTime>)context.Session["ListaTelaManutencao"]).Item2).TotalMinutes > context.Session.Timeout)
            {
                //Se não, jogo a lista de URLs para a sessão
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite oSite = new SPSite(ConfigurationManager.AppSettings["SiteTelaManutencao"]))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
                            context.Session["ListaTelaManutencao"] =
                                new Tuple<SPListItemCollection, DateTime>(oWeb.Lists["Telas de manutenção"].Items, DateTime.Now);
                        }
                    }
                });
            }

            //Verifico se a URL consta na lista de aplicação da tela de manutenção (URL sem host)
            if (!VerificarSeAbreTela(context.Request.Url.PathAndQuery, context)) return;

            //Se sim, faço o redirect para a tela de aviso com as informações da lista
            QueryStringSegura query = new QueryStringSegura
            {
                {
                    "mensagem",
                    context.Server.UrlEncode(Mensagem)
                },
                {
                    "titulo", 
                    context.Server.UrlEncode(Titulo)
                }
            };

            context.Response.Redirect(
                context.Request.Url.GetLeftPart(UriPartial.Authority) +
                ConfigurationManager.AppSettings["RedirectTelaManutencao"] + "?dados=" + query, true);
        }

        public void Dispose()
        {

        }

        public bool VerificarSeAbreTela(string pURLTratada, HttpApplication context)
        {
            bool retorno = false;

            //Verifico se a URL consta na lista de aplicação da tela de manutenção
            foreach (SPListItem item in ((Tuple<SPListItemCollection, DateTime>)context.Session["ListaTelaManutencao"]).Item1.Cast<SPListItem>().Where(item => pURLTratada.Contains(item["URL"].ToString())))
            {
                retorno = true;
                Titulo = item.Title;
                Mensagem = item["Mensagem"].ToString();
            }

            return retorno;
        }
    }
}