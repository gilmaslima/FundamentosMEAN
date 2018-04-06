using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.Portal.Helper.Paginacao;
using Redecard.Portal.Helper.Web;

namespace Redecard.Portal.Aberto.WebParts.ControlTemplates.Redecard.Portal.Aberto.WebParts
{
    public partial class PaginadorHibrido : UserControlPaginador
    {
        public override void MontarPaginador(int totalItens, int itensPorPagina, string ancora)
        {
            MontadorPaginadorBase montador = new MontadorPaginadorPadraoTV1(ChavesQueryString.Pagina); //Pode receber uma dependência aqui via factory class

            this.ltlPaginador.Text = montador.MontarPaginadorHTML(totalItens, itensPorPagina, ParametrosGeraisPaginacao.QuantidadeLimiteItensPaginadores, this.Pagina, ancora);
            montador = null;
        }
    }
}