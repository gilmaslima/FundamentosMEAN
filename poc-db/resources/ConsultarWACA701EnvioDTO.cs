using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarWACA701EnvioDTO
    {
        public int NumeroResumoVenda { get; set; }
        public int NumeroEstabelecimento { get; set; }
        public DateTime DataApresentacao { get; set; }
        public string TipoResumoVenda { get; set; }
        public string Timestamp { get; set; }
    }
}
