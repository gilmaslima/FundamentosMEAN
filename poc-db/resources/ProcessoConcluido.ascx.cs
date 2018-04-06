#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [05/06/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using Redecard.PN.Comum;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    public partial class ProcessoConcluido : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        public String Mensagem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Cancelar o processo de Confirmação Positiva
        /// </summary>
        protected void Cancelar(object sender, EventArgs e)
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            Response.Redirect(url, true);
        }
    }
}
