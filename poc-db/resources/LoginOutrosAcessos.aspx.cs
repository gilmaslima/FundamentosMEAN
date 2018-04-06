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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{

    /// <summary>
    /// Página que exibe o quadro de aviso conforma parâmetros enviados para a página
    /// </summary>
    public partial class LoginOutrosAcessos : ApplicationPageBaseAnonima
    {

        /// <summary>
        /// Exibe quadro de logins para outros tipos de Acessos
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

    }
}