using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.SharePoint.Modelo
{
    [Serializable]
    public class PaginaVisitada
    {
        public string Nome { get; set; }
        public bool ConsultaEfetuada { get; set; }
        public BuscarDados BuscarDados { get; set; }
        public DateTime UltimoAcesso { get; set; }
    }
}
