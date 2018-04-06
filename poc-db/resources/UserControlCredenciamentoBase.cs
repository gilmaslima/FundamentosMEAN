using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Redecard.PN.Comum;

namespace Rede.PN.Credenciamento.Sharepoint
{
    public class UserControlCredenciamentoBase : UserControlBase
    {

        public const string FONTE = "Rede.PN.Credenciamento.SharePoint";

        #region [ Propriedades ]

        /// <summary>
        /// Dados do Credenciamento que persistem na Sessão
        /// </summary>
        public Modelo.Credenciamento Credenciamento
        {
            get
            {
                if (Session["RedecardPNCredenciamento"] == null)
                    Session["RedecardPNCredenciamento"] = new Modelo.Credenciamento();

                return (Modelo.Credenciamento)Session["RedecardPNCredenciamento"];
            }
            set
            {
                Session["RedecardPNCredenciamento"] = value;
            }
        }

        #endregion
    }
}
