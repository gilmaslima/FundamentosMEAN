using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarExtratoDirfEnvioDTO
    {
        public int NumeroEstabelecimento { get; set; }
        public short AnoBaseDirf { get; set; }
        public string CnpjEstabelecimento { get; set; }
    }
}
