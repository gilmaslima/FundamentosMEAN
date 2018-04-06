using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.CadastroEndereco
{
    public partial class CadastroEnderecoUserControl : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Cadastro de Endereço - Carregando página"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        if (Request.QueryString["dados"] != null)
                        {
                            QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                            pnlIdentificacao.Visible = queryString["TipoDomicilio"].ToString().Equals("E") ||
                                                       queryString["TipoDomicilio"].ToString().Equals("C");
                        }
                        else
                        {
                            base.ExibirPainelExcecao("Erro ao recuperar as informações passadas via QueryString", CODIGO_ERRO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
    }
}
