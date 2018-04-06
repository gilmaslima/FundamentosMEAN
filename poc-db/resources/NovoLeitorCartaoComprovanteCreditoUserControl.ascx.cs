/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web.UI;
using Redecard.PN.Boston.Sharepoint.ControlTemplates.Redecard.PN.Boston.Sharepoint;
using Redecard.PN.Comum;

namespace Redecard.PN.Boston.Sharepoint.WebParts.NovoLeitorCartaoComprovanteCredito
{
    public partial class NovoLeitorCartaoComprovanteCreditoUserControl : UserControl
    {
        #region [ Propriedades / Variáveis ]

        /// <summary>
        /// ucRedirecionaDataCash
        /// </summary>
        private RedirecionaDataCash UcRedirecionaDataCash { get { return (RedirecionaDataCash)ucRedirecionaDataCash; } }

        #endregion

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Sessao.Contem())
            {
                if (!IsPostBack)
                {
                    //Atualiza URL do iframe, para carregamento do Comprovante
                    //Obs: Os dados da QueryString são automaticamente repassados para a queryString do iframe
                    UcRedirecionaDataCash.AtualizarRedirecionamento(null);
                }
            }
        }
    }
}
