#region Histórico do Arquivo
/*
(c) Copyright [2012] BRQ IT Solutions.
Autor       : [- 2012/08/21 - Tiago Barbosa dos Santos]
Empresa     : [BRQ IT Solutions]
Histórico   : Criação da Classe
- [08/06/2012] – [Tiago Barbosa dos Santos] – [Criação]
 *
*/
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using System.Collections.Generic;
using Redecard.PN.Comum;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.ProtocolarCancelamento
{
    public partial class ProtocolarCancelamentoUserControl : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(base.SessaoAtual, null))
            {
                using (Logger Log = Logger.IniciarLog("Protocolar Cancelamento - Page Load"))
                {
                    List<ItemCancelamentoEntrada> ItemCancelamentoEntrada = new List<ItemCancelamentoEntrada>();

                    int teste = ItemCancelamentoEntrada.Count;

                    while (ItemCancelamentoEntrada.Count < 10)
                    {
                        //    ItemCancelamentoEntrada.Add();
                    }

                    rptDados.DataSource = ItemCancelamentoEntrada;

                    rptDados.DataBind();
                }
            }
        }
    }
}
