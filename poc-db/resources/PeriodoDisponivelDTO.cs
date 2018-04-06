using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redecard.PN.Extrato.Modelo
{
    public class PeriodoDisponivelDTO
    {
        public DateTime DataPeriodoInicial { get; set; }
        public DateTime DataPeriodoFinal { get; set; }
        public String CodigoSolicitacao { get; set; }

    }
}