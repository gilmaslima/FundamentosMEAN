using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;

namespace Redecard.PN.DataCash.SharePoint.WebParts.RedirecionaDataCash
{
    public partial class RedirecionaDataCashUserControl : UserControlBaseDataCash
    {
        private RedirecionaDataCash WebPart
        {
            get
            {
                return (RedirecionaDataCash)this.Parent;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ifrDataCash.Attributes.Add("src", this.MontarUrl());
                ifrDataCash.Attributes.Add("width", this.WebPart.Largura);
                ifrDataCash.Attributes.Add("height", this.WebPart.Altura);
            }
        }

        /// <summary>
        /// Montar a URL de envio para o Komerci
        /// </summary>
        protected String MontarUrl()
        {
            if (!object.ReferenceEquals(this.SessaoAtual, null))
            {
                //Gera guid para vínculo do objeto de sessão no cache
                String guidSessao = Guid.NewGuid().ToString("N");
#if !DEBUG
                //Verifica se o usuário está com status pendente de identificação positiva
                //Se tiver, devemos considerar que é um usuário ativo para o tokebn da sessão
                bool alterouStatus = false;
                Comum.Enumerador.Status codStatus = Comum.Enumerador.Status.UsuarioAtivo;
                if (SessaoAtual.CodigoStatus == Comum.Enumerador.Status.RespostaIdPosPendente)
                {
                    alterouStatus = true;
                    codStatus = SessaoAtual.CodigoStatus;
                    SessaoAtual.CodigoStatus = Comum.Enumerador.Status.UsuarioAtivo;
                }
                //Adiciona objeto de sessão em cache
                CacheAdmin.Adicionar(Comum.Cache.DataCashIntegracao, guidSessao, SessaoAtual);
                //Se alterou o status da sessaoAtual para gerar o Token, 
                //seu valor deverá ser retornado para o status anterior
                if(alterouStatus)
                    SessaoAtual.CodigoStatus = codStatus;
#endif
                //Monta querystring
                QueryStringSegura dados = new QueryStringSegura();
                dados.Add("url", Request.Url.GetLeftPart(UriPartial.Path));
                dados.Add("id", guidSessao);
                //dados.ExpireTime = DateTime.Now.AddMinutes(1);

                if (Request.QueryString["dados"] != null)
                {
                    QueryStringSegura dadosWebPart = new QueryStringSegura(Request.QueryString["dados"]);
                    foreach (String key in dadosWebPart.Keys)
                    {
                        dados.Add(key, dadosWebPart[key]);
                    }
                }

                return String.Format("{0}?dados={1}", this.WebPart.URLDataCash, dados.ToString());
            }
            else
                return this.WebPart.URLDataCash;
        }
    }
}