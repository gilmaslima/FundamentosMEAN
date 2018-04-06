using System.Web.UI.WebControls;
using System.Web.UI;
using System;
using Redecard.Portal.Helper;

namespace Redecard.Portal.Aberto.WebControls {

    /// <summary>
    /// 
    /// </summary>
    public class LoginMessage : WebControl {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer) {
            string errorCode = this.Page.Request["errorCode"] as string;
            string usuario = this.Page.Request["txtUsuario"] as string;
            string cadastro = this.Page.Request["nCadastro"] as string;
            string sSenhaVezes = this.Page.Request["qtdVezes"] as string;
            if (!String.IsNullOrEmpty(errorCode)) {
                int iError = Int32.Parse(errorCode);
                string sErro = string.Empty;

                /*switch (iError) {
                    case 395:
                        errorCode = "2226";
                        break;
                    default:
                        errorCode = "8310";
                        break;
                }*/

                if (iError == 391 || iError == 396) {
                    sErro = RedecardHelper.GetErrorMessage(iError) + "<br />Você ainda possui <strong>" + sSenhaVezes + "</strong> tentativas.<br />Código de Ocorrência: " + errorCode;
                }
                else if (iError != 497) {
                    sErro = RedecardHelper.GetErrorMessage(iError) + "<br />Código de Retorno: " + errorCode;
                }
                writer.Write(String.Format("var sMsg = '{0}'; var sUser = '{1}'; var scadastro = '{2}';", sErro, usuario, cadastro));
            }
        }
    }
}