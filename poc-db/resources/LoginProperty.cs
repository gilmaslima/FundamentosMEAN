using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using Microsoft.SharePoint;

namespace Redecard.Portal.Aberto.WebControls {

    /// <summary>
    /// 
    /// </summary>
    public class LoginInformation : WebControl {

        //<ul class="identificacao">
        //    <li><strong>Nº do estabelecimento:</strong> 027154364</li>
        //    <li><strong>Empresa:</strong> CANAIS ELETRONICOS M</li>
        //    <li><strong>Usuário:</strong> Agência TV1</li>
        //</ul>

        // Somente se houver algum aviso de mudança de login
        // -------------------------------------------------------------
        //<script language="javascript" type="text/javascript">
        //    $(document).ready(function () {
        //        $('nEstab').val('{0}');
        //        $('empresa').val('{1}');
        //        $('usuario').val('{2}');
        //        $('#pnlAviso, #bgProtecao').show();
        //    });
        //</script>

        /// <summary>
        /// 
        /// </summary>
        string sInf = "<ul class='identificacao'><li><strong>Nº do estabelecimento:</strong> {0}</li><li><strong>Empresa:</strong> {1}</li><li><strong>Usuário:</strong> {2}</li></ul>";
        string sAviso = "<script language=\"javascript\" type=\"text/javascript\">$(document).ready(function () {{ $('#nEstab').text('{0}'); $('#empresa').text('{1}'); $('#usuario').text('{2}'); $('#pnlAviso, #bgProtecao').show(); }});</script>";
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer) {
            HttpCookieCollection cookies = this.Page.Request.Cookies;
            string sEmpresa = string.Empty;
            string sNEstabelecimento = string.Empty;
            string sNLogin = string.Empty;

            List<string> dicKeys = new List<string>();
            dicKeys.AddRange(cookies.AllKeys);

            string sRender = string.Empty;
            string sShowAviso = (dicKeys.Contains("avisoAcessoFiliais") ? cookies["avisoAcessoFiliais"].Value : string.Empty);
            string sShowMatrizAviso = (dicKeys.Contains("avisoMatrizRollback") ? cookies["avisoMatrizRollback"].Value : string.Empty);
            string sAcessoFiliais = (dicKeys.Contains("acessoFiliais") ? cookies["acessoFiliais"].Value : string.Empty);

            if (!String.IsNullOrEmpty(sAcessoFiliais) && Convert.ToBoolean(sAcessoFiliais)) {
                sEmpresa = (dicKeys.Contains("NEmpresa_2") ? cookies["NEmpresa_2"].Value : string.Empty);
                sNEstabelecimento = (dicKeys.Contains("NEstabelecimento_2") ? cookies["NEstabelecimento_2"].Value : string.Empty);
                sNLogin = (dicKeys.Contains("NLoginName_2") ? cookies["NLoginName_2"].Value : string.Empty);
                sRender = String.Format(sInf, sNEstabelecimento, sEmpresa, sNLogin);

                if (!String.IsNullOrEmpty(sShowAviso) && Convert.ToBoolean(sShowAviso)) {
                    this.Page.Response.Cookies["avisoAcessoFiliais"].Value = false.ToString();
                    // Aplicar aviso,o mesmo já está renderizado na masterpage para facilitar a edição.
                    sRender += String.Format(sAviso, sNEstabelecimento, sEmpresa, sNLogin);
                }
            }
            else {
                sEmpresa = (dicKeys.Contains("NEmpresa") ? cookies["NEmpresa"].Value : string.Empty);
                sNEstabelecimento = (dicKeys.Contains("NEstabelecimento") ? cookies["NEstabelecimento"].Value : string.Empty);
                sNLogin = (dicKeys.Contains("NLoginName") ? cookies["NLoginName"].Value : string.Empty);

                sRender = String.Format(sInf, sNEstabelecimento, sEmpresa, sNLogin);

                if (!String.IsNullOrEmpty(sShowMatrizAviso) && Convert.ToBoolean(sShowMatrizAviso)) {
                    this.Page.Response.Cookies["avisoMatrizRollback"].Value = false.ToString();
                    // Aplicar aviso, o mesmo já está renderizado na masterpage para facilitar a edição.
                    sRender += String.Format(sAviso, sNEstabelecimento, sEmpresa, sNLogin);
                }
            }

            writer.Write(sRender);
        }
    }
}