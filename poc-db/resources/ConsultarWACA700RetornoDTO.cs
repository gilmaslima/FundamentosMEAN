using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarWACA700RetornoDTO
    {
        public string Detalhe { get; set; }
        public short NumeroMes { get; set; }
        public string Timestamp { get; set; }
        public string TipoResumoVenda { get; set; }
    }
}
