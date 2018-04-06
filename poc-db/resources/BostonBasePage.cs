using System;
using Redecard.PN.Comum;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Redecard.PN.Boston.Sharepoint.Base
{
    public class BostonBasePage : ApplicationPageBaseAnonima
    {
        #region [ Constantes ]

        public const String MENSAGEM = "Houve um erro ao carregar essa página. Por favor, tente novamente mais tarde.";

        public const String COD_EQUIPAMENTO = "MPO";

        public const Int32 CANAL = 15;

        #endregion

        /// <summary>
        /// Dados do Credenciamento que ficam em memória
        /// </summary>
        public DadosCredenciamento DadosCredenciamento
        {
            get
            {
                if (Session["RedecardPNCredenciamento"] == null)
                    Session["RedecardPNCredenciamento"] = new DadosCredenciamento();

                return (DadosCredenciamento)Session["RedecardPNCredenciamento"];
            }
            set
            {
                Session["RedecardPNCredenciamento"] = value;
            }
        }
    }
}
