using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1116 - Consultar por transação - Carta.
    /// </summary>
    public class ConsultarCartasEnvioDTO
    {
        public decimal NumeroProcesso { get; set; }
        public string TimestampTransacao { get; set; }
        public short SistemaDados { get; set; }
    }
}
