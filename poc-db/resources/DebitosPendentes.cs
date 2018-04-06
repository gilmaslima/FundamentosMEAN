using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Credenciamento.Sharepoint.Modelo
{
    public class DebitosPendentes
    {
        public Int32 NumeroEstabelecimento { get; set; }

        public String NomeEstabelecimento { get; set; }

        public DateTime Data { get; set; }

        public Decimal Saldo { get; set; }
    }
}
