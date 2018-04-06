using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarRejeitadosEnvioDTO
    {
        public string Timestamp { get; set; }
        public short TipoResumoVenda { get; set; }
        public int NumeroEstabelecimento { get; set; }
        public int NumeroResumoVenda { get; set; }
        public DateTime DataApresentacao { get; set; }
    }
}
