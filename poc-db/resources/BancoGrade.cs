using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrasEntidades.Servicos
{
    /// <summary>
    /// Classe que retorna os Bancos que consultaram a Grade (SPB)
    /// Book: PV764CB
    /// REG-COMM-INTR-OCC
    /// </summary>
    public class BancoGrade : ModeloBase
    {
        /// <summary>
        /// CODIGO DO USUARIO(BANCO) QUE EFETUOU A PESQUISA
        /// </summary>
        public string Ususario { get; set; }

        /// <summary>
        /// DATA DA ULTIMA PESQUISA DO USUARIO.
        /// </summary>
        public DateTime DataPesuisa { get; set; }
        /// <summary>
        /// HORA DA ULTIMA PESQUISA DO USUARIO.
        /// </summary>
        public TimeSpan HoraPesuisa { get; set; }


    }
}
