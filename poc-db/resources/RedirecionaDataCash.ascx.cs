/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web.UI;
using Redecard.PN.Boston.Sharepoint.WebParts;
using Redecard.PN.Comum;

namespace Redecard.PN.Boston.Sharepoint.ControlTemplates.Redecard.PN.Boston.Sharepoint
{
    /// <summary>
    /// UserControl para redirecionamento para integração com as páginas do DataCash
    /// </summary>
    public partial class RedirecionaDataCash : UserControlBase
    {
        /// <summary>
        /// WebPart
        /// </summary>
        private RedirecionaDataCashBaseWebpart WebPart
        {
            get 
            {
                Control parent = this.Parent;
                while (parent != null && !(parent is RedirecionaDataCashBaseWebpart))
                    parent = parent.Parent;
                return (RedirecionaDataCashBaseWebpart)parent;
            }
        }

        /// <summary>
        /// URL DataCash
        /// </summary>
        public String UrlDataCash { get { return WebPart.URLDataCash; } }

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Redirecionamento DataCash"))
            {
                if (!IsPostBack && ifrDataCash.Attributes["src"].EmptyToNull() == null)
                {
                    String url = MontarUrl(null);;
                    log.GravarMensagem("Redirecionamento iframe", url);
                    ifrDataCash.Attributes["src"] = url;
                }
            }
        }

        /// <summary>
        /// Atualiza o redirecionamento repassando os dados adicionais por QueryString.
        /// Os dados da QueryString atual também é incluída na QueryString do iframe.
        /// </summary>
        /// <param name="qsAdicional">QueryString</param>
        public void AtualizarRedirecionamento(QueryStringSegura qsAdicional)
        {
            using (Logger log = Logger.IniciarLog("Atualizando redirecionamento iframe"))
            {
                String url = MontarUrl(qsAdicional);
                log.GravarMensagem("Atualizando redirecionamento iframe", url);
                ifrDataCash.Attributes["src"] = url;
            }
        }

        /// <summary>
        /// Montar a URL de envio para o Komerci
        /// </summary>
        /// <param name="qsAdicional">QueryString adicional</param>
        public String MontarUrl(QueryStringSegura qsAdicional)
        {
            if (!object.ReferenceEquals(this.SessaoAtual, null))
            {
                //Gera guid para vínculo do objeto de sessão no cache
                String guidSessao = Guid.NewGuid().ToString("N");

                //Adiciona objeto de sessão em cache
                CacheAdmin.Adicionar(Comum.Cache.DataCashIntegracao, guidSessao, this.SessaoAtual);

                //Monta querystring
                QueryStringSegura dados = new QueryStringSegura();
                dados.Add("url", Request.Url.GetLeftPart(UriPartial.Path));
                dados.Add("id", guidSessao);

                //Mantém os dados da queryString da página
                if (Request.QueryString["dados"] != null)
                {
                    try
                    {
                        var qsWebPart = new QueryStringSegura(Request.QueryString["dados"]);
                        foreach (String key in qsWebPart.Keys)
                            dados.Add(key, qsWebPart[key]);
                    }
                    catch (QueryStringExpiradaException ex)
                    {
                        Logger.GravarErro("QueryString expirada", ex);
                    }
                    catch (QueryStringInvalidaException ex)
                    {
                        Logger.GravarErro("QueryString inválida", ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.GravarErro("QueryString inválida", ex);
                    }
                }

                //Inclui os dados da queryString adicional
                if (qsAdicional != null)
                {
                    foreach (String key in qsAdicional.Keys)
                        dados.Add(key, qsAdicional[key]);
                }

                return String.Format("{0}?dados={1}", this.UrlDataCash, dados.ToString());
            }
            else
                return this.UrlDataCash;
        }
    }
}