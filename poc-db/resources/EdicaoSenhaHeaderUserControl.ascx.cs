using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DataCash.SharePoint.WebParts.EdicaoSenhaHeader
{
    public partial class EdicaoSenhaHeaderUserControl : UserControl
    {                  
        public void CarregarHeader(Boolean sucessoTrocaSenha)
        {
            Int32 passo = sucessoTrocaSenha ? 1 : 0;
            ucAssistente.AtivarPasso(passo);
        }
    }
}