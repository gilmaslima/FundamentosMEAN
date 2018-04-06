using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.Portal.Helper.Web;
using System.Web.UI;

namespace Redecard.Portal.Aberto.WebParts.ControlTemplates
{
    /// <summary>
    /// Classe base para UserControls que faz uso de paginação
    /// </summary>
    public abstract class UserControlPaginador : UserControl
    {
        /// <summary>
        /// Parâmetro obtido da querystring de página para auxílio na montagem de paginador
        /// </summary>
        public virtual int? Pagina
        {
            get
            {
                int pagina;
                if (!int.TryParse(Request.QueryString[ChavesQueryString.Pagina], out pagina))
                    return 1;

                return pagina;
            }
        }

        public abstract void MontarPaginador(int totalItens, int itensPorPagina, string ancora);
    }
}