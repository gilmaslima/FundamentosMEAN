using System;
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace Rede.PN.AtendimentoDigital.SharePoint.WebParts.AtendimentoDigital.Atendimento
{
    [ToolboxItemAttribute(false)]
    public partial class Atendimento : WebPart
    {
        public Atendimento()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
